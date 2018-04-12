rem inv_orders_bi0.trg
rem Generated on 24-JAN-05
rem Script to create BI audit trigger for the INV_ORDERS table.

create or replace trigger TRG_AUDIT_INV_ORDERS_BI0
  before insert
  on inv_orders
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;
/

