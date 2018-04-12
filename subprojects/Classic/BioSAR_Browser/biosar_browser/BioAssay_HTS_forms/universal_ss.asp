<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

body_default=  Application("BODY_BACKGROUND") & """ bgProperties=fixed"
formmode = request("formmode")
Select Case UCase(formmode)
Case "EDIT"
	table_main_L1 = " bgcolor =""#FFFFFF"" border = ""1"""
	td_bgcolor = " bgcolor=""#FFFFFF"""
	td_caption_bgcolor = " bgcolor=""#FFFFFF"""
	td_bgcolor_column = " bgcolor=""#CCCCCC"" align = ""left"""
	td_caption_bgcolor_column = " bgcolor=""#FFFFFF"" align = ""right"""
	td_header_bgcolor = " bgcolor=""#CCCCCC"" nowrap"
	font_default = " face=""Verdana"" size=""2"""
	font_default_caption = " face=""Verdana"" size=""2"""
	font_header_caption = " face=""Verdana"" size=""2"""
	
Case "SEARCH"
	table_main_L1 = " bgcolor =""#FFFFFF"" border = ""0"""
	td_bgcolor = " bgcolor=""#FFFFFF"""
	td_caption_bgcolor = " bgcolor=""#CCCCCC"""
	td_header_bgcolor = " bgcolor=""#CCCCCC"" nowrap"
	td_bgcolor_column = " bgcolor=""#CCCCCC"" align = ""left"""
	td_caption_bgcolor_column = " bgcolor=""#FFFFFF"" align = ""right"""
	font_default = " face=""Verdana"" size=""2"""
	font_default_caption = " face=""Verdana"" size=""2"""
	font_header_caption = " face=""Verdana"" size=""2"""

	
Case "LIST"
	table_main_L1 = " bgcolor =""#FFFFFF"" border = ""1"""
	td_bgcolor = " bgcolor=""#FFFFFF"""
	td_caption_bgcolor = " bgcolor=""#FFFFFF"""
	td_header_bgcolor = " bgcolor=""#CCCCCC"" nowrap"
	td_bgcolor_column = " bgcolor=""#CCCCCC"" align = ""left"""
	td_caption_bgcolor_column = " bgcolor=""#FFFFFF"" align = ""right"""
	td_bgcolor_list = " bgcolor=""#FFFFFF"" align = ""left"""
	td_caption_bgcolor_list = " bgcolor=""#CCCCCC"" align = ""right"""
	font_default = " face=""Verdana"" size=""2"""
	font_default_caption = " face=""Verdana"" size=""2"""
	font_header_caption = " face=""Verdana"" size=""2"""
Case Else
	
	table_main_L1 = " bgcolor =""#FFFFFF"" border = ""1"""
	td_bgcolor = " bgcolor=""#CCCCCC"""
	td_caption_bgcolor = " bgcolor=""#FFFFFF"""
	td_header_bgcolor = " bgcolor=""#CCCCCC"" nowrap"
	td_bgcolor_column = " bgcolor=""#CCCCCC"" align = ""left"""
	td_caption_bgcolor_column = " bgcolor=""#FFFFFF"" align = ""right"""
	font_default = " face=""Verdana"" size=""2"""
	font_default_caption = " face=""Verdana"" size=""2"""
	font_header_caption = " face=""Verdana"" size=""3"""
	
end select

%>