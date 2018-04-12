PROMPT Starting Alter_INV_VW_REG_BATCHES_Standard.sql

drop public synonym INV_VW_REG_BATCHES;
commit;

Connect &&InstallUser/&&sysPass@&&serverName

grant all on &&regSchemaName..MLOG$_MIXTURES to &&schemaName.;
grant all on &&regSchemaName..MLOG$_REG_NUMBERS to &&schemaName.;
grant all on &&regSchemaName..MLOG$_BATCHES to &&schemaName.;


DROP VIEW &&schemaName..INV_VW_REG_BATCHES;

create index &&regSchemaName..mixcomp_mixid_idx on &&regSchemaName..mixture_component(MIXTRUREID);

DROP MATERIALIZED VIEW &&schemaName..INV_VW_REG_BATCHES ;

CREATE MATERIALIZED VIEW &&schemaName..INV_VW_REG_BATCHES 
BUILD IMMEDIATE
REFRESH FAST ON COMMIT
WITH ROWID
AS
	SELECT
       rn.rowid as rnRowid,  -- Required for FASTREFRESH
       rn.reg_number as RegNumber,
       rn.reg_id as RegID,
       rn.sequence_number as RegSequence,
       b.rowid as bRowid,  -- Required for FASTREFRESH
       b.FULLREGNUMBER AS RegBatchID,
       b.batch_Number AS BatchNumber,
       cast(&&schemaName..getregcomponentidentifier(rn.reg_id, 'CAS Number') as VARCHAR(2000)) as RegCas,
       ' ' as RegPage,
       b.PURITY as RegPurity,
       b.NOTEBOOK_TEXT as RegNoteBook,
       ' ' as RegNoteBookId,
       cast(&&schemaName..getUserIDFromPersonID(b.batch_reg_person_id) as VARCHAR(2000)) as RegScientist,
       b.amount as RegAmount,
       b.amount_units as RegAmountUnits,    --previously this was a select statement looking up itself...seemed weird
       cast(&&schemaName..getregcomponentidentifier(rn.reg_id, 'Chemical Name') as VARCHAR(2000)) as RegName,
       ' ' as mol_id,
       ' ' as sequence_internal_id,
       ' ' as compound_type,
       ' ' as root_number,
       ' ' as mf_text,
       ' ' as mw_text,
       ' ' as root_reg_date,
       ' ' as structure_comments_txt,
       ' ' as product_type,
       ' ' as chiral,
       ' ' as clogp,
       ' ' as h_bond_donors,
       ' ' as h_bond_acceptors,
       m.rowid as mRowid,  -- Required for FASTREFRESH
       cast(&&schemaName..GetSaltName(rn.reg_id, b.batch_Number) as VARCHAR(2000)) as RegSaltName,
       cast(&&schemaName..GetSolvateName(rn.reg_id, b.batch_Number) as VARCHAR(2000)) as RegSolvateName,
       cast(&&schemaName..getRegAltID(rn.reg_id) as VARCHAR(1000)) as RegAltId --I wonder why we are returning an id instead of the actual value, but keep behavior
FROM   &&regSchemaName..batches b,
       &&regSchemaName..reg_numbers rn,
       &&regSchemaName..MIXTURES m
WHERE b.reg_internal_id = rn.reg_id
   and rn.reg_id = m.REGID;

COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGNUMBER IS 'Reg Number::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGID IS 'Root Number::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGCAS IS 'CAS::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGNAME IS 'Reagent Name::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.BATCHNUMBER IS 'Batch Number::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGBATCHID IS 'Reg Batch ID::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGPAGE IS 'Notebook Page::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGPURITY IS 'Purity::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGSCIENTIST IS 'Scientist::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGAMOUNT IS 'Amount::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGNOTEBOOK IS 'Notebook::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.RegSaltName IS 'Salt Name::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.RegSolvateName IS 'Solvate Name::on';
COMMENT ON COLUMN &&schemaName..INV_VW_REG_BATCHES.REGSEQUENCE IS 'Reg Sequence::off'; 

CREATE INDEX &&schemaName..VWREGBAT_REGNUMBER_IDX ON &&schemaName..INV_VW_REG_BATCHES(REGNUMBER ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_REGID_IDX ON &&schemaName..INV_VW_REG_BATCHES(REGID ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_REGBATCHID_IDX ON &&schemaName..INV_VW_REG_BATCHES(REGBATCHID ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_REGNOTEBOOK_IDX ON &&schemaName..INV_VW_REG_BATCHES(REGNOTEBOOK ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_REGSEQUENCE_IDX ON &&schemaName..INV_VW_REG_BATCHES(REGSEQUENCE ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_REGNUMBER_FIDX ON &&schemaName..INV_VW_REG_BATCHES(lower(REGNUMBER) ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_REGBATCHID_FIDX ON &&schemaName..INV_VW_REG_BATCHES(lower(REGBATCHID) ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_REGNOTEBOOK_FIDX ON &&schemaName..INV_VW_REG_BATCHES(lower(REGNOTEBOOK) ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_RNRID ON &&schemaName..INV_VW_REG_BATCHES(rnRowid ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_BRID ON &&schemaName..INV_VW_REG_BATCHES(bRowid ASC) TABLESPACE &&indexTableSpaceName;
CREATE INDEX &&schemaName..VWREGBAT_MRID ON &&schemaName..INV_VW_REG_BATCHES(mRowid ASC) TABLESPACE &&indexTableSpaceName;

GRANT SELECT ON &&schemaName..INV_VW_REG_BATCHES TO &&securitySchemaName WITH GRANT OPTION;

Connect &&InstallUser/&&sysPass@&&serverName
create public synonym INV_VW_REG_BATCHES FOR &&schemaName..INV_VW_REG_BATCHES;
