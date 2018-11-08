VERSION 5.00
Object = "{0ECD9B60-23AA-11D0-B351-00A0C9055D8E}#6.0#0"; "MSHFLXGD.OCX"
Object = "{2FFF2CD7-B92A-11D1-9C9A-0020182A75DB}#1.0#0"; "CSMol.ocx"
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "comdlg32.ocx"
Begin VB.Form frmExecute 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "CS ChemSQL"
   ClientHeight    =   8025
   ClientLeft      =   150
   ClientTop       =   720
   ClientWidth     =   7680
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   8025
   ScaleWidth      =   7680
   StartUpPosition =   3  'Windows Default
   Begin MSComDlg.CommonDialog CommonDialog1 
      Left            =   3525
      Top             =   5640
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin CSMolLib.CSMol molCtl 
      Height          =   2100
      Left            =   135
      TabIndex        =   12
      Top             =   2295
      Width           =   5085
      _Version        =   65536
      _ExtentX        =   8969
      _ExtentY        =   3704
      _StockProps     =   13
      BackColor       =   -2147483643
      TheMolecule     =   "Form1.frx":0000
   End
   Begin VB.Frame Frame1 
      Height          =   2205
      Left            =   5310
      TabIndex        =   7
      Top             =   2190
      Width           =   2235
      Begin VB.CommandButton cmdQueryString 
         Caption         =   "Create &Query String"
         Height          =   330
         Left            =   195
         TabIndex        =   11
         Top             =   975
         Width           =   1875
      End
      Begin VB.CommandButton cmdEditStruc 
         Caption         =   "&Edit Structure"
         Height          =   330
         Left            =   195
         TabIndex        =   10
         Top             =   360
         Width           =   1875
      End
      Begin VB.CommandButton cmdSaveToFile 
         Caption         =   "&Save To File"
         Height          =   330
         Left            =   195
         TabIndex        =   8
         Top             =   1590
         Width           =   1875
      End
   End
   Begin VB.ComboBox cmbHistory 
      Height          =   315
      Left            =   1170
      Style           =   2  'Dropdown List
      TabIndex        =   5
      Top             =   1710
      Width           =   4035
   End
   Begin MSHierarchicalFlexGridLib.MSHFlexGrid grdResults 
      Height          =   3345
      Left            =   120
      TabIndex        =   4
      Top             =   4560
      Width           =   7425
      _ExtentX        =   13097
      _ExtentY        =   5900
      _Version        =   393216
      AllowUserResizing=   1
      _NumberOfBands  =   1
      _Band(0).Cols   =   2
   End
   Begin VB.CommandButton cmdExecute 
      Caption         =   "E&xecute Query"
      Height          =   345
      Left            =   5340
      TabIndex        =   2
      Top             =   1695
      Width           =   2205
   End
   Begin VB.TextBox txtQuery 
      Height          =   750
      Left            =   135
      MultiLine       =   -1  'True
      TabIndex        =   0
      Top             =   765
      Width           =   7395
   End
   Begin VB.Line Line3 
      X1              =   0
      X2              =   7680
      Y1              =   345
      Y2              =   345
   End
   Begin VB.Line Line2 
      X1              =   0
      X2              =   7680
      Y1              =   0
      Y2              =   0
   End
   Begin VB.Line Line1 
      X1              =   3225
      X2              =   4425
      Y1              =   3360
      Y2              =   3840
   End
   Begin VB.Label lblStatus 
      Alignment       =   1  'Right Justify
      Caption         =   "Ready"
      Height          =   240
      Left            =   2385
      TabIndex        =   9
      Top             =   465
      Width           =   5160
   End
   Begin VB.Label Label2 
      Caption         =   "Past Queries:"
      Height          =   240
      Left            =   135
      TabIndex        =   6
      Top             =   1770
      Width           =   975
   End
   Begin VB.Label lblConn 
      Alignment       =   1  'Right Justify
      Caption         =   "No Connection Established."
      Height          =   240
      Left            =   105
      TabIndex        =   3
      Top             =   75
      Width           =   7470
   End
   Begin VB.Label Label1 
      Caption         =   "Enter SQL Query:"
      Height          =   270
      Left            =   150
      TabIndex        =   1
      Top             =   510
      Width           =   1425
   End
   Begin VB.Menu mnuConn 
      Caption         =   "&Connection"
      Begin VB.Menu mnuConnNew 
         Caption         =   "&New"
      End
      Begin VB.Menu mnuConnOpen 
         Caption         =   "&Open"
      End
      Begin VB.Menu mnuConnSave 
         Caption         =   "&Save"
      End
      Begin VB.Menu mnuConnSaveAs 
         Caption         =   "Save &As"
      End
      Begin VB.Menu mnuConnSep1 
         Caption         =   "-"
      End
      Begin VB.Menu mnuConnExit 
         Caption         =   "E&xit"
      End
   End
   Begin VB.Menu mnuOptions 
      Caption         =   "&Options"
      Begin VB.Menu mnuOptionsConnection 
         Caption         =   "&Connection Settings..."
      End
      Begin VB.Menu mnuOptionsChemRel 
         Caption         =   "Chem-&Rel Join Options..."
      End
      Begin VB.Menu mnuOptionsChemSearch 
         Caption         =   "Chem &Search Options..."
      End
   End
   Begin VB.Menu mnuHelp 
      Caption         =   "&Help"
      Begin VB.Menu mnuHelpAbout 
         Caption         =   "&About"
      End
   End
End
Attribute VB_Name = "frmExecute"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Dim csConnection As CSDO.Connection
Dim connFileName As String

Private Sub cmbHistory_lostfocus()
    txtQuery.Text = cmbHistory.List(cmbHistory.ListIndex)
End Sub

Private Sub cmdExecute_Click()
    Dim sql As String
    Dim statText As String
    Dim req As CSDO.Request
    Dim rs As ADODB.Recordset
    
    If csConnection Is Nothing Then
        MsgBox "Please open a connection before executing a query."
        Exit Sub
    End If

    sql = txtQuery.Text
    If sql = "" Then
        MsgBox "Please enter a query."
        Exit Sub
    End If
    
    Me.MousePointer = vbHourglass
    
    lblStatus.Caption = "Executing Query..."
    
    Dim i
    Dim found As Boolean
    found = False
    For i = 0 To cmbHistory.ListCount
        If cmbHistory.List(i) = sql Then
            found = True
            Exit For
        End If
    Next
    
    If Not found Then
        cmbHistory.AddItem sql
    End If

On Error GoTo ErrorHandler:
    Set req = csConnection.Execute(sql)
    req.Start
    Set rs = req.Recordset
    FillResultBox rs
    Me.MousePointer = vbDefault
    Exit Sub

ErrorHandler:
    Set req = Nothing
    Set rs = Nothing
    Me.MousePointer = vbDefault
    MsgBox "An error occurred while processing your query: " & vbCrLf & vbCrLf & _
          Err.Number & vbCrLf & vbCrLf & _
          Err.Description
End Sub

Private Sub cmdEditStruc_Click()
    molCtl.EditMol
End Sub

Private Sub cmdQueryString_click()
    frmQuery.clear molCtl
    frmQuery.Show vbModal, Me
End Sub

Private Sub cmdSaveToFile_Click()
    With CommonDialog1
        .ShowSave
        If .fileName <> "" Then
            molCtl.WriteMol .fileName
        End If
    End With
End Sub

Private Sub Form_Load()
    UpdateConnStatus
End Sub

Private Sub mnuConnExit_Click()
    Unload Me
End Sub

Private Sub mnuHelpAbout_Click()
    frmAbout.Show vbModal, Me
End Sub

Private Sub mnuOptions_Click()
    If csConnection Is Nothing Then
        mnuOptionsConnection.Enabled = False
        mnuOptionsChemRel.Enabled = False
        mnuOptionsChemSearch.Enabled = False
    Else
        mnuOptionsConnection.Enabled = True
        mnuOptionsChemRel.Enabled = True
        mnuOptionsChemSearch.Enabled = True
    End If
End Sub

Private Sub mnuOptionsConnection_click()
    EditConnection
End Sub

Private Sub mnuConn_click()
    If csConnection Is Nothing Then
        mnuConnSave.Enabled = False
        mnuConnSaveAs.Enabled = False
    Else
        mnuConnSave.Enabled = True
        mnuConnSaveAs.Enabled = True
    End If
End Sub

Private Sub mnuOptionsChemSearch_Click()
    With frmChemSearchOptions
        .clear csConnection
        .Show vbModal, Me
        If .isOK Then
            csConnection.ChemSearchOptions.Fill .Check1(4).Value, _
                            .Check1(7).Value, _
                            .Check1(5).Value, _
                            .Check1(6).Value, _
                            .Check1(2).Value, _
                            .Check1(1).Value, _
                            .Check1(0).Value, _
                            .Check1(3).Value
        End If
    End With
End Sub

Private Sub NewConnection()
    Dim clArr() As String
    Dim i As Integer

    Set csConnection = Nothing
    Set csConnection = New CSDO.Connection
    With frmConnect
        .clear csConnection
        .Show vbModal, Me

        If frmConnect.isOK Then
            For i = 0 To .lstChemLinks.ListCount - 1
                clArr = Split(.lstChemLinks.List(i), ".")
                csConnection.CSChemLinks.Add clArr(0), clArr(1), .txtMolString.Text, "", 0
            Next
            csConnection.OpenConn .txtADO.Text, "", "", 0
            UpdateConnStatus
        End If
    End With
End Sub

Private Sub EditConnection()
    Dim clArr() As String
    Dim i As Integer

    With frmConnect
        .clear csConnection
        .Show vbModal, Me

        If frmConnect.isOK Then
            csConnection.CSChemLinks.clear
            For i = 0 To .lstChemLinks.ListCount - 1
                clArr = Split(.lstChemLinks.List(i), ".")
                csConnection.CSChemLinks.Add clArr(0), clArr(1), .txtMolString.Text, "", 0
            Next
            csConnection.ADOConnString = .txtADO.Text
            UpdateConnStatus
        End If
    End With
End Sub

Private Sub SaveConnection(fileName As String)
    SaveTextToFile fileName, csConnection.PersistToXML
    connFileName = fileName
    UpdateConnStatus
End Sub

Private Sub OpenConnection(fileName As String)
    
On Error GoTo ErrorHandler
    Set csConnection = Nothing
    Set csConnection = New CSDO.Connection
    If csConnection.CreateFromXML(ReadTextFromFile(fileName)) Then
        csConnection.OpenConn
        connFileName = fileName
        UpdateConnStatus
    Else
        Set csConnection = Nothing
        UpdateConnStatus
    End If
    Exit Sub
    
ErrorHandler:
    MsgBox "An error occurred while opening the connection: " & vbCrLf & vbCrLf & _
           Err.Number & vbCrLf & vbCrLf & _
           Err.Description
End Sub

Private Sub UpdateConnStatus()
    If Not csConnection Is Nothing Then
        Dim str As String
        If connFileName = "" Then
            str = "<Unsaved Connection>"
        Else
            str = connFileName
        End If
        lblConn.Caption = "Connected: " & str
    Else
        lblConn.Caption = "No Connection Established"
    End If
End Sub

Private Sub FillResultBox(rs As ADODB.Recordset)
    Dim count As Long
        
    If Not rs Is Nothing Then
        If rs.State = adStateOpen Then
            lblStatus.Caption = "Total Records Found: " & rs.RecordCount
            If Not rs.EOF Then
                Set grdResults.DataSource = rs
                For count = 1 To rs.RecordCount
                    grdResults.TextMatrix(count, 0) = count
                Next
            Else
                lblStatus.Caption = "No records found."
            End If
        Else
            lblStatus.Caption = "Operation complete."
        End If
    Else
        lblStatus.Caption = "No recordset returned."
    End If
End Sub

Private Sub grdResults_RowColChange()
    With grdResults
        If InStr(.TextMatrix(0, .Col), "STRUCTFILE") <> 0 Then
            molCtl.ReadMol .TextMatrix(.Row, .Col)
        ElseIf InStr(.TextMatrix(0, .Col), "BASE64CDX") <> 0 Then
            Decode .TextMatrix(.Row, .Col), GetTmpPath & "temp.cdx"
            molCtl.ReadMol GetTmpPath & "temp.cdx"
        End If
    End With
End Sub

Private Sub mnuConnNew_Click()
    NewConnection
End Sub

Private Sub mnuConnSave_Click()
    If connFileName = "" Then
        mnuConnSaveAs_Click
    Else
        SaveConnection connFileName
    End If
End Sub

Private Sub mnuConnSaveAs_Click()
    With CommonDialog1
        .ShowSave
        If .fileName <> "" Then
            SaveConnection .fileName
            connFileName = .fileName
        End If
    End With
End Sub

Private Sub mnuConnOpen_click()
    With CommonDialog1
        .ShowOpen
        If .fileName <> "" Then
            OpenConnection .fileName
        End If
    End With
End Sub

Private Sub molCtl_DblClick()
    molCtl.EditMol
End Sub

Private Sub mnuOptionsChemRel_Click()
    frmAdvanced.clear csConnection
    frmAdvanced.Show vbModal, Me
    If frmAdvanced.isOK Then
        With frmAdvanced
            csConnection.ADOOptions.Fill .cmbStoreType.ListIndex, _
                .cmbSJM.ListIndex, _
                .cmbLJM.ListIndex, _
                .txtJT.Text, _
                .txtWD.Text, _
                "", _
                .txtADSN.Text, _
                .txtAUID.Text, _
                .txtAPWD.Text, _
                .txtTTBN.Text, _
                .txtOSP.Text
        End With
    End If
End Sub


