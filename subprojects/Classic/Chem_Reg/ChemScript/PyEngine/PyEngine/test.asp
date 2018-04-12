<%@ LANGUAGE=VBScript %>
<%
Dim soapClient

Set soapClient = CreateObject("MSSOAP.SoapClient30")

soapClient.ClientProperty("ServerHTTPRequest") = True
On Error Resume Next
Call soapClient.MSSoapInit("http://localhost/PyEngine/Service.asmx?wsdl")

ver = soapClient.Version

dim code, ret, error, invar
code = _
"from ChemScript import *" + vbcrlf + _
"m=Mol.loadData(base64, 'smiles')" + vbcrlf +  _
"m.normalizeStructure()" + vbcrlf +  _
"frags=m.splitSalt()" + vbcrlf +  _
"if frags != None:" + vbcrlf +  _
"    main = frags[0]" + vbcrlf +  _
"    salt = frags[1]" + vbcrlf +  _
"else:" + vbcrlf +  _
"    main = m" + vbcrlf +  _
"    salt = None"

Dim inputvars(0)
inputvars(0) = "base64"
Dim inputs(0)
inputs(0) = "CCCCC.[Na+]"

Dim outputvars(1)
outputvars(0) = "main"
outputvars(1) = "salt"

'stop
f = soapClient.SingleExecute(code, inputvars, inputs, outputvars, ret, error)

Response.Write "ret=" + "<br />" + ret
%>
