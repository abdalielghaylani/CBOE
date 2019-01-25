-- Add/modify columns 
begin
  -- Update the table
 update REGDB.temporary_batch SET STATUSID = '1' where STATUSID IS NULL; 
 
end;
/