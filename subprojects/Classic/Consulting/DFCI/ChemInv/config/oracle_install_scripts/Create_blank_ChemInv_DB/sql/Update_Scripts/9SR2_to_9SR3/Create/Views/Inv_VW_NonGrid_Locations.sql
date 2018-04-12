create or replace view inv_vw_nongrid_locations (
	location_id,
	location_name,
	location_description
) as select
	inv_locations.location_id,
	inv_locations.location_name,
	inv_locations.location_description
from inv_locations, inv_grid_element
where inv_locations.location_id = inv_grid_element.location_id_fk(+)
and inv_grid_element.location_id_fk is null
and inv_locations.location_id > 999;
