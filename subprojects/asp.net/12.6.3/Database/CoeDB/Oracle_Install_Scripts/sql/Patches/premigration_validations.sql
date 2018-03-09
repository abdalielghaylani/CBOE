SET serveroutput on
prompt .
prompt 'Executing premigration validation...'

WHENEVER SQLERROR EXIT
DECLARE
	cnt int;
	errorsFound char(1):='N';
BEGIN
	-- Check for non-existing site assignments
	select count(*) into cnt
	from &&securitySchemaNameOld..people p, &&securitySchemaNameOld..sites s
	where s.site_id(+) = p.site_id
	and p.site_id is not null
	and s.site_id is null;

	if cnt > 0 then
	dbms_output.put_line('*****************************************************************');
	dbms_output.put_line('WARNING: Invalid data found in CS_SECURITY.People table.');
	dbms_output.put_line('There are rows in people table that point to non-existent sites.');
	dbms_output.put_line('Adding Sites to correct for the problem -  sites will be named .');
        dbms_output.put_line('NEWSITE - side_id .   This can be manually changed later');
	dbms_output.put_line('******************************************************************');
	dbms_output.put_line(CHR(13)||CHR(10));
    
    
    Declare
        cursor cur is select distinct p.site_id
        from &&securitySchemaNameOld..people p, &&securitySchemaNameOld..sites s
        where s.site_id(+) = p.site_id
        and p.site_id is not null
        and s.site_id is null;
        
        rec cur%rowtype;
    
    Begin
    
        execute immediate('Alter trigger &&securitySchemaNameOld..SITES_TRIG disable');
        
        open cur;
        loop
            fetch cur into rec;
            exit when cur%notfound;
            
            insert into &&securitySchemaNameOld..sites values (rec.site_id, rec.site_id, 'NEWSITE-'||rec.site_id, 1) ;
            
            dbms_output.put_line('Added site - NEWSITE-'||rec.site_id);
            
        end loop;
        close cur;
        
       execute immediate('Alter trigger &&securitySchemaNameOld..SITES_TRIG enable'); 
       
       -- we do not need to worry about the sequence becuase the CS_SECURITY schema is being dropped
    end;
    
	end if;
    
    
	
	-- Look for duplicate site codes
	select count(*) into cnt from (
	select 1
	from cs_security.sites
	group by site_code
	having count(site_code) > 1);
			
	if cnt > 0 then
		errorsFound := 'Y';
		dbms_output.put_line('*****************************************************************');
		dbms_output.put_line('WARNING: Duplicate site_codes found in  CS_SECURITY.Sites table.');
		dbms_output.put_line('There are rows in sites table that have the same site_code.');
		dbms_output.put_line('Execute the following SQL to identify the offending rows:');
		dbms_output.put_line(CHR(13)||CHR(10));
		dbms_output.put_line('	select site_id, site_code DupCode, site_name');
		dbms_output.put_line('	from cs_Security.sites where site_code in(');
		dbms_output.put_line('	select site_code');
		dbms_output.put_line('	from cs_security.sites');
		dbms_output.put_line('	group by site_code having count(site_code) > 1)');
		dbms_output.put_line('	order by site_code;');
		dbms_output.put_line(CHR(13)||CHR(10));
		dbms_output.put_line('Correct this problem before proceeding with the migration.');
		dbms_output.put_line('******************************************************************');
		dbms_output.put_line(CHR(13)||CHR(10));
	end if;
	
	if errorsFound <> 'N' then
		dbms_output.put_line('*****************************************************************');
		dbms_output.put_line('PRE-MIGRATION VALIDATION FAILED');
		dbms_output.put_line('PATCHING HAS BEEN TERMINATED');
		dbms_output.put_line('PLEASE REVIEW AND CORRECT THE WARNINGS ABOVE BEFORE RE-EXECUTING THE PATCH');
		dbms_output.put_line(CHR(13)||CHR(10));
		dbms_output.put_line('******************************************************************');
		dbms_output.put_line(CHR(13)||CHR(10));
		raise_application_error(-20000, 'Patch terminated due to validation errors.');
	end if;
		
END;
/
WHENEVER SQLERROR CONTINUE


