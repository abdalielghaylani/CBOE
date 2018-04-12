<%@ Language=VBScript %>
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
dim arrValues
dim Sum
dim ValueList

ValueList = request("valuelist")
arrValues= split(ValueList,"|")
result=0 

for each value in arrValues
    result= result+ value
Next

response.Write result
Response.End
%>