VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "LDAPQuery"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private sDN As String
Private lTimeout As Long
Private ldScope As LDAPSearchScope
Private sFilter As String
Private attrs() As String
Private queryErrorNode As String
Private Results As LDAPResults

Friend Property Get DN() As String
    DN = sDN
End Property

Friend Property Get Timeout() As Long
    Timeout = lTimeout
End Property

Friend Property Get Scope() As LDAPSearchScope
    Scope = ldScope
End Property

Friend Property Get Filter() As String
    Filter = sFilter
End Property

Friend Property Get ErrorNode() As String
    ErrorNode = queryErrorNode
End Property

Friend Property Get ResultsClass() As LDAPResults
    Set ResultsClass = Results
End Property

Friend Property Get DefaultError() As String
    DefaultError = "Failed to find items specified in the filter."
End Property

Friend Property Get Attributes() As String()
    Attributes = attrs
End Property


Friend Sub Initialize(ByRef node As IXMLDOMNode, _
                        ByRef ldResults As LDAPResults)

    Const kScopeAttr = "scope"
    Const kAttrNode = "attr"
    Const kErrorAttr = "error"
    Const kTimeOutAttr = "timeout"
    Const kFilterNode = "filter"
    Const kDnNode = "dn"
    
    Const kSearchLevelBase = "base"
    Const kSearchLevelSingle = "onelevel"
    Const kSearchLevelSubTree = "subtree"
    
    Dim nodeList As IXMLDOMNodeList
    Dim currNode As IXMLDOMNode
    Dim queryNodeMap As IXMLDOMNamedNodeMap
    Dim attr As IXMLDOMAttribute
    Dim numAttrNodes As Integer
    
    Set Results = ldResults
    
    queryErrorNode = ""
    sFilter = ""
    lTimeout = 1
    numAttrNodes = 0
    
    Set queryNodeMap = node.Attributes
    For Each attr In queryNodeMap
        Select Case LCase(attr.Name)
            Case kTimeOutAttr
                lTimeout = CLng(attr.Text)
                If lTimeout < 0 Then
                    lTimeout = 1
                End If
            Case kScopeAttr
                Select Case LCase(attr.Text)
                    Case kSearchLevelBase
                        ldScope = scopeBase
                    Case kSearchLevelSingle
                        ldScope = scopeOneLevel
                    Case kSearchLevelSubTree
                        ldScope = scopeSubtree
                    Case Else
                        ldScope = scopeOneLevel  ' default
                End Select
            Case kErrorAttr
                queryErrorNode = attr.Text
            Case Else
                ' nothing
        End Select
    Next attr
    Set queryNodeMap = Nothing
    
    Set nodeList = node.childNodes
    For Each currNode In nodeList
        Select Case LCase(currNode.NodeName)
            Case kFilterNode
                If sFilter = "" Then
                    sFilter = currNode.Text
                    ldResults.replaceVar item:=sFilter
                End If
            Case kDnNode
                sDN = currNode.Text
            Case kAttrNode
                ldResults.addAttribute attr:=currNode.Text, val:=""
                ReDim Preserve attrs(numAttrNodes)
                attrs(numAttrNodes) = currNode.Text
                numAttrNodes = numAttrNodes + 1
            Case Else
                ' nothing
        End Select
    Next currNode
    
End Sub

Friend Sub RunQuery(ByRef ldap As CLDAPProxy)
    Results.addResults attrs:=attrs, _
                Results:=ldap.Search(DN, Scope, Filter, Attributes, Timeout)
End Sub
