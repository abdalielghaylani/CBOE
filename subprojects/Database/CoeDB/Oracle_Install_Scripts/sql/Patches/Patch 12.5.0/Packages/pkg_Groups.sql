create or replace TYPE ID_TABLE AS TABLE OF NUMBER;
/

----------------------------------------------------------------
PROMPT >> CREATE PACKAGE Manage_Groups
----------------------------------------------------------------
create or replace
PACKAGE Manage_Groups AS

    TYPE ID_ARRAY IS TABLE OF NUMBER INDEX BY PLS_INTEGER;
    TYPE CURSOR_TYPE IS REF CURSOR;

    -- Returns the group organizations with the hierarchy for each organization under it.
    FUNCTION getGroupOrgHierarchyXML
      RETURN CLOB;

    -- Returns the group hierarchy for an organization
    -- with Group Name and ID.  Default organization is Inventory
    FUNCTION getGroupHierarchyXML (pGroupOrgID COEGROUP.GROUPORG_ID%TYPE) RETURN CLOB;    

    -- Internal function to return an XML fragment with the group hierarchy in a GroupOrg, 
    -- Must be public to function in an SQL statement
    --it does not returns the user information in the xml.
    FUNCTION GroupHierXMLFragWOUser (pGroupOrgID COEGROUP.GROUPORG_ID%TYPE)
      RETURN XMLTYPE;
      
     -- Get group by ID, returns a cursor for a group record that includes:
    -- GROUP_ID, GROUPORG_ID, GROUP_NAME, PARENT_GROUP_ID, LEADER_PERSON_ID
    PROCEDURE getGroup(pGroupID COEGROUP.GROUP_ID%TYPE,
                       pRS OUT CURSOR_TYPE);
    -- Get a cursor with the people in a group.  Returns PEOPLE.*
    -- with additional boolean (0/1) IS_LEADER column
    PROCEDURE getGroupPeople(pGroupID COEGROUP.GROUP_ID%TYPE,
                             pRS OUT CURSOR_TYPE);                             
    
      -- Get cursor with GROUPROLES in a group.  Cursor includes
    -- ROLE_ID and ROLE_NAME
    PROCEDURE getGroupRoles (pGroupID COEGROUP.GROUP_ID%TYPE,
                                pRS OUT CURSOR_TYPE);                                
     -- Get cursor with privsets not assigned to a specified group.
    -- Cursor includes PRIVSET_ID and PRIVSET_NAME
    PROCEDURE getAvailGroupRoles (pGroupID COEGROUP.GROUP_ID%TYPE,
                                     pRS OUT CURSOR_TYPE);
    -- Add a group, return the ID. Default organization is Inventory
    FUNCTION addGroup(pName COEGROUP.GROUP_NAME%TYPE,
                      pParentGroupID  COEGROUP.PARENT_GROUP_ID%TYPE,
                      pGroupOrgID COEGROUP.GROUPORG_ID%TYPE,
                      pLeaderPersonID COEGROUP.LEADER_PERSON_ID%TYPE)
      RETURN COEGROUP.GROUP_ID%TYPE;
      
      -- Set people in a group to a specific list of people, by ID.
    -- People are added and removed as necessary.
    PROCEDURE setGroupPeople(pGroupID COEGROUP.GROUP_ID%TYPE,
                             pPersonIDList ID_ARRAY);
                             
    -- Update group, given ID.  Cannot change organization
    PROCEDURE updateGroup(pGroupID COEGROUP.GROUP_ID%TYPE,
                          pName COEGROUP.GROUP_NAME%TYPE,
                          pParentGroupID COEGROUP.PARENT_GROUP_ID%TYPE,
                          pLeaderPersonID COEGROUP.LEADER_PERSON_ID%TYPE);
    -- Set privsets for a group to a specific list of privsets, by ID.
    -- Privsets are added and removed as necessary.
    PROCEDURE setGroupRoles(pGroupID COEGROUP.GROUP_ID%TYPE,
                               pPrivsetIDList ID_ARRAY);
    -- Delete group, given ID.
    PROCEDURE deleteGroup (pGroupID COEGROUP.GROUP_ID%TYPE);
    
	-- Getting roles assigned to an users which are not grated by a group.
    PROCEDURE GetPersonNonGSRoles (pGroupID COEGROUP.GROUP_ID%TYPE,
              pPersonID PEOPLE.person_id%TYPE,
              pRS OUT CURSOR_TYPE);
	-- Remove all users of a group
    PROCEDURE ClearGroupPeople (pGroupID COEGROUP.GROUP_ID%TYPE);
END;
/

----------------------------------------------------------------
PROMPT >> CREATE PACKAGE Manage_Groups Body
----------------------------------------------------------------

create or replace
PACKAGE BODY Manage_Groups AS

     -- Private function to get the default (inventory) group organization
    FUNCTION getDefaultGroupOrgID (pGroupOrgID COEGROUP.GROUPORG_ID%TYPE)
      RETURN COEGROUP.GROUPORG_ID%TYPE IS
        lOrgID COEGROUP.GROUPORG_ID%TYPE;
    BEGIN
        IF pGroupOrgID IS NULL THEN
            SELECT ORG.GROUPORG_ID
              INTO lOrgID
              FROM COEGROUPORG ORG,
                   PRIVILEGE_TABLES PT
              WHERE APP_NAME = 'Inventory Enterprise' 
                AND PT.PRIVILEGE_TABLE_ID  = ORG.DEFAULT_APP_ID;
        ELSE
            lOrgID := pGroupOrgID;
        END IF;
        RETURN lOrgID;
    END getDefaultGroupOrgID;
    
    FUNCTION getGroupOrgHierarchyXML
      RETURN CLOB IS
        lXML CLOB;
    BEGIN
    --changed the call from GroupHierXMLFrag to GroupHierXMLFragWOUser, as we dont need to show the users in the tree.
        SELECT XMLSERIALIZE (DOCUMENT XMLROOT(
            XMLELEMENT("GROUPORGLIST",
                       XMLELEMENT("GROUPORG",
                                  XMLAttributes(GROUPORG_ID,GROUPORG_NAME),
                                  GroupHierXMLFragWOUser (GROUPORG_ID))
            ), VERSION '1.0', STANDALONE YES) AS CLOB)
        INTO lXML
        FROM COEGROUPORG;
        RETURN lXML;
    END getGroupOrgHierarchyXML;

    FUNCTION getGroupHierarchyXML(pGroupOrgID COEGROUP.GROUPORG_ID%TYPE)
      RETURN CLOB IS
        lXML CLOB;
        lOrgID COEGROUP.GROUPORG_ID%TYPE;
    BEGIN
    --changed the call from GroupHierXMLFrag to GroupHierXMLFragWOUser, as we dont need to show the users in the tree.
        lOrgID := getDefaultGroupOrgID(pGroupOrgID);
        SELECT XMLSERIALIZE (DOCUMENT XMLROOT(
            XMLELEMENT("GROUPLIST",GroupHierXMLFragWOUser (lOrgID)
            ), VERSION '1.0', STANDALONE YES) AS CLOB)
        INTO lXML
        FROM DUAL;
        RETURN lXML;
    END getGroupHierarchyXML;

    -- Internal function to return an XML fragment with the group hierarchy in a GroupOrg
    FUNCTION GroupHierXMLFragWOUser (pGroupOrgID COEGROUP.GROUPORG_ID%TYPE)
      RETURN XMLTYPE IS
        lXML XMLTYPE;
    BEGIN
        SELECT 
            DBMS_XMLGEN.GetXMLType
               (DBMS_XMLGEN.NewContextFromHierarchy
                  ('SELECT LEVEL,
                           XMLElement("GROUP",
                                      XMLAttributes(GROUP_ID,GROUP_NAME))
                   FROM COEGROUP
                   WHERE GROUPORG_ID='||pGroupOrgID||
                   ' START WITH PARENT_GROUP_ID is null
                   CONNECT BY PRIOR GROUP_ID = PARENT_GROUP_ID
                   ORDER SIBLINGS BY GROUP_NAME'))
          INTO lXML
          FROM DUAL;
        RETURN lXML;
    END;
  
   PROCEDURE getGroup(pGroupID COEGROUP.GROUP_ID%TYPE,
                       pRS OUT CURSOR_TYPE) IS
    BEGIN
        OPEN pRS FOR
          SELECT GROUP_ID, GROUPORG_ID, GROUP_NAME, PARENT_GROUP_ID, LEADER_PERSON_ID
            FROM COEGROUP
            WHERE GROUP_ID = pGroupID;
    END getGroup;
    
    PROCEDURE getGroupPeople (pGroupID COEGROUP.GROUP_ID%TYPE,
                              pRS OUT CURSOR_TYPE) IS
        lLeader COEGROUP.LEADER_PERSON_ID%TYPE;
    BEGIN
        SELECT LEADER_PERSON_ID
          INTO lLeader
          FROM COEGROUP
          WHERE GROUP_ID = pGroupID;
        OPEN pRS FOR
          SELECT P.*,
                 TO_NUMBER(DECODE(P.PERSON_ID,lLeader,1,0)) AS IS_LEADER
            FROM PEOPLE P,
                 COEGROUPPEOPLE GP
           WHERE GP.GROUP_ID = pGroupID
              AND P.PERSON_ID = GP.PERSON_ID;
    END;
    
     PROCEDURE getGroupRoles (pGroupID COEGROUP.GROUP_ID%TYPE,
                                pRS OUT CURSOR_TYPE) IS
    BEGIN
        OPEN pRS FOR
         SELECT SR.ROLE_ID, SR.ROLE_NAME
            FROM  COEGROUPROLE GR,
                  COEGROUP GRP,
                 SECURITY_ROLES SR
           WHERE GR.GROUP_ID = pGroupID
              AND GR.GROUP_ID = GRP.GROUP_ID
              AND SR.ROLE_ID = GR.ROLE_ID;
    END;
    
    PROCEDURE getAvailGroupRoles (pGroupID COEGROUP.GROUP_ID%TYPE,
                                     pRS OUT CURSOR_TYPE) IS
    BEGIN
        OPEN pRS FOR
          SELECT SR.ROLE_ID, SR.ROLE_NAME
            FROM SECURITY_ROLES SR
           WHERE SR.ROLE_ID NOT IN
               (SELECT GR.ROLE_ID
                  FROM COEGROUPROLE GR,
                       COEGROUP GRP
                  WHERE GR.GROUP_ID = GRP.GROUP_ID
                    AND GR.GROUP_ID = pGroupID)
                    AND UPPER(SR.ROLE_NAME)<>'CSS_USER';
    END;
    
  FUNCTION addGroup(pName COEGROUP.GROUP_NAME%TYPE,
                      pParentGroupID COEGROUP.PARENT_GROUP_ID%TYPE,
                      pGroupOrgID COEGROUP.GROUPORG_ID%TYPE,
                      pLeaderPersonID COEGROUP.LEADER_PERSON_ID%TYPE)
      RETURN COEGROUP.GROUP_ID%TYPE IS
        lGroupID COEGROUP.GROUP_ID%TYPE;
        lGroupOrgID COEGROUP.GROUPORG_ID%TYPE;
        lCount INTEGER;
        TYPE temp_tab IS VARRAY(1) OF INTEGER ;
    	tempUserList ID_ARRAY;
    BEGIN
        IF pParentGroupID IS NULL THEN
            IF pGroupOrgID IS NULL THEN
                RAISE_APPLICATION_ERROR (-20001,'A root Group must have the Organization specified');
            END IF;
            SELECT COUNT(*) INTO lCount
              FROM COEGROUP
              WHERE PARENT_GROUP_ID IS NULL
                AND GROUPORG_ID = pGroupOrgID;
            IF lCount > 0 THEN
                RAISE_APPLICATION_ERROR (-20001,'Only one root Group is allowed in an Organization');
            END IF;
            lGroupOrgID := pGroupOrgID;
        ELSE
            SELECT GROUPORG_ID
              INTO lGroupOrgID
              FROM COEGROUP
              WHERE GROUP_ID = pParentGroupID;
            IF lGroupOrgID = pGroupOrgID THEN
                NULL;
            ELSE
               RAISE_APPLICATION_ERROR (-20001,'A Group must have the same Organization as it''s parent');
            END IF;
        END IF;
        INSERT INTO COEGROUP(GROUPORG_ID, GROUP_NAME, PARENT_GROUP_ID, LEADER_PERSON_ID)
          VALUES (pGroupOrgID,pName,pParentGroupID,pLeaderPersonID)
          RETURNING GROUP_ID INTO lGroupID;
   	  tempUserList(1):= pLeaderPersonID;
          setGroupPeople(lGroupID,tempUserList);
        RETURN lGroupID; 
        
    END addGroup;
    
    PROCEDURE setGroupPeople(pGroupID COEGROUP.GROUP_ID%TYPE,
                             pPersonIDList ID_ARRAY) IS
        lTable ID_TABLE;
        lNum NUMBER;
    BEGIN
        lTable := ID_TABLE();
        lNum := pPersonIDList.FIRST;
        WHILE lNum IS NOT NULL LOOP
            lTable.EXTEND;
            lTable(lTable.LAST) := pPersonIDList(lNum);
            lNum := pPersonIDList.NEXT(lNum);
        END LOOP;
        DELETE FROM COEGROUPPEOPLE
          WHERE GROUP_ID = pGroupID
          AND PERSON_ID NOT IN (SELECT * FROM TABLE(lTable));
        INSERT INTO COEGROUPPEOPLE (GROUP_ID, PERSON_ID)
          SELECT pGroupID, LIST.*
            FROM (SELECT COLUMN_VALUE FROM TABLE(lTable)) LIST
            WHERE LIST.COLUMN_VALUE NOT IN (SELECT PERSON_ID FROM COEGROUPPEOPLE
                                  WHERE GROUP_ID = pGroupID);
        commit;
        
    END setGroupPeople;
    
    PROCEDURE updateGroup(pGroupID COEGROUP.GROUP_ID%TYPE,
                          pName COEGROUP.GROUP_NAME%TYPE,
                          pParentGroupID COEGROUP.PARENT_GROUP_ID%TYPE,
                          pLeaderPersonID COEGROUP.LEADER_PERSON_ID%TYPE) IS
        lName COEGROUP.GROUP_NAME%TYPE;
        lParent COEGROUP.PARENT_GROUP_ID%TYPE;
        lLeader COEGROUP.LEADER_PERSON_ID%TYPE;
        lCount PLS_INTEGER;
    BEGIN
        SELECT GROUP_NAME,PARENT_GROUP_ID,LEADER_PERSON_ID
          INTO lName, lParent, lLeader
          FROM COEGROUP
          WHERE GROUP_ID = pGroupID;
        IF NOT (lName = pName OR (lName IS NULL AND pName IS NULL))  THEN
            UPDATE COEGROUP
              SET GROUP_NAME = pName
              WHERE GROUP_ID = pGroupID;
        END IF;
        IF NOT (lParent = pParentGroupID OR (lParent IS NULL AND pParentGroupID IS NULL)) THEN
            SELECT COUNT(*) INTO lCount FROM
              (SELECT GROUP_ID FROM COEGROUP
                CONNECT BY PARENT_GROUP_ID = PRIOR GROUP_ID
                START WITH GROUP_ID = pGroupID)
              WHERE GROUP_ID=pParentGroupID;
            IF lCount>0 THEN
                RAISE_APPLICATION_ERROR(-20000,'Group parent cannot be itself or its children.');
            END IF;
            UPDATE COEGROUP
              SET PARENT_GROUP_ID = pParentGroupID
              WHERE GROUP_ID = pGroupID;
        END IF;
        IF NOT (lLeader = pLeaderPersonID OR (lLeader IS NULL AND pLeaderPersonID IS NULL)) THEN
            UPDATE COEGROUP
              SET LEADER_PERSON_ID = pLeaderPersonID
              WHERE GROUP_ID = pGroupID;
        END IF;
    END updateGroup;
    
    PROCEDURE setGroupRoles(pGroupID COEGROUP.GROUP_ID%TYPE,
                               pPrivsetIDList ID_ARRAY) IS
        lTable ID_TABLE;
        lNum NUMBER;        
    BEGIN
        lTable := ID_TABLE();
        lNum := pPrivsetIDList.FIRST;
        WHILE lNum IS NOT NULL LOOP
            lTable.EXTEND;
            lTable(lTable.LAST) := pPrivsetIDList(lNum);
            lNum := pPrivsetIDList.NEXT(lNum);
        END LOOP;        
        DELETE FROM COEGROUPROLE
          WHERE GROUP_ID = pGroupID
            AND ROLE_ID NOT IN (SELECT * FROM TABLE(lTable));
        INSERT INTO COEGROUPROLE (GROUP_ID, ROLE_ID)
          SELECT pGroupID, LIST.COLUMN_VALUE
            FROM (SELECT COLUMN_VALUE FROM TABLE(lTable)) LIST
            WHERE LIST.COLUMN_VALUE NOT IN (SELECT ROLE_ID FROM COEGROUPROLE
                                  WHERE GROUP_ID = pGroupID);
        commit;
        
    END setGroupRoles;
    
    PROCEDURE deleteGroup (pGroupID COEGROUP.GROUP_ID%TYPE) IS
    BEGIN
        DELETE FROM COEGROUP
          WHERE GROUP_ID = pGroupID;
         
    END deleteGroup;
    
    FUNCTION updateRolesGrantedToRole (
      pRoleName       IN   security_roles.role_name%TYPE,
      pRolesGranted   IN   VARCHAR2 := NULL,
      pRolesRevoked   IN   VARCHAR2 := NULL)
      RETURN VARCHAR2
   AS
      source_cursor    INTEGER;
      rows_processed   INTEGER;
   BEGIN
      source_cursor := DBMS_SQL.open_cursor;

      IF (prolesrevoked IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'REVOKE ' || prolesrevoked || ' FROM ' || prolename, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      IF (prolesgranted IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'GRANT ' || prolesgranted || ' TO ' || prolename, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      RETURN '1';
   END updateRolesGrantedToRole;

   FUNCTION updateUsersGrantedARole (
      pRoleName       IN   security_roles.role_name%TYPE,
      pUsersGranted   IN   VARCHAR2 := NULL,
      pUsersRevoked   IN   VARCHAR2 := NULL)
      RETURN VARCHAR2
   AS
      source_cursor    INTEGER;
      rows_processed   INTEGER;
   BEGIN
      source_cursor := DBMS_SQL.open_cursor;

      IF (pUsersRevoked IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'REVOKE ' || pRoleName || ' FROM ' || pUsersRevoked, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      IF (pUsersGranted IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'GRANT ' || pRoleName || ' TO ' || pUsersGranted, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      RETURN '1';
   END updateUsersGrantedARole;   

   PROCEDURE GetPersonNonGSRoles (pGroupID COEGROUP.GROUP_ID%TYPE,
                                 pPersonID PEOPLE.person_id%TYPE,
                                 pRS OUT CURSOR_TYPE) IS
   BEGIN
        OPEN pRS FOR
      	SELECT  sr.role_id as Role_id, granted_role AS role_name
        FROM dba_role_privs, people p, security_roles SR
        WHERE UPPER (grantee) = UPPER (p.user_id)
          AND SR.role_name = granted_role
          AND p.person_id = pPersonID
          AND UPPER (dba_role_privs.granted_role) IN 
          (
            SELECT UPPER (role_name)
            FROM security_roles s, privilege_tables p
            WHERE s.privilege_table_int_id = p.privilege_table_id
           )
          AND UPPER (dba_role_privs.granted_role) NOT IN 
          (  
            SELECT SR.ROLE_NAME
            FROM  COEGROUPROLE GR, COEGROUP GRP, SECURITY_ROLES SR
            WHERE GR.GROUP_ID = pGroupID 
            AND GR.GROUP_ID = GRP.GROUP_ID
            AND SR.ROLE_ID = GR.ROLE_ID
		  );
    END;    
	
	PROCEDURE ClearGroupPeople (pGroupID COEGROUP.GROUP_ID%TYPE)  IS
    BEGIN
        DELETE FROM COEGROUPPEOPLE
          WHERE GROUP_ID = pGroupID;
    END ClearGroupPeople;
END;
/