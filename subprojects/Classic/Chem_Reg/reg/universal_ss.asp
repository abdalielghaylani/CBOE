<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

if CBool(Application("PROJECT_LEVEL_ROW_SECURITY")) = true then
	if Session("UserName" & dbkey) = Application("REG_USERNAME") then
		iF Session("storeSessionUser" & dbkey) <> "" then
			Session("UserName" & dbkey) = Session("storeSessionUser" & dbkey)
			Session("UserID" & dbkey) = Session("storeSessionPWD" & dbkey)
		else
			response.Write "Houston we have a problem"
		end if
	end if
end if

body_default=  Application("BODY_BACKGROUND") & """ bgProperties=fixed"

if not UCase(formmode)= "EDIT" or UCase(formmode) = "SEARCH" then
	table_main_L1 = " bgcolor =""#FFFFFF"" border = ""1"""
	
	registered_compound_table = " bgcolor =""#CCCCCC"" border = ""1"""
	td_bgcolor = " bgcolor=""#FFFFFF"""
	td_bgcolor_c1 = " bgcolor=""#FFFFFF"""
	td_bgcolor_c2 = " bgcolor=""#FFFFFF"""
	td_caption_bgcolor = " bgcolor=""#CCCCCC"""
	td_header_bgcolor = " bgcolor=""#CCCCCC"""
	font_default_small= " face=""Verdana"" size=""1"""
	font_default = " face=""Verdana"" size=""2"""
	font_default_caption = " face=""Verdana"" size=""2"""
	font_header_default_1 = " face=""Verdana"" size=""3"""
	font_header_default_2 = " face=""Verdana"" size=""2"""
else
	table_main_L1 = " bgcolor =""#FFFFFF"" border = ""1"""
	registered_compound_table = " bgcolor =""#CCCCCC"" border = ""1"""
	td_bgcolor = " bgcolor=""#CCCCCC"""
	td_bgcolor_c1 = " bgcolor=""#CCCCCC"""
	td_bgcolor_c2 = " bgcolor=""#CCCCCC"""
	td_caption_bgcolor = " bgcolor=""#FFFFFF"""
	td_header_bgcolor = " bgcolor=""#CCCCCC"""
	font_default = " face=""Verdana"" size=""2"""
	font_default_small= " face=""Verdana"" size=""1"""
	font_default_caption = " face=""Verdana"" size=""2"""
	font_header_default_1 = " face=""Verdana"" size=""3"""
	font_header_default_2 = " face=""Verdana"" size=""2"""
end if

%>