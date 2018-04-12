-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Incrementing Sequences after data load
--######################################################### 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Incrementing sequences...'
prompt '#########################################################'

DECLARE
	PROCEDURE incrementSequence(pTableName IN varchar2, pPK IN varchar2, pSeqName IN varchar2) IS
		vMaxID number;
		vCurrValue number;
	BEGIN

		EXECUTE IMMEDIATE 'SELECT max(' || pPK || ') FROM ' || pTableName INTO vMaxID;
		EXECUTE IMMEDIATE 'select ' || pSeqName || '.nextval from dual' INTO vCurrValue;
		WHILE vCurrValue <= vMaxID
		LOOP
			EXECUTE IMMEDIATE 'select ' || pSeqName || '.nextval from dual' INTO vCurrValue;
		END LOOP;
	END incrementSequence;

BEGIN
	incrementSequence('inv_graphics', 'graphic_id', 'seq_inv_graphics');
	incrementSequence('inv_location_types', 'location_type_id', 'seq_inv_location_types');
	
END;
/
