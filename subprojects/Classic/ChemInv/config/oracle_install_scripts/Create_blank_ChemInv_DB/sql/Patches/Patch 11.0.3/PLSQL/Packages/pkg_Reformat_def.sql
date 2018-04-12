create or replace PACKAGE "REFORMAT" AS
		FUNCTION ReformatPlates(p_sourcePlateIDList   VARCHAR2,
													p_reformatMapID       inv_xmldocs.xmldoc_id%TYPE,
													p_barcodeList         VARCHAR2,
													p_plateTypeID         inv_plates.plate_type_id_fk%TYPE,
													p_amt                 NUMBER,
													p_amtUnitID           inv_units.unit_id%TYPE,
													p_solventID           inv_solvents.solvent_id%TYPE,
													p_solventVolume       inv_plates.solvent_volume%TYPE,
													p_solventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE,
													p_locationID          inv_plates.location_id_fk%TYPE,
                                                    p_Ownership           inv_plates.Principal_ID_FK%TYPE:=NULL)
		RETURN VARCHAR2;

		FUNCTION CreateTargetPlateXML(p_engine              DBMS_xslprocessor.Processor,
																p_parser              DBMS_xmlparser.Parser,
																p_mapClob             inv_xmldocs.xmldoc%TYPE,
																p_isDaughter          BOOLEAN,
																p_sourcePlateIDList   VARCHAR2,
																p_barcodeList_t       STRINGUTILS.t_char,
																p_locationID          inv_plates.location_id_fk%TYPE,
																p_plateTypeID         inv_plates.plate_type_id_fk%TYPE,
																p_plateFormatIDList_t STRINGUTILS.t_char,
                                                                p_Ownership           inv_plates.Principal_ID_FK%TYPE:=NULL)
		RETURN DBMS_xmldom.DOMDocument;

	FUNCTION CreateTargetWellData(p_sourceWell  platechem.chemdata,
																p_amt         NUMBER,
																p_amtUnitID   inv_units.unit_id%TYPE,
																p_amtUnitType inv_unit_types.unit_type_id%TYPE)
		RETURN PLATECHEM.chemdata;

	PROCEDURE UpdateWellAmounts(p_newPlateIDList_t    STRINGUTILS.t_char,
															p_isDaughter          BOOLEAN,
															p_addSolvent          BOOLEAN,
															p_amt                 NUMBER,
															p_amtUnitID           inv_units.unit_id%TYPE,
															p_solventID           inv_solvents.solvent_id%TYPE,
															p_solventVolume       inv_plates.solvent_volume%TYPE,
															p_solventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE);

		FUNCTION GetPlateFormats(p_reformatMapID inv_xmldocs.xmldoc_id%TYPE,
													 p_plateType     VARCHAR2) RETURN VARCHAR2;

		FUNCTION CheckPlateQuantity(p_sourcePlateIDs VARCHAR2) RETURN VARCHAR2;

		FUNCTION CheckValidSourcePlates(p_sourcePlateIDs VARCHAR2,
																	p_reformatMapID  inv_xmldocs.xmldoc_id%TYPE)
		RETURN VARCHAR2;

		FUNCTION GetValidReformatMaps(p_sourcePlateIDs VARCHAR2) RETURN VARCHAR2;

			FUNCTION GetNumTargetPlates(p_reformatMapID inv_xmldocs.xmldoc_id%TYPE)
		RETURN NUMBER;

		FUNCTION GetDaughterReformatMaps(p_sourcePlateIDs VARCHAR2) RETURN VARCHAR2;

		FUNCTION InsertReformatMap(p_mapXML CLOB, p_name inv_xmldocs.NAME%TYPE)
		RETURN inv_xmldocs.xmldoc_id%TYPE;

		FUNCTION DeleteReformatMap(p_xmlDocID inv_xmldocs.xmldoc_id%TYPE)
		RETURN inv_xmldocs.xmldoc_id%TYPE;

		FUNCTION GetPlateXML(p_plateIDs VARCHAR2) RETURN CLOB;

	FUNCTION CreateDaughteringMap(p_plateFormatID inv_plate_format.plate_format_id%TYPE)
		RETURN inv_xmldocs.xmldoc_id%TYPE;

		FUNCTION DryPlate(p_plateIDList VARCHAR2) RETURN VARCHAR2;
END reformat;


/
show errors;