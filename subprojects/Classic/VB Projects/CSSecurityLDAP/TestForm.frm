VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "COMDLG32.OCX"
Begin VB.Form TestForm 
   Caption         =   "CS LDAP Test"
   ClientHeight    =   7485
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   11355
   LinkTopic       =   "Form1"
   ScaleHeight     =   7485
   ScaleWidth      =   11355
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton btn_Send 
      Caption         =   "Send"
      Height          =   375
      Left            =   9720
      TabIndex        =   9
      Top             =   1560
      Width           =   1095
   End
   Begin VB.Frame Frame1 
      Height          =   7215
      Left            =   480
      TabIndex        =   1
      Top             =   240
      Width           =   10815
      Begin VB.TextBox txtSvcAct 
         Height          =   315
         Left            =   7680
         TabIndex        =   14
         Top             =   360
         Width           =   2295
      End
      Begin VB.TextBox txtPassword 
         Height          =   315
         Left            =   4320
         TabIndex        =   12
         Top             =   360
         Width           =   2295
      End
      Begin VB.TextBox txtUserName 
         Height          =   315
         Left            =   960
         TabIndex        =   10
         Top             =   360
         Width           =   2295
      End
      Begin VB.CommandButton btn_open 
         Caption         =   "open"
         Height          =   375
         Left            =   9240
         TabIndex        =   8
         Top             =   840
         Width           =   1095
      End
      Begin VB.ComboBox Combo1 
         Enabled         =   0   'False
         Height          =   315
         Left            =   960
         TabIndex        =   6
         Top             =   1200
         Width           =   3735
      End
      Begin VB.TextBox txtXml 
         Enabled         =   0   'False
         Height          =   5175
         Left            =   360
         MultiLine       =   -1  'True
         ScrollBars      =   3  'Both
         TabIndex        =   5
         Top             =   1800
         Width           =   10095
      End
      Begin VB.CommandButton btn_browse 
         Caption         =   "Browse"
         Height          =   375
         Left            =   8160
         TabIndex        =   3
         Top             =   840
         Width           =   1095
      End
      Begin MSComDlg.CommonDialog CommonDialog1 
         Left            =   120
         Top             =   6120
         _ExtentX        =   847
         _ExtentY        =   847
         _Version        =   393216
         Filter          =   "(*.xml)|*.xml"
      End
      Begin VB.TextBox TxtConfigXmlPath 
         Height          =   315
         Left            =   960
         TabIndex        =   2
         Text            =   "Text1"
         Top             =   840
         Width           =   7095
      End
      Begin VB.Label Label5 
         Caption         =   "Service Act:"
         Height          =   255
         Left            =   6720
         TabIndex        =   15
         Top             =   360
         Width           =   975
      End
      Begin VB.Label Label4 
         Caption         =   "Password:"
         Height          =   255
         Left            =   3600
         TabIndex        =   13
         Top             =   360
         Width           =   975
      End
      Begin VB.Label Label3 
         Caption         =   "User Name:"
         Height          =   255
         Left            =   120
         TabIndex        =   11
         Top             =   360
         Width           =   975
      End
      Begin VB.Label Label2 
         Caption         =   "Node:"
         Height          =   255
         Left            =   480
         TabIndex        =   7
         Top             =   1200
         Width           =   495
      End
      Begin VB.Label Label1 
         Caption         =   "Config File:"
         Height          =   255
         Left            =   120
         TabIndex        =   4
         Top             =   840
         Width           =   855
      End
   End
   Begin VB.CommandButton btn_test 
      Caption         =   "Test"
      Height          =   375
      Left            =   9360
      TabIndex        =   0
      Top             =   1560
      Width           =   1455
   End
End
Attribute VB_Name = "TestForm"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Dim mConfigElement As IXMLDOMElement



Private Sub btn_Browse_Click()
    CommonDialog1.ShowOpen
    TxtConfigXmlPath = CommonDialog1.FileName
    ParseConfig
End Sub

Private Sub ParseConfig()
    Dim config As MSXML2.DOMDocument
    Dim ConfigFile As String
    
    Dim chld As IXMLDOMNode
    
    Set config = New DOMDocument
    ConfigFile = TxtConfigXmlPath

    ConfigDocument config, ConfigFile, ""
    Combo1.Enabled = True
    Set mConfigElement = config.documentElement
    Combo1.Clear
    Combo1.AddItem "Select a node to test ==>"
    Combo1.ItemData(Combo1.NewIndex) = 0
    For Each chld In mConfigElement.childNodes
        Combo1.AddItem chld.NodeName
    Next
    Combo1.ListIndex = 0
End Sub

Private Sub btn_open_Click()
    ParseConfig
End Sub



Private Sub btn_Send_Click()
    
    Dim LoginID As String
    Dim password As String
    Dim oLDAPCOM As New LDAPCOM
    
    Dim ldResults As LDAPResults
    Dim testXML As MSXML2.DOMDocument
    
    On Error GoTo ErrorHandler
    
    LoginID = txtUserName.Text
    password = txtPassword.Text
    
    If LoginID = "" Then
        MsgBox "UserName is required to connect to LDAP"
        Exit Sub  '  ===> Exit Point
    End If
    
    Set ldResults = New LDAPResults
    ldResults.addAttribute "username", LoginID
    ldResults.addAttribute "svcaccount", txtSvcAct.Text
    
    
    Set testXML = New DOMDocument
    
    With testXML
        .async = False
        .loadXML (txtXml.Text)
        With .parseError
            If .errorCode <> 0 Then
                Set testXML = Nothing
                MsgBox "An error occured while reading test xml" & vbCrLf & _
                "Line: " & .Line & vbCrLf & "Reason: " & .reason
                Exit Sub
            End If
        End With
    End With
    
    oLDAPCOM.RunLDAPProcess testXML.documentElement, ldResults, Combo1.Text, password
    
    MsgBox ldResults.XML.XML
    
    Set oLDAPCOM = Nothing
    
ErrorHandler:
    If Err Then
        MsgBox Err.Description, vbExclamation, "Error Sending to LDAP"
    End If
End Sub

Sub ConfigDocument(ByRef config, ByVal ConfigFile, ByVal ErrorSource)
    Dim configPath
    Dim fso

    Set fso = CreateObject("Scripting.FileSystemObject")

    configPath = fso.BuildPath("", ConfigFile)
    With config
        .async = False
        .Load configPath
        With .parseError
            If .errorCode <> 0 Then
                Set config = Nothing
                MsgBox "An error occured while reading " & configPath & vbCrLf & _
                "Line: " & .Line & vbCrLf & "Reason: " & .reason
                Exit Sub
            End If
        End With
    End With
End Sub
    





Private Sub Combo1_Click()
    Dim s As String
    
    If Combo1.ListIndex = 0 Then
        txtXml.Text = ""
        txtXml.Enabled = False
    Else
        txtXml.Enabled = True
        
        txtXml.Text = "<test>" & vbCrLf & mConfigElement.selectSingleNode(Combo1.Text).XML & vbCrLf & "</test>"
    End If
End Sub

Private Sub Form_Load()
    TxtConfigXmlPath.Text = App.Path & "\LDAPConfig.xml"
End Sub

