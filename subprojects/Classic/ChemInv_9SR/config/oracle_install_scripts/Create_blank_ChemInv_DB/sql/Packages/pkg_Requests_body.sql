CREATE OR REPLACE PACKAGE BODY "REQUESTS"
AS

	FUNCTION CREATEREQUEST(
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
	END CREATEREQUEST;


	FUNCTION UPDATEREQUEST(
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
	END UPDATEREQUEST;

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
		  WHERE request_id IN (' || pRequestIDList ||')';
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
		pRequestID IN inv_requests.request_ID%Type)
  RETURN inv_requests.request_ID%Type AS
	BEGIN
		UPDATE inv_requests SET request_status_id_fk = 7 WHERE request_id = pRequestID;
		RETURN pRequestID;
	END CANCELREQUEST;


	PROCEDURE GETREQUEST(
		pRequestID IN inv_requests.request_ID%Type,
    pDateFormat IN VARCHAR2,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			SELECT	request_id,
					container_id_fk,
          c.location_id_fk,
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
          r.expense_center,
          ct.container_type_name,
          u.unit_abreviation,
          c.unit_of_meas_id_fk,
          r.creator,
          c.family
			FROM inv_requests r, inv_locations l, cs_security.people p1,cs_security.people p2, inv_container_types ct, inv_containers c, inv_units u
			WHERE	r.user_id_fk = p1.user_id
			AND		Upper(r.delivered_by_id_fk) = p2.user_id(+)
   AND r.container_id_fk = c.container_id
   AND c.unit_of_meas_id_fk = u.unit_id(+)
   AND r.container_type_id_fk = ct.container_type_id (+)
			AND		r.delivery_location_id_fk = l.location_id
			AND		r.request_id = pRequestID;
	END GETREQUEST;

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
      FROM inv_requests r, inv_locations l, cs_security.people p1,cs_security.people p2, inv_container_types ct, inv_request_status rs
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
                 FROM	inv_requests r, inv_locations l, cs_security.people p1, cs_security.people p2, inv_containers c, inv_units u, inv_container_types ct
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

	 OPEN O_RS FOR
	 	my_sql;
	END GETREQUESTS;

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
		  WHERE request_id IN (' || pRequestIDList ||')';

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
          decline_reason = ''' || TRIM(vDeclineReasons_t(i)) || '''
  		  WHERE request_id = ' || vRequestIDs_t(i);
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
		  WHERE request_id IN (' || pRequestIDList ||')';

		  vNumUpdated := SQL%ROWCOUNT;
		  RETURN (vNumUpdated);
	END CLOSEREQUESTS;


 FUNCTION GETSAMPLESPERCONTAINER(
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
 vReturnString varchar2(4000) := '';
 BEGIN
      SELECT container_id_fk, quantity_list, unit_of_meas_id_fk INTO vContainerID, vQtyList, vRequestUOMID
      FROM inv_requests, inv_containers
      WHERE
            container_id_fk = container_id
            AND request_id = pRequestID;

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

 END GETSAMPLESPERCONTAINER;

PROCEDURE FULFILLREQUEST(
	pRequestID inv_requests.request_id%TYPE,
 	pSampleContainerIDs varchar2)
IS
 	vSampleContainerIDs_t STRINGUTILS.t_char;
BEGIN
	--change request status
  UPDATE inv_requests SET request_status_id_fk = 5 WHERE request_id = pRequestID;

  --insert into inv_request_samples
  vSampleContainerIDs_t := STRINGUTILS.split(pSampleContainerIDs, '|');
  FORALL i IN vSampleContainerIDs_t.First..vSampleContainerIDs_t.Last
	  INSERT INTO inv_request_samples VALUES (pRequestID, vSampleContainerIDs_t(i));
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
    vCount integer;
  BEGIN
  	--get the order delivery_location_id_fk
    SELECT delivery_location_id_fk INTO vDeliveryLocationID FROM inv_orders WHERE order_id = pOrderID;

  	vContainerIDList_t := STRINGUTILS.split(pContainerIDList, ',');
    FOR i IN vContainerIDList_t.First..vContainerIDList_t.Last
    LOOP
    	--update the status of the container
 	   	--update the location of the containers to the order delivery location
    	UPDATE inv_containers SET
      	container_status_id_fk = pStatusID,
        location_id_fk = vDeliveryLocationID
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

  FUNCTION CANCELORDER (
  	pOrderID inv_orders.order_id%TYPE,
    pCancelReason inv_orders.cancel_reason%TYPE)
  RETURN VARCHAR2
  IS
		vOldValue audit_column.old_value%TYPE := NULL;
  BEGIN

      --set the status back to its previous value for these containers
    FOR vOrderContainer_rec IN (SELECT container_id_fk FROM inv_order_containers WHERE order_id_fk = pOrderID)
    LOOP
    	vOldValue :=  audit_trail.GETLASTCOLUMNVALUE(vOrderContainer_rec.container_id_fk,'inv_containers','container_status_id_fk');
			IF vOldValue IS NOT NULL THEN
        UPDATE inv_containers SET container_status_id_fk = vOldValue WHERE container_id = vOrderContainer_rec.container_id_fk;
    	END IF;
    END LOOP;

  	UPDATE inv_orders SET
    	cancel_reason = pCancelReason,
      order_status_id_fk = 4
    WHERE order_id = pOrderID;

    RETURN SQL%ROWCOUNT;

  END CANCELORDER;


END REQUESTS;
/
show errors;