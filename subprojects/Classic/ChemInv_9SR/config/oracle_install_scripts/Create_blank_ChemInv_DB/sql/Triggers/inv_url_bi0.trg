-- inv_url_bi0.trg
-- Before insert on inv_url 
CREATE OR REPLACE TRIGGER "TRG_AUDIT_INV_URL_BI0" 
    BEFORE INSERT 
    ON "INV_URL" 
    FOR EACH ROW WHEN (new.rid is null or new.rid = 0) begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

