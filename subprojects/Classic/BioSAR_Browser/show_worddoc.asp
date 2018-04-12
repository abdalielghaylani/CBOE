<!--#INCLUDE VIRTUAL = "/cfserverasp/source/xml_source/rs2html.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE FILE="source/app_js.js"-->
<!--#INCLUDE FILE="source/app_vbs.asp"-->
<% tPageSize = request("page_size")
	current_index = request("current_index")
	sort_fields = request("sort_fields")
	on error resume next
	template_definition = getXMLTemplate(dbkey, formgroup, EXCEL_TEMPLATE)
	if Not isObject(Session("TemplateDom")) then 'if it is an object then don't recreate. Only wiped when edittin formgroup or creteing  new search
		Set Session("TemplateDom") = Server.CreateObject("MSXML2.FreeThreadedDOMDocument")
	end if	
		
	Set displayDom = Session("TemplateDom")		
	displayDom.loadXML(template_definition)
	theIndex = current_index - 1
	output= RS2HTML(Session("LIST_RS"),displayDom,tPageSize,NULL,theIndex,theIndex,sort_fields,"","")
	Response.ContentType = "application/vnd.ms-word"
    Response.Charset = "ISO-8859-1"
     
    Response.Write "<html><head></head>" & output & "</html>"
    Response.Flush
       
 %>