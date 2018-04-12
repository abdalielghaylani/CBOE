<!--SYAN added on 2/14/2005 to fix CSBR-50932-->
<%
t1 = Application(formgroup & dbkey)
t2 = split(t1(7),".")
strucTable = t2(0)

%>
<%ShowInputField dbkey, formgroup, strucTable & ".SDF", "0","17"%>
<script language="javascript">

	function getOpenFileButton2(fieldname){
		var outpuval = ""
		outputval = '<a href="javascript:MainWindow.doOpenFileLoadWindow2(&quot;' + fieldname + '&quot;)">'
		outputval = outputval + '<img SRC="/cfserverasp/source/graphics/open_file_btn.gif" BORDER="0"></a>'
		document.write(outputval)
	}


	function doOpenFileLoadWindow2(fieldname){
		var w = ""
		if (w.name == null){		
			var w = window.open("/<%=application("appkey")%>/Load_IDS.asp?dbname=<%=dbkey%>&formgroup=<%=formgroup%>&fieldname=" + fieldname,"load_ids_from_file","width=450,height=30,scrollbars=yes,status=yes,resizable=yes");
			w.focus()}
		else{
			w.focus()}
	}

	getOpenFileButton2("<%=strucTable%>.SDF")
</script>
<input type="hidden" name="MultipleExactSearch" value="">
<!--End of SYAN modification-->
