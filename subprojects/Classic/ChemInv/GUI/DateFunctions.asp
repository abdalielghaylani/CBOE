<%
'This Function is used to display a date in MM/DD/YYYY HH:MM:SS format to the date format configured on the server machine.  

Function GetFormatedDate(Inputdate)
   DayValue = datepart("d",Inputdate)
   If Len(DayValue) = 1 Then DayValue = "0" & DayValue
   MonthValue = DatePart("m", Inputdate)
   If Len(MonthValue) = 1 Then MonthValue = "0" & MonthValue
   YearValue = DatePart ("YYYY",Inputdate)
   HourValue = datepart("h",Inputdate)
   If Len(HourValue) = 1 Then HourValue = "0" & HourValue
   MinuteValue = DatePart("n", Inputdate)
   If Len(MinuteValue) = 1 Then MinuteValue = "0" & MinuteValue
   SecondValue = DatePart ("s",Inputdate)
   If Len(SecondValue) = 1 Then SecondValue = "0" & SecondValue
   
   Select Case Application("Date_Format")
	    Case "8"
		   GetFormatedDate = Inputdate
	    Case "9"
		    GetFormatedDate = DayValue & "/" & MonthValue & "/" & YearValue & " " & HourValue & ":" & MinuteValue & ":" & SecondValue
	    Case "10"
		    GetFormatedDate = YearValue & "/" & MonthValue & "/" & DayValue & " " & HourValue & ":" & MinuteValue & ":" & SecondValue
    End Select
End Function 
%>
