CREATE OR REPLACE FUNCTION "UPDATECONTAINER"
  (pContainerIDs varchar2,
   pValuePairs varchar2)
RETURN varchar2
--RETURN inv_containers.Container_ID%Type
IS
--pContainerIDs: a :: delimited list of Container_IDs
--pValues: name/value pairs for fields to be updated, comma delimited

ValuePairs_t STRINGUTILS.t_char;
v_ContainerIDs_t STRINGUTILS.t_char;
v_ColumnName varchar2(100);
v_Start int;
v_EqualsPosition int;
v_CommaPosition int;
v_BarcodeDescID varchar2(100) := '';
vParentContainerIDFK varchar2(100) := '';
vParentFamily inv_containers.family%TYPE;
v_Count int;
v_ValuePairs varchar2(1000);
v_ValuePairsTemp varchar2(1000);
invalid_column exception;
v_Barcode inv_containers.barcode%TYPE;
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
		IF lower(v_ColumnName) = 'barcode_desc_id' THEN
    	v_BarcodeDescID := TRIM(SUBSTR(ValuePairs_t(i), (v_EqualsPosition+1)));
      v_ValuePairs := replace(v_ValuePairs,v_ColumnName||'='||v_BarcodeDescID,'#BARCODE#');
    ELSE
  		--validate column name
  		SELECT count(*) INTO v_Count FROM user_tab_columns WHERE table_name = 'INV_CONTAINERS' AND column_name = upper(v_ColumnName);
  		IF v_Count = 0 THEN
  			 RAISE invalid_column;
  		END IF;
    	--update family
    	IF lower(v_ColumnName) = 'parent_container_id_fk' THEN
	    	vParentContainerIDFK := TRIM(SUBSTR(ValuePairs_t(i), (v_EqualsPosition+1)));
        IF length(vParentContainerIDFK) > 0 THEN
        	EXECUTE IMMEDIATE 'SELECT family FROM inv_containers WHERE container_id = ' || vParentContainerIDFK INTO vParentFamily;
					IF vParentFamily IS NOT NULL THEN
						v_ValuePairs := v_ValuePairs || '::family=' || vParentFamily;
          END IF;
        END IF;
      END IF;
		END IF;
  End loop;

 	v_ValuePairs := replace(v_ValuePairs,'::',',');
	v_ContainerIDs_t := STRINGUTILS.split(pContainerIDs,',');
  IF length(v_BarcodeDescID) > 0 THEN
	  FOR i IN v_ContainerIDs_t.First..v_ContainerIDs_t.Last
	  LOOP
	    v_Barcode := barcodes.GETNEXTBARCODE(v_BarcodeDescID);
			v_ValuePairsTemp := replace(v_ValuePairs,'#BARCODE#', 'barcode='''||v_Barcode||'''');
	    --RETURN 'UPDATE inv_containers SET ' || v_ValuePairs || ' WHERE container_ID in (' || pContainerIDs || ')';
	    --EXECUTE IMMEDIATE 'UPDATE inv_containers SET ' || v_ValuePairs || ' WHERE container_ID in (' || pContainerIDs || ')';
	    EXECUTE IMMEDIATE 'UPDATE inv_containers SET ' || v_ValuePairsTemp || ' WHERE container_ID = ' || v_ContainerIDs_t(i);

	  END LOOP;
  ELSE
	 EXECUTE IMMEDIATE 'UPDATE inv_containers SET ' || v_ValuePairs || ' WHERE container_ID in (' || pContainerIDs || ')';
   -- update reservations
	  FOR i IN v_ContainerIDs_t.First..v_ContainerIDs_t.Last
	  LOOP
        Reservations.ReconcileQtyAvailable(v_ContainerIDs_t(i));
	  END LOOP;   

  END IF;
    RETURN 0;

exception
WHEN invalid_column then
	--'Invalid column specified.';
    RETURN -130;
END UPDATECONTAINER;
/
show errors;
