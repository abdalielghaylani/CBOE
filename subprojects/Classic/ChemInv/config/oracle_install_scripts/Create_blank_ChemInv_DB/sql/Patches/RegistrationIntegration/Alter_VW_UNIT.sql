-- Overwrite the units view in registration with inventory units
Create or replace force view VW_UNIT AS
SELECT UNIT_ID AS ID, UNIT_ABREVIATION AS UNIT, ACTIVE, SORTORDER, UNIT_TYPE_ID_FK AS TYPEID
FROM &&schemaName..INV_UNITS;

GRANT SELECT on VW_Unit to &&schemaName with grant option;
