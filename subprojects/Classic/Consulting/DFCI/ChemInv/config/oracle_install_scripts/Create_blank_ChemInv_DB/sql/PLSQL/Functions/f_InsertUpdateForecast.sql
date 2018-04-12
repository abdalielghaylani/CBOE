CREATE OR REPLACE
Function              "INSERTUPDATEFORECAST"
  (pLocation IN inv_locations.location_barcode%TYPE,
   pNDC IN inv_compounds.CAS%TYPE,
   pAmount IN INV_forecast.amount%TYPE,
) 
return integer
IS
vCount integer;
vLocationId integer;
vCompoundId integer;
location_error exception;
compound_error exception;
alreadyexists_error exception;
returnVal integer;
BEGIN



IF pLocation is not null then
select count(*) into vCount from INV_LOCATIONS where location_barcode=pLocation;
	if vCount=0 then
		select location_id into vLocationId from INV_LOCATIONS where location_barcode=pLocation;
	else
		raise location_error;
	end if;
end if;

if pNDC is not null then
select count(*) into vCount from INV_COMPOUNDS where CAS=pNDC;
--Need to add Protocol is null
	if vCount=0 then
		select compound_id into vCompoundId from INV_COMPOUNDS where CAS=pNDC;
--Need to add Protocol is null
	else
		raise compound_error;
	end if;
end if;


-- Check to see if there is already a row for this combination
select count(*) into vCount from inv_forecast where location_id_fk=vlocationid and compound_id_fk=vCompoundId;

if vCount=0 then  
	-- if not, then add a row
	insert into inv_forecast (location_id_fk, compound_id_fk,amount) values (vLocationId,vCompoundId,pAmount);
	
else --if pUpdateMode = 1 then -- if updatemode=1 then update value, otherwise, return error code
	update inv_forecast set Amount=pAmount where location_id_fk=vlocationid and compound_id_fk=vCompoundId;
	--returnVal:=1;
	--else
	--raise alreadyexists_error;
--end if;
end if;
returnVal:=0;

commit;
return returnVal;  

Exception
	when location_error  then
		return -1000;
	when compound_error then
		return -1001;
	when alreadyexists_error then
		return 0;

END "INSERTUPDATEFORECAST";
/
