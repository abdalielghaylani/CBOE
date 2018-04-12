CREATE OR REPLACE
TRIGGER "TRG_INV_CONTAINERS_FAMILY_BU"
    BEFORE UPDATE
    OF parent_container_id_fk
    ON "INV_CONTAINERS"
    FOR EACH ROW
BEGIN
		--set family to null if the parent_container_id_fk is removed
		IF :NEW.parent_container_id_fk IS NULL THEN
    	:NEW.family := NULL;
		END IF;
END;
/
