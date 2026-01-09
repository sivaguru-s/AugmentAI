USE [Ashley]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetEpayUsers]    Script Date: 5/19/2014 7:50:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Tyler Kiel
-- Create date:	08/20/2010
-- Description:	To obtain a list of E-payment users with search capability.

-- SeSekar		05/19/2014	Modified the SQL Paging to show all records
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetEpayUsers](@searchCustomerNumber varchar(8), @searchCustomerName varchar(25),             
               @searchBillToState char(3), @searchTerritory varchar(3), @searchTermsCode varchar(3), @pageNum int, @pageSize int)

AS
BEGIN
	
	SET NOCOUNT ON;
	
	DECLARE @sqlString varchar(5000)
	SELECT @sqlString = ''
	DECLARE @recordCount int
		DECLARE @lbound int, @ubound int
		DECLARE @pageCount int

		CREATE table #tempEpayUsers(row_number int PRIMARY KEY,
															[Acct #] varchar(8),
															[Acct Name] varchar(25),
															[Bill-To-State] char(3),
															Territory varchar(3),
															[Terms Code] varchar(3))
															
	SELECT @sqlString = 'INSERT INTO #tempEpayUsers 
				SELECT DISTINCT row_number() OVER ( ORDER BY Cast(cmacustomernumber as Decimal(8,0))) as rowid, '
	
	SELECT @sqlString = @sqlString + 'Cast(cmacustomernumber as Decimal(8,0)), cmaCustomerName, paaState as [Bill-To-State], '
	SELECT @sqlString = @sqlString + '	CAST(cmaCreditTerritoryID as varchar(3)) as Territory, cmaTermsCode as [Terms Code] '
	SELECT @sqlString = @sqlString + 'FROM Ashley.dbo.tblCustomerAccountMaster t1 WITH (NOLOCK) '
	SELECT @sqlString = @sqlString + 'LEFT JOIN '
	SELECT @sqlString = @sqlString + '	(SELECT LTrim(Replace(tblUserProfile.usrUserLogin,''AD_'', '''')) as EPayAcct '
	SELECT @sqlString = @sqlString + '	 FROM Security.dbo.tblGroupPermissions WITH (NOLOCK) '
	SELECT @sqlString = @sqlString + '	 INNER JOIN Security.dbo.tblUserProfile WITH (NOLOCK) '
	SELECT @sqlString = @sqlString + '		ON tblGroupPermissions.gprUserLogin = tblUserProfile.usrUserLogin '
	SELECT @sqlString = @sqlString + '  INNER JOIN Security.dbo.tblGroupProfile WITH (NOLOCK) '
	SELECT @sqlString = @sqlString + '     ON tblGroupPermissions.gprGroupId = tblGroupProfile.grpSecurityId '
	SELECT @sqlString = @sqlString + '  WHERE grpGroupId = ''EPAY'' AND usrUserLogin LIKE ''AD_%'') as EPayAccts '
	SELECT @sqlString = @sqlString + ' on cmaCustomerNumber = EPayAcct '
	SELECT @sqlString = @sqlString + 'INNER JOIN Ashley.dbo.tblCustomerShippingLocations with (nolock) '
	SELECT @sqlString = @sqlString + '	ON cmaCustomerNumber=cslCustomerNumber AND cslShiptoNumber='''' '
	SELECT @sqlString = @sqlString + 'INNER JOIN Ashley.dbo.tblPartyAddressMaster with (nolock) '
	SELECT @sqlString = @sqlString + ' ON cslBuyerAddressID=paaAddressID '
	SELECT @sqlString = @sqlString + 'WHERE EpayAcct IS NOT NULL and t1.acrec = ''A'' '
				
	IF ltrim(rtrim(@searchCustomerNumber)) <> '' BEGIN
		SELECT @sqlString = @sqlString + 'AND cmaCustomerNumber LIKE ''' + ltrim(rtrim(@searchCustomerNumber)) + '%'' '
	END
	
	IF ltrim(rtrim(@searchCustomerName)) <> '' BEGIN
		SELECT @sqlString = @sqlString + 'AND cmaCustomerName LIKE ''' + ltrim(rtrim(@searchCustomerName)) + '%'' '
	END
	
	IF ltrim(rtrim(@searchBillToState)) <> 'All' BEGIN
		IF ltrim(rtrim(@searchBillToState)) <> ''
		BEGIN	
			SELECT @sqlString = @sqlString + 'AND paaState LIKE ''' + ltrim(rtrim(@searchBillToState)) + '%'' '
		END
		ELSE
		BEGIN
			SELECT @sqlString = @sqlString + 'AND paaState = ''' + ltrim(rtrim(@searchBillToState)) + ''' '
		END
	END

	IF ltrim(rtrim(@searchTerritory)) <> 'All' BEGIN
		SELECT @sqlString = @sqlString + 'AND CAST(cmaCreditTerritoryID as varchar(3)) LIKE ''' + ltrim(rtrim(@searchTerritory)) + '%'' '
	END
	
	IF ltrim(rtrim(@searchTermsCode)) <> 'All' BEGIN
		IF ltrim(rtrim(@searchTermsCode)) <> ''
		BEGIN
			SELECT @sqlString = @sqlString + 'AND cmaTermsCode LIKE ''' + ltrim(rtrim(@searchTermsCode)) + '%'' '
		END
		ELSE
		BEGIN
			SELECT @sqlString = @sqlString + 'AND cmaTermsCode = ''' + ltrim(rtrim(@searchTermsCode)) + ''' '
		END
	END
	
	EXEC(@sqlString)
	
	
	SELECT @recordCount = count(*) FROM #tempEpayUsers

		
	 IF @pageSize = 0 BEGIN
			SET @pageCount = 1
			SET @lbound = 1
			SET @ubound = @recordCount
		END		
		ELSE BEGIN
		
		
			SELECT @pageCount = ceiling(cast(@recordCount as decimal(10,2))/cast(@pageSize as decimal(10,2)))
		
			
		--	SET @pageNum = ABS(@pageNum)
			SET @pageSize = ABS(@pageSize)
		--	IF @pageNum < 1 SET @pageNum = 1
			IF @pageSize < 1 SET @pageSize = 1

		
			IF @pageNum = -1 Begin
			
			SET @lbound = 1
			Set @ubound = @recordCount
			End
			else begin
			SET @lbound = ((@pageNum - 1) * @pageSize) + 1
			SET @ubound = @lbound + @pageSize - 1
				
				IF @lbound >= @recordCount BEGIN
					SET @ubound = @recordCount + 1
					SET @lbound = @ubound - (@pageSize +1 )
				END
		END
		END
		
		SELECT [Acct #], [Acct Name], [Bill-To-State], Territory, [Terms Code], @pageCount as pageCount FROM #tempEpayUsers
	WHERE row_number between @lbound and @ubound

    SET NOCOUNT OFF;
END

GO

