''(c) Ashley Furniture Industries, Inc. 2010
''All Rights Reserved
''
'' Modification History
'' Author			Date		Description
'' TKiel             08/24/2010  Creation


'Public Class TermsCode
'    Implements IDisposable


'#Region " Attribute Declarations "

'    Private m_bDisposed As Boolean = False

'#End Region

'#Region " Property Declarations "


'#End Region

'#Region " Constructors Declarations "




'#End Region

'#Region " Private Methods "


'#End Region

'#Region " Public Methods "

'    Public Shared Function GetTermsCode() As System.Data.SqlClient.SqlDataReader

'        Dim sbSQL As New System.Text.StringBuilder

'        Try

'            sbSQL.Append("EXEC Ashley.dbo.usp_GetEpayTermsCodes")

'            System.Web.HttpContext.Current.Trace.Warn(sbSQL.ToString)

'            Return DataAccess.GetDataReader(Ashley.Data.DataAccess.SqlConnections.SQL_Dynamic, sbSQL.ToString)

'        Catch ex As Exception
'            Throw

'        Finally
'            sbSQL = Nothing

'        End Try
'    End Function

'#End Region

'#Region " Dispose Methods "

'    Public Overridable Overloads Sub Dispose() Implements IDisposable.Dispose

'        If Not m_bDisposed Then
'            ' Call the dispose method
'            Me.Dispose(True)

'            ' Tell the garbage collector that the object doesn't require cleanup
'            GC.SuppressFinalize(Me)
'        End If

'    End Sub

'    Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)


'        If Not Me.m_bDisposed Then
'            If disposing Then

'                ' Free managed resources              


'            End If

'            ' TODO: free shared unmanaged resources
'        End If
'        Me.m_bDisposed = True

'    End Sub

'    Protected Overrides Sub Finalize()
'        Me.Dispose(False)
'    End Sub

'#End Region



'End Class
