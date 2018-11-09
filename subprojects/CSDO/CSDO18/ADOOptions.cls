VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ADOOptions"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

' ADOOptions

' a structure of ADO options
     
Public Enum StoreTypes
    kSTUnknown
    kSTAccess
    kSTOracle
    kSTSQLServer
End Enum

Public Enum JoinMethods
    kJMChooseBest
    kJMChemFirstMOLIDString
    kJMChemFirstTempTable
    kJMChemFirstTempTableUsingORASQLLDR
    kJMRelFirst
End Enum

Private m_StoreType As StoreTypes
Private m_DirectKeywordType As Long
Private m_SmallJoinMethod As JoinMethods
Private m_LargeJoinMethod As JoinMethods
Private m_SmallToLargeJoinThreshold As Long        ' at what number does a join become "big"
Private m_ScratchDir As String
Private m_UniqueSessID As String
Private m_AdminUID As String
Private m_AdminPWD As String
Private m_AdminConnStr As String
Private m_TempTableBaseName As String
Private m_ORASQLLDRPath As String
Private m_BaseADOConnStr As String
Private m_MaxHits As Long
Private m_ServiceName As String



Public Property Get AdminConnStr() As String
    AdminConnStr = m_AdminConnStr
End Property

Public Property Get TempTableBaseName() As String
    TempTableBaseName = m_TempTableBaseName
End Property

Friend Property Let BaseADOConnStr(aStr As String)
    m_BaseADOConnStr = aStr
End Property

Public Property Get ORASQLLDRPath() As String
    ORASQLLDRPath = m_ORASQLLDRPath
End Property
Public Property Let ORASQLLDRPath(aStr As String)
    m_ORASQLLDRPath = aStr
End Property
Public Property Let ServiceName(aStr As String)
    m_ServiceName = aStr
End Property
Public Property Get ServiceName() As String
    ServiceName = m_ServiceName
End Property



Public Property Get AdminUID() As String
    AdminUID = m_AdminUID
End Property

Public Property Let AdminUID(ByVal aStr As String)
    m_AdminUID = aStr
End Property

Public Property Let AdminPWD(ByVal aStr As String)
    m_AdminPWD = aStr
End Property

Public Property Get AdminPWD() As String
    AdminPWD = m_AdminPWD
End Property

Public Property Get StoreType() As StoreTypes
    StoreType = m_StoreType
End Property
Public Property Get DirectKeywordType() As Long
    DirectKeywordType = m_DirectKeywordType
End Property
Public Property Let DirectKeywordType(ByVal aLng As Long)
    m_DirectKeywordType = aLng
End Property
Public Property Get SmallJoinMethod() As JoinMethods
    SmallJoinMethod = m_SmallJoinMethod
End Property

Public Property Get LargeJoinMethod() As JoinMethods
    LargeJoinMethod = m_LargeJoinMethod
End Property

Public Property Get SmallToLargeJoinThreshold()
    SmallToLargeJoinThreshold = m_SmallToLargeJoinThreshold
End Property

Public Property Get ScratchDir() As String
    ScratchDir = m_ScratchDir
End Property

Public Property Let ScratchDir(ByVal aStr As String)
    m_ScratchDir = aStr
End Property

Public Property Get UniqueSessID() As String
    UniqueSessID = m_UniqueSessID
End Property

Public Property Let UniqueSessID(ByVal aStr As String)
    m_UniqueSessID = aStr
End Property
Public Property Get maxhits() As Long
    maxhits = m_MaxHits
End Property

Public Property Let maxhits(ByVal aLng As Long)
    m_MaxHits = aLng
End Property


Public Sub Fill(Optional ByVal StoreType As Long = kSTUnknown, _
                Optional ByVal SmallJoinMethod As Long = kJMChooseBest, _
                Optional ByVal LargeJoinMethod As Long = kJMChooseBest, _
                Optional ByVal SmallToLargeJoinThreshold As Long = 1000, _
                Optional ByVal ScratchDir As String = "C:\Temp\", _
                Optional ByVal UniqueSessID As String = "", _
                Optional ByVal AdminConnStr As String = "", _
                Optional ByVal AdminUID As String = "", _
                Optional ByVal AdminPWD As String = "", _
                Optional ByVal TempTableBaseName As String = "CS_TEMP", _
                Optional ByVal ORASQLLDRPath As String = "", _
                Optional ByVal maxhits As Long = 0, _
                Optional ByVal ServiceName As String = "")
               
    m_StoreType = StoreType
    m_SmallJoinMethod = SmallJoinMethod
    m_LargeJoinMethod = LargeJoinMethod
    m_SmallToLargeJoinThreshold = SmallToLargeJoinThreshold
    m_ScratchDir = ScratchDir
    m_UniqueSessID = UniqueSessID
    m_AdminConnStr = AdminConnStr
    m_AdminUID = AdminUID
    m_AdminPWD = AdminPWD
    m_TempTableBaseName = TempTableBaseName
    m_ORASQLLDRPath = ORASQLLDRPath
    m_MaxHits = maxhits
    m_ServiceName = ServiceName
 
End Sub

Public Function PersistToXML() As String
    Dim retVal As String
    retVal = TopLevelTagOpen("CSDO18.ADOOptions")
    retVal = retVal & TaggedItem("StoreType", CStr(m_StoreType))
    retVal = retVal & TaggedItem("SmallJoinMethod", CStr(m_SmallJoinMethod))
    retVal = retVal & TaggedItem("LargeJoinMethod", CStr(m_LargeJoinMethod))
    retVal = retVal & TaggedItem("SmallToLargeJoinThreshold", CStr(m_SmallToLargeJoinThreshold))
    retVal = retVal & TaggedItem("ScratchDir", m_ScratchDir)
    retVal = retVal & TaggedItem("AdminUID", m_AdminUID)
    retVal = retVal & TaggedItem("AdminPWD", m_AdminPWD)
    retVal = retVal & TaggedItem("AdminConnStr", m_AdminConnStr)
    retVal = retVal & TaggedItem("TempTableBaseName", m_TempTableBaseName)
    retVal = retVal & TaggedItem("ORASQLLDRPath", m_ORASQLLDRPath)
    retVal = retVal & TaggedItem("MaxHits", CStr(m_MaxHits))
    retVal = retVal & TaggedItem("ServiceName", m_ServiceName)
    retVal = retVal & TopLevelTagClose("CSDO18.ADOOptions")
    PersistToXML = retVal
End Function

Public Function CreateFromXML(strDoc As String) As Boolean
    m_StoreType = CLng(GetNextTaggedItem(strDoc, "StoreType"))
    m_SmallJoinMethod = CLng(GetNextTaggedItem(strDoc, "SmallJoinMethod"))
    m_LargeJoinMethod = CLng(GetNextTaggedItem(strDoc, "LargeJoinMethod"))
    m_SmallToLargeJoinThreshold = CLng(GetNextTaggedItem(strDoc, "SmallToLargeJoinThreshold"))
    m_ScratchDir = GetNextTaggedItem(strDoc, "ScratchDir")
    m_AdminUID = GetNextTaggedItem(strDoc, "AdminUID")
    m_AdminPWD = GetNextTaggedItem(strDoc, "AdminPWD")
    m_AdminConnStr = GetNextTaggedItem(strDoc, "AdminConnStr")
    m_TempTableBaseName = GetNextTaggedItem(strDoc, "TempTableBaseName")
    m_ORASQLLDRPath = GetNextTaggedItem(strDoc, "ORASQLLDRPath")
    m_MaxHits = CLng(GetNextTaggedItem(strDoc, "MaxHits"))
    m_ServiceName = GetNextTaggedItem(strDoc, "ServiceName")


    CreateFromXML = True
End Function

Friend Function GetAdminADOConnString() As String
    ' returns admin ado string for use in making
    ' temp tables and the like
    '!SAG! 06/04 Replaced Instr and Replace functions with their equivalent case insensitive onces
    '!SAG! 10/04/04 Safer Replace ... the old replace would replace the user or pwd string from the DB name too
    '                  e.g. ...Initial Catalog = 'CHEMINDB2Test' would become Initial Catalog = 'CHEMINDB2' for a username = 'test'
    '   The following lines:
    '                existADO = Replace(existADO, Mid(existADO, a + 4, b - (a + 4)), "", 1, -1, vbTextCompare)
    '                existADO = Replace(existADO, Mid(existADO, a + 4), "", 1, -1, vbTextCompare)
    '   have been changed to:
    '                existADO = Replace(existADO, Mid(existADO, a, b - a), "UID=", 1, -1, vbTextCompare)
    '                existADO = Replace(existADO, Mid(existADO, a), "UID=", 1, -1, vbTextCompare)
    
    If m_AdminConnStr <> "" Then
        GetAdminADOConnString = m_AdminConnStr
    Else
        Dim existADO As String
        existADO = m_BaseADOConnStr
        If existADO <> "" Then
            ' if there are existing uid and password in
            ' the string, cut them out and replace them
            Dim a As Long
            Dim b As Long
            
                      
            a = InStr(1, existADO, "UID", vbTextCompare)
            If a <> 0 Then
                b = InStr(a, existADO, ";")
                If b <> 0 Then
                    existADO = Replace(existADO, Mid(existADO, a, b - a), "UID=", 1, -1, vbTextCompare)
                Else
                    ' b = 0 i.e. no ';' at end of DSN string
                    existADO = Replace(existADO, Mid(existADO, a), "UID=", 1, -1, vbTextCompare)
                End If
                existADO = Replace(existADO, "UID=", "UID=" & m_AdminUID, 1, -1, vbTextCompare)
            End If
    
            '!DGB! 01/02 Handle the syntax of an OLEDB connection string
            a = InStr(1, existADO, "USER ID", vbTextCompare)
            If a <> 0 Then
                b = InStr(a, existADO, ";")
                If b <> 0 Then
                    existADO = Replace(existADO, Mid(existADO, a, b - a), "USER ID=", 1, -1, vbTextCompare)
                Else
                    ' b = 0 i.e. no ';' at end of DSN string
                    existADO = Replace(existADO, Mid(existADO, a), "USER ID=", 1, -1, vbTextCompare)
                End If
                existADO = Replace(existADO, "USER ID=", "USER ID=" & m_AdminUID, 1, -1, vbTextCompare)
            End If
            
            a = InStr(1, existADO, "PWD", vbTextCompare)
            If a <> 0 Then
                b = InStr(a, existADO, ";")
                If b <> 0 Then
                    existADO = Replace(existADO, Mid(existADO, a, b - a), "PWD=", 1, -1, vbTextCompare)
                Else
                    ' b = 0 i.e. no ';' at end of DSN string
                    existADO = Replace(existADO, Mid(existADO, a), "PWD=", 1, -1, vbTextCompare)
                End If
                existADO = Replace(existADO, "PWD=", "PWD=" & m_AdminPWD, 1, -1, vbTextCompare)
            End If
            
            '!DGB! 01/02 Handle the syntax of an OLEDB connection string
            
            'existADO = Replace(existADO, " ", "")
            a = InStr(1, existADO, "PASSWORD=", vbTextCompare)
            If a <> 0 Then
                b = InStr(a, existADO, ";")
                If b <> 0 Then
                    existADO = Replace(existADO, Mid(existADO, a, b - a), "PASSWORD=", 1, -1, vbTextCompare)
                Else
                    ' b = 0 i.e. no ';' at end of DSN string
                    existADO = Replace(existADO, Mid(existADO, a), "PASSWORD=", 1, -1, vbTextCompare)
                End If
                existADO = Replace(existADO, "PASSWORD=", "PASSWORD=" & m_AdminPWD, 1, -1, vbTextCompare)
            End If
        End If
        GetAdminADOConnString = existADO
    End If
End Function
                    
Friend Function GetJoinMethod(numHits As Long)
    ' returns the join method to use
    Dim retVal As JoinMethods
    
    If numHits < m_SmallToLargeJoinThreshold Then
        ' determine small join method to return
        If m_SmallJoinMethod <> kJMChooseBest Then
            retVal = m_SmallJoinMethod
        Else
            retVal = kJMChemFirstMOLIDString
        End If
    Else
        ' determine large join method to return
        If m_LargeJoinMethod <> kJMChooseBest Then
            retVal = m_LargeJoinMethod
        Else
            Select Case m_StoreType
                Case kSTOracle
                    If m_ORASQLLDRPath <> "" Then
                        retVal = kJMChemFirstTempTableUsingORASQLLDR
                    Else
                        retVal = kJMChemFirstTempTable
                    End If
                Case Else
                    retVal = kJMChemFirstTempTable
            End Select
        End If
    End If
    GetJoinMethod = retVal
End Function
                    
Private Sub Class_Initialize()
    ' init structure with default values
    m_StoreType = kSTAccess
    m_SmallJoinMethod = kJMChooseBest
    m_LargeJoinMethod = kJMChooseBest
    m_SmallToLargeJoinThreshold = 1000
    m_ScratchDir = GetTmpPath
    m_TempTableBaseName = "CS_TEMP"
    m_UniqueSessID = ""
    m_AdminConnStr = ""
    m_AdminUID = ""
    m_AdminPWD = ""
    m_ORASQLLDRPath = ""
    m_MaxHits = 0
    m_DirectKeywordType = 1
End Sub


