
CREATE OR REPLACE PROCEDURE ClobToTable(c IN CLOB) AS  i INTEGER;  commaPos INTEGER;  previousCommaPos INTEGER;  val VARCHAR(30);  length INTEGER; BEGIN  i := 1;  previousCommaPos := 0;  length := DBMS_LOB.GETLENGTH(c);  LOOP       commaPos := DBMS_LOB.INSTR(c, ',', 1, i);      IF commaPos != 0 THEN       val := DBMS_LOB.SUBSTR(c, commaPos - previousCommaPos - 1, previousCommaPos + 1);       previousCommaPos := commaPos;       INSERT INTO TEMP_IDS (ID) VALUES (val);   ELSE       val := DBMS_LOB.SUBSTR(c, length - previousCommaPos, previousCommaPos + 1);       INSERT INTO TEMP_IDS (ID) VALUES (val);    EXIT;   END IF;   i := i + 1;   END LOOP; END;
/

SHOW ERRORS;

GRANT EXECUTE ON  CLOBTOTABLE TO PUBLIC;