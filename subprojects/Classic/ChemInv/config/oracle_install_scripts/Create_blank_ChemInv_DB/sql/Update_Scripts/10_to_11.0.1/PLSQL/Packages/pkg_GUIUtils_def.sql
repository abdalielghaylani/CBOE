CREATE OR REPLACE PACKAGE "GUIUTILS" AS
	TYPE CURSOR_TYPE IS REF CURSOR;

	PROCEDURE GETKEYCONTAINERATTRIBUTES(pContainerIDList IN VARCHAR2 := NULL,
								 pContainerBarcodeList IN VARCHAR2 := NULL,
								 O_RS OUT CURSOR_TYPE);
	
	PROCEDURE GETCONTAINERATTRIBUTES(pContainerID IN inv_containers.container_id%TYPE := NULL,
								 pContainerBarcode IN inv_containers.barcode%TYPE := NULL,
								 pUseReg IN NUMBER,
								 O_RS OUT CURSOR_TYPE);

	PROCEDURE GETCONTAINERSUBSTANCE(pContainerIDList IN VARCHAR2 := NULL,
	pContainerBarcodeList IN VARCHAR2 := NULL,
	O_RS OUT CURSOR_TYPE);
	
	PROCEDURE SETCONTAINERSUBSTANCE(pContainerIDList IN VARCHAR2 := NULL,
								 pContainerBarcodeList IN VARCHAR2 := NULL,
								 pCompoundID IN inv_containers.compound_id_fk%TYPE);

	PROCEDURE GETKEYPLATEATTRIBUTES(pPlateIDList IN VARCHAR2 := NULL,
							  pPlateBarcodeList IN VARCHAR2 := NULL,
							  O_RS OUT CURSOR_TYPE);

	PROCEDURE DISPLAYCREATEDCONTAINERS(p_Containerlist IN VARCHAR2 := NULL,
								O_RS OUT CURSOR_TYPE);

	FUNCTION GETLOCATIONPATH(pLocationID IN inv_locations.location_id%TYPE)
		RETURN VARCHAR2;

	FUNCTION GETRACKLOCATIONPATH(pLocationID IN inv_locations.location_id%TYPE)
		RETURN VARCHAR2;

	FUNCTION GETFULLRACKLOCATIONPATH(pLocationID IN inv_locations.location_id%TYPE)
		RETURN VARCHAR2;

	PROCEDURE GETRECENTLOCATIONS(pContainerID IN inv_containers.container_id%TYPE,
						    pNumRows IN INTEGER := 5,
						    O_RS OUT CURSOR_TYPE);

	FUNCTION GETPARENTPLATEIDS(pPlateID inv_plates.plate_id%TYPE) RETURN VARCHAR2;

	FUNCTION GETPARENTPLATEBARCODES(pPlateID inv_plates.plate_id%TYPE)
		RETURN VARCHAR2;

	FUNCTION GETPARENTPLATELOCATIONIDS(pPlateID inv_plates.plate_id%TYPE)
		RETURN VARCHAR2;

	FUNCTION GETPARENTWELLIDS(pWellID inv_wells.well_id%TYPE) RETURN VARCHAR2;

	FUNCTION GETPARENTWELLLABELS(pWellID inv_wells.well_id%TYPE) RETURN VARCHAR2;

	FUNCTION GETPARENTWELLNAMES(pWellID inv_wells.well_id%TYPE) RETURN VARCHAR2;

	FUNCTION GETPARENTPLATEIDS2(pWellID inv_wells.well_id%TYPE) RETURN VARCHAR2;

	FUNCTION GETPARENTPLATELOCATIONIDS2(pWellID inv_wells.well_id%TYPE)
		RETURN VARCHAR2;

	FUNCTION GETPARENTPLATEBARCODES2(pWellID inv_wells.well_id%TYPE)
		RETURN VARCHAR2;

	/* Returns the sum of qty_available for all child containers of the given container */
	FUNCTION GETBATCHAMOUNTSTRING(pContainerID inv_containers.container_id%TYPE,
							p_DryWeightUOM VARCHAR2) RETURN VARCHAR2;
							
	FUNCTION GETBATCHUOMAMOUNTSTRING(pContainerID inv_containers.container_id%TYPE, pBatchID inv_container_batches.batch_id%TYPE) RETURN VARCHAR2;
	FUNCTION GETQTYAVAILABLE(pContainerID inv_containers.container_id%TYPE) RETURN NUMBER;

	FUNCTION IS_NUMBER(p VARCHAR2) RETURN NUMBER;
	FUNCTION IS_CONTAINER_AVAILABLE(pContainerID inv_containers.container_id%TYPE) RETURN NUMBER;

	PROCEDURE GetRootNodes(p_selectedID IN NUMBER,
					   p_assetType IN VARCHAR2,
					   O_RS OUT CURSOR_TYPE);

	PROCEDURE GetLineage(p_rootID IN NUMBER,
					 p_assetType IN VARCHAR2,
					 O_RS OUT CURSOR_TYPE);

	--FUNCTION GetLocationId(p_locationIds VARCHAR2)
--		RETURN inv_locations.location_id%TYPE;
	FUNCTION GetLocationId(p_locationIds VARCHAR2, p_containerId inv_containers.container_id%TYPE, p_plateId inv_plates.plate_id%TYPE, p_currLocationId inv_locations.location_id%TYPE)
		RETURN inv_locations.location_id%TYPE ;

	FUNCTION GetRefreshLocationID(p_locationId inv_locations.location_id%TYPE)
     RETURN inv_locations.location_id%TYPE;

	FUNCTION UseGetLocation(p_locationIds VARCHAR2)
  RETURN NUMBER;

	PROCEDURE GETLABELPRINTER (	pLabelPrinterID IN INV_LABEL_PRINTERS.LABEL_PRINTER_ID%TYPE, 
								O_RS OUT CURSOR_TYPE);

	PROCEDURE GETLABELPRINTERS(	pReportTypeID IN INV_LABEL_PRINTERS.REPORTTYPE_ID_FK%TYPE, O_RS OUT CURSOR_TYPE );
	PROCEDURE GETFKReference(   pColumnName IN  user_cons_columns.Column_name%TYPE, pTableName IN  user_cons_columns.Table_name%TYPE, pOwner IN  user_cons_columns.Owner%TYPE,  O_RS OUT CURSOR_TYPE  );



END GUIUTILS;
/
show errors;


