<%

Function RenderCustomInventoryMenu()
	'Sample Links
	'------------
	'CustomInventoryMenu = CustomInventoryMenu & "<a class=""MenuLink"" href=""#"">Inventory Link 1</a><br />"
	'CustomInventoryMenu = CustomInventoryMenu & "<a class=""MenuLink"" href=""#"">Inventory Link 2</a><br />"
	
	CustomInventoryMenu = ""
	
	'Custom Reports
	'--------------
	CustomInventoryMenu = CustomInventoryMenu & "<a class=""MenuLink"" href=""Custom Reports"" ONCLICK=""OpenDialog('/cheminv/Gui/CreateReport_frset.asp?isCustomReport=1&ReportTypeID=4', 'ReportDiag', 2); return false;"">Custom Reports</a><br />"
	CustomInventoryMenu = CustomInventoryMenu & "<a class=""MenuLink"" href=""/cheminv/gui/addreport.asp"">Add Report</a><br />"
	CustomInventoryMenu = CustomInventoryMenu & "<a class=""MenuLink"" href=""/cheminv/gui/editreport.asp"">Edit Report</a><br />"
	RenderCustomInventoryMenu = CustomInventoryMenu
End Function

Function RenderCustomSecurityMenu()
	'Sample Links
	'------------
	'CustomSecurityMenu = CustomSecurityMenu & "<a class=""MenuLink"" href=""#"">Security Link 1</a><br />"
	'CustomSecurityMenu = CustomSecurityMenu & "<a class=""MenuLink"" href=""#"">Security Link 2</a><br />"
	CustomSecurityMenu = ""
	
	RenderCustomSecurityMenu = CustomSecurityMenu
End Function

Function RenderCustomContainerMenu()
	'Sample Links
	'------------
	'CustomContainerMenu = CustomContainerMenu & "<a class=""MenuLink"" href=""#"">Container Link 1</a><br />"
	'CustomContainerMenu = CustomContainerMenu & "<a class=""MenuLink"" href=""#"">Container Link 2</a><br />"

	CustomContainerMenu = ""
	
	RenderCustomContainerMenu = CustomContainerMenu
End Function

Function RenderCustomPlateMenu()
	'Sample Links
	'------------
	'CustomPlateMenu = CustomPlateMenu & "<a class=""MenuLink"" href=""#"">Plate Link 1</a><br />"
	'CustomPlateMenu = CustomPlateMenu & "<a class=""MenuLink"" href=""#"">Plate Link 2</a><br />"

	CustomPlateMenu = ""
	
	'Create Plates from Excel and Pipette Log
	'----------------------------------------
	if Application("PLATES_ENABLED") AND Session("INV_CREATE_PLATE" & dbkey) then
		CustomPlateMenu = CustomPlateMenu & "<a class=""MenuLink"" href=""/cheminv/custom/gui/CreatePlatesFromExcel.asp?ReturnURL=menu.asp"">Create Plates from Excel</a><br />"
		CustomPlateMenu = CustomPlateMenu & "<a class=""MenuLink"" HREF=""/cheminv/custom/gui/CreatePlatesFromPipetteLog.asp?ReturnURL=menu.asp"">Create Plates from Pipette Log</a><br />"
	end if

	RenderCustomPlateMenu = CustomPlateMenu
End Function

%>