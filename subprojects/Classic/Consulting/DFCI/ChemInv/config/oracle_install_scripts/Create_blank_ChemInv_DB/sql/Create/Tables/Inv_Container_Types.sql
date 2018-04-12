CREATE TABLE "INV_CONTAINER_TYPES"(
    "CONTAINER_TYPE_ID" NUMBER(4) NOT NULL, 
    "CONTAINER_TYPE_NAME" VARCHAR2(50), 
    CONSTRAINT "INV_CONTAINER_TYPES_PK" 
		PRIMARY KEY("CONTAINER_TYPE_ID") USING INDEX  TABLESPACE &&indexTableSpaceName  
	)
; 

create sequence SEQ_INV_CONTAINER_TYPES INCREMENT BY 1 START WITH 1000;

CREATE OR REPLACE TRIGGER "TRG_INV_CONTAINER_TYPES" 
    BEFORE INSERT 
    ON "INV_CONTAINER_TYPES" 
    FOR EACH ROW 
    begin
		if :new.Container_type_ID is null then
			select seq_inv_Container_types.nextval into :new.Container_type_id from dual;
		end if;
end;
/