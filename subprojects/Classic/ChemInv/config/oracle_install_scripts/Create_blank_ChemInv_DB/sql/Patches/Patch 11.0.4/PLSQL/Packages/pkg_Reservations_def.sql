-- Reservations Package Definition
CREATE OR REPLACE  PACKAGE "&&SchemaName"."RESERVATIONS"                              
    AS
   TYPE  CURSOR_TYPE IS REF CURSOR;
   
   /* Overloaded function */
   FUNCTION CREATERESERVATION
      (pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     pUserID IN inv_Reservations.User_ID_FK%Type,
	     pQtyReserved IN inv_Reservations.Qty_Reserved%Type,
	     pReservationTypeID IN inv_Reservations.Reservation_Type_ID_FK%Type)
	     RETURN inv_Reservations.Reservation_ID%Type;
   
   /* CSBR ID : 135218 Overloading CreateReservation function with a new parameter - RequestID */
   FUNCTION CREATERESERVATION
      (pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     pUserID IN inv_Reservations.User_ID_FK%Type,
	     pQtyReserved IN inv_Reservations.Qty_Reserved%Type,
	     pReservationTypeID IN inv_Reservations.Reservation_Type_ID_FK%Type,
       pRequestID IN inv_requests.request_id%Type)
	     RETURN inv_Reservations.Reservation_ID%Type;
   
   /* CSBR ID : 135218 This function should not be called directly. */
   FUNCTION INTERNALCREATERESERVATION        
      (pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     pUserID IN inv_Reservations.User_ID_FK%Type,
	     pQtyReserved IN inv_Reservations.Qty_Reserved%Type,
	     pReservationTypeID IN inv_Reservations.Reservation_Type_ID_FK%Type,
       pRequestID IN inv_requests.request_id%Type)
	     RETURN inv_Reservations.Reservation_ID%Type;
	 
	 FUNCTION UPDATERESERVATION
      (pReservationID IN  inv_Reservations.Reservation_ID%Type,
	     pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     pUserID IN inv_Reservations.User_ID_FK%Type, 
	     pQtyReserved IN inv_Reservations.Qty_Reserved%Type,
	     pReservationTypeID IN inv_Reservations.Reservation_Type_ID_FK%Type)
	     RETURN inv_Reservations.Reservation_ID%Type;
	 
	 FUNCTION DELETERESERVATION
      (pReservationID IN  inv_Reservations.Reservation_ID%Type,
       pContainerID IN  inv_Containers.Container_ID%Type)
      RETURN inv_Reservations.Reservation_ID%Type;
	 
   FUNCTION GETTOTALQTYRESERVED
      (pContainerID in inv_Reservations.Container_ID_FK%Type)
      RETURN inv_Reservations.Qty_Reserved%Type;

	 FUNCTION GETQTYAVAILABLE
      (pContainerID IN inv_Containers.Container_ID%Type)
      RETURN inv_Containers.Qty_Available%Type;
	 
	 PROCEDURE RECONCILEQTYAVAILABLE
	    (pContainerID IN  inv_Containers.Container_ID%Type);
	 
	 PROCEDURE GETRESERVATIONS
	    (pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     O_RS OUT CURSOR_TYPE);
	 END RESERVATIONS;
/
show errors;
