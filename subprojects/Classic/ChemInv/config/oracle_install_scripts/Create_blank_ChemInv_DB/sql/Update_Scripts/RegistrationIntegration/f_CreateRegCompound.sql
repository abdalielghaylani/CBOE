CREATE OR REPLACE
FUNCTION CREATEREGCOMPOUND
(
    p_reg_id_fk IN INV_COMPOUNDS.REG_ID_FK%TYPE,
    p_batch_number_fk IN INV_COMPOUNDS.BATCH_NUMBER_FK%TYPE
)
    RETURN inv_compounds.compound_id%TYPE IS
    
    l_CompoundID INV_COMPOUNDS.COMPOUND_ID%TYPE;
    l_SubstanceName INV_COMPOUNDS.SUBSTANCE_NAME%TYPE;
    l_CASNumber INV_COMPOUNDS.CAS%TYPE;
    l_TempCompoundID INV_COMPOUNDS.COMPOUND_ID%TYPE;
    l_CompoundInternalID INV_COMPOUNDS.CPD_INTERNAL_ID_FK%TYPE;
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
    -- Update the existing record in case the substance name or CAS have changed
        l_TempCompoundID := UpdateRegCompound(p_reg_id_fk, p_batch_number_fk);
    ELSE
    -- Insert new
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

        l_CompoundInternalID := p_reg_id_fk;
        /* This appears to be obsolete, replaced by the line above:
        BEGIN
            SELECT REGNUMBER INTO l_CompoundInternalID  -- formerly cpd_internal_id
              FROM &&REGSCHEMANAME..VW_REGISTRYNUMBER
              WHERE REGID = p_reg_id_fk;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN l_CompoundInternalID := NULL;
        END;
        */

        INSERT INTO INV_COMPOUNDS( SUBSTANCE_NAME, CAS, DENSITY, REG_ID_FK, BATCH_NUMBER_FK, CPD_INTERNAL_ID_FK )
        -- TSM 6/3/08: fix for search form issue where the same reg compound appears twice when searching on CAS, once against 
        -- inv_compounds, the second against inv_vw_reg_batches.
        --values( decode(l_SubstanceName, NULL,'RegSubstance',l_SubstanceName), l_CASNumber, 1.0, p_reg_id_fk, p_batch_number_fk, l_CompoundInternalID )
          VALUES( 'Registration_Substance', null, 1.0, p_reg_id_fk, p_batch_number_fk, l_CompoundInternalID )
          RETURNING COMPOUND_ID INTO l_CompoundID;
    END IF;

    RETURN l_CompoundID;

END CREATEREGCOMPOUND;
/
show errors;
