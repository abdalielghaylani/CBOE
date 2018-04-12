VERSION 5.00
Begin VB.Form frmModalEditDialog 
   Caption         =   "ISIS/Draw editing status"
   ClientHeight    =   1155
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   4680
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   ScaleHeight     =   1155
   ScaleWidth      =   4680
   StartUpPosition =   1  'CenterOwner
   Begin VB.Label mStatus 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   495
      Left            =   240
      TabIndex        =   0
      Top             =   360
      Width           =   4215
   End
End
Attribute VB_Name = "frmModalEditDialog"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub Form_Activate()
        WaitForEditDone ' Uses Windows APIs to wait until user closes ISIS/Draw
        Me.Hide
End Sub

Public Function WaitForEditDone()
    Const LAUNCH_WAIT As Long = 100 ' ms.
    Const ISIS_LAUNCH_TIMEOUT As Double = 60#  ' seconds
    
    Const EDIT_WAIT As Long = 100 ' ms.
    Const EDIT_WAIT_TIMEOUT As Double = 100000#  ' hours.

    Dim IDrawPID As Long
    Dim EditWaitTime As Double
    Dim hProcess As Long
    Dim WaitSeconds As Double
    Dim LaunchWaitTime As Double
    Dim bSuccess As Boolean
    Dim EditWindowExists As Boolean
    Dim EditChildWindowExists As Boolean
    Dim TitleBarSubstring As String
    Dim TitleBarFound As String
    Dim cF As New cFindWIndow
    Dim ErrorString As String
    
    mStatus.Caption = "Launching ISIS/Draw..."
    TitleBarSubstring = "%Untitled in " & gENWindowName & "%" ' % is a wildcard here.
    ' Wait until there is an ISIS/Draw child process.
    Do
        Call Wait(LAUNCH_WAIT) ' Nonblocking wait - 100 milliseconds
        
' Old code:
'        IDrawPID = GetISISDrawChildPID
'        EditWindowExists = WindowExistsEx(TitleBarSubstring, False, TitleBarFound)

        ' Find a top-level window whose title bar indicates it is ISIS/Draw editing our structure.
        EditWindowExists = False
        cF.ClassToFind = "ISIS/Draw"
        cF.TitleToFind = TitleBarSubstring
        cF.ChildTitleToFind = ""
        EditWindowExists = cF.FindWindow
        
        If Not EditWindowExists Then
            ' Find an instance of ISIS/Draw that has a child window that is editing our structure.
            cF.ClassToFind = "ISIS/Draw"  ' Class string for this process
            cF.TitleToFind = ""
            cF.ChildTitleToFind = TitleBarSubstring
            EditWindowExists = cF.FindWindow
        End If
        LaunchWaitTime = LaunchWaitTime + LAUNCH_WAIT / 1000
        mStatus.Caption = "Launching ISIS/Draw... " ' & CStr(LaunchWaitTime) & " s."
        DoEvents
    Loop Until EditWindowExists Or (LaunchWaitTime > ISIS_LAUNCH_TIMEOUT)
    If (LaunchWaitTime > ISIS_LAUNCH_TIMEOUT) Then
        ErrorString = "ISIS/Draw Interface timeout - wait for ISIS/Draw launch time > " _
                  & CStr(ISIS_LAUNCH_TIMEOUT) & " s."
        MsgBox ErrorString, vbOKOnly, "ISIS/Draw Edit"
'        We cannot raise this error here because we are inside a double-click handler.
'        Err.Raise vbObjectError + 513, "ISIS/Draw Interface error - Wait for ISIS/Draw launch time > " _
'                  & CStr(ISIS_LAUNCH_TIMEOUT) & " s."
    End If
    
    ' Wait until the user has finished editing (there isn't an ISIS/Draw child process).
    Do
        mStatus.Caption = "ISIS/Draw editing in process..."
        DoEvents
        Call Wait(EDIT_WAIT) ' Nonblocking wait - 100 milliseconds
        
' Old code:
'        IDrawPID = GetISISDrawChildPID
'        EditWindowExists = WindowExistsEx(TitleBarSubstring, False, TitleBarFound)

        ' Find a top-level window whose title bar indicates it is ISIS/Draw editing our structure.
        EditWindowExists = False
        cF.ClassToFind = "ISIS/Draw"
        cF.TitleToFind = TitleBarSubstring
        cF.ChildTitleToFind = ""
        EditWindowExists = cF.FindWindow
        If Not EditWindowExists Then
            ' Find an instance of ISIS/Draw that has a child window that is editing our structure.
            cF.ClassToFind = "ISIS/Draw"  ' Class string for this process
            cF.TitleToFind = ""
            cF.ChildTitleToFind = TitleBarSubstring
            EditWindowExists = cF.FindWindow
        End If
        
        EditWaitTime = EditWaitTime + EDIT_WAIT / 1000
        mStatus.Caption = "ISIS/Draw editing in process... " ' & CStr(EditWaitTime) & " s."
        DoEvents
    Loop Until (Not EditWindowExists) Or (EditWaitTime > EDIT_WAIT_TIMEOUT * 60 * 60)
    If (EditWaitTime > EDIT_WAIT_TIMEOUT) Then
        ErrorString = "ISIS/Draw Interface timeout - wait for ISIS/Draw edit time > " _
                  & CStr(EDIT_WAIT_TIMEOUT) & " s."
        MsgBox ErrorString, vbOKOnly, "ISIS/Draw Edit"
'        We cannot raise this error here because we are inside a double-click handler.
'        Err.Raise vbObjectError + 513, "ISIS/Draw Interface error - Wait for ISIS/Draw edit time > " _
'                  & CStr(EDIT_WAIT_TIMEOUT) & " s."
    End If
    
    ' Fix bug CSBR-56406.  Look for an ISIS/Draw process that has no child window.  If so, terminate it.
    EditWindowExists = False
    cF.ClassToFind = "ISIS/Draw"
    cF.TitleToFind = "ISIS/Draw"
    cF.ChildTitleToFind = ""
    EditWindowExists = cF.FindWindow
    If EditWindowExists Then
        ' Find an instance of ISIS/Draw that has no child window.
        cF.ClassToFind = "ISIS/Draw"  ' Class string for this process
        cF.TitleToFind = ""
        cF.ChildTitleToFind = "%"
        EditWindowExists = cF.FindWindow ' This terminates ISIS/Draw if it's wondow is empty...
    End If
End Function

