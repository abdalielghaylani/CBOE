
<%
'****************************************************************************************
'*  TYPE: Sub
'*	PURPOSE: build menubar dictionarys                        
'*	INPUT: none- pulls information form menubar.ini file                   			
'*	OUTPUT: Session dictionary containing menus specified in menu.ini file.			
'*  COMMENTS: if menubar.ini is not found in an application config folder then the default configuration is used
'*  this is found at cfserverasp/source/menubar.ini		
'****************************************************************************************


Sub BuildMenuDictionary()
	'check to see if menubar.ini exists in the local application directory
	'!DGB! 02/2003 fixed the path to application level menubar.ini
	temp_path = Application("AppPath") & "\config\menubar.ini"
	fileExists = doFileExists(temp_path)
	if fileExists = true then
		menubar_ini_location = "web_app"
	else
		menubar_ini_location = "chemoffice_menu"
	end if
	MenuItems = GetINIValue( "optional", "GLOBALS", "MENUBAR_ITEMS", menubar_ini_location, "menubar")
	temp_menu_items = split(MenuItems, ",", -1)
	dim MenuBarSettings
	
	Set MenuBarSettings = CreateObject("Scripting.Dictionary")
	MenuBarSettings.Add "BACKGROUND_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "BACKGROUND_COLOR", menubar_ini_location, "menubar")
	onMouseOver = GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SHOW_ON_MOUSEOVER", menubar_ini_location, "menubar")
	if  onMouseOver = "" then
		onMouseOver= "1"
	end if
	MenuBarSettings.Add "SHOW_ON_MOUSEOVER",CBool(onMouseOver)
	MenuBarSettings.Add "SHADOW_ENABLED", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SHADOW_ENABLED", menubar_ini_location, "menubar")
	MenuBarSettings.Add "SHADOW_COLOR", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SHADOW_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "SHADOW_STRENGTH", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SHADOW_STRENGTH", menubar_ini_location, "menubar")
	MenuBarSettings.Add "SHADOW_X_OFFSET", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SHADOW_X_OFFSET", menubar_ini_location, "menubar")
	MenuBarSettings.Add "SHADOW_Y_OFFSET", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SHADOW_Y_OFFSET", menubar_ini_location, "menubar")

	MenuBarSettings.Add "SELECTED_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SELECTED_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "SELECTED_HIGHLIGHT_COLOR", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SELECTED_HIGHLIGHT_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "SELECTED_SHADOW_COLOR", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SELECTED_SHADOW_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "SELECTED_TEXT_COLOR", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SELECTED_TEXT_COLOR", menubar_ini_location, "menubar")
	
	MenuBarSettings.Add "DISABLED_TEXT_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "DISABLED_TEXT_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "INNER_SHADOW_COLOR", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "INNER_SHADOW_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "INNER_HIGHLIGHT_COLOR", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "INNER_HIGHLIGHT_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "OUTER_HIGHLIGHT_COLOR", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "OUTER_HIGHLIGHT_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "OUTER_SHADOW_COLOR", GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "OUTER_SHADOW_COLOR", menubar_ini_location, "menubar")


	
	MenuBarSettings.Add "CLEAR_PIXEL_IMAGE",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "CLEAR_PIXEL_IMAGE", menubar_ini_location, "menubar")
	MenuBarSettings.Add "POPUP_ICON",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "POPUP_ICON", menubar_ini_location, "menubar")
	MenuBarSettings.Add "SELECTED_POPUP_ICON",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "SELECTED_POPUP_ICON", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ABSOLUTE_LEFT",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ABSOLUTE_LEFT", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ABSOLUTE_TOP",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ABSOLUTE_TOP", menubar_ini_location, "menubar")
	MenuBarSettings.Add "HOVER_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "HOVER_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "HOVER_HIGHLIGHT_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "HOVER_HIGHLIGHT_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "HOVER_SHADOW_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "HOVER_SHADOW_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "HOVER_TEXT_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "HOVER_TEXT_COLOR", menubar_ini_location, "menubar")

	MenuBarSettings.Add "MENU_FONT_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_FAMILY",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_FAMILY", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_PADDING_BOTTOM",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_PADDING_BOTTOM", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_PADDING_TOP",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_PADDING_TOP", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_PADDING_LEFT",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_PADDING_LEFT", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_PADDING_RIGHT",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_PADDING_RIGHT", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_SIZE",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_SIZE", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_STYLE",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_STYLE", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_TEXT_DECORATION",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_TEXT_DECORATION", menubar_ini_location, "menubar")
	MenuBarSettings.Add "MENU_FONT_WEIGHT",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "MENU_FONT_WEIGHT", menubar_ini_location, "menubar")


	MenuBarSettings.Add "ITEM_FONT_COLOR",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_COLOR", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ITEM_FONT_FAMILY",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_FAMILY", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ITEM_FONT_PADDING_BOTTOM",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_PADDING_BOTTOM", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ITEM_FONT_PADDING_TOP",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_PADDING_TOP", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ITEM_FONT_PADDING_LEFT",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_PADDING_LEFT", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ITEM_FONT_PADDING_RIGHT",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_PADDING_RIGHT", menubar_ini_location, "menubar")

	MenuBarSettings.Add "ITEM_FONT_SIZE",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_SIZE", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ITEM_FONT_STYLE",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_STYLE", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ITEM_FONT_TEXT_DECORATION",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_TEXT_DECORATION", menubar_ini_location, "menubar")
	MenuBarSettings.Add "ITEM_FONT_WEIGHT",GetINIValue( "optional", "MENU_BAR_DISPLAY_SETTINGS", "ITEM_FONT_WEIGHT", menubar_ini_location, "menubar")

	

	Set MenuBarDict = CreateObject("Scripting.Dictionary")
	' always add menubarsettings as first item in dictionary
	MenuBarDict.Add "MENU_BAR_SETTINGS", MenuBarSettings
	for i = 0 to UBound(temp_menu_items)
		menu_group_name = temp_menu_items(i)
		menu_group_name_display = 	GetINIValue( "optional", temp_menu_items(i), "MENU_NAME", menubar_ini_location, "menubar")
		menu_type =GetINIValue( "optional", temp_menu_items(i), "MENU_TYPE", menubar_ini_location, "menubar")
		
		if menu_type = "SUBMENU" then
			menu_group_name_display = menu_group_name_display & ":" & menu_group_name
		end if
		menu_group_dict = menu_group_name & "_dict"
		dim menu_group_dict
		Set menu_group_dict = CreateObject("Scripting.Dictionary")
		populateMenuItem menubar_ini_location,menu_group_name,menu_group_dict
		MenuBarDict.Add menu_group_name_display, menu_group_dict
	next
	
	Set Session("MenuBar_MENUS") = MenuBarDict
	
	
	
	
End Sub


'****************************************************************************************
'*  TYPE: Sub
'*	PURPOSE: build menu item subdictionary                          
'*	INPUT: menubar_ini_location as string, menu_group_name as string, menu_group_dict as dicationary reference
'*		menubar_ini_location: location on menubar.ini file
'*		menu_group_name: name of menu
'*		menu_group_dict: reference to menu_group_dicationary created in calling routine
'*	OUTPUT: adds menu items to menu group dicationary	
'****************************************************************************************


Sub populateMenuItem(menubar_ini_location,menu_group_name, ByRef menu_group_dict)
	Dim i
	NumMenuItems = GetINIValue( "optional", menu_group_name, "NUM_MENU_ITEMS", menubar_ini_location, "menubar")
	if NumMenuItems <> "" then
		menu_group_dict.Add "MENU_HIDE", GetINIValue( "optional", menu_group_name, "MENU_TYPE", menubar_ini_location, "menubar")
		menu_group_dict.Add "MENU_TYPE", GetINIValue( "optional", menu_group_name, "MENU_HIDE", menubar_ini_location, "menubar")
		menu_group_dict.Add "PARENT_MENU", GetINIValue( "optional", menu_group_name, "PARENT_MENU", menubar_ini_location, "menubar")
		menu_group_dict.Add "MENU_NAME", GetINIValue( "optional", menu_group_name, "MENU_NAME", menubar_ini_location, "menubar")
		menu_group_dict.Add "MENU_FORMGROUPS", GetINIValue( "optional", menu_group_name, "MENU_FORMGROUPS", menubar_ini_location, "menubar")

		for i = 1 to CInt(NumMenuItems)
			dim menu_item_dict
			menu_item_name = GetINIValue( "optional", menu_group_name, "NAME_ITEM_" & i, menubar_ini_location, "menubar")
			menu_item_dict = menu_item_name & "_dict" 
			Set menu_item_dict= CreateObject("Scripting.Dictionary")
			populateOptions menubar_ini_location,menu_group_name,i, menu_item_name, menu_item_dict
			menu_group_dict.Add menu_item_name, menu_item_dict
		
		next
	end if
	

End Sub


'****************************************************************************************
'*  TYPE: Sub
'*	PURPOSE: build menuoptions subdictionary                      
'*	INPUT: menubar_ini_location as string, menu_group_name as string, theItemNumber as number, menu_item_name as string, menu_item_dict as dicationary reference
'*		menubar_ini_location: location on menubar.ini file
'*		menu_group_name: name of menu
'*		theItemNumber: menu item number
'*		menu_item_name: menu item name
'*		menu_item_dict: reference to menu_item_dicationary created in called routine
'*	OUTPUT: adds menu options to menu items dicationary		
'****************************************************************************************

Sub populateOptions(menubar_ini_location,menu_group_name, theItemNumber, menu_item_name, byRef menu_item_dict)

	on error resume next
	menu_item_dict.Add "NAME", GetINIValue( "optional", menu_group_name, "NAME_ITEM_" & theItemNumber, menubar_ini_location, "menubar")
	menu_item_dict.Add "URL", GetINIValue( "optional", menu_group_name, "URL_ITEM_" & theItemNumber, menubar_ini_location, "menubar")
	menu_item_dict.Add "ONCLICK",GetINIValue( "optional", menu_group_name, "ONCLICK_ITEM_" & theItemNumber, menubar_ini_location, "menubar")
	menu_item_dict.Add "DIM", GetINIValue( "optional", menu_group_name, "DIM_ITEM_" & theItemNumber, menubar_ini_location, "menubar")
	menu_item_dict.Add "HIDE", GetINIValue( "optional", menu_group_name, "HIDE_ITEM_" & theItemNumber, menubar_ini_location, "menubar")
	menu_item_dict.Add "HELP", GetINIValue( "optional", menu_group_name, "HELP_ITEM_" & theItemNumber, menubar_ini_location, "menubar")
	menu_item_dict.Add "NAME_ADD_INFO", GetINIValue( "optional", menu_group_name, "NAME_ADD_INFO_ITEM_" & theItemNumber, menubar_ini_location, "menubar")
	menu_item_dict.Add "ICON_IMAGE", GetINIValue( "optional", menu_group_name, "ICON_IMAGE_ITEM_" & theItemNumber, menubar_ini_location, "menubar")
	menu_item_dict.Add "FORMGROUPS", GetINIValue( "optional", menu_group_name, "FORMGROUPS_ITEM_" & theItemNumber, menubar_ini_location, "menubar")

End Sub






%>