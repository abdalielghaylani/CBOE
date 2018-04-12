CREATE OR REPLACE FUNCTION CHECKOPENPOSITIONS
(pOrderID IN INV_ORDERS.ORDER_ID%TYPE)
 RETURN Inv_Locations.Location_ID%Type 
 IS
l_locationId inv_locations.location_id%TYPE;
l_numContainers number;
l_numShippedContainers number;
l_openPositions number;

BEGIN
	Select (select count(*) from INV_ORDER_CONTAINERS where ORDER_ID_FK= pOrderId ),DELIVERY_LOCATION_ID_FK into l_numContainers,l_locationId from INV_ORDERS where ORDER_ID= pOrderID;
	IF Racks.isRack(l_locationId) = 0 and Racks.isRackLocation(l_locationId) = 0 THEN
		return 1;
	Else
		l_openPositions := Racks.multiOpenPositionCount(l_locationId);
		IF Racks.isRackLocation(l_locationId) = 0 then
		  SELECT COUNT(INV_ORDER_CONTAINERS.CONTAINER_ID_FK) into l_numShippedContainers
		  FROM INV_ORDER_CONTAINERS,inv_orders 
		  WHERE inv_orders.DELIVERY_LOCATION_ID_FK in (select LOCATION_ID from inv_locations where location_id= l_locationId or PARENT_ID=l_locationId)
		  AND inv_orders.ORDER_ID=INV_ORDER_CONTAINERS.ORDER_ID_FK
		  AND inv_orders.ORDER_STATUS_ID_FK=2;
		else
		  SELECT COUNT(INV_ORDER_CONTAINERS.CONTAINER_ID_FK) into l_numShippedContainers
		  FROM INV_ORDER_CONTAINERS,inv_orders 
		  WHERE inv_orders.DELIVERY_LOCATION_ID_FK in (select LOCATION_ID from inv_locations where PARENT_ID= (select PARENT_ID from inv_locations where  LOCATION_ID=l_locationId))
		  AND inv_orders.ORDER_ID=INV_ORDER_CONTAINERS.ORDER_ID_FK
		  AND inv_orders.ORDER_STATUS_ID_FK=2;
		END IF;
		l_openPositions:= l_openPositions - l_numShippedContainers;
		If l_numContainers > l_openPositions Then
				return 0;
			Else
				return 1;
		END IF ;
	End if;
END;
/