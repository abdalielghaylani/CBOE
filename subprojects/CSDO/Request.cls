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

' private objects
Private m_ReturnRS As ADODB.Recordset     ' recordset returned
Attribute m_ReturnRS.VB_VarHelpID = -1
Private m_RelRS As ADODB.Recordset       ' relational recordset
Attribute m_RelRS.VB_VarHelpID = -1
Private m_oCSDOConnection As CSDO.Connection      ' parent connection
Private m_Command As CSDO.CSCommand                ' Command
Private m_Hitlists() As MolServer.hitlist           ' Hitlists from searches
Private m_HitlistRelTables() As String              ' Hitlist RelTable Names
Private m_ChemSearch() As MolServer.Search          ' array of MolServer searches
Private m_ChemSearchInfo() As MolServer.searchInfo  ' array of MolServer search infos
Private m_dropTempTables As Boolean                 ' do we need to drop any temp tables?

Public Property Get Status() As RequestStatus
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

Friend Property Set CSDOConnection(aCSDOConnection As CSDO.Connection)
    Set m_oCSDOConnection = aCSDOConnection
End Property

Friend Property Set Command(aCSCommand As CSDO.CSCommand)
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
    
    Set conn = m_oCSDOConnection.ADOConnection

    With m_Command
        If .HasChemReturnFields = False And .HasChemQueryFields = False Then
            ' rel - only search
            ' use disconnected recordset
            Set m_ReturnRS = Nothing
            Set m_ReturnRS = New ADODB.Recordset
            m_ReturnRS.CursorLocation = adUseClient
            m_ReturnRS.MaxRecords = m_oCSDOConnection.ADOOptions.MaxHits 'enables get more option for non Access DB
            m_ReturnRS.Open .RelSql, conn, adOpenStatic, adLockReadOnly
            Set m_ReturnRS.ActiveConnection = Nothing
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
            m_RelRS.MaxRecords = m_oCSDOConnection.ADOOptions.MaxHits 'enables get more option for non Access DB
            
            m_RelRS.Open RelSql, conn, adOpenStatic, adLockReadOnly
            
            Set m_RelRS.ActiveConnection = Nothing
            ' drop temp tables created during operation, if necessary
            If m_dropTempTables Then
                DropTempTables
            End If
            ' merge chemical data from all chemical searches
            If .HasChemReturnFields Then
                AddChemInfo
            Else
                Set m_ReturnRS = m_RelRS
            End If
        End If
    End With
    
    Set conn = Nothing
End Sub

Private Sub DropTempTables()
    Dim conn As ADODB.Connection
    Dim cmd As ADODB.Command
    Dim ADOOpts As ADOOptions
    Dim tempTableName As String
    Dim AdminDSN As String
    
    Set conn = New ADODB.Connection
    Set ADOOpts = m_oCSDOConnection.ADOOptions
    tempTableName = ADOOpts.TempTableBaseName & ADOOpts.UniqueSessID
    AdminDSN = ADOOpts.GetAdminADOConnString
    
    conn.Open AdminDSN
    Set cmd = New ADODB.Command
    
    With cmd
        ' complains if table doesn't already exist
        Set .ActiveConnection = conn
        .CommandType = adCmdText
        .CommandText = "drop table " & tempTableName
        On Error Resume Next
        .Execute
        On Error GoTo 0
    End With
    
    Set cmd = Nothing
    Set conn = Nothing
    m_dropTempTables = False

End Sub

Private Sub Do_Insert()
    Dim conn As ADODB.Connection
    Dim md As MolServer.Document
    Dim mol As MolServer.molecule
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
            Set mol = New MolServer.molecule
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
    Dim md As MolServer.Document
    Dim hl As MolServer.hitlist
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
    Dim md As MolServer.Document
    Dim mol As MolServer.molecule
    Dim hl As MolServer.hitlist
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
            Set mol = New MolServer.molecule
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
                Set mol = New MolServer.molecule
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
    Dim myMolDocument As MolServer.Document
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
            Do While m_ChemSearch(count - 1).Status = 1
                ' wait for search to complete
                m_NumChemExamined = m_ChemSearch(count - 1).NSeen
                m_NumChemHits = m_ChemSearch(count - 1).CurID
            Loop
            
            Set m_Hitlists(count - 1) = m_ChemSearch(count - 1).hitlist
            m_HitlistRelTables(count - 1) = myLink.RelTable
            
            If bUpdateRelSql Or bUpdateRelSelectSql Then
                retString = AddChemHitlistCriteria(retString, m_Hitlists(count - 1), myLink)
            End If

            count = count + 1
        End If
    Next
    DoChemicalSearch = retString
End Function

Private Function ChemHitlistFromRS(aRS As ADODB.Recordset, molField As String) As MolServer.hitlist
    Dim rHL As MolServer.hitlist
    Dim myMolDocument As MolServer.Document
    Dim i As Long
    
    Set rHL = New MolServer.hitlist
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

Private Function RSFromChemHitlist(aHitlist As MolServer.hitlist, aReturnFields As CSFields) As ADODB.Recordset
    ' create an ADO Recordset from a MolServer hitlist
    Dim returnRS As ADODB.Recordset
    Dim count As Long
    Dim idAtCount As Long
    ' Dim needMol As Boolean
    Dim tempMol As MolServer.molecule
    Dim theValue As Variant
    Dim myFields As CSFields
    Dim myField As CSField
    Dim myMolDocument As MolServer.Document
    
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
    Dim tempMol As MolServer.molecule
    Dim theValue As Variant
    Dim myMolDocument As MolServer.Document
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

Private Function AddChemHitlistCriteria(RelSql As String, aHitlist As MolServer.hitlist, aChemLink As CSChemLink) As String
    ' adds the chemical hitlist criteria to an existing rel query
    Dim i As Long
    Dim tempStr As String
    Dim replaceString As String
    Dim actualjoinmethod As Long
    Dim ADOOpts As ADOOptions
    Dim conn As ADODB.Connection
    
    Set ADOOpts = m_oCSDOConnection.ADOOptions
    replaceString = "<CHEMLINK_" & aChemLink.RelTable & ">"
    
    If aHitlist.count = 0 Then
        actualjoinmethod = -1
    Else
        actualjoinmethod = ADOOpts.GetJoinMethod(aHitlist.count)
    End If
    
    Select Case actualjoinmethod
        Case -1
            tempStr = "-1"
        Case kJMChemFirstMOLIDString
         
            For i = 0 To aHitlist.count - 1
                If i = 0 Then
                    tempStr = aHitlist.At(i)
                Else
                    tempStr = tempStr & "," & aHitlist.At(i)
                End If
            Next
        
        Case kJMChemFirstTempTable
            Dim cmd As ADODB.Command
            Dim tempTableName As String
            tempTableName = ADOOpts.TempTableBaseName & ADOOpts.UniqueSessID
            
            Set conn = New ADODB.Connection
            Dim AdminDSN As String
            AdminDSN = ADOOpts.GetAdminADOConnString
            
            conn.Open AdminDSN
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
                        .CommandText = "create table " & tempTableName & " (ID NUMBER)"
                End Select
                .Execute
                ' fill with ids
                
                Dim numInserts As Long
                Dim j As Long
                Dim offset As Long
                Dim lastNum As Long
                Dim chunkSize As Long
                Dim tempStr2 As String
               chunkSize = ADOOpts.SmallToLargeJoinThreshold
                numInserts = aHitlist.count / chunkSize
                For j = 0 To numInserts
                    offset = j * chunkSize
                    If offset + chunkSize > aHitlist.count - 1 Then
                        lastNum = aHitlist.count - 1
                    Else
                        lastNum = offset + chunkSize - 1
                    End If
                    For i = offset To lastNum
                        If i = offset Then
                            tempStr2 = aHitlist.At(i)
                        Else
                            tempStr2 = tempStr2 & "," & aHitlist.At(i)
                        End If
                    Next
                    .CommandText = "INSERT INTO " & tempTableName & _
                                   " SELECT " & aChemLink.RelField & " FROM " & _
                                   aChemLink.RelTable & " WHERE " & _
                                   aChemLink.RelField & " IN " & "(" & tempStr2 & ")"
                    .Execute
                Next
            
            ' create index
                .CommandText = "create index ID_INDX" & tempTableName & " on " & tempTableName & " (ID)"
                .Execute
            End With
            
            Select Case ADOOpts.StoreType
                Case kSTOracle
                    tempStr = "SELECT ID FROM " & ADOOpts.AdminUID & "." & tempTableName
                Case Else
                    tempStr = "SELECT ID FROM " & tempTableName
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
            TempTable = ADOOpts.TempTableBaseName & ADOOpts.UniqueSessID
            aHitlist.Write ExportIDsPath
            AdminDSN = ADOOpts.GetAdminADOConnString
            Dim retVal As Boolean
            Set conn = New ADODB.Connection
            conn.Open AdminDSN
            retVal = RunSQLLdr(conn:=conn, _
                                   uname:=ADOOpts.AdminUID, _
                                   pwd:=ADOOpts.AdminPWD, _
                                   tempTableName:=TempTable, _
                                   batchPath:=ExportBatchPath, _
                                   sqlLdrPath:=SQLLoaderPath, _
                                   ServiceName:=ORAServiceName, _
                                   ExportIDsPath:=ExportIDsPath, _
                                   ExportCtlPath:=ExportCtlPath, _
                                   logPath:=ExportLogPath)
                               
            If retVal = True Then
                 tempStr = "SELECT ID FROM " & ADOOpts.AdminUID & "." & TempTable
            End If
            
    End Select
       
    If (Not conn Is Nothing) Then
        ' Explicitely close the connection to flush its table operations.
        conn.Close
        
        ' Since you have created a temporary table, any cached connection to the database
        ' may have invalid table caches. These connections will need to be closed, then re-opened
        ' so that their internal table lists are up-to-date.
        m_oCSDOConnection.ADOConnection.Close
        m_oCSDOConnection.ADOConnection.Open
        
        m_dropTempTables = True
    End If
    
    AddChemHitlistCriteria = Replace(RelSql, replaceString, tempStr)

End Function

Private Function SearchInfoFromQueryFields(aQueryFields As CSFields) As MolServer.searchInfo
    ' return a search info object
    Dim returnInfo As MolServer.searchInfo
    Dim tempMol As MolServer.molecule
    Dim myField As CSField
    
    Set returnInfo = New MolServer.searchInfo
    
    For Each myField In aQueryFields
        Select Case myField.fieldtype
            Case kCSFDMolWt
                returnInfo.MolwtQuery = myField.Comparator & myField.Value
            Case kCSFDFormula
                returnInfo.FmlaQuery = myField.Value
            Case kCSFDStructFile, kCSFDBase64Cdx, kCSFDStructure
                Set tempMol = New MolServer.molecule
                tempMol.Read myField.StrucPath
                Set returnInfo.MolQuery = tempMol
                Select Case myField.StrucSearchType
                    Case kCSSSTExact
                        returnInfo.FullStructure = True
                    Case kCSSSTSub
                        returnInfo.FullStructure = False
                    Case kCSSSTSim
                        returnInfo.FullStructure = False
                        returnInfo.Similarity = True
                        returnInfo.SimThreshold = myField.Value
                End Select
        End Select
    Next
    
    Set SearchInfoFromQueryFields = returnInfo
        
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

Private Function Base64CdxFromMolecule(ByRef mol As MolServer.molecule, tempPath As String)
    mol.Write tempPath
    Base64CdxFromMolecule = Encode(tempPath)
End Function



