
Connect &&InstallUser/&&sysPass@&&serverName
GRANT SELECT ANY DICTIONARY TO &&schemaName;
Grant create view to &&schemaName;

Connect &&schemaName/&&schemaPass@&&serverName		

-- Add a column to store formatting mask for numbers and dates
alter table db_form_item add (format_mask varchar2(50) NULL);

-- Fix problem with Date field Width
update db_display_type set default_width = 10 where disp_typ_id = 6;

-- Remove bad lookup for solvate name
update BIOSARDB.DB_COLUMN
set   	LOOKUP_TABLE_ID = null,
		LOOKUP_COLUMN_ID = null,
		LOOKUP_COLUMN_DISPLAY = null,
		LOOKUP_JOIN_TYPE = null,
		LOOKUP_SORT_DIRECT = null
where table_id = (select table_id from BIOSARDB.DB_TABLE
					where table_name = 'REGDB.BATCHES')
and column_name = 'SOLVATE_NAME';

commit;

-- Add formatMask to compact view
CREATE OR REPLACE VIEW db_vw_formitems_compact 
	AS
	SELECT
		(select formgroup_id from biosardb.db_form where fi.form_id = db_form.form_id) as formgroup_id,
		(select formtype_id from biosardb.db_form where  fi.form_id=db_form.form_id) as formtype_id,
		(select distinct table_order from biosardb.db_formgroup_tables ft where ft.table_id=fi.table_id and ft.formgroup_id=(select formgroup_id from biosardb.db_form where fi.form_id = db_form.form_id)) as table_order,
		fi.column_order as column_order,
		(select table_name from biosardb.db_table bt where bt.table_id = fi.table_id) as table_name,table_id as table_id,
		(select display_name from biosardb.db_table bt where bt.table_id = fi.table_id) as table_display_name,
		(select description from biosardb.db_table bt where bt.table_id = fi.table_id) as table_description,
		(select column_name from biosardb.db_column where column_id = (select base_column_id from db_table where fi.table_id=db_table.table_id)) as base_column_name ,
		(select column_name from biosardb.db_column ct where ct.column_id = fi.column_id) as column_name,
		(select display_name from biosardb.db_column ct where ct.column_id = fi.column_id) as display_name,
		fi.width as width, fi.height as height,
		(select datatype from biosardb.db_column ct where ct.column_id = fi.column_id) as data_type,
		(select disp_opt_name from biosardb.db_display_option where  biosardb.db_display_option.disp_opt_id= fi.disp_opt_id)as display_option,
		(select disp_typ_name from biosardb.db_display_type where  biosardb.db_display_type.disp_typ_id= fi.disp_typ_id)as display_type_name,
		(select column_id from biosardb.db_column ct where ct.column_id = fi.column_id)  as column_id,
		(select lookup_column_display from biosardb.db_column ct where ct.column_id = fi.column_id)  as lookup_display_column__id,
		(select table_name from biosardb.db_table,biosardb.db_column where fi.column_id=db_column.column_id and db_column.lookup_table_id = db_table.table_id) as lookup_table_name ,
		(select biosardb.db_table.table_id  from biosardb.db_table,biosardb.db_column where fi.column_id=db_column.column_id and db_column.lookup_table_id = db_table.table_id) as lookup_table_id ,
		(select ct2.column_name from biosardb.db_column ct2, biosardb.db_column ct1 where ct1.lookup_column_id = ct2.column_id and fi.column_id = ct1.column_id) as lookup_column_name,
		(select ct2.column_name from biosardb.db_column ct2, biosardb.db_column ct1 where ct1.lookup_column_display = ct2.column_id and fi.column_id = ct1.column_id) as lookup_display_column,
		(select ct2.display_name from biosardb.db_column ct2, biosardb.db_column ct1 where ct1.lookup_column_display = ct2.column_id and fi.column_id = ct1.column_id) as lookup_display_name,
		(select lookup_join_type from biosardb.db_column ct where ct.column_id = fi.column_id) as lookup_join_type,
		(select lookup_sort_direct from biosardb.db_column ct where ct.column_id = fi.column_id) as lookup_sort_direction,
		(select ct2.datatype from biosardb.db_column ct2, biosardb.db_column ct1 where ct1.lookup_column_display = ct2.column_id and fi.column_id = ct1.column_id) as lookup_display_datatype,
		format_mask
	FROM 
		biosardb.db_form_item fi
	WHERE 
		fi.form_id >0
	ORDER BY 
		formtype_id,table_order,column_order asc;


CREATE OR REPLACE VIEW DB_VW_COL_CONSTRAINTS
	AS
	SELECT
		ALL_CONS_COLUMNS.OWNER,ALL_CONS_COLUMNS.CONSTRAINT_NAME,ALL_CONS_COLUMNS.TABLE_NAME,
   		ALL_CONS_COLUMNS.COLUMN_NAME, ALL_CONS_COLUMNS.POSITION, ALL_CONSTRAINTS.CONSTRAINT_TYPE, ALL_CONSTRAINTS.R_CONSTRAINT_NAME
	FROM ALL_CONS_COLUMNS, ALL_CONSTRAINTS
	WHERE ALL_CONS_COLUMNS.CONSTRAINT_NAME = ALL_CONSTRAINTS.CONSTRAINT_NAME;



commit;

@@pkg_tree_def.sql
@@pkg_tree.sql

-- missing grants
grant select, insert, update,delete on tree_node to BIOSAR_BROWSER_USER_ADMIN;
grant select, insert, update,delete  on tree_item to BIOSAR_BROWSER_USER_ADMIN;
grant select, insert, update,delete  on tree_type to BIOSAR_BROWSER_USER_ADMIN;
grant select  on tree_item_type to BIOSAR_BROWSER_USER_ADMIN;
grant execute on tree to BIOSAR_BROWSER_USER_ADMIN;

grant select, insert, update,delete on tree_node to BIOSAR_BROWSER_USER;
grant select, insert, update,delete  on tree_item to BIOSAR_BROWSER_USER;
grant select, insert, update,delete  on tree_type to BIOSAR_BROWSER_USER;
grant select on tree_item_type to BIOSAR_BROWSER_USER;
grant execute on tree to BIOSAR_BROWSER_USER;

grant select, insert, update,delete on tree_node to BIOSAR_BROWSER_USER_BROWSER;
grant select, insert, update,delete  on tree_item to BIOSAR_BROWSER_USER_BROWSER;
grant select, insert, update,delete  on tree_type to BIOSAR_BROWSER_USER_BROWSER;
grant select  on tree_item_type to BIOSAR_BROWSER_USER_BROWSER;
grant execute on tree to BIOSAR_BROWSER_USER_BROWSER;