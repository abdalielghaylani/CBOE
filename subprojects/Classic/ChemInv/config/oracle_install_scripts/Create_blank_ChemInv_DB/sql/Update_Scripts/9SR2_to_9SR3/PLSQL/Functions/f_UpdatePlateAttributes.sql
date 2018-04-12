CREATE OR REPLACE FUNCTION "UPDATEPLATEATTRIBUTES"
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
v_ValuePairsTemp     VARCHAR2(1000);
invalid_column exception;
v_Barcode varchar2(1000);
v_BarcodeDescIDText varchar2(100);
l_locationIdTemp inv_locations.location_id%TYPE;
l_locationId inv_locations.location_id%TYPE;
l_plateId_t stringutils.t_char;
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
		IF lower(v_ColumnName) = 'barcode_desc_id' THEN
			v_BarcodeDescIDText := TRIM(SUBSTR(ValuePairs_t(i), (v_EqualsPosition + 1)));
			v_ValuePairs    := REPLACE(v_ValuePairs, v_ColumnName || '=' || v_BarcodeDescIDText, '#BARCODE#');
        	--' lookup the location id
		ELSIF lower(v_ColumnName) = 'location_id_fk' THEN
     		l_locationIdTemp := TRIM(SUBSTR(ValuePairs_t(i), (v_EqualsPosition + 1)));
				v_ValuePairs    := REPLACE(v_ValuePairs, v_ColumnName || '=' || l_locationIdTemp, '#LOCATIONID#');
		ELSE
			--validate column name
			SELECT count(*) INTO v_Count FROM user_tab_columns WHERE table_name = 'INV_PLATES' AND column_name = upper(v_ColumnName);
			IF v_Count = 0 THEN
				 RAISE invalid_column;
			END IF;
		END IF;
    End loop;


	v_ValuePairs     := REPLACE(v_ValuePairs, '::', ',');
	l_plateId_t := STRINGUTILS.split(pPlateIDs, ',');
	IF length(v_BarcodeDescIDText) > 0 OR length(l_locationIdTemp) > 0 THEN
		FOR i IN l_plateId_t.FIRST .. l_plateId_t.LAST
		LOOP
          	IF length(v_BarcodeDescIDText) > 0 THEN
      	 			v_Barcode        := barcodes.GETNEXTBARCODE(v_BarcodeDescIDText);
						END IF;
            IF length(l_locationIdTemp) > 0 THEN
              	l_locationId := guiutils.GetLocationId(l_locationIdTemp, NULL, l_plateId_t(i), NULL);
            END IF;
						v_ValuePairsTemp := REPLACE(v_ValuePairs, '#BARCODE#', 'plate_barcode=''' || v_Barcode || '''');
						v_ValuePairsTemp := REPLACE(v_ValuePairsTemp, '#LOCATIONID#', 'location_id_fk=' || l_locationId);

						EXECUTE IMMEDIATE 'UPDATE inv_plates SET ' || v_ValuePairsTemp || ' WHERE plate_id = ' || l_plateId_t(i);
		END LOOP;
    RETURN 0;
	ELSE

    EXECUTE IMMEDIATE 'UPDATE inv_Plates SET ' || v_ValuePairs || ' WHERE Plate_ID in (' || pPlateIDs || ')';
    RETURN 0;
	END IF;
exception
WHEN invalid_column then
	--'Invalid column specified.';
	RETURN -130;
END UPDATEPLATEATTRIBUTES;
/
show errors;

