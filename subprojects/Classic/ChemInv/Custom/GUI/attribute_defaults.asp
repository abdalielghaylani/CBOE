<%

Sub SetDefaultAttributeValues()
	' Intial values for container attributes
	Session("UOMIDOptionValue")= "6=mg"
	Session("UOWIDOptionValue")= "5=g"
	Session("UOCIDOptionValue")= "14=mmol"
	Session("UODIDOptionValue")= "26=mg/ml"
	Session("UOPIDOptionValue")= "12=%"
	Session("UOCostIDOptionValue")= "23=$"
	Session("ContainerTypeID")= "0"
	Session("ContainerStatusID") = Application("DefConStatusID")
	If Session("ContainerStatusID") = "" or IsEmpty(Session("ContainerStatusID")) then Session("ContainerStatusID")= ""
	Session("SupplierID") = ""
	
	' the default barcode description is 3 = Container Barcodes
    Session("BarcodeDescID") = 3
	
	

	Session("ContainerTypeID") = ""
	'tempDate = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now()) + ExpYears 
	Session("ExpDate") = GetLocalNowPlusDate(0,0,ExpYears)
	'Session("ExpDate") = ConvertDateToStr(Application("DATE_FORMAT"),CDate(tempDate))	
	'tempDate = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now())
	'Session("DateReceived") = ConvertDateToStr(Application("DATE_FORMAT"),tempDate)
	Session("DateReceived") = GetLocalNowPlusDate(0,0,0)
	'Session("DateOrdered") = ConvertDateToStr(Application("DATE_FORMAT"),tempDate)
	Session("DateOrdered") = GetLocalNowPlusDate(0,0,0)
	Session("OwnerID") = ""
	
	'Custom fields
	Session("Field_1") = ""
	Session("Field_2") = ""
	Session("Field_3") = ""
	Session("Field_4") = ""
	Session("Field_5") = ""
	Session("Field_6") = ""
	Session("Field_7") = ""
	Session("Field_8") = ""
	Session("Field_9") = ""
	Session("Field_10") = ""
	Session("Date_1") = ""
	Session("Date_2") = ""
	Session("Date_3") = ""
	Session("Date_4") = ""
	Session("Date_5") = ""
End Sub
%>
