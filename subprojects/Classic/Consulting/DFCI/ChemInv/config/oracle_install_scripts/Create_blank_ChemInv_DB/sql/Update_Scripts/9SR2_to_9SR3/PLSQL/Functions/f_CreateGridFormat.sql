create or replace function
"CREATEGRIDFORMAT"
	(p_grid_format_type in inv_enumeration.enum_id%type,
	 p_row_count in inv_grid_format.row_count%type,
	 p_col_count in inv_grid_format.col_count%type,
	 p_Row_Prefix IN inv_grid_format.row_prefix%type,
	 p_Col_Prefix in inv_grid_format.col_prefix%type,
	 p_Row_Use_Letters in inv_grid_format.row_use_letters%type,
	 p_Col_Use_Letters in inv_grid_format.col_use_letters%type,
	 p_Name_Separator in inv_grid_format.name_separator%type,
	 p_Number_Start_Corner in inv_grid_format.number_start_corner%type,
	 p_Number_Direction in inv_grid_format.number_direction%type,
	 p_name in inv_grid_format.name%type,
	 p_description in inv_grid_format.description%type)

	 return inv_grid_format.grid_format_id%type

is

new_grid_format_id inv_grid_format.grid_format_id%type;
row_counter inv_grid_position.row_index%Type;
col_counter inv_grid_position.col_index%Type;
new_sort_order inv_grid_position.sort_order%type;
new_name inv_grid_position.name%type;
new_row_name inv_grid_position.row_name%type;
new_col_name inv_grid_position.col_name%type;

begin
	
	-- insert into grid format

	insert into inv_grid_format
		(name, description, row_count, col_count, 
		 row_prefix, col_prefix, row_use_letters, 
		 col_use_letters, name_separator, number_start_corner,
		 number_direction, grid_format_type_fk)
	values
		(p_name, p_description, p_row_count, p_col_count, 
		p_row_prefix, p_col_prefix, p_row_use_letters, 
		 p_col_use_letters, p_name_separator, p_number_start_corner,
		 p_number_direction, p_grid_format_type)
	returning grid_format_id into new_grid_format_id;

	-- insert new rows into inv_grid_position
	-- supported - Upper Left Corner, Row First or Column First
	-- supported - Prefixes
	-- supported - Name Separator
	-- supported - Use Letters or Numbers
	-- not supported - starting at any other corner

	for row_counter in 1..p_row_count loop
		for col_counter in 1..p_col_count loop
			-- add new record
			-- determine sort order
			if p_number_direction = 17 then
				-- if rows first
				new_sort_order := col_counter + ((row_counter-1) * p_col_count);
			else
				-- if columns first
				new_sort_order := row_counter + ((col_counter-1) * p_row_count);
			end if;
			
			-- determine row and column names
			if p_row_use_letters = 1 then
				new_row_name := p_row_prefix || chr(65 + row_counter - 1);
			else
				new_row_name := p_row_prefix || row_counter;
			end if;
			if p_col_use_letters = 1 then
				new_col_name := p_col_prefix || chr(65 + col_counter - 1);
			else
				new_col_name := p_col_prefix || col_counter;
			end if;	
			
			-- determine cell name
			if p_number_direction = 17 then
				new_name := new_row_name || p_name_separator || new_col_name;
			else
				new_name := new_col_name || p_name_separator || new_row_name;
			end if;
			
			-- add new record
			insert into inv_grid_position
			(grid_format_id_fk, row_index, col_index, row_name, col_name, sort_order, name)
			values
			(new_grid_format_id, row_counter, col_counter, new_row_name, new_col_name, new_sort_order, new_name);
		end loop;
	end loop;

	return new_grid_format_id;

end CreateGridFormat;
/ 
show errors;