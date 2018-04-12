CREATE OR REPLACE  PACKAGE "&&SchemaName"."LINKS"      
	AS
   TYPE  CURSOR_TYPE IS REF CURSOR;

   FUNCTION CREATELINK
      (pFK_value IN inv_URL.FK_value%Type,
	     pFK_name IN inv_URL.FK_name%Type,
	     pTable_Name IN inv_URL.Table_Name%Type,
	     pURL IN inv_URL.URL%Type,
	     pLinkText IN inv_URL.Link_Txt%Type,
	     pImageSource IN inv_URL.Image_Src%Type,
	     pURLType IN inv_URL.URL_Type%Type
	     )
	     RETURN inv_URL.URL_ID%Type;

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
	     RETURN inv_URL.URL_ID%Type;

	 FUNCTION DELETELINK
      (pURLID IN inv_URL.URL_ID%Type)
	     RETURN inv_URL.URL_ID%Type;

	 PROCEDURE GETLINKS
	    (pFK_value IN inv_URL.FK_value%Type,
	     pFK_name IN inv_URL.FK_name%Type,
	     pTable_Name IN inv_URL.Table_Name%Type,
	     pURLType IN inv_URL.URL_Type%Type,
	     O_RS OUT CURSOR_TYPE);

END LINKS;
/
show errors;
