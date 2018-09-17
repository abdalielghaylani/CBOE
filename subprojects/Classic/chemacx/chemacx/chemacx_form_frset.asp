<html>
	<head>
		<title>Form View Frameset</title>
		<script language="JavaScript">
			// Make sure that framset is loaded inside of COWS main frame
			if (top.main.location.href != self.location.href){
				top.main.location.href = self.location.href;
			}
		</script>
	</head>
	<frameset border="0" frameborder="0" framespacing="5" cols="265,*">
		<frame resize="no" scrolling="auto" BORDER="0" MARGINWIDTH="1" MARGINHEIGHT="1" SRC="chemacx_form.asp?<%=request.QueryString%>" NAME="mainFrame">	
		<frame BORDER="0" MARGINWIDTH="1" MARGINHEIGHT="1" SRC="../loading.html" NAME="DisplayFrame" scrolling="auto">	
	</frameset>
	<body>
	</body>
</html>

