VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ISISDrawCQListener"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Implements IENObject
Implements IENConfigurationObject
Implements ENFramework9.IENFieldListener
Implements ENStandard9.IENChemQueryListener
Implements IENFieldEditListener

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

' ## The cdxml representation of this chemical structure before it was changed.
Private mOldData As String

' ## Module flag to allow ChemDraw changes or not.
Private mAllowEditing As Boolean

Private Sub SetData(ByVal Format As String, ByVal Contents As Variant)
    Dim rootNode As IXMLDOMElement
    Dim dataNode As IXMLDOMNode
            
    Select Case Format
        Case "initialize"
            Set rootNode = Contents
            
            mKey = rootNode.getAttribute("key")
            mName = rootNode.getAttribute("name")
            mProgID = rootNode.getAttribute("progID")
            mPosition = rootNode.getAttribute("position")
            
            mSaved = True
        Case "xml"
            Set rootNode = Contents
            
            Set dataNode = rootNode.getAttributeNode("name")
            If (Not dataNode Is Nothing) Then IENFieldListener_Name = dataNode.nodeValue
            
            Set dataNode = rootNode.getAttributeNode("progID")
            If (Not dataNode Is Nothing) Then IENFieldListener_ProgID = dataNode.nodeValue
            
            Set dataNode = rootNode.getAttributeNode("position")
            If (Not dataNode Is Nothing) Then IENFieldListener_Position = CLng(dataNode.nodeValue)
    
    End Select
    
End Sub

Private Sub Class_Initialize()
    RecordInitialize (App.EXEName & "." & TypeName(Me))
    mAllowEditing = False  ' Set ChemDraw to VeiwOnly.
End Sub

Private Sub Class_Terminate()
    Terminate
    RecordTerminate (App.EXEName & "." & TypeName(Me))
End Sub

Private Sub IENChemQueryListener_AfterDataChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalQuery As ENStandard9.ChemicalQuery)

End Sub

Private Sub IENChemQueryListener_BeforeDataChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalQuery As ENStandard9.ChemicalQuery)
    
End Sub

Private Sub IENChemQueryListener_SelectionChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalQuery As ENStandard9.ChemicalQuery)

End Sub

Private Sub IENChemQueryListener_StatusChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalQuery As ENStandard9.ChemicalQuery)

End Sub

Private Sub IENChemQueryListener_ToolChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalQuery As ENStandard9.ChemicalQuery)

End Sub

' Make sure we're in ViewOnly mode.
Private Sub IENChemQueryListener_GotFocus(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ChemicalQuery As ENStandard9.ChemicalQuery)
' Fix bug CSBR-57083: ChemDraw is available the first time you right click.
'  ChemDrawCtl is now made ViewOnly by the code "IENFieldEditListener_CanEditCell = False", below.
'    Dim CCQCtl As ENStandardCtl9.CChemicalQueryCtl
'    Dim cqCtl As ENStandardCtl9.ChemicalQueryCtl
'
'    Set CCQCtl = FieldCtlContainer.Control(mField).object.CChemicalQueryCtl
'    CCQCtl.ChemicalQueryCtl.ChemDrawCtl.ViewOnly = True
End Sub

Private Sub IENChemQueryListener_DoubleClick( _
                ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, _
                ByVal ChemicalQuery As ENStandard9.ChemicalQuery)
    Dim CCQCtl As ENStandardCtl9.CChemicalQueryCtl
    Dim Structure As String
    Dim OutputCDXML As String
    Dim oldEncoded As Boolean
    Dim ErrorString As String
    Dim DX As Double, DY As Double
                
    If (Not (ChemicalQuery.SectionCell Is Nothing)) Then
        If ChemicalQuery.SectionCell.IsEditable Then
            ' Make sure we're in ViewOnly mode.
            Set CCQCtl = FieldCtlContainer.Control(mField).object.CChemicalQueryCtl
            With CCQCtl.ChemicalQueryCtl.ChemDrawCtl
                .ViewOnly = True
                Structure = .Data("text/xml") ' Get cdxml structure from ChemDraw.
                
                Dim EditStructure As ENIsisDraw9.ISISDrawEdit
                Set EditStructure = New ISISDrawEdit
                
                ErrorString = "Error editing in ISIS/Draw"
                OutputCDXML = EditStructure.ISISEdit(Structure, False) ' Let the user edit in ISIS/Draw.
                
                ErrorString = "Error loading data into ChemDraw after editing in ISIS/Draw"
                .Objects.Clear
                oldEncoded = .DataEncoded
                .DataEncoded = False
                If (Len(OutputCDXML) > 0) Then
                    .SourceURL = "data:chemical/x-cdx;base64," & OutputCDXML
                End If
                .DataEncoded = oldEncoded
                
                ' Recenter.
                ErrorString = "Error recentering after editing in ISIS/Draw"
                If (Not .RecenterWhenFitting) Then
                    DX = .Width / 40# - (.Objects.Left + .Objects.Width / 2#)
                    DY = .Height / 40# - (.Objects.Top + .Objects.Height / 2#)
                    DX = IIf((.Objects.Left + DX) < 0, 0, DX)
                    DY = IIf((.Objects.Top + DY) < 0, 0, DY)
                    Call .Objects.Move(DX, DY)
                End If

            End With
            
            ErrorString = "Error updating Rection section after editing in ISIS/Draw"
            mAllowEditing = True ' Permit changes to occur.
            CCQCtl.ChemicalQueryCtl.DataChanged
            CCQCtl.ChemicalQueryCtl.ENFieldCtl.Validate
            mAllowEditing = False ' Set ChemDraw back to VeiwOnly.
            
            Set EditStructure = Nothing
            Set CCQCtl = Nothing
        End If
    End If
    
CatchError:
    ShowError ErrorString
End Sub

Private Property Get IENFieldListener_ENObject() As ENFramework9.IENObject
    Set IENFieldListener_ENObject = Me
End Property

Private Property Get GetAttributesCommand() As DOMDocument
    Dim Src As MSXML2.DOMDocument
    Dim verbNode As IXMLDOMNode
    Dim mXMLS As New XMLScriptingObject
    
    Set Src = New MSXML2.DOMDocument
    Set verbNode = mXMLS.XMLCommand(Src, "getData")
    
    Dim ppNode As IXMLDOMNode
    
    Set ppNode = Src.createElement("tag")
    mField.ENObject.XMLAppendPath(verbNode).appendChild ppNode
    
    Set GetAttributesCommand = Src
End Property

Private Sub XMLLoadAttributes()
    Dim ResultDoc As MSXML2.DOMDocument
    Dim N As IXMLDOMNode
    
    Set ResultDoc = mConnection.DoCommands(GetAttributesCommand())
    Set N = ResultDoc.selectSingleNode("result/getData/fieldListener")
    
    SetData "initialize", N
End Sub

Private Sub IENFieldListener_Initialize(ByVal NewData As Variant, ByVal Field As ENFramework9.Field, ByVal conn As ENFramework9.Connection, ByVal FieldListeners As FieldListeners)
    Set mConnection = conn
    Set mField = Field
    Set mFieldListeners = FieldListeners
    
    If (mConnection.ConnectionType = kConnectionTypeADO) Then
        Dim toRS As ADODB.Recordset
        
        Set toRS = NewData
        With toRS
            mKey = !field_listener_key
            mName = !Name
            mProgID = !prog_id
            mPosition = !Position
        End With
        
        mSaved = True
    Else
        If (TypeName(NewData) = "IXMLDOMElement") Then
            SetData "initialize", NewData
        Else
            XMLLoadAttributes
        End If
    End If
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
    MsgBox "Field Listener """ & mProgID & """ has no properties."
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
' Fix bug CSBR-57083: ChemDraw is available the first time you right click.
'  ChemDrawCtl is now made ViewOnly by the code "IENFieldEditListener_CanEditCell = False", below.
'    Dim CCQCtl As ENStandardCtl9.CChemicalQueryCtl
'    Dim cqCtl As ENStandardCtl9.ChemicalQueryCtl
'
'    Set CCQCtl = FieldCtlContainer.Control(mField).object.CChemicalQueryCtl
'    CCQCtl.ChemicalQueryCtl.ChemDrawCtl.ViewOnly = True
End Sub

Private Sub IENFieldListener_Terminate()
    Terminate
End Sub

Private Property Get IENObject_Object() As Object
    Set IENObject_Object = Me
End Property

Private Property Get IENObject_Description(Optional ByVal Flags As Long = 0) As String
    IENObject_Description = "Chemical Query Listener """ & mName & """ of " & mField.ENObject.Description(Flags)
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
    SetData "xml", Contents
    
    If (Not mSaved) Then
        mField.ConfigurationChanged Me
        
        With mConnection.ADOConnection
            .Execute "UPDATE ELN_field_listeners" & _
                     " SET name = " & .ConstantSyntax(mName) & _
                     ", prog_ID = " & .ConstantSyntax(mProgID) & _
                     ", position = " & .ConstantSyntax(mPosition) & _
                     " WHERE field_listener_key = " & mKey
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

Private Property Get XMLAttributes(ByVal Src As MSXML2.DOMDocument, ByVal Depth As String) As IXMLDOMNode
    Dim rootNode As IXMLDOMElement
    
    Set rootNode = Src.createElement("fieldListener")
    If (Depth <> "export") Then
        rootNode.setAttribute "key", mKey
    End If
    rootNode.setAttribute "progID", mProgID
    rootNode.setAttribute "name", mName
    rootNode.setAttribute "position", mPosition
    
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

Private Sub IENFieldListener_ContentCached(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal CachedContents As Collection)

End Sub

Private Sub IENFieldEditListener_AfterContentChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCellObject As ENFramework9.IENSectionCellObject, ByVal Description As String)

End Sub

Private Sub IENFieldEditListener_AfterContentChanging(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCellObject As ENFramework9.IENSectionCellObject)

End Sub

Private Sub IENFieldEditListener_BeforeContentChanged(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCellObject As ENFramework9.IENSectionCellObject, ByVal Description As String, Cancel As Boolean)

End Sub

Private Sub IENFieldEditListener_BeforeContentChanging(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCellObject As ENFramework9.IENSectionCellObject, Cancel As Boolean)

End Sub

Private Function IENFieldEditListener_CanEditCell(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell) As Boolean
    If mAllowEditing Then
        IENFieldEditListener_CanEditCell = True
    Else
        IENFieldEditListener_CanEditCell = False
    End If
End Function

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


