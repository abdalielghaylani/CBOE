CREATE OR REPLACE function

"UPDATEWELLFORMAT"
	(pWellID IN inv_Wells.Well_id%type,
	 pWellFormatIdFK In inv_wells.well_format_id_fk%type,
	 pConcentration in float,
	 pConcUnitFK in inv_wells.conc_unit_fk%type)

	 return inv_wells.well_format_id_fk%type

is

begin

	update inv_wells
	set
	well_format_id_fk = pWellFormatIDFK,
	concentration = pConcentration,
	conc_unit_fk = pConcUnitFk
	where well_id = pWellID;

return pWellId;

end UpdateWellFormat;
/
show errors;