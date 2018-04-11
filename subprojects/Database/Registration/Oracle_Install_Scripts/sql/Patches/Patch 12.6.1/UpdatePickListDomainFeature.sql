-- Add columns to views if not exists:

declare
  viewText varchar2(250) ;
  LSortOrderColumnExists integer := 0;
  LActiveColumnExists integer := 0;
  begin
    select count(*) into LSortOrderColumnExists from all_tab_cols t where t.owner = upper('REGDB')
      and t.table_name = 'VW_UNIT' and t.column_name = 'SORTORDER';
    select count(*) into LActiveColumnExists from all_tab_cols t where t.owner = upper('REGDB')
      and t.table_name = 'VW_UNIT' and t.column_name = 'ACTIVE';
    select TEXT into viewText  from dba_views  where OWNER = 'REGDB'  and VIEW_NAME  = 'VW_UNIT';
    if instr(upper(viewText),'PICKLIST')>0 then
      if ( LActiveColumnExists = 0 ) then  
       &&schemaName..configurationcompoundregistry.addfieldtoview('ACTIVE', 'ACTIVE', 'VW_UNIT', 'PICKLIST');
      end if;
      if ( LSortOrderColumnExists = 0 ) then  
       &&schemaName..configurationcompoundregistry.addfieldtoview('SORTORDER', 'SORTORDER', 'VW_UNIT', 'PICKLIST');
      end if;
    end if;
end;

/



