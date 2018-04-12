Create or replace FUNCTION "DFCI_CYCLECOUNTUPDATE"
(
pLocationId number,
pCompoundId number,
pQTY in inv_containers.qty_remaining%type,
pReason in transaction_record.transaction_input%type
) return varchar2

IS
retval integer:=0;
retval2 integer:=0;
row_c INTEGER;
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
pSequenceNum varchar2(10):=null; -- pass a null for the transaction id
pLotNum varchar2(10):=null;
vNewContainerId inv_containers.container_id%type;

cursor com_curs is select container_id, lot_num, qty_available from inv_containers where compound_id_fk=vCompoundId and location_id_fk=vLocationId and container_status_id_fk = 1 order by date_created asc;

com_rs com_curs%rowtype;

BEGIN
--INSERT INTO TRANSACTION TABLE
--select COUNT(*) INTO row_c from transaction_record where sequence_number = pSequenceNum and transaction_status=1 and rownum=1;
--No need to check since there is no transaction id


--if row_c>0 then
--retval:=-9100; -- Transaction already has completed successfully.
--end if ;
--No need to check since there is no transaction id


INSERT INTO transaction_record (internal_seq_num, sequence_number, location_barcode, ndc, lot_num,qty, transaction_type, transaction_status, transaction_input) values (seq_transaction_record.nextval, pSequenceNum,to_char(pLocationId), to_char(pCompoundId), pLotNum, pQTY,'CYCLECOUNT', 0, pReason) returning internal_seq_num into vSequenceNum;  --returning this trans num for updates later.

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
SELECT COUNT(*) INTO row_c FROM inv_locations WHERE location_id=pLocationId;
IF row_c=1 then
--Get the compound Id
vLocationId:=pLocationId;
elsif row_c>1 then
retval:=-9002; -- location multiple matches
else
retval:=-9003; -- location no matches
end if;
end if;


-- Check to see if the ndc exists
if retval>=0 then
SELECT COUNT(*) INTO row_c FROM inv_compounds WHERE compound_id=pCompoundId AND alt_id_3 IS null AND alt_id_5 in ('1000');
IF row_c=1 then
--Get the compound Id
SELECT compound_id,alt_id_5, container_uom_id_fk, Substance_Name INTO vCompoundId, vDrugType, vUOM, vCompoundName FROM inv_compounds, chemacxdb.packagesizeconversion psc WHERE inv_compounds.package_size=psc."SIZE_FK" (+) AND compound_id=pCompoundId AND alt_id_3 IS null AND alt_id_5 in ('1000');
elsif row_c>1 then
retval:=-9001; -- compounds multiple matches
else
retval:=-9000; -- compounds no matches
end if;
end if;




-- check to see if adequat qtys exist for the distribution
if retval>=0 then
select sum(qty_available) into vSumQtyRemaining FROM inv_containers where container_status_id_fk=1 and compound_id_fk=vCompoundId and location_id_fk= vLocationId and lot_num is null;
end if;

if vSumQtyRemaining is null then 
vSumQtyRemaining:=0; 
end if;

if retval>=0 then
if (pQTY<vSumQtyRemaining) then  -- decrement for update
vUpdateRemaining:=vSumQtyRemaining-pQty;

      open com_curs;
      loop
      fetch com_curs into com_rs;
      exit when vUpdateRemaining=0;
      exit when com_curs%notfound;
      if (com_rs.lot_num is null) then -- skip containers that have have lot nums 
      if com_rs.qty_available>=vUpdateRemaining then

		retval:=updatecontainer(com_rs.container_id, 'qty_remaining='||to_char(com_rs.qty_available-vUpdateRemaining)||'::field_10='||chr(39)||pSequenceNum||chr(39));
        update inv_containers set field_10=null where container_id=com_rs.container_id;
      	vUpdateRemaining:=0;
      else
        retval:=updatecontainer(to_char(com_rs.container_id), 'qty_remaining=0::field_10='||chr(39)||pSequenceNum||chr(39));
      	update inv_containers set field_10=null where container_id=com_rs.container_id;
      	vUpdateRemaining:=vUpdateRemaining-com_rs.qty_available;
      	retval2:=retirecontainer(to_char(com_rs.container_id),0,2,2);
      end if;
      --else just move to the next container.
      end if;
  commit;

      end loop;
      close com_curs;

elsif pQTY>vSumQtyRemaining  then
vUpdateRemaining:=pQTY-vSumQtyRemaining;
--adding additional container to contain the difference in inventory
insert into cheminvdb2.inv_containers(
barcode,
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
UNIT_OF_MEAS_ID_FK)
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
nvl(to_number(vUOM),1)) returning container_id into vNewContainerId;

UpdateContainerBatches(vNewContainerID);

  else
  --no update necessary, be we want to update the transaction record regardless
 retval:=-5000;
 -- retval:=-5000; --no update necessary
end if;
end if;



-- Update par level ndc / location cycle count field

--UPDATE TRANSACTION TABLE WITH ANY ERRORS OR UPDATE STATUS AS COMPLETE
if retval=0 then
UPDATE transaction_record SET transaction_status=1 where internal_seq_num=vSequenceNum;
else
UPDATE transaction_record SET transaction_status=retval where internal_seq_num=vSequenceNum;
end if;

return retval;

END "DFCI_CYCLECOUNTUPDATE";
/
