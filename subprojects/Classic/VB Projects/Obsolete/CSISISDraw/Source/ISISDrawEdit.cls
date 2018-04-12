VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ISISDrawEdit"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Implements IObjectSafety

Public Event ISISEditChanges(ByVal cdxml As String)

' ## A form containing the MDL Form Box control
Private WithEvents mfStructure As frmStructure
Attribute mfStructure.VB_VarHelpID = -1

' # A form containing a one-shot timer.
Private mfrmISISDrawEditTimer As frmISISDrawEditTimer

Private mPreserveDataSGroups As Boolean

Private mEditedStructure As String

' # A flag to indicate whether we were able to parse the reaction unambiguously (or not).
Private mRxnParsingOK As Boolean

Private Const HKEY_CURRENT_USER = &H80000001

' Code no longer used:
'Private Type PROCESSENTRY32
'  dwSize As Long
'  cntUsage As Long
'  th32ProcessID As Long
'  th32DefaultHeapID As Long
'  th32ModuleID As Long
'  cntThreads As Long
'  th32ParentProcessID As Long
'  pcPriClassBase As Long
'  dwFlags As Long
'  szExeFile As String * 260
'End Type
'
'Private Declare Function CreateToolhelp32Snapshot Lib "kernel32" (ByVal dwFlags As Long, ByVal th32ProcessID As Long) As Long
'Private Declare Function Process32First Lib "kernel32" (ByVal hSnapshot As Long, lppe As PROCESSENTRY32) As Long
'Private Declare Function Process32Next Lib "kernel32" (ByVal hSnapshot As Long, lppe As PROCESSENTRY32) As Long
'Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal Handle As Long) As Long
'Private Declare Function OpenProcess Lib "kernel32.dll" (ByVal dwDesiredAccessas As Long, ByVal bInheritHandle As Long, ByVal dwProcId As Long) As Long
'Private Declare Function GetCurrentProcessId Lib "kernel32" () As Long
'Private Declare Function WaitForSingleObject Lib "kernel32" (ByVal hHandle As Long, ByVal dwMilliseconds As Long) As Long
'
'Private Const SYNCHRONIZE = &H100000
'Private Const WAIT_FAILED = &HFFFFFFFF
'Private Const WAIT_TIMEOUT = &H102
'Private Const TH32CS_SNAPPROCESS = &H2

Public Function ISISEdit(ByVal cdxml As String, Optional PreserveDataSGroups As Boolean = False) As String
    Dim frm As frmModalEditDialog
    Dim Result As Long
    Dim Message As String
    Dim i As Long

    If (Not gEditInProcess) Then
        gEditInProcess = True
        Do
            ' Fix bug CSBR-57915. Enable PreserveDataSGroups for this case.  It is necessary (and does no harm) to
            '   execute the Data Sgroup code even in cases where there aren't any Data Sgroups.  (If not, we
            '   never cpmpute NReactants and NProducts, for one thing.)
            ' mPreserveDataSGroups = PreserveDataSGroups
            mEditedStructure = ""
            mPreserveDataSGroups = True
            If Not mfStructure Is Nothing Then
                Unload mfStructure ' We have no easy way to tear this down when we're done, so we tear it down before the next use.
                Set mfStructure = Nothing
            End If
            Set mfStructure = New frmStructure
            Call mfStructure.Initialize("") ' (Argument is no longer used.)
            
            ' ****** KICK OFF ISIS/DRAW EDITING ******
            ' This timer allows a double-click event to finish before the editing.
            Set mfrmISISDrawEditTimer = New frmISISDrawEditTimer
            Call mfrmISISDrawEditTimer.Initialize(Me, cdxml)
            mfrmISISDrawEditTimer.Timer1.Interval = 10 ' This will call DoEdit after a necessary 10 ms delay.
            
            ' ******* WAIT UNTIL EDIT COMPLETION *******
            Set frm = New frmModalEditDialog
            frm.Show vbModal ' This waits for the edit to be done, and prevents the user from clicking elsewhere.
            Set frm = Nothing
            
            DoEvents
            Wait (100)
            DoEvents
            
            ' ******** EDIT IS COMPLETE ********
            If mEditedStructure <> "" Then
                If mRxnParsingOK Then
                    ISISEdit = mEditedStructure
                Else
                    ' Try again?
                    Message = "The reaction could not be interpreted because the placing of molecules is ambiguous." & vbCrLf & _
                              "Please place molecules so they are completely left, right, above, or below the arrow." & vbCrLf & _
                              "Click Retry to continue editing, or Cancel to discard the edit."
                    Result = MsgBox(Message, vbQuestion + vbRetryCancel, "E-Notebook ISIS/Draw Interface")
                    If (Result = vbRetry) Then
                        cdxml = mEditedStructure ' Prepare to re-edit the structure we just edited.
                    Else
                        ISISEdit = cdxml ' No editing result -- return input structure
                        mRxnParsingOK = True
                    End If
                End If
            Else
                ISISEdit = cdxml ' No editing result -- return input structure
            End If
            ' Tear-down:
            Unload mfStructure ' We have no easy way to tear this down when we're done, so we tear it down before the next use.
            Set mfStructure = Nothing
        Loop Until ((mRxnParsingOK = True) Or mEditedStructure = "")
        gEditInProcess = False
    Else
        MsgBox "ISIS/Draw editing is already in process.   More than one editing session is not allowed.", _
            vbOKOnly, "ISIS/Draw Edit Interface"
        ISISEdit = cdxml ' No editing result -- return input structure
    End If
End Function

' # Function called by one-shot timer.
Friend Sub DoEdit(ByVal cdxml As String)
    Dim f As New frmChemDraw
    Dim dsd() As DataSgroupData
    Dim NReactants As Long
    Dim NProducts As Long
    Dim vStruct As Variant
    Dim StyleSheetPath As String
    Dim i As Long
    Dim MWReactant1 As Double
    Dim MWProduct1 As Double
    
    ' Debug code:
    Call DebugWriteFiles("Structure coming in to ISISEdit", cdxml)
    
    With f.ChemDrawCtl1
        .Objects.Clear
        .DataEncoded = False
        
        ' Apply style sheet.  Note:  This code was developed to fix CSBR-57917, but was not in fact needed.  It is
        '                            left here for future reference.
        'StyleSheetPath = GetStyleSheetPath
        '.Open StyleSheetPath, False
        
        ' Load data.
        If (Len(cdxml) > 0) Then
            .SourceURL = "data:chemical/x-cdx;base64," & cdxml
        End If
        'old code: .Data("cdxml") = cdxml  Note: Replaced by above .SourceURL code to fix CSBR-57917 and CSBR-57942.
        
        If mPreserveDataSGroups Then
            ' Extract Data Sgroups From the stored structure, and get NReactants, NProducts.
            dsd = ExtractDataSgroupsFromChemDraw(f.ChemDrawCtl1, NReactants, NProducts, MWReactant1, MWProduct1)
        End If
        Call DebugWriteFiles("Structure after ExtractDataSgroupsFromChemDraw", cdxml)
        ' Fix bug CSBR-56843.  Remove AtomNumbers that are not shown.
        For i = 1 To .Atoms.Count
            If (.Atoms(i).Settings.ShowAtomNumber = False) Then
                .Atoms(i).AtomNumber = ""
            End If
        Next i
        
        .DataEncoded = False
        ' Extract the structure in ISIS format to send to ISIS/Draw.
        If .Objects.Count > 0 Then
            vStruct = .Data("skc")
        Else
            vStruct = ""
        End If
    End With

    Set f = Nothing

    ' Launch ISIS/Draw to edit structure.  The result is returned in the event below.
    Call mfStructure.EditStructure(vStruct, "skc", "skc", dsd, NReactants, NProducts, MWReactant1, MWProduct1)
End Sub

' # Event raised to return data from ISIS/Draw editing session.
' # Note: This is only raised if there were changes made in the editing session.

Private Sub mfStructure_EditReady(ByVal vStruct As Variant, ByRef dsd() As DataSgroupData)
    Dim f As New frmChemDraw
    Dim DX As Double, DY As Double
    Dim convFormat As ConvertFormat
    Dim var As Variant
    Dim cdxml As String
    Dim oldEncoded As Boolean
    Dim bStruct() As Byte
    Dim NReactants As Long
    Dim NProducts As Long
    
    On Error GoTo CatchError

    With f.ChemDrawCtl1
        .Objects.Clear
        oldEncoded = .DataEncoded
        .DataEncoded = False
        
        bStruct = vStruct ' Convert to byte array.
        ' Load data and settings into ChemDraw.
        .SourceURL = "data:chemical/x-mdl-isis;base64," & Base64Encode(bStruct)
        
        .DataEncoded = oldEncoded
        
        If mPreserveDataSGroups Then
            Call SaveDataSgroupsInChemDraw(f.ChemDrawCtl1, dsd, NReactants, NProducts)
        End If
        
        cdxml = .Data("text/xml")
        
        'Call NarrowArrow(f.ChemDrawCtl1)  ' Fix CSBR-59058: Reaction arrow is displayed very thick
        cdxml = NarrowArrowCDxml(cdxml)  ' Fix CSBR-59058: Reaction arrow is displayed very thick
        
        ' Test to see if two different methods of determining reactants and products agree:
        ' (1) .ReactionSchemes(1).Reactants.Count and .ReactionSchemes(1).Products.Count are
        '       from the ChemDraw reaction parser.
        ' (2) NReactants and NProducts are found by which molecules are to the left and right
        '       of the arrowhead respectively.
        If (.ReactionSchemes.Count > 0) Then
            If ((.ReactionSchemes(1).Reactants.Count = NReactants) And _
                (.ReactionSchemes(1).Products.Count = NProducts)) Then
                mRxnParsingOK = True
            Else
                mRxnParsingOK = False
            End If
        Else
            mRxnParsingOK = True
        End If
    End With
    
    Set f = Nothing
        
    ' Debug code:
    'Call DebugWriteFiles("Structure returned from ISISEdit", cdxml)
    
    mEditedStructure = cdxml
CatchError:
    'ShowError "Error returning from CSISISDraw"
End Sub

'Note:  This code was developed to fix CSBR-57917, but was not in fact needed.  It is
'       left here for future reference.
Private Function GetStyleSheetPath() As String
    ' Style sheet path is in registry.
    'TODO: Figure out how to make the code work on ChemDraw 10, etc.
    GetStyleSheetPath = GetRegKeyValue(HKEY_CURRENT_USER, "Software\CambridgeSoft\ChemDraw\9.0\General", "Last used Stationery")
End Function

' Code no longer used:
'Public Function GetISISDrawChildPID() As Long
'    Const ISISDrawApp As String = "IDraw32.exe"
'    Dim OurPID As Long
'
'    Dim aPID() As Long
'    Dim lProcesses As Long
'    Dim lProcess As Long
'    Dim lModule As Long
'    Dim sName As String
'    Dim iIndex As Integer
'    Dim bCopied As Long
'    Dim lSnapShot As Long
'    Dim tPE As PROCESSENTRY32
'    Dim bSuccess As Boolean
'    Dim ProcessID As Long
'    Dim ParentPID As Long
'
'    OurPID = GetCurrentProcessId
'
'    lSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
'    If lSnapShot >= 0 Then
'        tPE.dwSize = Len(tPE)
'        bCopied = Process32First(lSnapShot, tPE)
'        Do While bCopied
'            sName = Left$(tPE.szExeFile, InStr(tPE.szExeFile, Chr(0)) - 1)
'            sName = mid(sName, InStrRev(sName, "\") + 1)
'            If InStr(sName, Chr(0)) Then
'                sName = Left(sName, InStr(sName, Chr(0)) - 1)
'            End If
'            bCopied = Process32Next(lSnapShot, tPE)
'            If StrComp(ISISDrawApp, sName, vbTextCompare) = 0 Then
'                ProcessID = tPE.th32ProcessID
'                If tPE.th32ParentProcessID = OurPID Then
'                    bSuccess = True
'                    Exit Do
'                End If
'            End If
'        Loop
'    Else
'        ' Raise error
'    End If
'    If bSuccess Then
'        GetISISDrawChildPID = ProcessID
'    End If
'End Function

Private Sub NarrowArrow(cd As ChemDrawControl9Ctl.ChemDrawCtl)
    Dim ReactionsCount As Long
    Dim NumArrows As Long
    Dim cdxml As String

    With cd
        ReactionsCount = .ReactionSchemes.Count
        If ReactionsCount > 0 Then
            NumArrows = .ReactionSchemes(1).Arrows.Count
            If NumArrows > 0 Then
                If NumArrows > 1 Then
                    MsgBox "Error - only one arrow is supported per reaction.", vbOKOnly, "ISIS/Draw Interface"
                Else
                    ' The following statement isn't taking effect.
                    ' TODO: Find out how to un-bold this arrow, or to delete it and create a new one programmatically.
                    '.ReactionSchemes(1).Arrows(1).IsBold = False
                End If
            End If
        End If
    End With
End Sub

' Low-level editing of the cdxml to make the arrow narrow.
Private Function NarrowArrowCDxml(cdxml As String) As String
    Dim CDxmlDOM As MSXML2.DOMDocument40
    Dim ret As Boolean
    Dim pageNode As MSXML2.IXMLDOMElement
    Dim oNode As MSXML2.IXMLDOMElement
    Dim oNodeList As MSXML2.IXMLDOMNodeList
    Dim Count As Integer
    Dim i As Long
    Dim j As Long
    'Dim attr As xx

    NarrowArrowCDxml = cdxml ' In case of an error (like no arrow present), return the input.
    On Error GoTo CatchError
    Set CDxmlDOM = New MSXML2.DOMDocument40
    CDxmlDOM.async = False
    CDxmlDOM.validateOnParse = False
    ret = CDxmlDOM.loadXML(cdxml)
    Set pageNode = CDxmlDOM.selectSingleNode("//page")
    Set oNodeList = pageNode.getElementsByTagName("graphic")
    For Each oNode In oNodeList
        Count = oNode.Attributes.length
        For i = 0 To Count - 1
            If (oNode.Attributes(i).baseName = "ArrowType") Then
                For j = 0 To Count - 1
                    If (oNode.Attributes(j).baseName = "LineType") Then
                        oNode.Attributes(j).nodeValue = ""
                    End If
                    If (oNode.Attributes(j).baseName = "HeadSize") Then
                        oNode.Attributes(j).nodeValue = "1000"
                    End If
                Next j
            End If
        Next i
    Next oNode
    NarrowArrowCDxml = CDxmlDOM.XML
CatchError:
End Function

Private Sub IObjectSafety_GetInterfaceSafetyOptions(ByVal riid As Long, pdwSupportedOptions As Long, pdwEnabledOptions As Long)
    GetDefaultInterfaceSafetyOptions riid, pdwSupportedOptions, pdwEnabledOptions
End Sub

Private Sub IObjectSafety_SetInterfaceSafetyOptions(ByVal riid As Long, ByVal dwOptionsSetMask As Long, ByVal dwEnabledOptions As Long)
    Exit Sub    ' return success
End Sub


