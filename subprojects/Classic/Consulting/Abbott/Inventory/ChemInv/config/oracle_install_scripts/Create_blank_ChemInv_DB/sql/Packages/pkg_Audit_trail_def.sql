CREATE OR REPLACE PACKAGE audit_trail IS

  PROCEDURE record_transaction
    (raid IN NUMBER,
     tabname IN VARCHAR2,
     erid IN NUMBER,
     act IN VARCHAR2);

  PROCEDURE column_update
    (raid IN NUMBER,
     colname IN VARCHAR2,
     oldval IN VARCHAR2,
     newval IN VARCHAR2);

  FUNCTION GETLASTCOLUMNVALUE(
  	pContainerID inv_containers.container_id%type,
    pTableName audit_row.table_name%TYPE,
		pColumnName audit_column.column_name%TYPE)
 	RETURN audit_column.old_value%TYPE;     

END audit_trail;
/



