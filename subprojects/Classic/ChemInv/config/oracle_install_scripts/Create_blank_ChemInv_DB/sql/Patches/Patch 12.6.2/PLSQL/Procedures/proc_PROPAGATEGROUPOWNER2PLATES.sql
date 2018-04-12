create or replace
PROCEDURE PROPAGATEGROUPOWNER2PLATES(
    pLocationID in  CHEMINVDB2.inv_locations.location_id%type )
    AS
  cursor cur(location_id_in in CHEMINVDB2.inv_locations.location_id%type) is
  SELECT Location_Name, location_id, L.PRINCIPAL_ID_FK, parent_id
      FROM CHEMINVDB2.inv_Locations L
      CONNECT BY Parent_id  = prior  Location_id
      START WITH parent_id = location_id_in
      ORDER BY location_name ASC;
BEGIN
    FOR REC IN CUR(pLocationID) LOOP
        UPDATE CHEMINVDB2.INV_PLATES P
        SET PRINCIPAL_ID_FK=rec.principal_id_FK
        WHERE P.LOCATION_ID_FK=rec.location_id;
    END LOOP;
END PROPAGATEGROUPOWNER2PLATES;
/
