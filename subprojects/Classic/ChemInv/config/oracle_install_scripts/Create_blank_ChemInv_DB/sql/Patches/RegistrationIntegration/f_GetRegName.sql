create or replace FUNCTION &&schemaName..GETREGNAME(pRegID in number) Return Varchar2 DETERMINISTIC
IS
RegName varchar2(4000);
BEGIN

select nvl(nvl(trim(getregcomponentidentifier(pRegID, 'Chemical Name')),
(SELECT
     S.struct_name
  FROM
      &&regSchemaName..vw_mixture_component MXC ,  &&REGSchemaName..VW_MIXTURE MX, &&REGSchemaName..vw_compound c,
            &&REGSchemaName..vw_structure s
  WHERE
      MXC.MIXTUREID=MX.MIXTUREID
      AND mxc.compoundid = c.compoundid
      AND c.structureid = s.structureid
      and s.struct_name is not null
      and mx.regid=pRegID and rownum = 1)),(SELECT REG_NUMBER FROM &&REGSchemaName..REG_NUMBERS WHERE REG_ID = pRegID)) into RegName
      from dual;
      
RETURN RegName;
END;
/
show errors
