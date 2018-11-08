VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Request"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

' Request - gives the user control over a pending request

Public Enum RequestStatus
    kCSRSDone = 0
    kCSRSCanceling = 1
    kCSRSChemSearch = 2
    kCSRSRelSearch = 3
    kCSRSMerging = 4
    kCSRSPreparing = 5
End Enum

Private m_Status As RequestStatus   ' current status
Private m_NumChemHits As Long       ' number of chem hits found thus far
Private m_NumChemExamined As Long   ' number of chemical records examined
Private m_NumRelHits As Long        ' number of relational hits

'!DGB! 04/18/01 new properties for hitlist management
Private m_hitlistID As Long      ' the id number for the hitlist to be saved
Private m_hitlistRecordCount As Long ' the number of records saved to hitlist

' private objects
Private m_ReturnRS As ADODB.Recordset     ' recordset returned
Attribute m_ReturnRS.VB_VarHelpID = -1
Private m_RelRS As ADODB.Recordset       ' relational recordset
Attribute m_RelRS.VB_VarHelpID = -1
Private m_oCSDOConnection As CSDO17.Connection      ' parent connection
Private m_Command As CSDO17.CSCommand                ' Command
Private m_Hitlists() As MolServer17.Hitlist          ' Hitlists from searches
Private m_HitlistRelTables() As String              ' Hitlist RelTable Names
Private m_ChemSearch() As MolServer17.Search         ' array of MolServer searches
Private m_ChemSearchInfo() As MolServer17.searchInfo ' array of MolServer search infos
Private m_dropTempTables As Boolean                 ' do we need to drop any temp tables?

'!DGB! 04/18/01 new properties for hitlist management
Public Property Get hitlistID() As Long
    hitlistID = m_hitlistID
End Property

Public Property Get hitlistRecordCount() As Long
    hitlistRecordCount = m_hitlistRecordCount
End Property

Public Property Get Status() As CSDO17.RequestStatus
    Status = m_Status
End Property

Public Property Get NumChemHits() As Long
    If m_ChemSearch(0).Status <> 0 Then
        NumChemHits = m_ChemSearch(0).CurID
    Else
        NumChemHits = m_NumChemHits
    End If
End Property

Public Property Get NumChemExamined() As Long
    If m_ChemSearch(0).Status <> 0 Then
        NumChemExamined = m_ChemSearch(0).NSeen
    Else
        NumChemExamined = m_NumChemExamined
    End If
End Property

Public Property Get NumRelHits() As Long
    NumRelHits = m_NumRelHits
End Property

Public Property Get Recordset() As ADODB.Recordset
    ' only return the recordset if we are done searching
    If m_Status = kCSRSDone Then
        Set Recordset = m_ReturnRS
    Else
        Set Recordset = Nothing
    End If
End Property

Friend Property Set CSDOConnection(aCSDOConnection As CSDO17.Connection)
    Set m_oCSDOConnection = aCSDOConnection
End Property

Friend Property Set Command(aCSCommand As CSDO17.CSCommand)
    Set m_Command = aCSCommand
End Property

Private Sub Class_Initialize()
    m_Status = kCSRSPreparing
    m_NumChemHits = 0
    m_NumChemExamined = 0
    m_NumRelHits = 0
    m_dropTempTables = False
End Sub

Private Sub Class_Terminate()
    Set m_RelRS = Nothing
    Set m_ReturnRS = Nothing
End Sub

Public Sub Cancel()
    ' cancels the pending operation
End Sub

Public Sub Start()
    ' Starts the requested operation
    ' Decide which kind of action to perform
    
On Error GoTo HandleError
    Select Case m_Command.CommandType
        Case kCSCTSelect
            Do_Select
        Case kCSCTDelete
            Do_Delete
        Case kCSCTUpdate
            Do_Update
        Case kCSCTInsert
            Do_Insert
        Case Else
            Set m_ReturnRS = Nothing
    End Select
    m_Status = kCSRSDone
    Exit Sub

HandleError:
    RaiseError "Start"
    
End Sub

Private Sub Do_Select()
    Dim conn As ADODB.Connection
    Dim myFields As CSFields
    Dim RelSql As String
    Dim hitlistID As Long
    Dim sHitlistContentsField As String
    
    Set conn = m_oCSDOConnection.ADOConnection

    With m_Command
        If .HasChemReturnFields = False And .HasChemQueryFields = False Then
            ' rel - only search
            ' use disconnected recordset
            Set m_ReturnRS = Nothing
            Set m_ReturnRS = New ADODB.Recordset
            m_ReturnRS.CursorLocation = adUseClient
            m_ReturnRS.MaxRecords = m_oCSDOConnection.ADOOptions.maxhits 'enables get more option for non Access DB
            
            '!DGB! 04/22/01 Join the CSDOhistlist table for if searching over a previous hitlist
            If m_oCSDOConnection.ChemSearchOptions.SearchDomain > 0 Then
                RelSql = JoinhitlistTableToRelSql(.RelSql)
            Else
                RelSql = .RelSql
            End If
            Debug.Print RelSql
            m_ReturnRS.Open RelSql, conn, adOpenStatic, adLockReadOnly
            Set m_ReturnRS.ActiveConnection = Nothing
            '!DGB! 04/22/01 Save the hitlist even for Relonly searches
            WriteHitlist RelSql
        ElseIf .HasRelReturnFields = False And .HasRelQueryFields = False Then
            ' chem - only search
            DoChemicalSearch False
            If UBound(m_Hitlists) = 1 Then
                ' if only one chemical command, simply make a hitlist
                Set myFields = .ChemLinks.Item(m_HitlistRelTables(0)).ReturnFields
                Set m_ReturnRS = RSFromChemHitlist(m_Hitlists(0), myFields)
            Else
                ' do something much more complicated...
                ' TODO - do it...
            End If
        Else
            ' chem - rel search
            If .HasChemQueryFields Then
                ' always do chemical searches first
                RelSql = DoChemicalSearch(True)
            Else
                RelSql = m_Command.RelSql
            End If
            ' use a disconnected client recordset.  Otherwise
            ' when the temp table is deleted, the recordset
            ' vanishes as well.
            ' Set m_RelRS = conn.Execute(RelSql)
            Set m_RelRS = Nothing
            Set m_RelRS = New ADODB.Recordset
            m_RelRS.CursorLocation = adUseClient
            m_RelRS.MaxRecords = m_oCSDOConnection.ADOOptions.maxhits 'enables get more option for non Access DB
            
            m_RelRS.Open RelSql, conn, adOpenStatic, adLockReadOnly
            Debug.Print RelSql
             '!DGB! 4/18/01 persist the hitlist to the db
            WriteHitlist RelSql
        
            ' drop temp tables created during operation, if necessary
            If m_dropTempTables Then
                DropTempTables
            End If
            Set m_RelRS.ActiveConnection = Nothing
            ' merge chemical data from all chemical searches
            If .HasChemReturnFields Then
                AddChemInfo
            Else
                Set m_ReturnRS = m_RelRS
            End If
        End If
    End With
    
    Set conn = Nothing
    Debug.Print RelSql
End Sub

'!DGB! 04/22/01
Private Function JoinhitlistTableToRelSql(aSql As String) As String
    Dim ADOOpts As ADOOptions
 Set ADOOpts = m_oCSDOConnection.ADOOptions
  Select Case ADOOpts.StoreType
    Case kSTOracle
        ' Add CSDOhitlist at the front of the the FROM clause
         aSql = Replace(aSql, "FROM ", "FROM " & ADOOpts.AdminUID & ".CSDOhitlist, ")
        ' Add Join and constrain at the front of the WHERE clause
         aSql = Replace(aSql, "WHERE ", "WHERE " & ADOOpts.AdminUID & ".CSDOhitlist.ID = " & m_oCSDOConnection.ChemSearchOptions.hitlistContentsField & " AND " & ADOOpts.AdminUID & ".CSDOhitlist.hitlistID =" & m_oCSDOConnection.ChemSearchOptions.SearchDomain & " AND ")
    Case Else
        ' Add CSDOhitlist at the front of the the FROM clause
        aSql = Replace(aSql, "FROM ", "FROM CSDOhitlist, ")
        ' Add Join and constrain at the front of the WHERE clause
         aSql = Replace(aSql, "WHERE ", "WHERE CSDOhitlist.ID = " & m_oCSDOConnection.ChemSearchOptions.hitlistContentsField & " AND CSDOhitlist.hitlistID =" & m_oCSDOConnection.ChemSearchOptions.SearchDomain & " AND ")
    End Select
    JoinhitlistTableToRelSql = aSql
End Function

'!DGB! 04/18/01 New Sub saves the hitlist to DB tables
'TODO check oracle syntax and test!
Private Sub WriteHitlist(aSql As String)
   Dim conn As ADODB.Connection
    Dim RS As ADODB.Recordset
    Dim ADOOpts As ADOOptions
    Dim sql As String
    Dim Sql1, sql2 As String
    Dim FromClause As String
    Dim lRecsAffected As Long
    Dim AdminDSN As String
    Dim hitlistID As Long
    Dim hitlistContentsField As String
    Dim st As Single
    Dim hitlistTableExists As Boolean
    Dim hitlistIDTableExists As Boolean
    
    Set ADOOpts = m_oCSDOConnection.ADOOptions
    
    
    ' Get the name of the field to be saved from the ChemSearchOptions object
    hitlistContentsField = m_oCSDOConnection.ChemSearchOptions.hitlistContentsField
    
    If Not hitlistContentsField = "" Then
        st = Timer
        hitlistTableExists = TableExists("CSDOhitlist")
        hitlistIDTableExists = TableExists("CSDOhitlistID")
        Set conn = New ADODB.Connection
        AdminDSN = ADOOpts.GetAdminADOConnString
        conn.Open AdminDSN
        
        If Not (hitlistIDTableExists And hitlistTableExists) Then
            'Create the hitlist Tables
                If hitlistTableExists Then
                    'Drop CSDOhitlist
                    sql = "DROP TABLE CSDOhitlist"
                    conn.Execute sql, , adCmdText + adExecuteNoRecords
                End If
                If hitlistIDTableExists Then
                    'Drop CSDOhitlistID
                    sql = "DROP TABLE CSDOhitlistID"
                    conn.Execute sql, , adCmdText + adExecuteNoRecords
                End If
                
            'Create both tables
                Select Case ADOOpts.StoreType
                    Case kSTOracle
               
                        'TODO Check oracle syntax for Autonum field and pk constraint
                        'LJB 4/19 corrected oracle syntax
                        On Error Resume Next
                        conn.Execute ("CREATE TABLE CSDOhitlistID (NAME VARCHAR2(50), ID NUMBER(8,0),  CONSTRAINT pkHitlistID PRIMARY KEY(ID))")
                        conn.Execute ("DROP SEQUENCE CSDOhitLISTID_SEQ")
                        conn.Execute ("CREATE SEQUENCE CSDOhitLISTID_SEQ INCREMENT BY 1 START WITH 1")
                        conn.Execute ("CREATE TABLE CSDOhitlist (hitlistID NUMBER(8,0), ID NUMBER(8,0) )")
                         'make sure current user has privileges on hitlist tables, otherwise table does not exist message occurs.
                        conn.Execute ("GRANT DELETE,UPDATE,INSERT, SELECT on CSDOhitLISTID to " & m_oCSDOConnection.ADOUID)
                        conn.Execute ("GRANT DELETE,UPDATE,INSERT, SELECT on CSDOhitlist to " & m_oCSDOConnection.ADOUID)
                        conn.Execute ("commit")
                        On Error GoTo 0
                    '!DGB! 06/14/01 added SQLServer case
                    Case kSTSQLServer
                        Sql1 = "CREATE TABLE CSDOhitlistID (NAME CHAR(50), ID INT IDENTITY CONSTRAINT pkHitlistID PRIMARY KEY)"
                        sql2 = "CREATE TABLE CSDOhitlist (hitlistID INT, ID INT)"
                        conn.Execute Sql1, , adCmdText + adExecuteNoRecords
                        conn.Execute sql2, , adCmdText + adExecuteNoRecords
                    Case Else
                        Sql1 = "CREATE TABLE CSDOhitlistID (NAME CHAR(50), ID COUNTER CONSTRAINT pkHitlistID PRIMARY KEY)"
                        sql2 = "CREATE TABLE CSDOhitlist (hitlistID INTEGER, ID INTEGER)"
                        conn.Execute Sql1, , adCmdText + adExecuteNoRecords
                        conn.Execute sql2, , adCmdText + adExecuteNoRecords
                End Select
        End If
        
        ' Come up with a unique hitlistID
            If m_oCSDOConnection.ChemSearchOptions.hitlistID = 0 Then
            'LJB 4/19 added case for oracle insert so nexval from sequence can be grabbed.
                Select Case ADOOpts.StoreType
                    Case kSTOracle
                        conn.Execute "INSERT INTO CSDOhitlistID (Name, ID)VALUES ('TEMP',CSDOhitLISTID_SEQ.NEXTVAL )", lRecsAffected, adCmdText + adExecuteNoRecords
                        Set RS = conn.Execute("SELECT Max(ID) AS ID FROM CSDOhitlistID", lRecsAffected, adCmdText)
                        hitlistID = RS("ID")
                        RS.Close
                    Case Else
                        conn.Execute "INSERT INTO CSDOhitlistID (Name)VALUES ('TEMP')", lRecsAffected, adCmdText + adExecuteNoRecords
                        Set RS = conn.Execute("SELECT Max(ID) AS MaxID FROM CSDOhitlistID", lRecsAffected, adCmdText)
                        hitlistID = RS("MaxID")
                        RS.Close
                End Select
            Else
                hitlistID = m_oCSDOConnection.ChemSearchOptions.hitlistID
            End If
        
        ' Delete the previous hitlist
        conn.Execute "DELETE FROM CSDOhitlist WHERE hitlistID=" & hitlistID, lRecsAffected, adCmdText + adExecuteNoRecords
        
        ' Write Hitlist to table
        FromClause = Right(aSql, Len(aSql) - InStr(1, aSql, "FROM") + 2)
         '!LJB! 10/9/01 allow for order by and group sql
        If InStr(FromClause, " ORDER BY ") > 0 Or InStr(FromClause, " GROUP BY ") > 0 Then
            Dim OrderByArray() As String
            Dim GroupByArray() As String
            If InStr(FromClause, " ORDER BY ") Then
                OrderByArray = Split(FromClause, " ORDER BY ", -1)
                FromClause = OrderByArray(0)
            End If
            If InStr(FromClause, " GROUP BY ") Then
                GroupByArray = Split(FromClause, " GROUP BY ", -1)
                FromClause = GroupByArray(0)
            End If
        End If
        
        Select Case ADOOpts.StoreType
            '!DGB! 04/22/01 Reversed order of select clause fields to support odbc driver
            Case kSTOracle
                'TODO check oracle syntax for insert into from select
                sql = "INSERT INTO CSDOhitlist SELECT DISTINCT " & hitlistID & " AS hitlistID, " & hitlistContentsField & " AS ID " & FromClause
               
            Case Else
                sql = "INSERT INTO CSDOhitlist SELECT DISTINCT " & hitlistID & " AS hitlistID, " & hitlistContentsField & " AS ID " & FromClause
        End Select
         Debug.Print "csdohit list insert " & sql
        conn.Execute sql, lRecsAffected, adCmdText + adExecuteNoRecords
        conn.Close
        '!DGB! return hitlistID and hitlistRecordCount
        m_hitlistID = hitlistID
        m_hitlistRecordCount = lRecsAffected
        Debug.Print "Number of records in hitlist= " & lRecsAffected
        Debug.Print "Time to save hitlist= " & Timer - st & " seconds."
    Else
        sql = "Hitlist not saved"
    End If
End Sub



Private Sub DropTempTables()
    Dim conn As ADODB.Connection
    Dim cmd As ADODB.Command
    Dim ADOOpts As ADOOptions
    Dim tempTableName As String
    Dim AdminDSN As String
    Dim myCSChemLink As CSChemLink
    
    Set conn = New ADODB.Connection
    Set ADOOpts = m_oCSDOConnection.ADOOptions
    tempTableName = ADOOpts.TempTableBaseName & ADOOpts.UniqueSessID
    AdminDSN = ADOOpts.GetAdminADOConnString
    
    conn.Open AdminDSN
    Set cmd = New ADODB.Command
    For Each myCSChemLink In m_Command.ChemLinks
            tempTableName = ADOOpts.TempTableBaseName & ADOOpts.UniqueSessID & Left(myCSChemLink.RelTable, 5)
        With cmd
            ' complains if table doesn't already exist
            Set .ActiveConnection = conn
            .CommandType = adCmdText
            .CommandText = "drop table " & tempTableName
            On Error Resume Next
            .Execute
            'if oracle, then revoke the select privileges on the temptables created
           If ADOOpts.StoreType = kSTOracle Then
                .CommandText = "REVOKE Select on " & tempTableName & " to " & m_oCSDOConnection.ADOUID
                .Execute
           End If
            On Error GoTo 0
        End With
    Next
    Set cmd = Nothing
    Set conn = Nothing
    m_dropTempTables = False

End Sub

Private Sub Do_Insert()
    Dim conn As ADODB.Connection
    Dim md As MolServer17.Document
    Dim mol As MolServer17.Molecule
    Dim fld As CSField
    Dim link As CSChemLink
    Dim RelSql As String
    Dim linkfield$
    Dim linkTable$
    Dim count As Long
    
    Set conn = m_oCSDOConnection.ADOConnection
    Set md = m_oCSDOConnection.MolDocument
    
    ' get structure field
    For Each link In m_Command.ChemLinks
        If link.ReturnFields.count <> 0 Then
            Set fld = link.ReturnFields.Item("STRUCTURE")
            linkfield = link.RelField
            linkTable = link.RelTable
            Exit For
        End If
    Next
    
    With m_Command
        If Not .HasChemReturnFields Then
            ' rel - only insert
            Set m_ReturnRS = conn.Execute(.RelSql)
        Else
            ' insert chemical and relational data
            ' insert into molserver first
            count = md.count
            count = count + 1
            Set mol = New MolServer17.Molecule
            mol.Read fld.StrucPath
            md.Lock
            md.PutMol mol, count
            md.Unlock
            ' then insert into rel
            RelSql = Replace(.RelSql, "<CHEMLINK_" & linkTable & ">", count)
            Set m_ReturnRS = conn.Execute(RelSql)
        End If
    End With

End Sub

Private Sub Do_Delete()
    Dim conn As ADODB.Connection
    Dim md As MolServer17.Document
    Dim hl As MolServer17.Hitlist
    Dim link As CSChemLink
    Dim linkfield As String
    Dim linkTable As String
    Dim RelSql As String
    Dim count As Long
    
    Set conn = m_oCSDOConnection.ADOConnection
    Set md = m_oCSDOConnection.MolDocument
    
    
    For Each link In m_Command.ChemLinks
        If link.QueryFields.count <> 0 Then
            linkfield = link.RelField
            linkTable = link.RelTable
            Exit For
        End If
    Next

    
    With m_Command
        If .HasRelQueryFields And Not .HasChemQueryFields Then
            ' rel - only criteria for delete
            Set m_ReturnRS = conn.Execute(.RelSql)
        ElseIf .HasChemQueryFields And Not .HasRelQueryFields Then
            ' chem - only criteria (but must execute a rel query to delete)
            RelSql = DoChemicalSearch(True)
            ' there can be only one hitlist, since there is only one table in a
            ' DELETE statement
            For count = 0 To m_Hitlists(0).count - 1
                md.DeleteMol (m_Hitlists(0).At(count))
            Next
            Set m_ReturnRS = conn.Execute(RelSql)
        Else
            ' both chem and rel criteria - must execute combined
            ' select query first to determine appropriate records
            ' for deletion
            RelSql = DoChemicalSearch(False, True)
            Set m_RelRS = conn.Execute(RelSql)
            hl = ChemHitlistFromRS(m_RelRS, linkfield)
            md.Lock
            For count = 0 To hl.count - 1
                md.DeleteMol (hl.At(count))
            Next
            md.Unlock
            ' delete from rel table based on results of combined chem and rel queries
            RelSql = AddChemHitlistCriteria(.RelSql, hl, link)
            Set m_ReturnRS = conn.Execute(RelSql)
        End If
    End With
End Sub

Private Sub Do_Update()
    Dim conn As ADODB.Connection
    Dim link As CSChemLink
    Dim fld As CSField
    Dim md As MolServer17.Document
    Dim mol As MolServer17.Molecule
    Dim hl As MolServer17.Hitlist
    Dim RelSql As String
    Dim count As Long
    Dim linkfield$
    Dim linkTable$
    
    Set conn = m_oCSDOConnection.ADOConnection
    Set md = m_oCSDOConnection.MolDocument
    
    For Each link In m_Command.ChemLinks
        If link.ReturnFields.count <> 0 Then
            Set fld = link.ReturnFields.Item("STRUCTURE")
            linkfield = link.RelField
            linkTable = link.RelTable
            Exit For
        End If
    Next
    
    With m_Command
        If Not .HasChemQueryFields And Not .HasChemReturnFields Then
            ' rel - only criteria and input
            Set m_ReturnRS = conn.Execute(.RelSql)
        ElseIf Not .HasRelQueryFields And Not .HasRelReturnFields Then
            ' chem - only criteria and update
            DoChemicalSearch False
            Set mol = New MolServer17.Molecule
            ' get the structure from the return field and update chem
            mol.Read fld.StrucPath
            For count = 0 To m_Hitlists(0).count - 1
                md.PutMol mol, m_Hitlists(0).At(count)
            Next
            Set m_ReturnRS = Nothing
        Else
            If .HasChemQueryFields And .HasRelQueryFields Then
                ' merge mol_id hitlists from both searches to get final update list
                RelSql = DoChemicalSearch(False, True)
                Set m_RelRS = conn.Execute(RelSql)
                Set hl = ChemHitlistFromRS(m_RelRS, linkfield)
                RelSql = AddChemHitlistCriteria(.RelSql, hl, link)
            ElseIf .HasChemQueryFields Then
                ' mol_id hitlist from just chemical search
                RelSql = DoChemicalSearch(True)
                Set hl = m_Hitlists(1)
            Else
                ' mol_id hitlist from just rel search
                RelSql = .RelSelectSql
                Set m_RelRS = conn.Execute(RelSql)
                Set hl = ChemHitlistFromRS(m_RelRS, linkfield)
            End If
            
            If .HasChemReturnFields Then
                ' read the structure in and and update chem
                Set mol = New MolServer17.Molecule
                mol.Read fld.StrucPath
                Dim posToInsertAt As Long
                For count = 0 To hl.count - 1
                    ' if mol_id = 0, then no correalation
                    ' appears yet - add one.
                    If hl.At(count) = 0 Then
                        posToInsertAt = md.count + 1
                    Else
                        posToInsertAt = hl.At(count)
                    End If
                    If posToInsertAt > md.count Then
                        ' insert the new mol_id into the relational store
                        Dim insSql As String
                        Dim wherePos As Integer
                        insSql = "UPDATE " & linkTable & _
                                 " SET " & linkfield & " = " & posToInsertAt & " "
                        wherePos = InStr(RelSql, " WHERE")
                        If wherePos <> 0 Then
                            insSql = insSql & Right(RelSql, Len(RelSql) - wherePos)
                        End If
                        conn.Execute insSql
                    End If
                    md.PutMol mol, posToInsertAt
                Next
            End If
            
            If .HasRelReturnFields Then
                ' execute the rel update command
                Set m_ReturnRS = conn.Execute(RelSql)
            End If
            
            Set m_ReturnRS = Nothing
        End If
    End With
End Sub

Private Function DoChemicalSearch(bUpdateRelSql As Boolean, Optional bUpdateRelSelectSql As Boolean = False) As String
    Dim myChemLinks As CSChemLinks
    Dim myLink As CSChemLink
    Dim myFields As CSFields
    Dim myMolDocument As MolServer17.Document
    Dim retString As String
    Dim count As Integer
    
    Set myMolDocument = m_oCSDOConnection.MolDocument
    
    Set myChemLinks = m_Command.ChemLinks
    
    If bUpdateRelSql Then
        retString = m_Command.RelSql
    ElseIf bUpdateRelSelectSql Then
        retString = m_Command.RelSelectSql
    Else
        retString = ""
    End If
  
    count = 1
    For Each myLink In myChemLinks
        Set myFields = myLink.QueryFields
        If myFields.count <> 0 Then
            ReDim Preserve m_ChemSearchInfo(count)
            ReDim Preserve m_ChemSearch(count)
            ReDim Preserve m_Hitlists(count)
            ReDim Preserve m_HitlistRelTables(count)
            Set m_ChemSearchInfo(count - 1) = SearchInfoFromQueryFields(myFields)
            ' use global miscellaneous search options in connection's
            ' ChemSearchInfo
            m_oCSDOConnection.ChemSearchOptions.FillSearchInfo m_ChemSearchInfo(count - 1)
            Set m_ChemSearch(count - 1) = myMolDocument.Search(m_ChemSearchInfo(count - 1))
            m_ChemSearch(count - 1).Start
            
            Dim st As Single
            st = Timer
            Do
                ' wait for search to complete
                m_ChemSearch(count - 1).WaitForCompletion 1000
                m_NumChemExamined = m_ChemSearch(count - 1).NSeen
                m_NumChemHits = m_ChemSearch(count - 1).CurID
            Loop While m_ChemSearch(count - 1).Status = 1
            Debug.Print "MolServer search time= " & Timer - st & " seconds"
            Set m_Hitlists(count - 1) = m_ChemSearch(count - 1).Hitlist
            m_HitlistRelTables(count - 1) = myLink.RelTable
            
            If bUpdateRelSql Or bUpdateRelSelectSql Then
                retString = AddChemHitlistCriteria(retString, m_Hitlists(count - 1), myLink)
            End If

            count = count + 1
        End If
    Next
    DoChemicalSearch = retString
End Function

Private Function ChemHitlistFromRS(aRS As ADODB.Recordset, molField As String) As MolServer17.Hitlist
    Dim rHL As MolServer17.Hitlist
    Dim myMolDocument As MolServer17.Document
    Dim i As Long
    
    Set rHL = New MolServer17.Hitlist
    Set myMolDocument = m_oCSDOConnection.MolDocument
    
    aRS.MoveFirst
    i = 1
    Do Until aRS.EOF
        If IsNull(aRS.Fields.Item(molField)) Then
            ' add new mol_id(s) at the end of the mst file
            rHL.AddHit myMolDocument.count + i
            i = i + 1
        Else
            rHL.AddHit (aRS.Fields.Item(molField))
        End If
        aRS.MoveNext
    Loop

    Set ChemHitlistFromRS = rHL
End Function

Private Function RSFromChemHitlist(aHitlist As MolServer17.Hitlist, aReturnFields As CSFields) As ADODB.Recordset
    ' create an ADO Recordset from a MolServer hitlist
    Dim returnRS As ADODB.Recordset
    Dim count As Long
    Dim idAtCount As Long
    ' Dim needMol As Boolean
    Dim tempMol As MolServer17.Molecule
    Dim theValue As Variant
    Dim myFields As CSFields
    Dim myField As CSField
    Dim myMolDocument As MolServer17.Document
    
    ' TODO - this needs help
    
On Error GoTo 0
    
    Set returnRS = New ADODB.Recordset
    Set myFields = aReturnFields
    Set myMolDocument = m_oCSDOConnection.MolDocument
    
    ' Append appropriate fields, and determine if
    ' we need to retrieve the actual molecule...
    ' needMol = False
    For Each myField In myFields
        With myField
            ' If .fieldtype <> kCSFDMol_Id Then
            '     needMol = True
            ' End If
            returnRS.Fields.Append .AssignedName, .ADOType, .DefaultSize, .ADOFieldAttrs
        End With
    Next
    
    ' need to open the recordset before applying data to it
    returnRS.Open
    
    For count = 0 To aHitlist.count - 1
        returnRS.AddNew
        idAtCount = aHitlist.At(count)
        ' If needMol Then
        Set tempMol = myMolDocument.GetMol(idAtCount)
        ' End If
        
        For Each myField In myFields
             If Not tempMol Is Nothing Then
                 ' if a valid mol was found, get its attributes
                 Select Case myField.fieldtype
                     Case kCSFDFormula
                         theValue = tempMol.Formula
                     Case kCSFDMolWt
                         theValue = tempMol.MolWeight
                     Case kCSFDSmiles
                         theValue = ""
                     Case kCSFDStructFile
                        theValue = Replace(myField.StrucPath, "*", idAtCount) & myField.StrucExt
                         tempMol.Write theValue
                     Case kCSFDBase64Cdx
                        theValue = Base64CdxFromMolecule(tempMol, m_oCSDOConnection.ADOOptions.ScratchDir & "b64temp.cdx")
                     Case Else
                         ' do nothing
                 End Select
             Else
                 ' if no valid mol, set the value to an empty type
                 Select Case myField.fieldtype
                     Case kCSFDFormula
                         theValue = ""
                     Case kCSFDMolWt
                         theValue = -1
                     Case kCSFDSmiles
                         theValue = ""
                     Case kCSFDStructFile
                         theValue = ""
                     Case kCSFDBase64Cdx
                         theValue = ""
                     Case Else
                         ' do nothing
                 End Select
             End If
            returnRS.Fields(myField.AssignedName).Value = theValue
        Next
    Next
    
    returnRS.UpdateBatch
    Set RSFromChemHitlist = returnRS
    Exit Function
    
ErrorHandler:
    RaiseError "RSFromChemHitlist"
    
End Function

Private Function AddChemInfo() As ADODB.Recordset
    ' append chemical info to an existing relational recordset
    ' for now, we are creating a new recordset from scratch - not ideal
    Dim count As Long
    Dim idAtCount As Long
    Dim tempMol As MolServer17.Molecule
    Dim theValue As Variant
    Dim myMolDocument As MolServer17.Document
    Dim returnRS As ADODB.Recordset
    Dim myADOField As ADODB.Field
    Dim myField As CSField
    Dim myFields As CSFields
    Dim myLink As CSChemLink
    Dim numReturnLinks As Integer
    Dim ReturnFields As CSFields
    Dim fieldname As String
        
On Error GoTo 0

    If m_RelRS.EOF Then
        Set AddChemInfo = Nothing
        ' ** FUNCTION EXIT POINT **
        Exit Function
    End If
    
    Set returnRS = New ADODB.Recordset
    Set myMolDocument = m_oCSDOConnection.MolDocument
    
    ' Find out how many of our ChemLinks contain data to be returned in
    ' the recordset (this affects the naming of the recordset fields)
    For Each myLink In m_Command.ChemLinks
        Set ReturnFields = myLink.ReturnFields
        If ReturnFields.count <> 0 Then
            numReturnLinks = numReturnLinks + 1
        End If
    Next
        
    ' Append chemical fields
    For Each myLink In m_Command.ChemLinks
        Set ReturnFields = myLink.ReturnFields
        If ReturnFields.count <> 0 Then
            For Each myField In ReturnFields
                With myField
                    ' Determine name of field
                    If .AssignedName <> .DefaultName And .AssignedName <> "" Then
                        fieldname = .AssignedName
                    ElseIf numReturnLinks > 1 Then
                        fieldname = myLink.RelTable & "." & .DefaultName
                        .AssignedName = fieldname
                    Else
                        fieldname = .DefaultName
                    End If
                    ' If .fieldtype <> kCSFDMol_Id Then
                    returnRS.Fields.Append fieldname, .ADOType, .DefaultSize, .ADOFieldAttrs + adFldIsNullable
                    ' End If
                End With
            Next
        End If
    Next
    
    ' append fields from original recordset
    For Each myADOField In m_RelRS.Fields
        'returnRS.Fields.Append myADOField.Name, myADOField.Type, myADOField.DefinedSize, adFldUpdatable ' myADOField.Attributes
        ' TODO - figure out why Oracle returns bad field type
        returnRS.Fields.Append myADOField.Name, adVariant, myADOField.DefinedSize, adFldUpdatable + adFldIsNullable
    Next
    
    returnRS.Open
        
    ' fill in return recordset
    m_RelRS.MoveFirst
    Do While m_RelRS.EOF = False
        returnRS.AddNew
        For Each myLink In m_Command.ChemLinks
            Set ReturnFields = myLink.ReturnFields
            If ReturnFields.count <> 0 Then
                If IsNull(m_RelRS.Fields.Item(myLink.UniqueRelFieldAlias).Value) Then
                    Set tempMol = Nothing
                Else
                    idAtCount = m_RelRS.Fields.Item(myLink.UniqueRelFieldAlias).Value
                    Set tempMol = myMolDocument.GetMol(idAtCount)
                End If
                ' append chemical data
                For Each myField In ReturnFields
                    If Not tempMol Is Nothing Then
                        ' if a valid mol was found, get its attributes
                        Select Case myField.fieldtype
                            Case kCSFDFormula
                                theValue = tempMol.Formula
                            Case kCSFDMolWt
                                theValue = tempMol.MolWeight
                            Case kCSFDSmiles
                                theValue = ""
                            Case kCSFDStructFile
                                theValue = Replace(myField.StrucPath, "*", idAtCount) & myField.StrucExt
                                tempMol.Write theValue
                            Case kCSFDBase64Cdx
                                theValue = Base64CdxFromMolecule(tempMol, m_oCSDOConnection.ADOOptions.ScratchDir & "b64temp.cdx")
                            Case Else
                                ' do nothing
                        End Select
                    Else
                        ' if no valid mol, set the value to an empty type
                        Select Case myField.fieldtype
                            Case kCSFDFormula
                                theValue = ""
                            Case kCSFDMolWt
                                theValue = -1
                            Case kCSFDSmiles
                                theValue = ""
                            Case kCSFDStructFile
                                theValue = ""
                            Case kCSFDBase64Cdx
                                theValue = ""
                            Case Else
                                ' do nothing
                        End Select
                    End If
                    ' If myField.fieldtype <> kCSFDMol_Id Then
                    returnRS(myField.AssignedName).Value = theValue
                    ' End If
                Next
            End If
        Next
        ' append relational data
        For Each myADOField In m_RelRS.Fields
            returnRS.Fields(myADOField.Name).Value = myADOField.Value
        Next
        ' returnRS.Update
        m_RelRS.MoveNext
    Loop
        
    returnRS.UpdateBatch
    Set m_ReturnRS = returnRS
    Exit Function
    
ErrorHandler:
    RaiseError "AddChemInfo"
End Function

Private Function AddChemHitlistCriteria(RelSql As String, aHitlist As MolServer17.Hitlist, aChemLink As CSChemLink) As String
    ' adds the chemical hitlist criteria to an existing rel query
    Dim i As Long
    Dim tempStr As String
    Dim replaceString, replaceString2 As String
    Dim replacementStr1, replacementStr2 As String
    Dim actualjoinmethod As Long
    Dim ADOOpts As ADOOptions
    Dim conn As ADODB.Connection
    Dim tempTableName As String
    Dim oTempTableRS As ADODB.Recordset
    Dim lngRecs As Long
    Dim st As Single
    
    Set ADOOpts = m_oCSDOConnection.ADOOptions
    '!DGB! 04/18/01 Added ChemTable name to temptable name to avoid temptable overwrites on a subsequent chemsearch
    tempTableName = ADOOpts.TempTableBaseName & ADOOpts.UniqueSessID & Left(aChemLink.RelTable, 5)
  
    If aHitlist.count = 0 Then
        actualjoinmethod = -1
    Else
        actualjoinmethod = ADOOpts.GetJoinMethod(aHitlist.count)
    End If
    '!DGB! 04/18/01 Modified replacement strategy to support new PlaceHolder syntax
    Select Case actualjoinmethod
        Case -1
            '!DGB! 04/18/01
            ' In this case the replacement forces an always false WHERE clause
            replaceString = "<CHEMLINKFIELD_" & aChemLink.RelTable & ">"
            replaceString2 = ", <CHEMLINKTABLE_" & aChemLink.RelTable & ">"
            replacementStr1 = "-1"
            replacementStr2 = ""
        Case kJMChemFirstMOLIDString
            '!DGB! 04/18/01
            ' In this case the replacement recreates the IN () clasue join method
            replaceString = "= <CHEMLINKFIELD_" & aChemLink.RelTable & ">"
            replaceString2 = ", <CHEMLINKTABLE_" & aChemLink.RelTable & ">"
            For i = 0 To aHitlist.count - 1
                If i = 0 Then
                    replacementStr1 = "IN (" & aHitlist.At(i)
                Else
                    replacementStr1 = replacementStr1 & "," & aHitlist.At(i)
                End If
            Next
            replacementStr1 = replacementStr1 & ")"
            replacementStr2 = ""
        Case kJMChemFirstTempTable
            
            
            Set conn = New ADODB.Connection
            Dim AdminDSN As String
            AdminDSN = ADOOpts.GetAdminADOConnString
            conn.Open AdminDSN
            Dim cmd As ADODB.Command
            Set cmd = New ADODB.Command
            ' create the temp table
            With cmd
                Set .ActiveConnection = conn
                .CommandType = adCmdText
                ' complains if table doesn't already exist
                On Error Resume Next
                .CommandText = "drop table " & tempTableName
                .Execute
                On Error GoTo 0
                Select Case ADOOpts.StoreType
                    Case kSTOracle
                        .CommandText = "create table " & tempTableName & " (ID NUMBER(7))"
                    Case Else
                    '!DGB! 06/14/01 Changed field type for SQL Server compat
                        .CommandText = "create table " & tempTableName & " (ID INT)"
                End Select
                .Execute
                'Use new ADO RS Update method to create temp table
                ' fill with ids
                
                ' Create an ADO Recordset bound to the empty temp table
                    st = Timer
                    Set oTempTableRS = New ADODB.Recordset
                    With oTempTableRS
                        .ActiveConnection = conn
                        .CursorType = adOpenDynamic
                        .LockType = adLockOptimistic
                        .Open tempTableName, , , , adCmdTableDirect
        
                        'Loop over the hitlist and populate the temptable by updating the RS
                        For i = 0 To aHitlist.count - 1
                            .AddNew
                            .Fields("ID") = aHitlist.At(i)
                        Next
                        .Update
                        .Close
                    End With
                    Debug.Print "Number of rows in tempTable= " & aHitlist.count
               
                '!DGB! 06/14/01 do not create index
                ' create index on temptable
                '.CommandText = "create index ID_INDX" & tempTableName & " on " & tempTableName & " (ID)"
                '.Execute
                Debug.Print "Time to build tempTable= " & Timer - st & " seconds"
            End With
            '!DGB! 04/18/01
            'In this case replacement yields new Join syntax
            'LJB 4/22/01 moved and modified for oracle compatability
            
            replaceString = "<CHEMLINKFIELD_" & aChemLink.RelTable & ">"
            replaceString2 = "<CHEMLINKTABLE_" & aChemLink.RelTable & ">"
            replacementStr1 = tempTableName & ".ID"
            Select Case ADOOpts.StoreType
                Case kSTOracle
                    'make sure current user has privileges on the temptable, otherwise table does not exist message occurs.
                    'also note that this is revoked in DropTempTables
                    conn.Execute ("GRANT Select on " & tempTableName & " to " & m_oCSDOConnection.ADOUID)
                    conn.Execute ("commit")
                    replacementStr2 = ADOOpts.AdminUID & "." & tempTableName
                Case Else
                    replacementStr2 = tempTableName
            End Select
        Case kJMChemFirstTempTableUsingORASQLLDR
            
           
            ' Oracle-only join solution
            Dim BasePath As String
            Dim ExportIDsPath As String
            Dim ExportCtlPath As String
            Dim ExportLogPath As String
            Dim ExportBatchPath As String
            Dim SQLLoaderPath As String
            Dim TempTable As String
            Dim ORAServiceName As String
       
            BasePath = ADOOpts.ScratchDir & ADOOpts.UniqueSessID
            ExportIDsPath = BasePath & "exportids.dat"
            ExportCtlPath = BasePath & "exportids.ctl"
            ExportLogPath = BasePath & "sqlldr.log"
            ExportBatchPath = BasePath & "loader.bat"
            SQLLoaderPath = ADOOpts.ORASQLLDRPath
            ORAServiceName = ADOOpts.ServiceName
            TempTable = tempTableName
            aHitlist.Write ExportIDsPath
            AdminDSN = ADOOpts.GetAdminADOConnString
            Dim retVal As Boolean
            Set conn = New ADODB.Connection
            conn.Open AdminDSN
            '!LJB! 10/2/01 Add direct keyword support
            retVal = RunSQLLdr(conn:=conn, _
                               uname:=ADOOpts.AdminUID, _
                               pwd:=ADOOpts.AdminPWD, _
                               tempTableName:=TempTable, _
                               batchPath:=ExportBatchPath, _
                               sqlLdrPath:=SQLLoaderPath, _
                               ServiceName:=ORAServiceName, _
                               ExportIDsPath:=ExportIDsPath, _
                               ExportCtlPath:=ExportCtlPath, _
                               logPath:=ExportLogPath, _
                               directKeyword:=ADOOpts.DirectKeywordType)
               
           'make sure current user has privileges on the temptable, otherwise table does not exist message occurs.
           'also note that this is revoked in DropTempTables
           conn.Execute ("GRANT Select on " & TempTable & " to " & m_oCSDOConnection.ADOUID)
           conn.Execute ("commit")
            'LJB 04/22/01 added missing In this case replacement yields new Join syntax in the oracle fashion
            replaceString = "<CHEMLINKFIELD_" & aChemLink.RelTable & ">"
            replaceString2 = "<CHEMLINKTABLE_" & aChemLink.RelTable & ">"
            replacementStr1 = tempTableName & ".ID"
            replacementStr2 = ADOOpts.AdminUID & "." & tempTableName
    End Select
       
    ' conn will be nothing if the actualjoinmethod is -1 or kJMChemFirstMOLIDString
    If (Not conn Is Nothing) Then
        If conn.State <> adStateClosed Then
            ' Explicitely close the connection to flush its table operations.
            conn.Close
            Set conn = Nothing
            ' Since you have created a temporary table, any cached connection to the database
            ' may have invalid table caches. These connections will need to be closed, then re-opened
            ' so that their internal table lists are up-to-date.
            m_oCSDOConnection.ADOConnection.Close
            m_oCSDOConnection.ADOConnection.Open
            m_dropTempTables = True
        End If
    End If
       
    ' Replace the <CHEMLINK...> place holders with the temp Field and Table
    tempStr = Replace(RelSql, replaceString, replacementStr1)
    tempStr = Replace(tempStr, replaceString2, replacementStr2)
    
    AddChemHitlistCriteria = tempStr
End Function

'!DGB! 4/18/01 New function to Read hitlist from DB table
'TODO Check Oracle syntax and test!
Private Function GetHitlist() As MolServer17.Hitlist
    Dim conn As ADODB.Connection
    Dim cmd As ADODB.Command
    Dim RS As ADODB.Recordset
    Dim oHitlist As MolServer17.Hitlist
    Dim AdminDSN As String
    Dim ADOOpts As ADOOptions
    Dim sHitlist As String
    Dim sql As String
    Dim hitlistID As Long
    Dim sTranslateTable As String
    Dim sTranslateStrucKey As String
    Dim sTranslateRelKey As String
    
    Set ADOOpts = m_oCSDOConnection.ADOOptions
    
    sTranslateTable = m_oCSDOConnection.ChemSearchOptions.hitlistTranslateTable
    sTranslateStrucKey = m_oCSDOConnection.ChemSearchOptions.hitlistTranslateStrucKey
    sTranslateRelKey = m_oCSDOConnection.ChemSearchOptions.hitlistTranslateRelKey
    hitlistID = m_oCSDOConnection.ChemSearchOptions.SearchDomain
    
    If hitlistID > 0 Then
        If Not sTranslateTable = "" Then
            ' Translate hitlist contents to MOL_IDs
            'sql = "SELECT DISTINCT " & sTranslateTable & "." & sTranslateStrucKey & " AS ID FROM " & sTranslateTable & ", CSDOhitlist WHERE hitlistID=" & hitlistID & " AND CSDOhitlist.ID=" & sTranslateTable & "." & sTranslateRelKey
            sql = sTranslateTable
        Else
            Select Case ADOOpts.StoreType
                Case kSTOracle
                    sql = "SELECT DISTINCT ID FROM CSDOhitlist WHERE hitlistID=" & hitlistID
                Case Else
                    sql = "SELECT DISTINCT ID FROM CSDOhitlist WHERE hitlistID=" & hitlistID
            End Select
        End If
        ' Fetch the Hitlist into a recordset
        Set conn = New ADODB.Connection
        AdminDSN = ADOOpts.GetAdminADOConnString
        conn.Open AdminDSN
        Set RS = conn.Execute(sql, , adCmdText)
        ' Turn the recordset into a string
        If Not (RS.BOF And RS.EOF) Then
            sHitlist = RS.GetString(, , ",", ",", "0")
        Else
            sHitlist = ""
        End If
    Else
        sHitlist = ""
    End If
    
    ' Parse the hitlist string into the hitlist object
    Set oHitlist = New MolServer17.Hitlist
    oHitlist.Parse (sHitlist)
    Debug.Print "Read " & oHitlist.count & " hits from the current list."
    Set GetHitlist = oHitlist
End Function


Private Function SearchInfoFromQueryFields(aQueryFields As CSFields) As MolServer17.searchInfo
    ' return a search info object
    Dim returnInfo As MolServer17.searchInfo
    Dim tempMol As MolServer17.Molecule
    Dim myField As CSField
    Dim ADOOpts As ADOOptions
    Set ADOOpts = m_oCSDOConnection.ADOOptions
    
    Set returnInfo = CreateObject("MolServer17.searchInfo")
    
    '!DGB! 04/18/01 Add Support for searching over hitlist
    If m_oCSDOConnection.ChemSearchOptions.SearchDomain > 0 Then
        returnInfo.Domain = GetHitlist()
    End If
     '!LJB! 10/2/01 support for setting molserver maxhits. doesn't seem to do a whole lot...
    If m_oCSDOConnection.ChemSearchOptions.molServer_MaxHits > 0 Then
        returnInfo.maxhits = m_oCSDOConnection.ChemSearchOptions.molServer_MaxHits
    End If
    
    For Each myField In aQueryFields
        Select Case myField.fieldtype
            Case kCSFDMolWt
                returnInfo.MolwtQuery = myField.Comparator & myField.Value
            Case kCSFDFormula
                returnInfo.FmlaQuery = myField.Value
            Case kCSFDStructFile, kCSFDBase64Cdx, kCSFDStructure
                Set tempMol = New MolServer17.Molecule
                tempMol.Read myField.StrucPath
                Set returnInfo.MolQuery = tempMol
                Select Case myField.StrucSearchType
                    Case kCSSSTExact
                        returnInfo.FullStructure = True
                    Case kCSSSTSub
                        returnInfo.FullStructure = False
                    Case kCSSSTSim
                     '!LJB! 10/2/01 support for switching between fullstructure= true and flse for sim searching

                        If m_oCSDOConnection.ChemSearchOptions.SimSearchFullStructure = False Then
                            returnInfo.FullStructure = False
                        Else
                            returnInfo.FullStructure = True
                        End If
                        returnInfo.Similarity = True
                        returnInfo.SimThreshold = myField.Value
                End Select
        End Select
    Next
    
    Set SearchInfoFromQueryFields = returnInfo
        
End Function




Private Function TableExists(aTableName As String) As Boolean
    Dim ADOOpts As ADOOptions
    Dim conn As ADODB.Connection
    Dim AdminDSN As String
    Dim SysTableName As String
    Dim sql As String
    Dim lRecsAffected As Long
    Dim RS As ADODB.Recordset
    
    Set ADOOpts = m_oCSDOConnection.ADOOptions
    Set conn = New ADODB.Connection
    AdminDSN = ADOOpts.GetAdminADOConnString
    conn.Open AdminDSN
    
    '!DGB! 04/20/01 Fixed Oracle syntax
    'LJB 4/22 ugly but reliable way to determine if a table exist in access and sql server. MSysObjects requries to much pre configuration of access files to be useful
        
        sql = "SELECT ID FROM " & aTableName
        On Error Resume Next
        conn.Execute (sql)
            If Err.Number <> 0 Then
                TableExists = False
            Else
                TableExists = True
            End If
        On Error GoTo 0
    conn.Close
    Set conn = Nothing
End Function



' Private Functions

Private Function RaiseError(asProc As String) As Boolean
    ' this function is essentially unchanged in every class
    ' log the error to the event log and raise it
    Dim ErrSrc As String
    Dim ErrProc As String
    ErrProc = "Request::" & asProc
    App.LogEvent Err.Description & " in " & ErrProc, vbLogEventTypeError
    If Err.Source <> App.Title Then
        ErrSrc = Err.Source
    Else
        ErrSrc = ErrProc
    End If
    If Err.Number < 0 Then
        Err.Raise Err.Number, ErrSrc, Err.Description
    Else
        Err.Raise vbObjectError + Err.Number, ErrSrc, Err.Description
    End If
End Function

Private Function Base64CdxFromMolecule(ByRef mol As MolServer17.Molecule, tempPath As String)
    mol.Write tempPath
    Base64CdxFromMolecule = Encode(tempPath)
End Function



