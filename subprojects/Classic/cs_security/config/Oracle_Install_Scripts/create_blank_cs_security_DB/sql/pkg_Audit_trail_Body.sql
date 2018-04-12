
CREATE OR REPLACE PACKAGE BODY audit_trail IS

  PROCEDURE record_transaction

   (raid IN NUMBER,
     tabname IN VARCHAR2,
     erid IN NUMBER,
     act IN VARCHAR2) IS

  BEGIN   

  -- Write a record of the transaction to the master audit trail
  -- table.  Timestamp and User_Name are not included here - 
  -- they are filled in by column defaults defined at the table
  -- level (should be faster). 

    INSERT INTO audit_row
      (raid, table_name, rid, action, timestamp, user_name)
      VALUES  
      (raid, tabname, erid, act, sysdate, user); 

  -- Return a scary message if for some reason the statement failed.  

    IF sql%NOTFOUND THEN  
      RAISE_APPLICATION_ERROR
        (-20000, 'Error creating Row Audit record.');
    END IF;   
  END;

  PROCEDURE column_update

    (raid IN NUMBER, 
     colname IN VARCHAR2, 
     oldval IN VARCHAR2, 
     newval IN VARCHAR2) IS 

  BEGIN

  -- Record the column names and old/new data values of individual
  -- columns altered in the transaction to the AUDIT_COLUMN table.

    INSERT INTO audit_column 
      (raid, caid, column_name, old_value, new_value) 
      VALUES
      (raid, seq_audit.nextval, colname, oldval, newval);

  -- Return a scary message if for some reason the statement fails.

    IF sql%NOTFOUND THEN  
      RAISE_APPLICATION_ERROR
        (-20000, 'Error creating Column Audit record.');
    END IF;   
  END;
END audit_trail;
/
show errors;

