CREATE OR REPLACE VIEW INV_VW_REG_BATCHES AS
SELECT 
       NULL AS RegNumber
       , NULL AS BatchNumber
       , NULL as RegID
       , NULL  AS RegCas
       , NULL AS RegBatchID
       , NULL AS RegPage
       , NULL AS RegPurity
       , NULL AS RegNoteBook
       , NULL AS RegScientist
       , NULL AS RegAmount
       , NULL AS RegAmountUnits
       , NULL  AS RegName
       , NULL as RegSequence
       , NULL  AS mol_id
       , NULL  AS sequence_internal_id
       , NULL  AS compound_type
       , NULL  AS root_number
       , NULL  AS mf_text
       , NULL AS mw_text
       , NULL AS root_reg_date
       , NULL AS structure_comments_txt
       , NULL AS product_type
       , NULL AS chiral
       , NULL AS clogp
       , NULL AS h_bond_donors
       , NULL AS h_bond_acceptors
	   , NULL AS RegSaltName
	   , NULL AS RegSolvateName
	   , NULL AS RegAltId
FROM dual;
