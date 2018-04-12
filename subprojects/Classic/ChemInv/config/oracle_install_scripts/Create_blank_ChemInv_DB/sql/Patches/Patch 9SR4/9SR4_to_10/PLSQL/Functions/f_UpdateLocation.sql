CREATE OR REPLACE  FUNCTION "&&SchemaName"."UPDATELOCATION"                
(
 pBarcode IN Inv_Locations.Location_Barcode%Type,
 pLocationId IN Inv_Locations.Location_ID%Type,
 pParentId in inv_locations.parent_id%type,
 pName IN Inv_Locations.Location_Name%Type,
 pLocationTypeID in inv_Locations.Location_Type_ID_FK%Type,
 pDesc IN Inv_Locations.Location_Description%Type,
 pGridFormatID IN Inv_Grid_Format.Grid_Format_ID%Type:=0,
 pAllowedPlateTypeList in varchar2:=Null,
 pLocationOwner in inv_Locations.owner_id_fk%type,
 pPropagateAddress IN NUMBER
 )
return Inv_Locations.Location_ID%Type
IS
duplicate_barcode exception;
duplicate_name exception;
cannot_update_system_location exception;
CURSOR dupBarcode_cur(Barcode_in in Inv_Locations.Location_Barcode%Type) IS
  SELECT Location_ID FROM Inv_Locations WHERE inv_Locations.Location_Barcode = Barcode_in;
CURSOR dupName_cur(LocationName_in in Inv_Locations.Location_Name%Type) IS
  SELECT Location_ID FROM Inv_Locations WHERE Location_Name = LocationName_in
  AND Parent_ID IN
    (SELECT Parent_ID FROM inv_locations WHERE Location_ID = pLocationID);
dupBarcode_id Inv_Locations.Location_ID%Type;
dupName_id Inv_Locations.Location_ID%Type;
GridStorageID inv_grid_storage.grid_storage_id%type;
GridFormatID inv_grid_format.grid_format_id%type;
vAddressID inv_address.address_id%TYPE;
l_parentID inv_locations.parent_id%type;
rc integer;
BEGIN
  if pLocationID < 500 then
    RAISE cannot_update_system_location;
  end if;
-- Check for duplicate barcode
if pBarcode is not null then
  OPEN dupBarcode_cur(pBarcode);
  FETCH dupBarcode_cur into dupBarcode_id;
 if (dupBarcode_cur%FOUND) AND (NOT dupBarcode_id = pLocationID) then
    RAISE duplicate_barcode;
  end if;
  CLOSE dupBarcode_cur;
end if;
-- Assign parent_id correctly
if pParentId is null then
    select parent_id into l_parentID from inv_locations where location_id = pLocationId;
else
    l_parentID := pParentId;
end if;

-- Check for duplicate name within this location
OPEN dupName_cur(pName);
FETCH dupName_cur into dupName_id;
if (dupName_cur%FOUND) AND (NOT dupName_id = pLocationID) then
  RAISE duplicate_name;
end if;
CLOSE dupName_cur;

UPDATE Inv_Locations
SET
  Location_Barcode= pBarcode,
  Location_Name = pName,
  Parent_ID = l_parentID,
  Location_Description = pDesc,
  Location_Type_ID_FK = pLocationTypeID,
  Owner_ID_FK = pLocationOwner
WHERE
  Location_ID = pLocationID;

GridStorageID := GetGridStorageID(Null, pLocationID);
GridFormatID := GetGridFormatID(pLocationID);

if NVL(pGridFormatID, 0) <> GridFormatID then
  rc := DELETELOCATIONGRID(GridStorageID);
  If pGridFormatID IS NOT NULL then
  	GridStorageID := EnableGridForLocation(pLocationID, pGridFormatID);
  end if;	
End if;


If pAllowedPlateTypeList IS NOT NULL then
  FOR GridElement_rec in (SELECT location_id_fk FROM inv_grid_element WHERE grid_storage_id_fk = GridStorageID)
  LOOP
  	rc := ASSIGNPLATETYPESTOLOCATION(GridElement_rec.location_id_fk, pAllowedPlateTypeList);
  END LOOP;
  rc := ASSIGNPLATETYPESTOLOCATION(pLocationID, pAllowedPlateTypeList);
End if;

-- propagate address to children with no address if specified
IF pPropagateAddress = 1 THEN
	SELECT address_id_fk INTO vAddressID FROM inv_locations WHERE location_id = pLocationID;
  IF vAddressID IS NOT NULL THEN 
		UPDATE inv_locations SET address_id_fk = vAddressID
    	WHERE 
      	location_id IN (
      		SELECT location_id FROM inv_locations 
        		START WITH location_id = pLocationId
          	CONNECT BY PRIOR location_id = parent_id)
         AND address_id_fk IS NULL;
  END IF;
END IF;

RETURN pLocationID;
exception
WHEN duplicate_barcode then
  RETURN -105;
WHEN duplicate_name then
  RETURN -106;
WHEN cannot_update_system_location then
  RETURN -110;
END UpdateLocation;
/
show errors;
