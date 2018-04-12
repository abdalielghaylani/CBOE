Create or replace PROCEDURE "DFCI_INVENTORYUPDATE"
(
pSequenceNum number,
pLocationBarcode varchar2,
pNDC varchar2,
pLotNum in inv_containers.lot_num%type,
pQTY in inv_containers.qty_remaining%type,
pTransType varchar2)

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
vNewContainerId inv_containers.container_id%type;

cursor com_curs is select container_id, lot_num, qty_available from inv_containers where compound_id_fk=vCompoundId and location_id_fk=vLocationId and container_status_id_fk = 1 order by date_created asc;

com_rs com_curs%rowtype;

BEGIN
--INSERT INTO TRANSACTION TABLE
select COUNT(*) INTO row_c from transaction_record where sequence_number = pSequenceNum and transaction_status=1 and rownum=1;

if row_c>0 then
retval:=-9100; -- Transaction already has completed successfully.
end if ;

INSERT INTO transaction_record (internal_seq_num, sequence_number, location_barcode, ndc, lot_num,qty, transaction_type, transaction_status) values (seq_transaction_record.nextval, pSequenceNum,pLocationBarcode, pNDC, pLotNum, pQTY,pTransType, 0) returning internal_seq_num into vSequenceNum;  --returning this trans num for updates later.


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

-- COMMERCIAL
-- GET INFORMATION ABOUT DRUG FROM INV_COMPOUNDS TABLE

-- IF BLOOD PRODUCT OR VACCINE, A LOT NUMBER MUST BE PASSED (9100)
-- OTHERWISE, THE LOT_NUM IS OPTIONAL--IE WE DECREMENT IF IT IS THERE
-- OR IF NOT, ADD THE LOT NUM TO A FIELD SO WE CAN SEE IT IN THE TRANSACTION.

-- Check to see if the ndc exists
if retval>=0 then
SELECT COUNT(*) INTO row_c FROM inv_compounds WHERE cas=pNDC AND alt_id_3 IS null AND alt_id_5 in ('1000','1002','1003');
IF row_c=1 then
--Get the compound Id
SELECT compound_id,alt_id_5, container_uom_id_fk, Substance_Name INTO vCompoundId, vDrugType, vUOM, vCompoundName FROM inv_compounds, chemacxdb.packagesizeconversion psc WHERE inv_compounds.package_size=psc."SIZE_FK" (+) AND cas=pNDC AND alt_id_3 IS null AND alt_id_5 in ('1000','1002','1003');
elsif row_c>1 then
retval:=-9001; -- compounds multiple matches
else
retval:=-9000; -- compounds no matches
end if;
end if;

-- for blood and vac products check to see if a lot number was given, if so
-- move it to vQueryLotNum, otherwise leave that parameter null
if retval>=0 then
if ((vDrugType='1003' OR vDrugType='1002')) then
if pLotNum is null then
retval:=-9004; --no lot num for blood or vaccine
else
vQueryLotNum:=pLotNum;
end if;
elsif vDrugType='1000' then
vQueryLotNum:=pLotNum;
else
retval:=-8000; --compound is not commercial, blood or vaccine.
end if;
end if;


-- check to see if adequat qtys exist for the distribution
if retval>=0 then
if vQueryLotNum is null then
select sum(qty_available) into vSumQtyRemaining FROM inv_containers where container_status_id_fk=1 and compound_id_fk=vCompoundId and location_id_fk= vLocationId and lot_num is null;
else
select sum(qty_available) into vSumQtyRemaining FROM inv_containers where container_status_id_fk=1 and compound_id_fk=vCompoundId and location_id_fk= vLocationId and lot_num=vQueryLotNum;
end if;

if vSumQtyRemaining is null then 
vSumQtyRemaining:=0; 
end if;


if vSumQtyRemaining<pQTY and pQTY>0 and pTransType='decrement' then
retval:=-9005; -- not enough supply
end if;
end if;

if retval>=0 then

if (pQTY<vSumQtyRemaining and pTransType='update') or (pTransType='decrement' and pQTY>0) then
if pTransType='update' then
vUpdateRemaining:=vSumQtyRemaining-pQty;
else
--otherwise it is a simple decrement
vUpdateRemaining:=pQty;
end if;
      open com_curs;
      loop
      fetch com_curs into com_rs;
      exit when vUpdateRemaining=0;
      exit when com_curs%notfound;
      if (com_rs.lot_num is null and vQueryLotNum is null) or (com_rs.lot_num is not null and vQueryLotNum is not null) then -- skip containers that have/do not have lot nums for com/bldvacs.
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

elsif pTransType='update' or pQTY<0 then
if pTransType='update'              then
vUpdateRemaining:=pQTY-vSumQtyRemaining;
else
vUpdateRemaining:=-1 * pQTY;
end if;
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
  retval:=-5000; --no update necessary
end if;



end if;


--- else IDS protocol
--UPDATE TRANSACTION TABLE WITH ANY ERRORS
if retval=0 then
UPDATE transaction_record SET transaction_status=1 where internal_seq_num=vSequenceNum;
else
UPDATE transaction_record SET transaction_status=retval where internal_seq_num=vSequenceNum;
end if;
END "DFCI_INVENTORYUPDATE";
/
