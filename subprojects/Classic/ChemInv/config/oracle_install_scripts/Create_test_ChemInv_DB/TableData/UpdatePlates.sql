-- Set the solvent information for these test plates
update 
(select iw.plate_id_fk, iw.solvent_id_fk, iw.solvent_volume, iw.solvent_volume_initial, iw.solvent_volume_unit_id_fk 
from inv_wells iw, inv_plates ip
where iw.plate_id_fk = ip.plate_id
and ip.plate_id in (1000,1004,1005,1006,1007,1008))
set solvent_id_fk = 1, solvent_volume = 100.0, solvent_volume_initial = 100.0, solvent_volume_unit_id_fk = 4;

-- Set the aggregated information at the plate level
call PLATECHEM.SetAggregatedPlateData('1000,1004,1005,1006,1007,1008');