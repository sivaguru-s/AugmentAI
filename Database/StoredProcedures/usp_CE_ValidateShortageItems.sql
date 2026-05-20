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
*		SELECT '8888300', '0001', 110433, 'C310325', '7320325', 1, 'XP', 'WU', 1234567, 100;
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

-- First, create the User Defined Table Type for input parameters
IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'typCEShortageItemValidation')
BEGIN
	CREATE TYPE [dbo].[typCEShortageItemValidation] AS TABLE(
		[CustomerNumber] VARCHAR(8) NOT NULL,
		[ShipToNumber] VARCHAR(4) NOT NULL,
		[InvoiceNumber] NUMERIC(6,0) NOT NULL,
		[ItemNumber] VARCHAR(15) NOT NULL,
		[SerialNumber] VARCHAR(10) NOT NULL,
		[ShortageQuantity] NUMERIC(7,0) NOT NULL,
		[DefectCode] VARCHAR(4) NULL,
		[LocationCode] VARCHAR(2) NULL,
		[OrderNumber] NUMERIC(7,0) NULL,
		[OrderItemSeq] INT NULL
	)
END
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
		InvoiceNumber NUMERIC(6,0),
		ItemNumber VARCHAR(15),
		SerialNumber VARCHAR(10),
		ShortageQuantity NUMERIC(7,0),
		DefectCode VARCHAR(4),
		LocationCode VARCHAR(2),
		OrderNumber NUMERIC(7,0),
		OrderItemSeq INT,
		IsValid BIT DEFAULT 1,
		ValidationErrors VARCHAR(MAX) DEFAULT '',
		OrderedQuantity NUMERIC(7,0) DEFAULT 0,
		AlreadyCreditedQuantity DECIMAL(10,2) DEFAULT 0,
		RemainingCreditableQuantity DECIMAL(10,2) DEFAULT 0,
		ItemExists BIT DEFAULT 0,
		CustomerSerialItemValid BIT DEFAULT 0,
		DefectCodeValid BIT DEFAULT 0,
		LocationCodeValid BIT DEFAULT 0
	);

	-- Initialize results with input data and defaults
	INSERT INTO #ValidationResults (
		CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber,
		ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq
	)
	SELECT 
		CustomerNumber,
		ShipToNumber,
		InvoiceNumber,
		ItemNumber,
		SerialNumber,
		ShortageQuantity,
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
	SET ItemExists = CASE WHEN IM.itmItemnumber IS NOT NULL THEN 1 ELSE 0 END,
		ValidationErrors = CASE 
			WHEN IM.itmItemnumber IS NULL 
			THEN ValidationErrors + 'ERROR: Item [' + VR.ItemNumber + '] does not exist or is invalid. ' 
			ELSE ValidationErrors 
		END,
		IsValid = CASE WHEN IM.itmItemnumber IS NULL THEN 0 ELSE IsValid END
	FROM #ValidationResults VR
	LEFT JOIN Ashley.dbo.tblItemMaster IM WITH (NOLOCK)
		ON IM.itmItemnumber = VR.ItemNumber;

	--=============================================================================
	-- VALIDATION 2: Verify Customer/Serial/Item Combination
	--=============================================================================
	UPDATE VR
	SET CustomerSerialItemValid = CASE WHEN IND.indCusno IS NOT NULL THEN 1 ELSE 0 END,
		ValidationErrors = CASE 
			WHEN IND.indCusno IS NULL 
			THEN ValidationErrors + 'ERROR: Customer/Serial/Item combination is invalid. Item not found on serial ' + VR.SerialNumber + ' for customer ' + VR.CustomerNumber + '. '
			ELSE ValidationErrors 
		END,
		IsValid = CASE WHEN IND.indCusno IS NULL THEN 0 ELSE IsValid END
	FROM #ValidationResults VR
	LEFT JOIN Datawhse.dbo.tblInvoiceDetail IND WITH (NOLOCK)
		ON IND.indCusno = VR.CustomerNumber
		AND IND.indInvno = VR.InvoiceNumber
		AND IND.indSerno = VR.SerialNumber
		AND IND.indItnbr = VR.ItemNumber
	LEFT JOIN Archive.dbo.tblInvoiceDetail INDA WITH (NOLOCK)
		ON INDA.indCusno = VR.CustomerNumber
		AND INDA.indInvno = VR.InvoiceNumber
		AND INDA.indSerno = VR.SerialNumber
		AND INDA.indItnbr = VR.ItemNumber
		AND IND.indCusno IS NULL -- Only check archive if not found in current
	WHERE IND.indCusno IS NOT NULL OR INDA.indCusno IS NOT NULL;

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
	SET DefectCodeValid = CASE WHEN DC.defDefectCode IS NOT NULL THEN 1 ELSE 0 END,
		ValidationErrors = CASE
			WHEN DC.defDefectCode IS NULL
			THEN ValidationErrors + 'ERROR: Defect code [' + VR.DefectCode + '] is invalid or inactive. '
			ELSE ValidationErrors
		END,
		IsValid = CASE WHEN DC.defDefectCode IS NULL THEN 0 ELSE IsValid END
	FROM #ValidationResults VR
	LEFT JOIN Ashley.dbo.tblDefectCodes DC WITH (NOLOCK)
		ON DC.defDefectCode = VR.DefectCode
		AND DC.defActive = 'Y'; -- Only active defect codes

	--=============================================================================
	-- VALIDATION 7: Validate Location Code
	--=============================================================================
	UPDATE VR
	SET LocationCodeValid = CASE WHEN WH.whsWhseCode IS NOT NULL THEN 1 ELSE 0 END,
		ValidationErrors = CASE
			WHEN WH.whsWhseCode IS NULL
			THEN ValidationErrors + 'ERROR: Location code [' + VR.LocationCode + '] is invalid or inactive. '
			ELSE ValidationErrors
		END,
		IsValid = CASE WHEN WH.whsWhseCode IS NULL THEN 0 ELSE IsValid END
	FROM #ValidationResults VR
	LEFT JOIN Ashley.dbo.tblWarehouse WH WITH (NOLOCK)
		ON WH.whsWhseCode = VR.LocationCode
		AND WH.whsActive = 'Y'; -- Only active warehouses

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
		LocationCodeValid
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

