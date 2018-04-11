PROMPT 'Dropping public synonyms'

DECLARE
   PROCEDURE dropSynonym (synName IN VARCHAR2) IS
      n   NUMBER;
   BEGIN
      SELECT COUNT (*) INTO n
        FROM dba_synonyms
       WHERE UPPER (synonym_name) = synName;

      IF n > 0 THEN
         EXECUTE IMMEDIATE 'DROP PUBLIC SYNONYM ' || synName;
      END IF;
   END dropSynonym;
BEGIN
    dropSynonym('COEDATAVIEW');
    dropSynonym('COEDATABASE');
    dropSynonym('COEFORM');
    dropSynonym('COEGENERICOBJECT');
    dropSynonym('COEGLOBALS');
    dropSynonym('COETEMPIDS');
    dropSynonym('COEOBJECTTYPE');
    dropSynonym('COEPRINCIPTALTYPE');
    dropSynonym('COEPERMISSIONS');
    dropSynonym('COESESSION');
    dropSynonym('COESAVEDSEARCHCRITERIA');
    dropSynonym('COESEARCHCRITERIA');
    dropSynonym('COESAVEDHITLIST');
    dropSynonym('COETEMPHITLIST');
    dropSynonym('COESAVEDHITLIST');
    dropSynonym('COETEMPHITLIST');
    dropSynonym('COESAVEDHITLISTID');
    dropSynonym('COETEMPHITLISTID');
    dropSynonym('COEPARTITIONMANAGMENT');
    dropSynonym('CREATESERVICETABLES');
    dropSynonym('CONFIGURATIONMANAGER');
    dropSynonym('MYTABLETYPE');	
END;
/

PROMPT 'Dropping user...'

set serveroutput on;

DECLARE
        i NUMBER;
BEGIN
	select count(*) into i FROM v$session WHERE SCHEMANAME='&&schemaName';
	IF i>0 THEN
		DBMS_OUTPUT.PUT_LINE('*************************************************************');		
		DBMS_OUTPUT.PUT_LINE(' The &&schemaName user have currentely '||i||' connection/s.');		
                DBMS_OUTPUT.PUT_LINE(' Please close all connections to continue. Waiting...');		
		DBMS_OUTPUT.PUT_LINE('*************************************************************');		
	END IF;
END;
/

DECLARE
        i NUMBER;
BEGIN
	select count(*) into i from dba_users where username = '&&schemaName';
	if i = 1 then
		select count(*) into i FROM v$session WHERE SCHEMANAME='&&schemaName';
		WHILE i>0 LOOP
			select count(*) into i FROM v$session WHERE SCHEMANAME='&&schemaName';
			dbms_lock.SLEEP(1);
		END LOOP;
		EXECUTE immediate 'DROP USER &&schemaName CASCADE';	
	end if;
END;
/

PROMPT 'Dropping tablespaces...'

DECLARE
   n                NUMBER;
   dataFileClause   VARCHAR2 (20);
BEGIN
   IF &&oraVersionNumber = 8 THEN
      dataFileClause := '';
   ELSE
      dataFileClause := 'AND DATAFILES';
   END IF;

   SELECT COUNT (*) INTO n
     FROM dba_tablespaces
    WHERE tablespace_name = '&&tableSpaceName';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP TABLESPACE &&tableSpaceName INCLUDING CONTENTS ' || dataFileClause || ' CASCADE CONSTRAINTS';
   END IF;

   SELECT COUNT (*) INTO n
     FROM dba_tablespaces
    WHERE tablespace_name = '&&indexTableSpaceName';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP TABLESPACE &&indexTableSpaceName INCLUDING CONTENTS ' || dataFileClause || ' CASCADE CONSTRAINTS';
   END IF;

   SELECT COUNT (*) INTO n
     FROM dba_tablespaces
    WHERE tablespace_name = '&&tempTableSpaceName';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP TABLESPACE &&tempTableSpaceName INCLUDING CONTENTS ' || dataFileClause || ' CASCADE CONSTRAINTS';
   END IF;

   SELECT COUNT (*) INTO n
     FROM dba_tablespaces
    WHERE tablespace_name = '&&auditTableSpaceName';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP TABLESPACE &&auditTableSpaceName INCLUDING CONTENTS ' || dataFileClause || ' CASCADE CONSTRAINTS';
   END IF;

   SELECT COUNT (*) INTO n
     FROM dba_tablespaces
    WHERE tablespace_name = '&&lobsTableSpaceName';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP TABLESPACE &&lobsTableSpaceName INCLUDING CONTENTS ' || dataFileClause || ' CASCADE CONSTRAINTS';
   END IF;

   SELECT COUNT (*) INTO n
     FROM dba_tablespaces
    WHERE tablespace_name = '&&cscartTableSpaceName';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP TABLESPACE &&cscartTableSpaceName INCLUDING CONTENTS ' || dataFileClause || ' CASCADE CONSTRAINTS';
   END IF;
END;
/

PROMPT 'dropping test roles...'

DECLARE
   n   NUMBER;
BEGIN



   SELECT COUNT (*) INTO n
     FROM dba_roles
    WHERE ROLE = 'COE_DV_ADMIN';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP ROLE COE_DV_ADMIN';
   END IF;

   
   SELECT COUNT (*) INTO n
     FROM dba_roles
    WHERE ROLE = 'COE_SEC_ADMIN';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP ROLE COE_SEC_ADMIN';
   END IF; 
END;
/



PROMPT 'Dropping CsUserProfile...'

DECLARE
   n   NUMBER;
BEGIN
   SELECT COUNT (*) INTO n
     FROM dba_profiles
    WHERE PROFILE = 'CSUSERPROFILE';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP PROFILE CSUSERPROFILE CASCADE';
   END IF;
END;
/

PROMPT Finished dropping &&schemaName.
PROMPT 