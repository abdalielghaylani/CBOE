--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

prompt 
prompt Starting "Create_ChemReg_Test_Users.sql"...
prompt 

--#########################################################
-- PROMPT THE USER
--######################################################### 

prompt 'dropping test users...'

DECLARE
	PROCEDURE dropUser(uName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_users where username = uName;
			if n =1 then
				execute immediate 'DROP USER ' || uName;
			end if;
		END dropUser;
BEGIN
	dropUser('T1_84');
	dropUser('T2_84');
	dropUser('T3_84');
	dropUser('T4_84');
	dropUser('T5_84');
	dropUser('T6_84');
	dropUser('T1_85');
	dropUser('T2_85');
	dropUser('T3_85');
	dropUser('T4_85');
	dropUser('T5_85');
	dropUser('T6_85');
end;
/

prompt creating test users...

CREATE USER T1_84 IDENTIFIED BY T1_84 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT BROWSER TO T1_84 ;
ALTER USER T1_84 DEFAULT ROLE ALL;
ALTER USER T1_84 GRANT CONNECT THROUGH COEUSER;

CREATE USER T2_84 IDENTIFIED BY T2_84 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT SUBMITTER TO T2_84;
ALTER USER T2_84 DEFAULT ROLE ALL;
ALTER USER T2_84 GRANT CONNECT THROUGH COEUSER;


CREATE USER T3_84 IDENTIFIED BY T3_84 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT SUPERVISING_SCIENTIST TO T3_84;
ALTER USER T3_84 DEFAULT ROLE ALL;
ALTER USER T3_84 GRANT CONNECT THROUGH COEUSER;


CREATE USER T4_84 IDENTIFIED BY T4_84 DEFAULT TABLESPACE &&tableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT CHEMICAL_ADMINISTRATOR TO T4_84;
ALTER USER T4_84 DEFAULT ROLE ALL;
ALTER USER T4_84 GRANT CONNECT THROUGH COEUSER;


CREATE USER T5_84 IDENTIFIED BY T5_84 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT SUPERVISING_CHEMICAL_ADMIN TO T5_84;
ALTER USER T5_84 DEFAULT ROLE ALL;
ALTER USER T5_84 GRANT CONNECT THROUGH COEUSER;


CREATE USER T6_84 IDENTIFIED BY T6_84 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT PERFUME_CHEMIST TO T6_84;
ALTER USER T6_84 DEFAULT ROLE ALL;
ALTER USER T6_84 GRANT CONNECT THROUGH COEUSER;


CREATE USER T1_85 IDENTIFIED BY T1_85 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT BROWSER TO T1_85 ;
ALTER USER T1_85 DEFAULT ROLE ALL;
ALTER USER T1_85 GRANT CONNECT THROUGH COEUSER;


CREATE USER T2_85 IDENTIFIED BY T2_85 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT SUBMITTER TO T2_85;
ALTER USER T2_85 DEFAULT ROLE ALL;
ALTER USER T2_85 GRANT CONNECT THROUGH COEUSER;


CREATE USER T3_85 IDENTIFIED BY T3_85 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT SUPERVISING_SCIENTIST TO T3_85;
ALTER USER T3_85 DEFAULT ROLE ALL;
ALTER USER T3_85 GRANT CONNECT THROUGH COEUSER;


CREATE USER T4_85 IDENTIFIED BY T4_85 DEFAULT TABLESPACE &&tableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT CHEMICAL_ADMINISTRATOR TO T4_85;
ALTER USER T4_85 DEFAULT ROLE ALL;
ALTER USER T4_85 GRANT CONNECT THROUGH COEUSER;


CREATE USER T5_85 IDENTIFIED BY T5_85 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT SUPERVISING_CHEMICAL_ADMIN TO T5_85;
ALTER USER T5_85 DEFAULT ROLE ALL;
ALTER USER T5_85 GRANT CONNECT THROUGH COEUSER;


CREATE USER T6_85 IDENTIFIED BY T6_85 DEFAULT TABLESPACE &&tableSpaceName  PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT PERFUME_CHEMIST TO T6_85;
ALTER USER T6_85 DEFAULT ROLE ALL;
ALTER USER T6_85 GRANT CONNECT THROUGH COEUSER;

Delete from &&securitySchemaName..people where user_id = 'T1_84';
Delete from &&securitySchemaName..people where user_id = 'T2_84';
Delete from &&securitySchemaName..people where user_id = 'T3_84';
Delete from &&securitySchemaName..people where user_id = 'T4_84';
Delete from &&securitySchemaName..people where user_id = 'T5_84';
Delete from &&securitySchemaName..people where user_id = 'T6_84';
Delete from &&securitySchemaName..people where user_id = 'T1_85';
Delete from &&securitySchemaName..people where user_id = 'T2_85';
Delete from &&securitySchemaName..people where user_id = 'T3_85';
Delete from &&securitySchemaName..people where user_id = 'T4_85';
Delete from &&securitySchemaName..people where user_id = 'T5_85';
Delete from &&securitySchemaName..people where user_id = 'T6_85';

Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T6_84','C684',(SELECT person_id from &&securitySchemaName..people where user_id = 'CSSADMIN'),'T6_84','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T1_84','C184',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_84'),'T1_84','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T2_84','C284',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_84'),'T2_84','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T3_84','C384',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_84'),'T3_84','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T4_84','C484',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_84'),'T4_84','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T5_84','C584',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_84'),'T5_84','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T6_85','C685',(SELECT person_id from &&securitySchemaName..people where user_id = 'CSSADMIN'),'T6_85','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T1_85','C185',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_85'),'T6_85','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T2_85','C285',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_85'),'T2_85','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T3_85','C385',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_85'),'T3_85','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T4_85','C485',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_85'),'T4_85','1');
Insert into &&securitySchemaName..people(person_id, user_id, user_code, supervisor_internal_id, last_name,active)values(&&securitySchemaName..PEOPLE_SEQ.NEXTVAL,'T5_85','C585',(SELECT person_id from &&securitySchemaName..people where user_id = 'T6_85'),'T5_85','1');


commit;
