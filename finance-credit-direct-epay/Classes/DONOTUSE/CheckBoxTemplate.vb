'Public Class CheckBoxTemplate
'    Implements ITemplate

'    Shared itemcount As Integer = 0
'    Dim TemplateType As ListItemType

'    Sub New(ByVal type As ListItemType)
'        TemplateType = type
'    End Sub

'    Sub InstantiateIn(ByVal container As Control) _
'       Implements ITemplate.InstantiateIn
'        Dim lc As New Literal()
'        Select Case TemplateType
'            Case ListItemType.Header
'                lc.Text = "<TABLE border=1><TR><TH>Items</TH></TR>"
'            Case ListItemType.Item
'                lc.Text = "<TR><TD>Item number: " & itemcount.ToString _
'                   & "</TD></TR>"
'            Case ListItemType.AlternatingItem
'                lc.Text = "<TR><TD bgcolor=lightblue>Item number: " _
'                   & itemcount.ToString & "</TD></TR>"
'            Case ListItemType.Footer
'                lc.Text = "</TABLE>"
'        End Select
'        container.Controls.Add(lc)
'        itemcount += 1
'    End Sub


'End Class
