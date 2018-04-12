


CONNECT &&InstallUser/&&sysPass@&&serverName


CREATE USER D3BROWSER IDENTIFIED BY D3BROWSER DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT D3_BROWSER TO D3BROWSER;
ALTER USER D3BROWSER DEFAULT ROLE ALL;

CREATE USER SCIENTIST IDENTIFIED BY SCIENTIST DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT D3_BROWSER TO SCIENTIST;
ALTER USER Scientist DEFAULT ROLE ALL;

commit;
-- Add the users created above to the cs_security tables.


Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

Delete from people where user_id = 'D3BROWSER';
Delete from people where user_id = 'SCIENTIST';


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
   'D3BROWSER',
   '',
   '4',
   'D3BROWSER',
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