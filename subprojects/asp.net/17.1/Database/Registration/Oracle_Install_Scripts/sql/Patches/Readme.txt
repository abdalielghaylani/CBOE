--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

How to create the scripts for a new patch?

	1) Create a new folder into sql\Patches\ using the name "Patch x.x.x". (x.x.x is the new version)
	2) Copy files from sql\Patches\Patch Template\ to the new folder
	3) Edit and update:
		a) the parameters.sql file copied
			replace 'x.x.x' with the new version number.
			replace 'z.z.z' with the new "application" version number.
         	   	You should not modify the y.y.y string
		b) the parameters.sql file of the previous version folder: 
			replace 'y.y.y' with the new version number.
		c) the sql\Patches\parameters.sql file: 
			replace the current value of the LastPatch variable to new version number
			
	4) Edit the Patches.sql file copied in new folder to run all changes. 
	   If is necesary create new files and puts these in new folder and then to add the called in patch.sql
	   You should not be to be necesary call to files of other folders.
	   
