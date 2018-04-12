

LineItem/Requiesion handling.
1) Copy the cheminv folder to the chemoffice web folder.
2) Run the file cheminv/config/oracle_install_Scripts/Create_blank_ChemInv_DB/sql/update_scripts/InstallBatchUpdate.cmd and respond to the prompts
3) In Invconfig.ini, set DEFAULT_CONTAINER_ORDER_STATUS=500 and iisreset.

At this point, the database objects for handling the requisition updates, etc should work fine.

BatchUpdate Installation
1) Create a workspace folder for the update processes on a usable drive.
2) Copy the batchupdate folder to that folder.
3) Configure the batchupdate process.
	1) start.bat contains the line "batchupdate.exe config=.\batchupdate.ini".  The path points to the config file.
	2) Batchupdate.ini file.
		1) Any number of processes can be configured.  the syntax is illustrated below:


[PO]     <-- name of the process
isSecure = false <-- https?
logFile = true <-- log process details?
deleteFile = true <-- delete when finished?
filePrefix="PO_" <-- prefix for the process files
fileExt="txt" <-- extension of process file.
readFolder="c:\\biogen\\new\\" <-- files from which the process will read
processFolder="c:\\biogen\\new\\" <-- files read are transferred here.
errorFolder="c:\\biogen\\new\\" <-- location to write error records.
doneFolder="c:\\biogen\\new\\" <-- location to write the completed file.
logFolder = "c:\\biogen\\new\\" <-- location for log files.
server = "BIOGENTEST" <-- server name
username = "CSSADMIN" <-- user to run the api call
userpass = "CSSADMIN" <-- password to run the api call
configParameters = "ContainerIds:ContainerIds,ValuePairs:PO_Number:string" <--parameters to pass the API call
configDefaults = "valuepairs:container_status_id_fk=1" <-- Parameters passed to ALL transactions
APICALL = "updatecontainer2.asp" <-- API call
linkedApp = "cheminv" <-- application for the API (actually the virtual directory path)

