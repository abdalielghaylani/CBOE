-- Add/modify columns 
alter table &schemaName..TEMPORARY_COMPOUND add drawing_type number(1);
-- Create/Recreate primary, unique and foreign key constraints 
alter table &schemaName..TEMPORARY_COMPOUND
  add constraint TMPCMP_DRAWING_TYPE_FK foreign key (DRAWING_TYPE)
  references &schemaName..drawing_type (ID);

begin
  -- Call the procedure
  &schemaName..configurationcompoundregistry.addfieldtoview(
    'DRAWING_TYPE', 'DRAWINGTYPE', 'VW_TEMPORARYCOMPOUND', 'TEMPORARY_COMPOUND');
end;
/

update &schemaName..vw_temporarycompound tc
set tc.drawingtype = ( 
  select nvl(dt.id, 0) from &schemaName..drawing_type dt
  where (
    dbms_lob.compare(nvl(tc.base64_cdx,'Null'),nvl(dt.default_drawing,'Null')) =0
  )
)
where tc.drawingtype is null;
