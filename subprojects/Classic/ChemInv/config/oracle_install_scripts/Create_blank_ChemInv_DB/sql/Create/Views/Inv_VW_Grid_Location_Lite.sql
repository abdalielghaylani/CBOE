CREATE OR REPLACE VIEW INV_VW_GRID_LOCATION_LITE ( LOCATION_ID,
PARENT_ID ) AS select
	inv_locations.location_id,
	inv_locations.parent_id
from inv_locations, inv_grid_element
where inv_locations.location_id = inv_grid_element.location_id_fk;
