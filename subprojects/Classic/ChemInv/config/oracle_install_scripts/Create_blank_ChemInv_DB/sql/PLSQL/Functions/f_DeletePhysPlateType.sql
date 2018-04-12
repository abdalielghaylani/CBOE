-- DeletePhysicalPlateType
CREATE OR REPLACE FUNCTION

"DELETEPHYSPLATETYPE"
	(pPhysPlateId IN inv_physical_plate.phys_plate_id%Type)

RETURN inv_physical_plate.phys_plate_id%Type

IS

	gridFormatId NUMBER;

BEGIN
	select grid_format_id_fk into gridFormatId from inv_physical_plate where phys_plate_id = pPhysPlateId;

	-- delete from inv_grid_format - this will cascade delete the physical plate as well
	delete from inv_grid_format
	where
	grid_format_id = gridFormatId;


RETURN pPhysPlateId;

END DeletePhysPlateType;
/
show errors;