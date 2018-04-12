CREATE OR REPLACE  PACKAGE BODY "&&SchemaName"."LINKS"           
    IS
   FUNCTION CREATELINK
      (pFK_value IN inv_URL.FK_value%Type,
	     pFK_name IN inv_URL.FK_name%Type,
	     pTable_Name IN inv_URL.Table_Name%Type,
	     pURL IN inv_URL.URL%Type,
	     pLinkText IN inv_URL.Link_Txt%Type,
	     pImageSource IN inv_URL.Image_Src%Type,
	     pURLType IN inv_URL.URL_Type%Type
	     )
	   RETURN inv_URL.URL_ID%Type AS
	   newURLID inv_URL.URL_ID%Type;
	 BEGIN
	   INSERT INTO   inv_URL
	                 (FK_value, FK_name, Table_Name, URL, Link_txt, Image_src, URL_Type)
	   VALUES        (pFK_value, pFK_name, pTable_Name, pURL, pLinktext, pImageSource, pURLType)
	   RETURNING URL_ID INTO newURLID;
	 RETURN newURLID;
	 END CREATELINK;

	 FUNCTION UPDATELINK
      (pURLID IN inv_URL.URL_ID%Type,
       pFK_value IN inv_URL.FK_value%Type,
	     pFK_name IN inv_URL.FK_name%Type,
	     pTable_Name IN inv_URL.Table_Name%Type,
	     pURL IN inv_URL.URL%Type,
	     pLinkText IN inv_URL.Link_Txt%Type,
	     pImageSource IN inv_URL.Image_Src%Type,
	     pURLType IN inv_URL.URL_Type%Type:=' '
	     )
	   RETURN inv_URL.URL_ID%Type AS
	 BEGIN
	   UPDATE inv_URL
	   SET  FK_value = pFK_value,
	        FK_name = pFK_name, 
	        Table_Name = pTable_Name, 
	        URL = pURL, 
	        Link_txt = pLinkText, 
	        Image_src = pImageSource, 
	        URL_Type = pURLType
	   WHERE URL_ID = pURLID;
	 RETURN pURLID;
	 END UPDATELINK;

	 FUNCTION DELETELINK (pURLID IN inv_URL.URL_ID%Type)
	   RETURN inv_URL.URL_ID%Type AS
	 BEGIN
	   DELETE FROM inv_URL
	   WHERE URL_ID = pURLID;
	 RETURN pURLID;
	 END DELETELINK;

   PROCEDURE GETLINKS 
      (pFK_value IN inv_URL.FK_value%Type,
	     pFK_name IN inv_URL.FK_name%Type,
	     pTable_Name IN inv_URL.Table_Name%Type,
	     pURLType IN inv_URL.URL_Type%Type,
	     O_RS OUT CURSOR_TYPE) AS
   BEGIN
     OPEN O_RS FOR
     SELECT  URL_ID, FK_value, FK_name, Table_Name, URL, Link_txt, Image_src, URL_Type
     FROM inv_URL
	   WHERE FK_value LIKE NVL(pFK_value,'%') 
	   AND   Upper(FK_Name) LIKE NVL(Upper(pFK_name),'%')
	   AND   Upper(Table_Name) LIKE NVL(Upper(Table_Name),'%')
	   --AND   Upper(URL_Type) LIKE NVL(Upper(pURLType),'%')
	   ORDER BY URL_Type;
   END GETLINKS;

END LINKS;
/
show errors;
