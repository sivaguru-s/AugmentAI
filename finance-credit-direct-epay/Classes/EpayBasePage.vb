Option Explicit On
Option Strict On

#If DEBUG Then
Imports Generic = Ashley.Web.LocalUI
#Else
Imports Generic = Ashley.Web.UI
#End If

Public Class EpayBasePage
    Inherits Generic.BasePage

#Region " Variable Declarations "



#End Region

#Region " Public Properties "

    Public ReadOnly Property CustomerNumber As String
        Get
            Return Me.SessionData("CUSTOMERNUMBER")
        End Get
    End Property

    Public ReadOnly Property TermsCodesDataReader As SqlClient.SqlDataReader
        Get
            Return CType(DataAccess.ExecuteStoredProcedure("AFI_Dynamic", "Ashley.dbo.usp_GetEpayTermsCodes", DataAccess.StoredProcedureReturnType.DataReader), SqlClient.SqlDataReader)
        End Get
    End Property

    Public ReadOnly Property CreditTerritoryDataReader As SqlClient.SqlDataReader
        Get
            Return CType(DataAccess.ExecuteStoredProcedure("AFI_Batch", "Datawhse.dbo.usp_GetEpayCreditTerritories", DataAccess.StoredProcedureReturnType.DataReader), SqlClient.SqlDataReader)
        End Get
    End Property

    Public ReadOnly Property BillToStateDataReader As SqlClient.SqlDataReader
        Get
            Return CType(DataAccess.ExecuteStoredProcedure("AFI_Dynamic", "Ashley.dbo.usp_GetEpayBillToStates", DataAccess.StoredProcedureReturnType.DataReader), SqlClient.SqlDataReader)
        End Get
    End Property

#End Region

#Region " Event Handlers "

#If DEBUG Then
    Protected Overloads Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If String.IsNullOrEmpty(SessionData("LOGON_USER")) Then
            'Impersonate an end user
            SessionData("LOGON_USER") = "tkiel"
            SessionData("SECURITY_MHS") = GetUserMHS(SessionData("LOGON_USER"))
            SessionData("APPLICATION_SECURITY_ACCESS") = GetApplicationSecurityAccess(SessionData("LOGON_USER"))

            'Set customer selection
            SessionData("CUSTOMERNUMBER") = "1011000"
            SessionData("SHIPTONUMBER") = ""
            SessionData("ALLSHIPTOS") = "TRUE"
            GetSelectedAccountContactInformation()

        End If

    End Sub

#End If

#End Region

End Class
