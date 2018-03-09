-- Add/modify columns 
alter table &&schemaName..MIXTURES add statusid number(1) default 0;

begin
  -- Call the procedure
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'STATUSID', 'STATUSID', 'VW_MIXTURE', 'MIXTURES');

  update &&schemaName..MIXTURES SET STATUSID = DECODE(NVL(APPROVED, 'F'), 'T', 4, 3); -- It it was approved, then status is LOCKED (4) otherwhise it is REGISTERED (3)
  
end;
/