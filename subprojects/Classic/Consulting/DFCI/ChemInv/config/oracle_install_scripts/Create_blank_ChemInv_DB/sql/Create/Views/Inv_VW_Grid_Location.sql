create or replace view inv_vw_grid_location (
	location_id,
	parent_id,
	description,
	location_type_id_fk,
	location_name,
	location_description,
	location_barcode,
	plate_id_fk,
	plate_barcode,
	row_index,
	col_index,
	row_name,
	col_name,
	name,
	sort_order
) as select
	inv_locations.location_id,
	inv_locations.parent_id,
	inv_locations.description,
	inv_locations.location_type_id_fk,
	inv_locations.location_name,
	inv_locations.location_description,
	inv_locations.location_barcode,
	inv_grid_element.plate_id_fk,
	inv_vw_plate.plate_barcode,
	inv_grid_position.row_index,
	inv_grid_position.col_index,
	inv_grid_position.row_name,
	inv_grid_position.col_name,
	inv_grid_position.name,
	inv_grid_position.sort_order
from inv_locations, inv_grid_position, inv_grid_element, inv_vw_plate
where inv_locations.location_id = inv_grid_element.location_id_fk
and inv_grid_element.grid_position_id_fk = inv_grid_position.grid_position_id
and inv_grid_element.plate_id_fk = inv_vw_plate.plate_id(+)
order by inv_locations.parent_id, inv_grid_position.sort_order;
