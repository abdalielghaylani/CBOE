CREATE OR REPLACE FUNCTION "&&SchemaName"."UPDATEPLATEATTRIBUTES"
  (pPlateIDs varchar2,
   pValuePairs varchar2)
RETURN varchar2
IS
--pPlateIDs: a :: delimited list of Plate_IDs
--pValues: name/value pairs for fields to be updated, :: delimited

ValuePairs_t STRINGUTILS.t_char;
v_ColumnName varchar2(100);
v_Start int;
v_EqualsPosition int;
v_CommaPosition int;
v_Count int;
v_ValuePairs varchar2(1000);
invalid_column exception;   
v_Barcode varchar2(1000);
v_BarcodeDescIDText varchar2(100);
BEGIN

	v_ValuePairs := pValuePairs;
	--validate the columns
	v_Start := 2;
	ValuePairs_t := STRINGUTILS.split(pValuePairs,'::');
	FOR i in ValuePairs_t.First..ValuePairs_t.Last
	Loop
		--get column name
		v_EqualsPosition := INSTR(ValuePairs_t(i),'=',v_Start,1);
		v_CommaPosition := INSTR(ValuePairs_t(i),'::',v_Start,1);
		v_ColumnName := TRIM(SUBSTR(ValuePairs_t(i), v_Start, (v_EqualsPosition-2)));
		v_Start := v_CommaPosition + 2;               
		--v_ColumnValue
		IF upper(v_ColumnName) = 'BARCODE_DESC_ID' THEN
			v_BarcodeDescIDText := TRIM(SUBSTR(ValuePairs_t(i), (v_EqualsPosition+1)));
			v_Barcode := barcodes.GetNextBarcode(v_BarcodeDescIDText);
			v_ValuePairs := replace(v_ValuePairs,'barcode_desc_id=' || v_BarcodeDescIDText , 'plate_barcode=''' || v_Barcode || '''');
		ELSE
			--validate column name
			SELECT count(*) INTO v_Count FROM user_tab_columns WHERE table_name = 'INV_PLATES' AND column_name = upper(v_ColumnName);
			IF v_Count = 0 THEN
				 RAISE invalid_column;
			END IF;
		END IF;
    End loop;

    v_ValuePairs := replace(v_ValuePairs,'::',',');  
    EXECUTE IMMEDIATE 'UPDATE inv_Plates SET ' || v_ValuePairs || ' WHERE Plate_ID in (' || pPlateIDs || ')';
    RETURN 0;

exception
WHEN invalid_column then
	--'Invalid column specified.';
	RETURN -130;
END UPDATEPLATEATTRIBUTES;
/
show errors;

