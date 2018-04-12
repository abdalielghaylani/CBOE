connect &&cartSchemaName/&&cartSchemaPass@&&serverName;

Create or Replace package fastIndexAccess as
	function RowIdToMolWeight(pRid in rowid, pTableName in varchar2) return float;

	function RowIdToFormula(pRid in rowid, pTableName in varchar2) return varchar2;
end fastIndexAccess;
/

Create or Replace package body fastIndexAccess
as

-- Global variables
g_cursor number := dbms_sql.open_cursor;  -- Global reusable cursor for molweight
g_last_tname varchar2(30); -- Remember the last table name  
g_cursor2 number := dbms_sql.open_cursor;  -- Global reusable cursor for formula
g_last_tname2 varchar2(30); -- Remember the last table name  

function RowIdToMolWeight(pRid in rowid, pTableName in varchar2) return float
is
mw float;
-- variable to catch the number of rows returned
l_numrows number;
BEGIN
   -- Parse the cursor only if the table name changes
   if (g_last_tname <> pTableName or g_last_tname is null) then 
   	dbms_sql.parse(	g_cursor, 
   					'select molweight from ' || pTableName || ' where rid = :x' , 
   					dbms_sql.NATIVE);
   	-- Define output column
   	dbms_sql.define_column(g_cursor, 1, mw);
   	
   	-- Remember the table name
   	g_last_tname := pTableName;
   	
   end if;
   
   -- Bind it
   dbms_sql.bind_variable(g_cursor, ':x', pRid);
   
   -- Execute it
   l_numrows := dbms_sql.execute(g_cursor);
   
   -- Fetch the row
  if dbms_sql.fetch_rows(g_cursor) > 0 then 							
   -- Grab the result
   dbms_sql.column_value(g_cursor, 1, mw);
   
  else
    mw := 0;
  end if; 
   return mw;
END RowIdToMolWeight;


function RowIdToFormula(pRid in rowid, pTableName in varchar2) return varchar2
is
fm varchar2(200);
-- variable to catch the number of rows returned
l_numrows number;
BEGIN
   -- Parse the cursor only if the table name changes
   if (g_last_tname2 <> pTableName or g_last_tname2 is null) then 
   	dbms_sql.parse(	g_cursor2, 
   					'select formula from ' || pTableName || ' where rid = :x' , 
   					dbms_sql.NATIVE);
   	-- Define output column
   	dbms_sql.define_column(g_cursor2, 1, fm,200);
   	
   	-- Remember the table name
   	g_last_tname2 := pTableName;
   	
   end if;
   
   -- Bind it
   dbms_sql.bind_variable(g_cursor2, ':x', pRid);
   
   -- Execute it
   l_numrows := dbms_sql.execute(g_cursor2);
   
   -- Fetch the row
  if dbms_sql.fetch_rows(g_cursor2) > 0 then 							
   -- Grab the result
   dbms_sql.column_value(g_cursor2, 1, fm);
   
  else
    fm := '';
  end if; 
   return fm;
END RowIdToFormula;

end fastIndexAccess;

/


grant execute on fastIndexAccess to public;
