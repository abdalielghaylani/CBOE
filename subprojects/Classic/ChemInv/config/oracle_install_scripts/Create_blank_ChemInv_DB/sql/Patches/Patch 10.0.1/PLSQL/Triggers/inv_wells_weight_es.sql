Create or replace TRIGGER "TRG_INV_WELLS_WEIGHT_ES" BEFORE
    INSERT 
    ON "CHEMINVDB2"."INV_WELLS"
    FOR EACH ROW

WHEN ( NEW.weight is not null)
DECLARE

BEGIN
:new.qty_initial:=:new.weight;
:new.qty_remaining:=:new.weight;
:new.qty_unit_fk:=:new.weight_unit_fk;
END;

/
