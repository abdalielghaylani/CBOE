CREATE OR REPLACE
VIEW INV_VW_PLATE_LOCATIONS ( LOCATION_ID, LOCATION_NAME, LOCATION_DESCRIPTION, LOCATION_BARCODE, LOCATION_TYPE_NAME ) AS
select
	inv_locations.location_id,
	inv_locations.location_name,
	inv_locations.location_description,
	inv_locations.location_barcode,
	inv_location_types.location_type_name
from inv_locations, inv_location_types where
	inv_locations.location_type_id_fk = inv_location_types.location_type_id(+)
	and inv_locations.location_id in
	(select distinct location_id_fk
	 from inv_allowed_ptypes) and location_id not in (select location_id_fk from inv_grid_element);
