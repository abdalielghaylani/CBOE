-- Add columns :

/* Add a new column BATCH_PREFIX to BATCHES,TEMPORARY_BATCH table */
DECLARE
  LColumnExists integer := 0;
BEGIN
  select count(*) into LColumnExists
  from all_tab_cols t
  where t.owner = upper('&&schemaName')
    and t.table_name = 'BATCHES'
    and t.column_name = 'BATCH_PREFIX';

  if ( LColumnExists = 0 ) then
	  execute immediate
      'ALTER TABLE &&schemaName..BATCHES ADD BATCH_PREFIX VARCHAR2(10 BYTE)';
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
    and t.column_name = 'BATCH_PREFIX';

  if ( LColumnExists = 0 ) then
	  execute immediate
      'ALTER TABLE &&schemaName..TEMPORARY_BATCH ADD BATCH_PREFIX VARCHAR2(10 BYTE)';
  end if;
END;

/

-- Add Views :

begin
  -- Call the procedure
  -- @BATCH_PREFIX
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'BATCH_PREFIX', 'BATCH_PREFIX', 'VW_BATCH', 'BATCHES');

&&schemaName..configurationcompoundregistry.addfieldtoview(
    'BATCH_PREFIX', 'BATCH_PREFIX', 'VW_TEMPORARYBATCH', 'TEMPORARY_BATCH');

&&schemaName..configurationcompoundregistry.addfieldtoview(
    'BATCH_PREFIX', 'BATCH_PREFIX', 'VW_MIXTURE_BATCH', 'BATCHES');
  
end;
/