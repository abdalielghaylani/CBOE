--#########################################################
--VALIDATE TABLESPACES
--#########################################################

PROMPT Validating tablespaces...

set serveroutput on

VARIABLE continue_or_exist VARCHAR2(50)
COLUMN :continue_or_exist NEW_VALUE script_file NOPRINT; 
EXEC :continue_or_exist:='error_exit.sql';

DECLARE
	validateFailures	NUMBER	:=0;
	PROCEDURE validateTablespace(pName	IN	VARCHAR2)
	IS	
		n				NUMBER;
	BEGIN
		SELECT COUNT (*) INTO n
		FROM dba_tablespaces
		WHERE tablespace_name = UPPER(pName);
		 
		IF n > 0 THEN
			DBMS_OUTPUT.PUT_LINE('tablespace '|| pName || ' exists, validated');
		ELSE			
			validateFailures := validateFailures + 1;
			DBMS_OUTPUT.PUT_LINE('tablespace '|| pName || ' does not exist, validate fail.');
		END IF;
	END validateTablespace;
BEGIN
	validateTablespace ('&&tableSpaceName');
	validateTablespace ('&&indexTableSpaceName');
	validateTablespace ('&&lobsTableSpaceName');
	validateTablespace ('&&cscartTableSpaceName');
	validateTablespace ('&&auditTableSpaceName');
	validateTablespace ('&&securityTableSpaceName');
	validateTablespace ('&&tempTableSpaceName');
	
	IF validateFailures > 0 THEN
		DBMS_OUTPUT.PUT_LINE('tablespace validate failed, please make sure the tablespaces pre-created on bypassing tablespaces drop/create mode.');
		RAISE_APPLICATION_ERROR(-20001, 'Errors in tablespace validation');
	ELSE
		:continue_or_exist := 'continue.sql';
		DBMS_OUTPUT.PUT_LINE('all required tablespaces are validated.');
	END IF;
END;
/

-- if tablespace validation fail, it should exit.
SELECT :continue_or_exist FROM DUAL;
@@&script_file