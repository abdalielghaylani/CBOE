Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

update people set supervisor_internal_id = null
where user_id IN
(select p.user_id from people p left join people b on b.person_id = p.supervisor_internal_id where b.USER_ID is null);

commit;
