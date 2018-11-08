VERSION 5.00
Begin VB.Form frmTest 
   Caption         =   "Form1"
   ClientHeight    =   3195
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   4680
   LinkTopic       =   "Form1"
   ScaleHeight     =   3195
   ScaleWidth      =   4680
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton Command1 
      Caption         =   "DO IT."
      Height          =   2850
      Left            =   105
      TabIndex        =   0
      Top             =   165
      Width           =   4440
   End
End
Attribute VB_Name = "frmTest"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub Command1_Click()
    ' Dim conn As Connection
    ' Set conn = New ADODB.Connection
    ' conn.Open "DSN=chem_reg;uid=scott;pwd=tiger;"
    
    'RunSQLLdr conn, 1
    
    Dim i As Long
    Dim req As Request
    Dim rs As ADODB.Recordset
    Dim conn As csdo.Connection
    Dim sql As String
    
    Set conn = New csdo.Connection

    conn.CSChemLinks.Add "MOLTABLE", "MOL_ID", "D:\program files\chemoffice2001\chemfinder\samples\cs_demo.mst", "", CLng(0)
    conn.OpenConn "DBQ=D:\program files\chemoffice2001\chemfinder\samples\cs_demo.MDB;Driver={Microsoft Access Driver (*.mdb)};DriverId=25;FIL=MS Access;; ; ", "", "", 0

    sql = "SELECT * FROM MOLTABLE WHERE MOL_ID = 78"
    
    Set req = conn.Execute(sql)
    req.Start
    
    Set rs = req.Recordset
        
    Set rs = Nothing
    Set req = Nothing
    Set conn = Nothing


End Sub

Private Sub dead()
'    Dim sql As String
'    Dim loopsql As String
'    Dim i As Long
'
'    sql = "insert into temporary_structures (commit_type,notebook_number, project_id, sequence_id, compound_type, salt_code, entry_person_id, mol_id) values ('FULL_COMMIT', 1, 1, 1, 1, 1, 6,"
'    '  sql = "update temporary_structures set mol_id = mol_id * 2.5"
'    ' sql = "delete from temporary_structures where mol_id > 23"
'    ' conn.Execute sql
'    ' Exit Sub
'
'
'    For i = 1 To 6493
'        loopsql = sql & i & ")"
'        conn.Execute loopsql
'    Next
    
    

End Sub
