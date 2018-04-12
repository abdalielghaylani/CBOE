CREATE OR REPLACE PACKAGE BODY BATCH

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

	BEGIN
        l_Sql := l_Sql || 'UPDATE INV_CONTAINER_BATCHES SET ' ;
        IF p_batchStatusId IS NULL THEN
            l_Sql := l_Sql || 'batch_status_id_fk = NULL, ' ;
        ELSE
            l_Sql := l_Sql || 'batch_status_id_fk = ' || p_batchStatusId || ', ' ;
        END IF ;
        IF p_MinStockThreshold IS NULL THEN
            l_Sql := l_Sql || 'MINIMUM_STOCK_THRESHOLD = NULL, ' ;
        ELSE
            l_Sql := l_Sql || 'MINIMUM_STOCK_THRESHOLD = ' || p_MinStockThreshold || ', ' ;
        END IF ;
        IF p_Field_1 IS NOT NULL THEN
              l_Sql := l_Sql || 'FIELD_1 = ''' || p_Field_1 || ''', ' ;
        END IF ;
        IF p_Field_2 IS NOT NULL THEN
              l_Sql := l_Sql || 'FIELD_2 = ''' || p_Field_2 || ''', ' ;
        END IF ;
        IF p_Field_3 IS NOT NULL THEN
              l_Sql := l_Sql || 'FIELD_3 = ''' || p_Field_3 || ''', ' ;
        END IF ;
        IF p_Field_4 IS NOT NULL THEN
              l_Sql := l_Sql || 'FIELD_4 = ''' || p_Field_4 || ''', ' ;
        END IF ;
        IF p_Field_5 IS NOT NULL THEN
              l_Sql := l_Sql || 'FIELD_5 = ''' || p_Field_5 || ''', ' ;
        END IF ;
        IF p_Date_1 IS NOT NULL THEN
              l_Sql := l_Sql || 'DATE_1 = ''' || p_Date_1 || ''', ' ;
        END IF ;
        IF p_Date_2 IS NOT NULL THEN
              l_Sql := l_Sql || 'DATE_2 = ''' || p_Date_2 || ''', ' ;
        END IF ;
        l_Sql := l_Sql || ' COMMENTS = ''' || p_Comments || '''' ;
        l_Sql := l_Sql || ' WHERE BATCH_ID = ' || p_BatchID ;

        EXECUTE IMMEDIATE l_Sql;

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

PROCEDURE ClearBatchingFields
AS
BEGIN
  --' delete existing batching  fields
  DELETE inv_container_batch_fields;

  --' delete existing batch references
  UPDATE inv_containers SET batch_id_fk = NULL;

  --' delete existing batches
  DELETE inv_container_batches;

END ClearBatchingFields;

PROCEDURE UPDATEBATCHINGFIELDS(
										p_field1 inv_container_batch_fields.field_name%TYPE,
										p_field2 inv_container_batch_fields.field_name%TYPE,
										p_field3 inv_container_batch_fields.field_name%TYPE,
										p_displayName1 inv_container_batch_fields.display_name%TYPE,
										p_displayName2 inv_container_batch_fields.display_name%TYPE,
										p_displayName3 inv_container_batch_fields.display_name%TYPE)
AS

BEGIN

     ClearBatchingFields;

--'  insert new batching fields
--INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order)  VALUES(p_field1, p_displayName1, 1);
INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order)  VALUES(p_field1, p_displayName1, 1);
IF p_field2 IS NOT NULL THEN
			INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order)  VALUES(p_field2, p_displayName2, 2);
END IF;
IF p_field3 IS NOT NULL THEN
				INSERT INTO inv_container_batch_fields(field_name, display_name, sort_order)  VALUES(p_field3, p_displayName3, 3);
END IF;

--' calculate new batches
FOR container_rec IN (SELECT container_id FROM inv_containers ORDER BY container_id)
LOOP
				updatecontainerbatches(container_rec.container_id);
END LOOP;


END UPDATEBATCHINGFIELDS;



END BATCH;
/
show errors;