CREATE OR REPLACE
Function              "GENABCFEED"
  (pPONumber IN inv_containers.po_number%TYPE
	) 
return clob
IS

                 
vacx_id varchar(15);
vqty number;
vClobReturn CLOB:= '';
 
cursor po_feed is select acx_id, qty_remaining from inv_containers, inv_compounds where inv_containers.compound_id_fk=inv_compounds.compound_id and po_number=pPONumber and Location_id_fk= 1 ; -- 1 is on order location

BEGIN

open po_feed;
	Loop
	Fetch po_feed into vacx_id,vqty;
	Exit when po_feed%NOTFOUND;
	vClobReturn:=vClobReturn||'+AAD'||vacx_id||'+AAF'||vqty;
	end loop; 
 --Append customer # and po number	

close po_feed;
	vClobReturn:='AAA'||'DFCI#'||'+AAC'||pPONumber||'+'||vClobReturn;
	
return vClobReturn;  

END "GENABCFEED";
/
