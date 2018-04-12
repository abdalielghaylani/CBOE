--' Table Function: Used to map our vendor ids to purchasing system vendor ids.

CREATE TABLE "CUSTOM_ACX_ST_VENDORS"(
	"ACX" NUMBER(4) NOT NULL, 
	"NAME" VARCHAR2(255) NOT NULL, 
	"ST" VARCHAR2(8) NOT NULL, 
    CONSTRAINT "CUSTOM_ACX_ST_VENDORS_PK" 
		PRIMARY KEY("ACX") USING INDEX  TABLESPACE &&indexTableSpaceName
	); 
