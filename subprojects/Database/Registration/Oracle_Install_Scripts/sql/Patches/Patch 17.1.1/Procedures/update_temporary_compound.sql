create or replace procedure update_temporary_compound as
begin 
-- fill formulaweight, molecularformula  into vw_temporarycompound VIEW
merge into vw_temporarycompound vtc using
(select 
	tc.TEMPCOMPOUNDID
	, case
	when tc.normalizedstructure is null then
	  cscartridge.molweight(tc.base64_cdx)
	else
	  cscartridge.molweight(tc.normalizedstructure)
	end  as formulaweight
	, case
	when tc.normalizedstructure is null then
	   cscartridge.formula(tc.base64_cdx, null)
	else
	   cscartridge.formula(tc.normalizedstructure, null)
	end as molecularformula
	from vw_temporarycompound tc 
	where tc.formulaweight != 
		case
		when tc.normalizedstructure is null then
		  cscartridge.molweight(tc.base64_cdx)
		else
		  cscartridge.molweight(tc.normalizedstructure)
		end ) tc 
on (vtc.TEMPCOMPOUNDID=tc.TEMPCOMPOUNDID)
when matched then update set vtc.formulaweight = tc.formulaweight, vtc.molecularformula = tc.molecularformula;
commit; 
end;
/