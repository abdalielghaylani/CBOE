-- Before insert on inv_reports 
CREATE OR REPLACE TRIGGER "TRG_INV_REPORTS" 
    BEFORE INSERT 
    ON "INV_REPORTS" 
    FOR EACH ROW WHEN (new.id is null or new.id = 0) begin
  select trunc(seq_rid.nextval)
  into :new.id
  from dual;
end;
/

