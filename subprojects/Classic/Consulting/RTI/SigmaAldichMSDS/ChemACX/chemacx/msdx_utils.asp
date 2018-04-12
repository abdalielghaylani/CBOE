<%
Sub ShowMSDXLink(ACX_ID, CAS, SupplierID, SupplierCatNum, bHasMSDX)
	Dim QS	
	msdxLinkText = Application("msdxLinkText")
	' DGB the bHasMSDX flag is only meaningful when the system is configured
	' to use the MSDX data sheets.  We assume that this is the case when
	' MSDX_LINK_TEXT = MSDX
	' If we are not using MSDX then always show the link
	'if bHasMSDX OR msdxLinkText <> "MSDX" then
	if true then
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