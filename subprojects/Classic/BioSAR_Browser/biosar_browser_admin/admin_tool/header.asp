
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/source/secure_nav.asp"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/source/app_js.js"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/source/app_vbs.asp"-->
<!--#INCLUDE VIRTUAL=  "/biosar_browser/biosar_browser_admin/admin_tool/admin_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/manage_queries.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/marked_hits_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/hitlist_management_func_vbs.asp"-->
<script LANGUAGE="javascript" src="/biosar_browser/biosar_browser_admin/admin_tool/Choosecss.js"></script>
<%Session("UserFormgroupsExist") = UserFormgroupsExist(dbkey, formgroup)
%>