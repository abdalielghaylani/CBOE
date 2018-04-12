-- inv_containers_ad0.trg
-- Audit Trigger After Delete on inv_containers
create or replace trigger TRG_AUDIT_INV_CONTAINERS_AD0
  after delete
  on inv_containers
  for each row

declare
  raid number(10);
  deleted_data varchar2(4000);

begin
  select seq_audit.nextval into raid from dual;

  audit_trail.record_transaction
    (raid, 'INV_CONTAINERS', :old.rid, 'D');

  deleted_data :=
  to_char(:old.container_id) || '|' ||
  to_char(:old.location_id_fk) || '|' ||
  to_char(:old.compound_id_fk) || '|' ||
  to_char(:old.parent_container_id_fk) || '|' ||
  to_char(:old.family) || '|' ||
  :old.container_name || '|' ||
  :old.container_description || '|' ||
  to_char(:old.qty_max) || '|' ||
  to_char(:old.qty_initial) || '|' ||
  to_char(:old.qty_remaining) || '|' ||
  to_char(:old.qty_minstock) || '|' ||
  to_char(:old.qty_maxstock) || '|' ||
  to_char(:old.well_number) || '|' ||
  :old.well_row || '|' ||
  :old.well_column || '|' ||
  to_char(:old.date_expires) || '|' ||
  to_char(:old.date_created) || '|' ||
  to_char(:old.container_type_id_fk) || '|' ||
  to_char(:old.purity) || '|' ||
  :old.solvent_ID_FK || '|' ||
  to_char(:old.concentration) || '|' ||
  to_char(:old.unit_of_meas_id_fk) || '|' ||
  to_char(:old.unit_of_wght_id_fk) || '|' ||
  to_char(:old.unit_of_conc_id_fk) || '|' ||
  :old.grade || '|' ||
  to_char(:old.weight) || '|' ||
  to_char(:old.unit_of_purity_id_fk) || '|' ||
  to_char(:old.tare_weight) || '|' ||
  :old.owner_id_fk || '|' ||
  :old.container_comments || '|' ||
  :old.storage_conditions || '|' ||
  :old.handling_procedures || '|' ||
  :old.ordered_by_id_fk || '|' ||
  to_char(:old.date_ordered) || '|' ||
  to_char(:old.date_received) || '|' ||
  to_char(:old.date_certified) || '|' ||
  to_char(:old.date_approved) || '|' ||
  :old.lot_num || '|' ||
  to_char(:old.container_status_id_fk) || '|' ||
  :old.received_by_id_fk || '|' ||
  to_char(:old.final_wght) || '|' ||
  to_char(:old.net_wght) || '|' ||
  to_char(:old.qty_available) || '|' ||
  to_char(:old.qty_reserved) || '|' ||
  to_char(:old.physical_state_id_fk) || '|' ||
  :old.current_user_id_fk || '|' ||
  to_char(:old.supplier_id_fk) || '|' ||
  :old.supplier_catnum || '|' ||
  to_char(:old.date_produced) || '|' ||
  to_char(:old.container_cost) || '|' ||
  to_char(:old.unit_of_cost_id_fk) || '|' ||
  to_char(:old.def_location_id_fk) || '|' ||
  :old.barcode || '|' ||
  :old.po_number || '|' ||
  :old.req_number || '|' ||
  to_char(:old.reg_id_fk) || '|' ||
  to_char(:old.batch_number_fk) || '|' ||
  to_char(:old.rid) || '|' ||
  :old.creator || '|' ||
  to_char(:old.timestamp) || '|' ||
  to_char(:old.density) || '|' ||
  to_char(:old.unit_of_density_id_fk) || '|' ||
  :old.field_1 || '|' ||
  :old.field_2 || '|' ||
  :old.field_3 || '|' ||
  :old.field_4 || '|' ||
  :old.field_5 || '|' ||
  :old.field_6 || '|' ||
  :old.field_7 || '|' ||
  :old.field_8 || '|' ||
  :old.field_9 || '|' ||
  :old.field_10 || '|' ||
  to_char(:old.date_1) || '|' ||
  to_char(:old.date_2) || '|' ||
  to_char(:old.date_3) || '|' ||
  to_char(:old.date_4) || '|' ||
  to_char(:old.date_5) || '|' ||
  to_char(:old.po_line_number);

insert into audit_delete
(raid, row_data) values (raid, deleted_data);
end;
/
