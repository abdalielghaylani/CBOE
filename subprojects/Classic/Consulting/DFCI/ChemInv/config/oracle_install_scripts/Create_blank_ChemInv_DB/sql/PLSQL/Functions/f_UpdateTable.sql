CREATE OR REPLACE FUNCTION "&&SchemaName"."UPDATETABLE"
  (pTableName varchar2,
   pPKColumnName varchar2,
   pPKIDs varchar2,
   pValuePairs varchar2)
RETURN varchar2
IS
--TableName: name of the table to be updated
--pPKColumnName: a comma delimited list of the primary key column(s)
--pPKIDs: a colon delimited list of primary keys
--pValues: name/value pairs for fields to be updated, :: delimited

ValuePairs_t STRINGUTILS.t_char;
v_ColumnName varchar2(100);
v_Start int;
v_EqualsPosition int;
v_CommaPosition int;
v_Count int;
v_ValuePairs varchar2(1000);
invalid_column exception;
lPKClause VARCHAR2(500) := '';
lPKCol_t StringUtils.t_char;
lPKID_t StringUtils.t_char;

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
		SELECT count(*) INTO v_Count FROM all_tab_columns WHERE table_name = upper(substr(pTableName, instr(pTableName,'.',1)+1)) AND column_name = upper(v_ColumnName);
		IF v_Count = 0 THEN
			 RAISE invalid_column;
		END IF;
    End loop;

  	v_ValuePairs := replace(pValuePairs,'::',',');
    
    --' support multiple primary key columns
    IF instr(pPKColumnName, ',') > 0 THEN 
    	lPKCol_t := StringUtils.split(pPKColumnName, ',');
      lPKID_t := StringUtils.split(pPKIDs, ':');
      FOR i IN lPKCol_t.FIRST..lPKCol_t.LAST
      LOOP
      	lPKClause := lPKClause || lPKCol_t(i) || ' in (' || lPKID_t(i) || ')';
        IF i <> lPKCol_t.LAST THEN
        	lPKClause := lPKClause || ' and ';
        END IF;
      END LOOP;
    ELSE 
    	lPKClause := pPKColumnName || ' in (' || pPKIDs || ')';
    END IF;
    
    EXECUTE IMMEDIATE 'UPDATE ' || pTableName || ' SET ' || v_ValuePairs || ' WHERE ' || lPKClause ;
    RETURN 0;

exception
WHEN invalid_column then
	RETURN 'Invalid column specified';
END UPDATETABLE;

/
show errors;

