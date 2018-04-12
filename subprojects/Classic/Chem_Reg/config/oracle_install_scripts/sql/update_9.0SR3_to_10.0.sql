--CARA
alter table temporary_structures add (base64_cdx_original clob null);
alter table temporary_structures add (status varchar(200) null);
alter table temporary_structures add (log clob null);
alter table temporary_structures add (fragments clob null);



--insert into compound_type (description, active) values ('in-house', 1);
--insert into compound_type (description, active) values ('acquired', 1);

alter table fragments add (type varchar(50) null);

alter table salts add (base64_cdx clob null);
alter table solvates add (base64_cdx clob null);

alter table temporary_structures add (stripping_succeeded int default 0 null);
alter table temporary_structures add (stripping_failed int default 0 null); 
alter table temporary_structures add (salt_MF varchar(200) null);
alter table temporary_structures add (solvate_MF varchar(200) null);

commit;




CREATE OR REPLACE TRIGGER populateMFMW_trig
    AFTER update of active ON salts
declare
	saltID integer;
	formula varchar(200);
	molweight number(8, 0);
BEGIN
  	--select :updatedRow.salt_code into saltID from salts ;
	for x in (select * from salts) loop

		select salts.salt_code into saltID from salts where salts.salt_code = x.salt_code;

		select CsCartridge.FORMULA(salts.base64_cdx, '') into formula from salts where salt_code = saltID;
		update salts set salt_mf = formula where salt_code = saltID;
		----:NEW.SALT_MF := formula;

		select CsCartridge.MolWeight(salts.base64_cdx) into molweight from salts where salt_code = saltID;
		update salts set SALT_MW = molweight where salt_code = saltID;
		----:NEW.SALT_MW := molweight;

	end loop;
END;
/

alter trigger populateMFMW_trig compile;
CREATE OR REPLACE TRIGGER populateSolvateMFMW_trig
    AFTER update of active ON solvates
    --for each row
declare
	solvateID integer;
	formula varchar(200);
	molweight number(8, 0);
BEGIN
	for x in (select * from solvates) loop

		select solvates.solvate_id into solvateID from solvates where solvates.solvate_id = x.solvate_id;

		select CsCartridge.FORMULA(solvates.base64_cdx, '') into formula from solvates where solvate_id = solvateID;
		update solvates set solvate_mf = formula where solvate_id = solvateID;

		select CsCartridge.MolWeight(solvates.base64_cdx) into molweight from solvates where solvate_id = solvateID;
		update solvates set solvate_mw = molweight where solvate_id = solvateID;

	end loop;

END;
/

alter trigger populateSolvateMFMW_trig compile;

--DGB's changes
set escape \

declare
	BatchFormulaDelimeter varchar2(10):=' ';
	DefaultCode varchar2(2):='1';
begin

update batches t set t.batch_formula = (select 
       /*b.solvate_id, b.solvate_name,solvate_equivalents,b.salt_internal_id,b.salt_equivalents,salt_name,*/
       c.formula2
       ||
       case when ((b.salt_internal_id is null) OR (b.salt_internal_id = DefaultCode )) then ''
           else 
                case  when (b.salt_equivalents = 0) then ''
		      when (b.salt_equivalents <>1) then BatchFormulaDelimeter ||b.salt_equivalents||'(' 
                      else BatchFormulaDelimeter 
                      end
                      ||
                      (select salt_mf from salts where salt_code= b.salt_internal_id)||
                      case  when (b.salt_equivalents <>1) 
                            then ')' 
                            else '' 
                            end
       end   
       ||                     
       case when ((b.solvate_id is null) OR (b.solvate_id = DefaultCode) ) then ''
            else 
                case when (b.solvate_equivalents = 0) then '' 
		     when (b.solvate_equivalents <>1) then BatchFormulaDelimeter ||b.solvate_equivalents||'(' 
                      else BatchFormulaDelimeter 
                      end
                      ||
                      (select solvate_mf from solvates where solvate_id= b.solvate_id)||
                      case  when (b.solvate_equivalents <>1) 
                            then ')' 
                            else '' 
                            end            
       end  
       as BatchFormula            
from batches b, compound_molecule c
where c.cpd_database_counter = b.reg_internal_id
and b.batch_internal_id = t.batch_internal_id);

commit;
end;
/
set escape off
--End of DGB changes

--SYAN added on 4/25/2007 to fix CSBR-88001
alter trigger TRG_SALTS compile;
alter trigger TRG_SOLVATES compile;
ALTER TRIGGER TRG_FRAGMENT_ID COMPILE;
alter package cschemreg compile;
--End of SYAN modification

prompt 'Creating additional chemical structure index ...'

DECLARE
	PROCEDURE createCartridgeIndex(iName IN varchar2, tName IN varchar2, fName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from user_indexes where Upper(index_name) = iName AND Upper(table_owner)= '&&schemaName';
			if n = 1 then
				execute immediate 'DROP INDEX '||iName||' force';
			end if;
			--execute immediate 'CREATE INDEX '||iName|| ' ON ' || tName || '('||fName||')
			--					indexType is cscartridge.moleculeindextype
			--					PARAMETERS(''SKIP_POPULATING=YES,TABLESPACE=&&cscartTableSpaceName,FULLEXACT=INDEX'')';

			execute immediate 'CREATE INDEX '||iName|| ' ON ' || tName || '('||fName||')
								indexType is cscartridge.moleculeindextype
								PARAMETERS(''TABLESPACE=&&cscartTableSpaceName,FULLEXACT=INDEX'')';
		END createCartridgeIndex;
BEGIN
	--createCartridgeIndex('MX', 'structures', 'base64_cdx');
	--createCartridgeIndex('MX2', 'temporary_structures', 'base64_cdx');
	createCartridgeIndex('MX3', 'fragments', 'base64_cdx');
	createCartridgeIndex('MX4', 'salts', 'base64_cdx');
	createCartridgeIndex('MX5', 'solvates', 'base64_cdx');
	
END;
/

-- TSM added on 5/21/2008 to fix issue with RLS enabled and integration with Reg.
-- Use RegRLSScript to detect when RLS is enabled.  Then (and only then) do we
-- want to update the RLL function.
DEFINE RegRLSScript = PeopleProject_RLL_Function
col rls_col for a30 new_value RegRLSScript

@@DetectRegRLS.sql;
-- This next line will run either PeopleProject_RLL_Function.sql or Blank.sql, depending on the results
-- of the query just executed in DetectRegRLS.sql
@@&RegRLSScript..sql;

-- End of TSM

UPDATE GLOBALS SET VALUE = '10.0' WHERE ID = 'VERSION_SCHEMA';
UPDATE GLOBALS SET VALUE = '10.0' WHERE ID = 'VERSION_APP';
