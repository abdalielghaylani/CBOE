DECLARE
  CURSOR c_Batches IS
    SELECT Batch_Internal_ID, FullRegNumber 
    FROM   RegDB.Batches
    WHERE FullRegNumber like ('%'||UNISTR('\2022')||'%');
  TYPE t_ids_array  IS TABLE OF NUMBER INDEX BY BINARY_INTEGER;
  TYPE t_FullRegNumber_array IS TABLE OF VARCHAR2(100) INDEX BY BINARY_INTEGER;
  v_batches_ids         t_ids_array;
  v_FullRegNumber_array t_FullRegNumber_array;
  v_newFullRegNumber_array t_FullRegNumber_array;
  v_row_count      NUMBER := 0;
BEGIN
 
 OPEN c_Batches;
  LOOP
  
    FETCH c_Batches
    BULK COLLECT INTO v_batches_ids, v_FullRegNumber_array
    LIMIT 100000;
    
    EXIT WHEN v_row_count = c_Batches%ROWCOUNT;
    
    v_row_count := c_Batches%ROWCOUNT;
    
    FOR i IN 1..v_batches_ids.count LOOP
      v_newFullRegNumber_array(i) := REPLACE (v_FullRegNumber_array(i),UNISTR('\2022'),'*');
      
    END LOOP;
    
    FORALL i IN 1..v_batches_ids.COUNT
      UPDATE RegDB.Batches
        SET    FullRegNumber = v_newFullRegNumber_array(i)
        WHERE  Batch_Internal_ID = v_batches_ids(i);
        
    COMMIT;    
      
  END LOOP;
  
  CLOSE c_Batches;
  
END;
/