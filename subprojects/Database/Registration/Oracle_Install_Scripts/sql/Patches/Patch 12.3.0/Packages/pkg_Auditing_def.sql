prompt 
prompt Starting "pkg_Auditing_def.sql"...
prompt 

CREATE OR REPLACE PACKAGE &&schemaName.."AUDITING" is
  procedure AddTriggerGenerator(triggerName varchar2,
                                tableName   varchar2,
                                auditAction      varchar2,
								triggerAction varchar2);
  procedure UpdateTrigger(triggerName varchar2);
  procedure Audit(table_name  varchar2,
                  column_name varchar2,
                  oldVal      clob,
                  newVal      clob,
                  actionType varchar2,
				  row_id varchar2);
  function CompareValue(oldVal clob, newVal clob) return boolean;
  procedure WriteAudit(table_name  varchar2,
                       column_name varchar2,
                       oldVal      clob,
                       newVal      clob,
                       actionType varchar2,
					   row_id varchar2);
	procedure Toggle(status varchar2);
end Auditing;
/