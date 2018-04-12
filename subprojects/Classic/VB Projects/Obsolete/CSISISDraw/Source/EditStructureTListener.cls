VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "EditStructureTListener"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

' ## This table listener responds to double-clicks on chemical structures and lets users edit or view them.

' ## This is a specialized version of the Core EditStructureTListener for ISIS/Draw integration.

Implements IENObject
Implements IENConfigurationObject
Implements IENFieldListener
Implements IENTableListener

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

' ## The key of the property whose change prompts a change in the target.
Private mDefaultCDX As String

Private Sub SetData(ByVal NewData As Variant, ByVal Format As String)

    Dim rootNode As IXMLDOMElement
    Dim dataNode As IXMLDOMNode
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
                    
                    For Each N In rootNode.childNodes
                        If (N.nodeType = NODE_CDATA_SECTION) Then
                            Set cDataNode = N
                            mDefaultCDX = cDataNode.Data
                            Exit For
                        End If
                    Next N
                End If
            End If
        Else
            Set rootNode = NewData
            mKey = rootNode.getAttribute("key")
            mName = rootNode.getAttribute("name")
            mProgID = rootNode.getAttribute("progID")
            mPosition = rootNode.getAttribute("position")
            
            For Each N In rootNode.childNodes
                If (N.nodeType = NODE_CDATA_SECTION) Then
                    Set cDataNode = N
                    mDefaultCDX = cDataNode.Data
                    Exit For
                End If
            Next N
            
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

        Set dataNode = rootNode.selectSingleNode("@connectionString")
        ' If (Not dataNode Is Nothing) Then ConnectionString = dataNode.nodeValue
        
        For Each N In rootNode.childNodes
            If (N.nodeType = NODE_CDATA_SECTION) Then
                Set cDataNode = N
                DefaultCDX = cDataNode.Data
                Exit For
            End If
        Next N
    End If
End Sub

Private Sub Class_Initialize()
    RecordInitialize (App.EXEName & "." & TypeName(Me))
End Sub

Private Sub Class_Terminate()
    Terminate
    RecordTerminate (App.EXEName & "." & TypeName(Me))
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
    Dim frm As New frmEditStructure
    
    frm.Initialize mConnection
    frm.Caption = IENFieldListener_Name & " Properties"
    If (Len(mDefaultCDX) > 0) Then
        frm.CDXStructure = mDefaultCDX
    End If
    frm.Show vbModal
    
    If (frm.OK) Then
        DefaultCDX = frm.CDXStructure
        
        If (Not mSaved) Then
            mField.SectionType.Collection.SaveConfigurationObject Me, "Saving changes to " & IENObject_Description
        End If
    End If
    
CatchError:
    ErrorMsgBox Err, "Error Setting " & IENFieldListener_Name & " Properties", mConnection
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
    IENObject_Description = "Table Listener " & mName & " of " & mField.ENObject.Description(Flags)
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
    
    SetData Contents, "xml"
    
    If (Not mSaved) Then
        mField.ConfigurationChanged Me
        
        With mConnection.ADOConnection
            Set nameDataDoc = New DOMDocument40
            Set rootNode = nameDataDoc.createElement("fieldListener")
            
            rootNode.appendChild(nameDataDoc.createCDATASection("defaultCDX")).Text = mDefaultCDX
            
            formToolConfigString = ", config_xml = " & .ParamSyntax(":configXML")
            .Execute "UPDATE ELN_field_listeners" & _
                     " SET name = " & .ConstantSyntax(mName) & _
                     ", prog_ID = " & .ConstantSyntax(mProgID) & _
                     ", position = " & .ConstantSyntax(mPosition) & _
                     formToolConfigString & _
                     " WHERE field_listener_key = " & mKey, ":configXML", rootNode.XML
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
    
    rootNode.appendChild(Src.createCDATASection("defaultCDX")).Text = mDefaultCDX
    
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

Private Sub IENTableListener_AfterChangeReference(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableCell As ENStandard9.TableCell)

End Sub

Private Sub IENTableListener_AfterEditValue(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableCell As ENStandard9.TableCell, ByVal newValue As Variant)

End Sub

Private Sub IENTableListener_AfterPropertyAdded(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty)

End Sub

Private Sub IENTableListener_AfterPropertyMoved(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty, ByVal Delta As Long)

End Sub

Private Sub IENTableListener_AfterPropertyRemoved(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty)

End Sub

Private Sub IENTableListener_AfterRowAdded(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableRow As ENStandard9.TableRow)

End Sub

Private Sub IENTableListener_AfterRowMoved(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableRow As ENStandard9.TableRow, ByVal Delta As Long)

End Sub

Private Sub IENTableListener_AfterRowRemoved(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal firstPosition As Long, ByVal lastPosition As Long)

End Sub

Private Sub IENTableListener_AfterSort(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty)

End Sub

Private Sub IENTableListener_BeforeChangeReference(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableCell As ENStandard9.TableCell, ByVal newValue As Variant, Cancel As Boolean)

End Sub

Private Sub IENTableListener_BeforeEditValue(ByVal FieldCtlContainer As IFieldCtlContainer, ByVal TableCell As ENStandard9.TableCell, Cancel As Boolean)

End Sub

Private Sub SetRowAttributes(ByVal frm As frmEditStructure, ByVal curRow As TableRow)
    ' ## Set the attributes of the row that identify the parent compound, salt and solvate.
    
    Dim parentID As Long
    Dim saltCodeID As Long
    Dim solvateCodeID As Long
    Dim parentCoefficient As Long
    Dim saltCodeCoefficient As Long
    Dim solvateCodeCoefficient As Long
    Dim g As ChemDrawControl9Ctl.Group
    
    If (frm.mCDXDrawing.Groups.Count = 0) Then
        parentID = 0
        parentCoefficient = 0
        saltCodeID = 0
        saltCodeCoefficient = 0
        solvateCodeID = 0
        solvateCodeCoefficient = 0
    Else
        Dim i As Long
        Dim parentFormula As String
        Dim saltFormula As String
        Dim solvateFormula As String
        Dim oldParentID As Long
        Dim oldSaltID As Long
        Dim oldSolvateID As Long
        
        parentCoefficient = 0
        saltCodeCoefficient = 0
        solvateCodeCoefficient = 0
        
        If (curRow.getAttribute("parentID") <> "") Then oldParentID = CLng(curRow.getAttribute("parentID"))
        If (curRow.getAttribute("saltID") <> "") Then oldSaltID = CLng(curRow.getAttribute("saltID"))
        If (curRow.getAttribute("solvateID") <> "") Then oldSolvateID = CLng(curRow.getAttribute("solvateID"))
        
        ' Run through the groups once looking for the old IDs.
        ' Set the coefficients to 0 so that they will be incremented properly, below.
        For i = 1 To frm.mCDXDrawing.Groups.Count
            Set g = frm.mCDXDrawing.Groups.Item(i)
            If (g.ID = oldParentID) Then
                parentID = oldParentID
                parentCoefficient = 0
                parentFormula = g.Objects.Formula
            ElseIf (g.ID = oldSaltID) Then
                saltCodeID = oldSaltID
                saltCodeCoefficient = 0
                saltFormula = g.Objects.Formula
            ElseIf (g.ID = oldSolvateID) Then
                solvateCodeID = g.ID
                solvateCodeCoefficient = 0
                solvateFormula = g.Objects.Formula
            End If
        Next i
        
        ' TODO: Instead of going through this group in order, do it left-to-right if necessary.
        For i = 1 To frm.mCDXDrawing.Groups.Count
            Set g = frm.mCDXDrawing.Groups.Item(i)
            If (g.Groups.Count > 0) Then
                ' Do nothing
            ElseIf (parentID = 0) Then
                parentID = g.ID
                parentCoefficient = 1
                parentFormula = g.Objects.Formula
            ElseIf (g.Objects.Formula = parentFormula) Then
                parentCoefficient = parentCoefficient + 1
            ElseIf (saltCodeID = 0) Then
                saltCodeID = g.ID
                saltCodeCoefficient = 1
                saltFormula = g.Objects.Formula
            ElseIf (g.Objects.Formula = saltFormula) Then
                saltCodeCoefficient = saltCodeCoefficient + 1
            ElseIf (solvateCodeID = 0) Then
                solvateCodeID = g.ID
                solvateCodeCoefficient = 1
                solvateFormula = g.Objects.Formula
            ElseIf (g.Objects.Formula = solvateFormula) Then
                solvateCodeCoefficient = solvateCodeCoefficient + 1
            End If
        Next i
    End If
    
    If (parentID = 0) Then
        curRow.setAttribute "parentID", ""
        curRow.setAttribute "parentCoefficient", ""
    Else
        curRow.setAttribute "parentID", CStr(parentID)
        curRow.setAttribute "parentCoefficient", CStr(parentCoefficient)
    End If
    
    If (saltCodeID = 0) Then
        curRow.setAttribute "saltID", ""
        curRow.setAttribute "saltCoefficient", ""
    Else
        curRow.setAttribute "saltID", CStr(saltCodeID)
        curRow.setAttribute "saltCoefficient", CStr(saltCodeCoefficient)
        curRow.setAttribute "saltCode", ""
    End If
    
    If (solvateCodeID = 0) Then
        curRow.setAttribute "solvateID", ""
        curRow.setAttribute "solvateCoefficient", ""
    Else
        curRow.setAttribute "solvateID", CStr(solvateCodeID)
        curRow.setAttribute "solvateCoefficient", CStr(solvateCodeCoefficient)
        curRow.setAttribute "solvateCode", ""
    End If
End Sub

Private Sub ClearCellEntered(ByVal CTableCtl As CTableCtl, ByVal tc As TableCell)
    If (tc.Tag = "entered") Then
        tc.Tag = ""
        tc.ForeColor = RGB(0, 0, 0)
        CTableCtl.TableCtl.SetupCellColors tc
    End If
End Sub

Private Function IENTableListener_EditValue(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableCell As ENStandard9.TableCell) As Boolean
    Dim f As frmEditStructure
    
    If TableCell.PropertyTypeName = "structure" Then
        Dim oldCDX() As Byte
        Dim structureIsEditable As Boolean
        Dim CTableCtl As CTableCtl
        Dim sc As SectionCell
        Dim tp As TableProperty
        
        oldCDX = TableCell.CDXStructure
        
        Set f = New frmEditStructure
        On Error GoTo CatchError
        f.Initialize FieldCtlContainer.Connection
        
        If (UBound(oldCDX) > 1) Then
            f.CDXStructure = oldCDX
        ElseIf (Len(mDefaultCDX) > 0) Then
            f.CDXStructure = mDefaultCDX
        End If
        
        Set sc = TableCell.TableRow.TableSection.SectionCell
        structureIsEditable = FieldCtlContainer.CanEditCell(sc) _
                And Not TableCell.TableRow.IsDeleted _
                And Not TableCell.TableProperty.IsDeleted _
                And Not TableCell.TableProperty.ELNProperty.IsReadOnly
        
        If (structureIsEditable) Then
            f.Caption = "Edit " & TableCell.TableProperty.Nickname
        Else
            f.Caption = "View " & TableCell.TableProperty.Nickname
        End If
        
        f.IsEditable = structureIsEditable
        
        ' ISIS/Draw Integration customization
        
        If structureIsEditable Then
            ' Launch ISIS/Draw for editing.
            Dim EditStructure As ENIsisDraw9.ISISDrawEdit
            Set EditStructure = New ISISDrawEdit
            Dim InputCDXML As String
            Dim OutputCDXML As String
            
            InputCDXML = f.CDXStructure
            OutputCDXML = EditStructure.ISISEdit(InputCDXML, True)  ' Let the user edit in ISIS/Draw.
            If (OutputCDXML <> InputCDXML) Then
                ' The structure has changed.
                ' Put tne modified structure in the table cell.
                Dim tVal As Variant
                
                tVal = OutputCDXML
                Set CTableCtl = FieldCtlContainer.Control(sc.Field).object.CTableCtl
                
                If (CTableCtl.TableCtl.TableCellValueChanging(TableCell, tVal)) Then
                
                    ' Set the row attributes before setting the table cell value so that the salt weight tlistener is set up with the right values.
                    SetRowAttributes f, TableCell.TableRow
                    
                    Set tp = TableCell.TableRow.TableSection.TablePropertyByName("Formula Mass")
                    If (Not tp Is Nothing) Then
                        ClearCellEntered CTableCtl, TableCell.TableRow.TableCell(tp)
                    End If
                    Set tp = TableCell.TableRow.TableSection.TablePropertyByName("Molecular Weight")
                    If (Not tp Is Nothing) Then
                        ClearCellEntered CTableCtl, TableCell.TableRow.TableCell(tp)
                    End If
                    
                    CTableCtl.TableCtl.MonitorSetTableCellValue TableCell, tVal, True, True
                    CTableCtl.TableCtl.SetBestPropertySpacing TableCell.TableProperty.Index
                    CTableCtl.TableCtl.SetBestRowSpacing TableCell.TableRow.Index
                    CTableCtl.TableCtl.CheckControlBounds
                End If
            End If
        Else
            f.Show vbModal
        End If
        
        Unload f
        IENTableListener_EditValue = True
    Else
        IENTableListener_EditValue = False
    End If
    
CatchError:
    Dim errNumber As Long
    Dim errDesc As String
    Dim errSource As String
    
    errNumber = Err.Number
    errDesc = Err.Description
    errSource = Err.Source
    
    If (Not f Is Nothing) Then
        Unload f
        Set f = Nothing
    End If
    If (errNumber <> 0) Then
        Err.Raise errNumber, errSource, errDesc
    End If
End Function

Private Property Get Field() As ENFramework9.Field
    Set Field = mField
End Property

Public Property Get DefaultCDX() As String
    ' This method is used by the reaction toolbar to get the default cdx for a table.
    DefaultCDX = mDefaultCDX
End Property

Private Property Let DefaultCDX(ByVal aData As String)
    If (mDefaultCDX <> aData) Then
        mDefaultCDX = aData
        mSaved = False
    End If
End Property

Private Sub IENFieldListener_ContentCached(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal CachedContents As Collection)

End Sub

Private Sub IENTableListener_BeforePropertyAdded(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ELNProperty As ELNProperty, Position As Long, Cancel As Boolean)

End Sub

Private Sub IENTableListener_BeforePropertyMoved(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty, ByVal Delta As Long, Cancel As Boolean)

End Sub

Private Sub IENTableListener_BeforePropertyRemoved(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty, Cancel As Boolean)

End Sub

Private Sub IENTableListener_BeforeRowAdded(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, Position As Long, Cancel As Boolean)

End Sub

Private Sub IENTableListener_BeforeRowMoved(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableRow As ENStandard9.TableRow, ByVal Delta As Long, Cancel As Boolean)

End Sub

Private Sub IENTableListener_BeforeRowRemoved(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal firstPosition As Long, ByVal lastPosition As Long, Cancel As Boolean)

End Sub

Private Sub IENTableListener_BeforeSort(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty, Cancel As Boolean)

End Sub

Private Property Get IENTableListener_CanAddProperty(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal Position As Long) As Boolean
    IENTableListener_CanAddProperty = True
End Property

Private Property Get IENTableListener_CanAddRow(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal Position As Long) As Boolean
    IENTableListener_CanAddRow = True
End Property

Private Property Get IENTableListener_CanChangeReference(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableCell As ENStandard9.TableCell) As Boolean
    IENTableListener_CanChangeReference = True
End Property

Private Property Get IENTableListener_CanEditValue(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableCell As ENStandard9.TableCell) As Boolean
    IENTableListener_CanEditValue = True
End Property

Private Property Get IENTableListener_CanMoveProperty(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty, ByVal Delta As Long) As Boolean
    IENTableListener_CanMoveProperty = True
End Property

Private Property Get IENTableListener_CanMoveRow(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableRow As ENStandard9.TableRow, ByVal Delta As Long) As Boolean
    IENTableListener_CanMoveRow = True
End Property

Private Property Get IENTableListener_CanRemoveProperty(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty) As Boolean
    IENTableListener_CanRemoveProperty = True
End Property

Private Property Get IENTableListener_CanRemoveRow(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal firstPosition As Long, ByVal lastPosition As Long) As Boolean
    IENTableListener_CanRemoveRow = True
End Property

Private Property Get IENTableListener_CanSort(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableProperty As ENStandard9.TableProperty) As Boolean
    IENTableListener_CanSort = True
End Property

Private Sub IENTableListener_CellChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal TableCell As ENStandard9.TableCell)

End Sub

Private Sub IENTableListener_Scroll(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENTableListener_SelChange(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENTableListener_UpdateHasAddPropertyUI(HasPropertyUI As Boolean)

End Sub

Private Sub IENTableListener_UpdateHasAddRowUI(HasRowUI As Boolean)

End Sub

Private Sub IENTableListener_UpdateHasMovePropertyUI(HasPropertyUI As Boolean)

End Sub

Private Sub IENTableListener_UpdateHasMoveRowUI(HasRowUI As Boolean)

End Sub

Private Sub IENTableListener_UpdateHasRemovePropertyUI(HasPropertyUI As Boolean)

End Sub

Private Sub IENTableListener_UpdateHasRemoveRowUI(HasRowUI As Boolean)

End Sub

Private Sub IENTableListener_UpdateHasSortUI(HasSortUI As Boolean)

End Sub

Private Sub IENTableListener_UpdatePluralRowName(PluralRowName As String)

End Sub

Private Sub IENTableListener_UpdateSingularRowName(SingularRowName As String)

End Sub

Private Sub IENTableListener_UpdateTableName(NewTableName As String)

End Sub

Private Sub ShowError(Optional ByVal ErrTitle As String = "ISIS/Draw editing Control Error")
    ' ## Helper function to display an error message with an appropriate title when an error occurs in this module.
    ' ##PARAM ErrTitle The title of the message box to be displayed.
    ' ##REMARKS Processing Logic: _
    <ol> _
    <li>Send the current error, the error title and the connection associated with the current section cell (if any) _
        to the ErrMsgBox routine to  display the message.</li> _
    <li>Reset the pointer for this control to the default pointer.</li> _
    <ol>
    
    ErrorMsgBox Err, ErrTitle, Nothing
End Sub

