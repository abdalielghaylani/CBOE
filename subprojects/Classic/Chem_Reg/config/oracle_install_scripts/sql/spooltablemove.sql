--  spool commands to grant old table grants on new table
-- Used later during CLOB update script
set pagesize 0
column order_col1 noprint
column order_col2 noprint

set termout off

spool sql\moveToNewTablespaces.sql

select 'prompt ''moving tables to new table spaces...''' from dual;

select decode( segment_type, 'TABLE',
                       segment_name, table_name ) order_col1,
       decode( segment_type, 'TABLE', 1, 2 ) order_col2,
      'alter ' || segment_type || ' ' || segment_name ||
      decode( segment_type, 'TABLE', ' move ', ' rebuild ' )||chr(10) ||
      ' tablespace '||decode( segment_type, 'TABLE', ' &&tableSpaceName ', ' &&indexTableSpaceName ')|| chr(10)||';'
      --' storage ( initial ' || initial_extent ||
    -- ' freelists ' ||freelists || ');'
  from user_segments,
       (select table_name, index_name from user_indexes )
 where segment_type in ( 'TABLE', 'INDEX' )
   and segment_name = index_name (+)
   and decode( segment_type, 'TABLE',
                       segment_name, table_name ) NOT in ('MOLFILES')
 order by 1, 2
/

spool off

set termout on
