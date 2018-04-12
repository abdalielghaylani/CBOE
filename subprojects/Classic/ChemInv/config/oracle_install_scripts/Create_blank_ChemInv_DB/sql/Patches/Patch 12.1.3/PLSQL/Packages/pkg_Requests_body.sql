CREATE OR REPLACE PACKAGE BODY "REQUESTS"
AS

  FUNCTION CreateRequest(
    p_containerId inv_containers.container_id%TYPE,
    p_batchId inv_requests.batch_id_fk%TYPE,
    p_userId inv_requests.User_Id_Fk%TYPE,
    p_dateRequired inv_requests.date_required%TYPE,
    p_qtyRequired inv_requests.qty_required%TYPE,
    p_deliveryLocationId inv_requests.delivery_location_id_fk%TYPE,
    p_requestComments inv_requests.request_comments%TYPE,
    p_requestTypeId inv_requests.request_type_id_fk%TYPE,
    p_requestStatusId inv_requests.request_status_id_fk%TYPE,
    p_containerTypeId inv_requests.container_type_id_fk%TYPE,
    p_numberContainers inv_requests.number_containers%TYPE,
    p_quantityList inv_requests.quantity_list%TYPE,
    p_shipToName inv_requests.ship_to_name%TYPE,
    p_expenseCenter inv_requests.expense_center%TYPE,
    p_orgUnitId inv_requests.org_unit_id_fk%TYPE,
    p_assignedUserId inv_requests.Assigned_User_Id_Fk%TYPE,
    p_field1 inv_requests.field_1%TYPE,
    p_field2 inv_requests.field_2%TYPE,
    p_field3 inv_requests.field_3%TYPE,
    p_field4 inv_requests.field_4%TYPE,
    p_field5 inv_requests.field_5%TYPE,
    p_date1 inv_requests.date_1%TYPE,
    p_date2 inv_requests.date_2%TYPE,
    p_requiredUnitId inv_requests.required_unit_id_fk%TYPE 
  ) RETURN inv_requests.request_ID%Type AS
  --' inv_requests fields not included in the parameter list: request_id, date_delivered, delivered_by_id_fk, decline_reason, qty_delivered
                l_requestId inv_requests.request_ID%Type;
    l_requestStatusID inv_requests.request_status_id_fk%type;

  BEGIN
    IF (p_requestStatusId is null) THEN
       l_requestStatusID := 1;
    ELSE
        l_requestStatusID := p_requestStatusId;
    END IF;
    INSERT INTO inv_requests (
           container_id_fk,
           batch_id_fk,
           user_id_fk,
           date_required,
           qty_required,
           delivery_location_id_fk,
           request_comments,
           request_type_id_fk,
           request_status_id_fk,
           container_type_id_fk,
           number_containers,
           quantity_list,
           ship_to_name,
           expense_center,
           org_unit_id_fk,
           assigned_user_id_fk,
           field_1,
           field_2,
           field_3,
           field_4,
           field_5,
           date_1,
           date_2,
           required_unit_id_fk)
    VALUES (
      p_containerId,
      p_batchId,
      p_userId,
      p_dateRequired,
      p_qtyRequired,
      p_deliveryLocationId,
      p_requestComments,
      p_requestTypeId,
      l_requestStatusId,
      p_containerTypeId,
      p_numberContainers,
      p_quantityList,
      p_shipToName,
      p_expenseCenter,
      p_orgUnitId,
      p_assignedUserId,
      p_field1,
      p_field2,
      p_field3,
      p_field4,
      p_field5,
      p_date1,
      p_date2,
      p_requiredUnitId
    ) RETURNING request_id INTO l_requestId;

    IF p_requestTypeId = 1 THEN
                UPDATE inv_containers SET container_status_id_fk = Constants.cRequested
                WHERE container_id = p_containerID;
    END IF;

                RETURN l_requestId;

  END CreateRequest;


	FUNCTION CREATEREQUEST_old(
		pContainerID IN inv_requests.Container_ID_FK%Type,
		pQtyRequired IN inv_requests.Qty_Required%Type,
		pDateRequired IN Date,
		pUserID inv_requests.User_ID_FK%type,
		pDeliveryLocation inv_requests.delivery_location_id_fk%type,
		pRequestComments inv_requests.request_comments%type,
    pRequestTypeID inv_requests.request_type_id_fk%type,
    pContainerTypeID inv_requests.container_type_id_fk%type,
    pNumberContainers inv_requests.number_containers%type,
    pQuantityList inv_requests.quantity_list%type,
    pShipToName inv_requests.ship_to_name%type,
    pRequestStatusID inv_requests.request_status_id_fk%type,
    pExpenseCenter inv_requests.expense_center%TYPE
    ) RETURN inv_requests.request_ID%Type AS

		RequestID inv_requests.request_ID%Type;
    vRequestStatusID inv_requests.request_status_id_fk%type;
	BEGIN
    IF (pRequestStatusID is null) THEN
       vRequestStatusID := 1;
    ELSE
        vRequestStatusID := pRequestStatusID;
    END IF;
		INSERT INTO inv_requests (
		request_id,
		container_id_fk,
		qty_required,
		date_required,
		user_id_fk,
		delivery_location_id_fk,
		request_comments,
    request_type_id_fk,
    container_type_id_fk,
    number_containers,
    quantity_list,
    ship_to_name,
    expense_center,
    request_status_id_fk)
		VALUES(
		seq_inv_requests.nextval,
		pContainerID,
		pQtyRequired,
		pDateRequired,
		pUserID,
		pDeliveryLocation,
		pRequestComments,
    pRequestTypeID,
    pContainerTypeID,
    pNumberContainers,
    pQuantityList,
    pShipToName,
    pExpenseCenter,
    vRequestStatusID
    ) RETURNING request_id into RequestID;

  IF pRequestTypeID = 1 THEN
  		UPDATE inv_containers SET container_status_id_fk = Constants.cRequested
  		WHERE container_id = pContainerID;
  END IF;

		RETURN RequestID;
	END CREATEREQUEST_old;


	FUNCTION CREATEBATCHREQUEST(
		p_BatchID IN inv_requests.Batch_ID_FK%Type,
		p_QtyRequired IN inv_requests.Qty_Required%Type,
		p_DateRequired IN Date,
		p_UserID inv_requests.User_ID_FK%type,
        p_OrgUnitID inv_org_unit.org_unit_id%type,
		p_DeliveryLocation inv_requests.delivery_location_id_fk%type,
    p_RequestTypeID inv_requests.request_type_id_fk%type,
    p_RequestStatusID inv_requests.request_status_id_fk%type,
    p_Field_1 inv_requests.field_1%type,
    p_Field_2 inv_requests.field_2%type,
    p_Field_3 inv_requests.field_3%type,
    p_Field_4 inv_requests.field_4%type,
    p_Field_5 inv_requests.field_5%type,
    p_Date_1 inv_requests.date_1%type,
    p_Date_2 inv_requests.date_2%type,
    p_requiredUnitId inv_requests.required_unit_id_fk%TYPE,
    p_shipToName inv_requests.ship_to_name%TYPE
    ) RETURN inv_requests.request_ID%Type AS
		l_RequestID inv_requests.request_ID%Type;
    l_RequestStatusID inv_requests.request_status_id_fk%type;
	BEGIN
    IF (p_RequestStatusID is null) THEN
       l_RequestStatusID := 1;
    ELSE
       l_RequestStatusID := p_RequestStatusID;
    END IF;
		INSERT INTO inv_requests (
  		request_id,
  		batch_id_fk,
  		qty_required,
  		date_required,
  		user_id_fk,
        org_unit_id_fk,
  		delivery_location_id_fk,
      request_type_id_fk,
      request_status_id_fk,
      field_1,
      field_2,
      field_3,
      field_4,
      field_5,
      date_1,
      date_2,
      required_unit_id_fk,
      ship_to_name
    )
		VALUES(
  		seq_inv_requests.nextval,
  		p_BatchID,
  		p_QtyRequired,
  		p_DateRequired,
  		p_UserID,
        p_OrgUnitID,
  		p_DeliveryLocation,
      p_RequestTypeID,
  		l_RequestStatusID,
      p_Field_1,
      p_Field_2,
      p_Field_3,
      p_Field_4,
      p_Field_5,
      p_Date_1,
      p_Date_2,
      p_requiredUnitId,
      p_shipToName
    ) RETURNING request_id into l_RequestID;
		RETURN l_RequestID;
	END CREATEBATCHREQUEST;

FUNCTION UpdateRequest(
                p_requestId inv_requests.request_id%TYPE,
    p_userId inv_requests.User_Id_Fk%TYPE,
    p_dateRequired inv_requests.date_required%TYPE,
    p_qtyRequired inv_requests.qty_required%TYPE,
    p_deliveryLocationId inv_requests.delivery_location_id_fk%TYPE,
    p_requestComments inv_requests.request_comments%TYPE,
    p_requestTypeId inv_requests.request_type_id_fk%TYPE,
    p_requestStatusId inv_requests.request_status_id_fk%TYPE,
    p_containerTypeId inv_requests.container_type_id_fk%TYPE,
    p_numberContainers inv_requests.number_containers%TYPE,
    p_quantityList inv_requests.quantity_list%TYPE,
    p_shipToName inv_requests.ship_to_name%TYPE,
    p_expenseCenter inv_requests.expense_center%TYPE,
    p_orgUnitId inv_requests.org_unit_id_fk%TYPE,
    p_assignedUserId inv_requests.Assigned_User_Id_Fk%TYPE,
    p_field1 inv_requests.field_1%TYPE,
    p_field2 inv_requests.field_2%TYPE,
    p_field3 inv_requests.field_3%TYPE,
    p_field4 inv_requests.field_4%TYPE,
    p_field5 inv_requests.field_5%TYPE,
    p_date1 inv_requests.date_1%TYPE,
    p_date2 inv_requests.date_2%TYPE,
    p_requiredUnitId inv_requests.required_unit_id_fk%TYPE 
) RETURN inv_requests.request_ID%Type AS
BEGIN
        UPDATE inv_requests SET
        user_id_fk = p_userId,
    date_required = p_dateRequired,
    qty_required = p_qtyRequired,
    delivery_location_id_fk = p_deliveryLocationId,
    request_comments = p_requestComments,
                request_type_id_fk = p_requestTypeId,
    request_status_id_fk = p_requestStatusId,
    container_type_id_fk = p_containerTypeId,
                number_containers = p_numberContainers,
    quantity_list = p_quantityList,
    ship_to_name = p_shipToName,
                org_unit_id_fk = p_orgUnitId,
    assigned_user_id_fk = p_assignedUserId,
    expense_Center = p_expenseCenter,
    field_1 = p_field1,
    field_2 = p_field2,
    field_3 = p_field3,
    field_4 = p_field4,
    field_5 = p_field5,
    date_1 = p_date1,
    date_2 = p_date2,
    required_unit_id_fk=  p_requiredUnitId
        WHERE
        request_id = p_requestId;

        RETURN p_requestId;

END UpdateRequest;

	FUNCTION UPDATEREQUEST_old(
		pRequestID IN inv_requests.request_ID%Type,
		pQtyRequired IN inv_requests.Qty_Required%Type,
		pDateRequired IN Date,
		pUserID inv_requests.User_ID_FK%type,
		pDeliveryLocation inv_requests.delivery_location_id_fk%type,
		pRequestComments inv_requests.request_comments%type,
    pContainerTypeID inv_requests.container_type_id_fk%type,
    pNumberContainers inv_requests.number_containers%type,
    pQuantityList inv_requests.quantity_list%type,
    pShipToName inv_requests.ship_to_name%TYPE,
    pExpenseCenter inv_requests.expense_center%TYPE
    ) RETURN inv_requests.request_ID%Type AS

	BEGIN
		UPDATE inv_requests SET
		qty_required = pQtyRequired,
		date_required = pDateRequired,
		user_id_fk = pUserID,
		delivery_location_id_fk = pDeliveryLocation,
		request_comments = pRequestComments,
    container_type_id_fk = pContainerTypeID,
    number_containers = pNumberContainers,
    quantity_list = pQuantityList,
    ship_to_name = pShipToName,
    expense_center = pExpenseCenter
		WHERE request_id = pRequestID;
		RETURN pRequestID;
	END UPDATEREQUEST_old;

 	FUNCTION UPDATEBATCHREQUEST(
  	p_RequestID IN inv_requests.request_id%TYPE,
		p_BatchID IN inv_requests.Batch_ID_FK%Type,
		p_QtyRequired IN inv_requests.Qty_Required%Type,
		p_DateRequired IN Date,
		p_UserID inv_requests.User_ID_FK%type,
        p_OrgUnitID inv_org_unit.org_unit_id%type,
		p_DeliveryLocation inv_requests.delivery_location_id_fk%type,
		 p_requestComments inv_requests.request_comments%TYPE,	--Added for CSBR 121371
    p_RequestTypeID inv_requests.request_type_id_fk%type,
    p_RequestStatusID inv_requests.request_status_id_fk%type,
    p_Field_1 inv_requests.field_1%type,
    p_Field_2 inv_requests.field_2%type,
    p_Field_3 inv_requests.field_3%type,
    p_Field_4 inv_requests.field_4%type,
    p_Field_5 inv_requests.field_5%type,
    p_Date_1 inv_requests.date_1%type,
    p_Date_2 inv_requests.date_2%type,
    p_requiredUnitId inv_requests.required_unit_id_fk%TYPE,
    p_shipToName inv_requests.ship_to_name%TYPE
    ) RETURN inv_requests.request_ID%Type
  AS
    l_RequestStatusID inv_requests.request_status_id_fk%type;
	BEGIN
    IF (p_RequestStatusID is null) THEN
      SELECT Request_Status_ID_FK INTO l_RequestStatusID
          FROM inv_requests
          WHERE request_id = p_RequestID;
    ELSE
       l_RequestStatusID := p_RequestStatusID;
    END IF;
		UPDATE inv_requests SET
			batch_id_fk = p_BatchID,
  		qty_required = p_QtyRequired,
  		date_required = p_DateRequired,
  		user_id_fk = p_UserID,
        org_unit_id_fk = p_OrgUnitID,
  		delivery_location_id_fk = p_DeliveryLocation,
		request_comments = p_requestComments,    --Added for CSBR 121371
      request_type_id_fk = p_RequestTypeID,
      request_status_id_fk = l_RequestStatusID,
      field_1 = p_Field_1,
      field_2 = p_Field_2,
      field_3 = p_Field_3,
      field_4 = p_Field_4,
      field_5 = p_Field_5,
      date_1 = p_Date_1,
      date_2 = p_Date_2,
      required_unit_id_fk=  p_requiredUnitId,
      ship_to_name =  p_shipToName
    WHERE
			request_id = p_RequestID;
		RETURN p_RequestID;
	END UPDATEBATCHREQUEST;
	FUNCTION DELIVERREQUESTS(
		pRequestIDList IN varchar2
	  ) RETURN integer AS

		DestinationLocation inv_requests.delivery_location_id_fk%type;
		CurrentUserID inv_requests.User_ID_FK%type;
		ContainerID inv_requests.Container_ID_FK%type;
	  rowCount integer;
	  O_RS CURSOR_TYPE;
	BEGIN

		EXECUTE IMMEDIATE
		  'UPDATE inv_requests SET
		  date_delivered = sysdate,
		  delivered_by_id_fk = user
		  WHERE request_id IN ('||pRequestIDList ||')';
    OPEN O_RS FOR
		    'SELECT delivery_location_id_fk, user_id_fk, container_id_fk
		     FROM inv_requests
		     WHERE request_id IN (' ||  pRequestIDList || ')';


        LOOP
		      FETCH O_RS INTO DestinationLocation, CurrentUserID, ContainerID;
		      EXIT WHEN O_RS%NOTFOUND;
		      rowCount := O_RS%ROWCOUNT;

		      UPDATE inv_containers SET
		      location_id_fk = DestinationLocation,
		      current_user_id_fk = CurrentUserID,
		      container_status_id_fk = 1
		      WHERE container_id = ContainerID;
        END LOOP;

     CLOSE O_RS;
		RETURN rowCount;
	END DELIVERREQUESTS;
	FUNCTION DELIVERBATCHREQUESTS(
		p_RequestIDList IN varchar2,
		p_ContainerIDList IN varchar2,
    p_RetireContainer IN integer,
    p_QtyDelivered IN inv_requests.qty_delivered%type,
    p_Field_1 inv_requests.field_1%type,
    p_Field_2 inv_requests.field_2%type,
    p_Field_3 inv_requests.field_3%type,
    p_Field_4 inv_requests.field_4%type,
    p_Field_5 inv_requests.field_5%type,
    p_Date_1 inv_requests.date_1%type,
    p_Date_2 inv_requests.date_2%type
	  ) RETURN integer AS
		DestinationLocation inv_requests.delivery_location_id_fk%type;
		CurrentUserID inv_requests.User_ID_FK%type;
		ContainerID inv_requests.Container_ID_FK%type;
		BatchID inv_requests.Batch_ID_FK%type;
		QtyRequired inv_requests.Qty_Required%type;
		QtyAvailable inv_containers.Qty_Available%type;
    QtyUnFilfilled integer;
    NewQty integer;
    RequestID integer;
    ContainerStatus integer;
    RequestStatus integer;
    FinalLocation integer;
    l_DateDelivered varchar(30);
    l_Sql varchar(30000);         --CSBR 144631 SJ Increasing the size of the variable to accomodate the update query. 
	  rowCount integer;
	  O_RS CURSOR_TYPE;
	BEGIN
    QtyUnFilfilled := 1;
    if p_RetireContainer = 1 then
      RequestStatus := Constants.cClosedRequest; -- Closed(6)
      l_DateDelivered := 'sysdate';
    else
      RequestStatus := Constants.cFilledRequest; -- Filled(5)
      l_DateDelivered := 'null';
    end if;
    l_Sql := l_Sql || 'UPDATE inv_requests SET date_delivered = ' || l_DateDelivered  ||' , qty_delivered = ' || p_QtyDelivered || ', delivered_by_id_fk = user,';
    if p_Field_1 is not null then
       l_Sql := l_Sql || 'field_1 = ''' || p_Field_1 || ''', ';
    end if;
    if p_Field_2 is not null then
       l_Sql := l_Sql || 'field_2 = ''' || p_Field_2 || ''', ';
    end if;
    if p_Field_3 is not null then
       l_Sql := l_Sql || 'field_3 = ''' || p_Field_3 || ''', ';
    end if;
    if p_Field_4 is not null then
       l_Sql := l_Sql || 'field_4 = ''' || p_Field_4 || ''', ';
    end if;
    if p_Field_5 is not null then
       l_Sql := l_Sql || 'field_5 = ''' || p_Field_5 || ''', ';
    end if;
    if p_Date_1 is not null then
       l_Sql := l_Sql || 'date_1 = ''' || p_Date_1 || ''', ';
    end if;
    if p_Date_2 is not null then
       l_Sql := l_Sql || 'date_2 = ''' || p_Date_2 || ''', ';
    end if;
    l_Sql := l_Sql || 'request_status_id_fk = ' || RequestStatus || ' WHERE request_id IN (' || p_RequestIDList ||')';
		EXECUTE IMMEDIATE l_Sql;
    OPEN O_RS FOR
        'SELECT delivery_location_id_fk, inv_requests.request_id, user_id_fk, inv_containers.container_id, inv_requests.batch_id_fk, qty_required, qty_available
        FROM inv_requests, inv_containers
        WHERE inv_requests.batch_id_fk = inv_containers.batch_id_fk(+)
        AND request_id IN (' ||  p_RequestIDList || ') And inv_containers.container_id in (' || p_ContainerIDList || ')';
        LOOP
		      FETCH O_RS INTO DestinationLocation, RequestID, CurrentUserID, ContainerID, BatchID, QtyRequired, QtyAvailable;
		      EXIT WHEN O_RS%NOTFOUND;
		      rowCount := O_RS%ROWCOUNT;
          if QtyUnFilfilled > 0 then
            if QtyAvailable > QtyRequired then
               NewQty := QtyAvailable - QtyRequired;
               QtyUnFilfilled := 0;
            end if;
            if QtyAvailable = QtyRequired then
               NewQty := 0;
               QtyUnFilfilled := 0;
            end if;
            if QtyAvailable < QtyRequired then
               NewQty := 0;
               QtyUnFilfilled := QtyRequired - QtyAvailable;
            end if;
          else
            NewQty := QtyAvailable;
            QtyUnFilfilled := 0;
          end if;
          if p_RetireContainer = 1 then
            ContainerStatus := Constants.cDisposed;
            FinalLocation := Constants.cDisposedLoc;
            NewQty := 0;
          else
            ContainerStatus := Constants.cAvailable;
            FinalLocation := DestinationLocation;
          end if;
		      UPDATE inv_containers SET
		      location_id_fk = FinalLocation,
		      current_user_id_fk = CurrentUserID,
		      qty_available = NewQty,
		      qty_remaining = NewQty,
		      container_status_id_fk = ContainerStatus
		      WHERE container_id = ContainerID;
		      
		      INSERT into inv_request_samples(
				request_id_fk, container_id_fk )
			  VALUES( RequestID, ContainerID );
		      
        END LOOP;
     CLOSE O_RS;
		RETURN rowCount;
	END DELIVERBATCHREQUESTS;

  FUNCTION UNDODELIVERY(
		pRequestID IN inv_requests.request_id%type
	  ) RETURN integer AS
    NewLocationID inv_requests.delivery_location_id_fk%type;
		NewUserID inv_requests.User_ID_FK%type;
		OldLocationID inv_requests.delivery_location_id_fk%type;
		OldUserID inv_requests.User_ID_FK%type;
		ContainerID inv_requests.Container_ID_FK%type;
		CURSOR get_old_value_cur(NewValue_in in varchar2, RequestID_in in integer, TableName_in in varchar2) IS
		  SELECT ac.old_value
      FROM inv_containers c, inv_requests r, audit_column ac , audit_row ar
      WHERE ac.raid = ar.raid
      AND c.rid = ar.rid
      AND c.container_id = r.container_id_fk
      AND ac.new_value = NewValue_in
      AND r.request_id = RequestID_in
      AND ac.column_name = TableName_in
      ORDER BY ac.CAID DESC;
	BEGIN

	  SELECT delivery_location_id_fk, user_id_fk, container_id_fk
		INTO NewLocationID, NewUserID, ContainerID
		FROM inv_requests
		WHERE request_id = pRequestID;

		-- Get oldLocationID from audit trail
		OPEN get_old_value_cur(NewLocationID, pRequestID, 'LOCATION_ID_FK');
		FETCH get_old_value_cur INTO OldLocationID;
		CLOSE get_old_value_cur;
    -- handle the case where the OldLocationID=NewLocationID
    IF OldLocationID IS NULL THEN
    	OldLocationID := NewLocationID;
    END IF;

		-- Get oldUserID from audit trail
		OPEN get_old_value_cur(NewUserID, pRequestID, 'CURRENT_USER_ID_FK');
		FETCH get_old_value_cur INTO OldUserID;
		CLOSE get_old_value_cur;

	  UPDATE inv_requests SET
	  date_delivered = NULL,
		delivered_by_id_fk = NULL
		WHERE request_id = pRequestID;



		IF OldUserID IS NULL THEN
  		UPDATE inv_containers SET
  		location_id_fk = OldLocationID,
  		container_status_id_fk = 5
  		WHERE container_id = ContainerID;
		ELSE
  		UPDATE inv_containers SET
  		location_id_fk = OldLocationID,
  		current_user_id_fk = OldUserID,
  		container_status_id_fk = 5
  		WHERE container_id = ContainerID;

		END IF;

		RETURN 1;
	END UNDODELIVERY;

	FUNCTION DELETEREQUEST(
		pRequestID IN inv_requests.request_ID%Type) RETURN inv_requests.request_ID%Type AS

	BEGIN
		UPDATE inv_containers SET container_status_id_fk = 1
		WHERE container_id = (SELECT container_id_fk FROM inv_requests WHERE request_id = pRequestID);

		DELETE FROM inv_requests WHERE request_id = pRequestID;
		RETURN pRequestID;
	END DELETEREQUEST;

	FUNCTION CANCELREQUEST(
		pRequestID IN inv_requests.request_ID%Type,
		pCancelReason IN inv_requests.decline_reason%Type)
  RETURN inv_requests.request_ID%Type AS
	BEGIN
		UPDATE inv_requests SET request_status_id_fk = 7, decline_reason = pCancelReason WHERE request_id = pRequestID;
		RETURN pRequestID;
	END CANCELREQUEST;

	FUNCTION UPDATEASSIGNEDTOUSER(
		pRequestID IN inv_requests.request_ID%Type,
		pAssignedToUser IN inv_requests.assigned_user_id_fk%Type)
  RETURN inv_requests.request_ID%Type AS
	BEGIN
		UPDATE inv_requests SET assigned_user_id_fk = pAssignedToUser WHERE request_id = pRequestID;
		RETURN pRequestID;
	END UPDATEASSIGNEDTOUSER;
	PROCEDURE GETBATCHCONTAINERS(
		pBatchID IN inv_requests.batch_id_fk%Type,
		pRequestID IN inv_requests.request_id%Type,
		O_RS OUT CURSOR_TYPE) AS
	BEGIN
		OPEN O_RS FOR
          Select distinct
            cb.batch_id
            , c.barcode
            , c.container_id
            , c.location_id_fk
            , c.qty_remaining
            , uom.unit_abreviation AS uomAbbrv
            , concentration
            , uoc.unit_abreviation AS uocAbbrv
            , l.location_name
            , GUIUTILS.GETLOCATIONPATH(c.location_id_fk) AS Location_Path
            , GUIUTILS.GETRACKLOCATIONPATH(c.location_id_fk) || gl.name AS Rack_Path
            , gl.parent_id as RackID
            , lp.collapse_child_nodes as IsRack
            , cb.batch_status_id_fk as BatchStatus
            , ic.reg_id_fk
            , ic.batch_number_fk
        -- Added for CSBR-123488
		    , r.field_1
		    , r.field_2
		    , r.field_3
		    , r.field_4
		    , r.field_5
		    , r.date_1
		    , r.date_2
        -- Addition ends for CSBR-123488
          From inv_container_batches cb
               , inv_containers c
			   , inv_compounds ic
               , inv_units uom
               , inv_units uoc
               , inv_locations l
               , inv_vw_grid_location gl
               , inv_locations lp
               , inv_requests r
          Where cb.batch_id = c.batch_id_fk(+)
			and c.compound_id_fk = ic.compound_id(+)
          And c.unit_of_conc_id_fk = uoc.unit_id(+)
          And c.unit_of_meas_id_fk = uom.unit_id
          And c.location_id_fk not in (Constants.cDisposedLoc, Constants.cTrashCanLoc)
          And c.location_id_fk = l.location_id
          And c.location_id_fk = gl.location_id(+)
          and gl.parent_id = lp.location_id(+)
	      AND c.UNIT_OF_MEAS_ID_FK = r.required_unit_id_fk
          AND (r.batch_id_fk = c.batch_id_fk)
          And c.container_status_id_fk not in (6,7)
          And cb.batch_id = pBatchID
          And r.request_id=pRequestID
          Order by l.location_name;
	END GETBATCHCONTAINERS;

	PROCEDURE GETREQUEST(
                pRequestID IN inv_requests.request_ID%Type,
    pDateFormat IN VARCHAR2,
                O_RS OUT CURSOR_TYPE) AS

        BEGIN
                OPEN O_RS FOR
      SELECT    request_id,
                                                        r.request_status_id_fk,
      c.container_id,
      r.batch_id_fk,
      c.location_id_fk,
      c.barcode,
      r.user_id_fk,
      r.org_unit_id_fk,
      r.delivered_by_id_fk,
      p1.user_id AS RUserID,
      p2.user_id AS DUserID,
                        to_char(trunc(r.timestamp), pDateFormat) as TIMESTAMP,
                        to_char(trunc(r.date_required), pDateFormat) as date_required,
                        to_char(trunc(r.date_delivered), pDateFormat) as date_delivered,
      l.location_name,
      r.delivery_location_id_fk,
      r.qty_required,
      r.request_comments,
      r.request_type_id_fk,
      c.container_type_id_fk,
      (select count(*) from inv_containers where batch_id_fk = r.batch_id_fk) as number_containers,
      case when (Select count(c1.Qty_Available) From CHEMINVDB2.inv_containers c1 Where c1.batch_id_fk = r.batch_id_fk AND c1.container_status_id_fk not in (6,7)) = 0 then 0 else (Select Sum(c2.Qty_Available*c2.concentration) From CHEMINVDB2.inv_containers c2 Where c2.batch_id_fk = r.batch_id_fk AND c2.container_status_id_fk not in (6,7)) end as AmountRemaining,
      (select sum(r1.qty_required) from inv_requests r1 where r1.batch_id_fk=c.batch_id_fk and r1.request_status_id_fk=9) as AmountReserved,
      r.number_containers,
      r.quantity_list,
      r.ship_to_name,
      r.expense_center,
      ct.container_type_name,
      u.unit_abreviation,
      c.unit_of_meas_id_fk,
      r.creator,
      c.family,
      r.field_1,
      r.field_2,
      r.field_3,
      r.field_4,
      r.field_5,
      r.date_1,
      r.date_2,
      --r.required_unit_id_fk
      um.unit_id||'='||um.Unit_Abreviation AS unitstring
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_containers c, inv_units u, inv_units um
      WHERE     r.user_id_fk = p1.user_id(+)
      AND       Upper(r.delivered_by_id_fk) = p2.user_id(+)
      AND (r.container_id_fk = c.container_id or r.batch_id_fk = c.batch_id_fk)
      AND c.unit_of_meas_id_fk = u.unit_id(+)
      AND c.location_id_fk not in (Constants.cDisposedLoc, Constants.cTrashCanLoc)
      AND r.container_type_id_fk = ct.container_type_id (+)
      AND       r.delivery_location_id_fk = l.location_id
                        AND     r.request_id = pRequestID
      AND r.required_unit_id_fk = um.unit_id(+);
      END GETREQUEST;

	PROCEDURE GETBATCHREQUEST(
		pRequestID IN inv_requests.request_ID%Type,
    pDateFormat IN VARCHAR2,
		O_RS OUT CURSOR_TYPE) AS
	BEGIN
		OPEN O_RS FOR
      SELECT	request_id,
      c.container_id,
      r.batch_id_fk,
      c.location_id_fk,
      r.user_id_fk,
      r.org_unit_id_fk,
      r.delivered_by_id_fk,
      p1.user_id AS RUserID,
      p2.user_id AS DUserID,
			to_char(trunc(r.timestamp), pDateFormat) as TIMESTAMP,
			to_char(trunc(r.date_required), pDateFormat) as date_required,
			to_char(trunc(r.date_delivered), pDateFormat) as date_delivered,
      l.location_name,
      r.delivery_location_id_fk,
      r.qty_required,
      r.request_comments,
      r.request_type_id_fk,
      c.container_type_id_fk,
      (select count(*) from inv_containers where batch_id_fk = r.batch_id_fk) as number_containers,
      case when (Select count(c1.Qty_Available) From CHEMINVDB2.inv_containers c1 Where c1.batch_id_fk = r.batch_id_fk AND c1.container_status_id_fk not in (6,7)) = 0 then 0 else (Select Sum(c2.Qty_Available*c2.concentration) From CHEMINVDB2.inv_containers c2 Where c2.batch_id_fk = r.batch_id_fk AND c2.container_status_id_fk not in (6,7)) end as AmountRemaining,
      (select sum(r1.qty_required) from inv_requests r1 where r1.batch_id_fk=c.batch_id_fk and r1.request_status_id_fk=9) as AmountReserved,
      --r.number_containers,
      r.quantity_list,
      r.ship_to_name,
      r.expense_center,
      ct.container_type_name,
      u.unit_abreviation,
      c.unit_of_meas_id_fk,
      r.creator,
      c.family,
      r.field_1,
      r.field_2,
      r.field_3,
      r.field_4,
      r.field_5,
      r.date_1,
      r.date_2,
       um.unit_id||'='||um.Unit_Abreviation AS unitstring
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_containers c, inv_units u, inv_units um
      WHERE	r.user_id_fk = p1.user_id(+)
      AND r.creator = p2.user_id(+) /* changed for fixing bug 77167*/
      AND (r.container_id_fk = c.container_id or r.batch_id_fk = c.batch_id_fk)
      AND c.unit_of_meas_id_fk = u.unit_id(+)
      AND c.location_id_fk not in (Constants.cDisposedLoc, Constants.cTrashCanLoc)
      AND r.container_type_id_fk = ct.container_type_id (+)
      AND	r.delivery_location_id_fk = l.location_id
			AND	r.request_id = pRequestID
                         AND r.required_unit_id_fk = um.unit_id(+);
	END GETBATCHREQUEST;

  FUNCTION GETNUMSHIPPEDCONTAINERS(
		pRequestID IN inv_requests.request_ID%TYPE)
	RETURN INTEGER
  AS
  	lNumShipped INTEGER;
  BEGIN

    SELECT COUNT(oc.container_id_fk) INTO lNumShipped
    FROM inv_orders o, inv_order_containers oc
    WHERE
    	order_id = order_id_fk
      AND oc.container_id_fk IN (SELECT container_id_fk FROM inv_request_samples WHERE request_id_fk = pRequestID) AND o.order_status_id_fk IN (2,3);

		RETURN lNumShipped;
	END;

	PROCEDURE GETREQUEST2(
		pContainerID IN inv_containers.container_id%Type,
  pRequestTypeID IN inv_request_types.request_type_id%Type,
		pUserID IN inv_requests.User_ID_FK%type,
    pDateFormat IN VARCHAR2,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			SELECT	request_id,
					container_id_fk,
					r.user_id_fk,
					r.delivered_by_id_fk,
					p1.user_id AS RUserID,
					p2.user_id AS DUserID,
					to_char(trunc(r.timestamp), pDateFormat) as TIMESTAMP,
					to_char(trunc(r.date_required), pDateFormat) as date_required,
					to_char(trunc(r.date_delivered), pDateFormat) as date_delivered,
					l.location_name,
					r.delivery_location_id_fk,
					r.qty_required,
					r.request_comments,
          r.request_type_id_fk,
          r.container_type_id_fk,
          r.number_containers,
          r.quantity_list,
          r.ship_to_name,
          ct.container_type_name,
          rs.request_status_name,
          r.creator,
          GETNUMSHIPPEDCONTAINERS(r.request_id) AS NumShippedContainers,
          guiutils.GETLOCATIONPATH(l.location_id) AS LocationPath
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_request_status rs
			WHERE	r.user_id_fk = p1.user_id
      AND 	r.request_status_id_fk = rs.request_status_id
			AND		Upper(r.delivered_by_id_fk) = p2.user_id(+)
			AND		r.delivery_location_id_fk = l.location_id
      AND   r.container_type_id_fk = ct.container_type_id(+)
      AND   r.request_type_id_fk = pRequestTypeID
			AND		r.container_id_fk = pContainerID
			AND		(r.user_id_fk LIKE NVL(pUserID, '%') or r.creator LIKE NVL(pUserID, '%'));
					--(SELECT COUNT(oc.container_id_fk) FROM inv_orders o, inv_order_containers oc WHERE order_id = order_id_fk AND oc.container_id_fk IN (SELECT container_id_fk FROM inv_request_samples WHERE request_id_fk = r.request_id) AND o.order_status_id_fk IN (2,3)) AS NumShippedContainers,

	END GETREQUEST2;
PROCEDURE GETBATCHREQUEST2(
    p_RequestTypeID IN inv_request_types.request_type_id%Type,
	p_UserID IN inv_requests.User_ID_FK%type,
    p_DateFormat IN VARCHAR2,
				p_regServer VARCHAR2,
	O_RS OUT CURSOR_TYPE) AS
	BEGIN

IF p_regServer = 'NULL' THEN
		OPEN O_RS FOR
      SELECT	request_id,
				container_id_fk,
        batch_id_fk,
      	container_id_fk,
      	r.user_id_fk,
      	r.delivered_by_id_fk,
      	p1.user_id AS RUserID,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = p1.user_id) as RequestedForName,
      	p2.user_id AS DUserID,
      	to_char(trunc(r.timestamp), p_DateFormat) as TIMESTAMP,
      	to_char(trunc(r.date_required), p_DateFormat) as date_required,
      	to_char(trunc(r.date_delivered), p_DateFormat) as date_delivered,
      	l.location_name,
      	r.delivery_location_id_fk,
      	r.qty_required,
      	r.request_comments,
        r.request_type_id_fk,
        r.container_type_id_fk,
        r.number_containers,
        r.quantity_list,
        r.ship_to_name,
        ct.container_type_name,
        rs.request_status_name,
        r.creator,
        r.decline_reason,
        icb.batch_field_1 as batch_field_1,
        icb.batch_field_2 as batch_field_2,
        icb.batch_field_3 as batch_field_3,
        replace(CHEMINVDB2.GUIUTILS.GETBATCHUOMAMOUNTSTRING(NULL,r.batch_id_fk),':','') as Batch_Amount,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = r.creator) as CreatorName,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=1) as RequestReceiptDocID,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=2) as RequestWorksheetDocID,
        (select unit_abreviation FROM inv_units u where r.required_unit_id_fk = u.unit_id(+) And rownum = 1) as RequiredUOMabbrv,
        GETNUMSHIPPEDCONTAINERS(r.request_id) AS NumShippedContainers,
        GETLOCATIONPATH(l.location_id) AS LocationPath,
        -- Added for CSBR-121935
        r.FIELD_1,
        r.FIELD_2,
        r.FIELD_3,
        r.FIELD_4,
        r.FIELD_5,
        r.DATE_1,
        r.DATE_2
        -- Addition ends for CSBR-121935		
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_request_status rs, inv_container_batches icb
      WHERE	r.user_id_fk = p1.user_id(+)
      AND   r.batch_id_fk = icb.batch_id(+)
      AND 	r.request_status_id_fk = rs.request_status_id
      AND	Upper(r.delivered_by_id_fk) = p2.user_id(+)
      AND	r.delivery_location_id_fk = l.location_id
      AND   r.container_type_id_fk = ct.container_type_id(+)
      AND   r.request_type_id_fk = p_RequestTypeID
      AND	((r.user_id_fk LIKE NVL(p_UserID, '%') or r.creator LIKE NVL(p_UserID, '%') or p_UserID in (select USER_ID_FK from INV_ORG_USERS where org_unit_id_fk=r.org_unit_id_fk)))
      AND   request_status_id_fk not in (9)
      Order by request_id asc, request_status_id_fk asc, timestamp desc; --CSBR ID : 139040 SJ Ordering by request_id
ELSE
		OPEN O_RS FOR
      SELECT distinct request_id,
				container_id_fk,
        r.batch_id_fk,
      	container_id_fk,
      	r.user_id_fk,
      	r.delivered_by_id_fk,
      	p1.user_id AS RUserID,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = p1.user_id) as RequestedForName,
      	p2.user_id AS DUserID,
      	to_char(trunc(r.timestamp), p_DateFormat) as TIMESTAMP,
      	to_char(trunc(r.date_required), p_DateFormat) as date_required,
      	to_char(trunc(r.date_delivered), p_DateFormat) as date_delivered,
      	l.location_name,
      	r.delivery_location_id_fk,
      	r.qty_required,
      	r.request_comments,
        r.request_type_id_fk,
        r.container_type_id_fk,
        r.number_containers,
        r.quantity_list,
        r.ship_to_name,
        ct.container_type_name,
        rs.request_status_name,
        r.creator,
        r.decline_reason,
        request_status_id_fk,
        icb.batch_field_1 as batch_field_1,
        icb.batch_field_2 as batch_field_2,
        icb.batch_field_3 as batch_field_3,
        replace(CHEMINVDB2.GUIUTILS.GETBATCHUOMAMOUNTSTRING(NULL,r.batch_id_fk),':','') as Batch_Amount,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = r.creator) as CreatorName,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=1) as RequestReceiptDocID,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=2) as RequestWorksheetDocID,
        (select unit_abreviation FROM inv_units u where r.required_unit_id_fk = u.unit_id(+) And rownum = 1) as RequiredUOMabbrv,
        GETNUMSHIPPEDCONTAINERS(r.request_id) AS NumShippedContainers,
        GETLOCATIONPATH(l.location_id) AS LocationPath,
        -- Added for CSBR-121935
        r.FIELD_1,
        r.FIELD_2,
        r.FIELD_3,
        r.FIELD_4,
        r.FIELD_5,
        r.DATE_1,
        r.DATE_2,
        -- Addition ends for CSBR-121935
        ivrb.*
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_request_status rs, inv_container_batches icb, inv_vw_reg_batches ivrb,inv_containers c, inv_compounds cmpd
      WHERE	r.user_id_fk = p1.user_id(+)
      AND   r.batch_id_fk = icb.batch_id(+)
						AND icb.batch_id = c.batch_id_fk
      AND	c.compound_id_fk = cmpd.compound_id(+)
	  AND   cmpd.reg_id_fk = ivrb.RegID(+)
      AND   cmpd.batch_number_fk = ivrb.BatchNumber(+)
      AND 	r.request_status_id_fk = rs.request_status_id
      AND	Upper(r.delivered_by_id_fk) = p2.user_id(+)
      AND	r.delivery_location_id_fk = l.location_id
      AND   r.container_type_id_fk = ct.container_type_id(+)
      AND   r.request_type_id_fk = p_RequestTypeID
      AND	((r.user_id_fk LIKE NVL(p_UserID, '%') or r.creator LIKE NVL(p_UserID, '%') or p_UserID in (select USER_ID_FK from INV_ORG_USERS where org_unit_id_fk=r.org_unit_id_fk)))
      AND   request_status_id_fk not in (9)
      Order by request_id asc,request_status_id_fk asc, timestamp desc; --CSBR ID : 139040 SJ Ordering by Request_id

END IF;

	END GETBATCHREQUEST2;

PROCEDURE GetRequestByBatch(
	p_batchId inv_requests.batch_id_fk%TYPE,
  p_RequestTypeID IN inv_request_types.request_type_id%Type,
	p_UserID IN inv_requests.User_ID_FK%type,
  p_DateFormat IN VARCHAR2,
	p_regServer VARCHAR2,
	O_RS OUT CURSOR_TYPE) AS
	BEGIN

IF p_regServer = 'NULL' THEN
		OPEN O_RS FOR
      SELECT	request_id,
				container_id_fk,
        batch_id_fk,
      	container_id_fk,
      	r.user_id_fk,
      	r.delivered_by_id_fk,
      	p1.user_id AS RUserID,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = p1.user_id) as RequestedForName,
      	p2.user_id AS DUserID,
      	to_char(trunc(r.timestamp), p_DateFormat) as TIMESTAMP,
      	to_char(trunc(r.date_required), p_DateFormat) as date_required,
      	to_char(trunc(r.date_delivered), p_DateFormat) as date_delivered,
      	l.location_name,
      	r.delivery_location_id_fk,
      	r.qty_required,
      	r.request_comments,
        r.request_type_id_fk,
        r.container_type_id_fk,
        r.number_containers,
        r.quantity_list,
        r.ship_to_name,
        ct.container_type_name,
        rs.request_status_name,
        r.creator,
        r.decline_reason,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = r.creator) as CreatorName,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=1) as RequestReceiptDocID,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=2) as RequestWorksheetDocID,
        (select unit_abreviation FROM inv_units u where r.required_unit_id_fk = u.unit_id(+) And rownum = 1) as RequiredUOMabbrv,
        GETNUMSHIPPEDCONTAINERS(r.request_id) AS NumShippedContainers,
        GETLOCATIONPATH(l.location_id) AS LocationPath
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_request_status rs, inv_container_batches icb
      WHERE
				r.batch_id_fk = p_batchId
      AND r.user_id_fk = p1.user_id(+)
      AND   r.batch_id_fk = icb.batch_id(+)
      AND 	r.request_status_id_fk = rs.request_status_id
      AND	Upper(r.delivered_by_id_fk) = p2.user_id(+)
      AND	r.delivery_location_id_fk = l.location_id
      AND   r.container_type_id_fk = ct.container_type_id(+)
      AND   r.request_type_id_fk = p_RequestTypeID
      AND	((r.user_id_fk LIKE NVL(p_UserID, '%') or r.creator LIKE NVL(p_UserID, '%') or p_UserID in (select USER_ID_FK from INV_ORG_USERS where org_unit_id_fk=r.org_unit_id_fk)))
      AND   request_status_id_fk not in (9)
      Order by Request_Id asc, request_status_id_fk asc, timestamp desc; --CSBR ID : 139040 SJ Ordering by Request_Id
ELSE
		OPEN O_RS FOR
      SELECT	distinct request_id,
				container_id_fk,
        r.batch_id_fk,
      	container_id_fk,
      	r.user_id_fk,
      	r.delivered_by_id_fk,
      	p1.user_id AS RUserID,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = p1.user_id) as RequestedForName,
      	p2.user_id AS DUserID,
      	to_char(trunc(r.timestamp), p_DateFormat) as TIMESTAMP,
      	to_char(trunc(r.date_required), p_DateFormat) as date_required,
      	to_char(trunc(r.date_delivered), p_DateFormat) as date_delivered,
      	l.location_name,
      	r.delivery_location_id_fk,
      	r.qty_required,
      	r.request_comments,
        r.request_type_id_fk,
        r.container_type_id_fk,
        r.number_containers,
        r.quantity_list,
        r.ship_to_name,
        ct.container_type_name,
        rs.request_status_name,
        r.creator,
        r.decline_reason, 
        request_status_id_fk,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = r.creator) as CreatorName,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=1) as RequestReceiptDocID,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=2) as RequestWorksheetDocID,
        (select unit_abreviation FROM inv_units u where r.required_unit_id_fk = u.unit_id(+) And rownum = 1) as RequiredUOMabbrv,
        GETNUMSHIPPEDCONTAINERS(r.request_id) AS NumShippedContainers,
        GETLOCATIONPATH(l.location_id) AS LocationPath,
        ivrb.*
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_request_status rs, inv_container_batches icb, inv_vw_reg_batches ivrb,inv_containers c, inv_compounds cmpd
      WHERE
				r.batch_id_fk = p_batchId
      AND r.user_id_fk = p1.user_id(+)
      AND   r.batch_id_fk = icb.batch_id(+)
						AND icb.batch_id = c.batch_id_fk
	  AND	c.compound_id_fk = cmpd.compound_id(+)
      AND   cmpd.reg_id_fk = ivrb.RegID(+)
      AND   cmpd.batch_number_fk = ivrb.BatchNumber(+)
      AND 	r.request_status_id_fk = rs.request_status_id
      AND	Upper(r.delivered_by_id_fk) = p2.user_id(+)
      AND	r.delivery_location_id_fk = l.location_id
      AND   r.container_type_id_fk = ct.container_type_id(+)
      AND   r.request_type_id_fk = p_RequestTypeID
      AND	((r.user_id_fk LIKE NVL(p_UserID, '%') or r.creator LIKE NVL(p_UserID, '%') or p_UserID in (select USER_ID_FK from INV_ORG_USERS where org_unit_id_fk=r.org_unit_id_fk)))
      AND   request_status_id_fk not in (9)
      Order by request_id asc, request_status_id_fk asc, timestamp desc; --CSBR ID : 139040 SJ Ordering by request_id

END IF;

	END GetRequestByBatch;

PROCEDURE GETBATCHRESERVATION(
    p_RequestTypeID IN inv_request_types.request_type_id%Type,
	p_UserID IN inv_requests.User_ID_FK%type,
    p_DateFormat IN VARCHAR2,
				p_regServer VARCHAR2,
	O_RS OUT CURSOR_TYPE) AS
	BEGIN

	IF p_regServer = 'NULL' THEN
		OPEN O_RS FOR
      SELECT	request_id,
        batch_id_fk,
      	container_id_fk,
      	r.user_id_fk,
      	r.delivered_by_id_fk,
      	p1.user_id AS RUserID,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = p1.user_id) as RequestedForName,
      	p2.user_id AS DUserID,
      	to_char(trunc(r.timestamp), p_DateFormat) as TIMESTAMP,
      	to_char(trunc(r.date_required), p_DateFormat) as date_required,
      	to_char(trunc(r.date_delivered), p_DateFormat) as date_delivered,
      	l.location_name,
      	r.delivery_location_id_fk,
      	r.qty_required,
      	r.request_comments,
        r.request_type_id_fk,
        r.container_type_id_fk,
        r.number_containers,
        r.quantity_list,
        r.ship_to_name,
        ct.container_type_name,
        rs.request_status_name,
        replace(CHEMINVDB2.GUIUTILS.GETBATCHUOMAMOUNTSTRING(NULL,r.batch_id_fk),':','') as Batch_Amount,
        (select org_name from inv_org_unit where org_unit_id = r.org_unit_id_fk) as Organization,
        r.creator,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = r.creator) as CreatorName,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=1) as RequestReceiptDocID,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=2) as RequestWorksheetDocID,
        (select unit_abreviation FROM inv_units u where r.required_unit_id_fk = u.unit_id(+) And rownum = 1) as RequiredUOMabbrv,
        --GETNUMSHIPPEDCONTAINERS(r.request_id) AS NumShippedContainers,
        GETLOCATIONPATH(l.location_id) AS LocationPath
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_request_status rs, inv_container_batches icb
      WHERE	r.user_id_fk = p1.user_id(+)
      AND   r.batch_id_fk = icb.batch_id(+)
      AND 	r.request_status_id_fk = rs.request_status_id
      AND	Upper(r.delivered_by_id_fk) = p2.user_id(+)
      AND	r.delivery_location_id_fk = l.location_id
      AND   r.container_type_id_fk = ct.container_type_id(+)
      AND   r.request_type_id_fk = p_RequestTypeID
      AND   r.request_status_id_fk = 9
      AND	((r.user_id_fk LIKE NVL(p_UserID, '%') or r.creator LIKE NVL(p_UserID, '%') or p_UserID in (select USER_ID_FK from INV_ORG_USERS where org_unit_id_fk=r.org_unit_id_fk)))
      Order by request_id desc, timestamp desc;
ELSE
		OPEN O_RS FOR
      SELECT	distinct request_id,
        r.batch_id_fk,
      	container_id_fk,
      	r.user_id_fk,
      	r.delivered_by_id_fk,
      	p1.user_id AS RUserID,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = p1.user_id) as RequestedForName,
      	p2.user_id AS DUserID,
      	to_char(trunc(r.timestamp), p_DateFormat) as TIMESTAMP,
      	to_char(trunc(r.date_required), p_DateFormat) as date_required,
      	to_char(trunc(r.date_delivered), p_DateFormat) as date_delivered,
      	l.location_name,
      	r.delivery_location_id_fk,
      	r.qty_required,
      	r.request_comments,
        r.request_type_id_fk,
        r.container_type_id_fk,
        r.number_containers,
        r.quantity_list,
        r.ship_to_name,
        ct.container_type_name,
        rs.request_status_name,
        replace(CHEMINVDB2.GUIUTILS.GETBATCHUOMAMOUNTSTRING(NULL,r.batch_id_fk),':','') as Batch_Amount,
        (select org_name from inv_org_unit where org_unit_id = r.org_unit_id_fk) as Organization,
        r.creator,
        (select last_name || ', ' || first_name from &&securitySchemaName..people p where p.user_id = r.creator) as CreatorName,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=1) as RequestReceiptDocID,
        (Select doc_id from inv_docs where inv_docs.table_name='INV_REQUESTS' and field_name='REQUEST_ID' and field_value=r.request_id and doc_type_id_fk=2) as RequestWorksheetDocID,
        (select unit_abreviation FROM inv_units u where r.required_unit_id_fk = u.unit_id(+) And rownum = 1) as RequiredUOMabbrv,
        --GETNUMSHIPPEDCONTAINERS(r.request_id) AS NumShippedContainers,
        ivrb.*,
        GETLOCATIONPATH(l.location_id) AS LocationPath
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_request_status rs, inv_container_batches icb, inv_vw_reg_batches ivrb, inv_containers c, inv_compounds cmpd
      WHERE	r.user_id_fk = p1.user_id(+)
      AND   r.batch_id_fk = icb.batch_id(+)
						AND icb.batch_id = c.batch_id_fk
	  AND	c.compound_id_fk = cmpd.compound_id(+)
      AND   cmpd.reg_id_fk = ivrb.RegID(+)
      AND   cmpd.batch_number_fk = ivrb.BatchNumber(+)
      AND 	r.request_status_id_fk = rs.request_status_id
      AND	Upper(r.delivered_by_id_fk) = p2.user_id(+)
      AND	r.delivery_location_id_fk = l.location_id
      AND   r.container_type_id_fk = ct.container_type_id(+)
      AND   r.request_type_id_fk = p_RequestTypeID
      AND   r.request_status_id_fk = 9
      AND	((r.user_id_fk LIKE NVL(p_UserID, '%') or r.creator LIKE NVL(p_UserID, '%') or p_UserID in (select USER_ID_FK from INV_ORG_USERS where org_unit_id_fk=r.org_unit_id_fk)))
      Order by request_id desc, timestamp desc;
END IF;
	END GETBATCHRESERVATION;

  FUNCTION GETNUMSAMPLES(
  	pRequestID inv_requests.request_id%TYPE)
  RETURN number
  IS
  	vCount number;
  BEGIN
  	SELECT count(*) INTO vCount
    FROM inv_request_samples
    WHERE
    	container_id_fk NOT IN (SELECT container_id_fk FROM inv_order_containers)
      AND request_id_fk = pRequestID;
    RETURN vCount;
  END GETNUMSAMPLES;

  FUNCTION GETORDERSFORREQUEST(
  	pRequestID inv_requests.request_id%TYPE)
  RETURN varchar2
  IS
  	vOrderList varchar2(500) := '';
    vOrderStatusID inv_orders.order_status_id_fk%TYPE;
  BEGIN
  	FOR vOrder_rec in (SELECT DISTINCT order_id_fk FROM inv_order_containers os, inv_request_samples rs WHERE os.container_id_fk = rs.container_id_fk AND rs.request_id_fk = pRequestID)
    LOOP
    	SELECT order_status_id_fk INTO vOrderStatusID FROM inv_orders WHERE order_id = vOrder_rec.order_id_fk;
    	vOrderList := vOrderList || vOrder_rec.Order_ID_FK || ':' || vOrderStatusID || ',';
    END LOOP;
    RETURN rTrim(vOrderList,',');
  END GETORDERSFORREQUEST;
	PROCEDURE GETOnOrderContainers(
		pContainerID IN inv_containers.container_id%Type,
		pDestinationLocationID IN inv_locations.location_id%Type,
		pFromDate IN varchar2,
		pToDate IN varchar2,
		pCurrentUserIDList IN varchar2,
		pSupplierID IN inv_containers.supplier_id_FK%type,
    	pSupplierCatNum IN inv_containers.supplier_catnum%type,
    	pCAS IN inv_compounds.cas%type,
   	 	pACXID IN inv_compounds.acx_id%type,
   		pSubstanceName IN inv_compounds.substance_name%type,
   		pPONumber in inv_containers.po_number%type,
		pPOLineNumber in inv_containers.po_line_number%type,
		pReqNumber in inv_containers.req_number%type,
		O_RS OUT CURSOR_TYPE) AS

		my_sql varchar2(3000);
    	keyWord varchar2(3):='';
	BEGIN

	    my_sql := '	SELECT 	c.container_id,
	        				c.container_name,
	        				c.current_user_id_fk,
	        				p.last_name||'', ''||p.first_name as userName,
	        				s.supplier_name,
	        				c.supplier_catnum,
	        				c.date_created,
	        				c.po_number,
	        				c.po_line_number,
	        				c.req_number,
	        				c.qty_max||'' ''||u.unit_Abreviation as csize,
	        				l.location_name as DeliveryLocation,
	        				l.location_id as DeliveryLocationID,
	        				getlocationpath(l.location_id) DeliveryPath,
	        				cpd.substance_name,
	        				cpd.cas,
	        				cpd.acx_id
	    			FROM
	    					inv_containers c,
	    					inv_locations l,
	    					inv_compounds cpd,
	    					&&securitySchemaName..people p,
	    					inv_suppliers s,
	    					inv_container_order co,
	    					inv_units u
	    			WHERE
	    					c.compound_id_fk = cpd.compound_id
	    			AND
	    					c.supplier_id_fk = s.supplier_id(+)
	    			AND
	    					co.container_id = c.container_id(+)
	    			AND
	    					co.delivery_location_id_fk = l.location_id
	    			AND
	    					c.current_user_id_fk = p.user_id
	    			AND
	    					c.unit_of_meas_id_fk = u.unit_id
	    			AND
	    					c.location_id_fk = 1
	    			AND
	    					c.date_created BETWEEN NVL(' || pFromDate || ',to_date(''1980-01-01 0:0:0'',''YYYY-MM-DD HH24:MI:SS'')) AND NVL(' || pToDate || ',SYSDATE)
	    					';

	     	if pContainerID is NOT NULL then
         		my_sql := my_sql || ' AND c.container_ID = '|| pContainerID;
      		end if;

	      	if pCurrentUserIDList is NOT NULL then
         		my_sql := my_sql || ' AND c.current_user_id_fk in (' || pCurrentUserIDList || ') ';
      		end if;

	        if pSupplierID is NOT NULL then
         		my_sql := my_sql || ' AND c.supplier_id_fk = '|| pSupplierID;
      		end if;

      		if pSupplierCatNum is NOT NULL then
         		my_sql := my_sql || ' AND lower(c.supplier_catnum) LIKE '''|| lower(pSupplierCatNum) || '''';
      		end if;

	     	if pSubstanceName is NOT NULL then
         		my_sql := my_sql || ' AND lower(cpd.substance_name) LIKE '''|| lower(pSubstanceName) || '''';
      		end if;

	     	if pCAS is NOT NULL then
         		my_sql := my_sql || ' AND cpd.cas LIKE '''|| pCAS || '''';
      		end if;

      		if pACXID is NOT NULL then
         		my_sql := my_sql || ' AND cpd.acx_id LIKE '''|| pACXID || '''';
      		end if;

	     OPEN O_RS FOR
	 		my_sql;
	END GETOnOrderContainers;

	Function ReceiveContainer(	pContainerID in inv_containers.container_id%type,
								pDeliveryLocationID in inv_locations.location_id%type,
								pContainerBarcode in inv_containers.barcode%type,
								pContainerStatus in inv_containers.container_status_id_fk%type) return integer as
	    duplicate_barcode exception;
		container_type_not_allowed exception;
		CURSOR dupBarcode_cur(Barcode_in in Inv_Containers.Barcode%Type) IS
  			SELECT Container_ID FROM Inv_Containers WHERE inv_Containers.Barcode = Barcode_in;
		dupBarcode_id Inv_Containers.Container_ID%Type;
    	l_DeliveryLocationID Inv_Locations.Location_ID%Type;

	begin
	    -- Check that the container can be moved to the delivery location
		if is_container_type_allowed(NULL, pDeliveryLocationID) = 0 then
  			RAISE container_type_not_allowed;
		end if;
		-- Check that the Barcode is not already in use
	      if pContainerBarcode is not null then
	        OPEN dupBarcode_cur(pContainerBarcode);
	        FETCH dupBarcode_cur into dupBarcode_id;
	        if dupBarcode_cur%ROWCOUNT = 1 then
	          RAISE duplicate_barcode;
	        end if;
	        CLOSE dupBarcode_cur;
	      end if;
		--get the next available rack position
		l_DeliveryLocationID := guiutils.GetLocationId(pDeliveryLocationID, NULL, NULL, NULL);
		-- update the container to change location, barcode and status
		update inv_containers
			set location_id_fk = l_DeliveryLocationID,
			def_location_id_fk = l_DeliveryLocationID,
				barcode = pContainerBarcode,
				container_status_id_fk = pContainerStatus,
              DATE_Received=sysdate
		where container_id = pContainerID;
		RETURN 1;
	exception
		WHEN duplicate_barcode then
			--RETURN 'A container with same barcode ID already exists:' || to_Char(dupBarcode_id);
		    RETURN -102;
		WHEN container_type_not_allowed then
		  	RETURN -128;
	end ReceiveContainer;

	PROCEDURE GETREQUESTS(
		pDeliverToLocationID IN inv_requests.delivery_location_id_fk%Type,
		pCurrentLocationID IN inv_requests.delivery_location_id_fk%Type,
		pFromDate IN varchar2,
		pToDate IN varchar2,
		pRequestComments IN inv_requests.request_comments%type,
		pUserID IN inv_requests.User_ID_FK%type,
		pContainerBarcode IN varchar2,
		pRequestType IN varchar2,
    pRequestTypeID IN inv_requests.request_type_id_fk%type,
    pRequestStatusID IN inv_requests.request_status_id_fk%type,
    pShipToName IN inv_requests.ship_to_name%type,
    pCreator IN inv_requests.creator%type,
    pDateFormat IN VARCHAR2,
		O_RS OUT CURSOR_TYPE)  AS

    my_sql varchar2(3000);
    keyWord varchar2(3):='';
	BEGIN
	    if Upper(pRequestType) = 'CLOSED' OR pRequestStatusID = 6 then
	       keyWord := 'NOT';
	    end if;
	    my_sql := 'SELECT	request_id,
		        		 container_id_fk,
		        		 c.container_name,
					       r.user_id_fk,
					       r.delivered_by_id_fk,
					       r.qty_required,
					       r.request_comments,
					       c.barcode,
					       u.unit_abreviation,
					       p1.user_id AS RUserID,
				         p2.user_id AS DUserID,
				         to_char(trunc(r.timestamp), ''' || pDateFormat || ''') as timestamp,
					       to_char(trunc(r.date_required), ''' || pDateFormat || ''') as date_required,
					       to_char(trunc(r.date_delivered), ''' || pDateFormat || ''') as date_delivered,
					       l.location_name,
                r.request_type_id_fk,
                r.container_type_id_fk,
                r.number_containers,
                r.quantity_list,
                r.ship_to_name,
                r.expense_center,
                ct.container_type_name,
                r.delivery_location_id_fk,
			          requests.GetNumSamples(r.request_id) as NumSamples,
                requests.GetOrdersForRequest(r.request_id) as OrderList,
                r.creator
                 FROM	inv_requests r, inv_locations l, &&securitySchemaName..people p1, &&securitySchemaName..people p2, inv_containers c, inv_units u, inv_container_types ct
					       WHERE	Upper(r.user_id_fk) = Upper(p1.user_id)
					       AND    r.container_id_fk = c.container_id
					       AND    c.unit_of_meas_id_fk = u.unit_id
                 AND   r.container_type_id_fk = ct.container_type_id(+)
					       AND		Upper(r.delivered_by_id_fk) = p2.user_id(+)
					       AND		r.delivery_location_id_fk = l.location_id
					       AND		r.timestamp BETWEEN NVL(' || pFromDate || ',to_date(''1980-01-01 0:0:0'',''YYYY-MM-DD HH24:MI:SS'')) AND NVL(' || pToDate || ',SYSDATE)
					       AND		r.user_id_fk LIKE NVL(''' || pUserID || ''', ''%'')
					       AND    Date_Delivered IS ' || keyWord || ' NULL';
	    if pDeliverToLocationID is NOT Null then
			   my_sql := my_sql || ' AND		r.delivery_location_id_fk IN (SELECT Location_ID from inv_Locations CONNECT BY prior Location_ID = Parent_ID START WITH Location_ID =' ||  to_char(pDeliverToLocationID) || ')';
			end if;
			if pCurrentLocationID is NOT Null then
			   my_sql := my_sql || ' AND		c.location_id_fk IN (SELECT Location_ID from inv_Locations CONNECT BY prior Location_ID = Parent_ID START WITH Location_ID =' ||  to_char(pCurrentLocationID) || ')';
			end if;
			if pRequestComments is NOT Null then
			   my_sql := my_sql || ' AND		r.request_comments LIKE ''%' || pRequestComments || '%''';
	    end if;
	    if pContainerBarcode is NOT NULL then
	       my_sql := my_sql || ' AND r.container_id_fk = (SELECT container_id FROM inv_containers WHERE barcode =''' || pContainerBarcode || ''')';
	    end if;
      if pRequestTypeID is NOT NULL then
         my_sql := my_sql || ' AND r.request_type_id_fk = ' || pRequestTypeID;
      end if;
	    if pRequestStatusID is NOT NULL then
         my_sql := my_sql || ' AND r.request_status_id_fk = ' || pRequestStatusID;
      end if;
	    if pShipToName is NOT NULL then
         my_sql := my_sql || ' AND r.ship_to_name  LIKE ''%' || pShipToName || '%''';
      end if;
	    if pCreator is NOT NULL then
         my_sql := my_sql || ' AND r.creator  LIKE ''%' || pCreator || '%''';
      end if;

	 OPEN O_RS FOR
	 	my_sql;
	END GETREQUESTS;

	PROCEDURE GETBATCHREQUESTS(
		pDeliverToLocationID IN inv_requests.delivery_location_id_fk%Type,
		pCurrentLocationID IN inv_requests.delivery_location_id_fk%Type,
		pFromDate IN varchar2,
		pToDate IN varchar2,
		pUserID IN inv_requests.User_ID_FK%type,
		pRequestType IN varchar2,
        pRequestTypeID IN inv_requests.request_type_id_fk%type,
        pRequestStatusID IN inv_requests.request_status_id_fk%type,
        pDateFormat IN VARCHAR2,
		O_RS OUT CURSOR_TYPE)  AS
    my_sql varchar2(4000);
    keyWord varchar2(50):='';
    l_batchFieldName1Display varchar2(50);
    l_batchFieldName2Display varchar2(50);
    l_batchFieldName3Display varchar2(50);
    		l_count INT;
          l_isRegBatch BOOLEAN;

    l_test1 VARCHAR2(2000);
    l_test2 VARCHAR2(2000);
    l_test3 VARCHAR2(2000);
    l_test4 VARCHAR2(2000);
    l_test5 VARCHAR2(2000);
	BEGIN
		--' figure out if the batching fields are reg_id and batch_number
		SELECT COUNT(*) INTO l_count FROM inv_container_batch_fields WHERE field_name IN ('REG_ID_FK','BATCH_NUMBER_FK');
          IF l_count = 2 THEN
          	l_isRegBatch := TRUE;
		END IF;

      if pRequestStatusID = 0 then
         keyWord := '' ;
      elsif Upper(pRequestType) = 'CLOSED' OR pRequestStatusID = 6 then
         keyWord := 'AND    Date_Delivered IS NOT NULL' ;
      else
         keyWord := 'AND    Date_Delivered IS NULL' ;
      end if;
      l_batchFieldName1Display := Rtrim(Ltrim(constants.cContainerBatchField1Display));
      l_batchFieldName2Display := Rtrim(Ltrim(constants.cContainerBatchField2Display));
      l_batchFieldName3Display := Rtrim(Ltrim(constants.cContainerBatchField3Display));
	    my_sql := 'SELECT request_id,
                  r.batch_id_fk as batch_id_fk,
                  r.user_id_fk,
                  r.delivered_by_id_fk,
                  r.qty_required,
                  r.qty_delivered,
		          r.request_comments,
                  p1.user_id AS RUserID,
                  p2.user_id AS DUserID,
                  to_char(trunc(r.timestamp), ''' || pDateFormat || ''') as timestamp,
                  to_char(trunc(r.date_required), ''' || pDateFormat || ''') as date_required,
                  to_char(trunc(r.date_delivered), ''' || pDateFormat || ''') as date_delivered,
                  l.location_name,
                  cb.batch_field_1 as batchvalue1,
                  cb.batch_field_2 as batchvalue2,
                  cb.batch_field_3 as batchvalue3,';
		IF l_isRegBatch THEN
                 my_sql := my_sql || 'ivrb.*,';
		END IF;
		my_sql := my_sql || 'cb.batch_status_id_fk as BatchStatus,
                  ''' || l_batchFieldName1Display || ''' as batchdisplay1,
                  ''' || l_batchFieldName2Display || ''' as batchdisplay2,
                  ''' || l_batchFieldName3Display || ''' as batchdisplay3,
                  r.request_type_id_fk,
                  r.container_type_id_fk,
                  r.number_containers,
                  r.quantity_list,
                  r.decline_reason,
                  r.assigned_user_id_fk,
                  r.ship_to_name,
                  r.expense_center,
                  r.delivery_location_id_fk,
                  requests.GetNumSamples(r.request_id) as NumSamples,
                  requests.GetOrdersForRequest(r.request_id) as OrderList,
                  r.creator,
                  case when (Select count(c1.Qty_Available) From CHEMINVDB2.inv_containers c1 Where c1.batch_id_fk = r.batch_id_fk AND c1.container_status_id_fk not in (6,7)) = 0 then 0 else (Select Sum(c2.Qty_Available* decode(c2.concentration,NULL,1,c2.concentration)) From CHEMINVDB2.inv_containers c2 Where c2.batch_id_fk = r.batch_id_fk AND c2.container_status_id_fk not in (6,7) and c2.unit_of_meas_id_fk=r.required_unit_id_fk and GUIUTILS.is_container_available(c2.container_id) = 1) end as AmountRemaining,
                  (select sum(r1.qty_required) from inv_requests r1 where r1.batch_id_fk=r.batch_id_fk and r1.request_status_id_fk=9) as AmountReserved,
                  cb.minimum_stock_threshold,
                  rs.request_status_name,
                  (Select doc_id from inv_docs where inv_docs.table_name=''INV_REQUESTS'' and field_name=''REQUEST_ID'' and field_value=r.request_id and doc_type_id_fk=1) as RequestReceiptDocID,
                  (Select doc_id from inv_docs where inv_docs.table_name=''INV_REQUESTS'' and field_name=''REQUEST_ID'' and field_value=r.request_id and doc_type_id_fk=2) as RequestWorksheetDocID,
                  (select unit_abreviation FROM inv_units u where r.required_unit_id_fk = u.unit_id(+) And rownum = 1) as RequiredUOMabbrv,
                  (select unit_abreviation FROM inv_containers c, inv_units u where batch_id_fk=r.batch_id_fk AND c.unit_of_meas_id_fk = u.unit_id(+) And rownum = 1) as uomabbrv,
                  r.FIELD_1,
                  r.FIELD_2,
                  r.FIELD_3,
                  r.FIELD_4,
                  r.FIELD_5,
                  r.DATE_1,
                  r.DATE_2
                  FROM	inv_requests r, inv_locations l, &&securitySchemaName..people p1, &&securitySchemaName..people p2, inv_container_batches cb, inv_request_status rs';
               IF l_isRegBatch THEN
				my_sql := my_sql || ', inv_vw_reg_batches ivrb';
			END IF;
              	my_sql := my_sql || ' WHERE	Upper(r.user_id_fk) = Upper(p1.user_id)';
               IF l_isRegBatch THEN
				my_sql := my_sql || 'AND cb.batch_field_1 = ivrb.regid(+)
                  					AND cb.batch_field_2 = ivrb.batchnumber(+)';
			END IF;
			my_sql := my_sql || 'AND r.batch_id_fk = cb.batch_id(+)
                  AND Upper(r.delivered_by_id_fk) = p2.user_id(+)
                  AND r.delivery_location_id_fk = l.location_id
                  AND r.request_status_id_fk = rs.request_status_id
                  AND r.timestamp BETWEEN NVL(' || pFromDate || ',to_date(''1980-01-01 0:0:0'',''YYYY-MM-DD HH24:MI:SS'')) AND NVL(' || pToDate || ',SYSDATE)
                  AND r.user_id_fk LIKE NVL(''' || pUserID || ''', ''%'') ' || keyWord ;
	  if pDeliverToLocationID is NOT Null and pDeliverToLocationID > 0 then
			   my_sql := my_sql || ' AND		r.delivery_location_id_fk IN (SELECT Location_ID from inv_Locations CONNECT BY prior Location_ID = Parent_ID START WITH Location_ID =' ||  to_char(pDeliverToLocationID) || ')';
	  end if;
      if pRequestTypeID is NOT NULL then
         my_sql := my_sql || ' AND r.request_type_id_fk = ' || pRequestTypeID;
      end if;
	    if pRequestStatusID is NOT NULL and pRequestStatusID > 0 then
         my_sql := my_sql || ' AND r.request_status_id_fk = ' || pRequestStatusID;
      end if;
      my_sql := my_sql || ' order by request_id desc';

	l_test1 := substr(my_sql, 1 , 950);
	l_test2 := substr(my_sql, 951, 950);
	l_test3 := substr(my_sql,1901, 950);
	l_test4 := substr(my_sql,2851, 950);
	l_test5 := substr(my_sql,3801, 950);
	 OPEN O_RS FOR
	 	my_sql;
	END GETBATCHREQUESTS;
	FUNCTION APPROVEREQUESTS(
		pRequestIDList varchar2
		)
	RETURN varchar2
	IS
		vNumUpdated number;
	BEGIN
		EXECUTE IMMEDIATE
		  'UPDATE inv_requests SET
			  request_status_id_fk = 3
		  WHERE request_id IN ('|| pRequestIDList ||')' ;

		  vNumUpdated := SQL%ROWCOUNT;
		  RETURN (vNumUpdated);
	END APPROVEREQUESTS;

	FUNCTION DECLINEREQUESTS(
		pRequestIDList varchar2,
    pDeclineReasonList varchar2
		)
	RETURN varchar2
	IS
		vNumUpdated number := 0;
    vRequestIDs_t STRINGUTILS.t_char;
    vDeclineReasons_t STRINGUTILS.t_char;
	BEGIN
  	vRequestIDs_t := STRINGUTILS.split(pRequestIDList, ',');
    vDeclineReasons_t := STRINGUTILS.split(pDeclineReasonList, ',');
    --RETURN vRequestIDs_t.LAST || '-' || vDeclineReasons_t.LAST || 'test';
  	FOR i in vRequestIDs_t.First..vRequestIDs_t.Last
  	LOOP
    	EXECUTE IMMEDIATE
  		   'UPDATE inv_requests SET
  			  request_status_id_fk = 4,
          decline_reason = :DeclineReasons
  		  WHERE request_id = :RequestIDs' 
  		  USING TRIM(vDeclineReasons_t(i)), vRequestIDs_t(i);
  		  vNumUpdated := vNumUpdated + SQL%ROWCOUNT;
      --insert into inv_debug values('test',vNumUpdated,null);
    End Loop;
	  RETURN (vNumUpdated);
	END DECLINEREQUESTS;


	FUNCTION APPROVEANDDECLINEREQUESTS(
		pApprovedRequestIDList varchar2,
		pDeclinedRequestIDList varchar2,
    pDeclineReasonList varchar2
	)
	RETURN varchar2
	IS
		vApproveReturn varchar2(200) := '0';
		vDeclineReturn varchar2(200) := '0';
	BEGIN
		if length(pApprovedRequestIDList) > 0 THEN
			vApproveReturn := ApproveRequests(pApprovedRequestIDList);
		end if;
		if length(pDeclinedRequestIDList) > 0 then
			vDeclineReturn := DeclineRequests(pDeclinedRequestIDList, pDeclineReasonList);
  	end if;
		RETURN vApproveReturn || '|' || vDeclineReturn;

		EXCEPTION
			WHEN OTHERS THEN
				RETURN 'ERROR:' || SQLCODE || ':' || SQLERRM;

	END APPROVEANDDECLINEREQUESTS;

	FUNCTION CLOSEREQUESTS(
		pRequestIDList varchar2)
	RETURN varchar2
	IS
		vNumUpdated number;
	BEGIN
		EXECUTE IMMEDIATE
		  'UPDATE inv_requests SET
			  request_status_id_fk = 6,
        date_delivered = sysdate
		  WHERE request_id IN ('|| pRequestIDList ||')' ;
		  
		  
		  vNumUpdated := SQL%ROWCOUNT;
		  RETURN (vNumUpdated);
	END CLOSEREQUESTS;


 FUNCTION GETSAMPLESPERCONTAINER(
  pRequestID inv_requests.request_id%TYPE,
  pQtyList inv_requests.quantity_list%TYPE,
  pBatchContainerIDs varchar2
 )
 RETURN varchar2
 IS
 vContainerID inv_containers.container_id%TYPE;
 vQtyList inv_requests.quantity_list%TYPE;
 vRequestUOMID inv_units.unit_id%TYPE;
 vBatchContainerIDs_t STRINGUTILS.t_char;
 vQtyList_t STRINGUTILS.t_char;
 vCurrBatchContainerID inv_containers.container_id%TYPE;
 vCurrBatchBarcode inv_containers.barcode%TYPE;
 vNumSamples number;
 vCurrQtyIndex number :=1;
 vQtyID inv_units.unit_id%TYPE;
 vUOMAbv inv_units.unit_abreviation%TYPE;
 vTempQty inv_containers.qty_remaining%TYPE;
 vCurrTempQty inv_containers.qty_remaining%TYPE;
 vReturnString varchar2(4000) := '';
 vUOMCnt integer;
 duplicate_uom exception;
 BEGIN
      select count(distinct unit_of_meas_id_fk) into vUOMCnt from inv_requests, inv_containers
        where inv_requests.batch_id_fk = inv_containers.batch_id_fk
        and inv_containers.location_id_fk not in (Constants.cOnOrderLoc,Constants.cDisposedLoc,Constants.cTrashCanLoc)
        and inv_containers.UNIT_OF_MEAS_ID_FK = inv_requests.required_unit_id_fk
        and request_id = pRequestID;
      if vUOMCnt > 1 then
        raise duplicate_uom;
      end if ;
      if Length(pQtyList) > 0 then
        select quantity_list,unit_of_meas_id_fk into vQtyList, vRequestUOMID
        from inv_requests, inv_containers
        where inv_requests.batch_id_fk = inv_containers.batch_id_fk
        and inv_containers.location_id_fk not in (Constants.cOnOrderLoc,Constants.cDisposedLoc,Constants.cTrashCanLoc)
        and inv_containers.UNIT_OF_MEAS_ID_FK = inv_requests.required_unit_id_fk
        and request_id = pRequestID
        group by quantity_list,unit_of_meas_id_fk;
        vQtyList := pQtyList;
      else
      SELECT container_id_fk, quantity_list, unit_of_meas_id_fk INTO vContainerID, vQtyList, vRequestUOMID
      FROM inv_requests, inv_containers
        WHERE container_id_fk = container_id
            AND request_id = pRequestID;
      end if;

      vBatchContainerIDs_t := STRINGUTILS.split(pBatchContainerIDs, ',');
     	vQtyList_t := STRINGUTILS.split(vQtyList, ',');

      FOR i IN vBatchContainerIDs_t.First..vBatchContainerIDs_t.Last
      LOOP
          SELECT container_id, barcode, qty_remaining, unit_of_meas_id_fk, unit_abreviation
          INTO vCurrBatchContainerID, vCurrBatchBarcode, vTempQty, vQtyID, vUOMAbv
          FROM inv_containers, inv_units
          WHERE
                unit_of_meas_id_fk = unit_id(+)
                AND container_id = vBatchContainerIDs_t(i);
          vNumSamples := 0;
          vCurrTempQty := vTempQty;
          WHILE vCurrQtyIndex <= vQtyList_t.Last
          LOOP
              --vReturnString := vReturnString || vTempQty || '|';
              vCurrTempQty := vTempQty;
              vTempQty := PlateChem.QuantitySubtraction(vTempQty, vQtyID, vQtyList_t(vCurrQtyIndex), vRequestUOMID);
              --RETURN vTempQty;
              IF vTempQty >= 0 THEN
                 vCurrTempQty := vTempQty;
                 vNumSamples := vNumSamples + 1;
                 vCurrQtyIndex := vCurrQtyIndex + 1;
              ELSE
                 EXIT;
              END IF;
          END LOOP;
          vReturnString := vReturnString || vCurrBatchContainerID || ':' || vCurrBatchBarcode || ':' || vNumSamples || ':' || vCurrTempQty || ':' || vUOMAbv || ',';
      END LOOP;
      RETURN rtrim(vReturnString,',');
      --return 'test';

      exception
      WHEN duplicate_uom then
        RETURN -103;
 END GETSAMPLESPERCONTAINER;
 FUNCTION GETBATCHSAMPLESPERCONTAINER(
  pRequestID inv_requests.request_id%TYPE,
  pBatchContainerIDs varchar2
 )
 RETURN varchar2
 IS
 vContainerID inv_containers.container_id%TYPE;
 vQtyList inv_requests.quantity_list%TYPE;
 vRequestUOMID inv_units.unit_id%TYPE;
 vBatchContainerIDs_t STRINGUTILS.t_char;
 vQtyList_t STRINGUTILS.t_char;
 vCurrBatchContainerID inv_containers.container_id%TYPE;
 vCurrBatchBarcode inv_containers.barcode%TYPE;
 vNumSamples number;
 vCurrQtyIndex number :=1;
 vQtyID inv_units.unit_id%TYPE;
 vUOMAbv inv_units.unit_abreviation%TYPE;
 vTempQty inv_containers.qty_remaining%TYPE;
 vCurrTempQty inv_containers.qty_remaining%TYPE;
 vReturnString varchar2(500) := '';
 BEGIN
      SELECT qty_required INTO vQtyList
      FROM inv_requests
      WHERE request_id = pRequestID;
      vBatchContainerIDs_t := STRINGUTILS.split(pBatchContainerIDs, ',');
     	--vQtyList_t := STRINGUTILS.split(vQtyList, ',');
      --vQtyList_t := vQtyList;
      FOR i IN vBatchContainerIDs_t.First..vBatchContainerIDs_t.Last
      LOOP
          SELECT container_id, barcode, qty_remaining, unit_of_meas_id_fk, unit_abreviation
          INTO vCurrBatchContainerID, vCurrBatchBarcode, vTempQty, vQtyID, vUOMAbv
          FROM inv_containers, inv_units
          WHERE unit_of_meas_id_fk = unit_id(+)
          AND container_id = vBatchContainerIDs_t(i);
          vNumSamples := 0;
          -- vCurrTempQty := vTempQty;
          -- WHILE vCurrQtyIndex <= vQtyList_t.Last
          -- LOOP
          --vReturnString := vReturnString || vTempQty || '|';
          vCurrTempQty := vTempQty;
          vTempQty := PlateChem.QuantitySubtraction(vTempQty, vQtyID, vQtyList, vQtyID);
          --RETURN vTempQty;
          IF vTempQty >= 0 THEN
             vCurrTempQty := vTempQty;
             vNumSamples := vNumSamples + 1;
             --vCurrQtyIndex := vCurrQtyIndex + 1;
          END IF;
          -- END LOOP;
          vReturnString := vReturnString || vCurrBatchContainerID || ':' || vCurrBatchBarcode || ':' || vNumSamples || ':' || vCurrTempQty || ':' || vUOMAbv || ',';
          --vReturnString := vReturnString || vCurrBatchContainerID || ':' || vCurrBatchBarcode || ':2:3:ml,';
      END LOOP;
      RETURN rtrim(vReturnString,',');
      --return 'test';
 END GETBATCHSAMPLESPERCONTAINER;

PROCEDURE FULFILLREQUEST(
	pRequestID inv_requests.request_id%TYPE,
 	pSampleContainerIDs varchar2
    -- Added for CSBR-123488
 	,p_field1 inv_requests.field_1%TYPE
    ,p_field2 inv_requests.field_2%TYPE
    ,p_field3 inv_requests.field_3%TYPE
    ,p_field4 inv_requests.field_4%TYPE
    ,p_field5 inv_requests.field_5%TYPE
    ,p_date1 inv_requests.date_1%TYPE
    ,p_date2 inv_requests.date_2%TYPE
    -- Addition ends for CSBR-123488
)
IS
 	vSampleContainerIDs_t STRINGUTILS.t_char;
BEGIN
	--change request status
  UPDATE inv_requests SET request_status_id_fk = 5
  -- Added for CSBR-123488
      , field_1 = p_field1
      , field_2 = p_field2
      , field_3 = p_field3
      , field_4 = p_field4
      , field_5 = p_field5
      , date_1 = p_date1
      , date_2 = p_date2
    -- Added ends for CSBR-123488
WHERE request_id = pRequestID;

  --insert into inv_request_samples
  vSampleContainerIDs_t := STRINGUTILS.split(pSampleContainerIDs, '|');
  FORALL i IN vSampleContainerIDs_t.First..vSampleContainerIDs_t.Last
	  INSERT INTO inv_request_samples VALUES (pRequestID, vSampleContainerIDs_t(i));
END;
PROCEDURE FULFILLBATCHREQUEST(
	pRequestID inv_requests.request_id%TYPE
  )
IS
BEGIN
	--change request status
  UPDATE inv_requests SET request_status_id_fk = 5 WHERE request_id = pRequestID;
  --insert into inv_request_samples
  /*
  vSampleContainerIDs_t := STRINGUTILS.split(pSampleContainerIDs, '|');
  FORALL i IN vSampleContainerIDs_t.First..vSampleContainerIDs_t.Last
	  INSERT INTO inv_request_samples VALUES (pRequestID, vSampleContainerIDs_t(i));
  */
END;

 FUNCTION CREATEORDER(
 	pDeliveryLocationID inv_orders.delivery_location_id_fk%TYPE,
  pShipToName inv_orders.ship_to_name%TYPE,
  pShippingConditions inv_orders.shipping_conditions%TYPE,
  pSampleContainerIDs varchar2,
  pStatusID inv_container_status.container_status_id%TYPE)
 RETURN inv_orders.order_id%TYPE
 IS
 	vNewOrderID inv_orders.order_id%TYPE;
 	vSampleContainerIDs_t STRINGUTILS.t_char;
 BEGIN
 	--insert into inv_orders with a status of 1(New)
  INSERT INTO inv_orders (delivery_location_id_fk, ship_to_name, order_status_id_fk, shipping_conditions, date_created) VALUES(pDeliveryLocationID, pShipToName, 1, pShippingConditions, SYSDATE) RETURNING order_id INTO vNewOrderID;
 	--insert into inv_order_containers
  IF length(pSampleContainerIDs) > 0 THEN
    vSampleContainerIDs_t := STRINGUTILS.split(pSampleContainerIDs, ',');
    FORALL i IN vSampleContainerIDs_t.First..vSampleContainerIDs_t.Last
			INSERT INTO inv_order_containers (order_id_fk,container_id_fk) VALUES (vNewOrderID, vSampleContainerIDs_t(i));
    --update the status of containers in the order
    FORALL i IN vSampleContainerIDs_t.First..vSampleContainerIDs_t.LAST
    	UPDATE inv_containers SET container_status_id_fk = pStatusID WHERE container_id = vSampleContainerIDs_t(i);
 	END IF;
 	RETURN vNewOrderID;

 END CREATEORDER;

 FUNCTION EDITORDER(
 	pOrderID inv_orders.order_id%TYPE,
 	pDeliveryLocationID inv_orders.delivery_location_id_fk%TYPE,
	pShipToName inv_orders.ship_to_name%TYPE,
  pShippingConditions inv_orders.shipping_conditions%TYPE,
	pSampleContainerIDs VARCHAR2,
  pStatusID inv_container_status.container_status_id%TYPE)
 RETURN inv_orders.order_id%TYPE
 IS
 	vSampleContainerIDs_t STRINGUTILS.t_char;
	vOldValue audit_column.old_value%TYPE := NULL;
  BEGIN
		UPDATE inv_orders SET
			delivery_location_id_fk = pDeliveryLocationID,
	  	ship_to_name = pShipToName,
      shipping_conditions = pShippingConditions
		WHERE order_id = pOrderID;

    --set the status back to its previous value for these containers
    FOR vOrderContainer_rec IN (SELECT container_id_fk FROM inv_order_containers WHERE order_id_fk = pOrderID)
    LOOP
    	vOldValue :=  audit_trail.GETLASTCOLUMNVALUE(vOrderContainer_rec.container_id_fk,'inv_containers','container_status_id_fk');
			IF vOldValue IS NOT NULL THEN
        UPDATE inv_containers SET container_status_id_fk = vOldValue WHERE container_id = vOrderContainer_rec.container_id_fk;
    	END IF;
      vOldValue := NULL;
    END LOOP;
		--delete then insert sample ids
    DELETE inv_order_containers WHERE order_id_fk = pOrderID;
    IF length(pSampleContainerIDs) > 0 THEN
      vSampleContainerIDs_t := STRINGUTILS.split(pSampleContainerIDs, ',');
      FORALL i IN vSampleContainerIDs_t.First..vSampleContainerIDs_t.Last
    	  INSERT INTO inv_order_containers (order_id_fk,container_id_fk) VALUES (pOrderID, vSampleContainerIDs_t(i));
	    --update the status of containers in the order
      FORALL i IN vSampleContainerIDs_t.First..vSampleContainerIDs_t.LAST
	    	UPDATE inv_containers SET container_status_id_fk = pStatusID WHERE container_id = vSampleContainerIDs_t(i);
    END IF;
   	RETURN pOrderID;

 END EDITORDER;

  PROCEDURE GETORDER(
   	pOrderID inv_orders.order_id%TYPE,
    O_RS OUT CURSOR_TYPE)
  IS
  		vSQL varchar2(500);
      vSampleContainerIDs varchar2(500) := '';
  BEGIN
  	FOR vOrderSample_rec IN (SELECT container_id_fk FROM inv_order_containers WHERE order_id_fk = pOrderID)
    LOOP
    	vSampleContainerIDs := vSampleContainerIDs || vOrderSample_rec.container_id_fk || ',';
    END LOOP;
    vSampleContainerIDs := rtrim(vSampleContainerIDs,',');
  	vSQL := 'SELECT o.*, ''' ||  vSampleContainerIDs || ''' as SampleContainerIDs FROM inv_orders o WHERE ORDER_ID = ' || pOrderID;

  	OPEN O_RS FOR
  		vSQL;
  END GETORDER;

  FUNCTION SHIPORDER(
  	pOrderID inv_orders.order_id%TYPE)
  RETURN inv_orders.order_id%TYPE
  IS

  BEGIN
  	--update order
  	UPDATE inv_orders SET
    	order_status_id_fk = 2,
      date_shipped = SYSDATE
    WHERE order_id = pOrderID;
   	--update order containers: change status,location
		UPDATE inv_containers SET
    	location_id_fk = constants.cOnOrderLoc,
      container_status_id_fk = constants.cInTransit
    WHERE container_id IN
    	(SELECT container_id_fk FROM inv_order_containers WHERE order_id_fk = pOrderID);
    RETURN pOrderID;
  END SHIPORDER;

  FUNCTION SHIPORDERS(
  	pOrderIDList varchar2)
  RETURN varchar2
  IS
 		vOrderIDList_t STRINGUTILS.t_char;
    vStatus number;
  BEGIN

  	vOrderIDList_t := STRINGUTILS.split(pOrderIDList, ',');
    FOR i IN vOrderIDList_t.First..vOrderIDList_t.Last
    LOOP
    	vStatus := ShipOrder(vOrderIDList_t(i));
    END LOOP;

  	RETURN pOrderIDList;

  END SHIPORDERS;

	PROCEDURE GETORDERS(
		pShipToName IN inv_orders.ship_to_name%TYPE,
		pDeliveryLocationID IN inv_orders.delivery_location_id_fk%Type,
    pOrderStatusID IN inv_orders.order_status_id_fk%TYPE,
		pFromDate IN varchar2,
		pToDate IN varchar2,
		pContainerBarcode IN inv_containers.barcode%TYPE,
    pDateFormat IN VARCHAR2,
		O_RS OUT CURSOR_TYPE)  AS

    my_sql varchar2(2000);
	BEGIN
			my_sql := 'SELECT	order_id,
      						delivery_location_id_fk,
						      l.location_name,
                  GetLocationPath(l.location_id) AS Path,
                  ship_to_name,
                  to_char(trunc(date_created), ''' || pDateFormat || ''') as date_created,
                  to_char(trunc(date_shipped), ''' || pDateFormat || ''') as date_shipped,
                  to_char(trunc(date_received), ''' || pDateFormat || ''') as date_received,
                  order_status_id_fk,
                  order_status_name,
                  cancel_reason,
                  (SELECT count(*) FROM inv_order_containers WHERE order_id_fk = o.order_id) AS NumContainers
                 FROM	inv_orders o, inv_locations l, inv_order_status os
					       WHERE	o.delivery_location_id_fk = l.location_id
                 	AND o.order_status_id_fk = os.order_status_id';                 	
	    if pDeliveryLocationID is NOT Null then
			  my_sql := my_sql || ' AND		o.delivery_location_id_fk IN (SELECT Location_ID from inv_Locations CONNECT BY prior Location_ID = Parent_ID START WITH Location_ID =' ||  to_char(pDeliveryLocationID) || ')';
			end if;
			if pShipToName is NOT Null then
			  my_sql := my_sql || ' AND		lower(o.ship_to_name) LIKE ''%' || lower(pShipToName) || '%''';
	    end if;
	    if pContainerBarcode is NOT NULL then
	      my_sql := my_sql || ' AND o.order_id in (SELECT order_id_fk FROM inv_order_containers, inv_containers WHERE container_id_fk = container_id and barcode =''' || pContainerBarcode || ''')';
	    end if;
      if pOrderStatusID is NOT NULL then
        my_sql := my_sql || ' AND o.order_status_id_fk = ' || pOrderStatusID;
				IF pFromDate <> 'NULL' AND pToDate <> 'NULL' THEN
        	IF pOrderStatusID = 1 THEN
           	my_sql := my_sql || ' AND		o.date_created BETWEEN NVL(' || pFromDate || ',to_date(''1980-01-01 0:0:0'',''YYYY-MM-DD HH24:MI:SS'')) AND NVL(' || pToDate || ',SYSDATE)';
  				ELSIF pOrderStatusID = 2 THEN
           	my_sql := my_sql || ' AND		o.date_shipped BETWEEN NVL(' || pFromDate || ',to_date(''1980-01-01 0:0:0'',''YYYY-MM-DD HH24:MI:SS'')) AND NVL(' || pToDate || ',SYSDATE)';
          END IF;
				END IF;
      end if;
	 OPEN O_RS FOR
	       my_sql;
	END GETORDERS;

  FUNCTION RECEIVECONTAINERS(
  	pOrderID inv_orders.order_id%TYPE,
  	pContainerIDList varchar2,
    pStatusID inv_container_status.container_status_id%TYPE)
  RETURN varchar2
  IS
 		vContainerIDList_t STRINGUTILS.t_char;
    vRequestIDList_t STRINGUTILS.t_char;
    vStatus varchar2(50);
    vIsAll boolean;
    vRequestID inv_requests.request_id%TYPE;
    vRequestIDList varchar2(500) := ' ';
    vDeliveryLocationID inv_locations.location_id%TYPE;
    vTempDeliveryLocationID inv_locations.location_id%TYPE;
    vCount integer;
  BEGIN
  	--get the order delivery_location_id_fk
    SELECT delivery_location_id_fk INTO vDeliveryLocationID FROM inv_orders WHERE order_id = pOrderID;

  	vContainerIDList_t := STRINGUTILS.split(pContainerIDList, ',');
    FOR i IN vContainerIDList_t.First..vContainerIDList_t.Last
    LOOP
	  vTempDeliveryLocationID:= guiutils.GetLocationId(vDeliveryLocationID, NULL, NULL, NULL);
    	--update the status of the container
 	   	--update the location of the containers to the order delivery location
    	UPDATE inv_containers SET
      	container_status_id_fk = pStatusID,
        location_id_fk = vTempDeliveryLocationID,
          DATE_RECEIVED=sysdate
      WHERE container_id = vContainerIDList_t(i);

			--build a list of the requests associated with the containers
			SELECT count(*) INTO vCount FROM inv_request_samples WHERE container_id_fk = vContainerIDList_t(i);
      IF vCount > 0 THEN
        SELECT request_id_fk INTO vRequestID FROM inv_request_samples WHERE container_id_fk = vContainerIDList_t(i);
        IF instr(vRequestIDList,vRequestID) = 0 THEN
        	vRequestIDList := vRequestIDList || vRequestID || ',';
        END IF;
      END IF;
    END LOOP;

    --check to see if an entire order has been received
    vIsAll := true;
    FOR vOrderSample_rec IN (SELECT * FROM inv_order_containers WHERE order_id_fk = pOrderID)
    LOOP
    	IF instr(pContainerIDList, vOrderSample_rec.Container_ID_FK) = 0 THEN
      	vIsAll := false;
      END IF;
    END LOOP;
    IF vIsAll THEN
    	--set the order status to closed
    	UPDATE inv_orders SET
      	order_status_id_fk = 3,
        date_received = SYSDATE
      WHERE order_id = pOrderID;
    END IF;

    --check to see if all containers for a request have been received
    vRequestIDList := rtrim(vRequestIDList,',');
    IF vRequestIDList <> ' ' THEN
      vRequestIDList_t := STRINGUTILS.split(vRequestIDList,',');
      FOR i IN vRequestIDList_t.First..vRequestIDList_t.Last
      LOOP
      	vIsAll := true;
      	FOR vRequestSample_rec IN (SELECT * FROM inv_request_samples WHERE request_id_fk = vRequestIDList_t(i))
        LOOP
        	IF instr(pContainerIDList, vRequestSample_rec.Container_ID_FK) = 0 THEN
          	vIsAll := false;
          END IF;
        END LOOP;
        IF vIsAll THEN
        	--close the request
  				vStatus := CloseRequests(vRequestIDList_t(i));
        END IF;
      END LOOP;
		END IF;
    RETURN pContainerIDList;

  END RECEIVECONTAINERS;

FUNCTION REMOVECONTAINERS(
  	pOrderID inv_orders.order_id%TYPE,
  	pContainerIDList varchar2)
RETURN varchar2
IS
	vContainerIDList_t STRINGUTILS.t_char;
	vOldValue audit_column.old_value%TYPE := NULL;	
BEGIN  	
    vContainerIDList_t := STRINGUTILS.split(pContainerIDList, ',');
    FOR i IN vContainerIDList_t.First..vContainerIDList_t.Last
    LOOP
		-- Set the status and location back to their previous values for these containers
		BEGIN
    		vOldValue :=  audit_trail.GETLASTCOLUMNVALUE(vContainerIDList_t(i),'inv_containers','container_status_id_fk');
			IF vOldValue IS NOT NULL THEN
				UPDATE inv_containers SET container_status_id_fk = vOldValue WHERE container_id = vContainerIDList_t(i);
			END IF;
			vOldValue := NULL;
		EXCEPTION
				WHEN NO_DATA_FOUND THEN vOldValue := NULL;
		END;
		
		BEGIN
			vOldValue :=  audit_trail.GETLASTCOLUMNVALUE(vContainerIDList_t(i),'inv_containers','location_id_fk');
			IF vOldValue IS NOT NULL THEN
				UPDATE inv_containers SET location_id_fk = vOldValue WHERE container_id = vContainerIDList_t(i);
    		END IF;
    		
    		vOldValue := NULL;
		EXCEPTION
				WHEN NO_DATA_FOUND THEN vOldValue := NULL;
		END;
		
		-- Delete from inv_order_containers
		delete inv_order_containers where container_id_fk = vContainerIDList_t(i);
	END LOOP;	
	
RETURN pContainerIDList;

END REMOVECONTAINERS;

  FUNCTION CANCELORDER (
  	pOrderID inv_orders.order_id%TYPE,
    pCancelReason inv_orders.cancel_reason%TYPE)
  RETURN VARCHAR2
  IS
		vOldValue audit_column.old_value%TYPE := NULL;
  BEGIN

	--set the status and location back to their previous values for these containers
    FOR vOrderContainer_rec IN (SELECT container_id_fk FROM inv_order_containers WHERE order_id_fk = pOrderID)
    LOOP
    	vOldValue :=  audit_trail.GETLASTCOLUMNVALUE(vOrderContainer_rec.container_id_fk,'inv_containers','container_status_id_fk');
		IF vOldValue IS NOT NULL THEN
			UPDATE inv_containers SET container_status_id_fk = vOldValue WHERE container_id = vOrderContainer_rec.container_id_fk;
    	END IF;
    	vOldValue := NULL;
    	
    	vOldValue :=  audit_trail.GETLASTCOLUMNVALUE(vOrderContainer_rec.container_id_fk,'inv_containers','location_id_fk');
		IF vOldValue IS NOT NULL THEN
			UPDATE inv_containers SET location_id_fk = vOldValue WHERE container_id = vOrderContainer_rec.container_id_fk;
    	END IF;
    	vOldValue := NULL;
    END LOOP;    

  	UPDATE inv_orders SET
    	cancel_reason = pCancelReason,
      order_status_id_fk = 4
    WHERE order_id = pOrderID;

    RETURN SQL%ROWCOUNT;

  END CANCELORDER;
  
  FUNCTION CLOSEORDER (
  	pOrderID inv_orders.order_id%TYPE)
  RETURN VARCHAR2
  IS		
  BEGIN    
  	UPDATE inv_orders SET    	
		order_status_id_fk = 3,
		date_received = SYSDATE
    WHERE order_id = pOrderID;

    RETURN SQL%ROWCOUNT;

  END CLOSEORDER;

PROCEDURE GETBATCHREQUESTBYREQUESTEDUNIT(
		pRequestID IN inv_requests.request_ID%Type,
    pDateFormat IN VARCHAR2,
		O_RS OUT CURSOR_TYPE) AS
	BEGIN
		OPEN O_RS FOR
      SELECT	request_id,
      c.container_id,
      r.batch_id_fk,
      c.location_id_fk,
      r.user_id_fk,
      r.org_unit_id_fk,
      r.delivered_by_id_fk,
      p1.user_id AS RUserID,
      p2.user_id AS DUserID,
			to_char(trunc(r.timestamp), pDateFormat) as TIMESTAMP,
			to_char(trunc(r.date_required), pDateFormat) as date_required,
			to_char(trunc(r.date_delivered), pDateFormat) as date_delivered,
      l.location_name,
      r.delivery_location_id_fk,
      r.qty_required,
      r.request_comments,
      r.request_type_id_fk,
      c.container_type_id_fk,
      (select count(*) from inv_containers where batch_id_fk = r.batch_id_fk) as number_containers,
      case when (Select count(c1.Qty_Available) From CHEMINVDB2.inv_containers c1 Where c1.batch_id_fk = r.batch_id_fk AND c1.container_status_id_fk not in (6,7)) = 0 then 0 else (Select Sum(c2.Qty_Available*c2.concentration) From CHEMINVDB2.inv_containers c2 Where c2.batch_id_fk = r.batch_id_fk AND c2.container_status_id_fk not in (6,7)) end as AmountRemaining,
      (select sum(r1.qty_required) from inv_requests r1 where r1.batch_id_fk=c.batch_id_fk and r1.request_status_id_fk=9) as AmountReserved,
      --r.number_containers,
      r.quantity_list,
      r.ship_to_name,
      r.expense_center,
      ct.container_type_name,
      u.unit_abreviation,
      c.unit_of_meas_id_fk,
      r.creator,
      c.family,
      r.field_1,
      r.field_2,
      r.field_3,
      r.field_4,
      r.field_5,
      r.date_1,
      r.date_2
      FROM inv_requests r, inv_locations l, &&securitySchemaName..people p1,&&securitySchemaName..people p2, inv_container_types ct, inv_containers c, inv_units u
      WHERE	r.user_id_fk = p1.user_id(+)
      AND r.creator = p2.user_id(+) /* changed for fixing bug 77167*/
      AND (r.container_id_fk = c.container_id or r.batch_id_fk = c.batch_id_fk)
      AND c.unit_of_meas_id_fk = u.unit_id(+)
      AND c.location_id_fk not in (Constants.cDisposedLoc, Constants.cTrashCanLoc)
      AND r.container_type_id_fk = ct.container_type_id (+)
      AND	r.delivery_location_id_fk = l.location_id
      AND	r.request_id = pRequestID
     AND c.UNIT_OF_MEAS_ID_FK = r.required_unit_id_fk;
	END GETBATCHREQUESTBYREQUESTEDUNIT;

END REQUESTS;
/
show errors;
