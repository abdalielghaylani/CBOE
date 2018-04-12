<%
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' This file is used to set the IIS metabase to allow out of process components to be 
' executed from an asp application.
'The ChemACX application needs to create instances of Excel.exe during the export to 
'Excel process.  For this feature to work it is necessary that out-of-process executables
'be allowed to start from an asp in the chemacx virtual directory.
'
'To accomplish this:
'
'1. Make sure that the folder where oop.asp is placed forces windows authentication when executed
'from a browser
'2. Make sure the user accesing the file has admin priviledges on the IIS server machine
'3. Execute the file as oop.asp?virtual=chemacx
'4. Restart IIS
'
'In addition, for Windows 2000 it seems like two more steps are require
'Using OLEVIEW set the Word Document Object to Launch as Interactive
'Using DCOMCNF set the Word Application to Allow Access for Everyone
'Hopefully this will ensure that Export to Excel works
'
	 vdName = request.querystring("virtual")
	 
if len(vdName)> 0 then
	 
     ' Get the IIsWebVirtualDir Admin Object
     Set VDirObj = GetObject("IIS://LocalHost/W3svc/1/Root/" & vdName)

     ' Enable the AspAllowOutOfProcComponents Parameter
     VDirObj.Put "AspAllowOutOfProcComponents", True

     ' Save the changed value to the metabase
     VDirObj.SetInfo
     Response.Write "Out of process components are now allowed for the " & vdName & " virtual directory"   
Else
	response.write "USAGE: oop.asp?virtual=VirtualDirName"
End if

%> 

