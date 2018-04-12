CREATE OR REPLACE
Function              "GENABCFEED"
  (pPONumber IN inv_containers.po_number%TYPE
	) 
return clob
IS

                 
vacx_id varchar(15);
vqty number;
vClobReturn CLOB:= '';
vCount number;
vCustomerNumber varchar2(255);
recCount number :=0;
vDeliveryLocationId number;
 
cursor po_feed is select acx_id, qty_remaining, delivery_location_id_fk from inv_containers, inv_compounds, inv_Container_order where inv_containers.container_id=inv_Container_order.container_id and inv_containers.compound_id_fk=inv_compounds.compound_id and po_number=pPONumber and Location_id_fk= 1 ; -- 1 is on order location

BEGIN



open po_feed;
	Loop
	Fetch po_feed into vacx_id,vqty,vDeliveryLocationId;
	Exit when po_feed%NOTFOUND;
	vClobReturn:=vClobReturn||'+AAD'||vacx_id||'+AAF'||vqty;
	end loop; 
-- get the customer number from the first item in the list.  after that, ignore.
	if recCount=0 then
		select count(*) into vCount from inv_locations where rownum=1 and location_type_id_fk in (1000,1001) start with location_id=vDeliveryLocationId connect by prior parent_id=location_id;
		if vCount=1 then
		select location_Description into vCustomerNumber from inv_locations where rownum=1 and location_type_id_fk in (1000,1001) start with location_id=vDeliveryLocationId connect by prior parent_id=location_id;
		recCount:=recCount+1;
		else
		vCustomerNumber:='UNKNOWN';
		end if;
	end if;
		
	recCount:=recCount+1;
	
 --Append customer # and po number	

close po_feed;
	vClobReturn:='AAA'||vCustomerNumber||'+AAC'||pPONumber||'+'||vClobReturn;
	
return vClobReturn;  

END "GENABCFEED";
/
 

