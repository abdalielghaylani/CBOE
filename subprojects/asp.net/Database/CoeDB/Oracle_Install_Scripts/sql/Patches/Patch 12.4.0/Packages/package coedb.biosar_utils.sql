CREATE OR REPLACE TYPE t_table_ids IS TABLE OF NUMBER;
/

CREATE OR REPLACE
PACKAGE       BIOSAR_UTILS as

PROCEDURE createOrUpdateDVFromBioSAR(
									 DataviewName in COEDataview.name%Type,
									 DataviewDescription in COEDataview.description%Type,
									 IsPublic in COEDataview.is_public%Type,
									 BaseTableID in INTEGER,
									 DatabaseName in COEDataView.DataBase%Type,
									 CategoryName in BIOSARDB.DB_TABLE.CATEGORY%Type,
									 IncludeTags in BIOSARDB.DB_TABLE.CATEGORY%Type);


PROCEDURE createOrUpdateMasterFromBioSAR;

END BIOSAR_UTILS;
/

