--' Table Function: Gets populated when the user places a custom order for an item that is not in ACX by using the Create Order link wich allows them to enter a new vendor during the ordering process.

CREATE TABLE CUSTOM_CHEM_ORDER (
       ORD_LINE             NUMBER(18) NOT NULL,
       REGISTERED           VARCHAR2(1) NOT NULL,
       DUE_DATE             VARCHAR2(10) NOT NULL,
       RUSH                 NUMBER(1) DEFAULT 0 NOT NULL,
       CATALOG_NO           VARCHAR2(32) NOT NULL,
       CATALOG_COST         NUMBER(15,4) DEFAULT 0 NOT NULL,
       NUM_BOTTLES          NUMBER(10,2) NOT NULL,
       AMT_BOTTLE           NUMBER(10,2) NOT NULL,
       UOM                  VARCHAR2(5) NOT NULL,
       SCIENTIST            VARCHAR2(20) NULL,
       VENDOR_NO            VARCHAR2(8) NOT NULL,
       site                 VARCHAR2(255),
       COMMENTS             VARCHAR2(255) NULL,
       ITEM_NO              VARCHAR2(32) NOT NULL,
       NAME                 VARCHAR2(255) NOT NULL,
       STATUS               VARCHAR2(1) NULL,
       STATUS_DESC          VARCHAR2(500) NULL,
       PROJECT_NO           VARCHAR2(8) NOT NULL,
       PRIMARY KEY (ORD_LINE) USING INDEX TABLESPACE &&indexTableSpaceName
	)
;

create sequence Custom_CHEM_ORD_LINE_SEQ INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E9 MINVALUE 1 NOCYCLE NOCACHE ORDER;
