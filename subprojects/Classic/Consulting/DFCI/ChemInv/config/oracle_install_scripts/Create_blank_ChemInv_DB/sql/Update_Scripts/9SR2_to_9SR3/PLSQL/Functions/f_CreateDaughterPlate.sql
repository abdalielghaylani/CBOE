CREATE OR REPLACE
function
"CREATEDAUGHTERPLATE"
	(p_parent_plate_id in inv_wells.plate_id_fk%type,
	 p_daughter_plate_id in inv_wells.plate_id_fk%type,
	 p_amount_taken in inv_wells.qty_remaining%type,
	 p_solvent_added in inv_wells.qty_remaining%type,
	 p_solvent in inv_wells.solvent%type,
	 p_date_performed in inv_plate_history.plate_history_date%type)

	 return inv_wells.plate_id_fk%type

is

parent_group_id inv_plates.group_id_fk%type;
new_qty_initial inv_wells.qty_initial%type;
cursor parent_wells is
	select well_id, well_compound_id_fk, compound_id_fk, grid_position_id, qty_remaining from inv_vw_well where plate_id_fk = p_parent_plate_id;

begin
	-- get old group id
	select group_id_fk into parent_group_id from inv_plates where plate_id = p_parent_plate_id;

	-- determine amount of liquid in new well
	new_qty_initial := p_amount_taken + p_solvent_added;

	-- update parent plate
	update inv_plates set qty_remaining = qty_remaining - p_amount_taken
		where plate_id = p_parent_plate_id;

	-- update daughter plate
	update inv_plates set group_id_fk = parent_group_id,
						  qty_initial = new_qty_initial,
						  qty_remaining = new_qty_initial
				where plate_id = p_daughter_plate_id;

	-- loop over parent wells, insert values into daughter wells

	for parent_wells_cur in parent_wells loop

		-- update daughter wells - set compound id and update amount
		update inv_wells
			set compound_id_fk = parent_wells_cur.compound_id_fk,
			qty_initial = new_qty_initial,
			qty_remaining = new_qty_initial,
			solvent = p_solvent,
			parent_well_id_fk = parent_wells_cur.well_id,
			well_compound_id_fk = parent_wells_cur.well_compound_id_fk
		where plate_id_fk = p_daughter_plate_Id
		and grid_position_id_fk = parent_wells_cur.grid_position_id;

		-- update parent wells - subtract amount taken
		update inv_wells
			set qty_remaining = parent_wells_cur.qty_remaining - p_amount_taken
		where plate_id_fk = p_parent_plate_id
		and grid_position_id_fk = parent_wells_cur.grid_position_id;

	end loop;

	-- update history
	-- 4 = liquid taken
	insert into inv_plate_history
	(plate_id_fk, plate_history_date, plate_action_id_fk, is_ft_incremented)
	values
	(p_parent_plate_id, p_date_performed, 4, 0);


	return p_daughter_plate_id;

end CreateDaughterPlate;
/
