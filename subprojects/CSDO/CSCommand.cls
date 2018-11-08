VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CSCommand"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' CSCommand

Public Enum CSCommandType
    kCSCTSelect = 0
    kCSCTInsert = 1
    kCSCTDelete = 2
    kCSCTUpdate = 3
    kCSCTUnknown = 4
End Enum

' command info
Private m_Type As CSCommandType
Private m_Name As String

' chemical search information
Private m_ChemLinks As CSChemLinks
Private m_ChemTablesInFromStr() As String             ' stores which links are used in this command
Private m_NumChemTablesInFromStr As Integer

' relational search information
Private m_hasRelQueryFields As Boolean
Private m_hasRelReturnFields As Boolean
Private m_hasChemQueryFields As Boolean
Private m_hasChemReturnFields As Boolean

Private m_ChemJoinSql As String             ' rel sql necessary to join two chemical searches
Private m_RelSql As String                  ' plain ol' rel sql for doing chem search first
Private m_RelSqlRelFirst As String          ' rel sql for doing rel sql first
Private m_RelSqlSelect As String            ' rel sql to select instead of ordered command

' originally entered sql
Private m_EnteredSQL As String

Public Property Get CommandType() As CSCommandType
    CommandType = m_Type
End Property

Friend Property Let CommandType(aCommandType As CSCommandType)
    m_Type = aCommandType
    ' Set name
    Select Case m_Type
        Case kCSCTSelect
            m_Name = "SELECT"
        Case kCSCTInsert
            m_Name = "INSERT"
        Case kCSCTDelete
            m_Name = "DELETE"
        Case kCSCTUpdate
            m_Name = "UPDATE"
        Case Else
            m_Type = kCSCTUnknown
    End Select
End Property

Friend Property Set ChemLinks(aChemLinks As CSChemLinks)
    Set m_ChemLinks = aChemLinks
End Property

Friend Property Get ChemLinks() As CSChemLinks
    Set ChemLinks = m_ChemLinks
End Property

Public Property Get Name() As String
    Name = m_Name
End Property

Friend Property Let CommandName(aName As String)
    m_Name = aName
    ' Set type
    Select Case m_Name
        Case "SELECT"
            Me.CommandType = kCSCTSelect
        Case "INSERT"
            Me.CommandType = kCSCTInsert
        Case "DELETE"
            Me.CommandType = kCSCTDelete
        Case "UPDATE"
            Me.CommandType = kCSCTUpdate
        Case Else
            Me.CommandType = kCSCTUnknown
    End Select
End Property

Public Property Get RelSql() As String
    RelSql = m_RelSql
End Property

Public Property Get RelSelectSql() As String
    RelSelectSql = m_RelSqlSelect
End Property

Public Property Get HasRelQueryFields()
    HasRelQueryFields = m_hasRelQueryFields
End Property

Public Property Get HasRelReturnFields()
    HasRelReturnFields = m_hasRelReturnFields
End Property

Public Property Get HasChemQueryFields()
    HasChemQueryFields = m_hasChemQueryFields
End Property

Public Property Get HasChemReturnFields()
    HasChemReturnFields = m_hasChemReturnFields
End Property

Public Function CreateFromSQL(aSQL As String) As CSCommandType
    ' creates the function from SQL, parses, and returns command type
    Dim tempArr() As String
    Dim cl As CSChemLink
        
    ' zero out member vars
    m_NumChemTablesInFromStr = 0
    m_ChemJoinSql = ""
    m_RelSql = ""
    m_RelSqlRelFirst = ""
    m_RelSqlSelect = ""
    m_hasChemQueryFields = False
    m_hasChemReturnFields = False
    m_hasRelQueryFields = False
    m_hasRelReturnFields = False
    
    ' clear out chemlinks
    For Each cl In m_ChemLinks
        cl.ReturnFields.Clear
        cl.QueryFields.Clear
    Next
    
    ' clean up sql and standardize so we can parse it
    m_EnteredSQL = CleanSQL(aSQL)
    
    ' DEBUG OUTPUT
    Debug.Print " ------------------------------- "
    Debug.Print "* OUTPUT FROM CSCommand.CreateFromSQL *"
    Debug.Print "Cleaned SQL = " & m_EnteredSQL
    
    ' split on first space - tempArr(0) should be a command (SELECT, etc.)
    ' and tempArr(1) will be rest of string..
    tempArr = Split(m_EnteredSQL, " ", 2)
    
    ' Retrieve first command word and set command type
    Me.CommandName = tempArr(0)
    
    ' Parse rest of SQL based on command type
    Select Case m_Type
        Case kCSCTSelect
            Parse_Select Trim(tempArr(1))
        Case kCSCTInsert
            Parse_Insert Trim(tempArr(1))
        Case kCSCTDelete
            Parse_Delete Trim(tempArr(1))
        Case kCSCTUpdate
            Parse_Update Trim(tempArr(1))
        Case Else
            ' Do nothing
    End Select
    
    ' DEBUG OUTPUT
    Debug.Print "m_RelSql = " & m_RelSql
    Debug.Print "m_hasChemQueryFields = " & m_hasChemQueryFields
    Debug.Print "m_hasChemReturnFields = " & m_hasChemReturnFields
    Debug.Print "m_hasRelQueryFields = " & m_hasRelQueryFields
    Debug.Print "m_hasRelReturnFields = " & m_hasRelReturnFields
    
    ' DEBUG OUTPUT
    For Each cl In m_ChemLinks
        Debug.Print "CHEMLINK:" & cl.RelTable & "." & cl.RelField
        Debug.Print "Num ChemQuery Fields: " & cl.QueryFields.count
        Debug.Print "Num ChemReturn Fields: " & cl.ReturnFields.count
    Next
    
    CreateFromSQL = m_Type
End Function

Private Sub Parse_Select(sql As String)
    ' rel strings for SELECT statements
    Dim RelSelectStr As String
    Dim RelFromStr As String
    Dim RelWhereStr As String
    Dim hasWhereStr As Boolean
                                              
    ' The functions called in this section do many things to
    ' the member variables of this class - i.e., lots of side
    ' effects, mostly having to do with setting the number and
    ' type of CSFields in the CSChemLinks collection
    
    If InStr(sql, " WHERE ") <> 0 Then
        hasWhereStr = True
    Else
        hasWhereStr = False
    End If
    
    ' get relational FROM string
    RelFromStr = GetFromStr(sql)
    
    ' get the rel SELECT String
    RelSelectStr = GetSelectStr(sql)
    
    ' if there is a WHERE clause, get the rel WHERE String
    If hasWhereStr Then
        RelWhereStr = GetWhereStr(sql)
    End If
    
    ' build relsql string
    ' add SELECT clause
    m_RelSql = "SELECT " & RelSelectStr
    
    ' add the RelTable.RelField to the select clause
    m_RelSql = AddChemLinkRelFieldsToSelect(m_RelSql)
    
    ' add FROM clause
    m_RelSql = m_RelSql & " FROM " & RelFromStr
        
    ' add WHERE clause, if necessary
    If hasWhereStr Then
        m_RelSql = m_RelSql & " WHERE " & RelWhereStr
    End If
    
    m_RelSqlSelect = m_RelSql
End Sub

Private Sub Parse_Insert(sql As String)
    Dim fieldArr() As String
    Dim valueArr() As String
    Dim tableStr As String
    Dim lnkFld As String
    Dim mLink As CSChemLink
    Dim myField As CSField
    Dim myFunc As CSFunction
    Dim i As Integer
    Dim j As Integer
    Dim k As Integer
    Dim numChemLinks As Integer
    Dim restOfSql As String
    
    restOfSql = sql
    
    ' get table
    i = InStr(sql, " INTO ") + Len(" INTO ")
    j = InStr(sql, "(")
    tableStr = Trim(Mid(sql, i, j - i))
    
    ' set up commandlink
    numChemLinks = 0
    For Each mLink In m_ChemLinks
        If mLink.RelTable = tableStr Then
            lnkFld = mLink.RelField
            ReDim Preserve m_ChemTablesInFromStr(numChemLinks)
            m_ChemTablesInFromStr(numChemLinks) = mLink.RelTable
            numChemLinks = numChemLinks + 1
        End If
    Next
    m_NumChemTablesInFromStr = numChemLinks
    
    ' if no chemical tables, exit
    If numChemLinks = 0 Then
        m_hasRelReturnFields = True
        m_hasChemReturnFields = False
        m_RelSql = m_Name & " " & restOfSql
        ' ** FUNCTION EXIT POINT **
        Exit Sub
    End If
    
    ' get list of values
    k = InStr(sql, ") VALUES")
    valueArr = GetArrayPreserve(StripParens(Right(sql, Len(sql) - (k + 8))), ",", "(", ")")
    ' look for STRUCT function
    For i = 0 To UBound(valueArr)
        Set myFunc = New CSFunction
        myFunc.CreateFromSQL valueArr(i)
        If myFunc.FunctionType = kCSFTStruct Then
            m_hasChemReturnFields = True
            ' add the field to the chemlink queryfields collection
            m_ChemLinks.Item(m_ChemTablesInFromStr(0)).ReturnFields.AddFromExisting myFunc.ReturnField
            ' trim the chemical part out of the relational sql
            ' trim out of values
            restOfSql = Replace(restOfSql, valueArr(i), "")
            ' trim out of fields (hack)
            restOfSql = Replace(restOfSql, "(STRUCTURE", "(")
            restOfSql = Replace(restOfSql, ",STRUCTURE", "")
        End If
    Next
    
    ' if only one thing in the insert statement, then no relational data to be inserted
    If i = 0 And m_hasChemReturnFields Then
        m_hasRelReturnFields = False
    End If
    
    ' build relsql string
    
    ' if no relational fields to be inserted, must insert MOL_ID field
    If Not m_hasRelReturnFields Then
        restOfSql = Replace(restOfSql, tableStr & " (", tableStr & " (" & lnkFld & ", ")
        restOfSql = Replace(restOfSql, "VALUES (", "VALUES (" & "<CHEMLINK_" & m_ChemTablesInFromStr(0) & ">, ")
    End If
    
    m_RelSql = m_Name & " " & restOfSql
    m_RelSql = RemoveExtraCommas(m_RelSql)
End Sub

Private Sub Parse_Delete(sql As String)
    Dim RelFromStr As String
    Dim RelWhereStr As String
    Dim linkfield As String
    Dim hasWhereStr As Boolean
        
    If InStr(sql, " WHERE ") <> 0 Then
        hasWhereStr = True
    Else
        hasWhereStr = False
    End If
    
    ' add space so " FROM " will be found
    RelFromStr = GetFromStr(" " & sql)
    
    If hasWhereStr Then
        RelWhereStr = GetWhereStr(sql)
    End If

    ' build relsql string
    m_RelSql = m_Name
    
    ' add FROM clause
    m_RelSql = m_RelSql & " FROM " & RelFromStr
    
    ' add WHERE clause, if necessary
    If hasWhereStr Then
        m_RelSql = m_RelSql & " WHERE " & RelWhereStr
    End If
        
    linkfield = m_ChemLinks.Item(m_ChemTablesInFromStr(0)).RelField
    m_RelSqlSelect = Replace(m_RelSql, "DELETE ", "SELECT " & m_ChemTablesInFromStr(0) & "." & linkfield)
End Sub

Private Sub Parse_Update(sql As String)
    Dim tableStr As String
    Dim numChemLinks As Integer
    Dim mLink As CSChemLink
    Dim myField As CSField
    Dim myFunc As CSFunction
    Dim RelWhereStr As String
    Dim hasWhereStr As Boolean
    Dim RelSetArr() As String
    Dim SetArr() As String
    Dim tempArr() As String
    Dim i&
    Dim j&
    Dim k&
    
    If InStr(sql, " WHERE ") <> 0 Then
        hasWhereStr = True
    Else
        hasWhereStr = False
    End If
    
    numChemLinks = 0
    
    ' get table
    i = InStr(sql, " SET ") - 1
    tableStr = Trim(Left(sql, i))
        
    ' set up commandlink
    numChemLinks = 0
    For Each mLink In m_ChemLinks
        If mLink.RelTable = tableStr Then
            ReDim Preserve m_ChemTablesInFromStr(numChemLinks)
            m_ChemTablesInFromStr(numChemLinks) = mLink.RelTable
            numChemLinks = numChemLinks + 1
        End If
    Next
    m_NumChemTablesInFromStr = numChemLinks
        
    ' if no chemical tables, exit
    If numChemLinks = 0 Then
        m_RelSql = m_Name & " " & sql
        ' ** FUNCTION EXIT POINT **
        Exit Sub
    End If
    
    ' get set array
    j = InStr(sql, " WHERE ")
    SetArr = GetArrayPreserve(Trim(Mid(sql, i + 5, j - (i + 5))), ",", "(", ")")
    k = 0
    For i = 0 To UBound(SetArr)
        tempArr = Split(SetArr(i), "=", 2)
        Set myFunc = New CSFunction
        myFunc.CreateFromSQL Trim(tempArr(1))
        If myFunc.FunctionType = kCSFTStruct Then
            m_hasChemReturnFields = True
            m_ChemLinks.Item(m_ChemTablesInFromStr(0)).ReturnFields.AddFromExisting _
            myFunc.ReturnField
        Else
            m_hasRelReturnFields = True
            ReDim Preserve RelSetArr(k)
            RelSetArr(k) = SetArr(i)
            k = k + 1
        End If
    Next
    
    ' get WHERE array
    RelWhereStr = GetWhereStr(sql)
    
    ' set up rel sql
    m_RelSql = m_Name & " " & tableStr & " SET " & _
               Join(RelSetArr, ",")
        
    ' add where string
    If hasWhereStr Then
        m_RelSql = m_RelSql & " WHERE " & RelWhereStr
    End If
    
    ' set up rel sql select
    m_RelSqlSelect = "SELECT " & m_ChemLinks.Item(m_ChemTablesInFromStr(0)).RelField & _
                     " FROM " & tableStr
    
    ' add where string
    If hasWhereStr Then
        m_RelSqlSelect = m_RelSqlSelect & " WHERE " & RelWhereStr
    End If
             
End Sub

Private Sub AddToChemLinks(origString As String, aField As CSField, IsQuery As Boolean)
    Dim myCSChemLink As CSChemLink
    Dim retStr As String
    
    For Each myCSChemLink In m_ChemLinks
        If m_NumChemTablesInFromStr = 1 Then
            ' if only one chemlink, then we can ignore table names (if present)
            If myCSChemLink.RelTable = m_ChemTablesInFromStr(0) Then
                If IsQuery Then
                    myCSChemLink.QueryFields.AddFromExisting aField
                    m_hasChemQueryFields = True
                Else
                    myCSChemLink.ReturnFields.AddFromExisting aField
                    m_hasChemReturnFields = True
                End If
            End If
        Else
            If InStr(UCase(origString), myCSChemLink.RelTable) Then
                If IsQuery Then
                    myCSChemLink.QueryFields.AddFromExisting aField
                    m_hasChemQueryFields = True
                Else
                    myCSChemLink.ReturnFields.AddFromExisting aField
                    m_hasChemReturnFields = True
                End If
            End If
        End If
    Next
    
End Sub

Private Function GetFromStr(sql As String) As String
    Dim tempArr() As String
    Dim tempArr2() As String
    Dim tableStr As String
    Dim arrSize As Integer
    Dim count As Integer
    Dim FROMArray() As String
    Dim myCSChemLink As CSChemLink
    Dim numChemLinks As Integer
    
    numChemLinks = 0

    ' get the FROM ... WHERE portion of the statement
    ' and check to see what we've got for tables
    tempArr = GetArrayPreserve(sql, " FROM ", "(", ")")
    tempArr2 = GetArrayPreserve(tempArr(1), " WHERE ", "(", ")")
    ' get FROM portion of statement
    If InStr(Trim(tempArr2(0)), ",") > 0 Then
        FROMArray = Split(Trim(tempArr2(0)), ", ")  ' split on comma/space combo for oracle using FROM table1,table2
    Else
        FROMArray = Split(Trim(tempArr2(0)), " ")  ' split on space for access /sql server using JOIN table1 ON table2
    End If
    arrSize = UBound(FROMArray)
    ' check to see what we've got for tables
    For count = 0 To arrSize
        Select Case FROMArray(count)
            Case "INNER", "OUTER", "RIGHT", "LEFT", "JOIN", "ON" ' add ON as keyword to ignore
                ' Ignore keywords
            Case Else
                ' fill the ChemTablesInFromStr array
                For Each myCSChemLink In m_ChemLinks
                    If myCSChemLink.RelTable = FROMArray(count) Then
                        ReDim Preserve m_ChemTablesInFromStr(numChemLinks)
                        m_ChemTablesInFromStr(numChemLinks) = myCSChemLink.RelTable
                        numChemLinks = numChemLinks + 1
                    End If
                Next
        End Select
    Next
    
    m_NumChemTablesInFromStr = numChemLinks
    GetFromStr = tempArr2(0)
End Function

Private Function GetWhereStr(sql As String)
    ' get WHERE portion of statement
    Dim prefix$
    Dim numChemLinks As Integer
    Dim tempArr() As String
    Dim tempArr2() As String
    Dim WHEREArray() As String
    Dim RelWhereArr() As String
    Dim chemAlreadyAdded() As String
    Dim myCSFunction As CSFunction
    Dim myCSField As CSField
    Dim arrSize As Integer
    Dim relCount As Integer
    Dim count As Integer
    Dim secCommand As CSCommand
    Dim tempStr As String
    Dim chemStr As String
    Dim relStr As String
        
    tempArr = Split(sql, " WHERE ", 2)
    WHEREArray = SplitOnMultipleAndAppend(tempArr(1), " AND ", " OR ")
    arrSize = UBound(WHEREArray)
    relCount = 0
    For count = 0 To arrSize
        ' SplitOnMultipleAndAppend gives back things of form "AND|OR <statement>"
        ' We just want the <statement> part
        tempArr = Split(WHEREArray(count), " ", 2)
        prefix = tempArr(0)
        WHEREArray(count) = tempArr(1)
        ' parse defined CambridgeSoft functions
        Set myCSFunction = New CSFunction
        myCSFunction.CreateFromSQL WHEREArray(count)
        Select Case myCSFunction.FunctionType
            Case kCSFTSubStruct, kCSFTExactStruct, kCSFTSimilarity
                Call AddToChemLinks(WHEREArray(count), myCSFunction.ReturnField, True)
            Case Else
                ' Parse simple field request
                Set myCSField = New CSField
                myCSField.CreateFromSQL WHEREArray(count)
                If myCSField.fieldtype <> kCSFDUnknown Then
                    Call AddToChemLinks(WHEREArray(count), myCSField, True)
                ElseIf myCSField.Comparator = "IN" Then
                    ' look at the field's value for another command
                    If InStr(myCSField.Value, "(SELECT ") Then
                        Set secCommand = New CSCommand
                        Set secCommand.ChemLinks = m_ChemLinks
                        ' create another command object to parse the subcommand
                        secCommand.CreateFromSQL StripParens(myCSField.Value)
                        ' add a command to the relational string, which will be filled
                        ' in later
                        m_hasRelQueryFields = True
                        ReDim Preserve RelWhereArr(relCount)
                        Dim t$
                        t = Left(WHEREArray(count), InStr(WHEREArray(count), "(") - 1)
                        RelWhereArr(relCount) = prefix & " " & t & "(" & secCommand.RelSql & ")"
                        relCount = relCount + 1
                    Else
                        ' not a chem field - pass on to relational string
                        m_hasRelQueryFields = True
                        ReDim Preserve RelWhereArr(relCount)
                        RelWhereArr(relCount) = prefix & " " & WHEREArray(count)
                        relCount = relCount + 1
                    End If
                Else
                    ' not a chem field - pass on to relational string
                    m_hasRelQueryFields = True
                    ReDim Preserve RelWhereArr(relCount)
                    RelWhereArr(relCount) = prefix & " " & WHEREArray(count)
                    relCount = relCount + 1
                End If
        End Select
    Next
    
    chemStr = GetChemQueryPlaceHolder
    relStr = Trim(Join(RelWhereArr, " "))
    If Left(relStr, 3) <> "OR " And Left(relStr, 4) <> "AND " And relStr <> "" And chemStr <> "" Then
        relStr = "AND " & relStr
    End If
    GetWhereStr = chemStr & " " & relStr
    
End Function

Private Function GetSelectStr(sql As String)
    Dim tempArr() As String
    Dim tempArr2() As String
    Dim SELECTArray() As String
    Dim RelSelectArr() As String
    Dim arrSize As Integer
    Dim relCount As Integer
    Dim preservePos As Integer
    Dim count As Integer
    Dim myCSFunction As CSFunction
    Dim myCSField As CSField
    Dim myCSChemLink As CSChemLink
        
    ' get SELECT portion of statement
    tempArr = Split(sql, " FROM ")
    SELECTArray = GetArrayPreserve(tempArr(0), ", ", "(", ")")
    arrSize = UBound(SELECTArray)
    ' loop over all expressions of form "[TableName].[FieldName] AS Blah"
    ' or [FieldName] AS Blah
    relCount = 0
    For count = 0 To arrSize
        ' is the statement a CS Function?
        Set myCSFunction = New CSFunction
        myCSFunction.CreateFromSQL SELECTArray(count)
        Select Case myCSFunction.FunctionType
            Case kCSFTStructFile, kCSFTBase64Cdx
                ' we only handle STRUCTFILE and BASE64CDX in the SELECT clause
                Call AddToChemLinks(SELECTArray(count), myCSFunction.ReturnField, False)
            Case Else
                ' not a function, see if it's a chem field
                Set myCSField = New CSField
                myCSField.CreateFromSQL SELECTArray(count)
                If myCSField.fieldtype <> kCSFDUnknown Then
                    Call AddToChemLinks(SELECTArray(count), myCSField, False)
                Else
                    ' not a chem field - pass on to relational string
                    m_hasRelReturnFields = True
                    ReDim Preserve RelSelectArr(relCount)
                    RelSelectArr(relCount) = SELECTArray(count)
                    relCount = relCount + 1
                End If
        End Select
    Next
      
    GetSelectStr = Join(RelSelectArr, ", ")
End Function

Private Function AddChemLinkRelFieldsToSelect(sql As String) As String
    Dim myCSChemLink As CSChemLink
    Dim count As Integer
    Dim needToAdd As Boolean
    
    ' add RelTable.RelField to select statement - this should always be returned if
    ' chemical information is to be returned
    If m_hasChemReturnFields Then
        For Each myCSChemLink In m_ChemLinks
            needToAdd = False
            With myCSChemLink
                If .QueryFields.count <> 0 Or .ReturnFields.count <> 0 Then
                    If count <> 0 Or sql <> "SELECT " Then
                        sql = sql & ", "
                    End If
                    sql = sql & .RelTable & "." & .RelField & " AS " & .UniqueRelFieldAlias
                End If
            End With
        Next
    End If
    
    AddChemLinkRelFieldsToSelect = sql
End Function

Private Function GetChemQueryPlaceHolder() As String
    Dim relCount As Integer
    Dim count As Integer
    Dim tempStr As String
    Dim returnSql As String
    Dim myCSChemLink As CSChemLink

    ' If the query has chemical attributes, add a clause representing them
    ' The list of mol_ids or temporary table will be added in CSDO.Request,
    ' after the chemical search takes place
    If m_hasChemQueryFields Then
        relCount = 0
        For Each myCSChemLink In m_ChemLinks
            With myCSChemLink
                For count = 0 To UBound(m_ChemTablesInFromStr)
                    If .RelTable = m_ChemTablesInFromStr(count) Then
                        If myCSChemLink.QueryFields.count <> 0 Then
                            tempStr = .RelTable & _
                                "." & .RelField & _
                                " IN (<CHEMLINK_" & .RelTable & ">)"
                            If relCount <> 0 Then
                                tempStr = " AND " & tempStr
                            End If
                            returnSql = returnSql & tempStr
                            relCount = relCount + 1
                        End If
                    End If
                Next
            End With
        Next
    End If

    GetChemQueryPlaceHolder = returnSql
End Function

Private Function GetArrayPreserve(sql As String, delim As String, preserveStart As String, preserveEnd As String) As Variant
    ' splits up a list, preserving delimiters in preserveStart - preserveEnd pairs
    Dim tempArr() As String
    Dim returnArr() As String
    Dim tempArrCount As Long
    Dim i As Long
    Dim j As Long
    
    ' TODO - return error if number of starting characters <> num of ending characters
    
    tempArr = Split(sql, delim)
    tempArrCount = UBound(tempArr)
    j = 0
    
    If tempArrCount = 0 Then
        returnArr = tempArr
    Else
        For i = 0 To tempArrCount
            If NumOfChars(tempArr(i), preserveStart) <> NumOfChars(tempArr(i), preserveEnd) Then
               tempArr(i + 1) = Join(Array(tempArr(i), tempArr(i + 1)), delim)
            Else
                ReDim Preserve returnArr(j)
                returnArr(j) = Trim(tempArr(i))
                j = j + 1
            End If
        Next
    End If
    
    GetArrayPreserve = returnArr
    
End Function

Private Function SplitOnMultipleAndAppend(aString$, delim1$, delim2$) As Variant
    ' given a string and two delimiters, splits the string
    ' on all delimiters and reappends the delimiters to each element of the array
    Dim tempArr() As String
    Dim tempArr2() As String
    Dim returnArr() As String
    Dim i&
    Dim j&
    Dim k&
    
    k = 0
    tempArr = GetArrayPreserve(aString, delim1, "(", ")")
    For i = 0 To UBound(tempArr)
        tempArr2 = GetArrayPreserve(tempArr(i), delim2, "(", ")")
        If UBound(tempArr2) > 0 Then
            For j = 0 To UBound(tempArr2)
                ReDim Preserve returnArr(k)
                returnArr(k) = Trim(delim2) & " " & Trim(tempArr2(j))
                k = k + 1
            Next
        Else
            ReDim Preserve returnArr(k)
            If i = 0 Then
                returnArr(k) = " " & Trim(tempArr(i))
            Else
                returnArr(k) = Trim(delim1) & " " & Trim(tempArr(i))
            End If
            k = k + 1
        End If
    Next
    
    SplitOnMultipleAndAppend = returnArr
End Function

Private Function NumOfChars(testString As String, charToTest As String) As Long
    ' returns the number of occurences of a given string
    Dim i As Long
    Dim numFound As Long
    
    numFound = 0
    
    For i = 1 To Len(testString)
        If Mid(testString, i, 1) = charToTest Then
            numFound = numFound + 1
       End If
    Next
    
    NumOfChars = numFound
    
End Function

Private Function StripParens(aString As String) As String
    Dim temp As String
    temp = Right(aString, Len(aString) - 1)
    StripParens = Left(temp, Len(temp) - 1)
End Function

Private Function RemoveExtraCommas(aString As String) As String
    ' just a hack to remove extra commas lying around
    ' also other garbage from my poor algorithms
    Dim retString As String
    
    retString = Replace(aString, ",)", ")")
    retString = Replace(retString, ", )", ")")
    retString = Replace(retString, "(,", "(")
    retString = Replace(retString, "( ,", "(")
    retString = Replace(retString, ",,", ",")
    retString = Replace(retString, ", ,", ",")
    
    retString = Replace(retString, " WHERE AND ", " WHERE  ")
    retString = Replace(retString, " WHERE OR", " WHERE ")
    
    RemoveExtraCommas = retString
End Function

Private Function CleanSQL(sql As String) As String
    Dim i&
    Dim charAtPos$
    Dim uon As Boolean
    Dim spon As Boolean
    Dim retString$
    
    uon = True
    
    ' get rid of brackets
    sql = Replace(sql, "[", "")
    sql = Replace(sql, "]", "")
    ' get rid of line breaks
    ' replace with spaces - if extras are added,
    ' they will be removed later
    sql = Replace(sql, vbCrLf, " ")
    sql = Replace(sql, vbCr, " ")
    sql = Replace(sql, vbLf, " ")
    ' add a space after all commas for consistency
    sql = Replace(sql, ",", ", ")
    
    For i = 1 To Len(sql)
        charAtPos = Mid(sql, i, 1)
        If charAtPos = "'" Then
            uon = Not uon
            retString = retString & charAtPos
            spon = False
        Else
            If charAtPos = " " Then
                If spon = False Then
                    spon = True
                    retString = retString & " "
                End If
            Else
                If uon Then
                    retString = retString & UCase(charAtPos)
                Else
                    retString = retString & charAtPos
                End If
                spon = False
            End If
        End If
    Next
    
    CleanSQL = retString
End Function
