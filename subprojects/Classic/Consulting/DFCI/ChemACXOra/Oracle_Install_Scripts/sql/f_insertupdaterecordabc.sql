CREATE OR REPLACE
FUNCTION InsertUpdateRecordABC (
pecitemnum abc_catalog.ecitemnum%type,
pecdesc abc_catalog.ecdesc%type,
pecui abc_catalog.ecui%type,
pecname abc_catalog.ecname%type,
pecNDC abc_catalog.ecNDC%type,
pPRPrice abc_catalog.prprice%type,
pECAWP abc_catalog.ecawp%type,
pPRCONT abc_catalog.prcont%type
) 
RETURN integer
AS
 vCount Integer;
 
BEGIN
 
 select count(*) into vCount from ABC_CATALOG where ecitemnum=pecitemnum;
 if vCount=0 then --insert
 	insert into abc_catalog (ecitemnum, ecdesc, ecui, ecname, ecndc, prprice, ecawp, prcont) values
 	( pecitemnum, pecdesc, pecui, pecname, pecndc, pprprice, pecawp, pprcont); 
 
 else -- update
    update abc_catalog set
    	ecitemnum=pecitemnum,
    	ecdesc=pecdesc, 
    	ecui=pecui, 
    	ecname=pecname,
    	ecndc=pecndc, 
    	prprice=pPrPrice, 
    	ecawp=pecawp,
    	prcont=pprcont
    where ecitemnum=pecitemnum; 
 
 end if;
 

commit;
  RETURN 0;
END;
/
