create or replace
PACKAGE BODY BATCH

    IS

	FUNCTION UPDATEMINTHRESHOLD
		(
        p_BatchID IN INV_CONTAINER_BATCHES.BATCH_ID%TYPE,
        p_MinStockThreshold IN INV_CONTAINER_BATCHES.MINIMUM_STOCK_THRESHOLD%TYPE
        ) RETURN INV_CONTAINER_BATCHES.BATCH_ID%TYPE
	IS
	BEGIN

        UPDATE INV_CONTAINER_BATCHES SET
               MINIMUM_STOCK_THRESHOLD = p_MinStockThreshold
        WHERE BATCH_ID = p_BatchID ;

		RETURN p_BatchID;

	END UPDATEMINTHRESHOLD;


	FUNCTION UPDATEBATCHFIELDS
		(
        p_BatchID IN INV_CONTAINER_BATCHES.BATCH_ID%TYPE,
        p_batchStatusId inv_container_batches.batch_status_id_fk%TYPE,
        p_MinStockThreshold IN INV_CONTAINER_BATCHES.MINIMUM_STOCK_THRESHOLD%TYPE,
        p_Comments IN INV_CONTAINER_BATCHES.COMMENTS%TYPE,
        p_Field_1 IN INV_CONTAINER_BATCHES.FIELD_1%TYPE,
        p_Field_2 IN INV_CONTAINER_BATCHES.FIELD_2%TYPE,
        p_Field_3 IN INV_CONTAINER_BATCHES.FIELD_3%TYPE,
        p_Field_4 IN INV_CONTAINER_BATCHES.FIELD_4%TYPE,
        p_Field_5 IN INV_CONTAINER_BATCHES.FIELD_5%TYPE,
        p_Date_1 IN INV_CONTAINER_BATCHES.DATE_1%TYPE,
        p_Date_2 IN INV_CONTAINER_BATCHES.DATE_2%TYPE
        ) RETURN INV_CONTAINER_BATCHES.BATCH_ID%TYPE
	IS
    l_Sql VARCHAR(5000) ;
	l_batchStatusId inv_container_batches.batch_status_id_fk%TYPE;
    l_MinStockThreshold  INV_CONTAINER_BATCHES.MINIMUM_STOCK_THRESHOLD%TYPE;
    BEGIN
        l_Sql := l_Sql || 'UPDATE INV_CONTAINER_BATCHES SET ' ;
        l_Sql := l_Sql || ' batch_status_id_fk = :1, ' ;
        IF p_batchStatusId IS NULL THEN
            l_batchStatusId := NULL;
        ELSE
            l_batchStatusId := p_batchStatusId  ;
        END IF ;
        l_Sql := l_Sql || ' MINIMUM_STOCK_THRESHOLD = :2, ' ;
        IF p_MinStockThreshold IS NULL THEN
            l_MinStockThreshold := NULL;
        ELSE
            l_MinStockThreshold := p_MinStockThreshold ;
        END IF ;
        l_Sql := l_Sql || ' COMMENTS = :3 ';
        l_Sql := l_Sql || ' WHERE BATCH_ID = :4' ;

		EXECUTE IMMEDIATE l_Sql USING l_batchStatusId, l_MinStockThreshold, p_Comments, p_BatchID;

        IF p_Field_1 IS NOT NULL THEN
              l_Sql := l_Sql || ' UPDATE INV_CONTAINER_BATCHES SET ' ;
              l_Sql := l_Sql || ' FIELD_1 = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ID = :2' ;
              EXECUTE IMMEDIATE l_Sql USING p_Field_1, p_BatchID;
        END IF ;
        IF p_Field_2 IS NOT NULL THEN
              l_Sql := l_Sql || ' UPDATE INV_CONTAINER_BATCHES SET ' ;
              l_Sql := l_Sql || ' FIELD_2 = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ID = :2' ;
              EXECUTE IMMEDIATE l_Sql USING p_Field_2, p_BatchID;
        END IF ;
        IF p_Field_3 IS NOT NULL THEN
			  l_Sql := l_Sql || ' UPDATE INV_CONTAINER_BATCHES SET ' ;
              l_Sql := l_Sql || ' FIELD_3 = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ID = :2' ;
              EXECUTE IMMEDIATE l_Sql USING p_Field_3, p_BatchID;
        END IF ;
        IF p_Field_4 IS NOT NULL THEN
              l_Sql := l_Sql || ' UPDATE INV_CONTAINER_BATCHES SET ' ;
              l_Sql := l_Sql || ' FIELD_4 = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ID = :2' ;
              EXECUTE IMMEDIATE l_Sql USING p_Field_4, p_BatchID;
        END IF ;
        IF p_Field_5 IS NOT NULL THEN
              l_Sql := l_Sql || ' UPDATE INV_CONTAINER_BATCHES SET ' ;
              l_Sql := l_Sql || ' FIELD_5 = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ID = :2' ;
              EXECUTE IMMEDIATE l_Sql USING p_Field_5, p_BatchID;
        END IF ;
        IF p_Date_1 IS NOT NULL THEN
              l_Sql := l_Sql || ' UPDATE INV_CONTAINER_BATCHES SET ' ;
              l_Sql := l_Sql || ' DATE_1 = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ID = :2' ;
              EXECUTE IMMEDIATE l_Sql USING p_Date_1, p_BatchID;
        END IF ;
        IF p_Date_2 IS NOT NULL THEN
              l_Sql := l_Sql || ' UPDATE INV_CONTAINER_BATCHES SET ' ;
              l_Sql := l_Sql || ' DATE_2 = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ID = :2' ;
              EXECUTE IMMEDIATE l_Sql USING p_Date_2, p_BatchID;
        END IF ;
		RETURN p_BatchID;

	END UPDATEBATCHFIELDS;


	PROCEDURE GETBATCHDETAILS
        (
		p_BatchID IN INV_CONTAINER_BATCHES.BATCH_ID%Type,
		O_RS OUT CURSOR_TYPE) AS
   	BEGIN
   		OPEN O_RS FOR

        SELECT
					*
        FROM     INV_CONTAINER_BATCHES ICB
        WHERE BATCH_ID = p_BatchID ;

   	END GETBATCHDETAILS;


	PROCEDURE GETBATCHFIELDS
        (	p_RS OUT CURSOR_TYPE) AS
   	BEGIN
   		OPEN p_RS FOR
        SELECT * FROM inv_container_batch_fields ORDER BY sort_order;
   	END GETBATCHFIELDS;

PROCEDURE ClearBatchingFields (pBatchTypeID Inv_Batch_type.Batch_Type%TYPE:=NULL)
AS
l_batch_field_name  varchar2(50);
BEGIN
 if pBatchTypeID is not null then
  l_batch_field_name:= 'batch_id' || pBatchTypeID ||  '_fk';
  if pBatchTypeID = 1 then 
     l_batch_field_name:= 'batch_id_fk';       
  end if;
 end if;
  
 if pBatchTypeID is not null then
      --' delete existing batching  fields
    DELETE inv_container_batch_fields where BATCH_TYPE_ID_FK= pBatchTypeID;
      --' delete existing batch references
    execute immediate ' UPDATE inv_containers SET ' || l_batch_field_name ||  ' = NULL';
     --' delete existing batches
     DELETE inv_container_batches where batch_type= pBatchTypeID;
 else
   --' delete existing batching  fields
    DELETE inv_container_batch_fields;
     --' delete existing batch references
    UPDATE inv_containers SET batch_id_fk = NULL, batch_id2_fk = NULL, batch_id3_fk = NULL;
     --' delete existing batches
    DELETE inv_container_batches; 
end if;

END ClearBatchingFields;

PROCEDURE UPDATEBATCHINGFIELDS(
										p_field1 inv_container_batch_fields.field_name%TYPE,
										p_field2 inv_container_batch_fields.field_name%TYPE,
										p_field3 inv_container_batch_fields.field_name%TYPE,
										p_displayName1 inv_container_batch_fields.display_name%TYPE,
										p_displayName2 inv_container_batch_fields.display_name%TYPE,
										p_displayName3 inv_container_batch_fields.display_name%TYPE,
                    p_field1_2 inv_container_batch_fields.field_name%TYPE,
                    p_displayName1_2 inv_container_batch_fields.display_name%TYPE,
                    p_field2_2 inv_container_batch_fields.field_name%TYPE,
                    p_displayName2_2 inv_container_batch_fields.display_name%TYPE,
                    p_field3_2 inv_container_batch_fields.field_name%TYPE,
                    p_displayName3_2 inv_container_batch_fields.display_name%TYPE,
                    p_field1_3 inv_container_batch_fields.field_name%TYPE,
                    p_displayName1_3 inv_container_batch_fields.display_name%TYPE,
                    p_field2_3 inv_container_batch_fields.field_name%TYPE,
                    p_displayName2_3 inv_container_batch_fields.display_name%TYPE,
                    p_field3_3 inv_container_batch_fields.field_name%TYPE,
                    p_displayName3_3 inv_container_batch_fields.display_name%TYPE)
AS
          v_batch_modification_val integer ;
BEGIN
  
  v_batch_modification_val := CheckBatchModifications(1,p_field1, p_field2, p_field3, p_displayName1, p_displayName2, p_displayName3);
  IF v_batch_modification_val>0 THEN     
      IF v_batch_modification_val=2 THEN
        ClearBatchingFields(1);
        INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field1, p_displayName1, 1,1);
        IF p_field2 IS NOT NULL THEN
              INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field2, p_displayName2, 2,1);
        END IF;
        IF p_field3 IS NOT NULL THEN
                INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field3, p_displayName3, 3,1);
        END IF;
      END IF;
      IF v_batch_modification_val=1 THEN
        IF p_displayName1 IS NOT NULL THEN
          UPDATE inv_container_batch_fields SET display_name=p_displayName1 WHERE BATCH_TYPE_ID_FK=1 AND sort_order=1;
        END IF;  
        IF p_displayName2 IS NOT NULL THEN
        UPDATE inv_container_batch_fields SET display_name=p_displayName2 WHERE BATCH_TYPE_ID_FK=1 AND sort_order=2;
        END IF;
        IF p_displayName3 IS NOT NULL THEN
        UPDATE inv_container_batch_fields SET display_name=p_displayName2 WHERE BATCH_TYPE_ID_FK=1 AND sort_order=3;
        END IF;
      END IF;
  END IF;

  IF (p_field1_2 IS NULL) THEN 
    ClearBatchingFields(2);
  ELSE
    v_batch_modification_val := CheckBatchModifications(2,p_field1_2, p_field2_2, p_field3_2, p_displayName1_2, p_displayName2_2, p_displayName3_2);
    IF v_batch_modification_val>0 THEN     
        IF v_batch_modification_val=2 THEN
          ClearBatchingFields(2);
          IF p_field1_2 IS NOT NULL THEN
                  INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field1_2, p_displayName1_2, 1,2);
          END IF;
          IF p_field2_2 IS NOT NULL THEN
                  INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field2_2, p_displayName2_2, 2,2);
          END IF;
          IF p_field3_2 IS NOT NULL THEN
                  INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field3_2, p_displayName3_2, 3,2);
          END IF;
        END IF;
        IF v_batch_modification_val=1 THEN
          IF p_displayName1_2 IS NOT NULL THEN
            UPDATE inv_container_batch_fields SET display_name=p_displayName1_2 WHERE BATCH_TYPE_ID_FK=2 AND sort_order=1;
          END IF;  
          IF p_displayName2_2 IS NOT NULL THEN
          UPDATE inv_container_batch_fields SET display_name=p_displayName2_2 WHERE BATCH_TYPE_ID_FK=2 AND sort_order=2;
          END IF;
          IF p_displayName3_2 IS NOT NULL THEN
          UPDATE inv_container_batch_fields SET display_name=p_displayName3_2 WHERE BATCH_TYPE_ID_FK=2 AND sort_order=3;
          END IF;
        END IF;
    END IF;
  END IF;
  IF (p_field1_3 IS NULL) THEN 
    ClearBatchingFields(3);
  ELSE
    v_batch_modification_val := CheckBatchModifications(3,p_field1_3, p_field2_3, p_field3_3, p_displayName1_3, p_displayName2_3, p_displayName3_3);
    IF v_batch_modification_val>0 THEN     
        IF v_batch_modification_val=2 THEN
          ClearBatchingFields(3);
          IF p_field1_3 IS NOT NULL THEN
                  INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field1_3, p_displayName1_3, 1,3);
          END IF;
          IF p_field2_3 IS NOT NULL THEN
                  INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field2_3, p_displayName2_3, 2,3);
          END IF;
          IF p_field3_3 IS NOT NULL THEN
                  INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order, BATCH_TYPE_ID_FK)  VALUES(p_field3_3, p_displayName3_3, 3,3);
          END IF;      
        END IF;
        IF v_batch_modification_val=1 THEN
          IF p_displayName1_3 IS NOT NULL THEN
            UPDATE inv_container_batch_fields SET display_name=p_displayName1_3 WHERE BATCH_TYPE_ID_FK=3 AND sort_order=1;
          END IF;  
          IF p_displayName2_3 IS NOT NULL THEN
          UPDATE inv_container_batch_fields SET display_name=p_displayName2_3 WHERE BATCH_TYPE_ID_FK=3 AND sort_order=2;
          END IF;
          IF p_displayName3_3 IS NOT NULL THEN
          UPDATE inv_container_batch_fields SET display_name=p_displayName3_3 WHERE BATCH_TYPE_ID_FK=3 AND sort_order=3;
          END IF;
        END IF;
     END IF;    
  END IF; 

--' calculate new batches
FOR container_rec IN (SELECT container_id FROM inv_containers ORDER BY container_id)
LOOP
				updatecontainerbatches(container_rec.container_id);
END LOOP;


END UPDATEBATCHINGFIELDS;

PROCEDURE UpdateBatchFields(
pBatchField1  IN Inv_Batch_type.Batch_Type%TYPE,
pBatchField2  IN Inv_Batch_type.Batch_Type%TYPE,
pBatchField3  IN Inv_Batch_type.Batch_Type%TYPE)
AS
   l_Sql VARCHAR(5000);
BEGIN
      IF pBatchField1 IS NOT NULL THEN
              l_Sql := ' UPDATE INV_BATCH_TYPE SET ' ;
              l_Sql := l_Sql || ' BATCH_TYPE = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ORDER = :2' ;
              EXECUTE IMMEDIATE l_Sql USING pBatchField1, 1;
        END IF ;
        IF pBatchField2 IS NOT NULL THEN
              l_Sql := ' UPDATE INV_BATCH_TYPE SET ' ;
              l_Sql := l_Sql || ' BATCH_TYPE = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ORDER = :2' ;
              EXECUTE IMMEDIATE l_Sql USING pBatchField2, 2;
        END IF ;
        IF pBatchField3 IS NOT NULL THEN
              l_Sql := ' UPDATE INV_BATCH_TYPE SET ' ;
              l_Sql := l_Sql || ' BATCH_TYPE = :1'  ;
              l_Sql := l_Sql || ' WHERE BATCH_ORDER = :2' ;
              EXECUTE IMMEDIATE l_Sql USING pBatchField3, 3;
        END IF ;

END UpdateBatchFields;

PROCEDURE GETBATCHFIELD
        (	p_RS OUT CURSOR_TYPE) AS
   	BEGIN
   		OPEN p_RS FOR
        SELECT * FROM Inv_Batch_type  ;
   	END GETBATCHFIELD;
 	FUNCTION GetBatchRegFieldCount
		(p_RequestID IN INV_REQUESTS.REQUEST_ID%TYPE) RETURN integer
    IS
    FieldCount integer;
    BEGIN
      select count(*) INTO FieldCount 
      from inv_container_batch_fields TR3 
      where 
      TR3.BATCH_TYPE_ID_FK= (select tr2.batch_type from inv_requests TR, inv_container_batches TR2 where  tr.request_id=p_RequestID and tr.batch_id_fk=tr2.batch_id)
      and (tr3.field_name='REG_ID_FK' or tr3.field_name='BATCH_NUMBER_FK') ;
    return FieldCount;
    END GetBatchRegFieldCount;

	FUNCTION GetBatchRegBatchvalues
		(p_RequestID IN INV_REQUESTS.REQUEST_ID%TYPE,
    pReturnVal varchar2) RETURN integer
    is
    regfieldVal integer;
    Begin
        select 
      decode(tr3.sort_order,1, tr2.batch_field_1, decode(tr3.sort_order,2, tr2.batch_field_2,tr2.batch_field_3)  )  INTO regfieldVal
       from inv_requests TR, inv_container_batches TR2 ,  inv_container_batch_fields TR3 
       where tr.request_id=p_RequestID 
       and tr.batch_id_fk=tr2.batch_id
       and TR3.BATCH_TYPE_ID_FK=tr2.batch_type
       and tr3.field_name=pReturnVal;
    return regfieldVal;   
    END GetBatchRegBatchvalues;

 FUNCTION GetRegBatchid ( pBatchid IN INV_CONTAINER_BATCHES.BATCH_ID %TYPE) RETURN VARCHAR2
    AS
      l_Sql VARCHAR(5000);      
      RegID inv_container_batch_fields.FIELD_NAME%TYPE;
      RegIDCol inv_container_batch_fields.SORT_ORDER%TYPE;
      BatchNum inv_container_batch_fields.FIELD_NAME%TYPE;
      BatchNumCol inv_container_batch_fields.SORT_ORDER%TYPE;
      ReturnType VARCHAR2(100):=NULL;
      ReturnVal VARCHAR2(100):=NULL;
    BEGIN
        select  ICB1.FIELD_NAME , ICB1.SORT_ORDER ,  ICB2.FIELD_NAME , ICB2.SORT_ORDER  INTO RegID, RegIDCol, BatchNum,BatchNumCol
        FROM inv_container_batch_fields ICB1, inv_container_batch_fields ICB2
        where ICB1.batch_type_id_fk = ICB2.batch_type_id_fk(+)
        and ICB1.field_name= 'REG_ID_FK'
        and ICB2.field_name(+)= 'BATCH_NUMBER_FK'
        and ICB1.batch_type_id_fk=(select batch_type from inv_container_batches where batch_id=pBatchid) ;        
        l_Sql :='';      
         IF RegID is not null and BatchNum is not null THEN           
              l_Sql :='SELECT DISTINCT ';
              l_Sql := l_Sql || ' regbatchid  ';
              l_Sql := l_Sql || ' from inv_vw_reg_batches, inv_container_batches '  ;
              l_Sql := l_Sql || ' where inv_container_batches.batch_field_'|| RegIDCol ||' = inv_vw_reg_batches.regid'  ;
              l_Sql := l_Sql || '  and inv_container_batches.batch_field_'|| BatchNumCol ||' = inv_vw_reg_batches.batchnumber'  ;
              l_Sql := l_Sql || '  and inv_container_batches.batch_id='''|| pBatchid ||''''  ; 
              ReturnType := 'FULLREGNUM';
          ELSE
            IF RegID  is not null and BatchNum is null  then
                l_Sql :='SELECT DISTINCT ';
                l_Sql := l_Sql || ' regnumber ';
                l_Sql := l_Sql || ' from inv_vw_reg_batches, inv_container_batches '  ;
                l_Sql := l_Sql || ' where inv_container_batches.batch_field_'|| RegIDCol ||'= inv_vw_reg_batches.regid'  ;
                l_Sql := l_Sql || ' and inv_container_batches.batch_id='''|| pBatchid ||''''  ;    
                ReturnType := 'REGNUM';
            END IF;

        END IF;
        If  l_Sql ='' then  
            ReturnVal:=null;
        ELSE 
            EXECUTE IMMEDIATE l_Sql INTO  ReturnVal;
        END IF;
        RETURN ReturnType || ':' || ReturnVal;
        exception
        when NO_DATA_FOUND
        then return null;
    End GetRegBatchid;
    Function CheckBatchModifications
      (pBatchType IN Inv_Batch_type.Batch_Type%TYPE,
       p_field1 inv_container_batch_fields.field_name%TYPE,
			 p_field2 inv_container_batch_fields.field_name%TYPE,
			 p_field3 inv_container_batch_fields.field_name%TYPE,
			 p_displayName1 inv_container_batch_fields.display_name%TYPE,
			 p_displayName2 inv_container_batch_fields.display_name%TYPE,
			 p_displayName3 inv_container_batch_fields.display_name%TYPE
      ) return integer
      AS
       v_field1 inv_container_batch_fields.field_name%TYPE;
			 v_field2 inv_container_batch_fields.field_name%TYPE;
			 v_field3 inv_container_batch_fields.field_name%TYPE;
       v_displayName1 inv_container_batch_fields.display_name%TYPE;
			 v_displayName2 inv_container_batch_fields.display_name%TYPE;
			 v_displayName3 inv_container_batch_fields.display_name%TYPE;
       v_returnval integer :=0;
      
      Begin 
       
         select B1.field_name,B1.Display_Name, B2.field_name,B2.Display_Name, B3.field_name,B3.Display_Name into v_field1,v_displayName1, v_field2, v_displayName2, v_field3, v_displayName3 
         From (select * from inv_container_batch_fields where batch_type_id_fk= pBatchType AND SORT_ORDER=1) B1,
        (select * from inv_container_batch_fields where batch_type_id_fk= pBatchType AND SORT_ORDER=2) B2,
        (select * from inv_container_batch_fields where batch_type_id_fk= pBatchType AND SORT_ORDER=3) B3
        WHERE b1.batch_type_id_fk=b2.batch_type_id_fk(+) AND b1.batch_type_id_fk=b3.batch_type_id_fk(+);
        if NVL(v_displayName1,0)<>NVL(P_DISPLAYNAME1,0) or NVL(v_displayName2,0)<>NVL(P_DISPLAYNAME2,0) or NVL(v_displayName3,0)<>NVL(P_DISPLAYNAME3,0) then
          v_returnval :=1;
         end if;
        if (NVL(p_field1,0) <>NVL(v_field1,0) or NVL(p_field2,0)<>NVL(v_field2, 0) or NVL(p_field3,0)<> NVL(v_field3,0)) then
          v_returnval :=2;
         end if;
        
        return v_returnval;
        exception
        when NO_DATA_FOUND
        then return 2;
       End CheckBatchModifications;
END BATCH;
/
show errors;
