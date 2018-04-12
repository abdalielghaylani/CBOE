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

  procedure check_val( 
	l_raid IN audit_row.raid%TYPE,
	l_cname in varchar2, 
	l_new in varchar2, 
	l_old in varchar2 );

  procedure check_val(
	l_raid IN audit_row.raid%TYPE,
	l_cname in varchar2, 
	l_new in date, 
	l_old in date );

  procedure check_val(
	l_raid IN audit_row.raid%TYPE,
	l_cname in varchar2, 
	l_new in number, 
	l_old in number );  

END audit_trail;
/



