

--#########################################################
--update biosardb schema for bioviz, excel and tree control features
--#########################################################

Connect &&schemaName/&&schemaPass@&&serverName	

drop table tree_item;
drop table tree_node;
drop table tree_type;
drop table tree_item_type;
drop sequence tree_seq;
drop sequence user_tree_type_seq;