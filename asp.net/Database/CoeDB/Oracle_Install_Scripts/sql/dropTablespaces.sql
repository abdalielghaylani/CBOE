--#########################################################
--DROP TABLESPACES
--#########################################################

PROMPT 'detecting tablespaces...'

set serveroutput on

DECLARE
	PROCEDURE dropTablespace(pName	IN	VARCHAR2)
	IS	
		n				NUMBER;
		dataFileClause	VARCHAR2 (20);
		dropSql         VARCHAR2 (300);
	BEGIN
		IF &&oraVersionNumber = 8 THEN
			dataFileClause := '';
		ELSE
			dataFileClause := 'AND DATAFILES';
		END IF;

		SELECT COUNT (*) INTO n
		FROM dba_tablespaces
		WHERE tablespace_name = UPPER(pName);

		dropSql := 'DROP TABLESPACE ' || pName || ' INCLUDING CONTENTS ' || dataFileClause || ' CASCADE CONSTRAINTS';
		 
		IF n > 0 THEN
			DBMS_OUTPUT.PUT_LINE('tablespace '|| pName || ' exists, start to drop it');	
			EXECUTE IMMEDIATE dropSql;
		END IF;
	END dropTablespace;
BEGIN
	DBMS_OUTPUT.PUT_LINE('start to detect and drop the existing tablespaces');
	dropTablespace ('&&tableSpaceName');
	dropTablespace ('&&indexTableSpaceName');
	dropTablespace ('&&lobsTableSpaceName');
	dropTablespace ('&&cscartTableSpaceName');
	dropTablespace ('&&auditTableSpaceName');
	dropTablespace ('&&tempTableSpaceName');
END;
/