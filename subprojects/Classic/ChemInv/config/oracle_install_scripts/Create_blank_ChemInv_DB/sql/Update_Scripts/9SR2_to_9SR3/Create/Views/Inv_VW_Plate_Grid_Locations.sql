create or replace view inv_vw_plate_grid_locations (
	location_id,
	location_name,
	location_description,
	location_barcode,
	row_count,
	col_count,
	plate_type_id,
	plate_type_name
) as select
	inv_vw_grid_location_parent.*,
	inv_allowed_ptypes.plate_type_id_fk,
	inv_plate_types.plate_type_name
from inv_vw_grid_location_parent, inv_allowed_ptypes, inv_plate_types
where inv_allowed_ptypes.location_id_fk = inv_vw_grid_location_parent.location_id
and inv_allowed_ptypes.plate_type_id_fk = inv_plate_types.plate_type_id;
