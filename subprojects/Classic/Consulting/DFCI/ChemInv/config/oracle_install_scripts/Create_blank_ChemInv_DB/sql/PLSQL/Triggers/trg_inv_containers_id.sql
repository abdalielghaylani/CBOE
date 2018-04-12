CREATE OR REPLACE
TRIGGER "TRG_INV_CONTAINERS_ID"
    BEFORE INSERT
    ON "INV_CONTAINERS"
    FOR EACH ROW
    
begin
		if :new.container_id is null then
			select seq_Inv_Containers.nextval into :new.container_id from dual;
		end if;
		if :new.Barcode is null then
			:new.Barcode := :new.container_id;
		end if;
		if :new.Container_Name is null then
			select seq_Inv_Containers.currval into :new.Container_Name from dual;
		end if;
		IF :NEW.parent_container_id_fk IS NULL THEN
    	:NEW.family := :NEW.container_id;
		END IF;    
end;
/
