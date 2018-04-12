Declare
	i NUMBER;
	vUserID varchar2(100);
	vPropertyName varchar2(100);
	vPropertyValue varchar2(100);
	vPropertyValue_t &&schemaName..STRINGUTILS.t_char;
	vPropertyType_t &&schemaName..STRINGUTILS.t_char;
	vTargetString varchar2(100);		
	vSQL varchar2(500);
	CURSOR c1 IS
	select user_id_fk, propertyname, propertyvalue from INV_USER_PROPERTIES where propertyname = 'PlateChemInvFA2';
BEGIN
	OPEN c1;
	LOOP
		FETCH c1 INTO vUserID,vPropertyName,vPropertyValue;
		EXIT WHEN c1%NOTFOUND;
		VTargetString :='';
		vPropertyValue_t := &&schemaName..STRINGUTILS.split(vPropertyValue, '|');
		FOR i in vPropertyValue_t.First..vPropertyValue_t.Last
		LOOP
			vPropertyType_t := &&schemaName..STRINGUTILS.split(vPropertyValue_t(i), ':');
			if (vPropertyType_t(1) !='11' and vPropertyType_t(1)!='12' and vPropertyType_t(1)!='13') then
				if to_number(vPropertyType_t(1))>0 then
					vTargetString := vTargetString || (to_number(vPropertyType_t(1))-1) || ':' || vPropertyType_t(2)|| '|' ;
				else
					vTargetString := vTargetString || vPropertyValue_t(i) || '|' ;
				end if;
			end if;
		End Loop;
		vTargetString := rtrim(vTargetString,'|');
		vSQL:= 'update INV_USER_PROPERTIES set propertyvalue = ''' ||  VTargetString  || ''' where user_id_fk = ''' || vUserID || '''  and propertyvalue = ''' || vPropertyValue || ''' and propertyname = ''' ||  vPropertyName ||'''' ;
		execute immediate vSQL;
		commit;
	END LOOP;
  	CLOSE c1;
END;
/
show errors;