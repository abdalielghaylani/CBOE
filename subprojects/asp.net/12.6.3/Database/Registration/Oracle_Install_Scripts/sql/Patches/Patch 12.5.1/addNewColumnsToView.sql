
-- Add New Columns To View [VW_COMPOUND_STRUCTURE]:

begin
  -- Call the procedure
  -- @NORMALIZEDSTRUCTURE
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'C.NORMALIZEDSTRUCTURE', 'NORMALIZEDSTRUCTURE', 'VW_COMPOUND_STRUCTURE', 'VW_COMPOUND');

-- @USENORMALIZATION
  &&schemaName..configurationcompoundregistry.addfieldtoview(
    'C.USENORMALIZATION', 'USENORMALIZATION', 'VW_COMPOUND_STRUCTURE', 'VW_COMPOUND');
end;
/
