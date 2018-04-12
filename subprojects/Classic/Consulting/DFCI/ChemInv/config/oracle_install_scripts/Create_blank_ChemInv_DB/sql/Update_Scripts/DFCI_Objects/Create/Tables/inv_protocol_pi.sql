create table inv_protocol_PI (
		protocol_pi_id number,
		protocol_id_fk number,
		PI varchar2(500),
		PI_NCI_NUM varchar(100),
		start_date date,
		end_date date,
		RID NUMBER(12) NOT NULL,
		create_date timestamp default sysdate,
		CONSTRAINT "PROTOCOL_PI_ID_PK"
		PRIMARY KEY("PROTOCOL_PI_ID") USING INDEX TABLESPACE &&indexTableSpaceName
		);

CREATE SEQUENCE SEQ_INV_PROTOCOL_PI INCREMENT BY 1 START WITH 1000 MAXVALUE 1.0E28 MINVALUE 1 NOCYCLE;

CREATE OR REPLACE TRIGGER "TRG_INV_PROTOCOL_PI"
    BEFORE INSERT
    ON "INV_PROTOCOL_PI"
    FOR EACH ROW
    begin
        if :new.PROTOCOL_PI_ID is null then
            select SEQ_INV_PROTOCOL_PI.nextval into :new.PROTOCOL_PI_ID from dual;
        end if;
    end;
/