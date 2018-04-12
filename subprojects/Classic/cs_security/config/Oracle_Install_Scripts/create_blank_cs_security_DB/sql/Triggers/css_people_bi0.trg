rem css_people_bi0.trg
rem Generated on 28-JAN-05
rem Script to create BI audit trigger for the PEOPLE table.

create or replace trigger TRG_AUDIT_CSS_PEOPLE_BI0
  before insert
  on people
  for each row
  when (new.rid is null or new.rid = 0)

begin
  select trunc(seq_rid.nextval)
  into :new.rid
  from dual;
end;

/

