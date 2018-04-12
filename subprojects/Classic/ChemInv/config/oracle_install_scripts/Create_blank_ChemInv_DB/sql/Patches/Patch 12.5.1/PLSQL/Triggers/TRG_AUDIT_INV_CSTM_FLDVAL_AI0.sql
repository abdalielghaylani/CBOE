CREATE OR REPLACE TRIGGER "TRG_AUDIT_INV_CSTM_FLDVAL_AI0"
  after insert
  on "INV_CUSTOM_CPD_FIELD_VALUES"
  for each row
WHEN ( NEW.compound_id_fk is not null ) 
declare
  raid number(10);
  compound_rid inv_compounds.rid%TYPE;
  new_customfieldname inv_custom_fields.custom_field_name%TYPE;

begin
  select seq_audit.nextval into raid from dual;
  select rid into compound_rid from inv_compounds where compound_id = :new.COMPOUND_ID_FK;
  select custom_field_name into new_customfieldname from inv_custom_fields where custom_field_id = :new.CUSTOM_FIELD_ID_FK;
                             
  audit_trail.record_transaction
    (raid, 'INV_CUSTOM_CPD_FIELD_VALUES', compound_rid, 'I');
  
  audit_trail.check_val(raid, 'CUSTOM_CPD_FIELD_VALUE_ID', :new.CUSTOM_CPD_FIELD_VALUE_ID, :old.CUSTOM_CPD_FIELD_VALUE_ID);
  audit_trail.check_val(raid, 'PICKLIST_ID_FK', :new.PICKLIST_ID_FK, :old.PICKLIST_ID_FK);
  audit_trail.check_val(raid, 'CUSTOM_FIELD_NAME', new_customfieldname, null);
  audit_trail.check_val(raid, 'COMPOUND_ID_FK', :new.COMPOUND_ID_FK, :old.COMPOUND_ID_FK);
end;
/
