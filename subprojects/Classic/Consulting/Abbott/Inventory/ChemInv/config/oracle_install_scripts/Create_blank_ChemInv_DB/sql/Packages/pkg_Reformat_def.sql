CREATE OR REPLACE PACKAGE "REFORMAT"
AS
	invalidAmtType exception;
  
	FUNCTION ReformatPlates(
		pSourcePlateIDList varchar2,
		pReformatMapID inv_xmldocs.xmldoc_id%TYPE,
		pBarcodeList varchar2,
		pPlateTypeIDList varchar2,
		pAmtList varchar2,
		pAmtUnitIDList varchar2,
		pAmtType varchar2,
		pSolventIDList varchar2,
		pSolventVolumeList varchar2,
		pSolventVolumeUnitIDList varchar2,
		pLocationIDList varchar2,
		pNumTargetPlates number,
		pTargetVolumeList varchar2,
		pTargetVolumeUnitIDList varchar2)
	RETURN varchar2;

  FUNCTION CreateTargetPlateXML(
  	pEngine xslprocessor.Processor,
  	pParser xmlparser.Parser,
    pMapClob inv_xmldocs.xmldoc%TYPE,
    pIsDaughter BOOLEAN,
  	pSourcePlateIDList VARCHAR2,  
   	pSourcePlates_t STRINGUTILS.t_char,
   	pLocationIDList_t STRINGUTILS.t_char,
    pBarcodeList_t STRINGUTILS.t_char,
		pPlateTypeIDList_t STRINGUTILS.t_char,
		pPlateFormatIDList_t STRINGUTILS.t_char)
  RETURN xmldom.DOMDocument;
  
  FUNCTION CreateTargetWellData (
  	pSourceWell platechem.chemdata,
  	pAmtType varchar2,
    pAmount NUMBER,
    pAmountUnitID inv_units.unit_id%TYPE,
    pTargetVolume NUMBER,
    pTargetVolumeUnitID inv_units.unit_id%TYPE)
  RETURN PLATECHEM.chemdata;

	PROCEDURE UpdateWellAmounts(
    pNewPlateIDList_t STRINGUTILS.t_char,
    pIsDaughter BOOLEAN,
    pAddSolvent BOOLEAN,
  	pAmtType varchar2,
  	pAmtList_t STRINGUTILS.t_char,
  	pAmtUnitIDList_t STRINGUTILS.t_char,
    pSolventIDList_t STRINGUTILS.t_char,
   	pSolventVolumeList_t STRINGUTILS.t_char,
  	pSolventVolumeUnitIDList_t STRINGUTILS.t_char,
    pTargetVolumeList_t STRINGUTILS.t_char,
    pTargetVolumeUnitIDList_t STRINGUTILS.t_char);
  
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