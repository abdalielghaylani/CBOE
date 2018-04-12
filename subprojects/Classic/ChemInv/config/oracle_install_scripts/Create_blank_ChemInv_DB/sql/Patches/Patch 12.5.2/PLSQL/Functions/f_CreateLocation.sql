create or replace FUNCTION "CREATELOCATION"
(
 pBarcode IN Inv_Locations.Location_Barcode%Type,
 pBarcodeDescID IN inv_barcode_desc.barcode_desc_id%TYPE,
 pParentId IN Inv_Locations.Location_ID%Type,
 pName IN Inv_Locations.Location_Name%Type,
 pLocationTypeID in inv_Locations.Location_Type_ID_FK%Type,
 pDesc IN Inv_Locations.Location_Description%Type,
 pGridFormatID IN Inv_Grid_Format.Grid_Format_ID%Type:=Null,
 pAllowedPlateTypeList in varchar2:=Null,
 pLocationOwner in inv_locations.Owner_ID_FK%type,
 p_CollapseChildNodes in inv_locations.collapse_child_nodes%type,
 pPrincipal_ID_FK in inv_locations.Principal_ID_Fk%type:=Null,
 pIspublic in inv_locations.IsPublic%Type:=Null
 )
return Inv_Locations.Location_ID%Type
IS
NewLocationID Inv_Locations.Location_ID%Type;
duplicate_barcode exception;
duplicate_name exception;
invalid_grid_format exception;
CURSOR dupBarcode_cur(Barcode_in in Inv_Locations.Location_Barcode%Type) IS
  SELECT Location_ID FROM Inv_Locations WHERE inv_Locations.Location_Barcode = Barcode_in;
CURSOR dupName_cur(LocationName_in in Inv_Locations.Location_Name%Type) IS
  SELECT Location_ID FROM Inv_Locations WHERE UPPER(inv_Locations.Location_Name) = UPPER(LocationName_in) AND inv_Locations.Parent_ID = pParentID;
dupBarcode_id Inv_Locations.Location_ID%Type;
dupName_id Inv_Locations.Location_ID%Type;
GridStorageID inv_grid_storage.grid_storage_id%type;
lBarcode inv_locations.location_barcode%TYPE;
rc integer;
lPrincipal_ID_FK inv_locations.Principal_ID_Fk%type;
lLocationTypeID inv_Locations.Location_Type_ID_FK%Type;
lCount integer;
BEGIN

-- Check for duplicate barcode
if pBarcode is not null then
  OPEN dupBarcode_cur(pBarcode);
  FETCH dupBarcode_cur into dupBarcode_id;
  if dupBarcode_cur%FOUND then
    RAISE duplicate_barcode;
  end if;
  CLOSE dupBarcode_cur;
  lBarcode := pBarcode;
--* if barcode description id given then get the next barcode
ELSIF pBarcodeDescID IS NOT NULL THEN
	lBarcode := barcodes.GetNextBarcode(pBarcodeDescID);
end if;

-- Check for duplicate name within this location
OPEN dupName_cur(pName);
FETCH dupName_cur into dupName_id;
if (dupName_cur%FOUND) then
  RAISE duplicate_name;
end if;
CLOSE dupName_cur;


lPrincipal_ID_FK:=pPrincipal_ID_FK;
if lPrincipal_ID_FK=0 then
     select Principal_ID_FK into lPrincipal_ID_FK from inv_locations where location_ID=pParentID;    
end if;

  if pIspublic=1 then
  	select location_type_id into lLocationTypeID from inv_location_types where upper(location_type_name)='PUBLIC' ;
  else
  	lLocationTypeID:=pLocationTypeID;
  end if;

INSERT INTO Inv_Locations
(Location_Barcode,
 Parent_ID,
 Location_Name,
 Location_Type_ID_FK,
 Location_Description,
 Owner_ID_FK,
 Collapse_Child_Nodes,
 Principal_ID_FK,
 ispublic)
VALUES
(lBarcode,
 pParentID,
 pName,
 pLocationTypeID,
 pDesc,
 pLocationOwner,
 p_CollapseChildNodes,
 lPrincipal_ID_FK, 
 pIspublic)
 RETURNING Location_ID into NewLocationID;

If pGridFormatID IS NOT NULL then
  GridStorageID := EnableGridForLocation(NewLocationID, pGridFormatID);
  FOR GridElement_rec in (SELECT location_id_fk FROM inv_grid_element WHERE grid_storage_id_fk = GridStorageID)
  LOOP
  	rc := ASSIGNPLATETYPESTOLOCATION(GridElement_rec.location_id_fk, pAllowedPlateTypeList);
  END LOOP;
End if;

If pAllowedPlateTypeList IS NOT NULL then
  rc := ASSIGNPLATETYPESTOLOCATION(NewLocationID, pAllowedPlateTypeList);
End if;

RETURN NewLocationID;
exception
WHEN duplicate_barcode then
	--RETURN 'A location with same barcode ID already exists:' || to_Char(dupBarcode_id);
  RETURN -105;
WHEN duplicate_name then
	--RETURN 'A location with same name already exists at this location:' || to_Char(dupName_id);
  RETURN -106;
END CreateLocation;

/
show errors;
