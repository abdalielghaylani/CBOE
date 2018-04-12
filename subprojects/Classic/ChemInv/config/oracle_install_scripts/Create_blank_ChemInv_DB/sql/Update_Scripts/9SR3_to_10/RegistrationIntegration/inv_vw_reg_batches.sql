CREATE OR REPLACE VIEW INV_VW_REG_BATCHES AS
SELECT 
       reg_numbers.reg_number AS RegNumber
       , batches.batch_Number AS BatchNumber
       , reg_numbers.reg_id as RegID
       , ( SELECT identifier 
           FROM alt_ids 
           WHERE reg_numbers.Reg_id = alt_ids.Reg_internal_id 
           AND alt_ids.identifier_type = 1
		   AND ROWNUM = 1
           ) AS RegCas
       , reg_numbers.reg_number||'-'||batches.batch_Number AS RegBatchID
       , batches.notebook_page AS RegPage
       , batches.purity AS RegPurity
       , batches.notebook_text AS RegNoteBook
       , batches.notebook_internal_id AS RegNoteBookId
       , &&securitySchemaName..people.user_id AS RegScientist
       , batches.amount||' '||batches.amount_units AS RegAmount
       , batches.amount_units AS RegAmountUnits
       , ( SELECT alt_ids.identifier 
           FROM alt_ids 
           WHERE alt_ids.identifier_type = 0 
           AND reg_internal_id = reg_numbers.reg_id 
           AND ROWNUM = 1
           ) AS RegName
       , reg_numbers.sequence_number as RegSequence
       , cm.mol_id
       , cm.sequence_internal_id
       , cm.compound_type
       , cm.root_number
       , cm.mf_text
       , cm.mw_text
       , cm.root_reg_date
       , structure_comments_txt
       , cm.product_type
       , cm.chiral
       , cm.clogp
       , cm.h_bond_donors
       , cm.h_bond_acceptors
       , batches.salt_name as RegSaltName
       , batches.solvate_name as RegSolvateName
       , ( SELECT alt_ids.identifier 
           FROM alt_ids 
           WHERE alt_ids.identifier_type in (0,1,2) 
           AND reg_internal_id = reg_numbers.reg_id 
           AND ROWNUM = 1
           ) AS RegAltId
FROM reg_numbers
     , batches 
     , &&securitySchemaName..people
     , regdb.compound_molecule cm
WHERE reg_numbers.reg_id = batches.reg_internal_id
AND batches.scientist_id = &&securitySchemaName..people.person_id(+)
And reg_numbers.cpd_internal_id = cm.cpd_database_counter;

COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNUMBER IS 'Reg Number::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGID IS 'Root Number::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGCAS IS 'CAS::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNAME IS 'Reagent Name::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.BATCHNUMBER IS 'Batch Number::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGBATCHID IS 'Reg Batch ID::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGPAGE IS 'Notebook Page::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGPURITY IS 'Purity::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGSCIENTIST IS 'Scientist::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGAMOUNT IS 'Amount::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNOTEBOOK IS 'Notebook::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.RegSaltName IS 'Salt Name::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.RegSolvateName IS 'Solvate Name::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGSEQUENCE IS 'Reg Sequence::off'; 
