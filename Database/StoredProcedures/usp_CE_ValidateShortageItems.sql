USE [Ashley]
GO

/****** Object:  StoredProcedure [dbo].[usp_CE_ValidateShortageItems]    Script Date: 2026-05-20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*******************************************************************************
* Object Name: usp_CE_ValidateShortageItems
* Database: Ashley
* Environments: AFI_Dynamic, WVF_Dynamic
* Function: Validates shortage items for credit entry
*           Performs comprehensive validation checks including:
*           - Item validity
*           - Customer/Serial/Item combination validation
*           - Duplicate credit check
*           - Quantity validation against order quantity
*           - Defect code validation (default: XP)
*           - Location code validation (default: WU from web.config)
* Projects: Credit Shortage Validation Microservice
* Sample Execution:
*		DECLARE @ShortageItems AS dbo.typCEShortageItemValidation;
*		INSERT INTO @ShortageItems
*		SELECT '8888300', '0001', 110433, 'C310325', '7320325', 1, 125.50, 'XP', 'WU', 1234567, 100;
*		EXECUTE usp_CE_ValidateShortageItems @ShortageItems, 'AFI';
*
* Created: 2026-05-20
* Author: Credit Shortage Validation Team
* ------------------------------------------------------------------------------
* Validation Rules:
* 1. Item must exist and be valid
* 2. Customer/Serial/Item combination must be valid
* 3. Shortage must not already be credited
* 4. Shortage quantity must not exceed original order quantity
* 5. Defect code must be valid (default: XP)
* 6. Location code must be valid (default: WU)
*******************************************************************************/

-- First, check if the table type exists and drop it if the schema needs to change
IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'typCEShortageItemValidation')
BEGIN
	-- Drop the existing type (requires no active dependencies)
	DROP TYPE [dbo].[typCEShortageItemValidation];
END
GO

-- Create the User Defined Table Type for input parameters
-- Updated: OrderNumber changed from NUMERIC(7,0) to VARCHAR(10) to support alphanumeric formats like 'D623146'
-- Updated: InvoiceNumber changed from NUMERIC(6,0) to NUMERIC(8,0) to support larger invoice numbers
-- Updated: Added Amount DECIMAL(10,2) to support credit amount
CREATE TYPE [dbo].[typCEShortageItemValidation] AS TABLE(
	[CustomerNumber] VARCHAR(8) NOT NULL,
	[ShipToNumber] VARCHAR(4) NOT NULL,
	[InvoiceNumber] NUMERIC(8,0) NOT NULL,  -- Changed from NUMERIC(6,0) to NUMERIC(8,0)
	[ItemNumber] VARCHAR(15) NOT NULL,
	[SerialNumber] VARCHAR(10) NOT NULL,
	[ShortageQuantity] NUMERIC(7,0) NOT NULL,
	[Amount] DECIMAL(10,2) NULL,  -- Credit amount for the shortage
	[DefectCode] VARCHAR(4) NULL,
	[LocationCode] VARCHAR(2) NULL,
	[OrderNumber] VARCHAR(10) NULL,  -- Changed from NUMERIC(7,0) to VARCHAR(10)
	[OrderItemSeq] INT NULL
);
GO

-- Create the stored procedure
CREATE OR ALTER PROCEDURE [dbo].[usp_CE_ValidateShortageItems]
(
	@ShortageItems		dbo.typCEShortageItemValidation READONLY,
	@Environment		VARCHAR(3) = NULL,
	@DefaultDefectCode	VARCHAR(4) = 'XP',
	@DefaultLocationCode VARCHAR(2) = 'WU'
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Temporary table to hold validation results
	CREATE TABLE #ValidationResults (
		CustomerNumber VARCHAR(8),
		ShipToNumber VARCHAR(4),
		InvoiceNumber NUMERIC(8,0),  -- Changed from NUMERIC(6,0)
		ItemNumber VARCHAR(15),
		SerialNumber VARCHAR(10),
		ShortageQuantity NUMERIC(7,0),
		Amount DECIMAL(10,2),  -- Credit amount for the shortage
		DefectCode VARCHAR(4),
		LocationCode VARCHAR(2),
		OrderNumber VARCHAR(10),  -- Changed from NUMERIC(7,0)
		OrderItemSeq INT,
		IsValid BIT DEFAULT 1,
		ValidationErrors VARCHAR(MAX) DEFAULT '',
		OrderedQuantity NUMERIC(7,0) DEFAULT 0,
		AlreadyCreditedQuantity DECIMAL(10,2) DEFAULT 0,
		RemainingCreditableQuantity DECIMAL(10,2) DEFAULT 0,
		ItemExists BIT DEFAULT 0,
		CustomerSerialItemValid BIT DEFAULT 0,
		DefectCodeValid BIT DEFAULT 0,
		LocationCodeValid BIT DEFAULT 0,
		IsDFICustomer BIT DEFAULT 0,
		WarehouseCode VARCHAR(10) DEFAULT NULL
	);

	-- Initialize results with input data and defaults
	INSERT INTO #ValidationResults (
		CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber,
		ShortageQuantity, Amount, DefectCode, LocationCode, OrderNumber, OrderItemSeq
	)
	SELECT
		CustomerNumber,
		ShipToNumber,
		InvoiceNumber,
		ItemNumber,
		SerialNumber,
		ShortageQuantity,
		Amount,  -- Credit amount
		ISNULL(DefectCode, @DefaultDefectCode) AS DefectCode,
		ISNULL(LocationCode, @DefaultLocationCode) AS LocationCode,
		OrderNumber,
		OrderItemSeq
	FROM @ShortageItems;

	-- Set default environment if not provided
	IF @Environment IS NULL
	BEGIN
		SELECT @Environment = ISNULL((SELECT TOP 1 encEnvironmentCode
										FROM Environment.dbo.tblEnvironmentControl WITH (NOLOCK)), 'AFI');
	END;

	--=============================================================================
	-- VALIDATION 1: Verify Item Exists and is Valid
	--=============================================================================
	UPDATE VR
	SET ItemExists = CASE WHEN IM.imaItnbr IS NOT NULL THEN 1 ELSE 0 END,
		ValidationErrors = CASE
			WHEN IM.imaItnbr IS NULL
			THEN ValidationErrors + 'ERROR: Item [' + VR.ItemNumber + '] does not exist or is invalid. '
			ELSE ValidationErrors
		END,
		IsValid = CASE WHEN IM.imaItnbr IS NULL THEN 0 ELSE IsValid END
	FROM #ValidationResults VR
	LEFT JOIN Ashley.dbo.tblItemMaster IM WITH (NOLOCK)
		ON IM.imaItnbr = VR.ItemNumber;

	--=============================================================================
	-- VALIDATION 2: Verify Customer/Serial/Item Combination
	--=============================================================================
	-- Special handling for serial number 999999 (no serial number)
	-- For 999999: Only check if customer purchased the item (ignore serial match)
	-- For specific serial: Check exact customer/invoice/serial/item match
	UPDATE VR
	SET CustomerSerialItemValid = CASE
			-- For serial 999999: Check if customer purchased this item
			WHEN VR.SerialNumber = '999999' THEN
				CASE WHEN COALESCE(IND_NOSER.indCusno, INDA_NOSER.indCusno) IS NOT NULL THEN 1 ELSE 0 END
			-- For specific serial: Check exact match
			ELSE
				CASE WHEN COALESCE(IND.indCusno, INDA.indCusno) IS NOT NULL THEN 1 ELSE 0 END
		END,
		ValidationErrors = CASE
			WHEN VR.SerialNumber = '999999' AND COALESCE(IND_NOSER.indCusno, INDA_NOSER.indCusno) IS NULL
			THEN ValidationErrors + 'ERROR: Customer [' + VR.CustomerNumber + '] did not purchase item [' + VR.ItemNumber + ']. '
			WHEN VR.SerialNumber <> '999999' AND COALESCE(IND.indCusno, INDA.indCusno) IS NULL
			THEN ValidationErrors + 'ERROR: Customer/Serial/Item combination is invalid. Item not found on serial ' + VR.SerialNumber + ' for customer ' + VR.CustomerNumber + '. '
			ELSE ValidationErrors
		END,
		IsValid = CASE
			WHEN VR.SerialNumber = '999999' AND COALESCE(IND_NOSER.indCusno, INDA_NOSER.indCusno) IS NULL THEN 0
			WHEN VR.SerialNumber <> '999999' AND COALESCE(IND.indCusno, INDA.indCusno) IS NULL THEN 0
			ELSE IsValid
		END
	FROM #ValidationResults VR
	-- For specific serial numbers: exact match on customer/invoice/serial/item
	LEFT JOIN Datawhse.dbo.tblInvoiceDetail IND WITH (NOLOCK)
		ON IND.indCusno = VR.CustomerNumber
		AND IND.indInvno = VR.InvoiceNumber
		AND IND.indSeriesCode = VR.SerialNumber
		AND IND.indItnbr = VR.ItemNumber
		AND VR.SerialNumber <> '999999'
	LEFT JOIN Archive.dbo.tblInvoiceDetail INDA WITH (NOLOCK)
		ON INDA.indCusno = VR.CustomerNumber
		AND INDA.indInvno = VR.InvoiceNumber
		AND INDA.indSeriesCode = VR.SerialNumber
		AND INDA.indItnbr = VR.ItemNumber
		AND IND.indCusno IS NULL
		AND VR.SerialNumber <> '999999'
	-- For serial 999999: check if customer purchased item (ignore serial)
	LEFT JOIN Datawhse.dbo.tblInvoiceDetail IND_NOSER WITH (NOLOCK)
		ON IND_NOSER.indCusno = VR.CustomerNumber
		AND IND_NOSER.indItnbr = VR.ItemNumber
		AND VR.SerialNumber = '999999'
	LEFT JOIN Archive.dbo.tblInvoiceDetail INDA_NOSER WITH (NOLOCK)
		ON INDA_NOSER.indCusno = VR.CustomerNumber
		AND INDA_NOSER.indItnbr = VR.ItemNumber
		AND IND_NOSER.indCusno IS NULL
		AND VR.SerialNumber = '999999';

	--=============================================================================
	-- VALIDATION 3: Get Original Order Quantities
	--=============================================================================
	UPDATE VR
	SET OrderedQuantity = ISNULL(COALESCE(IND.indQtysh, INDA.indQtysh), 0),
		ValidationErrors = CASE
			WHEN COALESCE(IND.indQtysh, INDA.indQtysh) IS NULL OR COALESCE(IND.indQtysh, INDA.indQtysh) = 0
			THEN ValidationErrors + 'ERROR: Cannot determine original order quantity for item. '
			ELSE ValidationErrors
		END,
		IsValid = CASE
			WHEN COALESCE(IND.indQtysh, INDA.indQtysh) IS NULL OR COALESCE(IND.indQtysh, INDA.indQtysh) = 0
			THEN 0
			ELSE IsValid
		END
	FROM #ValidationResults VR
	LEFT JOIN Datawhse.dbo.tblInvoiceDetail IND WITH (NOLOCK)
		ON IND.indCusno = VR.CustomerNumber
		AND IND.indInvno = VR.InvoiceNumber
		AND IND.indOrdno = VR.OrderNumber
		AND IND.indItnbr = VR.ItemNumber
	LEFT JOIN Archive.dbo.tblInvoiceDetail INDA WITH (NOLOCK)
		ON INDA.indCusno = VR.CustomerNumber
		AND INDA.indInvno = VR.InvoiceNumber
		AND INDA.indOrdno = VR.OrderNumber
		AND INDA.indItnbr = VR.ItemNumber
		AND IND.indCusno IS NULL; -- Only check archive if not found in current

	--=============================================================================
	-- VALIDATION 4: Get Already Credited Quantities (Duplicate Check)
	--=============================================================================
	-- Check for existing credits (approved or pending) for same customer/invoice/item
	UPDATE VR
	SET AlreadyCreditedQuantity = ISNULL(EXISTING.CreditedQty, 0)
	FROM #ValidationResults VR
	OUTER APPLY (
		SELECT ROUND(SUM((radOrdQty * (radPctAllow / 100.0))), 2) AS CreditedQty
		FROM Ashley.dbo.tblRetAllowHeader RAH WITH (NOLOCK)
		INNER JOIN Ashley.dbo.tblRetAllowDetail RAD WITH (NOLOCK)
			ON RAD.radEnterDate = RAH.rahEnterDate
			AND RAD.radEnterTime = RAH.rahEnterTime
		WHERE RAH.rahCustNo = VR.CustomerNumber
			AND RAD.radInvoiceNo = VR.InvoiceNumber
			AND RAD.radOrderNumber = VR.OrderNumber
			AND RAD.radItemNo = VR.ItemNumber
			AND RAH.rahAprvDny = 'A' -- Only count approved credits
	) EXISTING;

	-- Calculate remaining creditable quantity
	UPDATE #ValidationResults
	SET RemainingCreditableQuantity = OrderedQuantity - AlreadyCreditedQuantity;

	--=============================================================================
	-- VALIDATION 5: Check if Shortage Quantity Exceeds Available Credit
	--=============================================================================
	UPDATE #ValidationResults
	SET ValidationErrors = ValidationErrors +
			'ERROR: Shortage quantity (' + CAST(ShortageQuantity AS VARCHAR) +
			') exceeds remaining creditable quantity (' + CAST(RemainingCreditableQuantity AS VARCHAR) +
			'). Already credited: ' + CAST(AlreadyCreditedQuantity AS VARCHAR) + '. ',
		IsValid = 0
	WHERE ShortageQuantity > RemainingCreditableQuantity;

	-- Check for exact duplicate (same quantity already credited)
	UPDATE #ValidationResults
	SET ValidationErrors = ValidationErrors +
			'WARNING: This exact shortage may have already been credited. Please verify. ',
		IsValid = 0
	WHERE AlreadyCreditedQuantity > 0
		AND RemainingCreditableQuantity = 0;

	--=============================================================================
	-- VALIDATION 6: Validate Defect Code
	--=============================================================================
	UPDATE VR
	SET DefectCodeValid = CASE WHEN DC.raaDefCode IS NOT NULL THEN 1 ELSE 0 END,
		ValidationErrors = CASE
			WHEN DC.raaDefCode IS NULL
			THEN ValidationErrors + 'ERROR: Defect code [' + VR.DefectCode + '] is invalid or inactive. '
			ELSE ValidationErrors
		END,
		IsValid = CASE WHEN DC.raaDefCode IS NULL THEN 0 ELSE IsValid END
	FROM #ValidationResults VR
	LEFT JOIN Ashley.dbo.tblRetAllowAddDefects DC WITH (NOLOCK)
		ON DC.raaDefCode = VR.DefectCode;

	--=============================================================================
	-- VALIDATION 7: Validate Location Code
	--=============================================================================
	UPDATE VR
	SET LocationCodeValid = CASE WHEN WH.wmahouse IS NOT NULL THEN 1 ELSE 0 END,
		ValidationErrors = CASE
			WHEN WH.wmahouse IS NULL
			THEN ValidationErrors + 'ERROR: Location code [' + VR.LocationCode + '] is invalid or inactive. '
			ELSE ValidationErrors
		END,
		IsValid = CASE WHEN WH.wmahouse IS NULL THEN 0 ELSE IsValid END
	FROM #ValidationResults VR
	LEFT JOIN Ashley.dbo.tblWarehouseMaster WH WITH (NOLOCK)
		ON WH.wmahouse = VR.LocationCode;

	--=============================================================================
	-- VALIDATION 8: Check if DFI Customer
	--=============================================================================
	UPDATE VR
	SET IsDFICustomer = CASE
		WHEN EXISTS (
			SELECT 1
			FROM Ashley.dbo.tblDiscountRates DR WITH (NOLOCK)
			INNER JOIN Ashley.dbo.tblCustomerShippingLocations CSL WITH (NOLOCK)
				ON DR.draDcode = CSL.cslDiscountCode
			WHERE CSL.cslCustomerNumber = VR.CustomerNumber
				AND CSL.cslShiptoNumber = ''
				AND DR.draDisc6 > 0
		) THEN 1
		ELSE 0
	END
	FROM #ValidationResults VR;

	--=============================================================================
	-- VALIDATION 9: Get Warehouse Code (for Container Warehouse check in application)
	--=============================================================================
	UPDATE VR
	SET WarehouseCode = COALESCE(IND.indInwhse, INDA.indInwhse)
	FROM #ValidationResults VR
	LEFT JOIN Datawhse.dbo.tblInvoiceDetail IND WITH (NOLOCK)
		ON IND.indCusno = VR.CustomerNumber
		AND IND.indInvno = VR.InvoiceNumber
		AND IND.indSeriesCode = VR.SerialNumber
		AND IND.indItnbr = VR.ItemNumber
	LEFT JOIN Archive.dbo.tblInvoiceDetail INDA WITH (NOLOCK)
		ON INDA.indCusno = VR.CustomerNumber
		AND INDA.indInvno = VR.InvoiceNumber
		AND INDA.indSeriesCode = VR.SerialNumber
		AND INDA.indItnbr = VR.ItemNumber
		AND IND.indCusno IS NULL; -- Only check archive if not found in current

	--=============================================================================
	-- Return Validation Results
	--=============================================================================
	SELECT
		CustomerNumber,
		ShipToNumber,
		InvoiceNumber,
		ItemNumber,
		SerialNumber,
		ShortageQuantity,
		Amount,  -- Credit amount
		DefectCode,
		LocationCode,
		OrderNumber,
		OrderItemSeq,
		IsValid,
		CASE
			WHEN ValidationErrors = '' THEN 'SUCCESS: All validations passed.'
			ELSE RTRIM(ValidationErrors)
		END AS ValidationErrors,
		OrderedQuantity,
		AlreadyCreditedQuantity,
		RemainingCreditableQuantity,
		ItemExists,
		CustomerSerialItemValid,
		DefectCodeValid,
		LocationCodeValid,
		IsDFICustomer,
		WarehouseCode
	FROM #ValidationResults
	ORDER BY
		CASE WHEN IsValid = 0 THEN 0 ELSE 1 END, -- Failed validations first
		CustomerNumber,
		InvoiceNumber,
		ItemNumber;

	-- Cleanup
	DROP TABLE #ValidationResults;

	SET NOCOUNT OFF;
END;
GO

