--migrate inv_plates.parent_plate_id_fk to inv_plate_parent and delete the column
INSERT INTO inv_plate_parent (SELECT p.parent_plate_id_fk, p.plate_id FROM inv_plates p WHERE p.plate_id NOT IN (SELECT child_plate_id_fk FROM inv_plate_parent WHERE child_plate_id_fk = p.plate_id AND parent_plate_id_fk = p.parent_plate_id_fk)  AND p.parent_plate_id_fk IS NOT NULL);
ALTER TABLE "INV_PLATES" DROP ("PARENT_PLATE_ID_FK") CASCADE CONSTRAINTS;
--DROP INDEX PARENT_PLATE_ID_FK_IDX;

--migrate inv_wells.parent_well_id_fk to inv_well_parent and delete the column
INSERT INTO inv_well_parent (SELECT w.parent_well_id_fk, w.well_id FROM inv_wells w WHERE w.well_id NOT IN (SELECT child_well_id_fk FROM inv_well_parent WHERE child_well_id_fk = w.well_id AND parent_well_id_fk = w.parent_well_id_fk)  AND w.parent_well_id_fk IS NOT NULL);
ALTER TABLE "INV_WELLS" DROP ("PARENT_WELL_ID_FK") CASCADE CONSTRAINTS;

--migrate inv_wells.compound_id_fk, inv_wells.reg_id_fk, inv_wells.batch_number_fk to inv_well_compounds and delete the column
INSERT INTO inv_well_compounds (well_id_fk, compound_id_fk, reg_id_fk, batch_number_fk) (SELECT w.well_id, w.compound_id_fk, w.reg_id_fk, w.batch_number_fk FROM inv_wells w WHERE w.compound_id_fk IS NOT NULL or w.reg_id_fk IS NOT NULL);
ALTER TABLE "INV_WELLS" DROP ("COMPOUND_ID_FK", "REG_ID_FK", "BATCH_NUMBER_FK") CASCADE CONSTRAINTS;
ALTER TABLE "INV_WELLS" DROP ("WELL_COMPOUND_ID_FK");
--DROP INDEX COMPOUND_ID_FK_IDX;

-- Alter table INV_REQUESTS
DROP INDEX XAK1INV_REQUESTS;

-- uppercase users
-- Alter table INV_CONTAINERS
UPDATE inv_containers SET current_user_id_fk = upper(current_user_id_fk);
alter table INV_CONTAINERS add(
  constraint INV_CONT_CURRUSERID_FK 
  	foreign key ("CURRENT_USER_ID_FK")
  	references "PEOPLE"("USER_ID")
  );
CREATE INDEX inv_containesr_curruserid_idx ON inv_containers(CURRENT_USER_ID_FK) TABLESPACE &&indexTableSpaceName;

-- Alter table INV_REQUESTS  
UPDATE inv_requests SET user_id_fk = upper(user_id_fk);
alter table INV_REQUESTS add(
	constraint INV_REQUESTS_USERID_FK foreign key (USER_ID_FK)
  	references CS_SECURITY.PEOPLE (USER_ID)        
  );
CREATE INDEX INV_REQUESTS_USERID_IDX ON INV_REQUESTS(USER_ID_FK) TABLESPACE &&indexTableSpaceName;
  
-- Alter table INV_RESERVATIONS
UPDATE inv_reservations SET user_id_fk = upper(user_id_fk);
alter table INV_RESERVATIONS add(
 	constraint INV_RESV_USERID_FK foreign key (USER_ID_FK)
  	references CS_SECURITY.PEOPLE (USER_ID)
  );
CREATE INDEX INV_RESERVATIONS_USERID_IDX ON INV_RESERVATIONS(USER_ID_FK) TABLESPACE &&indexTableSpaceName;  

  
COMMIT;



