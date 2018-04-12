CREATE OR REPLACE PACKAGE ValidateAndRepairSchema AS
  FUNCTION FindNoLongerExistTables RETURN VARCHAR2;
  FUNCTION FindNoLongerExistTableColumns RETURN VARCHAR2;
  PROCEDURE InsertMissingTableColumns;
  PROCEDURE UpdateOutOfSyncTableColumns;
  FUNCTION FindDifferingDatatypeTableCols RETURN VARCHAR2;
END;
/