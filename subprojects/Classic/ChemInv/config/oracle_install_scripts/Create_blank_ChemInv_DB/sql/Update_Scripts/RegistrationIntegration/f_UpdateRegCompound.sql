CREATE OR REPLACE
FUNCTION UPDATEREGCOMPOUND
(
    p_reg_id_fk IN INV_COMPOUNDS.REG_ID_FK%TYPE,
    p_batch_number_fk IN INV_COMPOUNDS.BATCH_NUMBER_FK%TYPE
)
    RETURN INV_COMPOUNDS.COMPOUND_ID%TYPE IS

    l_CompoundID INV_COMPOUNDS.COMPOUND_ID%TYPE;
    l_SubstanceName INV_COMPOUNDS.SUBSTANCE_NAME%TYPE;
    l_CASNumber INV_COMPOUNDS.CAS%TYPE;
    l_MolWeight iNV_COMPOUNDS.MOLECULAR_WEIGHT%TYPE;

BEGIN
    BEGIN
        SELECT COMPOUND_ID INTO l_CompoundID
          FROM INV_COMPOUNDS
          WHERE REG_ID_FK = p_reg_id_fk
            AND BATCH_NUMBER_FK = p_batch_number_fk;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN l_CompoundID := NULL;
    END;

    IF l_CompoundID IS NOT NULL THEN
    -- Update the existing record in case the substance name, CAS, or structure (mol weight) have changed
        BEGIN
            SELECT ID INTO l_SubstanceName
              FROM &&REGSCHEMANAME..VW_COMPOUND_IDENTIFIER
              WHERE REGID = p_reg_id_fk
                AND "TYPE" = 0
                AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN l_SubstanceName := NULL;
        END;
  
        BEGIN
            SELECT ID INTO l_CASNumber
              FROM &&REGSCHEMANAME..VW_COMPOUND_IDENTIFIER
              WHERE REGID = p_reg_id_fk
                AND "TYPE" = 1
                AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN l_CASNumber := NULL;
        END;

        BEGIN
            SELECT CSCARTRIDGE.MOLWEIGHT(STRUCTUREAGGREGATION) INTO l_MolWeight
              FROM &&REGSCHEMANAME..VW_MIXTURE      
              WHERE REGID = p_reg_id_fk
                AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN l_MolWeight := 0;
        END;

        IF l_MolWeight IS NULL THEN
          l_MolWeight := 0;
        END IF;

        UPDATE INV_COMPOUNDS
        --set substance_name = decode(l_SubstanceName, NULL,'RegSubstance',l_SubstanceName), CAS = l_CASNumber, molecular_weight = l_MolWeight
        SET SUBSTANCE_NAME = 'Registration_Substance', CAS = null, molecular_weight = 0
          WHERE COMPOUND_ID = l_CompoundID;		
    END IF;

    RETURN l_CompoundID;

END UPDATEREGCOMPOUND;
/
show errors;
