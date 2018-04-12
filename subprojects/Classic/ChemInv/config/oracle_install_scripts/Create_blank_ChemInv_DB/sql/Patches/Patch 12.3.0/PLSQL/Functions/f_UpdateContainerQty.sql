CREATE OR REPLACE FUNCTION "UPDATECONTAINERQTY"(pValuePairs VARCHAR2)
	RETURN number
  IS
	ValuePairs_t         STRINGUTILS.t_char;
	v_ContainerDetails_t     STRINGUTILS.t_char;
  v_QtyUnitID number;
  v_UpadtedQTY number;
BEGIN
  ValuePairs_t:= STRINGUTILS.split(pValuePairs, ',');
 	FOR i IN ValuePairs_t.FIRST .. ValuePairs_t.LAST
	LOOP
    v_ContainerDetails_t:= STRINGUTILS.split(ValuePairs_t(i), ':');
    select unit_id into v_QtyUnitID from inv_units where unit_abreviation=v_ContainerDetails_t(3);
    select chemcalcs.convert(v_ContainerDetails_t(2), v_QtyUnitID , (select c.unit_of_meas_id_fk from inv_containers c where c.container_id=v_ContainerDetails_t(1))) into v_UpadtedQTY from dual;
    update inv_containers set  Qty_Available= v_UpadtedQTY where container_id=v_ContainerDetails_t(1);
  end loop;
  return 1;
END UPDATECONTAINERQTY;
/
show errors;
