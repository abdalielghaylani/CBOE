CREATE OR REPLACE
TRIGGER TRG_PLATE_ID BEFORE INSERT ON INV_PLATES FOR EACH ROW
begin
if :new.PLATE_ID is null then
  select SEQ_INV_PLATES.nextval into :new.PLATE_ID from dual;
end if;
if :new.Plate_Barcode is null then
   :new.Plate_Barcode := :new.PLATE_ID;
end if;
end;
/
