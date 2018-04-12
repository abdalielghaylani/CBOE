-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Incrementing Sequences after data load
--######################################################### 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Incrementing sequences...'
prompt '#########################################################'

DECLARE
	PROCEDURE incrementSequence(pTableName IN varchar2, pPK IN varchar2, pSeqName IN varchar2) IS
		vMaxID number;
		vCurrValue number;
	BEGIN

		EXECUTE IMMEDIATE 'SELECT max(' || pPK || ') FROM ' || pTableName INTO vMaxID;
		EXECUTE IMMEDIATE 'select ' || pSeqName || '.nextval from dual' INTO vCurrValue;
		WHILE vCurrValue <= vMaxID
		LOOP
			EXECUTE IMMEDIATE 'select ' || pSeqName || '.nextval from dual' INTO vCurrValue;
		END LOOP;
	END incrementSequence;

BEGIN
	incrementSequence('inv_compounds', 'compound_id', 'seq_Inv_Compounds');
	incrementSequence('inv_barcode_desc', 'BARCODE_DESC_ID', 'SEQ_INV_BARCODE_DESC');
	incrementSequence('inv_location_types', 'location_type_id', 'seq_inv_location_types');
	incrementSequence('inv_locations', 'Location_id', 'seq_Inv_Locations');
	incrementSequence('inv_suppliers', 'Supplier_id', 'seq_Inv_Suppliers');
	incrementSequence('inv_solvents', 'SOLVENT_ID', 'SEQ_INV_SOLVENTS');
	incrementSequence('inv_units', 'unit_id', 'seq_Inv_units');
	incrementSequence('inv_container_status', 'Container_Status_ID', 'seq_Inv_Container_Status');
	incrementSequence('inv_container_types', 'Container_type_id', 'seq_inv_Container_types');
	incrementSequence('inv_eset_type', 'ESET_TYPE_ID', 'SEQ_INV_ESET_TYPE');
 	incrementSequence('inv_enumeration_set', 'ESET_ID', 'SEQ_INV_ENUMERATION_SET');
	incrementSequence('inv_grid_format', 'GRID_FORMAT_ID', 'SEQ_INV_GRID_FORMAT');
	incrementSequence('inv_grid_position', 'GRID_POSITION_ID', 'SEQ_INV_GRID_POSITION');
	incrementSequence('inv_physical_plate', 'PHYS_PLATE_ID', 'SEQ_INV_PHYSICAL_PLATE');
	incrementSequence('inv_plate_format', 'PLATE_FORMAT_ID', 'SEQ_INV_PLATE_FORMAT');
	incrementSequence('inv_plate_types', 'PLATE_TYPE_ID', 'SEQ_INV_PLATE_TYPES');
	incrementSequence('inv_plates', 'PLATE_ID', 'SEQ_INV_PLATES');
	incrementSequence('inv_containers', 'container_id', 'seq_Inv_Containers');
	incrementSequence('inv_plate_actions', 'PLATE_ACTION_ID', 'SEQ_INV_PLATE_ACTIONS');
	incrementSequence('inv_plate_history', 'PLATE_HISTORY_ID', 'SEQ_INV_PLATE_HISTORY');
	incrementSequence('inv_reporttypes', 'REPORTTYPE_ID', 'SEQ_INV_REPORTTYPES');
	incrementSequence('inv_reservations', 'RESERVATION_ID', 'SEQ_INV_RESERVATIONS');
	incrementSequence('inv_reservation_types', 'RESERVATION_TYPE_ID', 'SEQ_INV_RESERVATION_TYPES');
	incrementSequence('inv_wells', 'WELL_ID', 'SEQ_INV_WELLS');
	incrementSequence('inv_well_compounds', 'WELL_COMPOUND_ID', 'SEQ_INV_WELL_COMPOUNDS');
	incrementSequence('inv_xmldocs', 'XMLDOC_ID', 'SEQ_INV_XMLDOCS');
	incrementSequence('inv_xmldoc_types', 'XMLDOC_TYPE_ID', 'SEQ_INV_XMLDOC_TYPE');
	incrementSequence('inv_xslts', 'XSLT_ID', 'SEQ_INV_XSLTS');
	incrementSequence('inv_country', 'COUNTRY_ID', 'SEQ_INV_COUNTRY');
	incrementSequence('inv_states', 'STATE_ID', 'SEQ_INV_STATES');
END;
/

--' Create default barcode description sequences
DECLARE
CURSOR vBarcodeDesc_cur IS
	SELECT * FROM inv_barcode_desc;
temp varchar(20);
BEGIN
   	FOR vBarcodeDesc_rec IN vBarcodeDesc_cur
   	LOOP
   		IF NOT barcodes.BarcodeSequenceExists(vBarcodeDesc_rec.barcode_desc_id) THEN
			barcodes.CreateBarcodeSequence(vBarcodeDesc_rec.barcode_desc_id, vBarcodeDesc_rec.run_start, vBarcodeDesc_rec.run_end, vBarcodeDesc_rec.run_increment);
		END IF;
	END LOOP;

	For i in 1..10
	LOOP
		select barcodes.GetNextBarcode(constants.cPlateBarcodeDesc) INTO temp from dual;
	END LOOP;

END;
/
