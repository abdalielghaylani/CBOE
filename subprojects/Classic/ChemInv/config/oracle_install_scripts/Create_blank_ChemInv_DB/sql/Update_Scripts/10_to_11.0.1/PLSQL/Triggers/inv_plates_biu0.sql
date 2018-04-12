CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_PLATES_BIU0
  before insert or update
  on inv_plates
  for each row
begin
    if :new.rid is null or :new.rid = 0 then
        select trunc(seq_rid.nextval) into :new.rid
          from dual;
    end if;
    if :new.creator is null then
        select rtrim(user) into :new.creator
          from dual;
    end if;
    if :new.timestamp is null then
        select sysdate into :new.timestamp
          from dual;
    end if;
end;
/