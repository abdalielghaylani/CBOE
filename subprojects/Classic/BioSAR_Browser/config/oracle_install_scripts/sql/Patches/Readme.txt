How to create the scripts for a new patch?

	1) Create a new folder into sql\Patches\ using the name "Patch x.x.x". (x.x.x is the new version)
	2) Copy file/s from sql\Patches\Patch Template\ to the new folder
	3) Edit and update:
		a) the PATCH_chemreg_ora.sql file copied
			replace 'x.x.x' with the new version number.
         	   	You should not modify the y.y.y string
		b) the PATCH_chemreg_ora.sql file of the previous version folder: 
			replace 'y.y.y' with the new version number.
		c) the sql\Patches\parameters.sql file: 
			replace the current value of the LastPatch variable to new version number

	4) Edit the PATCH_chemreg_ora.sql file copied in ne folder to run all changes. 
	   If is necesary create new files and puts these in new folder and then to add the called in PATCH_chemreg_ora.sql
	   You should not be to be necesary call to files of other folders.
	   
