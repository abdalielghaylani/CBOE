CREATE OR REPLACE PACKAGE BODY "REFORMAT"
AS

FUNCTION GetPlateXML(pPlateIDs varchar2)
RETURN CLOB
IS
vParser xmlparser.Parser := xmlparser.newParser;
vPlateXML CLOB;
vWellXML CLOB;
vReturnXML CLOB;
BEGIN

	vPlateXML := xmlutils.RemoveXMLElement(vParser, DBMS_XMLQuery.getXML('Select * from inv_plates where plate_id in (' || pPlateIDs ||')'));
	vWellXML := xmlutils.RemoveXMLElement(vParser, DBMS_XMLQuery.getXML('Select w.*, row_index, col_index from inv_wells w, inv_grid_position gp where w.plate_id_fk in (' || pPlateIDs || ') and w.grid_position_id_fk = gp.grid_position_id'));
	--vWellXML := xmlutils.RemoveXMLElement(vParser, DBMS_XMLQuery.getXML('Select w.*, row_index, col_index from inv_wells w, inv_vw_well2 vw where w.plate_id_fk in (' || pPlateIDs ||') and w.plate_id_fk = vw.plate_id_fk'));
	vReturnXML := xmlutils.MERGE_XML_CLOBS(vPlateXML,vWellXML);
	Return vReturnXML;

	xmlparser.freeParser(vParser);

END GetPlateXML;

FUNCTION GetNumTargetPlates(pReformatMapID inv_xmldocs.xmldoc_id%TYPE)
RETURN number
IS
vParser xmlparser.Parser;
vMapClob inv_xmldocs.xmldoc%TYPE;
vMapDoc xmldom.DOMDocument;
vTargetPlatesNL xmldom.DOMNodeList;
vNumTargetPlates number;
BEGIN
    -- get map document
	SELECT xmldoc into vMapClob FROM inv_xmldocs WHERE xmldoc_ID = pReformatMapID;
	vParser := xmlparser.newParser;
    vMapDoc := xmlutils.CLOB2DOC(vParser, vMapClob);
    xmlparser.freeParser(vParser);

	vTargetPlatesNL := xmldom.getElementsByTagName(vMapDoc, 'TARGET_PLATE');
	vNumTargetPlates := xmldom.getLength(vTargetPlatesNL);

	xmldom.freeDocument(vMapDoc);
    xmlparser.freeParser(vParser);

	RETURN vNumTargetPlates;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN -1;

END GetNumTargetPlates;

FUNCTION GetPlateFormats(
	pReformatMapID inv_xmldocs.xmldoc_id%TYPE,
	pPlateType varchar2)
RETURN varchar2
IS

vEngine xslprocessor.Processor := xslprocessor.newProcessor;
vParser xmlparser.Parser;
vMapClob inv_xmldocs.xmldoc%TYPE;
vMapDoc xmldom.DOMDocument;
vSourcePlatesNL xmldom.DOMNodeList;
vCounter number;
vNode xmldom.DOMNode;
vSourcePlateFormats varchar2(200) ;
vElementName varchar2(50);

invalidPlateType exception;
BEGIN
    -- get map document
	SELECT xmldoc into vMapClob FROM inv_xmldocs WHERE xmldoc_ID = pReformatMapID;
	vParser := xmlparser.newParser;
    vMapDoc := xmlutils.CLOB2DOC(vParser, vMapClob);
    xmlparser.freeParser(vParser);

	if lower(pPlateType) = 'source' then
		vElementName := 'SOURCE_PLATE';
	elsif lower(pPlateType) = 'target' then
		vElementName :=	 'TARGET_PLATE';
	else
		RAISE invalidPlateType;
	end if;
	vSourcePlatesNL := xmldom.getElementsByTagName(vMapDoc, vElementName);
	FOR vCounter IN 0 .. xmldom.getLength(vSourcePlatesNL) - 1
	LOOP
    	vNode := xmldom.item(vSourcePlatesNL, vCounter);
		IF vCounter = 0 THEN
	      	vSourcePlateFormats := xslprocessor.valueOf(vNode, './@PLATE_FORMAT_ID_FK');
		ELSE
	      	vSourcePlateFormats := vSourcePlateFormats || ',' || xslprocessor.valueOf(vNode, './@PLATE_FORMAT_ID_FK');
		END IF;
    END LOOP;

	xslprocessor.freeProcessor(vEngine);
    xmlparser.freeParser(vParser);
	xmldom.freeDocument(vMapDoc);

	RETURN vSourcePlateFormats;

	EXCEPTION
		WHEN invalidPlateType THEN
			RETURN '-1';

END GetPlateFormats;


FUNCTION GetDaughterReformatMaps(pSourcePlateIDs varchar2)
RETURN varchar2
IS
vPlateIDs_t STRINGUTILS.t_char;
vPlateID inv_plates.plate_id%TYPE;
vPlateFormatID inv_plate_format.plate_format_id%TYPE;
vXMLDocID inv_xmldocs.xmldoc_id%TYPE;
vXMLDocIDList varchar2(500) := '';
vCount number;
BEGIN
	vPlateIDs_t := STRINGUTILS.split(pSourcePlateIDs, ',');
	FOR i in vPlateIDs_t.First..vPlateIDs_t.Last
	Loop
		vPlateID := vPlateIDs_t(i);
		SELECT plate_format_id_fk INTO vPlateFormatID FROM inv_plates WHERE plate_id = vPlateID;
		SELECT count(xmldoc_id) INTO vCount FROM inv_xmldocs WHERE xmldoc_type_id_fk = 1 AND lower(name) = 'daughter' || vPlateFormatID;
		if vCount > 0 then
			SELECT xmldoc_id INTO vXMLDocID FROM inv_xmldocs WHERE xmldoc_type_id_fk = 1 AND lower(name) = 'daughter' || vPlateFormatID;
		else
			vXMLDocID := -1;
		end if;
		vXMLDocIDList := vXMLDocIDList || vXMLDocID || ',';
    End loop;
	RETURN TRIM(',' FROM vXMLDocIDList);

    EXCEPTION
	   	WHEN OTHERS THEN
		   	RETURN -1;
END;

FUNCTION GetPlateBarcodes(pPlateIDs varchar2)
RETURN varchar2
IS
vPlateIDs_t STRINGUTILS.t_char;
vPlateID varchar2(10);
vPlateBarcode inv_plates.plate_barcode%TYPE;
vPlateBarcodeList varchar2(200) := '';

BEGIN
	vPlateIDs_t := STRINGUTILS.split(pPlateIDs, ',');
	FOR i in vPlateIDs_t.First..vPlateIDs_t.Last
	Loop
		vPlateId := vPlateIDs_t(i);
		SELECT plate_barcode INTO vPlateBarcode FROM inv_plates WHERE plate_id = vPlateID;
		vPlateBarcodeList := vPlateBarcodeList || vPlateBarcode || ',';
    End loop;

	RETURN TRIM(',' FROM vPlateBarcodeList);

END GetPlateBarcodes;

/*
	Purpose: Checks to see if the plate has material in it.  Material is defined here as a qty_remaining or solution volume.
*/
FUNCTION CheckPlateQuantity(
	pSourcePlateIDs VARCHAR2)
RETURN varchar2
IS
	lSourcePlateIDs_t STRINGUTILS.t_char;
	lPlateID inv_plates.plate_id%TYPE;
	--lMolarAmount inv_plates.molar_amount%TYPE;
  lQtyRemaining inv_plates.qty_remaining%TYPE;
  lSolutionVolume inv_plates.solution_volume%TYPE;
	lAmountProblemPlateIDs varchar2(100) := '';

	lNoMaterial exception;
BEGIN

  --' update plate properties before checking
	platechem.SetAggregatedPlateData(pSourcePlateIDs);

  --' check each plate
	lSourcePlateIDs_t := STRINGUTILS.split(pSourcePlateIDs, ',');
	FOR i IN lSourcePlateIDs_t.First..lSourcePlateIDs_t.Last
	Loop
		lPlateId := lSourcePlateIDs_t(i);
		SELECT qty_remaining, solution_volume INTO lQtyRemaining, lSolutionVolume FROM inv_plates WHERE plate_id = lPlateID;
		--' check qty_remaining amount 
    --' check solution volume 
		IF (lQtyRemaining is null or lQtyRemaining <= 0) AND (lSolutionVolume IS NULL OR lSolutionVolume <= 0) THEN
			lAmountProblemPlateIDs := lAmountProblemPlateIDs || lPlateID || ',';      
		END IF;
  END LOOP;

  IF length(lAmountProblemPlateIDs) > 0 THEN
  	lAmountProblemPlateIDs := TRIM(',' FROM lAmountProblemPlateIDs);
		RAISE lNoMaterial;
  END IF;

	RETURN '1';

  EXCEPTION
	  WHEN lNoMaterial THEN
    	RETURN 'These plates do not contain material: ' || GETPLATEBARCODES(lAmountProblemPlateIDs);

END CheckPlateQuantity;


FUNCTION CheckValidSourcePlates(
	pSourcePlateIDs varchar2,
	pReformatMapID inv_xmldocs.xmldoc_id%TYPE)
RETURN varchar2
IS
vSourcePlateFormats varchar2(200);
vSourcePlateIDs_t STRINGUTILS.t_char;
vSourcePlateTypes_t STRINGUTILS.t_char;
vPlateID inv_plates.plate_id%TYPE;
vSourcePlateTypeIDFK inv_plates.plate_format_id_fk%TYPE;
vTypeProblemPlateIDs varchar2(100) := '';

incorrectSourcePlateCount exception;
incorrectSourcePlateType exception;
BEGIN

	vSourcePlateFormats := GETPLATEFORMATS(pReformatMapID, 'source');
	vSourcePlateIDs_t := STRINGUTILS.split(pSourcePlateIDs, ',');
	vSourcePlateTypes_t := STRINGUTILS.split(vSourcePlateFormats, ',');
	if vSourcePlateIDs_t.count != vSourcePlateTypes_t.count then
		RAISE incorrectSourcePlateCount;
	end if;

    -- check each plate
	FOR i in vSourcePlateIDs_t.First..vSourcePlateIDs_t.Last
	Loop
		vPlateId := vSourcePlateIDs_t(i);
		SELECT plate_format_id_fk INTO vSourcePlateTypeIDFK FROM inv_plates WHERE plate_id = vPlateID;
		if vSourcePlateTypeIDFK != vSourcePlateTypes_t(i) then
			vTypeProblemPlateIDs := vTypeProblemPlateIDs || vPlateID || ',';
		end if;
	End loop;

  if length(vTypeProblemPlateIDs) > 0 then
  	vTypeProblemPlateIDs := TRIM(',' FROM vTypeProblemPlateIDs);
		RAISE incorrectSourcePlateType;
  end if;

	RETURN '1';

  EXCEPTION
	  WHEN incorrectSourcePlateCount THEN
   		RETURN 'Incorrect number of source plates.';
	  WHEN incorrectSourcePlateType THEN
  		RETURN 'Incorrect source plate types for plate ids: ' || GETPLATEBARCODES(vTypeProblemPlateIDs);

END CheckValidSourcePlates;

FUNCTION GetValidReformatMaps(pSourcePlateIDs varchar2)
RETURN varchar2
IS
CURSOR vMap_cur IS
	SELECT xmldoc_id FROM inv_xmldocs WHERE xmldoc_type_id_fk = 1;
	vIsValid varchar2 (200);
	vValidIDs varchar2 (200) := '';
	Invalid_SourcePlates exception;
BEGIN
  FOR vMap_rec IN vMap_cur
  LOOP
		vIsValid := CHECKVALIDSOURCEPLATES(pSourcePlateIDs, vMap_rec.xmldoc_id);
	 	IF vIsValid = '1' THEN
			vValidIDs := vValidIDs || vMap_rec.xmldoc_id || ',';
		--ELSE
			--RAISE Invalid_SourcePlates;
  	END IF;
 	END LOOP;


 	IF vValidIDs is null THEN
		RETURN '-1';
  ELSE
  	RETURN TRIM(',' from vValidIDs);
  END IF;

  EXCEPTION
  	WHEN Invalid_SourcePlates THEN
   		Return '-2';
   	WHEN OTHERS THEN
   		RETURN '-5';

END GetValidReformatMaps;

FUNCTION CreateTargetPlateXML(
	pEngine xslprocessor.Processor,
	pParser xmlparser.Parser,
  pMapClob inv_xmldocs.xmldoc%TYPE,
  pIsDaughter BOOLEAN,
	pSourcePlateIDList VARCHAR2,
  pBarcodeList_t STRINGUTILS.t_char,
 	pLocationID inv_plates.location_id_fk%TYPE,
	pPlateTypeID inv_plates.plate_type_id_fk%TYPE,
	pPlateFormatIDList_t STRINGUTILS.t_char)
RETURN xmldom.DOMDocument
IS
	vSourcePlateIDList_t STRINGUTILS.t_char;

	vQuery VARCHAR(500);
  vMapClob CLOB;
	vMapDoc xmldom.DOMDocument;
	vMapNode xmldom.DOMNode;

	vQueryClob CLOB;
	vMergeDoc xmldom.DOMDocument;
	vXsltClob CLOB;
	vTargetDoc xmldom.DOMDocument;
	vTargetNode xmldom.DOMNode;

  vPlateID varchar2(10);
	vNode xmldom.DOMNode;
	vElement xmldom.DOMElement;
	vAttribute xmldom.DOMAttr;

  vBarcodeDescId inv_barcode_desc.barcode_desc_id%TYPE;
	vBarcode inv_plates.plate_barcode%TYPE;
BEGIN
	--' Prep the input values
  vSourcePlateIDList_t := STRINGUTILS.split(pSourcePlateIDList,',');

	--This query gets the positions of the source plate wells and data that will be copied to the target wells
	vQuery := 'SELECT plate_id_fk, well_id as parent_well_id_fk, row_index, col_index, compound_id_fk, reg_id_fk, batch_number_fk
			FROM inv_wells w, inv_grid_position g, inv_well_compounds wc
			WHERE w.grid_position_id_fk = g.grid_position_id
      	AND well_id = well_id_fk(+)
				AND plate_id_fk IN ( ' || pSourcePlateIDList || ')
			ORDER BY plate_id_fk, row_index, col_index';
  vQueryClob := XMLUTILS.RemoveXMLElement(pParser, DBMS_XMLQuery.getXML(vQuery));

  -- update source plates with plate ids
  vMapDoc := xmlutils.CLOB2DOC(pParser, pMapClob);
	vMapNode := xmldom.makeNode(vMapDoc);
	FOR i in vSourcePlateIDList_t.First..vSourcePlateIDList_t.Last
	Loop
		vPlateId := vSourcePlateIDList_t(i);
  	vNode := xslprocessor.selectSingleNode(vMapNode,'/REFORMAT_MAP/SOURCE_PLATE[' || i || ']');
	  vElement := xmldom.makeElement(vNode);
		vAttribute := xmldom.createAttribute(vMapDoc, 'PLATE_ID_FK');
		xmldom.setValue(vAttribute, vPlateID);
		vAttribute := xmldom.setAttributeNode(vElement, vAttribute);
  End loop;
  DBMS_LOB.CREATETEMPORARY(vMapClob, FALSE, DBMS_LOB.CALL);
  xmldom.writeToClob(vMapNode, vMapClob);

	--merge the mapdoc with the querydoc
  vMergeDoc := XMLUTILS.CLOB2DOC(pParser, XMLUTILS.MERGE_XML_CLOBS(vQueryClob, vMapClob));

	--transform merged document into target plates node
  SELECT xslt INTO vXsltClob FROM inv_xslts WHERE xslt_name = 'Create Target Plates';
  vTargetDoc := XMLUTILS.CLOB2DOC(pParser, XMLUTILS.transformXML(vXsltClob, vMergeDoc, pParser, pEngine));
  vTargetNode := xmldom.MakeNode(vTargetDoc);

	--set barcode desc id or plate_barcode ,location, plate type on the target plates
	FOR i in pPlateFormatIDList_t.First..pPlateFormatIDList_t.Last
	Loop
 		vNode := xslprocessor.selectSingleNode(vTargetNode,'/PLATES/PLATE[' || i || ']');
	  vElement := xmldom.makeElement(vNode);
		IF NOT pIsDaughter THEN
			IF instr(pBarcodeList_t(i), ':auto') > 0 THEN
				vBarcodeDescID := rtrim(pBarcodeList_t(i),':auto');
				vAttribute := xmldom.createAttribute(vTargetDoc, 'BARCODE_DESC_ID');
				xmldom.setValue(vAttribute, vBarcodeDescID);
			ELSE
				vBarcode := pBarcodeList_t(i);
				vAttribute := xmldom.createAttribute(vTargetDoc, 'PLATE_BARCODE');
				xmldom.setValue(vAttribute, vBarcode);
			END IF;
			vAttribute := xmldom.setAttributeNode(vElement, vAttribute);
		END IF;
		xmldom.setValue(xmldom.getAttributeNode(vElement, 'LOCATION_ID_FK'),pLocationID);
		xmldom.setValue(xmldom.getAttributeNode(vElement, 'PLATE_TYPE_ID_FK'),pPlateTypeID);
	End loop;

  --update well formats and concentration based on the plate format
  FOR i IN pPlateFormatIDList_t.FIRST..pPlateFormatIDList_t.LAST
  LOOP
  	FOR wellFormat_rec IN (SELECT grid_position_id, well_format_id_fk, concentration, conc_unit_fk FROM inv_vw_well_format WHERE plate_format_id_fk = pPlateFormatIDList_t(i))
    LOOP
   		vNode := xslprocessor.selectSingleNode(vTargetNode,'/PLATES/PLATE[' || i || ']/ROW/COL/WELL[@GRID_POSITION_ID_FK="' || wellFormat_rec.Grid_Position_Id || '"]');
  	  vElement := xmldom.makeElement(vNode);
			vAttribute := xmldom.createAttribute(vTargetDoc, 'WELL_FORMAT_ID_FK');
			xmldom.setValue(vAttribute, wellFormat_rec.Well_Format_Id_Fk);
 			vAttribute := xmldom.setAttributeNode(vElement, vAttribute);

			vAttribute := xmldom.createAttribute(vTargetDoc, 'CONCENTRATION');
			xmldom.setValue(vAttribute, wellFormat_rec.Concentration);
 			vAttribute := xmldom.setAttributeNode(vElement, vAttribute);
			vAttribute := xmldom.createAttribute(vTargetDoc, 'CONC_UNIT_FK');
			xmldom.setValue(vAttribute, wellFormat_rec.Conc_Unit_Fk);
 			vAttribute := xmldom.setAttributeNode(vElement, vAttribute);
    END LOOP;
  END LOOP;

  RETURN vTargetDoc;

 	xmldom.freeDocument(vMapDoc);
 	xmldom.freeDocument(vMergeDoc);


END CreateTargetPlateXML;

FUNCTION CreateDaughterPlates (
	pPlateXMLDoc xmldom.DOMDocument,
	pPlateXMLNode xmldom.DOMNode,
  pBarcodeList_t STRINGUTILS.t_char
)
RETURN VARCHAR2
IS
	lNode xmldom.DOMNode;
	lElement xmldom.DOMElement;
	lAttribute xmldom.DOMAttr;
	lBarcodeDescID inv_barcode_desc.barcode_desc_id%TYPE;
	lBarcode inv_plates.plate_barcode%TYPE;
  lPlateXMLClob CLOB;
  lTempPlateID varchar2(200);
  lNewPlateIDs varchar2(200);
  lLibraryCount integer;
  lGroupCount integer;

BEGIN
	FOR i in pBarcodeList_t.First..pBarcodeList_t.Last
	LOOP
		IF instr(pBarcodeList_t(i), ':auto') > 0 THEN
			lBarcodeDescID := rtrim(pBarcodeList_t(i),':auto');
			lAttribute := xmldom.createAttribute(pPlateXMLDoc, 'BARCODE_DESC_ID');
			xmldom.setValue(lAttribute, lBarcodeDescID);
		ELSE
			lBarcode := pBarcodeList_t(i);
			lAttribute := xmldom.createAttribute(pPlateXMLDoc, 'PLATE_BARCODE');
			xmldom.setValue(lAttribute, lBarcode);
		END IF;
	 	lNode := xslprocessor.selectSingleNode(pPlateXMLNode,'/PLATES/PLATE[1]');
	  lElement := xmldom.makeElement(lNode);
		lAttribute := xmldom.setAttributeNode(lElement, lAttribute);
	  DBMS_LOB.CREATETEMPORARY(lPlateXMLClob, FALSE, DBMS_LOB.CALL);
	  xmldom.writeToClob(pPlateXMLNode, lPlateXMLClob);
		lTempPlateID := createPlateXML(lPlateXMLClob, false);
     lNewPlateIDs := lNewPlateIDs || lTempPlateID || ',';
		commit;
		dbms_lob.freetemporary(lPlateXMLClob);
		--copy source plate attributes to new plates if this is a daughtering
		--only update library,group if there is the same value for all the parents
		SELECT COUNT(DISTINCT library_id_fk), COUNT(DISTINCT group_name) INTO lLibraryCount, lGroupCount FROM inv_plates, inv_plate_parent WHERE plate_id = parent_plate_id_fk AND child_plate_id_fk = lTempPlateID;
		IF lLibraryCount = 1 THEN UPDATE inv_plates SET library_id_fk = (SELECT DISTINCT library_id_fk FROM inv_plates, inv_plate_parent WHERE plate_id = parent_plate_id_fk AND child_plate_id_fk = lTempPlateID) WHERE PLATE_ID = lTempPlateID; END IF;
		IF lGroupCount = 1 THEN UPDATE inv_plates SET group_name = (SELECT DISTINCT group_name FROM inv_plates, inv_plate_parent WHERE plate_id = parent_plate_id_fk AND child_plate_id_fk = lTempPlateID) WHERE PLATE_ID = lTempPlateID; END IF;
	END LOOP;

  RETURN TRIM(',' FROM lNewPlateIDs);
END CreateDaughterPlates;

FUNCTION CreateTargetWellData (
	pSourceWell platechem.chemdata,
  pAmt NUMBER,
  pAmtUnitID inv_units.unit_id%TYPE,
  pAmtUnitType inv_unit_types.unit_type_id%TYPE)
RETURN PLATECHEM.chemdata
IS

lTargetWell platechem.chemdata;

BEGIN
	-- create target well data
	lTargetWell := platechem.GetChemDataCopy(pSourceWell);

	IF pAmtUnitType = constants.cVolumeID THEN
		IF pSourceWell.QtyUnitTypeID = constants.cMassID THEN
	 		--solvated well, dry compound
			--vTargetWell.QtyRemaining := 0;
			--lTargetWell.SolutionVolume := 0;
			--vTargetWell.MolarAmount := 0;
	 		lTargetWell.SolutionVolume := pAmt;
  		lTargetWell.SolventVolumeUnitID := pAmtUnitID;
			lTargetWell.QtyRemaining := PlateChem.GetQtyFromSolutionVolume(lTargetWell);
		ELSIF pSourceWell.QtyUnitTypeID = constants.cVolumeID THEN
			--wet compound
			IF pSourceWell.SolutionVolume > 0 THEN
				--solvated
   			lTargetWell.QtyRemaining := 0;
   			lTargetWell.SolventVolume := 0;
   			lTargetWell.MolarAmount := 0;
	 			lTargetWell.SolutionVolume := pAmt;
  			lTargetWell.SolventVolumeUnitID := pAmtUnitID;
				lTargetWell.QtyRemaining := PlateChem.GetQtyFromSolutionVolume(lTargetWell);
        IF lTargetWell.QtyRemaining < 0 THEN lTargetWell.QtyRemaining := 0; END IF;
				--vTargetWell.SolventVolume := 0;
			ELSE
				--wet
   			lTargetWell.QtyRemaining := 0;
   			--vTargetWell.SolventVolume := 0;
   			lTargetWell.MolarAmount := 0;
	   		lTargetWell.QtyRemaining := pAmt;
  			lTargetWell.QtyRemainingUnitID := pAmtUnitID;
			END IF;
		END IF;
	ELSIF pAmtUnitType = constants.cMassID THEN
		lTargetWell.QtyRemaining := pAmt;
		lTargetWell.QtyRemainingUnitID := pAmtUnitID;
	END IF;

  RETURN lTargetWell;

END CreateTargetWellData;


PROCEDURE UpdateWellAmounts(
  pNewPlateIDList_t STRINGUTILS.t_char,
  pIsDaughter BOOLEAN,
  pAddSolvent BOOLEAN,
	pAmt NUMBER,
	pAmtUnitID inv_units.unit_id%TYPE,
	pSolventID inv_solvents.solvent_id%TYPE,
	pSolventVolume inv_plates.solvent_volume%TYPE,
	pSolventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE) 
IS

lAmtUnitType inv_unit_types.unit_type_id%TYPE;
lIndex number;
lNewPlateID inv_plates.plate_id%TYPE;
lIsCompoundWell boolean := false;
lCompoundCount integer;
lSourceWell platechem.chemdata;
lTargetWell platechem.chemdata;

lSourceQtyTaken inv_wells.qty_remaining%TYPE;
lSourceSolventTaken inv_wells.solvent_volume%TYPE;
lSourceSolutionTaken inv_wells.solution_volume%TYPE := NULL;
lSolventVolumeAdded inv_wells.solvent_volume%TYPE;
lTargetSolventIDFK inv_solvents.solvent_id%TYPE;
lChangeWellQtyReturn2 varchar2(200);

lCurrTWellID inv_wells.well_id%TYPE := 0;
lSourceAction varchar2(10);
BEGIN

  --' Determine the amount unit type
  lAmtUnitType := chemcalcs.GetUnitType(pAmtUnitID);

	FOR lIndexTemp IN pNewPlateIDList_t.FIRST..pNewPlateIDList_t.LAST
	LOOP
		lNewPlateID := pNewPlateIDList_t(lIndexTemp);
		IF pIsDaughter THEN
			lIndex := 1;
		ELSE
			lIndex := lIndexTemp;
		END IF;
		--update well amounts well by well
		FOR well_rec IN (
			SELECT source.well_id AS source_well_id, target.well_id AS target_well_id, source.qty_remaining, source.qty_unit_fk,
			 source.molar_amount, source.molar_conc, source.solvent_volume, source.solvent_volume_unit_id_fk, source.solution_volume,
       source.concentration, source.conc_unit_fk
			FROM inv_wells source, inv_wells target
			WHERE
				source.well_id IN (SELECT parent_well_id_fk FROM inv_well_parent WHERE child_well_id_fk = target.well_id)
				AND target.plate_id_fk = lNewPlateID ORDER BY target_well_id)
		LOOP
			-- determine if there are compounds in the well
      lIsCompoundWell := false;
      SELECT count(*) INTO lCompoundCount FROM inv_well_compounds WHERE well_id_fk = well_rec.source_well_id;
      IF lCompoundCount > 0 THEN lIsCompoundWell := TRUE; END IF;
			-- populate source well data
      lSourceWell.CompoundID := null;
      lSourceWell.RegID := null;
      lSourceWell.BatchNumber := null;
			lSourceWell.QtyRemaining := well_rec.qty_remaining;
			lSourceWell.QtyRemainingUnitID := well_rec.qty_unit_fk;
      lSourceWell.Concentration := well_rec.concentration;
      lSourceWell.ConcentrationUnitID := well_rec.conc_unit_fk;
			lSourceWell.MolarAmount := well_rec.molar_amount;
			lSourceWell.MolarConc := well_rec.molar_conc;
			lSourceWell.SolventVolume := well_rec.solvent_volume;
      lSourceWell.SolutionVolume := well_rec.solution_volume;
			lSourceWell.SolventVolumeUnitID := well_rec.solvent_volume_unit_id_fk;
			IF lSourceWell.QtyRemainingUnitID IS NOT NULL AND lSourceWell.QtyRemainingUnitID > 0  THEN
				lSourceWell.QtyUnitTypeID := chemcalcs.GetUnitType(lSourceWell.QtyRemainingUnitID);
			ELSE
				lSourceWell.QtyUnitTypeID := null;
			END IF;
			lSourceWell.CompoundState := platechem.GetWellCompoundState(lSourceWell.SolutionVolume, lSourceWell.QtyRemainingUnitID);
			lSourceWell.AvgMW := plateChem.GetAverageMW(well_rec.source_well_id);
      lSourceWell.AvgDensity := plateChem.GetAverageDensity(well_rec.source_well_id);

    	-- Create target well data
			lTargetWell := CreateTargetWellData(lSourceWell, pAmt, pAmtUnitID, lAmtUnitType);

    	IF lAmtUnitType = constants.cVolumeID THEN
    		IF lSourceWell.QtyUnitTypeID = constants.cMassID THEN
    			lSourceQtyTaken := PlateChem.GetQtyFromSolutionVolume(lTargetWell);
    			lSourceSolventTaken := PlateChem.GetSolVol_SolutionVolume(lTargetWell);
  	      --' if the qty calculation returns 0 but the well is a solvated well then 
	        --' the qty taken will be calculated as a percentage of solvent taken
	        IF lSourceQtyTaken = 0 AND lTargetWell.CompoundState = PlateChem.cSolvatedDry THEN
  	      	lSourceQtyTaken := (ChemCalcs.Convert(pAmt,pAmtUnitID, lSourceWell.SolventVolumeUnitID)/lSourceWell.SolventVolume) * lSourceWell.QtyRemaining;
  	      END IF;
	   		ELSIF lSourceWell.QtyUnitTypeID = constants.cVolumeID THEN
    			--wet compound
    			IF lSourceWell.SolventVolume > 0 THEN
    				--solvated
    				lSourceQtyTaken := PlateChem.GetQtyFromSolutionVolume(lTargetWell);
            IF lSourceQtyTaken < 0 THEN lSourceQtyTaken := 0; END IF;
    	      --' if the qty calculation returns 0 but the well is a solvated well then 
  	        --' the qty taken will be calculated as a percentage of solution taken
  	        IF lSourceQtyTaken = 0 AND lTargetWell.CompoundState = PlateChem.cSolvatedWet THEN
    	      	lSourceQtyTaken := (ChemCalcs.Convert(pAmt,pAmtUnitID, lSourceWell.SolventVolumeUnitID)/lSourceWell.SolutionVolume) * lSourceWell.QtyRemaining;
    	      END IF;
            lTargetWell.QtyRemaining := lSourceQtyTaken;
    				lSourceSolventTaken := PlateChem.GetSolVol_SolutionVolume(lTargetWell);
    			ELSE
    				--wet
    				lSourceQtyTaken := lTargetWell.QtyRemaining;
    				lSourceSolventTaken := 0;
    			END IF;
        --' well with solution only
        ELSIF lSourceWell.QtyUnitTypeID IS NULL AND lSourceWell.SolventVolume IS NULL AND lSourceWell.SolutionVolume > 0 THEN
        	lSourceQtyTaken := 0;
        	lSourceSolutionTaken := ChemCalcs.Convert(pAmt,pAmtUnitID, lSourceWell.SolventVolumeUnitID);
    		END IF;
        lTargetWell.QtyRemaining := lSourceQtyTaken;
    	ELSIF lAmtUnitType = constants.cMassID THEN
    	  lSourceSolventTaken := PlateChem.GetSolVol_Qty(lTargetWell);
        --' if the solvent volume calculation returns 0 but the well is a solvated well then 
        --' the solvent volume taken will be calculated as a percentage of qty taken
        IF lSourceSolventTaken <= 0 AND lTargetWell.CompoundState = PlateChem.cSolvatedDry THEN
        	lSourceSolventTaken := (ChemCalcs.Convert(pAmt,pAmtUnitID, lSourceWell.QtyRemainingUnitID)/lSourceWell.QtyRemaining) * lSourceWell.SolventVolume;
	        --PlateChem.QuantitySubtraction(lSourceWell.QtyRemaining, lSourceWell.QtyRemainingUnitID, pAmt, pAmtUnitID)/lSourceWell.QtyRemaining) * lSourceWell.SolventVolume;
        END IF;
    	END IF;

			IF pAddSolvent THEN
				lTargetSolventIDFK := pSolventID;
				IF lTargetWell.SolventVolumeUnitID is null then
					lSolventVolumeAdded := pSolventVolume;
					lTargetWell.SolventVolumeUnitID := pSolventVolumeUnitID;
				ELSE
					lSolventVolumeAdded := ChemCalcs.Convert(pSolventVolume, pSolventVolumeUnitID, lTargetWell.SolventVolumeUnitID);
				END IF;
			ELSE
				lSolventVolumeAdded := 0;
			END IF;
			lTargetWell.SolventVolume := lSourceSolventTaken + lSolventVolumeAdded;

      IF lSourceSolutionTaken IS NOT NULL THEN
      	lTargetWell.SolutionVolume := lSourceSolutionTaken;
      END IF;
  
      IF lCurrTWellID <> well_rec.target_well_id THEN
				--add to target wells
				UPDATE inv_wells SET
					qty_initial = lTargetWell.QtyRemaining,
  				qty_remaining = lTargetWell.QtyRemaining,
	   			qty_unit_fk = lTargetWell.QtyRemainingUnitID,
   				solvent_id_fk = lTargetSolventIDFK,
   				solvent_volume = lTargetWell.SolventVolume,
   				solvent_volume_initial = lTargetWell.SolventVolume,
	   			solvent_volume_unit_id_fk = lTargetWell.SolventVolumeUnitID,
          solution_volume = lTargetWell.SolutionVolume,
	   			concentration = lTargetWell.Concentration,
	   			conc_unit_fk =lTargetWell.ConcentrationUnitID
	   			--concentration = lTargetConcentration,
	   			--conc_unit_fk =lTargetConcUnitFK
   			WHERE
   				plate_id_fk = lNewPlateID
   				AND well_id = well_rec.target_well_id;

          lCurrTWellID := well_rec.target_well_id;
          lSourceAction := 'replace';
       ELSE
          lSourceAction := 'add';
       END IF;

			--subtract from source wells
			--vChangeWellQtyReturn := platechem.ChangeWellQty(well_rec.source_well_id,(vTargetWell.QtyRemaining*-1),vTargetWell.QtyRemainingUnitID);
		  lChangeWellQtyReturn2 := platechem.DecrementWellQuantities(
          well_rec.source_well_id,
          well_rec.target_well_id,
          lTargetWell.QtyRemaining,
          lTargetWell.QtyRemaining,
          lTargetWell.QtyRemainingUnitID,
          lSourceSolventTaken,
          lSourceSolutionTaken,
          lTargetWell.SolventVolumeUnitID,
          null,
     			NULL,
          lSourceAction);
		END LOOP;
	END LOOP;


END UpdateWellAmounts;


FUNCTION ReformatPlates(
	pSourcePlateIDList varchar2,
	pReformatMapID inv_xmldocs.xmldoc_id%TYPE,
	pBarcodeList varchar2,
	pPlateTypeID inv_plates.plate_type_id_fk%TYPE,
	pAmt NUMBER,
	pAmtUnitID inv_units.unit_id%TYPE,
	pSolventID inv_solvents.solvent_id%TYPE,
	pSolventVolume inv_plates.solvent_volume%TYPE,
	pSolventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE,
	pLocationID inv_plates.location_id_fk%TYPE)
RETURN varchar2
IS
vEngine xslprocessor.Processor := xslprocessor.newProcessor;
vParser xmlparser.Parser := xmlparser.newParser;

vBarcodeList_t STRINGUTILS.t_char;
vPlateFormatIDList_t STRINGUTILS.t_char;
vNewPlateIDList_t STRINGUTILS.t_char;

vMapClob CLOB;
vMapName inv_xmldocs.name%TYPE;
vTargetClob CLOB;
vTargetDoc xmldom.DOMDocument;
vTargetNode xmldom.DOMNode;
vIsDaughter boolean:=false;
vAddSolvent boolean := true;
vNewPlateIDs varchar2(200);
BEGIN
	--' Prep the input values
	vBarcodeList_t := STRINGUTILS.split(pBarcodeList, ',');
	IF pSolventVolume is null THEN
		vAddSolvent := false;
	END IF;

	--' Get the target plate formats
	vPlateFormatIDList_t := STRINGUTILS.split(GETPLATEFORMATS(pReformatMapID, 'target'), ',');

 	--' Get reformat map info
  SELECT xmldoc, name into vMapClob, vMapName FROM inv_xmldocs WHERE xmldoc_id = pReformatMapID;

	--' check if this is a daughtering
	IF instr(lower(vMapName),'daughter')>0 THEN
		vIsDaughter := true;
	END IF;
  
  --Create the target plate xml
	vTargetDoc := CreateTargetPlateXML(vEngine, vParser, vMapClob, vIsDaughter, pSourcePlateIDList, vBarcodeList_t, pLocationID, pPlateTypeID, vPlateFormatIDList_t);
  vTargetNode := xmldom.MakeNode(vTargetDoc);

	--create the target plates
	IF vIsDaughter THEN
		vNewPlateIDs := CreateDaughterPlates(vTargetDoc, vTargetNode, vBarcodeList_t);
	ELSE
	  DBMS_LOB.CREATETEMPORARY(vTargetClob, FALSE, DBMS_LOB.CALL);
   	xmldom.writeToClob(vTargetNode, vTargetClob);
		vNewPlateIDs := createPlateXML(vTargetClob, false);
	END IF;
	vNewPlateIDList_t := STRINGUTILS.split(vNewPlateIDs, ',');

  -- update well amounts plate by plate
	UpdateWellAmounts(vNewPlateIDList_t, vIsDaughter, vAddSolvent,  pAmt, pAmtUnitID, pSolventID, pSolventVolume, pSolventVolumeUnitID);

  --Set the aggregate plate data
	platechem.SetAggregatedPlateData(TRIM(',' FROM vNewPlateIDs));
	platechem.SetAggregatedPlateData(pSourcePlateIDList);

  --Clean Up
	xmldom.freeDocument(vTargetDoc);
	IF NOT vIsDaughter THEN
  	dbms_lob.freetemporary(vTargetClob);
  END IF;
  xmlparser.freeParser(vParser);
	xslprocessor.freeProcessor(vEngine);

  --Return  new plate IDs
  RETURN TRIM(',' FROM vNewPlateIDs);

	EXCEPTION
  	WHEN OTHERS THEN
			xmldom.freeDocument(vTargetDoc);
   		xmlparser.freeParser(vParser);
			xslprocessor.freeProcessor(vEngine);
			RETURN 'Reformat Error: ' || SQLCODE || ':' || SQLERRM;


END ReformatPlates;


FUNCTION InsertReformatMap(pMapXML CLOB, pName inv_xmldocs.name%TYPE)
RETURN inv_xmldocs.xmldoc_id%TYPE
IS
vDocTypeID inv_xmldoc_types.xmldoc_type_id%TYPE;
vDocID inv_xmldocs.xmldoc_id%TYPE;
BEGIN
	SELECT xmldoc_type_id INTO vDocTypeID FROM inv_xmldoc_types WHERE lower(type_name) = 'reformat map';
	INSERT INTO inv_xmldocs (xmldoc, name, xmldoc_type_id_fk) VALUES (pMapXML, pName, vDocTypeID) RETURNING xmldoc_ID INTO vDocID;
	RETURN vDocID;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN -1;

END InsertReformatMap;

FUNCTION DeleteReformatMap(pXmlDocID inv_xmldocs.xmldoc_id%TYPE)
RETURN inv_xmldocs.xmldoc_id%TYPE
IS
BEGIN

	DELETE inv_xmldocs WHERE xmldoc_id = pXmlDocID;
	RETURN pXmlDocID;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN -1;

END DeleteReformatMap;

FUNCTION CreateDaughteringMap(
	pPlateFormatID inv_plate_format.plate_format_id%TYPE)
RETURN inv_xmldocs.xmldoc_id%TYPE
IS
vEngine xslprocessor.Processor := xslprocessor.newProcessor;
vParser xmlparser.Parser := xmlparser.newParser;

vPhysPlateID inv_physical_plate.phys_plate_id%TYPE;
vGridFormatID inv_grid_format.grid_format_id%TYPE;
vQuery varchar2(1000);
vQueryClob CLOB;
vQueryDoc xmldom.DOMDocument;
vXsltClob CLOB;
vTargetClob CLOB;
vTargetNode xmldom.DOMNode;
vMapNode xmldom.DOMNode;
vMapClob CLOB;

vXsltDoc xmldom.DOMDocument;
vXslt xslprocessor.Stylesheet;
vNewXMLDocID inv_xmldocs.xmldoc_id%TYPE;
BEGIN

	SELECT phys_plate_id_fk INTO vPhysPlateID FROM inv_plate_format WHERE plate_format_id = pPlateFormatID;
	SELECT grid_format_id_fk INTO vGridFormatID FROM inv_physical_plate WHERE phys_plate_id = vPhysPlateID;
	vQuery := 'SELECT grid_position_id, grid_format_id_fk, row_index, col_index, ' || pPlateFormatID || ' AS plate_format_id, row_count, col_count
			FROM inv_grid_position g, inv_grid_format gf
			WHERE grid_format_id = ' || vGridFormatID || '
				AND grid_format_id = grid_format_id_fk
			ORDER BY row_index, col_index';
	vQueryClob := DBMS_XMLQuery.getXML(vQuery);

	xmlparser.parseClob(vParser, vQueryClob);
	vQueryDoc := xmlparser.getDocument(vParser);
	SELECT xslt INTO vXsltClob FROM inv_xslts WHERE xslt_name = 'Create Daugtering Map';
	DBMS_LOB.CREATETEMPORARY(vTargetClob, FALSE, DBMS_LOB.CALL);
	xmlparser.parseClob(vParser, vXsltClob);
	vXsltDoc := xmlparser.getDocument(vParser);
	vXslt := xslprocessor.newStyleSheet(vXsltDoc, null);
	xslprocessor.processXSL(vEngine, vXslt, vQueryDoc, vTargetClob);

	--get query XML without the <?xml?> tag
	vTargetNode := XMLUTILS.CLOB2NODE(vParser, vTargetClob);
	vMapNode := xmldom.getLastChild(vTargetNode);
 	DBMS_LOB.CREATETEMPORARY(vMapClob, FALSE, DBMS_LOB.CALL);
	xmldom.writeToClob(vMapNode,vMapClob);

	INSERT INTO inv_xmldocs (xmldoc,name,xmldoc_type_id_fk) VALUES (vMapClob,'Daughter' || pPlateFormatID,1) RETURNING xmldoc_id into vNewXMLDocID;

	RETURN vNewXMLDocID;

	xmldom.freeDocument(vXsltDoc);
	dbms_lob.freetemporary(vTargetClob);
	dbms_lob.freetemporary(vMapClob);
  xmlparser.freeParser(vParser);
	xslprocessor.freeProcessor(vEngine);
END CreateDaughteringMap;

FUNCTION DryPlate(pPlateIDList varchar2)
	RETURN varchar2
IS
vPlateIDList_t STRINGUTILS.t_char;

BEGIN
	vPlateIDList_t := STRINGUTILS.split(pPlateIDList, ',');
	--update plates
	FORALL i in vPlateIDList_t.First..vPlateIDList_t.Last
		UPDATE inv_plates SET
			solvent_id_fk = null,
			solvent_volume = 0,
			concentration = null,
			conc_unit_fk = null
		WHERE plate_id = vPlateIDList_t(i);

	--update wells
	FORALL i in vPlateIDList_t.First..vPlateIDList_t.Last
		UPDATE inv_wells SET
			solvent_id_fk = null,
			solvent_volume = 0,
			concentration = 0,
			conc_unit_fk = 0
		WHERE plate_id_fk = vPlateIDList_t(i)
			AND well_format_id_fk <> 2;

	RETURN '1';

	EXCEPTION
		WHEN OTHERS THEN
			RETURN '-1';

End DryPlate;

END REFORMAT;
/
show errors;