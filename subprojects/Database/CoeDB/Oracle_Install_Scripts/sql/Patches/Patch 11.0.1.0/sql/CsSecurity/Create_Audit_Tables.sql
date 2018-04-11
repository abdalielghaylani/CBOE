--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

PROMPT Creating Audit Tables...

-- unique record IDs for each row in audited tables.
CREATE SEQUENCE "SEQ_RID"
  START WITH 1000
  INCREMENT BY 1
  ORDER
  NOCYCLE
  CACHE 100
  MINVALUE 1
  NOMAXVALUE;

-- Unique id for audit row table
CREATE SEQUENCE "SEQ_AUDIT"
  START WITH 1000
  INCREMENT BY 1
  ORDER
  NOCYCLE
  CACHE 100
  MINVALUE 1
  NOMAXVALUE;

-- AUDIT_ROW - one row per transaction.
CREATE TABLE audit_row (
   raid        NUMBER(10)                       NOT NULL,                       /* Row audit ID */
   table_name  VARCHAR2(30)                     NOT NULL,
   rid         NUMBER(10)                       NOT NULL,             /* Record ID (from table) */
   action      VARCHAR2(1)                      NOT NULL,                   /* Action (I, U, D) */
   TIMESTAMP   DATE         DEFAULT SYSDATE     NOT NULL,
   user_name   VARCHAR2(30) DEFAULT RTRIM(USER) NOT NULL
)
   TABLESPACE &&auditTableSpaceName
   PCTFREE 0
   PCTUSED 90
   STORAGE (INITIAL 100 k
               NEXT 100 k
         MAXEXTENTS UNLIMITED
        PCTINCREASE 0);

-- AUDIT_COLUMN - one row per changed column.
CREATE TABLE audit_column (
   raid        NUMBER(10)       NOT NULL,                       /* Row audit ID */
   caid        NUMBER(10)       NOT NULL,                    /* Column audit ID */
   column_name VARCHAR2(30)     NOT NULL,                        /* Column name */
   old_value   VARCHAR2(4000),
   new_value   VARCHAR2(4000)
)
TABLESPACE &&auditTableSpaceName
   PCTFREE 0
   PCTUSED 90
   STORAGE (INITIAL 100 k
               NEXT 100 k
         MAXEXTENTS UNLIMITED
        PCTINCREASE 0);

-- AUDIT_DELETE - one row per deleted row.
CREATE TABLE audit_delete (
   raid      NUMBER(10)     NOT NULL,
   row_data  VARCHAR2(4000) NOT NULL
)
TABLESPACE &&auditTableSpaceName
   PCTFREE 0
   PCTUSED 90
   STORAGE (INITIAL 100 k
               NEXT 100 k
         MAXEXTENTS UNLIMITED
        PCTINCREASE 0);

-- Audit Trail Package
@"sql\Patches\Patch 11.0.1.0\sql\packages\pkg_Audit_trail.sql"

-- Audit Trail Triggers
@"sql\Patches\Patch 11.0.1.0\sql\triggers\css_people_ad0.trg"
@"sql\Patches\Patch 11.0.1.0\sql\triggers\css_people_au0.trg"
@"sql\Patches\Patch 11.0.1.0\sql\triggers\css_people_bi0.trg"
@"sql\Patches\Patch 11.0.1.0\sql\triggers\css_security_roles_ad0.trg"
@"sql\Patches\Patch 11.0.1.0\sql\triggers\css_security_roles_au0.trg"
@"sql\Patches\Patch 11.0.1.0\sql\triggers\css_security_roles_bi0.trg"

CREATE TABLE cs_security_privileges(
   role_internal_id     NUMBER(8)                           NOT NULL,
   css_login            NUMBER(1),
   css_create_user      NUMBER(1),
   css_edit_user        NUMBER(1),
   css_delete_user      NUMBER(1),
   css_change_password  NUMBER(1),
   css_create_role      NUMBER(1),
   css_edit_role        NUMBER(1),
   css_delete_role      NUMBER(1),
   css_create_workgrp   NUMBER(1),
   css_edit_workgrp     NUMBER(1),
   css_delete_workgrp   NUMBER(1),
   "RID"                NUMBER(10)                          NOT NULL,
   "CREATOR"            VARCHAR2(30)    DEFAULT RTRIM(USER) NOT NULL,
   "TIMESTAMP"          DATE            DEFAULT SYSDATE     NOT NULL,
   CONSTRAINT cs_security_privileges_pk
      PRIMARY KEY (role_internal_id) USING INDEX TABLESPACE &&indextablespacename
   );

-- Audit Trail Triggers
@"sql\Patches\Patch 11.0.1.0\sql\triggers\cs_security_privileges_ad0.trg"
@"sql\Patches\Patch 11.0.1.0\sql\triggers\cs_security_privileges_au0.trg"
@"sql\Patches\Patch 11.0.1.0\sql\triggers\cs_security_privileges_bi0.trg"

CREATE TABLE object_privileges(
   privilege_name   VARCHAR2(30) NOT NULL,
   PRIVILEGE        VARCHAR2(10) NOT NULL,
   SCHEMA           VARCHAR2(30) NOT NULL,
   object_name      VARCHAR2(30) NOT NULL,
   CONSTRAINT object_priv_u
      UNIQUE(privilege_name, PRIVILEGE, SCHEMA, object_name)
   );

-- Create user and role management packages is cs_Security
@"sql\Patches\Patch 11.0.1.0\sql\packages\pkg_Users.sql"
@"sql\Patches\Patch 11.0.1.0\sql\packages\pkg_Roles.sql"
@"sql\Patches\Patch 11.0.1.0\sql\packages\pkg_Login.sql"
