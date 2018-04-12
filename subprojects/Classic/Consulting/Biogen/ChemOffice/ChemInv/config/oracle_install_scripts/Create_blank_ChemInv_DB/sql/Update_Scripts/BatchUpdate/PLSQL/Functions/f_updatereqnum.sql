create or replace function UPDATEREQNUM(pReqNumber inv_containers.req_number%type,
											pOrigQty number,
											pNewQty number,
											pValuePairs varchar2,
											pValuePairsAlt varchar2											
											) return varchar2
is

Cursor reqnum_curs is select container_id from inv_containers where req_number = pReqNumber;											
vContainerId number;
retval varchar2(2000);
vCount number;
vnumContPerPack number;
vNumProcessed number:=0; 
vNumContainers number;
vNewIds varchar2(32000); 
vOrigQty number;
vNewQty number;

begin



-- find out the number of contaienrs for that reqnumn
 select count(*) into vCount from inv_containers where  req_number = pReqNumber;
 
if vCount = 0 then
retval:='-1';
else
-- figure out the proportions of the package size based on number of containers.

if pOrigQty is null then
vOrigQty:=vCount;
else 
vOrigQty:=pOrigQty;
end if;


if pNewQty is null then 
vNewQty:=vOrigQty;
else 
vNewQty:=pNewQty;
end if;
if vCount/vOrigQty=Round(vCount/vOrigQty,0) then
vNumContPerPack:=vCount/vOrigQty;
vNumContainers:=vNumContPerPack * vNewQty;



-- get the first container_id
select container_id into vContainerId from inv_containers where req_number = pReqNumber and rownum=1;

-- if there are not enough containers for the order, create some.                                          
if vnumContainers<vCount then
retval:=copyContainer(vContainerId,vnumContainers-vCount, vNewIds);
vCount:=vnumContainers;
end if;
       
open reqnum_curs;


loop
fetch reqnum_curs into vContainerId;
exit when reqnum_curs%notfound;                   
if vnumProcessed < vnumContainers then
-- 
retval:=updateContainer(vContainerId,pValuePairs);
else
-- extra ones get updated as specified.
retval:=updateContainer(vContainerId,pValuePairsAlt);
end if;
vNumProcessed := vNumProcessed +1;

end loop;	
close reqnum_curs;                                   
retval:=vNewQty;   

else 
retval:='-1';
end if;	

end if;									

return retval;
exception when others then
return '-1';											
end;