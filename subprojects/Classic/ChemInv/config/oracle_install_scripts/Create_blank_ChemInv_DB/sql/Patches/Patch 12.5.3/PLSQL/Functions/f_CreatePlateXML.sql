CREATE OR REPLACE FUNCTION CREATEPLATEXML(pPlateXML IN CLOB, pDecrementParents boolean := true)
RETURN varchar2
IS
vEngine DBMS_XSLPROCESSOR.Processor := DBMS_XSLPROCESSOR.newProcessor;
vParser DBMS_XMLPARSER.Parser := DBMS_XMLPARSER.newParser;

vXSLT CLOB;
vTargetClob CLOB;
vPlateXML CLOB;
vPlateXML2 CLOB;
vPlateDoc DBMS_XMLDOM.DOMDocument;
--vPlateNL xmldom.DOMNodeList;
vBarcodePlatesNL DBMS_XMLDOM.DOMNodeList;
vPlateWellsNL DBMS_XMLDOM.DOMNodeList;
l_amt_nl DBMS_XMLDOM.DOMNodeList;
vPlateNode DBMS_XMLDOM.DOMNode;
vNode DBMS_XMLDOM.DOMNode;
vElement DBMS_XMLDOM.DOMElement;
vAttribute DBMS_XMLDOM.DOMAttr;

vNumPlates number;
vNumWells number;
vNumRows number;
vPlateID inv_plates.plate_id%TYPE;
vParentPlateID inv_plates.plate_id%TYPE;
vGridPostionID inv_wells.grid_position_id_fk%TYPE;
vParentWellID inv_wells.well_id%TYPE;
vPlateIDList varchar2(16384) := '';
vBarcodeDescID inv_barcode_desc.barcode_desc_id%TYPE;
vPlateBarcode inv_plates.plate_barcode%TYPE;
vCompoundID inv_well_compounds.compound_id_fk%TYPE;
vRegID inv_well_compounds.reg_id_fk%TYPE;
vBatchNumber inv_well_compounds.batch_number_fk%TYPE;
vContext DBMS_XMLSave.ctxType;
vDecrementStatus varchar2(200);
vSQL varchar2(500);
vWellID inv_wells.well_id%TYPE;
vCount INTEGER;
vSolventDilutionVolume inv_wells.solvent_volume%TYPE;
l_amt NUMBER;
l_amtUnitID inv_units.unit_id%TYPE;
l_solventID inv_plates.solvent_id_fk%TYPE;
l_solventVolume inv_plates.solvent_volume%TYPE;
l_solventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE;
l_addSolvent BOOLEAN;
l_plateIDList_t stringutils.t_char;
l_decrementParents BOOLEAN;
l_setQTYFromXML BOOLEAN;
l_index INTEGER;
l_newPlateID inv_plates.plate_id%TYPE;
l_locationId inv_locations.location_id%TYPE;
l_compound_id Inv_compounds.compound_id%Type;
l_ParentPlateIDs varchar2(2000);
l_tempParentPlateIDs varchar2(2000);
l_ParentPlateIDList_t stringutils.t_char; 
l_HasParentList BOOLEAN;

vWellField1 inv_wells.field_1%TYPE;
vWellField2 inv_wells.field_2%TYPE;
vWellField3 inv_wells.field_3%TYPE;
vWellField4 inv_wells.field_4%TYPE;
vWellField5 inv_wells.field_5%TYPE;
vWellDate1 inv_wells.date_1%TYPE;
vWellDate2 inv_wells.date_2%TYPE;

l_xmlType xmltype;
BEGIN
  INSERT INTO CLOBS VALUES (pPlateXML);
  SELECT theCLOB INTO vPlateXML from CLOBS;

	--' default the local decrement flag to the value of the parameter
	l_decrementParents := pDecrementParents;

  --get number of plates to be created
	vPlateDoc := xmlutils.CLOB2DOC(vParser, vPlateXML);
	vNumPlates := DBMS_XMLDOM.getLength(DBMS_XMLDOM.getElementsByTagName(vPlateDoc, 'PLATE'));
	--vPlateNL := DBMS_XMLDOM.getElementsByTagName(vPlateDoc, 'PLATE');
	--vNumPlates := DBMS_XMLDOM.getLength(vPlateNL);

  --update plates with plate ids
  --update location_id
	vPlateNode := DBMS_XMLDOM.makeNode(vPlateDoc);
	FOR i in 1 .. vNumPlates
	LOOP
		select SEQ_INV_PLATES.nextval into vPlateID from dual;
		vPlateIDList := vPlateIDList || vPlateID || ',';
  	vNode := DBMS_XSLPROCESSOR.selectSingleNode(vPlateNode,'/PLATES/PLATE[' || i || ']');
	  vElement := DBMS_XMLDOM.makeElement(vNode);
		vAttribute := DBMS_XMLDOM.createAttribute(vPlateDoc, 'PLATE_ID');
		DBMS_XMLDOM.setValue(vAttribute, vPlateID);
		vAttribute := DBMS_XMLDOM.setAttributeNode(vElement, vAttribute);

		--' set the location_id_fk value
    l_locationId := DBMS_XMLDOM.getAttribute(vElement,'LOCATION_ID_FK');
		DBMS_XMLDOM.setAttribute(vElement,'LOCATION_ID_FK',guiutils.GetLocationId(l_locationId, NULL, NULL, NULL));
    --DBMS_XMLDOM.setValue(DBMS_XMLDOM.getAttributeNode(l_element, 'LOCATION_ID_FK'),	p_locationID);
	
		--remove deprecated PARENT_PLATE_ID_FK
		IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'PARENT_PLATE_ID_FK')) THEN DBMS_XMLDOM.removeAttribute(vElement,'PARENT_PLATE_ID_FK'); END IF;
		--update well plate_id_fk
		vPlateWellsNL := DBMS_XSLPROCESSOR.selectNodes(vPlateNode,'/PLATES/PLATE[' || i || ']//WELL');
		vNumWells := DBMS_XMLDOM.getLength(vPlateWellsNL);
		IF vNumWells > 0 THEN
			FOR i in 0 .. (vNumWells-1)
			LOOP
				vNode := DBMS_XMLDOM.item(vPlateWellsNL, i);
	    		vElement := DBMS_XMLDOM.makeElement(vNode);
				vAttribute := DBMS_XMLDOM.createAttribute(vPlateDoc, 'PLATE_ID_FK');
				DBMS_XMLDOM.setValue(vAttribute, vPlateID);
				vAttribute := DBMS_XMLDOM.setAttributeNode(vElement, vAttribute);
			END LOOP;
		END IF;
    End loop;
    vPlateIDList := rTrim(vPlateIDList,',');

  --update plates with barcodes if barcode_desc_id specified
	vBarcodePlatesNL := DBMS_XSLPROCESSOR.selectNodes(vPlateNode,'/PLATES/PLATE[string-length(@BARCODE_DESC_ID) > 0]');
	vNumRows  := DBMS_XMLDOM.getLength(vBarcodePlatesNL);
	IF vNumRows  > 0 THEN
		FOR i in 0 .. (vNumPlates-1)
		LOOP
			vNode := DBMS_XMLDOM.item(vBarcodePlatesNL, i);
			vElement := DBMS_XMLDOM.makeElement(vNode);
			vBarcodeDescID := DBMS_XMLDOM.getAttribute(vElement, 'BARCODE_DESC_ID');
			DBMS_XMLDOM.removeAttribute(vElement,'BARCODE_DESC_ID');
			vPlateBarcode := barcodes.GetNextBarcode(vBarcodeDescID);
			vAttribute := DBMS_XMLDOM.createAttribute(vPlateDoc, 'PLATE_BARCODE');
			DBMS_XMLDOM.setValue(vAttribute, vPlateBarcode);
			vAttribute := DBMS_XMLDOM.setAttributeNode(vElement, vAttribute);
		END LOOP;
	END IF;

  DBMS_LOB.CREATETEMPORARY(vPlateXML2, FALSE, DBMS_LOB.CALL);
  DBMS_XMLDOM.writeToClob(vPlateNode, vPlateXML2);

	--create inv_plates canonical xml document
	SELECT xslt INTO vXSLT FROM inv_xslts WHERE xslt_name = 'Create Plates';
	vTargetClob := xmlUtils.transformXML(vXSLT, vPlateDoc, vParser, vEngine);

	--create plates
	vContext := DBMS_XMLSave.newContext('inv_plates');
	DBMS_XMLSave.setDateFormat(vContext, 'MM/dd/yyyy');
	vNumRows := DBMS_XMLSave.insertXML(vContext, vTargetClob);
	DBMS_XMLSave.closeContext(vContext);
	--vNumRows := xmlgen.insertXML('inv_plates',vTargetClob);

	--create inv_wells canonical xml document
	SELECT xslt INTO vXSLT FROM inv_xslts WHERE xslt_name = 'Create Wells';
	vTargetClob := xmlUtils.transformXML(vXSLT, vPlateDoc, vParser, vEngine);

	--create wells
	vContext := DBMS_XMLSave.newContext('inv_wells');
	DBMS_XMLSave.setDateFormat(vContext, 'MM/dd/yyyy');
	vNumRows := DBMS_XMLSave.insertXML(vContext, vTargetClob);
	DBMS_XMLSave.closeContext(vContext);
	--vNumRows := xmlgen.insertXML('inv_wells',vTargetClob);

  --inserts into inv_plate_parent, inv_well_parent
  --propagate custom fields
	vPlateWellsNL := DBMS_XSLPROCESSOR.selectNodes(vPlateNode,'//COL[count(WELL)>=1]/WELL[string-length(@PARENT_WELL_ID_FK)>0]');
	vNumWells := DBMS_XMLDOM.getLength(vPlateWellsNL);
	IF vNumWells > 0 THEN
		FOR i in 0 .. (vNumWells-1)
		LOOP
			--insert into inv_well_parent
			vNode := DBMS_XMLDOM.item(vPlateWellsNL, i);
			vElement := DBMS_XMLDOM.makeElement(vNode);
			vGridPostionID := DBMS_XMLDOM.getAttribute(vElement, 'GRID_POSITION_ID_FK');
			vPlateID := DBMS_XMLDOM.getAttribute(vElement, 'PLATE_ID_FK');
			vParentWellID := DBMS_XMLDOM.getAttribute(vElement, 'PARENT_WELL_ID_FK');
			--remove deprecated PARENT_WELL_ID_FK
			IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'PARENT_WELL_ID_FK')) THEN DBMS_XMLDOM.removeAttribute(vElement,'PARENT_WELL_ID_FK'); END IF;
			SELECT well_id INTO vWellID FROM inv_wells WHERE grid_position_id_fk = vGridPostionID AND plate_id_fk = vPlateID;
      --test to if this is already a parent
      SELECT COUNT(*) INTO vCount FROM inv_well_parent WHERE child_well_id_fk = vWellID AND parent_well_id_fk = vParentWellID;
      IF vCount = 0 THEN
  			INSERT INTO inv_well_parent VALUES (
  				vParentWellID,
  				vWellID
  			);
        vWellField1 := null;
        vWellField2 := null;
        vWellField3 := null;
        vWellField4 := null;
        vWellField5 := null;
        vWellDate1 := null;
        vWellDate2 := null;

        -- If the XML has one of the FIELD_X or DATE_X fields mapped, use that value.  Otherwise, pull from the parent.
        IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'FIELD_1')) THEN vWellField1 := DBMS_XMLDOM.getAttribute(vElement, 'FIELD_1'); END IF;
        IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'FIELD_2')) THEN vWellField2 := DBMS_XMLDOM.getAttribute(vElement, 'FIELD_2'); END IF;
        IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'FIELD_3')) THEN vWellField3 := DBMS_XMLDOM.getAttribute(vElement, 'FIELD_3'); END IF;
        IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'FIELD_4')) THEN vWellField4 := DBMS_XMLDOM.getAttribute(vElement, 'FIELD_4'); END IF;
        IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'FIELD_5')) THEN vWellField5 := DBMS_XMLDOM.getAttribute(vElement, 'FIELD_5'); END IF;
        IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'DATE_1')) THEN vWellDate1 := DBMS_XMLDOM.getAttribute(vElement, 'DATE_1'); END IF;
        IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'DATE_2')) THEN vWellDate2 := DBMS_XMLDOM.getAttribute(vElement, 'DATE_2'); END IF;
        
        UPDATE inv_wells SET (Field_1, Field_2, Field_3, Field_4, Field_5, Date_1, DATE_2) = 
        ( SELECT decode(vWellField1,null,field_1,vWellField1), decode(vWellField2,null,field_2,vWellField2), decode(vWellField3,null,field_3,vWellField3), decode(vWellField4,null,field_4,vWellField4), decode(vWellField5,null,field_5,vWellField5), decode(vWellDate1,null,date_1,vWellDate1), decode(vWellDate2,null,date_2,vWellDate2) 
          FROM inv_wells 
          WHERE well_id = vParentWellID) 
        WHERE well_id = vWellID;
    	END IF;
		END LOOP;
  END IF;
	
  FOR i in 1 .. vNumPlates
	LOOP
		vNode := DBMS_XSLPROCESSOR.selectSingleNode(vPlateNode,'/PLATES/PLATE[' || i || ']');
	  vElement := DBMS_XMLDOM.makeElement(vNode);
    vPlateID := DBMS_XMLDOM.getAttribute(vElement,'PLATE_ID');

    vParentPlateID := null;
		
    -- Get the list of parent plates.  Since we're looping over all the parents, vParentPlateID will end up 
    -- being the last in the list.  We can only set one value for field_x and date_x, so we have to choose one 
    -- of the parents from which to copy... this will have to suffice for now.        
     l_tempParentPlateIDs :=  rTrim(DBMS_XMLDOM.getAttribute(vElement,'PARENT_PLATE_ID_LIST'),',');
     if  l_tempParentPlateIDs is not null then
     --if not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'PARENT_PLATE_ID_LIST')) then
       l_ParentPlateIDs := rTrim(DBMS_XMLDOM.getAttribute(vElement,'PARENT_PLATE_ID_LIST'), ',');
       l_ParentPlateIDList_t := stringutils.split(l_ParentPlateIDs,',');
       for l_index in l_ParentPlateIDList_t.FIRST .. l_ParentPlateIDList_t.LAST
       LOOP
         INSERT INTO inv_plate_parent VALUES (l_ParentPlateIDList_t(l_index), vPlateID);               
         vParentPlateID := l_ParentPlateIDList_t(l_index);
       END LOOP;
     else
       FOR plateID_rec IN
          (SELECT DISTINCT parentWell.plate_id_fk FROM  inv_well_parent wellParent, inv_wells parentWell, inv_wells parentWell2
            WHERE wellParent.parent_well_id_fk = parentWell.well_id
              AND wellParent.child_well_id_fk = parentWell2.well_id AND parentWell2.plate_id_fk = vPlateID)
       LOOP
         INSERT INTO inv_plate_parent VALUES  (plateID_rec.plate_id_fk, vPlateID);
         vParentPlateID := plateID_rec.plate_id_fk;                           
       END LOOP;
     end if;

      if vParentPlateID is not null then
        -- Only copy over the value if it's null.  This allows the user to map a value into the field if they so choose
        UPDATE inv_plates SET Field_1 = (SELECT Field_1 FROM inv_plates WHERE plate_id = vParentPlateID) 
        WHERE plate_id = vPlateID
        AND Field_1 IS NULL;
        UPDATE inv_plates SET Field_2 = (SELECT Field_2 FROM inv_plates WHERE plate_id = vParentPlateID) 
        WHERE plate_id = vPlateID
        AND Field_2 IS NULL;
        UPDATE inv_plates SET Field_3 = (SELECT Field_3 FROM inv_plates WHERE plate_id = vParentPlateID) 
        WHERE plate_id = vPlateID
        AND Field_3 IS NULL;
        UPDATE inv_plates SET Field_4 = (SELECT Field_4 FROM inv_plates WHERE plate_id = vParentPlateID) 
        WHERE plate_id = vPlateID
        AND Field_4 IS NULL;
        UPDATE inv_plates SET Field_5 = (SELECT Field_5 FROM inv_plates WHERE plate_id = vParentPlateID) 
        WHERE plate_id = vPlateID
        AND Field_5 IS NULL;
        UPDATE inv_plates SET Date_1 = (SELECT Date_1 FROM inv_plates WHERE plate_id = vParentPlateID) 
        WHERE plate_id = vPlateID
        AND Date_1 IS NULL;
        UPDATE inv_plates SET Date_2 = (SELECT Date_2 FROM inv_plates WHERE plate_id = vParentPlateID) 
        WHERE plate_id = vPlateID
        AND Date_2 IS NULL;
      end if;

  End loop;	

   vPlateWellsNL := DBMS_XSLPROCESSOR.selectNodes(vPlateNode,'//COL[count(WELL)>=1]/WELL[string-length(@QTY_INITIAL)>0]');
	vNumWells := DBMS_XMLDOM.getLength(vPlateWellsNL);
	IF vNumWells > 0 THEN
        l_setQTYFromXML:=true;
        else
        l_setQTYFromXML:=false;
        END IF;
  --inserts into inv_well_compounds
	vPlateWellsNL := DBMS_XSLPROCESSOR.selectNodes(vPlateNode,'//COL[count(WELL)>=1]/WELL[string-length(@REG_ID_FK)>0 or string-length(@COMPOUND_ID_FK)>0]');
	vNumWells := DBMS_XMLDOM.getLength(vPlateWellsNL);
	IF vNumWells > 0 THEN
		FOR i in 0 .. (vNumWells-1)
		LOOP
			vNode := DBMS_XMLDOM.item(vPlateWellsNL, i);
			vElement := DBMS_XMLDOM.makeElement(vNode);
			vGridPostionID := DBMS_XMLDOM.getAttribute(vElement, 'GRID_POSITION_ID_FK');
			vPlateID := DBMS_XMLDOM.getAttribute(vElement, 'PLATE_ID_FK');
			vCompoundID := DBMS_XMLDOM.getAttribute(vElement, 'COMPOUND_ID_FK');
			vRegID := DBMS_XMLDOM.getAttribute(vElement, 'REG_ID_FK');
			vBatchNumber := DBMS_XMLDOM.getAttribute(vElement, 'BATCH_NUMBER_FK');
			--remove deprecated PARENT_WELL_ID_FK
			--IF not DBMS_XMLDOM.IsNull(DBMS_XMLDOM.getAttributeNode(vElement,'PARENT_WELL_ID_FK')) THEN DBMS_XMLDOM.removeAttribute(vElement,'PARENT_WELL_ID_FK'); END IF;
			--vRegID := null;
			--vBatchNumber := null;
			IF vCompoundID IS NOT NULL OR vRegID IS NOT NULL THEN
				
		 	IF vRegID IS NOT NULL THEN
      				l_compound_id := CREATEREGCOMPOUND( vRegID, vBatchNumber );
      			ELSE
      				l_compound_id := vCompoundID;	
      		END IF;		
				
				
			SELECT well_id INTO vWellID FROM inv_wells WHERE grid_position_id_fk = vGridPostionID AND plate_id_fk = vPlateID;
  			
  			--' Do not enter same compound for a mix well twice 
  			EXECUTE IMMEDIATE 'select count(*)  from inv_well_compounds where well_id_fk= :vWellID and compound_id_fk= :l_compound_id ' into vcount  USING vWellID,l_compound_id;
  		    IF vcount = 0 then 
  				INSERT INTO inv_well_compounds (well_id_fk, compound_id_fk) VALUES (vWellID, l_compound_id);
  			END IF;	
			END IF;
		END LOOP;

	END IF;

	--' if specified, calculate the amount taken from source wells
	--vNode := DBMS_XSLPROCESSOR.selectSingleNode(vPlateNode,'/PLATES/@AMT_TAKEN');
	l_amt_nl := DBMS_XSLPROCESSOR.selectNodes(vPlateNode,'/PLATES[string-length(@AMT_TAKEN) > 0]');
	vCount := DBMS_XMLDOM.getLength(l_amt_nl);
	IF vCount > 0 THEN
			l_plateIDList_t := stringutils.split(vPlateIDList, ',');

		 --' decrement parents using amt_taken
		FOR i in 0 .. (vCount-1)
		LOOP
			vNode := DBMS_XMLDOM.item(l_amt_nl, i);
			vElement := DBMS_XMLDOM.makeElement(vNode);
			l_amt := DBMS_XMLDOM.getAttribute(vElement, 'AMT_TAKEN');
			l_amtUnitID := DBMS_XMLDOM.getAttribute(vElement, 'AMT_TAKEN_UNIT_ID');
			l_solventID := DBMS_XMLDOM.getAttribute(vElement,'SOLVENT_ID');
			l_solventVolume := DBMS_XMLDOM.getAttribute(vElement,'SOLVENT_VOLUME');
			l_solventVolumeUnitID := DBMS_XMLDOM.getAttribute(vElement,'SOLVENT_VOLUME_UNIT_ID');

			l_addSolvent := FALSE;
			IF length(l_solventVolume) > 0 THEN
				 l_addSolvent := TRUE;
		 END IF;

			Reformat.UpdateWellAmounts(l_plateIDList_t,  FALSE, l_addSolvent,  l_amt, l_amtUnitID, l_solventID, l_solventVolume, l_solventVolumeUnitID);
			--' set decrement parents to false
			l_decrementParents := FALSE;
		END LOOP;
		--' set parent aggregate info

  		FOR l_index IN l_plateIDList_t.FIRST .. l_plateIDList_t.LAST
  		LOOP
  			l_newPlateID := l_plateIDList_t(l_index);
  			FOR plateID_rec IN
  			(SELECT DISTINCT parent_plate_id_fk	FROM  inv_plate_parent WHERE child_plate_id_fk = l_newPlateID)
  		LOOP
  				platechem.SetAggregatedPlateData(plateID_rec.Parent_Plate_Id_Fk);
  		END LOOP;
		END LOOP;


	ELSE
       IF l_setQTYFromXML=false  THEN
			IF l_decrementParents THEN
    			--' decrement parents by what is stored in the well
				vDecrementStatus := platechem.DecrementParentQuantities(vPlateIDList);
				--update quantity initial
				vSQL :=  'UPDATE inv_wells set qty_initial = qty_remaining WHERE plate_id_fk IN ('||vPlateIDList ||')';
				EXECUTE IMMEDIATE vSQL  ;
             END IF;
		END IF;
	END IF;

	-- Take care of the solvent dilution.  This has to be done after the parent decrement, as we do not want to deduct
	-- this volume from the source well
	vPlateWellsNL := DBMS_XSLPROCESSOR.selectNodes(vPlateNode,'//COL[count(WELL)>=1]/WELL[string-length(@SOLVENT_VOLUME_DILUTION_NODBFIELD)>0]');
	vNumWells := DBMS_XMLDOM.getLength(vPlateWellsNL);
	IF vNumWells > 0 THEN
		FOR i in 0 .. (vNumWells-1)
		LOOP
			vNode := DBMS_XMLDOM.item(vPlateWellsNL, i);
			vElement := DBMS_XMLDOM.makeElement(vNode);
			vGridPostionID := DBMS_XMLDOM.getAttribute(vElement, 'GRID_POSITION_ID_FK');
			vPlateID := DBMS_XMLDOM.getAttribute(vElement, 'PLATE_ID_FK');
			vSolventDilutionVolume := DBMS_XMLDOM.getAttribute(vElement, 'SOLVENT_VOLUME_DILUTION_NODBFIELD');

			UPDATE inv_wells SET solvent_volume = solvent_volume + vSolventDilutionVolume
			WHERE grid_position_id_fk = vGridPostionID AND plate_id_fk = vPlateID;			
			
		END LOOP;
	END IF;

 	--update the quantities to themselves so that the molar calc trigger fires.  compounds are getting updated after quantities so have to reupdate
  EXECUTE IMMEDIATE 'UPDATE inv_wells SET qty_remaining = qty_remaining WHERE plate_id_fk IN ('|| vPlateIDList ||')'  ;

  --Set the aggregate plate data
	platechem.SetAggregatedPlateData(vPlateIDList);

	-- update date_created if its not specified
  EXECUTE IMMEDIATE 'UPDATE inv_plates SET date_created = trunc(sysdate) WHERE plate_id IN ('||vPlateIDList ||') AND date_created IS NULL' ;


  DBMS_XMLDOM.freeDocument(vPlateDoc);
  DBMS_XMLPARSER.freeParser(vParser);
  DBMS_XSLPROCESSOR.freeProcessor(vEngine);
  dbms_lob.freetemporary(vPlateXML2);
	return vPlateIDList;

 	EXCEPTION
	  	WHEN OTHERS THEN
			RETURN 'CreatePlateXML Error: ' || SQLCODE || ':' || SQLERRM;

END;
/
show errors;
