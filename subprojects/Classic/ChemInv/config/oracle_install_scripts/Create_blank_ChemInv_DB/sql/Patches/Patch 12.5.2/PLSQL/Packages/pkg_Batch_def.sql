create or replace
PACKAGE BATCH
    IS

	TYPE  CURSOR_TYPE IS REF CURSOR;

	FUNCTION UPDATEMINTHRESHOLD
		(
        p_BatchID IN INV_CONTAINER_BATCHES.BATCH_ID%TYPE,
        p_MinStockThreshold IN INV_CONTAINER_BATCHES.MINIMUM_STOCK_THRESHOLD%TYPE
        ) RETURN INV_CONTAINER_BATCHES.BATCH_ID%TYPE ;

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
        ) RETURN INV_CONTAINER_BATCHES.BATCH_ID%TYPE;

	PROCEDURE GETBATCHDETAILS
        (
		p_BatchID IN INV_CONTAINER_BATCHES.BATCH_ID%Type,
		O_RS OUT CURSOR_TYPE) ;

	PROCEDURE GETBATCHFIELDS
        (	p_RS OUT CURSOR_TYPE) ;

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
                    p_displayName3_3 inv_container_batch_fields.display_name%TYPE);

PROCEDURE ClearBatchingFields(pBatchTypeID Inv_Batch_type.Batch_Type%TYPE:=NULL);
PROCEDURE GETBATCHFIELD
        (	p_RS OUT CURSOR_TYPE) ;
PROCEDURE UpdateBatchFields(
          pBatchField1  IN Inv_Batch_type.Batch_Type%TYPE,
          pBatchField2  IN Inv_Batch_type.Batch_Type%TYPE,
          pBatchField3  IN Inv_Batch_type.Batch_Type%TYPE) ;

FUNCTION GetBatchRegFieldCount
		(p_RequestID IN INV_REQUESTS.REQUEST_ID%TYPE) RETURN integer;     
 	
FUNCTION GetBatchRegBatchvalues
		(p_RequestID IN INV_REQUESTS.REQUEST_ID%TYPE,
    pReturnVal varchar2) RETURN integer;     

FUNCTION GetRegBatchid(
           pBatchid IN INV_CONTAINER_BATCHES.BATCH_ID %TYPE) RETURN VARCHAR2;  
Function CheckBatchModifications
      (pBatchType IN Inv_Batch_type.Batch_Type%TYPE,
       p_field1 inv_container_batch_fields.field_name%TYPE,
			 p_field2 inv_container_batch_fields.field_name%TYPE,
			 p_field3 inv_container_batch_fields.field_name%TYPE,
			 p_displayName1 inv_container_batch_fields.display_name%TYPE,
			 p_displayName2 inv_container_batch_fields.display_name%TYPE,
			 p_displayName3 inv_container_batch_fields.display_name%TYPE
      ) return integer;
 
 Function GETBATCHFIELDVAL
          (pFieldVal varChar2,
          pBatchType  inv_container_batches.batch_type%TYPE,
          pSortOrder  INV_CONTAINER_BATCH_FIELDS.sort_order%TYPE
          ) return varchar2;
END BATCH ;
/
show errors;