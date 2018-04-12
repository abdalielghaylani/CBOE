create table inv_protocol (
		protocol_id number,
		protocol_identifier varchar2(255),
		protocol_name varchar2(1000),
		sponsor_id varchar2(1000),
		nci_protocol_num varchar2(100),
		start_date date,
		end_date date,
		"RID" NUMBER(12) NOT NULL,
		create_date timestamp default sysdate,
	CONSTRAINT "PROTOCOL_ID_PK"
		PRIMARY KEY("PROTOCOL_ID") USING INDEX TABLESPACE &&indexTableSpaceName,
	CONSTRAINT "U_PROTOCOL_IDENTIFIER"
		UNIQUE ("PROTOCOL_IDENTIFIER") USING INDEX TABLESPACE &&indexTableSpaceName
)
;

CREATE SEQUENCE SEQ_INV_PROTOCOL INCREMENT BY 1 START WITH 1000 MAXVALUE 1.0E28 MINVALUE 1 NOCYCLE;

CREATE OR REPLACE TRIGGER "TRG_INV_PROTOCOL"
    BEFORE INSERT
    ON "INV_PROTOCOL"
    FOR EACH ROW
    begin
        if :new.PROTOCOL_ID is null then
            select SEQ_INV_PROTOCOL.nextval into :new.PROTOCOL_ID from dual;
        end if;
    end;
/
