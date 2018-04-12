CREATE OR REPLACE
Function              "INSERTREQUESTS"
  (pLocationId IN inv_locations.location_id%TYPE
	) 
return integer
IS
vCount integer;
vLocationId integer;
vCompoundId integer;
vQtyAvailable number;
vRequested number;
location_error exception;
compound_error exception;
alreadyexists_error exception;
returnVal integer;
vBatchId inv_container_batches.batch_id%type;
vStock Number;

cursor process_all is select compound_id_fk, location_id_fk, amount from inv_forecast;
cursor process_location_only is select location_id_fk, compound_id_fk, amount from inv_forecast where location_id_fk = pLocationId;

forecast_row inv_forecast%rowtype;

BEGIN

IF pLocationId is not null then
--Location Id is passed.  Create/Update requests for this location_id only based on INV_FORECAST
vRequested:=0;
else

--Location Id is null, so create/update requests for all locations in INV_FORECAST
open process_all;
	Loop
	Fetch process_all into forecast_row;
	Exit when process_all%NOTFOUND;
	-- Find current amount for compound_id at this location;
	select count(*) into vCount from inv_containers where compound_id_fk=forecast_row.compound_id_fk and location_id_fk=forecast_row.location_id_fk;
	      insert into error_tab(errortext) values (forecast_row.Amount);
	      commit;   
	if vCount>0 then
		select sum(qty_available) into vQtyAvailable from inv_containers where compound_id_fk=forecast_row.compound_id_fk and location_id_fk=forecast_row.location_id_fk;				 	
     else
     	vQtyAvailable:=0;
    end if;
     
       insert into error_tab(errortext) values (vQtyAvailable);
	-- Check to see if there are current outstanding requests
 
 	select count(*) into vCount FROM inv_requests, inv_container_batches where inv_requests.batch_id_fk=inv_container_batches.batch_id
		and request_status_id_fk in (2,3) and inv_container_batches.batch_field_1=forecast_row.compound_id_fk and inv_requests.delivery_location_id_fk = forecast_row.location_id_fk;

	if vCount>0 then
		select sum(qty_required) into vRequested FROM inv_requests, inv_container_batches where inv_requests.batch_id_fk=inv_container_batches.batch_id
			and request_status_id_fk in (2,3) and inv_container_batches.batch_field_1=forecast_row.compound_id_fk and inv_requests.delivery_location_id_fk = forecast_row.location_id_fk;
		else
		vRequested:=0;	
	end if;
	
	  insert into error_tab(errortext) values (vRequested);
	  commit;       
	-- Find current amount for compound_id at the central location;
	select count(*) into vCount from inv_containers, inv_locations where compound_id_fk=forecast_row.compound_id_fk and inv_locations.location_id=inv_containers.location_id_fk and inv_locations.location_type_id_fk=1000;
	         
	if vCount>0 then
	select sum(qty_Available) into vStock from inv_containers, inv_locations where compound_id_fk=forecast_row.compound_id_fk and inv_locations.location_id=inv_containers.location_id_fk and inv_locations.location_type_id_fk=1000;
	
   else
     	vStock:=0;
    end if;
       insert into error_tab(errortext) values (forecast_row.compound_id_fk||'-'||forecast_row.location_id_fk|| '*' ||vStock);
                                                      commit;


	if (vRequested+vQtyAvailable)<forecast_row.amount and forecast_row.amount-(vRequested+vQtyAvailable)<vStock then
	-- Get the batch id     
		select count(*) into vCount from inv_container_batches where batch_field_1=forecast_row.compound_id_fk;	
		if vCount=1 then
			select batch_id into vBatchId from inv_container_batches where batch_field_1=forecast_row.compound_id_fk;	
		-- If the current amount does not meet par level requirements
		-- Insert new request if no request exists.
			returnVal :=  requests.createbatchrequest(vBatchId, forecast_row.amount - (vRequested+vQtyAvailable), sysdate, 'INVADMIN', null, forecast_row.location_id_fk,2,2,null,null,null,null,null,null,null);	
       		commit;
        else
        	returnVal:=-1;
		end if;			
	end if; 
	


	end loop;
close process_all;

End If;
return 0; 

Exception
	when location_error  then
		return -1000;
	when compound_error then
		return -1001;
	when alreadyexists_error then
		return 0;

END "INSERTREQUESTS";
/
