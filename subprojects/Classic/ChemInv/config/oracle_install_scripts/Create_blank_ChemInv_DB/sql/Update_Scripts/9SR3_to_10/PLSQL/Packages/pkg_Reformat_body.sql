CREATE OR REPLACE PACKAGE BODY "REFORMAT" AS

	FUNCTION GetPlateXML(p_plateIDs VARCHAR2) RETURN CLOB IS
		l_parser    DBMS_xmlparser.Parser := DBMS_xmlparser.newParser;
		l_plateXML  CLOB;
		l_wellXML   CLOB;
		l_returnXML CLOB;
	BEGIN

		l_plateXML := xmlutils.RemoveXMLElement(l_parser,
																						DBMS_XMLGEN.getXML('Select * from inv_plates where plate_id in (' ||
																																 p_plateIDs || ')'));
		l_wellXML  := xmlutils.RemoveXMLElement(l_parser,
																						DBMS_XMLGEN.getXML('Select w.*, row_index, col_index from inv_wells w, inv_grid_position gp where w.plate_id_fk in (' ||
																																 p_plateIDs ||
																																 ') and w.grid_position_id_fk = gp.grid_position_id'));
		--vWellXML := xmlutils.RemoveXMLElement(vParser, DBMS_XMLGEN.getXML('Select w.*, row_index, col_index from inv_wells w, inv_vw_well2 vw where w.plate_id_fk in (' || pPlateIDs ||') and w.plate_id_fk = vw.plate_id_fk'));
		l_returnXML := xmlutils.MERGE_XML_CLOBS(l_plateXML, l_wellXML);
		DBMS_xmlparser.freeParser(l_parser);
		
		RETURN l_returnXML;

	END GetPlateXML;

	FUNCTION GetNumTargetPlates(p_reformatMapID inv_xmldocs.xmldoc_id%TYPE)
		RETURN NUMBER IS
		l_parser          DBMS_xmlparser.Parser;
		l_mapClob         inv_xmldocs.xmldoc%TYPE;
		l_mapDoc          DBMS_xmldom.DOMDocument;
		l_targetPlatesNL  DBMS_xmldom.DOMNodeList;
		l_numTargetPlates NUMBER;
	BEGIN
		-- get map document
		SELECT xmldoc
			INTO l_mapClob
			FROM inv_xmldocs
		 WHERE xmldoc_ID = p_reformatMapID;
		l_parser := DBMS_xmlparser.newParser;
		l_mapDoc := xmlutils.CLOB2DOC(l_parser, l_mapClob);
		DBMS_xmlparser.freeParser(l_parser);

		l_targetPlatesNL  := DBMS_xmldom.getElementsByTagName(l_mapDoc,
																										 'TARGET_PLATE');
		l_numTargetPlates := DBMS_xmldom.getLength(l_targetPlatesNL);

		DBMS_xmldom.freeDocument(l_mapDoc);
		DBMS_xmlparser.freeParser(l_parser);

		RETURN l_numTargetPlates;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN - 1;

	END GetNumTargetPlates;

	FUNCTION GetPlateFormats(p_reformatMapID inv_xmldocs.xmldoc_id%TYPE,
													 p_plateType     VARCHAR2) RETURN VARCHAR2 IS

		l_engine             DBMS_xslprocessor.Processor := DBMS_xslprocessor.newProcessor;
		l_parser             DBMS_xmlparser.Parser;
		l_mapClob            inv_xmldocs.xmldoc%TYPE;
		l_mapDoc             DBMS_xmldom.DOMDocument;
		l_sourcePlatesNL     DBMS_xmldom.DOMNodeList;
		l_counter            NUMBER;
		l_node               DBMS_xmldom.DOMNode;
		l_sourcePlateFormats VARCHAR2(200);
		l_elementName        VARCHAR2(50);
		l_formatID           VARCHAR2(50);

		invalidPlateType EXCEPTION;
	BEGIN
		-- get map document
		SELECT xmldoc
			INTO l_mapClob
			FROM inv_xmldocs
		 WHERE xmldoc_ID = p_reformatMapID;
		l_parser := DBMS_xmlparser.newParser;
		l_mapDoc := xmlutils.CLOB2DOC(l_parser, l_mapClob);
		DBMS_xmlparser.freeParser(l_parser);

		IF lower(p_plateType) = 'source' THEN
			l_elementName := 'SOURCE_PLATE';
		ELSIF lower(p_plateType) = 'target' THEN
			l_elementName := 'TARGET_PLATE';
		ELSE
			RAISE invalidPlateType;
		END IF;
		l_sourcePlatesNL := DBMS_xmldom.getElementsByTagName(l_mapDoc, l_elementName);
		FOR l_counter IN 0 .. DBMS_xmldom.getLength(l_sourcePlatesNL) - 1
		LOOP
			l_node := DBMS_xmldom.item(l_sourcePlatesNL, l_counter);
			IF l_counter = 0 THEN
				DBMS_xslprocessor.valueOf(l_node,'./@PLATE_FORMAT_ID_FK',l_sourcePlateFormats);
			ELSE
				DBMS_xslprocessor.valueOf(l_node,'./@PLATE_FORMAT_ID_FK',l_formatID);
				l_sourcePlateFormats := l_sourcePlateFormats || ',' || l_formatID;
			END IF;
		END LOOP;

		DBMS_xslprocessor.freeProcessor(l_engine);
		DBMS_xmldom.freeDocument(l_mapDoc);

		RETURN l_sourcePlateFormats;

	EXCEPTION
		WHEN invalidPlateType THEN
			DBMS_xslprocessor.freeProcessor(l_engine);
			DBMS_xmldom.freeDocument(l_mapDoc);
			RETURN '-1';

	END GetPlateFormats;

	FUNCTION GetDaughterReformatMaps(p_sourcePlateIDs VARCHAR2) RETURN VARCHAR2 IS
		l_plateIDs_t    STRINGUTILS.t_char;
		l_plateID       inv_plates.plate_id%TYPE;
		l_plateFormatID inv_plate_format.plate_format_id%TYPE;
		l_xmlDocID      inv_xmldocs.xmldoc_id%TYPE;
		l_xmlDocIDList  VARCHAR2(500) := '';
		l_count         NUMBER;
	BEGIN
		l_plateIDs_t := STRINGUTILS.split(p_sourcePlateIDs, ',');
		FOR i IN l_plateIDs_t.FIRST .. l_plateIDs_t.LAST
		LOOP
			l_plateID := l_plateIDs_t(i);
			SELECT plate_format_id_fk
				INTO l_plateFormatID
				FROM inv_plates
			 WHERE plate_id = l_plateID;
			SELECT COUNT(xmldoc_id)
				INTO l_count
				FROM inv_xmldocs
			 WHERE xmldoc_type_id_fk = 1
						 AND lower(NAME) = 'daughter' || l_plateFormatID;
			IF l_count > 0 THEN
				SELECT xmldoc_id
					INTO l_xmlDocID
					FROM inv_xmldocs
				 WHERE xmldoc_type_id_fk = 1
							 AND lower(NAME) = 'daughter' || l_plateFormatID;
			ELSE
				l_xmlDocID := -1;
			END IF;
			l_xmlDocIDList := l_xmlDocIDList || l_xmlDocID || ',';
		END LOOP;
		RETURN TRIM(both ',' FROM l_xmlDocIDList);

	EXCEPTION
		WHEN OTHERS THEN
			RETURN - 1;
	END;

	FUNCTION GetPlateBarcodes(p_plateIDs VARCHAR2) RETURN VARCHAR2 IS
		l_plateIDs_t       STRINGUTILS.t_char;
		l_plateID          VARCHAR2(10);
		l_plateBarcode     inv_plates.plate_barcode%TYPE;
		l_plateBarcodeList VARCHAR2(200) := '';

	BEGIN
		l_plateIDs_t := STRINGUTILS.split(p_plateIDs, ',');
		FOR i IN l_plateIDs_t.FIRST .. l_plateIDs_t.LAST
		LOOP
			l_plateID := l_plateIDs_t(i);
			SELECT plate_barcode
				INTO l_plateBarcode
				FROM inv_plates
			 WHERE plate_id = l_plateID;
			l_plateBarcodeList := l_plateBarcodeList || l_plateBarcode || ',';
		END LOOP;

		RETURN TRIM(both ',' FROM l_plateBarcodeList);

	END GetPlateBarcodes;

	/*
    Purpose: Checks to see if the plate has material in it.  Material is defined here as a qty_remaining or solution volume.
  */
	FUNCTION CheckPlateQuantity(p_sourcePlateIDs VARCHAR2) RETURN VARCHAR2 IS
		l_sourcePlateIDs_t STRINGUTILS.t_char;
		l_plateID          inv_plates.plate_id%TYPE;
		--lMolarAmount inv_plates.molar_amount%TYPE;
		l_qtyRemaining          inv_plates.qty_remaining%TYPE;
		l_solutionVolume        inv_plates.solution_volume%TYPE;
		l_amountProblemPlateIDs VARCHAR2(100) := '';

		lNoMaterial EXCEPTION;
	BEGIN

		--' update plate properties before checking
		platechem.SetAggregatedPlateData(p_sourcePlateIDs);

		--' check each plate
		l_sourcePlateIDs_t := STRINGUTILS.split(p_sourcePlateIDs, ',');
		FOR i IN l_sourcePlateIDs_t.FIRST .. l_sourcePlateIDs_t.LAST
		LOOP
			l_plateID := l_sourcePlateIDs_t(i);
			SELECT qty_remaining, solution_volume
				INTO l_qtyRemaining, l_solutionVolume
				FROM inv_plates
			 WHERE plate_id = l_plateID;
			--' check qty_remaining amount
			--' check solution volume
			IF (l_qtyRemaining IS NULL OR l_qtyRemaining <= 0) AND
				 (l_solutionVolume IS NULL OR l_solutionVolume <= 0) THEN
				l_amountProblemPlateIDs := l_amountProblemPlateIDs || l_plateID || ',';
			END IF;
		END LOOP;

		IF length(l_amountProblemPlateIDs) > 0 THEN
			l_amountProblemPlateIDs := TRIM(both ',' FROM l_amountProblemPlateIDs);
			RAISE lNoMaterial;
		END IF;

		RETURN '1';

	EXCEPTION
		WHEN lNoMaterial THEN
			RETURN 'These plates do not contain material: ' || GETPLATEBARCODES(l_amountProblemPlateIDs);

	END CheckPlateQuantity;

	FUNCTION CheckValidSourcePlates(p_sourcePlateIDs VARCHAR2,
																	p_reformatMapID  inv_xmldocs.xmldoc_id%TYPE)
		RETURN VARCHAR2 IS
		l_sourcePlateFormats  VARCHAR2(200);
		l_sourcePlateIDs_t    STRINGUTILS.t_char;
		l_sourcePlateTypes_t  STRINGUTILS.t_char;
		l_plateID             inv_plates.plate_id%TYPE;
		l_sourcePlateTypeIDFK inv_plates.plate_format_id_fk%TYPE;
		l_typeProblemPlateIDs VARCHAR2(100) := '';

		incorrectSourcePlateCount EXCEPTION;
		incorrectSourcePlateType EXCEPTION;
	BEGIN

		l_sourcePlateFormats := GETPLATEFORMATS(p_reformatMapID, 'source');
		l_sourcePlateIDs_t   := STRINGUTILS.split(p_sourcePlateIDs, ',');
		l_sourcePlateTypes_t := STRINGUTILS.split(l_sourcePlateFormats, ',');
		IF l_sourcePlateIDs_t.COUNT != l_sourcePlateTypes_t.COUNT THEN
			RAISE incorrectSourcePlateCount;
		END IF;

		-- check each plate
		FOR i IN l_sourcePlateIDs_t.FIRST .. l_sourcePlateIDs_t.LAST
		LOOP
			l_plateID := l_sourcePlateIDs_t(i);
			SELECT plate_format_id_fk
				INTO l_sourcePlateTypeIDFK
				FROM inv_plates
			 WHERE plate_id = l_plateID;
			IF l_sourcePlateTypeIDFK != l_sourcePlateTypes_t(i) THEN
				l_typeProblemPlateIDs := l_typeProblemPlateIDs || l_plateID || ',';
			END IF;
		END LOOP;

		IF length(l_typeProblemPlateIDs) > 0 THEN
			l_typeProblemPlateIDs := TRIM(both ',' FROM l_typeProblemPlateIDs);
			RAISE incorrectSourcePlateType;
		END IF;

		RETURN '1';

	EXCEPTION
		WHEN incorrectSourcePlateCount THEN
			RETURN 'Incorrect number of source plates.';
		WHEN incorrectSourcePlateType THEN
			RETURN 'Incorrect source plate types for plate ids: ' || GETPLATEBARCODES(l_typeProblemPlateIDs);

	END CheckValidSourcePlates;

	FUNCTION GetValidReformatMaps(p_sourcePlateIDs VARCHAR2) RETURN VARCHAR2 IS
		CURSOR l_map_cur IS
			SELECT xmldoc_id FROM inv_xmldocs WHERE xmldoc_type_id_fk = 1;
		l_isValid  VARCHAR2(200);
		l_validIDs VARCHAR2(200) := '';
		Invalid_SourcePlates EXCEPTION;
	BEGIN
		FOR vMap_rec IN l_map_cur
		LOOP
			l_isValid := CHECKVALIDSOURCEPLATES(p_sourcePlateIDs,
																					vMap_rec.xmldoc_id);
			IF l_isValid = '1' THEN
				l_validIDs := l_validIDs || vMap_rec.xmldoc_id || ',';
				--ELSE
				--RAISE Invalid_SourcePlates;
			END IF;
		END LOOP;

		IF l_validIDs IS NULL THEN
			RETURN '-1';
		ELSE
			RETURN TRIM(both ',' FROM l_validIDs);
		END IF;

	EXCEPTION
		WHEN Invalid_SourcePlates THEN
			RETURN '-2';
		WHEN OTHERS THEN
			RETURN '-5';

	END GetValidReformatMaps;

	FUNCTION CreateTargetPlateXML(p_engine              DBMS_xslprocessor.Processor,
																p_parser              DBMS_xmlparser.Parser,
																p_mapClob             inv_xmldocs.xmldoc%TYPE,
																p_isDaughter          BOOLEAN,
																p_sourcePlateIDList   VARCHAR2,
																p_barcodeList_t       STRINGUTILS.t_char,
																p_locationID          inv_plates.location_id_fk%TYPE,
																p_plateTypeID         inv_plates.plate_type_id_fk%TYPE,
																p_plateFormatIDList_t STRINGUTILS.t_char)
		RETURN DBMS_xmldom.DOMDocument IS
		l_sourcePlateIDList_t STRINGUTILS.t_char;

		l_query   VARCHAR(2000);
		l_mapClob CLOB;
		l_mapDoc  DBMS_xmldom.DOMDocument;
		l_mapNode DBMS_xmldom.DOMNode;

		l_queryClob  CLOB;
		l_mergeDoc   DBMS_xmldom.DOMDocument;
		l_xsltClob   CLOB;
		l_targetDoc  DBMS_xmldom.DOMDocument;
		l_targetNode DBMS_xmldom.DOMNode;

		l_plateID   VARCHAR2(10);
		l_node      DBMS_xmldom.DOMNode;
		l_element   DBMS_xmldom.DOMElement;
		l_attribute DBMS_xmldom.DOMAttr;

		l_barcodeDescId inv_barcode_desc.barcode_desc_id%TYPE;
		l_barcode       inv_plates.plate_barcode%TYPE;
	BEGIN
		--' Prep the input values
		l_sourcePlateIDList_t := STRINGUTILS.split(p_sourcePlateIDList, ',');

		--This query gets the positions of the source plate wells and data that will be copied to the target wells
		l_query     := 'SELECT w.plate_id_fk , w.well_id as parent_well_id_fk,  g.row_index, g.col_index,wc.compound_id_fk AS compound_id_fk,wc.reg_id_fk AS reg_id_fk, wc.batch_number_fk as batch_number_fk FROM inv_wells w INNER JOIN inv_grid_position g on g.grid_position_id = w.grid_position_id_fk LEFT OUTER JOIN inv_well_compounds wc on wc.well_id_fk = w.well_id
				WHERE  plate_id_fk IN ( ' || p_sourcePlateIDList || ')
                                ORDER BY w.plate_id_fk, g.row_index, g.col_index';
		l_queryClob := XMLUTILS.RemoveXMLElement(p_parser,
																						 DBMS_XMLGEN.getXML(l_query));

		-- update source plates with plate ids
		--INSERT INTO inv_debug  VALUES (pMapClob);
		l_mapDoc  := xmlutils.CLOB2DOC(p_parser, p_mapClob);
		l_mapNode := DBMS_xmldom.makeNode(l_mapDoc);
		FOR i IN l_sourcePlateIDList_t.FIRST .. l_sourcePlateIDList_t.LAST
		LOOP
			l_plateID   := l_sourcePlateIDList_t(i);
			l_node      := DBMS_xslprocessor.selectSingleNode(l_mapNode,
																									 '/REFORMAT_MAP/SOURCE_PLATE[' || i || ']');
			l_element   := DBMS_xmldom.makeElement(l_node);
			l_attribute := DBMS_xmldom.createAttribute(l_mapDoc, 'PLATE_ID_FK');
			DBMS_xmldom.setValue(l_attribute, l_plateID);
			l_attribute := DBMS_xmldom.setAttributeNode(l_element, l_attribute);
		END LOOP;

		DBMS_LOB.CREATETEMPORARY(l_mapClob, FALSE, DBMS_LOB.CALL);
		DBMS_xmldom.writeToClob(l_mapNode, l_mapClob);
		--merge the mapdoc with the querydoc
		l_mergeDoc := XMLUTILS.CLOB2DOC(p_parser,
																		XMLUTILS.MERGE_XML_CLOBS(l_queryClob,
																														 l_mapClob));
		--transform merged document into target plates node
		SELECT xslt
			INTO l_xsltClob
			FROM inv_xslts
		 WHERE xslt_name = 'Create Target Plates';
		l_targetDoc  := XMLUTILS.CLOB2DOC(p_parser,
																			XMLUTILS.transformXML(l_xsltClob,
																														l_mergeDoc,
																														p_parser,
																														p_engine));
		l_targetNode := DBMS_xmldom.MakeNode(l_targetDoc);

		--set barcode desc id or plate_barcode ,location, plate type on the target plates
		FOR i IN p_plateFormatIDList_t.FIRST .. p_plateFormatIDList_t.LAST
		LOOP
			l_node    := DBMS_xslprocessor.selectSingleNode(l_targetNode,
																								 '/PLATES/PLATE[' || i || ']');
			l_element := DBMS_xmldom.makeElement(l_node);
			IF NOT p_isDaughter THEN
				IF instr(p_barcodeList_t(i), ':auto') > 0 THEN
					l_barcodeDescId := rtrim(p_barcodeList_t(i), ':auto');
					l_attribute     := DBMS_xmldom.createAttribute(l_targetDoc,
																										'BARCODE_DESC_ID');
					DBMS_xmldom.setValue(l_attribute, l_barcodeDescId);
				ELSE
					l_barcode   := p_barcodeList_t(i);
					l_attribute := DBMS_xmldom.createAttribute(l_targetDoc,
																								'PLATE_BARCODE');
					DBMS_xmldom.setValue(l_attribute, l_barcode);
				END IF;
				l_attribute := DBMS_xmldom.setAttributeNode(l_element, l_attribute);
			END IF;
			DBMS_xmldom.setValue(DBMS_xmldom.getAttributeNode(l_element, 'LOCATION_ID_FK'),
											p_locationID);
			DBMS_xmldom.setValue(DBMS_xmldom.getAttributeNode(l_element,
																							'PLATE_TYPE_ID_FK'),
											p_plateTypeID);
		END LOOP;

		--update well formats and concentration based on the plate format
		FOR i IN p_plateFormatIDList_t.FIRST .. p_plateFormatIDList_t.LAST
		LOOP
			FOR wellFormat_rec IN (SELECT grid_position_id,
																		well_format_id_fk,
																		concentration,
																		conc_unit_fk
															 FROM inv_vw_well_format
															WHERE plate_format_id_fk =
																		p_plateFormatIDList_t(i))
			LOOP
				l_node    := DBMS_xslprocessor.selectSingleNode(l_targetNode,
																									 '/PLATES/PLATE[' || i ||
																									 ']/ROW/COL/WELL[@GRID_POSITION_ID_FK="' ||
																									 wellFormat_rec.Grid_Position_Id || '"]');
				l_element := DBMS_xmldom.makeElement(l_node);
				--vAttribute := DBMS_xmldom.createAttribute(vTargetDoc, 'WELL_FORMAT_ID_FK');
				--DBMS_xmldom.setValue(vAttribute, wellFormat_rec.Well_Format_Id_Fk);
				--vAttribute := DBMS_xmldom.setAttributeNode(vElement, vAttribute);
				DBMS_xmldom.setAttribute(l_element,
														'WELL_FORMAT_ID_FK',
														wellFormat_rec.Well_Format_Id_Fk);

				l_attribute := DBMS_xmldom.createAttribute(l_targetDoc, 'CONCENTRATION');
				DBMS_xmldom.setValue(l_attribute, wellFormat_rec.Concentration);
				l_attribute := DBMS_xmldom.setAttributeNode(l_element, l_attribute);
				l_attribute := DBMS_xmldom.createAttribute(l_targetDoc, 'CONC_UNIT_FK');
				DBMS_xmldom.setValue(l_attribute, wellFormat_rec.Conc_Unit_Fk);
				l_attribute := DBMS_xmldom.setAttributeNode(l_element, l_attribute);
			END LOOP;
		END LOOP;

		DBMS_xmldom.freeDocument(l_mapDoc);
		DBMS_xmldom.freeDocument(l_mergeDoc);

		RETURN l_targetDoc;
	END CreateTargetPlateXML;

	FUNCTION CreateDaughterPlates(p_plateXMLDoc   DBMS_xmldom.DOMDocument,
																p_plateXMLNode  DBMS_xmldom.DOMNode,
																p_barcodeList_t STRINGUTILS.t_char)
		RETURN VARCHAR2 IS
		l_node          DBMS_xmldom.DOMNode;
		l_element       DBMS_xmldom.DOMElement;
		l_attribute     DBMS_xmldom.DOMAttr;
		l_barcodeDescID inv_barcode_desc.barcode_desc_id%TYPE;
		l_barcode       inv_plates.plate_barcode%TYPE;
		l_plateXMLClob  CLOB;
		l_tempPlateID   VARCHAR2(200);
		l_newPlateIDs   VARCHAR2(2000);
		l_libraryCount  INTEGER;
		l_groupCount    INTEGER;

	BEGIN
		FOR i IN p_barcodeList_t.FIRST .. p_barcodeList_t.LAST
		LOOP
			IF instr(p_barcodeList_t(i), ':auto') > 0 THEN
				l_barcodeDescID := rtrim(p_barcodeList_t(i), ':auto');
				l_attribute     := DBMS_xmldom.createAttribute(p_plateXMLDoc,
																									'BARCODE_DESC_ID');
				DBMS_xmldom.setValue(l_attribute, l_barcodeDescID);
			ELSE
				l_barcode   := p_barcodeList_t(i);
				l_attribute := DBMS_xmldom.createAttribute(p_plateXMLDoc,
																							'PLATE_BARCODE');
				DBMS_xmldom.setValue(l_attribute, l_barcode);
			END IF;
			l_node      := DBMS_xslprocessor.selectSingleNode(p_plateXMLNode,
																									 '/PLATES/PLATE[1]');
			l_element   := DBMS_xmldom.makeElement(l_node);
			l_attribute := DBMS_xmldom.setAttributeNode(l_element, l_attribute);
			DBMS_LOB.CREATETEMPORARY(l_plateXMLClob, FALSE, DBMS_LOB.CALL);
			DBMS_xmldom.writeToClob(p_plateXMLNode, l_plateXMLClob);
			l_tempPlateID := createPlateXML(l_plateXMLClob, FALSE);
			l_newPlateIDs := l_newPlateIDs || l_tempPlateID || ',';
			COMMIT;
			dbms_lob.freetemporary(l_plateXMLClob);
			--copy source plate attributes to new plates if this is a daughtering
			--only update library,group if there is the same value for all the parents
			SELECT COUNT(DISTINCT library_id_fk), COUNT(DISTINCT group_name)
				INTO l_libraryCount, l_groupCount
				FROM inv_plates, inv_plate_parent
			 WHERE plate_id = parent_plate_id_fk
						 AND child_plate_id_fk = l_tempPlateID;
			IF l_libraryCount = 1 THEN
				UPDATE inv_plates
					 SET library_id_fk = (SELECT DISTINCT library_id_fk
																	FROM inv_plates, inv_plate_parent
																 WHERE plate_id = parent_plate_id_fk
																			 AND child_plate_id_fk = l_tempPlateID)
				 WHERE PLATE_ID = l_tempPlateID;
			END IF;
			IF l_groupCount = 1 THEN
				UPDATE inv_plates
					 SET group_name = (SELECT DISTINCT group_name
															 FROM inv_plates, inv_plate_parent
															WHERE plate_id = parent_plate_id_fk
																		AND child_plate_id_fk = l_tempPlateID)
				 WHERE PLATE_ID = l_tempPlateID;
			END IF;
		END LOOP;

		RETURN TRIM(both ',' FROM l_newPlateIDs);
	END CreateDaughterPlates;

	FUNCTION CreateTargetWellData(p_sourceWell  platechem.chemdata,
																p_amt         NUMBER,
																p_amtUnitID   inv_units.unit_id%TYPE,
																p_amtUnitType inv_unit_types.unit_type_id%TYPE)
		RETURN PLATECHEM.chemdata IS

		l_targetWell platechem.chemdata;

	BEGIN
		-- create target well data
		l_targetWell := platechem.GetChemDataCopy(p_sourceWell);

		IF p_amtUnitType = constants.cVolumeID THEN
			IF p_sourceWell.QtyUnitTypeID = constants.cMassID THEN
				--solvated well, dry compound
				--vTargetWell.QtyRemaining := 0;
				--lTargetWell.SolutionVolume := 0;
				--vTargetWell.MolarAmount := 0;
				l_targetWell.SolutionVolume      := p_amt;
				l_targetWell.SolventVolumeUnitID := p_amtUnitID;
				l_targetWell.QtyRemaining        := PlateChem.GetQtyFromSolutionVolume(l_targetWell);
			ELSIF p_sourceWell.QtyUnitTypeID = constants.cVolumeID THEN
				--wet compound
				IF p_sourceWell.SolutionVolume > 0 THEN
					--solvated
					l_targetWell.QtyRemaining        := 0;
					l_targetWell.SolventVolume       := 0;
					l_targetWell.MolarAmount         := 0;
					l_targetWell.SolutionVolume      := p_amt;
					l_targetWell.SolventVolumeUnitID := p_amtUnitID;
					l_targetWell.QtyRemaining        := PlateChem.GetQtyFromSolutionVolume(l_targetWell);
					IF l_targetWell.QtyRemaining < 0 THEN
						l_targetWell.QtyRemaining := 0;
					END IF;
					--vTargetWell.SolventVolume := 0;
				ELSE
					--wet
					l_targetWell.QtyRemaining := 0;
					--vTargetWell.SolventVolume := 0;
					l_targetWell.MolarAmount        := 0;
					l_targetWell.QtyRemaining       := p_amt;
					l_targetWell.QtyRemainingUnitID := p_amtUnitID;
				END IF;
			END IF;
		ELSIF p_amtUnitType = constants.cMassID THEN
			l_targetWell.QtyRemaining       := p_amt;
			l_targetWell.QtyRemainingUnitID := p_amtUnitID;
		END IF;

		RETURN l_targetWell;

	END CreateTargetWellData;

	PROCEDURE UpdateWellAmounts(p_newPlateIDList_t    STRINGUTILS.t_char,
															p_isDaughter          BOOLEAN,
															p_addSolvent          BOOLEAN,
															p_amt                 NUMBER,
															p_amtUnitID           inv_units.unit_id%TYPE,
															p_solventID           inv_solvents.solvent_id%TYPE,
															p_solventVolume       inv_plates.solvent_volume%TYPE,
															p_solventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE) IS

		l_amtUnitType    inv_unit_types.unit_type_id%TYPE;
		l_index          NUMBER;
		l_newPlateID     inv_plates.plate_id%TYPE;
		l_isCompoundWell BOOLEAN := FALSE;
		l_compoundCount  INTEGER;
		l_sourceWell     platechem.chemdata;
		l_targetWell     platechem.chemdata;

		l_sourceQtyTaken       inv_wells.qty_remaining%TYPE;
		l_sourceSolventTaken   inv_wells.solvent_volume%TYPE;
		l_sourceSolutionTaken  inv_wells.solution_volume%TYPE := NULL;
		l_solventVolumeAdded   inv_wells.solvent_volume%TYPE;
		l_targetSolventIDFK    inv_solvents.solvent_id%TYPE;
		l_changeWellQtyReturn2 VARCHAR2(200);

		l_solutionVolume1 inv_wells.solution_volume%TYPE;
		l_solutionVolume2 inv_wells.solution_volume%TYPE;

		l_currTWellID  inv_wells.well_id%TYPE := 0;
		l_sourceAction VARCHAR2(10);
	BEGIN

		--' Determine the amount unit type
		l_amtUnitType := chemcalcs.GetUnitType(p_amtUnitID);

		FOR lIndexTemp IN p_newPlateIDList_t.FIRST .. p_newPlateIDList_t.LAST
		LOOP
			l_newPlateID := p_newPlateIDList_t(lIndexTemp);
			IF p_isDaughter THEN
				l_index := 1;
			ELSE
				l_index := lIndexTemp;
			END IF;
			--update well amounts well by well
			FOR well_rec IN (SELECT SOURCE.well_id AS source_well_id,
															target.well_id AS target_well_id,
															SOURCE.qty_remaining,
															SOURCE.qty_unit_fk,
															SOURCE.molar_amount,
															SOURCE.molar_conc,
															SOURCE.Solvent_Id_Fk,
															SOURCE.solvent_volume,
															SOURCE.solvent_volume_unit_id_fk,
															SOURCE.solution_volume,
															SOURCE.concentration,
															SOURCE.conc_unit_fk
												 FROM inv_wells SOURCE, inv_wells target
												WHERE SOURCE.well_id IN
															(SELECT parent_well_id_fk
																 FROM inv_well_parent
																WHERE child_well_id_fk = target.well_id)
															AND target.plate_id_fk = l_newPlateID
												ORDER BY target_well_id)
			LOOP
				-- determine if there are compounds in the well
				l_isCompoundWell := FALSE;
				SELECT COUNT(*)
					INTO l_compoundCount
					FROM inv_well_compounds
				 WHERE well_id_fk = well_rec.source_well_id;
				IF l_compoundCount > 0 THEN
					l_isCompoundWell := TRUE;
				END IF;
				-- populate source well data
				l_sourceWell.CompoundID          := NULL;
				l_sourceWell.RegID               := NULL;
				l_sourceWell.BatchNumber         := NULL;
				l_sourceWell.QtyRemaining        := well_rec.qty_remaining;
				l_sourceWell.QtyRemainingUnitID  := well_rec.qty_unit_fk;
				l_sourceWell.Concentration       := well_rec.concentration;
				l_sourceWell.ConcentrationUnitID := well_rec.conc_unit_fk;
				l_sourceWell.MolarAmount         := well_rec.molar_amount;
				l_sourceWell.MolarConc           := well_rec.molar_conc;
				l_sourceWell.SolventID           := well_rec.solvent_id_fk;
				l_sourceWell.SolventVolume       := well_rec.solvent_volume;
				l_sourceWell.SolutionVolume      := well_rec.solution_volume;
				l_sourceWell.SolventVolumeUnitID := well_rec.solvent_volume_unit_id_fk;
				IF l_sourceWell.QtyRemainingUnitID IS NOT NULL AND
					 l_sourceWell.QtyRemainingUnitID > 0 THEN
					l_sourceWell.QtyUnitTypeID := chemcalcs.GetUnitType(l_sourceWell.QtyRemainingUnitID);
				ELSE
					l_sourceWell.QtyUnitTypeID := NULL;
				END IF;
				l_sourceWell.CompoundState := platechem.GetWellCompoundState(l_sourceWell.SolutionVolume,
																																		 l_sourceWell.QtyRemainingUnitID);
				l_sourceWell.AvgMW         := plateChem.GetAverageMW(well_rec.source_well_id);
				l_sourceWell.AvgDensity    := plateChem.GetAverageDensity(well_rec.source_well_id);

				-- Create target well data
				l_targetWell := CreateTargetWellData(l_sourceWell,
																						 p_amt,
																						 p_amtUnitID,
																						 l_amtUnitType);

				IF l_amtUnitType = constants.cVolumeID THEN
					IF l_sourceWell.QtyUnitTypeID = constants.cMassID THEN
						l_sourceQtyTaken     := PlateChem.GetQtyFromSolutionVolume(l_targetWell);
						l_sourceSolventTaken := PlateChem.GetSolVol_SolutionVolume(l_targetWell);
						--' if the qty calculation returns 0 but the well is a solvated well then
						--' the qty taken will be calculated as a percentage of solvent taken
						IF l_sourceQtyTaken = 0 AND
							 l_targetWell.CompoundState = PlateChem.cSolvatedDry THEN
							 --' if the well has nothing in it don't take anything
							 IF l_sourceWell.SolventVolume = 0 OR l_sourceWell.QtyRemaining = 0 THEN
							 		l_sourceQtyTaken := 0;
							 ELSE

    							l_sourceQtyTaken := (ChemCalcs.Convert(p_amt,
    																										 p_amtUnitID,
    																										 l_sourceWell.SolventVolumeUnitID) /
    																	l_sourceWell.SolventVolume) *
    																	l_sourceWell.QtyRemaining;
                END IF;
						END IF;
					ELSIF l_sourceWell.QtyUnitTypeID = constants.cVolumeID THEN
						--wet compound
						IF l_sourceWell.SolventVolume > 0 THEN
							--solvated
							l_sourceQtyTaken := PlateChem.GetQtyFromSolutionVolume(l_targetWell);
							IF l_sourceQtyTaken < 0 THEN
								l_sourceQtyTaken := 0;
							END IF;
							--' if the qty calculation returns 0 but the well is a solvated well then
							--' the qty taken will be calculated as a percentage of solution taken
							IF l_sourceQtyTaken = 0 AND
								 l_targetWell.CompoundState = PlateChem.cSolvatedWet THEN
								l_sourceQtyTaken := (ChemCalcs.Convert(p_amt,
																											 p_amtUnitID,
																											 l_sourceWell.SolventVolumeUnitID) /
																		l_sourceWell.SolutionVolume) *
																		l_sourceWell.QtyRemaining;
							END IF;
							l_targetWell.QtyRemaining := l_sourceQtyTaken;
							l_sourceSolventTaken      := PlateChem.GetSolVol_SolutionVolume(l_targetWell);
						ELSE
							--wet
							l_sourceQtyTaken     := l_targetWell.QtyRemaining;
							l_sourceSolventTaken := 0;
						END IF;
						--' well with solution only
					ELSIF l_sourceWell.QtyUnitTypeID IS NULL AND
								l_sourceWell.SolventVolume IS NULL AND
								l_sourceWell.SolutionVolume > 0 THEN
						l_sourceQtyTaken      := 0;
						l_sourceSolutionTaken := ChemCalcs.Convert(p_amt,
																											 p_amtUnitID,
																											 l_sourceWell.SolventVolumeUnitID);
					END IF;
					l_targetWell.QtyRemaining := l_sourceQtyTaken;
				ELSIF l_amtUnitType = constants.cMassID THEN
					l_sourceSolventTaken := PlateChem.GetSolVol_Qty(l_targetWell);
					--' if the solvent volume calculation returns 0 but the well is a solvated well then
					--' the solvent volume taken will be calculated as a percentage of qty taken
					IF l_sourceSolventTaken <= 0 AND
						 l_targetWell.CompoundState = PlateChem.cSolvatedDry THEN
						IF l_sourceWell.QtyRemaining > 0 THEN
							--' try as % of qty first
							l_sourceSolventTaken := (ChemCalcs.Convert(p_amt,
																												 p_amtUnitID,
																												 l_sourceWell.QtyRemainingUnitID) /
																			l_sourceWell.QtyRemaining) *
																			l_sourceWell.SolventVolume;
						ELSIF l_sourceWell.SolventVolume > 0 THEN
							--' try as % of solvent volume
							l_sourceSolventTaken := (ChemCalcs.Convert(p_amt,
																												 p_amtUnitID,
																												 l_sourceWell.SolventVolume) /
																			l_sourceWell.SolventVolume) *
																			l_sourceWell.SolventVolume;
						ELSE
							--' don't take any
							l_sourceSolventTaken := 0;
						END IF;
						--PlateChem.QuantitySubtraction(lSourceWell.QtyRemaining, lSourceWell.QtyRemainingUnitID, pAmt, pAmtUnitID)/lSourceWell.QtyRemaining) * lSourceWell.SolventVolume;
					END IF;
				END IF;

				IF p_addSolvent THEN
					l_targetSolventIDFK := p_solventID;

					IF l_targetWell.SolventVolumeUnitID IS NULL THEN
						l_solventVolumeAdded             := p_solventVolume;
						l_targetWell.SolventVolumeUnitID := p_solventVolumeUnitID;
					ELSE
						l_solventVolumeAdded := ChemCalcs.Convert(p_solventVolume,
																											p_solventVolumeUnitID,
																											l_targetWell.SolventVolumeUnitID);
					END IF;
				ELSE
					l_solventVolumeAdded := 0;
					--' if the parent well had a solvent and the target well has no solvent assigned then use the parent well solvent
					IF l_targetWell.SolventID IS NULL AND l_sourceWell.SolventID IS NOT NULL THEN
						l_targetWell.SolventID := l_sourceWell.SolventID;
					END IF;
					l_targetSolventIDFK := l_targetWell.SolventID;

				END IF;
				l_targetWell.SolventVolume  := l_sourceSolventTaken +
																			 l_solventVolumeAdded;
				l_targetWell.SolutionVolume := l_targetWell.SolutionVolume +
																			 l_solventVolumeAdded;

				IF l_sourceSolutionTaken IS NOT NULL THEN
					l_targetWell.SolutionVolume := l_sourceSolutionTaken;
				END IF;

				IF p_addSolvent THEN
          --' calculate the new concentration: c2 = v1c1/v2
          --' get the solution volume given based on what's been taken from source well
          l_solutionVolume1 := platechem.GetSolutionVolume(NULL,l_sourceSolutionTaken,l_sourceSolventTaken,l_targetWell.SolventVolumeUnitID,l_sourceQtyTaken,l_targetWell.QtyRemainingUnitID, l_targetWell.concentration);
         	--' get new solution volume
          l_solutionVolume2 := platechem.GetSolutionVolume(NULL,l_targetWell.SolutionVolume,l_targetWell.SolventVolume,l_targetWell.SolventVolumeUnitID,l_targetWell.QtyRemaining,l_targetWell.QtyRemainingUnitID, l_targetWell.concentration);
					IF l_solutionVolume2 > 0 THEN
	          l_targetWell.Concentration := (l_targetWell.concentration * l_solutionVolume1)/l_solutionVolume2;
					END IF;           
				END IF;
        
				IF l_currTWellID <> well_rec.target_well_id THEN
					--add to target wells
					UPDATE inv_wells
						 SET qty_initial               = l_targetWell.QtyRemaining,
								 qty_remaining             = l_targetWell.QtyRemaining,
								 qty_unit_fk               = l_targetWell.QtyRemainingUnitID,
								 solvent_id_fk             = l_targetSolventIDFK,
								 solvent_volume            = l_targetWell.SolventVolume,
								 solvent_volume_initial    = l_targetWell.SolventVolume,
								 solvent_volume_unit_id_fk = l_targetWell.SolventVolumeUnitID,
								 solution_volume           = l_targetWell.SolutionVolume,
								 concentration             = l_targetWell.Concentration,
								 conc_unit_fk              = l_targetWell.ConcentrationUnitID
					--concentration = lTargetConcentration,
					--conc_unit_fk =lTargetConcUnitFK
					 WHERE plate_id_fk = l_newPlateID
								 AND well_id = well_rec.target_well_id;

					l_currTWellID  := well_rec.target_well_id;
					l_sourceAction := 'replace';
				ELSE
					l_sourceAction := 'add';
				END IF;

				--subtract from source wells
				--vChangeWellQtyReturn := platechem.ChangeWellQty(well_rec.source_well_id,(vTargetWell.QtyRemaining*-1),vTargetWell.QtyRemainingUnitID);
				l_changeWellQtyReturn2 := platechem.DecrementWellQuantities(well_rec.source_well_id,
																																		well_rec.target_well_id,
																																		l_targetWell.QtyRemaining,
																																		l_targetWell.QtyRemaining,
																																		l_targetWell.QtyRemainingUnitID,
																																		l_sourceSolventTaken,
																																		l_sourceSolutionTaken,
																																		l_targetWell.SolventVolumeUnitID,
																																		NULL,
																																		NULL,
																																		l_sourceAction);
			END LOOP;
		END LOOP;

	END UpdateWellAmounts;

	FUNCTION ReformatPlates(p_sourcePlateIDList   VARCHAR2,
													p_reformatMapID       inv_xmldocs.xmldoc_id%TYPE,
													p_barcodeList         VARCHAR2,
													p_plateTypeID         inv_plates.plate_type_id_fk%TYPE,
													p_amt                 NUMBER,
													p_amtUnitID           inv_units.unit_id%TYPE,
													p_solventID           inv_solvents.solvent_id%TYPE,
													p_solventVolume       inv_plates.solvent_volume%TYPE,
													p_solventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE,
													p_locationID          inv_plates.location_id_fk%TYPE)
		RETURN VARCHAR2 IS
		l_engine DBMS_xslprocessor.Processor := DBMS_xslprocessor.newProcessor;
		l_parser DBMS_xmlparser.Parser := DBMS_xmlparser.newParser;

		l_barcodeList_t       STRINGUTILS.t_char;
		l_plateFormatIDList_t STRINGUTILS.t_char;
		l_newPlateIDList_t    STRINGUTILS.t_char;

		l_mapClob     CLOB;
		l_mapName     inv_xmldocs.NAME%TYPE;
		l_targetClob  CLOB;
		l_targetDoc   DBMS_xmldom.DOMDocument;
		l_targetNode  DBMS_xmldom.DOMNode;
		l_isDaughter  BOOLEAN := FALSE;
		l_addSolvent  BOOLEAN := TRUE;
		l_newPlateIDs VARCHAR2(2000);
	BEGIN
		--' Prep the input values
		l_barcodeList_t := STRINGUTILS.split(p_barcodeList, ',');
		IF p_solventVolume IS NULL THEN
			l_addSolvent := FALSE;
		END IF;

		--' Get the target plate formats
		l_plateFormatIDList_t := STRINGUTILS.split(GETPLATEFORMATS(p_reformatMapID,
																															 'target'),
																							 ',');

		--' Get reformat map info
		SELECT xmldoc, NAME
			INTO l_mapClob, l_mapName
			FROM inv_xmldocs
		 WHERE xmldoc_id = p_reformatMapID;

		--' check if this is a daughtering
		IF instr(lower(l_mapName), 'daughter') > 0 THEN
			l_isDaughter := TRUE;
		END IF;

		--Create the target plate xml
		l_targetDoc  := CreateTargetPlateXML(l_engine,
																				 l_parser,
																				 l_mapClob,
																				 l_isDaughter,
																				 p_sourcePlateIDList,
																				 l_barcodeList_t,
																				 p_locationID,
																				 p_plateTypeID,
																				 l_plateFormatIDList_t);
		l_targetNode := DBMS_xmldom.MakeNode(l_targetDoc);

		--create the target plates
		IF l_isDaughter THEN
			l_newPlateIDs := CreateDaughterPlates(l_targetDoc,
																						l_targetNode,
																						l_barcodeList_t);
		ELSE
			DBMS_LOB.CREATETEMPORARY(l_targetClob, FALSE, DBMS_LOB.CALL);
			DBMS_xmldom.writeToClob(l_targetNode, l_targetClob);
			l_newPlateIDs := createPlateXML(l_targetClob, FALSE);
		END IF;
		l_newPlateIDList_t := STRINGUTILS.split(l_newPlateIDs, ',');

		-- update well amounts plate by plate
		UpdateWellAmounts(l_newPlateIDList_t,
											l_isDaughter,
											l_addSolvent,
											p_amt,
											p_amtUnitID,
											p_solventID,
											p_solventVolume,
											p_solventVolumeUnitID);

		--Set the aggregate plate data
		platechem.SetAggregatedPlateData(TRIM(both ',' FROM l_newPlateIDs));
		platechem.SetAggregatedPlateData(p_sourcePlateIDList);

		--Clean Up
		DBMS_xmldom.freeDocument(l_targetDoc);
		IF NOT l_isDaughter THEN
			dbms_lob.freetemporary(l_targetClob);
		END IF;
		DBMS_xmlparser.freeParser(l_parser);
		DBMS_xslprocessor.freeProcessor(l_engine);

		--Return  new plate IDs
		RETURN TRIM(both ',' FROM l_newPlateIDs);

	EXCEPTION
		WHEN OTHERS THEN
			DBMS_xmldom.freeDocument(l_targetDoc);
			DBMS_xmlparser.freeParser(l_parser);
			DBMS_xslprocessor.freeProcessor(l_engine);
			RETURN 'Reformat Error: ' || SQLCODE || ':' || SQLERRM;

	END ReformatPlates;

	FUNCTION InsertReformatMap(p_mapXML CLOB, p_name inv_xmldocs.NAME%TYPE)
		RETURN inv_xmldocs.xmldoc_id%TYPE IS
		l_docTypeID inv_xmldoc_types.xmldoc_type_id%TYPE;
		l_docID     inv_xmldocs.xmldoc_id%TYPE;
	BEGIN
		SELECT xmldoc_type_id
			INTO l_docTypeID
			FROM inv_xmldoc_types
		 WHERE lower(type_name) = 'reformat map';
		INSERT INTO inv_xmldocs
			(xmldoc, NAME, xmldoc_type_id_fk)
		VALUES
			(p_mapXML, p_name, l_docTypeID)
		RETURNING xmldoc_ID INTO l_docID;
		RETURN l_docID;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN - 1;

	END InsertReformatMap;

	FUNCTION DeleteReformatMap(p_xmlDocID inv_xmldocs.xmldoc_id%TYPE)
		RETURN inv_xmldocs.xmldoc_id%TYPE IS
	BEGIN

		DELETE inv_xmldocs WHERE xmldoc_id = p_xmlDocID;
		RETURN p_xmlDocID;

	EXCEPTION
		WHEN OTHERS THEN
			RETURN - 1;

	END DeleteReformatMap;

	FUNCTION CreateDaughteringMap(p_plateFormatID inv_plate_format.plate_format_id%TYPE)
		RETURN inv_xmldocs.xmldoc_id%TYPE IS
		l_engine DBMS_xslprocessor.Processor := DBMS_xslprocessor.newProcessor;
		l_parser DBMS_xmlparser.Parser := DBMS_xmlparser.newParser;

		l_physPlateID  inv_physical_plate.phys_plate_id%TYPE;
		l_gridFormatID inv_grid_format.grid_format_id%TYPE;
		l_query        VARCHAR2(1000);
		l_queryClob    CLOB;
		l_queryDoc     DBMS_xmldom.DOMDocument;
		l_xsltClob     CLOB;
		l_targetClob   CLOB;
		l_targetNode   DBMS_xmldom.DOMNode;
		l_mapNode      DBMS_xmldom.DOMNode;
		l_mapClob      CLOB;

		l_xsltDoc     DBMS_xmldom.DOMDocument;
		l_xslt        DBMS_xslprocessor.Stylesheet;
		l_newXMLDocID inv_xmldocs.xmldoc_id%TYPE;
	BEGIN

		SELECT phys_plate_id_fk
			INTO l_physPlateID
			FROM inv_plate_format
		 WHERE plate_format_id = p_plateFormatID;
		SELECT grid_format_id_fk
			INTO l_gridFormatID
			FROM inv_physical_plate
		 WHERE phys_plate_id = l_physPlateID;
		l_query     := 'SELECT grid_position_id, grid_format_id_fk, row_index, col_index, ' ||
									 p_plateFormatID || ' AS plate_format_id, row_count, col_count
			FROM inv_grid_position g, inv_grid_format gf
			WHERE grid_format_id = ' || l_gridFormatID || '
				AND grid_format_id = grid_format_id_fk
			ORDER BY row_index, col_index';
		l_queryClob := DBMS_XMLQuery.getXML(l_query);

		DBMS_xmlparser.parseClob(l_parser, l_queryClob);
		l_queryDoc := DBMS_xmlparser.getDocument(l_parser);
		SELECT xslt
			INTO l_xsltClob
			FROM inv_xslts
		 WHERE xslt_name = 'Create Daugtering Map';
		DBMS_LOB.CREATETEMPORARY(l_targetClob, FALSE, DBMS_LOB.CALL);
		DBMS_xmlparser.parseClob(l_parser, l_xsltClob);
		l_xsltDoc := DBMS_xmlparser.getDocument(l_parser);
		l_xslt    := DBMS_xslprocessor.newStyleSheet(l_xsltDoc, NULL);
		DBMS_xslprocessor.processXSL(l_engine, l_xslt, l_queryDoc, l_targetClob);

		--get query XML without the <?xml?> tag
		l_targetNode := XMLUTILS.CLOB2NODE(l_parser, l_targetClob);
		l_mapNode    := DBMS_xmldom.getLastChild(l_targetNode);
		DBMS_LOB.CREATETEMPORARY(l_mapClob, FALSE, DBMS_LOB.CALL);
		DBMS_xmldom.writeToClob(l_mapNode, l_mapClob);

		INSERT INTO inv_xmldocs
			(xmldoc, NAME, xmldoc_type_id_fk)
		VALUES
			(l_mapClob, 'Daughter' || p_plateFormatID, 1)
		RETURNING xmldoc_id INTO l_newXMLDocID;
		
		DBMS_xmldom.freeDocument(l_xsltDoc);
		dbms_lob.freetemporary(l_targetClob);
		dbms_lob.freetemporary(l_mapClob);
		DBMS_xmlparser.freeParser(l_parser);
		DBMS_xslprocessor.freeProcessor(l_engine);

		RETURN l_newXMLDocID;
	END CreateDaughteringMap;

	FUNCTION DryPlate(p_plateIDList VARCHAR2) RETURN VARCHAR2 IS
		l_plateIDList_t STRINGUTILS.t_char;

	BEGIN
		l_plateIDList_t := STRINGUTILS.split(p_plateIDList, ',');
		--update plates
		FORALL i IN l_plateIDList_t.FIRST .. l_plateIDList_t.LAST
			UPDATE inv_plates
				 SET solvent_id_fk  = NULL,
						 solvent_volume = 0,
						 concentration  = NULL,
						 conc_unit_fk   = NULL
			 WHERE plate_id = l_plateIDList_t(i);

		--update wells
		FORALL i IN l_plateIDList_t.FIRST .. l_plateIDList_t.LAST
			UPDATE inv_wells
				 SET solvent_id_fk  = NULL,
						 solvent_volume = 0,
						 concentration  = 0,
						 conc_unit_fk   = 0
			 WHERE plate_id_fk = l_plateIDList_t(i)
						 AND well_format_id_fk <> 2;

		RETURN '1';

	EXCEPTION
		WHEN OTHERS THEN
			RETURN '-1';

	END DryPlate;

END REFORMAT;
/
show errors;