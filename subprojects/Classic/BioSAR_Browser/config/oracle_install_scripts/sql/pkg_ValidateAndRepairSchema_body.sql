
CREATE OR REPLACE PACKAGE BODY ValidateAndRepairSchema AS

  -- These tables are in db_table, but no longer exist in the database.
  FUNCTION FindNoLongerExistTables RETURN VARCHAR2 IS
    CURSOR cMissingTables IS
      SELECT  owner, table_short_name
      FROM    db_table
      MINUS
      (SELECT  all_tables.owner, all_tables.table_name
       FROM    all_tables, db_schema
       WHERE   db_schema.owner = all_tables.owner
       UNION ALL
       SELECT  all_views.owner, all_views.view_name
       FROM    all_views, db_schema
       WHERE   all_views.owner = db_schema.owner);

    vResult VARCHAR2(4000);
  BEGIN
    FOR vMissingTables IN cMissingTables LOOP
      vResult := vResult || vMissingTables.owner || '.' ||
                   vMissingTables.table_short_name || ', ';
    END LOOP;

    RETURN SUBSTR(vResult, 1, LENGTH(VResult) - 2);
  END;

  -- These columns are in db_column, but no longer exist in the database.
  -- Does not include columns that no longer exist because the table 
  -- no longer exists.
  FUNCTION FindNoLongerExistTableColumns RETURN VARCHAR2 IS
    CURSOR cMissingColumns IS
      SELECT db_table.owner, db_table.table_short_name, db_column.column_name
      FROM   db_table, db_column
      WHERE  db_table.table_id = db_column.table_id AND
             (db_table.owner, db_table.table_short_name) NOT IN
               (SELECT  owner, table_short_name
                FROM    db_table
                MINUS
               (SELECT  all_tables.owner, all_tables.table_name
                FROM    all_tables, db_schema
                WHERE   db_schema.owner = all_tables.owner
                UNION ALL
                SELECT  all_views.owner, all_views.view_name
                FROM    all_views, db_schema
                WHERE   all_views.owner = db_schema.owner))
      MINUS
      SELECT owner, table_name, column_name
      FROM   all_tab_columns
      WHERE  owner IN (SELECT owner FROM db_schema);

    vResult VARCHAR2(4000);
  BEGIN
    FOR vMissingColumns IN cMissingColumns LOOP
      vResult := vResult || vMissingColumns.owner || '.' ||
                   vMissingColumns.table_short_name || '.' ||
                   vMissingColumns.column_name || ', ';
    END LOOP;

    RETURN SUBSTR(vResult, 1, LENGTH(VResult) - 2);
  END;

  -- These columns are not in db_column, but exist in the database.
  -- Insert them.
  PROCEDURE InsertMissingTableColumns IS
  BEGIN
    INSERT INTO db_column
    (COLUMN_ID, TABLE_ID, 
     COLUMN_NAME, 
     DISPLAY_NAME, 
     DESCRIPTION, 
     IS_VISIBLE, DATATYPE, LOOKUP_TABLE_ID, LOOKUP_COLUMN_ID, 
     LOOKUP_COLUMN_DISPLAY, MST_FILE_PATH, LENGTH, SCALE, 
     PRECISION, NULLABLE)
    SELECT db_column_seq.NEXTVAL, db_table.table_id, 
           all_tab_columns.column_name,
           INITCAP(REPLACE(all_tab_columns.column_name, '_', ' ')),
           INITCAP(REPLACE(all_tab_columns.column_name, '_', ' ')),
           'N', all_tab_columns.data_type, NULL, NULL,
           NULL, NULL, all_tab_columns.data_length, all_tab_columns.data_scale,
           all_tab_columns.data_precision, all_tab_columns.nullable
    FROM   all_tab_columns, db_schema, db_table
    WHERE  db_schema.owner = all_tab_columns.owner AND
           db_table.owner = db_schema.owner AND
           db_table.table_short_name = all_tab_columns.table_name AND
           NOT EXISTS (SELECT 1
                       FROM   db_column
                       WHERE  db_table.table_id = db_column.table_id AND
                              db_column.column_name = all_tab_columns.column_name);
  END;

  -- These columns exist in db_column and the database and have the same datatype.
  -- But their, length, scale, precision, or nullability are different.
  -- Update BioSar so that they are the same.
  PROCEDURE UpdateOutOfSyncTableColumns IS
  BEGIN
    UPDATE db_column
    SET    (length, scale, precision, nullable) =
            (SELECT all_tab_columns.data_length, all_tab_columns.data_scale,
                    all_tab_columns.data_precision, all_tab_columns.nullable
             FROM   all_tab_columns, db_schema, db_table
             WHERE  db_schema.owner = all_tab_columns.owner AND
                    db_table.owner = db_schema.owner AND
                    db_table.table_short_name = all_tab_columns.table_name AND
                    db_table.table_id = db_column.table_id AND
                    db_column.column_name = all_tab_columns.column_name)
    WHERE  EXISTS (SELECT 1
                   FROM   all_tab_columns, db_schema, db_table
                   WHERE  db_schema.owner = all_tab_columns.owner AND
                          db_table.owner = db_schema.owner AND
                          db_table.table_short_name = all_tab_columns.table_name AND
                          db_table.table_id = db_column.table_id AND
                          db_column.column_name = all_tab_columns.column_name AND
                          db_column.datatype = all_tab_columns.data_type AND
                          (db_column.length <> all_tab_columns.data_length OR
                           db_column.scale <> all_tab_columns.data_scale OR
                           db_column.precision <> all_tab_columns.data_precision OR
                           db_column.nullable <> all_tab_columns.nullable));
  END;

  -- These columns exist in db_column and the database and do not have the same datatype.
  FUNCTION FindDifferingDatatypeTableCols RETURN VARCHAR2 IS
    CURSOR cDifferingDatatypeTableCols IS
      SELECT db_table.owner, db_table.table_short_name, db_column.column_name,
             db_column.datatype, all_tab_columns.data_type
      FROM   all_tab_columns, db_schema, db_table, db_column
      WHERE  db_schema.owner = all_tab_columns.owner AND
             db_table.owner = db_schema.owner AND
             db_table.table_short_name = all_tab_columns.table_name AND
             db_table.table_id = db_column.table_id AND
             db_column.column_name = all_tab_columns.column_name AND
             db_column.datatype <> all_tab_columns.data_type;

    vResult VARCHAR2(4000);
  BEGIN
    FOR vDifferingDatatypeTableCols IN cDifferingDatatypeTableCols LOOP
      vResult := vResult || vDifferingDatatypeTableCols.owner || '.' ||
                   vDifferingDatatypeTableCols.table_short_name || '.' ||
                   vDifferingDatatypeTableCols.column_name || 
                   ' BioSAR: ' || vDifferingDatatypeTableCols.datatype || 
                   ' Oracle: ' ||  vDifferingDatatypeTableCols.data_type || ', ';
                         dbms_output.put_line(vDifferingDatatypeTableCols.owner || '.' ||
                   vDifferingDatatypeTableCols.table_short_name || '.' ||
                   vDifferingDatatypeTableCols.column_name || 
                   ' is datatype ' || vDifferingDatatypeTableCols.datatype || 
                   ' in BioSAR and ' ||  vDifferingDatatypeTableCols.data_type || ' in Oracle., ');
    END LOOP;

    RETURN SUBSTR(vResult, 1, LENGTH(VResult) - 2);
  END;
END;
/
