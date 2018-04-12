CREATE OR REPLACE  FUNCTION "&&SchemaName"."ASSIGNPLATETYPESTOLOCATION"    
	(pLocationID IN inv_locations.
     location_id%Type,
	 pPlateTypeList IN varchar2
	)
	RETURN integer
IS
PlateType_t STRINGUTILS.t_char;
BEGIN
  PlateType_t := STRINGUTILS.split(pPlateTypeList,',');
  DELETE FROM inv_allowed_ptypes WHERE Location_ID_FK = pLocationID;
  
  FOR i in PlateType_t.First..PlateType_t.Last
    Loop
        if PlateType_t(i) = 0 then
          DELETE FROM inv_allowed_ptypes WHERE Location_ID_FK = pLocationID;
          RETURN 1;
        end if;
        INSERT INTO inv_allowed_ptypes 
                    (Location_ID_FK,  Plate_Type_ID_FK)
             VALUES (pLocationID,     PlateType_t(i));                         
    End loop;
    
  RETURN 1;
END ASSIGNPLATETYPESTOLOCATION;
/
show errors;
