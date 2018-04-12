


CONNECT &&InstallUser/&&sysPass@&&serverName


CREATE USER DRUGDEGBROWSER IDENTIFIED BY DRUGDEGBROWSER DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT DRUGDEG_BROWSER TO DRUGDEGBROWSER;
ALTER USER DRUGDEGBROWSER DEFAULT ROLE ALL;

CREATE USER DRUGDEGADMIN IDENTIFIED BY DRUGDEGADMIN DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT DRUGDEG_ADMINISTRATOR TO DRUGDEGADMIN;
ALTER USER DRUGDEGADMIN DEFAULT ROLE ALL;

CREATE USER DRUGDEGSUBMITTER IDENTIFIED BY DRUGDEGSUBMITTER DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT DRUGDEG_SUBMITTER TO DRUGDEGSUBMITTER;
ALTER USER DRUGDEGSUBMITTER DEFAULT ROLE ALL;

CREATE USER SCIENTIST IDENTIFIED BY SCIENTIST DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT DRUGDEG_BROWSER TO SCIENTIST;
ALTER USER Scientist DEFAULT ROLE ALL;

commit;
-- Add the users created above to the cs_security tables.


Connect &&securitySchemaName/&&securitySchemaPass@&&serverName


Delete from people where user_id = 'DRUGDEGADMIN';
Delete from people where user_id = 'DRUGDEGBROWSER';
Delete from people where user_id = 'SCIENTIST';
Delete from people where user_id = 'DRUGDEGSUBMITTER';

commit;

Insert into people
  (person_id, 
   user_id, 
   user_code, 
   supervisor_internal_id, 
   last_name,
   site_id,
   active,
   email)
values
  (PEOPLE_SEQ.NEXTVAL,
   'DRUGDEGADMIN',
   '',
   '4',
   'DRUGDEGADMIN',
   '1',
   '1',
   '');

Insert into people
  (person_id, 
   user_id, 
   user_code, 
   supervisor_internal_id, 
   last_name,
   site_id,
   active,
   email)
values
  (PEOPLE_SEQ.NEXTVAL,
   'DRUGDEGSUBMITTER',
   '',
   '4',
   'DRUGDEGSUBMITTER',
   '1',
   '1',
   '');
   
Insert into people
  (person_id, 
   user_id, 
   user_code, 
   supervisor_internal_id, 
   last_name,
   site_id,
   active,
   email)
values
  (PEOPLE_SEQ.NEXTVAL,
   'DRUGDEGBROWSER',
   '',
   '4',
   'DRUGDEGBROWSER',
   '1',
   '1',
   '');

Insert into people
  (person_id, 
   user_id, 
   user_code, 
   supervisor_internal_id, 
   last_name,
   site_id,
   active,
   email)
values
  (PEOPLE_SEQ.NEXTVAL,
   'SCIENTIST',
   '',
   '4',
   'SCIENTIST',
   '1',
   '1',
   '');
commit;


CONNECT &&InstallUser/&&sysPass@&&serverName


-- DRUGDEGBROWSER 
GRANT CSS_USER TO DRUGDEGBROWSER;
GRANT DOCMGR_BROWSER TO DRUGDEGBROWSER;
GRANT D3_BROWSER TO DRUGDEGBROWSER;

-- DRUGDEGSUBMITTER;
GRANT CSS_USER TO DRUGDEGSUBMITTER;
GRANT DOCMGR_SUBMITTER TO DRUGDEGSUBMITTER;
GRANT DRUGDEG_SUBMITTER TO DRUGDEGSUBMITTER;
GRANT D3_BROWSER TO DRUGDEGSUBMITTER;

-- SCIENTIST;
GRANT CSS_USER TO SCIENTIST;
GRANT DOCMGR_SUBMITTER TO SCIENTIST;
GRANT DRUGDEG_SUBMITTER TO SCIENTIST;
GRANT D3_BROWSER TO SCIENTIST;

--DRUGDEGADMIN
GRANT CSS_ADMIN TO DRUGDEGADMIN;
GRANT DOCMGR_ADMINISTRATOR TO DRUGDEGADMIN;
GRANT D3_BROWSER TO DRUGDEGADMIN;