CREATE OR REPLACE FUNCTION "CERTIFYCONTAINER"
  (pContainerID inv_containers.container_id%TYPE,
   pStatusIDFK inv_containers.container_status_id_fk%TYPE,
   pPurity inv_containers.purity%TYPE,
   pInterval number,
   pStorageConditions inv_containers.storage_conditions%TYPE,
   pHandlingProcedures inv_containers.handling_procedures%TYPE)
RETURN varchar2
IS

BEGIN

	UPDATE inv_containers SET
		container_status_id_fk = pStatusIDFK,
		purity = pPurity,
		date_certified = trunc(sysdate),
		date_expires = add_months(trunc(sysdate), pInterval),
    storage_conditions = pStorageConditions,
    handling_procedures = pHandlingProcedures
	WHERE container_id = pContainerID;

  RETURN '1';

exception
WHEN OTHERS then
	RETURN '0';
END CERTIFYCONTAINER;
/
show errors;