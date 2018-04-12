
--Reservations Package Body
CREATE OR REPLACE PACKAGE BODY "RESERVATIONS"
    IS
  FUNCTION GETTOTALQTYRESERVED(pContainerID in inv_Reservations.Container_ID_FK%Type)
  RETURN inv_Reservations.Qty_Reserved%Type AS
  TotalReserved inv_Reservations.Qty_Reserved%Type;
  BEGIN
    SELECT Sum(Qty_Reserved) INTO TotalReserved FROM inv_Reservations WHERE Container_ID_FK = pContainerID AND Is_Active = 1;
    IF TotalReserved IS NULL THEN
      TotalReserved := 0;
    END IF;
    RETURN TotalReserved;
  END GETTOTALQTYRESERVED;

 /* Overloaded function */
  FUNCTION CREATERESERVATION
      (pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     pUserID IN inv_Reservations.User_ID_FK%Type,
	     pQtyReserved IN inv_Reservations.Qty_Reserved%Type,
	     pReservationTypeID IN inv_Reservations.Reservation_Type_ID_FK%Type)
	     RETURN inv_Reservations.Reservation_ID%Type as
       NewReservationId inv_reservations.reservation_id%Type;
  BEGIN
      NewReservationId := INTERNALCREATERESERVATION(pContainerID,pUserID,pQtyReserved,pReservationTypeID,null);
      return NewReservationId;
  END CREATERESERVATION;
  
  /* Overloaded function */
  FUNCTION CREATERESERVATION
      (pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     pUserID IN inv_Reservations.User_ID_FK%Type,
	     pQtyReserved IN inv_Reservations.Qty_Reserved%Type,
	     pReservationTypeID IN inv_Reservations.Reservation_Type_ID_FK%Type,
       pRequestID IN inv_requests.request_id%Type)
	     RETURN inv_Reservations.Reservation_ID%Type as
       NewReservationId inv_reservations.reservation_id%Type;
  BEGIN
      NewReservationId := INTERNALCREATERESERVATION(pContainerID,pUserID,pQtyReserved,pReservationTypeID,pRequestID);
      return NewReservationId;
  END CREATERESERVATION;
  
  /* This function should not be called directly.*/        
  FUNCTION INTERNALCREATERESERVATION
      (pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     pUserID IN inv_Reservations.User_ID_FK%Type,
	     pQtyReserved IN inv_Reservations.Qty_Reserved%Type,
	     pReservationTypeID IN inv_Reservations.Reservation_Type_ID_FK%Type,
       pRequestID IN inv_requests.request_id%Type)
	     RETURN inv_Reservations.REservation_ID%Type AS
       NewReservationID inv_Reservations.Reservation_ID%Type;
       insuficient_qty exception;
          QtyAvailable inv_Containers.Qty_Available%Type;
   BEGIN
        QtyAvailable := GETQTYAVAILABLE(pContainerID);
        if QtyAvailable < pQtyReserved then
          RAISE insuficient_qty;
        End if;
        INSERT INTO inv_Reservations
          (Reservation_ID,
           Container_ID_FK,
           User_ID_FK,
           Qty_Reserved,
           Date_Reserved,
           Reservation_Type_ID_FK,
           Is_Active,
           Request_ID_FK)
        VALUES
          (seq_inv_reservations.nextval,
           pContainerID,
           pUserID,
           pQtyReserved,
           sysdate,
           pReservationTypeID,
           1,
           pRequestID)
         RETURNING Reservation_ID INTO NewReservationID;
         RECONCILEQTYAVAILABLE(pContainerID);
         RETURN NewReservationID;
         EXCEPTION
         WHEN insuficient_qty then
	         --'Cannot reserve more than total quantity available.';
	         RETURN -122;
	  END INTERNALCREATERESERVATION;

    FUNCTION UPDATERESERVATION
      (pReservationID IN  inv_Reservations.Reservation_ID%Type,
	     pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     pUserID IN inv_Reservations.User_ID_FK%Type,
	     pQtyReserved IN inv_Reservations.Qty_Reserved%Type,
	     pReservationTypeID IN inv_Reservations.Reservation_Type_ID_FK%Type)
	     RETURN inv_Reservations.Reservation_ID%Type AS
	  insuficient_qty exception;
    QtyAvailable inv_Containers.Qty_Available%Type;
	  currQtyReserved inv_Reservations.Qty_Reserved%Type;
	  BEGIN
	   QtyAvailable := GETQTYAVAILABLE(pContainerID);
     SELECT Qty_Reserved INTO currQtyReserved FROM inv_Reservations WHERE Reservation_ID = pReservationID;
     if QtyAvailable + currQtyReserved < pQtyReserved then
        RAISE insuficient_qty;
     End if;
	   UPDATE
	     inv_Reservations
	   SET
	     User_ID_FK = pUserID,
	     Qty_Reserved = pQtyReserved,
	     Date_Reserved = sysdate,
	     Reservation_Type_ID_FK = pReservationTypeID,
	     Is_Active = 1
	   WHERE
	     Reservation_ID = pReservationID;
	   RECONCILEQTYAVAILABLE(pContainerID);
	   RETURN pReservationID;
	   EXCEPTION
     WHEN insuficient_qty then
	     --RETURN 'Cannot reserve more than total quantity available.';
	     RETURN -122;
	  END UPDATERESERVATION;

    PROCEDURE GETRESERVATIONS
      (pContainerID IN  inv_Reservations.Container_ID_FK%Type,
	     O_RS OUT CURSOR_TYPE) AS
    BEGIN
     OPEN O_RS FOR
     SELECT  Reservation_ID, Container_ID_FK, User_ID_FK, Qty_Reserved, Date_Reserved, Reservation_Type_ID_FK, Is_Active, inv_Reservation_types.Reservation_Type_Name
     FROM inv_Reservations, inv_Reservation_Types
	   WHERE
	     inv_Reservations.Reservation_Type_ID_FK = inv_Reservation_Types.Reservation_Type_ID
	   AND
	     Container_ID_FK = pContainerID
	   ORDER BY Date_Reserved;
    END GETRESERVATIONS;

    FUNCTION DELETERESERVATION
      (pReservationID IN  inv_Reservations.Reservation_ID%Type,
       pContainerID IN  inv_Containers.Container_ID%Type)
      RETURN inv_Reservations.Reservation_ID%Type AS
    BEGIN
      DELETE FROM inv_Reservations WHERE Reservation_ID = pReservationID;
      RECONCILEQTYAVAILABLE(pContainerID);
      RETURN pReservationID;
    END DELETERESERVATION;

    PROCEDURE RECONCILEQTYAVAILABLE
	    (pContainerID IN  inv_Containers.Container_ID%Type) AS
	  QtyReserved_total inv_Reservations.Qty_Reserved%Type;
    QtyRemaining inv_Containers.Qty_Remaining%Type;
	  QtyTemp inv_Containers.Qty_Remaining%Type;
    vCurrQtyAvailable inv_containers.qty_available%TYPE;
    vQtyAvailable inv_containers.qty_available%TYPE;
	  CURSOR Reservations_curr (pCID in inv_Reservations.Container_ID_FK%Type) IS
	     SELECT Reservation_ID, QTY_Reserved
	     FROM inv_Reservations WHERE Container_ID_FK = pCID AND Is_Active = 1;
	  BEGIN
	     SELECT Qty_Remaining, qty_available INTO QtyRemaining, vCurrQtyAvailable FROM inv_Containers WHERE Container_ID = pContainerID;
	     QtyTemp := QtyRemaining;
	     QtyReserved_total := GETTOTALQTYRESERVED(pContainerID);
	  IF QtyRemaining < QtyReserved_total THEN
	     -- Cancel on or more reserves
	     FOR Reservation_rec IN Reservations_curr(pContainerID)
	     LOOP
	       IF Reservation_rec.Qty_Reserved > QtyTemp THEN
	         -- UPDATE inv_Reservations SET IS_Active = 0 WHERE Reservation_ID = Reservation_rec.Reservation_ID;
                 Delete From inv_Reservations WHERE Reservation_ID = Reservation_rec.Reservation_ID;
	       ELSE
	         QtyTemp := QtyTemp - Reservation_rec.Qty_Reserved;
	       END IF;
	     END LOOP;
	     QtyReserved_total := GETTOTALQTYRESERVED(pContainerID);
	  END IF;
    -- Update the QtyAvailable if it has changed
    vQtyAvailable := QtyRemaining - QtyReserved_total;
    IF vQtyAvailable != vCurrQtyAvailable THEN
  	  UPDATE inv_Containers
  	  SET Qty_Available = vQtyAvailable
  	  WHERE Container_ID = pContainerID;
		END IF;
	  END RECONCILEQTYAVAILABLE;

    FUNCTION GETQTYAVAILABLE
      (pContainerID IN inv_Containers.Container_ID%Type)
      RETURN inv_Containers.Qty_Available%Type AS
    QtyAvailable inv_containers.Qty_Available%Type;
    BEGIN
      SELECT Qty_Available INTO QtyAvailable FROM inv_Containers WHERE Container_ID = pContainerID;
      RETURN QtyAvailable;
    END GETQTYAVAILABLE;
END RESERVATIONS;
/
show errors;


