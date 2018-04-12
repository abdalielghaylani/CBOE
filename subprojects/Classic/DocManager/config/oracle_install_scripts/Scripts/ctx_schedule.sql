REM 
Rem  Copyright (c) 2000 by Oracle Corporation
Rem
Rem    NAME
Rem      CTX_SCHEDULE_DOCMANAGER.sql - Text index synchronize/optimize script
Rem
Rem    NOTES
Rem      sets up tables, views and a package - CTX_SCHEDULE_DOCMANAGER - in the
Rem      CTXSYS schema. Users can call CTX_SCHEDULE_DOCMANAGER to regularly synchronize
Rem      and/or optimize their Text indexes.
Rem
Rem      CTX_SCHEDULE_DOCMANAGER calls DBMS_JOBS to schedule index operations.
Rem      Before running this script, please ensure that your database
Rem      is set up to run DBMS_JOBS.  job_queue_processes must be set
Rem      (and non-zero) in init.ora.  See Administrator's Guide for more
Rem      information.
Rem
Rem      Note that operations scheduled via CTX_SCHEDULE_DOCMANAGER are run 
Rem      as the CTXSYS user.
Rem
Rem    USAGE
Rem      as CTXSYS, in SQL*Plus:
Rem
Rem        @CTX_SCHEDULE_DOCMANAGER
Rem
Rem      then as any user with a Text index, call 
Rem        CTX_SCHEDULE_DOCMANAGER.startup
Rem        CTX_SCHEDULE_DOCMANAGER.stop
Rem        CTX_SCHEDULE_DOCMANAGER.get_wait_time
Rem
Rem      Typical usage example, from SQL*Plus:
Rem
Rem        ---synchronize index testaI every 5 minutes, optimize every hour
Rem        exec CTX_SCHEDULE_DOCMANAGER.startup ( 'testaI', 'SYNC', 5 ) ;
Rem        exec CTX_SCHEDULE_DOCMANAGER.startup ( 'testaI', 'OPTIMIZE FAST', 60 ) ;
Rem
Rem        ---stop synchronize and optimize
Rem        exec CTX_SCHEDULE_DOCMANAGER.stop ('testaI') ;
Rem        exec CTX_SCHEDULE_DOCMANAGER.stop ('testaI', 'OPTIMIZE FAST') ;
Rem
Rem        For a fuller discussion of usage, see README_CTX_SCHEDULE_DOCMANAGER
Rem
Rem    SEE ALSO
Rem      $ORACLE_HOME/ctx/sample/drjobdml.sql
Rem      $ORACLE_HOME/ctx/sample/drbgdml.sql
Rem
Rem    MODIFIED          (MM/DD/YY)
Rem    roger.ford         08/25/00 -  Creation
Rem    roger.ford         05/08/01 -  Increased size of 'theindex' to 63 chars
Rem                                   (was 30) needs to hold 'owner.index' including quotes
Rem

ALTER USER ctxsys IDENTIFIED BY ctxsys ACCOUNT UNLOCK;

DECLARE
vQueue INT;
BEGIN
  SELECT value INTO vQueue FROM v$parameter WHERE name = 'job_queue_processes';
  IF vQueue = 0 THEN
      EXECUTE IMMEDIATE 'ALTER SYSTEM SET job_queue_processes = 1';
  END IF;
END;
/

BEGIN
	IF &&OraVersionNumber = 9 THEN
		EXECUTE IMMEDIATE 'ALTER USER CTXSYS IDENTIFIED BY CTXSYS ACCOUNT UNLOCK'; -- default accounts locked after installation in 9i.
	END IF;
END;
/

connect ctxsys/&&ctxPass@&&serverName;

--drop table dr$job_list;

create table dr$job_list (
  idx_name	varchar2(30),
  owner         varchar2(30),
  operation	varchar2(14),
  wait_time	number,
  maxtime	number,
  job_number    number)
/

--drop view ctx_job_list;

create view ctx_job_list as 
  select c.idx_name, c.owner, c.operation, c.wait_time, c.maxtime,
  d.broken, d.failures, d.what
  from dr$job_list c, dba_jobs d
  where c.job_number = d.job
/

--drop view ctx_user_job_list;

create view ctx_user_job_list as
  select c.idx_name, c.operation, c.wait_time, c.maxtime,
  d.broken, d.failures, d.what  
  from dr$job_list c, dba_jobs d
  where c.job_number = d.job
  and c.owner = user
/


--drop public synonym ctx_user_job_list

create public synonym ctx_user_job_list
  for ctx_user_job_list
/

create or replace package CTX_SCHEDULE_DOCMANAGER is

-- Package constants:

OP_SYNC           constant char(4)   := 'SYNC';
OP_OPT_FAST       constant char(13)  := 'OPTIMIZE FAST';
OP_OPT_FULL       constant char(13)  := 'OPTIMIZE FULL';

/*
Schedule a job to SYNC or OPTIMIZE index(es) at regular intervals
*/

procedure startup (

   idx_name   varchar2 default null,
                        /* Default: NULL = All indexes. Can only be
                             called in this mode by CTXSYS */
   operation  varchar2 default OP_SYNC,
                        /* Valid: 'SYNC', 'OPTIMIZE FAST', 'OPTIMIZE FULL'
                                Default: SYNC */
   wait_time number   default 1, 
                        /* Minutes - fractions allowed */
   first_run  date     default sysdate,
                        /* First time to run */
   maxtime    number   default null
                        /* Minutes - fractions allowed - for opt. full*/
   );

/* Can raise following exceptions:
   Operation already running. Use CTX_SCHEDULE_DOCMANAGER.stop first
   Operation initiated by other user - cannot schedule
   Not owner of table and not CTXSYS
   IDX_NAME is null and USER != 'CTXSYS'
   OPERATION not in list 'SYNC', 'OPTIMIZE FAST', 'OPTIMIZE FULL'
   JOB_QUEUE_PROCESSES set to 0 - cannot schedule job
   WAIT_TIME value less than JOB_QUEUE_INTERVAL
 */

/*
Unschedule a running SYNC or OPTIMIZE job
*/

procedure stop (

   idx_name varchar2 default null,   
                        /* Default: NULL = All indexes. Can only be
                                called in this mode by CTXSYS */
   operation varchar2 default OP_SYNC
                        /* Valid: 'SYNC', 'OPTIMIZE FAST', 'OPTIMIZE FULL'
                                Default: SYNC */
   );

/* Can raise exceptions:
   IDX_NAME is null and USER != 'CTXSYS
   Operation scheduled by another user and USER != 'CTXSYS' 
   No such operation scheduled (possibly don't raise exception if
     IDX_NAME is null - means 'stop any that might be running'
 */

/*
Find out how often an index will be SYNC'd or OPTIMIZEd.
*/

function get_wait_time (

   idx_name varchar2 default null,   
                        /* Default: NULL = All indexes. Can only be
                                called in this mode by CTXSYS */
   operation varchar2 default OP_SYNC
                        /* Valid: 'SYNC', 'OPTIMIZE FAST', 'OPTIMIZE FULL'
                                Default: SYNC */
) return integer;

/* Can raise exceptions:
   Not owner of table and not CTXSYS
   IDX_NAME is null and USER != 'CTXSYS'
     No other exceptions - will return 0 if no such job scheduled.
     Will return 0 if request is for a specific index name, but job
      was originally scheduled by CTXSYS with IDX_NAME=>null.
*/

end CTX_SCHEDULE_DOCMANAGER;

/
show errors

create or replace package body CTX_SCHEDULE_DOCMANAGER is

-- Local functions

function splituserobject (
   inputstr IN varchar2, owner OUT varchar2, obj OUT varchar2)
return boolean is
   p         number;
begin
   p := instr(inputstr, '.');
   if (p = 0) then
     owner := '';
     obj := inputstr;
     return TRUE;
   else
     owner := substr(inputstr, 1, p-1);
     obj   := substr(inputstr, p+1, length(inputstr)-p);
     -- Check for any unwanted extra dots
     p := instr(obj, '.');
     if (p != 0) then
      return FALSE;
     end if;
     return TRUE;
   end if;
end splituserobject;

procedure startup (

   idx_name   varchar2 default null,   
                        /* Default: NULL = All indexes. Can only be
                             called in this mode by CTXSYS */
   -- SYAN modified on 11/1/2002
   -- Oracle sample code has it like this and was complained by SQL*Plus
   -- operation  varchar2 default 'SYNC',
   -- Now changed to:
   operation  varchar2 default OP_SYNC,
                        /* Valid: 'SYNC', 'OPTIMIZE FAST', 'OPTIMIZE FULL'
                                Default: SYNC */
   wait_time number   default 1, 
                        /* Minutes - fractions allowed */
   first_run  date     default sysdate,
                        /* First time to run */
   maxtime    number   default null
                        /* Minutes - fractions allowed - for opt. full*/
   ) is

   indexname  varchar2(30);
   ownername  varchar2(30);
   theindex   varchar2(63) := '';
   id         number;
   theop      varchar2(30);
   param      number;
   v_interval   varchar2(100);
   sync_job   number;
   v_what     varchar2(256);
   v_next     date;
   v_wait     number;
   v_first    date;
begin

   -- Check args

   -- first check for any args set explicitly to nulls and set
   --   them to their defaults.

   v_wait := v_wait;
   if v_wait is null then v_wait := 1; end if;
   v_first := first_run;
   if v_first is null then v_first := sysdate; end if;

   if (idx_name is null and user != 'CTXSYS') then
     raise_application_error(-20000, 
       'Specify an indexname - only user CTXSYS may schedule all indexes');
   end if;

   -- We've got an indexname OR indexname is null and user = CTXSYS

   if (idx_name is not null) then
     if (splituserobject( idx_name, ownername, indexname) = false) then
        raise_application_error(-20000, 
         'failed to parse index name '||upper(idx_name));
     end if;
     
     if ownername is null then 
       ownername := user;
     end if;

     ownername := upper(ownername);
     indexname := upper(indexname);

     -- check that this user owns such an index

     begin
        select idx_id into id
        from ctx_indexes
        where idx_name  = indexname
        and   idx_owner = ownername;
     exception
        when no_data_found then
           raise_application_error(-20000,
             'No such index of type context: '||ownername||'.'||indexname);
        when others then
           raise_application_error(-20000,
             'Unexpected exception raised while checking index name');
     end;

   end if;

   -- finished checking indexname
   -- check operation

   theop := upper(operation);
   if theop is null then theop := OP_SYNC; end if;

   if ( theop != OP_SYNC and theop != OP_OPT_FAST
       and theop != OP_OPT_FULL) then
     raise_application_error(-20000,
       'invalid: '||theop||' use only SYNC, OPTMIZE FAST, OPTIMIZE FULL');
   end if;

   if theop != OP_SYNC and indexname is null then
     raise_application_error(-20000,
       'must specifiy indexname with OPTIMIZE FAST or FULL');
   end if;

   if theop != OP_OPT_FULL and maxtime is not null then
     raise_application_error(-20000,
       'maxtime parameter only valid with OPTIMIZE FULL');
   end if;

   -- make sure there isn't already a job scheduled on this index

   if idx_name is null then
     select count(*) into sync_job
     from dr$job_list j
     where j.owner is null
     and j.idx_name is null
     and j.operation = theop;
   else
     select count(*) into sync_job
     from dr$job_list j
     where j.owner = ownername
     and j.idx_name = indexname
     and j.operation = theop;
   end if;

   if sync_job > 0 then
     raise_application_error(-20000,
      'operation '||theop||' already scheduled for index. Use STOP first');

   end if;

   -- check system parameters

   select value into param from v$parameter where name = 'job_queue_processes';

   if param < 1 then
     raise_application_error(-20000,
       'cannot schedule - system parameter job_queue_processes is set to 0');
   end if;

   -- SYAN modified on 11/1/2002
   -- Oracle sample code has it like this yet there is no parameter named 'job_queue_interval' in v$parameter
   -- It works fine with it commented out
   
   /*select value into param from v$parameter where name = 'job_queue_interval';

   if v_wait*60 < param then
     raise_application_error(-20000,
       'cannot schedule - v_wait ('||to_char(v_wait)||
          'm) is less than job_queue_interval ('||
          to_char(param)||'s)');
   end if;*/
   -------------


   if indexname is not null then
     theindex := ''''||ownername||'.'||indexname||'''';
   else 
     theindex := null;
   end if;

   if theop = OP_SYNC then

     v_what := 'ctxsys.ctx_ddl.sync_index('||theindex||');';

   else  -- optimize

     v_what := 'ctxsys.ctx_ddl.optimize_index('|| theindex ||', ';

     if theop = OP_OPT_FULL then
       if maxtime is not null then
         v_what := v_what || '''FULL'', ' || to_char(maxtime) || ');';
       else
         v_what := v_what || '''FULL'');';
       end if;
     else
       v_what := v_what || '''FAST'');';
     end if;

   end if;

   -- create the "next date" string using v_wait. Remember this
   -- gets evaluated EACH TIME the job is run.

   v_interval := 'sysdate + ('||v_wait||' / 24 / 60)';

   if first_run is not null then
     v_next := first_run;
   else
     v_next := sysdate;
   end if;

   begin
     dbms_job.submit(
       job       => sync_job,
       what      => v_what,
       next_date => v_next,
       interval  => v_interval
       );
   end;

   insert into dr$job_list (
     idx_name,
     owner,
     operation,
     wait_time,
     maxtime,
     job_number )
   values (
     indexname,
     ownername,
     theop,
     v_wait,
     maxtime,
     sync_job);

   commit;

end;

procedure stop (

   idx_name varchar2 default null,   
                        /* Default: NULL = All indexes. Can only be
                                called in this mode by CTXSYS */
   operation varchar2 default OP_SYNC
                        /* Valid: 'SYNC', 'OPTIMIZE FAST', 'OPTIMIZE FULL'
                                Default: SYNC */
   ) is 

 indexname  varchar2(30);
 ownername  varchar2(30);
 theop      varchar2(30);
 sync_job   number;

begin  

   -- Check args

   if (idx_name is null and user != 'CTXSYS') then
     raise_application_error(-20000, 
       'Specify an indexname - only user CTXSYS may schedule all indexes');
   end if;

   -- We've got an indexname OR indexname is null and user = CTXSYS

   if (idx_name is not null) then
     if (splituserobject( idx_name, ownername, indexname) = false) then
        raise_application_error(-20000, 
         'failed to parse index name '||upper(idx_name));
     end if;
     
     if ownername is null then 
       ownername := user;
     end if;

     ownername := upper(ownername);
     indexname := upper(indexname);

   end if;

   theop := upper(operation);
   if theop is null then theop := OP_SYNC; end if;
	
   begin
     if indexname is null then
       select job_number into sync_job
       from dr$job_list j
       where j.owner is null
       and j.idx_name is null
       and j.operation = theop;
     else
       select job_number into sync_job
       from dr$job_list j
       where j.owner = ownername
       and j.idx_name = indexname
       and j.operation = theop;
     end if;
   exception
     when no_data_found then
       raise_application_error(-20000,
         'No such operation scheduled');
     when too_many_rows then
       raise_application_error(-20000,
         'Too many rows in DR$JOB_LIST - clear it and stop jobs manually');
   end;

   if indexname is null then
     delete from dr$job_list j
       where j.owner is null
       and j.idx_name is null
       and j.operation = theop;
   else
     delete from dr$job_list j
       where j.owner = ownername
       and j.idx_name = indexname
       and j.operation = theop;
   end if;

   commit;  -- in case next stage fails

   -- if the job has been removed by some other process, the following
   -- will fail with Oracle error 23421. Hence we do this AFTER the
   -- delete from dr$job_list, else there would be no way to remove
   -- the rows from that table, other than manual intervention

   dbms_job.remove(sync_job);
 
   commit;   

end stop;

function get_wait_time (

   idx_name varchar2 default null,   
                        /* Default: NULL = All indexes. Can only be
                                called in this mode by CTXSYS */
   operation varchar2 default OP_SYNC
                        /* Valid: 'SYNC', 'OPTIMIZE FAST', 'OPTIMIZE FULL'
                                Default: SYNC */
) return integer is

 indexname  varchar2(30);
 ownername  varchar2(30);
 theop      varchar2(30);
 freq       number;

begin
   -- Check args

   theop := upper(operation);
   if theop is null then theop := OP_SYNC; end if;

   if (idx_name is null and user != 'CTXSYS') then
     raise_application_error(-20000, 
       'Specify an indexname - only user CTXSYS may schedule all indexes');
   end if;

   -- We've got an indexname OR indexname is null and user = CTXSYS

   if (idx_name is not null) then
     if (splituserobject( idx_name, ownername, indexname) = false) then
        raise_application_error(-20000, 
         'failed to parse index name '||upper(idx_name));
     end if;
     
     if ownername is null then 
       ownername := user;
     end if;

     ownername := upper(ownername);
     indexname := upper(indexname);

   end if;

   begin
     if indexname is null then
       select j.wait_time into freq
       from dr$job_list j
       where j.owner is null
       and j.idx_name is null
       and j.operation = theop;
     else
       select j.wait_time into freq
       from dr$job_list j
       where j.owner = ownername
       and j.idx_name = indexname
       and j.operation = theop;
     end if;
   exception
     when no_data_found then
       raise_application_error(-20000,
         'No such operation scheduled');
   end;

   return freq;

end get_wait_time;

end CTX_SCHEDULE_DOCMANAGER;

/
show errors

--drop public synonym CTX_SCHEDULE_DOCMANAGER;

create public synonym CTX_SCHEDULE_DOCMANAGER for CTX_SCHEDULE_DOCMANAGER;
grant execute on CTX_SCHEDULE_DOCMANAGER to public;

