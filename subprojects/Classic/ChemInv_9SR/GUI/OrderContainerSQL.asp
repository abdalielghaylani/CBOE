<%
'Container_ID
'Barcode
'Container_Type_Name
'Container_Status_Name 
'Container_Description
'Container_Comments
'Location_Name
'Substance_Name 
'CAS 
'ACX_ID
'DATE_Expires
'DATE_Created
'DATE_Produced
'DATE_Ordered
'DATE_Received
'ContainerSize 
'AmountRemaining 
'AmountAvailable 
'Concentration_Text 
'Purity_Text  
'ContainerCost	     
'Grade 
'Solvent
'Lot_Num
'Supplier_CATNUM   
'Supplier_Name
'Supplier_Short_Name
'PO_Number
'Req_Number
'Qty_MinStock 
'Qty_MaxStock 
'Reg_ID_FK
'BatchNumber 

' SQL to get all attributes of a container
SQL = "SELECT " & _ 
    "inv_Containers.Container_ID AS Container_ID,  " & _
    "inv_Containers.Barcode AS Barcode," & _  
    "inv_Containers.Container_Name AS Container_Name, " & _
    "inv_Container_Types.Container_Type_Name AS Container_Type_Name,  " & _
    "inv_Container_Status.Container_Status_Name AS Container_Status_Name,  " & _ 
    "inv_Containers.Container_Description AS Container_Description, " & _
    "inv_Containers.Container_Comments AS Container_Comments, " & _
    "inv_Locations.Location_Name AS Location_Name,  " & _
    "inv_Compounds.Substance_Name AS Substance_Name, " & _ 
    "inv_Compounds.Compound_ID AS Compound_ID, " & _
    "inv_Compounds.CAS AS CAS, " & _ 
    "inv_Compounds.ACX_ID AS ACX_ID, " & _
    "inv_Compounds.BASE64_CDX AS BASE64_CDX, " & _
    "inv_Containers.DATE_Expires AS DATE_Expires,  " & _
    "inv_Containers.DATE_Created AS DATE_Created,  " & _
    "inv_Containers.DATE_Produced AS DATE_Produced, " & _
    "inv_Containers.DATE_Ordered AS DATE_Ordered, " & _
    "inv_Containers.DATE_Received AS DATE_Received, " & _
    
    "DECODE(inv_Containers.Qty_Max, NULL, ' ', inv_Containers.Qty_Max||' '||UOM.Unit_Abreviation) AS ContainerSize ,  " & _
    "DECODE(inv_Containers.Qty_Remaining, NULL, ' ', inv_Containers.Qty_Remaining||' '||UOM.Unit_Abreviation) AS AmountRemaining ,  " & _
    "DECODE(inv_Containers.Qty_Available, NULL, ' ', inv_Containers.Qty_Available||' '||UOM.Unit_Abreviation) AS AmountAvailable ,  " & _
    "DECODE(inv_Containers.Concentration, NULL, ' ', inv_Containers.Concentration||' '||UOC.Unit_Abreviation) AS Concentration_Text, " & _ 
    "DECODE(inv_Containers.Purity, NULL, ' ', inv_Containers.Purity||' '||UOP.Unit_Abreviation) AS Purity_Text, " & _  
	"DECODE(inv_Containers.Container_Cost, NULL, ' ', UOCost.Unit_Abreviation||TO_CHAR(inv_Containers.Container_Cost,'999,999.99')) AS ContainerCost,  " & _	     
    
    "inv_Containers.Concentration AS Concentration, " & _
    "inv_Containers.Purity AS Purity, " & _
    "inv_Containers.Grade AS Grade, " & _ 
    "inv_Containers.Solvent AS Solvent, " & _
    "inv_Containers.Lot_Num AS Lot_Num, " & _
    "inv_Containers.Supplier_CATNUM AS Supplier_CATNUM, " & _   
    "inv_Suppliers.Supplier_Name AS Supplier_Name, " & _
    "inv_Suppliers.Supplier_Short_Name AS Supplier_Short_Name, " & _
    "inv_Containers.PO_Number AS PO_Number, " & _
    "inv_Containers.Req_Number AS Req_Number, " & _
    "inv_Containers.Qty_MinStock AS Qty_MinStock, " & _ 
    "inv_Containers.Qty_MaxStock AS Qty_MaxStock, " & _ 
    "inv_Containers.Reg_ID_FK,  " & _ 
    "inv_Containers.Batch_Number_FK,  " & _
    "inv_Containers.Location_ID_FK,  " & _
    "inv_Containers.Compound_ID_FK,  " & _
    "inv_Containers.Container_Type_ID_FK,  " & _
    "inv_Containers.Container_Status_ID_FK,  " & _
    "inv_Containers.Supplier_ID_FK, " & _
    "inv_Containers.Owner_ID_FK, " & _ 
    "inv_Containers.Current_User_ID_FK, " & _
    
    "inv_Containers.Qty_Initial AS Qty_Initial,  " & _
    "inv_Containers.Qty_Max AS Qty_Max,  " & _
    "inv_Containers.Qty_Remaining AS Qty_Remaining,  " & _
    "inv_Containers.Qty_Available AS Qty_Available,  " & _
    
    "UOM.Unit_ID AS UOMID,  " & _
    "UOP.Unit_ID AS UOPID,  " & _
    "UOC.Unit_ID AS UOCID,  " & _
    "UOW.Unit_ID AS UOWID,  " & _
    "UOCost.Unit_ID AS UOCostID,  " & vblf & _
    "UOM.Unit_Name AS UOMName,  " & _
    "UOP.Unit_Name AS UOPName,  " & _
    "UOC.Unit_Name AS UOCName,  " & _
    "UOW.Unit_Name AS UOWName,  " & _
    "UOCost.Unit_Name AS UOCostName,  " & vblf & _
    "UOM.Unit_Abreviation AS UOMAbv,  " & _
    "UOP.Unit_Abreviation AS UOPAbv,  " & _
    "UOC.Unit_Abreviation AS UOCAbv,  " & _
    "UOW.Unit_Abreviation AS UOWAbv,  " & _
    "UOCost.Unit_Abreviation AS UOCostAbv,  " & vblf & _
    
    "inv_Containers.Tare_Weight AS Tare_Weight,  " & _
    "TO_CHAR(inv_Containers.Container_Cost, '999,999.99') AS Container_Cost " & _
"FROM " & _ 
    "inv_Containers, inv_Locations, inv_Units UOM, inv_Units UOP, inv_Units UOC, inv_Units UOW, inv_Container_Types, inv_Compounds, inv_Suppliers, inv_container_status, inv_units UOCost  " & _
"WHERE  " & _ 
  "inv_Containers.Location_ID_FK = inv_Locations.Location_ID  " & _
"AND " & _ 
  "inv_Containers.UNIT_OF_MEAS_ID_FK = UOM.Unit_ID " & _ 
"AND " & _ 
  "inv_Containers.Unit_of_Purity_ID_FK = UOP.Unit_ID(+) " & _ 
"AND " & _ 
  "inv_Containers.Unit_of_Conc_ID_FK = UOC.Unit_ID(+) " & _ 
"AND " & _ 
  "inv_Containers.Unit_of_wght_ID_FK = UOW.Unit_ID(+) " & _
"AND " & vblf & _ 
  "inv_Containers.Unit_of_Cost_ID_FK = UOCost.Unit_ID(+) " & vblf & _
"AND " & _ 
  "inv_Containers.Container_Status_ID_FK = inv_Container_Status.Container_Status_ID(+) " & _    
"AND " & _ 
  "inv_Containers.Container_Type_ID_FK = inv_Container_Types.Container_Type_ID " & _ 
"AND " & _ 
  "inv_Containers.Compound_ID_FK = inv_Compounds.Compound_ID(+) " & _ 
"AND " & _ 
  "inv_Containers.Supplier_ID_FK = inv_Suppliers.Supplier_ID(+) "


'SQL to check for an 'Available' container with the same compound as the one being ordered
SQL2 = "SELECT 1 AS Dupl " & _
		"FROM   inv_containers conta, inv_containers contb " & _
		"WHERE  conta.supplier_catnum = '" & Session("SupplierCatNum") & "' AND " & _
		"       conta.supplier_id_fk = " & Session("SupplierID") & " AND " & _
		"       conta.compound_id_fk = contb.compound_id_fk AND " & _
		"       contb.container_status_id_fk = 1 " & _
		"UNION ALL " & _
		"SELECT 1 AS Dupl " & _
		"FROM   inv_containers, inv_compounds " & _
		"WHERE  TRANSLATE(UPPER(inv_compounds.CAS), " & _ 
		"                 '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-_/ ', " & _
		"                 '0123456789') =  " & _
		"         TRANSLATE(UPPER('" & Session("CAS") & "'),  " & _
		"                   '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-_/ ', " & _
		"                   '0123456789') AND " & _
		"       inv_containers.compound_id_fk = inv_compounds.compound_id AND " & _
		"       inv_containers.container_status_id_fk = 1"

%>