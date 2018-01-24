--Copyright 1999-2011 CambridgeSoft Corporation. All rights reserved

--#########################################################
--CREATE COEEXPORTTEMPLATE TABLE, SEQUENCE in COEDB
--#########################################################

-- Table Creation
CREATE TABLE COEEXPORTTEMPLATE(
	ID					NUMBER(9,0),
	NAME				        VARCHAR2(255 BYTE)  NOT NULL,
	DESCRIPTION			        VARCHAR2(255 BYTE),
	COERESULTCRITERIA	                CLOB NOT NULL,
	IS_PUBLIC			        VARCHAR2(1 BYTE) NOT NULL,
	DATE_CREATED		                DATE NOT NULL,
	DATAVIEW_ID			        NUMBER(9,0) NOT NULL,
	WEBFORM_ID			        NUMBER(9,0),
	WINFORM_ID			        NUMBER(9,0),
	USER_ID				        VARCHAR2(30 BYTE),
	CONSTRAINT COEExportTemplatePK PRIMARY KEY(ID) USING INDEX TABLESPACE &&indexTableSpaceName
);

-- Index creation for column DATAVIEW_ID
CREATE INDEX COEExportTemplatDATAVIEW_IDIDX 
        ON COEEXPORTTEMPLATE(DATAVIEW_ID);

-- Index creation for column NAME
CREATE INDEX COEExportTemplateNAMEIDX 
        ON COEEXPORTTEMPLATE(NAME);

-- Index creation for column IS_PUBLIC
CREATE INDEX COEExportTemplateIS_PUBLICIDX 
        ON COEEXPORTTEMPLATE(IS_PUBLIC);

-- Index creation for column USER_ID
CREATE INDEX COEExportTemplateUSER_IDIDX 
        ON COEEXPORTTEMPLATE(USER_ID);

-- Index creation for column WEBFORM_ID
CREATE INDEX COEExportTemplateWEBFORM_IDIDX 
        ON COEEXPORTTEMPLATE(WEBFORM_ID);

-- Index creation for column WINFORM_ID
CREATE INDEX COEExportTemplateWINFORM_IDIDX 
        ON COEEXPORTTEMPLATE(WINFORM_ID);


-- Sequence Creation
CREATE SEQUENCE COEExportTemplateSEQ
	INCREMENT BY 1 
	START WITH 1 
	MAXVALUE 1.0E27 
	MINVALUE 1 
	NOCYCLE 
	CACHE 2
	NOORDER;
        
-- Trigger to get next ID value from table 'COEEXPORTTEMPLATE'
CREATE OR REPLACE TRIGGER COEExportTemplateTRIG
BEFORE INSERT
ON COEEXPORTTEMPLATE
REFERENCING NEW AS NEW
FOR EACH ROW
BEGIN
SELECT COEExportTemplateSEQ.nextval INTO :NEW.ID FROM dual;
END;
/