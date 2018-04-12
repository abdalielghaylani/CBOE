Attribute VB_Name = "Wizard"
Dim TablesCollection As Collection
Dim ParseTablesCollection As Collection
Dim ParsedLinksCollection As Collection
Dim ParsedConnectionCollection As Collection

Dim TableLinks As Collection
Dim theDBName
Dim TableLinkNames As Collection
Dim DB As clsDB
Dim TableNames As Collection
Dim RelFields As String



Function RunWizard(createType, theAppName, theCFWFormPath, theDataType, theNumListView)
    theDBName = GetDBName(theCFWFormPath)
    Select Case createType
        Case "New"
            Set Common.App = New clsApp
            Set Common.AppGlobals = New clsAppGlobals
            'Set Common.App.AppGlobals = Common.AppGlobals
            Common.AppGlobals.APP_NAME = theAppName
            Common.AppGlobals.DB_NAMES = theDBName
            Common.AppGlobals.MAIN_WINDOW = "top.frames[""main""]"
            Common.AppGlobals.NAV_BUTTONS_GIF_PATH = "/cfserverasp/source/graphics/navbuttons/"
            Common.AppGlobals.TEMP_DIR_NAME = "CFWTEMP"
            Common.AppGlobals.TEMP_DIR_PATH = GetCOWSPath() & "\" & Common.AppGlobals.TEMP_DIR_NAME

            Set Common.mcolDBs = New DBs
            Set Common.theDB = CreateNewDB(theAppName, theCFWFormPath, theDataType, theNumListView, theDBName)
            Common.mcolDBs.Add Common.theDB, theDBName
         '   Set theDB = Nothing
             RunWizard = theDBName
        Case "Add"
            temp = Common.AppGlobals.DB_NAMES
            newNames = temp & "," & theDBName
            Set Common.theDB = CreateNewDB(theAppName, theCFWFormPath, theDataType, theNumListView, theDBName)
            Common.mcolDBs.Add theDB, theDBName
    End Select
    
End Function
Function CreateNewDB(theAppName, theCFWFormPath, theDataType, theNumListView, theDBName) As clsDB
Dim STRSubFormGroups, STRChemConnGroups, STRFormGroups, STRTableGroups, STRFieldMapGroups, STRTableAliases As String
theASCIIPath = GetCFWASCII(theCFWFormPath)
Set DB = New clsDB
Set ParseTablesCollection = New Collection

DB.ABOUT_WINDOW = theDBName
DB.DISPLAY_NAME = theDBName
DB.MAXHITS = "100"
DB.CFW_INSTANCE = "new"
DB.DB_RECORD_COUNT = GetDBRecordCount(theCFWFormPath)
DB.DB_TYPE = theDataType

'returns three collection, TablesCollection,TableLinks and TableNames
ParseCFWOutput theASCIIPath
For i = 1 To TableNames.Count
    If STRTableAliases <> "" Then
        STRTableAliases = STRTableAliases & "," & TableNames.Item(i)
    Else
        STRTableAliases = TableNames.Item(i)
    End If
Next
DB.DBName = theDBName
Set DB.TableAliases = BuildTable_Aliases(STRTableAliases)
Set DB.FormGroups = GetFormGroups("base_form_group, basenp_form_group")

'Set DB.SubFormGroups = STRSubFormGroups
'Set DB.ADOConnGroups = STRADOConnGroups
'Set DB.ChemConnGroups = STRChemConnGroups
'Set DB.TableGroups = STRTableGroups

'Set DB.FieldMapGroups = STRFieldMapGroups

Set CreateNewDB = DB

Set DB = Nothing
End Function


Public Function BuildTable_Aliases(ByVal inputStr As String) As TableAliases

Dim mcolTableAliases As TableAliases

Set mcolTableAliases = New TableAliases
Dim TableFields As Collection
Set TableFields = New Collection
Dim TableLinks As Collection
Set TableLinks = New Collection
Dim TableItem As Collection
Dim Table As Collection
Dim i As Integer
'loop through TableNames collection, use its index to access relevant table
'information in other collections

theTables = Split(inputStr, ",", -1)
theBaseTable = theTables(0)
For i = 0 To UBound(theTables)
    masterTableIndex = i + 1
    Dim TableAlias As New clsTableAlias
    RelFields = ""
    GetFieldsInfo masterTableIndex
    GetLinksInfo masterTableIndex
    theTableName = theTables(i)
    TableAlias.GroupName = theTableName
    TableAlias.TABLE_ALIAS_NAME = theTableName
    'RelFields
    Set Table = TablesCollection.Item(theTableName)
        For j = 1 To Table.Count
        Set theField = Table.Item(j)
            If (theField.Item(1) = "FBOX 0" Or theField.Item(1) = "FBOX 8") Then
                    If (Not theField.Item(3) = "4" Or Not theField.Item(3) = "3" Or Not theField.Item(3) = "5") Then
                        If RelFields <> "" Then
                            RelFields = RelFields & "," & theField.Item(4) & ";" & theField.Item(3)
                        Else
                            RelFields = theField.Item(4) & ";" & theField.Item(3)
                        End If
                    End If
                RelFields = Replace(RelFields, """", "")
               End If
            If theField.Item(1) = "FBOX 4" Then
            
                If StructureFields <> "" Then
                     StructureFields = StructureFields & theField.Item(4)
                 Else
                      StructureFields = theField.Item(4)
                 End If
            End If
        Next
        Set theField = Nothing
       TableAlias.RELATIONAL_FIELDS = RelFields
       For k = 1 To Table.Count
        Set theField = Table.Item(k)
            If theField.Item(1) = "FBOX 6" Then
            theTableInfo = Split(theField.Item(4), " ", -1)
                theBaseLinkInfo = Split(theField.Item(3), ",", -1)
                theTableInfo(0) = Replace(theTableInfo(2), """", "")
                theTableInfo(1) = Replace(theTableInfo(2), """", "")
                theTableInfo(2) = Replace(theTableInfo(2), """", "")
                theTableInfo(3) = Replace(theTableInfo(2), """", "")
                theBaseLinkInfo(0) = Replace(theBaseLinkInfo(0), """", "")
                theBaseLinkInfo(1) = Replace(theBaseLinkInfo(1), """", "")
                 theBaseLinkInfo(1) = Replace(theBaseLinkInfo(2), """", "")
                TableAlias.PRIMARY_KEY = theTableInfo(2)
                TableAlias.SQL_SYNTAX = "ACCESS"
                If theTableName = theBaseTable Then
                    TableAlias.SELECT_KEYWORD = "DISTINCT"
                    Else
                     TableAlias.SELECT_KEYWORD = "NULL"
                End If
                TableAlias.SELECT_ADDITIONAL = "NULL"
                
                theJoin = theBaseLinkInfo(0) & "." & theBaseLinkInfo(1) & "=" & theTableInfo(3) & "." & theTableInfo(2)
                TableAlias.SELECT_JOIN = theJoin
                theSelectLinks = theBaseLinkInfo(0) & "." & theBaseLinkInfo(1) & ";" & theBaseLinkInfo(2) & "," & theTableInfo(3) & "." & theTableInfo(2) & ";" & theBaseLinkInfo(2)
                TableAlias.SELECT_LINKS = theSelectLinks
                 tableNumber = masterTableIndex - 1
                    If Not tableNumber <= 1 Then
                        InterTable = TableNames.Item(tableNumber)
                    Else
                        InterTable = "NULL"
                    End If
               
                TableAlias.INTER_TABLES = InterTable
                If theTableName = theBaseTable Then
                    theBaseConn = theTableInfo(0)
                    CreateADOConnGroup theTableName, theTableInfo(0), "base"
                     TableAlias.ADO_CONNECTION = "base_connection"
            
                Else
                If Not theTableInfo(0) = theBaseConn Then
                    CreateADOConnGroup theTableName, theTableInfo(0), "other"
                    TableAlias.ADO_CONNECTION = theTableName & "connection"
                End If
            End If
        If StructureFields <> "" Then
            TableAlias.STRUC_FIELD_ID = GetStrucID(theTableInfo(0))
            If theTableName = theBaseTable Then
                CreateChemConnGroup theDBName, theTableInfo(0), "base"
                TableAlias.CHEM_CONNECTION = theDBName & "cfw_form"
            Else
                CreateChemConnGroup theDBName, theTableInfo(0), "other"
                TableAlias.CHEM_CONNECTION = theTableName & "cfw_form"
            End If
        Else
        TableAlias.CHEM_CONNECTION = "NULL"
        TableAlias.STRUC_FIELD_ID = "NULL"
        End If
     End If
Next
mcolTableAliases.Add TableAlias, theTableName
Set TableAlias = Nothing
Next


Set BuildTable_Aliases = mcolTableAliases

End Function
Function GetStrucID(baseConnInfo)



End Function

Sub CreateADOConnGroup(tableName, baseConnInfo, theType)



End Sub

Sub CreateChemConnGroup(tableName, baseConnInfo, theType)



End Sub

Sub GetLinksInfo(masterIndex)
Dim theTableLink As Collection
Set theTableLink = New Collection
    tableName = TableNames.Item(masterIndex)
   On Error Resume Next
    theLinkStr = TableLinks.Item(tableName)
    If Err.Number = 5 Then Exit Sub
    On Error GoTo 0
    theLinkArray = Split(theLinkStr, "  ", -1)
    temp = Split(theLinkArray(0), ",", -1)
    theTableAbove = temp(0)
    theTableLink.Add temp(1), "boxtype"
    theTableLink.Add theLinkArray(1), "coords"
    theBaseLinkField = Split(theLinkArray(6), " ", -1)
    theTableLink.Add theTableAbove & "," & theBaseLinkField(0) & "," & theLinkArray(5) & ":" & theLinkArray(4), "base_link"
    theTableLink.Add theLinkArray(6), "link_info"
    temp = Split(theLinkArray(6), " ", -1)
    theTableName = temp(2)
    theTableName = Replace(theTableName, """", "")
    Set Table = TablesCollection.Item(theTableName)
    Table.Add theTableLink

End Sub


Sub GetFieldsInfo(theIndex)
Dim theParseString As String
Dim theSingleTable As Collection
Dim theField As Collection
Dim theTable As Collection
Set theTable = New Collection
Dim theLinkStr As String
'get the tables colletion from the input index
Set theSingleTable = ParseTablesCollection.Item(theIndex)
tableName = TableNames.Item(theIndex)
'loop through the individual table's items and get the relational fields name (array index 6) and field types (array index 5)from the string
'each item has the following format that is split at a double space: FBOX boxtype  l t r b  id  dtype  fntno  fldtype  fldname
For i = 1 To theSingleTable.Count
    Set theField = New Collection
    parseStr = theSingleTable.Item(i)
    If InStr(parseStr, "FBOX") = 1 Then
        'split on the double spaces
        parseArray = Split(parseStr, "  ", -1)
        parseArray(6) = Replace(parseArray(6), """", "")
            If Len(parseArray(6)) > 0 Then
                parseArray(1) = Replace(parseArray(1), """", "")
                theField.Add parseArray(0), "box type"
                theField.Add parseArray(1), "coords"
                theField.Add parseArray(5), "field_type"
                theField.Add parseArray(6), "field_name"
                theTable.Add theField
                
             End If
    End If
Set theField = Nothing
Next
TablesCollection.Add theTable, tableName
Set theTable = Nothing
Set theSingleTable = Nothing
End Sub

Function GetSubFormView()

End Function
Function GetADOConnGroups()

End Function

Function GetFieldMapGroups()

End Function
Function GetTableGroups()
End Function
Function GetChemConnNames()

End Function

Function GetFormGroups(inputStr)
RelFields = ""
For masterTableIndex = 1 To TableNames.Count
Set Table = TablesCollection.Item(masterTableIndex)
    theTableName = TableNames.Item(masterTableIndex)
        For j = 1 To Table.Count
        Set TableItem = Table.Item(j)
            
            Select Case TableItem.Item(1)
            Case "FBOX 0", "FBOX 8"
                If Not TableItem.Item(3) = "5" Then
                    If RelFields <> "" Then
                        RelFields = RelFields & "," & theTableName & "." & TableItem.Item(4) & ";" & TableItem.Item(3)
                    Else
                        RelFields = theTableName & "." & TableItem.Item(4) & ";" & TableItem.Item(3)
                    End If
                Else
                    If MolWeightFields <> "" Then
                        MolWeightFields = MolWeightFields & "," & theTableName & "." & TableItem.Item(4)
                    Else
                        MolWeightFields = theTableName & "." & TableItem.Item(4)
                    End If
                End If
              Case "FBOX 4"
                If StructureFields <> "" Then
                        StructureFields = StructureFields & "," & theTableName & "." & TableItem.Item(4)
                    Else
                        StructureFields = theTableName & "." & TableItem.Item(4)
                    End If
                
              Case "FBOX 5"
               If FormulaFields <> "" Then
                        FormulaFields = FormulaFields & "," & theTableName & "." & TableItem.Item(4)
                    Else
                        FormulaFields = theTableName & "." & TableItem.Item(4)
                    End If
                
            End Select
            
            RelFields = Replace(RelFields, """", "")
            MolWeightFields = Replace(MolWeightFields, """", "")
            StructureFields = Replace(StructureFields, """", "")
             FormulaFields = Replace(FormulaFields, """", "")
    Next
Next
theArray = Split(inputStr, ",", -1)
Set mcolFormGroups = New FormGroups

For i = 0 To UBound(theArray)
    Dim FormGroup As New clsFormGroup
    FormGroup.GroupName = theArray(i)
    FormGroup.INPUT_FORM_PATH = LCase(theDBName) & "_input_form.asp"
    FormGroup.INPUT_FORM_MODE = "search"
    FormGroup.RESULT_FORM_PATH = LCase(theDBName) & "_result_list.asp"
    FormGroup.RESULT_FORM_MODE = "list"
    If theArray(i) = "basenp_form_group" Then
        FormGroup.PLUGIN_VALUE = "False"
        FormGroup.STRUCTURE_FIELDS = "NULL"
        FormGroup.MW_FIELDS = MolWeightFields
        FormGroup.FORMULA_FIELDS = FormulaFields
    Else
        FormGroup.PLUGIN_VALUE = "True"
        FormGroup.STRUCTURE_FIELDS = StructureFields
        FormGroup.MW_FIELDS = MolWeightFields
        FormGroup.FORMULA_FIELDS = FormulaFields
    End If
        FormGroup.SEARCHABLE_ADO_FIELDS = RelFields
        mcolFormGroups.Add FormGroup, UCase(theArray(i))
        Set FormGroup = Nothing
Next
Set GetFormGroups = mcolFormGroups
Set mcolFormGroups = Nothing
End Function
Function GetDBName(theCFWFormPath)
    temp = Split(theCFWFormPath, "\", -1)
    FileName = temp(UBound(temp))
    tempDBName = Split(FileName, ".", -1)
    DBName = tempDBName(0)
    GetDBName = DBName
End Function

Function GetCOWSPath()
    Set INIVar = CreateObject("cowsUtils.cowsini")
    theDrive = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DRIVE", "cows.ini")
    theServerRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DIR", "cows.ini")
    theDocumentRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "DOC_ROOT", "cows.ini")
    theCowsRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "COWS_ROOT", "cows.ini")
    theCOWSPath = theDrive & "\" & theServerRoot & "\" & theDocumentRoot & "\" & theCowsRoot
    GetCOWSPath = theCOWSPath
    Set INIVar = Nothing
End Function
Function GetTableNames(FilePath)
Set theNames = New Collection
'find number of subforms
Set ts = OpenCFWOutput(FilePath)
NumSubForms = 0
Do While ts.AtEndOfStream <> True
    s = ts.ReadLine
    If InStr(s, "DSRC") = 1 Then
    temp = Split(s, " ", -1)
    theNames.Add temp(2)
    End If
    
    
Loop
ts.Close
Set GetTableNames = theNames
Set theNames = Nothing

End Function

Sub GetTableLinks(FilePath, NumTables)
Set TableLinks = New Collection
For i = 1 To NumTables
     DSRCcounter = 0
     Set ts = OpenCFWOutput(FilePath)

        Do While ts.AtEndOfStream <> True
        s = ts.ReadLine
        If InStr(s, "DSRC") = 1 Then
            DSRCcounter = DSRCcounter + 1
            temp = Split(s, " ", -1)
            tableName = temp(UBound(temp))
        End If
        If DSRCcounter = i Then
            If InStr(s, "FBOX 6") = 1 Then
                TableLinks.Add tableName & s, tableName
            End If
        End If
    Loop
    ts.Close
Next
End Sub

'c:\Inetpub\samples\cs_demo.cfw
Sub ParseCFWOutput(ByVal FilePath)
Dim Table As Collection
Dim subformcount As Integer
Dim NumTables As Integer
Dim tableName As String
Dim StoreFBOX As String
Set TablesCollection = New Collection
Set TableLinks = New Collection
Set TableLinkNames = New Collection
Set TableNames = New Collection
Set TableNames = GetTableNames(FilePath)
 
NumTables = TableNames.Count
'get base table
Set Table = New Collection
Set ts = OpenCFWOutput(FilePath)
subformcount = 0
Do While ts.AtEndOfStream <> True
    s = ts.ReadLine
    
    If InStr(s, "SUBFORM") = 1 Then subformcount = subformcount + 1
    If InStr(s, "END SUBFORM") = 1 Then subformcount = subformcount - 1
    If InStr(s, "DSRC") = 1 And subformcount = 0 Then
        temp = Split(s, " ", -1)
        tableName = temp(UBound(temp))
        Table.Add s, tableName
    End If
    If InStr(s, "FBOX") = 1 And Not (InStr(s, "FBOX 6") = 1) And subformcount = 0 Then Table.Add s
    If InStr(s, "FBOX 6") = 1 And subformcount = 0 Then
        TableLinkNames.Add tableName, tableName
    End If
Loop
ts.Close
ParseTablesCollection.Add Table
Set Table = Nothing

'get subforms
Dim current_subform As Integer
Dim subform_counter As Integer
Dim start_current_read As Boolean
Dim foreign_subform As Integer
For current_subform = 1 To NumTables - 1 'remove base table reference
subform_counter = 0
start_current_read = False
foreign_subform = 0
Set Table = New Collection
    Set ts = OpenCFWOutput(FilePath)
    Do While ts.AtEndOfStream <> True
        s = ts.ReadLine
        
        If InStr(s, "SUBFORM") = 1 Then
            subform_counter = subform_counter + 1
        End If
        If subform_counter = current_subform And foreign_subform = 0 Then start_current_read = True
        If start_current_read = True Then
        
        If InStr(s, "FBOX 6") = 1 Then
            If subform_counter > 0 Then
                'If foreign_subform = 0 Then
                currentTableName = TableNames.Item(subform_counter + 1)
                 StoreFBOX = currentTableName
            End If
            foreign_subform = foreign_subform + 1
           
        End If
        If InStr(s, "END SUBFORM") = 1 Then
            If Not foreign_subform = 0 Then foreign_subform = foreign_subform - 1
            subform_counter = subform_counter - 1
        End If
        If InStr(s, "DSRC") = 1 And subform_counter = current_subform And foreign_subform = 0 And start_current_read = True Then
            temp = Split(s, " ", -1)
            tableName = temp(UBound(temp))
            Table.Add s, tableName
            If StoreFBOX <> "" Then
         TableLinkNames.Add StoreFBOX, tableName
         End If
         StoreFBOX = ""
         End If
      
       
        If InStr(s, "FBOX") = 1 And subform_counter = current_subform And foreign_subform = 0 And start_current_read = True Then Table.Add s
               End If
    Loop
    ParseTablesCollection.Add Table
    ts.Close
    Set ts = Nothing
    Set Table = Nothing
Next current_subform
Set ts = OpenCFWOutput(FilePath)
Counter = 1
Do While ts.AtEndOfStream <> True
        s = ts.ReadLine
        If InStr(s, "FBOX 6") = 1 Then
        theName = TableLinkNames.Item(Counter)
        temp = Split(s, "  ", -1)
        temp2 = Split(temp(6), """ """, -1)
        tableName = temp2(2)
        TableLinks.Add theName & "," & s, tableName
        Counter = Counter + 1
        End If
        
Loop
ts.Close
        Set ts = Nothing
End Sub
Function OpenCFWOutput(ByVal outputpath) As Object
    Const ForReading = 1, ForWriting = 2, ForAppending = 3
    Const TristateUseDefault = -2, TristateTrue = -1, TristateFalse = 0
    Dim fs, f, ts, s
    Set fs = CreateObject("Scripting.FileSystemObject")
    Set f = fs.GetFile(outputpath)
    Set ts = f.OpenAsTextStream(ForReading, TristateUseDefault)
    Set OpenCFWOutput = ts
End Function
Function doAddToString(ByVal varname, ByVal inputStr)
    If varname <> "" Then
      varname = varname & "," & inputStr
    Else
        varname = inputStr
    End If
    doAddToString = varname
End Function
Function GetCFWASCII(theCFWFormPath)
    On Error Resume Next
    Set CFWApp = CreateObject("ChemFinder.Application")
    'CFWApp.Visible = True
    Set CFWDoc = CFWApp.Documents.Open(theCFWFormPath)
    'CFWDoc.Activate
    outputpath = "c:\inetpub\wwwroot\chemoffice\cfwtemp\cfwoutput.txt"
    success = CFWDoc.SaveAs(outputpath)
    CFWDoc.Close
    Set CFWDoc = Nothing
    CFWApp.Quit
    Set CFWApp = Nothing
    GetCFWASCII = outputpath
End Function

Function GetDBRecordCount(theCFWFormPath)
    Set CFWApp = CreateObject("ChemFinder.Application")
    Set CFWDoc = CFWApp.Documents.Open(theCFWFormPath)
    On Error Resume Next
    CFWDoc.ExecuteCALCommand ("Search Retrieve All")
    GetDBRecordCount = CFWDoc.Recordset.RecordCount
    CFWDoc.Close
    Set CFWDoc = Nothing
    CFWApp.Quit
    Set CFWApp = Nothing
End Function


Function GetFields(thePath)
    Set CFWApp = CreateObject("ChemFinder.Application")
    Set CFWDoc = CFWApp.Documents.Open(thePath)
    Dim FieldObject As Object
    Dim FieldName As String
    For Each FieldObject In CFWDoc.Fields
        FieldName = FieldObject.Name
        FieldType = FieldObject.Type
    Next FieldObject
End Function
