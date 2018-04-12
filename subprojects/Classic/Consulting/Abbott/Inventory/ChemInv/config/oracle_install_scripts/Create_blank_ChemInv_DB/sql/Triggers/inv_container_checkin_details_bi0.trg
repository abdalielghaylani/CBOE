rem inv_container_checkin_details_bi0.trg
rem Generated on 26-JAN-05
rem Script to create BI audit trigger for the INV_CONTAINER_CHECKIN_DETAILS table.

create or replace trigger TRG_AUDIT_INV_CTNR_CHK_DTL_BI0
  before insert
  on inv_container_checkin_details
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

