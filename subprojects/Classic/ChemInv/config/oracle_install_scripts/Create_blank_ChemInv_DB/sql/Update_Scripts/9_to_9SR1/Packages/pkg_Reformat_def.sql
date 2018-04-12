CREATE OR REPLACE PACKAGE "REFORMAT"
AS
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
    
	RETURN varchar2;

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
  RETURN xmldom.DOMDocument;

  FUNCTION CreateTargetWellData (
  	pSourceWell platechem.chemdata,
    pAmt NUMBER,
    pAmtUnitID inv_units.unit_id%TYPE,
    pAmtUnitType inv_unit_types.unit_type_id%TYPE)
  RETURN PLATECHEM.chemdata;

	PROCEDURE UpdateWellAmounts(
  pNewPlateIDList_t STRINGUTILS.t_char,
  pIsDaughter BOOLEAN,
  pAddSolvent BOOLEAN,
	pAmt NUMBER,
	pAmtUnitID inv_units.unit_id%TYPE,
	pSolventID inv_solvents.solvent_id%TYPE,
	pSolventVolume inv_plates.solvent_volume%TYPE,
	pSolventVolumeUnitID inv_plates.solvent_volume_unit_id_fk%TYPE); 

	FUNCTION GetPlateFormats(
		pReformatMapID inv_xmldocs.xmldoc_id%TYPE,
		pPlateType varchar2)
	RETURN varchar2;

	FUNCTION CheckPlateQuantity(
		pSourcePlateIDs VARCHAR2)
	RETURN VARCHAR2;

	FUNCTION CheckValidSourcePlates(
		pSourcePlateIDs varchar2,
		pReformatMapID inv_xmldocs.xmldoc_id%TYPE)
	RETURN varchar2;

	FUNCTION GetValidReformatMaps(
		pSourcePlateIDs varchar2)
	RETURN varchar2;

	FUNCTION GetNumTargetPlates(
		pReformatMapID inv_xmldocs.xmldoc_id%TYPE)
	RETURN number;

	FUNCTION GetDaughterReformatMaps(
  	pSourcePlateIDs varchar2)
  RETURN varchar2;

	FUNCTION InsertReformatMap(
  	pMapXML CLOB,
    pName inv_xmldocs.name%TYPE)
  RETURN inv_xmldocs.xmldoc_id%TYPE;

  FUNCTION DeleteReformatMap(
  	pXmlDocID inv_xmldocs.xmldoc_id%TYPE)
	RETURN inv_xmldocs.xmldoc_id%TYPE;
	FUNCTION GetPlateXML(
  	pPlateIDs varchar2)
  RETURN CLOB;

	FUNCTION CreateDaughteringMap(
  	pPlateFormatID inv_plate_format.plate_format_id%TYPE)
  RETURN inv_xmldocs.xmldoc_id%TYPE;

	FUNCTION DryPlate(
  	pPlateIDList varchar2)
  RETURN varchar2;
END REFORMAT;
/
show errors;