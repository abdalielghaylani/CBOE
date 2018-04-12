CREATE OR REPLACE FUNCTION
&&schemaName..GETSOLVATENAME(vid in Number, BNumber in Number)
  Return varchar2 DETERMINISTIC is
  Result varchar2(2000);
  cpdID Integer;

Begin
  --This will break for mixtures. Should do nothing if >1 components


Result := '';


select min(ms.regid) INTO cpdID
from &&regSchemaName..VW_MIXTURE M, &&regSchemaName..VW_MIXTURE_structure MS
Where M.regid = vid
and Ms.mixtureid = M.mixtureid;


--CSBR 141592: Fragment information is obtained at batch level.
For i in (Select distinct F.description
            From &&regSchemaName..vw_fragment F, &&regSchemaName..vw_compound_fragment cf, &&regSchemaName..vw_compound c,
                 &&regSchemaName..vw_batchcomponentfragment bcf, &&regSchemaName..vw_batchcomponent bc, &&regSchemaName..vw_batch b
           where f.fragmentid = cf.fragmentid
             and cf.COMPOUNDID = c.compoundid
             and c.regid = cpdID
             and cf.ID = bcf.COMPOUNDFRAGMENTID
             and bcf.BATCHCOMPONENTID = bc.ID
             and bc.BATCHID = b.BATCHID
             and f.FRAGMENTTYPEID=2
             and b.BATCHNUMBER = BNumber)
Loop
   if (Result is null) OR length(Result) < 2000 then
   Result := i.description||';  '||Result;
   End if;
End Loop;

Return(Result);

End GETSOLVATENAME;

/