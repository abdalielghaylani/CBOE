CREATE OR REPLACE
FUNCTION	"ISPLATETYPEALLOWED"(
	pPlateID in varchar2,
        pLocationID in varchar2,
	pPlateTypeID in inv_plate_types.plate_type_id%Type,
	pIsPlateMap in integer)
	return integer
is
rec_count integer;
vIsPlateMap integer;
vPlateTypeID integer;
vPlateId STRINGUTILS.t_char;
vLocationID STRINGUTILS.t_char;
BEGIN
  vLocationID := STRINGUTILS.split(pLocationID, ',');
   FOR j in vLocationID.First..vLocationID.Last
   LOOP
      	IF pPlateID is NULL THEN
              vPlateTypeID := pPlateTypeID;
              IF pIsPlateMap is NULL THEN
                  vIsPlateMap := 0;
              ELSE
                  vIsPlateMap := pIsPlateMap;
              END IF;
               IF vIsPlateMap = 1 THEN
                          SELECT count(*) INTO rec_count
                                  FROM inv_locations, inv_location_types
                                  WHERE
                                          location_id = vLocationID(j)
                                          AND location_type_id_fk = location_type_id
                                          AND location_type_name = 'Plate Map';
                     ELSE
                            SELECT count(*) INTO rec_count
                                            FROM  INV_ALLOWED_PTYPES
                                            WHERE
                                                    LOCATION_ID_FK = vLocationID(j)
                                                    AND PLATE_TYPE_ID_FK = pPlateTypeID				;
                     END IF;
                   IF rec_count = 0 THEN
                      return 0;
                   END IF;
        ELSE

              vPlateId := STRINGUTILS.split(pPlateID, ',');
             FOR i in vPlateId.First..vPlateId.Last
              LOOP
                    IF pIsPlateMap is NULL THEN -- Checking the plate map type
                          SELECT count(*) INTO rec_count  FROM Inv_Plates
                                      WHERE	IS_PLATE_MAP = 1  AND PLATE_ID = vPlateId(i);
                          IF rec_count=1 THEN
                              vIsPlateMap := 1;
                          ELSE
                              vIsPlateMap :=0;
                          END IF;
                    ELSE
                          vIsPlateMap := pIsPlateMap;
                    END IF;
                    -- checking Plate Type Id
                    IF  pPlateTypeID is NULL THEN
                          SELECT PLATE_TYPE_ID_FK INTO vPlateTypeID from Inv_Plates where PLATE_ID= vPlateId(i);
                    ELSE
                          vPlateTypeID := pPlateTypeID;
                    END IF;

                    IF vIsPlateMap = 1 THEN
                          SELECT count(*) INTO rec_count
                                  FROM inv_locations, inv_location_types
                                  WHERE
                                          location_id = vLocationID(j)
                                          AND location_type_id_fk = location_type_id
                                          AND location_type_name = 'Plate Map';
                     ELSE
                            SELECT count(*) INTO rec_count
                                            FROM  INV_ALLOWED_PTYPES
                                            WHERE
                                                    LOCATION_ID_FK = vLocationID(j)
                                                    AND PLATE_TYPE_ID_FK = vPlateTypeID				;
                     END IF;
                    IF rec_count = 0 THEN
                      return 0;
                    END IF;
              End Loop;
        END IF;
    End Loop;     
        return 1;
END;
/
