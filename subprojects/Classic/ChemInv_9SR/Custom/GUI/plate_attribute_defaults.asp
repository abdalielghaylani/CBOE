<%

Sub SetDefaultAttributeValues()
	' Intial values for plate attributes
	tempDate = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now()) + ExpYears 
	tempDate = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now())
	
	'Custom fields
	Session("plField_1") = ""
	Session("plField_2") = ""
	Session("plField_3") = ""
	Session("plField_4") = ""
	Session("plField_5") = ""
	Session("plDate_1") = ""
	Session("plDate_2") = ""
End Sub
%>
