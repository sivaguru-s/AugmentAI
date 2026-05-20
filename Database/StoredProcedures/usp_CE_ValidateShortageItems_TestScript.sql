/*******************************************************************************
* Test Script for usp_CE_ValidateShortageItems
* Purpose: Comprehensive test cases for shortage item validation
* Created: 2026-05-20
*******************************************************************************/

USE [Ashley]
GO

PRINT '========================================';
PRINT 'TEST SUITE: usp_CE_ValidateShortageItems';
PRINT '========================================';
PRINT '';

--==============================================================================
-- TEST 1: Valid Shortage Item (All Validations Pass)
--==============================================================================
PRINT 'TEST 1: Valid Shortage Item - All validations should pass';
PRINT '--------------------------------------------------------';

DECLARE @Test1 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test1 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES ('8888300', '0001', 110433, 'C310325', '7320325', 1, 'XP', 'WU', 1234567, 100);

EXEC usp_CE_ValidateShortageItems @Test1, 'AFI';

PRINT '';
PRINT '';

--==============================================================================
-- TEST 2: Invalid Item Number
--==============================================================================
PRINT 'TEST 2: Invalid Item Number - Should fail item validation';
PRINT '--------------------------------------------------------';

DECLARE @Test2 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test2 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES ('8888300', '0001', 110433, 'INVALIDITEM', '7320325', 1, 'XP', 'WU', 1234567, 100);

EXEC usp_CE_ValidateShortageItems @Test2, 'AFI';

PRINT '';
PRINT '';

--==============================================================================
-- TEST 3: Invalid Customer/Serial/Item Combination
--==============================================================================
PRINT 'TEST 3: Invalid Customer/Serial/Item Combination';
PRINT '--------------------------------------------------------';

DECLARE @Test3 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test3 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES ('9999999', '0001', 110433, 'C310325', 'BADSERIAL', 1, 'XP', 'WU', 1234567, 100);

EXEC usp_CE_ValidateShortageItems @Test3, 'AFI';

PRINT '';
PRINT '';

--==============================================================================
-- TEST 4: Shortage Quantity Exceeds Order Quantity
--==============================================================================
PRINT 'TEST 4: Shortage Quantity Exceeds Order Quantity';
PRINT '--------------------------------------------------------';

DECLARE @Test4 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test4 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES ('8888300', '0001', 110433, 'C310325', '7320325', 999999, 'XP', 'WU', 1234567, 100);

EXEC usp_CE_ValidateShortageItems @Test4, 'AFI';

PRINT '';
PRINT '';

--==============================================================================
-- TEST 5: Invalid Defect Code
--==============================================================================
PRINT 'TEST 5: Invalid Defect Code - Should fail defect code validation';
PRINT '--------------------------------------------------------';

DECLARE @Test5 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test5 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES ('8888300', '0001', 110433, 'C310325', '7320325', 1, 'INVALID', 'WU', 1234567, 100);

EXEC usp_CE_ValidateShortageItems @Test5, 'AFI';

PRINT '';
PRINT '';

--==============================================================================
-- TEST 6: Invalid Location Code
--==============================================================================
PRINT 'TEST 6: Invalid Location Code - Should fail location validation';
PRINT '--------------------------------------------------------';

DECLARE @Test6 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test6 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES ('8888300', '0001', 110433, 'C310325', '7320325', 1, 'XP', 'ZZ', 1234567, 100);

EXEC usp_CE_ValidateShortageItems @Test6, 'AFI';

PRINT '';
PRINT '';

--==============================================================================
-- TEST 7: Default Defect and Location Codes (NULL values)
--==============================================================================
PRINT 'TEST 7: Default Defect and Location Codes - Should use defaults (XP, WU)';
PRINT '--------------------------------------------------------';

DECLARE @Test7 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test7 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES ('8888300', '0001', 110433, 'C310325', '7320325', 1, NULL, NULL, 1234567, 100);

EXEC usp_CE_ValidateShortageItems @Test7, 'AFI';

PRINT '';
PRINT '';

--==============================================================================
-- TEST 8: Batch Validation - Multiple Items
--==============================================================================
PRINT 'TEST 8: Batch Validation - Multiple items with mixed validation results';
PRINT '--------------------------------------------------------';

DECLARE @Test8 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test8 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES 
	('8888300', '0001', 110433, 'C310325', '7320325', 1, 'XP', 'WU', 1234567, 100),    -- Valid
	('8888300', '0001', 110433, 'INVALID', '7320325', 1, 'XP', 'WU', 1234567, 200),    -- Invalid item
	('8888300', '0001', 110433, 'C310325', '7320325', 999, 'XP', 'WU', 1234567, 300),  -- Qty too high
	('8888300', '0001', 110433, 'C310325', '7320325', 1, 'BAD', 'WU', 1234567, 400),   -- Bad defect code
	('8888300', '0001', 110433, 'C310325', '7320325', 1, 'XP', 'ZZ', 1234567, 500);    -- Bad location

EXEC usp_CE_ValidateShortageItems @Test8, 'AFI';

PRINT '';
PRINT '';

--==============================================================================
-- TEST 9: Custom Default Codes
--==============================================================================
PRINT 'TEST 9: Custom Default Codes - Override default defect and location';
PRINT '--------------------------------------------------------';

DECLARE @Test9 AS dbo.typCEShortageItemValidation;

INSERT INTO @Test9 (CustomerNumber, ShipToNumber, InvoiceNumber, ItemNumber, SerialNumber, 
					ShortageQuantity, DefectCode, LocationCode, OrderNumber, OrderItemSeq)
VALUES ('8888300', '0001', 110433, 'C310325', '7320325', 1, NULL, NULL, 1234567, 100);

-- Pass custom defaults (if your warehouse uses different codes)
EXEC usp_CE_ValidateShortageItems @Test9, 'AFI', 'DA', 'RC';

PRINT '';
PRINT '';

PRINT '========================================';
PRINT 'TEST SUITE COMPLETED';
PRINT '========================================';

