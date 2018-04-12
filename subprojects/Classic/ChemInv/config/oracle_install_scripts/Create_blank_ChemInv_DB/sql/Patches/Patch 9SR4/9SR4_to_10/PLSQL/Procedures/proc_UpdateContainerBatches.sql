CREATE OR REPLACE PROCEDURE "UPDATECONTAINERBATCHES"(
  pContainerID in Inv_Containers.Container_ID%Type
)
is
  l_cID integer;
  l_batchFieldName1 varchar(100);
  l_batchFieldName2 varchar(100);
  l_batchFieldName3 varchar(100);
  l_batchFieldValue1 varchar(100);
  l_batchFieldValue2 varchar(100);
  l_batchFieldValue3 varchar(100);
  l_sql varchar(500);
  l_batchSQL varchar(500);
  l_batchSqlCnt varchar(500);
  l_batchCriteriaSql varchar(800);
  l_UpdateBatchIdFk boolean;
  l_batchID integer;
  l_batchCnt integer;
  l_joinClose varchar(10);
		l_count INTEGER;
begin

  l_cID := pContainerID;
  l_UpdateBatchIdFk := false;

		l_batchFieldName1 := '';
		l_batchFieldName2 := '';
		l_batchFieldName3 := '';
		SELECT COUNT(*) INTO l_count FROM inv_container_batch_fields WHERE sort_order = 1;
		IF l_count > 0 THEN
				SELECT field_name INTO l_batchFieldName1 FROM inv_container_batch_fields WHERE sort_order = 1;
  END IF;
		SELECT COUNT(*) INTO l_count FROM inv_container_batch_fields WHERE sort_order = 2;
		IF l_count > 0 THEN
				SELECT field_name INTO l_batchFieldName2 FROM inv_container_batch_fields WHERE sort_order = 2;
  END IF;
		SELECT COUNT(*) INTO l_count FROM inv_container_batch_fields WHERE sort_order = 3;
		IF l_count > 0 THEN
				SELECT field_name INTO l_batchFieldName3 FROM inv_container_batch_fields WHERE sort_order = 3;
  END IF;

 -- l_batchFieldName1 := Rtrim(Ltrim(constants.cContainerBatchField1));
  --l_batchFieldName2 := Rtrim(Ltrim(constants.cContainerBatchField2));
  --l_batchFieldName3 := Rtrim(Ltrim(constants.cContainerBatchField3));
  l_batchFieldValue1 := '';
  l_batchFieldValue2 := '';
  l_batchFieldValue3 := '';

  -- These values used to be stored in inv_containers, but with 10.1 they are now at the inv_compounds level
  if lower(l_batchFieldName1) = 'reg_id_fk' then l_batchFieldName1 := 'inv_compounds.reg_id_fk'; end if;
  if lower(l_batchFieldName2) = 'reg_id_fk' then l_batchFieldName2 := 'inv_compounds.reg_id_fk'; end if;
  if lower(l_batchFieldName3) = 'reg_id_fk' then l_batchFieldName3 := 'inv_compounds.reg_id_fk'; end if;
  if lower(l_batchFieldName1) = 'batch_number_fk' then l_batchFieldName1 := 'inv_compounds.batch_number_fk'; end if;
  if lower(l_batchFieldName2) = 'batch_number_fk' then l_batchFieldName2 := 'inv_compounds.batch_number_fk'; end if;
  if lower(l_batchFieldName3) = 'batch_number_fk' then l_batchFieldName3 := 'inv_compounds.batch_number_fk'; end if;
  if lower(l_batchFieldName1) = 'density' then l_batchFieldName1 := 'inv_containers.density'; end if;
  if lower(l_batchFieldName2) = 'density' then l_batchFieldName2 := 'inv_containers.density'; end if;
  if lower(l_batchFieldName3) = 'density' then l_batchFieldName3 := 'inv_containers.density'; end if;
  if lower(l_batchFieldName1) = 'creator' then l_batchFieldName1 := 'inv_containers.creator'; end if;
  if lower(l_batchFieldName2) = 'creator' then l_batchFieldName2 := 'inv_containers.creator'; end if;
  if lower(l_batchFieldName3) = 'creator' then l_batchFieldName3 := 'inv_containers.creator'; end if;
  

  l_batchCriteriaSql := '';
  if Length(l_batchFieldName1) > 0 then
    l_sql := 'Select LTrim(RTrim(lpad('|| l_batchFieldName1 || ',100)))  From inv_containers, inv_compounds Where Container_ID = :cID and inv_containers.compound_id_fk = inv_compounds.compound_id(+)';
        
    EXECUTE IMMEDIATE l_sql into l_batchFieldValue1 USING l_cID;
    l_UpdateBatchIdFk := true;
    if Length(l_batchFieldValue1) > 0 then
      if Length(l_batchCriteriaSql) > 0 then
        l_joinClose := 'AND';
      else
        l_joinClose := 'WHERE';
      end if;
      l_batchCriteriaSql := l_batchCriteriaSql || ' ' || l_joinClose || ' batch_field_1 = ''' || replace(l_batchFieldValue1,'''','''''') || '''';
    else
      l_UpdateBatchIdFk := false;
    end if;
  end if;

  if Length(l_batchFieldName2) > 0 then
      l_sql := 'Select LTrim(RTrim(lpad('|| l_batchFieldName2 || ',100)))  From inv_containers, inv_compounds Where Container_ID = :cID and inv_containers.compound_id_fk = inv_compounds.compound_id(+)';
    
    EXECUTE IMMEDIATE l_sql into l_batchFieldValue2 USING l_cID;
    l_UpdateBatchIdFk := true;
    if Length(l_batchFieldValue2) > 0 then
      if Length(l_batchCriteriaSql) > 0 then
        l_joinClose := 'AND';
      else
        l_joinClose := 'WHERE';
      end if;
      l_batchCriteriaSql := l_batchCriteriaSql || ' ' || l_joinClose || ' batch_field_2 = ''' || replace(l_batchFieldValue2,'''','''''') || '''';
    else
      l_UpdateBatchIdFk := false;
    end if;
  end if;

  if Length(l_batchFieldName3) > 0 then
    l_sql := 'Select LTrim(RTrim(lpad('|| l_batchFieldName3 || ',100)))  From inv_containers, inv_compounds Where Container_ID = :cID and inv_containers.compound_id_fk = inv_compounds.compound_id(+)';
    
    EXECUTE IMMEDIATE l_sql into l_batchFieldValue3 USING l_cID;
    l_UpdateBatchIdFk := true;
    if Length(l_batchFieldValue3) > 0 then
      if Length(l_batchCriteriaSql) > 0 then
        l_joinClose := 'AND';
      else
        l_joinClose := 'WHERE';
      end if;
      l_batchCriteriaSql := l_batchCriteriaSql || ' ' || l_joinClose || ' batch_field_3 = ''' || replace(l_batchFieldValue3,'''','''''') || '''';
    else
      l_UpdateBatchIdFk := false;
    end if;
  end if;

  if l_UpdateBatchIdFk = true then
     l_batchSQL := 'Select batch_id from inv_container_batches' || l_batchCriteriaSql;
     l_batchSqlCnt := 'Select count(batch_id) as Cnt from inv_container_batches' || l_batchCriteriaSql;
     EXECUTE IMMEDIATE l_batchSqlCnt into l_batchCnt;

     if l_batchCnt > 0 then
       EXECUTE IMMEDIATE l_batchSQL into l_batchID;
     else
       insert into inv_container_batches (batch_field_1, batch_field_2, batch_field_3, batch_status_id_fk)
       values (l_batchFieldValue1,l_batchFieldValue2,l_batchFieldValue3, 2)		-- 2 = Pending CoT
	   returning batch_id into l_batchID;
       --EXECUTE IMMEDIATE l_batchSQL into l_batchID;
     end if;
     update inv_containers set batch_id_fk = l_batchID where container_id = l_cID;
  else
     update inv_containers set batch_id_fk = null where container_id = l_cID;
  end if;

END "UPDATECONTAINERBATCHES";
/

show errors;
