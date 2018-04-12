<%
Sub ShowMSDXLink(ACX_ID, CAS, SupplierID, SupplierCatNum, bHasMSDX)
	Dim QS	
	msdxLinkText = Application("msdxLinkText")
	if bHasMSDX OR msdxLinkText <> "MSDX" then
		msdxURL = Application("msdxURL")
		msdxLinkTitle = Application("msdxLinkTitle")
		msdxACXIDKeywd = Application("msdxACXIDKeywd")
		msdxCasKeywd = Application("msdxCASKeywd")
		msdxSupplierIdKeywd = Application("msdxSupplierIDKeywd")
		msdxSupplierCatNumKeywd = Application("msdxSupplierCatNumKeywd")
		
		
		if msdxACXIDKeywd <> "NULL" then 
			if ACX_ID <> "" then
				if QS <> "" then QS = QS & "&" 
				QS = msdxACXIDKeywd & "=" & ACX_ID
			end if		
		End if
		
		if msdxCasKeywd <> "NULL" then 
			if CAS <> "" then 
				if QS <> "" then QS = QS & "&"	
				QS = QS & msdxCasKeywd & "=" & CAS
			end if
		End if
		
		if msdxSupplierIdKeywd <> "NULL" then 
			SupplierID = Cstr(SupplierID)
			if SupplierID <> "0" AND SupplierID <> "" then 
				if QS <> "" then QS = QS & "&"	
				QS = QS & msdxSupplierIdKeywd & "=" & SupplierID
			end if
		End if
		
		if msdxSupplierCatNumKeywd <> "NULL" then
			if SupplierCatNum <> "" then 
				if QS <> "" then QS = QS & "&"
				QS = QS & msdxSupplierCatNumKeywd & "=" & SupplierCatNum 
			end if
		end if
		
		Response.Write "<a href=""" & msdxURL & "?" & QS & """ class=""MenuLink"" target=""_new"" title=""" & msdxLinkTitle & """>" & msdxLinkText & "</a>"
	Else
		Response.Write "<a href=""#"" disabled title=""No materials safety data available"" class=""MenuLink"" onclick=""return false;"">" & msdxLinkText & "</a>"
	End if
End Sub
%>