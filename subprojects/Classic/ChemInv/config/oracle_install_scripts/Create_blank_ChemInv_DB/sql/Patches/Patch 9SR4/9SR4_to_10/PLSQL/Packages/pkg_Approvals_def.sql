CREATE OR REPLACE
PACKAGE              "APPROVALS"
AS
	TYPE  CURSOR_TYPE IS REF CURSOR;
                           
	--Returns a RS of containers to be approved
	PROCEDURE GETCONTAINERS(
		pCurrentLocationID IN inv_containers.location_id_fk%TYPE,
		pFromDate IN varchar2,
		pToDate IN varchar2,
		pUserID IN inv_containers.current_user_id_fk%TYPE,   
		pContainerBarcode IN inv_containers.barcode%TYPE,
		pStatusID IN inv_containers.container_status_id_fk%TYPE,
		O_RS OUT CURSOR_TYPE);    

	
	FUNCTION APPROVECONTAINERS(
		pContainerIDList varchar2,
		pStatusID IN inv_containers.container_status_id_fk%TYPE) 
	RETURN varchar2;
	
	FUNCTION REJECTCONTAINERS(
		pContainerIDList varchar2,
		pStatusID IN inv_containers.container_status_id_fk%TYPE) 
	RETURN varchar2;
    
	FUNCTION APPROVEANDREJECTCONTAINERS(
		pApprovedContainerIDList varchar2,
		pApprovedStatusID IN inv_containers.container_status_id_fk%TYPE,
		pRejectedContainerIDList varchar2,
		pRejectedStatusID IN inv_containers.container_status_id_fk%TYPE
	)
	RETURN varchar2;
	
END APPROVALS;
/
show errors;