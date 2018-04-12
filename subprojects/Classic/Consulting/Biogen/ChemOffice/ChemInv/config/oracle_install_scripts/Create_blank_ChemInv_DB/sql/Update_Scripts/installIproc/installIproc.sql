-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Update file for integrating Inventory Enterprise with Registration Enterprise
--######################################################### 


spool ON
spool ..\..\Logs\LOG_installIproc.txt

--' Intialize variables
@../../Parameters.sql
@../../Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName

GRANT CREATE ANY INDEX TO &&schemaName;
GRANT CREATE ANY SNAPSHOT TO &&schemaName;
GRANT CREATE ANY VIEW TO &&schemaName;

create user iproc identified by iproc123;
grant "CONNECT" to iproc;

Connect &&schemaName/&&schemaPass@&&serverName

-- Create Shopping Session table

create table shopping_session(
session_id number(9),
userid varchar2(30),
session_name varchar2(200),
status number(1) default 0,
timestamp date default sysdate,
  CONSTRAINT "SHOPPING_SESSION_PK"
		PRIMARY KEY("SESSION_ID") USING INDEX TABLESPACE &&indexTableSpaceName
);
CREATE SEQUENCE SEQ_CART_SESSION INCREMENT BY 1 START WITH 1000 MAXVALUE 999999999 MINVALUE 1 NOCYCLE NOCACHE ORDER;

CREATE OR REPLACE TRIGGER "TRG_CART_SESSION_ID"
    BEFORE INSERT
    ON "SHOPPING_SESSION"
    FOR EACH ROW
    begin
		if :new.SESSION_ID is null then
			select seq_cart_session.nextval into :new.session_id from dual;
		end if;
end;
/


-- Create shopping carts table

create table shopping_carts (
cart_id NUMBER(9),
userid VARCHAR(30),
create_date DATE default sysdate,
cart_xml clob,
status number(1),
  CONSTRAINT "SHOPPING_CART_PK"
		PRIMARY KEY("CART_ID") USING INDEX TABLESPACE &&indexTableSpaceName
		);

CREATE SEQUENCE SEQ_CART INCREMENT BY 1 START WITH 1000 MAXVALUE 999999999 MINVALUE 1 NOCYCLE NOCACHE ORDER;
CREATE INDEX CART_USER_IDX ON SHOPPING_CARTS(USERID) TABLESPACE &&indexTableSpaceName;

CREATE OR REPLACE TRIGGER "TRG_CART_ID"
    BEFORE INSERT
    ON "SHOPPING_CARTS"
    FOR EACH ROW
    begin
		if :new.CART_ID is null then
			select seq_CART.nextval into :new.cart_id from dual;
		end if;
end;
/

create or replace function getCartSeq return number
IS
vReturnSeq number;
Begin
select seq_cart.nextval into vReturnSeq from dual;
return vReturnSeq;
end;
/
create or replace function insertShoppingCartXML(
			pCartId number, pUserId varchar2,pOrderLine varchar2) return number

			IS
				vTempClob CLOB;
			   vCount number;
			Begin

			-- Check to see if shopping cart already exists.  If not, create it.
			select count(*) into vCount from shopping_carts where cart_id= pCartId;
			if vCount=0 then
			       		INSERT INTO shopping_carts (cart_id, userid, status, cart_xml, create_date) VALUES
														(pCartId,pUserId,-1,
'<?xml version=''1.0'' encoding=''UTF-8''?>
<response>
	<header version="1.0">
		<return returnCode="S" />
	</header>
	<body>
		<OrderLinesDataElements>
			<catalogTradingPartner>ChemACX</catalogTradingPartner>
			',
			sysdate);
			end if;

			update shopping_carts set cart_xml = cart_xml || '

			' || pOrderLine where cart_id=pCartId;
			return pCartId;

			end;
/
create or replace function finishShoppingCartXML(pCartId number) return number
is

begin
update shopping_carts set status=0,cart_xml=cart_xml||'</OrderLinesDataElements></body></response>' where cart_id=pCartId and status=-1;
return pCartId;
exception when others then
return -1;
end;
/

create or replace
FUNCTION CREATEPROCSESSION
(
  puserid IN VARCHAR2
, psessionname IN VARCHAR2
)
RETURN NUMBER AS
pSessionId number;
BEGIN
insert into shopping_session(userid, session_name, status) values (pUserId, pSessionName, 0) returning session_id into pSessionId;
  RETURN pSessionId;

END CREATEPROCSESSION;
/
CREATE OR REPLACE FUNCTION CHECKPROCSESSION
(
 pUserId IN VARCHAR2
, pSessionName IN VARCHAR2
,  pSessionId IN NUMBER
) RETURN NUMBER AS
vCount number;
BEGIN
select count(*) into vCount from shopping_session where status = 0 and userid=pUserId and session_name=pSessionName and session_id=pSessionId;
if vCount = 1 then
update shopping_session set status = 1 where session_id=pSessionId;
return 1;
  else
return -1;
end if;
exception when others then
  RETURN -1;
END CHECKPROCSESSION;
/
create or replace
FUNCTION CHECKPROCSESSION2
(
 pUserId IN VARCHAR2
, pSessionName IN VARCHAR2
,  pSessionId IN VARCHAR2
, pCartId in varchar2
, pStatus in varchar2
) RETURN NUMBER AS
vCount number;
BEGIN
select count(*) into vCount from shopping_session where  userid=pUserId and session_name=pSessionName and session_id=to_number(pSessionId);
if vCount = 1 then
update shopping_carts set status = to_char(pStatus) where cart_id = to_number(pCartId);
return 1;
  else
return -1;
end if;
exception when others then
 RETURN -1;
END CHECKPROCSESSION2;
/


-- Grant select to iproc user
Grant select,update on shopping_session to iproc;
Grant select,update on shopping_carts to iproc;
Grant execute on CREATEPROCSESSION to iproc;
Grant execute on CHECKPROCSESSION to iproc;
Grant execute on CHECKPROCSESSION2 to iproc;

--Other grants
grant execute on getCartSeq to Cs_security with grant option;
grant execute on insertShoppingCartXML to Cs_security with grant option;
grant execute on finishShoppingCartXML to Cs_security with grant option;
connect cs_security/oracle
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_ORDER_CONTAINER', 'EXECUTE', '&&SchemaName', 'GETCARTSEQ');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_ORDER_CONTAINER', 'EXECUTE', '&&SchemaName', 'INSERTSHOPPINGCARTXML');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_ORDER_CONTAINER', 'EXECUTE', '&&SchemaName', 'FINISHSHOPPINGCARTXML');

--connect chemacxdb/oracle
-- These are disabled here but must be added to chemacx!
--alter table supplier add duns varchar2(20)   ;
--alter table package add unspsc varchar2(20)      ;



--' grant roles based on privs
begin
	FOR role_rec IN (SELECT role_name FROM security_roles WHERE privilege_table_int_id = (SELECT privilege_table_id FROM privilege_tables WHERE privilege_table_name = '&&privTableName'))
	LOOP
		GrantPrivsToRole(role_rec.role_name);
	END LOOP;
end;
/


DECLARE
	cursor cur_invalid_objects is
		select object_name, object_type from user_objects where status='INVALID' AND object_type <> 'VIEW';
	rec_columns cur_invalid_objects%ROWTYPE;
	err_status NUMERIC;
BEGIN
	dbms_output.enable(10000);
	open cur_invalid_objects;
	loop

		fetch cur_invalid_objects into rec_columns;
		EXIT WHEN cur_invalid_objects%NOTFOUND;

		--dbms_output.put_line ('Recompiling ' || rec_columns.object_type || '  ' || rec_columns.object_name);
		dbms_ddl.alter_compile(rec_columns.object_type,NULL,rec_columns.object_name);

	end loop;
	close cur_invalid_objects;

	EXCEPTION
	When others then
		begin
			err_status := SQLCODE;
			--dbms_output.put_line(' Recompilation failed : ' || SQLERRM(err_status));
			if ( cur_invalid_objects%ISOPEN) then
				CLOSE cur_invalid_objects;
			end if;
	
		exception when others then
		null;
	end;
end;
/


prompt 
spool off

exit
