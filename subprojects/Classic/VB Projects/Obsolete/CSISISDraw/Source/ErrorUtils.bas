Attribute VB_Name = "ErrorUtils"
Option Explicit

' ##MODULE_SUMMARY A set of functions used to consistently display and log errors.

Public Function ErrorMsgBox(ByVal E As ErrObject, _
    Optional ByVal ErrTitle As String = "", _
    Optional ByVal Connection As ENFramework9.Connection = Nothing)
    Const kCancelError = 32755
    Dim errSource As String
    Dim errDesc As String
    Dim errNumber As Long
    
    If (ErrTitle = "") Then
        ErrTitle = "Error in " & App.Path
    End If

    If (E.Number <> 0 And E.Number <> kCancelError) Then
        errSource = E.Source
        errDesc = E.Description
        errNumber = E.Number
        If (VBA.Right(errDesc, 1) <> ".") Then
            errDesc = errDesc & "."
        End If
        
        MsgBox "Sorry, " & errDesc, vbExclamation, Title:=ErrTitle
        
        If (Not Connection Is Nothing) Then
            ' Write log information to the log file.
            Connection.WriteLogStream ""
            Connection.WriteLogStream "The Following Error Message Appeared:"
            Connection.WriteLogStream "Title: " & ErrTitle
            Connection.WriteLogStream "Message: " & "Sorry, " & errDesc
            Connection.WriteLogStream "Source: " & errSource
            Connection.WriteLogStream "Number: " & errNumber
        End If
    End If
End Function

