-- Add/modify columns 
alter table &&schemaName..TEMPORARY_BATCH add SUBMISSIONCOMMENTS varchar2(2000);
-- Create/Recreate primary, unique and foreign key constraints 


begin
  -- Call the procedure
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'SUBMISSIONCOMMENTS', 'SUBMISSIONCOMMENTS', 'VW_TEMPORARYBATCH', 'TEMPORARY_BATCH');
end;
/


