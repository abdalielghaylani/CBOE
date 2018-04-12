create or replace view inv_vw_enumerated_values (
	enum_id,
	enum_value,
	eset_id_fk,
	value,
	lookupsql
) as select
	inv_enumeration.enum_id,
	inv_enumeration.enum_value,
	inv_enumeration.eset_id_fk,
	inv_enum_values.value,
	inv_enum_values.lookupsql
from inv_enumeration, inv_enum_values
where inv_enumeration.enum_id = inv_enum_values.enum_id_fk(+);
