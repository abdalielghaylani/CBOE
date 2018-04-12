CREATE OR REPLACE FUNCTION "DELETETABLEROW"
  (pTableName varchar2,
   pPKColumnName varchar2,
   pPKIDs VARCHAR2)
RETURN varchar2
IS
--TableName: name of the table to be updated
--pPKColumnName: a comma delimited list of the primary key column(s)
--pPKIDs: a colon delimited list of primary keys
lPKClause VARCHAR2(500) := '';
lPKCol_t StringUtils.t_char;
lPKID_t StringUtils.t_char;

--' create an exception for "ORA-2292 violated integrity constraint"
child_data_exists EXCEPTION;
PRAGMA EXCEPTION_INIT (child_data_exists, -2292);
BEGIN
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

    EXECUTE IMMEDIATE 'DELETE ' || pTableName || ' WHERE ' || lPKClause ;
    RETURN 0;

		EXCEPTION
						 WHEN child_data_exists THEN
						 			RETURN 'Child data exists for this row.  It could not be deleted.';
		
END "DELETETABLEROW";
/
show errors;