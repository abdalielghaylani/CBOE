create or replace
function "&&SchemaName"."UPDATEREGCOMPOUND"
(
	p_reg_id_fk IN inv_compounds.reg_id_fk%TYPE,
	p_batch_number_fk IN inv_compounds.batch_number_fk%TYPE
)

return inv_compounds.compound_id%TYPE is
	l_CompoundID inv_compounds.compound_id%TYPE;
	l_SubstanceName inv_compounds.substance_name%TYPE;
  l_CASNumber inv_compounds.CAS%TYPE;
  l_MolWeight inv_compounds.MOLECULAR_WEIGHT%TYPE;
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
    -- Update the existing record in case the substance name, CAS, or structure (mol weight) have changed
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
      select cscartridge.molweight("&&regSchemaName.".structures.base64_cdx) into l_MolWeight
      from "&&regSchemaName.".structures, "&&regSchemaName.".reg_numbers      
      where &&regSchemaName..reg_numbers.reg_id = p_reg_id_fk
      and &&regSchemaName..structures.cpd_internal_id(+) = &&regSchemaName..reg_numbers.cpd_internal_id
      and ROWNUM = 1;
    exception
      when NO_DATA_FOUND then l_MolWeight := 0;
    end;

    if l_MolWeight is null then
      l_MolWeight := 0;
    end if;

    update inv_compounds
    --set substance_name = decode(l_SubstanceName, NULL,'RegSubstance',l_SubstanceName), CAS = l_CASNumber, molecular_weight = l_MolWeight
    set substance_name = 'Registration_Substance', CAS = null, molecular_weight = 0
    where compound_id = l_CompoundID;		
  end if;

	return l_CompoundID;

end UPDATEREGCOMPOUND;
/
show errors;
