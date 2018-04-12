Create or Replace Function REGDB.PeopleProject_RLL_Function(p_schema in varchar2, p_object in varchar2) return varchar
as 
begin
	if (sys_context('userenv','session_user') = 'REGDB' or sys_context('userenv','session_user') = 'SYSTEM' or sys_context('userenv','session_user') = 'CHEMINVDB2') then
		return '';
	else
		if (p_object='TEMPORARY_STRUCTURES') then
			return 'project_id IN(select project_id from people_project where person_id =(select person_id from people where user_id =''' || sys_context('userenv','session_user') || '''))';
		elsif (p_object='COMPOUND_MOLECULE') then
			return 'cpd_database_counter IN(select cpd_internal_id from compound_project where project_internal_id IN(select project_id from people_project where person_id =(select person_id from people where user_id =''' || sys_context('userenv','session_user') || ''')))';
		elsif (p_object='BATCHES') then
			return 'cpd_internal_id IN(select cpd_internal_id from compound_project where project_internal_id IN(select project_id from people_project where person_id =(select person_id from people where user_id =''' || sys_context('userenv','session_user') || ''')))';
		elsif (p_object='REG_NUMBERS') then
			return 'cpd_internal_id IN(select cpd_internal_id from compound_project where project_internal_id IN(select project_id from people_project where person_id =(select person_id from people where user_id =''' || sys_context('userenv','session_user') || ''')))';
		elsif (p_object='STRUCTURES') then
			return 'cpd_internal_id IN(select cpd_internal_id from compound_project where project_internal_id IN(select project_id from people_project where person_id =(select person_id from people where user_id =''' || sys_context('userenv','session_user') || ''')))';
		elsif (p_object='COMPOUND_SALT') then
			return 'cpd_internal_id IN(select cpd_internal_id from compound_project where project_internal_id IN(select project_id from people_project where person_id =(select person_id from people where user_id =''' || sys_context('userenv','session_user') || ''')))';
		elsif (p_object='PROJECTS') then
			return 'project_internal_id IN(select project_id from people_project where person_id =(select person_id from people where user_id =''' || sys_context('userenv','session_user') || '''))';
    elsif (p_object='INV_VW_COMPOUNDS') then
      -- 0 is for the case of inventory-owned compounds; these are not row-level secured.
      return 'cpd_internal_id in (select 0 from dual union all select cpd_internal_id from compound_project where project_internal_id IN(select project_id from people_project where person_id =(select person_id from people where user_id =''' || sys_context('userenv','session_user') || ''')))';
		else
			return '';
		end if;
	end if;
end;
/
show errors
