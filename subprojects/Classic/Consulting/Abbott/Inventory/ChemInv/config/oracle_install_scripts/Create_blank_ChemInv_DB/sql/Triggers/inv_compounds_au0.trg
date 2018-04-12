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
if Updating('compound_id') then 
 if :old.compound_id != :new.compound_id then
     audit_trail.column_update
       (raid, 'COMPOUND_ID',
       :old.compound_id, :new.compound_id);
 end if;
end if;
 if Updating('mol_id') then 
  if nvl(:old.mol_id,0) !=
     NVL(:new.mol_id,0) then
     audit_trail.column_update
       (raid, 'MOL_ID',
       :old.mol_id, :new.mol_id);
  end if;
 end if; 
 if Updating('cas') then 
  if nvl(:old.cas,' ') !=
     NVL(:new.cas,' ') then
     audit_trail.column_update
       (raid, 'CAS',
       :old.cas, :new.cas);
  end if;
 end if; 
 if Updating('acx_id') then 
  if nvl(:old.acx_id,' ') !=
     NVL(:new.acx_id,' ') then
     audit_trail.column_update
       (raid, 'ACX_ID',
       :old.acx_id, :new.acx_id);
  end if;
 end if; 
 if Updating('substance_name') then 
  if nvl(:old.substance_name,' ') !=
     NVL(:new.substance_name,' ') then
     audit_trail.column_update
       (raid, 'SUBSTANCE_NAME',
       :old.substance_name, :new.substance_name);
  end if;
 end if; 
 if Updating('molecular_weight') then 
  if nvl(:old.molecular_weight,' ') !=
     NVL(:new.molecular_weight,' ') then
     audit_trail.column_update
       (raid, 'MOLECULAR_WEIGHT',
       :old.molecular_weight, :new.molecular_weight);
  end if;
 end if; 
 if Updating('density') then 
  if nvl(:old.density,0) !=
     NVL(:new.density,0) then
     audit_trail.column_update
       (raid, 'DENSITY',
       :old.density, :new.density);
  end if;
 end if; 
 if Updating('clogp') then 
  if nvl(:old.clogp,0) !=
     NVL(:new.clogp,0) then
     audit_trail.column_update
       (raid, 'CLOGP',
       :old.clogp, :new.clogp);
  end if;
 end if; 
 if Updating('rotatable_bonds') then 
  if nvl(:old.rotatable_bonds,0) !=
     NVL(:new.rotatable_bonds,0) then
     audit_trail.column_update
       (raid, 'ROTATABLE_BONDS',
       :old.rotatable_bonds, :new.rotatable_bonds);
  end if;
 end if; 
 if Updating('tot_pol_surf_area') then 
  if nvl(:old.tot_pol_surf_area,0) !=
     NVL(:new.tot_pol_surf_area,0) then
     audit_trail.column_update
       (raid, 'TOT_POL_SURF_AREA',
       :old.tot_pol_surf_area, :new.tot_pol_surf_area);
  end if;
 end if; 
 if Updating('hbond_acceptors') then 
  if nvl(:old.hbond_acceptors,0) !=
     NVL(:new.hbond_acceptors,0) then
     audit_trail.column_update
       (raid, 'HBOND_ACCEPTORS',
       :old.hbond_acceptors, :new.hbond_acceptors);
  end if;
 end if; 
 if Updating('hbond_donors') then 
  if nvl(:old.hbond_donors,0) !=
     NVL(:new.hbond_donors,0) then
     audit_trail.column_update
       (raid, 'HBOND_DONORS',
       :old.hbond_donors, :new.hbond_donors);
  end if;
 end if; 
 if Updating('alt_id_1') then 
  if nvl(:old.alt_id_1,' ') !=
     NVL(:new.alt_id_1,' ') then
     audit_trail.column_update
       (raid, 'ALT_ID_1',
       :old.alt_id_1, :new.alt_id_1);
  end if;
 end if; 
 if Updating('alt_id_2') then 
  if nvl(:old.alt_id_2,' ') !=
     NVL(:new.alt_id_2,' ') then
     audit_trail.column_update
       (raid, 'ALT_ID_2',
       :old.alt_id_2, :new.alt_id_2);
  end if;
 end if; 
 if Updating('alt_id_3') then 
  if nvl(:old.alt_id_3,' ') !=
     NVL(:new.alt_id_3,' ') then
     audit_trail.column_update
       (raid, 'ALT_ID_3',
       :old.alt_id_3, :new.alt_id_3);
  end if;
 end if; 
 if Updating('alt_id_4') then 
  if nvl(:old.alt_id_4,' ') !=
     NVL(:new.alt_id_4,' ') then
     audit_trail.column_update
       (raid, 'ALT_ID_4',
       :old.alt_id_4, :new.alt_id_4);
  end if;
 end if; 
 if Updating('alt_id_5') then 
  if nvl(:old.alt_id_5,' ') !=
     NVL(:new.alt_id_5,' ') then
     audit_trail.column_update
       (raid, 'ALT_ID_5',
       :old.alt_id_5, :new.alt_id_5);
  end if;
 end if; 
 if Updating('conflicting_fields') then 
  if nvl(:old.conflicting_fields,' ') !=
     NVL(:new.conflicting_fields,' ') then
     audit_trail.column_update
       (raid, 'CONFLICTING_FIELDS',
       :old.conflicting_fields, :new.conflicting_fields);
  end if;
 end if; 
 if Updating('rid') then
  if :old.rid != :new.rid then 
     audit_trail.column_update
       (raid, 'RID',
       :old.rid, :new.rid);
  end if;
 end if; 
  
  INSERT into Audit_compound (RAID, old_base64_cdx) VALUES (raid, :old.base64_cdx);
end;
/
