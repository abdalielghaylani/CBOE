---Insert PeopleTable Data in CoePrincipal Table
INSERT INTO COEPRINCIPAL
       (PRINCIPAL_ID,PERSON_ID,ACTIVE)
     SELECT COEPRINCIPAL_ID_SEQ.NEXTVAL,
       PERSON_ID,ACTIVE
       FROM PEOPLE;