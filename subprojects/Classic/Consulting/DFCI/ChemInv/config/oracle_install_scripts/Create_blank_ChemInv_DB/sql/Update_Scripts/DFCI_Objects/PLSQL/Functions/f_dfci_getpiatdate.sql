CREATE OR REPLACE
FUNCTION "DFCI_GETPIATDATE"
  (pContainerID inv_containers.container_id%TYPE,
   pTimestamp date,
   pOutput varchar2
)
RETURN varchar2
IS
cursor pi_curs is select pi, pi_nci_num, start_date, end_date from inv_protocol_pi where protocol_id_fk in (select protocol_id 
from inv_protocol,inv_containers, inv_compounds 
where inv_containers.compound_id_fk =inv_compounds.compound_id 
and inv_containers.container_id=pContainerId and protocol_identifier=alt_id_3);

vPIList varchar2(2000) := '';
vName varchar(255);
vNum varchar(255);
vCount number :=0;
vStart date;
vEnd date;

BEGIN
open pi_curs;
loop
fetch pi_curs into vName, vNum,vStart, vEnd;
exit when pi_curs%NOTFOUND;

if vStart<=pTimeStamp then
  
	if vEnd is null or vEnd>pTimestamp then
		if vCount>0 then 
			vPiList:=vPiList|| '::';
		end if; 
        if pOutput='ID' THEN
        vPiList :=vPiList || vNum;
        ELSE
  		vPiList :=vPiList || vName; --|| '\\' || vNum;
  		END IF;
  		vCount:=vCount + 1;
                                           
  	end if;
end if;  
  
end loop;

close pi_curs;

if vCount=0 then 
vPiList:='None Found';
end if;
    
RETURN vPiList;

exception
WHEN OTHERS then
	RETURN -100;
END "DFCI_GETPIATDATE";
/
