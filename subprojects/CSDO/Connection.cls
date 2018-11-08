VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Connection"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

' Contained Objects

Private m_ADOConnString As String
Private m_ADOUID As String
Private m_ADOPWD As String
Private m_ADOConnOptions As Long

Private m_oAdoConnection As ADODB.Connection        ' our relational data connection
Private m_oCSChemLinks As CSChemLinks       ' collection of chemical data connections
Private m_oMolDocument As MolServer.Document  ' one or more molserver documents
Private m_oChemSearchOptions As CSDO.ChemSearchOptions  ' chemsearch options
Private m_oADOOptions As CSDO.ADOOptions  ' ado join options

' Property Let/Get

Public Property Let ADOConnString(aStr As String)
    m_ADOConnString = aStr
    m_oADOOptions.BaseADOConnStr = m_ADOConnString
End Property

Public Property Get ADOConnString() As String
    ADOConnString = m_ADOConnString
End Property

Public Property Let ADOUID(aStr As String)
    m_ADOUID = aStr
End Property

Public Property Let ADOPWD(aStr As String)
    m_ADOPWD = aStr
End Property

Public Property Let ADOConnOptions(aLng As Long)
    ADOConnOptions = aLng
End Property

Public Property Get ADOOptions() As ADOOptions
    Set ADOOptions = m_oADOOptions
End Property

Public Property Get ChemSearchOptions() As ChemSearchOptions
    Set ChemSearchOptions = m_oChemSearchOptions
End Property

Public Property Get ADOConnection() As ADODB.Connection
    Set ADOConnection = m_oAdoConnection
End Property

Public Property Get CSChemLinks() As CSChemLinks
    Set CSChemLinks = m_oCSChemLinks
End Property

Public Property Set CSChemLinks(aCSChemLinks As CSChemLinks)
    Set m_oCSChemLinks = aCSChemLinks
End Property

Friend Property Get MolDocument() As MolServer.Document
    Set MolDocument = m_oMolDocument
End Property

' Class Initialize / Terminate

Private Sub Class_Initialize()
    Set m_oAdoConnection = New ADODB.Connection
    Set m_oChemSearchOptions = New CSDO.ChemSearchOptions
    Set m_oCSChemLinks = New CSDO.CSChemLinks
    Set m_oADOOptions = New CSDO.ADOOptions
End Sub

Private Sub Class_Terminate()
    If m_oAdoConnection.State <> adStateClosed Then
        m_oAdoConnection.Close
    End If
    Set m_oAdoConnection = Nothing
    If m_oMolDocument.IsOpen Then
        m_oMolDocument.Close
    End If
    Set m_oMolDocument = Nothing
    Set m_oCSChemLinks = Nothing
    Set m_oChemSearchOptions = Nothing
    Set m_oADOOptions = Nothing
End Sub

' Public Functions

Public Function Execute(ByVal CommandText As String, _
                        Optional RecordsAffected, _
                        Optional Options As Long) As CSDO.Request
    Dim returnRequest As CSDO.Request
    Dim mySearchInfo As MolServer.searchInfo
    Dim myCSCommand As CSCommand
    Dim myChemLink As CSChemLink
    Dim aField As CSField
    Dim count As Integer
    
On Error GoTo ErrorHandler
    
    Set returnRequest = New CSDO.Request
    Set returnRequest.CSDOConnection = Me
    
    Set myCSCommand = New CSCommand
    
    ' clear out any old criteria from the chemlinks
    m_oCSChemLinks.ClearFields
    
    Set myCSCommand.ChemLinks = m_oCSChemLinks
    myCSCommand.CreateFromSQL CommandText
            
    Set returnRequest.Command = myCSCommand
    
    Set Execute = returnRequest
    Exit Function
    
ErrorHandler:
    RaiseError "Execute"
    
End Function

Public Function PersistToXML() As String
    Dim cl As CSChemLink

    ' returns connection data as xml string
    Dim retVal As String
    retVal = VersionHeader
    retVal = retVal & TopLevelTagOpen("CSDO.Connection")
    retVal = retVal & TaggedItem("ADOConnString", m_ADOConnString)
    retVal = retVal & TaggedItem("ADOUID", m_ADOUID)
    retVal = retVal & TaggedItem("ADOPWD", m_ADOPWD)
    retVal = retVal & TaggedItem("ADOConnOptions", CStr(m_ADOConnOptions))
    retVal = retVal & m_oADOOptions.PersistToXML
    retVal = retVal & m_oChemSearchOptions.PersistToXML
    For Each cl In m_oCSChemLinks
        retVal = retVal & cl.PersistToXML
    Next
    retVal = retVal & TopLevelTagClose("CSDOConnection")
    PersistToXML = retVal
End Function

Public Function CreateFromXML(strDoc As String) As Boolean
    Dim temp As String

    m_ADOConnString = GetNextTaggedItem(strDoc, "ADOConnString")
    m_oADOOptions.BaseADOConnStr = m_ADOConnString
    m_ADOUID = GetNextTaggedItem(strDoc, "ADOUID")
    m_ADOPWD = GetNextTaggedItem(strDoc, "ADOPWD")
    m_ADOConnOptions = CLng(GetNextTaggedItem(strDoc, "ADOConnOptions"))
    m_oADOOptions.CreateFromXML GetNextTopLevelTaggedItem(strDoc, "CSDO.ADOOptions")
    m_oChemSearchOptions.CreateFromXML GetNextTopLevelTaggedItem(strDoc, "CSDO.ChemSearchOptions")
    
    Dim l As Long
    l = 1
    Do Until l = 0
        temp = GetNextTopLevelTaggedItem(strDoc, "CSDO.CSChemLink", l)
        If l <> 0 Then
            m_oCSChemLinks.AddFromXML temp
        End If
    Loop
    
    CreateFromXML = True
End Function

Public Sub OpenConn(Optional ByVal ConnectionString As String = "", _
                    Optional ByVal UserID As String = "", _
                    Optional ByVal Password As String = "", _
                    Optional ByVal Options As Long = 0)
    Dim cl As CSChemLink
    
On Error GoTo HandleError
    
    If ConnectionString <> "" Then m_ADOConnString = ConnectionString
    If UserID <> "" Then m_ADOUID = UserID
    If Password <> "" Then m_ADOPWD = Password
    If Options <> 0 Then m_ADOConnOptions = Options
    
    m_oAdoConnection.Open m_ADOConnString, m_ADOUID, m_ADOPWD, m_ADOConnOptions
    m_oADOOptions.BaseADOConnStr = m_ADOConnString
    
    Set m_oMolDocument = New MolServer.Document
    
    Set cl = m_oCSChemLinks.Item(1)
    m_oMolDocument.Open cl.MolConnString, cl.MolOptions, cl.MolPassword
    
    Exit Sub
    
HandleError:
    RaiseError "OpenConn"
    
End Sub

' Private Functions

Private Function RaiseError(asProc As String) As Boolean
    ' this function is essentially unchanged in every class
    ' log the error to the event log and raise it
    Dim ErrSrc As String
    Dim ErrProc As String
    ErrProc = "CSDO.Connection::" & asProc
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


