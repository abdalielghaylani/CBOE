CREATE OR REPLACE FUNCTION CREATEPLATEXML(pPlateXML IN CLOB, pDecrementParents boolean := true)
RETURN varchar2
IS
vEngine xslprocessor.Processor := xslprocessor.newProcessor;
vParser xmlparser.Parser := xmlparser.newParser;

vXSLT CLOB;
vTargetClob CLOB;
vPlateXML CLOB;
vPlateXML2 CLOB;
vPlateDoc xmldom.DOMDocument;
--vPlateNL xmldom.DOMNodeList;
vBarcodePlatesNL xmldom.DOMNodeList;
vPlateWellsNL xmldom.DOMNodeList;
l_amt_nl xmldom.DOMNodeList;
vPlateNode xmldom.DOMNode;
vNode xmldom.DOMNode;
vElement xmldom.DOMElement;
vAttribute xmldom.DOMAttr;

vNumPlates number;
vNumWells number;
vNumRows number;
vPlateID inv_plates.plate_id%TYPE;
vGridPostionID inv_wells.grid_position_id_fk%TYPE;
vParentWellID inv_wells.well_id%TYPE;
vPlateIDList varchar2(200) := '';
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
l_amt NUMBER;
l_amtUnitID inv_units.unit_id%TYPE;
l_solventID inv_plates.solvent_id_fk%TYPE;
l_solventVolume inv_plates.solvent_volume%TYPE;
l_solventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE;
l_addSolvent BOOLEAN;
l_plateIDList_t stringutils.t_char;
l_decrementParents BOOLEAN;
l_index INTEGER;
l_newPlateID inv_plates.plate_id%TYPE;

l_xmlType xmltype;
BEGIN
  INSERT INTO CLOBS VALUES (pPlateXML);
  SELECT theCLOB INTO vPlateXML from CLOBS;
	
	--' default the local decrement flag to the value of the parameter
	l_decrementParents := pDecrementParents;
	
  --get number of plates to be created
	vPlateDoc := xmlutils.CLOB2DOC(vParser, vPlateXML);
	vNumPlates := xmldom.getLength(xmldom.getElementsByTagName(vPlateDoc, 'PLATE'));
	--vPlateNL := xmldom.getElementsByTagName(vPlateDoc, 'PLATE');
	--vNumPlates := xmldom.getLength(vPlateNL);

  --update plates with plate ids
	vPlateNode := xmldom.makeNode(vPlateDoc);
	FOR i in 1 .. vNumPlates
	LOOP
		select SEQ_INV_PLATES.nextval into vPlateID from dual;
		vPlateIDList := vPlateIDList || vPlateID || ',';
  	vNode := xslprocessor.selectSingleNode(vPlateNode,'/PLATES/PLATE[' || i || ']');
	  vElement := xmldom.makeElement(vNode);
		vAttribute := xmldom.createAttribute(vPlateDoc, 'PLATE_ID');
		xmldom.setValue(vAttribute, vPlateID);
		vAttribute := xmldom.setAttributeNode(vElement, vAttribute);
		--remove deprecated PARENT_PLATE_ID_FK
		IF not xmldom.IsNull(xmldom.getAttributeNode(vElement,'PARENT_PLATE_ID_FK')) THEN xmldom.removeAttribute(vElement,'PARENT_PLATE_ID_FK'); END IF;
		--update well plate_id_fk
		vPlateWellsNL := xslprocessor.selectNodes(vPlateNode,'/PLATES/PLATE[' || i || ']//WELL');
		vNumWells := xmldom.getLength(vPlateWellsNL);
		IF vNumWells > 0 THEN
			FOR i in 0 .. (vNumWells-1)
			LOOP
				vNode := xmldom.item(vPlateWellsNL, i);
	    		vElement := xmldom.makeElement(vNode);
				vAttribute := xmldom.createAttribute(vPlateDoc, 'PLATE_ID_FK');
				xmldom.setValue(vAttribute, vPlateID);
				vAttribute := xmldom.setAttributeNode(vElement, vAttribute);
			END LOOP;
		END IF;
    End loop;
    vPlateIDList := rTrim(vPlateIDList,',');

  --update plates with barcodes if barcode_desc_id specified
	vBarcodePlatesNL := xslprocessor.selectNodes(vPlateNode,'/PLATES/PLATE[string-length(@BARCODE_DESC_ID) > 0]');
	vNumPlates := xmldom.getLength(vBarcodePlatesNL);
	IF vNumPlates > 0 THEN
		FOR i in 0 .. (vNumPlates-1)
		LOOP
			vNode := xmldom.item(vBarcodePlatesNL, i);
			vElement := xmldom.makeElement(vNode);
			vBarcodeDescID := xmldom.getAttribute(vElement, 'BARCODE_DESC_ID');
			xmldom.removeAttribute(vElement,'BARCODE_DESC_ID');
			vPlateBarcode := barcodes.GetNextBarcode(vBarcodeDescID);
			vAttribute := xmldom.createAttribute(vPlateDoc, 'PLATE_BARCODE');
			xmldom.setValue(vAttribute, vPlateBarcode);
			vAttribute := xmldom.setAttributeNode(vElement, vAttribute);
		END LOOP;
	END IF;

  DBMS_LOB.CREATETEMPORARY(vPlateXML2, FALSE, DBMS_LOB.CALL);
  xmldom.writeToClob(vPlateNode, vPlateXML2);

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
	vPlateWellsNL := xslprocessor.selectNodes(vPlateNode,'//COL[count(WELL)>=1]/WELL[string-length(@PARENT_WELL_ID_FK)>0]');
	vNumWells := xmldom.getLength(vPlateWellsNL);
	IF vNumWells > 0 THEN
		FOR i in 0 .. (vNumWells-1)
		LOOP
			--insert into inv_well_parent
			vNode := xmldom.item(vPlateWellsNL, i);
			vElement := xmldom.makeElement(vNode);
			vGridPostionID := xmldom.getAttribute(vElement, 'GRID_POSITION_ID_FK');
			vPlateID := xmldom.getAttribute(vElement, 'PLATE_ID_FK');
			vParentWellID := xmldom.getAttribute(vElement, 'PARENT_WELL_ID_FK');
			--remove deprecated PARENT_WELL_ID_FK
			IF not xmldom.IsNull(xmldom.getAttributeNode(vElement,'PARENT_WELL_ID_FK')) THEN xmldom.removeAttribute(vElement,'PARENT_WELL_ID_FK'); END IF;
			SELECT well_id INTO vWellID FROM inv_wells WHERE grid_position_id_fk = vGridPostionID AND plate_id_fk = vPlateID;
      --test to if this is already a parent
      SELECT COUNT(*) INTO vCount FROM inv_well_parent WHERE child_well_id_fk = vWellID AND parent_well_id_fk = vParentWellID;
      IF vCount = 0 THEN
			INSERT INTO inv_well_parent VALUES (
				vParentWellID,
				vWellID
			);
    	END IF;
		END LOOP;

		--insert into inv_plate_parent
/*    	FOR plateID_rec IN
    		(SELECT DISTINCT parentPlate.plate_id
    			FROM inv_wells childWell, inv_well_parent wellParent, inv_wells parentWell, inv_plates parentPlate
    				WHERE childWell.plate_id_fk = vPlateID
    					AND childWell.well_id = child_well_id_fk
    					AND wellParent.parent_well_id_fk = parentWell.well_id
    					AND parentWell.plate_id_fk = parentPlate.plate_id)
		*/
 	FOR plateID_rec IN
		(SELECT DISTINCT plate_id_fk
    			FROM  inv_well_parent wellParent, inv_wells parentWell, (SELECT well_id FROM inv_wells, inv_well_parent WHERE well_id = child_well_id_fk AND plate_id_fk = vPlateID) childWells
    				WHERE 
    					wellParent.parent_well_id_fk = parentWell.well_id
							AND child_well_id_fk = childWells.well_id)
		LOOP
			INSERT INTO inv_plate_parent VALUES  (plateID_rec.plate_id_fk, vPlateID);
		END LOOP;
	END IF;

  --inserts into inv_well_compounds
	vPlateWellsNL := xslprocessor.selectNodes(vPlateNode,'//COL[count(WELL)>=1]/WELL[string-length(@REG_ID_FK)>0 or string-length(@COMPOUND_ID_FK)>0]');
	vNumWells := xmldom.getLength(vPlateWellsNL);
	IF vNumWells > 0 THEN
		FOR i in 0 .. (vNumWells-1)
		LOOP
			vNode := xmldom.item(vPlateWellsNL, i);
			vElement := xmldom.makeElement(vNode);
			vGridPostionID := xmldom.getAttribute(vElement, 'GRID_POSITION_ID_FK');
			vPlateID := xmldom.getAttribute(vElement, 'PLATE_ID_FK');
			vCompoundID := xmldom.getAttribute(vElement, 'COMPOUND_ID_FK');
      vRegID := xmldom.getAttribute(vElement, 'REG_ID_FK');
      vBatchNumber := xmldom.getAttribute(vElement, 'BATCH_NUMBER_FK');
			--remove deprecated PARENT_WELL_ID_FK
			--IF not xmldom.IsNull(xmldom.getAttributeNode(vElement,'PARENT_WELL_ID_FK')) THEN xmldom.removeAttribute(vElement,'PARENT_WELL_ID_FK'); END IF;
			--vRegID := null;
      --vBatchNumber := null;
			IF vCompoundID IS NOT NULL OR vRegID IS NOT NULL THEN
        SELECT well_id INTO vWellID FROM inv_wells WHERE grid_position_id_fk = vGridPostionID AND plate_id_fk = vPlateID;
  			INSERT INTO inv_well_compounds (well_id_fk, compound_id_fk, reg_id_fk, batch_number_fk) VALUES (
  				vWellID,
          vCompoundID,
          vRegID,
          vBatchNumber
  			);
			END IF;
		END LOOP;

	END IF;

	--' if specified, calculate the amount taken from source wells
	--vNode := xslprocessor.selectSingleNode(vPlateNode,'/PLATES/@AMT_TAKEN');
	l_amt_nl := xslprocessor.selectNodes(vPlateNode,'/PLATES[string-length(@AMT_TAKEN) > 0]');
	vCount := xmldom.getLength(l_amt_nl);
	IF vCount > 0 THEN
			l_plateIDList_t := stringutils.split(vPlateIDList, ',');
			
		 --' decrement parents using amt_taken 
		FOR i in 0 .. (vCount-1)
		LOOP
			vNode := xmldom.item(l_amt_nl, i);
			vElement := xmldom.makeElement(vNode);
			l_amt := xmldom.getAttribute(vElement, 'AMT_TAKEN');
			l_amtUnitID := xmldom.getAttribute(vElement, 'AMT_TAKEN_UNIT_ID');
			l_solventID := xmldom.getAttribute(vElement,'SOLVENT_ID');
			l_solventVolume := xmldom.getAttribute(vElement,'SOLVENT_VOLUME');
			l_solventVolumeUnitID := xmldom.getAttribute(vElement,'SOLVENT_VOLUME_UNIT_ID');
			
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
		IF l_decrementParents THEN
    		--' decrement parents by what is stored in the well
        vDecrementStatus := platechem.DecrementParentQuantities(vPlateIDList);
        --update quantity initial
        vSQL :=  'UPDATE inv_wells set qty_initial = qty_remaining WHERE plate_id_fk IN (' || vPlateIDList || ')';
        EXECUTE IMMEDIATE vSQL;
		 END IF;
	END IF;
	
 	--update the quantities to themselves so that the molar calc trigger fires.  compounds are getting updated after quantities so have to reupdate
  EXECUTE IMMEDIATE 'UPDATE inv_wells SET qty_remaining = qty_remaining WHERE plate_id_fk IN (' || vPlateIDList || ')';

  --Set the aggregate plate data
	platechem.SetAggregatedPlateData(vPlateIDList);

	-- update date_created if its not specified
  EXECUTE IMMEDIATE 'UPDATE inv_plates SET date_created = sysdate WHERE plate_id IN (' || vPlateIDList || ') AND date_created IS NULL';


	return vPlateIDList;

	dbms_lob.freetemporary(vPlateXML2);
	xmldom.freeDocument(vPlateDoc);
    xmlparser.freeParser(vParser);
	xslprocessor.freeProcessor(vEngine);

 	EXCEPTION
	  	WHEN OTHERS THEN
			RETURN 'Reformat Error: ' || SQLCODE || ':' || SQLERRM;

END;
/
show errors;