create or replace
function "&&SchemaName"."CREATEREGCOMPOUND"
(
	p_reg_id_fk IN inv_compounds.reg_id_fk%TYPE,
	p_batch_number_fk IN inv_compounds.batch_number_fk%TYPE
)

return inv_compounds.compound_id%TYPE is
	l_CompoundID inv_compounds.compound_id%TYPE;
	l_SubstanceName inv_compounds.substance_name%TYPE;
    l_CASNumber inv_compounds.CAS%TYPE;
    l_TempCompoundID inv_compounds.compound_id%TYPE;
    l_CompoundInternalID inv_compounds.cpd_internal_id_fk%TYPE;
begin
	begin
		select inv_compounds.compound_id into l_CompoundID
		from inv_compounds
		where inv_compounds.reg_id_fk = p_reg_id_fk
		and inv_compounds.batch_number_fk = p_batch_number_fk;
	exception
		when NO_DATA_FOUND then l_CompoundID := null;
	end;  

	if l_CompoundID is not null then
    -- Update the existing record in case the substance name or CAS have changed
		l_TempCompoundID:= UpdateRegCompound(p_reg_id_fk, p_batch_number_fk);
  else		
    -- Insert new
    begin
		  select "&&regSchemaName.".alt_ids.identifier into l_SubstanceName
		  from "&&regSchemaName.".alt_ids
		  where "&&regSchemaName.".alt_ids.reg_internal_id = p_reg_id_fk
		  and alt_ids.identifier_type = 0
		  and ROWNUM = 1;
	  exception
		  when NO_DATA_FOUND then l_SubstanceName := null;
	  end;
  
    begin
		  select "&&regSchemaName.".alt_ids.identifier into l_CASNumber
		  from "&&regSchemaName.".alt_ids
		  where "&&regSchemaName.".alt_ids.reg_internal_id = p_reg_id_fk
		  and alt_ids.identifier_type = 1
		  and ROWNUM = 1;
	  exception
		  when NO_DATA_FOUND then l_CASNumber := null;
	  end;

    begin
      select "&&regSchemaName.".reg_numbers.cpd_internal_id into l_CompoundInternalID
      from "&&regSchemaName.".reg_numbers
      where "&&regSchemaName.".reg_numbers.reg_id = p_reg_id_fk;
    exception
		  when NO_DATA_FOUND then l_CompoundInternalID := null;
	  end;

		insert into inv_compounds( substance_name, CAS, density, reg_id_fk, batch_number_fk, cpd_internal_id_fk )		
    -- TSM 6/3/08: fix for search form issue where the same reg compound appears twice when searching on CAS, once against 
    -- inv_compounds, the second against inv_vw_reg_batches.
    --values( decode(l_SubstanceName, NULL,'RegSubstance',l_SubstanceName), l_CASNumber, 1.0, p_reg_id_fk, p_batch_number_fk, l_CompoundInternalID )
    values( 'Registration_Substance', null, 1.0, p_reg_id_fk, p_batch_number_fk, l_CompoundInternalID )
		returning compound_id into l_CompoundID;
	end if;

	return l_CompoundID;

end CREATEREGCOMPOUND;
/
show errors;
