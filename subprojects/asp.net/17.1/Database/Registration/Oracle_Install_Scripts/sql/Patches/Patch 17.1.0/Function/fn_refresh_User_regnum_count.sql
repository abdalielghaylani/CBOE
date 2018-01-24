prompt 
prompt Starting "fn_refresh_User_regnum_count.sql"...
 
CREATE OR REPLACE function refresh_User_regnum_count(i_proj number:=null,i_user_id varchar2) return number as
-- it works only for i_user_id is not null. I turned off ability update count of all users and for project Due to function is very slow then
   l_user        VARCHAR2 (50);
   l_ExactRecordCount number;
   l_user_id     VARCHAR2 (50);
   l_ExemptRLS number := 0;
   l_ExemptRLS_cnt number := 0;
   l_cnt number := 0;
   l_RLS_flag number;
begin
   l_user_id := upper(i_user_id);
   SELECT SYS_CONTEXT ('userenv', 'client_identifier') INTO l_user FROM DUAL;
   DBMS_SESSION.Set_Identifier ('REGDB');
 
   if l_user_id is not null  then
     select count(1) into l_RLS_flag from dual where RegistrationRLS.GetConfigStateRLS ='T';
--     l_RLS_flag := 0;
     if l_RLS_flag = 1 then 
         l_ExemptRLS := coedb.GetExemptRLSFromPrivileges(l_user_id);
 
    -- for ExemptRLS
         if  l_ExemptRLS = 1 then
 
             select count(reg_id) into l_ExemptRLS_cnt
              from reg_numbers rn
              join sequence s on s.sequence_id= rn.sequence_internal_id
              where  s.type !=  'C';
 
            merge into regdb.User_regnum_count urc using (
            select l_user_id as user_id,l_ExemptRLS_cnt as reg_num_cnt from dual)
             urc1 on (upper(urc.User_ID) = upper(urc1.User_ID))
            when matched then update set urc.reg_num_cnt = urc1.reg_num_cnt
            when not matched then insert  (urc.user_id, urc.reg_num_cnt) values (urc1.user_id, urc1.reg_num_cnt);
         else
            merge into regdb.User_regnum_count urc using (
            select l_user_id as user_id, count(distinct regid) as reg_num_cnt from (
             with pp as (select PROJECTID,type from (		  
                select pr.project_internal_id as PROJECTID, pr.type,
                case when upper(pr.is_public) = 'T' 
                then upper(l_user_id) 
                else UserID end as USERID 
                from projects  pr
                left join VW_PeopleProject PP on pr.project_internal_id = pp.projectid 
                left join vw_People P on PP.PersonID = P.PERSONID 
                and upper(UserID)=nvl(l_user_id,1)) t where t.USERID is not null)
                -- for private and public batches projects
                 select
                  b.reg_internal_id as regid
                 from
                   pp
                  join VW_BATCH_PROJECT bp   on bp.projectid = pp.PROJECTID and upper(pp.type) in ('A','B')
                  join batches b on b.batch_internal_id=bp.batchid
                  union all
                -- for private and public registry projects
                  select
                  regid as regid
                 from
                  pp
                  join VW_RegistryNumber_Project rnp   on rnp.projectid = pp.PROJECTID and upper(pp.type) in ('A','R')
             )) urc1 on (upper(urc.User_ID) = upper(urc1.User_ID))
            when matched then update set urc.reg_num_cnt = urc1.reg_num_cnt
            when not matched then insert  (urc.user_id, urc.reg_num_cnt) values (urc1.user_id, urc1.reg_num_cnt);
         end if;
       else
       
       -- for reg_numbers without any projects
--                      select count(*) into l_cnt from
--                      (select reg_id   from reg_numbers rn
--                  join sequence s on s.sequence_id= rn.sequence_internal_id
--                  where  s.type !=  'C'
--                  minus
--                  select regid from VW_RegistryNumber_Project rnp
--                  minus
--                  select
--                      b.reg_internal_id as regid
--                     from
--                        VW_BATCH_PROJECT bp
--                      join batches b on b.batch_internal_id=bp.batchid
--                  );
        -- for reg_numbers without any projects
              select count(reg_id) into l_ExactRecordCount
              from reg_numbers rn
              join sequence s on s.sequence_id= rn.sequence_internal_id
              where  s.type !=  'C';
 
                merge into User_regnum_count urc using (
                    select l_ExactRecordCount as reg_num_cnt, l_user_id as User_ID from dual
                    ) urc1 on (upper(urc.User_ID) = upper(urc1.User_ID))
                when matched then update set urc.reg_num_cnt = urc1.reg_num_cnt
                when not matched then insert  (urc.user_id, urc.reg_num_cnt) values (urc1.user_id, urc1.reg_num_cnt);
                
       end if;
         commit;
         select reg_num_cnt into l_ExactRecordCount from User_regnum_count where upper(user_id) = l_user_id;
         
   else
	 l_ExactRecordCount := null;
   end if;
   DBMS_SESSION.Set_Identifier (l_user);
   return l_ExactRecordCount;
end;
/