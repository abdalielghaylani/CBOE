VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "DataImporter"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Imports a chemfinder database, Excel document, or .txt/.csv document to the specified table

Private Const MOL_TABLE = "MolTable"
Private Const STRUCTURE = "STRUCTURE"

Public ADOConnection As ADODB.Connection
Public fileName As String
Public MstFilename As String
Public MdbFilename As String
Public ImportTableName As String
Public RecordCount As Long
Public FieldsDict As Dictionary
Public FieldTypesDict As Dictionary
Public FieldMappingsDict As Dictionary
Public NumRecords As Long
Public SortFieldName As String
Public SortFieldName2 As String
Public SortFieldName3 As String
Public MolTable As String
Public bSelectMolTable As Boolean
Public MolTableList As String
Public tableCount As Integer

Private Const kMSReadOnly = 1

Public Enum EnumDatabaseTypes
    eDBChemFinder = 0
    eDBExcel
    eDBText
End Enum

Public mbHasChemDraw As Boolean
Public moChemDraw As Object
Public moMS As Object
Public miStatus As Long
Public meDatabaseType As EnumDatabaseTypes
Public msChemOffice As String

            'CSBR-121711 Structure not loaded into inventory from Excel using Invloader
            'Partial fix
            'JBattles 23-Feb-10
            'Return base64 cdx from Structure_Data field
Private Const kStructureMessage = "Please Note: InvLoader will look for structure data" & vbCrLf & _
                                  "in SMILES format under a 'Structures' header or in " & vbCrLf & _
                                  "Base64CDX under a 'Structure_Data' header.  Please " & vbCrLf & _
                                  "see technical notes in the readme file if using " & vbCrLf & _
                                  "ChemDraw for Excel."

                                  


Public Property Get Status() As Long
    Status = miStatus
End Property

Public Function TestValidity() As Boolean
    ' test validity of database
    TestValidity = True
End Function

Public Static Property Get OpenDatabaseFilter() As String
    'CSBR-121609 Invloder does not support Excel 2007; JBattles 17-Feb-10
    If miStatus >= eMolServer11 Then
        ' Add the new .cfx type
        OpenDatabaseFilter = "ChemFinder Databases (*.cfw,*.cfx)|*.cfw;*.cfx|Excel Files (*.xls,*xlsx)|*.xls;*xlsx|Comma-delimited Text Files (*.csv)|*.csv|"
    Else
        OpenDatabaseFilter = "ChemFinder Databases (*.cfw)|*.cfw|Excel Files (*.xls,*.xls)|*.xls;*.xlsx|Comma-delimited Text Files (*.csv)|*.csv|"
    End If
End Property

Public Sub SetFilterIndex(FilterIndex As Long)
    Select Case (FilterIndex)
        Case 1
            meDatabaseType = eDBChemFinder
        Case 2
            'CSBR-121711 Structure not loaded into inventory from Excel using Invloader
            'Partial fix
            'JBattles 23-Feb-10
            'Return base64 cdx from Structure_Data field
            MsgBox kStructureMessage, vbOKOnly, "An Important Note About Structures"
            meDatabaseType = eDBExcel
        Case 3
            'CSBR-121711 Structure not loaded into inventory from Excel using Invloader
            'Partial fix
            'JBattles 23-Feb-10
            'Return base64 cdx from Structure_Data field
            MsgBox kStructureMessage, vbOKOnly, "An Important Note About Structures"
            meDatabaseType = eDBText
    End Select
End Sub

Public Function OpenDB() As Boolean
    'MolTable = "" 'CSBR 163928 Commenting the line so that the MolTable property will not loose the value.
    bSelectMolTable = False

    Select Case (meDatabaseType)
        Case eDBChemFinder
            OpenDB = OpenChemFinderDB()
        Case eDBExcel
            OpenDB = OpenExcelDB()
        Case eDBText
            OpenDB = OpenTextDB()
    End Select
End Function

Public Function OpenTextDB() As Boolean
    
    Dim fso As FileSystemObject
    Dim sParentFolder As String
    Dim sConnect As String
    
    Call CloseDB
    OpenTextDB = FileExists(fileName)
    
    If Not OpenTextDB Then
        Exit Function
    End If
    
    Set fso = New FileSystemObject
    sParentFolder = fso.GetParentFolderName(fileName)
        
    On Error GoTo ErrorHandler
    
    ' Open connection to the text file
    sConnect = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & sParentFolder & ";" & _
        "Extended Properties=""text;HDR=YES;FMT=Delimited"""
    ADOConnection.open sConnect
    
    ' The file itself becomes the "table" to select from
    MolTable = fso.GetFileName(fileName)
    SetUpDictionary
    
    On Error GoTo 0
    
    Exit Function
    
ErrorHandler:
    OpenTextDB = False

End Function

Public Function OpenExcelDB() As Boolean
    
    Call CloseDB
    OpenExcelDB = FileExists(fileName)
    
On Error GoTo ErrorHandler
    If OpenExcelDB Then
        ' Open connection to Excel
        Dim sConnect As String
        'CSBR-121609 Invloder does not support Excel 2007; JBattles 17-Feb-10
        sConnect = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & fileName & ";Extended Properties=""Excel 12.0 Xml;HDR=YES"";"
        ADOConnection.open sConnect
        
        ' Try to determine the MolTable
        If MolTable = "" And MolTableList = "" Then
            bSelectMolTable = GetMolTable()
        End If
        If bSelectMolTable = False Then
            SetUpDictionary
        End If
    End If
    Exit Function
    
ErrorHandler:
    OpenExcelDB = False
End Function

Public Sub CloseDB()
    On Error Resume Next
    ADOConnection.Close
    On Error GoTo 0
    bSelectMolTable = False
    
    ' Reset these fields
    SortFieldName = ""
    SortFieldName2 = ""
    SortFieldName3 = ""

End Sub

Public Function OpenChemFinderDB() As Boolean
    OpenChemFinderDB = True
    MstFilename = ChExt(fileName, "mst")
    MdbFilename = ChExt(fileName, "mdb")
    OpenChemFinderDB = FileExists(MstFilename)
    OpenChemFinderDB = FileExists(MdbFilename)
    Call CloseDB
On Error GoTo ErrorHandler
    If OpenChemFinderDB Then
        ' open chem connection
        moMS.open MstFilename, kMSReadOnly, ""
        
        ' Open rel connection
        Dim sConnect As String
        sConnect = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & _
                    MdbFilename & _
                    ";Persist Security Info=False"
        ADOConnection.open sConnect
        
        'try to determine the MolTable
        If MolTable = "" And MolTableList = "" Then
            bSelectMolTable = GetMolTable()
        End If
        If bSelectMolTable = False Then
            SetUpDictionary
        End If
    End If
    Exit Function
ErrorHandler:
    OpenChemFinderDB = False

End Function

Public Sub SetUpDictionary()
    ' set up dictionary
    Dim oRS As Recordset
    Dim i As Long: i = 0
    Dim oField As Field
    Dim sSelect As String
    
    Select Case (meDatabaseType)
        Case eDBChemFinder
            sSelect = SelectSTR & " where null = null"
        Case eDBExcel
            sSelect = SelectSTR
        Case eDBText
            sSelect = SelectSTR
    End Select
    
    FieldsDict.RemoveAll
    FieldTypesDict.RemoveAll
    FieldMappingsDict.RemoveAll
    
    Set oRS = Me.ADOConnection.Execute(sSelect)
    FieldsDict.Add "1000", "Use Default Value:"
    For Each oField In oRS.fields
        If UCase(oField.name) <> STRUCTURE Then 'CSBR-158697:SJ
            FieldsDict.Add CStr(i), oField.name
            FieldTypesDict.Add CStr(i), oField.Type
            FieldMappingsDict.Add oField.name, i
        End If
        i = i + 1
    Next
    oRS.Close
    Set oRS = Me.ADOConnection.Execute(CountSelectSTR)
    NumRecords = oRS(0)
    oRS.Close
    Set oRS = Nothing
    On Error GoTo 0
End Sub

Public Function GetFieldMapping(sFieldName As String)
    If FieldMappingsDict.Exists(sFieldName) Then
        GetFieldMapping = FieldMappingsDict.Item(sFieldName)
    Else
        GetFieldMapping = "1000"
    End If
End Function

Public Function GetFieldName(sFieldIndex As String)
    If FieldsDict.Exists(sFieldIndex) Then
        GetFieldName = FieldsDict.Item(sFieldIndex)
    Else
        GetFieldName = "Unknown"
    End If
End Function

Public Function GetFieldType(sFieldName As String)
    If FieldTypesDict.Exists(sFieldName) Then
        GetFieldType = FieldTypesDict.Item(sFieldName)
    Else
        GetFieldType = adEmpty
    End If
End Function

Private Function GetMolTable()
    Dim oRSSchema As Recordset
    Dim sTableName As String
    tableCount = 0  'CSBR 164698 SJ declared the variable as public.
    MolTableList = ""
    Set oRSSchema = Me.ADOConnection.OpenSchema(adSchemaTables)
    While Not oRSSchema.EOF
    
        Select Case (meDatabaseType)
            Case eDBChemFinder
                If oRSSchema("table_type") <> "ACCESS TABLE" And oRSSchema("table_type") <> "SYSTEM TABLE" Then
                    MolTableList = MolTableList & oRSSchema("Table_Name") & ","
                tableCount = tableCount + 1
                End If
            
            Case eDBExcel
                ' All sheets in Excel are listed as "SYSTEM TABLE".  We'll build a list of all sheets in
                ' the document and let the user specify which one to use.  First remove the $ from the end
                ' of the table name so our list matches the actual Excel sheet names
                sTableName = oRSSchema("Table_Name")
                sTableName = Replace(sTableName, "'", "")
                sTableName = Replace(sTableName, "$", "")
                MolTableList = MolTableList & sTableName & ","
                tableCount = tableCount + 1
        
            Case eDBText
                ' This is handled within the OpenTextDB() function, as there is only ever one "table"
                ' in a text document; that is, the entire text document is the table
        End Select
            
        oRSSchema.MoveNext
    Wend
    oRSSchema.Close
    
    MolTableList = Left(MolTableList, Len(MolTableList) - 1)
    If tableCount = 1 Then
        MolTable = MolTableList
        GetMolTable = False
    Else
        GetMolTable = True
    End If
End Function

Public Property Get Recordset(ByVal SortFieldIndex As Long, ByVal SortFieldIndex2 As Long, ByVal SortFieldIndex3 As Long) As ADODB.Recordset
    If SortFieldIndex <> NULL_AS_LONG Then
        SortFieldName = FieldsDict(CStr(SortFieldIndex))
    End If
    If SortFieldIndex2 <> NULL_AS_LONG Then
        SortFieldName2 = FieldsDict(CStr(SortFieldIndex2))
    End If
    If SortFieldIndex3 <> NULL_AS_LONG Then
        SortFieldName3 = FieldsDict(CStr(SortFieldIndex3))
    End If
    Set Recordset = ADOConnection.Execute(SelectSTR)
End Property

Public Property Get Recordset2(ByVal sSQL As String) As ADODB.Recordset
    'CSBR 148240 SJ Calling the OpenDB function to open the database connection which got closed when the back button was clicked
    Call OpenDB
    Set Recordset2 = ADOConnection.Execute(sSQL)
End Property

Public Property Get RealTableName() As String
    Select Case (meDatabaseType)
        Case eDBChemFinder
            RealTableName = "[" & Me.MolTable & "]"
        Case eDBExcel
            RealTableName = "`" & Me.MolTable & "$`"
        Case eDBText
            RealTableName = "[" & Me.MolTable & "]"
    End Select
End Property

Public Property Get DistinctRecordset(ByVal DistinctFieldIndex As Long) As ADODB.Recordset
    Dim DistinctFieldName As String
    Dim sSQL As String
    
    If DistinctFieldIndex <> NULL_AS_LONG Then
        DistinctFieldName = FieldsDict(CStr(DistinctFieldIndex))
        sSQL = "SELECT DISTINCT([" & DistinctFieldName & "]) FROM " & Me.RealTableName() & " where [" & DistinctFieldName & "] is not null"
        Set DistinctRecordset = ADOConnection.Execute(sSQL)
    Else
        Set DistinctRecordset = ADOConnection.Execute(SelectSTR)
    End If
End Property

Public Property Get DuplicateChecking(ByVal FieldIndex As Long) As ADODB.Recordset
    Dim DistinctFieldName As String
    Dim sSQL As String
    
    If FieldIndex <> NULL_AS_LONG Then
        DistinctFieldName = FieldsDict(CStr(FieldIndex))
        sSQL = "select [" & DistinctFieldName & "], count(*) from " & Me.RealTableName() & " where [" & DistinctFieldName & "] is not null "
        sSQL = sSQL & " group by [" & DistinctFieldName & "] having count(*) > 1"
        Set DuplicateChecking = ADOConnection.Execute(sSQL)
    Else
        Set DuplicateChecking = ADOConnection.Execute(SelectSTR)
    End If
End Property

Public Property Get CountSelectSTR() As String
    Select Case (meDatabaseType)
        Case eDBChemFinder
            CountSelectSTR = "select count(*) from " & Me.RealTableName()
        Case eDBExcel
            CountSelectSTR = "select count(*) from " & Me.RealTableName()
        Case eDBText
            CountSelectSTR = "select count(*) from " & Me.RealTableName()
    End Select
End Property


Public Property Get SelectSTR() As String
    Select Case (meDatabaseType)
        Case eDBChemFinder
            SelectSTR = CFWSelectSTR()
        Case eDBExcel
            SelectSTR = ExcelSelectSTR()
        Case eDBText
            SelectSTR = TextSelectSTR()
    End Select
End Property

Public Property Get CFWSelectSTR() As String
    CFWSelectSTR = "SELECT * FROM " & Me.RealTableName()
    Dim bHasOrderBy As Boolean: bHasOrderBy = False
    If SortFieldName <> "" Then
        If Not bHasOrderBy Then
            CFWSelectSTR = CFWSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        CFWSelectSTR = CFWSelectSTR & "[" & SortFieldName & "] ASC"
    End If
    If SortFieldName2 <> "" Then
        If bHasOrderBy Then
            CFWSelectSTR = CFWSelectSTR & ", "
        Else
            CFWSelectSTR = CFWSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        CFWSelectSTR = CFWSelectSTR & "[" & SortFieldName2 & "] ASC"
    End If
    If SortFieldName3 <> "" Then
        If bHasOrderBy Then
            CFWSelectSTR = CFWSelectSTR & ", "
        Else
            CFWSelectSTR = CFWSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        CFWSelectSTR = CFWSelectSTR & "[" & SortFieldName3 & "] ASC"
    End If
End Property

Public Property Get ExcelSelectSTR() As String
    ExcelSelectSTR = "SELECT * FROM " & Me.RealTableName()
    Dim bHasOrderBy As Boolean: bHasOrderBy = False
    If SortFieldName <> "" Then
        If Not bHasOrderBy Then
            ExcelSelectSTR = ExcelSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        ExcelSelectSTR = ExcelSelectSTR & "[" & SortFieldName & "] ASC"
    End If
    If SortFieldName2 <> "" Then
        If bHasOrderBy Then
            ExcelSelectSTR = ExcelSelectSTR & ", "
        Else
            ExcelSelectSTR = ExcelSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        ExcelSelectSTR = ExcelSelectSTR & "[" & SortFieldName2 & "] ASC"
    End If
    If SortFieldName3 <> "" Then
        If bHasOrderBy Then
            ExcelSelectSTR = ExcelSelectSTR & ", "
        Else
            ExcelSelectSTR = ExcelSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        ExcelSelectSTR = ExcelSelectSTR & "[" & SortFieldName3 & "] ASC"
    End If
End Property

Public Property Get TextSelectSTR() As String
    TextSelectSTR = "SELECT * FROM " & Me.RealTableName()
    Dim bHasOrderBy As Boolean: bHasOrderBy = False
    If SortFieldName <> "" Then
        If Not bHasOrderBy Then
            TextSelectSTR = TextSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        TextSelectSTR = TextSelectSTR & "[" & SortFieldName & "] ASC"
    End If
    If SortFieldName2 <> "" Then
        If bHasOrderBy Then
            TextSelectSTR = TextSelectSTR & ", "
        Else
            TextSelectSTR = TextSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        TextSelectSTR = TextSelectSTR & "[" & SortFieldName2 & "] ASC"
    End If
    If SortFieldName3 <> "" Then
        If bHasOrderBy Then
            TextSelectSTR = TextSelectSTR & ", "
        Else
            TextSelectSTR = TextSelectSTR & " ORDER BY "
            bHasOrderBy = True
        End If
        TextSelectSTR = TextSelectSTR & "[" & SortFieldName3 & "] ASC"
    End If
End Property

Public Property Get Base64CDX(ByVal rs As ADODB.Recordset) As String
    Dim lMolID As Long
    
    Base64CDX = ""
    If Not IsObject(moMS) Then
        Exit Property
    End If
    
    Select Case (meDatabaseType)
        Case eDBChemFinder
            ' Return base64 cdx for given molid
            If Not IsNull(rs("mol_id")) Then
                On Error Resume Next
                Dim FilePath As String
                'FilePath = GetTmpPath + "temp.mol"
                FilePath = GetTmpPath + "temp.cdx"
                lMolID = CLng(rs("mol_id"))
                moMS.GetMol(lMolID).Write FilePath
                Base64CDX = Base64EncodeFile(FilePath)
                If Left(Base64CDX, Len("53 File not found")) = "53 File not found" Or Left(Base64CDX, Len("53+File+not+found")) = "53+File+not+found" Then Base64CDX = ""
                Kill FilePath
                On Error GoTo 0
            End If
        Case eDBExcel, eDBText
            'CSBR-121711 Structure not loaded into inventory from Excel using Invloader
            'Partial fix
            'JBattles 23-Feb-10
            'Return base64 cdx from Structure_Data field
            If UtilsMisc.RecordSetHasField(rs, "Structure_Data") Then
                If Not IsNull(rs("Structure_Data").value) Then
                    Base64CDX = rs("Structure_Data").value
                End If
            ' Return base64 cdx from SMILES format (changed "Structures" to match message, was "Structure", kfd2, 4-Apr-2011)
            ElseIf mbHasChemDraw And UtilsMisc.RecordSetHasField(rs, "Structures") Then
                If Not IsNull(rs("Structures").value) Then
                    moChemDraw.Data("chemical/x-smiles") = rs("Structures").value
                    moChemDraw.DataEncoded = True
                    Base64CDX = moChemDraw.Data("chemical/x-cdx")
                End If
            End If
    End Select

End Property

Private Sub Class_Initialize()
    Err.Clear
    Set ADOConnection = New ADODB.Connection
    
    Set FieldsDict = New Dictionary
    Set FieldTypesDict = New Dictionary
    Set FieldMappingsDict = New Dictionary
    
    On Error Resume Next
    'MolTable = MOL_TABLE
    bSelectMolTable = False
	
	' try version 19
	Err.Clear
    Set moMS = CreateObject("MolServer19.Document")
    If Err.Number = 0 Then
        miStatus = eMolServer19
        msChemOffice = "Molserver19"
    Else
	' try version 18
	Err.Clear
    Set moMS = CreateObject("MolServer18.Document")
    If Err.Number = 0 Then
        miStatus = eMolServer18
        msChemOffice = "Molserver18"
    Else
	' try version 17
	Err.Clear
    Set moMS = CreateObject("MolServer17.Document")
    If Err.Number = 0 Then
        miStatus = eMolServer17
        msChemOffice = "Molserver17"
    Else
	' try version 15
    Err.Clear
    Set moMS = CreateObject("MolServer15.Document")
    If Err.Number = 0 Then
        miStatus = eMolServer15
        msChemOffice = "Molserver15"
    Else
    ' try version 14
    Err.Clear
    Set moMS = CreateObject("MolServer14.Document")
    If Err.Number = 0 Then
        miStatus = eMolServer14
        msChemOffice = "Molserver14"
	Else
        ' try version 12
        Err.Clear
        Set moMS = CreateObject("MolServer12.Document")
        If Err.Number = 0 Then
            miStatus = eMolServer12
            msChemOffice = "Molserver12"
        Else
            ' try version 11
            Err.Clear
            Set moMS = CreateObject("MolServer11.Document")
            If Err.Number = 0 Then
                miStatus = eMolServer11
                msChemOffice = "Molserver11"
            Else
                ' try version 10
                Err.Clear
                Set moMS = CreateObject("MolServer10.Document")
                If Err.Number = 0 Then
                    miStatus = eMolServer10
                    msChemOffice = "Molserver10"
                Else
                    ' try version 9
                    Err.Clear
                    Set moMS = CreateObject("MolServer9.Document")
                    If Err.Number = 0 Then
                        miStatus = eMolServer9
                        msChemOffice = "Molserver9"
                    Else
                        ' try version 8
                        Err.Clear
                        Set moMS = CreateObject("MolServer8.Document")
                        If Err.Number = 0 Then
                            miStatus = eMolServer8
                            msChemOffice = "Molserver8"
                        Else
                            ' try version 7
                            Err.Clear
                            Set moMS = CreateObject("MolServer7.Document")
                            If Err.Number = 0 Then
                                miStatus = eMolServer7
                            Else ' All versions have failed
                                msChemOffice = "No Molserver"
                                miStatus = eNoMolServer
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End If
	End If
	End If
	End If
	End If
	
    
    ' Create the ChemDraw object.
    mbHasChemDraw = False
    msChemOffice = msChemOffice & "/"
    ' Version 10 of ChemDrawControl seems to cause crashing problems with VB6 and the GDI.  This is
    ' being enabled to fix CSBR-90766, since they are not testing with version 9.  Whether this
    ' breaks or not remains to be seen.
	Err.Clear
	Set moChemDraw = CreateObject("ChemDrawControl19.ChemDrawCtl")
    If Err.Number = 0 Then
        mbHasChemDraw = True
        msChemOffice = msChemOffice & "ChemDraw19"
	Else
	Err.Clear
	Set moChemDraw = CreateObject("ChemDrawControl18.ChemDrawCtl")
    If Err.Number = 0 Then
        mbHasChemDraw = True
        msChemOffice = msChemOffice & "ChemDraw18"
	Else
	Err.Clear
	Set moChemDraw = CreateObject("ChemDrawControl17.ChemDrawCtl")
    If Err.Number = 0 Then
        mbHasChemDraw = True
        msChemOffice = msChemOffice & "ChemDraw17"
	Else
	Err.Clear
    Set moChemDraw = CreateObject("ChemDrawControl15.ChemDrawCtl")
    If Err.Number = 0 Then
        mbHasChemDraw = True
        msChemOffice = msChemOffice & "ChemDraw15"
	Else
    Err.Clear
    Set moChemDraw = CreateObject("ChemDrawControl14.ChemDrawCtl")
    If Err.Number = 0 Then
        mbHasChemDraw = True
        msChemOffice = msChemOffice & "ChemDraw14"
    Else
        Err.Clear
        Set moChemDraw = CreateObject("ChemDrawControl12.ChemDrawCtl")
        If Err.Number = 0 Then
            mbHasChemDraw = True
            msChemOffice = msChemOffice & "ChemDraw12"
        Else
            Err.Clear
            Set moChemDraw = CreateObject("ChemDrawControl11.ChemDrawCtl")
            If Err.Number = 0 Then
                mbHasChemDraw = True
                msChemOffice = msChemOffice & "ChemDraw11"
            Else
                Err.Clear
                Set moChemDraw = CreateObject("ChemDrawControl10.ChemDrawCtl")
                If Err.Number = 0 Then
                    mbHasChemDraw = True
                    msChemOffice = msChemOffice & "ChemDraw10"
                Else
                    Err.Clear
                    Set moChemDraw = CreateObject("ChemDrawControl9.ChemDrawCtl")
                    If Err.Number = 0 Then
                        mbHasChemDraw = True
                        msChemOffice = msChemOffice & "ChemDraw9"
                    Else
                        Err.Clear
                        Set moChemDraw = CreateObject("ChemDrawControl8.ChemDrawCtl")
                        If Err.Number = 0 Then
                            mbHasChemDraw = True
                            msChemOffice = msChemOffice & "ChemDraw8"
                        Else
                            Err.Clear
                            Set moChemDraw = CreateObject("ChemDrawControl7.ChemDrawCtl")
                            If Err.Number = 0 Then
                                mbHasChemDraw = True
                                msChemOffice = msChemOffice & "ChemDraw7"
                            Else
                                msChemOffice = msChemOffice & "No ChemDraw"
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End If
	End If
	End If
	End If
	End If
    
    On Error GoTo 0
    ' Set moCSDO = New CSDO7.Connection
End Sub

Private Sub Class_Terminate()
    Set moChemDraw = Nothing
    Set moMS = Nothing
    Set ADOConnection = Nothing
    Set FieldsDict = Nothing
    Set FieldTypesDict = Nothing
    Set FieldMappingsDict = Nothing
End Sub
