CREATE OR REPLACE
PROCEDURE "DFCI_INVENTORYUPDATE_IDS"
(
pSequenceNum number,
pLocationBarcode varchar2,
pBDPP IN inv_compounds.alt_id_1%type,
pProtocol inv_protocol.protocol_identifier%type,
pPI	IN inv_coMPOUNDS.alt_id_1%type,
pDispensingPharmacist inv_containers.field_1%type,
pContainerBarcode inv_containers.barcode%type,
pMRN inv_containers.field_1%type,
pLotNum in inv_containers.lot_num%type,
pQTY in inv_containers.qty_remaining%type,
pDose in transaction_Record.dose%type,
pTransType varchar2)
IS
retval integer:=0;
retval2 integer:=0;
row_c INTEGER;
vCount integer;
vCompoundId INTEGER;
vDrugType varchar2(20);
vLocationId inv_locations.location_id%type;
vSumQtyRemaining inv_containers.qty_remaining%type;
vQueryLotNum inv_containers.lot_num%type := null;
vSql varchar2(2000);
vUpdateRemaining inv_containers.qty_remaining%type;
vSequenceNum number(10);
vNEW_IDS varchar(2000);
vUOM varchar(100);
vCompoundName varchar(255);
vNewContainerId inv_containers.container_id%type;
vMRNSearch number;
vContainerSearch number;
vContainerId inv_containers.container_id%type := null;
mrn_Required integer;
id_required integer;
vProtocolId inv_compounds.alt_id_3%type;  
vContainerMax number;
vMRN inv_containers.field_1%type;



cursor ids_wmrn_curs is select container_id, lot_num, qty_available from inv_containers where compound_id_fk=vCompoundId and location_id_fk=vLocationId and lot_num=vQueryLotNum and field_5=vMRN and container_status_id_fk = 1 order by date_created asc;
cursor ids_curs is select container_id, lot_num, qty_available from inv_containers where compound_id_fk=vCompoundId and location_id_fk=vLocationId and lot_num=vQueryLotNum and field_5 is null and container_status_id_fk = 1 order by date_created asc;


ids_rs ids_curs%rowtype;

BEGIN
--INSERT INTO TRANSACTION TABLE
select COUNT(*) INTO row_c from transaction_record where sequence_number = pSequenceNum and transaction_status=1 and rownum=1;

if row_c>0 then
retval:=-9100; -- Transaction already has completed successfully.
end if ;

-- Additoinal checks:
-- MRN passed
-- Dose passed
-- Patients Initials passed
-- Dispensing pharmacist passed
-- bdpp passed
-- protocol passed

--NO MRN CHECK
--if pMRN is null then
--retval:=-10001; -- no mrn
--end if;

if pDose is null then
retval:=-10002; --no dose
end if;

if pPI is null then
retval:=-10003; -- no PI
end if;

if pDispensingPharmacist is null then
retval:=-10004; -- no dispensing pharmacist
end if;

if pBDPP is null then
retval:=-10005; -- no bdpp
end if;

if pProtocol is null then
retval:=-10006; -- no protocol
end if;

INSERT INTO transaction_record (internal_seq_num, sequence_number, location_barcode,container_barcode,
		     bdpp, protocol_id,mrn,pi,lot_num,qty, dispensing_pharmacist, dose, transaction_type, transaction_status)
		     values (seq_transaction_record.nextval, pSequenceNum,pLocationBarcode, pContainerBarcode, pBDPP,
		     pProtocol,pMRN,pPI, pLotNum, pQTY,pDispensingPharmacist, pDose, pTransType, 0) returning internal_seq_num into vSequenceNum;  --returning this trans num for updates later.


-- DETERMINE THE TYPE OF TRANSACTION
-- TYPES ARE:
-- COMMERCIAL
-- COMMERCIAL WITH LOT_NUM (IE, VACCINES, BLOOD)
-- IDS WITH LOT_NUM
-- IDS CONTAINER SPECIFIC

-- this section applies to ALL transactions

-- if pQTY is negative return an error
--if pQTY<0 then
--	retval:=-9007;
--end if;

-- if no error, get the location information
if retval>=0 then
	SELECT COUNT(*) INTO row_c FROM inv_locations WHERE location_barcode=pLocationBarcode;
	IF row_c=1 then
		--Get the compound Id
		SELECT location_id INTO vLocationId FROM inv_locations WHERE location_barcode=pLocationBarcode;
	elsif row_c>1 then
		retval:=-9002; -- location multiple matches
	else
		retval:=-9003; -- location no matches
	end if;
end if;

-- IDS get information from inv_compounds table and supporting tables.
-- OTHERWISE, THE LOT_NUM IS OPTIONAL--IE WE DECREMENT IF IT IS THERE
-- OR IF NOT, ADD THE LOT NUM TO A FIELD SO WE CAN SEE IT IN THE TRANSACTION.

-- Check to see if the ndc exists
if retval>=0 then
	SELECT COUNT(*) INTO row_c FROM inv_compounds WHERE inv_compounds.alt_id_1=pBDPP and alt_id_5='1001' and alt_id_3=pProtocol;
	IF row_c=1 then
		--Get the compound Id and related information
		SELECT compound_id,alt_id_5, alt_id_3, Substance_Name, nvl(container_uom_id_fk,76) , case when id_required.picklist_id_fk is null then 0 else 1 end as id_required,
		case when mrn_required.picklist_id_fk is null then 0 else 1 end as mrn_required
		into vCompoundId,vDrugType,vProtocolId,vCompoundName,vUOM,id_required,mrn_Required
		FROM inv_compounds, inv_custom_cpd_field_values id_required, inv_custom_cpd_field_values mrn_required, chemacxdb.packagesizeconversion psc
		WHERE inv_compounds.compound_id=id_required.compound_id_fk (+) and id_required.custom_field_id_Fk (+) = 100 AND
		inv_compounds.compound_id=mrn_required.compound_id_fk (+) and mrn_required.custom_field_id_Fk (+) = 101 AND
		inv_compounds.package_size=psc."SIZE_FK"(+) AND
		alt_id_1=pBDPP AND alt_id_5 ='1001'
		and alt_id_3=pProtocol
		--need also to check the protocol id ***
		;
	elsif row_c>1 then
		retval:=-9001; -- compounds multiple matches
	else
		retval:=-9000; -- compounds no matches
	end if;
end if;


if retval>=0 then
	if pLotNum is null then
		retval:=-10009; --lot number required for commercial drugs.
	else
		vQueryLotNum:=pLotNum;
	end if;

elsif vDrugType!='1001' then
retval:=-10010; --compound is not Investigational
end if;

if retval>=0 and mrn_required=1 and pMRN is null then 

retval:=-10101; -- MRN IS REQUIRED.
else       
-- this section either populates or nulls the mrn value for the cursor.
-- if mrn is not required, we should not look at containers with a defined mrn,
-- since these are associated to a patient.
--otherwise, all mrn nulls are ok. (open study)
	if mrn_required=1 then
		vMRN:=pMRN;
		else
		vMRN:= null;
	end if;
end if;

-- check to see if adequat qtys exist for the distribution
-- lot num required  -- always required
-- Protocol required -- checked in determining the substance id
-- location required -- always required
-- bdpp required -- checked in determining the substance id
-- MRN search optional (field_id 100);
-- Container search optional (field_id 101)



if retval>=0 then

	if (pTransType='decrement' and pQTY>0) then

		if retval>=0 then
			if pContainerBarcode is not null then

			if length(vMRN)>0 then
				select count(*) into vCount FROM inv_containers
				where container_status_id_fk=1
				and barcode=pContainerBarcode
				and compound_id_fk=vCompoundId
				and location_id_fk= vLocationId
				and field_5=vMRN
				and lot_num=vQueryLotNum
				;
				if vCount =0 then
			 		retval:=-10100; --Container id does not exist or has insufficient quantity.
				else
    				select Container_Id, nvl(sum(qty_available),0), count(*) into vContainerId, vSumQtyRemaining,vCount FROM inv_containers
						where container_status_id_fk=1
						and barcode=pContainerBarcode
						and compound_id_fk=vCompoundId
						and location_id_fk= vLocationId
						and field_5=vMRN
						and lot_num=vQueryLotNum
						group by container_id;
				
				end if;						
			else	
				select count(*) into vCount FROM inv_containers
				where container_status_id_fk=1
				and barcode=pContainerBarcode
				and compound_id_fk=vCompoundId
				and location_id_fk= vLocationId
				and field_5 is null
				and lot_num=vQueryLotNum
				;
				if vCount =0 then
			 		retval:=-10100; --Container id does not exist or has insufficient quantity.    
			    else
					select Container_Id, nvl(sum(qty_available),0) into vContainerId, vSumQtyRemaining FROM inv_containers
					where container_status_id_fk=1
					and barcode=pContainerBarcode
					and compound_id_fk=vCompoundId
					and location_id_fk= vLocationId
					and field_5 is null
					and lot_num=vQueryLotNum
					group by container_id;			 		
			 		
			 		
				end if;
			
			end if;
			
			else            
				if vMRN is not null then
				select nvl(sum(qty_available),0) into vSumQtyRemaining FROM inv_containers
					where container_status_id_fk=1
					and compound_id_fk=vCompoundId
					and location_id_fk=vLocationId
					and field_5=vMRN
					and lot_num=vQueryLotNum;								       
				else
				select nvl(sum(qty_available),0) into vSumQtyRemaining FROM inv_containers
					where container_status_id_fk=1
					and compound_id_fk=vCompoundId
					and location_id_fk=vLocationId
					and field_5 is null
					and lot_num=vQueryLotNum;
			    end if;
			end if;
    	end if;

if vSumQtyRemaining is null then 
vSumQtyRemaining:=0; 
end if;
		
		if retval>=0 then
			if vSumQtyRemaining<pQTY and pQTY>0 and pTransType='decrement' then
			retval:=-9005; -- not enough supply
			end if;
		end if;
		 
		if retval>=0 then
		vUpdateRemaining:=pQty;
			if vContainerId is not null then
				retval:=updatecontainer(vContainerId, 'qty_remaining='||to_char(vSumQtyRemaining-vUpdateRemaining)||'::field_10='||chr(39)||pSequenceNum||chr(39));
  				update inv_containers set field_10=null where container_id=vContainerId;

		--if qty left is zero, retire container.
				if vSumQtyRemaining-vUpdateRemaining = 0 then
			      	retval2:=retirecontainer(to_char(vContainerId),0,2,2);
				end if;
      				vUpdateRemaining:=0;
			else
      	
      			if vMRN is null then
      			open ids_curs;
      			loop
      				fetch ids_curs into ids_rs;
      				exit when vUpdateRemaining=0;
      				exit when ids_curs%notfound;
     				if ids_rs.qty_available>=vUpdateRemaining then
						retval:=updatecontainer(ids_rs.container_id, 'qty_remaining='||to_char(ids_rs.qty_available-vUpdateRemaining)||'::field_10='||chr(39)||pSequenceNum||chr(39));
        				update inv_containers set field_10=null where container_id=ids_rs.container_id;
						--if qty left is zero, retire container.
						if ids_rs.qty_available-vUpdateRemaining = 0 then
			      			retval2:=retirecontainer(to_char(ids_rs.container_id),0,2,2);
						end if;
      					vUpdateRemaining:=0;
      				else
        				retval:=updatecontainer(to_char(ids_rs.container_id), 'qty_remaining=0::field_10='||chr(39)||pSequenceNum||chr(39));
      					update inv_containers set field_10=null where container_id=ids_rs.container_id;
      					vUpdateRemaining:=vUpdateRemaining-ids_rs.qty_available;
      					retval2:=retirecontainer(to_char(ids_rs.container_id),0,2,2);
      				end if;
      			end loop;
      			close ids_curs;
				else
				open ids_wmrn_curs;
      			loop
      				fetch ids_wmrn_curs into ids_rs;
      				exit when vUpdateRemaining=0;
      				exit when ids_wmrn_curs%notfound;
     				if ids_rs.qty_available>=vUpdateRemaining then
						retval:=updatecontainer(ids_rs.container_id, 'qty_remaining='||to_char(ids_rs.qty_available-vUpdateRemaining)||'::field_10='||chr(39)||pSequenceNum||chr(39));
        				update inv_containers set field_10=null where container_id=ids_rs.container_id;
						--if qty left is zero, retire container.
						if ids_rs.qty_available-vUpdateRemaining = 0 then
			      			retval2:=retirecontainer(to_char(ids_rs.container_id),0,2,2);
						end if;
      					vUpdateRemaining:=0;
      				else
        				retval:=updatecontainer(to_char(ids_rs.container_id), 'qty_remaining=0::field_10='||chr(39)||pSequenceNum||chr(39));
      					update inv_containers set field_10=null where container_id=ids_rs.container_id;
      					vUpdateRemaining:=vUpdateRemaining-ids_rs.qty_available;
      					retval2:=retirecontainer(to_char(ids_rs.container_id),0,2,2);
      					vUpdateRemaining:=0;
      				end if;
      			end loop;
      			close ids_wmrn_curs;
			end if;			
		end if;      
	end if;	

	elsif pQTY<0 then
		vUpdateRemaining:=-1 * pQTY;
--adding additional container to contain the difference in inventory

		if pContainerBarcode is null then	
				insert into inv_containers(barcode,
				LOCATION_ID_FK,
				COMPOUND_ID_FK,
				CONTAINER_NAME,
				CONTAINER_COMMENTS,
				QTY_MAX,
				QTY_INITIAL,
				QTY_AVAILABLE,
				QTY_REMAINING,
				DATE_CREATED,
				CONTAINER_TYPE_ID_FK,
				CONTAINER_STATUS_ID_FK,
				CURRENT_USER_ID_FK,
				DEF_LOCATION_ID_FK,
				UNIT_OF_MEAS_ID_FK,
				LOT_NUM,
				FIELD_5, -- MRN,
				field_10 -- seqnum
				)
				values (
				barcodes.GetNextBarcode(constants.cContainerBarcodeDesc),
				vLocationId,
				vCompoundId,
				VCompoundName,
				'Inventory Adjustment',
				vUpdateRemaining+1,
				vUpdateRemaining,
				vUpdateRemaining,
				vUpdateRemaining,
				sysdate,
				6,
				1,
				'INVADMIN',
				vLocationId,
				nvl(vUOM,1),
				pLOTNUM,
				vMRN, -- if the mrn is required, then we should use this.  if not, the container should not be given an mrn.
				pSequenceNum
				) returning container_id into vNewContainerId;

--UpdateContainerBatches(vNewContainerID);

		else
    			select count(*) into vCount FROM inv_containers
				where barcode=pContainerBarcode;
				if vCount =0 then
					retval:=-10111; --Container id does not exist or has insufficient quantity.
				end if; 
    			select Container_Id, qty_available, qty_max into vContainerId, vSumQtyRemaining,vContainerMax FROM inv_containers
				where barcode=pContainerBarcode;
				if vContainerMax<vSumQtyRemaining+vUpdateRemaining then
					vContainerMax:= vSumQtyRemaining+vUpdateRemaining; 
				elsif vContainerMax=0 then
					vContainerMax:=1;			     
				end if;
		
				if retval>=0 then
				update inv_containers set field_10=pSequenceNum, qty_max=vContainerMax, container_status_id_fk=1, qty_available = vSumQtyRemaining + vUpdateRemaining, qty_remaining= vSumQtyRemaining + vUpdateRemaining, location_id_fk=vLocationId where barcode=pContainerBarcode; 
				end if;         
		end if;

	ELSE
	retval:=-5000; 

	end if;

END IF;



---IDS protocol
--UPDATE TRANSACTION TABLE WITH ANY ERRORS
if retval=0 then
UPDATE transaction_record SET transaction_status=1 where internal_seq_num=vSequenceNum;
else
UPDATE transaction_record SET transaction_status=retval where internal_seq_num=vSequenceNum;
end if;                

END "DFCI_INVENTORYUPDATE_IDS";


/
