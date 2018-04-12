CREATE OR REPLACE
FUNCTION InsertUpdateRecordDD (
pName phdout.name%type,
pDsgForm phdout.dsg_form%type,
pRoute phdout.route%type,
pBdpp phdout.bdpp%type,
pBrand phdout.brand%type,
pStrength phdout.strength%type,
pDSize phdout.d_size%type,
pDup phdout.dup%type,
pCost phdout.cost%type,
pInvstFlg phdout.invst_flg%type,
pPCN phdout.pcn%type,
pProt phdout.prot%type,
pSui phdout.sui%type,
pNDC phdout.NDC%type,
pOrderNo phdout.Order_No%type,
pBarcodes phdout.barcodes%type,
pFDBName phdout.FDB_Name%type,
pFDBRoute phdout.FDB_Route%type,
pFDBGCN phdout.FDB_GCN%type,
pFDBDSGForm phdout.fdb_dsg_form%type,
pFDBBrand phdout.fdb_brand%type,
pFDBStrength phdout.fdb_strength%type,
pFDBSize phdout.fdb_size%type
) 
RETURN integer
AS
 vCount Integer;
 
BEGIN
 
 select count(*) into vCount from phdout where ndc=pNDC;
 if vCount=0 then --insert
 	insert into phdout (
 	NAME      ,
DSG_FORM     ,
ROUTE        ,
BDPP         ,
BRAND       ,
STRENGTH    ,
D_SIZE      ,
DUP         ,
COST        ,
INVST_FLG   ,
PCN         ,
PROT        ,
SUI         ,
NDC         ,
ORDER_NO    ,
BARCODES    ,
FDB_NAME    ,
FDB_ROUTE   ,
FDB_GCN    ,
FDB_DSG_FORM  ,
FDB_BRAND    ,
FDB_STRENGTH,
FDB_SIZE
) values
(
pName,
pDsgForm,
pRoute,
pBdpp,
pBrand,
pStrength,
pDSize,
pDup,
pCost,
pInvstFlg,
pPCN,
pProt,
pSui,
pNDC,
pOrderNo,
pBarcodes,
pFDBName,
pFDBRoute,
pFDBGCN,
pFDBDSGForm,
pFDBBrand,
pFDBStrength,
pFDBSize
)
; 
 
 else -- update
update phdout set
    	Name=pName,
DSG_Form=pDsgForm,
Route=pRoute,
BDPP=pBdpp,
Brand=pBrand,
Strength=pStrength,
D_Size=pDSize,
Dup=pDup,
Cost=pCost,
invst_flg=pInvstFlg,
PCN=pPCN,
Prot=pProt,
Sui=pSui,
NDC=pNDC,
Order_No=pOrderNo,
Barcodes=pBarcodes,
FDB_Name=pFDBName,
FDB_Route=pFDBRoute,
FDB_GCN=pFDBGCN,
FDB_Dsg_Form=pFDBDSGForm,
FDB_Brand=pFDBBrand,
FDB_Strength=pFDBStrength,
FDB_Size=pFDBSize
    where NDC=pNDC; 
 
 end if;
 

commit;
  RETURN 0;
END;
/
