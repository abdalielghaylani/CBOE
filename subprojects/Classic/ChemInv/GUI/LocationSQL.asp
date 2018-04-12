<%
schemaName = Application("ORASCHEMANAME")
dateFormatString = Application("DATE_FORMAT_STRING")
' SQL to get all attributes of a location
SQL = " select inv_locations.location_id, inv_locations.parent_id, inv_locations.description, inv_location_types.location_type_name, inv_locations.location_name, inv_locations.location_description, inv_locations.location_barcode as barcode, "
SQL = SQL & schemaName & ".GUIUTILS.GETLOCATIONPATH(inv_locations.location_id) as location_path, "
SQL = SQL & " inv_locations.owner_id_fk, inv_locations.creator, inv_address.contact_name, inv_address.address1, inv_address.address2, inv_address.address3, inv_address.address4, inv_address.city, inv_states.state_name, inv_states.state_abbreviation, "
SQL = SQL & " inv_country.country_name, inv_address.zip, inv_address.fax, inv_address.phone, inv_address.email "
SQL = SQL & " from inv_locations, inv_location_types, inv_address, inv_states, inv_country "
SQL = SQL & " where inv_locations.location_type_id_fk = inv_location_types.location_type_id "
SQL = SQL & " and inv_locations.address_id_fk = inv_address.address_id(+) "
SQL = SQL & " and inv_address.state_id_fk = inv_states.state_id(+) "
SQL = SQL & " and inv_address.country_id_fk = inv_country.country_id(+) "


%>
