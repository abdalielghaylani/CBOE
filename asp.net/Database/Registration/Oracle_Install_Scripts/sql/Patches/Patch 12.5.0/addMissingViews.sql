
-- Add Views :

begin
  -- Call the procedure
  -- @STATUSID
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'STATUSID', 'STATUSID', 'VW_MIXTURE_REGNUMBER', 'VW_MIXTURE');

-- @SEQUENCENUMBER
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'SEQUENCENUMBER', 'SEQUENCENUMBER', 'VW_MIXTURE_REGNUMBER', 'VW_REGISTRYNUMBER');

-- @PERSONAPPROVED
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'PERSONAPPROVED', 'PERSONAPPROVED', 'VW_MIXTURE_REGNUMBER', 'VW_MIXTURE');
end;
/