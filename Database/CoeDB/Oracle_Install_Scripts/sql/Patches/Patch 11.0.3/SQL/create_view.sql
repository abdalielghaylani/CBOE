----------------------------------------------------------------
PROMPT >> CREATE View USER_ROLES_VW
----------------------------------------------------------------
CREATE OR REPLACE FORCE VIEW "COEDB"."USER_ROLES_VW" ("USER_ID", "ROLE_NAME")
AS
  SELECT DISTINCT P.USER_ID,
    R.ROLE_NAME
  FROM PEOPLE P,
    SECURITY_ROLES R,
    (SELECT GP.PERSON_ID,
      PS.ROLE_ID
    FROM COEGROUPPEOPLE GP,
      COEPRINCIPAL P,
      COEGROUPROLE PP,
      SECURITY_ROLES PS
    WHERE P.GROUP_ID = GP.GROUP_ID
    AND P.GROUP_ID   = PP.GROUP_ID
    AND PS.ROLE_ID   = PP.ROLE_ID
    UNION
    SELECT P.PERSON_ID,
      PS.ROLE_ID
    FROM COEPRINCIPAL P,
      COEGROUPROLE PP,
      SECURITY_ROLES PS
    WHERE P.GROUP_ID = PP.GROUP_ID
    AND PS.ROLE_ID   = PP.ROLE_ID
    ) PPS,
    (SELECT COEPRINCIPAL.person_id ,
      SECURITY_ROLES.role_id
    FROM dba_role_privs ,
      SECURITY_ROLES,
      COEPRINCIPAL,
      people
    WHERE UPPER (dba_role_privs.granted_role) IN
      ( SELECT DISTINCT role_name FROM security_roles s
      )
    AND upper(SECURITY_ROLES.role_name)=upper(dba_role_privs.granted_role)
    AND COEPRINCIPAL.person_id         = people.person_id
    AND UPPER(people.user_code)        = UPPER (grantee)
    ) PSR
  WHERE PPS.PERSON_ID = P.PERSON_ID
  AND PPS.ROLE_ID     = PSR.ROLE_ID
  AND PSR.role_id     = R.role_id;
  /
 ----------------------------------------------------------------
PROMPT >> CREATE View USER_GRANT_NEEDS_VW
----------------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "COEDB"."USER_GRANT_NEEDS_VW" ("USER_ID", "ROLE_NAME", "NEEDED", "GRANTED")
AS
  SELECT USER_ID,
    ROLE_NAME,
    NEEDED,
    GRANTED
  FROM
    (SELECT NVL(VRV.USER_ID,DRPJ.USER_ID) AS USER_ID,
      NVL(VRV.ROLE_NAME,DRPJ.ROLE_NAME)   AS ROLE_NAME,
      NVL(VRV.NEEDED,0)                   AS NEEDED,
      NVL(DRPJ.GRANTED,0)                 AS GRANTED
    FROM
      (SELECT URV.USER_ID,
        URV.ROLE_NAME,
        1 AS NEEDED
      FROM USER_ROLES_VW URV
      JOIN DBA_USERS DU
      ON (DU.USERNAME = URV.USER_ID)
      ) VRV
    FULL OUTER JOIN
      (SELECT GRANTEE AS USER_ID,
        GRANTED_ROLE  AS ROLE_NAME,
        1             AS GRANTED
      FROM DBA_ROLE_PRIVS DRP
      JOIN SECURITY_ROLES DBR
      ON (DBR.ROLE_NAME = DRP.GRANTED_ROLE)
      JOIN PEOPLE P
      ON (P.USER_ID           = DRP.GRANTEE)
      ) DRPJ ON (DRPJ.USER_ID = VRV.USER_ID
    AND DRPJ.ROLE_NAME        = VRV.ROLE_NAME)
    );
    /