-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--#########################################################
-- Insert Inventory test data
--######################################################### 

prompt '#########################################################'
prompt 'Inserting test data...'
prompt '#########################################################'

--' use dump file for inv_compounds because they have clob fields
-- Oracle 10.2.0.3 throws an error during import if table exists and contains a clob column
-- drop the table and let imp recreate it.

-- Disable FK constraint in order to drop inv_compounds
alter table inv_well_compounds drop constraint INV_WELLCMPD_COMPOUNDID_FK;
alter table inv_synonyms drop constraint INV_SYN_COMPOUND_FK;
alter table inv_containers drop constraint INV_CONT_COMPOUNDS_FK;
alter table inv_solvents drop constraint INV_SOLVENTS_COMPOUND_ID_FK;

drop table inv_compounds;
host imp &&schemaName/&&schemaPass@&&serverName file=..\..\Create_test_ChemInv_DB\TableData\inv_compounds.dmp ignore=yes full=yes log=Logs\LOG_Load_InvCompounds_TestData.txt
GRANT SELECT ON "&&SchemaName".INV_COMPOUNDS TO &&securitySchemaName WITH GRANT OPTION;

CREATE OR REPLACE TRIGGER "TRG_INV_COMPOUNDS_ID" 
    BEFORE INSERT 
    ON "INV_COMPOUNDS" 
    FOR EACH ROW 
    begin
		if :new.Compound_ID is null then
			select seq_Inv_Compounds.nextval into :new.compound_id from dual;
		end if;
		if :new.MOL_ID is null then
			SELECT MOLID_SEQ.NEXTVAL INTO :NEW.MOL_ID FROM DUAL;
		end if;
end;
/
@@..\..\Create_blank_ChemInv_DB\sql\PLSQL\Triggers\inv_compounds_ad0.trg;
@@..\..\Create_blank_ChemInv_DB\sql\PLSQL\Triggers\inv_compounds_au0.trg;
@@..\..\Create_blank_ChemInv_DB\sql\PLSQL\Triggers\inv_compounds_bi0.trg;
-- Recreate the FK constraints referencing inv_compounds
alter table inv_well_compounds add (constraint INV_WELLCMPD_COMPOUNDID_FK FOREIGN KEY("COMPOUND_ID_FK") 
  REFERENCES "INV_COMPOUNDS"("COMPOUND_ID"));
alter table inv_synonyms add (constraint INV_SYN_COMPOUND_FK FOREIGN KEY("COMPOUND_ID_FK") 
  REFERENCES "INV_COMPOUNDS"("COMPOUND_ID"));
alter table inv_containers add (constraint INV_CONT_COMPOUNDS_FK FOREIGN KEY("COMPOUND_ID_FK") 
  REFERENCES "INV_COMPOUNDS"("COMPOUND_ID"));
alter table inv_solvents add (constraint INV_SOLVENTS_COMPOUND_ID_FK FOREIGN KEY("COMPOUND_ID_FK") 
  REFERENCES "INV_COMPOUNDS"("COMPOUND_ID"));
-- Create the cartridge index
--CREATE INDEX MX ON inv_compounds(base64_cdx) indexType is cscartridge.moleculeindextype PARAMETERS('&&cscartIndexOptions');

Connect &&schemaName/&&schemaPass@&&serverName

alter table inv_locations disable constraint INV_LOC_LOC_FK;
alter table INV_GRID_STORAGE disable constraint INV_GRIDSTOR_LOCATIONID_FK;
alter table INV_GRID_ELEMENT disable constraint INV_GRIDELM_LOCATIONID_FK;
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Grid_Format.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Grid_Position.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Grid_Storage.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Grid_Element.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Locations.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Allowed_Ptypes.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Containers.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Plates.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Wells.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Well_Compounds.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Plate_History.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Reservations.sql
@@..\..\Create_test_ChemInv_DB\TableData\Inv_Solvents.sql
@@..\..\Create_test_ChemInv_DB\TableData\UpdatePlates.sql
alter table INV_GRID_ELEMENT enable constraint INV_GRIDELM_LOCATIONID_FK;
alter table INV_GRID_STORAGE enable constraint INV_GRIDSTOR_LOCATIONID_FK;
alter table inv_locations enable constraint INV_LOC_LOC_FK;

@data\IncrementSequences.sql

				 						 						 


      
