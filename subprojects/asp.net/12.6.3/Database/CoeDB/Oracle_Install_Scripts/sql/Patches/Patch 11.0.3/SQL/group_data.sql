PROMPT Starting NewCOEDBGroupData.sql

-- Initialize the inventory groups

-- Create the Inventory organization
INSERT INTO COEGROUPORG
    (GROUPORG_NAME, DEFAULT_APP_ID)
    values (       'Inventory Organization',
         null);

--  Create the Inventory Master Group (inventory gods who own everthing)
INSERT INTO COEGROUP
    (GROUPORG_ID, GROUP_NAME, PARENT_GROUP_ID)
  values((select min(GROUPORG_ID) from COEGROUPORG), 'Inventory Masters',
    NULL);
    


commit;
