--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

PROMPT Starting Synonyms.sql

DECLARE
   PROCEDURE createSynonym (synName IN VARCHAR2) IS
      n   NUMBER;
   BEGIN
      SELECT COUNT (*) INTO n
        FROM dba_synonyms
       WHERE UPPER (synonym_name) = synName;

      IF n = 0 THEN
         EXECUTE IMMEDIATE 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || synName;
      END IF;
   END createSynonym;
BEGIN
   createSynonym('COEDATAVIEW');
   createSynonym('COEDATABASE');
   createSynonym('COEFORM');
   createSynonym('COEGENERICOBJECT');
   createSynonym('COEGLOBALS');
   createSynonym('COETEMPIDS');
   createSynonym('COEOBJECTTYPE');
   createSynonym('COEPRINCIPTALTYPE');
   createSynonym('COEPERMISSIONS');
   createSynonym('COESESSION');
   createSynonym('COESAVEDSEARCHCRITERIA');
   createSynonym('COESEARCHCRITERIA');
   createSynonym('COESAVEDHITLIST');
   createSynonym('COETEMPHITLIST');
   createSynonym('COESAVEDHITLIST');
   createSynonym('COETEMPHITLIST');
   createSynonym('COESAVEDHITLISTID');
   createSynonym('COETEMPHITLISTID');
   createSynonym('COEPARTITIONMANAGMENT');
   createSynonym('CREATESERVICETABLES');
   createSynonym('CONFIGURATIONMANAGER');
   createSynonym('MYTABLETYPE');
END;
/