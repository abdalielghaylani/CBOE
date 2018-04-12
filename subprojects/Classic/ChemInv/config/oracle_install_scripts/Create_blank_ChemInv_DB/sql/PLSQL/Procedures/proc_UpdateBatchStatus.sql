CREATE OR REPLACE PROCEDURE "UPDATEBATCHSTATUS"(
  p_fkName inv_url.fk_name%Type,
  p_tableName inv_url.table_name%Type,
  p_urlType inv_url.url_type%Type,
  p_fkValue in inv_url.fk_value%type
)
is
  l_lnkCnt number(4);
begin

     if p_tableName = 'INV_CONTAINER_BATCHES' and p_fkName = 'BATCH_ID' then

        if p_urlType = 'Certificate of Testing' then
           update inv_container_batches set batch_status_id_fk = 1 where batch_id = p_fkValue ;
        else
           select count(*) into l_lnkCnt from inv_url where url_type='Certificate of Testing' and fk_name = 'BATCH_ID' and table_name = 'INV_CONTAINER_BATCHES' and fk_value = p_fkValue ;
               if l_lnkCnt = 0 then
                   update inv_container_batches set batch_status_id_fk = null where batch_id = p_fkValue ;
               end if ;
        end if ;

     end if ;

END "UPDATEBATCHSTATUS";
/


show errors;