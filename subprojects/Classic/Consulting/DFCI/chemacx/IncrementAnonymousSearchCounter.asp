<!--#INCLUDE FILE = "AnonymousSearchCounterFunctions.asp"-->
<%
Select Case LCase(Request.QueryString("dataaction"))
		Case "search" , "search_no_gui", "query_string"
			IncrementSearchCount(ServiceID)	
END Select
%>
