VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "AddSaltGListener"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

' ##MODULE_SUMMARY An implementation of IENFieldListener to perform specialized functions on a table containing the _
reactants of a stoichiometry grid with a section cell.

' ##IGNORE_REFERENCES mConnection, mField

Implements ENFramework9.IENObject
Implements IENConfigurationObject
Implements ENFramework9.IENFieldListener
Implements ENFramework9.IENCommand
Implements ENStandard9.IENGridListener

Private Const kNameRow = 0
Private Const kFormulaRow = 1
Private Const kMWRow = 2
Private Const kFormulaMassRow = 3
Private Const kStructureRow = 4
Private Const kMaxRow = 4

Private Const MIME_XML = "text/xml"
Private Const INTERSTRUCTURE_DIST As Long = 20

Private mConnection As ENFramework9.Connection
Private mField As ENFramework9.Field
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

' ## True if names are computed from structures.
Private mComputeName As Boolean

' ## True if users can add salt codes from within the identify salt and solvate dialog box
Private mNewSaltCode As Boolean

' ## True if users can add solvate codes from within the identify salt and solvate dialog box
Private mNewSolvateCode As Boolean

Private mSaltCodeSecs As Scripting.Dictionary
Private mSolvateCodeSecs As Scripting.Dictionary

Private mfrmEditStructure As frmEditStructure

Public Sub Terminate()
    ' This method should be private when the interface changes.
    Set mConnection = Nothing
    Set mField = Nothing
    Set mFieldListeners = Nothing
    Set mSaltCodeSecs = Nothing
    Set mSolvateCodeSecs = Nothing
    
    If (Not mfrmEditStructure Is Nothing) Then
        Unload mfrmEditStructure
        Set mfrmEditStructure = Nothing
    End If
End Sub

Private Sub Class_Initialize()
    RecordInitialize (App.EXEName & "." & TypeName(Me))
End Sub

Private Sub Class_Terminate()
    Terminate
    RecordTerminate (App.EXEName & "." & TypeName(Me))
End Sub

Private Sub ClearCellEntered(ByVal CTableCtl As IENGridCtl, ByVal tc As IENGridCell)
    If (tc.Tag = "entered") Then
        tc.Tag = ""
        tc.ForeColor = RGB(0, 0, 0)
        CTableCtl.SetupCellColors tc
    End If
End Sub

' Workaround type mismatch.
Private Function GetGroupByID(ChemDrawCtl As Object, ByVal GroupID As Long) As ChemDrawControl9Ctl.Group
'Private Function GetGroupByID(ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl, ByVal GroupID As Long) As ChemDrawControl9Ctl.Group
    Dim i As Long
    For i = 1 To ChemDrawCtl.Groups.Count
        If (ChemDrawCtl.Groups(i).ID = GroupID) Then
            Set GetGroupByID = ChemDrawCtl.Groups(i)
            Exit Function
        End If
    Next i
End Function

Private Function GetBounds(oRect As ChemDrawControl9Ctl.RECT, oObject As Object) As ChemDrawControl9Ctl.RECT
    oRect.Left = oObject.Left
    oRect.Top = oObject.Top
    oRect.Width = oObject.Width
    oRect.Height = oObject.Height
    Set GetBounds = oRect
End Function

Private Function GetCDXML(ByVal Section As ENFramework9.Section) As String
    ' ## Return the CDXML string for the chemical structure in the specified section.
    Dim i As Long
    Dim sc As SectionCell
    Dim cs As ENStandard9.ChemicalStructure
    
    For i = 1 To Section.SectionType.Fields.Count
        Set sc = Section.SectionCell(Section.SectionType.Fields(i))
        If (TypeOf sc.Tag Is ChemicalStructure) Then
            Set cs = sc.Tag
            GetCDXML = cs.CDX
            Exit Function
        End If
    Next i
End Function

Private Function GetGroupIsParent(ByVal g As ChemDrawControl9Ctl.Group) As Boolean
    Dim ot As ObjectTag
    
    Set ot = g.GetObjectTag("parent")
    GetGroupIsParent = (Not ot Is Nothing)
End Function

Private Sub SetGroupIsParent(ByVal g As ChemDrawControl9Ctl.Group, ByVal RHS As Boolean)
' Disable this code, as Object tags do not survive round-trip edits to ISIS/Draw.
'    Dim ot As ObjectTag
'
'    Set ot = g.GetObjectTag("parent")
'    If (RHS) Then
'        If (Not ot Is Nothing) Then
'            ' This is already marked as the parent.
'            Exit Sub
'        End If
'
'        g.MakeObjectTag "parent", ot, False
'        ot.Visible = False
'    Else
'        If (Not ot Is Nothing) Then
'            ot.Delete
'        End If
'    End If
End Sub

' Workaround type mismatch.
Private Function CombineStructures(ChemDrawCtl As Object, ByVal oldXML As String, ByVal saltXML As String, ByVal saltCoefficient As String) As String
'Private Function CombineStructures(ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl, ByVal oldXML As String, ByVal saltXML As String, ByVal saltCoefficient As string) As String
    Dim frm As frmEditStructure
    Dim aGroups As Scripting.Dictionary
    Dim g As ChemDrawControl9Ctl.Group
    Dim minLeft As Double
    Dim minTop As Double
    Dim maxBottom As Double
    Dim hasParentTag As Boolean
    Dim newParentGroup As ChemDrawControl9Ctl.Group
    
    Set frm = New frmEditStructure
    On Error GoTo CatchError
    
    ' Make sure that the salt XML has the same settings as the old XML
    frm.CDXStructure = saltXML
    frm.mCDXDrawing.ApplySettings "data:chemical/x-cdx;base64", ChemDrawCtl.Data(MIME_XML)
    saltXML = frm.CDXStructure
    
    frm.CDXStructure = oldXML
    If (frm.mCDXDrawing.Groups.Count = 1) Then
        If (frm.mCDXDrawing.Groups(1).GroupType = kCDGroupTypeGroup) Then
            ' Eliminate the top level group
            'Debug.Print "Eliminate the top level group"
        End If
    End If
    
    hasParentTag = False
    Set newParentGroup = Nothing
    ' Create a dictionary of all top-level groups
    Set aGroups = New Scripting.Dictionary
    For Each g In frm.mCDXDrawing.Groups
        If (g.Group.ID = 0) Then
            aGroups.Add g.ID, g.ID
            If (GetGroupIsParent(g)) Then
                hasParentTag = True
            ElseIf (newParentGroup Is Nothing) Then
                Set newParentGroup = g
            End If
        End If
    Next g
    
    If (Not hasParentTag And Not newParentGroup Is Nothing) Then
        SetGroupIsParent newParentGroup, True
    End If
    
    ' Add the new salt
    frm.mCDXDrawing.Objects.Data(MIME_XML) = saltXML
    
    ' Calculate the right edge of the old elements of the drawing. This has to be done after adding the
    ' salt in case there was auto centering.
    minLeft = 0
    For Each g In frm.mCDXDrawing.Groups
        If (g.Group.ID = 0) Then
            If (aGroups.Exists(g.ID)) Then
                If (minLeft = 0) Then
                    minLeft = g.Bounds.Right
                    minTop = g.Bounds.Top
                    maxBottom = g.Bounds.Bottom
                Else
                    If (minLeft < g.Bounds.Right) Then
                        minLeft = g.Bounds.Right
                    End If
                    If (minTop > g.Bounds.Top) Then
                        minTop = g.Bounds.Top
                    End If
                    If (maxBottom < g.Bounds.Bottom) Then
                        maxBottom = g.Bounds.Bottom
                    End If
                End If
            End If
        End If
    Next g
    
    ' Move the salt to the right
    For Each g In frm.mCDXDrawing.Groups
        If (g.Group.ID = 0) Then
            If (Not aGroups.Exists(g.ID)) Then
                g.Objects.Move _
                    (minLeft - g.Bounds.Left) / 20 + INTERSTRUCTURE_DIST, _
                    ((maxBottom + minTop) / 2 - (g.Bounds.Bottom + g.Bounds.Top) / 2) / 20
                PutStoiCoeff g, saltCoefficient, frm.mCDXDrawing
            End If
        End If
    Next g
    
    ' Add the coefficient to the salt
    
    CombineStructures = Replace(Replace(frm.mCDXDrawing.Data(MIME_XML), "<page", "<page><group"), "</page>", "</group></page>")
    
CatchError:

    Dim errNumber As Long
    Dim errDesc As String
    Dim errSource As String
    
    errNumber = Err.Number
    errSource = Err.Source
    errDesc = Err.Description
    
    Unload frm
    Set frm = Nothing
    
    If (errNumber <> 0) Then
        Err.Raise errNumber, "AddSaltGListener.CombineStructures/" & errSource, errDesc
    End If
End Function


Private Sub MoveArrow(oArrow As ChemDrawControl9Ctl.Graphic, ByVal DX As Double, ByVal DY As Double)
    Dim oPoint As ChemDrawControl9Ctl.Point
    
    Set oPoint = oArrow.Start
    oPoint.x = oPoint.x + DX
    oPoint.y = oPoint.y + DY
    oArrow.Start = oPoint
    Set oPoint = oArrow.End
    oPoint.x = oPoint.x + DX
    oPoint.y = oPoint.y + DY
    oArrow.End = oPoint
End Sub

Private Sub AddSalt(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal GridRow As ENStandard9.IENGridRow)
    Dim frm As frmAddSalt
    Dim CTableCtl As IENGridCtl
    Dim ts As IENGrid
    Dim changedParent As Boolean
    Dim changedFormula As Boolean
    Dim changedName As Boolean
    Dim tps(0 To kMaxRow) As IENGridProperty
    
    Dim g As ChemDrawControl9Ctl.Group
    Dim newG As ChemDrawControl9Ctl.Group
    ' Workaround to type mismatch
    Dim ChemDrawCtl As Object
    'Dim ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl
    Dim oldXML As String
    Dim oldRect As ChemDrawControl9Ctl.RECT
    Dim oldLeft As Double
    Dim oldRight As Double
    Dim oldTop As Double
    Dim DX As Double
    Dim DY As Double
    Dim dWidth As Double
    Dim minLeft As Double
    Dim i As Long
    Dim aGroups As Scripting.Dictionary
    Dim newGBounds As ChemDrawControl9Ctl.RECT
    Dim newID As Long
    
    Dim saltXML As String
    Dim combinedXML As String
    Dim f As frmChemDraw
    Dim oldEncoded As Boolean
    Dim j As Long
    
    Set CTableCtl = FieldCtlContainer.Control(mField).object
    
    Set frm = New frmAddSalt
    frm.Initialize mConnection
    frm.Caption = IENFieldListener_Name

    frm.Show vbModal

    If (Not frm.IsOK) Then
        Unload frm
        Exit Sub
    End If
    
    saltXML = GetCDXML(frm.NewSaltSection)
    
    ' Fix CSBR-59769. See if there is an group surrounding the fragment, which should can delete
    ' (because it wreaks havoc with the Data Sgroup code).
    Set f = New frmChemDraw
    With f.ChemDrawCtl1
        .Objects.Clear
        oldEncoded = .DataEncoded
        .DataEncoded = False
        .Data("cdxml") = saltXML
        ' If there are only two groups, and the second group is a child of the first, delete the first.
        If (.Groups.Count = 2) Then
            ' Find the outermost group.
            For i = 1 To 2
                If (.Groups(i).Group.ID = 0) Then ' test for top level group - possibly always true?
                    If (i = 1) Then
                        j = 2
                    Else
                        j = 1
                    End If
                    If ((.Groups(j).Group.ID = .Groups(i).ID) And _
                        (Abs(.Groups(i).Objects.Count - .Groups(j).Objects.Count) = 1)) Then
                        .Groups(i).Delete
                        saltXML = .Data("text/xml")
                        Exit For
                    End If
                End If
            Next i
        End If
        .DataEncoded = oldEncoded
    End With
    Set f = Nothing
    
    Set ChemDrawCtl = ChemDrawFieldCtl(FieldCtlContainer, FieldCtlContainer.SelectedSection)
    
    ' Create a dictionary of all top-level groups
    Set aGroups = New Scripting.Dictionary
    For Each g In ChemDrawCtl.Groups
        If (g.Group.ID = 0) Then
            If (g.ID <> CLng(GridRow.getAttribute("ID"))) Then
                aGroups.Add g.ID, g.ID
            End If
        End If
    Next g
    
    Set g = GetGroupByID(ChemDrawCtl, CLng(GridRow.getAttribute("ID")))
    
    Debug.Assert Not g Is Nothing
    
    oldXML = g.Objects.Data(MIME_XML)
    
    combinedXML = CombineStructures(ChemDrawCtl, oldXML, saltXML, frm.NewSaltCoefficient)
    
    ' Get the old bounding rectangle and critical coordinates thereof.
    Set oldRect = g.Bounds
    oldLeft = oldRect.Left
    oldRight = oldRect.Right
    oldTop = oldRect.Top
    
    ' Clear the old object
    g.Objects.Clear
    Set g = Nothing
    
    ' Replace the object with a new object
    ChemDrawCtl.Objects.Data(MIME_XML) = combinedXML
    
    ' Set newG to the newly created top-level group
    For Each g In ChemDrawCtl.Groups
        If (g.Group.ID = 0) Then
            If (Not aGroups.Exists(g.ID)) Then
                Set newG = g
                Exit For
            End If
        End If
    Next g
    
    Set newGBounds = GetBounds(newG.Objects(1).Bounds, newG.Bounds)
    
    ' dX and dY are the offsets for the new object to put it back into its old position.
    DX = (oldLeft - newGBounds.Left) / 20
    DY = (oldTop - newGBounds.Top) / 20
    
    newID = newG.ID
    
    ' Objects that are to the left of the point 3/4 toward the right of the old drawing are shifted.
    minLeft = ((3 * oldRight) + oldLeft) / 4
    dWidth = ((newGBounds.Right - newGBounds.Left) - (oldRight - oldLeft)) / 20
    
    ' Take every group that appears to the right of the new group and adjust it to account for the size of the new group
    For Each g In ChemDrawCtl.Groups
        If (g.Group.ID = 0) Then
            If (g.ID = newID) Then
                g.Objects.Move DX, DY
            ElseIf (g.Bounds.Left >= minLeft) Then
                g.Objects.Move dWidth
            End If
        End If
    Next g
    
    ' Shift graphics and captions that appear to the right of the new drawing.
    Dim oGraphic As ChemDrawControl9Ctl.Graphic
    Dim oCaption As ChemDrawControl9Ctl.Caption
    Dim oPoint As ChemDrawControl9Ctl.Point
    
    For Each oGraphic In ChemDrawCtl.Graphics
        If (oGraphic.Group.ID = 0) Then
            If (oGraphic.Bounds.Left >= minLeft) Then
                MoveArrow oGraphic, dWidth, 0#
            End If
        End If
    Next oGraphic
    For Each oCaption In ChemDrawCtl.Captions
        If (oCaption.Group.ID = 0) Then
            Set oPoint = oCaption.Position
            If (oPoint.x >= minLeft) Then
                oPoint.x = oPoint.x + dWidth
                oCaption.Position = oPoint
            End If
        End If
    Next oCaption
    
    Set ts = GridRow.Grid
        
    changedParent = False
    changedFormula = False
    changedName = False
    FieldCtlContainer.ContentChanging GridRow
    FieldCtlContainer.StartChangeMonitor
    FieldCtlContainer.ContentCaching GridRow
    
    GridRow.setAttribute "ID", newID
    GridRow.setAttribute "parentID", ""

'        If (GridRow.getAttribute("parentID") <> CStr(frm.NewParentID)) Then
'            GridRow.setAttribute "parentID", frm.NewParentID
'            changedParent = True
'            changedName = True
'        End If
'
'        GridRow.setAttribute "parentCoefficient", frm.NewParentCoefficient
'
'        If (GridRow.getAttribute("saltID") <> CStr(frm.NewSaltID)) Then
'            GridRow.setAttribute "saltID", frm.NewSaltID
'            changedName = True
'        End If
'
'        If (GridRow.getAttribute("solvateID") <> CStr(frm.NewSolvateID)) Then
'            GridRow.setAttribute "solvateID", frm.NewSolvateID
'            changedName = True
'        End If
'
'        If (GridRow.getAttribute("saltCoefficient") <> frm.NewSaltCoefficient) Then
'            GridRow.setAttribute "saltCoefficient", frm.NewSaltCoefficient
'            changedFormula = True
'            changedName = True
'        End If
'
'        If (GridRow.getAttribute("solvateCoefficient") <> CStr(frm.NewSolvateCoefficient)) Then
'            GridRow.setAttribute "solvateCoefficient", frm.NewSolvateCoefficient
'            changedFormula = True
'            changedName = True
'        End If
'
'        If (GridRow.getAttribute("saltCode") <> frm.NewSaltName) Then
'            GridRow.setAttribute "saltCode", frm.NewSaltName
'            changedFormula = True
'            changedName = True
'        End If
'
'        If (GridRow.getAttribute("solvateCode") <> frm.NewSolvateName) Then
'            GridRow.setAttribute "solvateCode", frm.NewSolvateName
'            changedFormula = True
'            changedName = True
'        End If
    
    FieldCtlContainer.ContentChanged GridRow, "Add Salt to [Object]"
    
    ' Clear the tags of the specified table cells if the data changed.
    If (mComputeName) Then
        Set tps(kNameRow) = ts.GridPropertyByName("Name")
    Else
        Set tps(kNameRow) = Nothing
    End If
    Set tps(kFormulaRow) = ts.GridPropertyByName("Molecular Formula")
    If (Not tps(kNameRow) Is Nothing) Then
        ClearCellEntered CTableCtl, GridRow.GridCell(tps(kNameRow))
    End If
    If (Not tps(kFormulaRow) Is Nothing) Then
        ClearCellEntered CTableCtl, GridRow.GridCell(tps(kFormulaRow))
    End If

'        If (changedParent) Then
'            Set tps(kMWRow) = ts.GridPropertyByName("Molecular Weight")
'            If (Not tps(kMWRow) Is Nothing) Then
'                ClearCellEntered CTableCtl, GridRow.GridCell(tps(kMWRow))
'            End If
'        End If
'
    Set tps(kFormulaMassRow) = ts.GridPropertyByName("Formula Mass")
    If (Not tps(kFormulaMassRow) Is Nothing) Then
        ClearCellEntered CTableCtl, GridRow.GridCell(tps(kFormulaMassRow))
    End If

    FieldCtlContainer.FinishChangeMonitor

    ValidateReactionControl FieldCtlContainer, FieldCtlContainer.SelectedSection
    
CatchError:
    Dim errNumber As Long
    Dim errDesc As String
    Dim errSource As String
    
    errNumber = Err.Number
    errSource = Err.Source
    errDesc = Err.Description
    
    If (Not frm Is Nothing) Then
        Unload frm
        Set frm = Nothing
    End If
    
    If (Err.Number <> 0) Then
        Err.Raise errNumber, errSource, errDesc
    End If
End Sub

Private Sub IENCommand_Clicked(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ENClientCtl As ENFramework9.IObjectDisplay)
    Dim CTableCtl As IENGridCtl
    
    Set CTableCtl = FieldCtlContainer.Control(mField).object
    AddSalt FieldCtlContainer, CTableCtl.SelectedRow
End Sub

Private Sub IENCommand_Delete()

End Sub

Private Function HasCompound(ByVal TableRow As IENGridRow) As Boolean
    ' ## True if the row has an associated structure.
    ' ##REMARKS The structure is either the first chemical structure field for this row, or, if there is none, the first chemical structure in the section.
    Dim ts As IENGrid
    Dim tp As IENGridProperty
    Dim tc As IENGridCell
    Dim sec As ENFramework9.Section
    Dim i As Long
    Dim f As ENFramework9.Field
    Dim sc As SectionCell
    Dim cs As ChemicalStructure
    Dim foundOne As Boolean
    
    HasCompound = False
    If (TableRow.getAttribute("ID") = "") Then
        Set ts = TableRow.Grid
        For i = 1 To ts.CountProperties
            Set tp = ts.GridProperty(i)
            If (tp.PropertyTypeName = "structure") Then
                Set tc = TableRow.GridCell(tp)
                HasCompound = Len(tc.CDXStructure) > 0
                Exit For
            End If
        Next i
    Else
        Set sec = TableRow.Grid.SectionCell.Section
        For i = 1 To sec.SectionType.Fields.Count
            Set f = sec.SectionType.Fields(i)
            If (TypeOf f.Tag Is ChemicalStructureFactory) Then
                Set sc = sec.SectionCell(f)
                Set cs = sc.Tag
                HasCompound = Len(cs.CDX) > 0
                Exit For
            End If
        Next i
    End If
End Function

Private Property Get IENCommand_Enabled(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ENClientCtl As ENFramework9.IObjectDisplay) As Boolean
    Dim CTableCtl As IENGridCtl
    
    Set CTableCtl = FieldCtlContainer.Control(mField).object
    
    If (CTableCtl.SelectedRow Is Nothing) Then
        IENCommand_Enabled = False
    ElseIf (Not FieldCtlContainer.CanEditCell(FieldCtlContainer.SelectedSection.SectionCell(mField))) Then
        IENCommand_Enabled = False
    Else
        IENCommand_Enabled = HasCompound(CTableCtl.SelectedRow)
    End If
End Property

Private Property Get IENCommand_ENObject() As ENFramework9.IENObject
    Set IENCommand_ENObject = Me
End Property

Private Sub IENCommand_Initialize(ByVal Command As ENFramework9.Command, ByVal Connection As ENFramework9.Connection, ByVal InitialData As Variant)
    ' Not used for table listeners
End Sub

Private Sub IENCommand_Load(ByVal InitialData As MSXML2.IXMLDOMElement)
    ' Not used for table listeners
End Sub

Private Property Get IENCommand_Object() As Object
    Set IENCommand_Object = Me
End Property

Private Sub IENCommand_Properties()
    ' Not used for table listeners
End Sub

Private Sub IENCommand_Terminate()
    ' Not used for table listeners
End Sub

Private Property Get IENCommand_Visible(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal ENClientCtl As ENFramework9.IObjectDisplay) As Boolean
    IENCommand_Visible = True
End Property

Private Sub IENFieldListener_EnterFocus(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_ExitFocus(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_Hide(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_Resize(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)

End Sub

Private Sub IENFieldListener_Show(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal SectionCell As ENFramework9.SectionCell)
    ' Fix bug CSBR-57083.  The IENFieldListener_Show method for the ISISDrawCSListener never gets called when an experiment
    '                      is created, but this does, so we use this to set ChemDraw to ViewOnly.
    '                      Note the hard-wired field name, which is valid for the Organon configuration.
    Dim ccsCtl As ENStandardCtl9.CChemicalStructureCtl
    Dim oField As ENFramework9.Field

    For Each oField In FieldCtlContainer.SelectedSection.SectionType.Fields
        If oField.FieldType.Name = "Chemical Structure" Then
            Set ccsCtl = FieldCtlContainer.Control(oField).object.CChemicalStructureCtl
            ccsCtl.ChemicalStructureCtl.ChemDrawCtl.ViewOnly = True
            Exit For
        End If
    Next
End Sub

Private Sub IENFieldListener_Terminate()
    Terminate
End Sub

Private Sub IENFieldListener_Properties()
    Dim frm As frmSaltWeightConfig
    
    Set frm = New frmSaltWeightConfig
    frm.Initialize mComputeName, mNewSaltCode, mNewSolvateCode
    frm.Caption = IENFieldListener_Name & " Properties"
    frm.Show vbModal
    
    If (frm.IsOK) Then
        If (frm.ComputeName <> mComputeName) Then
            mComputeName = frm.ComputeName
            mSaved = False
        End If
        If (frm.NewSaltCode <> mNewSaltCode) Then
            mNewSaltCode = frm.NewSaltCode
            mSaved = False
        End If
        If (frm.NewSolvateCode <> mNewSolvateCode) Then
            mNewSolvateCode = frm.NewSolvateCode
            mSaved = False
        End If
        
        If (Not mSaved) Then
            mField.SectionType.Collection.SaveConfigurationObject Me, "Saving changes to " & IENObject_Description
        End If
    End If
    
CatchError:
    Unload frm
    ErrorMsgBox Err, IENFieldListener_Name & " Properties", mConnection
End Sub

Private Sub IENGridListener_AfterChangeReference(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal GridCell As ENStandard9.IENGridCell)

End Sub

Private Sub IENGridListener_AfterEditValue(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal GridCell As ENStandard9.IENGridCell, ByVal newValue As Variant)

End Sub

Private Sub IENGridListener_BeforeChangeReference(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal GridCell As ENStandard9.IENGridCell, ByVal newValue As Variant, Cancel As Boolean)

End Sub

Private Sub IENGridListener_BeforeEditValue(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal GridCell As ENStandard9.IENGridCell, Cancel As Boolean)

End Sub

Private Property Get IENGridListener_CanChangeReference(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal GridCell As ENStandard9.IENGridCell) As Boolean
    IENGridListener_CanChangeReference = True
End Property

Private Property Get IENGridListener_CanEditValue(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal GridCell As ENStandard9.IENGridCell) As Boolean
    IENGridListener_CanEditValue = True
End Property

Private Property Get IENObject_Object() As Object
    Set IENObject_Object = Me
End Property

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
            
            Set dataNode = rootNode.selectSingleNode("saltWeightTListener/@computeName")
            If (Not dataNode Is Nothing) Then
                mComputeName = XMLBoolean(dataNode.nodeValue)
            End If
    
            Set dataNode = rootNode.selectSingleNode("saltWeightTListener/@newSaltCode")
            If (Not dataNode Is Nothing) Then
                mNewSaltCode = XMLBoolean(dataNode.nodeValue)
            End If
    
            Set dataNode = rootNode.selectSingleNode("saltWeightTListener/@newSolvateCode")
            If (Not dataNode Is Nothing) Then
                mNewSolvateCode = XMLBoolean(dataNode.nodeValue)
            End If
            
            mSaved = True
        Case "xml"
            Set rootNode = Contents
            Dim b As Boolean
            
            Set dataNode = rootNode.getAttributeNode("name")
            If (Not dataNode Is Nothing) Then IENFieldListener_Name = dataNode.nodeValue
            
            Set dataNode = rootNode.getAttributeNode("progID")
            If (Not dataNode Is Nothing) Then IENFieldListener_ProgID = dataNode.nodeValue
            
            Set dataNode = rootNode.getAttributeNode("position")
            If (Not dataNode Is Nothing) Then IENFieldListener_Position = CLng(dataNode.nodeValue)
            
            Set dataNode = rootNode.selectSingleNode("saltWeightTListener/@computeName")
            If (Not dataNode Is Nothing) Then
                b = XMLBoolean(dataNode.nodeValue)
                If (b <> mComputeName) Then
                    mComputeName = b
                    mSaved = False
                End If
            End If
    
            Set dataNode = rootNode.selectSingleNode("saltWeightTListener/@newSaltCode")
            If (Not dataNode Is Nothing) Then
                b = XMLBoolean(dataNode.nodeValue)
                If (b <> mNewSaltCode) Then
                    mNewSaltCode = b
                    mSaved = False
                End If
            End If
    
            Set dataNode = rootNode.selectSingleNode("saltWeightTListener/@newSolvateCode")
            If (Not dataNode Is Nothing) Then
                b = XMLBoolean(dataNode.nodeValue)
                If (b <> mNewSolvateCode) Then
                    mNewSolvateCode = b
                    mSaved = False
                End If
            End If
    
    End Select
    
End Sub

Private Property Get XMLAttributes(ByVal Src As MSXML2.DOMDocument, ByVal Depth As String) As IXMLDOMNode
    Dim rootNode As IXMLDOMElement
    
    Set rootNode = Src.createElement("fieldListener")
    If (Depth <> "export") Then
        rootNode.setAttribute "key", mKey
    End If
    rootNode.setAttribute "progID", mProgID
    rootNode.setAttribute "name", mName
    rootNode.setAttribute "position", mPosition
    
    Dim configNode As IXMLDOMElement
    Set configNode = rootNode.appendChild(Src.createElement("saltWeightTListener"))
    configNode.setAttribute "computeName", XMLString(mComputeName)
    configNode.setAttribute "newSaltCode", XMLString(mNewSaltCode)
    configNode.setAttribute "newSolvateCode", XMLString(mNewSolvateCode)
    
    Set XMLAttributes = rootNode
End Property

Private Function IENObject_XMLCreate(ByVal NewType As String, ByVal NewRelation As String, ByVal NewData As MSXML2.IXMLDOMElement) As IENObject
    Err.Raise vbObjectError + 513, Description:="XMLCreate not supported for " & TypeName(Me)
End Function

Private Sub IENObject_XMLAppendData(ByVal resultNode As IXMLDOMElement, ByVal Depth As String)
    ' Append the data of this object to the resultNode.
    resultNode.appendChild XMLAttributes(resultNode.ownerDocument, Depth)
End Sub

Private Sub IENObject_XMLDelete()
    mFieldListeners.Remove CStr(IENFieldListener_Key)
End Sub

Private Sub IENObject_XMLDeleteCollection(ByVal col As Collection)
    Err.Raise vbObjectError + 513, Description:="XMLDeleteCollection not supported for " & TypeName(Me)
End Sub

Private Function IENObject_XMLDoCommand(ByVal verbNode As MSXML2.IXMLDOMNode, ByVal ResultDoc As MSXML2.IXMLDOMDocument2) As MSXML2.IXMLDOMNode
    Err.Raise vbObjectError + 513, Description:="XMLDoCommand not supported for " & TypeName(Me)
End Function

Private Function IENObject_XMLDuplicate(Src As IENObject, ByVal NewRelation As String, ByVal NewData As IXMLDOMElement) As IENObject
    Err.Raise vbObjectError + 513, Description:="XMLDuplicate not supported for " & TypeName(Me)
End Function

Private Sub IENObject_XMLMove(Src As IENObject, ByVal NewRelation As String)
    Err.Raise vbObjectError + 513, Description:="XMLMove not supported for " & TypeName(Me)
End Sub

Private Function IENObject_XMLAppendPath(ByVal rootNode As IXMLDOMElement, Optional ByVal isPersistant As Boolean = False) As IXMLDOMElement
    Dim resultNode As IXMLDOMElement
    
    Set resultNode = rootNode.ownerDocument.createElement("fieldListener")
    resultNode.setAttribute "key", CStr(IENFieldListener_Key)
    mField.ENObject.XMLAppendPath(rootNode, isPersistant).appendChild resultNode
    
    Set IENObject_XMLAppendPath = resultNode
End Function

Private Function IENObject_XMLResolve(ByVal toType As String, ByVal whoseNode As IXMLDOMNode) As IENObject
    ' Return the number of the specified subItem.
    Err.Raise vbObjectError + 513, Description:="the type """ & toType & """ cannot be resolved from " & IENObject_Description
End Function

Private Function IENObject_XMLCount(ByVal ResultDoc As MSXML2.DOMDocument, ByVal toType As String) As IXMLDOMElement
    ' Return the number of the specified subItem.
    Err.Raise vbObjectError + 513, Description:="the type """ & toType & """ cannot be resolved from " & IENObject_Description
End Function

Private Sub IENFieldListener_ContentCached(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal CachedContents As Collection)
    Dim ts As TableSection
    Dim i As Long
    Dim j As Long
    Dim k As Long
    Dim tr As TableRow
    Dim tc As TableCell
    Dim tps(0 To kMaxRow) As TableProperty
    Dim parentID As Long
    Dim saltID As Long
    Dim solvateID As Long
    Dim GroupID As Long
    Dim parentCoefficient As String
    Dim saltCoefficient As String
    Dim solvateCoefficient As String
    Dim saltName As String
    Dim solvateName As String
    ' Workaround to type mismatch
    Dim ChemDrawCtl As Object
    'Dim ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl
    Dim newValue As String
    Dim newNumericValue As Single
    Dim CTableCtl As IENGridCtl
    Dim compoundLoaded As Boolean
    Dim cdGroup As ChemDrawControl9Ctl.Group
    Dim isChemicalStructureChanged As Boolean
    Dim subGroup As ChemDrawControl9Ctl.Group
    Dim LeftMax As Double
    Dim LeftCoord As Double
    Dim LeftMolecule As Long
    Dim foundOne As Boolean
    Dim gLeft As ChemDrawControl9Ctl.Group
    Dim gFound As ChemDrawControl9Ctl.Group
    Dim Found() As Boolean
    Dim gSalt1 As ChemDrawControl9Ctl.Group
    Dim gParent As ChemDrawControl9Ctl.Group
        
    Set ts = FieldCtlContainer.SelectedSection.SectionCell(mField).Tag
    If (mComputeName) Then
        Set tps(kNameRow) = ts.TablePropertyByName("Name")
    Else
        Set tps(kNameRow) = Nothing
    End If
    Set tps(kFormulaRow) = ts.TablePropertyByName("Molecular Formula")
    Set tps(kMWRow) = ts.TablePropertyByName("Molecular Weight")
    Set tps(kFormulaMassRow) = ts.TablePropertyByName("Formula Mass")
    Set tps(kStructureRow) = ts.TablePropertyByName("Chemical Structure")
    
    Set CTableCtl = FieldCtlContainer.Control(mField).object
    
    ' Optimize the case where the chemical structure hasn't changed.
    ' These calculations are affected by changes in chemical structures.
    ' These calculations are affected by table rows when this class is called to change the salt weights.
    ' These calculates are affected by the addition of the intensive properties calculated here.
    isChemicalStructureChanged = False
    For i = 1 To CachedContents.Count
        If (TypeOf CachedContents.Item(i) Is ChemicalStructure Or _
            TypeOf CachedContents.Item(i) Is IENGridProperty Or _
            TypeOf CachedContents.Item(i) Is IENGridRow) Then
            isChemicalStructureChanged = True
            Exit For
        End If
    Next i
    
    For i = 1 To ts.CountRows
        Set tr = ts.TableRow(i)
        If (Not tr.IsDeleted And _
            (tr.getAttribute("ID") = "" Or isChemicalStructureChanged)) Then
            compoundLoaded = False
            
            For j = LBound(tps) To UBound(tps)
                If (Not tps(j) Is Nothing) Then
                    Set tc = tr.TableCell(tps(j))
                    If (Not tc.Tag = "entered") Then
                        If (Not compoundLoaded) Then
                            compoundLoaded = True
                            Set ChemDrawCtl = LoadCompound(FieldCtlContainer, tr)
                            
'                            If (IsNumeric(tr.getAttribute("parentID"))) Then
'                                parentID = Val(tr.getAttribute("parentID"))
'                            Else
'                                parentID = 0
'                            End If
'
                            If (IsNumeric(tr.getAttribute("saltID"))) Then
                                saltID = val(tr.getAttribute("saltID"))
                            Else
                                saltID = 0
                            End If
                            
                            If (IsNumeric(tr.getAttribute("solvateID"))) Then
                                solvateID = val(tr.getAttribute("solvateID"))
                            Else
                                solvateID = 0
                            End If
                            
                            If (IsNumeric(tr.getAttribute("ID"))) Then
                                GroupID = val(tr.getAttribute("ID"))
                                Set cdGroup = GetGroup(ChemDrawCtl, GroupID)
                                If (cdGroup Is Nothing) Then
                                    Err.Raise kApplicationError, "AddSaltGListener.IENFieldListener_ContentCached", _
                                        "the group ID " & GroupID & " is not recognized in the structure associated with " & tr.ENObject.Description
                                End If
                                                             
                                ' See if we need to drill down one level, as in the SDS case (salt
                                '  spanned by a Data Sgroup).

                                ' Is there only one of the contained groups that is a child of this one?
                                k = 0
                                For Each gSalt1 In cdGroup.Groups
                                    If (cdGroup.ID = gSalt1.Group.ID) Then
                                        k = k + 1
                                        Set gParent = gSalt1
                                    End If
                                Next gSalt1
                                If k = 1 Then
                                    ' Yes, there is only one of the contained groups that is a child of this one.
                                    ' Now, is there at least one child of this gParent?
                                    If (gParent.Groups.Count > 0) Then
                                        Set cdGroup = gParent ' Drill down one level.
                                    End If
                                End If
                            
                            Else
                                Set cdGroup = Nothing
                                GroupID = 0
                            End If
                            
                            parentCoefficient = tr.getAttribute("parentCoefficient")
                            saltCoefficient = tr.getAttribute("saltCoefficient")
                            solvateCoefficient = tr.getAttribute("solvateCoefficient")
                            
                            If (parentCoefficient = "1") Then parentCoefficient = ""
                            If (saltCoefficient = "1") Then saltCoefficient = ""
                            If (solvateCoefficient = "1") Then solvateCoefficient = ""
                            
                            saltName = tr.getAttribute("saltCode")
                            solvateName = tr.getAttribute("solvateCode")
                        End If
                    
                        If (ChemDrawCtl Is Nothing) Then
                            ' There was no compound to load
                            Select Case j
                                Case kNameRow:
                                    CTableCtl.SetCellValue tc, ""
                                Case kFormulaRow:
                                    CTableCtl.SetCellValue tc, ""
                                Case kMWRow:
                                    CTableCtl.ClearNumericCell tc
                                Case kFormulaMassRow:
                                    If (Not tps(kMWRow) Is Nothing) Then
                                        If (tr.TableCell(tps(kMWRow)).value = "") Then
                                            CTableCtl.ClearNumericCell tc
                                        Else
                                            CTableCtl.SetNumericCellValue tc, tr.TableCell(tps(kMWRow)).MinValue, "0.00"
                                        End If
                                    Else
                                        CTableCtl.ClearNumericCell tc
                                    End If
                                Case kStructureRow:
                                    CTableCtl.SetCellValue tc, ""
                            End Select
                        Else
                            If (Not cdGroup Is Nothing) Then
                                Select Case j
                                    Case kNameRow:
                                        If (GroupID <> 0) Then
                                            newValue = cdGroup.Objects.Data("chemical/x-name")
                                        Else
                                            newValue = ChemDrawCtl.Objects.Data("chemical/x-name")
                                        End If
                                        
                                        If (saltName <> "") Then
                                            newValue = newValue & ", " & IIf(saltCoefficient = "1", "", saltCoefficient) & saltName
                                        End If
                                        
                                        If (solvateName <> "") Then
                                            newValue = newValue & ", " & IIf(solvateCoefficient = "1", "", solvateCoefficient) & solvateName
                                        End If
                                        CTableCtl.SetCellValue tc, newValue
                                    Case kFormulaRow:
                                        If (GroupID <> 0) Then
                                            If (cdGroup.Groups.Count = 0) Then
                                                newValue = cdGroup.Objects.Formula
                                                While Right(newValue, 1) = ChrW(8226)
                                                    newValue = Left(newValue, Len(newValue) - 1)
                                                Wend
                                            Else
                                                newValue = ""
                                            ' Do the parent group first.
                                                For Each subGroup In cdGroup.Groups
                                                    If (subGroup.Group.ID = cdGroup.ID) Then
                                                        If (Not subGroup.GetObjectTag("parent") Is Nothing) Then
                                                            ' Prefix the stoichiometric coefficient to the formula for this group
                                                            If (GetStoiCoeff(subGroup) <> "1") Then
                                                                newValue = newValue & GetStoiCoeff(subGroup)
                                                            End If
                                                            newValue = newValue & subGroup.Objects.Formula
                                                            While Right(newValue, 1) = ChrW(8226)
                                                                newValue = Left(newValue, Len(newValue) - 1)
                                                            Wend
                                                            Exit For
                                                        End If
                                                    End If
                                                Next subGroup
                                                
                                                ' Do each group that is not the parent group.
                                                ReDim Found(cdGroup.Groups.Count) As Boolean
                                                For Each subGroup In cdGroup.Groups
                                                    If (subGroup.Group.ID = cdGroup.ID) Then
                                                    
                                                        ' Find the next leftmost fragment.
                                                        LeftMax = 1000000#
                                                        LeftMolecule = 0
                                                        foundOne = False
                                                        k = 0
                                                        For Each gLeft In cdGroup.Groups
                                                            k = k + 1
                                                            If Not Found(k) Then
                                                                LeftCoord = gLeft.Objects.Left
                                                                If LeftCoord < LeftMax Then
                                                                    LeftMax = LeftCoord
                                                                    LeftMolecule = k
                                                                    Set gFound = gLeft
                                                                    foundOne = True
                                                                End If
                                                            End If
                                                        Next
                                                        If foundOne Then
                                                            Found(LeftMolecule) = True
                                                            If (subGroup.GetObjectTag("parent") Is Nothing) Then
                                                                ' Put a dot between each group.
                                                                If (Len(newValue) > 0) Then
                                                                    newValue = newValue & ChrW(183)
                                                                End If
                                                                ' Prefix the stoichiometric coefficient to the formula for this group
                                                                If (GetStoiCoeff(subGroup) <> "1") Then
                                                                    newValue = newValue & GetStoiCoeff(gFound)
                                                                End If
                                                                newValue = newValue & gFound.Objects.Formula
                                                                While Right(newValue, 1) = ChrW(8226)
                                                                    newValue = Left(newValue, Len(newValue) - 1)
                                                                Wend
                                                            End If
                                                        End If
                                                    End If
                                                Next subGroup
                                            End If
                                        End If
                                        While Right(newValue, 1) = ChrW(8226)
                                            newValue = Left(newValue, Len(newValue) - 1)
                                        Wend
                                        
                                        CTableCtl.SetCellValue tc, newValue
                                    Case kMWRow:
                                        If (GroupID <> 0) Then
                                            If (cdGroup.Groups.Count = 0) Then
                                                newNumericValue = cdGroup.Objects.MolecularWeight / 1000#
                                            Else
                                                newNumericValue = 0
                                                For Each subGroup In cdGroup.Groups
                                                    If (subGroup.Group.ID = cdGroup.ID) Then
                                                        If (Not subGroup.GetObjectTag("parent") Is Nothing) Then
                                                            ' Prefix the stoichiometric coefficient to the formula for this group
                                                            newNumericValue = subGroup.Objects.MolecularWeight / 1000#
                                                            Exit For
                                                        End If
                                                    End If
                                                Next subGroup
                                            End If
                                        End If
                                        
                                        CTableCtl.SetNumericCellValue tc, newNumericValue
                                    Case kFormulaMassRow:
                                        If (GroupID <> 0) Then
                                            If (cdGroup.Groups.Count = 0) Then
                                                newNumericValue = cdGroup.Objects.MolecularWeight / 1000#
                                            Else
                                                newNumericValue = 0
                                                For Each subGroup In cdGroup.Groups
                                                    If (subGroup.Group.ID = cdGroup.ID) Then
                                                        ' Prefix the stoichiometric coefficient to the formula for this group
                                                        If (GetStoiCoeff(subGroup) <> "") Then
                                                            newNumericValue = newNumericValue + CDbl(GetStoiCoeff(subGroup)) * subGroup.Objects.MolecularWeight / 1000#
                                                        Else
                                                            newNumericValue = newNumericValue + subGroup.Objects.MolecularWeight / 1000#
                                                        End If
                                                    End If
                                                Next subGroup
                                            End If
                                        End If
                                        
                                        Dim coefficientValue As Single
                                        
                                        CTableCtl.SetNumericCellValue tc, newNumericValue
                                    Case kStructureRow:
                                        If (GroupID <> 0) Then
                                            CTableCtl.SetCellValue tc, cdGroup.Objects.Data("cdxml")
                                        End If
                                End Select
                            End If
                        End If
                    End If
                End If
            Next j
        End If
    Next i
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
        Dim configString As String
        Dim rootNode As IXMLDOMElement
        Dim dataNode As IXMLDOMNode
        
        Set toRS = NewData
        With toRS
            mKey = !field_listener_key
            mName = !Name
            mProgID = !prog_id
            mPosition = !Position
        
            configString = NonNull(toRS!config_xml)
            If (Len(configString) > 0) Then
                Dim nameDataDoc As DOMDocument
                
                Set nameDataDoc = New DOMDocument
                If (nameDataDoc.loadXML(configString)) Then
                    ' Extract names from the XML Config file.
                    Set rootNode = nameDataDoc.documentElement
                    
                    Set dataNode = rootNode.getAttributeNode("computeName")
                    If (Not dataNode Is Nothing) Then
                        mComputeName = XMLBoolean(dataNode.nodeValue)
                    End If
                    
                    Set dataNode = rootNode.getAttributeNode("newSaltCode")
                    If (Not dataNode Is Nothing) Then
                        mNewSaltCode = XMLBoolean(dataNode.nodeValue)
                    End If
                    
                    Set dataNode = rootNode.getAttributeNode("newSolvateCode")
                    If (Not dataNode Is Nothing) Then
                        mNewSolvateCode = XMLBoolean(dataNode.nodeValue)
                    End If
                End If
            End If
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

Private Sub IENFieldListener_Save()
    mField.SectionType.Collection.SaveConfigurationObject Me, "Saving changes to " & IENObject_Description
End Sub

Private Property Get IENObject_Description(Optional ByVal Flags As Long = 0) As String
    IENObject_Description = "Table Listener """ & mName & """ of " & mField.ENObject.Description(Flags)
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
            Dim rootNode As IXMLDOMElement
            Dim nameDataDoc As DOMDocument
            Dim configString As String
            
            Set nameDataDoc = New DOMDocument
            Set rootNode = nameDataDoc.createElement("saltWeightTListener")
            
            rootNode.setAttribute "computeName", XMLString(mComputeName)
            rootNode.setAttribute "newSaltCode", XMLString(mNewSaltCode)
            rootNode.setAttribute "newSolvateCode", XMLString(mNewSolvateCode)
            
            configString = ", config_xml = " & .ConstantSyntax(rootNode.XML)
            
            .Execute "UPDATE ELN_field_listeners" & _
                     " SET name = " & .ConstantSyntax(mName) & _
                     ", prog_ID = " & .ConstantSyntax(mProgID) & _
                     ", position = " & .ConstantSyntax(mPosition) & _
                     configString & _
                     " WHERE field_listener_key = " & mKey
        
            Dim com As ADODB.Command
            
            Set com = .CreateCommand
            com.CommandType = adCmdStoredProc
            
            com.Parameters.Append com.CreateParameter(":fKey", adInteger, adParamInput, value:=mKey)
            
            com.CommandText = .AuditSchemaPrefix & "audit_field_listener"
            .ExecuteCommand com
        End With
        
        mSaved = True
    End If
End Sub

Private Function IENGridListener_EditValue(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal GridCell As ENStandard9.IENGridCell) As Boolean
    mConnection.WriteLogStream "==>> Standard Add-ins: SaltWeightTListener.IENGridListener_EditValue: Edit Value"
    
    If (GridCell.ELNProperty.Name = "Molecular Formula") Then
        AddSalt FieldCtlContainer, GridCell.GridRow
        IENGridListener_EditValue = True
    Else
        IENGridListener_EditValue = False
    End If
End Function

Private Sub IENGridListener_CellChanged(ByVal FieldCtlContainer As IFieldCtlContainer, ByVal GridCell As IENGridCell)
    If (GridCell.ELNProperty.Name = "Name") Then
        If (GridCell.ELNProperty.IsTextProperty) Then
            If (Len(GridCell.value) = 0) Then
                ' This is no longer an edited cell.
                GridCell.Tag = ""
'                TableCell.ForeColor = RGB(0, 0, 0)
'                CTableCtl.TableCtl.SetupCellColors TableCell
            Else
                GridCell.Tag = "entered"
'                TableCell.ForeColor = RGB(0, 0, 255)
'                CTableCtl.TableCtl.SetupCellColors TableCell
            End If
        End If
    End If
End Sub

' Workaround type mismatch.
Private Function ChemDrawFieldCtl(ByVal FieldCtlContainer As IFieldCtlContainer, ByVal Section As ENFramework9.Section) As Object
' Private Function ChemDrawFieldCtl(ByVal FieldCtlContainer As IFieldCtlContainer, ByVal Section As ENFramework9.Section) As ChemDrawControl9Ctl.ChemDrawCtl
        Dim CC As IENChemStructureCtl
        Dim foundOne As Boolean
        Dim i As Long
        Dim f As ENFramework9.Field
        
        foundOne = False
        For i = 1 To Section.SectionType.Fields.Count
            Set f = Section.SectionType.Fields(i)
            If (TypeOf f.Tag Is ChemicalStructureFactory) Then
                Set CC = FieldCtlContainer.Control(f).object
                foundOne = True
                Exit For
            End If
        Next i
        If (Not foundOne) Then
            Set ChemDrawFieldCtl = Nothing
        Else
            Set ChemDrawFieldCtl = CC.ChemDrawCtl
        End If

End Function

'This is added to validate the reaction field, causing the products table to be reset accordingly
'I have used the same logic as Function ChemDrawFieldCtl (above) to ensure I use the same field as is updated by the AddSalt function
Private Sub ValidateReactionControl(ByVal FieldCtlContainer As IFieldCtlContainer, ByVal Section As ENFramework9.Section)

        Dim theCCSCtl As ENStandardCtl9.CChemicalStructureCtl
        Dim i As Long
        Dim f As ENFramework9.Field
        
        For i = 1 To Section.SectionType.Fields.Count
            Set f = Section.SectionType.Fields(i)
            If (TypeOf f.Tag Is ChemicalStructureFactory) Then
                Set theCCSCtl = FieldCtlContainer.Control(f).object.CChemicalStructureCtl
                Exit For
            End If
        Next i
        
        If Not theCCSCtl Is Nothing Then
            theCCSCtl.ChemicalStructureCtl.DataChanged
            theCCSCtl.ChemicalStructureCtl.ENFieldCtl.Validate
        End If

End Sub

' Workaround type mismatch.
Private Function LoadCompound(ByVal FieldCtlContainer As IFieldCtlContainer, ByVal tr As TableRow) As Object
'Private Function LoadCompound(ByVal FieldCtlContainer As IFieldCtlContainer, ByVal tr As TableRow) As ChemDrawControl9Ctl.ChemDrawCtl
    ' ## Load the structure referenced by this table row.
    ' ##REMARKS The structure is either the first chemical structure field for this row, or, if there is none, the first chemical structure in the section.
    Dim ts As TableSection
    Dim tp As TableProperty
    Dim tc As TableCell
    Dim sec As ENFramework9.Section
    Dim i As Long
    Dim f As ENFramework9.Field
    Dim sc As SectionCell
    Dim cs As ChemicalStructure
    Dim foundOne As Boolean
    
    If (tr.getAttribute("ID") = "") Then
        Set ts = tr.TableSection
        For i = 1 To ts.CountProperties
            Set tp = ts.TableProperty(i)
            If (tp.PropertyTypeName = "structure") Then
                Set tc = tr.TableCell(tp)
                If (Len(tc.CDXStructure) = 0) Then
                    Set LoadCompound = Nothing
                    Exit Function
                End If
                If (mfrmEditStructure Is Nothing) Then
                    Set mfrmEditStructure = New frmEditStructure
                End If
                mfrmEditStructure.CDXStructure = tc.CDXStructure
                foundOne = True
                Exit For
            End If
        Next i
        If (Not foundOne) Then
            Set LoadCompound = Nothing
        Else
            Set LoadCompound = mfrmEditStructure.mCDXDrawing
        End If
    Else
        Dim CC As IENChemStructureCtl
        foundOne = False
        Set sec = tr.TableSection.SectionCell.Section
        For i = 1 To sec.SectionType.Fields.Count
            Set f = sec.SectionType.Fields(i)
            If (TypeOf f.Tag Is ChemicalStructureFactory) Then
                Set CC = FieldCtlContainer.Control(f).object
                foundOne = True
                Exit For
            End If
        Next i
        If (Not foundOne) Then
            Set LoadCompound = Nothing
        Else
            Set LoadCompound = CC.ChemDrawCtl
        End If
    End If
    
'    Set LoadCompound = mFrmEditStructure
End Function

' Workaround to type mismatch
Private Function GetGroup(ByVal ChemDrawCtl As Object, ByVal ID As Long) As ChemDrawControl9Ctl.Group
'Private Function GetGroup(ByVal ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl, ByVal ID As Long) As ChemDrawControl9Ctl.Group
    Dim i As Long
    Dim gs As ChemDrawControl9Ctl.Groups
    
    Set gs = ChemDrawCtl.Groups
    For i = 1 To gs.Count
        If (gs.Item(i).ID = ID) Then
            Set GetGroup = gs.Item(i)
            Exit Function
        End If
    Next i
End Function

Private Property Get SectionNames(ByVal Connection As ENFramework9.Connection, ByVal CollectionTypeName As String) As Scripting.Dictionary
    Dim i As Long
    Dim ct As CollectionType
    
    Set ct = Connection.CollectionTypes(Name:=CollectionTypeName)
    For i = 1 To ct.CollectionListeners.Count
        If (ct.CollectionListeners.Item(i).StorageProgID = "ENStandard9.SectionListCListener") Then
            Set SectionNames = ct.CollectionListeners.Item(i).Tag.object.SectionNames
            Exit Property
        End If
    Next i
    
    Set SectionNames = New Scripting.Dictionary
End Property

Private Function GetReactantProperties(ByVal Section As ENFramework9.Section) As PropertyInstances
    ' ## Return the first set of property instances of this section that contains a molecular weight property.
    
    Dim i As Long
    Dim j As Long
    Dim sc As SectionCell
    Dim ps As PropertyInstances
    Dim p As PropertyInstance
    Dim f As ENFramework9.Field
    
    For i = 1 To Section.SectionType.Fields.Count
        Set f = Section.SectionType.Fields(i)
        Set sc = Section.SectionCell(f)
        Err.Clear
        On Error Resume Next
        Set ps = sc.Tag
        If (Err.Number = 0) Then
            For j = 1 To ps.Count
                If (ps.Item(j).ELNProperty.Name = "Molecular Weight") Then
                    Set GetReactantProperties = ps
                    Exit Function
                End If
            Next j
        End If
    Next i
    
    Set GetReactantProperties = Nothing
End Function

Private Function GetConfigPropertyValue(ByVal SectionName As String, _
                                  ByVal Connection As ENFramework9.Connection, _
                                  ByVal CollectionTypeName As String, _
                                  ByVal propertyName As String)
    ' ## Return the molecular weight for the specified code, scanning the sections in the specified collection type
    Dim SaltCodeSecs As Scripting.Dictionary
    Dim saltCodeSec As ENFramework9.Section
    Dim ps As PropertyInstances
    Dim i As Long
    
    Set SaltCodeSecs = SectionNames(Connection, CollectionTypeName)

    ' Find the section that contains the salt code data
    If (SaltCodeSecs.Exists(SectionName)) Then
        Set saltCodeSec = SaltCodeSecs(SectionName)
    Else
        Err.Raise vbObjectError + 513, "SaltWeightTListener.GetConfigPropertyValue", _
                "the code """ & SectionName & """ is not recognized"
    End If
    
    ' Find the reactant properties that contain the molecular weight
    Set ps = GetReactantProperties(saltCodeSec)
    If (ps Is Nothing) Then
        Err.Raise vbObjectError + 513, "SaltWeightTListener.GetConfigPropertyValue", _
                "the section """ & SectionName & """ does not contain properties"
    End If
    
    ' Find the property of the specified name
    GetConfigPropertyValue = 0
    
    For i = 1 To ps.Count
        If (ps.Item(i).ELNProperty.Name = propertyName) Then
            GetConfigPropertyValue = ps.Item(i).MinValue
            Exit For
        End If
    Next i

End Function




