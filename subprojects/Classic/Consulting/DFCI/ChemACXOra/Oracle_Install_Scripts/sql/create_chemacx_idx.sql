DEFINE schemaName = CHEMACXDB
DEFINE tableSpaceName = T_&&schemaName
DEFINE indexTableSpaceName = T_&&schemaName._INDEX

-- SUBSTANCE IDXS
CREATE INDEX FIDX_SUBSTANCE_ACX_ID
    ON chemacxdb.SUBSTANCE(LOWER(ACX_ID))
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX IDX_SUBSTANCE_ACX_ID
    ON chemacxdb.SUBSTANCE(ACX_ID)
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX FIDX_SUBSTANCE_CAS
    ON chemacxdb.SUBSTANCE(LOWER(CAS))
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX IDX_SUBSTANCE_CAS
    ON chemacxdb.SUBSTANCE(CAS)
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX IDX_SUBSTANCE_MOL_ID
    ON chemacxdb.SUBSTANCE(MOL_ID)
    TABLESPACE &&indexTableSpaceName
    ;

-- PRODUCT IDXS
CREATE INDEX IDX_PRODUCT_CATNUM
    ON chemacxdb.PRODUCT(CATALOGNUM)
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX FIDX_PRODUCT_CATNUM
    ON chemacxdb.PRODUCT(LOWER(CATALOGNUM))
    TABLESPACE &&indexTableSpaceName
    ;


CREATE INDEX IDX_PRODUCT_NAME
    ON chemacxdb.PRODUCT(PRODNAME)
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX FIDX_PRODUCT_NAME
    ON chemacxdb.PRODUCT(LOWER(PRODNAME))
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX IDX_PRODUCT_DESCRIP
    ON chemacxdb.PRODUCT(PRODDESCRIP)
    TABLESPACE &&indexTableSpaceName
    ;

-- PACKAGE IDXS

--CREATE INDEX FIDX_PACKAGE_CATNUM
--    ON chemacxdb.PACKAGE(LOWER(CATALOGNUM))
--    TABLESPACE &&indexTableSpaceName
--    ;

--CREATE INDEX IDX_PACKAGE_CATNUM
--    ON chemacxdb.PACKAGE(CATALOGNUM)
--    TABLESPACE &&indexTableSpaceName
--    ;

--CREATE INDEX IDX_PACKAGE_SIZE
--    ON chemacxdb.PACKAGE("SIZE")
--    TABLESPACE &&indexTableSpaceName
--    ;

-- SYNONYM IDXS
CREATE INDEX FIDX_SYNONYM_NAME
    ON chemacxdb.ACX_SYNONYM(LOWER(NAME))
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX IDX_SYNONYM_NAME
    ON chemacxdb.ACX_SYNONYM(NAME)
    TABLESPACE &&indexTableSpaceName
    ;

-- PACKAGESIZECONVERSION
CREATE INDEX PACKAGESIZECONVERSION_SIZE_FK
    ON chemacxdb.PACKAGESIZECONVERSION(SIZE_FK)
    TABLESPACE &&indexTableSpaceName
    ;

-- MSDX
CREATE INDEX IDX_MSDX_CATALOGNUM
    ON chemacxdb.MSDX(CATALOGNUM)
    TABLESPACE &&indexTableSpaceName;

CREATE INDEX IDX_MSDX_CAS
    ON chemacxdb.MSDX(CAS)
    TABLESPACE &&indexTableSpaceName;


-- PRIMARY KEYS
ALTER TABLE chemacxdb.PACKAGE
    ADD CONSTRAINT PACKAGE_PK PRIMARY KEY(
    PACKAGEID)USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.PRODUCT
    ADD CONSTRAINT PRODUCT_PK PRIMARY KEY(
    PRODUCTID) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.SUBSTANCE
    ADD CONSTRAINT SUBSTANCE_PK PRIMARY KEY(
    CSNUM) USING INDEX TABLESPACE &&indexTableSpaceName
    ;


ALTER TABLE chemacxdb.ACX_SYNONYM
    ADD CONSTRAINT SYNONYM_PK PRIMARY KEY(
    SYNONYMID) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.SUPPLIER
    ADD CONSTRAINT SUPPLIER_PK PRIMARY KEY(
    SUPPLIERID) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.SUPPLIERADDR
    ADD CONSTRAINT SUPPLIERADDR_PK PRIMARY KEY(
    SUPPLIERADDRID) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.SUPPLIERPHONEID
    ADD CONSTRAINT SUPPLIERPHONEID_PK PRIMARY KEY(
    SUPPLIERPHONEID) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.SHOPPINGCART
    ADD CONSTRAINT SHOPPINGCART_PK PRIMARY KEY(
    CSUSERNAME) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.MSDX
    ADD CONSTRAINT MSDX_PK PRIMARY KEY(
    IDAUTONUMBER) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.PROPERTYCLASSALPHA
    ADD CONSTRAINT PROPERTYCLASSALPHA_PK PRIMARY KEY(
    PROPERTYCLASSALPHAID) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.PROPERTYALPHA
    ADD CONSTRAINT PROPERTYALPHA_PK PRIMARY KEY(
    PRODUCTID, PROPERTY) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

ALTER TABLE chemacxdb.PACKAGESIZECONVERSION
    ADD CONSTRAINT PACKAGESIZECONVERSION_PK PRIMARY KEY(
    ID) USING INDEX TABLESPACE &&indexTableSpaceName
    ;

--- ALTERNATE KEYS

ALTER TABLE chemacxdb.substance
add constraint U_SUBSTANCE_ACXID
unique(acx_id);

ALTER TABLE chemacxdb.packagesizeconversion
add constraint U_packsizeconversion_size
unique(size_fk);


-- FOREIGN KEYS
-- SUBSTANCE table Foreign Keys
ALTER TABLE chemacxdb.substance
add constraint FK_SUBSTANCE_SYNID
foreign key(synonymid)
references acx_synonym(synonymid);

CREATE INDEX IDX_SUBSTANCE_SYNID
    ON chemacxdb.SUBSTANCE(SYNONYMID)
    TABLESPACE &&indexTableSpaceName;

ALTER TABLE chemacxdb.substance
add constraint FK_SUBSTANCE_SUPID
foreign key(supplierid)
references supplier(supplierid);

CREATE INDEX IDX_SUBSTANCE_SUPPLIER
    ON chemacxdb.SUBSTANCE(SUPPLIERID)
    TABLESPACE &&indexTableSpaceName;

-- Product table Foreign Keys
ALTER TABLE chemacxdb.product
add constraint FK_PROD_SUPID
foreign key(supplierid)
references supplier(supplierid);

CREATE INDEX IDX_PRODUCT_SUPPLIER_ID
    ON chemacxdb.PRODUCT(SUPPLIERID)
    TABLESPACE &&indexTableSpaceName;

ALTER TABLE chemacxdb.product
add constraint FK_PROD_CSNUM
foreign key(csnum)
references substance(csnum);

CREATE INDEX IDX_PRODUCT_CSNUM
    ON chemacxdb.PRODUCT(CSNUM)
    TABLESPACE &&indexTableSpaceName;

ALTER TABLE chemacxdb.product
add constraint FK_SUBSTANCE_ACXID
foreign key(acx_id)
references substance(acx_id);

CREATE INDEX IDX_PRODUCT_ACXID
    ON chemacxdb.PRODUCT(ACX_ID)
    TABLESPACE &&indexTableSpaceName;

-- PACKAGE table Foreign Keys
ALTER TABLE chemacxdb.package
add constraint FK_PACK_SUPID
foreign key(supplierid)
references supplier(supplierid);

CREATE INDEX IDX_PACKAGE_SUPPLIERID
    ON chemacxdb.PACKAGE(SUPPLIERID)
    TABLESPACE &&indexTableSpaceName;

ALTER TABLE chemacxdb.package
add constraint FK_PACK_PRODID
foreign key(productid)
references product(productid);

CREATE INDEX IDX_PACKAGE_PROD_ID
    ON chemacxdb.PACKAGE(PRODUCTID)
    TABLESPACE &&indexTableSpaceName;

-- ACX_SYNONYM table Foreign Keys

ALTER TABLE chemacxdb.acx_synonym
add constraint FK_SYN_CSNUM
foreign key(csnum)
references substance(csnum);

CREATE INDEX IDX_SYNONYM_CSNUM
    ON chemacxdb.ACX_SYNONYM(CSNUM)
    TABLESPACE &&indexTableSpaceName;

ALTER TABLE chemacxdb.acx_synonym
add constraint FK_SYN_SUPID
foreign key(supplierid)
references supplier(supplierid);

CREATE INDEX IDX_SYNONYM_SUPID
    ON chemacxdb.ACX_SYNONYM(supplierid)
    TABLESPACE &&indexTableSpaceName;

-- PROPERTYALPHA table Foreign Keys

ALTER TABLE chemacxdb.propertyalpha
add constraint FK_propalpha_productid
foreign key(productid)
references product(productid);

CREATE INDEX IDX_PROPALPHA_PRODID
    ON chemacxdb.propertyalpha(productid)
    TABLESPACE &&indexTableSpaceName;


-- MSDX table Foreign Keys

ALTER TABLE chemacxdb.MSDX
add constraint FK_msdx_vendorid
foreign key(vendorid)
references supplier(supplierid);

CREATE INDEX IDX_MSDX_VENDORID
    ON chemacxdb.msdx(vendorid)
    TABLESPACE &&indexTableSpaceName;

CREATE INDEX IDX_MDSX_PRODID
    ON chemacxdb.msdx(productid)
    TABLESPACE &&indexTableSpaceName;

CREATE INDEX IDX_ECNDC
    ON chemacxdb.ABC_CATALOG("ECNDC")
    TABLESPACE &&indexTableSpaceName
    ;

CREATE INDEX IDX_SIZE
    ON chemacxdb.ABC_CATALOG("SIZE")
    TABLESPACE &&indexTableSpaceName
    ;


CREATE INDEX IDX_NDC
    ON chemacxdb.PHDOUT("NDC")
    TABLESPACE &&indexTableSpaceName
    ;


-- Gather Optimizer statistics
connect system/manager2@&&serverName
begin
	dbms_stats.gather_schema_stats(ownname=> 'CHEMACXDB' , cascade=> TRUE);
end;

--Create Cartridge Index
--create index mx on substance(base64_cdx) indextype is cscartridge.moleculeindextype;

