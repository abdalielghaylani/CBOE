<%@ EnableSessionState=False Language=VBScript%>
<Script RUNAT="Server" Language="VbScript">
Dim key, item    
for each key in Application.Contents
    if not IsObject(Application.Contents(key)) and not IsArray(Application.Contents(key)) then
        Response.Write (key & "::")
        Response.Write (Application.Contents(key) & "||" )
    end if
next
</SCRIPT>
