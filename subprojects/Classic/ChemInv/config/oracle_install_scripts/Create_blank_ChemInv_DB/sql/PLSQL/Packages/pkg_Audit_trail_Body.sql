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
  
  FUNCTION GETLASTCOLUMNVALUE(
  	pContainerID inv_containers.container_id%type,
    pTableName audit_row.table_name%TYPE,
		pColumnName audit_column.column_name%TYPE)
 	RETURN audit_column.old_value%TYPE
  IS
	  mySQL varchar2(2000);
    vRAID audit_row.raid%TYPE;
    vRID audit_row.rid%TYPE;
    vOldvalue audit_column.old_value%TYPE;
    vNewValue audit_column.new_value%TYPE;
  BEGIN
 		mySQL:=	 'SELECT c.raid, r.rid, old_value, new_value
			FROM audit_column c, audit_row r, (SELECT MAX(ar.raid) AS maxRAID FROM audit_row ar, audit_column ac WHERE ar.raid = ac.raid AND column_name = :ColumnName GROUP BY rid) maxRAID
			WHERE c.raid = r.raid
      AND table_name = :TableName
			AND column_Name = :pColumnName
			AND r.rid = (SELECT rid from inv_containers where container_ID = :ContainerID)
      AND r.raid = maxRAID.maxRAID';
    --RETURN mySQL;
		EXECUTE IMMEDIATE mySQL INTO vRAID, vRID, vOldValue, vNewValue 
		USING Upper(pColumnName), Upper(pTableName), Upper(pColumnName), pContainerID;
    
    RETURN vOldValue;

	END GETLASTCOLUMNVALUE;

  PROCEDURE check_val( 
  	l_raid IN audit_row.raid%TYPE,
    l_cname IN VARCHAR2,
    l_new IN VARCHAR2,
    l_old IN VARCHAR2 )
  IS
  BEGIN
      IF ( l_new <> l_old OR
           (l_new IS NULL AND l_old IS NOT NULL) OR
           (l_new IS NOT NULL AND l_old IS NULL) )
      THEN
	       column_update(l_raid, upper(l_cname), l_old, l_new);
      END IF;
  END;
  
  PROCEDURE check_val(
  	l_raid IN audit_row.raid%TYPE, 
    l_cname IN VARCHAR2,
		l_new IN DATE, 
    l_old IN DATE )
  IS
  BEGIN
      IF ( l_new <> l_old OR
           (l_new IS NULL AND l_old IS NOT NULL) OR
           (l_new IS NOT NULL AND l_old IS NULL) )
      THEN
	       column_update(l_raid, upper(l_cname), l_old, l_new);
      END IF;
  END;
  
  PROCEDURE check_val(
  	l_raid IN audit_row.raid%TYPE, 
		l_cname IN VARCHAR2,
    l_new IN number, 
    l_old IN number )
  IS
  BEGIN
      IF ( l_new <> l_old OR
           (l_new IS NULL AND l_old IS NOT NULL) OR
           (l_new IS NOT NULL AND l_old IS NULL) )
      THEN
	       column_update(l_raid, upper(l_cname), l_old, l_new);
      END IF;
  END;  

END audit_trail;
/
show errors;

