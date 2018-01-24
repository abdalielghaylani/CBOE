prompt 
prompt Starting "pkg_Auditing_body.sql"...
prompt 

CREATE OR REPLACE PACKAGE BODY &&schemaName.."AUDITING" is
  
  procedure AddTriggerGenerator(triggerName   varchar2,
                                tableName     varchar2,
                                auditAction   varchar2,
                                triggerAction varchar2) as
  begin
    insert into TRIGGER_GENERATOR
      (TRIGGER_NAME, GENERATOR_SQL)
    values
      (triggerName,
       'declare
  trigger_sql clob; iCount integer;  cursor column_sqls is
    select '' Auditing.audit('''' '' || table_name || '''''','''''' || column_name ||
           '''''', to_clob(:old.'' || column_name || ''),to_clob(:new.'' || column_name || ''), ''''' ||
        auditAction || ''''', :new.rowid);'' sql_snippet
      from user_tab_columns
     where table_name = ''' || tableName || '''
     ORDER BY column_id; begin
  select ''create or replace trigger ' || triggerName || '
  ' || triggerAction || '
  for each row
begin ''  
    into trigger_sql
    from dual;  trigger_sql := trigger_sql || chr(10);  for column_sql in column_sqls loop
    trigger_sql := trigger_sql || column_sql.sql_snippet || chr(10);  end loop; trigger_sql := trigger_sql || ''end;'';select count(1) into iCount from trigger_generated where trigger_name = ''' ||
         triggerName || ''';if iCount = 1 then
update trigger_generated
set generated_sql = trigger_sql
where trigger_name = ''' || triggerName || ''';else
insert into trigger_generated values (
0,
''' || triggerName || ''',
trigger_sql);end if; /* dbms_output.put_line(trigger_sql);*/end;');
  end;
  
  procedure UpdateTrigger(triggerName varchar2) as
    generatorSql varchar2(32000);
    generatedSql varchar2(32000);
  begin
    select generator_sql
      into generatorSql
      from trigger_generator
     where trigger_name = triggerName;
  
    execute immediate generatorsql;
  
    select generated_sql
      into generatedSql
      from trigger_generated
     where trigger_name = triggername;
  
    execute immediate generatedSql;
    --    select sql into clobsql from trigger_generation_sql where id = 1;
  
    --  execute immediate clobsql;
  end;

  procedure Audit(table_name  varchar2,
                  column_name varchar2,
                  oldVal      clob,
                  newVal      clob,
                  actionType varchar2,
				  row_id varchar2) as
  begin
    if (comparevalue(oldVal, newVal) = true) then
      begin
        WriteAudit(table_name, column_name, oldval, newval, actionType, row_id);
      end;
    end if;
  end;

  procedure WriteAudit(table_name  varchar2,
                       column_name varchar2,
                       oldVal      clob,
                       newVal      clob,
                       actionType varchar2,
					   row_id varchar2) as
    nextid number;
  begin
    select max(id) + 1 into nextid from audit_history;
    if (nextid is null) then
      nextid := 1;
    end if;
    insert into audit_history
    values
      (nextid,
       SYS_Context('userenv', 'client_identifier'),
       table_name,
       column_name,
       oldval,
       newval,
       systimestamp,
       actionType,
	   row_id);
  end;

  function CompareValue(oldVal clob, newVal clob) return boolean as
  begin
    return(newVal <> oldVal OR (newVal IS NULL AND oldVal IS NOT NULL) OR
           (newVal IS NOT NULL AND oldVal IS NULL));
  end;

procedure Toggle(status varchar2) as
    cursor curTrigger is
      select trigger_name from trigger_generated;
  begin
  
    for trg in curTrigger loop
      if (upper(status) = 'TRUE') then
        execute immediate 'alter trigger ' || trg.trigger_name || ' enable';
      else
        if (upper(status) = 'FALSE') then
          execute immediate 'alter trigger ' || trg.trigger_name ||
                            ' disable';
        end if;
      end if;
    end loop;
  end;
  
end Auditing;
/