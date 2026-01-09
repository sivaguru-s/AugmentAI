Public Class EpayNav
    Inherits System.Web.UI.UserControl

    Public Property AppAuthorization As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.AppAuthorization Then

            liUserList.Visible = True
            liAdminMaintenance.Visible = True
            liReport.Visible = True

        Else

            liUserList.Visible = False
            liAdminMaintenance.Visible = False
            liReport.Visible = False

        End If


    End Sub

End Class