Attribute VB_Name = "XML"
Option Explicit

' Writing functions

Public Function VersionHeader() As String
    VersionHeader = "<?xml version=" & """" & "1.0" & """" & "?>"
End Function

Public Function TopLevelTagOpen(tag As String)
    TopLevelTagOpen = "<CambridgeSoft:" & tag & " xmlns:CambridgeSoft='http://www.camsoft.com/'>"
End Function

Public Function TopLevelTagClose(tag As String)
    TopLevelTagClose = "</CambridgeSoft:" & tag & ">"
End Function

Public Function TagOpen(contents As String) As String
    TagOpen = "<" & contents & ">"
End Function

Public Function TagClose(contents As String) As String
    TagClose = "</" & contents & ">"
End Function

Public Function TaggedItem(tagData As String, itemData As String) As String
    TaggedItem = TagOpen(tagData) & itemData & TagClose(tagData)
End Function

' reading functions

Public Function GetNextTaggedItem(strDoc As String, tagData As String, Optional ByRef startPos As Long = 1) As String
    ' gets next instance of the given tag and returns the position of found data
    Dim foundPos As Long
    Dim endPos As Long
    Dim dataPos As Long
    
    foundPos = InStr(startPos, strDoc, TagOpen(tagData))
    If foundPos = 0 Then
        startPos = 0
        GetNextTaggedItem = ""
        Exit Function
    End If
    endPos = InStr(foundPos, strDoc, TagClose(tagData))
    
    dataPos = foundPos + Len(TagOpen(tagData))
    GetNextTaggedItem = Mid(strDoc, dataPos, endPos - dataPos)
    startPos = endPos + Len(TagClose(tagData))
End Function

Public Function GetNextTopLevelTaggedItem(strDoc As String, tagData As String, Optional ByRef startPos As Long = 1) As String
    ' gets next instance of the given tag and returns the position of found data
    Dim foundPos As Long
    Dim endPos As Long
    Dim dataPos As Long
    
    foundPos = InStr(startPos, strDoc, TopLevelTagOpen(tagData))
    If foundPos = 0 Then
        startPos = 0
        GetNextTopLevelTaggedItem = ""
        Exit Function
    End If
    endPos = InStr(foundPos, strDoc, TopLevelTagClose(tagData))
    
    dataPos = foundPos + Len(TopLevelTagOpen(tagData))
    GetNextTopLevelTaggedItem = Mid(strDoc, dataPos, endPos - dataPos)
    startPos = endPos + Len(TopLevelTagClose(tagData))
End Function
