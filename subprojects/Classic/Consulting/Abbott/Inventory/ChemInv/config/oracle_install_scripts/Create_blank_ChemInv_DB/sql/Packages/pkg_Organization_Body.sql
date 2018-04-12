CREATE OR REPLACE PACKAGE BODY ORGANIZATION

    IS

	FUNCTION CREATEORGANIZATION
        (
        p_OrgName in INV_ORG_UNIT.ORG_NAME%TYPE ,
        p_OrgType in INV_ORG_UNIT.ORG_TYPE_ID_FK%TYPE,
        p_Users in VARCHAR2,
        p_Roles in VARCHAR2
        ) RETURN INV_ORG_UNIT.ORG_UNIT_ID%TYPE
	IS
        l_NewOrgUnitID INV_ORG_UNIT.ORG_UNIT_ID%TYPE ;
        l_UserIDs STRINGUTILS.t_char ;
        l_RoleIDs STRINGUTILS.t_char ;
	BEGIN

		INSERT INTO INV_ORG_UNIT
			(ORG_NAME, ORG_TYPE_ID_FK)
		VALUES
			(p_OrgName, p_OrgType) RETURNING ORG_UNIT_ID INTO l_NewOrgUnitID ;

        /* Insert Users into group */
        IF p_Users is not NULL THEN
    	    l_UserIDs := STRINGUTILS.split(p_Users, ',') ;
          FOR i in l_UserIDs.First..l_UserIDs.Last
          LOOP
            EXECUTE IMMEDIATE 
            'INSERT INTO INV_ORG_USERS (USER_ID_FK,ORG_UNIT_ID_FK) VALUES (''' || l_UserIDs(i) || ''',' || l_NewOrgUnitID || ')' ;
          END LOOP ;
        END IF;

        /* Insert Roles into group */
        IF p_Roles is not NULL THEN
    	    l_RoleIDs := STRINGUTILS.split(p_Roles, ',') ;
          FOR i in l_RoleIDs.First..l_RoleIDs.Last
          LOOP
            EXECUTE IMMEDIATE 
            'INSERT INTO INV_ORG_ROLES (ROLE_ID_FK,ORG_UNIT_ID_FK) VALUES (''' || l_RoleIDs(i) || ''',' || l_NewOrgUnitID || ')' ;
          END LOOP ;
        END IF;

		RETURN l_NewOrgUnitID ;
	END  CREATEORGANIZATION ;

	FUNCTION DELETEORGANIZATION
		(p_OrgUnitID IN INV_ORG_UNIT.ORG_UNIT_ID%TYPE) RETURN INTEGER
	IS
        cntChildren integer;
	begin
         
        SELECT COUNT(*) INTO cntChildren FROM INV_REQUESTS
        WHERE org_unit_id_fk=p_OrgUnitID ;

        IF cntChildren > 0 THEN
            RETURN -1 ;
        ELSE
            DELETE FROM INV_ORG_USERS IOU WHERE IOU.ORG_UNIT_ID_FK = p_OrgUnitID ; 
            DELETE FROM INV_ORG_ROLES IOR WHERE IOR.ORG_UNIT_ID_FK = p_OrgUnitID ; 
            DELETE FROM INV_ORG_UNIT WHERE ORG_UNIT_ID = p_OrgUnitID ;
            RETURN p_OrgUnitID ;
        END IF;

	END DELETEORGANIZATION ;

	FUNCTION UPDATEORGANIZATION
		(
        p_OrgUnitID IN INV_ORG_UNIT.ORG_UNIT_ID%TYPE , 
        p_OrgName IN INV_ORG_UNIT.ORG_NAME%TYPE ,
        p_OrgType IN INV_ORG_UNIT.ORG_TYPE_ID_FK%TYPE,
        P_Users in VARCHAR2,
        P_Roles in VARCHAR2
        ) RETURN INV_ORG_UNIT.ORG_UNIT_ID%TYPE
	IS
        l_UserIDs STRINGUTILS.t_char ;
        l_RoleIDs STRINGUTILS.t_char ;
	BEGIN
        
        UPDATE INV_ORG_UNIT SET
               ORG_NAME = p_OrgName , 
               ORG_TYPE_ID_FK = p_OrgType
        WHERE ORG_UNIT_ID = p_OrgUnitID ;

        DELETE FROM INV_ORG_USERS WHERE ORG_UNIT_ID_FK = p_OrgUnitID ;
        DELETE FROM INV_ORG_ROLES WHERE ORG_UNIT_ID_FK = p_OrgUnitID ;

        /* Insert Users into group */
        IF p_Users is not NULL THEN
    	    l_UserIDs := STRINGUTILS.split(p_Users, ',') ;
          FOR i in l_UserIDs.First..l_UserIDs.Last
          LOOP
            EXECUTE IMMEDIATE 
            'INSERT INTO INV_ORG_USERS (USER_ID_FK,ORG_UNIT_ID_FK) VALUES (''' || l_UserIDs(i) || ''',' || p_OrgUnitID || ')' ;
          END LOOP ;
        END IF;

        /* Insert Roles into group */
        IF p_Roles is not NULL THEN
    	    l_RoleIDs := STRINGUTILS.split(p_Roles, ',') ;
          FOR i in l_RoleIDs.First..l_RoleIDs.Last
          LOOP
            EXECUTE IMMEDIATE 
            'INSERT INTO INV_ORG_ROLES (ROLE_ID_FK,ORG_UNIT_ID_FK) VALUES (''' || l_RoleIDs(i) || ''',' || p_OrgUnitID || ')' ;
          END LOOP ;
        END IF;


		RETURN p_OrgUnitID ;

	END UPDATEORGANIZATION ;

END ORGANIZATION ;

/

show errors;