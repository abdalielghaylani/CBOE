VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CSChemLink"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
    Option Explicit

' represents a mapping between a relational and chemical data source

Private m_RelTable As String                    ' must map to a table in the rel data source
Private m_RelField As String                    ' field that maps to MOL_ID
Private m_UniqueRelFieldAlias As String         ' unique rel field alias
Private m_MolConnString As String               ' connection string that opens an appropriate molserver
Private m_MolPassword As String                 ' password, if any
Private m_MolOptions As MolServer19.MSOpenModes   ' open mode

Private m_QueryFields As CSFields               ' Query Fields for a request
Private m_ReturnFields As CSFields              ' return fields for a request

Friend Property Get QueryFields() As CSFields
    Set QueryFields = m_QueryFields
End Property

Friend Property Get ReturnFields() As CSFields
    Set ReturnFields = m_ReturnFields
End Property

Public Property Get RelTable() As String
    RelTable = m_RelTable
End Property

Public Property Let RelTable(aRelTable As String)
    m_RelTable = UCase(aRelTable)
End Property

Public Property Get RelField() As String
    RelField = m_RelField
End Property

Public Property Let RelField(aRelField As String)
    m_RelField = UCase(aRelField)
    Dim temp() As String
    If InStr(m_RelTable, ".") > 0 Then
        temp = Split(m_RelTable, ".", -1)
        m_UniqueRelFieldAlias = m_RelField & "_" & temp(1)
    Else
        m_UniqueRelFieldAlias = m_RelField & "_" & m_RelTable
    End If
End Property

Public Property Get UniqueRelFieldAlias() As String
    UniqueRelFieldAlias = m_UniqueRelFieldAlias
End Property

Public Property Get MolConnString() As String
    MolConnString = m_MolConnString
End Property

Public Property Let MolConnString(aMolConnString As String)
    m_MolConnString = aMolConnString
End Property

Public Property Get MolPassword() As String
    MolPassword = m_MolPassword
End Property

Public Property Let MolPassword(aMolPassword As String)
    m_MolPassword = aMolPassword
End Property

Public Property Get MolOptions() As Long
    MolOptions = m_MolOptions
End Property

Public Property Let MolOptions(aMolOptions As Long)
    m_MolOptions = aMolOptions
End Property

Public Function PersistToXML() As String
    Dim retVal As String
    Dim tempString As String
    
    retVal = TopLevelTagOpen("CSDO19.CSChemLink")
    retVal = retVal & TaggedItem("RelTable", m_RelTable)
    retVal = retVal & TaggedItem("RelField", m_RelField)
    retVal = retVal & TaggedItem("MolConnString", m_MolConnString)
    retVal = retVal & TaggedItem("MolPassword", m_MolPassword)
    retVal = retVal & TaggedItem("MolOptions", CStr(m_MolOptions))
    retVal = retVal & TopLevelTagClose("CSDO19.CSChemLink")
    PersistToXML = retVal
End Function

Private Sub Class_Initialize()
    ' Init objects
    Set m_QueryFields = New CSDO19.CSFields
    Set m_ReturnFields = New CSDO19.CSFields
    ' Set defaults
    m_MolOptions = kMSNormal
    m_MolPassword = ""
    m_RelTable = "MOLTABLE"
    Me.RelField = "MOL_ID"
End Sub

Private Sub Class_Terminate()
    Set m_QueryFields = Nothing
    Set m_ReturnFields = Nothing
End Sub

