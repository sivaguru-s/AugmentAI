
'Public Class InvoiceCollection
'    Inherits System.Collections.CollectionBase
'    Implements IDisposable


'#Region " Attribute Declarations "

'    Private m_bDisposed As Boolean = False


'#End Region

'#Region " Property Declarations "

'    Public Overloads ReadOnly Property Item(ByVal index As Integer) As Invoice

'        Get
'            'Return CType(List.Item(index), Item)
'            Return DirectCast(List.Item(index), Invoice)
'        End Get

'    End Property

'    Public Overloads ReadOnly Property Item(ByVal invoiceNum As String) As Invoice

'        Get
'            Dim iIndex As Integer = 0
'            Dim bFound As Boolean = False

'            ' find the ordinal position in the collection of the object to remove 
'            'For iIndex = 0 To Me.Count - 1
'            For Each currentItem As Invoice In List

'                If currentItem.InvoiceNumber.Equals(invoiceNum) Then
'                    bFound = True
'                    Exit For
'                End If
'                iIndex += 1

'            Next

'            If bFound = True Then
'                Return DirectCast(List.Item(iIndex), Invoice)

'            Else
'                Return Nothing

'            End If

'        End Get

'    End Property



'#End Region

'#Region " Constructors Declarations "



'#End Region

'#Region " Private Methods "



'#End Region

'#Region " Public Methods "

'    Public Function AddItem(ByVal invoice As Invoice) As Boolean

'        Dim returnVal As Boolean = False

'        'See if item is already in collection so we don't create duplicates
'        If Me.Item(invoice.InvoiceNumber) Is Nothing Then
'            List.Add(invoice)
'            returnVal = True
'        End If

'        'return boolean if item as added or not
'        Return returnVal

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
'        Dim iIndex As Integer

'        If Not Me.m_bDisposed Then
'            If disposing Then

'                ' Free managed resources 
'                For iIndex = 0 To List.Count - 1
'                    'CType(List.Item(iIndex), Item).Dispose()
'                    DirectCast(List.Item(iIndex), Invoice).Dispose()
'                Next

'                ' clear the collection
'                Me.Clear()


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


