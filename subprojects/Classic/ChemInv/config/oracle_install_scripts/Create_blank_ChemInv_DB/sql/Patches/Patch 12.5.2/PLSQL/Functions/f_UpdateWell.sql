CREATE OR REPLACE FUNCTION "UPDATEWELL"
  (pWellIDs varchar2,
   pPlateIDs varchar2,
   pValuePairs varchar2)
RETURN varchar2
IS
--pWellIDs: a ',' delimited list of Well_IDs
--pPlateIDs: a ',' delimited list of Plate_IDs
--pValues: name/value pairs for fields to be updated, :: delimited
--must have a value for EITHER pWellIDs OR pPlateIDs
ValuePairs_t STRINGUTILS.t_char;
vWellIDs_t STRINGUTILS.t_char;
v_ColumnName varchar2(100);
v_Start int;
v_EqualsPosition int;
v_CommaPosition int;
v_Count int;
v_ValuePairs varchar2(1000);
invalid_column exception;
vCompoundID VARCHAR2(25);
vIsReg BOOLEAN := FALSE;
vRegID VARCHAR2(25);
vBatchNumber VARCHAR2(25);
l_compound_id Inv_compounds.compound_id%Type;

BEGIN
  --return length(pWellIDs) || ',' || length(pPlateIDs);

	v_ValuePairs := pValuePairs;
  --validate the columns
	v_Start := 2;
	ValuePairs_t := STRINGUTILS.split(pValuePairs,'::');
  vWellIDs_t := STRINGUTILS.split(pWellIDS, ',');
	FOR i in ValuePairs_t.First..ValuePairs_t.Last
	Loop
		--get column name
		v_EqualsPosition := INSTR(ValuePairs_t(i),'=',v_Start,1);
		v_CommaPosition := INSTR(ValuePairs_t(i),'::',v_Start,1);
		v_ColumnName := TRIM(SUBSTR(ValuePairs_t(i), v_Start, (v_EqualsPosition-2)));
		v_Start := v_CommaPosition + 2;
		--v_ColumnValue
		IF upper(v_ColumnName) = 'COMPOUND_ID_FK' THEN
			vCompoundID := TRIM(SUBSTR(ValuePairs_t(i), (v_EqualsPosition+1)));
      v_ValuePairs := replace(v_ValuePairs,'::' || v_ColumnName || '=' || vCompoundID, ' ');
      EXECUTE IMMEDIATE 'DELETE inv_well_compounds WHERE well_id_fk IN ('|| pWellIDs ||')'  ;
      FOR j IN vWellIDs_t.FIRST..vWellIDs_t.LAST
      LOOP
      	IF NOT vCompoundID='NULL' THEN
      		INSERT INTO inv_well_compounds (well_id_fk, compound_id_fk) VALUES (vWellIDs_t(j), vCompoundID);
      	END IF;
      END LOOP;
    ELSIF upper(v_ColumnName) = 'REG_ID_FK' THEN
    	vIsReg := TRUE;
      vRegID := TRIM(SUBSTR(ValuePairs_t(i), (v_EqualsPosition+1)));
      v_ValuePairs := replace(v_ValuePairs,'::' || v_ColumnName || '=' || vRegID, ' ');
    ELSIF upper(v_ColumnName) = 'BATCH_NUMBER_FK' THEN
    	vIsReg := TRUE;
			vBatchNumber := TRIM(SUBSTR(ValuePairs_t(i), (v_EqualsPosition+1)));
      v_ValuePairs := replace(v_ValuePairs,'::' || v_ColumnName || '=' || vBatchNumber, ' ');
		ELSE
  		--validate column name
  		SELECT count(*) INTO v_Count FROM user_tab_columns WHERE table_name = 'INV_WELLS' AND column_name = upper(v_ColumnName);
  		IF v_Count = 0 THEN
  			 RAISE invalid_column;
  		END IF;
		END IF;
    End loop;
		-- add reg_id to inv_well_compounds if nec.
    IF vIsReg AND vCompoundID='NULL' THEN
      EXECUTE IMMEDIATE 'DELETE inv_well_compounds WHERE well_id_fk IN ('|| pWellIDs ||')'  ;
      FOR j IN vWellIDs_t.FIRST..vWellIDs_t.LAST
      LOOP
      IF NOT (vRegID='NULL' and vBatchNumber='NULL') THEN
		  l_compound_id := CREATEREGCOMPOUND( vRegID, vBatchNumber );
		  INSERT INTO inv_well_compounds (well_id_fk,compound_id_fk) VALUES (vWellIDs_t(j),l_compound_id);
      END IF;
    END LOOP;
    END IF;

    v_ValuePairs := replace(v_ValuePairs,'::',',');

    IF length(pWellIDs) > 0 THEN
	    --RETURN 'UPDATE inv_Wells SET ' || v_ValuePairs || ' WHERE Well_ID in (' || pWellIDs || ')';
	    EXECUTE IMMEDIATE 'UPDATE inv_Wells SET ' || v_ValuePairs || ' WHERE Well_ID in ('|| pWellIDs ||')';
    END IF;
    IF length(pPlateIDs) > 0 THEN
	    --RETURN 'UPDATE inv_Wells SET ' || v_ValuePairs || ' WHERE Plate_ID_FK in (' || pPlateIDs || ')';
	    EXECUTE IMMEDIATE 'UPDATE inv_Wells SET ' || v_ValuePairs || ' WHERE Plate_ID_FK in ('|| pPlateIDs ||') and well_format_id_fk <> (SELECT ENUM_ID FROM INV_ENUMERATION WHERE UPPER(ENUM_VALUE) = ''EMPTY'')';
    END IF;
    RETURN '0';

exception
WHEN invalid_column then
	RETURN '-130';
END UPDATEWELL;
/
show errors;


