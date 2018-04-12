-- inv_compounds_au0.trg
-- Audit Trigger After Update on inv_compounds
create or replace trigger TRG_AUDIT_INV_COMPOUNDS_AU0
  after update of
  compound_id,
  mol_id,
  cas,
  acx_id,
  substance_name,
  molecular_weight,
  density,
  clogp,
  rotatable_bonds,
  tot_pol_surf_area,
  hbond_acceptors,
  hbond_donors,
  alt_id_1,
  alt_id_2,
  alt_id_3,
  alt_id_4,
  alt_id_5,
  conflicting_fields,
  rid
  on inv_compounds
  for each row

declare
  raid number(10);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_COMPOUNDS', :old.rid, 'U');

  audit_trail.check_val(raid, 'COMPOUND_ID', :new.COMPOUND_ID, :old.COMPOUND_ID);
  audit_trail.check_val(raid, 'MOL_ID', :new.MOL_ID, :old.MOL_ID);
  audit_trail.check_val(raid, 'CAS', :new.CAS, :old.CAS);
  audit_trail.check_val(raid, 'ACX_ID', :new.ACX_ID, :old.ACX_ID);
  audit_trail.check_val(raid, 'SUBSTANCE_NAME', :new.SUBSTANCE_NAME, :old.SUBSTANCE_NAME);
  audit_trail.check_val(raid, 'BASE64_CDX', :new.BASE64_CDX, :old.BASE64_CDX);
  audit_trail.check_val(raid, 'MOLECULAR_WEIGHT', :new.MOLECULAR_WEIGHT, :old.MOLECULAR_WEIGHT);
  audit_trail.check_val(raid, 'DENSITY', :new.DENSITY, :old.DENSITY);
  audit_trail.check_val(raid, 'CLOGP', :new.CLOGP, :old.CLOGP);
  audit_trail.check_val(raid, 'ROTATABLE_BONDS', :new.ROTATABLE_BONDS, :old.ROTATABLE_BONDS);
  audit_trail.check_val(raid, 'TOT_POL_SURF_AREA', :new.TOT_POL_SURF_AREA, :old.TOT_POL_SURF_AREA);
  audit_trail.check_val(raid, 'HBOND_ACCEPTORS', :new.HBOND_ACCEPTORS, :old.HBOND_ACCEPTORS);
  audit_trail.check_val(raid, 'HBOND_DONORS', :new.HBOND_DONORS, :old.HBOND_DONORS);
  audit_trail.check_val(raid, 'ALT_ID_1', :new.ALT_ID_1, :old.ALT_ID_1);
  audit_trail.check_val(raid, 'ALT_ID_2', :new.ALT_ID_2, :old.ALT_ID_2);
  audit_trail.check_val(raid, 'ALT_ID_3', :new.ALT_ID_3, :old.ALT_ID_3);
  audit_trail.check_val(raid, 'ALT_ID_4', :new.ALT_ID_4, :old.ALT_ID_4);
  audit_trail.check_val(raid, 'ALT_ID_5', :new.ALT_ID_5, :old.ALT_ID_5);
  audit_trail.check_val(raid, 'CONFLICTING_FIELDS', :new.CONFLICTING_FIELDS, :old.CONFLICTING_FIELDS);
  audit_trail.check_val(raid, 'RID', :new.RID, :old.RID);
  --audit_trail.check_val(raid, 'CREATOR', :new.CREATOR, :old.CREATOR);
  --audit_trail.check_val(raid, 'TIMESTAMP', :new.TIMESTAMP, :old.TIMESTAMP);
  
  INSERT into Audit_compound (RAID, old_base64_cdx) VALUES (raid, :old.base64_cdx);
end;
/
