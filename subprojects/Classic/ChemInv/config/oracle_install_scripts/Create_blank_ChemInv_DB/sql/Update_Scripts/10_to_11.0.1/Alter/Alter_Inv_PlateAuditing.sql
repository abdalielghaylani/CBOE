PROMPT Starting Alter_Inv_PlateAuditing.sql
-- Execute as inventory owner
-- Add audit columns to plates and wells

ALTER TABLE INV_PLATES ADD (
  RID       NUMBER(10)   NULL,
  CREATOR   VARCHAR2(30) DEFAULT RTRIM(user) NOT NULL,
  TIMESTAMP DATE         DEFAULT sysdate     NOT NULL
);

ALTER TABLE INV_WELLS ADD (
  RID       NUMBER(10)   NULL,
  CREATOR   VARCHAR2(30) DEFAULT RTRIM(user) NOT NULL,
  TIMESTAMP DATE         DEFAULT sysdate     NOT NULL
);
