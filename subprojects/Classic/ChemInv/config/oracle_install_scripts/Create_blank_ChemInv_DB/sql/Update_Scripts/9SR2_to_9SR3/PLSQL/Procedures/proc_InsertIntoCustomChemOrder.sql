CREATE OR REPLACE Procedure "&&SchemaName"."InsertIntoCustomChemOrder"
  (pContainerID IN inv_containers.container_id%TYPE,
   pNumBottles IN INT)
IS
  vOrdLine PLS_INTEGER;
  vSite PLS_INTEGER;
BEGIN
  -- get the next ord_line value.
  SELECT Custom_CHEM_ORD_LINE_SEQ.NEXTVAL
  INTO   vOrdLine
  FROM   dual;

  --SELECT TO_NUMBER(location_description)
  --INTO   vSite
  --FROM   inv_locations
  --WHERE  LEVEL = (SELECT MAX(LEVEL) - 1
  --                FROM   inv_locations
  --                START WITH location_id = (SELECT delivery_location_id_fk FROM inv_container_order WHERE container_id = pContainerID)
  --                CONNECT BY location_id = PRIOR parent_id)
  --START WITH location_id = (SELECT delivery_location_id_fk FROM inv_container_order WHERE container_id = pContainerID)
  --CONNECT BY location_id = PRIOR parent_id;

  vsite := 0;
  -- create the order
  INSERT INTO Custom_chem_order
    (ord_line, site,
     due_date, rush,
     catalog_no, catalog_cost,
     num_bottles, amt_bottle, uom,
     scientist,
     vendor_no, comments,
     project_no, registered, item_no, name
     )
  SELECT vOrdLine, vSite,
         TO_CHAR(inv_Container_Order.due_Date, 'MM/DD/YYYY'),
         inv_Container_Order.isRushOrder,
         SUBSTR(inv_containers.supplier_catnum, 1, 32), NVL(inv_containers.container_cost, 0),
         pNumBottles, inv_containers.qty_initial, SUBSTR(inv_units.unit_abreviation, 1, 5),
         SUBSTR(inv_containers.Owner_ID_FK, 1, 20),
         NVL(Custom_ACX_ST_VENDORS.ST, 0), Inv_Containers.Container_Comments,
         inv_Container_Order.project_no, 'N', NVL(inv_compounds.alt_id_1, 0), inv_compounds.substance_name
  FROM   inv_containers, inv_Container_Order, inv_compounds, inv_units, inv_locations,
         Custom_ACX_ST_VENDORS
  WHERE  inv_containers.container_id = pContainerID AND
         inv_Container_Order.container_id = pContainerID AND
         inv_compounds.compound_id (+)= inv_containers.compound_id_fk AND
         inv_units.unit_id = inv_containers.unit_of_meas_id_fk AND
         inv_locations.location_id = inv_containers.location_id_fk AND
         Custom_ACX_ST_VENDORS.ACX (+) = inv_containers.supplier_id_fk;

  IF SQL%ROWCOUNT = 0 THEN
    RAISE_APPLICATION_ERROR(-20000, 'Cannot add item to Custom_chem_order.');
  END IF;

  -- set the back pointer from the inventory container.
  UPDATE inv_containers
  SET    req_number = vOrdLine
  WHERE  inv_containers.container_id IN (SELECT ID FROM TempIDs);

END "InsertIntoCustomChemOrder";
/
show errors;