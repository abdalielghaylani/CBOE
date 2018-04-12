CREATE OR REPLACE PACKAGE audit_trail IS

  PROCEDURE record_transaction 
    (raid IN NUMBER,
     tabname IN VARCHAR2,
     erid IN NUMBER,
     act IN VARCHAR2);

  PROCEDURE column_update
    (raid IN NUMBER,
     colname IN VARCHAR2,
     oldval IN VARCHAR2,
     newval IN VARCHAR2);

END audit_trail;
/



