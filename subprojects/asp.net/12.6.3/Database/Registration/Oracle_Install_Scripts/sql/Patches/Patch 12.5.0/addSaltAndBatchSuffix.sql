-- Add columns :

/* Add a new column SALTANDBATCHSUFFIX to BATCHES,TEMPORARY_BATCH table */
DECLARE
  LColumnExists integer := 0;
BEGIN
  select count(*) into LColumnExists
  from all_tab_cols t
  where t.owner = upper('&&schemaName')
    and t.table_name = 'BATCHES'
    and t.column_name = 'SALTANDBATCHSUFFIX';

  if ( LColumnExists = 0 ) then
	  execute immediate
      'ALTER TABLE &&schemaName..BATCHES ADD SALTANDBATCHSUFFIX VARCHAR2(100 BYTE)';
  end if;
END;

/

DECLARE
  LColumnExists integer := 0;
BEGIN
  select count(*) into LColumnExists
  from all_tab_cols t
  where t.owner = upper('&&schemaName')
    and t.table_name = 'TEMPORARY_BATCH'
    and t.column_name = 'SALTANDBATCHSUFFIX';

  if ( LColumnExists = 0 ) then
	  execute immediate
      'ALTER TABLE &&schemaName..TEMPORARY_BATCH ADD SALTANDBATCHSUFFIX VARCHAR2(100 BYTE)';
  end if;
END;

/

-- Add Views :

begin
  -- Call the procedure
   -- @SALTANDBATCHSUFFIX
 &&schemaName..configurationcompoundregistry.addfieldtoview(
    'SALTANDBATCHSUFFIX', 'SALTANDBATCHSUFFIX', 'VW_BATCH', 'BATCHES');

&&schemaName..configurationcompoundregistry.addfieldtoview(
    'SALTANDBATCHSUFFIX', 'SALTANDBATCHSUFFIX', 'VW_TEMPORARYBATCH', 'TEMPORARY_BATCH');

&&schemaName..configurationcompoundregistry.addfieldtoview(
    'SALTANDBATCHSUFFIX', 'SALTANDBATCHSUFFIX', 'VW_MIXTURE_BATCH', 'BATCHES');
 
end;
/