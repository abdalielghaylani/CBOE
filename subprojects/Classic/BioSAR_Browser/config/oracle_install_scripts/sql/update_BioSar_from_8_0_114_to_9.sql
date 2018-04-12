


--#########################################################
--update biosardb schema for bioviz, excel and tree control features
--#########################################################


Connect &&schemaName/&&schemaPass@&&serverName		
--Connect &&InstallUser/&&sysPass@&&serverName
--truncate db_xml_templ table so all form definitions will be regenerated

truncate table db_xml_templ_def;


--add field to formgroup table for bioviz and excel integration
ALTER TABLE BIOSARDB.DB_FORMGROUP ADD (BIOVIZ VARCHAR2(1) DEFAULT 0 NULL);
ALTER TABLE BIOSARDB.DB_FORMGROUP ADD (EXCEL VARCHAR2(1) DEFAULT 0 NULL);

--add tables, fields and procedures for tree control



-- Defines a hierarchical tree type (ie. Projects, Workgroups, Categories...)
create table TREE_TYPE(
	id number(8,0) not null,
	name varchar2(50) not null,
	description varchar2(254) null,
	user_name varchar2(30) null,
	CONSTRAINT TREE_TYPE_PK Primary Key(id)
);

create sequence user_tree_type_seq start with 1000 increment by 1;

-- Defines the types of items that can be organized in a tree (ie. Tables, Forms...)
create table TREE_ITEM_TYPE(
	id number(8,0) not null,
	name varchar2(50) null,
	table_name varchar2(30) not null,
	id_field_name varchar2(30) not null,
	display_field_name varchar2(250) not null,
	CONSTRAINT TREE_ITEM_TYPE_PK Primary Key(id)
);

-- Stores tree nodes
create table TREE_NODE(
	id number(8,0) not null,
	parent_id number(8,0) null,
	node_name varchar2(50) not null,
	node_description varchar2(254) null,
	tree_type_id number(8,0) not null,
	CONSTRAINT TREE_NODE_PK PRIMARY KEY(id)
   	using index);





alter table TREE_NODE
	add constraint FK_PARENT_NODE foreign key (
		parent_id)
	 references TREE_NODE(id);

create index parent_node_idx on tree_node(parent_id);

alter table TREE_NODE
	add constraint FK_TREE_TYPE foreign key (
		tree_type_id)
	 references TREE_TYPE(id);

create index tree_type_idx on tree_node(tree_type_id);


-- Stores items associated with a tree node
create table TREE_ITEM(
	id number(8,0) not null,
	node_id number(8,0) not null,
	item_type_id number(8,0) not null,
	item_id number(10,0) not null,
	CONSTRAINT TREE_ITEM_PK primary key(id)) organization index;

create sequence TREE_SEQ increment by 1 start with 1000;

alter table TREE_ITEM
	add constraint FK_NODE foreign key (
		node_id)
	 references TREE_NODE(id);

create index item_node_idx on tree_item(node_id);

alter table TREE_ITEM
	add constraint FK_ITEM_TYPE foreign key (
		item_type_id)
	 references TREE_ITEM_TYPE(id);

create index item_type_idx on tree_item(item_type_id);

alter table TREE_ITEM
	add constraint U_ITEM Unique(node_id, item_type_id, item_id);




@@pkg_tree_def.sql;
@@pkg_tree.sql;

-- TO DO: figure out appropriate grants.  Hit it with a hammer for now
grant select, insert, update,delete on tree_node to biosar_browser_admin;
grant select, insert, update,delete  on tree_item to biosar_browser_admin;
grant select, insert, update,delete  on tree_type to biosar_browser_admin;
grant select  on tree_item_type to biosar_browser_admin;
grant execute on tree to biosar_browser_admin;

grant select, insert, update,delete on tree_node to BIOSAR_USER_ADMIN;
grant select, insert, update,delete  on tree_item to BIOSAR_USER_ADMIN;
grant select, insert, update,delete  on tree_type to BIOSAR_USER_ADMIN;
grant select  on tree_item_type to BIOSAR_USER_ADMIN;
grant execute on tree to BIOSAR_USER_ADMIN;

grant select, insert, update,delete on tree_node to BIOSAR_USER;
grant select, insert, update,delete  on tree_item to BIOSAR_USER;
grant select, insert, update,delete  on tree_type to BIOSAR_USER;
grant select on tree_item_type to BIOSAR_USER;
grant execute on tree to BIOSAR_USER;

grant select, insert, update,delete on tree_node to BIOSAR_USER_BROWSER;
grant select, insert, update,delete  on tree_item to BIOSAR_USER_BROWSER;
grant select, insert, update,delete  on tree_type to BIOSAR_USER_BROWSER;
grant select  on tree_item_type to BIOSAR_USER_BROWSER;
grant execute on tree to BIOSAR_USER_BROWSER;

-- Define a Projects hierarchy to organize forms
insert into tree_type (id, name, description) values (1, 'Projects', 'Tree of Projects');
insert into tree_item_type (id, name, table_name, id_field_name, display_field_name) values (1, 'Table', 'DB_TABLE', 'TABLE_ID', 'DISPLAY_NAME');

-- Define a categories hierarchy to organize tables
insert into tree_type (id, name, description) values (2, 'Public Categories', 'Tree of Public Categories');
insert into tree_item_type (id, name, table_name, id_field_name, display_field_name) values (2, 'Form', 'DB_FORMGROUP', 'FORMGROUP_ID', 'FORMGROUP_NAME || ''-'' || DESCRIPTION');

select * from tree_node;
select * from tree_item;

commit;



--Define the project and public categories route nodes
insert into tree_node(id, parent_id,node_name, node_description, tree_type_id)values('1', null, 'Child Tables','Child Tables Root Node', '1');
insert into tree_node(id, parent_id,node_name, node_description, tree_type_id)values('2', null, 'Public Forms','Public Forms Root Node', '2');




commit;



Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'EXECUTE', 'BIOSARDB', 'TREE');

INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'SELECT', 'BIOSARDB', 'TREE_NODE');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'DELETE', 'BIOSARDB', 'TREE_NODE');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'INSERT', 'BIOSARDB', 'TREE_NODE');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'UPDATE', 'BIOSARDB', 'TREE_NODE');


INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'SELECT', 'BIOSARDB', 'TREE_ITEM_TYPE');


INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'SELECT', 'BIOSARDB', 'TREE_TYPE');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'DELETE', 'BIOSARDB', 'TREE_TYPE');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'INSERT', 'BIOSARDB', 'TREE_TYPE');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'UPDATE', 'BIOSARDB', 'TREE_TYPE');


INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'SELECT', 'BIOSARDB', 'TREE_ITEM');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'DELETE', 'BIOSARDB', 'TREE_ITEM');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'INSERT', 'BIOSARDB', 'TREE_ITEM');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'UPDATE', 'BIOSARDB', 'TREE_ITEM');