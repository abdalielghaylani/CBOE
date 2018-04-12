CREATE OR REPLACE FUNCTION "UPDATEADDRESS"
  (pTableName varchar2,
   pTablePKID varchar2,
   pAddressID inv_address.Address_ID%TYPE,
   pContactName varchar2,
   pAddress1 inv_address.Address1%TYPE,
   pAddress2 inv_address.Address2%TYPE,
   pAddress3 inv_address.Address3%TYPE,
   pAddress4 inv_address.Address4%TYPE,
   pCity inv_address.City%TYPE,
   pStateIDFK inv_address.State_ID_FK%TYPE,
   pCountryIDFK inv_address.Country_ID_FK%TYPE,
   pZIP inv_address.ZIP%TYPE,
   pFAX inv_address.FAX%TYPE,
   pPhone inv_address.Phone%TYPE,
   pEmail inv_address.Email%TYPE)
RETURN varchar2
IS

vAddressID inv_address.Address_ID%TYPE;
vAction varchar2(10);
BEGIN

	vAction := 'edit';
	IF pAddressID is NULL THEN
		vAction := 'new';
	END IF;

	IF vAction = 'new' THEN
		INSERT INTO inv_address (Contact_Name, Address1, Address2, Address3, Address4, City, State_ID_FK, Country_ID_FK, ZIP, FAX, Phone, Email)
			VALUES(pContactName,
      	pAddress1,
				pAddress2,
				pAddress3,
        pAddress4,
				pCity,
				pStateIDFK,
				pCountryIDFK,
				pZIP,
				pFAX,
				pPhone,
				pEmail) RETURNING Address_ID INTO vAddressID;
			IF lower(pTableName) = 'inv_suppliers' THEN
				 	UPDATE inv_suppliers SET SUPPLIER_ADDRESS_ID_FK = vAddressID WHERE supplier_id = pTablePKID;
      ELSIF lower(pTableName)='inv_locations' THEN
          UPDATE inv_locations SET ADDRESS_ID_FK = vAddressID WHERE location_id = pTablePKID;
			END IF;
	ELSIF vAction = 'edit' THEN
		UPDATE inv_address SET Contact_Name = pContactName, Address1 = pAddress1, Address2 = pAddress2, Address3 = pAddress3, Address4 = pAddress4, City = pCity, State_ID_FK = pStateIDFK, Country_ID_FK = pCountryIDFK, ZIP = pZIP, FAX = pFAX, Phone = pPhone, Email = pEmail WHERE Address_ID = pAddressID;
    vAddressID := pAddressID;
	END IF;

    RETURN vAddressID;

exception
WHEN OTHERS then
	RETURN '0';
END UPDATEADDRESS;
/
show errors;