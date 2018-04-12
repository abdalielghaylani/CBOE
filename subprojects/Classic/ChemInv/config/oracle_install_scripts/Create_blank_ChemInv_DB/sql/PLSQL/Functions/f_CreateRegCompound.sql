CREATE OR REPLACE FUNCTION "CREATEREGCOMPOUND"
(
	p_reg_id_fk IN inv_compounds.reg_id_fk%TYPE,
	p_batch_number_fk IN inv_compounds.batch_number_fk%TYPE
)

RETURN inv_compounds.compound_id%TYPE IS	
BEGIN
	
	return null;

END CREATEREGCOMPOUND;
/
show errors;
