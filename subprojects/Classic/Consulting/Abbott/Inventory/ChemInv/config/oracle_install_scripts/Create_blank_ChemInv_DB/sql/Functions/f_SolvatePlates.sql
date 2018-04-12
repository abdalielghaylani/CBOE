CREATE OR REPLACE FUNCTION "SOLVATEPLATES" (
	pPlateIDList varchar2,
	pSolventIDList varchar2,
	pSolventVolumeAddedList varchar2,
	pSolventVolumeUnitIDList varchar2,
	pConcentrationList varchar2,
	pConcentrationUnitIDList varchar2)
	RETURN varchar2
IS
vPlateIDList_t STRINGUTILS.t_char;
vSolventIDList_t STRINGUTILS.t_char;
vSolventVolumeAddedList_t STRINGUTILS.t_char;
vSolventVolumeUnitIDList_t STRINGUTILS.t_char;
vConcentrationList_t STRINGUTILS.t_char;
vConcentrationUnitIDList_t STRINGUTILS.t_char;

BEGIN
	vPlateIDList_t := STRINGUTILS.split(pPlateIDList, ',');
	vSolventIDList_t := STRINGUTILS.split(pSolventIDList, ',');
	vSolventVolumeAddedList_t := STRINGUTILS.split(pSolventVolumeAddedList, ',');
	vSolventVolumeUnitIDList_t := STRINGUTILS.split(pSolventVolumeUnitIDList, ',');
	vConcentrationList_t := STRINGUTILS.split(pConcentrationList, ',');
	vConcentrationUnitIDList_t := STRINGUTILS.split(pConcentrationUnitIDList, ',');

	--update plates
	FORALL i in vPlateIDList_t.First..vPlateIDList_t.Last
		UPDATE inv_plates SET
			solvent_id_fk = vSolventIDList_t(i),
			solvent_volume = (vSolventVolumeAddedList_t(i) + chemcalcs.Convert(solvent_volume, solvent_volume_unit_id_fk, vSolventVolumeUnitIDList_t(i))),
			solvent_volume_unit_id_fk = vSolventVolumeUnitIDList_t(i),
			concentration = vConcentrationList_t(i),
			conc_unit_fk = vConcentrationUnitIDList_t(i)
		WHERE plate_id = vPlateIDList_t(i);

	--update wells
	FORALL i in vPlateIDList_t.First..vPlateIDList_t.Last
		UPDATE inv_wells SET
			solvent_id_fk = vSolventIDList_t(i),
			solvent_volume = (vSolventVolumeAddedList_t(i) + chemcalcs.Convert(solvent_volume, solvent_volume_unit_id_fk, vSolventVolumeUnitIDList_t(i))),
			solvent_volume_unit_id_fk = vSolventVolumeUnitIDList_t(i),
			concentration = vConcentrationList_t(i),
			conc_unit_fk = vConcentrationUnitIDList_t(i)
		WHERE plate_id_fk = vPlateIDList_t(i)
			AND well_format_id_fk <> 2;

	RETURN '1';

	EXCEPTION
		WHEN OTHERS THEN
			RETURN '-1';

END SOLVATEPLATES;
/
show errors;
