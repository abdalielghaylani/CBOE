update inv_container_batch_fields set BATCH_TYPE_ID_FK = 1 where batch_type_id_fk is null;
commit;
