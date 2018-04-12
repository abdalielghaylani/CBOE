VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CFWretrieve"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True

Function SaveCFWASCII(formpath, savepath)
    Dim CFWApp As Object
    Dim CFWDoc As Object
   
    On Error Resume Next
    Set CFWApp = CreateObject("ChemFinder.Application")
        
    Set CFWDoc = CFWApp.Documents.Open(formpath)
      
    CFWDoc.ExecuteCALCommand "SET OPTIONS NON_INTERACTIVE 1"
       
    outputpath = savepath & "\cfwtemp\cfwoutput.txt"
    CFWDoc.SaveAs outputpath
        
    CFWDoc.Close
     
    Set CFWDoc = Nothing
       
    CFWApp.Quit
      
    Set CFWApp = Nothing
     
     
    SaveCFWASCII = outputpath
    
End Function

Function GetCFWRecordCount(formpath)
    Dim CFWApp As Object
    Dim CFWDoc As Object
    Set CFWApp = CreateObject("ChemFinder.Application")
        
    On Error Resume Next
    Set CFWDoc = CFWApp.Documents.Open(formpath)
        
    CFWDoc.ExecuteCALCommand "SET OPTIONS NON_INTERACTIVE 1"
        
    CFWDoc.ExecuteCALCommand ("Search Retrieve All")
       
     GetCFWRecordCount = CFWDoc.Recordset.RecordCount
         
    CFWDoc.Close
        
    Set CFWDoc = Nothing
        
    CFWApp.Quit
        
    Set CFWApp = Nothing
       
   
End Function


Sub SaveCFWForm(formpath, savepath)
    Dim CFWApp As Object
    Dim CFWDoc As Object
    Set CFWApp = CreateObject("ChemFinder.Application")
    On Error Resume Next
    Set CFWDoc = CFWApp.Documents.Open(formpath)
    CFWDoc.ExecuteCALCommand "SET OPTIONS NON_INTERACTIVE 1"
    theLongSaveName = savepath & "long" & ".cfw"
    theShortSaveName = savepath & "short" & ".cfw"
    CFWDoc.SaveAs theLongSaveName
        
    CFWDoc.SaveAs theShortSaveName
       
    CFWDoc.Close
       
    Set CFWDoc = Nothing
        
    CFWApp.Quit
       
    Set CFWApp = Nothing
        
End Sub
