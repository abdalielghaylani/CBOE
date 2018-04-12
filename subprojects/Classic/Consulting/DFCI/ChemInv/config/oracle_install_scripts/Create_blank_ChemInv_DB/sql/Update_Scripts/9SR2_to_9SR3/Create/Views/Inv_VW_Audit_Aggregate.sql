CREATE OR REPLACE VIEW "INV_VW_AUDIT_AGGREGATE" (
    "RID","RAID","TABLE_NAME","ACTION","USER_NAME","TIMESTAMP") AS 
    	SELECT 	
		rid,
		0 raid,
		'INV_COMPOUNDS' table_name,
		'I' action,
		creator user_name,
		timestamp
	FROM inv_compounds
	UNION
	SELECT 
		rid,
		0 raid,	
		'INV_CONTAINERS' table_name,
		'I' action,
		creator user_name,
		timestamp
	FROM inv_containers
	UNION
	SELECT 
		rid,
		0 raid,
		'INV_URL' table_name,
		'I' action,
		creator user_name,
		timestamp
	FROM inv_url
	UNION
	SELECT 
		rid,
		0 raid,
		'INV_LOCATIONS' table_name,
		'I' action,
		creator user_name,
		timestamp
	FROM inv_locations
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_REQUESTS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_requests
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_SUPPLIERS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_suppliers
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_ORDERS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_orders
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_ORDER_CONTAINERS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_order_containers
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_CONTAINER_CHECKIN_DETAILS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_container_checkin_details
	UNION
  SELECT
  	rid,
    0 raid,
    'INV_SYNONYMS' table_name,
    'I' action,
    creator user_name,
    TIMESTAMP
  FROM inv_synonyms
  UNION
	SELECT 
		rid,
		raid as raid,
		table_name,
		action,
		user_name,
		timestamp
		FROM audit_row r WITH READ ONLY;