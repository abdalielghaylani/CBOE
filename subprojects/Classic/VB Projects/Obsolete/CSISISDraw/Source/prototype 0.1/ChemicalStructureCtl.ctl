VERSION 5.00
Object = "{BDA04258-9536-4779-A86A-49164A7741D7}#9.0#0"; "ChemDrawCtl9.dll"
Begin VB.UserControl ChemicalStructureCtl 
   ClientHeight    =   4515
   ClientLeft      =   0
   ClientTop       =   0
   ClientWidth     =   5505
   KeyPreview      =   -1  'True
   ScaleHeight     =   4515
   ScaleWidth      =   5505
   Begin ChemDrawControl9Ctl.ChemDrawCtl mDrawing 
      Height          =   3615
      Left            =   0
      TabIndex        =   0
      Top             =   0
      Width           =   4815
      _cx             =   8493
      _cy             =   6376
      SourceURL       =   ""
      DataURL         =   ""
      ViewOnly        =   0   'False
      DontCache       =   0   'False
      ShowToolsWhenVisible=   0   'False
      AuthenticateURL =   ""
      DataEncoded     =   -1  'True
      ShrinkToFit     =   -1  'True
      EnlargeToFit    =   0   'False
      WrapReactionsToFit=   0   'False
      RecenterWhenFitting=   0   'False
      BorderColor     =   0
      BorderVisible   =   0   'False
      BorderWidth     =   1
      OpenDoesImport  =   -1  'True
   End
End
Attribute VB_Name = "ChemicalStructureCtl"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_HelpID = 25611
Attribute VB_Description = "ENStandardCtl7.ChemicalStructureCtl"
Option Explicit

' ##MODULE_SUMMARY An implementation of IENFieldCtl that displays the contents of a _
%Chemical Structure:ENStandard8~ChemicalStructure% section cell _
for browsing and editing.

' ##SEEALSO Target=ENStandard8~ChemicalStructure, Caption=ChemicalStructure class

' ##IGNORE_REFERENCES mSectionCell, mTag, mFieldCtlContainer

Implements ENFramework9.IENFieldCtl
Implements ENFramework9.IENTOCFieldCtl
Implements ENRenderWord9.IENWordRenderer

Dim mTag As ChemicalStructure
Dim mSectionCell As ENFramework9.SectionCell
Dim mFieldCtlContainer As IFieldCtlContainer

Dim mSuppressContentChanged As Boolean
Dim mDataChanged As Boolean

' ISISDraw begin
Dim mfrmISISServer As frmISISServer
Dim mTempFolder As Folder
Dim mTempSkcFilePath As String
' ISISDraw end

Public Property Get ENFieldCtl() As IENFieldCtl
Attribute ENFieldCtl.VB_HelpID = 25612
    Set ENFieldCtl = Me
End Property

Public Property Get ChemDrawCtl() As Object
Attribute ChemDrawCtl.VB_HelpID = 25613
    Set ChemDrawCtl = mDrawing
End Property

Public Sub DataChanging()
Attribute DataChanging.VB_HelpID = 25614
    mFieldCtlContainer.ContentChanging mSectionCell.Tag.ENObject
    mFieldCtlContainer.ContentCaching mSectionCell.Tag.ENObject
End Sub

Private Sub MarkDataChanged()
    mDataChanged = True
End Sub

Private Sub RaiseAfterDataChanged()
    On Error GoTo CatchError
    
    Dim i As Long
    Dim csl As IENChemStructureListener
    Dim errNumber As Long
    Dim errSource As String
    Dim errDesc As String
    
    If (mSectionCell Is Nothing) Then Exit Sub
    If (Not mDataChanged) Then Exit Sub
    mDataChanged = False
    With mSectionCell.Field.FieldListeners
        For i = 1 To .count
            If (TypeOf .Item(i) Is IENChemStructureListener) Then
                Set csl = .Item(i)
                csl.AfterDataChanged mFieldCtlContainer, mSectionCell.Tag.object
            End If
        Next i
    End With

CatchError:
    errNumber = Err.Number
    If (errNumber <> 0) Then
        errSource = Err.Source
        errDesc = Err.Description
    End If
    On Error GoTo 0
    If (errNumber <> 0) Then Err.Raise errNumber, errSource, errDesc
End Sub

Private Sub RaiseBeforeDataChanged()
    On Error GoTo CatchError
    
    Dim i As Long
    Dim csl As IENChemStructureListener
    Dim errNumber As Long
    Dim errSource As String
    Dim errDesc As String
    
    If (mSectionCell Is Nothing) Then Exit Sub
    If (Not mDataChanged) Then Exit Sub
    With mSectionCell.Field.FieldListeners
        For i = 1 To .count
            If (TypeOf .Item(i) Is IENChemStructureListener) Then
                Set csl = .Item(i)
                csl.BeforeDataChanged mFieldCtlContainer, mSectionCell.Tag.object
            End If
        Next i
    End With

CatchError:
    errNumber = Err.Number
    If (errNumber <> 0) Then
        errSource = Err.Source
        errDesc = Err.Description
    End If
    On Error GoTo 0
    If (errNumber <> 0) Then Err.Raise errNumber, errSource, errDesc
End Sub

Private Sub MonitorDataChanged(ByVal DoMonitor As Boolean)
    If (Not mSuppressContentChanged) Then
        If (mFieldCtlContainer.CanEditCell(mSectionCell)) Then
            On Error Resume Next
            mFieldCtlContainer.ContentChanging mSectionCell.Tag.ENObject
            If (Err.Number <> 0) Then
                Dim oldEncoded As Boolean
                
                ShowError "Error Changing Chemical Structure"
                
                mSuppressContentChanged = True
                
                oldEncoded = mDrawing.DataEncoded
                mDrawing.DataEncoded = True
                
                mDrawing.Objects.Clear
                If (Len(mTag.CDX) > 0) Then
                    mDrawing.SourceURL = "data:chemical/x-cdx;base64," & mTag.CDX
                End If
                mDrawing.DataEncoded = oldEncoded
                
                mSuppressContentChanged = False
            Else
                On Error GoTo 0
                mFieldCtlContainer.ContentCaching mSectionCell.Tag.ENObject
                mDrawing.DataChanged = True
                MarkDataChanged
            End If
        End If
    End If
End Sub

Public Sub DataChanged()
    MonitorDataChanged False
End Sub

Private Sub IENFieldCtl_ContentCached(ByVal CachedContents As Collection)

End Sub

Private Sub ExportStructureToWord(Tag As ChemicalStructure, Location As Word.Range)
    Dim exportFile As String
    Dim fso As New FileSystemObject
    Dim ts As TextStream
    Dim s As InlineShape
    Dim w As Single
    Dim h As Single
    Dim f As frmEditStructure
    
    If (Len(Tag.CDX) = 0) Then
        Location.Delete
        Exit Sub
    End If
    
    exportFile = GetTempFile("emf")
    
    Set f = New frmEditStructure
    On Error Resume Next
    f.Initialize Tag.SectionCell.Connection
    If (Err.Number = 0) Then f.CDXStructure = Tag.CDX
    If (Err.Number = 0) Then f.WriteStructurePicture exportFile, "image/x-emf"
    Unload f
    
    If (Err.Number <> 0) Then
        Dim errNumber As Long
        Dim errSource As String
        Dim errDesc As String
        
        errNumber = Err.Number
        errSource = Err.Source
        errDesc = Err.Description
        On Error GoTo 0
        
        Err.Raise errNumber, "ExportStructureToWord/" & errSource, errDesc
    End If
    
    On Error GoTo 0
    
    Set s = Location.InlineShapes.AddPicture(FileName:=exportFile, _
        Range:=Location, LinkToFile:=False, SaveWithDocument:=True)

    ' If necessary, scale the new shape.
    If (Err.Number = 0) Then
        With Location.PageSetup
            w = .PageWidth - .RightMargin - .LeftMargin
            If (w < s.Width) Then
                s.Height = s.Height / s.Width * w
                s.Width = w
            End If
        End With
        
        If (Location.Information(wdWithInTable) = True) Then
            w = Location.Columns(1).Width
            If (w < s.Width) Then
                s.Height = s.Height / s.Width * w
                s.Width = w
            End If
                
            h = Location.Rows(1).Height
            If (h < s.Height) Then
                s.Width = s.Width / s.Height * h
                s.Height = h
            End If
        End If
    End If
    
    On Error Resume Next
    Kill exportFile
    Err.Clear
    DoEvents
End Sub
Private Sub IENWordRenderer_Render(ByVal SectionCell As SectionCell, ByVal Location As Word.Range, ByVal SubsectionRenderer As ENRenderWord9.SubsectionRenderer, ByVal ShowHistory As Boolean)
    ' ## Replace the contents of a range in a Microsoft Word document with the contents of the _
    section cell associated with this control.
    ' ##PARAM Location The range within a Microsoft Word document to replace.
    
    Dim vks() As String
    Dim r As Word.Range
    Dim doneAnything As Boolean
    Dim vIndex As Long
    Dim v As ENFramework9.Transition
    Dim col As ENFramework9.Notebook
    Dim vCell As ENFramework9.SectionCell
    Dim vTag As ENStandard9.ChemicalStructure
    Dim fieldRangeStart As Long
    Dim printingVersions As Boolean
    Dim t As Word.Table
    Dim Tag As ChemicalStructure
    Dim rangeDoc As Word.Document

    Set Tag = SectionCell.Tag
    Location.Delete
    If (Len(Tag.CDX) > 0) Then
        ExportStructureToWord Tag, Location
    End If

    If Not SectionCell.BaselineVersion Is Nothing And ShowHistory Then
        
        Set rangeDoc = GetContainerDocument(Location)
        Set r = rangeDoc.Content
        
        fieldRangeStart = r.End
        
        doneAnything = False
        Set v = SectionCell.BaselineVersion
        Set col = v.Collection
        vks = SectionCell.VersionKeys
    
        printingVersions = SectionCell.VersionCreated.Key > SectionCell.BaselineVersion.Key
        
        ' For every version of this property list equal to or later than the baseline version, print out the previous version
        For vIndex = LBound(vks) To UBound(vks)
            ' We start printing versions with the first version prior to the baseline
            If Not printingVersions And vIndex < UBound(vks) Then
                    If CLng(vks(vIndex + 1)) > SectionCell.BaselineVersion.Key Then
                        printingVersions = True
                    End If
            End If
            If printingVersions Then
                Set v = col.Transitions.Item(vks(vIndex))
                Set vCell = SectionCell.VersionSectionCell(v)
                Set vTag = vCell.Tag
                
                ' If this is the first time through, add the field name
                If Not doneAnything Then
                    doneAnything = True
                    r.Collapse wdCollapseEnd
                    r.InsertAfter vbCr & SectionCell.Field.Name
                    r.ParagraphFormat.TabStops.ClearAll
                    r.ParagraphFormat.TabStops.Add 18, wdAlignTabLeft, wdTabLeaderSpaces
                    r.ParagraphFormat.SpaceBefore = 0
                    r.ParagraphFormat.SpaceAfter = 0
                    r.Paragraphs.Last.KeepWithNext = True
                    r.Bold = False
                Else
                    r.Paragraphs.Last.KeepWithNext = False
                End If
                
                r.Collapse wdCollapseEnd
                r.InsertAfter vbCr & vbTab & v.ActionDate.Text
                r.Paragraphs.Last.KeepWithNext = True
                r.Bold = False
                r.Collapse wdCollapseEnd
                
                Set t = rangeDoc.Tables.Add(r, 1, 1, wdWord9TableBehavior, wdAutoFitContent)
                t.Borders(wdBorderTop).LineStyle = wdLineStyleNone
                t.Borders(wdBorderLeft).LineStyle = wdLineStyleNone
                t.Borders(wdBorderRight).LineStyle = wdLineStyleNone
                t.Borders(wdBorderBottom).LineStyle = wdLineStyleNone
                ExportStructureToWord vTag, t.Cell(1, 1).Range
                t.Rows(1).SetLeftIndent 36, wdAdjustNone
                Set r = rangeDoc.Content
                r.Paragraphs.Last.KeepWithNext = False
                r.Collapse wdCollapseEnd
            End If ' printingVersions
        Next vIndex
        If fieldRangeStart <> rangeDoc.Content.End Then
            Set r = rangeDoc.Range(fieldRangeStart, rangeDoc.Content.End)
            r.Font.Size = 9
        End If
    End If ' Not SectionCell.BaselineVersion Is Nothing
End Sub

Private Property Get IENFieldCtl_Object() As Object
    Set IENFieldCtl_Object = Me
End Property

Private Sub IENFieldCtl_SaveCompleted()
    
End Sub

Private Property Get IENFieldCtl_Saved() As Boolean
    IENFieldCtl_Saved = Not mDrawing.DataChanged
End Property

Private Property Get IENFieldCtl_SectionCell() As ENFramework9.SectionCell
    Set IENFieldCtl_SectionCell = mSectionCell
End Property

Private Property Set IENFieldCtl_SectionCell(ByVal SectionCell As SectionCell)
    Set mSectionCell = SectionCell
    If (mSectionCell Is Nothing) Then
        Set mTag = Nothing
    Else
        Dim oldEncoded As Boolean
        
        mSuppressContentChanged = True
        Set mTag = SectionCell.Tag
        
        
        oldEncoded = mDrawing.DataEncoded
        mDrawing.DataEncoded = True
        
        mDrawing.Objects.Clear
        If (Len(mTag.CDX) > 0) Then
            mDrawing.SourceURL = "data:chemical/x-cdx;base64," & mTag.CDX
        End If
        mDrawing.DataEncoded = oldEncoded
        
        mDrawing.DataChanged = False
        SetViewOnly
        mSuppressContentChanged = False
    End If
End Property

Private Sub SetViewOnly()
    If (mSectionCell Is Nothing) Then
        mDrawing.ViewOnly = True
    Else
        mDrawing.ViewOnly = Not mFieldCtlContainer.CanEditCell(mSectionCell)
    End If
End Sub

Private Sub IENFieldCtl_Initialize(ByVal FieldCtlContainer As IFieldCtlContainer, ByVal ObjectDisplay As IObjectDisplay)
    Set mFieldCtlContainer = FieldCtlContainer
    
    ' ISISDraw begin
    On Error GoTo CatchError
    
    Set mfrmISISServer = New frmISISServer
    Load mfrmISISServer
    mfrmISISServer.Initialize Me
    SetTempFolder
    mTempSkcFilePath = mTempFolder.Path & "\temp.skc"
    
CatchError:
    ShowError "Error initializing ISIS/Draw"
    ' ISISDraw end
End Sub

Private Sub IENFieldCtl_Terminate()
    Set mFieldCtlContainer = Nothing

    ' ISISDraw begin
    On Error GoTo CatchError
    If Not mfrmISISServer Is Nothing Then
        Unload mfrmISISServer
        Set mfrmISISServer = Nothing
    End If
CatchError:
    ShowError "Error terminating ISIS/Draw"
    ' ISISDraw end
End Sub

Public Property Get ChemicalStructure() As ENStandard9.ChemicalStructure
Attribute ChemicalStructure.VB_HelpID = 25618
    Set ChemicalStructure = mTag
End Property

Public Property Get Data(ByVal Format As String) As Variant
Attribute Data.VB_HelpID = 25619
    Dim oldEncoded As Boolean
    If (InStr(mDrawing.Version, " Net") > 0) Then
        Err.Raise vbObjectError + 513, "ChemicalStructureCtl.IENFieldCtl_SectionCell", "chemical structures can not be saved using the ChemDraw Net Control. Please activate ChemDraw using a serial number for ChemDraw Pro Control"
    End If
        
    oldEncoded = mDrawing.DataEncoded
    mDrawing.DataEncoded = True
    Data = mDrawing.Data(Format)
    mDrawing.DataEncoded = oldEncoded
End Property

Private Function IsGroupBelow(ByVal targetG As ChemDrawControl9Ctl.Group, ByVal rs As ChemDrawControl9Ctl.ReactionScheme) As Boolean
    Dim rStep As ChemDrawControl9Ctl.ReactionStep
    Dim g As ChemDrawControl9Ctl.Group
    For Each rStep In rs.ReactionSteps
        For Each g In rStep.GroupsBelowArrow
            If (g.ID = targetG.ID) Then
                IsGroupBelow = True
                Exit Function
            End If
        Next g
    Next rStep
    
    IsGroupBelow = False
End Function

Public Function ExtractComponents() As ReactionComponent()
    Dim retval() As ReactionComponent
    
    Dim rs As ChemDrawControl9Ctl.ReactionScheme
    Dim rStep As ChemDrawControl9Ctl.ReactionStep
    Dim g As ChemDrawControl9Ctl.Group
    Dim iReactants%, iProducts%, iIndex%
    Dim iSolvents As Long
    Dim j As Long

    For Each rs In mDrawing.ReactionSchemes
        iReactants = iReactants + rs.Reactants.count
        iProducts = iProducts + rs.Products.count
    Next rs
    If iReactants > 0 Or iProducts > 0 Then
        ReDim retval(1 To iReactants + iProducts)
        ReDim retval(1 To iReactants + iProducts + iSolvents)
        
        iIndex = 1
        For Each rs In mDrawing.ReactionSchemes
            For Each g In rs.Reactants
                    j = iIndex - 1
                    Do While (j >= 1)
                        If (retval(j).Top >= g.Bounds.Bottom) Then
                            Set retval(j + 1) = retval(j)
                            j = j - 1
                        ElseIf (retval(j).Bottom <= g.Bounds.Top) Then
                            Exit Do
                        ElseIf (retval(j).Left >= g.Bounds.Right) Then
                            Set retval(j + 1) = retval(j)
                            j = j - 1
                        ElseIf (retval(j).Right <= g.Bounds.Left) Then
                            Exit Do
                        ElseIf (retval(j).Left + retval(j).Right >= g.Bounds.Left + g.Bounds.Right) Then
                            Set retval(j + 1) = retval(j)
                            j = j - 1
                        Else
                            Exit Do
                        End If
                    Loop
                    j = j + 1
                    With g.Objects
                        Set retval(j) = New ReactionComponent
                        retval(j).MolecularWeight = .MolecularWeight / 1000# ' Convert to SI units (kg/mol)
                        retval(j).Formula = .Formula
                        retval(j).IsReactant = True
                        retval(j).IsProduct = False
                        retval(j).IsSolvent = False
                        retval(j).ID = g.ID
                        retval(j).Top = g.Bounds.Top
                        retval(j).Bottom = g.Bounds.Bottom
                        retval(j).Right = g.Bounds.Right
                        retval(j).Left = g.Bounds.Left
                        retval(j).Code = g.Objects.Data("chemical/x-smiles")
                    End With
                    iIndex = iIndex + 1
            Next g
            
            For Each g In rs.Products
                j = iIndex - 1
                Do While (j >= 1)
                    If (Not retval(j).IsProduct) Then
                        Exit Do
                    ElseIf (retval(j).Top >= g.Bounds.Bottom) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    ElseIf (retval(j).Bottom <= g.Bounds.Top) Then
                        Exit Do
                    ElseIf (retval(j).Left >= g.Bounds.Right) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    ElseIf (retval(j).Right <= g.Bounds.Left) Then
                        Exit Do
                    ElseIf (retval(j).Left + retval(j).Right >= g.Bounds.Left + g.Bounds.Right) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    Else
                        Exit Do
                    End If
                Loop
                j = j + 1
                With g.Objects
                    Set retval(j) = New ReactionComponent
                    retval(j).MolecularWeight = .MolecularWeight / 1000# ' Convert to SI units (kg/mol)
                    retval(j).Formula = .Formula
                    retval(j).IsReactant = False
                    retval(j).IsProduct = True
                    retval(j).IsSolvent = False
                    retval(j).ID = g.ID
                    retval(j).Top = g.Bounds.Top
                    retval(j).Bottom = g.Bounds.Bottom
                    retval(j).Right = g.Bounds.Right
                    retval(j).Left = g.Bounds.Left
                    retval(j).Code = g.Objects.Data("chemical/x-smiles")
                End With
                iIndex = iIndex + 1
            Next g
        Next rs
        
        Dim b As ChemDrawControl9Ctl.Graphic
        For Each b In mDrawing.Graphics
            If (b.IsBracket) Then
                If (b.BracketUsage = kCDBracketUsageMultipleGroup) Then
                    ' Need to get the repeat count and the containing group
                End If
            End If
        Next b
    Else
        ReDim retval(0, 0)
        ' Err.Raise vbObjectError + 513, , "the drawing can not be interpreted as a reaction"
    End If
    
    ExtractComponents = retval
End Function

Private Sub IENFieldCtl_ShowGrab()

End Sub

Private Sub IENFieldCtl_ShowRelease()

End Sub

Private Sub IENFieldCtl_ShowPath(ByVal Path As MSXML2.IXMLDOMElement)

End Sub

Private Sub ValidateDrawing()
    If mDrawing.DataChanged Then
        If (InStr(mDrawing.Version, " Net") > 0) Then
            Err.Raise vbObjectError + 513, "ChemicalStructureCtl.IENFieldCtl_SectionCell", "chemical structures can not be saved using the ChemDraw Net Control. Please activate ChemDraw using a serial number for ChemDraw Pro Control"
        End If
        
        mTag.CDX = mDrawing.Data("cdxml")
        mDrawing.DataChanged = False
    End If
End Sub

Private Sub IENFieldCtl_Validate()
    If mDataChanged Then
        mFieldCtlContainer.StartChangeMonitor
        
        RaiseBeforeDataChanged
        mFieldCtlContainer.ContentCaching mSectionCell.Tag.object
        ValidateDrawing
        mFieldCtlContainer.ContentChanged mSectionCell.Tag.ENObject
                
        RaiseAfterDataChanged
        mFieldCtlContainer.FinishChangeMonitor
    End If
End Sub

Public Sub PrepareValidateMenus()
Attribute PrepareValidateMenus.VB_Description = "Do whatever is necessary before validating menu items."
Attribute PrepareValidateMenus.VB_HelpID = 25620
    ' Do whatever is necessary before validating menu items.
    mDrawing.UpdateMenus
End Sub

Public Property Get CChemicalStructureCtl() As CChemicalStructureCtl
Attribute CChemicalStructureCtl.VB_HelpID = 25621
    Dim cc As New CChemicalStructureCtl
    Set cc.ChemicalStructureCtl = Me
    Set CChemicalStructureCtl = cc
End Property

Private Sub ShowError(Optional ByVal ErrTitle As String = "Chemical Structure Control Error")
    ' ## Helper function to display an error message with an appropriate title when an error occurs in this module.
    ' ##PARAM ErrTitle The title of the message box to be displayed.
    ' ##REMARKS Processing Logic: _
    <ol> _
    <li>Send the current error, the error title and the connection associated with the current section cell (if any) _
        to the ErrMsgBox routine to  display the message.</li> _
    <li>Reset the pointer for this control to the default pointer.</li> _
    <ol>
    
    If (mSectionCell Is Nothing) Then
        ErrorMsgBox Err, ErrTitle, Nothing
    Else
        ErrorMsgBox Err, ErrTitle, mSectionCell.Connection
    End If
    UserControl.MousePointer = vbDefault
End Sub

Private Sub IENTOCFieldCtl_ExportImage(ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, ByVal Connection As ENFramework9.Connection, ByVal imageElement As MSXML2.IXMLDOMElement, ByVal ExportType As String, ByVal Location As Variant)
    ' ## Compute the contents of the corresponding field as an image and/or as text.
    ' ##PARAM FieldCtlContainer The framework object used to display all of the contents of sections.
    ' ##PARAM Connection The object used to communicate between the client tier, the business tier and the database tier.
    ' ##PARAM imageElement The contents are to be displayed in the image.
    Dim cDataNode As IXMLDOMCDATASection
    Dim exportFile As String
    Dim fso As New FileSystemObject
    Dim s As InlineShape
    Dim w As Single
    Dim f As New frmEditStructure
    
    Set cDataNode = imageElement.firstChild
    Debug.Assert (Not cDataNode Is Nothing)
    If (Len(cDataNode.Data) > 0) Then
        Dim FilePath As String
        Dim oldEncoded As Boolean
        Dim oldAnitaliasedGIFs As Boolean
        Dim oldGraphicsOutputBorder As Long
        
        mSuppressContentChanged = True
        oldEncoded = mDrawing.DataEncoded
        mDrawing.DataEncoded = True
        
        mDrawing.Objects.Clear
        If (Len(cDataNode.Data) > 0) Then
            mDrawing.SourceURL = "data:chemical/x-cdx;base64," & cDataNode.Data
        End If
        
        exportFile = GetTempFile("emf")
        
        f.Initialize Connection
        f.CDXStructure = cDataNode.Data
        f.WriteStructurePicture exportFile, "image/x-emf", 40
            
        Set s = Location.InlineShapes.AddPicture(FileName:=exportFile, _
            Range:=Location, LinkToFile:=False, SaveWithDocument:=True)
    
        ' If necessary, scale the new shape.
        If (Err.Number = 0) Then
            With Location.PageSetup
                w = .PageWidth - .RightMargin - .LeftMargin
                If (w < s.Width) Then
                    s.Height = s.Height / s.Width * w
                    s.Width = w
                End If
            End With
        End If
        
        On Error Resume Next
        Kill exportFile
        Err.Clear
        DoEvents
    End If
End Sub

Private Sub mDrawing_DataChanged()
    On Error GoTo CatchError
    If (Not mSectionCell Is Nothing) Then
        mSectionCell.Connection.WriteLogStream "==>> Standard Add-ins: ChemicalStructureCtl.mDrawing_DataChanged: Drawing Changed"
                        
        MonitorDataChanged True
    End If
    
CatchError:
    Dim oldEncoded As Boolean
    
    If (Err.Number <> 0) Then
        ShowError "Error Handling Change In Chemical Structure"
        mSuppressContentChanged = True
        
        oldEncoded = mDrawing.DataEncoded
        mDrawing.DataEncoded = True
        
        mDrawing.Objects.Clear
        If (Len(mTag.CDX) > 0) Then
            mDrawing.SourceURL = "data:chemical/x-cdx;base64," & mTag.CDX
        End If
        mDrawing.DataEncoded = oldEncoded
        
        mSuppressContentChanged = False
    End If
End Sub

Private Sub mDrawing_SelectionChanged(ByVal selection As ChemDrawControl9Ctl.IChemDrawSelection)
    Dim i As Long
    Dim csl As IENChemStructureListener
    
    On Error GoTo CatchError
    
    If (Not mSectionCell Is Nothing) Then
        With mSectionCell.Field.FieldListeners
            For i = 1 To .count
                If (TypeOf .Item(i) Is IENChemStructureListener) Then
                    Set csl = .Item(i)
                    csl.SelectionChanged mFieldCtlContainer, mSectionCell.Tag.object
                End If
            Next i
        End With
    End If
    
CatchError:
    ShowError "Error Changing Selection"
End Sub

Private Sub mDrawing_StatusChanged(ByVal status As String)
    Dim i As Long
    Dim csl As IENChemStructureListener
    
    On Error GoTo CatchError
    
    If (Not mSectionCell Is Nothing) Then
        With mSectionCell.Field.FieldListeners
            For i = 1 To .count
                If (TypeOf .Item(i) Is IENChemStructureListener) Then
                    Set csl = .Item(i)
                    csl.StatusChanged mFieldCtlContainer, mSectionCell.Tag.object
                End If
            Next i
        End With
    End If
    
CatchError:
    ShowError "Error Changing Status"
End Sub

Private Sub mDrawing_ToolChanged(ByVal toolType As ChemDrawControl9Ctl.CDToolType, ByVal subType As Integer)
    Dim i As Long
    Dim csl As IENChemStructureListener
    
    On Error GoTo CatchError
        
    If (Not mSectionCell Is Nothing) Then
        With mSectionCell.Field.FieldListeners
            For i = 1 To .count
                If (TypeOf .Item(i) Is IENChemStructureListener) Then
                    Set csl = .Item(i)
                    csl.ToolChanged mFieldCtlContainer, mSectionCell.Tag.object
                End If
            Next i
        End With
    End If
    
CatchError:
    ShowError "Error Changing Tool"
End Sub

Private Sub UserControl_EnterFocus()
    On Error GoTo CatchError
    
    If (Not mSectionCell Is Nothing) Then
        mSectionCell.Field.FieldListeners.EnterFocus mFieldCtlContainer, mSectionCell
    End If
    
CatchError:
    ShowError "Error Entering Focus on Chemical Structure Control"
End Sub

Private Sub UserControl_ExitFocus()
    On Error GoTo CatchError
    
    IENFieldCtl_Validate
    If (Not mSectionCell Is Nothing) Then
        mSectionCell.Field.FieldListeners.ExitFocus mFieldCtlContainer, mSectionCell
    End If
    
CatchError:
    ShowError "Error Exiting Focus on Chemical Structure Control"
End Sub

Private Sub UserControl_GotFocus()
    On Error GoTo CatchError
    If (mDrawing.Visible) Then
        mDrawing.SetFocus
    End If
CatchError:
    ShowError "Error Establishing Focus in Chemical Structure Control"
End Sub

Private Sub UserControl_Hide()
    On Error GoTo CatchError
    mDrawing.Visible = False

    If (Not mSectionCell Is Nothing) Then
        mSectionCell.Field.FieldListeners.Hide mFieldCtlContainer, mSectionCell
    End If
    
CatchError:
    ShowError "Error Hiding Chemical Structure Control"
End Sub

Private Sub UserControl_KeyDown(KeyCode As Integer, Shift As Integer)
    On Error GoTo CatchError

    If (KeyCode = vbKeyDelete And mSectionCell.IsEditable) Then
        mSectionCell.Connection.WriteLogStream "==>> Standard Add-ins: ChemicalStructureCtl.UserControl_KeyDown(KeyCode=vbKeyDelete): Delete Key Pressed"
                        
        If (mDrawing.selection.Objects.count > 0) Then
            On Error Resume Next
            mFieldCtlContainer.ContentChanging mSectionCell.Tag.ENObject
            If (Err.Number <> 0) Then
                ShowError "Error Deleting"
            Else
                On Error GoTo CatchError
                mFieldCtlContainer.ContentCaching mSectionCell.Tag.ENObject
                If (mSectionCell.Tag.Saved) Then
                    mFieldCtlContainer.ContentChanged mSectionCell.Tag.ENObject
                End If
                mDrawing.selection.Objects.Clear
                DataChanged
                KeyCode = 0
            End If
        End If
    End If
    
CatchError:
    ShowError "Error Pressing Key Down"
End Sub

Private Sub UserControl_Resize()
    If (Not mSectionCell Is Nothing) Then
        mSectionCell.Connection.WriteLogStream "==>> Standard Add-ins: ChemicalStructureCtl.UserControl_Resize: Control Resized"
    End If
    
    On Error GoTo CatchError
    mDrawing.Move 0, 0, UserControl.Width, UserControl.Height
    
    If (Not mSectionCell Is Nothing) Then
        mSectionCell.Field.FieldListeners.Resize mFieldCtlContainer, mSectionCell
    End If
    
CatchError:
    ShowError "Error Resizing Chemical Structure Control"
End Sub

Private Sub UserControl_Show()
    On Error GoTo CatchError
    
    mDrawing.Visible = True
    If Not UserControl.Ambient.UserMode Then
        mDrawing.ViewOnly = True
    End If
    
    If (Not mSectionCell Is Nothing) Then
        mSectionCell.Field.FieldListeners.Show mFieldCtlContainer, mSectionCell
    End If
    
CatchError:
    ShowError "Error Showing Chemical Structure Control"
End Sub

Private Sub UserControl_Terminate()
    On Error Resume Next
    Set mTag = Nothing
    Set mSectionCell = Nothing
    Set mFieldCtlContainer = Nothing
    ShowError "Error Terminating Chemical Structure Control"
End Sub

Public Sub IENTOCFieldCtl_ComputeImage(ByVal FieldCtlContainer As IFieldCtlContainer, _
    ByVal Connection As ENFramework9.Connection, _
    ByVal imageElement As IXMLDOMElement, _
    ByRef newPicture As IPictureDisp, ByRef newTitle As String)
    ' ## Compute the contents of the corresponding field as an image and/or as text.
    ' ##PARAM FieldCtlContainer The framework object used to display all of the contents of sections.
    ' ##PARAM Connection The object used to communicate between the client tier, the business tier and the database tier.
    ' ##PARAM imageElement The contents are to be displayed in the image.
    ' ##PARAM newPicture On exit, the image that represents the imageElement data. On entry, Nothing.
    ' ##PARAM newTitle On exit, the text that represents the imageElement data. On entry, a 0 length string.
    Dim cDataNode As IXMLDOMCDATASection
    
    Set cDataNode = imageElement.firstChild
    Debug.Assert (Not cDataNode Is Nothing)
    If (Len(cDataNode.Data) > 0) Then
        Dim oldEncoded As Boolean
        Dim oldAnitaliasedGIFs As Boolean
        Dim oldGraphicsOutputBorder As Long
        
        mSuppressContentChanged = True
        oldEncoded = mDrawing.DataEncoded
        mDrawing.DataEncoded = True
        
        mDrawing.Objects.Clear
        If (Len(cDataNode.Data) > 0) Then
            mDrawing.SourceURL = "data:chemical/x-cdx;base64," & cDataNode.Data
        End If
        
        mDrawing.DataChanged = False
        mSuppressContentChanged = False
            
        
        ' Get the bitmap, making sure that it isn't encoded.
        mDrawing.DataEncoded = False
        oldAnitaliasedGIFs = mDrawing.Application.Preferences.AntialiasedGIFs
        oldGraphicsOutputBorder = mDrawing.Application.Preferences.GraphicsOutputBorder
        mDrawing.Application.Preferences.AntialiasedGIFs = False
        mDrawing.Application.Preferences.GraphicsOutputBorder = 40
        
        Set newPicture = mDrawing.Objects.Bitmap
        
        mDrawing.DataEncoded = oldEncoded
        mDrawing.Application.Preferences.AntialiasedGIFs = oldAnitaliasedGIFs
        mDrawing.Application.Preferences.GraphicsOutputBorder = oldGraphicsOutputBorder
        
    End If

End Sub

' ISISDraw begin
Private Sub mDrawing_DblClick(Cancel As Boolean)
    
    On Error GoTo CatchError
    
    mDrawing.SaveAs mTempSkcFilePath, "skc"
    mfrmISISServer.OLE1.CreateLink mTempSkcFilePath
    mfrmISISServer.OLE1.DoVerb vbOLEOpen

CatchError:
    ShowError "Error Loading ISIS/Draw"
End Sub

Private Sub DeleteFile(ByVal Path As String)
    Dim fso As FileSystemObject
    Set fso = New FileSystemObject
    fso.DeleteFile Path
End Sub


Friend Sub ClearAndLoad()
    
    On Error GoTo CatchError
    
    mDrawing.Objects.Clear
    mDrawing.Open mTempSkcFilePath, True
    DeleteFile mTempSkcFilePath
'    MarkDataChanged
    mDrawing_DataChanged

CatchError:
    ShowError "Error loading structure from ISIS/Draw"
End Sub

Private Sub mDrawing_GotFocus()
    mDrawing.ViewOnly = True
End Sub

Public Sub SetTempFolder()
    'This function creates two folder in the user temp directory
    '...\E-Notebook 9.0\ChemicalStructure
    
    Dim strPath As String
    Dim fso As FileSystemObject
    Dim tempFolderConst As SpecialFolderConst
    Dim tempFolder As Folder
    Dim eNotebookFolder As Folder
    Dim f As File
    
    Set fso = New FileSystemObject
    tempFolderConst = TemporaryFolder
    
    'Get a reference to the temporary folder
    Set tempFolder = fso.GetSpecialFolder(tempFolderConst)
    
    If Not (tempFolder Is Nothing) Then
        If Not (fso.FolderExists(tempFolder.Path & "\E-Notebook 9.0\ChemicalStructure")) Then
    
            'Check if the "E-Notebook 9.0" folder exist. If it's not found create it
            strPath = tempFolder.Path & "\" & "E-Notebook 9.0"
            If Not (fso.FolderExists(strPath)) Then
                Set eNotebookFolder = fso.CreateFolder(strPath)
            Else
                Set eNotebookFolder = fso.GetFolder(strPath)
            End If
        
            'Create the "ReactionExplorer" sub folder
            Set mTempFolder = fso.CreateFolder(strPath & "\ChemicalStructure")
        Else
            Set mTempFolder = fso.GetFolder(tempFolder.Path & "\E-Notebook 9.0\ChemicalStructure")
        End If
        
        If Not (mTempFolder Is Nothing) Then
            'Delete all files in this folder
            For Each f In mTempFolder.Files
                f.Delete True
            Next
        Else
            Err.Raise 513, "Chemical Structure control", "The Chemical Structure control cannot be used because a required temporary folder cannot be determined."
        End If
    Else
        Err.Raise 513, "Chemical Structure control", "The Chemical Structure control cannot be used because the system temporary folder cannot be determined."
    End If
    
    Set fso = Nothing
    Set tempFolder = Nothing
    Set eNotebookFolder = Nothing
End Sub

' ISISDraw end


