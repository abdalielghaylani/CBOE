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

' CS ChemSearchOptions

' just a structure of chemical search options

Public ExtraFragsOK As Boolean
Public ExtraFragsOKIfRxn As Boolean
Public FindChargedCarbon As Boolean
Public FindChargedHetero As Boolean
Public FragsCanOverlap As Boolean
Public StereoDB As Boolean
Public StereoTetr As Boolean
Public UseRxnCenters As Boolean

Public Function PersistToXML() As String
    Dim retVal As String
    retVal = TopLevelTagOpen("CSDO.ChemSearchOptions")
    retVal = retVal & TaggedItem("ExtraFragsOK", CStr(ExtraFragsOK))
    retVal = retVal & TaggedItem("ExtraFragsOKIfRxn", CStr(ExtraFragsOKIfRxn))
    retVal = retVal & TaggedItem("FindChargedCarbon", CStr(FindChargedCarbon))
    retVal = retVal & TaggedItem("FindChargedHetero", CStr(FindChargedHetero))
    retVal = retVal & TaggedItem("FragsCanOverlap", CStr(FragsCanOverlap))
    retVal = retVal & TaggedItem("StereoDB", CStr(StereoDB))
    retVal = retVal & TaggedItem("StereoTetr", CStr(StereoTetr))
    retVal = retVal & TaggedItem("UseRxnCenters", CStr(UseRxnCenters))
    retVal = retVal & TopLevelTagClose("CSDO.ChemSearchOptions")
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
    CreateFromXML = True
End Function

Public Function Fill(aExtraFragsOK As Boolean, _
                     aExtraFragsOKIfRxn As Boolean, _
                     aFindChargedCarbon As Boolean, _
                     aFindChargedHetero As Boolean, _
                     aFragsCanOverlap As Boolean, _
                     aStereoDB As Boolean, _
                     aStereoTetr As Boolean, _
                     aUseRxnCenters As Boolean)
    ExtraFragsOK = aExtraFragsOK
    ExtraFragsOKIfRxn = aExtraFragsOKIfRxn
    FindChargedCarbon = aFindChargedCarbon
    FindChargedHetero = aFindChargedHetero
    FragsCanOverlap = aFragsCanOverlap
    StereoDB = aStereoDB
    StereoTetr = aStereoTetr
    UseRxnCenters = aUseRxnCenters
End Function
                     
Friend Function FillSearchInfo(aSearchInfo As MolServer.searchInfo)
    With aSearchInfo
        .ExtraFragsOK = ExtraFragsOK
        .ExtraFragsOKIfRxn = ExtraFragsOKIfRxn
        .FindChargedCarbon = FindChargedCarbon
        .FindChargedHetero = FindChargedHetero
        .FragsCanOverlap = FragsCanOverlap
        .StereoDB = StereoDB
        .StereoTetr = StereoTetr
        .UseRxnCenters = UseRxnCenters
    End With
End Function
                     
                    
