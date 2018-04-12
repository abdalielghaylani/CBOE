Connect &&schemaName/&&schemaPass@&&serverName		

alter table BIOSARDB.DB_FORM_ITEM
	ADD (LINK varchar2(2000), LINKTEXT varchar2(2000));


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
                        fi.format_mask,
                        fi.link as link,
                        fi.linktext as linktext
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

CREATE OR REPLACE VIEW DB_VW_FORMITEMS_ALL
	AS
   	SELECT
		DB_FORM_ITEM.FORM_ITEM_ID, DB_FORM_ITEM.WIDTH, DB_FORM_ITEM.HEIGHT,
          	DB_FORM_ITEM.COLUMN_ORDER, DB_FORM_ITEM.TABLE_ID,
          	DB_FORM_ITEM.COLUMN_ID, DB_FORM.FORM_ID,DB_FORM.FORM_NAME,DB_FORM.FORMGROUP_ID,DB_FORM.FORMTYPE_ID,DB_FORM.URL,
	  	DB_DISPLAY_TYPE.DISP_TYP_ID,DB_DISPLAY_TYPE.DISP_TYP_NAME,DB_DISPLAY_TYPE.DEFAULT_WIDTH,DB_DISPLAY_TYPE.DEFAULT_HEIGHT,
          	DB_DISPLAY_OPTION.DISP_OPT_ID, DB_DISPLAY_OPTION.DISP_OPT_NAME, DB_DISPLAY_OPTION.DISPLAY_NAME AS DISP_OPT_DISPLAY_NAME,
          	DB_TABLE.OWNER, DB_TABLE.TABLE_NAME, DB_TABLE.TABLE_SHORT_NAME,
          	DB_TABLE.DISPLAY_NAME AS TABLE_DISPLAY_NAME, DB_TABLE.BASE_COLUMN_ID,
          	DB_TABLE.DESCRIPTION AS TABLE_DESCRIPTION, DB_TABLE.IS_VIEW,
          	DB_COLUMN.COLUMN_NAME, DB_COLUMN.DISPLAY_NAME, DB_COLUMN.DESCRIPTION,
          	DB_COLUMN.IS_VISIBLE, DB_COLUMN.DATATYPE, DB_COLUMN.LOOKUP_TABLE_ID,
          	DB_COLUMN.LOOKUP_COLUMN_ID, DB_COLUMN.LOOKUP_COLUMN_DISPLAY,DB_COLUMN.LOOKUP_JOIN_TYPE,DB_COLUMN.LOOKUP_SORT_DIRECT,
          	DB_COLUMN.MST_FILE_PATH, DB_COLUMN.LENGTH, DB_COLUMN.SCALE,
          	DB_COLUMN.PRECISION, DB_COLUMN.NULLABLE, DB_FORM_ITEM.V_COLUMN_ID,DB_COLUMN.DEFAULT_COLUMN_ORDER,
          	DB_FORMGROUP_TABLES.TABLE_ORDER, DB_FORM_ITEM.FORMAT_MASK, DB_FORM_ITEM.LINK, DB_FORM_ITEM.LINKTEXT
     	FROM
		DB_FORM_ITEM,
         	DB_FORM,
          	DB_COLUMN,
          	DB_TABLE,
          	DB_DISPLAY_TYPE,
         	DB_DISPLAY_OPTION,
          	DB_FORMGROUP_TABLES
    	WHERE
		DB_FORM_ITEM.FORM_ID = DB_FORM.FORM_ID
      		AND DB_FORM_ITEM.COLUMN_ID = DB_COLUMN.COLUMN_ID(+)
     		AND DB_FORM_ITEM.TABLE_ID = DB_TABLE.TABLE_ID
      		AND DB_FORM_ITEM.DISP_TYP_ID = DB_DISPLAY_TYPE.DISP_TYP_ID(+)
      		AND DB_FORM_ITEM.DISP_OPT_ID = DB_DISPLAY_OPTION.DISP_OPT_ID(+)
      		AND DB_FORM.FORMGROUP_ID = DB_FORMGROUP_TABLES.FORMGROUP_ID
      		AND DB_FORM_ITEM.TABLE_ID  = DB_FORMGROUP_TABLES.TABLE_ID;

commit;

@@globals.sql
@@RemoveBadLookup.sql
