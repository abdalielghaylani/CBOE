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
               ICB.BATCH_ID
               , ICB.BATCH_FIELD_1
               , ICB.BATCH_FIELD_2
               , ICB.BATCH_FIELD_3
               , ICB.MINIMUM_STOCK_THRESHOLD
               , ICB.COMMENTS
               , ICB.FIELD_1
               , ICB.FIELD_2
               , ICB.FIELD_3
               , ICB.FIELD_4
               , ICB.FIELD_5
               , ICB.DATE_1
               , ICB.DATE_2
        FROM     INV_CONTAINER_BATCHES ICB
        WHERE BATCH_ID = p_BatchID ;

   	END GETBATCHDETAILS;


END BATCH;



/
show errors;