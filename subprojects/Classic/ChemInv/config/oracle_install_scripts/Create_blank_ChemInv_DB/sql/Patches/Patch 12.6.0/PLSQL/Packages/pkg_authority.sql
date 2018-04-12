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
                                    pUserID VARCHAR2 DEFAULT NULL, 
                                    pBarcode INV_CONTAINERS.Barcode%TYPE DEFAULT NULL)
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
    FUNCTION GetOwnership (pPrincipalID &&securitySchemaName..COEPRINCIPAL.PRINCIPAL_ID%TYPE)
      RETURN varchar2 ;

    -- Retrun the Owner's principal id
    Function GetPrincipalID(pObjectID varchar2,
                            pObjectType varchar2)
     Return varchar2;
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
		vCount NUMBER;

        CURSOR cParentGroupCheck(aPersonID NUMBER, aObjectGroupID NUMBER) IS
          SELECT 1
            FROM (SELECT GROUP_ID,GROUP_NAME FROM &&securitySchemaName..COEGROUP
                    CONNECT BY PRIOR PARENT_GROUP_ID = GROUP_ID
                    START WITH GROUP_ID = aObjectGroupID) GG,
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
                  WHERE upper(USER_ID) = upper(pUserID);
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
                ELSIF vObjectPersonID IS NOT NULL AND vObjectPersonID = vUserPersonID THEN
                    vAnswer := 1;
				ELSIF  vObjectPersonID IS NOT NULL THEN
                     SELECT COUNT(*) INTO vCount 
                     FROM &&securitySchemaName..COEGROUPPEOPLE 
                     WHERE GROUP_ID = 1 
                     AND PERSON_ID = VUSERPERSONID;
					 IF vCount > 0 THEN 
						vAnswer := 1;
					 END IF;	 
                 END IF;
            END IF;
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL;
        END;
        RETURN vAnswer;
    END PrincipalIsAuthorized;

    FUNCTION ContainerIsAuthorized (pContainerID INV_CONTAINERS.CONTAINER_ID%TYPE,
                                    pUserID VARCHAR2 DEFAULT NULL, 
                                    pBarcode INV_CONTAINERS.Barcode%TYPE DEFAULT NULL)
      RETURN NUMBER IS
        vPrincipalID INV_CONTAINERS.PRINCIPAL_ID_FK%TYPE;
        vReturnValue number;
        vContainerID INV_CONTAINERS.CONTAINER_ID%TYPE;
		vLocationAdmin number;
        vLocationID INV_CONTAINERS.LOCATION_ID_FK%TYPE;
    BEGIN
        vReturnValue :=0;
		vLocationAdmin := 0;
        BEGIN
            if pContainerID is NULL and pBarcode is not NULL then 
              select container_id into vContainerID from inv_containers where BARCODE=''||pBarcode||'';
            else
              vContainerID := pContainerID;
            end if;
			SELECT Location_id_fk 
			  INTO vLocationID 
			  FROM inv_containers 
			  WHERE container_id = vContainerID;
            vLocationAdmin := LocationIsAuthorized(vLocationID,  pUserID);
            if vLocationAdmin = 1 then 
                RETURN 1;
            end if;    
            SELECT PRINCIPAL_ID_FK
              INTO vPrincipalID
              FROM INV_CONTAINERS
              WHERE CONTAINER_ID = vContainerID;
            if vPrincipalID is not null then 
                vReturnValue := PrincipalIsAuthorized(vPrincipalID, pUserID);
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
		vLocationAdmin number;
        vLocationID INV_CONTAINERS.LOCATION_ID_FK%TYPE;
    BEGIN
        BEGIN
			SELECT Location_id_fk 
				INTO vLocationID 
				FROM inv_plates 
				where plate_id = pPlateID;
            vLocationAdmin := LocationIsAuthorized(vLocationID,  pUserID);
            IF vLocationAdmin = 1 THEN 
                RETURN 1;
            END IF;  
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
		vIsPublic INV_LOCATIONS.ISPUBLIC%TYPE;
    BEGIN
        IF pLocationID=1 or pLocationID=2 or pLocationID=3 or pLocationID=4 THEN
            RETURN 1;
        ELSE
            BEGIN
                SELECT PRINCIPAL_ID_FK, ISPUBLIC
                  INTO vPrincipalID, vIsPublic
                  FROM INV_LOCATIONS
                  WHERE Location_ID = pLocationID;
                IF vIsPublic=1 THEN
                    RETURN 1; 
				ELSE
                    RETURN PrincipalIsAuthorized(vPrincipalID, pUserID);
                END IF; 
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
	   LOCCount := 0;

	 
	 IF pLocationID=1 or pLocationID=2 or pLocationID=3 or pLocationID=4 THEN
		  vAnswer := 1;
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
	      
		   IF LocationTypeID IS NULL or LocationTypeID = 0 THEN
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

    FUNCTION GetOwnership (pPrincipalID &&securitySchemaName..COEPRINCIPAL.PRINCIPAL_ID%TYPE)
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
	
	Function GetPrincipalID(pObjectID varchar2,
                            pObjectType varchar2)
     Return varchar2 IS
      vPrincipalID &&securitySchemaName..COEPRINCIPAL.PRINCIPAL_ID%TYPE;
     BEGIN
        if (lower(pObjectType) = 'location') then 
          SELECT PRINCIPAL_ID_FK INTO vPrincipalID FROM INV_LOCATIONS WHERE Location_ID = to_number(pObjectID);
       elsif (lower(pObjectType) = 'container') then 
          SELECT PRINCIPAL_ID_FK INTO vPrincipalID FROM INV_CONTAINERS WHERE Container_ID = to_number(pObjectID);
       elsif (lower(pObjectType) = 'plate') then 
          SELECT PRINCIPAL_ID_FK INTO vPrincipalID FROM INV_Plates WHERE Plate_ID = to_number(pObjectID);
       elsif (lower(pObjectType) = 'user') then 
         SELECT PRINCIPAL_ID INTO vPrincipalID FROM  &&securitySchemaName..COEPRINCIPAL PR , &&securitySchemaName..PEOPLE P WHERE PR.person_id = p.person_id AND UPPER(p.user_id)=UPPER(pObjectID);
      elsif (lower(pObjectType) = 'group') then  
         SELECT PRINCIPAL_ID INTO vPrincipalID FROM  &&securitySchemaName..COEPRINCIPAL P , &&securitySchemaName..COEGROUP G WHERE P.group_id= g.group_id AND UPPER(g.group_name)= UPPER('pObjectID');
      else
         RETURN 0;
      end if;
      if vPrincipalID is NULL then 
        vPrincipalID :=0;
     end if;   
      Return vPrincipalID;
      
      EXCEPTION
      WHEN NO_DATA_FOUND THEN
          RETURN 0;
  END GetPrincipalID;  
END Authority;
/
show error
