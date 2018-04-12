CREATE OR REPLACE FUNCTION "INSERTCHECKINDETAILS"(
	pContainerID IN inv_containers.container_id%TYPE,
 	pUserID IN inv_container_checkin_details.user_id_fk%TYPE:=NULL,
	pField1 IN inv_container_checkin_details.field_1%TYPE:=NULL,
 	pField2 IN inv_container_checkin_details.field_2%TYPE:=NULL,
 	pField3 IN inv_container_checkin_details.field_3%TYPE:=NULL,
 	pField4 IN inv_container_checkin_details.field_4%TYPE:=NULL,
 	pField5 IN inv_container_checkin_details.field_5%TYPE:=NULL,
 	pField6 IN inv_container_checkin_details.field_6%TYPE:=NULL,
 	pField7 IN inv_container_checkin_details.field_7%TYPE:=NULL,
 	pField8 IN inv_container_checkin_details.field_8%TYPE:=NULL,
 	pField9 IN inv_container_checkin_details.field_9%TYPE:=NULL,
 	pField10 IN inv_container_checkin_details.field_10%TYPE:=NULL,
 	pDate1 IN inv_container_checkin_details.date_1%TYPE:=NULL,
 	pDate2 IN inv_container_checkin_details.date_2%TYPE:=NULL,
 	pDate3 IN inv_container_checkin_details.date_3%TYPE:=NULL)
RETURN inv_container_checkin_details.checkin_details_ID%TYPE
IS
	vNewID inv_container_checkin_details.checkin_details_id%TYPE;
BEGIN

	INSERT INTO inv_container_checkin_details (
  	container_id_fk,
    user_id_fk,
    field_1,
    field_2,
    field_3,
    field_4,
    field_5,
    field_6,
    field_7,
    field_8,
    field_9,
    field_10,
    date_1,
    date_2,
    date_3
  )
  VALUES (
  	pContainerID,
    pUserID,
    pField1,
    pField2,
    pField3,
    pField4,
    pField5,
    pField6,
    pField7,
    pField8,
    pField9,
    pField10,
    pDate1,
    pDate2,
    pDate3
  ) RETURNING checkin_details_id INTO vNewID;

	RETURN vNewID;
  

END INSERTCHECKINDETAILS;
/
show errors;