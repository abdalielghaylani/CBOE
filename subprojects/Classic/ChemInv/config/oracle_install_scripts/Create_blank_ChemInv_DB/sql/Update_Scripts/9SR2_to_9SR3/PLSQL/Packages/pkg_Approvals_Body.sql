CREATE OR REPLACE
PACKAGE BODY              "APPROVALS"
AS


	PROCEDURE GETCONTAINERS(
		pCurrentLocationID IN inv_containers.location_id_fk%TYPE,
		pFromDate IN varchar2,
		pToDate IN varchar2,
		pUserID IN inv_containers.current_user_id_fk%TYPE,
		pContainerBarcode IN inv_containers.barcode%TYPE,       
		pStatusID IN inv_containers.container_status_id_fk%TYPE,
		O_RS OUT CURSOR_TYPE)  AS

    my_sql varchar2(2000);
    keyWord varchar2(3):='';
	BEGIN

	    my_sql := 'SELECT container_id, barcode, container_name, pl.user_id AS CurrentUser, container_status_name, location_name, date_certified, container_comments
					FROM inv_containers c, inv_locations, inv_container_status, &&securitySchemaName..people pl
					WHERE
						 upper(current_user_id_fk) = upper(pl.user_id)
						AND location_id_fk = location_id
						AND container_status_id_fk = container_status_id
				        AND	date_certified BETWEEN NVL(' || pFromDate || ',to_date(''1980-01-01 0:0:0'',''YYYY-MM-DD HH24:MI:SS'')) AND NVL(' || pToDate || ',SYSDATE)
  				        AND	current_user_id_fk LIKE NVL(''' || pUserID || ''', ''%'')';

			if pCurrentLocationID is NOT Null then
			   my_sql := my_sql || ' AND c.location_id_fk IN (SELECT Location_ID from inv_Locations CONNECT BY prior Location_ID = Parent_ID START WITH Location_ID =' ||  to_char(pCurrentLocationID) || ')';
			end if;
			if pStatusID is NOT Null then
			   my_sql := my_sql || ' AND c.container_status_id_fk = ' ||  to_char(pStatusID);
			end if;
		    if pContainerBarcode is NOT NULL then
	    	   my_sql := my_sql || ' AND c.barcode =''' || pContainerBarcode || '''';
		    end if;
    		--insert into inv_debug values (null,null,my_sql);
		    --my_sql := 'SELECT * FROM inv_locations WHERE location_id = 0';

		  OPEN O_RS FOR
		     my_sql;
	END GETCONTAINERS;  
	
	FUNCTION APPROVECONTAINERS(
		pContainerIDList varchar2,
		pStatusID IN inv_containers.container_status_id_fk%TYPE
		) 
	RETURN varchar2
	IS
		vNumUpdated number;	
	BEGIN
		EXECUTE IMMEDIATE
		  'UPDATE inv_containers SET
			  date_approved = sysdate,    
			  container_status_id_fk = ' || to_char(pStatusID) || '
		  WHERE container_id IN (' || pContainerIDList ||')';	

		  vNumUpdated := SQL%ROWCOUNT;
		  RETURN (vNumUpdated);
	END APPROVECONTAINERS;
	
	FUNCTION REJECTCONTAINERS(
		pContainerIDList varchar2,
		pStatusID IN inv_containers.container_status_id_fk%TYPE
		) 
	RETURN varchar2
	IS
		vNumUpdated number;	
	BEGIN
		EXECUTE IMMEDIATE
		  'UPDATE inv_containers SET
			  date_certified = null,
			  container_status_id_fk = ' || to_char(pStatusID) || '
		  WHERE container_id IN (' || pContainerIDList ||')';	

		  vNumUpdated := SQL%ROWCOUNT;
		  RETURN (vNumUpdated);
	END;
	

	FUNCTION APPROVEANDREJECTCONTAINERS(
		pApprovedContainerIDList varchar2,
		pApprovedStatusID IN inv_containers.container_status_id_fk%TYPE,
		pRejectedContainerIDList varchar2,
		pRejectedStatusID IN inv_containers.container_status_id_fk%TYPE
	)
	RETURN varchar2
	IS
		vApproveReturn varchar2(200) := '0';
		vRejectReturn varchar2(200) := '0';	
	BEGIN
		if length(pApprovedContainerIDList) > 0 then
			vApproveReturn := ApproveContainers(pApprovedContainerIDList, pApprovedStatusID);
		end if;
		if length(pRejectedContainerIDList) > 0 then
			vRejectReturn := REJECTCONTAINERS(pRejectedContainerIDList, pRejectedStatusID);
		end if;
		RETURN vApproveReturn || '|' || vRejectReturn;		
		
		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'ERROR:' || SQLCODE || ':' || SQLERRM;
		
	END APPROVEANDREJECTCONTAINERS;                 
	

END APPROVALS;
/
show errors;