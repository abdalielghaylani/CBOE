CREATE OR REPLACE
FUNCTION "EMPTYTRASH"
(pLocationID in inv_locations.location_id%type,
pTrashType IN varchar2)
return integer
IS
vNumContainers integer;
vNumPlates integer;
my_sql varchar2(2000);
currentLocation integer;
BEGIN                 
	IF lower(pTrashType) = 'containers' THEN
		SELECT count(*) INTO vNumContainers FROM inv_containers WHERE location_id_fk = pLocationID;
		IF vNumContainers > 0 THEN
			-- remove child container references
			UPDATE inv_containers SET parent_container_id_fk = null WHERE parent_container_id_fk IN (SELECT container_id FROM inv_containers WHERE location_id_fk = pLocationID);
    		-- delete the containers
			DELETE inv_containers WHERE location_id_fk = pLocationID;
		END IF;
	ELSIF lower(pTrashType) = 'plates' THEN
		SELECT count(*) INTO vNumContainers FROM inv_plates WHERE location_id_fk = pLocationID;
		IF vNumContainers > 0 THEN
			 --remove child plate references
			DELETE inv_plate_parent WHERE parent_plate_id_fk IN (SELECT plate_id FROM inv_plates WHERE location_id_fk = pLocationID);
    		-- delete the plates
			DELETE inv_plates WHERE location_id_fk = pLocationID;
		END IF;
	END IF;
    RETURN 1;
    
    exception
		WHEN others then
		  RETURN -1;
END EMPTYTRASH;
/
