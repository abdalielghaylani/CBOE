CREATE OR REPLACE
FUNCTION "DFCI_GETQTYREMAININGATDATE"
  (pContainerID inv_containers.container_id%TYPE,
   pTimestamp date,
   pInterval number
)
RETURN varchar2
IS
cursor audit_curs is select AUDIT_ROW.timestamp,TO_NUMBER(NEW_VALUE),TO_NUMBER(OLD_VALUE), date_created from INV_CONTAINERS, AUDIT_ROW, AUDIT_COLUMN where INV_CONTAINERS.CONTAINER_ID=pContainerId and INV_CONTAINERS.RID=AUDIT_ROW.RID AND AUDIT_ROW.RAID=AUDIT_COLUMN.RAID AND TABLE_NAME='INV_CONTAINERS' and COLUMN_NAME='QTY_REMAINING' order by timestamp desc; 
vDate DATE;
VQtyRemaining number;
vOldQtyRemaining number;
vMatch number :=-1;
vInterval number:=0;
vCount number := 0;
vDateCreated date;
BEGIN
open audit_curs;
loop
fetch audit_curs into vDate, vQtyRemaining,vOldQtyRemaining, vDateCreated;
exit when audit_curs%NOTFOUND;
--exit when vmatch>-1;
--exit when vInterval>pInterval;
exit when vDateCreated>pTimestamp; 

	if vMatch=-1 then
	if pTimestamp>=vDate then
		if vInterval=pInterval then
			vMatch:=vQtyRemaining;
		end if; 
    vInterval :=vInterval + 1;
    end if;
    end if; 
    vCount:=vCount + 1;
    
end loop;

close audit_curs;

if vMatch=-1 then
 	if vCount = 0 then -- grab the exisitng value if there are no audit records
	select qty_Remaining into vMatch from INV_CONTAINERS where container_id=pContainerId;
	elsif vDateCreated<=pTimeStamp then 
			vMatch := vOldQtyRemaining; --grab the old value from the earliest record.
	end if;
end if;   
 
  /* otherwise, the container didnt exist, return a -1 (vMatch not updated from original value) */
    
RETURN vMatch;

exception
WHEN OTHERS then
	RETURN -100;
END "DFCI_GETQTYREMAININGATDATE";
/
