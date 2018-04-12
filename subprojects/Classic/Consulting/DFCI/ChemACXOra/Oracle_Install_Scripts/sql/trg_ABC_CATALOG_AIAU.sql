CREATE OR REPLACE
TRIGGER "TRG_ABC_CATALOG_AIAU"
  after insert-- or update
  on ABC_CATALOG
  for each row
Declare
vCount integer; 
vSupplierId integer;
vSupplierName varchar(255);

begin  

select supplier.supplierid, supplier.name into vSupplierId, vSupplierName from supplier where supplierid=1;

select count(*) into vCount from phdout WHERE ndc=:new.ecndc;

if vCount>0 then

	select count(*) into vCount from SUBSTANCE WHERE CSNUM=to_number(:new.ecitemnum);

IF vCount=0 then
	insert into substance (csnum, cas, fema, mol_id, synonymid, datecreated, suppliername, acx_id, supplierid, hasproducts, hasmsds, numProducts, base64_cdx)
				values (
					to_number(:new.ecitemnum),
					:new.ecndc,
					null,
					null,
					to_number(:new.ecitemnum) + 100000000,
					sysdate,
					vSupplierName,
					:new.ecitemnum,
					vSupplierId,
					-1,
					0,
					1,
					null);
--else
  --  update substance set     			
	--				cas=:new.ecndc,
		--			fema=null,
		--			mol_id=null,
		--			synonymid=to_number(:new.ecitemnum) + 100000000,
		--			datecreated=sysdate,
		--			suppliername=vSupplierName,
		--			acx_id=:new.ecitemnum,
		--			supplierid=vSupplierId,
		--			hasproducts=-1,
		--			hasmsds=0,
		--			numproducts=1,
		--			base64_cdx=null
		--			where csnum=  to_number(:new.ecitemnum)
		--			;
end if;                                                                   
                   
end if;

end "TRG_ABC_CATALOG_AIAU";
/
