prompt starting triggers.sql
CREATE SEQUENCE Seq_Parent_Compounds
	INCREMENT BY 1
	START WITH 30000
;
CREATE SEQUENCE Seq_DrugDeg_Experiments
	INCREMENT BY 1
	START WITH 30000
;
CREATE SEQUENCE Seq_Degradant_Compounds
	INCREMENT BY 1
	START WITH 30000
;
CREATE SEQUENCE Seq_DrugDeg_Mechanisms
	INCREMENT BY 1
	START WITH 30000
;
CREATE SEQUENCE Seq_Salt_Names
	INCREMENT BY 1
	START WITH 30000
;
CREATE SEQUENCE Seq_DrugDeg_base64
	INCREMENT BY 1
	START WITH 30000
;
CREATE SEQUENCE Seq_DrugDeg_Conditions
	INCREMENT BY 1
	START WITH 30000
;
CREATE SEQUENCE Seq_DrugDeg_FGroups
	INCREMENT BY 1
	START WITH 30000
;


CREATE SEQUENCE SEQ_DRUGDEG_STATUSES
  START WITH 20001
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

-- SEQUENCE: MOLID_SEQ

--DROP SEQUENCE MOLID_SEQ;
CREATE SEQUENCE MOLID_SEQ INCREMENT BY 1 START WITH 500;



-- 
-- TRIGGER: TRIGGER_BASE64_ID 
--

-- BEGIN PL/SQL BLOCK (do not remove this line) -------------------------------- 
CREATE TRIGGER trigger_base64_ID 
BEFORE INSERT ON DrugDeg_base64 FOR EACH ROW 
BEGIN 
SELECT Seq_DrugDeg_base64.NEXTVAL INTO :NEW.BASE64_ID FROM DUAL; 
END; 
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/ 

-- 
-- TRIGGER: TRIGGER_DEG_COND_KEY 
--

-- BEGIN PL/SQL BLOCK (do not remove this line) -------------------------------- 
CREATE TRIGGER trigger_Deg_Cond_Key 
BEFORE INSERT ON DrugDeg_Conds FOR EACH ROW 
BEGIN 
SELECT Seq_DrugDeg_Conditions.NEXTVAL INTO :NEW.DEG_COND_KEY FROM DUAL; 
END; 
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/ 

-- TRIGGER: TRIGGER_DEG_COND_KEY 
--

-- BEGIN PL/SQL BLOCK (do not remove this line) -------------------------------- 
CREATE TRIGGER trigger_Deg_FGroup_Key 
BEFORE INSERT ON DrugDeg_FGroups FOR EACH ROW 
BEGIN 
SELECT Seq_DrugDeg_FGroups.NEXTVAL INTO :NEW.DEG_FGROUP_KEY FROM DUAL; 
END; 
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/ 




-- 
-- TRIGGER: TRIGGER_DEG_CMPD_KEY 
--

-- BEGIN PL/SQL BLOCK (do not remove this line) -------------------------------- 
CREATE TRIGGER trigger_Deg_Cmpd_Key 
BEFORE INSERT ON DrugDeg_Degs FOR EACH ROW 
BEGIN 
SELECT Seq_Degradant_Compounds.NEXTVAL INTO :NEW.DEG_CMPD_KEY FROM DUAL; 
END; 
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/ 

-- 
-- TRIGGER: TRIGGER_EXPT_KEY 
--

-- BEGIN PL/SQL BLOCK (do not remove this line) -------------------------------- 
CREATE TRIGGER trigger_Expt_Key 
BEFORE INSERT ON DrugDeg_Expts FOR EACH ROW 
BEGIN 
SELECT Seq_DrugDeg_Experiments.NEXTVAL INTO :NEW.EXPT_KEY FROM DUAL; 
END; 
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/ 

-- 
-- TRIGGER: TRIGGER_DEG_MECH_KEY 
--

-- BEGIN PL/SQL BLOCK (do not remove this line) -------------------------------- 
CREATE TRIGGER trigger_Deg_Mech_Key 
BEFORE INSERT ON DrugDeg_Mechs FOR EACH ROW 
BEGIN 
SELECT Seq_DrugDeg_Mechanisms.NEXTVAL INTO :NEW.MECH_KEY FROM DUAL; 
END; 
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/ 

-- 
-- TRIGGER: TRIGGER_PARENT_COMPOUND_KEY 
--

-- BEGIN PL/SQL BLOCK (do not remove this line) -------------------------------- 
CREATE TRIGGER trigger_Parent_Compound_Key 
BEFORE INSERT ON DrugDeg_Parents FOR EACH ROW 
BEGIN 
SELECT Seq_Parent_Compounds.NEXTVAL INTO :NEW.PARENT_CMPD_KEY FROM DUAL; 
END; 
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/ 

-- 
-- TRIGGER: TRIGGER_SALT_NAMES 
--

-- BEGIN PL/SQL BLOCK (do not remove this line) -------------------------------- 
CREATE TRIGGER trigger_Salt_Names 
BEFORE INSERT ON DrugDeg_Salts FOR EACH ROW 
BEGIN 
SELECT Seq_Salt_Names.NEXTVAL INTO :NEW.SALT_KEY FROM DUAL; 
END; 
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/ 


-- 
-- TRIGGER: TRG_DD_BASE64 
--

-- Create table level triggers for DRUGDEG_PARENTS table
create or replace trigger TRG_DD_BASE64
BEFORE INSERT ON DRUGDEG_PARENTS
FOR EACH ROW

BEGIN
SELECT MOLID_SEQ.NEXTVAL INTO :NEW.MOL_ID  FROM DUAL;
END;
/


-- Create table level triggers for DRUGDEG_PARENTS table
create or replace trigger TRG_DEG_BASE64
BEFORE INSERT ON DRUGDEG_DEGS
FOR EACH ROW

BEGIN
SELECT MOLID_SEQ.NEXTVAL INTO :NEW.MOL_ID  FROM DUAL;
END;
/

-- Create table level triggers for DRUGDEG_PARENTS table
create or replace trigger TRG_MECHS_BASE64
BEFORE INSERT ON DRUGDEG_MECHS
FOR EACH ROW

BEGIN
SELECT MOLID_SEQ.NEXTVAL INTO :NEW.MOL_ID  FROM DUAL;
END;
/


CREATE OR REPLACE TRIGGER trigger_Status_Key
BEFORE INSERT ON DrugDeg_Statuses FOR EACH ROW

BEGIN
SELECT Seq_DrugDeg_Statuses.NEXTVAL INTO :NEW.STATUS_KEY FROM DUAL;
END;
-- END PL/SQL BLOCK (do not remove this line) ---------------------------------- 
/