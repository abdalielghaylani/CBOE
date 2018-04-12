create or replace FUNCTION &&schemaName..GETSOLVATENAME(p_mixtureid in number, p_batch_internal_id in Number)
  Return varchar2 DETERMINISTIC is
  Result varchar2(2000):=' ';
 -- cpdID Integer;
Begin
--CSBR 141592: Fragment information is obtained at batch level.
select  rtrim(replace(substr(wm_concat(description||'&'),1, 2000),'&,',';'),'&') into Result 
from (
  Select distinct F.description
            From &&REGSchemaName..vw_fragment F, &&REGSchemaName..vw_compound_fragment cf, &&REGSchemaName..vw_compound c,
                 &&REGSchemaName..vw_batchcomponentfragment bcf, &&REGSchemaName..vw_batchcomponent bc
           where f.fragmentid = cf.fragmentid
             and cf.COMPOUNDID = c.compoundid
             and c.regid = (select min(c.regid)
                            from &&REGSchemaName..vw_mixture_component MXC , &&REGSchemaName..vw_compound c
                            Where MXC.mixtureid = p_mixtureid
                            AND mxc.compoundid = c.compoundid)
             and cf.ID = bcf.COMPOUNDFRAGMENTID
             and bcf.BATCHCOMPONENTID = bc.ID
             and bc.BATCHID = p_batch_internal_id
             and f.FRAGMENTTYPEID=2);
Return(Result);
End;
/
show errors