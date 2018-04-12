rem inv_order_containers_bi0.trg
rem Generated on 25-JAN-05
rem Script to create BI audit trigger for the INV_ORDER_CONTAINERS table.

create or replace trigger TRG_AUDIT_INV_ORD_CNTNRS_BI0
  before insert
  on inv_order_containers
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

