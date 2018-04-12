<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiUtils.asp"-->
<%


Dim Conn
Dim Cmd
Dim RS

stop
sql= "select max(to_char(searchtimestamp, 'MM/DD/YYYY')) SearchDate ,max(csuserid) as UserName, count(host) as NumSearches from chemacxdb.searchlog group by to_char(searchtimestamp, 'MM/DD/YYYYY'), csuserid"

Response.Expires = -1
GetACXConnection()
Set RS = Conn.execute(sql)
Response.ContentType = "text/xml"
Response.Write "<?xml:stylesheet type=""text/xsl"" href=""recordsetxml.xsl""?>" & vbCrLf
RS.Save Response, 1

%>