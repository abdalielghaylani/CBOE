create or replace function 
&&schemaName..getStructureProperty(p_regid in integer, p_propertyName in varchar2) 
return varchar2 DETERMINISTIC  
is
  Result varchar2(2000) ;
  vSQL varchar2(2000);
  TYPE PropCurTyp  IS REF CURSOR;
  v_prop_cursor PropCurTyp;
  currProp varchar2(2000);

begin
   vSQL := 'select s.' || p_propertyName || '
            from &&regSchemaName..VW_STRUCTURE s, 
                &&regSchemaName..VW_MIXTURE_STRUCTURE ms, 
                &&regSchemaName..VW_MIXTURE m
            where s.structureid = ms.STRUCTUREID
            and m.mixtureid = ms.MIXTUREID
            and m.REGID = :1';
   
   OPEN v_prop_cursor FOR vSQL USING p_regid;
   LOOP
    FETCH v_prop_cursor INTO currProp;
    EXIT WHEN v_prop_cursor%NOTFOUND;  
    Result := Result || ',' || currProp;
   END LOOP;
   
   RETURN Substr(LTRIM(Result, ','),1,2000);
   
end ;
/
