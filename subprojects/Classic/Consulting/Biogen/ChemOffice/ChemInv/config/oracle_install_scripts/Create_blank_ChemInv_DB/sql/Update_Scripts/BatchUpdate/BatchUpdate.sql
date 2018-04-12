-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--################################################################
-- Headerfile for updating the cheminvdb2 schema from 9SR3 to 10
--################################################################ 

spool ..\..\Logs\batchupdate.txt

-- Intialize variables
@@..\..\Parameters.sql
@@..\..\Prompts.sql
Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Updating Tables and Functions...'
prompt '#########################################################'

insert into inv_container_status values (500,'Submitted',null);
insert into inv_container_status values (501,'Assigned to Requisition',null);

@@PLSQL\Functions\f_UpdateReqNum.sql;


create or replace TRIGGER "TRG_REQ_SHOPPING_CART_STAT"
    After UPDATE
    OF container_status_id_Fk 
    ON "INV_CONTAINERS"
    FOR EACH ROW
BEGIN
		--update shopping cart if added to req. or ordered or order pending.
		IF :NEW.container_status_id_fk = 501 or :new.container_status_id_Fk=4 THEN
    	update shopping_carts set status=3 where to_char(cart_id) = :old.PO_Number;
		END IF;
END;

/

commit;

@@Alter\alter_cs_security.sql;

prompt '#########################################################'
prompt 'Recompiling pl/sql...'
prompt '#########################################################'

--@@PLSQL\RecompilePLSQL.sql


prompt #############################################################
prompt Logged session to: Logs\BatchUpdate.txt
prompt #############################################################

prompt 
spool off

exit
