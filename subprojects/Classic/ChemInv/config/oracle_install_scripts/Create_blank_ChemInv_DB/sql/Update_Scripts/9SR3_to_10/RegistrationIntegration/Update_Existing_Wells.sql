create GLOBAL TEMPORARY TABLE temp_well_compounds
(
  reg_id_fk number(9,0),
  batch_number_fk number(9,0),
  compound_id_fk number(9,0)
)
ON COMMIT DELETE ROWS;

create index TEMP_NDX_REG on inv_well_compounds(reg_id_fk) tablespace &&indexTableSpaceName online;
create index TEMP_NDX_BATCH on inv_well_compounds(batch_number_fk) tablespace &&indexTableSpaceName online;

declare
l_compound_id inv_compounds.compound_id%TYPE;
regIdFk inv_well_compounds.reg_id_fk%TYPE;
batchNumberFk inv_well_compounds.batch_number_fk%TYPE;
Cursor TEMP_RS is 
  SELECT reg_id_fk, batch_number_fk FROM temp_well_compounds
  FOR UPDATE;

begin
  insert into temp_well_compounds(reg_id_fk, batch_number_fk, compound_id_fk)
  select reg_id_fk, batch_number_fk, null as compound_id_fk
  from inv_well_compounds
  WHERE reg_id_fk IS NOT NULL and batch_number_fk IS NOT NULL
  group by reg_id_fk, batch_number_fk;
  
  OPEN TEMP_RS;
  LOOP
    FETCH TEMP_RS INTO regIdFk, batchNumberFk;
      EXIT WHEN TEMP_RS%NOTFOUND OR TEMP_RS%NOTFOUND IS NULL;
      l_compound_id := CREATEREGCOMPOUND( regIdFk, batchNumberFk );
      update temp_well_compounds
      set compound_id_fk = l_compound_id
      where current of TEMP_RS;    
  END LOOP;
  CLOSE TEMP_RS;
  
  update inv_well_compounds iwc
  set iwc.compound_id_fk = (
    select compound_id_fk
    from temp_well_compounds twc
    where iwc.reg_id_fk = twc.reg_id_fk
    and iwc.batch_number_fk = twc.batch_number_fk
  ), reg_id_fk = null, batch_number_fk = null
  where iwc.reg_id_fk is not null
  and iwc.batch_number_fk is not null;
end;
/

drop TABLE temp_well_compounds;
drop index TEMP_NDX_REG;
drop index TEMP_NDX_BATCH;

