USE [datawhse]
GO

/****** Object:  StoredProcedure [dbo].[usp_OrderAndInvoiceReportingOpenInvoices2]    Script Date: 5/19/2014 7:40:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:			Leandra Edgar
-- Create date:   05/29/2008
-- Description:	To bring back open invoices for a customer
-- Called From:	OrderAndInvoiceReporting/OpenInvoice.aspx
--
-- 12/08/2008	Ledgar Added SQL Paging and custom sorting
-- 1/14/2009    Slaberge  Removed last paid date per request
-- 5/28/2009   tlockington Changed invoiceDate to be >= instead of >
-- 6/2/2009  tlockington  Added parm of ToDate to create range and named it (2)
-- 02/02/2010		Ledgar	Added the blank shipto credits to be displayed with the shipto credits
-- 02/15/2010		Ledgar added logic to include the suspended shipto's invoices
-- 02/15/2010		Ledgar added a group by
-- 07/08/2010		Tkiel	Added RPP# and Days field to result set.
-- 07/26/2010		TKiel	Added inhTripNo field to result set.
-- 04/15/2011		TKiel   Removed all logic for custom paging
-- 06/27/2012		TKiel	Took out the check for active ShipTos in the WHERE clause.
-- 07/12/2012		TKiel   Increased invoice number to 9 characters in length and added CAST( AS VARCHAR(9)) on join to tblInvoiceHeader
-- 07/13/2012		TKiel	Changed to LEFT JOIN to tblSecurityCustomer when suspended shipto, otherwise force the join
-- 05/20/2013		TKiel	Added custom SQL paging and ISNUMERIC(opiInvNo)
-- 05/22/2013		TKiel	Sort by Invoice Date
-- 05/19/2014		SeSekar	Modified the SQL Paging to show all 
-- =============================================
CREATE PROCEDURE [dbo].[usp_OrderAndInvoiceReportingOpenInvoices2]
(
	@customerNumber varchar(8), 
	@shiptoNumber	varchar(4),
	@allShiptos		bit, 
	@securityMHS	varchar(8),
	@FromDate		datetime, 
	@ToDate			datetime,
	@showCredits	bit, 
	@showOldCredits bit, 
	@searchInvoice	varchar(9),
	@searchCredit	varchar(6), 
	@searchPo		varchar(22),
	@sortBy			varchar(20), 
	@sortAscending	varchar(5),
	@pageNum		int,
	@pageSize		int
)
AS
BEGIN

	SET NOCOUNT ON;

/*********************** TESTING VARIABLES **********************************/
--declare @customerNumber varchar(8), @shiptoNumber varchar(4),
--		  @allShiptos bit, @securityMHS varchar(8),
--		  @invoiceDate datetime, @showCredits bit, 
--		  @showOldCredits bit, @searchInvoice varchar(6),
--		  @searchCredit varchar(6),  @searchPo varchar(22),
--		  @sortBy varchar(20), @sortAscending varchar(5),
--		  @pageNum int, @pageSize int, @FromDate datetime, @ToDate datetime

--select @customerNumber='2825100'
--select @shiptoNumber=''
--select @allShiptos = 1
--select @securityMHS = 'MASTERXX'
--select @invoiceDate='05/01/2008'
--select @showCredits = 1
--select @showOldCredits = 1
--select @pageNum = 1
--select @pageSize = 500
--select @sortBy = 'opidagedt'
--select @sortAscending = 'ASC '
--select @FromDate = getDate()
--select @ToDate = '01/01/2010'

--drop table #tempOpenInvoices

/*********************** TESTING VARIABLES **********************************/
		DECLARE @sqlString		varchar(MAX) = '',
				@recordCount	int,
				@lbound			int,
				@ubound			int,
				@pageCount		int

		CREATE table #tempOpenInvoices(row_number int PRIMARY KEY,
															opicusno varchar(8),
															opishpno varchar(4),
															opiinvno varchar(9),
															opicrmnr varchar(6),
															opiDagedt datetime,
															opiInvam money,
															opiTtlcr money,
															opiOpamt money,
															opiDlpdtto datetime,
															opicatcd varchar(2),
															opiponum varchar(22),
															opiOrdno varchar(7),
															inhdordda datetime,
															[RPP #] int,
															[Days] int,
															inhTripNo int)

		SELECT @sqlString = 'INSERT INTO #tempOpenInvoices 
				SELECT DISTINCT row_number() OVER ( '

			IF ltrim(rtrim(@sortBy)) <> '' BEGIN
				SELECT @sqlString = @sqlString + 'ORDER BY ' + @sortBy + ' ' + @sortAscending
			END
			ELSE BEGIN
				SELECT @sqlString = @sqlString + 'ORDER BY opidagedt, T1.opiinvno '
			END

				SELECT @sqlString = @sqlString + ') as rowid, t1.opicusno, T1.opishpno,T1.opiinvno,
				T1.opicrmnr AS opicrmnr,convert(varchar, T1.opiDagedt, 101) as opiDagedt,T1.opiInvam,T1.opiTtlcr,
				T1.opiOpamt, convert(varchar, T1.opiDlpdto, 101) as opiDlpdto, T1.opicatcd, T1.opiponum,
				T1.opiOrdno, isnull(T2.inhdordda, '''') as inhdordda,
				CASE when left(inhShipInstructions,3) = ''RPP'' then right(inhShipInstructions, len(inhShipInstructions)-4) else '''' end as [RPP #],
				DATEDIFF(DAY, CONVERT(varchar(12), opidagedt, 101), getDate()) AS [Days], T2.inhTripNo
			FROM datawhse.dbo.tblopeninvoices T1 with (nolock) 
			LEFT OUTER JOIN datawhse.dbo.tblinvoiceheader T2 with (nolock) 
					ON T1.opiinvno=T2.inhinvno
					AND T1.opiordno = T2.inhOrdno 
					AND T1.opicusno=T2.inhcusno 
					AND T1.opishpno=T2.inhshpno 
			LEFT OUTER JOIN Ashley.dbo.tblsecuritycustomer T3 (nolock) 
					ON T1.opicusno=T3.seccusno 
					AND T1.opishpno=T3.secshpno 
			INNER JOIN Ashley.dbo.tblCustomerShippingLocations T4 WITH (NOLOCK)
					ON T1.opiCusNo = T4.cslCustomerNumber
					AND T1.opiShpNo = T4.cslShipToNumber
			WHERE ISNUMERIC(opiinvno) = 1
					AND T1.opicusno=''' + @customerNumber + '''  
					AND T1.opidagedt <= ''' + cast(@FromDate as varchar) + ''' 
					AND T1.opidagedt >= ''' + cast(@ToDate as varchar) + ''' 
					AND ((T4.acrec <> ''S''
							AND T1.opicusno=T3.seccusno
							AND T1.opishpno=T3.secshpno
							AND T3.secMhs_name = ''' + @securityMHS + ''')
						OR (T4.acrec = ''S'')) '

		If @allShiptos = 0 BEGIN
		SELECT @sqlString = @sqlString + 'AND (T1.opishpno=''' + @shiptoNumber + ''' '
				
		SELECT @sqlString = @sqlString + 'OR (t1.opiShpno='''' AND t1.opicrmnr <> ''0'')) '
		END

	IF @showCredits = 1 AND @showOldCredits = 0 BEGIN
		SELECT @sqlString = @sqlString + 'AND (T1.opiopamt <> 0 AND T1.opidagedt >= ''' + cast(getdate()-365 as varchar) + ''') '
	END
	ELSE IF @showCredits = 0 AND @showOldCredits = 0 BEGIN
		SELECT @sqlString = @sqlString + 'AND T1.opiopamt > 0 '
	END

	IF ltrim(rtrim(@searchInvoice)) <> '' BEGIN
		SELECT @sqlString = @sqlString + 'AND T1.opiinvno LIKE ''' + @searchInvoice + '%'' '
	END

	IF ltrim(rtrim(@searchCredit)) <> '' BEGIN
		SELECT @sqlString = @sqlString + 'AND T1.opicrmnr LIKE ''' + @searchCredit + '%'' '
	END

		IF ltrim(rtrim(@searchPO)) <> '' BEGIN
		SELECT @sqlString = @sqlString + 'AND T1.opiponum LIKE ''' + @searchPO + '%'' '
		END

	SELECT @sqlString = @sqlString + ' GROUP BY opicusno, opishpno, opiinvno, opicrmnr, opidagedt, opiinvam, opittlcr, opiopamt, opicatcd, opiponum, opiordno, inhdordda, opidlpdto,
										CASE when left(inhShipInstructions,3) = ''RPP'' then right(inhShipInstructions, len(inhShipInstructions)-4) else '''' end,
		DATEDIFF(DAY, CONVERT(varchar(12), opidagedt, 101), getDate()), inhTripNo '
--select @sqlString
	EXEC(@sqlString)

	SELECT @recordCount = count(*) FROM #tempOpenInvoices

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
--convert(varchar, opiDlpdtto, 101) as opiDlpdto,    last paid date removed 1/14/2009
	SELECT opicusno, opishpno, opiinvno, opicrmnr, convert(varchar, opidagedt, 101) as opiDagedt,
		opiInvam, opiTtlcr, opiOpamt, opicatcd, opiponum, opiOrdno,
		convert(varchar, inhdordda, 101) as inhdordda, [RPP #], [Days], inhTripNo, @pageCount AS [PageCount]
	FROM #tempOpenInvoices
	WHERE row_number between @lbound and @ubound
	ORDER BY ROW_NUMBER

	SET NOCOUNT OFF;
END







GO

