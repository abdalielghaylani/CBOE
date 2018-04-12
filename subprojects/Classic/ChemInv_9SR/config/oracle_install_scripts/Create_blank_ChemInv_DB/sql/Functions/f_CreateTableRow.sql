CREATE OR REPLACE FUNCTION "&&SchemaName"."CREATETABLEROW"
  (pTableName varchar2,
   pValuePairs varchar2)
RETURN varchar2
IS
--TableName: name of the table to be updated
--pPKIDs: a comma delimited list of primary keys
--pValues: name/value pairs for fields to be updated, :: delimited

ValuePairs_t STRINGUTILS.t_char;
v_ColumnName varchar2(100);
v_Value varchar(100);
v_Start int;
v_EqualsPosition int;
v_CommaPosition int;
v_Count int;
v_Columns varchar(2000);
v_Values varchar(2000);
invalid_column exception;

BEGIN

	--validate the columns
	v_Start := 2;
	ValuePairs_t := STRINGUTILS.split(pValuePairs,'::');
	FOR i in ValuePairs_t.First..ValuePairs_t.Last
	Loop
		--get column name
		v_EqualsPosition := INSTR(ValuePairs_t(i),'=',v_Start,1);
		v_CommaPosition := INSTR(ValuePairs_t(i),'::',v_Start,1);
		v_ColumnName := TRIM(SUBSTR(ValuePairs_t(i), v_Start, (v_EqualsPosition-2)));
		v_Start := v_CommaPosition + 2;
		--validate column name
		SELECT count(*) INTO v_Count FROM user_tab_columns WHERE table_name = upper(pTableName) AND column_name = upper(v_ColumnName);
		IF v_Count = 0 THEN
			 RAISE invalid_column;
		END IF; 
		v_Columns := v_Columns || v_ColumnName || ',';   
		--get value
		v_Value := TRIM(SUBSTR(ValuePairs_t(i),v_EqualsPosition+1));
		v_Values := v_Values || v_Value || ',';
    End loop;
	v_Columns := SUBSTR(v_Columns,0,LENGTH(v_Columns)-1);  
	v_Values := SUBSTR(v_Values,0,LENGTH(v_Values)-1);  
    --RETURN 'INSERT INTO ' || pTableName || ' (' || v_Columns || ') VALUES(' || v_Values || ')';
    EXECUTE IMMEDIATE 'INSERT INTO ' || pTableName || ' (' || v_Columns || ') VALUES(' || v_Values || ')';
    RETURN 0;

exception
WHEN invalid_column then
	RETURN 'Invalid column specified';
    --RETURN -127;
   	--RETURN 'SELECT count(*) INTO v_Count FROM user_tab_columns WHERE table_name = ' || pTableName || 'AND column_name = ' || upper(v_ColumnName);
    --RETURN pValuePairs;
    --RETURN ValuePairs_t(1) || ':columnName';
    --RETURN ValuePairs_t(1) || ':' || v_EqualsPosition || ':' || v_CommaPosition || ':' || v_ColumnName;
END "CREATETABLEROW";
/
show errors;
