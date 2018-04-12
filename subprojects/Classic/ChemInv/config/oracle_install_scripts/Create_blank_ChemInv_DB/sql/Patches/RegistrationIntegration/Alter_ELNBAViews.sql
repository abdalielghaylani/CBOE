declare 
	integrationViewExists number;
begin
select count(*) INTO integrationViewExists from all_objects where lower(owner) = lower('&&ELNSchemaName.') and object_type='VIEW' and lower(object_name)=lower('BAV_INV_WELL');
	if (integrationViewExists > 0) then 
			execute immediate 'GRANT SELECT ON &&schemaName..INV_PLATES to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_PLATE_PARENT to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_PLATE_FORMAT to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_PHYSICAL_PLATE to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_LOCATIONS to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_GRID_FORMAT to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_GRID_POSITION to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_WELLS to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_WELL_COMPOUNDS to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_COMPOUNDS to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_ENUMERATION to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_SOLVENTS to &&ELNSchemaName.';
			execute immediate 'GRANT SELECT ON &&schemaName..INV_VW_COMPOUNDS to &&ELNSchemaName.';
			execute immediate 'ALTER VIEW &&ELNSchemaName..BAV_INV_WELL COMPILE';
			execute immediate 'ALTER VIEW &&ELNSchemaName..BAV_INV_PLATE_WELLS COMPILE';
	end if;
end;
/

