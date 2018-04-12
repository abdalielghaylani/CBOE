CREATE OR REPLACE PACKAGE BODY "DOCS" IS

  -- Inserts NEW document record
  FUNCTION INSERTDOC(
    p_TableName INV_DOCS.TABLE_NAME%TYPE
    , p_FieldName INV_DOCS.FIELD_NAME%TYPE
    , P_FieldValue INV_DOCS.FIELD_VALUE%TYPE
    , p_DocType INV_DOCS.DOC_TYPE_ID_FK%TYPE
    , p_Doc INV_DOCS.DOC%TYPE
  ) RETURN INV_DOCS.DOC_ID%TYPE
  IS
    l_DocID INV_DOCS.DOC_ID%TYPE;
    l_checkExisting NUMBER;

  BEGIN

    SELECT COUNT(*) INTO l_checkExisting 
    FROM INV_DOCS WHERE TABLE_NAME=p_TableName 
    AND FIELD_NAME = P_FieldName 
    AND FIELD_VALUE = P_FieldValue 
    AND DOC_TYPE_ID_FK = p_DocType ;

    IF l_checkExisting = 0 THEN
    	INSERT INTO INV_DOCS (
    		  TABLE_NAME
    		, FIELD_NAME
    		, FIELD_VALUE
    		, DOC_TYPE_ID_FK
    		, DOC
    		, DATE_CREATED
    	) VALUES (
    		  p_TableName
    		, p_FieldName
    		, p_FieldValue
    		, p_DocType
    		, p_Doc            
    		, SYSDATE 
    	) RETURNING DOC_ID into l_DocID ;
    ELSE
        UPDATE INV_DOCS SET DOC = p_Doc 
        WHERE TABLE_NAME=p_TableName 
        AND FIELD_NAME = P_FieldName 
        AND FIELD_VALUE = p_FieldValue
        AND DOC_TYPE_ID_FK = p_DocType
        RETURNING DOC_ID into l_DocID ;
        --SELECT DOC_ID INTO l_DocID 
        --FROM INV_DOCS WHERE TABLE_NAME=p_TableName AND FIELD_NAME = P_FieldName AND FIELD_VALUE = P_FieldValue ;
    END IF;

    RETURN l_DocID ;
  
  END INSERTDOC ;

  -- Selects and returns existing document
  PROCEDURE GETDOC(
    p_DocID INV_DOCS.DOC_ID%TYPE,
    O_RS OUT CURSOR_TYPE
    ) AS
  BEGIN
    OPEN O_RS FOR  

      SELECT TABLE_NAME, FIELD_NAME, FIELD_VALUE, DATE_CREATED, DOC 
      FROM INV_DOCS 
      WHERE DOC_ID = p_DocID ;
  
  END GETDOC ;

END "DOCS" ;

/

show errors;