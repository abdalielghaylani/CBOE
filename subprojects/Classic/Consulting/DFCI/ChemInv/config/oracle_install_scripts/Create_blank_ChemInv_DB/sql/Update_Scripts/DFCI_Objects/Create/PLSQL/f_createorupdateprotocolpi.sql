CREATE OR REPLACE FUNCTION "CREATEORUPDATEPROTOCOLPI"
(
 pProtocolId In inv_protocol.protocol_id%type,
 pProtocolPIId in inv_protocol_pi.protocol_pi_id%type,
 pPI in inv_protocol_pi.pi%type,
 pPINCINUM in inv_protocol_pi.pi_nci_num%type,
 pStartDate IN inv_protocol_pi.start_date%Type,
 pEndDate IN inv_protocol_pi.end_date%Type
 )
return Inv_protocol_pi.protocol_pi_ID%Type
IS

vCount number;
vProtocolPiId number;              

vStartDate date;
vEndDate date;   

updateSql varchar2(2000):='';

date_issue exception;      
date_issue_overlap exception;     
no_protocol_exists exception;

BEGIN 

if pProtocolId is not null then 
select count(*) into vCount from inv_protocol where protocol_id=pProtocolId;
	if vCount!=1 then 
		raise no_protocol_exists;
	end if;
end if ;


if pStartDate is not null and pEndDate is not null then
if pStartDate>pEndDate then 
raise date_issue;
end if;
end if;  

                                     

if pProtocolId is not null then
   select count(*) into vCount from inv_protocol_pi where protocol_pi_id = pProtocolPiId;
          if vCount=0 then
       /*   	select count(*) into vcount from inv_protocol_pi where 
          		((start_date >= pStartDate and (Start_Date < penddate or penddate is null))  or
          		(End_Date > pstartdate and (End_Date < penddate or pendDate is null))) and protocol_id_fk = pProtocolId;
          		if vCount>0 then
          			raise date_issue_overlap;
          		else */
          		insert into inv_protocol_pi (protocol_id_fk,PI, PI_NCI_NUm, start_date, end_date) values (pProtocolId, pPI, pPINCINUM, pStartDate, pEndDate) returning protocol_pi_id into vProtocolPiId;
			/*	end if;
		*/		
				
		else

          /*	select count(*) into vcount from inv_protocol_pi where 
           ((start_date > pStartDate and (Start_Date < penddate or penddate is null))  or
          		(End_Date > pstartdate and (End_Date < penddate or pendDate is null)))and protocol_id_fk=pProtocolId and protocol_pi_id != pProtocolPiId;
          		if vCount>0 then
          			raise date_issue_overlap;    		
          		else

				*/
				updateSql:= ' pi = ' || chr(39) || pPI  || chr(39);
				updateSQL := updateSql || ',' ;
				updateSql:=updateSql || ' Start_Date = ' || chr(39) || pStartDate || chr(39);
				updateSQL := updateSql || ',' ;
				updateSQL := updateSql || ' end_date = ' || chr(39) || pEndDate || chr(39)	;
				updateSQL := updateSql || ', PI_NCI_NUM = ' || chr(39) || pPINCINUM || chr(39) ;
				if length(updateSql)>0 then
				execute immediate 'update inv_protocol_pi set ' || updateSql || ' where protocol_pi_id = ' || pProtocolPiId;
				end if;
    		/*	end if; */
		  end if;
end if;


RETURN vProtocolPiId;
exception
WHEN no_protocol_exists then
  RETURN -9603;
WHEN date_issue then
  RETURN -9601;
WHEN date_issue_overlap then
  RETURN -9602;
When Others then
 return -9600;
END "CREATEORUPDATEPROTOCOLPI";

/
