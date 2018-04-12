create or replace procedure update_compound_molecule as
begin 
-- fill formulaweight, molecularformula into vw_compound view
merge into vw_compound c using
(
select
        c.compoundid
        , nvl(c.formulaweight,cscartridge.molweight(s.structure)) formulaweight
        , nvl(c.molecularformula,cscartridge.formula(s.structure,'')) molecularformula
      from VW_Structure_Drawing s
        join vw_compound c on c.structureid = s.structureid where c.formulaweight != cscartridge.molweight(s.structure)) c1 on (c.compoundid=c1.compoundid)
when matched then update set c.formulaweight = c1.formulaweight, c.molecularformula = c1.molecularformula;
commit; 
end;
/
show errors
