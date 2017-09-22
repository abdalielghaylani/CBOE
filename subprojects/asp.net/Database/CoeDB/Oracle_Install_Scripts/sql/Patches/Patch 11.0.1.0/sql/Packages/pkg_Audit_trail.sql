--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE PACKAGE Audit_Trail IS
   PROCEDURE Record_Transaction (
      raId      IN   NUMBER,
      tabName   IN   VARCHAR2,
      erId      IN   NUMBER,
      act       IN   VARCHAR2
   );

   PROCEDURE Column_Update (
      raId      IN   NUMBER,
      colName   IN   VARCHAR2,
      oldVal    IN   VARCHAR2,
      newVal    IN   VARCHAR2
   );
END Audit_Trail;
/


CREATE OR REPLACE PACKAGE BODY Audit_Trail IS
   PROCEDURE Record_Transaction (
      raId      IN   NUMBER,
      tabName   IN   VARCHAR2,
      erId      IN   NUMBER,
      act       IN   VARCHAR2
   ) IS
   BEGIN
      -- Write a record of the transaction to the master audit trail
      -- table.  Timestamp and User_Name are not included here -
      -- they are filled in by column defaults defined at the table
      -- level (should be faster).
      INSERT INTO audit_row (raId, table_name, rId, action, TIMESTAMP, user_name)
           VALUES (raId, tabName, erId, act, SYSDATE, USER);

      -- Return a scary message if for some reason the statement failed.
      IF SQL%NOTFOUND THEN
         Raise_Application_Error (-20000, 'Error creating Row Audit record.');
      END IF;
   END;

   PROCEDURE column_update (
      raId      IN   NUMBER,
      colName   IN   VARCHAR2,
      oldVal    IN   VARCHAR2,
      newVal    IN   VARCHAR2
   ) IS
   BEGIN
      -- Record the column names and old/new data values of individual
      -- columns altered in the transaction to the AUDIT_COLUMN table.
      INSERT INTO audit_column (raId, caId, column_name, old_value, new_value)
           VALUES (raId, seq_audit.NEXTVAL, colName, oldVal, newVal);

      -- Return a scary message if for some reason the statement fails.
      IF SQL%NOTFOUND THEN
         Raise_Application_Error (-20000, 'Error creating Column Audit record.');
      END IF;
   END;
END audit_trail;
/

SHOW errors;