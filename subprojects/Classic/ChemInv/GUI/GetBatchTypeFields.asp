<% Sub GetBatchFiledName()

     Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".Batch.GETBATCHFIELD()}", adCmdText)	
     Cmd.Properties ("PLSQLRSet") = TRUE  
     Set RS = Cmd.Execute
     Cmd.Properties ("PLSQLRSet") = FALSE
     While Not RS.EOF 
     If RS ("Batch_Order") = "1" Then
        BatchName1=RS("Batch_type")
     End If 
     If RS ("Batch_Order") = "2" Then
        BatchName2=RS("Batch_type")
     End If 
     If RS ("Batch_Order") = "3" Then
        BatchName3=RS("Batch_type")
     End If 
            RS.MoveNext
     Wend
End Sub %>