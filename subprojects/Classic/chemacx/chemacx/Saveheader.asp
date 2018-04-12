<%
Response.Buffer = true
Session("error" & dbkey)= False
	
	TempDirURL =Application("TempFileDirectoryHTTP" & "chemacx") & "Sessiondir/" & Session.sessionid & "/"
	TempDirPath = Server.MapPath(TempDirURL) & "\"
	WordTemplatePath = Server.MapPath("WordTemplates") & "\"
	
	WordTemplateName = "chemacx_mailmerge.doc"
	CSVFileName= "ChemACX_shopcart.data"
	WordShopCartFileName = "ChemACXShopCart.doc"
		
	
	Set fs = Server.CreateObject("Scripting.FileSystemObject")
	Set a = fs.CreateTextFile(TempDirPath & CSVfilename)  
	a.Write request.form("headerlist") & request.form("itemslist")
	a.close
	
	Set WordApp = CreateObject("Word.Application")
	WordApp.Visible = False
	WordApp.DisplayAlerts = False
	WordApp.Documents.Open (WordTemplatePath & WordTemplateName)
    WordApp.ActiveDocument.MailMerge.MainDocumentType = 3
    WordApp.ActiveDocument.MailMerge.OpenDataSource (TempDirPath & CSVfilename)
    WordApp.ActiveDocument.MailMerge.Destination = 0
    WordApp.ActiveDocument.MailMerge.Execute
	WordApp.ActiveDocument.SaveAs (TempDirPath & WordShopCartFileName)
	WordApp.ActiveDocument.Close
	WordApp.Quit 0
	
	bServeLink = False 'True = get a link to doc file; False = stream the doc file
	ServeFile TempDirURL & WordShopCartFileName, bServeLink
	
'*************************************************************************
'* Serve the newly created .doc file
'* Setting bServeLink to True serves a link to .doc file
'* Setting bServeLink to False redirects the browser to the .doc file
Function ServeFile(byval FileURL, byval bServeLink)
	'FilePath = Server.MapPath (FileURL)
	'Set WaitObj = Server.CreateObject ("WaitFor.Comp")
	'WaitObj.TimeOut = 15
	'if WaitObj.WaitForExclusiveFileAccess(FilePath) then
		if NOT bServeLink then
			Response.clear
			Response.redirect FileURL
		Else
			Response.Write("<HTML><BODY>")
			Response.Write("<BR><BR><BR><CENTER>")
			Response.Write("Click <A HRef=" & FileURL & ">Here</A> to get your shopping cart as a Word file<BR> (right-click to save it on your local disk)")
			Response.Write("</CENTER>")
			Response.Write("</BODY></HTML>")
		End if
	'Else
		'Response.Write("<HTML><BODY>")
		'Response.Write("<BR><BR><BR><CENTER>")
		'Response.Write("Error:  Word File could not be accessed on the server")
		'Response.Write("</CENTER>")
		'Response.Write("</BODY></HTML>")
	'End if
End Function
%>
