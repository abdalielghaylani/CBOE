


Connect &&schemaName/&&schemaPass@&&serverName		

-- Rewrite for performance
CREATE OR REPLACE VIEW db_vw_formitems_compact as
SELECT
                        f.formgroup_id,
                        f.formtype_id,
                        ft.table_order,
                        fi.column_order,
                        t.table_name as table_name,
                        fi.table_id,
                        t.display_name as table_display_name,
                        t.description as table_description,
            bc.column_name as base_column_name,
                        c.column_name,
                        c.display_name,
                        fi.width,
                        fi.height,
                        c.datatype as data_type,
                        o.disp_opt_name as display_option,
                        d.disp_typ_name as display_type_name,
                        c.column_id,
                        c.lookup_column_display as lookup_display_column__id,
                        lctd.table_name as lookup_table_name,
                        lctd.table_id as lookup_table_id,
                        lctd.column_name as lookup_column_name,
                        lctd.lookup_display_name as lookup_display_column,
                        lctd.display_name as lookup_display_name,
                        c.lookup_join_type,
                        c.lookup_sort_direct as lookup_sort_direction,
                        lctd.datatype as lookup_display_datatype,
                        fi.format_mask
 FROM
                        biosardb.DB_FORM_ITEM fi,
                        biosardb.db_form f,
                        biosardb.DB_FORMGROUP_TABLES ft,
                        biosardb.db_table t,
                        biosardb.db_column c,
                        biosardb.db_column bc,
                        biosardb.db_display_option o,
                        biosardb.db_display_type d,
                        (select lc.column_name,
                          c.column_id,
                          lt.table_name,
                          lt.table_id,
                          ld.display_name,
                          ld.column_name as lookup_display_name,
                          ld.datatype
                           from biosardb.db_column lc, biosardb.db_column c, biosardb.db_table lt, biosardb.db_column ld
                          where c.lookup_column_id = lc.column_id
                            and c.lookup_table_id = lt.table_id
                            and c.lookup_column_display = ld.column_id) lctd
 WHERE
		fi.form_id = f.form_id
 AND    fi.form_id >0		
 AND    fi.table_id = ft.table_id
 AND    ft.formgroup_id = f.formgroup_id
 AND    t.table_id = fi.table_id
 AND    c.column_id = fi.column_id
 AND    t.base_column_id  = bc.column_id(+)
 AND    fi.disp_opt_id = o.disp_opt_id(+)
 AND    fi.disp_typ_id = d.disp_typ_id
 AND    fi.column_id = lctd.column_id(+)
 ORDER BY
        f.formtype_id, ft.table_order, fi.column_order asc;

commit;

@@pkg_tree_def.sql
@@pkg_tree.sql

Connect &&InstallUser/&&sysPass@&&serverName

create public synonym tree_item for biosardb.tree_item;
create public synonym tree_item_type for biosardb.tree_item_type;
create public synonym tree_node for biosardb.tree_node;
create public synonym tree_type for biosardb.tree_type;


Connect &&schemaName/&&schemaPass@&&serverName

GRANT SELECT ON biosardb.tree_seq TO BIOSAR_BROWSER_ADMIN;
GRANT SELECT ON biosardb.tree_seq TO BIOSAR_BROWSER_USER_ADMIN;
GRANT SELECT ON biosardb.tree_seq TO BIOSAR_BROWSER_USER;
GRANT SELECT ON biosardb.tree_seq TO BIOSAR_BROWSER_USER_BROWSER;
GRANT SELECT ON biosardb.tree_seq TO &&securitySchemaName WITH GRANT OPTION;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('SEARCH_USING_FORMGROUP', 'SELECT', 'BIOSARDB', 'TREE_SEQ');
commit;
