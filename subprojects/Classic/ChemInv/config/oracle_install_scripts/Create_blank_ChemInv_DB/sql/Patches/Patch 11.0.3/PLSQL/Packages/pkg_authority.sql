CREATE OR REPLACE PACKAGE Authority AS
    TYPE CURSOR_TYPE IS REF CURSOR;
    
    -- Given the Principal ID attached to an object, PrincipalIsAuthorized returns
    -- 0 or 1 as a boolean to indicate whether the specified user (defaults to the
    -- current user) is authorized to exercise control over the object.  If the
    -- current user is not a valid COE person, 0 is returned.  If the principal ID
    -- is null, 1 is returned.  If the Principal ID is non-null, but not known,
    -- 0 is returned.
    FUNCTION PrincipalIsAuthorized (pObjectPrincipalID &&securitySchemaName..COEPRINCIPAL.PRINCIPAL_ID%TYPE,
                                    pUserID VARCHAR2 DEFAULT NULL)
      RETURN NUMBER;

    -- Given the ID of a Container, ContainerIsAuthorized returns 0 or 1 as a boolean
    -- to indicate whether the specified user (defaults to the current user) is
    -- authorized to exercise control over the container.  If the Container ID is not
    -- known 0 is returned.
    FUNCTION ContainerIsAuthorized (pContainerID INV_CONTAINERS.CONTAINER_ID%TYPE,
                                    pUserID VARCHAR2 DEFAULT NULL)
      RETURN NUMBER;

    -- Given the ID of a Plate, PlateIsAuthorized returns 0 or 1 as a boolean to indicate
    -- whether the specified user (defaults to the current user) is authorized to exercise
    -- control over the plate.  If the Plate ID is not known 0 is returned.
    FUNCTION PlateIsAuthorized (pPlateID INV_PLATES.PLATE_ID%TYPE,
                                pUserID VARCHAR2 DEFAULT NULL)
      RETURN NUMBER;
      
    -- Given the ID of a Location, LocationIsAuthorized returns 0 or 1 as a boolean to indicate
    -- whether the specified user (defaults to the current user) is authorized to exercise
    -- control over the plate.  If the Location ID is not known 0 is returned.
    FUNCTION LocationIsAuthorized (pLocationID INV_LOCATIONS.Location_ID%TYPE,
                                   pUserID VARCHAR2 DEFAULT NULL)
      RETURN NUMBER;

    -- Returns a cursor with data concering the Principals that are accessible (can be used by)
    -- the specified user (defaults to the current user).  The cursor includes the PRINCIPAL_ID
    -- and the PRINCIPAL_NAME.  The latter is a distinguishing text value.  Currently returns
    -- the group(s) of the user and any sub-groups.
    PROCEDURE getAccessiblePrincipals(pRS OUT CURSOR_TYPE,
                                      pUserID VARCHAR2 DEFAULT NULL);

    -- Function version of getAccessiblePrincipals
    FUNCTION retAccessiblePrincipals(pUserID VARCHAR2 DEFAULT NULL)
      RETURN CURSOR_TYPE;

    -- isValidLocation determines whether pLocationID is the ID of a valid location for the plate
    -- or container indicated by pTargetObject and pTargetObjectID.  pTargetObject is either 'container' 
    -- or 'plate' and pTargetObjectID is the ID of the container or plate. If pTargetObject is NULL then pTargetObjectID will contain LocationTypeID.  pIsValide returns 0 or 1 as a
    -- boolean to indicate the result.  The special locations with ID 1, 2, 3, or 4 are always valid.
   FUNCTION isValidLocationType (pLocationID in INV_LOCATIONS.LOCATION_ID%TYPE,
                                pTargetObject in varchar2,
                                pTargetObjectID in Number
                               ) 
		RETURN number;
    --  Return the location type name given a location type ID
    FUNCTION GetLocationtype (pLocationID INV_LOCATION_TYPES.LOCATION_TYPE_ID%TYPE)
      RETURN varchar2;
      
    -- Return the user ID or group name for a principal
    FUNCTION GetOwnership (pPrincipalID COEDB.COEPRINCIPAL.PRINCIPAL_ID%TYPE)
      RETURN varchar2 ;
    END Authority;
/
show error

CREATE OR REPLACE PACKAGE BODY Authority AS

    -- Given the principal ID attached to an object, PrincipalIsAuthorized returns 0 or 1 as a boolean
    -- to indicate whether the current user is authorized to exercise control over the object.  If the
    -- current user is not a valid COE person, 0 is returned.  If the principal ID is not known, 0 is returned.
    -- If the principla ID is null, 1 is returned.
    FUNCTION PrincipalIsAuthorized (pObjectPrincipalID &&securitySchemaName..COEPRINCIPAL.PRINCIPAL_ID%TYPE,
                                    pUserID VARCHAR2 DEFAULT NULL)
      RETURN NUMBER IS
        vObjectPersonID &&securitySchemaName..COEPRINCIPAL.PERSON_ID%TYPE;
        vObjectGroupID &&securitySchemaName..COEPRINCIPAL.GROUP_ID%TYPE;
        vUserPersonID &&securitySchemaName..PEOPLE.PERSON_ID%TYPE;
        vCheck NUMBER;
        vAnswer NUMBER;

        CURSOR cParentGroupCheck(aPersonID NUMBER, aObjectGroupID NUMBER) IS
          SELECT 1
            FROM (SELECT GROUP_ID,GROUP_NAME FROM &&securitySchemaName..COEGROUP
                    CONNECT BY PRIOR PARENT_GROUP_ID = GROUP_ID
                    START WITH GROUP_ID = aObjectGroupID) GG,
                  &&securitySchemaName..COEGROUPPEOPLE GP
            WHERE GG.GROUP_ID = GP.GROUP_ID
              AND GP.PERSON_ID = aPersonID;              

        CURSOR cPersonParentGroupCheck(aPersonID NUMBER, aObjectPersonID NUMBER) IS
          SELECT 1
            FROM (SELECT GROUP_ID,GROUP_NAME FROM &&securitySchemaName..COEGROUP
                    CONNECT BY PRIOR PARENT_GROUP_ID = GROUP_ID
                    START WITH GROUP_ID IN (SELECT GP2.GROUP_ID FROM &&securitySchemaName..COEGROUPPEOPLE GP2
                                              WHERE GP2.PERSON_ID = aObjectPersonID)) GG,
                  &&securitySchemaName..COEGROUPPEOPLE GP
            WHERE GG.GROUP_ID = GP.GROUP_ID
              AND GP.PERSON_ID = aPersonID;              
        
    BEGIN
        vAnswer := 0;
        BEGIN
            IF pUserID IS NULL THEN
                SELECT PERSON_ID
                  INTO vUserPersonID
                  FROM &&securitySchemaName..PEOPLE
                  WHERE USER_ID = USER;
            ELSE
                SELECT PERSON_ID
                  INTO vUserPersonID
                  FROM &&securitySchemaName..PEOPLE
                  WHERE USER_ID = pUserID;
            END IF;
            IF pObjectPrincipalID IS NULL THEN
                vAnswer := 1;
            ELSE
                SELECT PERSON_ID, GROUP_ID
                  INTO vObjectPersonID,vObjectGroupID
                  FROM &&securitySchemaName..COEPRINCIPAL
                  WHERE PRINCIPAL_ID = pObjectPrincipalID;
                IF vObjectGroupID IS NOT NULL THEN
                    OPEN cParentGroupCheck(vUserPersonID,vObjectGroupID);
                    FETCH cParentGroupCheck INTO vCheck;
                    IF cParentGroupCheck%FOUND THEN
                        vAnswer := 1;
                    END IF;
                    CLOSE cParentGroupCheck;
                ELSIF vObjectPersonID IS NOT NULL THEN
                    OPEN cPersonParentGroupCheck(vUserPersonID, vObjectPersonID);
                    FETCH cPersonParentGroupCheck INTO vCheck;
                    IF cPersonParentGroupCheck%FOUND THEN
                        vAnswer := 1;
                    END IF;
                    CLOSE cPersonParentGroupCheck;
                END IF;
            END IF;
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL;
        END;
        RETURN vAnswer;
    END PrincipalIsAuthorized;

    FUNCTION ContainerIsAuthorized (pContainerID INV_CONTAINERS.CONTAINER_ID%TYPE,
                                    pUserID VARCHAR2 DEFAULT NULL)
      RETURN NUMBER IS
        vPrincipalID INV_CONTAINERS.PRINCIPAL_ID_FK%TYPE;
        vReturnValue number;
    BEGIN
        vReturnValue :=0;
        BEGIN
            SELECT PRINCIPAL_ID_FK
              INTO vPrincipalID
              FROM INV_CONTAINERS
              WHERE CONTAINER_ID = pContainerID;
            if vPrincipalID is not null then 
                vReturnValue :=PrincipalIsAuthorized(vPrincipalID,pUserID);
                if vReturnValue is NULL  then
                    vReturnValue :=0;
                end if;
            else
               vReturnValue :=1;
            end if;   
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL;
        END;
        RETURN vReturnValue; 
    END ContainerIsAuthorized;

    FUNCTION PlateIsAuthorized (pPlateID INV_PLATES.PLATE_ID%TYPE,
                                pUserID VARCHAR2 DEFAULT NULL)
      RETURN NUMBER IS
        vPlateMapFlag INV_PLATES.IS_PLATE_MAP%TYPE;
        vPrincipalID INV_PLATES.PRINCIPAL_ID_FK%TYPE;
    BEGIN
        BEGIN
            SELECT IS_PLATE_MAP,PRINCIPAL_ID_FK
              INTO vPlateMapFlag,vPrincipalID
              FROM INV_PLATES
              WHERE PLATE_ID = pPlateID;
            IF vPlateMapFlag = 0 THEN
                RETURN PrincipalIsAuthorized(vPrincipalID,pUserID);
            ELSE
                RETURN 1;
            END IF;
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 0;
        END;
    END PlateIsAuthorized;

     
    FUNCTION LocationIsAuthorized (pLocationID INV_LOCATIONS.Location_ID%TYPE,
                                   pUserID VARCHAR2 DEFAULT NULL)
      RETURN NUMBER IS
        vPrincipalID &&securitySchemaName..COEPRINCIPAL.PRINCIPAL_ID%TYPE;
    BEGIN
        IF pLocationID=1 or pLocationID=2 or pLocationID=3 or pLocationID=4 THEN
            RETURN 1;
        ELSE
            BEGIN
                SELECT PRINCIPAL_ID_FK
                  INTO vPrincipalID
                  FROM INV_LOCATIONS
                  WHERE Location_ID = pLocationID;
                RETURN PrincipalIsAuthorized(vPrincipalID,pUserID);
            EXCEPTION
            WHEN NO_DATA_FOUND THEN
                RETURN 0;
            END;
        END IF;
    END LocationIsAuthorized;
 
    PROCEDURE getAccessiblePrincipals(pRS OUT CURSOR_TYPE,
                                      pUserID VARCHAR2 DEFAULT NULL) IS
        vUserPersonID &&securitySchemaName..PEOPLE.PERSON_ID%TYPE;
    BEGIN
        IF pUserID IS NULL THEN
            SELECT PERSON_ID
              INTO vUserPersonID
              FROM &&securitySchemaName..PEOPLE
              WHERE USER_ID = USER;
        ELSE
            SELECT PERSON_ID
              INTO vUserPersonID
              FROM &&securitySchemaName..PEOPLE
              WHERE USER_ID = pUserID;
        END IF;
        OPEN pRS FOR
          SELECT DISTINCT P.PRINCIPAL_ID,
                          G.GROUP_NAME AS PRINCIPAL_NAME
            FROM &&securitySchemaName..COEGROUP G,
                 &&securitySchemaName..COEPRINCIPAL P
            WHERE P.GROUP_ID = G.GROUP_ID
            CONNECT BY PRIOR G.GROUP_ID = G.PARENT_GROUP_ID
            START WITH G.GROUP_ID IN (SELECT GP2.GROUP_ID FROM &&securitySchemaName..COEGROUPPEOPLE GP2
                                      WHERE GP2.PERSON_ID = vUserPersonID);
    END getAccessiblePrincipals;

    FUNCTION retAccessiblePrincipals (pUserID VARCHAR2 DEFAULT NULL)
      RETURN CURSOR_TYPE IS
        lRS CURSOR_TYPE;
    BEGIN
        getAccessiblePrincipals(lRS,pUserID);
        RETURN lRS;
    END retAccessiblePrincipals;

    FUNCTION isValidLocationType (pLocationID in INV_LOCATIONS.LOCATION_ID%TYPE,
                                pTargetObject in varchar2,
                                pTargetObjectID in Number
                               ) 
		RETURN number 
		IS
			vAnswer NUMBER;
			LOCCount NUMBER;
			LocationTypeID INV_LOCATION_TYPES.LOCATION_TYPE_ID%TYPE; 
    BEGIN
	   vAnswer := 0;
	   LOCCount:=0;

	 
	 IF pLocationID=1 or pLocationID=2 or pLocationID=3 or pLocationID=4 THEN
		  vAnswer:= 1;
	  ELSE
	 
		   IF pTargetObject IS NOT NULL THEN
				  IF pTargetObject='container' THEN
					Select Inv_Containers.LOCATION_TYPE_ID_FK into LocationTypeID FROM INV_CONTAINERS  where inv_Containers.CONTAINER_ID=pTargetObjectID;
				  END IF;
				  IF pTargetObject='plate' THEN
					Select INV_PLATES.LOCATION_TYPE_ID_FK into LocationTypeID FROM INV_PLATES where INV_PLATES.PLATE_ID=pTargetObjectID;
				  END IF;
		   ELSE
		   --if pTargetObject is null(blank parameter in javascript) then pTargetObjectID act as location type id
		   LocationTypeID:=pTargetObjectID;
		  END IF;
	      
		   IF LocationTypeID IS NULL THEN
			vAnswer:= 1;
		   ELSE 
		   SELECT COUNT(*) INTO LOCCount 
		   FROM INV_LOCATIONS 
		   WHERE LOCATION_ID = pLocationID
		   AND  LOCATION_TYPE_ID_FK = LocationTypeID ;
		   END IF;
	       
		   IF LOCCount >0 THEN
			vAnswer :=1;  
		   END IF;
	 END IF;
  Return vAnswer;
  END isValidLocationType;

    FUNCTION GetLocationtype (pLocationID INV_LOCATION_TYPES.LOCATION_TYPE_ID%TYPE)
      RETURN varchar2 IS
        vLocationType INV_LOCATION_TYPES.LOCATION_TYPE_NAME%TYPE;
    BEGIN
        SELECT LOCATION_TYPE_NAME INTO vLocationType 
          FROM INV_LOCATION_TYPES 
          WHERE LOCATION_TYPE_ID = pLocationID;
        RETURN vLocationType;
     END GetLocationtype;

    FUNCTION GetOwnership (pPrincipalID COEDB.COEPRINCIPAL.PRINCIPAL_ID%TYPE)
      RETURN VARCHAR2 IS
        vObjectPersonID &&securitySchemaName..COEPRINCIPAL.PERSON_ID%TYPE;
        vObjectGroupID  &&securitySchemaName..COEPRINCIPAL.GROUP_ID%TYPE;
        vOwnership      VARCHAR2(100);
    Begin
      IF pPrincipalID IS null THEN
          vOwnership := '';
      ELSE
            SELECT PERSON_ID, GROUP_ID
              INTO vObjectPersonID,vObjectGroupID
              FROM &&securitySchemaName..COEPRINCIPAL
              WHERE PRINCIPAL_ID = pPrincipalID;
            IF vObjectPersonID IS NOT NULL THEN 
                SELECT P.USER_ID INTO vOwnership  
                  FROM &&securitySchemaName..COEPRINCIPAL PR,
                       &&securitySchemaName..PEOPLE P
                  WHERE PR.PERSON_ID = P.PERSON_ID
                    AND PR.PRINCIPAL_ID = pPrincipalID;
            ELSIF vObjectGroupID IS NOT NULL THEN
                SELECT G.GROUP_NAME INTO vOwnership 
                  FROM &&securitySchemaName..COEPRINCIPAL PR,
                       &&securitySchemaName..COEGROUP G
                  WHERE PR.GROUP_ID = G.GROUP_ID
                    AND PR.PRINCIPAL_ID = pPrincipalID;
            END IF;
        END IF;      
       Return vOwnership;
    END GetOwnership;

END Authority;
/
show error
