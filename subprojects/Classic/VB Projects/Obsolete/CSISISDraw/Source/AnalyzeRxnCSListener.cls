VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "AnalyzeRxnCSListener"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Implements IENObject
Implements IENConfigurationObject
Implements ENFramework9.IENFieldListener
Implements ENStandard9.IENChemStructureListener


' ## The object used to communicate between the client tier, the business tier and the database tier.
Private mConnection As ENFramework9.Connection

' ## The field that contains this field listener.
Private mField As ENFramework9.Field

' ## The collection of field listeners that contains this field listener.
Private mFieldListeners As ENFramework9.FieldListeners

' ## True if the contents of this object have been saved to the database.
Private mSaved As Boolean

' ## The unique identifier for this field listener.
Private mKey As Long

' ## The human readable name for this field listener.
Private mName As String

' ## A COM ProgID that specifies the implementation of IENFieldListener associated with this object.
Private mProgID As String

' ## The position of this field listener within its set of field listeners.
Private mPosition As Long

' ## The key of the field containing the reactants table.
Private mReactantsFieldKey As Long

' ## The name of the field containing the reactants table.
Private mReactantsFieldName As String

' ## The property whose change prompts a change in the target.
Private mReactantsField As ENFramework9.Field

' ## The key of the field containing the reactants table.
Private mProductsFieldKey As Long

' ## The name of the field containing the reactants table.
Private mProductsFieldName As String

' ## The property whose change prompts a change in the target.
Private mProductsField As ENFramework9.Field

' ## The cdxml representation of this chemical structure before it was changed.
Private mOldData As String


Private Sub SetData(ByVal NewData As Variant, ByVal Format As String)

    Dim rootNode As IXMLDOMElement
    Dim dataNode As IXMLDOMNode
    Dim foundOne As Boolean
    Dim N As IXMLDOMNode
    Dim cDataNode As IXMLDOMCDATASection
    Dim formToolConfigString As String
    
    If (Format = "initialize") Then
        If (LCase(TypeName(NewData)) = "recordset") Then
            Dim toRS As ADODB.Recordset
            Set toRS = NewData
            
            mKey = toRS!field_listener_key
            mName = toRS!Name
            mProgID = toRS!prog_id
            mPosition = toRS!Position
            
            formToolConfigString = NonNull(toRS!config_xml)
            If (Len(formToolConfigString) > 0) Then
                Dim nameDataDoc As DOMDocument
                
                Set nameDataDoc = New DOMDocument
                If (nameDataDoc.loadXML(formToolConfigString)) Then
                    ' Extract names from the XML Config file.
                    Set rootNode = nameDataDoc.documentElement
                    
                    Set dataNode = rootNode.selectSingleNode("reactantsField/@name")
                    If (Not dataNode Is Nothing) Then
                        mReactantsFieldName = dataNode.nodeValue
                        mReactantsFieldKey = 0
                    Else
                        Set dataNode = rootNode.selectSingleNode("reactantsField/@key")
                        If (Not dataNode Is Nothing) Then mReactantsFieldKey = dataNode.nodeValue
                    End If
                    
                    Set dataNode = rootNode.selectSingleNode("productsField/@name")
                    If (Not dataNode Is Nothing) Then
                        mProductsFieldName = dataNode.nodeValue
                        mProductsFieldKey = 0
                    Else
                        Set dataNode = rootNode.selectSingleNode("productsField/@key")
                        If (Not dataNode Is Nothing) Then mProductsFieldKey = dataNode.nodeValue
                    End If
                End If
            End If
        Else
            Set rootNode = NewData
            mKey = rootNode.getAttribute("key")
            mName = rootNode.getAttribute("name")
            mProgID = rootNode.getAttribute("progID")
            mPosition = rootNode.getAttribute("position")
            
            Set dataNode = rootNode.selectSingleNode("reactantsField/@name")
            If (Not dataNode Is Nothing) Then
                mReactantsFieldName = dataNode.nodeValue
                mReactantsFieldKey = 0
            Else
                Set dataNode = rootNode.selectSingleNode("reactantsField/@key")
                If (Not dataNode Is Nothing) Then
                    mReactantsFieldKey = CLng(dataNode.nodeValue)
                Else
                    mReactantsFieldKey = 0
                End If
            End If
            
            Set dataNode = rootNode.selectSingleNode("productsField/@name")
            If (Not dataNode Is Nothing) Then
                mProductsFieldName = dataNode.nodeValue
                mProductsFieldKey = 0
            Else
                Set dataNode = rootNode.selectSingleNode("productsField/@key")
                If (Not dataNode Is Nothing) Then
                    mProductsFieldKey = CLng(dataNode.nodeValue)
                Else
                    mProductsFieldKey = 0
                End If
            End If
            
        End If
        
        mSaved = True
    ElseIf (Format = "xml") Then
        Set rootNode = NewData
        
        Set dataNode = rootNode.getAttributeNode("name")
        If (Not dataNode Is Nothing) Then IENFieldListener_Name = dataNode.nodeValue
        
        Set dataNode = rootNode.getAttributeNode("progID")
        If (Not dataNode Is Nothing) Then IENFieldListener_ProgID = dataNode.nodeValue
        
        Set dataNode = rootNode.getAttributeNode("position")
        If (Not dataNode Is Nothing) Then IENFieldListener_Position = CLng(dataNode.nodeValue)

        Set dataNode = rootNode.selectSingleNode("reactantsField/@name")
        If (Not dataNode Is Nothing) Then
            mReactantsFieldName = dataNode.nodeValue
            ReactantsFieldKey = 0
            mSaved = False
        Else
            Set dataNode = rootNode.selectSingleNode("reactantsField/@key")
            If (Not dataNode Is Nothing) Then
                ReactantsFieldKey = CLng(dataNode.nodeValue)
            Else
                ReactantsFieldKey = 0
            End If
        End If
        
        Set dataNode = rootNode.selectSingleNode("productsField/@name")
        If (Not dataNode Is Nothing) Then
            mProductsFieldName = dataNode.nodeValue
            ProductsFieldKey = 0
            mSaved = False
        Else
            Set dataNode = rootNode.selectSingleNode("productsField/@key")
            If (Not dataNode Is Nothing) Then
                ProductsFieldKey = CLng(dataNode.nodeValue)
            Else
                ProductsFieldKey = 0
            End If
        End If
    End If
End Sub

Private Sub Class_Initialize()
    RecordInitialize (App.EXEName & "." & TypeName(Me))
End Sub

Private Sub Class_Terminate()
    Terminate
    RecordTerminate (App.EXEName & "." & TypeName(Me))
End Sub

Private Sub IENChemStructureListener_AfterDataChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalStructure As ENStandard9.ChemicalStructure)
    Dim sc As ENIsisDraw9.StoicCalculator
    Dim ccsCtl As ENStandardCtl9.CChemicalStructureCtl
    Dim NewData As String
    
    Set sc = New ENIsisDraw9.StoicCalculator
    
    If (ReactantsField Is Nothing Or ProductsField Is Nothing) Then
        Exit Sub
    End If
    
    Set ccsCtl = FieldCtlContainer.Control(mField).object.CChemicalStructureCtl
    sc.Initialize ccsCtl, FieldCtlContainer.Control(ReactantsField).object.CTableCtl, FieldCtlContainer.Control(ProductsField).object.CTableCtl, Nothing
    
    NewData = ccsCtl.ChemicalStructureCtl.Data("text/xml") ' we need this to get Data Sgroup Data
    ' Workaround to type mismatch:
    sc.AnalyzeComponents ccsCtl.ChemicalStructureCtl, mOldData, NewData
    ' sc.AnalyzeComponents ccsCtl.ChemicalStructureCtl.ExtractComponents, mOldData, NewData
End Sub

Private Sub IENChemStructureListener_BeforeDataChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalStructure As ENStandard9.ChemicalStructure)
    
    If (ReactantsField Is Nothing Or ProductsField Is Nothing) Then
        Exit Sub
    End If
    
    Dim Tag As ENStandard9.ChemicalStructure
    Set Tag = FieldCtlContainer.SelectedSection.SectionCell(mField).Tag
    mOldData = Tag.CDX
End Sub

Private Sub IENChemStructureListener_SelectionChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalStructure As ENStandard9.ChemicalStructure)

End Sub

Private Sub IENChemStructureListener_StatusChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalStructure As ENStandard9.ChemicalStructure)

End Sub

Private Sub IENChemStructureListener_ToolChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalStructure As ENStandard9.ChemicalStructure)

End Sub

Private Sub IENChemStructureListener_GotFocus(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalStructure As ENStandard9.ChemicalStructure)

End Sub

Private Sub IENChemStructureListener_DoubleClick(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalStructure As ENStandard9.ChemicalStructure)

End Sub

Private Property Get IENFieldListener_ENObject() As ENFramework9.IENObject
    Set IENFieldListener_ENObject = Me
End Property

Private Sub IENFieldListener_Initialize(ByVal NewData As Variant, ByVal Field As ENFramework9.Field, ByVal Connection As ENFramework9.Connection, ByVal FieldListeners As FieldListeners)
    Set mConnection = Connection
    Set mField = Field
    Set mFieldListeners = FieldListeners
    
    SetData NewData, "initialize"
End Sub

Private Sub IENFieldListener_InitializeData(ByVal Field As ENFramework9.Field, ByVal Connection As ENFramework9.Connection, ByVal Name As String, ByVal ProgID As String, ByVal Position As Long)
    With Connection.ADOConnection
        .Execute ("INSERT INTO ELN_field_listeners(" & .UniqueKeyFieldname("field_listener_key") & _
                 "name, prog_ID, field_key, position)" & _
                 "VALUES(" & _
                 .UniqueKeyValue("ENS_field_listeners") & _
                 .ConstantSyntax(Name) & ", " & _
                 .ConstantSyntax(ProgID) & ", " & _
                 Field.Key & ", " & _
                 Position & ")")
'##CRUD INSERT ELN_field_listeners
    End With
End Sub

Private Property Get IENFieldListener_Key() As Long
    IENFieldListener_Key = mKey
End Property

Private Property Let IENFieldListener_Key(ByVal aData As Long)
    mKey = aData
End Property

Private Property Let IENFieldListener_Name(ByVal RHS As String)
    If (RHS <> mName) Then
        mName = RHS
        mSaved = False
    End If
End Property

Private Property Get IENFieldListener_Name() As String
    IENFieldListener_Name = mName
End Property

Private Property Get IENFieldListener_Object() As Object
    Set IENFieldListener_Object = Me
End Property

Private Property Let IENFieldListener_Position(ByVal RHS As Integer)
    If (RHS <> mPosition) Then
        mPosition = RHS
        mSaved = False
    End If
End Property

Private Property Get IENFieldListener_Position() As Integer
    IENFieldListener_Position = mPosition
End Property

Private Property Let IENFieldListener_ProgID(ByVal RHS As String)
    If (mProgID <> RHS) Then
        mProgID = RHS
        mSaved = False
    End If
End Property

Private Property Get IENFieldListener_ProgID() As String
    IENFieldListener_ProgID = mProgID
End Property

Private Sub IENFieldListener_Properties()
    Dim frm As New frmAnalyzeReactionListener
    
    CheckReactantsField
    CheckProductsField
    frm.Initialize Me, mField.SectionType, ReactantsFieldKey, ProductsFieldKey, mConnection
    frm.Show vbModal
    
    If (frm.IsOK) Then
        ReactantsFieldKey = frm.NewReactantsFieldKey
        ProductsFieldKey = frm.NewProductsFieldKey
        
        If (Not mSaved) Then
            mField.SectionType.Collection.SaveConfigurationObject Me, "Saving changes to " & IENObject_Description
        End If
    End If
    
CatchError:
    ErrorMsgBox Err, "Error Setting Property Listener Properties", mConnection
    Unload frm
End Sub

Private Sub IENFieldListener_Save()
    mField.SectionType.Collection.SaveConfigurationObject Me, "Saving changes to " & IENObject_Description
End Sub

Private Sub Terminate()
    Set mConnection = Nothing
    Set mField = Nothing
    Set mFieldListeners = Nothing
End Sub

Private Sub IENFieldListener_EnterFocus(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_ExitFocus(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_Hide(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_Resize(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_Show(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_Terminate()
    Terminate
End Sub

Private Property Get IENObject_Object() As Object
    Set IENObject_Object = Me
End Property

Private Property Get IENObject_Description(Optional ByVal Flags As Long = 0) As String
    IENObject_Description = "Chemical Structure Listener """ & mName & """ of " & mField.ENObject.Description(Flags)
End Property

Private Sub IENConfigurationObject_AppendSaveCommand(ByVal Src As MSXML2.IXMLDOMDocument2)
    Dim XMLS As New XMLScriptingObject
    
    If (Not mSaved) Then
        XMLS.AppendSaveCommand Me, Src
    End If
End Sub

Private Property Get IENConfigurationObject_ENObject() As IENObject
    Set IENConfigurationObject_ENObject = Me
End Property

Private Sub IENConfigurationObject_SaveCompleted(ByVal ResultParent As IXMLDOMElement)
    mSaved = True
End Sub

Private Sub IENObject_Save(ByVal Contents As IXMLDOMElement, ByVal ResultParent As IXMLDOMElement)
     ' Saves the data in the object to the database
    Dim nameDataDoc As DOMDocument
    Dim rootNode As IXMLDOMElement
    Dim formToolConfigString As String
    Dim dataNode As IXMLDOMElement
    
    SetData Contents, "xml"
    
    If (Not mSaved) Then
        mField.ConfigurationChanged Me
        
        With mConnection.ADOConnection
            Set nameDataDoc = New DOMDocument40
            Set rootNode = nameDataDoc.createElement("fieldListener")
            
            If (mReactantsFieldKey = 0 And Len(mReactantsFieldName) > 0) Then
                Set dataNode = rootNode.appendChild(nameDataDoc.createElement("reactantsField"))
                dataNode.setAttribute "name", mReactantsFieldName
            ElseIf (mReactantsFieldKey <> 0) Then
                Set dataNode = rootNode.appendChild(nameDataDoc.createElement("reactantsField"))
                dataNode.setAttribute "key", mReactantsFieldKey
            End If
            
            If (mProductsFieldKey = 0 And Len(mProductsFieldName) > 0) Then
                Set dataNode = rootNode.appendChild(nameDataDoc.createElement("productsField"))
                dataNode.setAttribute "name", mProductsFieldName
            ElseIf (mProductsFieldKey <> 0) Then
                Set dataNode = rootNode.appendChild(nameDataDoc.createElement("productsField"))
                dataNode.setAttribute "key", mProductsFieldKey
            End If
            
            If (rootNode.childNodes.length > 0) Then
                formToolConfigString = ", config_xml = " & .ConstantSyntax(rootNode.XML)
            Else
                formToolConfigString = ", config_xml = NULL"
            End If
            
            .Execute "UPDATE ELN_field_listeners" & _
                     " SET name = " & .ConstantSyntax(mName) & _
                     ", prog_ID = " & .ConstantSyntax(mProgID) & _
                     ", position = " & .ConstantSyntax(mPosition) & _
                     formToolConfigString & _
                     " WHERE field_listener_key = " & mKey
        
            Dim com As ADODB.Command
            
            Set com = .CreateCommand
            com.CommandType = adCmdStoredProc
            
            com.Parameters.Append com.CreateParameter(":fKey", adInteger, adParamInput, Value:=mKey)
            
            com.CommandText = .AuditSchemaPrefix & "audit_field_listener"
            .ExecuteCommand com
        End With
        
        mSaved = True
    End If
End Sub

Private Sub IENObject_XMLAppendData(ByVal resultNode As MSXML2.IXMLDOMElement, ByVal Depth As String)
    ' Append the data of this object to the resultNode.
    resultNode.appendChild XMLAttributes(resultNode.ownerDocument, Depth)
End Sub

Private Function IENObject_XMLAppendPath(ByVal rootNode As IXMLDOMElement, Optional ByVal isPersistant As Boolean = False) As MSXML2.IXMLDOMElement
    Dim resultNode As IXMLDOMElement
    
    Set resultNode = rootNode.ownerDocument.createElement("fieldListener")
    resultNode.setAttribute "key", CStr(IENFieldListener_Key)
    mField.ENObject.XMLAppendPath(rootNode, isPersistant).appendChild resultNode
    
    Set IENObject_XMLAppendPath = resultNode
End Function

Private Property Get XMLAttributes(ByVal Src As MSXML2.IXMLDOMDocument2, ByVal Depth As String) As MSXML2.IXMLDOMNode
    Dim rootNode As IXMLDOMElement
    Dim dataNode As IXMLDOMElement
    
    Set rootNode = Src.createElement("fieldListener")
    If (Depth <> "export") Then
        rootNode.setAttribute "key", mKey
    End If
    rootNode.setAttribute "progID", mProgID
    rootNode.setAttribute "name", mName
    rootNode.setAttribute "position", mPosition
    
    If (Depth = "export") Then
        If (Not ReactantsField Is Nothing) Then
            Set dataNode = rootNode.appendChild(Src.createElement("reactantsField"))
            dataNode.setAttribute "name", ReactantsField.Name
        End If
        If (Not ProductsField Is Nothing) Then
            Set dataNode = rootNode.appendChild(Src.createElement("productsField"))
            dataNode.setAttribute "name", ProductsField.Name
        End If
    Else
        If (Not ReactantsField Is Nothing) Then
            Set dataNode = rootNode.appendChild(Src.createElement("reactantsField"))
            dataNode.setAttribute "key", ReactantsField.Key
        End If
        If (Not ProductsField Is Nothing) Then
            Set dataNode = rootNode.appendChild(Src.createElement("productsField"))
            dataNode.setAttribute "key", ProductsField.Key
        End If
    End If
    
    Set XMLAttributes = rootNode
End Property

Private Function IENObject_XMLCount(ByVal ResultDoc As MSXML2.IXMLDOMDocument2, ByVal toType As String) As MSXML2.IXMLDOMElement
    ' Return the number of the specified subItem.
    Err.Raise vbObjectError + 513, Description:="the type """ & toType & """ cannot be resolved from " & IENObject_Description
End Function

Private Function IENObject_XMLCreate(ByVal NewType As String, ByVal NewRelation As String, ByVal NewData As MSXML2.IXMLDOMElement) As ENFramework9.IENObject

End Function

Private Sub IENObject_XMLDelete()
    mFieldListeners.Remove CStr(IENFieldListener_Key)
End Sub

Private Sub IENObject_XMLDeleteCollection(ByVal col As Collection)

End Sub

Private Function IENObject_XMLDoCommand(ByVal verbNode As MSXML2.IXMLDOMNode, ByVal ResultDoc As MSXML2.IXMLDOMDocument2) As MSXML2.IXMLDOMNode
    Select Case verbNode.nodeName
        Case Else
            Err.Raise vbObjectError + 513, Description:="the command """ & verbNode.nodeName & """ is not recognized for this table"
    End Select
End Function

Private Function IENObject_XMLDuplicate(Src As ENFramework9.IENObject, ByVal NewRelation As String, ByVal NewData As MSXML2.IXMLDOMElement) As ENFramework9.IENObject

End Function

Private Sub IENObject_XMLMove(Src As ENFramework9.IENObject, ByVal NewRelation As String)

End Sub

Private Function IENObject_XMLResolve(ByVal toType As String, ByVal whoseNode As MSXML2.IXMLDOMNode) As ENFramework9.IENObject
    ' Resolve the specified node into the specified type.
    Err.Raise vbObjectError + 513, Description:="the type """ & toType & """ cannot be resolved from " & IENObject_Description
End Function


Private Property Get Field() As ENFramework9.Field
    Set Field = mField
End Property

Private Property Get ReactantsFieldKey() As Long
    ReactantsFieldKey = mReactantsFieldKey
End Property

Private Property Let ReactantsFieldKey(ByVal aData As Long)
    If (mReactantsFieldKey <> aData) Then
        mReactantsFieldKey = aData
        Set mReactantsField = Nothing
        mReactantsFieldName = ""
        mSaved = False
    End If
End Property

Private Sub CheckReactantsField()
    If (mReactantsField Is Nothing) Then
        If (mReactantsFieldKey <> 0) Then
            Set mReactantsField = mField.SectionType.Fields(CStr(mReactantsFieldKey))
        ElseIf (Len(mReactantsFieldName) <> 0) Then
            Set mReactantsField = mField.SectionType.FieldByName(mReactantsFieldName)
            mReactantsFieldKey = mReactantsField.Key
        End If
    End If
End Sub

Private Property Get ReactantsField() As ENFramework9.Field
    CheckReactantsField
    Set ReactantsField = mReactantsField
End Property

Private Property Get ProductsFieldKey() As Long
    ProductsFieldKey = mProductsFieldKey
End Property

Private Property Let ProductsFieldKey(ByVal aData As Long)
    If (mProductsFieldKey <> aData) Then
        mProductsFieldKey = aData
        Set mProductsField = Nothing
        mProductsFieldName = ""
        mSaved = False
    End If
End Property

Private Sub CheckProductsField()
    If (mProductsField Is Nothing) Then
        If (mProductsFieldKey <> 0) Then
            Set mProductsField = mField.SectionType.Fields(CStr(mProductsFieldKey))
        ElseIf (Len(mProductsFieldName) <> 0) Then
            Set mProductsField = mField.SectionType.FieldByName(mProductsFieldName)
            mProductsFieldKey = mProductsField.Key
        End If
    End If
End Sub

Private Property Get ProductsField() As ENFramework9.Field
    CheckProductsField
    Set ProductsField = mProductsField
End Property

Private Sub IENFieldListener_ContentCached(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal CachedContents As Collection)

End Sub

