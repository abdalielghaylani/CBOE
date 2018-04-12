Deployment of Registration-ChemScript integration


•	Install MS .Net Framework 2.0 if it is not installed. From \\shares\public\Software\Windows\Net Framework\Net Framework 2.0 Shipping

•	Install MS Soap Client from \\shares\Public\Software\Windows\SOAP Installers\SoapToolkit SDK\SoapSDK.exe


•	Install ChemScript 11 (This will install Python 2.5)

•	Reboot

•	To test ChemScript installation

   	Go to Programs | Python 2.5 | IDLE (Python GUI), and type command
  	from ChemScript import * (the command is case sensitive)

   	If the return is 
  	----------------------------------------------------------------------------
   	Welcome to CS ChemScript Server for Python (10.1d190 September 8, 2006)
   	Copyright (C) 2005-2006 CambridgeSoft Corp., all rights reserved.
   	----------------------------------------------------------------------------
   	For help: use the "help()" command.  For example, type: "help(Mol)".

   	It indicates ChemScript is installed properly.

	If it asks for license, get serial number of ChemScript 11 and Active Over Internet

•	Apply  9.0SR4 file changes to the server (If SR3 installed, remove SR3 first)

•	Copy C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\ChemScriptUtils.py to C:\Python25\lib


•	Run Update_ChemReg_DB_From_9.0SR3_to_9.0SR4.cmd, or 

Update_ChemReg_DB_From_9.0SR2_to_9.0SR3.cmd then

Update_ChemReg_DB_From_9.0SR3_to_9.0SR4.cmd

•	Install ChemScript web service
C:\Inetpub\wwwroot\ChemOffice\Chem_reg\ChemScript\PyEngine\PyEngine, create a virtual directory PyEngine from IIS. 

•	In the IIS properties of PyEngine choose to run it under .Net 2.0.

•	Replace search_func_vbs.asp 
form_val_js.js
Display_func_vbs.asp
in core with the ones in Chem_reg
 

•	In cfserver.ini, these are the new settings:
MOLECULE_PROCESS=1
	MOLECULE_PROCESS_SCRIPT=C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScri	pt\ChemScriptParent.py



•	Config C:\Inetpub\wwwroot\ChemOffice\Chem_Reg\ChemScript\PyEngine\PyEngine\web.config to be

<add key=”log” value=”on” />
<add key=”logfolder” value=”C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\log\” />

<add key=”logdays” value=”30” /> 
<add key=”debug” value=”on” /> -- set this to off if too much log info
<add key=”pythoncoreversion” value=”2.5” />
 
<identity  impersonate=”true” userName=”camsoft_admin”  password=”cambridgesoft” /> -- This is the server admin account

•	Manage salt table via chemfinder
Open C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\Salts.cfw to add and edit salts in Registration Salts table.

After adding or editing in Salts.cfw, it needs to be exported as Salts.sdf. If error when exporting, process aspnet_wp.exe needs to be killed from Task Manager.

•	Manage solvate table via chemfinder
Open C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\Solvates.cfw to add and edit solvates in Registration Solvates table.

After adding or editing in Solvates.cfw, it needs to be exported as Solvates.sdf. If error when exporting, process aspnet_wp.exe needs to be killed from Task Manager.

•	When adding new salts to salts.cfw, do the following steps:
o	Put salt name then commit
o	Close the chemfinder form and reopen it
o	Put salt structure and Active = 1 (Remember to put Active = 1, this is new from last build, because a trigger to calculate MF and MW is based on insert of this field. I may find a better work around, but for now it is important to put Active = 1 together with structure)
o	Save and export as sdf


•	Testing suggestions

•	General ChemScript



•	Salt Stripping 
Useful information
http://scandium/ChemCentral/services/salts/salt_identification_and_splitting.htm

http://scandium/ChemCentral/services/salts/customFragments.htm


•	After edit saltsTable.cfw and solvates.cfw, you need to export them as sdf for ChemScript to read.

•	Create your own ChemScript and name it ChemScriptParent.py and test

•	BATCH_LEVEL = SALT and COMPOUND both need to be thoroughly tested.

•	Alternative required fields

To set up the required fields and the alternative required fields, edit following entries in reg.ini:


[REG_CTRBT_FORM_GROUP]
REQUIRED_FIELDS – This specifies the required fields when compound_type is “in house”
REQUIRED_FIELDS_ALTERNATIVE – This specifies the required fields when compound_type is “acquired”

For example,
REQUIRED_FIELDS=Temporary_Structures.Amount;0:Amount, Temporary_Structures.Vendor_Name;0:Vendor Name

REQUIRED_FIELDS_ALTERNATIVE=Temporary_Structures.Appearance;0:Appearance

The general format is:
TableName.FieldName;FieldType:AlertText,TableName.FieldName2;FieldType2:AlertText2
					
	CS Reg Fields	CS Reg Display Name	Cara Field	Required	Comments
	Structure	Structure	OBJSMOLFILE	Y	Empty structure field and NS are okay
	Registration Number	Corporate ID	CR_NUM	Y	 
	Batch Number	Batch	BATCH	Y	 
	Project	 	 	 	Pre-populate project list
	Prefix	Prefix	 	 	prefix will be CR
	CAS No	CAS No	CAS_NUM	 	 
Batch or parent?	MW Text	 	BATCH_MW	 	 
Batch or parent?	MF Text	 	BATCH_MF	 	 
	Custom_Cmpd_Text fields	 	 	 	 
	Custom_Cmpd_Num fields	 	 	 	 
	Structure/Stereochemistry Comment	 	 	 	 
	Chemical Name	Chemical Name	NAME	 	 
	Synonym	 	 	Y	 
	Salt Name	 	SALT	 	 
	Salt MW	 	 	 	 
	Salt Equivalents	 	SALT_EQ	 	 
	Solvate Name	 	SOLVATE	 	 
	Solvate MW	 	 	 	 
	Solvate Equivalents	 	SOLVATE_EQ	 	 
	Chemist	Chemist	SUBMITTER	Y - Synthesis	Last Name, Initial of First Name
	Purity	 	PURITY	Y - Synthesis	 
	Appearance	 	 	Y - Aquired	 
	Creation Date	 	 	Y	MM/DD/YYYY format
	Notebook	 	NOTEBOOK	Y - Synthesis	 
is this alphanumeric?	Page	 	PAGE	Y - Synthesis	 
	Submitted Amount	 	INITIAL_AMOUNT	 	 
	Units	 	"mg"	 	 
	Reference and Vendor Data	Vendor Lot ID	SUPPLIER_BATCH	Y - Aquired	 
	Vendor Name	Vendor	SUPPLIER	Y - Aquired	*Dropdown List
	Vendor ID	Vendor ID	SUPPLIER_NUM	Y - Aquired	 
	Batch Comment	 	 	Y - Synthesis	 
	Preparation	 	 	 	 
	Storage Requirements and Warnings	 	 	 	 
	HNMR	 	 	Y - Synthesis	 
	CNMR	 	 	Y - Synthesis	 
	HPLC	 	 	 	 
	FIA/MS	 	 	 	 
	LC/UV/MS	 	 	Y - Synthesis	 
	IR	 	 	 	 
	UV	 	 	 	 
	GC	 	 	 	 
	CHN	 	 	 	 
	MP	 	 	 	 
	BP	 	 	 	 
	Solubility	 	 	Y - Synthesis	 
	Optical Rotation	 	 	 	 
	Physical Form	 	PHYSSTATE	 	 
	LogD	 	 	 	 
	Refractive Index	 	 	 	 
	Flash Point	 	 	 	 
	Color	 	COLOUR	Y - Synthesis	 
	Custom_Batch_Text fields	 	 	 	 
	Custom_Batch_Num fields	 	 	 	 
					
					
	Library ID	Library ID	LIBRID		
					
					
					
					
					
					
	Supplier, Supplier ID number, supplier Lot number or Batch number, LibraryID, supplier Bottle ID, CAS number, plate ID, well ID, alias, registration date and appearance are required fields  for acquired compounds		Library, Plate, Well		
	Synthesized By, Synthesis Date, Notebook and page reference, purity, analytical data which might include NMR, HPLC and LCMS, solubility, vial ID, plate id, well id, compound comment field, batch comment field, library ID, alias, registration date, who registered it, and appearance (color and form) are required fields for in-house synthesized compounds		Library, vial id, Plate, Well, Registration date, registered by, compound comments

