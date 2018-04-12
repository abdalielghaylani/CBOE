cREATE OR REPLACE FUNCTION "CREATEORUPDATEPROTOCOL"
(
 pProtocolId In inv_protocol.protocol_id%type,
 pProtocolIdentifier IN inv_protocol.protocol_identifier%Type,
 pProtocolName in inv_protocol.protocol_name%type,
 pNCIProtocolNum in inv_protocol.nci_protocol_num%type,
 pStartDate IN inv_protocol.start_date%Type,
 pEndDate IN inv_protocol.end_date%Type
 )
return Inv_protocol.protocol_ID%Type
IS

vCount number;
vProtocolId number;              

vStartDate date;
vEndDate date;   

updateSql varchar2(2000):='';

protocol_exists exception;
date_issue exception;

BEGIN 

select count(*) into vCount from inv_protocol where protocol_identifier=pProtocolIdentifier;

if vCount>0 and pProtocolId is null then
raise protocol_exists;
end if;

if pStartDate is null and pEndDate is null then
if pStartDate>pEndDate then 
raise date_issue;
end if;
end if;                                       

if vCount=0 and pProtocolId is null then
	insert into inv_protocol (protocol_identifier, protocol_name, nci_protocol_num, start_date, end_date) values (pProtocolIdentifier, pProtocolName, pNCIProtocolNum, pStartDate, pEndDate) returning protocol_id into vProtocolId;
else
	if pProtocolId is null then 
	select protocol_id, Start_Date, End_Date into vProtocolId, vStartDate, vEndDate from inv_protocol where protocol_identifier=pProtocolIdentifier;
	else
	select Start_Date, End_Date into vStartDate, vEndDate from inv_protocol where Protocol_id=pProtocolId;
	vProtocolId:=pProtocolId;
	end if;
	   
	if pprotocolId is not null and pProtocolIdentifier is not null then
		updateSql:= ' protocol_identifier = ' || chr(39) || pProtocolIdentifier  || chr(39);
	end if;
	
	if vStartDate!=pStartDate or pStartDate is null then
		if length(updateSql)>0 then 
			updateSQL := updateSql || ',' ;
		end if ;
		updateSql:=updateSql || ' Start_Date = ' || chr(39) || pStartDate || chr(39);
	end if;
	
	
	if vEndDate != pEndDate or vEndDate Is Null then 
		if length(updateSql)>0 then 
			updateSQL := updateSql || ',' ;
		end if;
		updateSQL := updateSql || ' end_date = ' || chr(39) || pEndDate || chr(39)	;
	end if;

	if pProtocolName is not null then
		if length(updateSql)>0 then 
			updateSQL := updateSql || ',' ;
		end if;
		updateSQL := updateSql || ' protocol_name = ' || chr(39) || pProtocolName || chr(39)	;
	end if;	

	if pNCIProtocolNum is not null then
		if length(updateSql)>0 then 
			updateSQL := updateSql || ',' ;
		end if;
		updateSQL := updateSql || ' nci_protocol_num = ' || chr(39) || pNCIProtocolNum || chr(39)	;
	end if;	
		


	if length(updateSql)>0 then
		execute immediate 'update inv_protocol set ' || updateSql || ' where protocol_id = ' || vProtocolId;
	end if;


end if;


RETURN vProtocolId;
exception
WHEN date_issue then
  RETURN -9601;
WHEN protocol_exists then
  RETURN -9602;
when others then
 return -9600;
END "CREATEORUPDATEPROTOCOL";

/
