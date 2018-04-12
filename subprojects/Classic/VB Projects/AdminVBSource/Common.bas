Attribute VB_Name = "Common"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F45B0107"
'Option Base 0
Dim OutputItems As Collection
Dim BuildADOColl As ADOConnGroups
Dim BuildChemColl As ChemConnGroups
Dim TablesCollection As Collection
Dim ParsedTablesCollection As Collection
Dim ParsedLinksCollection As Collection
Dim ParsedConnectionCollection As Collection
Dim FormFonts As Collection
Dim TableLinks As Collection
Dim theDBName
Dim TableLinkNames As Collection
Dim DB As clsDB
Dim TableNames As Collection
Dim TableDBPaths As Collection
Dim InputFormFields As Collection
Dim TextInputFormFields As Collection
Dim AppendFields As Collection
Dim ResultsListFields As Collection
Dim TextResultsListFields As Collection
Dim ResultsColumnHeaders As Collection
Dim CommentedOutListFields As Collection
Dim CommentedOutRecordSet As Collection
Dim ResultsFormFields As Collection

Dim DataConnFormOpen As Collection
Dim DataConnFormClose As Collection
Dim DataConnListOpen As Collection
Dim DataConnListClose As Collection
Dim RecordSetListClose As Collection
Dim RecordSetFormClose As Collection



Dim TextResultsFormFields As Collection
Dim RecordSetList As Collection
Dim RecordSetForm As Collection
Dim JavaScripts As Collection
Public AppApp As clsApp
Public AppGlobals As New clsAppGlobals
Public INIVar As Object
Public DBGlobals As New clsDBGlobals
Public theDB As clsDB
Public mcolDBs As DBs
Dim gCFWFormPath As String
Dim RequestValues As Collection
Dim DebugMode As Boolean
Dim TableDupNames As New Collection
Dim AllTableNames As New Collection

Dim m_strMolServerVersion As String ' module variable to track the CFW version to instantiate


Const gs_stndSep = """" & " " & """"

Private Declare Function AscConvert Lib "Asc67Cvt" (ByVal inf As Variant, ByVal outf As Variant) As Boolean

' Win32 declares and constants for "synchronous" shell processing
Public Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, ByVal dwProcessId As Long) As Long
Public Declare Function WaitForSingleObject Lib "kernel32" (ByVal hHandle As Long, ByVal dwMilliseconds As Long) As Long
Public Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Long) As Long
Private Const INFINITE = &HFFFF
Private Const SYNCHRONIZE = &H100000






Sub AddINILineBreaks(theAppName As Variant, theName As Variant)
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theFile.CopyFile thePath & ".ini", thePath & "temp" & ".ini"
Set fsini = theFile.CreateTextFile(thePath & ".ini", True)
Set fstemp = theFile.OpenTextFile(thePath & "temp" & ".ini")
Do While fstemp.AtEndOfStream <> True
tempVar = fstemp.ReadLine
    If InStr(tempVar, "[") > 0 Then
        fsini.WriteBlankLines (1)
        fsini.WriteLine tempVar
    Else
    fsini.WriteLine tempVar
    End If
Loop
fstemp.Close
fsini.Close
Set fstemp = Nothing
Set fsini = Nothing
theFile.DeleteFile thePath & "temp" & ".ini", True

Set theFolder = Nothing
Set theFile = Nothing

End Sub


Sub RenameUDLFile(theAppName As Variant, theName As Variant)
    Set theFile = CreateObject("Scripting.FileSystemObject")
    Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
    cowspath = AppGlobals.COWS_PATH
    new_app_udl_path = cowspath & "\webserver_source\cfserveradmin\adminsource\templates\NewApp\config\dbname"
    thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
    theFile.CopyFile new_app_udl_path & ".udl", thePath & ".udl"
    
    SyncShell "Attrib.exe -R " & theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "*.*" & " /S /D"

    theFile.DeleteFile theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\dbname" & ".udl", True
    Set theFolder = Nothing
    Set theFile = Nothing
    

End Sub



Sub PopulateASPFiles()
'INPUT_FIELD: <%ShowInputField dbkey, formgroup, "MolTable.Molname", "0","30"%>
'BASE_RS:Set BaseRS = GetDisplayRecordset(dbkey, FormGroup, "MolTable.*", "MolTable", "", BaseID, "")
'SUBFORM_RS: Set SynRS= GetDisplayRecordset(dbkey, formgroup,"SYNONYM","SYNONYMS", "", BaseID, "")%>
'CHEMICAL_RESULT_FIELD: <%ShowCFWChemResult dbkey, formgroup, "Structure", "MolTable.Structure", BaseID, "cdx", 200, 200%>
'RESULT_FIELD: <%ShowResult dbkey, formgroup, BaseRS, "MolTable.Molname", "raw", 0, 0%>
'SUBFORM_RESULT_TABLE: <%ShowResultTable dbkey, formgroup, SynRS, "table_down", "200", "100%", "0"%>
'D:\ISI\CCR_FORM1old.CFW
Set InputFormFields = New Collection

Set ResultsListFields = New Collection
Set ResultsFormFields = New Collection
Set DataConnFormOpen = New Collection
Set DataConnFormClose = New Collection
Set DataConnListOpen = New Collection
Set DataConnListClose = New Collection

Set RecordSetList = New Collection
Set RecordSetForm = New Collection
Set RecordSetListClose = New Collection
Set RecordSetFormClose = New Collection

Set AppendFields = New Collection

Set JavaScripts = New Collection
Set ResultsColumnHeaders = New Collection
Dim theItem  As Collection


ListViewStr = RequestValues.Item("FIELDS_LIST")
ListViewArray = Split(ListViewStr, ",", -1)
'baseRS
baseTable = TableNames.Item(1)
baseFields = baseTable & ".*"
DataConnFormOpen.Add "<%Set BaseRSConn = GetConnection(dbkey, formgroup, " & QuotedString(baseTable) & ")%>"
DataConnFormClose.Add "<%CloseConn(BaseRSConn)%>"
DataConnListOpen.Add "<%Set BaseRSConn = GetConnection(dbkey, formgroup, " & QuotedString(baseTable) & ")%>"
DataConnListClose.Add "<%CloseConn(BaseRSConn)%>"

baserecordSetEntry1 = "<%sql=GetDisplaySQL(dbkey, formgroup," & QuotedString(baseFields) & "," & QuotedString(baseTable) & ", """", BaseID, """")%>"
baserecordSetEntry2 = "<%Set BaseRS = BaseRSConn.Execute(sql)%>"
baserecordSetClose = "<%CloseRS(BaseRS)%>"
RecordSetForm.Add baserecordSetEntry1
RecordSetForm.Add baserecordSetEntry2

RecordSetList.Add baserecordSetEntry1
RecordSetList.Add baserecordSetEntry2

RecordSetFormClose.Add baserecordSetClose
RecordSetListClose.Add baserecordSetClose
st = False
For i = 1 To OutputItems.Count
    varJavaScript = ""
   recordSetEntry = ""
   resultfield = ""
   inputfield = ""
   Dim varTableName, varBoxType, varleftCoord, varRightCoord, varWdith, varHeight, varFont As String
    Dim varDataType, varPartialField, varFullFieldName, varSubTable, varUniquieID As String
    Set theItem = OutputItems.Item(i)
    varTableName = theItem.Item("table_name")
    varBoxType = theItem.Item("box_type")
    varleftCoord = theItem.Item("left")
    vartopCoord = theItem.Item("top")
    varRightCoord = theItem.Item("right")
    varBottomCoord = theItem.Item("bottom")
    varWidth = QuotedString(theItem.Item("width") * 0.8)
    varWidthStruc = QuotedString(theItem.Item("width"))
    varHeight = QuotedString(theItem.Item("height"))
    On Error Resume Next
    varFont = CInt(theItem.Item("font_number"))
    varDataType = theItem.Item("field_type")
    varPartialField = theItem.Item("field_names")
    varPartialField = Replace(varPartialField, Chr(34), "")
    varSubTable = theItem.Item("sub_table_name")
    varSubTable = Replace(varSubTable, Chr(34), "")
    varUniqueID = QuotedString(i)
    varFullFieldName = QuotedString(varTableName & "." & varPartialField)
    thePK = UCase(Common.mcolDBs.Item(theDBName).TableAliases(varTableName).PRIMARY_KEY)
    'add font info and add to appropriate collection
    theFontInfo = GetFont(varFont)
        Select Case varBoxType
            Case "FBOX 0" ' Any field OR Molweight
                If varDataType = 5 Then ' Molweight
                    resultfield = "<%ShowCFWChemResult dbkey, formgroup, ""MolWeight""," & varFullFieldName & ", BaseID, ""raw_no_edit"", 1," & varWidth & "%>"
                    varDataType = 0
                Else ' Any other field
                    If varTableName = TableNames.Item(1) Then
                        ' Make Mol_ID non editable
                        If UCase(varPartialField) = "MOL_ID" Then
                            resultfield = "<%ShowResult dbkey, formgroup, BaseRS," & varFullFieldName & ",""raw_no_edit"", 0, 0%>"
                        Else
                            resultfield = "<%ShowResult dbkey, formgroup, BaseRS," & varFullFieldName & ",""raw"", 0, 0%>"
                        End If
                    End If
                End If
                'Only add input fields for base table
                If (varTableName = TableNames.Item(1)) Then
                    inputfield = "<%ShowInputField dbkey, formgroup," & varFullFieldName & "," & varDataType & "," & "18" & "%>"
                End If
            Case "FBOX 1"
                'labeltext = varPartialField
            Case "FBOX 2"
                labeltext = varPartialField
               
            Case "FBOX 3"
            Case "FBOX 4" ' STRUCTURE
                If varTableName = TableNames.Item(1) Then
                    resultfield = "<%ShowCFWChemResult dbkey, formgroup, ""Structure""," & varFullFieldName & ", BaseID, ""cdx""" & "," & varWidthStruc & "," & varHeight & "%>"
                End If
                inputfield = "<%ShowStrucInputField  dbkey, formgroup," & varFullFieldName & "," & varUniqueID & "," & varWidthStruc & "," & varHeight & ", ""AllOptions"", ""SelectList""%>"
            Case "FBOX 5" ' Formula
                 resultfield = "<%ShowCFWChemResult dbkey, formgroup, ""Formula""," & varFullFieldName & ", BaseID, ""raw_no_edit"", 1," & varWidth & "%>"
                 inputfield = "<%ShowInputField dbkey, formgroup," & varFullFieldName & "," & varDataType & "," & "18" & "%>"
            Case "FBOX 6" ' SUBFORM
                
                varTable = varSubTable
                ' TempID is the structure field id
                TempID = mcolDBs.Item(theDBName).TableAliases.Item(varTable).STRUC_FIELD_ID
                LinkingRSStr = mcolDBs.Item(theDBName).TableAliases.Item(varTable).SELECT_JOIN
                TempLink = Split(LinkingRSStr, "=", -1)
                TempLink2 = Split(TempLink(1), ".", -1) ' possible error
                If TempLink2(0) = TableNames.Item(1) Then
                    RS = "Base"
                Else
                    RS = TempLink2(0)
                End If
                LinkField = QuotedString(TempLink2(1))
                LinkingRS = RS & "RS" & "(" & LinkField & ")"
                
                If Not TempID = "NULL" Then ' There is a structure in the subtable
                    If Not InStr(UCase(varPartialField), UCase(TempID)) > 0 Then
                        If varPartialField <> "" Then
                            newVarFields = TempID & "," & varPartialField
                        Else
                            newVarFields = TempID
                        End If
                    Else
                        newVarFields = varPartialField
                    End If
                    
                    
                    chemfields = varTable & "," & "Structure,MolWeight,Formula"
                    
                    resultfield = "<%ShowChemResultTable dbkey, formgroup," & varTable & "RS" & "," & QuotedString("table_across") & "," & QuotedString("100%") & "," & QuotedString("0") & "," & QuotedString(varTable) & "," & QuotedString("MOL_ID") & "," & QuotedString("200") & "," & QuotedString("100") & "," & QuotedString("0") & "," & QuotedString("Structure,MolWeight,Formula") & "%>"
                    
                    
                    recordSetEntry1 = "<%sql=GetDisplaySQL(dbkey, formgroup, " & QuotedString(varTable & ".*") & ", " & QuotedString(varTable) & ", """", " & LinkingRS & ", """")%>"
                    recordSetEntry2 = "<%Set " & varTable & "RS=" & varTable & "Conn" & ".Execute(sql)" & "%>"
                    recordSetClose = "<%CloseRS(" & varTable & "RS)%>"
                    DataConnOpen = "<%Set " & varTable & "Conn=GetConnection(dbkey, formgroup," & QuotedString(varTable) & ")%>"
                    DataConnClose = "<%CloseConn(" & varTable & "Conn)%>"
                Else ' No structure in subtable
                    
                    'resultfield = "<%ShowResultTable dbkey, formgroup," & varTable & "RS" & ", ""table_across""," & varWidth & ", ""100%"", ""0""%>"
                    
                    'DGB to support Add/Edit mode the child table must be rendered using ShowResult
                    'We loop over the fields in the child table
                    displayfield_arr = Split(varPartialField, ",", -1)
                    st = True
                    ' Build the subtable
                    resultfield = vbCrLf & vbTab & vbTab & vbTab & "<table border=1 cellpadding=1 cellspacing=0 bordercolor=#ffffff isSubtable=true>" & vbCrLf
                    resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "<tr>" & vbCrLf
                    For h = 0 To UBound(displayfield_arr, 1)
                        ' we do not want ot add the pk field
                        If UCase(displayfield_arr(h)) <> thePK Then
                            resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & vbTab & "<th bgcolor=#ffffff>" & theFontInfo(0) & displayfield_arr(h) & theFontInfo(1) & "</th>" & vbCrLf
                        End If
                    Next h
                    resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "</tr>" & vbCrLf
                    resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "<%Do While Not " & varTable & "RS.EOF = True" & vbCrLf
                    resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "response.write ""   <tr>""" & vbCrLf
                    For h = 0 To UBound(displayfield_arr, 1)
                        ' we do not want ot add the pk field
                        If UCase(displayfield_arr(h)) <> thePK Then
                            resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "response.write ""       <td>""" & vbCrLf
                            resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "response.write ""       " & Replace(theFontInfo(0), """", "") & """" & vbCrLf
                            resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "ShowResult dbkey, FormGroup, " & varTable & "RS, """ & varTable & "." & displayfield_arr(h) & """, ""raw"", 0, ""40""" & vbCrLf
                            resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "response.write ""      " & theFontInfo(1) & """" & vbCrLf
                            resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "response.write ""       </td>""" & vbCrLf
                        End If
                    Next h
                    resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "response.write ""   </tr>""" & vbCrLf
                    resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & varTable & "RS.MoveNext" & vbCrLf
                    resultfield = resultfield & vbTab & vbTab & vbTab & vbTab & "loop%>" & vbCrLf
                    resultfield = resultfield & vbTab & vbTab & vbTab & "</table>"
                    
                    recordSetEntry1 = "<%sql=GetDisplaySQL(dbkey, formgroup, " & QuotedString(varTable & ".*") & ", " & QuotedString(varTable) & ", """", " & LinkingRS & ", """")%>"
                    recordSetEntry2 = "<%Set " & varTable & "RS=" & varTable & "Conn" & ".Execute(sql)" & "%>"
                    recordSetClose = "<%CloseRS(" & varTable & "RS)%>"
                    DataConnOpen = "<%Set " & varTable & "Conn=GetConnection(dbkey, formgroup," & QuotedString(varTable) & ")%>"
                    DataConnClose = "<%CloseConn(" & varTable & "Conn)%>"
                    
                    
                    
                    'For the input form
                    'We loop over the fields in the child table
                    displayfield_arr = Split(varPartialField, ",", -1)
                    st = True
                    ' Build the subtable
                    inputfield = vbCrLf & vbTab & vbTab & vbTab & "<table border=1 cellpadding=1 cellspacing=0 bordercolor=#d3d3d3 isSubtable=true>" & vbCrLf
                    inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & "<tr>" & vbCrLf
                    For h = 0 To UBound(displayfield_arr, 1)
                        ' we do not want ot add the pk field
                        If UCase(displayfield_arr(h)) <> thePK Then
                            inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & vbTab & "<th bgcolor=#ffffff>" & theFontInfo(0) & displayfield_arr(h) & theFontInfo(1) & "</th>" & vbCrLf
                        End If
                    Next h
                    inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & "</tr>" & vbCrLf
                    
                    inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & "<tr>" & vbCrLf
                    For h = 0 To UBound(displayfield_arr, 1)
                        fieldname = varTableName & "." & displayfield_arr(h)
                        ' we do not want ot add the pk field
                        If UCase(displayfield_arr(h)) <> thePK Then
                            inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & "<td>" & vbCrLf
                            inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & theFontInfo(0) & vbCrLf
                            inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<%ShowInputField dbkey, formgroup, """ & fieldname & """, 0, 18%>" & vbCrLf
                            inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & theFontInfo(1) & vbCrLf
                            inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & "</td>" & vbCrLf
                        End If
                    Next h
                    inputfield = inputfield & vbTab & vbTab & vbTab & vbTab & "</tr>" & vbCrLf
                    inputfield = inputfield & vbTab & vbTab & vbTab & "</table>"
                End If
                Case "FBOX 7"
                  ' varScriptName = Replace(varPartialField, " ", "_") & "_script"
                  ' varBtnWidth = Len(varPartialField)
                   'resultfield = "<Input type = ""button"" name = " & varPartialField & "value=" & varPartialField & "onClick=(" & QuotedString(varScriptName) & ") Width=" & varBtnWidth & ">"
                   'varJavaScript = "//function " & varScriptName & "{ //" & ConvertCaltoJS(varFieldName) & "}"
            Case "FBOX 8"
                    inputfield = "<%ShowInputField dbkey, formgroup," & varFullFieldName & "," & varDataType & "," & varWidth * 0.8 & "%>"
                    If varTableName = TableNames.Item(1) Then
                        resultfield = "<%ShowResult dbkey, formgroup, BaseRS," & varFullFieldName & ",""raw"", 0, 0%>"
                    End If
        End Select
        
        'NOTE: need to have an if statement for results list obtained from the wizard
        If resultfield <> "" Then
            If Not varBoxType = "FBOX 7" Then
                If Not (varBoxType = "FBOX 2" Or varBoxType = "FBOX 1") Then
                    If checkSelectedFields(ListViewArray, varTableName, varPartialField, varBoxType) = True Then
                            ResultsListFields.Add theFontInfo(0) & resultfield & theFontInfo(1)
                            If varBoxType = "FBOX 6" Then ' label for subforms
                                LabelsString = ""
                                'DGB Subform header included above.  No label needed here
                                LabelsArray = Split(varPartialField, ",", -1)
                                For j = 0 To UBound(LabelsArray)
                                thestring = Replace(LabelsArray(j), """", "")
                                    If LabelsString <> "" Then
                                        LabelsString = LabelsString & "/" & thestring
                                    Else
                                        LabelsString = thestring
                                    End If
                                Next
                            Else ' label for all other fields
                              If varTableName = TableNames.Item(1) Then
                                  LabelsString = varPartialField
                              End If
                            End If
                            ResultsColumnHeaders.Add "<small><strong>" & LabelsString & "</small></strong>"
                    Else
                            CommentedOutListFields.Add "<!---" & theFontInfo(0) & resultfield & theFontInfo(1) & "--->"
                    End If
                End If
            End If
        End If
        
        If (inputfield <> "") And st Then
            ' Add input fields for sub tables
            InputFormFields.Add "<td nowrap valign = ""top"">" & theFontInfo(0) & varTableName & ":" & theFontInfo(1) & "</td><td valign = ""top"">" & theFontInfo(0) & inputfield & theFontInfo(1) & "</td>" & vbcflf
        End If
        
        If (inputfield <> "") And (varTableName = TableNames.Item(1)) Then
            ' Add input fields for base table
            InputFormFields.Add "<td nowrap valign = ""top"">" & theFontInfo(0) & varPartialField & ":" & theFontInfo(1) & "</td><td valign = ""top"">" & theFontInfo(0) & inputfield & theFontInfo(1) & "</td>" & vbcflf
        End If
        
        ' Add result form fields
        If (resultfield <> "") And st Then
            ' This is a subtable field
            ResultsFormFields.Add "<td align=right nowrap valign=top width=50>" & theFontInfo(0) & varTableName & ":" & theFontInfo(1) & "</td><td bgcolor=#d3d3d3 align=right valign =top>" & theFontInfo(0) & resultfield & theFontInfo(1) & "</td>" & vbCrLf
        Else
            ' This is a basetable field
           If resultfield <> "" Then ResultsFormFields.Add "<td align=right nowrap valign=top>" & theFontInfo(0) & varPartialField & ":" & theFontInfo(1) & "</td><td bgcolor=#d3d3d3 align=right valign =top>" & theFontInfo(0) & resultfield & theFontInfo(1) & "</td>" & vbCrLf
        End If
        
        If recordSetEntry1 <> "" Then
            RecordSetForm.Add recordSetEntry1
            RecordSetForm.Add recordSetEntry2
            RecordSetFormClose.Add recordSetClose
            DataConnFormOpen.Add DataConnOpen
            DataConnFormClose.Add DataConnClose
           If checkRSSelectedFields(ListViewArray, varTableName) = True Then
                RecordSetList.Add recordSetEntry1
                RecordSetList.Add recordSetEntry2
                RecordSetListClose.Add recordSetClose
                DataConnListOpen.Add DataConnOpen
                DataConnListClose.Add DataConnClose
            Else
                CommentedOutRecordSet.Add "<!---" & recordSetEntry & "--->"
            End If
        recordSetEntry1 = ""
        End If
        If varJavaScript <> "" Then
            JavaScripts.Add varJavaScript
        End If
        If labeltext <> "" Then
            If appendtext <> "" Then
                appendtext = appendtext & "&nbsp;" & theFontInfo(0) & labeltext & theFontInfo(1)
            Else
                appendtext = theFontInfo(0) & labeltext & theFontInfo(1)
            End If
        End If
Next i

WriteASPFiles
End Sub
Function checkSelectedFields(theArray, theTableName, theFieldName, theBoxType) As Boolean

    AddToList = False
    If theBoxType = "FBOX 6" Then
        On Error Resume Next
        'varFields = Split theFieldName("
        varTemp = Split(theFieldName, Chr(34), -1)
        theTableName = Replace(varTemp(2), Chr(34), "")
        varTemp(3) = Replace(varTemp(3), Chr(34), "")
        varFields = Split(varTemp(3), ",", -1)
        For i = 0 To UBound(theArray)
            For j = 0 To UBound(varFields)
            test = Trim(theArray(i))
            test2 = theTableName & "." & varFields(j)
                If test = test2 Then
                    AddToList = True
                    Exit For
                End If
            Next j
            If AddToList = True Then Exit For
        Next i
   
    Else
        FullFieldName = theTableName & "." & theFieldName
        For i = 0 To UBound(theArray)
            test = Trim(theArray(i))
            If test = FullFieldName Then
                AddToList = True
                Exit For
            End If
        Next
    End If
    If AddToList = True Then
        checkSelectedFields = True
    Else
        checkSelectedFields = False
    End If

End Function

Function checkRSSelectedFields(theArray, theTableName) As Boolean
    AddToList = False
    For i = 0 To UBound(theArray)
            theSplitArray = Split(theArray(i), ".", -1)
                If Trim(theSplitArray(0)) = theTableName Then
                    AddToList = True
                Exit For
                End If
           
    Next
    If AddToList = True Then
        checkRSSelectedFields = True
    Else
        checkRSSelectedFields = False
    End If

End Function

Function QuotedString(varTemp)
    If IsNull(varTemp) Then
        QuotedString = Chr(34) & Chr(34)
    Else
        QuotedString = Chr(34) & CStr(varTemp) & Chr(34)
    End If
End Function
Sub WriteASPFiles()
Dim fso, fs, appName, dbName
appName = AppGlobals.APP_NAME
dbName = theDBName
'standard javatext for list and form view


Line1 = "getRecordNumber(<%=BaseRunningIndex%>)"
Line2 = "document.write ('<br>')"
Line3 = "getMarkBtn(<%=BaseID%>)"
Line4 = "document.write ('<br>')"
Line5 = "getFormViewBtn(""show_details_btn.gif""," & QuotedString(theDBName & "_form.asp") & ",""<%=BaseActualIndex%>"")"
'end standard text
Set fso = CreateObject("Scripting.FileSystemObject")
cowspath = AppGlobals.COWS_PATH
theAppPath = cowspath & "\" & appName
theDBPath = theAppPath & "\" & dbName
new_db_templates_path = cowspath & "\webserver_source\cfserveradmin\adminsource\templates\NewDB\"
db_dest_path = theDBPath & "\"
'Write Input File
Set fs = fso.OpenTextFile(new_db_templates_path & "db_input_form.asp")
Set fsd = fso.CreateTextFile(db_dest_path & dbName & "_input_form.asp")
Do While fs.AtEndOfStream <> True
tempVar = fs.ReadLine
If InStr(tempVar, "#DB_NAME") > 0 Then
    tempVar = Replace(tempVar, "#DB_NAME", theDBName)
    fsd.WriteLine tempVar
Else
    If InStr(tempVar, "<!--#INPUT_FIELDS-->") > 0 Then
        'Column = 2
        '    For i = 1 To InputFormFields.Count
        '        current_col = i
        '        remainder = (current_col - Column) Mod Column
        '        If i = 1 Then
        '            fsd.writeline "<table border = ""1""><tr><td><table border = ""0"">"
        '            fsd.writeline "<tr>"
        '        End If
        '        fsd.writeline InputFormFields.Item(i)
        '        If remainder = 0 Then fsd.writeline "</tr><tr>"
        '        If i = InputFormFields.Count Then fsd.writeline "</tr>"
        '    Next
           
           
        Column = 2
            current_col = 0
            fsd.WriteLine "<table border=1 cellspacing=0 bordercolor=#d3d3d3>"
            fsd.WriteLine vbTab & "<tr>"
            fsd.WriteLine vbTab & vbTab & "<td valign=top>"
            
            ' Loop and output Structure fields
            For i = 1 To InputFormFields.Count
                isStrucField = InStr(InputFormFields.Item(i), "<%ShowStrucInputField") > 0
                If isStrucField Then
                    fsd.WriteLine vbTab & vbTab & vbTab & "<table border=0 cellspacing=0>"
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & vbTab & InputFormFields.Item(i)
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
                    fsd.WriteLine vbTab & vbTab & vbTab & "</table><BR>"
                End If
            Next
            
            ' Start a new column for the basetable fields
            fsd.WriteLine vbTab & vbTab & "</td>"
            fsd.WriteLine vbTab & vbTab & "<td valign=top>"
            
            ' Loop again to output basetable fields
            fsd.WriteLine vbTab & vbTab & vbTab & "<table border=0 cellpadding=1 cellspacing=2 bordercolor=#eeeeee>"
            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
            For i = 1 To InputFormFields.Count
                isStrucField = InStr(InputFormFields.Item(i), "<%ShowStrucInputField") > 0
                isSubtable = InStr(InputFormFields.Item(i), "isSubtable=true") > 0
                
                If Not (isSubtable Or isStrucField) Then
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & vbTab & InputFormFields.Item(i)
                    current_col = current_col + 1
                    remainder = current_col Mod Column
                    If current_col > 0 And remainder = 0 Then
                        fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
                        fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
                    End If
                End If
            Next
            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
            fsd.WriteLine vbTab & vbTab & vbTab & "</table>"
            fsd.WriteLine vbTab & vbTab & "</td>"
            fsd.WriteLine vbTab & "</tr>"
            fsd.WriteLine "</table><BR>"
            
            ' Loop one last time to get child tables
            For i = 1 To InputFormFields.Count
                isSubtable = InStr(InputFormFields.Item(i), "isSubtable=true") > 0
                If isSubtable Then
                    fsd.WriteLine "<table>" & vbCrLf
                    fsd.WriteLine vbTab & "<tr>" & vbCrLf
                    fsd.WriteLine vbTab & vbTab & InputFormFields.Item(i) & vbCrLf
                    fsd.WriteLine vbTab & "</tr>" & vbCrLf
                    fsd.WriteLine "</table><BR>" & vbCrLf
                End If
            Next
                 
           
        fsd.WriteLine tempVar
        'fsd.writeline "</table></td></tr></table>"
        fsd.WriteLine "<!--#INCLUDE VIRTUAL = ""/cfserverasp/source/input_form_footer_vbs.asp""-->"
    Else
        fsd.WriteLine tempVar
    End If
End If
Loop
fsd.Close
fs.Close


'Write Input no plugin File
Set fs = fso.OpenTextFile(new_db_templates_path & "db_input_formnp.asp")
Set fsd = fso.CreateTextFile(db_dest_path & dbName & "_input_formnp.asp")
Do While fs.AtEndOfStream <> True
tempVar = fs.ReadLine
If InStr(tempVar, "#DB_NAME") > 0 Then
    tempVar = Replace(tempVar, "#DB_NAME", theDBName)
    fsd.WriteLine tempVar
Else
If InStr(tempVar, "<!--#INPUT_FIELDS-->") > 0 Then
    fsd.WriteLine tempVar
       
    Column = 2
            current_col = 0
            fsd.WriteLine "<table border=1 cellspacing=0 bordercolor=#d3d3d3>"
            fsd.WriteLine vbTab & "<tr>"
            fsd.WriteLine vbTab & vbTab & "<td valign=top>"
            
            ' Loop again to output basetable fields
            fsd.WriteLine vbTab & vbTab & vbTab & "<table border=0 cellpadding=1 cellspacing=2 bordercolor=#eeeeee>"
            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
            For i = 1 To InputFormFields.Count
                isStrucField = InStr(InputFormFields.Item(i), "<%ShowStrucInputField") > 0
                isSubtable = InStr(InputFormFields.Item(i), "isSubtable=true") > 0
                
                If Not (isSubtable Or isStrucField) Then
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & vbTab & InputFormFields.Item(i)
                    current_col = current_col + 1
                    remainder = current_col Mod Column
                    If current_col > 0 And remainder = 0 Then
                        fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
                        fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
                    End If
                End If
            Next
            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
            fsd.WriteLine vbTab & vbTab & vbTab & "</table>"
            fsd.WriteLine vbTab & vbTab & "</td>"
            fsd.WriteLine vbTab & "</tr>"
            fsd.WriteLine "</table><BR>"
            
            ' Loop one last time to get child tables
            For i = 1 To InputFormFields.Count
                isSubtable = InStr(InputFormFields.Item(i), "isSubtable=true") > 0
                If isSubtable Then
                    fsd.WriteLine "<table>" & vbCrLf
                    fsd.WriteLine vbTab & "<tr>" & vbCrLf
                    fsd.WriteLine vbTab & vbTab & InputFormFields.Item(i) & vbCrLf
                    fsd.WriteLine vbTab & "</tr>" & vbCrLf
                    fsd.WriteLine "</table><BR>" & vbCrLf
                End If
            Next
                     
       
       fsd.WriteLine "<!--#INCLUDE VIRTUAL = ""/cfserverasp/source/input_form_footer_vbs.asp""-->"
Else
    fsd.WriteLine tempVar
End If
End If
Loop
fsd.Close
fs.Close

'Wrirte Results List File

Set fs = fso.OpenTextFile(new_db_templates_path & "db_result_list.asp")
Set fsd = fso.CreateTextFile(db_dest_path & dbName & "_result_list.asp")
Do While fs.AtEndOfStream <> True
tempVar = fs.ReadLine
If InStr(tempVar, "#JAVA_SCRIPT") > 0 Then
    For i = 1 To JavaScripts.Count
        fsd.WriteLine JavaScripts.Item(i)
    Next
Else
If InStr(tempVar, "#DB_NAME") > 0 Then
    tempVar = Replace(tempVar, "#DB_NAME", theDBName)
    fsd.WriteLine tempVar
Else
If InStr(tempVar, "<!--#RS-->") > 0 Then
    For i = 1 To DataConnListOpen.Count
        fsd.WriteLine DataConnListOpen.Item(i)
    Next
    For i = 1 To RecordSetList.Count
        fsd.WriteLine RecordSetList.Item(i)
    Next
    For i = 1 To CommentedOutRecordSet.Count
        fsd.WriteLine CommentedOutRecordSet.Item(i)
    Next
    fsd.WriteLine tempVar
Else


If InStr(tempVar, "<!--#COLUMN_HEADERS-->") > 0 Then
fsd.WriteLine "<table border = ""1""><tr><td></td>"
'fsd.WriteLine "<tr><td></td>"


    For i = 1 To ResultsColumnHeaders.Count
        fsd.WriteLine "<td>" & ResultsColumnHeaders.Item(i) & "</td>"
    Next
fsd.WriteLine "</tr>"
Else
If InStr(tempVar, "<!--#RESULT_FIELDS-->") > 0 Then
fsd.WriteLine "<tr><td>"
'write standard java text
fsd.WriteLine "<script language = ""javascript"">"
fsd.WriteLine Line1
fsd.WriteLine Line2
fsd.WriteLine Line3
fsd.WriteLine Line4
fsd.WriteLine Line5
fsd.WriteLine "</script></td>"
'end write standard java text
    For i = 1 To ResultsListFields.Count
        fsd.WriteLine "<td>" & ResultsListFields.Item(i) & "</td>"
    Next
    fsd.WriteLine "</tr>"
    fsd.WriteLine tempVar
    For i = 1 To RecordSetListClose.Count
        fsd.WriteLine RecordSetListClose.Item(i)
    Next
    For i = 1 To DataConnListClose.Count
        fsd.WriteLine DataConnListClose.Item(i)
    Next
    fsd.WriteLine "<!--#INCLUDE VIRTUAL = ""/cfserverasp/source/recordset_footer_vbs.asp"" -->"
    fsd.WriteLine "</table>"

    
Else
If InStr(tempVar, "<!--#COMMENTED_OUT_RESULT_FIELDS-->") > 0 Then
    'DGB removed commented out fields.  Giving trouble for Add/Edit mode changes
    'For i = 1 To CommentedOutListFields.Count
    '    fsd.WriteLine CommentedOutListFields.Item(i)
    'Next
    'fsd.WriteLine tempVar
Else
    fsd.WriteLine tempVar
End If
End If
End If
End If
End If
End If
Loop
fsd.Close
fs.Close

'Write Result Form
Set fs = fso.OpenTextFile(new_db_templates_path & "db_form.asp")
Set fsd = fso.CreateTextFile(db_dest_path & dbName & "_form.asp")
Do While fs.AtEndOfStream <> True
    tempVar = fs.ReadLine
    If InStr(tempVar, "#JAVA_SCRIPT") > 0 Then
        For i = 1 To JavaScripts.Count
            fsd.WriteLine JavaScripts.Item(i)
        Next
    Else
        If InStr(tempVar, "#DB_NAME") > 0 Then
            tempVar = Replace(tempVar, "#DB_NAME", theDBName)
            fsd.WriteLine tempVar
        Else
            If InStr(tempVar, "<!--#RS-->") > 0 Then
                ' Delete Order
                torder = ""
                For i = DataConnFormOpen.Count To 1 Step -1
                    torder = torder & TableNames.Item(i) & ","
                Next
                fsd.WriteLine "<%table_delete_order=""" & Left(torder, Len(torder) - 1) & """%>"
                For i = 1 To DataConnFormOpen.Count
                    fsd.WriteLine DataConnFormOpen.Item(i)
                Next
                For i = 1 To RecordSetForm.Count
                    fsd.WriteLine RecordSetForm.Item(i)
                    ' Add delete support
                    If i = 2 Then
                        fsd.WriteLine "<%if Not (BaseRS.BOF and BaseRS.EOF) then 'added for applications that have delete capability %>"
                    End If
                Next
                fsd.WriteLine tempVar
            Else
                If InStr(tempVar, "<!--#RESULT_FIELDS-->") > 0 Then
                    Column = 2
                    current_col = 0
                    fsd.WriteLine "<table border=0 cellspacing=0 bordercolor=#dddddd>"
                    fsd.WriteLine vbTab & "<tr>"
                    fsd.WriteLine vbTab & vbTab & "<td valign=top>"
                    
                    ' Loop and output Structure fields
                    For i = 1 To ResultsFormFields.Count
                        isStrucField = InStr(ResultsFormFields.Item(i), "<%ShowCFWChemResult dbkey, formgroup, ""Structure") > 0
                        If isStrucField Then
                            fsd.WriteLine vbTab & vbTab & vbTab & "<table border =""1"" cellspacing=0 bordercolor=""#eeeeee"">"
                            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
                            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & vbTab & ResultsFormFields.Item(i)
                            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
                            fsd.WriteLine vbTab & vbTab & vbTab & "</table><BR>"
                        End If
                    Next
                    
                    ' Start a new column for the basetable fields
                    fsd.WriteLine vbTab & vbTab & "</td>"
                    fsd.WriteLine vbTab & vbTab & "<td valign=top>"
                    
                    ' Loop again to output basetable fields
                    fsd.WriteLine vbTab & vbTab & vbTab & "<table border=0 cellpadding=1 cellspacing=2 bordercolor=#eeeeee>"
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
                    For i = 1 To ResultsFormFields.Count
                        isStrucField = InStr(ResultsFormFields.Item(i), "<%ShowCFWChemResult dbkey, formgroup, ""Structure") > 0
                        isSubtable = InStr(ResultsFormFields.Item(i), "isSubtable=true") > 0
                        
                        If Not (isSubtable Or isStrucField) Then
                            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & vbTab & ResultsFormFields.Item(i)
                            current_col = current_col + 1
                            remainder = current_col Mod Column
                            If current_col > 0 And remainder = 0 Then
                                fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
                                fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
                            End If
                        End If
                    Next
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
                    fsd.WriteLine vbTab & vbTab & vbTab & "</table>"
                    fsd.WriteLine vbTab & vbTab & "</td>"
                    fsd.WriteLine vbTab & "</tr>"
                    fsd.WriteLine "</table><BR>"
                    
                    ' Loop one last time to get child tables
                    For i = 1 To ResultsFormFields.Count
                        isSubtable = InStr(ResultsFormFields.Item(i), "isSubtable=true") > 0
                        If isSubtable Then
                            fsd.WriteLine "<table>" & vbCrLf
                            fsd.WriteLine vbTab & "<tr>" & vbCrLf
                            fsd.WriteLine vbTab & vbTab & ResultsFormFields.Item(i) & vbCrLf
                            fsd.WriteLine vbTab & "</tr>" & vbCrLf
                            fsd.WriteLine "</table><BR>" & vbCrLf
                        End If
                    Next
                    
                    
                    fsd.WriteLine tempVar
                    
                    For i = 1 To RecordSetFormClose.Count
                        fsd.WriteLine RecordSetFormClose.Item(i)
                    Next
                    For i = 1 To DataConnFormClose.Count
                        fsd.WriteLine DataConnFormClose.Item(i)
                    Next
                    ' Added for delete support
                    fsd.WriteLine "<%else 'if BaseRS.BOF and BaseRS.EOF) = true then the record was deleted."
                    fsd.WriteLine "Response.Write ""record deleted"""
                    fsd.WriteLine "End If 'if NOT (BaseRS.BOF and BaseRS.EOF). added for applications that have delete capability %>"
                    fsd.WriteLine "<!--#INCLUDE VIRTUAL = ""/cfserverasp/source/recordset_footer_vbs.asp"" -->"
                Else
                    fsd.WriteLine tempVar
                End If
            End If
        End If
    End If
Loop
fsd.Close
fs.Close


'DGB CSBR-46804  add support for add mode in wizard apps
'Write add_form File
Set fs = fso.OpenTextFile(new_db_templates_path & "db_add_form.asp")
Set fsd = fso.CreateTextFile(db_dest_path & dbName & "_add_form.asp")

' Get an array of tables involved in the formgroup
strTables = Common.mcolDBs.Item(theDBName).FormGroups.Item("add_record_form_group").TABLE_ORDER_FULL_COMMIT
arrTables = Split(strTables, ",")

' Get the special fields for the formgroup
mwField = Common.mcolDBs.Item(theDBName).FormGroups.Item("add_record_form_group").MW_FIELDS
formulaField = Common.mcolDBs.Item(theDBName).FormGroups.Item("add_record_form_group").FORMULA_FIELDS
strucField = Common.mcolDBs.Item(theDBName).FormGroups.Item("add_record_form_group").STRUCTURE_FIELDS
                
Do While fs.AtEndOfStream <> True
tempVar = fs.ReadLine
If InStr(tempVar, "#DB_NAME") > 0 Then
    tempVar = Replace(tempVar, "#DB_NAME", theDBName)
    fsd.WriteLine tempVar
ElseIf InStr(tempVar, ">#DB_TABLE_ORDER") > 0 Then
   ' Figure table order from add record formgroup
    tempVar = Replace(tempVar, ">#DB_TABLE_ORDER", strTables)
    fsd.WriteLine tempVar
Else
    If InStr(tempVar, "<!--#INPUT_FIELDS-->") > 0 Then
        
        Column = 2
            current_col = 0
            fsd.WriteLine "<table border=1 cellspacing=0 bordercolor=#d3d3d3>"
            fsd.WriteLine vbTab & "<tr>"
            fsd.WriteLine vbTab & vbTab & "<td valign=top>"
            
            ' Loop and output Structure fields
            For i = 1 To InputFormFields.Count
                theInputField = InputFormFields.Item(i)
                isStrucField = InStr(theInputField, "<%ShowStrucInputField") > 0
                If isStrucField Then
                    ' Edit the struc field to remove the dropdown
                    theInputField = Replace(theInputField, "AllOptions", "EXACT")
                    theInputField = Replace(theInputField, "SelectList", "EXACT")
                    fsd.WriteLine vbTab & vbTab & vbTab & "<table border=0 cellspacing=0>"
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & vbTab & theInputField
                    fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
                    fsd.WriteLine vbTab & vbTab & vbTab & "</table><BR>"
                End If
            Next
            
            ' Start a new column for the basetable fields
            fsd.WriteLine vbTab & vbTab & "</td>"
            fsd.WriteLine vbTab & vbTab & "<td valign=top>"
            
            ' Loop again to output basetable fields
            fsd.WriteLine vbTab & vbTab & vbTab & "<table border=0 cellpadding=1 cellspacing=2 bordercolor=#eeeeee>"
            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
            For i = 1 To InputFormFields.Count
                theInputField = InputFormFields.Item(i)
                ' Check if the field corresponds to a Primary Key
                isPK = False
                For j = 0 To UBound(arrTables, 1)
                    If InStr(theInputField, arrTables(j) & "." & Common.mcolDBs.Item(theDBName).TableAliases(arrTables(j)).PRIMARY_KEY) > 0 Then
                        isPK = True
                    End If
                Next
                
                
                isStrucField = InStr(theInputField, "<%ShowStrucInputField") > 0
                isSubtable = InStr(theInputField, "isSubtable=true") > 0
                
                If Not (isSubtable Or isStrucField) Then
                    If Not ((isPK Or InStr(UCase(theInputField), "MOL_ID") > 0 Or InStr(theInputField, mwField) > 0 Or InStr(theInputField, formulaField) > 0)) Then
                        fsd.WriteLine vbTab & vbTab & vbTab & vbTab & vbTab & theInputField
                        current_col = current_col + 1
                        remainder = current_col Mod Column
                        If current_col > 0 And remainder = 0 Then
                            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
                            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "<tr>"
                        End If
                    End If
                    
                End If
            Next
            fsd.WriteLine vbTab & vbTab & vbTab & vbTab & "</tr>"
            fsd.WriteLine vbTab & vbTab & vbTab & "</table>"
            fsd.WriteLine vbTab & vbTab & "</td>"
            fsd.WriteLine vbTab & "</tr>"
            fsd.WriteLine "</table><BR>"
            
            ' Loop one last time to get child tables
            For i = 1 To InputFormFields.Count
                theInputField = InputFormFields.Item(i)
                ' Check if the field corresponds to a Primary Key
                'isPK = False
                ' this is not a good way to find the PK so we ignore it for now
                'For j = 0 To UBound(arrTables, 1)
                '    If InStr(theInputField, arrTables(j) & "." & Common.mcolDBs.Item(theDBName).TableAliases(arrTables(j)).PRIMARY_KEY) > 0 Then
                '        isPK = True
                '    End If
                'Next
                
                isSubtable = InStr(theInputField, "isSubtable=true") > 0
                If isSubtable Then
                    If Not (InStr(UCase(theInputField), "MOL_ID") > 0 Or InStr(theInputField, mwField) > 0 Or InStr(theInputField, formulaField) > 0) Then
                        fsd.WriteLine "<table>" & vbCrLf
                        fsd.WriteLine vbTab & "<tr>" & vbCrLf
                        fsd.WriteLine vbTab & vbTab & theInputField & vbCrLf
                        fsd.WriteLine vbTab & "</tr>" & vbCrLf
                        fsd.WriteLine "</table><BR>" & vbCrLf
                    End If
                End If
            Next
           
        fsd.WriteLine tempVar
        'fsd.writeline "</table></td></tr></table>"
        fsd.WriteLine "<!--#INCLUDE VIRTUAL = ""/cfserverasp/source/input_form_footer_vbs.asp""-->"
    Else
        fsd.WriteLine tempVar
    End If
End If
Loop
fsd.Close
fs.Close


'fso.CopyFile new_db_templates_path & "db_input_formnp.asp", db_dest_path & dbName & "_input_formnp.asp"
'fso.CopyFile new_db_templates_path & "db_result_list.asp", db_dest_path & dbName & "_result_list.asp"
'fso.CopyFile new_db_templates_path & "db_global_input_form.asp", db_dest_path & dbName & "_global_input_form.asp"

End Sub
Function ConvertCaltoJS(varFieldName)
Set f = CreateObject("Scripting.FileSystemObject")
On Error Resume Next
Set fo = f.OpenFile(gCFWFormPath & varFieldName & "_.cfs")
    If Err.Number > 0 Then
        ConvertCaltoJS = "no file found"
    Else
        ConvertCaltoJS = "not done"
    End If
End Function


Function GetFont(theItem)
theItem = CInt(theItem)
theItem = CInt(theItem + 1)
Dim theEntry As Integer
theEntry = theItem
Dim theVal(1) As String
theStr = FormFonts.Item(theEntry)
theArray = Split(theStr, " ", -1)
Count = UBound(theArray)
For i = 6 To Count
    If FontName <> "" Then
        FontName = FontName & " " & Trim(theArray(i))
    Else
        FontName = Trim(theArray(i))
    End If
Next
'8,10,12,14,18,24,36

FontSize = (theArray(1) / 8 * 4) - 3
    Select Case theArray(2)
        Case "700"
            StartStyle = "<strong>"
            EndStyle = "</strong>"
        Case "65936"
            StartStyle = "<em><strong>"
            EndStyle = "</strong></em>"
        Case "400"
            StartStyle = "<em>"
            EndStyle = "</em>"
        Case "0"
            StartStyle = ""
            EndStyle = ""
    End Select
FontColor = RGB(theArray(3), theArray(4), theArray(5))

FontColor = Hex(FontColor)
If FontColor = "0" Then FontColor = "000000"
StartFont = "<font face=""" & FontName & """ size=""" & FontSize & """ color=""#" & FontColor & """>"
EndFont = "</font>"
theVal(0) = StartStyle & StartFont
theVal(1) = EndFont & EndStyle
GetFont = theVal
End Function

Sub CreateAppStructure()

Dim fso, fs, appName, dbName
appName = AppGlobals.APP_NAME
dbName = theDBName
Set fso = CreateObject("Scripting.FileSystemObject")
cowspath = AppGlobals.COWS_PATH
theAppPath = cowspath & "\" & appName
'On Error Resume Next
Set fo = fso.CreateFolder(theAppPath)
new_app_templates_path = cowspath & "\webserver_source\cfserveradmin\adminsource\templates\NewApp\*"
app_dest_path = theAppPath & "\"
fso.CopyFolder new_app_templates_path, app_dest_path
fso.CopyFile new_app_templates_path, app_dest_path
'Set fo = fso.CreateFolder(app_dest_path & "config")
SyncShell "Attrib.exe -R " & theAppPath & "\" & "*.*" & " /S /D"

'Write Global asa
app_templates_path = cowspath & "\webserver_source\cfserveradmin\adminsource\templates\NewApp\"
Set fs = fso.OpenTextFile(app_templates_path & "global.asa")
Set fsd = fso.CreateTextFile(app_dest_path & "global.asa")
Do While fs.AtEndOfStream <> True
tempVar = fs.ReadLine
If InStr(tempVar, "#APP_KEY") > 0 Then
    tempVar = Replace(tempVar, "#APP_KEY", appName)
    fsd.WriteLine tempVar
Else
    fsd.WriteLine tempVar
End If
Loop
fs.Close
fsd.Close

Set fso = Nothing
Set fs = Nothing
Set fsd = Nothing
End Sub
Sub CreateDBStructure()
Dim fso, fs, appName, dbName
appName = AppGlobals.APP_NAME
dbName = theDBName
Set fso = CreateObject("Scripting.FileSystemObject")
cowspath = AppGlobals.COWS_PATH
theAppPath = cowspath & "\" & appName
theDBPath = theAppPath & "\" & dbName
On Error Resume Next
Set fo = fso.CreateFolder(theDBPath)
On Error GoTo 0
cfw_form_templates = cowspath & "\webserver_source\cfserveradmin\adminsource\templates\CFW_Template.cfw"
new_db_templates_path = cowspath & "\webserver_source\cfserveradmin\adminsource\templates\NewDB\"
db_dest_path = theDBPath & "\"
fso.CopyFile new_db_templates_path & "db_action.asp", db_dest_path & dbName & "_action.asp"
fso.CopyFile new_db_templates_path & "mainpage.asp", db_dest_path & "mainpage.asp"
fso.CopyFile new_db_templates_path & "global_input_form.asp", db_dest_path & "global_input_form.asp"

On Error Resume Next
Set fo = fso.CreateFolder(db_dest_path & "cfwforms")
On Error GoTo 0
db_form_path = db_dest_path & "cfwforms\"
For i = 1 To mcolDBs.Item(theDBName).ChemConnGroups.Count

theDataBasepath = mcolDBs.Item(theDBName).ChemConnGroups.Item(i).STRUC_DB_PATH
theTableName = mcolDBs.Item(theDBName).ChemConnGroups.Item(i).STRUC_TABLE_NAME
theFormName = mcolDBs.Item(theDBName).ChemConnGroups.Item(i).STRUC_FORM_NAME

If mcolDBs.Item(theDBName).ChemConnGroups.Item(i).GroupName = "base_cfw_form" Then
     theSourcePath = RequestValues.Item("CFW_FORM_NAME")
    GetCFWForm theSourcePath, theFormName, theDataBasepath, theTableName, db_form_path
Else
    GetCFWForm cfw_form_templates, theFormName, theDataBasepath, theTableName, db_form_path
End If

Next


End Sub

Public Sub RemoveApp(theAppName As Variant)
    
   Dim INIVar As Object
   Set INIVar = CreateObject("cowsUtils.cowsini")
    theDrive = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DRIVE", "cows.ini")
    theServerRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DIR", "cows.ini")
    theDocumentRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "DOC_ROOT", "cows.ini")
    theCowsRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "COWS_ROOT", "cows.ini")
    theCOWSPath = theDrive & "\" & theServerRoot & "\" & theDocumentRoot & "\" & theCowsRoot
    theTempPath = theDrive & "\" & theServerRoot & "\" & theDocumentRoot & "\" & theCowsRoot & "\cfwtemp\"
    thePermPath = theDrive & "\" & theServerRoot & "\" & theDocumentRoot & "\" & theCowsRoot & "\config\chemoffice.ini"
    INIPath = theCOWSPath & "\config\"
    On Error Resume Next
    custom_app_names = INIVar.VBGetPrivateProfileString("GLOBALS", "WIZARD_APPNAMES", INIPath & "chemoffice.ini")
    new_custom_apps = RemoveItemFromDelimetedList(custom_app_names, theAppName, ",")
    success = INIVar.VBWritePrivateProfileString(CStr(new_custom_apps), "GLOBALS", "WIZARD_APPNAMES", CStr(INIPath & "chemoffice.ini"))
    
    Dim fso
    Set fso = CreateObject("Scripting.FileSystemObject")
   
    SyncShell "Attrib.exe -R " & INIPath & "chemoffice.ini"

    
    If fso.fileExists(theTempPath & "temp.ini") Then
        Set f = fso.GetFile(theTempPath & "temp.ini")
        f.Delete
    End If
    If fso.fileExists(theTempPath & "chemoffice.ini") Then
    Set f = fso.GetFile(theTempPath & "chemoffice.ini")
    f.Delete
    End If
    
    Set f = fso.GetFile(thePermPath)
    f.Copy theTempPath & "temp.ini"
    
    
    GroupName = "[" & UCase(theAppName) & "]"
    bTargetFound = False
    
    Set Rts = fso.OpenTextFile(theTempPath & "temp.ini", 1, 0, -2)
    Set Wts = fso.OpenTextFile(theTempPath & "chemoffice.ini", 8, 1)
    Do While Rts.AtEndOfStream <> True
    tempVar = Rts.ReadLine
         
    If InStr(tempVar, GroupName) > 0 Then
        bTargetFound = True
    Else
        If bTargetFound = True Then
        If InStr(tempVar, "[") > 0 And InStr(tempVar, "]") > 0 Then
            bTargetFound = False
            Wts.WriteLine tempVar
        End If
        Else
        Wts.WriteLine tempVar
        End If
    End If
    Loop
    Rts.Close
    Wts.Close
    
    Set f = fso.GetFile(thePermPath)
    f.Delete
    Set f = fso.GetFile(theTempPath & "chemoffice.ini")
    
    f.Copy thePermPath
    
    Set fso = Nothing

   


    
    

End Sub



Public Sub BuildINIVals(taskType, theAppName As Variant)
    Set AppApp = Nothing
    Set AppGlobals = Nothing
    Set DBGlobals = Nothing
    Set theDB = Nothing
    AppGlobals.APP_NAME = theAppName
    taskType = UCase(taskType)
    Select Case taskType
        Case "EDIT"
            GetAppINIVals taskType, theAppName
        Case "ADD"
            GetAppINIVals taskType, theAppName
        Case "NEW"
    End Select
End Sub

Public Sub AddDBtoApp(ByVal newDBName As String)
DBNameStr = AppGlobals.DB_NAMES
newDBNameStr = DBNameStr & "," & inputval
    AppGlobals.DB_NAMES = newDBNameStr
     Set theDB = populateDBs(AppGlobals.APP_NAME, newDBName)
     On Error Resume Next
     mcolDBs.Add theDB, newDBName
    Set AppApp.DBs = mcolDBs

End Sub

Public Sub RemoveDBFromApp(ByVal dbName As String)
DBNameStr = AppGlobals.DB_NAMES
theArray = Split(DBNameStr, ",", -1)
For i = 0 To UBound(theArray)
    If Not theArray(i) = dbName Then
        If theNewNames <> "" Then
            theNewNames = theNewNames & "," & theArray(i)
        Else
            theNewNames = theArray(i)
        End If
    End If
Next

    newDBNameStr = DBNameStr & "," & inputval
    AppGlobals.DB_NAMES = theNewNames
     'Set theDB = populateDBs(AppGlobals.APP_NAME, newDBName)
     On Error Resume Next
     mcolDBs.Remove dbName
    Set AppApp.DBs = mcolDBs

End Sub

Public Sub GetAppINIVals(taskType, ByVal inputval As String)
    Dim theDrive, theServerRoot, theDocumentRoot, theCowsRoot, theCOWSPath, thePath, DBNameStr As String
    Dim theFile As Object
    Dim theFolder As Object
    Dim INIVar As Object
    Set INIVar = CreateObject("cowsUtils.cowsini")
    theDrive = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DRIVE", "cows.ini")
    theServerRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DIR", "cows.ini")
    theDocumentRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "DOC_ROOT", "cows.ini")
    theCowsRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "COWS_ROOT", "cows.ini")
    theCOWSPath = theDrive & "\" & theServerRoot & "\" & theDocumentRoot & "\" & theCowsRoot
    AppGlobals.COWS_PATH = theCOWSPath

    Set theFile = CreateObject("Scripting.FileSystemObject")
    Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
    thePath = theFolder.Path & "\" & inputval & "\" & "config\"
    DBNameStr = INIVar.VBGetPrivateProfileString("GLOBALS", "DB_NAMES", thePath & "cfserver.ini")
    AppGlobals.APP_NAME = inputval
    AppGlobals.NAV_BUTTONS_GIF_PATH = INIVar.VBGetPrivateProfileString("GLOBALS", "NAV_BUTTONS_GIF_PATH", thePath & "cfserver.ini")
    AppGlobals.MAIN_WINDOW = INIVar.VBGetPrivateProfileString("GLOBALS", "MAIN_WINDOW", thePath & "cfserver.ini")
    AppGlobals.NAV_BAR_WINDOW = INIVar.VBGetPrivateProfileString("GLOBALS", "NAV_BAR_WINDOW", thePath & "cfserver.ini")
    AppGlobals.USER_INFO_WINDOW = INIVar.VBGetPrivateProfileString("GLOBALS", "USER_INFO_WINDOW", thePath & "cfserver.ini")
    
    AppGlobals.TEMP_DIR_NAME = INIVar.VBGetPrivateProfileString("GLOBALS", "TEMP_DIR_NAME", thePath & "cfserver.ini")
    AppGlobals.TEMP_DIR_PATH = INIVar.VBGetPrivateProfileString("GLOBALS", "TEMP_DIR_PATH", thePath & "cfserver.ini")

    AppGlobals.DB_NAMES = DBNameStr
    If UCase(taskType) = "EDIT" Then
            Dim DB_Namesarray As Variant
            DB_Namesarray = Split(DBNameStr, ",", -1)
            Set mcolDBs = New DBs
            Dim i As Integer
            
            For i = 0 To UBound(DB_Namesarray)
                 Set theDB = populateDBs(inputval, DB_Namesarray(i))
                 'On Error Resume Next
                 mcolDBs.Add theDB, DB_Namesarray(i)
                 Set theDB = Nothing
            Next
    End If
Set INIVar = Nothing
Set theFile = Nothing
Set theFolder = Nothing
End Sub

'##ModelId=3778F45D0268
Public Function populateDBs(ByVal theAppName As String, ByVal theName As String) As clsDB
    Dim theFile As Object
    Dim thePath As String
    Dim theFolder As Object
    Dim DB As clsDB
    Set DB = New clsDB
    Set INIVar = CreateObject("cowsUtils.cowsini")
    Set theFile = CreateObject("Scripting.FileSystemObject")
    Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
    thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
    DB.ABOUT_WINDOW = INIVar.VBGetPrivateProfileString("GLOBALS", "ABOUT_WINDOW", thePath & ".ini")
    DB.DISPLAY_NAME = INIVar.VBGetPrivateProfileString("GLOBALS", "DISPLAY_NAME", thePath & ".ini")
    DB.MAXHITS = INIVar.VBGetPrivateProfileString("GLOBALS", "MAXHITS", thePath & ".ini")
    DB.MAIN_PAGE = INIVar.VBGetPrivateProfileString("GLOBALS", "MAIN_PAGE", thePath & ".ini")
    DB.DB_RECORD_COUNT = INIVar.VBGetPrivateProfileString("GLOBALS", "DB_RECORD_COUNT", thePath & ".ini")
    DB.DB_TYPE = INIVar.VBGetPrivateProfileString("GLOBALS", "DB_TYPE", thePath & ".ini")
    
    Dim STRSubFormGroups, STRChemConnGroups, STRFormGroups, STRTableGroups, STRFieldMapGroups, STRTableAliases As String
    STRSubFormGroups = INIVar.VBGetPrivateProfileString("GLOBALS", "SUBFORM_VIEW_NAMES", thePath & ".ini")
    STRADOConnGroups = INIVar.VBGetPrivateProfileString("GLOBALS", "ADO_CONNECTION_NAMES", thePath & ".ini")
    STRChemConnGroups = INIVar.VBGetPrivateProfileString("GLOBALS", "CHEM_CONNECTION_NAMES", thePath & ".ini")
    STRFormGroups = INIVar.VBGetPrivateProfileString("GLOBALS", "FORM_GROUPS", thePath & ".ini")
    STRTableGroups = INIVar.VBGetPrivateProfileString("GLOBALS", "TABLE_GROUPS", thePath & ".ini")
    STRFieldMapGroups = INIVar.VBGetPrivateProfileString("GLOBALS", "FIELD_MAP_GROUPS", thePath & ".ini")
    STRTableAliases = INIVar.VBGetPrivateProfileString("GLOBALS", "TABLE_ALIASES", thePath & ".ini")
    DB.dbName = theName

    Set DB.TableAliases = GetTable_Aliases(theName, STRTableAliases)
    Set DB.SubFormGroups = GetSubForm_Groups(theName, STRSubFormGroups)
    Set DB.ADOConnGroups = GetADO_Conn_Groups(theName, STRADOConnGroups)
    Set DB.ChemConnGroups = GetChem_Conn_Groups(theName, STRChemConnGroups)
    Set DB.TableGroups = GetTable_Groups(theName, STRTableGroups)
    Set DB.FormGroups = GetForm_Groups(theName, STRFormGroups)
    Set DB.FieldMapGroups = GetField_Map_Groups(theName, STRFieldMapGroups)
    
    Set populateDBs = DB
    
    Set DB = Nothing
End Function
'##ModelId=3778F45E035A
Public Function GetField_Map_Groups(ByVal theName As String, ByVal inputStr As String) As FieldMapGroups

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theArray = Split(inputStr, ",", -1)
Set mcolFieldMapGroups = New FieldMapGroups
Dim j As Integer
Dim i As Integer
For i = 0 To UBound(theArray)
    Dim FieldMapGroup As New clsFieldMapGroup
    FieldMapGroup.GroupName = theArray(i)
    FieldMapGroup.STRUC_FIELD_MAP = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "STRUC_FIELD_MAP", thePath & ".ini")
    FieldMapGroup.MW_FIELD_MAP = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "MW_FIELD_MAP", thePath & ".ini")
    FieldMapGroup.FORMULA_FIELD_MAP = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "FORMULA_FIELD_MAP", thePath & ".ini")
    FieldMapGroup.OTHER_FIELD_MAP1 = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "OTHER_FIELD_MAP1", thePath & ".ini")
    FieldMapGroup.OTHER_FIELD_MAP2 = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "OTHER_FIELD_MAP2", thePath & ".ini")
    FieldMapGroup.OTHER_FIELD_MAP3 = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "OTHER_FIELD_MAP3", thePath & ".ini")
    FieldMapGroup.OTHER_FIELD_MAP4 = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "OTHER_FIELD_MAP4", thePath & ".ini")
    FieldMapGroup.OTHER_FIELD_MAP5 = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "OTHER_FIELD_MAP5", thePath & ".ini")
    FieldMapGroup.OTHER_FIELD_MAP6 = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "OTHER_FIELD_MAP6", thePath & ".ini")
   
    
    mcolFieldMapGroups.Add FieldMapGroup, UCase(theArray(i))
    Set FieldMapGroup = Nothing

Next
Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
Set GetField_Map_Groups = mcolFieldMapGroups
Set mcolFieldMapGroups = Nothing
End Function
'##ModelId=3778F460014A
Public Function GetTable_Groups(ByVal theName As String, ByVal inputStr As String) As TableGroups

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)

thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theArray = Split(inputStr, ",", -1)
Set mcolTableGroups = New TableGroups

For i = 0 To UBound(theArray)
    Dim TableGroup As New clsTableGroup
    TableGroup.GroupName = theArray(i)
    TableGroup.Base_Table = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "BASE_TABLE", thePath & ".ini")
    TableGroup.MOLECULE_TABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "MOLECULE_TABLE", thePath & ".ini")
    TableGroup.Table_SQL_Order = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "TABLE_SQL_ORDER", thePath & ".ini")
 
    mcolTableGroups.Add TableGroup, UCase(theArray(i))
    Set TableGroup = Nothing
Next
Set GetTable_Groups = mcolTableGroups
Set mcolTableGroups = Nothing
End Function
'##ModelId=3778F46100AB
Public Function GetForm_Groups(ByVal theName As String, ByVal inputStr As String) As FormGroups

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theArray = Split(inputStr, ",", -1)
Set mcolFormGroups = New FormGroups

For i = 0 To UBound(theArray)
    Dim FormGroup As New clsFormGroup
    FormGroup.GroupName = theArray(i)
    FormGroup.INPUT_FORM_PATH = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "INPUT_FORM_PATH ", thePath & ".ini")
    FormGroup.INPUT_FORM_MODE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "INPUT_FORM_MODE", thePath & ".ini")
    FormGroup.RESULT_FORM_PATH = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "RESULT_FORM_PATH", thePath & ".ini")
    FormGroup.RESULT_FORM_MODE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "RESULT_FORM_MODE", thePath & ".ini")
    FormGroup.PLUGIN_VALUE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "PLUGIN_VALUE ", thePath & ".ini")
    FormGroup.STRUCTURE_FIELDS = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "STRUCTURE_FIELDS", thePath & ".ini")
    FormGroup.MW_FIELDS = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "MW_FIELDS", thePath & ".ini")
    FormGroup.FORMULA_FIELDS = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "FORMULA_FIELDS", thePath & ".ini")
    FormGroup.SEARCHABLE_ADO_FIELDS = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "SEARCHABLE_ADO_FIELDS ", thePath & ".ini")
    FormGroup.REQUIRED_FIELDS = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REQUIRED_FIELDS", thePath & ".ini")
    FormGroup.SDFILE_FIELDS = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "SDFILE_FIELDS", thePath & ".ini")
    FormGroup.FIELD_MAP_GROUP = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "FIELD_MAP_GROUP", thePath & ".ini")
    FormGroup.FORM_GROUP_FLAG = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "FORM_GROUP_FLAG", thePath & ".ini")
    FormGroup.TABLE_GROUP = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "TABLE_GROUP", thePath & ".ini")
    FormGroup.NUM_LIST_VIEW = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "NUM_LIST_VIEW", thePath & ".ini")
    Select Case FormGroup.FORM_GROUP_FLAG
        Case "GLOBAL_SEARCH"
           FormGroup.FIELD_MAP_GROUP = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "FIELD_MAP_GROUP", thePath & ".ini")

         Case "REG_TEMP"
            FormGroup.REG_TEMP_BASETABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REG_TEMP_BASETABLE", thePath & ".ini")
            FormGroup.REG_TEMP_MOLTABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REG_TEMP_MOLTABLE", thePath & ".ini")

        Case "REG_COMMIT"
            FormGroup.TABLE_ORDER_FULL_COMMIT = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "TABLE_ORDER_FULL_COMMIT", thePath & ".ini")
            FormGroup.TABLE_ORDER_PARTIAL_COMMIT = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "TABLE_ORDER_PARTIAL_COMMIT", thePath & ".ini")
            FormGroup.REG_COMMIT_BASETABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REG_COMMIT_BASETABLE", thePath & ".ini")
            FormGroup.REG_COMMIT_MOLTABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REG_COMMIT_MOLTABLE", thePath & ".ini")
            FormGroup.REG_TEMP_BASETABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REG_TEMP_BASETABLE", thePath & ".ini")
            FormGroup.REG_TEMP_MOLTABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REG_TEMP_MOLTABLE", thePath & ".ini")
            FormGroup.FIELD_MAP_GROUP = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "FIELD_MAP_GROUP", thePath & ".ini")

        Case "INDEX_SEARCH"
            FormGroup.INDEX_DB = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "INDEX_DB", thePath & ".ini")

        Case "ADD_RECORD"
            FormGroup.TABLE_ORDER_FULL_COMMIT = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "TABLE_ORDER_FULL_COMMIT", thePath & ".ini")
            FormGroup.TABLE_ORDER_PARTIAL_COMMIT = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "TABLE_ORDER_PARTIAL_COMMIT", thePath & ".ini")
            FormGroup.REG_COMMIT_BASETABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REG_COMMIT_BASETABLE", thePath & ".ini")
            FormGroup.REG_COMMIT_MOLTABLE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "REG_COMMIT_MOLTABLE", thePath & ".ini")

    End Select
    
    
    mcolFormGroups.Add FormGroup, UCase(theArray(i))
    Set FormGroup = Nothing


Next
Set GetForm_Groups = mcolFormGroups
Set mcolFormGroups = Nothing
End Function

'##ModelId=3778F462002B
Public Function GetChem_Conn_Groups(ByVal theName As String, ByVal inputStr As String) As ChemConnGroups

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theArray = Split(inputStr, ",", -1)
Set mcolChemConnGroups = New ChemConnGroups

For i = 0 To UBound(theArray)
    Dim ChemConnGroup As New clsChemConnGroup
    ChemConnGroup.GroupName = theArray(i)
    ChemConnGroup.STRUC_ENGINE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "STRUC_ENGINE ", thePath & ".ini")
    ChemConnGroup.STRUC_FORM_NAME = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "STRUC_FORM_NAME", thePath & ".ini")
    ChemConnGroup.STRUC_DB_PATH = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "STRUC_DB_PATH", thePath & ".ini")
    ChemConnGroup.STRUC_TABLE_NAME = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "STRUC_TABLE_NAME", thePath & ".ini")
 
    mcolChemConnGroups.Add ChemConnGroup, UCase(theArray(i))
    Set ChemConnGroup = Nothing
Next
Set GetChem_Conn_Groups = mcolChemConnGroups
Set mcolChemConnGroups = Nothing
Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function

'##ModelId=3778F46203E2
Public Function GetADO_Conn_Groups(ByVal theName As String, ByVal inputStr As String) As ADOConnGroups

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theArray = Split(inputStr, ",", -1)
Set mcolADOConnGroups = New ADOConnGroups

For i = 0 To UBound(theArray)
    Dim ADOConnGroup As New clsADOConnGroup
    ADOConnGroup.GroupName = theArray(i)
   
    
    ADOConnGroup.CONN_TYPE = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "CONN_TYPE", thePath & ".ini")
     ConnType = ADOConnGroup.CONN_TYPE
    ADOConnGroup.CONNECTION_STRING = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "CONNECTION_STRING", thePath & ".ini")

    If ConnType = "DSN" Then
        ADOConnGroup.DSN_NAME = ADOConnGroup.CONNECTION_STRING
    Else
        ConnString = ADOConnGroup.CONNECTION_STRING
        theTempArray = Split(ConnString, ";", -1)
        ADOConnGroup.DB_PATH = theTempArray(0)
        ADOConnGroup.DB_DRIVER = theTempArray(1)
    End If
    ADOConnGroup.CONNECTION_TIMEOUT = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "CONNECTION_TIMEOUT", thePath & ".ini")
    ADOConnGroup.COMMAND_TIMEOUT = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "COMMAND_TIMEOUT", thePath & ".ini")
    ADOConnGroup.CONNECTION_USERNAME = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "CONNECTION_USERNAME", thePath & ".ini")
    ADOConnGroup.CONNECTION_PASSWORD = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "CONNECTION_PASSWORD", thePath & ".ini")
    mcolADOConnGroups.Add ADOConnGroup, UCase(theArray(i))
    Set ADOConnGroup = Nothing
Next
Set GetADO_Conn_Groups = mcolADOConnGroups
Set mcolADOConnGroups = Nothing
Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function
'##ModelId=3778F46400B0
Public Function GetTable_Aliases(ByVal theName As String, ByVal inputStr As String) As TableAliases
Dim thePath As String
Dim theArray As Variant
Dim theFile As Object
Dim theFolder As Object
Dim mcolTableAliases As TableAliases

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theArray = Split(inputStr, ",", -1)
Set mcolTableAliases = New TableAliases
Dim i As Integer

For i = 0 To UBound(theArray)
    Dim TableAlias As New clsTableAlias
    TableAlias.GroupName = theArray(i)
    TableAlias.TABLE_ALIAS_NAME = UCase(theArray(i))
    TableAlias.RELATIONAL_FIELDS = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "RELATIONAL_FIELDS", thePath & ".ini")
    TableAlias.PRIMARY_KEY = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "PRIMARY_KEY", thePath & ".ini")
    TableAlias.TABLE_NAME = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "TABLE_NAME", thePath & ".ini")
    TableAlias.SQL_SYNTAX = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "SQL_SYNTAX", thePath & ".ini")
    TableAlias.SELECT_KEYWORD = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "SELECT_KEYWORD", thePath & ".ini")
    TableAlias.SELECT_ADDITIONAL = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "SELECT_ADDITIONAL", thePath & ".ini")
    TableAlias.SELECT_JOIN = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "SELECT_JOIN", thePath & ".ini")
    TableAlias.SELECT_LINKS = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "SELECT_LINKS", thePath & ".ini")
    TableAlias.INTER_TABLES = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "INTER_TABLES", thePath & ".ini")
    TableAlias.ADO_CONNECTION = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "ADO_CONNECTION", thePath & ".ini")
    TableAlias.CHEM_CONNECTION = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "CHEM_CONNECTION", thePath & ".ini")
    TableAlias.STRUC_FIELD_ID = INIVar.VBGetPrivateProfileString(UCase(theArray(i)), "STRUC_FIELD_ID", thePath & ".ini")
    mcolTableAliases.Add TableAlias, UCase(theArray(i))
    Set TableAlias = Nothing
Next
Set GetTable_Aliases = mcolTableAliases
Set mcolTableAliases = Nothing
Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function


'##ModelId=3778F46500B1
Public Function GetSubForm_Groups(ByVal theName As String, ByVal inputStr As String) As Variant

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theArray = Split(inputStr, ",", -1)
Set mcolSubFormGroups = New SubFormGroups

For i = 0 To UBound(theArray)
    Dim SubFormGroups As New clsSubFormGroup
    'Table.SubFormGroups = theArray(i)
   ' SubFormGroups.SUB_FORM_GROUP_NAME = UCase(theArray(i))
    Set SubFormGroups = Nothing
Next
Set GetSubForm_Groups = mcolSubFormGroups
Set mcolSubFormGroups = Nothing
Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function



Public Sub WriteINIVals(theAppName As Variant)
    SetAppINIVals (theAppName)
End Sub
'Public Function VBWritePrivateProfileString(KeyValue$, section$, key$, File$)

Public Sub SetAppINIVals(taskType)
taskType = UCase(taskType)
appName = AppGlobals.APP_NAME
dbName = theDBName
Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & appName & "\" & "config\"

Select Case taskType
    Case "EDIT"
        success = INIVar.VBWritePrivateProfileString(AppGlobals.APP_NAME, "GLOBALS", "APP_NAME", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.NAV_BUTTONS_GIF_PATH, "GLOBALS", "NAV_BUTTONS_GIF_PATH", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.MAIN_WINDOW, "GLOBALS", "MAIN_WINDOW", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.NAV_BAR_WINDOW, "GLOBALS", "NAV_BAR_WINDOW", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.USER_INFO_WINDOW, "GLOBALS", "USER_INFO_WINDOW", thePath & "cfserver.ini")

        success = INIVar.VBWritePrivateProfileString(AppGlobals.TEMP_DIR_NAME, "GLOBALS", "TEMP_DIR_NAME", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.TEMP_DIR_PATH, "GLOBALS", "TEMP_DIR_PATH", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.DB_NAMES, "GLOBALS", "DB_NAMES", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString("true", "GLOBALS", "CDX_CACHING", thePath & "cfserver.ini")

         DBNames = AppGlobals.DB_NAMES
        success = INIVar.VBWritePrivateProfileString(Trim(DBNames), "GLOBALS", "GLOBAL_SEARCH_DBS", thePath & "cfserver.ini")
        If InStr(DBNames, ",") > 0 Then
            DB_Namesarray = Split(DBNames, ",", -1)
            success = INIVar.VBWritePrivateProfileString(Trim(DB_Namesarray(0)), "GLOBALS", "GLOBAL_SEARCH_BASE_DB", thePath & "cfserver.ini")
        Else
            success = INIVar.VBWritePrivateProfileString(Trim(DBNames), "GLOBALS", "GLOBAL_SEARCH_BASE_DB", thePath & "cfserver.ini")
        End If
        For i = 0 To UBound(DB_Namesarray)
            WriteDBINI appName, DB_Namesarray(i)
        Next
    
    Case "NEW"
        success = INIVar.VBWritePrivateProfileString(AppGlobals.APP_NAME, "GLOBALS", "APP_NAME", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.NAV_BUTTONS_GIF_PATH, "GLOBALS", "NAV_BUTTONS_GIF_PATH", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.MAIN_WINDOW, "GLOBALS", "MAIN_WINDOW", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.NAV_BAR_WINDOW, "GLOBALS", "NAV_BAR_WINDOW", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.USER_INFO_WINDOW, "GLOBALS", "USER_INFO_WINDOW", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.TEMP_DIR_NAME, "GLOBALS", "TEMP_DIR_NAME", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.TEMP_DIR_PATH, "GLOBALS", "TEMP_DIR_PATH", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString(AppGlobals.DB_NAMES, "GLOBALS", "DB_NAMES", thePath & "cfserver.ini")
        success = INIVar.VBWritePrivateProfileString("true", "GLOBALS", "CDX_CACHING", thePath & "cfserver.ini")
        

        DBNames = AppGlobals.DB_NAMES
        success = INIVar.VBWritePrivateProfileString(Trim(DBNames), "GLOBALS", "GLOBAL_SEARCH_DBS", thePath & "cfserver.ini")
        If InStr(DBNames, ",") > 0 Then
            DB_Namesarray = Split(DBNames, ",", -1)
            success = INIVar.VBWritePrivateProfileString(Trim(DB_Namesarray(0)), "GLOBALS", "GLOBAL_SEARCH_BASE_DB", thePath & "cfserver.ini")
        Else
            success = INIVar.VBWritePrivateProfileString(Trim(DBNames), "GLOBALS", "GLOBAL_SEARCH_BASE_DB", thePath & "cfserver.ini")
        End If
        'The list view items are out of sync and need to be added here
        Common.mcolDBs.Item(theDBName).FormGroups.Item("base_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
        Common.mcolDBs.Item(theDBName).FormGroups.Item("basenp_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
        Common.mcolDBs.Item(theDBName).FormGroups.Item("gs_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
        Common.mcolDBs.Item(theDBName).FormGroups.Item("add_record_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
        Common.mcolDBs.Item(theDBName).FormGroups.Item("drill_down_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")

        WriteDBINI appName, theDBName
        AddINILineBreaks appName, theDBName
        
    Case "ADD"
        success = INIVar.VBWritePrivateProfileString(AppGlobals.DB_NAMES, "GLOBALS", "DB_NAMES", thePath & "cfserver.ini")
       ' On Error Resume Next
        DBNames = AppGlobals.DB_NAMES
        success = INIVar.VBWritePrivateProfileString(Trim(DBNames), "GLOBALS", "GLOBAL_SEARCH_DBS", thePath & "cfserver.ini")
        If InStr(DBNames, ",") > 0 Then
            DB_Namesarray = Split(DBNames, ",", -1)
            success = INIVar.VBWritePrivateProfileString(Trim(DB_Namesarray(0)), "GLOBALS", "GLOBAL_SEARCH_BASE_DB", thePath & "cfserver.ini")
        Else
            success = INIVar.VBWritePrivateProfileString(Trim(DBNames), "GLOBALS", "GLOBAL_SEARCH_BASE_DB", thePath & "cfserver.ini")
        End If
       ' On Error GoTo 0
        'The list view items are out of sync and need to be added here
        Common.mcolDBs.Item(theDBName).FormGroups.Item("base_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
        Common.mcolDBs.Item(theDBName).FormGroups.Item("basenp_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
        Common.mcolDBs.Item(theDBName).FormGroups.Item("gs_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
        Common.mcolDBs.Item(theDBName).FormGroups.Item("add_record_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
        Common.mcolDBs.Item(theDBName).FormGroups.Item("drill_down_form_group").NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")

        WriteDBINI appName, theDBName
        AddINILineBreaks appName, theDBName
       
End Select
'INIVAR = ""
Set INIVar = Nothing
theFile = ""
Set theFile = Nothing
theFolder = ""
Set theFolder = Nothing
End Sub

'##ModelId=3778F45D0268
Public Sub WriteDBINI(theAppName As Variant, theName As Variant)
Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
If Not Common.mcolDBs.Item(theName).MAXHITS <> "" Then Common.mcolDBs.Item(theName).MAXHITS = "100"
success = INIVar.VBWritePrivateProfileString(Common.mcolDBs.Item(theName).ABOUT_WINDOW, "GLOBALS", "ABOUT_WINDOW", thePath & ".ini")
success = INIVar.VBWritePrivateProfileString(Common.mcolDBs.Item(theName).DISPLAY_NAME, "GLOBALS", "DISPLAY_NAME", thePath & ".ini")
success = INIVar.VBWritePrivateProfileString(Common.mcolDBs.Item(theName).MAXHITS, "GLOBALS", "MAXHITS", thePath & ".ini")
success = INIVar.VBWritePrivateProfileString(Common.mcolDBs.Item(theName).MAIN_PAGE, "GLOBALS", "MAIN_PAGE", thePath & ".ini")
success = INIVar.VBWritePrivateProfileString(Common.mcolDBs.Item(theName).DB_RECORD_COUNT, "GLOBALS", "DB_RECORD_COUNT", thePath & ".ini")
success = INIVar.VBWritePrivateProfileString(Common.mcolDBs.Item(theName).DB_TYPE, "GLOBALS", "DB_TYPE", thePath & ".ini")
SetTable_Aliases theName
SetSubForm_Groups theName
SetADO_Conn_Groups theName
SetChem_Conn_Groups theName
SetTable_Groups theName
SetForm_Groups theName
SetField_Map_Groups theName


Set DB = Nothing
End Sub
'##ModelId=3778F45E035A
Public Function SetField_Map_Groups(theName As Variant) As Variant
Dim thestring As String
Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
thestring = ""
Dim i As Integer
If Common.mcolDBs.Item(theName).FieldMapGroups.Count > 0 Then
    For i = 1 To Common.mcolDBs.Item(theName).FieldMapGroups.Count
        If thestring <> "" Then
            thestring = thestring & "," & Common.mcolDBs.Item(theName).FieldMapGroups.Item(i).GroupName
        Else
            thestring = Common.mcolDBs.Item(theName).FieldMapGroups.Item(i).GroupName
        End If
    Next
success = INIVar.VBWritePrivateProfileString(thestring, "GLOBALS", "FIELD_MAP_GROUPS", thePath & ".ini")
Else
    success = INIVar.VBWritePrivateProfileString("NULL", "GLOBALS", "FIELD_MAP_GROUPS", thePath & ".ini")

End If
    theArray = Split(thestring, ",", -1)
    For i = 0 To UBound(theArray)
        Set FieldMapGroup = Common.mcolDBs.Item(theName).FieldMapGroups.Item(theArray(i))
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.STRUC_FIELD_MAP, UCase(theArray(i)), "STRUC_FIELD_MAP", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.MW_FIELD_MAP, UCase(theArray(i)), "MW_FIELD_MAP", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.FORMULA_FIELD_MAP, UCase(theArray(i)), "FORMULA_FIELD_MAP", thePath & ".ini")
        If FieldMapGroup.OTHER_FIELD_MAP1 = "" Then FieldMapGroup.OTHER_FIELD_MAP1 = "NULL"
        If FieldMapGroup.OTHER_FIELD_MAP2 = "" Then FieldMapGroup.OTHER_FIELD_MAP2 = "NULL"
        If FieldMapGroup.OTHER_FIELD_MAP3 = "" Then FieldMapGroup.OTHER_FIELD_MAP3 = "NULL"
        If FieldMapGroup.OTHER_FIELD_MAP4 = "" Then FieldMapGroup.OTHER_FIELD_MAP4 = "NULL"
        If FieldMapGroup.OTHER_FIELD_MAP5 = "" Then FieldMapGroup.OTHER_FIELD_MAP5 = "NULL"
        If FieldMapGroup.OTHER_FIELD_MAP6 = "" Then FieldMapGroup.OTHER_FIELD_MAP6 = "NULL"
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.OTHER_FIELD_MAP1, UCase(theArray(i)), "OTHER_FIELD_MAP1", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.OTHER_FIELD_MAP2, UCase(theArray(i)), "OTHER_FIELD_MAP2", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.OTHER_FIELD_MAP3, UCase(theArray(i)), "OTHER_FIELD_MAP3", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.OTHER_FIELD_MAP4, UCase(theArray(i)), "OTHER_FIELD_MAP4", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.OTHER_FIELD_MAP5, UCase(theArray(i)), "OTHER_FIELD_MAP5", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FieldMapGroup.OTHER_FIELD_MAP6, UCase(theArray(i)), "OTHER_FIELD_MAP6", thePath & ".ini")
      
        Set FieldMapGroup = Nothing
    Next
Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function
'##ModelId=3778F460014A
Public Function SetTable_Groups(theName As Variant) As Variant

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
Dim thestring As String
thestring = ""
If Common.mcolDBs.Item(theName).TableGroups.Count > 0 Then

    For i = 1 To Common.mcolDBs.Item(theName).TableGroups.Count
        If thestring <> "" Then
            thestring = thestring & "," & Common.mcolDBs.Item(theName).TableGroups.Item(i).GroupName
        Else
            thestring = Common.mcolDBs.Item(theName).TableGroups.Item(i).GroupName
        End If
    Next
    success = INIVar.VBWritePrivateProfileString(thestring, "GLOBALS", "TABLE_GROUPS", thePath & ".ini")
Else
    success = INIVar.VBWritePrivateProfileString("NULL", "GLOBALS", "TABLE_GROUPS", thePath & ".ini")

End If
    theArray = Split(thestring, ",", -1)
    For i = 0 To UBound(theArray)
        Set TableGroups = Common.mcolDBs.Item(theName).TableGroups.Item(theArray(i))
        success = INIVar.VBWritePrivateProfileString(TableGroups.Base_Table, UCase(theArray(i)), "BASE_TABLE", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableGroups.MOLECULE_TABLE, UCase(theArray(i)), "MOLECULE_TABLE", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableGroups.Table_SQL_Order, UCase(theArray(i)), "TABLE_SQL_ORDER", thePath & ".ini")
        Set TableGroups = Nothing
    Next


Set theFile = Nothing
Set theFolder = Nothing
Set thePath = Nothing
End Function
'##ModelId=3778F46100AB
Public Function SetForm_Groups(theName As Variant) As Variant

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
Dim thestring As String
thestring = ""
If Common.mcolDBs.Item(theName).FormGroups.Count > 0 Then
    For i = 1 To Common.mcolDBs.Item(theName).FormGroups.Count
        If thestring <> "" Then
            thestring = thestring & "," & Common.mcolDBs.Item(theName).FormGroups.Item(i).GroupName
        Else
            thestring = Common.mcolDBs.Item(theName).FormGroups.Item(i).GroupName
        End If
    Next
    success = INIVar.VBWritePrivateProfileString(thestring, "GLOBALS", "FORM_GROUPS", thePath & ".ini")
Else
    success = INIVar.VBWritePrivateProfileString("NULL", "GLOBALS", "FORM_GROUPS", thePath & ".ini")
End If
 theArray = Split(thestring, ",", -1)
 For i = 0 To UBound(theArray)
        Set FormGroups = Common.mcolDBs.Item(theName).FormGroups.Item(theArray(i))
        success = INIVar.VBWritePrivateProfileString(FormGroups.INPUT_FORM_PATH, UCase(theArray(i)), "INPUT_FORM_PATH", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.INPUT_FORM_MODE, UCase(theArray(i)), "INPUT_FORM_MODE", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.RESULT_FORM_PATH, UCase(theArray(i)), "RESULT_FORM_PATH", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.RESULT_FORM_MODE, UCase(theArray(i)), "RESULT_FORM_MODE", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.PLUGIN_VALUE, UCase(theArray(i)), "PLUGIN_VALUE", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.STRUCTURE_FIELDS, UCase(theArray(i)), "STRUCTURE_FIELDS", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.FORM_GROUP_FLAG, UCase(theArray(i)), "FORM_GROUP_FLAG", thePath & ".ini")

        success = INIVar.VBWritePrivateProfileString(FormGroups.MW_FIELDS, UCase(theArray(i)), "MW_FIELDS", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.FORMULA_FIELDS, UCase(theArray(i)), "FORMULA_FIELDS", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.SEARCHABLE_ADO_FIELDS, UCase(theArray(i)), "SEARCHABLE_ADO_FIELDS", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.REQUIRED_FIELDS, UCase(theArray(i)), "REQUIRED_FIELDS", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.SDFILE_FIELDS, UCase(theArray(i)), "SDFILE_FIELDS", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.TABLE_GROUP, UCase(theArray(i)), "TABLE_GROUP", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(FormGroups.NUM_LIST_VIEW, UCase(theArray(i)), "NUM_LIST_VIEW", thePath & ".ini")
        Select Case FormGroups.FORM_GROUP_FLAG
        Case "GLOBAL_SEARCH"
            success = INIVar.VBWritePrivateProfileString(FormGroups.FIELD_MAP_GROUP, UCase(theArray(i)), "FIELD_MAP_GROUP", thePath & ".ini")

         Case "REG_TEMP"
            success = INIVar.VBWritePrivateProfileString(FormGroups.REG_TEMP_BASETABLE, UCase(theArray(i)), "REG_TEMP_BASETABLE", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.REG_TEMP_MOLTABLE, UCase(theArray(i)), "REG_TEMP_MOLTABLE", thePath & ".ini")

        Case "REG_COMMIT"
            success = INIVar.VBWritePrivateProfileString(FormGroups.TABLE_ORDER_FULL_COMMIT, UCase(theArray(i)), "TABLE_ORDER_FULL_COMMIT", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.TABLE_ORDER_PARTIAL_COMMIT, UCase(theArray(i)), "TABLE_ORDER_PARTIAL_COMMIT", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.REG_COMMIT_BASETABLE, UCase(theArray(i)), "REG_COMMIT_BASETABLE", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.REG_COMMIT_MOLTABLE, UCase(theArray(i)), "REG_COMMIT_MOLTABLE", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.REG_TEMP_BASETABLE, UCase(theArray(i)), "REG_TEMP_BASETABLE", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.REG_TEMP_MOLTABLE, UCase(theArray(i)), "REG_TEMP_MOLTABLE", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.FIELD_MAP_GROUP, UCase(theArray(i)), "FIELD_MAP_GROUP", thePath & ".ini")

        Case "INDEX_SEARCH"
            success = INIVar.VBWritePrivateProfileString(FormGroups.INDEX_DB, UCase(theArray(i)), "INDEX_DB", thePath & ".ini")

        Case "ADD_RECORD"
            success = INIVar.VBWritePrivateProfileString(FormGroups.TABLE_ORDER_FULL_COMMIT, UCase(theArray(i)), "TABLE_ORDER_FULL_COMMIT", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.TABLE_ORDER_PARTIAL_COMMIT, UCase(theArray(i)), "TABLE_ORDER_PARTIAL_COMMIT", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.REG_COMMIT_BASETABLE, UCase(theArray(i)), "REG_COMMIT_BASETABLE", thePath & ".ini")
            success = INIVar.VBWritePrivateProfileString(FormGroups.REG_COMMIT_MOLTABLE, UCase(theArray(i)), "REG_COMMIT_MOLTABLE", thePath & ".ini")
     
     End Select
     
     

        Set FormGroups = Nothing
    Next

End Function

'##ModelId=3778F462002B
Public Function SetChem_Conn_Groups(theName As Variant) As Variant

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
Dim thestring As String
thestring = ""
If Common.mcolDBs.Item(theName).ChemConnGroups.Count > 0 Then

    For i = 1 To Common.mcolDBs.Item(theName).ChemConnGroups.Count
        If thestring <> "" Then
            thestring = thestring & "," & Common.mcolDBs.Item(theName).ChemConnGroups.Item(i).GroupName
        Else
            thestring = Common.mcolDBs.Item(theName).ChemConnGroups.Item(i).GroupName
        End If
    Next
    success = INIVar.VBWritePrivateProfileString(thestring, "GLOBALS", "CHEM_CONNECTION_NAMES", thePath & ".ini")
Else
    success = INIVar.VBWritePrivateProfileString(""" """, "GLOBALS", "CHEM_CONNECTION_NAMES", thePath & ".ini")
End If
 theArray = Split(thestring, ",", -1)
 For i = 0 To UBound(theArray)
        Set ChemConnGroups = Common.mcolDBs.Item(theName).ChemConnGroups.Item(theArray(i))
     
        'always molserver form 2001 onward
        success = INIVar.VBWritePrivateProfileString("MOLSERVER", UCase(theArray(i)), "STRUC_ENGINE", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(ChemConnGroups.STRUC_FORM_NAME, UCase(theArray(i)), "STRUC_FORM_NAME", thePath & ".ini")
        'always molserver form 2001 onward change mdb to mst for struc file path
        success = INIVar.VBWritePrivateProfileString(Replace(UCase(ChemConnGroups.STRUC_DB_PATH), ".MDB", ".MST"), UCase(theArray(i)), "STRUC_DB_PATH", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(ChemConnGroups.STRUC_TABLE_NAME, UCase(theArray(i)), "STRUC_TABLE_NAME", thePath & ".ini")
        Set ChemConnGroups = Nothing
    Next
Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function

'##ModelId=3778F46203E2
Public Function SetADO_Conn_Groups(theName As Variant) As Variant

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
theUDLPath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
Dim thestring As String
thestring = ""
If Common.mcolDBs.Item(theName).ADOConnGroups.Count > 0 Then
    For i = 1 To Common.mcolDBs.Item(theName).ADOConnGroups.Count
        If thestring <> "" Then
            thestring = thestring & "," & Common.mcolDBs.Item(theName).ADOConnGroups.Item(i).GroupName
        Else
            thestring = Common.mcolDBs.Item(theName).ADOConnGroups.Item(i).GroupName
        End If
    Next
    success = INIVar.VBWritePrivateProfileString(thestring, "GLOBALS", "ADO_CONNECTION_NAMES", thePath & ".ini")
Else
    success = INIVar.VBWritePrivateProfileString("NULL", "GLOBALS", "ADO_CONNECTION_NAMES", thePath & ".ini")
End If
 theArray = Split(thestring, ",", -1)
 For i = 0 To UBound(theArray)
 'Microsoft.Jet.OLEDB.4.0
        Set ADOConnGroups = Common.mcolDBs.Item(theName).ADOConnGroups.Item(theArray(i))
        If ADOConnGroups.CONN_TYPE = "DBQ" Then
            
            success = INIVar.VBWritePrivateProfileString("""FILE NAME""", UCase(theArray(i)), "CONN_TYPE", thePath & ".ini")
            ADOConnGroups.CONNECTION_STRING = thePath & ".udl"
            On Error Resume Next
            RenameUDLFile AppGlobals.APP_NAME, theName
            success = INIVar.VBWritePrivateProfileString(ADOConnGroups.DB_DRIVER & ";", "OLEDB", "Provider", thePath & ".udl")
            success = INIVar.VBWritePrivateProfileString(ADOConnGroups.DB_PATH & ";", "OLEDB", "Data Source", thePath & ".udl")
            success = INIVar.VBWritePrivateProfileString("False" & ";", "OLEDB", "Persist Security Info", thePath & ".udl")
           
        Else
            success = INIVar.VBWritePrivateProfileString(ADOConnGroups.CONN_TYPE, UCase(theArray(i)), "CONN_TYPE", thePath & ".ini")
            ADOConnGroups.CONNECTION_STRING = ADOConnGroups.DSN_NAME
        End If
        If ADOConnGroups.CONNECTION_USERNAME = "INIEmpty" Then ADOConnGroups.CONNECTION_USERNAME = ""
         If ADOConnGroups.CONNECTION_PASSWORD = "INIEmpty" Then ADOConnGroups.CONNECTION_PASSWORD = ""
        success = INIVar.VBWritePrivateProfileString(ADOConnGroups.CONNECTION_STRING, UCase(theArray(i)), "CONNECTION_STRING", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString("30", UCase(theArray(i)), "CONNECTION_TIMEOUT", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString("30", UCase(theArray(i)), "COMMAND_TIMEOUT", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(ADOConnGroups.CONNECTION_USERNAME, UCase(theArray(i)), "CONNECTION_USERNAME", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(ADOConnGroups.CONNECTION_PASSWORD, UCase(theArray(i)), "CONNECTION_PASSWORD", thePath & ".ini")
 
        Set ADOConnGroups = Nothing
Next

Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function
'##ModelId=3778F46400B0
Public Function SetTable_Aliases(theName As Variant) As Variant

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
Dim thestring As String
thestring = ""
If Common.mcolDBs.Item(theName).TableAliases.Count > 0 Then
    For i = 1 To Common.mcolDBs.Item(theName).TableAliases.Count
        If thestring <> "" Then
            thestring = thestring & "," & Common.mcolDBs.Item(theName).TableAliases.Item(i).GroupName
        Else
            thestring = Common.mcolDBs.Item(theName).TableAliases.Item(i).GroupName
        End If
    Next
        success = INIVar.VBWritePrivateProfileString(thestring, "GLOBALS", "TABLE_ALIASES", thePath & ".ini")
Else
        success = INIVar.VBWritePrivateProfileString("NULL", "GLOBALS", "TABLE_ALIASES", thePath & ".ini")

End If
 theArray = Split(thestring, ",", -1)
For i = 0 To UBound(theArray)
        Set TableAliases = Common.mcolDBs.Item(theName).TableAliases.Item(theArray(i))
        success = INIVar.VBWritePrivateProfileString(TableAliases.TABLE_NAME, UCase(theArray(i)), "TABLE_NAME", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.RELATIONAL_FIELDS, UCase(theArray(i)), "RELATIONAL_FIELDS", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.PRIMARY_KEY, UCase(theArray(i)), "PRIMARY_KEY", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.SQL_SYNTAX, UCase(theArray(i)), "SQL_SYNTAX", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.SELECT_KEYWORD, UCase(theArray(i)), "SELECT_KEYWORD", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.SELECT_ADDITIONAL, UCase(theArray(i)), "SELECT_ADDITIONAL", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.SELECT_JOIN, UCase(theArray(i)), "SELECT_JOIN", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.SELECT_LINKS, UCase(theArray(i)), "SELECT_LINKS", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.INTER_TABLES, UCase(theArray(i)), "INTER_TABLES", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.ADO_CONNECTION, UCase(theArray(i)), "ADO_CONNECTION", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.CHEM_CONNECTION, UCase(theArray(i)), "CHEM_CONNECTION", thePath & ".ini")
        success = INIVar.VBWritePrivateProfileString(TableAliases.STRUC_FIELD_ID, UCase(theArray(i)), "STRUC_FIELD_ID", thePath & ".ini")

        Set TableAliases = Nothing
Next

Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function


'##ModelId=3778F46500B1
Public Function SetSubForm_Groups(theName As Variant) As Variant

Set INIVar = CreateObject("cowsUtils.cowsini")
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & AppGlobals.APP_NAME & "\" & "config\" & theName
Dim thestring As String
thestring = ""
If Common.mcolDBs.Item(theName).SubFormGroups.Count > 0 Then
    For i = 1 To Common.mcolDBs.Item(theName).SubFormGroups.Count
        If thestring <> "" Then
            thestring = thestring & "," & Common.mcolDBs.Item(theName).SubFormGroups.Item(i).GroupName
        Else
            thestring = Common.mcolDBs.Item(theName).SubFormGroups.Item(i).GroupName
        End If
    Next
    success = INIVar.VBWritePrivateProfileString(thestring, "GLOBALS", "SUBFORM_VIEW_NAMES", thePath & ".ini")
Else
    success = INIVar.VBWritePrivateProfileString("NULL", "GLOBALS", "SUBFORM_VIEW_NAMES", thePath & ".ini")
End If
 theArray = Split(thestring, ",", -1)
 For i = 0 To UBound(theArray)
        Set SubFormGroups = Common.mcolDBs.Item(theName).SubFormGroups.Item(theArray(i))
        'success = INIVAR.VBWritePrivateProfileString(SubFormGroups.STRUC_ENGINE, UCase(theArray(i)), "STRUC_ENGINE", thePath & ".ini")
        Set SubFormGroups = Nothing
    Next

Set theFile = Nothing
Set theFolder = Nothing
Set INIVar = Nothing
End Function




Function RunWizard(taskType, theAppName)
   
    theCFWFormPath = RequestValues.Item("CFW_FORM_NAME")
    
    theDBName = GetDBName(theCFWFormPath)
    theFormName = theDBName
    taskType = UCase(taskType)
    Select Case taskType
        Case "NEW"
            Set AppApp = New clsApp
            Set AppGlobals = New clsAppGlobals
            'Set Common.AppApp.AppGlobals = Common.AppGlobals
            AppGlobals.APP_NAME = theAppName
            AppGlobals.DB_NAMES = theDBName
            AppGlobals.MAIN_WINDOW = "top.frames[""main""]"
            AppGlobals.NAV_BAR_WINDOW = "top.frames[""navbar""]"
            AppGlobals.USER_INFO_WINDOW = "top.frames[""userinfo""]"
            AppGlobals.NAV_BUTTONS_GIF_PATH = "/cfserverasp/source/graphics/navbuttons/"
            AppGlobals.TEMP_DIR_NAME = "CFWTEMP"
            AppGlobals.COWS_PATH = GetCOWSPath()
            AppGlobals.TEMP_DIR_PATH = GetCOWSPath() & "\" & Common.AppGlobals.TEMP_DIR_NAME

            Set mcolDBs = New DBs
            RunningTableNameStr = ""
            Set theDB = CreateNewDB(theAppName, theCFWFormPath, theDBName)
            
            mcolDBs.Add theDB, theDBName
         '   Set theDB = Nothing
            UpdateCOPage theAppName, theDBName, "NEW"
            RunWizard = theDBName
            
        Case "ADD"
            temp = AppGlobals.DB_NAMES
            tempArray = Split(temp, ",", -1)
            NameExists = False
            For i = 0 To UBound(tempArray)
                If theDBName = tempArray(i) Then
                    NameExists = True
                End If
            Next
            If NameExists = True Then
                
                  For j = 2 To 20
                   NewNameExists = False
                     For i = 0 To UBound(tempArray)
                        If theDBName & j = tempArray(i) Then
                             NewNameExists = True
                             Exit For
                        End If
                     Next i
                  If NewNameExists = False Then
                    theDBName = theDBName & j
                     Exit For
                 End If
                 Next j
            End If
                   
            newNames = temp & "," & theDBName
            AppGlobals.DB_NAMES = newNames
           
            Set mcolDBs = New DBs
            RunningTableNameStr = ""
            Set theDB = CreateNewDB(theAppName, theCFWFormPath, theDBName)
            mcolDBs.Add theDB, theDBName
            UpdateCOPage theAppName, theDBName, "ADD"
            RunWizard = theDBName
    End Select
    
End Function
Sub UpdateCOPage(theAppName, theDBName, taskType)
standardtext = "/" & theAppName
standardtext = standardtext & "/default.asp?formgroup=base_form_group&dbname="
standardtext = standardtext & theDBName
Dim INIVar As Object
Set INIVar = CreateObject("cowsUtils.cowsini")

Dim cowspath_inipath As String
Dim new_custom_dbnames As String
Dim new_custom_apps As String

cowspath_inipath = "" & AppGlobals.COWS_PATH & "\config\chemoffice.ini" & ""
SyncShell "Attrib.exe -R " & AppGlobals.COWS_PATH & "\config\chemoffice.ini" & " /S /D"

custom_app_name = INIVar.VBGetPrivateProfileString("GLOBALS", "WIZARD_APPNAMES", cowspath_inipath)

If custom_app_name <> "" And Not custom_app_name = "INIEmpty" Then
    If IsInString(custom_app_name, theAppName) = False Then
        new_custom_apps = custom_app_name & "," & theAppName
    Else
        new_custom_apps = custom_app_name
    End If
Else
    new_custom_apps = theAppName
End If
success = INIVar.VBWritePrivateProfileString(CStr(new_custom_apps), "GLOBALS", "WIZARD_APPNAMES", cowspath_inipath)


current_db_names = INIVar.VBGetPrivateProfileString(CStr(theAppName), "DBNAMES", cowspath_inipath)
If current_db_names <> "" And Not current_db_names = "INIEmpty" Then
    If IsInString(current_db_names, theDBName) = False Then
        new_custom_dbnames = current_db_names & "," & theDBName
    Else
        new_custom_dbnames = current_db_names
    End If
Else
    new_custom_dbnames = theDBName
End If
standardtext = Chr(34) & standardtext & Chr(34)
success = INIVar.VBWritePrivateProfileString("WIZARD", UCase(theAppName), "SECTION", cowspath_inipath)
success = INIVar.VBWritePrivateProfileString(CStr(new_custom_dbnames), UCase(theAppName), "DBNAMES", cowspath_inipath)
success = INIVar.VBWritePrivateProfileString(CStr(theAppName & "-" & theDBName), UCase(theAppName), UCase(theDBName) & "_DISPLAY_NAME", cowspath_inipath)
success = INIVar.VBWritePrivateProfileString(CStr(standardtext), UCase(theAppName), UCase(theDBName) & "_START_PATH", cowspath_inipath)
If UCase(taskType) = "NEW" Then
    success = INIVar.VBWritePrivateProfileString("0", UCase(theAppName), "GLOBAL_SEARCH", cowspath_inipath)
    globalsearch = theAppName & " Global Search"
    globalsearchstartpath = Chr(34) & "/" & theAppName & "/default.asp?formgroup=gs_form_group&dbname=" & theDBName & Chr(34)
    success = INIVar.VBWritePrivateProfileString(CStr(globalsearch), UCase(theAppName), "GLOBAL_SEARCH_DISPLAY_NAME", cowspath_inipath)
    success = INIVar.VBWritePrivateProfileString(CStr(globalsearchstartpath), UCase(theAppName), "GLOBAL_SEARCH_START_PATH", cowspath_inipath)
End If
If UCase(taskType) = "ADD" Then
    success = INIVar.VBWritePrivateProfileString("1", UCase(theAppName), "GLOBAL_SEARCH", cowspath_inipath)
End If
End Sub
Function RemoveItemFromDelimetedList(theList, theItem, delimiter) As String
    Dim str
    strList = delimiter & theList & delimiter
    strList = Replace(strList, delimiter & theItem & delimiter, ",", 1)
    If Left(strList, 1) = "," Then strList = Right(strList, Len(strList) - 1)
    If Right(strList, 1) = "," Then strList = Left(strList, Len(strList) - 1)
    RemoveItemFromDelimetedList = strList
End Function



Function IsInString(thestring, thenewitem) As Boolean
  
   Dim theReturn As Boolean
   
   temp = Split(thestring, ",", -1)
   theReturn = False
   For i = 0 To UBound(temp)
        If UCase(temp(i)) = UCase(thenewitem) Then
            theReturn = True
            Exit For
        End If
   Next
   IsInString = theReturn
End Function

Function CreateNewDB(theAppName, theCFWFormPath, theDBName) As clsDB
Dim STRSubFormGroups, STRChemConnGroups, STRFormGroups, STRTableGroups, STRFieldMapGroups, STRTableAliases As String
'theASCIIPath = "C:\inetpub\wwwroot\chemoffice\cfwtemp\cfwoutput.txt"
If DebugMode = True Then
    WriteToLog "CreateNewDB: BeforeCreateNewDB " & Err.Number & Err.Description
End If
theASCIIPath = GetCFWASCII(theCFWFormPath)
If DebugMode = True Then
    WriteToLog "CreateNewDB: AfterGetCFWASCII " & Err.Number & Err.Description
 End If
Set DB = New clsDB
Set ParsedTablesCollection = New Collection

DB.ABOUT_WINDOW = theDBName
DB.DISPLAY_NAME = theDBName
DB.MAXHITS = "100"
DB.MAIN_PAGE = "0"
DB.DB_RECORD_COUNT = GetDBRecordCount(theCFWFormPath)
DB.DB_TYPE = RequestValues.Item("DATA_TYPE")

'returns three collection, TablesCollection,TableLinks and TableNames
ParseCFWOutput theASCIIPath
For i = 1 To TableNames.Count
    If STRTableAliases <> "" Then
        STRTableAliases = STRTableAliases & "," & TableNames.Item(i)
    Else
        STRTableAliases = TableNames.Item(i)
    End If
Next
DB.dbName = theDBName
'in building table aliases, the ado and chem conn groups are built
Set BuildADOColl = New ADOConnGroups
Set BuildChemColl = New ChemConnGroups


Set DB.TableAliases = BuildTable_Aliases(STRTableAliases)
Set DB.FormGroups = BuildForm_Groups("base_form_group,basenp_form_group,gs_form_group,add_record_form_group,drill_down_form_group")
Set DB.TableGroups = Build_TableGroups("base_table_group")
Set DB.FieldMapGroups = Build_FieldMapGroups("gs_field_map_group")

Set DB.ADOConnGroups = BuildADOColl
Set DB.ChemConnGroups = BuildChemColl


Set CreateNewDB = DB

Set DB = Nothing
End Function
Sub SetRequestValue(inputStr, inputStrName)
    On Error Resume Next
    RequestValues.Add inputStr, inputStrName
    If Err.Number > 0 Then
    RequestValues.Remove inputStrName
    RequestValues.Add inputStr, inputStrName
    End If
End Sub
Sub CreateRequestValObj()
    Set RequestValues = Nothing
    Set RequestValues = New Collection
End Sub


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
    GetFieldsInfo masterTableIndex
Next

For i = 0 To UBound(theTables)
    masterTableIndex = i + 1
    GetLinksInfo masterTableIndex
Next


For i = 0 To UBound(theTables)
    masterTableIndex = i + 1
    Dim TableAlias As New clsTableAlias
    RelFields = ""
    
    theTableName = theTables(i)
    TableAlias.GroupName = theTableName
    TableAlias.TABLE_ALIAS_NAME = theTableName
    If InStr(theTableName, "0dup0") > 0 Then
    real_name = Split(theTableName, "0dup0", -1)
        TableAlias.TABLE_NAME = real_name(0)
    Else
        TableAlias.TABLE_NAME = theTableName
    End If

    'RelFields
    RelFields = ""
    StructureFields = ""
    Set Table = TablesCollection.Item(theTableName)
        For j = 1 To Table.Count
        
        Set theField = Table.Item(j)
            If (theField.Item(1) = "FBOX 0" Or theField.Item(1) = "FBOX 8") Then
                    If Not theField.Item(3) = "5" Then
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
        Next j
        Set theField = Nothing
        If Not RelFields <> "" Then RelFields = "NULL"
        TableAlias.RELATIONAL_FIELDS = RelFields
            If i = 0 Then 'this means it is the base table
            
                 
                 theTablePath = TableDBPaths.Item(theTableName)
                 Dim dbEngine
                 dbEngine = GetDBEngine(theTablePath)
                 If Not Trim(dbEngine) <> "" Then
                    dbEngine = "ACCESS"
                 End If
                 TableAlias.SQL_SYNTAX = dbEngine
                 TableAlias.SELECT_KEYWORD = "DISTINCT"
                 TableAlias.SELECT_ADDITIONAL = "NULL"
                 TableAlias.INTER_TABLES = "NULL"

                 theBaseConn = theTablePath
                 theBaseConn = Replace(theBaseConn, """", "")
                 BuildADOConnGroup theTableName, theTablePath, "base", TableAlias.SQL_SYNTAX
                 TableAlias.ADO_CONNECTION = "base_connection"
                 If StructureFields <> "" Then
                     PrimKey = "MOL_ID"
                     PrimKeyType = "1"
                     TableAlias.PRIMARY_KEY = PrimKey
                     TableAlias.SELECT_JOIN = TableAlias.TABLE_NAME & "." & PrimKey
                     TableAlias.SELECT_LINKS = TableAlias.TABLE_NAME & "." & PrimKey & ";" & PrimKeyType & "," & TableAlias.TABLE_NAME & "." & PrimKey & ";" & PrimKeyType
                     TableAlias.STRUC_FIELD_ID = "MOL_ID"
                     BuildChemConnGroup theDBName, theTablePath, theTableName, "base"
                     TableAlias.CHEM_CONNECTION = "base_cfw_form"
                 Else
                      If TableNames.Count > 1 Then
                            Dim theBaseTableLink As Collection
                            Set theBaseTableLink = TablesCollection.Item(2)
                            Count = theBaseTableLink.Count
                            Dim thebaseField As Collection
                            Set thebaseField = theBaseTableLink.Item(Count)
                            theinfo = thebaseField.Item(4)
                            theinfotype = thebaseField.Item(5)
                            'temp = Split(theinfo, ",", -1)
                            PrimKey = Replace(theinfo, """", "")
                            PrimKeyType = Replace(theinfotype, """", "")
                       Else
                       myArray = Split(RelFields, ",", -1)
                       myArrayFinal = Split(myArray(0), ";", -1)
                       PrimKey = myArrayFinal(0)
                       PrimKeyType = myArrayFinal(1)
                       
                       End If
                     TableAlias.PRIMARY_KEY = PrimKey
                     TableAlias.SELECT_JOIN = TableAlias.TABLE_NAME & "." & PrimKey
                     TableAlias.SELECT_LINKS = TableAlias.TABLE_NAME & "." & PrimKey & ";" & PrimKeyType & "," & TableAlias.TABLE_NAME & "." & PrimKey & ";" & PrimKeyType
                     TableAlias.CHEM_CONNECTION = "NULL"
                     TableAlias.STRUC_FIELD_ID = "NULL"
                 End If
            End If
            If i > 0 Then  ' this means it is not the base table
                For k = 1 To Table.Count
                
                    Set theField = Table.Item(k)
                    If theField.Item(1) = "FBOX 6" Then
                        
                        ' DGB to support Add/Edit mode
                        ' The linking field cannot be a PK
                        ' TableAlias.PRIMARY_KEY = theField.Item("sub_link_info")
                        ' Use the first realtional field instead
                        myArray = Split(RelFields, ",", -1)
                        myArrayFinal = Split(myArray(0), ";", -1)
                        PrimKey = myArrayFinal(0)
                        PrimKeyType = myArrayFinal(1)
                        TableAlias.PRIMARY_KEY = PrimKey
                        
                        dbEngine = GetDBEngine(theField.Item("sub_db_path"))
                        If Not Trim(dbEngine) <> "" Then
                           dbEngine = "ACCESS"
                        End If
                        TableAlias.SQL_SYNTAX = dbEngine
                        TableAlias.SELECT_KEYWORD = "NULL"
                        TableAlias.SELECT_ADDITIONAL = "NULL"
                        'If InStr(theField.Item("sub_table_name"), "*") > 0 Then
                            'temp = Split(theField.Item("sub_table_name"), "*", -1)
                            'On Error Resume Next
                            'theField.Item("sub_table_name") = temp(0)
                            'On Error GoTo 0
                       ' End If
                        'If InStr(theField.Item("base_table"), "*") > 0 Then
                            'temp = Split(theField.Item("base_table"), "*", -1)
                            'On Error Resume Next
                          ' theField.Item("base_table") = temp(0)
                             'On Error GoTo 0
                        'End If
                        theJoin = theField.Item("sub_table_name") & "." & theField.Item("sub_link_info") & "=" & theField.Item("base_table") & "." & theField.Item("base_link")
                        TableAlias.SELECT_JOIN = theJoin
                        theSelectLinks = theField.Item("sub_table_name") & "." & theField.Item("sub_link_info") & ";" & theField.Item("base_link_data_type") & "," & theField.Item("base_table") & "." & theField.Item("base_link") & ";" & theField.Item("base_link_data_type")
                        TableAlias.SELECT_LINKS = theSelectLinks
                        If Not theField.Item("base_table") = TableNames.Item(1) Then
                            InterTable = TableNames.Item(1) & "," & theField.Item("base_table")
                        Else
                            InterTable = TableNames.Item(1)
                        End If
                        TableAlias.INTER_TABLES = InterTable
                        If Not theField.Item("sub_db_path") = theBaseConn Then
                            BuildADOConnGroup theTableName, theField.Item("sub_db_path"), "other", TableAlias.SQL_SYNTAX
                            TableAlias.ADO_CONNECTION = theTableName & "connection"
                        Else
                            TableAlias.ADO_CONNECTION = "base_connection"
                        End If
                        If StructureFields <> "" Then
                            TableAlias.STRUC_FIELD_ID = "MOL_ID"
                            BuildChemConnGroup theTableName, theField.Item("sub_db_path"), theTableName, "new"
                            TableAlias.CHEM_CONNECTION = theTableName & "_cfw_form"
                        Else
                            TableAlias.CHEM_CONNECTION = "NULL"
                            TableAlias.STRUC_FIELD_ID = "NULL"
                        End If
                    End If
                 Next k
            End If
    
           
    mcolTableAliases.Add TableAlias, theTableName
    Set TableAlias = Nothing
Next i


Set BuildTable_Aliases = mcolTableAliases

End Function
Function GetDBEngine(ByVal inputStr)
    dbInfo = Split(inputStr, Chr(92), -1)
    Count = UBound(dbInfo)
    dbExt = Split(dbInfo(Count), ".", -1)
    dbExt(1) = Replace(dbExt(1), """", "")
    Select Case dbExt(1)
        Case "MDB"
            theType = "ACCESS"
        Case "sql"
            theType = "SQLSERVER"
        Case "orc"
            theType = "ORACLE"
    End Select
    GetDBEngine = theType
End Function




Sub BuildADOConnGroup(tableName, baseConnInfo, theType, SQLSyntax)
    If theType = "base" Then
        theName = "base_connection"
        CreateADOConn theName, baseConnInfo, SQLSyntax
    Else
        theName = "empty"
        For i = 1 To BuildADOColl.Count
            If BuildADOColl.Item(i).DB_PATH = baseConnInfo Then
                theName = BuildADOColl.Item(i).GroupName
            End If
        Next
        If theName = "empty" Then
            theName = tableName & "_conn_group"
            CreateADOConn theName, baseConnInfo, SQLSyntax
        End If
    End If
End Sub
Sub CreateADOConn(theName, baseConnInfo, SQLSyntax)
    baseConnInfo = Replace(baseConnInfo, """", "")
    Dim ADOConnGroups As clsADOConnGroup
    Set ADOConnGroups = New clsADOConnGroup
    If InStr(baseConnInfo, "dsn") > 0 Then
        varConn_Type = "DSN"
    Else
        varConn_Type = "DBQ"
    End If

    ADOConnGroups.GroupName = theName
    ADOConnGroups.CONN_TYPE = varConn_Type
    If varConn_Type = "DBQ" Then
        Select Case SQLSyntax
            Case "ACCESS"
                DriverName = "Microsoft.Jet.OLEDB.4.0"
            Case "ORACLE"
                DriverName = "OraOLEDB.Oracle.1"
            Case "SQLSERVER"
                DriverName = "SQLOLEDB.1"
        End Select
    ADOConnGroups.DB_DRIVER = DriverName
    End If
    
    ADOConnGroups.DB_PATH = baseConnInfo
    ADOConnGroups.CONNECTION_TIMEOUT = "15"
    ADOConnGroups.COMMAND_TIMEOUT = "30"
    ADOConnGroups.CONNECTION_USERNAME = """"""
    ADOConnGroups.CONNECTION_PASSWORD = """"""
 
    BuildADOColl.Add ADOConnGroups, theName

    Set ADOConnGroups = Nothing

End Sub

Sub BuildChemConnGroup(formName, baseConnInfo, tableName, theType)
 baseConnInfo = Replace(baseConnInfo, """", "")
If theType = "base" Then
    theName = "base_cfw_form"
Else
    theName = tableName & "_cfw_form"
End If
Dim ChemConnGroups As clsChemConnGroup

Set ChemConnGroups = New clsChemConnGroup

 ChemConnGroups.GroupName = theName
 ChemConnGroups.STRUC_DB_PATH = Replace(UCase(baseConnInfo), ".MDB", ".MST")
 ChemConnGroups.STRUC_ENGINE = "MOLSERVER"
 ChemConnGroups.STRUC_FORM_NAME = formName
 ChemConnGroups.STRUC_TABLE_NAME = tableName
 
 BuildChemColl.Add ChemConnGroups, theName
Set ChemConnGroups = Nothing


End Sub

Sub GetLinksInfo(masterIndex)
Dim theTable As Collection
Dim theTableLink As Collection
Set theTableLink = New Collection
    theWorkingIndex = masterIndex - 1
    If theWorkingIndex = 0 Then Exit Sub 'skip base table
    theLinkStr = TableLinks.Item(theWorkingIndex)
    theLinkArray = Split(theLinkStr, gs_stndSep, -1)
    temp = Split(theLinkArray(0), """,", -1)
    theTableAbove = temp(0)

    theTableLink.Add temp(1), "sub_boxtype" '1
    theTableLink.Add theLinkArray(1), "sub_coords" '2
    If InStr(theTableAbove, "0dup0") > 0 Then
        temp = Split(theTableAbove, "0dup0", -1)
        theTableLink.Add temp(0), "base_table" '3
    Else
        theTableLink.Add theTableAbove, "base_table" '3
    End If
    theTableLink.Add theLinkArray(6), "base_link" '4
    theTableLink.Add theLinkArray(5), "base_link_data_type" '5
    sub_link = Replace(theLinkArray(9), Chr(34), "")
    theTableLink.Add Trim(sub_link), "sub_link_info" '6
    theTableLink.Add Trim(theLinkArray(4)), "sub_font_info" '7
    theTableLink.Add Trim(theLinkArray(7)), "sub_db_path" '8
    sub_table_name = Trim(theLinkArray(8))
    
    If InStr(sub_table_name, "0dup0") > 0 Then
        temp = Split(sub_table_name, "0dup0", -1)
        theTableLink.Add temp(0), "sub_table_name" '9
    Else
        theTableLink.Add sub_table_name, "sub_table_name" '9
    End If
    On Error Resume Next
    sub_fields = ""
    sub_fields = Replace(theLinkArray(10), Chr(34), "")
    On Error GoTo 0
    theTableLink.Add sub_fields, "sub_fields"

    Set theTable = TablesCollection.Item(sub_table_name) '10
    theTable.Add theTableLink
    Set theTable = Nothing
End Sub



Sub GetFieldsInfo(theIndex)
Dim theParseString As String
Dim theSingleTable As Collection
Dim theField As Collection
Dim theTable As Collection
Set theTable = New Collection
Dim theLinkStr As String
'get the tables colletion from the input index
Set theSingleTable = ParsedTablesCollection.Item(theIndex)
tableName = TableNames.Item(theIndex)
'loop through the individual table's items and get the relational fields name (array index 6) and field types (array index 5)from the string
'each item has the following format that is split at a double space: FBOX boxtype  l t r b  id  dtype  fntno  fldtype  fldname
For i = 1 To theSingleTable.Count
    Set theField = New Collection
    parseStr = theSingleTable.Item(i)
    
    If InStr(parseStr, "FBOX") = 1 Then
        If Len(parseStr) > 5 Then
            'split on the double spaces
            parseArray = Split(parseStr, gs_stndSep, -1)
                If Len(parseArray(6)) > 0 Then
                    theField.Add parseArray(0), "box_type"
                    theField.Add parseArray(1), "coords"
                    theField.Add parseArray(5), "field_type"
                    theField.Add parseArray(6), "field_name"
                    theField.Add parseArray(4), "font_number"
                    theTable.Add theField
                    
                 End If
        End If
    End If
Set theField = Nothing
Next
'theSingleTable.Add theTable

TablesCollection.Add theTable, tableName


Set theTable = Nothing
Set theSingleTable = Nothing
End Sub

Function Build_TableGroups(inputStr) As TableGroups
Dim mcolTableGroups As TableGroups

Set mcolTableGroups = New TableGroups

   
    theArray = Split(inputStr, ",", -1)
    For i = 0 To UBound(theArray)
     Dim TableGroups As New clsTableGroup
    Set TableGroups = New clsTableGroup
    theBaseTable = TableNames.Item(1)
    theMolTable = theBaseTable
        For j = 1 To TableNames.Count
            If SQL_ORDER <> "" Then
                SQL_ORDER = SQL_ORDER & "," & TableNames.Item(j)
            Else
                SQL_ORDER = TableNames.Item(j)
            End If
        Next
        TableGroups.GroupName = theArray(i)
        TableGroups.Base_Table = theBaseTable
        TableGroups.MOLECULE_TABLE = theMolTable
        TableGroups.Table_SQL_Order = SQL_ORDER


        mcolTableGroups.Add TableGroups, theArray(i)
        Set TableGroups = Nothing

Next i


Set Build_TableGroups = mcolTableGroups

End Function
Function Build_FieldMapGroups(inputStr) As FieldMapGroups
Dim mcolFieldMapGroups As FieldMapGroups

Set mcolFieldMapGroups = New FieldMapGroups
   
        theArray = Split(inputStr, ",", -1)
        For i = 0 To UBound(theArray)
            Dim MapGroup As clsFieldMapGroup
            Set MapGroup = New clsFieldMapGroup
            theBaseTable = TableNames.Item(1)
            theMolTable = theBaseTable
            strucField = theMolTable & "." & "Structure"
            formulaField = theMolTable & "." & "Formula"
            mwField = theMolTable & "." & "MolWeight"
            'MolTable.Structure; & "theMolTable
            MapGroup.GroupName = theArray(i)
            MapGroup.STRUC_FIELD_MAP = "MolTable.Structure," & strucField
            MapGroup.MW_FIELD_MAP = "MolTable.MolWeight," & mwField
            MapGroup.FORMULA_FIELD_MAP = "MolTable.Formula," & formulaField
            MapGroup.OTHER_FIELD_MAP1 = "NULL"
            MapGroup.OTHER_FIELD_MAP2 = "NULL"
            MapGroup.OTHER_FIELD_MAP3 = "NULL"
            MapGroup.OTHER_FIELD_MAP4 = "NULL"
            MapGroup.OTHER_FIELD_MAP5 = "NULL"
            MapGroup.OTHER_FIELD_MAP6 = "NULL"
            mcolFieldMapGroups.Add MapGroup, theArray(i)
            Set MapGroup = Nothing
        Next i


Set Build_FieldMapGroups = mcolFieldMapGroups
End Function

Function BuildForm_Groups(inputStr)
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
                    If masterTableIndex = 1 Then
                        
                    End If
            End Select
            
            RelFields = Replace(RelFields, """", "")
            MolWeightFields = Replace(MolWeightFields, """", "")
            StructureFields = Replace(StructureFields, """", "")
             FormulaFields = Replace(FormulaFields, """", "")
             SDFields = "NULL"
    Next
Next
theArray = Split(inputStr, ",", -1)
Set mcolFormGroups = New FormGroups

For i = 0 To UBound(theArray)
  If Not MolWeightFields <> "" Then MolWeightFields = "NULL"
  If Not FormulaFields <> "" Then FormulaFields = "NULL"
    Dim FormGroup As New clsFormGroup
        FormGroup.GroupName = Trim(theArray(i))
     If theArray(i) = "basenp_form_group" Then
        FormGroup.INPUT_FORM_PATH = theDBName & "_input_formnp.asp"
        FormGroup.INPUT_FORM_MODE = "search"
        FormGroup.RESULT_FORM_PATH = theDBName & "_result_list.asp"
        FormGroup.RESULT_FORM_MODE = "list"
        FormGroup.PLUGIN_VALUE = "False"
        FormGroup.STRUCTURE_FIELDS = "NULL"
        FormGroup.MW_FIELDS = MolWeightFields
        FormGroup.FORMULA_FIELDS = FormulaFields
    Else
        If theArray(i) = "gs_form_group" Then
             FormGroup.INPUT_FORM_PATH = "global_input_form.asp"
        ElseIf theArray(i) = "add_record_form_group" Then
            FormGroup.INPUT_FORM_PATH = theDBName & "_add_form.asp"
        Else
             FormGroup.INPUT_FORM_PATH = theDBName & "_input_form.asp"
        End If
       
        FormGroup.INPUT_FORM_MODE = "search"
        FormGroup.RESULT_FORM_PATH = theDBName & "_result_list.asp"
        FormGroup.RESULT_FORM_MODE = "list"
     
        If Not StructureFields <> "" Then
            FormGroup.STRUCTURE_FIELDS = "NULL"
            FormGroup.PLUGIN_VALUE = "False"
            FormGroup.MW_FIELDS = "NULL"
            FormGroup.FORMULA_FIELDS = "NULL"
        Else
            FormGroup.STRUCTURE_FIELDS = StructureFields
            FormGroup.PLUGIN_VALUE = "True"
            FormGroup.MW_FIELDS = MolWeightFields
            FormGroup.FORMULA_FIELDS = FormulaFields
        End If
        
    End If
        FormGroup.SEARCHABLE_ADO_FIELDS = RelFields
    FormGroup.REQUIRED_FIELDS = "NULL"
    FormGroup.SDFILE_FIELDS = SDFields
    Select Case theArray(i)
        Case "base_form_group", "basenp_form_group"
            FormGroup.FORM_GROUP_FLAG = "SINGLE_SEARCH"
        Case "gs_form_group"
            FormGroup.FORM_GROUP_FLAG = "GLOBAL_SEARCH"
            FormGroup.FIELD_MAP_GROUP = "gs_field_map_group"
        Case "drill_down_form_group"
            FormGroup.FORM_GROUP_FLAG = "INDEX_SEARCH"
            FormGroup.INDEX_DB = theDBName
         Case "add_record_form_group"
            FormGroup.FORM_GROUP_FLAG = "ADD_RECORD"
            FormGroup.REG_COMMIT_BASETABLE = TableNames.Item(1)
            FormGroup.REG_COMMIT_MOLTABLE = TableNames.Item(1)
            For j = 1 To TableNames.Count
                If theNames <> "" Then
                    theNames = theNames & "," & TableNames.Item(j)
                Else
                    theNames = TableNames.Item(j)
                End If
            Next
            FormGroup.TABLE_ORDER_FULL_COMMIT = theNames
            FormGroup.TABLE_ORDER_PARTIAL_COMMIT = theNames
    End Select
    FormGroup.TABLE_GROUP = "base_table_group"
    'FormGroup.NUM_LIST_VIEW = RequestValues.Item("NUM_LIST_VIEW")
 mcolFormGroups.Add FormGroup, UCase(theArray(i))
 Set FormGroup = Nothing
Next
Set BuildForm_Groups = mcolFormGroups
Set mcolFormGroups = Nothing
End Function
Function GetDBName(theCFWFormPath)
        tempArray = Split(theCFWFormPath, Chr(92), -1)
        For i = 0 To UBound(tempArray) - 1
            If varPath <> "" Then
                varPath = varPath & Chr(92) & tempArray(i)
            Else
                varPath = tempArray(i)
            End If
        Next
        gCFWFormPath = varPath
        FileName = tempArray(UBound(tempArray))
        tempDBName = Split(FileName, ".", -1)
        dbName = tempDBName(0)
        GetDBName = dbName
    
        
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
    temp = Split(s, gs_stndSep, -1)
    tableName = Replace(temp(2), Chr(34), "")
    tableName = Trim(tableName)
    theNames.Add tableName
    End If
    
    
Loop
ts.Close

Set GetTableNames = theNames
Set theNames = Nothing
End Function

Function GetTableDups(FilePath)
Set theNames = New Collection
'find number of subforms
Set ts = OpenCFWOutput(FilePath)
NumSubForms = 0
Do While ts.AtEndOfStream <> True
    s = ts.ReadLine
    If InStr(s, "DSRC") = 1 Then
    temp = Split(s, gs_stndSep, -1)
    On Error Resume Next
    temp(2) = Replace(temp(2), Chr(34), "")
    theNames.Add Trim(temp(2)), Trim(temp(2))
    If Err.Number = 457 Then
        AllTableNames.Add Trim(temp(2))
    End If
    End If
    
    
Loop
ts.Close
Set GetTableDups = AllTableNames
Set theNames = Nothing

End Function
Function GetTableDBPaths(FilePath)
Set theNames = New Collection
'find number of subforms
Set ts = OpenCFWOutput(FilePath)
NumSubForms = 0
Do While ts.AtEndOfStream <> True
    s = ts.ReadLine
    If InStr(s, "DSRC") = 1 Then
    temp = Split(s, gs_stndSep, -1)
    'test to see what happens without temp(2) here
    tableName = Replace(temp(2), Chr(34), "")
    theNames.Add Trim(temp(1)), Trim(tableName)
    'theNames.Add temp(1)
    End If
    
    
Loop
ts.Close
Set GetTableDBPaths = theNames
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
            temp = Split(s, gs_stndSep, -1)
            tableName = Replace(temp(2), Chr(34), "")
        End If
        If DSRCcounter = i Then
            If InStr(s, "FBOX 6") = 1 Then
                TableLinks.Add tableName & s
            End If
        End If
    Loop
    ts.Close
Next
End Sub

Sub ConvertDupTableNames(FilePath)
Set AllTableNames = GetTableDups(FilePath)
Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & "cfwtemp" & "\" & "cfwoutput"
theFile.CopyFile thePath & ".txt", thePath & "temp" & ".txt"
Set ts_final = theFile.CreateTextFile(thePath & ".txt", True)
Set ts_temp = theFile.OpenTextFile(thePath & "temp" & ".txt")
DSCRCount = 0
FBOX6Count = 0
Do While ts_temp.AtEndOfStream <> True
s = ts_temp.ReadLine
If InStr(s, "SUBFORM") = 1 Then
    DSCRCount = DSCRCount + 1
End If
    If InStr(s, "FBOX 6") = 1 Then
        newline = ""
        tableName = ""
        temp = Split(s, gs_stndSep, -1)
        TableId = Trim(temp(2))
        isDup = False
        For i = 1 To AllTableNames.Count
        If Trim(temp(8)) = AllTableNames.Item(i) Then
            isDup = True
        End If
        Next
        If isDup = True Then
            temp(8) = Trim(temp(8)) & "0dup0" & TableId
            tableName = temp(8)
            For i = 0 To UBound(temp)
                If newline <> "" Then
                    newline = newline & gs_stndSep & Trim(temp(i))
                Else
                    newline = Trim(temp(i))
                End If
            Next
            ts_final.WriteLine newline
            
        Else
            ts_final.WriteLine s
        End If
     
    Else
        If InStr(s, "DSRC") = 1 And DSCRCount > 0 And isDup = True Then
            temp = Split(s, gs_stndSep, -1)
            table_count = UBound(temp)
            oldtableName = Trim(temp(table_count))
            
            newline = ""
            For i = 0 To UBound(temp) - 1
                If newline <> "" Then
                    newline = newline & gs_stndSep & Trim(temp(i))
                Else
                    newline = Trim(temp(i))
                End If
            Next
                newline = newline & gs_stndSep & tableName & Chr(34)
                ts_final.WriteLine newline
        
        
    Else
    ts_final.WriteLine s
    End If
    End If
Loop
ts_final.Close
ts_temp.Close
Set theFile = Nothing
End Sub

Sub CorrectCFWOutput(FilePath)
'fix cfwoutput file change doublespaces to standard quoted separator
' this allows easier parsing of the file.


Set theFile = CreateObject("Scripting.FileSystemObject")
Set theFolder = theFile.GetFolder(AppGlobals.COWS_PATH)
thePath = theFolder.Path & "\" & "cfwtemp" & "\" & "cfwoutput"
'convert the file to proper format
Dim inPath As Variant
Dim outPath As Variant
inPath = thePath & ".txt"
outPath = thePath & "converted.txt"
On Error Resume Next
success = AscConvert(inPath, outPath)
If Err.Number <> 0 Then
  App.LogEvent Err.Description & " in " & Err.Number, vbLogEventTypeError
End If

theFile.CopyFile thePath & "converted.txt", thePath & "temp" & ".txt"
Set ts_final = theFile.CreateTextFile(thePath & ".txt", True)
Set ts_temp = theFile.OpenTextFile(thePath & "temp" & ".txt")
DSCRCount = 0
FBOX6Count = 0
Do While ts_temp.AtEndOfStream <> True
s = ts_temp.ReadLine
If InStr(s, "DSRC") = 1 Then
    tempQuote = Split(s, Chr(34), -1)
    newline = tempQuote(0) & gs_stndSep & tempQuote(1) & gs_stndSep & tempQuote(2) & Chr(34)
     ts_final.WriteLine newline
Else
If InStr(s, "FBOX") = 1 Then
        newline = ""
        tableName = ""
        tempQuote = Split(s, gs_stndSep, -1)
        tempQuote(0) = Replace(tempQuote(0), "  ", gs_stndSep)
        For i = 0 To UBound(tempQuote)
            If newline <> "" Then
                newline = newline & gs_stndSep & tempQuote(i)
            Else
                newline = tempQuote(i)
            End If
           newline = Replace(newline, Chr(34) & Chr(34), Chr(34))

        Next
            ts_final.WriteLine newline
Else
    ts_final.WriteLine s
End If
End If
Loop
ts_final.Close
ts_temp.Close
Set theFile = Nothing
End Sub

'c:\Inetpub\samples\cs_demo.cfw
Sub ParseCFWOutput(ByVal FilePath)
Dim Table As Collection
Dim subformcount As Integer
Dim NumTables As Integer
Dim tableName As String
Dim StoreFBOX As String
Set TablesCollection = New Collection
Set CommentedOutListFields = New Collection
Set CommentedOutRecordSet = New Collection
Set TableLinks = New Collection
Set TableLinkNames = New Collection
Set TableNames = New Collection

'change table name references to include id value, thus avoiding duplicates problems
CorrectCFWOutput (FilePath)
ConvertDupTableNames (FilePath)

Set TableNames = GetTableNames(FilePath)
Set TableDBPaths = GetTableDBPaths(FilePath)
NumTables = TableNames.Count
'get base table

    

Set ts = OpenCFWOutput(FilePath)
Set Table = New Collection
Set FormFonts = New Collection

subformcount = 0

Do While ts.AtEndOfStream <> True
    s = ts.ReadLine
    
    If InStr(s, "SUBFORM") = 1 Then subformcount = subformcount + 1
    If InStr(s, "END SUBFORM") = 1 Then subformcount = subformcount - 1
    If InStr(s, "FONT") = 1 And subformcount = 0 Then
        FormFonts.Add s
    End If
    If InStr(s, "DSRC") = 1 And subformcount = 0 Then
        temp = Split(s, gs_stndSep, -1)
        theCount = UBound(temp)
        
        tableName = Trim(temp(theCount))
        
        Table.Add s
        
    End If
    If InStr(s, "FBOX") = 1 And Not (InStr(s, "FBOX 6") = 1) And subformcount = 0 Then Table.Add s
    If InStr(s, "FBOX 6") = 1 And subformcount = 0 Then
        TableLinkNames.Add tableName
    End If
Loop
ts.Close

ParsedTablesCollection.Add Table, tableName

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
            temp = Split(s, gs_stndSep, -1)
            theCount = UBound(temp)
            tableName = Trim(temp(theCount))
            Table.Add s
            If StoreFBOX <> "" Then
         TableLinkNames.Add StoreFBOX
         End If
         StoreFBOX = ""
         End If
      
       
        If InStr(s, "FBOX") = 1 And subform_counter = current_subform And foreign_subform = 0 And start_current_read = True Then Table.Add s
               End If
    Loop
  
    ParsedTablesCollection.Add Table, tableName
    
    ts.Close
    Set ts = Nothing
    Set Table = Nothing
Next current_subform
Set ts = OpenCFWOutput(FilePath)
Dim Counter As Integer
Counter = 1
Do While ts.AtEndOfStream <> True
        s = ts.ReadLine
        If InStr(s, "FBOX 6") = 1 Then
        theName = TableLinkNames.Item(Counter)
        'temp = Split(s, gs_stndSep, -1)
        'tableName = Trim(temp(8))
        If Not Right(theName, 1) = Chr(34) Then
         theName = theName & Chr(34)
        End If
        TableLinks.Add theName & "," & s
        
       
        Counter = Counter + 1
        End If
        
Loop
ts.Close

Dim theItem As Collection
Dim FTableArray() As String

Set OutputItems = New Collection
Set ts = OpenCFWOutput(FilePath)
    baseTable = 1
        current_table = 1
        subform_counter = 0
        foreign_subform = 0
    Do While ts.AtEndOfStream <> True
        s = ts.ReadLine
        
        If InStr(s, "SUBFORM") = 1 Then
            subform_counter = subform_counter + 1
        End If
        If InStr(s, "FBOX 6") = 1 Then
            foreign_subform = foreign_subform + 1
            If subform_counter = 0 Then
                ReDim FTableArray(0)
                FTableArray(0) = TableNames.Item(1)
            Else
                theItems = UBound(FTableArray)
                ReDim Preserve FTableArray(theItems + 1)
                FTableArray(theItems + 1) = tableName
            End If
            
        End If
        If InStr(s, "END SUBFORM") = 1 Then
            foreign_subform = foreign_subform - 1
            
        End If
        If subform_counter > 0 Then
            If foreign_subform > 0 Then
                    tableName = TableNames.Item(subform_counter + 1)
            Else
                currentCount = UBound(FTableArray)
                tableName = FTableArray(currentCount)
            End If
        Else
            tableName = TableNames.Item(1)
        End If
        If InStr(s, "FBOX") = 1 Then
        
            Set theItem = New Collection
          
            
           
            parseArray = Split(s, gs_stndSep, -1)
            If InStr(s, "FBOX 6") = 1 Then
                'varFBOX6 = parseArray(10)
                theItem.Add parseArray(8), "table_name"
            Else
                theItem.Add tableName, "table_name" '1
            End If
            On Error Resume Next
            If Len(parseArray(6)) > 0 Then
                theItem.Add parseArray(0), "box_type" '2
                indCoors = Split(parseArray(1), " ", -1)
                theItem.Add CInt(indCoors(0)), "left" '3
                theItem.Add CInt(indCoors(1)), "top" '4
                theItem.Add CInt(indCoors(2)), "right" '5
                theItem.Add CInt(indCoors(3)), "bottom" '6
                If Not InStr(s, "FBOX 4") = 1 Then
                    theItem.Add Round(((indCoors(2) - indCoors(0)) / 6), 0), "width" '7
                    theItem.Add Round(((indCoors(3) - indCoors(1)) / 6), 0), "height" '8
                Else
                    theItem.Add indCoors(2) - indCoors(0), "width" '7
                    theItem.Add indCoors(3) - indCoors(1), "height" '8
                End If
                    theItem.Add CInt(parseArray(4)), "font_number" '9
                    theItem.Add parseArray(5), "field_type" '10
                On Error GoTo 0
                If InStr(s, "FBOX 6") = 1 Then
                    field_names = ""
                    On Error Resume Next
                        field_names = parseArray(10)
                    If Err.Number > 0 Then
                        field_names = ""
                    End If
                    
                    theItem.Add field_names, "field_names" '11
                   
                    theItem.Add parseArray(8), "sub_table_name" '11
                Else
                    theItem.Add parseArray(6), "field_names" '11
                End If
                theItem.Add parseArray(2), "uniqueID" '12
                OutputItems.Add theItem
             End If
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


Function GetCFWASCII(formpath)
    Dim CFWApp As Object
    Dim CFWDoc As Object
    On Error Resume Next
    'Set CFWApp = GetCFWObject()
    Call GetCFWObject(CFWApp)
    'CFWApp.Visible = True
    CFWApp.ExecuteCALCommand "INTERACTIVE OFF"
    CFWApp.ExecuteCALCommand "SET OPTIONS NON_INTERACTIVE 1"
    Set CFWDoc = CFWApp.Documents.Open(formpath)
    
    outputpath = AppGlobals.COWS_PATH & "\cfwtemp\cfwoutput.txt"
    CFWDoc.SaveAs outputpath
    CFWDoc.Close
    Set CFWDoc = Nothing
    CFWApp.Quit
    Set CFWApp = Nothing
    GetCFWASCII = outputpath
    
End Function

Function GetDBRecordCount(formpath)
    Dim CFWApp As Object
    Dim CFWDoc As Object
    'Set CFWApp = GetCFWObject()
    Call GetCFWObject(CFWApp)
     On Error Resume Next
    CFWApp.ExecuteCALCommand "INTERACTIVE OFF"
    CFWApp.ExecuteCALCommand "SET OPTIONS NON_INTERACTIVE 1"
   
    Set CFWDoc = CFWApp.Documents.Open(formpath)
  
    CFWDoc.ExecuteCALCommand ("Search Retrieve All")
     GetDBRecordCount = CFWDoc.Recordset.RecordCount
    CFWDoc.Close
    Set CFWDoc = Nothing
    CFWApp.Quit
    Set CFWApp = Nothing
End Function


Sub GetCFWForm(theSourcePath, theFormName, theDBPath, theTableName, theDBDirPath)
  Dim CFWApp As Object
    Dim CFWDoc As Object
    'Set CFWApp = GetCFWObject()
    Call GetCFWObject(CFWApp)
    On Error Resume Next
    CFWApp.ExecuteCALCommand "INTERACTIVE OFF"
    CFWApp.ExecuteCALCommand "SET OPTIONS NON_INTERACTIVE 1"
    Set CFWDoc = CFWApp.Documents.Open(theSourcePath)
    savepath = theDBDirPath & theFormName
   
    CFWDoc.ExecuteCALCommand "OPENDB " & theDBPath & "<" & theTableName & ">"
    theLongSaveName = savepath & "long" & ".cfw"
    theShortSaveName = savepath & "short" & ".cfw"
    CFWDoc.SaveAs theLongSaveName
        
    CFWDoc.SaveAs theShortSaveName
    CFWDoc.Close
    Set CFWDoc = Nothing
    CFWApp.Quit
    Set CFWApp = Nothing
End Sub


Sub GetCFWObject(ByRef o As Object)
    Dim strMolServerVersion As String
    
    If m_strMolServerVersion = "" Then
        m_strMolServerVersion = GetMolServerVersionFromCowsIni()
    End If
    Set o = CreateObject("ChemFinder" & m_strMolServerVersion & ".Application")
End Sub

Function GetMolServerVersionFromCowsIni() As String
    
    Dim INIVar As Object
    Dim strMolServerVersion As String
    Set INIVar = CreateObject("cowsUtils.cowsini")
    theDrive = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DRIVE", "cows.ini")
    theServerRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DIR", "cows.ini")
    theDocumentRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "DOC_ROOT", "cows.ini")
    theCowsRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "COWS_ROOT", "cows.ini")
    theCOWSPath = theDrive & "\" & theServerRoot & "\" & theDocumentRoot & "\" & theCowsRoot
    strMolServerVersion = INIVar.VBGetPrivateProfileString("GLOBALS", "MOLSERVER_VERSION", theCOWSPath & "\config\chemoffice.ini")
    
    GetMolServerVersionFromCowsIni = strMolServerVersion
    
End Function


Sub WriteToLog(theText)
Set fs = CreateObject("Scripting.FileSystemObject")
Set f = fs.OpenTextFile("C:\Inetpub\WebServerAdminLog.txt", 8, 0)
f.WriteLine theText
f.Close
Set f = Nothing
Set fs = Nothing
End Sub

Public Sub SyncShell(exePath As String)
    Dim taskId As Long
    taskId = Shell(exePath, vbHide)
    WaitForTerm taskId
End Sub

Private Sub WaitForTerm(pid&)
    Dim phnd&
    phnd = OpenProcess(SYNCHRONIZE, 0&, pid)
    If phnd <> 0 Then
        Call WaitForSingleObject(phnd, INFINITE)
        Call CloseHandle(phnd)
    End If
End Sub

