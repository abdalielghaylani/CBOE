The Cambridgesoft.COE.Framework will look for it's centralized configuration file "Cambridgesoft.COE.Framework.config" at 
the special folder LocalApplicationData (C:\Documents and Settings\All Users\Local Settings\Application Data), in 
Cambridgesoft.COE.Framework subdirectory. 

This means that every time you get the latest version of this configuration file from source control, it has to be copied 
by hand from it's original location (CambridgeSoft\ServiceTier) to the LocalAplicationData location.

Alternatively, it's posible to create a hard link at the LocalApplicationData\Cambridgesoft.Coe.Framework directory pointing 
to the original file, using Windows XP fsutil. This is also possible for Windows 2000 and any system using NTFS >= 5.0

For example, logged in as user All Users: 

>fsutil hardlink create 
	C:\Documents and Settings\All Users\Local Settings\Application Data\Cambridgesoft.COE.Framework\Cambridgesoft.COE.Framework.config 
	C:\Projects\CambridgeSoft\ChemOfficeEnterprise\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.config

Note that this is NOT a regular shortcut.