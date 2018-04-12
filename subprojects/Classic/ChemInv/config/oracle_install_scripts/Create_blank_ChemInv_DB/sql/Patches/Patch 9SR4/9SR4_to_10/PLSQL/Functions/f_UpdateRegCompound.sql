create or replace
function "&&SchemaName"."UPDATEREGCOMPOUND"
(
	p_reg_id_fk IN inv_compounds.reg_id_fk%TYPE,
	p_batch_number_fk IN inv_compounds.batch_number_fk%TYPE
)

return inv_compounds.compound_id%TYPE is
begin
	return null;
end UPDATEREGCOMPOUND;
/
show errors;
