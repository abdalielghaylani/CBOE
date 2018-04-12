CREATE VIEW &&schemaName..INV_VW_COMPOUNDS
AS select
&&schemaName..inv_compounds.COMPOUND_ID, 
&&schemaName..inv_compounds.MOL_ID,
&&schemaName..inv_compounds.CAS,	
&&schemaName..inv_compounds.ACX_ID, 
&&schemaName..inv_compounds.SUBSTANCE_NAME,
&&schemaName..inv_compounds.BASE64_CDX,
&&schemaName..inv_compounds.MOLECULAR_WEIGHT,
&&schemaName..inv_compounds.DENSITY, 
&&schemaName..inv_compounds.CLOGP, 
&&schemaName..inv_compounds.ROTATABLE_BONDS,
&&schemaName..inv_compounds.TOT_POL_SURF_AREA, 
&&schemaName..inv_compounds.HBOND_ACCEPTORS, 
&&schemaName..inv_compounds.HBOND_DONORS,
&&schemaName..inv_compounds.ALT_ID_1, 
&&schemaName..inv_compounds.ALT_ID_2, 
&&schemaName..inv_compounds.ALT_ID_3, 
&&schemaName..inv_compounds.ALT_ID_4, 
&&schemaName..inv_compounds.ALT_ID_5,
&&schemaName..inv_compounds.CONFLICTING_FIELDS, 
-- These fields are only needed in cases of RegIntegration, but are still here to be consistent
&&schemaName..inv_compounds.REG_ID_FK, 
&&schemaName..inv_compounds.BATCH_NUMBER_FK,
'N/A' as REG_NUMBER,
'N/A' as REG_BATCH_ID,
0 as CPD_INTERNAL_ID,   -- For Reg RLS; 0 implies the compound is Inventory-owned
null as ic_rowid,
null as rn_rowid,
null as s_rowid
from &&schemaName..INV_COMPOUNDS;