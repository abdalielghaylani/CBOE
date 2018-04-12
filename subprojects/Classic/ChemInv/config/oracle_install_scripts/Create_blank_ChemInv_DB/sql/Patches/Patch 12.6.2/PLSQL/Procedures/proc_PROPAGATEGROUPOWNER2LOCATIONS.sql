CREATE OR REPLACE PROCEDURE PROPAGATEGROUPOWNER2LOCATIONS(
    pLocationID in  CHEMINVDB2.inv_locations.location_id%type,
    pPrincipalID in COEDB.COEPRINCIPAL.PRINCIPAL_ID%type )
    AS
  cursor cur(location_id_in in CHEMINVDB2.inv_locations.location_id%type) is
  SELECT Location_Name, location_id, L.PRINCIPAL_ID_FK, parent_id
      FROM CHEMINVDB2.inv_Locations L
      CONNECT BY Parent_id  = prior  Location_id
      START WITH parent_id = location_id_in
      ORDER BY location_name ASC;
BEGIN
    FOR REC IN CUR(pLocationID) LOOP
        UPDATE CHEMINVDB2.INV_LOCATIONS L 
        SET PRINCIPAL_ID_FK=pPrincipalID
        WHERE L.LOCATION_ID=rec.location_id and L.PARENT_ID=rec.parent_id;
    END LOOP;
END PROPAGATEGROUPOWNER2LOCATIONS;
/
