--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 


--#########################################################
--TABLES
--######################################################### 

ALTER TABLE &&schemaName..Projects ADD (Is_Public NCHAR(1) DEFAULT 'T' NOT NULL);

CREATE TABLE &&schemaName..TEMPORARY_BATCH_PROJECT
(
  ID                  NUMBER(8,0),
  TEMPBATCHID         NUMBER(8,0),
  PROJECTID           NUMBER(8,0), 
  CONSTRAINT TEMPORARY_BATCH_PROJECT_PK PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName
);

CREATE TABLE &&schemaName..TEMPORARY_REG_NUMBERS_PROJECT
(
  ID                  NUMBER(8,0),
  TEMPBATCHID         NUMBER(8,0),
  PROJECTID           NUMBER(8,0), 
  CONSTRAINT TEMP_REG_NUMBERS_PROJECT_PK PRIMARY KEY (ID) USING INDEX TABLESPACE &&indexTableSpaceName
);

CREATE TABLE &&schemaName..ReseredWords AS SELECT DISTINCT KeyWord FROM V$RESERVED_WORDS WHERE KeyWord IS NOT NULL;

ALTER TABLE &&schemaName..ReseredWords ADD CONSTRAINT ReseredWords_PK PRIMARY KEY (KeyWord) USING INDEX TABLESPACE &&indexTableSpaceName;

ALTER TABLE &&securitySchemaName..CHEM_REG_PRIVILEGES ADD RLS_EXEMPT NUMBER(1,0) Default 0;

--#########################################################
--SEQUENCES
--#########################################################

CREATE SEQUENCE &&schemaName..SEQ_TEMPORARY_BATCH_PROJECT INCREMENT By 1 START With 1;

CREATE SEQUENCE &&schemaName..SEQ_TEMP_REG_NUMBERS_PROJECT INCREMENT By 1 START With 1;

--#########################################################
--INDEXES
--#########################################################

--#########################################################
--CONSTRAINTS
--#########################################################

ALTER TABLE &&schemaName..TEMPORARY_BATCH_PROJECT ADD CONSTRAINT TEMP_BATCH_PROJ_TEMPBATCHID_FK FOREIGN KEY (TEMPBATCHID) 
REFERENCES &&schemaName..TEMPORARY_BATCH (TEMPBATCHID); 

ALTER TABLE &&schemaName..TEMPORARY_BATCH_PROJECT ADD CONSTRAINT TEMP_BATCH_PROJ_PROJECTID_FK FOREIGN KEY (PROJECTID) 
REFERENCES &&schemaName..PROJECTS (PROJECT_INTERNAL_ID); 

ALTER TABLE &&schemaName..TEMPORARY_REG_NUMBERS_PROJECT ADD CONSTRAINT TEMP_REG_PROJ_TEMPBATCHID_FK FOREIGN KEY (TEMPBATCHID) 
REFERENCES &&schemaName..TEMPORARY_BATCH (TEMPBATCHID); 

ALTER TABLE &&schemaName..TEMPORARY_REG_NUMBERS_PROJECT ADD CONSTRAINT TEMP_REG_PROJ_PROJECTID_FK FOREIGN KEY (PROJECTID) 
REFERENCES &&schemaName..PROJECTS (PROJECT_INTERNAL_ID); 

--#########################################################
--VIEWS
--#########################################################

CREATE OR REPLACE NOFORCE VIEW &&schemaName..VW_Project 
	(ProjectID,Name,Active,Description, Type, IsPublic) AS 
	SELECT PROJECT_INTERNAL_ID,PROJECT_NAME,ACTIVE,DESCRIPTION,TYPE,IS_PUBLIC FROM PROJECTS;

CREATE OR REPLACE NOFORCE VIEW &&schemaName..VW_TemporaryBatchProject 
	(ID,TempBatchID,ProjectID) AS 
	SELECT ID,TempBatchID,ProjectID FROM TEMPORARY_BATCH_PROJECT;

CREATE OR REPLACE NOFORCE VIEW &&schemaName..VW_TemporaryRegNumbersProject 
	(ID,TempBatchID,ProjectID) AS 
	SELECT ID,TempBatchID,ProjectID FROM TEMPORARY_REG_NUMBERS_PROJECT;

--Used by new duplicate-checking; gets the list of compound IDs
CREATE OR REPLACE NOFORCE VIEW &&schemaName..VW_COMPOUND_IDS
    (COMPOUNDID) AS 
    SELECT CPD_DATABASE_COUNTER AS COMPOUNDID FROM COMPOUND_MOLECULE;


--Get all the compound duplicated without the recurrent relations and identifying the compounds with same structure.
CREATE OR REPLACE VIEW &&schemaName..VW_DuplicatesGrouped(GroupID,RegNumber,Structure) AS   
    SELECT C.CompoundID GroupID, RegNumberbyGroup.RegNumber, S.Structure
      FROM 
       (
        SELECT 
          (
           SELECT CASE  WHEN Min(RegNumberDuplicated) > RND.RegNumber THEN  RND.RegNumber ELSE Min(RegNumberDuplicated) END 
             FROM 
               (
                SELECT RegNumber,RegNumberDuplicated 
                  FROM 
                   (
                    SELECT RegNumber,RegNumberDuplicated FROM VW_Duplicates
                    UNION
                    SELECT RegNumberDuplicated ,RegNumber FROM VW_Duplicates
                   )
               ) CoupleDupl
               START WITH CoupleDupl.RegNumber = RND.RegNumber
               CONNECT BY CoupleDupl.RegNumber = CoupleDupl.RegNumberDuplicated
          ) RegNumberGroup, RegNumber
          FROM 
            ( 
             SELECT RegNumber FROM VW_Duplicates
                UNION
             SELECT RegNumberDuplicated FROM VW_Duplicates) RND
            ) RegNumberbyGroup, VW_Compound C, VW_RegistryNumber RR,VW_Structure S 
     WHERE RR.RegID = C.RegID AND RR.RegNumber = RegNumberbyGroup.RegNumberGroup AND S.StructureID = C.StructureID 
     ORDER BY C.CompoundID, RegNumberbyGroup.RegNumber;

--Get all the compound's regnumber and regid duplicated without the recurrent relations.
CREATE OR REPLACE VIEW &&schemaName..VW_DUPLICATESDISTINCT(REGID, REGNUMBER, MIXTUREREGNUMBERS) AS 
    SELECT 
      RN.RegID, 
      RN.RegNumber, 
      XmlTransform
       (
        XmlType.CreateXml
         (dbms_xmlquery.getXML
           ('SELECT RNMixture.RegNumber 
                FROM  VW_Compound C, VW_Mixture_Component MC, VW_Mixture M, VW_RegistryNumber RNMixture
                WHERE '''||RN.RegID||'''=C.RegID and MC.CompoundID=C.CompoundID AND M.MixtureID = MC.MixtureID AND M.RegID = RNMixture.RegID'
            )
          ) 
       ,XmlType.CreateXml
        ('<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
             <xsl:template match="/ROWSET">
               <ul style="white-space:nowrap; color: #000000; font-family: Verdana; font-size: 11px; width: 90%; overflow: hidden;">
                 <xsl:for-each select="ROW/REGNUMBER">
                   <li style="margin:1em;">
                     <xsl:element name="a">
                       <xsl:attribute name="TARGET">_parent</xsl:attribute>
                       <xsl:attribute name="href">/COERegistration/Forms/ViewMixture/ContentArea/ViewMixture.aspx?RegisteredObjectId=<xsl:value-of select="."/></xsl:attribute>
                       <xsl:value-of select="."/> 
                     </xsl:element> 
                   </li>  
                </xsl:for-each>
              </ul>        
            </xsl:template>
          </xsl:stylesheet>'   
        )
       ).GetClobVal()
    FROM  
       (
        SELECT RegNumber FROM VW_Duplicates D
            UNION
        SELECT RegNumberDuplicated FROM VW_Duplicates
       ) D, VW_RegistryNumber RN
    WHERE D.RegNumber=RN.RegNumber;

CREATE OR REPLACE VIEW &&schemaName..VW_ReseredWords AS SELECT KeyWord FROM &&schemaName..ReseredWords;

--#########################################################
--CONTEXTS
--#########################################################
  
CREATE OR REPLACE CONTEXT RegistrationUsersCtx USING &&schemaName..RegistrationRLS ACCESSED GLOBALLY;

--#########################################################
--TYPES
--#########################################################

set define off

@"sql\Patches\Patch 11.0.3\Packages\typ_Component.sql"

set define on

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

--Get the status of the RLS_Exempt privileges. If some User's Role have the privileges then return '1' as true.
CREATE OR REPLACE FUNCTION &&securitySchemaName..GetExemptRLSFromPrivileges (
    AClientID IN VARCHAR2) 
    RETURN Chem_Reg_Privileges.RLS_Exempt%TYPE
  AS    
    LRLS_Exempt Chem_Reg_Privileges.RLS_Exempt%TYPE;
BEGIN
       
    SELECT NVL(Max(CRP.RLS_Exempt),0)
      INTO LRLS_Exempt
      FROM Chem_Reg_Privileges CRP, Security_Roles SR, DBA_Role_Privs RP
      WHERE CRP.Role_Internal_ID = SR.Role_ID AND UPPER(SR.Role_Name ) = UPPER(RP.Granted_Role)
      START WITH UPPER(RP.Grantee) = UPPER(AClientID)     
      CONNECT BY PRIOR RP.Granted_Role = RP.Grantee;
       
    RETURN LRLS_Exempt;    

END GetExemptRLSFromPrivileges;
/

--#########################################################
--GRANTS
--#########################################################

GRANT EXECUTE ON &&securitySchemaName..GetExemptRLSFromPrivileges To &&schemaName;

--#########################################################
--PACKAGES
--#########################################################

set define off

@"sql\Patches\Patch 11.0.3\Packages\pkg_RegistrationRLS_def.sql"
@"sql\Patches\Patch 11.0.3\Packages\pkg_RegistrationRLS_body.sql"
@"sql\Patches\Patch 11.0.3\Packages\pkg_Gui_Util_def.sql"
@"sql\Patches\Patch 11.0.3\Packages\pkg_Gui_Util_body.sql"
@"sql\Patches\Patch 11.0.3\Packages\pkg_CompoundRegistry_def.sql"
@"sql\Patches\Patch 11.0.3\Packages\pkg_CompoundRegistry_body.sql"
@"sql\Patches\Patch 11.0.3\Packages\pkg_RegistryDuplicateCheck_def.sql"
@"sql\Patches\Patch 11.0.3\Packages\pkg_RegistryDuplicateCheck_body.sql"
@"sql\Patches\Patch 11.0.3\Packages\Pkg_ConfigurationCompoundRegistry_body.sql"


set define on



--#########################################################
--TRIGGERS
--#########################################################

CREATE OR REPLACE TRIGGER &&schemaName..TRG_Project_Owner
   AFTER INSERT
   ON &&schemaName..Projects
   FOR EACH ROW
BEGIN

   INSERT INTO &&schemaName..People_Project(Person_ID, Project_ID) 
    (SELECT Person_ID,:new.Project_Internal_ID 
        FROM &&securitySchemaName..People 
        WHERE User_ID=UPPER(sys_context('userenv','client_identifier')));
   
END;
/

CREATE OR REPLACE TRIGGER &&securitySchemaName..TRG_UpdatingExemptRLS
   AFTER UPDATE OF RLS_Exempt OR INSERT OR DELETE
   ON &&securitySchemaName..Chem_Reg_Privileges 
   FOR EACH ROW
   
DECLARE   
        CURSOR C_Users(LRole_Internal_ID NUMBER) IS
        SELECT Distinct P.User_ID
            FROM
                (
                 SELECT RP.Grantee
                    FROM Security_Roles SR, DBA_Role_Privs RP
                    WHERE UPPER(SR.Role_Name ) = UPPER(RP.Granted_Role)
                    START WITH UPPER(RP.Granted_Role)=(SELECT SR.Role_Name FROM Security_Roles SR WHERE SR.Role_ID= LRole_Internal_ID)
                    CONNECT BY RP.Granted_Role = PRIOR RP.Grantee
                ) U,People P
            WHERE P.User_ID=U.Grantee;
BEGIN

    INSERT INTO LOG(LogProcedure,LogComment) VALUES('&&securitySchemaName..TRG_UpdatingExemptRLS','Begin User'||User||' NEW.Role_Internal_ID'||:NEW.Role_Internal_ID);
     
    FOR  R_Users IN  C_Users(:NEW.Role_Internal_ID)  LOOP
    
         INSERT INTO LOG(LogProcedure,LogComment) VALUES('&&securitySchemaName..TRG_UpdatingExemptRLS',':NEW.RLS_Exempt'||:NEW.RLS_Exempt||' R_Users.User_ID'||R_Users.User_ID);
         
         IF DELETING THEN
            &&schemaName..RegistrationRLS.SetExemptRLS('0',R_Users.User_ID);
         ELSE   
            &&schemaName..RegistrationRLS.SetExemptRLS(:NEW.RLS_Exempt,R_Users.User_ID);
         END IF;  

    END LOOP;

EXCEPTION
        WHEN OTHERS THEN
        BEGIN
             INSERT INTO LOG(LogProcedure,LogComment) VALUES('&&securitySchemaName..TRG_UpdatingExemptRLS','error &&securitySchemaName..TRG_UpdatingExemptRLS:'||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;   
END;
/

ALTER TRIGGER &&schemaName..TRG_Project_Owner DISABLE;

CREATE OR REPLACE TRIGGER &&schemaName..TRG_TEMPORARY_BATCH_PROJECT
BEFORE INSERT
ON &&schemaName..TEMPORARY_BATCH_PROJECT 
FOR EACH ROW
BEGIN
  SELECT SEQ_TEMPORARY_BATCH_PROJECT.NEXTVAL INTO :NEW.ID FROM DUAL;
END;
/

CREATE OR REPLACE TRIGGER &&schemaName..TRG_TEMP_REG_NUMBERS_PROJECT
BEFORE INSERT
ON &&schemaName..TEMPORARY_REG_NUMBERS_PROJECT
FOR EACH ROW
BEGIN
  SELECT SEQ_TEMP_REG_NUMBERS_PROJECT.NEXTVAL INTO :NEW.ID FROM DUAL;
END;
/

--#########################################################
--DATA
--#########################################################

--The data of new table relation between Temporary_Batch and Project need be updated.
INSERT INTO &&schemaName..VW_TemporaryBatchProject(TempbatchID,ProjectID)
  SELECT  TempBatchid, P.ProjectID
    FROM  &&schemaName..VW_TemporaryBatch , 
          TABLE( XMLSequence ( extract ( XmlType(ProjectXML),'ProjectList/Project/ProjectID') ) ) XMLProjectID,
          &&schemaName..VW_Project P
    WHERE P.ProjectID = extractValue ( value( XMLProjectID ), '/ProjectID');

COMMIT;

@"sql\Patches\Patch 11.0.3\UpdateConfigurations.sql"
COMMIT;

UPDATE &&schemaName..PICKLISTDOMAIN SET EXT_TABLE='&&schemaName..VW_Unit' WHERE ID=2;

COMMIT;

--RLS

COL ActivateRLS NEW_VALUE ActivateRLS NOPRINT
SELECT	CASE
		WHEN  &&schemaName..RegistrationRLS.GetConfigStateRLS='T'
		THEN  'Y'
		ELSE  'N'
	END	AS ActivateRLS
FROM	DUAL;
@"sql\Patches\Patch 11.0.3\&&ActivateRLS"

--#####################################################################

UPDATE &&schemaName..Globals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';

UPDATE &&schemaName..Globals
	SET Value = '&&versionApp' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&currentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&currentPatch'
		THEN  'sql\Patches\stop.sql'
		ELSE  '"sql\Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;









