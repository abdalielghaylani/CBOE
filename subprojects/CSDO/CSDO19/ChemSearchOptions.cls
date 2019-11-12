VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ChemSearchOptions"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit
'!DGB! 04/18/01 new properties for hitlist management
Private m_hitlistID As Long
Private m_SearchDomain As Long
Private m_hitlistContentsField As String
Private m_hitlistTranslateTable As String
Private m_hitlistTranslateRelKey As String
Private m_hitlistTranslateStrucKey As String

'!LJB! 10/2/01 support for maxhits
Private m_molServer_MaxHits As Long
'!LJB! 10/2/01 support for switching between fullstructure= true and flse for sim searching
Private m_SimSearchFullStructure As Boolean
' CS ChemSearchOptions
Public ExtraFragsOK As Boolean
Public ExtraFragsOKIfRxn As Boolean
Public FindChargedCarbon As Boolean
Public FindChargedHetero As Boolean
Public FragsCanOverlap As Boolean
Public StereoDB As Boolean
Public StereoTetr As Boolean
Public UseRxnCenters As Boolean
Public AbsHitsRel As Boolean
Public RelativeStereo As Boolean
Public IdentitySearch As Boolean

'!DGB! 04/18/01 new properties for hitlist management
Public Property Let hitlistID(ByVal aLng As Long)
    m_hitlistID = aLng
End Property
Public Property Get hitlistID() As Long
    hitlistID = m_hitlistID
End Property
Public Property Let SearchDomain(ByVal aLng As Long)
    m_SearchDomain = aLng
End Property
Public Property Get SearchDomain() As Long
    SearchDomain = m_SearchDomain
End Property
Public Property Let hitlistContentsField(ByVal aStr As String)
    m_hitlistContentsField = aStr
End Property
Public Property Get hitlistContentsField() As String
    hitlistContentsField = m_hitlistContentsField
End Property
Public Property Let hitlistTranslateTable(ByVal aStr As String)
    m_hitlistTranslateTable = aStr
End Property
Public Property Get hitlistTranslateTable() As String
    hitlistTranslateTable = m_hitlistTranslateTable
End Property
Public Property Let hitlistTranslateRelKey(ByVal aStr As String)
    m_hitlistTranslateRelKey = aStr
End Property
Public Property Get hitlistTranslateRelKey() As String
    hitlistTranslateRelKey = m_hitlistTranslateRelKey
End Property
Public Property Let hitlistTranslateStrucKey(ByVal aStr As String)
    m_hitlistTranslateStrucKey = aStr
End Property
Public Property Get hitlistTranslateStrucKey() As String
    hitlistTranslateStrucKey = m_hitlistTranslateStrucKey
End Property
Public Property Let molServer_MaxHits(ByVal aLng As Long)
    m_molServer_MaxHits = aLng
End Property
Public Property Get molServer_MaxHits() As Long
    molServer_MaxHits = m_molServer_MaxHits
End Property
Public Property Let SimSearchFullStructure(ByVal aBool As Boolean)
    m_SimSearchFullStructure = aBool
End Property
Public Property Get SimSearchFullStructure() As Boolean
    SimSearchFullStructure = m_SimSearchFullStructure
End Property

Public Function PersistToXML() As String
    Dim retVal As String
    retVal = TopLevelTagOpen("CSDO19.ChemSearchOptions")
    retVal = retVal & TaggedItem("ExtraFragsOK", CStr(ExtraFragsOK))
    retVal = retVal & TaggedItem("ExtraFragsOKIfRxn", CStr(ExtraFragsOKIfRxn))
    retVal = retVal & TaggedItem("FindChargedCarbon", CStr(FindChargedCarbon))
    retVal = retVal & TaggedItem("FindChargedHetero", CStr(FindChargedHetero))
    retVal = retVal & TaggedItem("FragsCanOverlap", CStr(FragsCanOverlap))
    retVal = retVal & TaggedItem("StereoDB", CStr(StereoDB))
    retVal = retVal & TaggedItem("StereoTetr", CStr(StereoTetr))
    retVal = retVal & TaggedItem("UseRxnCenters", CStr(UseRxnCenters))
    retVal = retVal & TaggedItem("AbsHitsRel", CStr(AbsHitsRel))
    retVal = retVal & TaggedItem("RelativeStereo", CStr(RelativeStereo))
    retVal = retVal & TaggedItem("IdentitySearch", CStr(IdentitySearch))
    retVal = retVal & TopLevelTagClose("CSDO19.ChemSearchOptions")
    PersistToXML = retVal
End Function
Public Function CreateFromXML(strDoc As String) As Boolean
    ExtraFragsOK = CBool(GetNextTaggedItem(strDoc, "ExtraFragsOK"))
    ExtraFragsOKIfRxn = CBool(GetNextTaggedItem(strDoc, "ExtraFragsOKIfRxn"))
    FindChargedCarbon = CBool(GetNextTaggedItem(strDoc, "FindChargedCarbon"))
    FindChargedHetero = CBool(GetNextTaggedItem(strDoc, "FindChargedHetero"))
    FragsCanOverlap = CBool(GetNextTaggedItem(strDoc, "FragsCanOverlap"))
    StereoDB = CBool(GetNextTaggedItem(strDoc, "StereoDB"))
    StereoTetr = CBool(GetNextTaggedItem(strDoc, "StereoTetr"))
    UseRxnCenters = CBool(GetNextTaggedItem(strDoc, "UseRxnCenters"))
    AbsHitsRel = CBool(GetNextTaggedItem(strDoc, "AbsHitsRel"))
    RelativeStereo = CBool(GetNextTaggedItem(strDoc, "RelativeStereo"))
    IdentitySearch = CBool(GetNextTaggedItem(strDoc, "IdentitySearch"))

    CreateFromXML = True
End Function
Public Function Fill(aExtraFragsOK As Boolean, _
                     aExtraFragsOKIfRxn As Boolean, _
                     aFindChargedCarbon As Boolean, _
                     aFindChargedHetero As Boolean, _
                     aFragsCanOverlap As Boolean, _
                     aStereoDB As Boolean, _
                     aStereoTetr As Boolean, _
                     aUseRxnCenters As Boolean, _
                     aAbsHitsRel As Boolean, _
                     aRelativeStereo As Boolean, _
                     aIdentitySearch As Boolean)
                    
    ExtraFragsOK = aExtraFragsOK
    ExtraFragsOKIfRxn = aExtraFragsOKIfRxn
    FindChargedCarbon = aFindChargedCarbon
    FindChargedHetero = aFindChargedHetero
    FragsCanOverlap = aFragsCanOverlap
    StereoDB = aStereoDB
    StereoTetr = aStereoTetr
    UseRxnCenters = aUseRxnCenters
    AbsHitsRel = aAbsHitsRel
    RelativeStereo = aRelativeStereo
    IdentitySearch = aIdentitySearch
End Function
Friend Function FillSearchInfo(aSearchInfo As MolServer19.searchInfo)
    With aSearchInfo
        .ExtraFragsOK = ExtraFragsOK
        .ExtraFragsOKIfRxn = ExtraFragsOKIfRxn
        .FindChargedCarbon = FindChargedCarbon
        .FindChargedHetero = FindChargedHetero
        .FragsCanOverlap = FragsCanOverlap
        .StereoDB = StereoDB
        .StereoTetr = StereoTetr
        .UseRxnCenters = UseRxnCenters
        .AbsHitsRel = AbsHitsRel
        .RelativeTetStereo = RelativeStereo
        .IdentitySearch = IdentitySearch
        
    End With
End Function
                     
'!DGB! 04/18/01 Initialize new properties for hitlist management
'TODO should also initialize the rest of the the properties
Private Sub Class_Initialize()
    ' init structure with default values
    m_molServer_MaxHits = 0
    m_SimSearchFullStructure = True
    m_hitlistID = 0
    m_SearchDomain = 0
    m_hitlistContentsField = ""
    m_hitlistTranslateTable = ""
    m_hitlistTranslateRelKey = ""
    m_hitlistTranslateStrucKey = ""
End Sub
                    
