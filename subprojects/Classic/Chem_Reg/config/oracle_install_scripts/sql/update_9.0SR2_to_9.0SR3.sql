

-- Add the temp id to the batches table
alter table batches add (temp_id number(8));

-- Create CSChemReg Package
@sql\pkg_cschemreg_def.sql
@sql\pkg_cschemreg_body.sql


-- Grant exectute on CSChemReg Package to appropriate roles
grant execute on cschemreg to supervising_chemical_admin;
grant execute on cschemreg to chemical_administrator;
grant execute on cschemreg to supervising_scientist; 
grant execute on cschemreg to submitter;

CREATE TABLE  FRAGMENTS (
	FRAGMENT_ID NUMBER(8,0) not null,
	BASE64_CDX CLOB null,	
	DATETIME_STAMP VARCHAR2(30) null,
	constraint PK_FRAGMENT_ID 
		primary key (FRAGMENT_ID) USING INDEX TABLESPACE &&indexTableSpaceName
	)
	LOB (BASE64_CDX) STORE AS(
		DISABLE STORAGE IN ROW NOCACHE PCTVERSION 10
		TABLESPACE &&lobsTableSpaceName
		STORAGE (INITIAL &&lobB64cdx NEXT &&lobB64cdx)
	)		
;

--DROP SEQUENCE SEQ_FRAGMENT_IDS;
CREATE SEQUENCE SEQ_FRAGMENT_ID INCREMENT BY 1 START WITH 1;  

create or replace trigger TRG_FRAGMENT_ID
BEFORE INSERT ON FRAGMENTS
FOR EACH ROW
BEGIN
SELECT SEQ_FRAGMENT_ID.NEXTVAL INTO :NEW.FRAGMENT_ID  FROM DUAL;
END;
/


ALTER TABLE SALTS ADD(FRAGMENT_ID NUMBER(8));
ALTER TABLE ALT_IDS MODIFY(IDENTIFIER VARCHAR2(2000));

--SYAN added on 11/27/2006 to fix CSBR-71619
DECLARE
	CURSOR batches_cur IS
		SELECT batch_internal_id, salt_name
	 	FROM batches
	    ORDER BY batch_internal_id;
BEGIN
	FOR batches_rec IN batches_cur LOOP
		--dbms_output.put_line('update batches set salt_internal_id = (select salt_code from salts where salt_name = ''' ||batches_rec.salt_name || ''') where batch_internal_id = ' || batches_rec.batch_internal_id);
 		execute immediate 'update batches set salt_internal_id = (select salt_code from salts where salt_name = ''' ||batches_rec.salt_name || ''') where batch_internal_id = ' || batches_rec.batch_internal_id;
	END LOOP;
END;
/
--End of SYAN modification

Connect &&schemaName/&&schemaPass@&&serverName;

GRANT SELECT ON FRAGMENTS TO BROWSER;
GRANT SELECT, INSERT, UPDATE ON FRAGMENTS TO SUBMITTER;
GRANT SELECT, INSERT, UPDATE ON FRAGMENTS TO PERFUME_CHEMIST;
GRANT SELECT, INSERT, UPDATE, DELETE ON FRAGMENTS TO SUPERVISING_SCIENTIST;
GRANT SELECT, INSERT, UPDATE, DELETE ON FRAGMENTS TO CHEMICAL_ADMINISTRATOR;
GRANT SELECT, INSERT, UPDATE, DELETE ON FRAGMENTS TO SUPERVISING_CHEMICAL_ADMIN;

--SYAN added on 4/25/2007 to fix CSBR-74698
alter package CSCHEMREG compile;
alter trigger TRG_ALT_IDS compile;
alter trigger TRG_BATCHES compile;
alter trigger TRG_SALTS compile;
alter trigger trg_sequence compile;
alter trigger trg_utilizations compile;
--End of SYAN modification

--SYAN added on 4/25/2007 to fix CSBR-88001
ALTER TRIGGER TRG_FRAGMENT_ID COMPILE;
--SYAN added on 4/25/2007 to fix CSBR-88001

--SYAN added on 4/30/2007 to fix CSBR-64663
create or replace procedure PopulateBatchFormula is
   	compoundFormula varchar(200);
	saltEquivalents varchar(200);
	saltName varchar(200);
	solvateEquivalents varchar(200);
	solvateName varchar(200);
	salt varchar(200);
	solvate varchar(200);
begin
	for x in (select * from batches) loop
		select compound_molecule.formula2 into compoundFormula from compound_molecule where compound_molecule.cpd_database_counter = x.reg_internal_id;

		select batches.salt_equivalents into saltEquivalents from batches where batches.batch_internal_id = x.batch_internal_id;
		select batches.salt_name into saltName from batches where batches.batch_internal_id = x.batch_internal_id;


		select batches.solvate_equivalents into solvateEquivalents from batches where batches.batch_internal_id = x.batch_internal_id;
		select batches.solvate_name into solvateName from batches where batches.batch_internal_id = x.batch_internal_id;

		if saltName is null then
			salt := 'no_salt';
		else
			if saltEquivalents = 1 or saltEquivalents = 0 then
				salt := saltName;
			else
				salt := saltEquivalents || saltName;
			end if;
		end if;

		if solvateName is null then
			solvate := '';
		else
			if solvateEquivalents = 1 or solvateEquivalents = 0 then
				solvate := solvateName;
			else
				solvate := solvateEquivalents || solvateName;
			end if;
		end if;

		update batches set batches.batch_formula = (compoundFormula || ' ' || salt || ' ' || solvate) where batches.batch_internal_id = x.batch_internal_id;
	end loop;
end PopulateBatchFormula;
/

begin
      PopulateBatchFormula;
end;
/

GRANT EXECUTE ON PopulateBatchFormula TO SUBMITTER;
GRANT EXECUTE ON PopulateBatchFormula TO PERFUME_CHEMIST;
GRANT EXECUTE ON PopulateBatchFormula TO SUPERVISING_SCIENTIST;
GRANT EXECUTE ON PopulateBatchFormula TO CHEMICAL_ADMINISTRATOR;
GRANT EXECUTE ON PopulateBatchFormula TO SUPERVISING_CHEMICAL_ADMIN;

--End of SYAN modification

