<%

' SQL to get attributes of a container needed for reorder
SQL = "SELECT " & _ 
		"inv_Containers.Container_ID AS Container_ID,  " & _
		"inv_Containers.Container_Name AS Container_Name, " & _
		"inv_Containers.Container_Comments AS Container_Comments, " & _
	    "inv_Containers.Location_ID_FK,  " & _
		"inv_Containers.Owner_ID_FK, " & _ 
		"inv_Containers.Current_User_ID_FK, " & _ 
	    "inv_Compounds.Reg_ID_FK,  " & _ 
	    "inv_Container_Order.Project_No, " & _
	    "inv_Container_Order.Job_No " & _
	"FROM " & _ 
		"inv_Containers, " & Application("CHEMINV_USERNAME") & ".inv_Container_Order, inv_Compounds  " & _
	"WHERE  " & _ 
		"inv_Containers.Container_ID = inv_Container_Order.Container_ID(+)  " &_
		"AND inv_Containers.Compound_ID_FK = inv_Compounds.Compound_ID(+) "

'SQL to check for an 'Available' container with the same compound as the one being re-ordered
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