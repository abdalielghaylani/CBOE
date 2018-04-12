-- Create procedure/function UPDATECONTAINERQTYREMAINING.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."UPDATECONTAINERQTYREMAINING"         (pContainerID in 
    inv_containers.Container_ID%Type, pQtyRemaining in 
    Inv_containers.Qty_Remaining%Type)
return inv_containers.Container_ID%Type
IS
excess_contents exception;
pragma exception_init (excess_contents, -2290);
BEGIN
  Update inv_containers set Qty_Remaining = pQtyRemaining where Container_ID = pContainerID;
  if sql%rowcount = 1 then
    Reservations.ReconcileQtyAvailable(pContainerID);
    RETURN pContainerID;
  Else
    --RETURN 'Container to update could not be found';
    RETURN -119;
  End if;
  
exception  
WHEN excess_contents then
	--RETURN 'Amount cannot exceed contianer size.';
  RETURN -103;
END UpdateContainerQtyRemaining;
/
show errors;
