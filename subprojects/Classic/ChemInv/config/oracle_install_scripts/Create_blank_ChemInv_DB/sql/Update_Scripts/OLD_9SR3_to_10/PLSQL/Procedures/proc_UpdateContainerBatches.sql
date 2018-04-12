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
  l_batchFieldValue1toInsert varchar(100);
  l_batchFieldValue2toInsert varchar(100);
  l_batchFieldValue3toInsert varchar(100);
  l_sql varchar(200);
  l_batchSQL varchar(200);
  l_batchSqlCnt varchar(200);
  l_batchCriteriaSql varchar(300);
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
  -- Fixing CSBR-93162,  where a batching field contains a single quote. 
  l_batchFieldValue1toInsert := '';
  l_batchFieldValue2toInsert := '';
  l_batchFieldValue3toInsert := '';

  l_batchCriteriaSql := '';
  if Length(l_batchFieldName1) > 0 then
    l_sql := 'Select LTrim(RTrim(' || l_batchFieldName1 || '))  From inv_containers Where Container_ID = ' || l_cID;
    EXECUTE IMMEDIATE l_sql into l_batchFieldValue1;
    l_batchFieldValue1toInsert := l_batchFieldValue1;
    if instr(l_batchFieldValue1,'''')> 0 then
		l_batchFieldValue1 :=  replace(l_batchFieldValue1,'''',''''''); 
    end if; 
    l_UpdateBatchIdFk := true;
    if Length(l_batchFieldValue1) > 0 then
      if Length(l_batchCriteriaSql) > 0 then
        l_joinClose := 'AND';
      else
        l_joinClose := 'WHERE';
      end if;
      l_batchCriteriaSql := l_batchCriteriaSql || ' ' || l_joinClose || ' batch_field_1 = ''' || l_batchFieldValue1 || '''';
    else
      l_UpdateBatchIdFk := false;
    end if;
  end if;

  if (Length(l_batchFieldName2) > 0 and l_UpdateBatchIdFk = true) then
    l_sql := 'Select LTrim(RTrim(' || l_batchFieldName2 || '))  From inv_containers Where Container_ID = ' || l_cID;
    EXECUTE IMMEDIATE l_sql into l_batchFieldValue2;
    l_batchFieldValue2toInsert := l_batchFieldValue2;
    if instr(l_batchFieldValue2,'''')> 0 then
		l_batchFieldValue2 :=  replace(l_batchFieldValue2,'''',''''''); 
    end if; 
    l_UpdateBatchIdFk := true;
    if Length(l_batchFieldValue2) > 0 then
      if Length(l_batchCriteriaSql) > 0 then
        l_joinClose := 'AND';
      else
        l_joinClose := 'WHERE';
      end if;
      l_batchCriteriaSql := l_batchCriteriaSql || ' ' || l_joinClose || ' batch_field_2 = ''' || l_batchFieldValue2 || '''';
    else
      l_UpdateBatchIdFk := false;
    end if;
  end if;

  if (Length(l_batchFieldName3) > 0 and l_UpdateBatchIdFk = true) then
    l_sql := 'Select LTrim(RTrim(' || l_batchFieldName3 || '))  From inv_containers Where Container_ID = ' || l_cID;
    EXECUTE IMMEDIATE l_sql into l_batchFieldValue3 ;
    l_batchFieldValue3toInsert := l_batchFieldValue3;
    if instr(l_batchFieldValue3,'''')> 0 then
		l_batchFieldValue3 :=  replace(l_batchFieldValue3,'''',''''''); 
    end if; 
    l_UpdateBatchIdFk := true;
    if Length(l_batchFieldValue3) > 0 then
      if Length(l_batchCriteriaSql) > 0 then
        l_joinClose := 'AND';
      else
        l_joinClose := 'WHERE';
      end if;
      l_batchCriteriaSql := l_batchCriteriaSql || ' ' || l_joinClose || ' batch_field_3 = ''' || l_batchFieldValue3 || '''';
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
       insert into inv_container_batches (batch_id,batch_field_1, batch_field_2, batch_field_3, batch_status_id_fk)
              values (seq_inv_container_batches.nextval,l_batchFieldValue1toInsert,l_batchFieldValue2toInsert,l_batchFieldValue3toInsert, 2);
       EXECUTE IMMEDIATE l_batchSQL into l_batchID;
     end if;
     update inv_containers set batch_id_fk = l_batchID where container_id = l_cID;
  else
     update inv_containers set batch_id_fk = null where container_id = l_cID;
  end if;

END "UPDATECONTAINERBATCHES";
/

show errors;
