


--#########################################################
--update biosardb schema for bioviz, excel and tree control features
--#########################################################


Connect &&schemaName/&&schemaPass@&&serverName		


GRANT SELECT, INSERT, DELETE, UPDATE ON BIOSARDB.TREE_NODE TO &&securitySchemaName with grant option;
GRANT SELECT, INSERT, DELETE, UPDATE ON BIOSARDB.TREE_TYPE TO &&securitySchemaName with grant option;
GRANT SELECT, INSERT, DELETE, UPDATE ON BIOSARDB.TREE_ITEM_TYPE TO &&securitySchemaName with grant option;
GRANT SELECT, INSERT, DELETE, UPDATE ON BIOSARDB.TREE_ITEM TO &&securitySchemaName with grant option;
GRANT EXECUTE ON BIOSARDB.TREE TO &&securitySchemaName with grant option;


commit;