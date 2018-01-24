grant create any trigger to &&schemaName;

/* Create TRIGGER_GENERATOR table to store the sql that generates another sql that creates all triggers necessary for auditing feature */
DECLARE
  LTableExists INTEGER := 0;
  
BEGIN
  select COUNT(1) INTO LTableExists from all_tables t where t.table_name = 'TRIGGER_GENERATOR' and t.owner = upper('&&schemaName');
  if ( LTableExists = 0 ) then
    execute immediate '
	create table &&schemaName..TRIGGER_GENERATOR
	(
	  id            INTEGER not null,
	  trigger_name  VARCHAR2(50) not null,
	  generator_sql CLOB not null
	)';
  end if;
END;
/

/* Create Sequence for assigning ID value to TRIGGER_GENERATOR table */
DECLARE
  LSeqExists INTEGER := 0;
  
BEGIN
  select COUNT(1) INTO LSeqExists from all_sequences t where t.sequence_name = 'SEQ_TRIGGER_GENERATOR' and t.sequence_owner = upper('&&schemaName');
  if ( LSeqExists = 0 ) then
  execute immediate '
    create sequence &&schemaName..SEQ_TRIGGER_GENERATOR
minvalue 1
maxvalue 9999999999999999999999999999
start with 1
increment by 1
cache 20';
  end if;
END;
/

/* Create trigger to set ID value to any new record being inserted into TRIGGER_GENERATOR */
create or replace trigger &&schemaName..TRG_TRIGGER_GENERATOR
  before insert on &&schemaName..trigger_generator  
  for each row
begin
  SELECT &&schemaName..SEQ_TRIGGER_GENERATOR.NEXTVAL INTO :NEW.ID FROM DUAL;
end;
/


/* Create table TRIGGER_GENERATED to store the actual sql to create triggers */
DECLARE
  LTableExists INTEGER := 0;
  
BEGIN
  select COUNT(1) INTO LTableExists from all_tables t where t.table_name = 'TRIGGER_GENERATED' and t.owner = upper('&&schemaName');
  if ( LTableExists = 0 ) then
    execute immediate '
	create table &&schemaName..TRIGGER_GENERATED
	(
	  id            INTEGER not null,
	  trigger_name  VARCHAR2(50) not null,
	  generated_sql CLOB not null
	)';
  end if;
END;
/

/* Create Sequence for assigning ID value to TRIGGER_GENERATED table */
DECLARE
  LSeqExists INTEGER := 0;
  
BEGIN
  select COUNT(1) INTO LSeqExists from all_sequences t where t.sequence_name = 'SEQ_TRIGGER_GENERATED' and t.sequence_owner = upper('&&schemaName');
  if ( LSeqExists = 0 ) then
  execute immediate '
create sequence &&schemaName..SEQ_TRIGGER_GENERATED
minvalue 1
maxvalue 9999999999999999999999999999
start with 21
increment by 1
cache 20';
  end if;
END;
/

/* Create trigger for TRIGGER_GENERATED to auto-generate ID value for new record */
create or replace trigger &&schemaName..TRG_TRIGGER_GENERATED
  before insert on &&schemaName..trigger_generated  
  for each row
begin
  SELECT SEQ_TRIGGER_GENERATED.NEXTVAL INTO :NEW.ID FROM DUAL;
end;
/


/* Create table AUDIT_HISTORY to store the actual auditing data */
DECLARE
  LTableExists INTEGER := 0;
  
BEGIN
  select COUNT(1) INTO LTableExists from all_tables t where t.table_name = 'AUDIT_HISTORY' and t.owner = upper('&&schemaName');
  if ( LTableExists = 0 ) then
    execute immediate '
	create table &&schemaName..AUDIT_HISTORY
(
  id             INTEGER not null,
  user_id        VARCHAR2(20) not null,
  table_name     VARCHAR2(50) not null,
  column_name    VARCHAR2(50) not null,
  old_val        CLOB,
  new_val        CLOB,
  operation_time TIMESTAMP(6) not null,
  operation_type VARCHAR2(20) not null,
  row_id         VARCHAR2(50)
)';
	execute immediate '
	alter table &&schemaName..AUDIT_HISTORY
  add constraint PK_AUTID_HISTORY primary key (ID)
  using index 
  tablespace T_REGDB_INDEX
  pctfree 10
  initrans 2
  maxtrans 255';
  end if;
END;
/

create or replace view &&schemaName..VW_AUDIT_HISTORY as
  select reg_number, audit_history.*
    from &&schemaName..reg_numbers, &&schemaName..mixtures, &&schemaName..batches, &&schemaName..mixture_component, &&schemaName..compound_molecule, &&schemaName..structures, &&schemaName..audit_history
   where reg_numbers.reg_id = mixtures.regid
   and mixtures.regid = batches.reg_internal_id
and mixtures.mix_internal_id = mixture_component.mixtrureid
and compound_molecule.cpd_database_counter = mixture_component.compoundid
and compound_molecule.structureid = structures.cpd_internal_id
and (mixtures.rowid = audit_history.row_id or batches.rowid = audit_history.row_id or compound_molecule.rowid = audit_history.row_id or structures.rowid = audit_history.row_id);


/* Create the new package for auditing */
@"sql\Patches\Patch 12.3.0\Packages\pkg_Auditing_def.sql"
@"sql\Patches\Patch 12.3.0\Packages\pkg_Auditing_body.sql"


/* Fill in TRIGGER_GENERATOR with trigger creation sql statements */
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_MIXTURES_AI', 'MIXTURES', 'INSERT', 'after insert on MIXTURES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_MIXTURES_AU', 'MIXTURES', 'UPDATE', 'after update on MIXTURES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_MIXTURES_AD', 'MIXTURES', 'DELETE', 'after delete on MIXTURES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_BATCHES_AI', 'BATCHES', 'INSERT', 'after insert on BATCHES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_BATCHES_AU', 'BATCHES', 'UPDATE', 'after update on BATCHES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_BATCHES_AD', 'BATCHES', 'DELETE', 'after delete on BATCHES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_STRUCTURES_AI', 'STRUCTURES', 'INSERT', 'after insert on STRUCTURES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_STRUCTURES_AU', 'STRUCTURES', 'UPDATE', 'after update on STRUCTURES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_STRUCTURES_AD', 'STRUCTURES', 'DELETE', 'after delete on STRUCTURES');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_COMPOUND_MOLECULE_AI', 'COMPOUND_MOLECULE', 'INSERT', 'after insert on COMPOUND_MOLECULE');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_COMPOUND_MOLECULE_AU', 'COMPOUND_MOLECULE', 'UPDATE', 'after update on COMPOUND_MOLECULE');
EXEC &&schemaName..Auditing.AddTriggerGenerator('TRG_COMPOUND_MOLECULE_AD', 'COMPOUND_MOLECULE', 'DELETE', 'after delete on COMPOUND_MOLECULE');

/* Fill in TRIGGER_GENERATED by executing corresponding sql stored in TRIGGER_GENERATOR */
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_MIXTURES_AI');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_MIXTURES_AU');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_MIXTURES_AD');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_BATCHES_AI');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_BATCHES_AU');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_BATCHES_AD');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_STRUCTURES_AI');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_STRUCTURES_AU');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_STRUCTURES_AD');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AI');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AU');
EXEC &&schemaName..Auditing.UpdateTrigger('TRG_COMPOUND_MOLECULE_AD');