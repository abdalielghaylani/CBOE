CREATE OR REPLACE VIEW INV_VW_REG_STRUCTURES AS
Select 
       base64_cdx AS BASE64_CDX
       , reg_id AS REGID
       , '' as LASTUPDATEDDATE
FROM structures, reg_numbers
WHERE structures.cpd_internal_id = reg_numbers.cpd_internal_id;


COMMENT ON COLUMN INV_VW_REG_STRUCTURES.BASE64_CDX IS 'Reg Structure::on';
COMMENT ON COLUMN INV_VW_REG_STRUCTURES.REGID IS 'Reg ID ::off';
COMMENT ON COLUMN INV_VW_REG_STRUCTURES.LASTUPDATEDDATE IS 'Last Updated Date::off'; 


CREATE OR REPLACE VIEW INV_VW_REG_BATCHES AS
SELECT 
       reg_numbers.reg_number AS RegNumber
       , batches.batch_Number AS BatchNumber
       , reg_numbers.reg_id as RegID
       , ( SELECT identifier 
           FROM alt_ids 
           WHERE reg_numbers.Reg_id = alt_ids.Reg_internal_id 
           AND alt_ids.identifier_type = 1
           ) AS RegCas
       , reg_numbers.reg_number||'-'||batches.batch_Number AS RegBatchID
       , batches.notebook_page AS RegPage
       , batches.purity AS RegPurity
       , batches.notebook_text AS RegNoteBook
       , cs_security.people.user_id AS RegScientist
       , batches.amount||' '||batches.amount_units AS RegAmount
       , batches.amount_units AS RegAmountUnits
       , ( SELECT alt_ids.identifier 
           FROM alt_ids 
           WHERE alt_ids.identifier_type = 0 
           AND reg_internal_id = reg_numbers.reg_id 
           AND ROWNUM = 1
           ) AS RegName
       , NULL as LastUpdatedDate
       , 'EPOR AB' as RegProject
       , 'ABBOTT LABORATORIES' as RegSource
       , 'M-040524.12.6wg2.3-15' as Cell_Line
       , 'Ling Santora' as Approver1
       , 'Limary Pereira' as Approver2
       , 'pA595' as DNA_SEQ_REF
       , 'EpoR' as Antigen
       , 'human' as Species
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
FROM reg_numbers
     , batches 
     , cs_security.people
     , regdb.compound_molecule cm
WHERE reg_numbers.reg_id = batches.reg_internal_id
AND batches.scientist_id = cs_security.people.person_id(+)
And reg_numbers.reg_id = cm.cpd_database_counter;

COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNUMBER IS 'A-Code::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGID IS 'Root Number::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGCAS IS 'CAS::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNAME IS 'Reagent Name::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.BATCHNUMBER IS 'Lot Number::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGBATCHID IS 'Batch ID::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGPAGE IS 'Notebook Page::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGPURITY IS 'Purity::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGSCIENTIST IS 'Scientist::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGAMOUNT IS 'Amount::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGNOTEBOOK IS 'Notebook::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGSOURCE IS 'Source::on'; 
COMMENT ON COLUMN INV_VW_REG_BATCHES.REGPROJECT IS 'Reg Project::on'; 

COMMENT ON COLUMN INV_VW_REG_BATCHES.DNA_SEQ_REF IS 'DNA Sequence Reference::on';
COMMENT ON COLUMN INV_VW_REG_BATCHES.APPROVER1 IS 'Dispensing Approver::on'; 
COMMENT ON COLUMN INV_VW_REG_BATCHES.APPROVER2 IS 'Backup Approver::on'; 
COMMENT ON COLUMN INV_VW_REG_BATCHES.CELL_LINE IS 'Cell_Line::on'; 
COMMENT ON COLUMN INV_VW_REG_BATCHES.DNA_SEQ_REF IS 'DNA_SEQ_REF::on'; 
COMMENT ON COLUMN INV_VW_REG_BATCHES.ANTIGEN IS 'Antigen::on'; 
COMMENT ON COLUMN INV_VW_REG_BATCHES.SPECIES IS 'Species::on'; 
COMMENT ON COLUMN INV_VW_REG_BATCHES.LASTUPDATEDDATE IS 'Last Update Date::on'; 


CREATE OR REPLACE VIEW INV_VW_REG_ALTIDS AS
SELECT 
       reg_internal_id AS regid
       , identifier
FROM Alt_ids
Where identifier_type=2;

