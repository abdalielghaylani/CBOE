CREATE OR REPLACE FUNCTION "DELETETABLEROW"
  (pTableName varchar2,
   pPKColumnName varchar2,
   pPKIDs VARCHAR2)
RETURN varchar2
IS
--TableName: name of the table to be updated
--pPKColumnName: name of the primary key column
--pPKIDs: a comma delimited list of primary keys

BEGIN

    EXECUTE IMMEDIATE 'DELETE ' || pTableName || ' WHERE ' || pPKColumnName || ' IN (' || pPKIDs || ')';
    RETURN 0;

END "DELETETABLEROW";
/
show errors;