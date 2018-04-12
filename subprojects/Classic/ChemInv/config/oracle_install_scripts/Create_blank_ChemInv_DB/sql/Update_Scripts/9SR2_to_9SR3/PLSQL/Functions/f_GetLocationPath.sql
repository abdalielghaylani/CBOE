CREATE OR REPLACE FUNCTION "&&SchemaName"."GETLOCATIONPATH" 
	(pLocationID in inv_locations.location_id%type)
	return varchar2
IS	
  CURSOR LocationNodes_cur(LocationID_in in inv_locations.location_id%type) IS
    SELECT Location_Name 
    FROM inv_Locations 
    CONNECT BY Location_id = prior Parent_id 
    START WITH Location_id = LocationID_in 
    ORDER BY Level DESC;
  path_str varchar2(2000);
  locationName varchar2(200);  
BEGIN
  OPEN LocationNodes_cur(pLocationID);
  LOOP 
    FETCH LocationNodes_cur INTO locationName;
    EXIT WHEN LocationNodes_cur%NOTFOUND;
    path_str := path_str || locationName || '\';
  END LOOP;
  --dbms_output.put_line(path_str);
  RETURN path_str;
END GETLOCATIONPATH;
/
show errors;
