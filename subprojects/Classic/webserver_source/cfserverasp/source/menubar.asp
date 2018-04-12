
<%

Dim objWM
InitializeMenuBar request("formmode"),request("dbname"), request("formgroup")

'Call menu bar with exclude list for standard buttons
'Add parameter for additional custom
'-------------------------------------------------------------------------------
' Name: LogAction(inputstr)
' Type: Sub
' Purpose:  writes imformation to a output file 
' Inputs:   inputstr  as string - variable to output
' Returns:	none
' Comments: writes informtion to /inetput/cfwlog.txt file
'-------------------------------------------------------------------------------
Sub LogAction(ByVal inputstr)
		on error goto 0
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub



Sub InitializeMenuBar(formmode,dbkey, formgroup)
	dim objGroup
	dim objItem
	set objWM = Server.CreateObject("Coalesys.CSWebMenu.6")
	set objWM = buildMenuBarMenus(formmode,dbkey, formgroup)
	'JHS add key for webmenu activation
	objWM.UserData = "Andrew Jackson:PerkinElmer:1447080201"
	
	%><style type="text/css">
			<%=objWM.GenerateStyleSheet%>
			<%=Session("button_StyleSheet")%>
	</style>
	<script language="JavaScript" type="text/javascript">
			<%=objWM.GenerateJavaScript(0)%>
	</script>
	
	<script language = "javascript">
		onLoad="<%=objWM.GenerateOnLoadEvent%>"
		onResize="<%=objWM.GenerateOnResizeEvent%>"
	</script><%
end sub

Sub buildMenuBar(formmode,dbkey, formgroup)
	%>
	<table border = "0" cellspacing="0" cellpadding = "0" ID="Table1"><td>
	<%= objWM.GenerateWebMenu(0)%>
	
	<%=Session("menubar_buttons") %>
	</td></table>
<%end sub




Function buildMenuBarMenus(formmode,dbkey, formgroup)
	dim objWM
	dim objGroup
	dim objItem

	set objWM = Server.CreateObject("Coalesys.CSWebMenu.6")
	user_agent = Request.ServerVariables("HTTP_USER_AGENT")
	objWM.UserAgent = user_agent
	Set menu_groups = Server.CreateObject("scripting.dictionary")
	on error resume next
	Set menu_groups = Session("Menubar_MENUS")
	if err.number <> 0 then
		BuildMenuDictionary()
		Set menu_groups = Session("Menubar_MENUS")
	end if
	
	objWM.BackgroundColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("BACKGROUND_COLOR")
	objWM.ClearPixelImage=menu_groups.Item("MENU_BAR_SETTINGS").Item("CLEAR_PIXEL_IMAGE")
	objWM.PopupIcon=menu_groups.Item("MENU_BAR_SETTINGS").Item("POPUP_ICON")
	objWM.SelectedPopupIcon=menu_groups.Item("MENU_BAR_SETTINGS").Item("SELECTED_POPUP_ICON")
	objWM.DisabledTextColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("DISABLED_TEXT_COLOR")
	objWM.InnerHighlightColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("INNER_SHADOW_COLOR")
	objWM.InnerShadowColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("INNER_HIGHLIGHT_COLOR")
	objWM.OuterHighlightColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("OUTER_HIGHLIGHT_COLOR")
	objWM.OuterShadowColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("OUTER_SHADOW_COLOR")
	objWM.SelectedColor= menu_groups.Item("MENU_BAR_SETTINGS").Item("SELECTED_COLOR")
	objWM.ShadowColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("SHADOW_COLOR")
	objWM.ShadowEnabled=menu_groups.Item("MENU_BAR_SETTINGS").Item("SHADOW_ENABLED")
	if menu_groups.Item("MENU_BAR_SETTINGS").Item("SHADOW_STRENGTH")>0 then
		objWM.ShadowStrength =menu_groups.Item("MENU_BAR_SETTINGS").Item("SHADOW_STRENGTH")
	end if
	if menu_groups.Item("MENU_BAR_SETTINGS").Item("SHADOW_X_OFFSET")>0 then
		objWM.ShadowXOffset=menu_groups.Item("MENU_BAR_SETTINGS").Item("SHADOW_X_OFFSET")
	end if
	if menu_groups.Item("MENU_BAR_SETTINGS").Item("SHADOW_Y_OFFSET")>0 then
		objWM.ShadowYOffset=menu_groups.Item("MENU_BAR_SETTINGS").Item("SHADOW_Y_OFFSET")
	end if
	
	if Not(inStr(UCase(user_agent), "NETSCAPE")> 0) then
	
		objWM.MenuBar.AbsoluteDockEnabled=false
		objWM.MenuBar.AbsoluteDockState = 1
		objWM.OverlayIEObjects = 1
	else
		objWM.MenuBar.AbsoluteDockEnabled=false
		objWM.MenuBar.AbsoluteDockState = 0
		objWM.OverlayIEObjects =0
	end if

	objWM.MenuBar.AbsoluteDragEnabled = 0
	objWM.MenuBar.AbsoluteLeft=menu_groups.Item("MENU_BAR_SETTINGS").Item("ABSOLUTE_LEFT")
	objWM.MenuBar.AbsoluteTop=menu_groups.Item("MENU_BAR_SETTINGS").Item("ABSOLUTE_TOP")
	objWM.MenuBar.DisplayMode=1
	
	BACKGROUND_COLOR = menu_groups.Item("MENU_BAR_SETTINGS").Item("BACKGROUND_COLOR")
	SELECTED_COLOR = menu_groups.Item("MENU_BAR_SETTINGS").Item("SELECTED_COLOR")
	SELECTED_TEXT_COLOR= menu_groups.Item("MENU_BAR_SETTINGS").Item("SELECTED_TEXT_COLOR")
	objWM.MenuBar.BackgroundColor=BACKGROUND_COLOR
	objWM.MenuBar.ShowOnMouseOver= menu_groups.Item("MENU_BAR_SETTINGS").Item("SHOW_ON_MOUSEOVER")
	objWM.MenuBar.HoverColor=HOVER_COLOR
	objWM.MenuBar.HoverHighlightColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("HOVER_HIGHLIGHT_COLOR")
	objWM.MenuBar.HoverShadowColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("HOVER_SHADOW_COLOR")
	objWM.MenuBar.HoverTextColor=menu_groups.Item("MENU_BAR_SETTINGS").Item("HOVER_TEXT_COLOR")
	objWM.MenuBar.SelectedColor = menu_groups.Item("MENU_BAR_SETTINGS").Item("SELECTED_COLOR")
	objWM.MenuBar.SelectedHighlightColor = menu_groups.Item("MENU_BAR_SETTINGS").Item("SELECTED_HIGHLIGHT_COLOR")
	objWM.MenuBar.SelectedShadowColor = menu_groups.Item("MENU_BAR_SETTINGS").Item(SELECTED_SHADOW_COLOR)
	objWM.MenuBar.SelectedTextColor = menu_groups.Item("MENU_BAR_SETTINGS").Item("SELECTED_TEXT_COLOR")
	objWM.MenuBar.BorderSize="0"
	objWM.MenuBar.InnerHighlightColor = menu_groups.Item("MENU_BAR_SETTINGS").Item("INNER_HIGHLIGHT_COLOR")
	objWM.MenuBar.InnerShadowColor = menu_groups.Item("MENU_BAR_SETTINGS").Item("INNER_SHADOW_COLOR")
	objWM.MenuBar.OuterHighlightColor = menu_groups.Item("MENU_BAR_SETTINGS").Item("OUTER_SHADOW_COLOR")
	objWM.MenuBar.OuterShadowColor = menu_groups.Item("MENU_BAR_SETTINGS").Item("OUTER_SHADOW_COLOR")
	
	'font properties
	MENU_FONT_COLOR = menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_COLOR")
	MENU_FONT_FAMILY =  menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_FAMILY")
	MENU_FONT_PADDING_BOTTOM= menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_PADDING_BOTTOM")
	MENU_FONT_PADDING_TOP= menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_PADDING_TOP")
	MENU_FONT_PADDING_LEFT= menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_PADDING_LEFT")
	MENU_FONT_PADDING_RIGHT= menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_PADDING_RIGHT")
	MENU_FONT_SIZE= menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_SIZE")
	MENU_FONT_STYLE= menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_STYLE")
	MENU_FONT_TEXT_DECORATION= menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_TEXT_DECORATION")
	MENU_FONT_WEIGHT = menu_groups.Item("MENU_BAR_SETTINGS").Item("MENU_FONT_WEIGHT")


	ITEM_FONT_COLOR =  menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_COLOR")
	ITEM_FONT_FAMILY =  menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_FAMILY")
	ITEM_FONT_PADDING_BOTTOM= menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_PADDING_BOTTOM")
	ITEM_FONT_PADDING_TOP= menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_PADDING_TOP")
	ITEM_FONT_PADDING_LEFT= menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_PADDING_LEFT")
	ITEM_FONT_PADDING_RIGHT= menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_PADDING_RIGHT")
	ITEM_FONT_SIZE= menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_SIZE")
	ITEM_FONT_STYLE= menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_STYLE")
	ITEM_FONT_TEXT_DECORATION= menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_TEXT_DECORATION")
	ITEM_FONT_WEIGHT =menu_groups.Item("MENU_BAR_SETTINGS").Item("ITEM_FONT_WEIGHT")
	
	

	a = menu_groups.Keys
	'start at 1 so that menubar_setting is not added as a menu item
	For i = 1 to menu_groups.Count -1
		on error resume next
			dim menu_hide
			set menu_items = Server.CreateObject("scripting.dictionary")
			set menu_items = menu_groups.Item(a(i))
			c = menu_items.Keys
			
			menu_name = Trim(menu_items.Item(c(3)))
			menu_hide = menu_items.Item(c(1))
			menu_formgroups = Trim(menu_items.Item(c(4)))
			
			if menu_hide <> "" then
				menu_hide = eval(menu_hide)
				if err.number <> 0 then
					logaction("menubar MENU_HIDE function call error: " &  menu_items.Item(c(1))& ": " & err.number & err.description)
					err.Clear()
				end if
				if menu_hide = true then 
					show_menu = false
				else
					show_menu = true
					menu_hide = false
				end if
			else
				menu_hide = false
				show_menu = true
			end if
			
			'Override show_menu if formgroup limitation is specified
			if menu_formgroups <> "" then
				show_formgroups = menu_formgroups
				temp = split(show_formgroups, ",", -1)
				show_menu_this_formgroup=false
				for f=0 to UBound(temp)
					if UCase(formgroup) = UCase(temp(f)) then
						show_menu_this_formgroup = true
						exit for
					end if
				next
			else
				show_menu_this_formgroup = true
			end if
			if show_menu = true and show_menu_this_formgroup = false then
				show_menu = false
			end if
			
			menu_type = menu_items.Item(c(0))
			select case UCase(menu_type)
				case "MENU"
					set objItemFont = objWM.ItemFont
					set objButtonFont = objWM.ButtonFont
				
					
					objItemFont.Color = ITEM_FONT_COLOR
					objItemFont.Family = ITEM_FONT_FAMILY
					objItemFont.PaddingBottom= ITEM_FONT_PADDING_BOTTOM
					objItemFont.PaddingTop = ITEM_FONT_PADDING_TOP
					objItemFont.PaddingLeft = ITEM_FONT_PADDING_LEFT
					objItemFont.PaddingRight = ITEM_FONT_PADDING_RIGHT
					'LJB 8/16/200 adjust menu font size for mac IE browser
					If detectIEMac() = true then
						objItemFont.Size = ITEM_FONT_SIZE  + 1
					else
						objItemFont.Size = ITEM_FONT_SIZE 
					end if
					
					objItemFont.Style = ITEM_FONT_STYLE
					
					'take the defaults for all padding.
					objButtonFont.Color = MENU_FONT_COLOR
					objButtonFont.Family = MENU_FONT_FAMILY
					objButtonFont.PaddingBottom= MENU_FONT_PADDING_BOTTOM
					objButtonFont.PaddingTop = MENU_FONT_PADDING_TOP
					objButtonFont.PaddingLeft = MENU_FONT_PADDING_LEFT
					objButtonFont.PaddingRight = MENU_FONT_PADDING_RIGHT
					
					'LJB 8/16/200 adjust menu font size for mac IE browser
					If detectIEMac() = true then
						objButtonFont.Size = MENU_FONT_SIZE  + 1
					else
						objButtonFont.Size = MENU_FONT_SIZE
					end if
					objButtonFont.Style = MENU_FONT_STYLE
					
				
					
					set objGroup = objWM.Groups.Add()
					If lcase(menu_name) = "marked hits" then
						objGroup.Caption = menu_name & " " & getMarkedCount(dbkey, formgroup)
					else
					objGroup.Caption = menu_name
					end if
					objGroup.ShowMenuBarItem = show_menu
				case "SUBMENU"
					parent_menu =  Trim(menu_items.Item(c(2)))
					temp_sub_menu_name =  split(menu_name, ":", -1)
					sub_menu_name = Trim(temp_sub_menu_name(0))
					set objGroup = objWM.Groups(parent_menu).items(sub_menu_name).AddGroup()
					objGroup.ShowMenuBarItem = show_menu
				case "BUTTON"
					
			end select
			
			
				for j = 5 to  menu_items.Count -1
					menu_item =  c(j)
					
						err.Clear()
						if menu_items.Item(c(j)).Item("HIDE") <> "" then
							hideItem =  eval(menu_items.Item(c(j)).Item("HIDE"))
							if err.number <> 0 then
								logaction("menubar HIDE function call error: " & menu_items.Item(c(j)).Item("HIDE")& ": " & err.number & err.description)
								err.Clear()
							end if
						else 
							hideItem = false
						end if
						if menu_items.Item(c(j)).Item("DIM") <> "" then
							dimItem =  eval(menu_items.Item(c(j)).Item("DIM"))
							
							if err.number <> 0 then
								logaction("menubar DIM function call error: " & menu_items.Item(c(j)).Item("DIM")& ": " & err.number & err.description)
								err.Clear()
							end if
						else
							dimItem = false
						end if
						if menu_items.Item(c(j)).Item("NAME_ADD_INFO") <> "" then
							addinfo = eval(menu_items.Item(c(j)).Item("NAME_ADD_INFO"))
							
							if err.number <> 0 then
								logaction("menubar NAME_ADD_INFO function call error: " & menu_items.Item(c(j)).Item("NAME_ADD_INFO") & ": " & err.number & err.description)
								err.Clear()
							end if
						else
							addinfo = ""
						end if
						if err.number <> 0 then
							logaction("menubar NAME_ADD_INFO function call error: " & menu_items.Item(c(j)).Item("NAME_ADD_INFO") & ": " & err.number & err.description)
							err.Clear()
						end if
						
						'Override hide item if formgroup limitation is specified
						if menu_items.Item(c(j)).Item("FORMGROUPS") <> "" then
							
							show_formgroups = menu_items.Item(c(j)).Item("FORMGROUPS")
							
							temp = split(show_formgroups, ",", -1)
							show_item_this_formgroup=false
							for f=0 to UBound(temp)
								if UCase(formgroup) = UCase(temp(f)) then
									show_item_this_formgroup = true
									exit for
								end if
							next
						else
							show_item_this_formgroup = true
						end if
						
						if hideItem = false and show_item_this_formgroup = false then
							hideItem = true
						end if
						if hideItem = false then
							
							if inStr( menu_items.Item(c(j)).Item("NAME"), ":")> 0 then
								temp=split( menu_items.Item(c(j)).Item("NAME"), ":", -1)
								item_name = Trim(temp(0))
							else
								item_name = Trim(menu_items.Item(c(j)).Item("NAME"))
							end if
							Select Case UCase(menu_type)
								Case "MENU", "SUBMENU"
									set objItem = objGroup.Items.Add()
									if dimItem = false then
										if addinfo <> "" then
											objItem.Caption = item_name & " " & addinfo & " "
										else
											objItem.Caption = item_name
										end if
										objItem.OnClick = menu_items.Item(c(j)).Item("ONCLICK")
										objItem.IconImage = menu_items.Item(c(j)).Item("ICON_IMAGE")
										if  menu_items.Item(c(j)).Item("URL") <> "" then
											'must overwrite onClick since.URL always sends to navbar farme
											objItem.OnClick= "window.open(&quot;" & menu_items.Item(c(j)).Item("URL")& "&quot;)"
										end if
										
										objItem.StatusBar = menu_items.Item(c(j)).Item("HELP")
									else
											
										objItem.Caption = item_name
										objItem.Enabled = false
										objItem.OnClick = ""
										objItem.URL= ""
										objItem.StatusBar = "item not available"
									end if
								Case "BUTTON"
								
									if menu_hide = false then
									
										if dimItem = false then
											if addinfo <> "" then
												buttonName = menu_items.Item(c(j)).Item("NAME") & " " & addinfo & ""
											else
												buttonName = menu_items.Item(c(j)).Item("NAME")
											end if
											
											selectable=""
											button_OnClick = " onClick=""" & menu_items.Item(c(j)).Item("ONCLICK") & ";"""
											BackGroundColor = " bgcolor= " & menu_groups.Item("MENU_BAR_SETTINGS").Item("BACKGROUNDCOLOR") & " " 
											button_HREF =  menu_items.Item(c(j)).Item("URL")
											if button_HREF = "" then
												button_HREF = " href=# "
											else
												'overwrite onClick event
													button_HREF =  " onClick=""window.open(&quot;" & button_HREF & "&quot;)"";"
											end if
											statusbar =" onmouseover="" status='" & menu_items.Item(c(j)).Item("HELP") & "'"""  & " "
						
										else
											
											buttonName = menu_items.Item(c(j)).Item("NAME")
											selectable = " unselectable=""on"" "
											button_OnClick   = ""
											button_HREF = ""
											statusbar = "item not available"
											
										end if
									
									'LJB Notes 10/27 Must define base "A" style before .buttonbar. if not then some parts won't work in IE5. 
									'At this time I'm unable to match the hover/active state of the coalyss buttons. when I add padding
									'the buttons move and if I try the borders they do not work.
									
									Session("button_StyleSheet") = "<STYLE> " &_
									" A:link { color:" & MENU_FONT_COLOR & " }" &_
									" A:visited { color:" & MENU_FONT_COLOR & "} " &_
									" A:active { color:" & BACKGROUND_COLOR & "}" &_ 
									" A:hover { color:" & SELECTED_TEXT_COLOR & "} " &_
                                   
                                    " .ButtonBar {color: " & MENU_FONT_COLOR & "; font-size:" & MENU_FONT_SIZE & "; font-weight:" & MENU_FONT_WEIGHT & "; font-size:" & MENU_FONT_SIZE & "; font-family: " & MENU_FONT_FAMILY &  "}" &_
                                    " A.ButtonBar:link {Text-Decoration: none; color:" & MENU_FONT_COLOR & "; font-weight:" & MENUFONT_WEIGHT & "; font-size:" & MENU_FONT_SIZE & "; font-family: " & MENU_FONT_FAMILY &  "}" &_
                                    " A.ButtonBar:visited {Text-Decoration: none; color:" & MENU_FONT_COLOR & "; font-weight:" & MENU_FONT_WEIGHT & "; font-size:" & MENU_FONT_SIZE & "; font-family: " & MENU_FONT_FAMILY &  "}" &_
									" A.ButtonBar:active {Text-Decoration: none;background-color: " & SELECTED_TEXT_COLOR & ";color:" & MENU_FONT_COLOR & "; font-weight:" & MENU_FONT_WEIGHT & "; font-size:" & MENU_FONT_SIZE & "; font-family: " & MENU_FONT_FAMILY &  "}" &_
                                    " A.ButtonBar:hover {Text-Decoration: none;background-color: " & SELECTED_TEXT_COLOR & ";color:" & MENU_FONT_COLOR & "; font-weight:" & MENU_FONT_WEIGHT & "; font-size:" & MENU_FONT_SIZE & "; font-family: " & MENU_FONT_FAMILY &  "}" &_
									".cswmButton {background-color:#CCCCCC}" &_
									" </STYLE> "
										

										buttonTD = "<td class=""cswmButton""  nowrap style="" border-width:0px; padding-top:" & MENU_FONT_PADDING_TOP & ";padding-bottom:" & MENU_FONT_PADDING_BOTTOM & ";padding-left:" & MENU_FONT_PADDING_LEFT & ";padding-right:" & MENU_FONT_PADDING_RIGHT & ";"" ><a " & button_HREF & button_OnClick  & " class=""ButtonBar"">" &  buttonName & "</a></td>"
										if buttonTD_List<> "" then
											buttonTD_List = buttonTD_List & buttonTD
										else
											buttonTD_List =  buttonTD
										end if
										Session("menubar_buttons") = buttonTD_List
									end if
							end select
						end if
				
					
				next
			
		
	next
	
	Set buildMenuBarMenus = objWM

End Function

'*****************************Dim and Hide functions for standard Cows menus and items**************************

function hideHomeBtn()
	Dim hideHome 
	CookieValue = Request.Cookies("CS_SEC_UserName")
	if Len(CookieValue) > 0 then
		hideHome = false
	else
		hideHome = true
	end if
	hideHomeBtn =  hideHome
end function

	
function dimClearMarkedHitsItem(dbkey, formgroup)
	dim lastListShown 
	dim hitsMarked
	dim thereturn
	'lastListShown = dimShowLastListItem(dbkey, formgroup)
	hitsMarked = dimShowMarkedHitsItem(dbkey, formgroup)
	if hitsMarked = false  then
		thereturn= false
	else
		thereturn= true
	end if
dimClearMarkedHitsItem = thereturn
End Function


function dimShowMarkedHitsItem(dbkey, formgroup)
	dim markedHitsShown
	dim thereturn
	dim theArray 
	dim hasValues
	on error resume next
	markedHitsShown = Session("MarkedHits" & dbkey & formgroup)
	
	if markedHitsShown = -1 then
		markedHitsShown = ""
	end if
	if Not markedHitsShown = "" then
		thereturn =  false
	else
	
		thereturn = true
	end if
	
	dimShowMarkedHitsItem = thereturn
End Function 



function dimRestoreHitListHistoryItem(dbkey, formgroup)

	dim HistoryExists
	dim thereturn
	dim theArray 
	dim hasValues
	on error resume next
	theArray=Application("form_group_array" & dbkey)
	
	if isArray(theArray) and CBool(Application("ALLOW_HITILIST_MNGMNT_FG_SELECTOR"))=true then
		HistoryExists=""
		hasValues = ""
		for i = 0 to UBound(theArray)
			if Application("bypass_ini")=true then
			
			
				if (Application("FORMGROUP_IS_PUBLIC" & dbkey &  theArray(i))="Y")  or (UCase(Application("FORMGROUP_OWNER" & dbkey &  theArray(i))) = UCase(session("username" & dbkey))) then
					hasValues = Session("HitListHistoryExists" & dbkey &  theArray(i))
				end if
			else
				hasValues = Session("HitListHistoryExists" & dbkey &  theArray(i))
			end if
			if hasValues <> "" then
				HistoryExists = "true"
				exit for
			end if
		next
	else
		HistoryExists = Session("HitListHistoryExists" & dbkey & formgroup)
	end if
	if HistoryExists  <> "" then
		thereturn =  false
	else
		thereturn = true
	end if
	
	dimRestoreHitListHistoryItem = thereturn
End Function 

function dimRestoreHitListItem(dbkey, formgroup)
	dim HistLists 
	dim thereturn
	
	dim theArray 
	dim hasValues
	on error resume next
	theArray=Application("form_group_array" & dbkey)
	
	if isArray(theArray) and CBool(Application("ALLOW_HITILIST_MNGMNT_FG_SELECTOR"))=true then
		hasValues = ""
		HistLists=""
		for i = 0 to UBound(theArray)

			if Application("bypass_ini") = true then
				if (Application("FORMGROUP_IS_PUBLIC" & dbkey &  theArray(i))="Y")  or (UCase(Application("FORMGROUP_OWNER" & dbkey &  theArray(i))) = UCase(session("username" & dbkey))) then
					hasValues = Session("HitListExists" & dbkey &  theArray(i))
				end if
			else
				hasValues = Session("HitListExists" & dbkey &  theArray(i))
			end if
			if hasValues <> "" then
				HistLists = "true"
				exit for
			end if
		next
	else
		HistLists = Session("HitListExists" & dbkey & formgroup)
	end if
	if HistLists = "" then
		thereturn =  true
	else
		thereturn = false
	end if
	
	dimRestoreHitListItem = thereturn
End Function 


function dimRestoreLastHitlistItem(dbkey, formgroup)
	dim HistLists 
	dim thereturn
	dim theArray 

	
	if Application("bypass_ini") = true then
		if Not (UCase(formgroup)="BASE_FORM_GROUP") then
			HistLists = Session("HitListHistoryExists" & dbkey &  formgroup)
		end if
	else
		HistLists = Session("HitListHistoryExists" & dbkey &  formgroup)
	end if
	
	if HistLists = ""  then
		thereturn = true
	else
		thereturn = false
	end if
	dimRestoreLastHitlistItem = thereturn
End Function 

function dimRestoreQueryItem(dbkey, formgroup)
	dim QueriesExists
	dim thereturn
	dim theArray 
	dim hasValues
	on error resume next
	theArray=Application("form_group_array" & dbkey)
	if isArray(theArray) and CBool(Application("ALLOW_QUERY_MNGMNT_FG_SELECTOR"))=true then
		hasValues = ""
		QueriesExists=""
		for i = 0 to UBound(theArray)
			if Application("bypass_ini") = true then
				if (Application("FORMGROUP_IS_PUBLIC" & dbkey &  theArray(i))="Y")  or (UCase(Application("FORMGROUP_OWNER" & dbkey &  theArray(i))) = UCase(session("username" & dbkey))) then
					hasValues = Session("SavedQueriesExists" & dbkey &  theArray(i))
				end if
			else
				hasValues = Session("SavedQueriesExists" & dbkey &  theArray(i))
			end if
			if hasValues <> "" then
				QueriesExists = "true"
				exit for
			end if
		next
	else
		QueriesExists = Session("SavedQueriesExists" & dbkey & formgroup)
	end if
	
	if QueriesExists  <> ""  then
		thereturn =  false
	else
		thereturn = true
	end if
	
	dimRestoreQueryItem = thereturn
End Function 

function dimRestoreLastQueryItem(dbkey, formgroup)
	dim HistoryExists
	dim thereturn
	dim theArray 
	if Application("bypass_ini") = true then
		if Not (UCase(formgroup)="BASE_FORM_GROUP") then
			HistoryExists = Session("QueryHistoryExists" & dbkey &  formgroup)
		end if
	else
		HistoryExists = Session("QueryHistoryExists" & dbkey &  formgroup)
	end if
	
	if HistoryExists  <> "" then
		thereturn =  false
	else
		thereturn = true
	end if
	dimRestoreLastQueryItem = thereturn
End Function 

function dimRestoreQueryHistoryItem(dbkey, formgroup)
	dim HistoryExists
	dim thereturn

	dim theArray 
	dim hasValues
	on error resume next
	theArray=Application("form_group_array" & dbkey)
	if isArray(theArray) and CBool(Application("ALLOW_QUERY_MNGMNT_FG_SELECTOR"))=true then
		hasValues = ""
		HistoryExists = ""
		for i = 0 to UBound(theArray)
			if Application("bypass_ini") = true then
			
				if (Application("FORMGROUP_IS_PUBLIC" & dbkey &  theArray(i))="Y")  or (UCase(Application("FORMGROUP_OWNER" & dbkey &  theArray(i))) = UCase(session("username" & dbkey))) then
					hasValues = Session("QueryHistoryExists" & dbkey &  theArray(i))
				end if
			else
				hasValues = Session("QueryHistoryExists" & dbkey &  theArray(i))
			end if
			if hasValues <> "" then
				HistoryExists = "true"
				exit for
			end if
		next
	else
		HistoryExists = Session("QueryHistoryExists" & dbkey & formgroup)
	end if
	
	if HistoryExists  <> "" then
		thereturn =  false
	else
		thereturn = true
	end if
	
	dimRestoreQueryHistoryItem = thereturn
End Function 


Function getMarkedCount(dbkey, formgroup)
	
	if Session("MarkedHits" & dbkey & formgroup) <> "" then
		temp = split(Session("MarkedHits" & dbkey & formgroup), ",", -1)
		count = UBound(temp)+1
		getMarkedCount = "(" & count & ")"
	else
		getMarkedCount = ""
	end if
	
End Function

function hideQueriesMenu(dbkey, formgroup)
	dim thereturn
	if Application("ALLOW_QUERY_MANAGEMENT")=1 then
		thereturn = false
	else
		thereturn = true
	end if
	hideQueriesMenu = thereturn
End function 

function hideHitListsMenu(dbkey, formgroup)
	dim thereturn
	if CBool(Application("ALLOW_HITLIST_MANAGEMENT")) = true then
		thereturn = false
	else
		thereturn = true
	end if

	hideHitListsMenu = thereturn
End function 

function hideHistoryMenu(dbkey, formgroup)
	dim thereturn
	if (CBool(Application("ALLOW_QUERY_MANAGEMENT")) = false) AND (CBool(Application("ALLOW_HITLIST_MANAGMENT")) = false) or (Application("EXPIRE_HITLIST_HISTORY_DAYS")= 0 AND Application("EXPIRE_QUERY_HISTORY_DAYS")= 0 )then
		thereturn = true
	else
		thereturn = false
	end if

	hideHistoryMenu = thereturn
End function 

function hideRestoreHitlistHistoryMenu(dbkey, formgroup)
	dim thereturn
	if Application("Expire_hitlist_history_days") = 0 then
		thereturn = true
	else
		thereturn = false
	end if

	hideRestoreHitlistHistoryMenu = thereturn
End function 

function hideRestoreQueryHistoryMenu(dbkey, formgroup)
	dim thereturn
	if Application("Expire_Query_history_days") = 0 then
		thereturn = true
	else
		thereturn = false
	end if

	hideRestoreQueryHistoryMenu = thereturn
End function 

function dimExportHits(dbkey, formgroup)
'-- CSBR ID:101924
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: to enable/disable SDF Export
'-- Date: 31/03/2011
	if Application("ENABLE_SDF_EXPORT") =  "INIEmpty" or  Application("ENABLE_SDF_EXPORT") = "NULL" or Application("ENABLE_SDF_EXPORT") = NULL or Application("ENABLE_SDF_EXPORT") =  "" then 
		Application("ENABLE_SDF_EXPORT")=1
	end if
	if  (UCase(request("formmode")) = "EDIT" or UCase(request("formmode")) = "LIST") AND Application("ENABLE_SDF_EXPORT") = 1 then
'-- End of Change #101924#
		thereturn = false
	else
		thereturn = true
	end if 
	dimExportHits = thereturn
end function

function dimSaveQuery(dbkey, formgroup)
	if  UCase(request("formmode")) = "EDIT" or UCase(request("formmode")) = "LIST" then
		thereturn = false
	else
		thereturn = true
	end if 
	dimSaveQuery = thereturn
end function

function dimSaveHitlist(dbkey, formgroup)
	if  UCase(request("formmode")) = "EDIT" or UCase(request("formmode")) = "LIST" then
		thereturn = false
	else
		thereturn = true
	end if 
	dimSaveHitlist = thereturn
end function

function dimSendMarkedHits(dbkey, formgroup)
	if Session("MarkedHits" & dbkey & formgroup) <> "" then
		theReturn = false
	else
		theReturn = true
	end if
	dimSendMarkedHits = thereturn
end function

function hideSendMarkedHits(dbkey, formgroup)
	formgroups = Application("POST_MARKED_SUPPORTED_FORMGROUPS")
	bShow = false
	if formgroups <> "" then
		formgroup_array = split(formgroups, ",", -1)
		for i = 0 to Ubound(formgroup_array)
			if formgroup_array(i) = formgroup then
				bShow = true
				exit for
			end if
		next
	end if
	if bShow = true and Application("POST_MARKED_HITS_PAGE") <> "" and Application("POST_MARKED_SEND_TO_PAGE") <> "" then
		theReturn = false
	else
		theReturn = true
	end if
	
	hideSendMarkedHits = thereturn
end function

function getSendMarkedName(dbkey, formgroup)
	getSendMarkedName= Application("POST_MARKED_HITS_MENU_NAME") 
end function


 %>