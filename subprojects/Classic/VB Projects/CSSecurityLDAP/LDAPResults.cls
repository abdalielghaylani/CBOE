VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "LDAPResults"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private xmlResults As DOMDocument
Private ldap As CLDAPProxy

Private Sub Class_Initialize()
    Set xmlResults = New DOMDocument
    xmlResults.async = False
    xmlResults.loadXML ("<results></results>")
End Sub

Friend Sub Initialize(ByRef ldapProxy As CLDAPProxy)
    Set ldap = ldapProxy
End Sub


Public Property Get XML() As DOMDocument
    Set XML = xmlResults
End Property

Public Property Get Count() As Integer
    Count = ldap.ResultCount
End Property

Public Sub addAttribute(attr As String, val As String)
    Dim testNode As IXMLDOMNode
    
    ' test if node already exists.. no duplicates
    Set testNode = xmlResults.selectSingleNode(attr)
    If testNode Is Nothing Then ' add the node
        ' add this node to variables.
        Dim newElem As IXMLDOMElement
        
        ' add this to the variables
        ' all attributes get added to variables.
        Set newElem = xmlResults.createElement(attr)
        xmlResults.childNodes.item(0).appendChild newElem
        xmlResults.childNodes.item(0).lastChild.Text = val
    End If
    Set testNode = Nothing
End Sub

Friend Sub addResults(attrs() As String, Results() As String)
    If (Count > 0) Then
        Dim i As Integer
        For i = 0 To UBound(attrs)
            Dim attrCnt As Long
            For attrCnt = 0 To Count - 1 Step 1
                Dim attrName As String
                Dim attrValue As String
                attrName = attrs(i)
                attrValue = Results(i, attrCnt)
                If (Not xmlResults.childNodes.item(0).selectSingleNode(attrName) Is Nothing) Then
                        xmlResults.childNodes.item(0).selectSingleNode(attrName).Text = attrValue
                End If
            Next attrCnt
        Next i
    End If
End Sub

Friend Sub replaceVar(ByRef item As String)
    Dim startPos As Long
    Dim endPos As Long
    Dim NodeName As String
    Dim value As String
    Dim node As IXMLDOMNode
    
    startPos = InStr(1, item, "%")
    Do While startPos > 0
        endPos = InStr(startPos + 1, item, "%")
        If endPos = 0 Then Exit Do
        
        NodeName = Mid(item, startPos + 1, endPos - startPos - 1)
        ' get the value of the variable
        Set node = xmlResults.selectSingleNode("results/" & NodeName)
        If Not node Is Nothing Then
            value = node.Text
            item = Mid(item, 1, startPos - 1) & value & Mid(item, endPos + 1, Len(item))
        Else
            'if variable not found
            startPos = endPos
            ' no replacement... look for the next
        End If
        Set node = Nothing
        startPos = InStr(startPos + 1, item, "%")
    Loop
    
End Sub

