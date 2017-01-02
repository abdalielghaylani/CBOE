declare jobno number;
BEGIN
   DBMS_JOB.SUBMIT(jobno, 
      q'[DECLARE
   v_now              DATE := SYSDATE;
   v_wait_seconds     INTEGER := 2;--num seconds
   library_lock_exc   EXCEPTION; -- This gets raised if something has the table locked in a NOWAIT
   v_max_retries      INTEGER := 10;
   v_retry_count      INTEGER := 0;
   PRAGMA EXCEPTION_INIT (library_lock_exc, -00054);
BEGIN
   WHILE v_retry_count <= v_max_retries
   LOOP
      BEGIN
         EXECUTE IMMEDIATE 'alter table COEFULLPAGE move';
         FOR cur IN (SELECT status, index_name
                       FROM user_indexes
                      WHERE table_name = 'COEFULLPAGE' AND status != 'VALID')
         LOOP
            EXECUTE IMMEDIATE 'alter index ' || cur.index_name || ' rebuild';
         END LOOP;
         v_retry_count := 100;
         dbms_stats.gather_table_stats('COEDB','COEFULLPAGE');
      EXCEPTION
         WHEN library_lock_exc
         THEN
            LOOP
               EXIT WHEN SYSDATE > v_now + (v_wait_seconds * (1 / 86400));
            END LOOP;
            v_now := SYSDATE;
         WHEN OTHERS
         THEN
            RAISE;
      END;
      v_retry_count := v_retry_count + 1;
   END LOOP;
END;]', 
      SYSDATE, 'SYSDATE + 1/5');
   COMMIT;
END;
/
