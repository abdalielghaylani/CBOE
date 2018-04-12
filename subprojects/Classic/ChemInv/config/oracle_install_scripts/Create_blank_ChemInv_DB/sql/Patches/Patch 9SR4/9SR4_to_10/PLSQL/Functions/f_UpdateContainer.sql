CREATE OR REPLACE FUNCTION "UPDATECONTAINER"(pContainerIDs VARCHAR2,
									pValuePairs VARCHAR2)
	RETURN VARCHAR2
--RETURN inv_containers.Container_ID%Type
 IS
	--pContainerIDs: a :: delimited list of Container_IDs
	--pValues: name/value pairs for fields to be updated, comma delimited

	ValuePairs_t         STRINGUTILS.t_char;
	v_ContainerIDs_t     STRINGUTILS.t_char;
	v_ColumnName         VARCHAR2(100);
	v_Start              INT;
	v_EqualsPosition     INT;
	v_CommaPosition      INT;
	v_BarcodeDescID      VARCHAR2(100) := '';
	vParentContainerIDFK VARCHAR2(100) := '';
	vParentFamily        inv_containers.family%TYPE;
	v_Count              INT;
	v_ValuePairs         VARCHAR2(1000);
	v_ValuePairsTemp     VARCHAR2(1000);
	invalid_column EXCEPTION;
	v_Barcode inv_containers.barcode%TYPE;
     l_locationIdTemp inv_locations.location_id%TYPE;
     l_locationId inv_locations.location_id%TYPE;
	l_reg_id_fk inv_compounds.reg_id_fk%TYPE;
	l_batch_number inv_compounds.batch_number_fk%TYPE;
	l_compound_id inv_containers.compound_id_fk%TYPE;

BEGIN
	v_ValuePairs := pValuePairs;
	l_reg_id_fk := null;
	l_batch_number := null;

	--validate the columns
	v_Start      := 2;
	ValuePairs_t := STRINGUTILS.split(pValuePairs, '::');
	FOR i IN ValuePairs_t.FIRST .. ValuePairs_t.LAST
	LOOP
		--get column name
		v_EqualsPosition := INSTR(ValuePairs_t(i), '=', v_Start, 1);
		v_CommaPosition  := INSTR(ValuePairs_t(i), '::', v_Start, 1);
		v_ColumnName     := TRIM(SUBSTR(ValuePairs_t(i),
								  v_Start,
								  (v_EqualsPosition - 2)));
		v_Start          := v_CommaPosition + 2;
		IF lower(v_ColumnName) = 'barcode_desc_id' THEN
			v_BarcodeDescID := TRIM(SUBSTR(ValuePairs_t(i),
									 (v_EqualsPosition + 1)));
			v_ValuePairs    := REPLACE(v_ValuePairs,
								  v_ColumnName || '=' || v_BarcodeDescID,
								  '#BARCODE#');
        	--' lookup the location id
		ELSIF lower(v_ColumnName) = 'location_id_fk' THEN
     		l_locationIdTemp := TRIM(SUBSTR(ValuePairs_t(i),
									 (v_EqualsPosition + 1)));
			v_ValuePairs    := REPLACE(v_ValuePairs,
								  v_ColumnName || '=' || l_locationIdTemp,
								  '#LOCATIONID#');
		ELSIF lower(v_ColumnName) = 'reg_id_fk' THEN
                IF TRIM(SUBSTR(ValuePairs_t(i),(v_EqualsPosition + 1)))='NULL' THEN l_reg_id_fk :=NULL;
                ELSE
     		l_reg_id_fk := TRIM(SUBSTR(ValuePairs_t(i),
		            						 (v_EqualsPosition + 1)));
               END IF;
                
			v_ValuePairs    := REPLACE(v_ValuePairs,
								  v_ColumnName || '=' || l_reg_id_fk,
								  '#RegID#');
                     
		ELSIF lower(v_ColumnName) = 'batch_number_fk' THEN
                     IF TRIM(SUBSTR(ValuePairs_t(i),(v_EqualsPosition + 1)))='NULL' THEN l_reg_id_fk :=NULL;
                    ELSE
                    l_batch_number := TRIM(SUBSTR(ValuePairs_t(i),
                                                                             (v_EqualsPosition + 1)));
                     END IF;
                     
                v_ValuePairs    := REPLACE(v_ValuePairs,
								  v_ColumnName || '=' || l_batch_number,
								  '#Batchnumber#');
		ELSE
			--validate column name
			SELECT COUNT(*)
			INTO v_Count
			FROM user_tab_columns
			WHERE table_name = 'INV_CONTAINERS'
				 AND column_name = upper(v_ColumnName);
			IF v_Count = 0 THEN
				RAISE invalid_column;
			END IF;
			--update family
			IF lower(v_ColumnName) = 'parent_container_id_fk' THEN
				vParentContainerIDFK := TRIM(SUBSTR(ValuePairs_t(i),
											 (v_EqualsPosition + 1)));
				IF length(vParentContainerIDFK) > 0 THEN
					EXECUTE IMMEDIATE 'SELECT family FROM inv_containers WHERE container_id = :ParentContainerID' 
						INTO vParentFamily
						USING vParentContainerIDFK;
					IF vParentFamily IS NOT NULL THEN
						v_ValuePairs := v_ValuePairs || '::family=' ||
									 vParentFamily;
					END IF;
				END IF;
			END IF;
		END IF;
	END LOOP;
             
	
        v_ValuePairs:= REPLACE(v_ValuePairs,
							   '#Batchnumber#',
							   'batch_number_fk=' || l_batch_number);
                                                           
         v_ValuePairs:= REPLACE(v_ValuePairs,
							   '#RegID#',
							   'reg_id_fk=' || l_reg_id_fk);
                                                           
       -- Go get the actual compound_id                                                      
	if l_reg_id_fk is not null and l_batch_number is not null then
		l_compound_id := CREATEREGCOMPOUND( l_reg_id_fk, l_batch_number );
                v_ValuePairs:= REPLACE(v_ValuePairs,
							   'compound_id_fk=NULL',
							   'compound_id_fk=' || l_compound_id);
         
	end if;

	v_ValuePairs     := REPLACE(v_ValuePairs, '::', ',');
	v_ContainerIDs_t := STRINGUTILS.split(pContainerIDs, ',');
	IF length(v_BarcodeDescID) > 0 OR length(l_locationIdTemp) > 0 THEN
		FOR i IN v_ContainerIDs_t.FIRST .. v_ContainerIDs_t.LAST
		LOOP
          	IF length(v_BarcodeDescID) > 0 THEN
				v_Barcode        := barcodes.GETNEXTBARCODE(v_BarcodeDescID);
			END IF;
               IF length(l_locationIdTemp) > 0 THEN
               	l_locationId := guiutils.GetLocationId(l_locationIdTemp, v_ContainerIDs_t(i), NULL, NULL);
               END IF;
			v_ValuePairsTemp := REPLACE(v_ValuePairs,
							   '#BARCODE#',
							   'barcode=''' || v_Barcode || '''');
			v_ValuePairsTemp := REPLACE(v_ValuePairsTemp,
							   '#LOCATIONID#',
							   'location_id_fk=' || l_locationId);

          	--RETURN 'UPDATE inv_containers SET ' || v_ValuePairs || ' WHERE container_ID in (' || pContainerIDs || ')';
			--EXECUTE IMMEDIATE 'UPDATE inv_containers SET ' || v_ValuePairs || ' WHERE container_ID in (' || pContainerIDs || ')';
			EXECUTE IMMEDIATE 'UPDATE inv_containers SET ' ||
						   v_ValuePairsTemp || ' WHERE container_ID = :ContainerID'
						   Using v_ContainerIDs_t(i);
			UpdateContainerBatches(v_ContainerIDs_t(i));
		END LOOP;
	ELSE
		EXECUTE IMMEDIATE 'UPDATE inv_containers SET ' || v_ValuePairs ||
					   ' WHERE container_ID in ('|| pContainerIDs ||')';
		-- update reservations
		FOR i IN v_ContainerIDs_t.FIRST .. v_ContainerIDs_t.LAST
		LOOP
			Reservations.ReconcileQtyAvailable(v_ContainerIDs_t(i));
			UpdateContainerBatches(v_ContainerIDs_t(i));
		END LOOP;

	END IF;
	RETURN 0;

EXCEPTION
	WHEN invalid_column THEN
		--'Invalid column specified.';
		RETURN - 130;
END UPDATECONTAINER;
/
show errors;
