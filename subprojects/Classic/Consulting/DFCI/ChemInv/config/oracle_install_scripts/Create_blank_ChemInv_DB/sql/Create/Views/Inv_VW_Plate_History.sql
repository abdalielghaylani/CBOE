CREATE OR REPLACE VIEW inv_vw_plate_history (
	plate_history_id,
	plate_id_fk,
	plate_barcode,
	plate_history_date,
	plate_action_name,
	from_location_name,
	to_location_name,
	is_ft_incremented
) AS SELECT
	INV_PLATE_HISTORY.plate_history_id,
	INV_PLATE_HISTORY.plate_id_fk,
	inv_vw_plate.plate_barcode,
	INV_PLATE_HISTORY.plate_history_date,
	INV_PLATE_ACTIONS.plate_action_name,
	FROM_LOCATIONS.location_name AS from_location_name,
	TO_LOCATIONS.location_name AS to_location_name,
	INV_PLATE_HISTORY.is_ft_incremented
FROM INV_PLATE_HISTORY, inv_vw_plate, INV_PLATE_ACTIONS, INV_LOCATIONS FROM_LOCATIONS, INV_LOCATIONS TO_LOCATIONS
WHERE INV_PLATE_HISTORY.plate_id_fk = inv_vw_plate.plate_id
AND INV_PLATE_HISTORY.plate_action_id_fk = INV_PLATE_ACTIONS.plate_action_id
AND INV_PLATE_HISTORY.from_location_id_fk = FROM_LOCATIONS.location_id(+)
AND INV_PLATE_HISTORY.to_location_id_fk = TO_LOCATIONS.location_id(+);
