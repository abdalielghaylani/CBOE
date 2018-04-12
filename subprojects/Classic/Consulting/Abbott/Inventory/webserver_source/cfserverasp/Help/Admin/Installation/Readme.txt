WebServer 2002/7.2 Service Release 2 Readme
For Microsoft Windows 2000 Server, Windows 2002 Advanced Server, or Windows NT Server
Copyright 1999-2002 CambridgeSoft Corporation

November 25, 2002

More up-to-date information can be found at:

http://www.cambridgesoft.com/support

Contents of this ReadMe Document:

I. Before Installing
II. Installation Instructions
III. Software Installed with WebServer 2002
IV. Documentation
V. WebServer Core - New Features and Release Notes
VI. ChemDraw Plugin version 7/Bracket issue
VII. ChemDraw Plugin version 7/Java Runtime Version Issue
VIII. Browser Information
IX. Software Development Kit / Application Program Interface
X. Technical Support
XI. Notices

=====================
I. Before Installing
=====================
We recommend that you make a backup of and uninstall or delete any previous versions of CambridgeSoft software before installing this version.  

The following software is necessary for some ChemOffice WebServer applications to function properly:
CambridgeSoft ChemDraw Pro Plugin/ActiveX Control 7.01 
Oracle ODBC Driver 8.01.07 OR Oracle OLEDB Driver 8.17.00 
SQLNet access to Oracle 8.16 or 8.17 database server 
Microsoft Access 2000 (required for ChemInv report writer)

For more information about requirements for a particular application, please see the "Minimum Requirements" in the individual application's Admin Guide.

Make sure your HTTP service is turned off.

==============================
II. Installation Instructions
==============================
Installation instructions for the ChemOffice WebServer and its applications can be found in a CHM files located on the CD
To view the files, double-click on it.  It will be opened with Microsoft's HTML Help Viewer Executable (C:\WINNT\hh.exe)

Installation instructions for ChemOffice WebServer 2002 can be found in:
	ChemOffice WebServer Admin Guide.chm

Before installing any ChemOffice WebServer applications, please see the application's readme file.

============================================
III. Software Installed with WebServer 2002
============================================
The following software is available on this CD:
	ChemOffice WebServer Core
	ChemReg: Chemical Registration Application
	ChemInv: Chemical Inventory Application
	Doc Manager
	ChemACX
	The Merck Index
	ChemOffice Ultra 6.2
	ChemOffice WebServer Documentation

==================
IV. Documentation
==================
User and Admin Documentation are available for the WebServer as well as each of its applications.  All PDF and CHM documentation can be found here:
	\chemoffice\appname\help\ * and \chemoffice\appname\help\Admin * respecitvely

The following is a list of documents available on this CD:

PDF:
	ChemOffice WebServer User's Guide.pdf
	ChemOffice 7.0.pdf - ChemInfo (ChemACX, Merck Index), ChemReg, ChemInv, Doc Manager
   	ChemDraw 6.0 Manual.pdf
   	Chem3D 6.0 Manual.pdf
	ChemFinder 6.0 Manual.pdf
CHM:
	ChemOffice WebServer Admin Guide.chm
	ChemOffice WebServer User's Guide.chm
	Chemical Registration Admin Guide.chm
	Chemical Registration User's Guide.chm
	Chemical Inventory Admin Guide.chm
	Chemical Inventory User's Guide.chm
	ChemACX Admin Guide.chm
	ChemACX User's Guide.chm
	Doc Manager Admin Guide.chm
	Doc Manager User's Guide.chm
	The Merck Index User's Guide.chm

Online HTML based help is also available after installation:
	User's Guide: can be accessed from the application Help button or at http://hostname/appname/help/deafult.htm  *
	Admin Guide: http://hostname/appname/help/Admin/default.htm  *

*Please note that hostname refers to the hostname of your server and appname refers to the application name:
        ChemOffice Webserver Core: webserver_source\cfserverasp
	Chemical Registration: chem_reg
	Chemical Inventory: Cheminv
	ChemACX: chemacx
	BioSAR Browser: biosar_browser
	The Merck Index: themerckindex

For more information about the online documentation for a particular application, please see the readme file for that application.

===================================================
V. WebServer Core - New Features and Release Notes
===================================================
New Features
- Marked Hits stored in User_Settings Tables. 
- Additional Structure Search Preferences
- Upgraded Installer technology
- Updated documentation
- Preferences stored once for an application rather then for each form set.
- Export Hits stores subtable data as concatenated list rather then as separate records
- New CS_Security Application for CambridgeSoft Enterprise applications (Chemical Registration, Chemical Inventory and BioSar)
- Duplicate checking preferences can be set in cfserver.ini for Enterprise APplications 
- Record Count at Session level can be set in cfserver.ini as alternative to Global Application scope 

Release Notes
- CSBR-27935: problem detecting browser properly when hotbar is installed	
- CSBR-29129: Global Search: relational field mappings incorrect if not identical between apps.
- CSBR-29150: Admin Tools cant publish CF 7.0 databases
- CSBR-29812: MF incorrect when compound has associated charge
- CSBR-30043: CannÕt enter MW range when both numbers have decimals
- CSBR-30044: After a field validation alert appears, no other validation is performed
- CSBR-30509: Search preferences for each type of searching different
- CSBR-30677: > or < seen as = when searching numeric field other than MW
- CSBR-30951: Reaction Searching does not work properly 
- CSBR-31086: calculated mw shows commas instead of decimals
- CSBR-32134: changing preferences then using forward arrow drops some hits from the lists
- CSBR-32157: IE 5.0: clear marked button not working
- Upon logging in when usign Netscape 4.7x, you will recieve an error:Object moved: object may be found 'here'. The link brings the user the the CS Security page.
- Win98 Security Issue: In order to store cookies for login purposes, expiration times are not allowed. Therefore, users are not logged out after a certain amount of time. It is recommended that users logout after they are done with their session and that administrators/users periodically clear out their cookies.

===========================================
VI. ChemDraw Plugin version 7/Bracket issue
===========================================

Currently applications that allow adding of compounds will not be able to take advantage of the new polymer features of the ChemDraw version 7 Active X/Plugin. This includes the Sample application and the Chemical Registration system. 
By default, the user will be warned when a bracket is encounted, the warning indicating that the bracket will not be interpreted with any chemical meaning. The user can still continue to add or edit the record if they so choose.
The default behavior can be changed to disallow brackets in compounds all together, or allow brackets but not show a message at all.

By adding the following variable to the globals section of the cfserver.ini for the application the above alternate behaviors can be acheieved:

	* This is the default which will occur even in the absense of the variable:

	BRACKET_IN_STRUC_HANDLING=WARN 

	* User will be presented with an error indicating the structure cannot be added or edited with brackets.

	BRACKET_IN_STRUC_HANDLING=DISALLOW

	* No message will be presented and structure can be added

	BRACKET_IN_STRUC_HANDLING=ALLOW

===========================================
VI. ChemDraw Plugin version 7/ Java Runtime Version Issue
===========================================

A server with IE 5 and Java 2 Runtime environment installed will not work well with the ChemDraw pro plugin. Installing the Java 2 Runtime environment is necessary to correct an imcompatability between Oracle and Pentium4.  If it is necessary to instll the Java 2 Runtime environment, please uninstall it before installing the ChemOffice WebServer.

========================
VII. Browser Information
========================
Particular Applications may have extra constraints on browsers.  For more information about a particular application, please see the Readme file for that application.

The following versions of Internet Explorer are supported by WebServer 2002: 
	Version 5.x or later

The following versions of Netscape are supported by WebServer 2002:
	Version 4.x or later
	Online help requires Netscape 6.x

If the appropriate plugin is not installed, you will be pointed to a download page to obtain the appropriate plugin.  Your reg codes are necessary to download the PRO plugin.  
We do not recommend downloading the free NET plugin available for it will limit the function of all WebServer applications.
The Cambridgesoft security warning will appear twice on installing the cab-based plug-in installer. Check the 'trust cambridgesoft' box twice. 

==============================================================
VIII. Software Development Kit / Application Program Interface
==============================================================
Documentation describing the ChemOffice Software Development Kit (SDK) can be found at:
	http://sdk.cambridgesoft.com

Documentation describing the ChemOffice WebServer Application Program Interface (API) functions can be found in the Admin guide for the ChemOffice WebServer found online after installation at:
	http://hostname/help/Admin/default.htm  *

And as a CHM file at:  
	\ChemOffice\webserver_source\cfserverasp\help\Admin\ChemOffice WebServer Admin Guide.chm

Documentation describing the API for particular WebServer Applications can be found in the Admin guide for the application, found online after installation at:
	http://hostname/appname/help/Admin/default.htm  *

And as a CHM file at:
	\ChemOffice\webserver_source\cfserverasp\help\Admin\Chemical Registration Admin Guide.chm
	\ChemOffice\chem_reg\help\Admin\Chemical Registration Admin Guide.chm
	\ChemOffice\cheminv\help\Admin\Chemical Inventory Admin Guide.chm


*Please note that hostname refers to the hostname of your server and appname refers to the application name:
	ChemOffice Webserver Core: webserver_source/cfserverasp
	Chemical Registration: chem_reg
	Chemical Inventory: cheminv

========================
IX. Technical Support
========================
Please see the support section f the CambridgeSoft website for answers to Frequently Asked Questions about the WebServer and its applications:
http://www.cambridgesoft.com/support

To report bugs or ask questions, please use one of the following avenues:

Web Submission Form: recommended tech support mode.  It is a form which asks for particular information assuring our technical support department receives all required information.
	http://www.cambridgesoft.com/support/mail

Email:esupport@cambridgesoft.com

Post:	Technical Support Staff
	CambridgeSoft Corporation
	100 CambridgePark Drive
	Cambridge, MA 02140

Fax: (617) 588-9360

===============
X. Notices:
===============

Please read the WDDX General License on your ChemOffice WebServer CD for important license information.
