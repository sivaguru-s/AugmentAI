USE [Ashley]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetOpenInvoicesFromNonAshleyPo3]    Script Date: 5/19/2014 7:42:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*******************************************************************
Procedure:			[usp_GetOpenInvoicesFromNonAshleyPo]
System:				AshleyDirect
Database:			BATCH/Ashley
Business Function:	Gets all open invoices associated with the  
					value entered in the PO Search field from 
					tblopeninvoices (Ashley PO#s) and 
					tblInvoiceDetailProperties (non-Ashley PO#s)

					Called from: /OrderAndInvoiceReporting/OpenInvoices.aspx
							
RuJohnson	04/16/2008	Creation 
Ledgar			08/15/2008	Added shiptonumber, allshiptos
										changed the temp table to be able to be
										called into a datareader with the same
										fields as usp_OrderAndInvoiceReportingOpenInvoices
tlockington  6/2/2009  Added ToDate and made the name usp_GetOpenInvoicesFromNonAshleyPo3
Tkiel		07/08/2010	Added RPP# and Days fields to result set.
TKiel		07/26/2010	Added inhTripNo field to result set.
TKiel		08/06/2010	Removed unnecessary opiDlpdto field from #openInvoices
TKiel		05/20/2013	Added custom SQL paging for Epay and ISNUMERIC(opiInvNo)
TKiel		05/22/2013	Sort by Invoice Date
SeSekar		05/19/2014	Modified the SQL Paging to show all records
********************************************************************/

-- we KNOW that there is a po number supplied 

CREATE PROCEDURE [dbo].[usp_GetOpenInvoicesFromNonAshleyPo3]
(
	@customerNumber VARCHAR(10),
	@shiptoNumber	varchar(4),
	@allShiptos		bit,
	@security_MHS	VARCHAR(25),
	@fromDate		DATETIME,
	@toDate			DATETIME,
	@showCredits	BIT,
	@showOldCredits BIT,
	@invoiceNumber	VARCHAR(6),
	@creditNumber	VARCHAR(15),
	@poNumber		VARCHAR(25),
	@sortBy			VARCHAR(20),
	@sortAscending	varchar(5),
	@pageNum		int,
	@pageSize		int
)
AS BEGIN

	SET NOCOUNT ON;


-- TESTING
--DECLARE @customerNumber VARCHAR(8)
--dECLARE @shiptoNumber varchar(4)
--DECLARE @allShiptos bit
--DECLARE @fromDate DATETIME
--DECLARE @showCredits BIT
--DECLARE @showOldCredits BIT
--DECLARE @security_MHS VARCHAR(25)
--DECLARE @invoiceNumber VARCHAR(6)
--DECLARE @creditNumber VARCHAR(15)
--DECLARE @poNumber VARCHAR(25)
--DECLARE @sortBy VARCHAR(15)
--DECLARE @sortAscending varchar(5)
--
--SET @customerNumber = '1251600'
--SET @fromDate = '05/01/2008'
--SET @allShiptos = 1
--SET @showCredits = 1
--SET @showOldCredits = 1
--SET @SECURITY_MHS = 'MASTERXX'
--SET @invoiceNumber = ''
--SET @creditNumber = ''
--SET @poNumber = ''
--SET @sortBy = 'opishpno'
--SET @sortAscending='ASC'
--drop table #openInvoices


	DECLARE @sqlString varchar(MAX) = '',
			@recordCount	int,
			@lbound			int,
			@ubound			int,
			@pageCount		int;

	CREATE TABLE #openInvoices (						row_number int PRIMARY KEY,
														opicusno VARCHAR(8),
														opishpno VARCHAR(4),
														opiinvno VARCHAR(6),
														opicrmnr INT,
														opiDagedt SMALLDATETIME,
														opiInvam DECIMAL(12,2),
														opiTtlcr DECIMAL(12,2),
														opiOpamt DECIMAL(12,2), 
														opicatcd VARCHAR(2),
														opiponum VARCHAR(22),
														opiordno VARCHAR(7),
														inhdordda DATETIME,
														[RPP #] int,
														[Days] int,
														inhTripNo int)


-- look for non-Ashley PO#s
SELECT @sqlString = 'INSERT INTO #openInvoices
				SELECT DISTINCT row_number() OVER ( '

					IF ltrim(rtrim(@sortBy)) <> '' BEGIN
						SELECT @sqlString = @sqlString + 'ORDER BY ' + @sortBy + ' ' + @sortAscending
					END
					ELSE BEGIN
						SELECT @sqlString = @sqlString + 'ORDER BY opidagedt, T1.opiinvno '
					END

						SELECT @sqlString = @sqlString + ') as rowid, opicusno, opishpno, opiinvno, opicrmnr, opiDagedt, opiInvam, 
											opiTtlcr, opiOpamt, opicatcd, opiponum, opiordno, inhdordda, 
											CASE when left(inhShipInstructions,3) = ''RPP'' then right(inhShipInstructions, len(inhShipInstructions)-4) else '''' end as [RPP #],
											DATEDIFF(DAY, CONVERT(varchar(12), opidagedt, 101), getDate()) AS [Days], inhTripNo

					FROM datawhse.dbo.tblopeninvoices T1  with (nolock)
					LEFT OUTER JOIN datawhse.dbo.tblinvoiceheader T2 with (nolock) 
							ON T1.opiinvno = T2.inhinvno 
							AND T1.opicusno = T2.inhcusno 
							AND T1.opishpno = T2.inhshpno 
							AND T1.opiponum = T2.inhponum 
							AND T1.opiordno = T2.inhordno  
					INNER JOIN Ashley.dbo.tblsecuritycustomer T3 with (nolock)
							ON T1.opicusno = T3.seccusno 
							AND T1.opishpno = T3.secshpno  
					INNER JOIN datawhse.dbo.tblInvoiceDetailProperties T4 WITH (NOLOCK) 
							ON T4.idpOrderNumber = T1.opiOrdno 
							AND T4.idpInvoiceNumber = T1.opiInvno
					WHERE ISNUMERIC(T1.opiInvNo) = 1
							AND T1.opicusno= ''' + @customerNumber  + ''' '

						IF @allShiptos = 0 BEGIN
							SELECT @sqlString = @sqlString + 'AND opishpno=''' + @shiptoNumber + ''' '
						END

					SELECT @sqlString = @sqlString + 'AND T1.opidagedt <= ''' + cast(@fromDate as varchar) + '''
						AND T1.opidagedt >= ''' + cast(@toDate as varchar) + '''
						AND T1.opiinvno LIKE (''' + @invoiceNumber + '%'')
						AND T1.opicrmnr LIKE (''' + @creditNumber + '%'') 
						AND T3.secMhs_name = ''' + @security_MHS + '''
						AND RTRIM(T4.idpFieldName) = ''CONSUMERPONUMBER''
						AND RTRIM(T4.idpFieldValue) LIKE (''' + @poNumber + '%'') '

--select @sqlString
	EXEC(@sqlString)



-- look for Ashley PO#s
SELECT @sqlString = 'INSERT INTO #openInvoices
									SELECT DISTINCT row_number() OVER ( '

									IF ltrim(rtrim(@sortBy)) <> '' BEGIN
										SELECT @sqlString = @sqlString + 'ORDER BY ' + @sortBy + ' ' + @sortAscending
									END
									ELSE BEGIN
										SELECT @sqlString = @sqlString + 'ORDER BY T1.opiinvno, opidagedt '
									END

										SELECT @sqlString = @sqlString + ') as rowid, opicusno, opishpno, opiinvno, opicrmnr, opiDagedt, opiInvam, 
																opiTtlcr, opiOpamt, opicatcd, opiponum, opiordno, inhdordda, 
											CASE when left(inhShipInstructions,3) = ''RPP'' then right(inhShipInstructions, len(inhShipInstructions)-4) else '''' end as [RPP #],
											DATEDIFF(DAY, CONVERT(varchar(12), opidagedt, 101), getDate()) AS [Days], inhTripNo

								FROM datawhse.dbo.tblopeninvoices T1  with (nolock)
								LEFT OUTER JOIN datawhse.dbo.tblinvoiceheader T2 with (nolock) 
										ON T1.opiinvno = T2.inhinvno 
										AND T1.opicusno = T2.inhcusno 
										AND T1.opishpno = T2.inhshpno 
										AND T1.opiponum = T2.inhponum 
										AND T1.opiordno = T2.inhordno  
								INNER JOIN Ashley.dbo.tblsecuritycustomer T3 with (nolock) 
										ON T1.opicusno = T3.seccusno 
										AND T1.opishpno = T3.secshpno  

								WHERE ISNUMERIC(T1.opiInvNo) = 1
									AND T1.opicusno= ''' + @customerNumber + ''' '
								IF @allShiptos = 0 BEGIN
									SELECT @sqlString = @sqlString + 'AND opishpno=''' + @shiptoNumber + ''' '
								END

					SELECT @sqlString = @sqlString + 'AND T1.opidagedt <= ''' + cast(@fromDate as varchar) + '''
						AND T1.opidagedt >= ''' + cast(@toDate as varchar) + '''
						AND T1.opiinvno LIKE (''' + @invoiceNumber + '%'')
						AND T1.opicrmnr LIKE (''' + @creditNumber + '%'') 
						AND T3.secMhs_name = ''' + @security_MHS + '''
						AND opiPonum LIKE ('''+ @poNumber + '%'')'

--select @sqlString
			EXEC(@sqlString)

	SELECT @recordCount = count(*) FROM #openInvoices

	IF @pageSize = 0 BEGIN
		SET @pageCount = 1
		SET @lbound = 1
		SET @ubound = @recordCount
	END

	ELSE BEGIN
		SELECT @pageCount = ceiling(cast(@recordCount as decimal(10,2))/cast(@pageSize as decimal(10,2)))

--		SET @pageNum = ABS(@pageNum)
		SET @pageSize = ABS(@pageSize)
--		IF @pageNum < 1 SET @pageNum = 1
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
	Select opicusno,opishpno,opiinvno,opicrmnr,CONVERT(VARCHAR, opiDagedt, 101) AS opiDagedt,
		opiInvam,opiTtlcr,opiOpamt,opicatcd,opiponum,opiOrdno,inhdordda, [RPP #], [Days], inhTripNo, @pageCount AS [PageCount]
	FROM #openInvoices
	WHERE row_number between @lbound and @ubound
	ORDER BY ROW_NUMBER

SET NOCOUNT OFF;

END





GO

