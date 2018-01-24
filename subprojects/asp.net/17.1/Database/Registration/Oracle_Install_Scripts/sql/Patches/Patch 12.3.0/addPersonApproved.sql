-- Add/modify columns 
alter table &&schemaName..BATCHES add personapproved number(8);
alter table &&schemaName..TEMPORARY_BATCH add personapproved number(8);
alter table &&schemaName..COMPOUND_MOLECULE add personapproved number(8);
alter table &&schemaName..TEMPORARY_COMPOUND add personapproved number(8);
alter table &&schemaName..MIXTURES add personapproved number(8);

begin
  -- Call the procedure
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'PERSONAPPROVED', 'PERSONAPPROVED', 'VW_BATCH', 'BATCHES');

 &&schemaName..configurationcompoundregistry.addfieldtoview(
    'PERSONAPPROVED', 'PERSONAPPROVED', 'VW_COMPOUND', 'COMPOUND_MOLECULE');

&&schemaName..configurationcompoundregistry.addfieldtoview(
    'PERSONAPPROVED', 'PERSONAPPROVED', 'VW_TEMPORARYCOMPOUND', 'TEMPORARY_COMPOUND');

&&schemaName..configurationcompoundregistry.addfieldtoview(
    'PERSONAPPROVED', 'PERSONAPPROVED', 'VW_TEMPORARYBATCH', 'TEMPORARY_BATCH');
&&schemaName..configurationcompoundregistry.addfieldtoview(
    'PERSONAPPROVED', 'PERSONAPPROVED', 'VW_MIXTURE', 'MIXTURES');
 
end;
/