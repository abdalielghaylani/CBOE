create or replace view inv_vw_grid_location_parent (
	location_id,
	location_name,
	location_description,
	location_barcode,
	row_count,
	col_count
) as select
	inv_locations.location_id,
	inv_locations.location_name,
	inv_locations.location_description,
	inv_locations.location_barcode,
	inv_grid_format.row_count,
	inv_grid_format.col_count
from inv_locations, inv_grid_format, inv_grid_storage
where inv_locations.location_id = inv_grid_storage.location_id_fk
and inv_grid_storage.grid_format_id_fk = inv_grid_format.grid_format_id;
