CREATE OR REPLACE PACKAGE COEDB.CoeDBLibrary
AS                        
    -- Constants
    DEFAULT_CHUNK_SIZE CONSTANT INTEGER := 1000;    -- can be changed if necessary
    Debuging           CONSTANT BOOLEAN := False;   -- Must always be False in PerForce
    eGenericException  CONSTANT Number:=-20000;
    
    -- Global public variables and types
    
    TYPE ID_TYPE IS TABLE OF coetemphitlist.id%TYPE;
    g_id_t_distinct ID_TYPE := id_type(); -- resulting collection for hitList
    g_insertedresults_cardinality INTEGER:=0;


    TYPE T_TableFloat IS TABLE OF FLOAT
       INDEX BY BINARY_INTEGER;

    TYPE T_TableNumber IS TABLE OF NUMBER(9)
       INDEX BY BINARY_INTEGER;

    TYPE T_TableInteger IS TABLE OF INTEGER
       INDEX BY BINARY_INTEGER;

    -- Procedues 

    PROCEDURE UpdateArray (AHitListID IN NUMBER, AIDArray  IN T_TableNumber, ASortArray IN T_TableFloat);

    PROCEDURE InsertHitList(refcursor IN sys_refcursor, commitSize IN INTEGER, recordCount OUT INTEGER, hasEnded OUT INTEGER);

    Function  ClobToTable(c IN CLOB) return myTableType;
    
END CoeDBLibrary; 
/


CREATE OR REPLACE PACKAGE BODY COEDB.CoeDBLibrary IS

    PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO LOG(LogProcedure,LogComment) VALUES($$plsql_unit||'.'||ALogProcedure,ALogComment);
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN NULL; --If logs don't work then don't stop
    END;
    
    PROCEDURE UpdateArray (AHitListID IN NUMBER, AIDArray IN T_TableNumber, ASortArray IN T_TableFloat) IS
       LIndex   NUMBER(9);
    BEGIN
       FORALL LIndex IN AIDArray.FIRST .. AIDArray.LAST
          UPDATE CoeTempHitList
             SET SortOrder = ASortArray (LIndex)
           WHERE HitListid = AHitListID AND ID = AIDArray (LIndex);
    END;

    -- insertHitList
    PROCEDURE insertHitList_old
            (refcursor IN SYS_REFCURSOR, 
             commitSize IN INTEGER, 
             recordCount OUT INTEGER) 
    AS
        -- Collection types declaration
        TYPE HITLISTID_TYPE IS TABLE OF coetemphitlist.hitlistid%TYPE;

        hitListID_t HITLISTID_TYPE := hitListID_type();
        id_t ID_TYPE := id_type();        -- id list from cursor
         -- DGB added two new collections for deduped cursor page and sortorder
         id_t_nodups ID_TYPE := id_type();        -- deduplicated id list from cursor
        sortorder_t ID_TYPE := id_type();
        counter INTEGER := 0;      -- counter by resulting collection
        cur_date DATE;             -- cacher for sysdate
        chunk_size INTEGER := DEFAULT_CHUNK_SIZE; 
        v_hitlist_id coetemphitlist.hitlistid%TYPE;  -- id of hitlits 

        EMPTY_COLLECTION EXCEPTION;        -- when first chunk is empty

    BEGIN
        -- Define chunk size
        IF commitSize > 0 THEN chunk_size := commitSize;
        END IF;

        -- If cursor is depleted close it and exit
        IF refcursor%ISOPEN THEN
            IF refcursor%NOTFOUND THEN
                CLOSE refcursor;
                RAISE EMPTY_COLLECTION;
            END IF;
        ELSE
            RAISE EMPTY_COLLECTION;
        END IF;
   
        -- Clear global values
        if refcursor%rowcount = 0 then
            CoeDBLibrary.g_id_t_distinct.DELETE;
            g_insertedresults_cardinality:=0;
        end if;
        
        -- Clear local collections
        hitListID_t.DELETE;
        id_t.DELETE;
    
        -- Fetch next chunk from cursor
        FETCH refcursor
         BULK COLLECT INTO hitListID_t, id_t
         LIMIT chunk_size;

        -- Cache date instead of calling SYSDATE at each row        
        cur_date := SYSDATE;

        -- If chunk is empty then cancel processing
        IF (id_t IS EMPTY) THEN
            RAISE EMPTY_COLLECTION;
        END IF;

        -- Hitlist id is the same for all list
        v_hitlist_id := hitListID_t(1);
    
        -- DGB build a deduplicated list from the current cursor chunk
        id_t_nodups := id_t MULTISET EXCEPT distinct g_id_t_distinct;

    
        -- Need a collection to contain the Sequencial increasing sortoder values
        -- [TODO:] is there a more efficient way to do this??
        FOR i IN  1..id_t_nodups.COUNT  LOOP
            sortorder_t.EXTEND(1); 
            sortorder_t(i) := i + g_insertedresults_cardinality;
        END LOOP;
        g_insertedresults_cardinality := g_insertedresults_cardinality + id_t_nodups.COUNT;
         
        -- DGB build a running list of inserted ids
        g_id_t_distinct := g_id_t_distinct MULTISET UNION id_t_nodups;
    
        -- Bulk insert of chunk
        FORALL i IN INDICES OF id_t_nodups
        SAVE EXCEPTIONS             
           INSERT /*+ APPEND */ INTO COETempHitList (hitlistid, id, datestamp, sortorder) 
             VALUES (v_hitlist_id, id_t_nodups(i), cur_date, sortorder_t(i));
           COMMIT;
    
        -- Count size of resulting collection
        recordCount := g_id_t_distinct.COUNT; 
     EXCEPTION
        WHEN EMPTY_COLLECTION THEN
            recordCount := 0; 
    END insertHitList_old;

    -- Sorts a collection by using a helper Associative array.
    -- Levereges the fact that array indexes are kept in order
    -- by the PL/SQL engine.
    -- TODO: Implement the "swaping" enhancement described in:
    -- http://technology.amis.nl/blog/?p=1214
    PROCEDURE aaSort
        (l_num_tbl in out NOCOPY ID_TYPE )
    AS
        l_idx integer;
    BEGIN
        declare
          -- here is where the sorting magic starts
          -- we create an Associative Array that is indexed by binary_integer
          -- we know that this collection's keys (the only thing we care about in this case)
          -- will always be kept in sorted order
          type num_aat_t is table of number index by binary_integer;
          l_num_aat num_aat_t;
        begin
            IF NOT (l_num_tbl IS EMPTY) THEN

       
    
          l_idx:= l_num_tbl.first;
          -- loop over all elements in the l_num_tbl collection 
          -- that we want to sort. Use every element in l_num_tbl
          -- as a key for the l_num_aat associative array. Associate
          -- the key with a meaningless value; we do not care about
          -- the value in this case.
          loop
            l_num_aat( l_num_tbl(l_idx)):=0;
            l_idx:= l_num_tbl.next(l_idx);
            exit when l_idx is null;
          end loop;
          -- remove all elements from l_num_tbl
          l_num_tbl.delete;
          -- start repopulating l_num_tbl - in the proper order -
          -- from the sorted collection of keys in the l_num_aat collection
          l_idx:= l_num_aat.first;
          loop
            l_num_tbl.extend;
            l_num_tbl(l_num_tbl.last):= l_idx;
            l_idx:= l_num_aat.next(l_idx);
            exit when l_idx is null;
          end loop;
      
          END IF;
        end;
        -- DONE! At this point, l_num_tbl is properly sorted  
   
    END aaSort;    

    PROCEDURE insertHitList
            (refcursor IN SYS_REFCURSOR, 
             commitSize IN INTEGER, 
             recordCount OUT INTEGER,
             hasEnded OUT INTEGER) 
    AS
        -- Collection types declaration
        TYPE HITLISTID_TYPE IS TABLE OF COETempHitList.hitlistid%TYPE;

        hitListID_t HITLISTID_TYPE := hitlistid_type();
        id_t        ID_TYPE := id_type();    -- id list from cursor
    
        sortorder_t ID_TYPE := id_type(); 
   
        cur_date    TIMESTAMP;             -- cacher for sysdate
        chunk_size  PLS_INTEGER := DEFAULT_CHUNK_SIZE; 
        v_hitlist_id COETempHitList.hitlistid%TYPE;  -- id of hitlits 
        id_t_count  PLS_INTEGER := 0;  

        EMPTY_COLLECTION EXCEPTION;        -- when first chunk is empty

    BEGIN
        
        $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'*BEGIN* - commitSize:'||commitSize); $end null;
        -- Define chunk size
        IF commitSize > 0 THEN chunk_size := commitSize;
        END IF;

        recordCount := 0;
        hasEnded := 0;

        -- Repeat until resulting set size is >= commitSize
        $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'Begin - LOOP'); $end null;
        <<size_loop>>
        LOOP
        BEGIN
            -- If cursor is depleted close it and exit
            $if CoeDBLibrary.Debuging $then IF refcursor%ISOPEN THEN InsertLog('insertHitList'||' Line:'||$$plsql_line,'refcursor%ISOPEN: True'); ELSE InsertLog('insertHitList'||' Line:'||$$plsql_line,'refcursor%ISOPEN: False'); END IF;  $end null;
            IF refcursor%ISOPEN THEN
                $if CoeDBLibrary.Debuging $then IF refcursor%NOTFOUND  THEN InsertLog('insertHitList'||' Line:'||$$plsql_line,'refcursor%NOTFOUND: True'); ELSE InsertLog('insertHitList'||' Line:'||$$plsql_line,'refcursor%NOTFOUND: False'); END IF;  $end null;                
                IF refcursor%NOTFOUND THEN
                     $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'ERROR - refcursor%NOTFOUND - EMPTY_COLLECTION'); $end null;
                    --CLOSE refcursor;
                    hasEnded := 1;
                    RAISE EMPTY_COLLECTION;
                END IF;
            ELSE
                $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'ERROR - EMPTY_COLLECTION'); $end null;
                RAISE EMPTY_COLLECTION;
            END IF;   
        
            -- Clear global values if this is a new cursor
            $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'refcursor%ROWCOUNT: '||refcursor%ROWCOUNT); $end null;
            IF refcursor%ROWCOUNT = 0 then
                coedblibrary.g_id_t_distinct.DELETE;
                g_insertedresults_cardinality := 0;
            END IF;
        
            -- Clear local collections
            hitListID_t.DELETE;
            id_t.DELETE;
            sortorder_t.DELETE;
    
            -- Fetch next chunk from cursor
            FETCH refcursor
             BULK COLLECT INTO hitListID_t, id_t
             LIMIT chunk_size;  
            $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'refcursor%ROWCOUNT: '||refcursor%ROWCOUNT); $end null;
            IF refcursor%ROWCOUNT = 0 then
                EXIT size_loop;
            END IF;
            -- Sort the collection
            aaSort(id_t);
        
            -- Cache date instead of calling SYSDATE at each row        
            cur_date := SYSTIMESTAMP;
        
            -- Collect only new records
            id_t := id_t MULTISET EXCEPT DISTINCT g_id_t_distinct;        
            -- Append new chunk to resulting collection 
            g_id_t_distinct := g_id_t_distinct MULTISET UNION id_t;  
        
            -- If chunk is empty then cancel processing
            IF (id_t IS EMPTY) THEN
                $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'ERROR - EMPTY_COLLECTION - id_t IS EMPTY'); $end null;
                RAISE EMPTY_COLLECTION;
            END IF;        
            -- If not - save its size
            id_t_count := id_t.COUNT;
        
            -- Hitlist id is the same for all list
            v_hitlist_id := hitListID_t(1);
        
            -- Initialize Sortorder collection
            FOR i IN 1 .. id_t_count LOOP
                sortorder_t.EXTEND;
                sortorder_t(i) := g_insertedresults_cardinality + i;
            END LOOP;
    
            -- Insert new records in the table and commit it
            FORALL i IN INDICES OF id_t
            SAVE EXCEPTIONS             
                INSERT INTO COETempHitList (hitlistid, id, datestamp, sortorder) 
                 VALUES (v_hitlist_id, id_t(i), cur_date, sortorder_t(i));

            COMMIT;

            -- increase global and local collection counters
            g_insertedresults_cardinality := g_insertedresults_cardinality + id_t_count;
            recordCount := recordCount + id_t_count;

            -- loop exit condition
            EXIT size_loop WHEN recordCount >= commitSize;

         EXCEPTION
            WHEN EMPTY_COLLECTION THEN
                EXIT size_loop;
        END;
        END LOOP size_loop;
        
        $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'END LOOP hasEnded:'||hasEnded); $end null;

        $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'NVL(v_hitlist_id,0):'||NVL(v_hitlist_id,0));$end null;
        IF NVL(v_hitlist_id,0)<>0 THEN
            UPDATE COETempHitListID COETHLID
                SET COETHLID.Number_Hits=(SELECT COUNT(1) FROM COETempHitList WHERE HitListID=COETHLID.ID) 
                WHERE COETHLID.ID=v_hitlist_id;
            $if CoeDBLibrary.Debuging $then IF NOT refcursor%ISOPEN THEN InsertLog('insertHitList'||' Line:'||$$plsql_line,'COETempHitListID Updated#:'||SQL%ROWCOUNT); END IF; $end null;
            COMMIT;    
        END IF;

        -- Output result
        recordCount := g_insertedresults_cardinality;  -- must be global??
        IF refcursor%ISOPEN THEN
          IF refcursor%NOTFOUND THEN
              $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'ENDING - refcursor%NOTFOUND'); $end null;
              --CLOSE refcursor;
              hasEnded := 1;
          END IF;
        END IF;
        $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,'*END* hasEnded:'||hasEnded); $end null;
    EXCEPTION
        WHEN OTHERS THEN
            $if CoeDBLibrary.Debuging $then InsertLog('insertHitList'||' Line:'||$$plsql_line,chr(10)||'*END* WITH *ERROR* EXCEPTION: '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(EGenericException, DBMS_UTILITY.FORMAT_ERROR_STACK);
    END insertHitList;  

    PROCEDURE InsertHitList_old_old(refcursor IN sys_refcursor, commitSize IN INTEGER, recordCount OUT INTEGER) AS
        TYPE hitListID_type IS TABLE OF coetemphitlist.hitlistid%TYPE;
        TYPE id_type IS TABLE OF coetemphitlist.id%type;
        TYPE sortOrder_type IS TABLE OF coetemphitlist.sortorder%TYPE;

        hitListID_t hitListID_type := hitListID_type();
        id_t id_type := id_type();
        sortOrder_t sortOrder_type := sortOrder_type();
        counter INTEGER := 1;
    BEGIN
        IF commitSize > 0 THEN
            FOR i IN 1..commitSize
            LOOP
                IF refcursor%NOTFOUND THEN
                    CLOSE refcursor;
                    EXIT;
                ELSE
                    hitlistID_t.EXTEND(1);
                    id_t.EXTEND(1);
                    sortOrder_t.EXTEND(1);

                    FETCH refcursor INTO hitListID_t(i), id_t(i);

                    sortOrder_t(i) := refcursor%rowcount;
                    recordCount := refcursor%rowcount;
                END IF;
            END LOOP;
        ELSE
            LOOP
                /* Retrieve each row of the result of the above query
                into PL/SQL variables: */
                IF refcursor%NOTFOUND THEN
                    CLOSE refcursor;
                    EXIT;
                ELSE
                       hitlistID_t.EXTEND(1);
                    id_t.EXTEND(1);
                    sortOrder_t.EXTEND(1);

                    FETCH refcursor INTO hitListID_t(counter), id_t(counter);

                    sortOrder_t(counter) := refcursor%rowcount;
                    counter := counter + 1;
                END IF;
            END LOOP;

            recordCount := refcursor%rowcount;
        END IF;

        IF (id_t(id_t.LAST) IS NULL OR hitlistID_t(hitlistID_t.LAST) IS NULL) THEN
            id_t.TRIM();
            hitlistID_t.TRIM();
            sortOrder_t.TRIM();
        END IF;
        /* TODO: if there is no commitSize, the insertion should be chunked */
        FORALL j IN 1..id_t.count
            INSERT INTO COETEMPHITLIST(hitlistid, id, datestamp, sortorder) VALUES(hitListID_t(j), id_t(j), SYSDATE, sortOrder_t(j));

    /*EXCEPTION
        WHEN OTHERS THEN
            RAISE_APPLICATION_ERROR(-20000, to_char(hitListID_t(recordCount) || ' ' || id_t(recordCount) || ' ' || id_t.count));*/

    END;

    FUNCTION ClobToTable(c IN CLOB) return myTableType IS
        i INTEGER;
        commaPos INTEGER;

        previousCommaPos INTEGER;
        val VARCHAR(30);
        length INTEGER;
        tab myTableType := myTableType();
    BEGIN
        i := 1;

        previousCommaPos := 0;
        length := DBMS_LOB.GETLENGTH(c);

        LOOP
            commaPos := DBMS_LOB.INSTR(c, ',', 1, i);
            IF commaPos != 0 THEN
                val := DBMS_LOB.SUBSTR(c, commaPos - previousCommaPos - 1, previousCommaPos + 1);

                previousCommaPos := commaPos;

                tab.extend;
                tab(tab.count) := val;
            ELSE
                val := DBMS_LOB.SUBSTR(c, length - previousCommaPos, previousCommaPos + 1);

               tab.extend;
               tab(tab.count) := val;

               EXIT;
            END IF;

            i := i + 1;
        END LOOP;

        RETURN tab;
    END;
END; 
/
