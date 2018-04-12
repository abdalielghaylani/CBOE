CREATE OR REPLACE FUNCTION "&&SchemaName"."GETPKCOLUMN"
  (pTableName varchar2)
RETURN varchar2
IS

lColumnName varchar2(100) := '';
v_Count int;      
invalid_table exception;

BEGIN

	--validate the table
	SELECT count(*) INTO v_Count FROM user_tables WHERE table_name = upper(pTableName) ;
	IF v_Count = 0 THEN
		 RAISE invalid_table;
	END IF;
                            
  FOR pk_rec IN (SELECT COLUMN_NAME FROM user_cons_columns cc, user_constraints c WHERE cc.table_name = c.table_name and cc.constraint_name = c.constraint_name and cc.table_name = upper(pTableName) and constraint_type = 'P')
  LOOP
 		lColumnName := lColumnName || pk_rec.column_name || ','; 	
  END LOOP;
    --SELECT COLUMN_NAME INTO v_ColumnName FROM user_cons_columns cc, user_constraints c WHERE cc.table_name = c.table_name and cc.constraint_name = c.constraint_name and cc.table_name = upper(pTableName) and constraint_type = 'P';
  RETURN TRIM(',' FROM lColumnName);

exception
WHEN invalid_table then
	RETURN 'Invalid table specified';
END GETPKCOLUMN;
/
show errors;
