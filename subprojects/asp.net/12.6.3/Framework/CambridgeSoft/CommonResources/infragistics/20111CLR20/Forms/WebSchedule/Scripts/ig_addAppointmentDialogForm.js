/*
* ig_addAppointmentDialogForm.js
* Version 11.1.20111.1006
* Copyright(c) 2001-2011 Infragistics, Inc. All Rights Reserved.
*/







var dialogInterface = null;
if(this.opener != null)
{
	dialogInterface = ig_CreateAddAppointmentDialog(this);
}
			
function AppointmentAddLoad()
{
	if(dialogInterface != null)
	{
		SetupUI();
		InitializeValues();
	}
}


var startDateChooser = null;
var endDateChooser = null;
var startTimeDropDown = null;
var endTimeDropDown = null;
var startDateId = null;
var endDateId = null;
var calDifference = null;

function SetupUI()
{    
	var ids = [':_ctl0:_ctl0:','$xctl0$ctl00$','xxctl0xctl00x','$_ctl0$ctl00$','__ctl0_ctl00_'];
	var i = -1;
	while(!startDateChooser && ++i < ids.length)
	{
		startDateId = 'UltraWebTab1' + ids[i] + 'wdcStartTime';
		startDateChooser = igdrp_getComboById(startDateId);
	}
	if(!startDateChooser)
	{
		alert(AppointmentDialogStrings[0]);
		return;
	}
	
	endDateId = 'UltraWebTab1' + ids[i] + 'wdcEndTime';
	endDateChooser = igdrp_getComboById(endDateId);
	
	if(navigator.userAgent.toLowerCase().indexOf("mozilla")>=0)
	{
		if(navigator.userAgent.toLowerCase().indexOf("firefox")>=0)
		{
			if(navigator.userAgent.indexOf("1.5") >= 0)
			{
				var parentstartDateElem = document.getElementById(startDateChooser.Element.id+"_input").parentNode;
				parentstartDateElem.style.width="0px";
				parentstartDateElem = parentstartDateElem.parentNode.parentNode.parentNode;
				parentstartDateElem.style.width="0px";
				var parentEndDateElem = document.getElementById(endDateChooser.Element.id+"_input").parentNode;
				parentEndDateElem.style.width="0px";
				parentEndDateElem = parentEndDateElem.parentNode.parentNode.parentNode;
				parentEndDateElem.style.width="0px";
			}
		}
	}
	
	
	try
	{
		startTimeDropDown = oUltraWebTab1__ctl0__ctl0_ddStartTime;
		endTimeDropDown = oUltraWebTab1__ctl0__ctl0_ddEndTime;
	}
	catch(e)
	{
		startTimeDropDown = oUltraWebTab1__ctl0_ctl00_ddStartTime;
		endTimeDropDown = oUltraWebTab1__ctl0_ctl00_ddEndTime;
	}
	
	startTimeDropDown._input.onblur = startTime_onBlur;
	startTimeDropDown._input.onfocus = startTime_onFocus;
	startTimeDropDown._elem.dropDownEvent = "startTime_DropDown";
	startTimeDropDown._elem.itemSelect = "startTime_itemSelect";

	endTimeDropDown._input.onblur = endTime_onBlur;
	endTimeDropDown._elem.dropDownEvent = "endTime_DropDown";
	endTimeDropDown._elem.itemSelect = "endTime_itemSelect";
		
	
	
	var startDatelabel508 = document.getElementById("UltraWebTab1__ctl0__ctl0_startdateLabel508");
	var startTimeLabel508 = document.getElementById("UltraWebTab1__ctl0__ctl0_startTimeLabel508");
	var endDateLabel508 = document.getElementById("UltraWebTab1__ctl0__ctl0_endDateLabel508");
	var endTimeLabel508 = document.getElementById("UltraWebTab1__ctl0__ctl0_endTimeLabel508")
	
	if(startDatelabel508 == null)
	{
	    startDatelabel508 = document.getElementById("UltraWebTab1__ctl0_ctl00_startdateLabel508");
	    startTimeLabel508 = document.getElementById("UltraWebTab1__ctl0_ctl00_startTimeLabel508");
	    endDateLabel508 = document.getElementById("UltraWebTab1__ctl0_ctl00_endDateLabel508");
	    endTimeLabel508 = document.getElementById("UltraWebTab1__ctl0_ctl00_endTimeLabel508")
	}
	
	startDatelabel508.htmlFor = startDateChooser.inputBox.id;
	startTimeLabel508.htmlFor = startTimeDropDown._input.id;
	endDateLabel508.htmlFor = endDateChooser.inputBox.id;
	endTimeLabel508.htmlFor = endTimeDropDown._input.id;
	    
    
	
	var toolbar = document.getElementById("UltraWebToolbar2");
	if(toolbar != null)
	{
		for(var i = 0 ; i < toolbar.childNodes.length; i++)
		{
			if(toolbar.childNodes[i].tagName == "TABLE")
			{
				toolbar.childNodes[i].cellSpacing = 0;
				break;
			}
		}
	}
		
	
	var apptTabContentArea = document.getElementById("UltraWebTab1_cp");
	if(apptTabContentArea != null)
		apptTabContentArea.style.border = "none";
		
	
	var appTabHeader = document.getElementById("UltraWebTab1td0");
	if(appTabHeader != null)
	{
		var lastTd = null;
		while(appTabHeader.nextSibling != null)
		{
			appTabHeader = appTabHeader.nextSibling;
			if(appTabHeader.tagName == "TD")
				lastTd = appTabHeader;
		}
		if(lastTd != null)
			lastTd.style.width = "100%";
	}
}

function InitializeValues()
{
	var appointment = dialogInterface.getAppointment();
	if(appointment.getIsOccurrence())
	{
		var recurrence = appointment.getRecurrence();
		if(recurrence != null)
		{
			var type = recurrence.getEditType();
			
			if(type == 0)
			{
				type = recurrence.tempEditType;
			}
			if(type == 1)
			{
			    document.getElementById("statusBar").style.display = "none";
				document.getElementById("recurrenceTable").style.display = "";
				document.getElementById("durationTable").style.display = "none";
				var label = document.getElementById("UltraWebTab1__ctl0__ctl0_recurrenceDescriptionText");
				if(!label)
				    label = document.getElementById("UltraWebTab1__ctl0_ctl00_recurrenceDescriptionText");
				label .innerHTML =  AppointmentDialogStrings[1] + " " + recurrence.getRecurrenceDescription();
			}
			else if(type == 2)
			{
			    var eReminder = document.getElementById("cbReminder");
			    if(eReminder)
				    eReminder.disabled = true;
				document.getElementById("statusBar").style.display = "";
			}
		}
	}	
	else if(appointment.getIsVariance())
	{
		document.getElementById("statusBar").style.display = "";
		var eReminder = document.getElementById("cbReminder");
		if(eReminder)
			eReminder.disabled = true;
	}
	
	if(!dialogInterface.getSupportsRecurrence() || ( (recurrence != null && type == 2) || appointment.getIsVariance())) 
	{		
		HideTBarButton("Recurrence");
	}	 
	
	var tdf = dialogInterface.getTimeDisplayFormat();
	if(tdf == 0)
		MilitaryTime = false;
	else if(tdf == 1)
		MilitaryTime = true;
		
	var startDate = new Date();
	startDate.setTime(appointment.getStartDateTime().getTime());
	startDateChooser.setValue(startDate);
	startTimeDropDown.setValue(convertDateTimeToString(startDate));
	
	var endDate = new Date();
	endDate.setTime(startDate.getTime());
	endDate.setMinutes(endDate.getMinutes() +  parseInt(appointment.getDuration()));
	endDateChooser.setValue(endDate);
	endTimeDropDown.setValue(convertDateTimeToString(endDate));
	
	var subject = document.getElementById("tbSubject");
	subject.value = appointment.getSubject();
	subject.focus();
	
	var loc = document.getElementById("tbLocation");
		loc.value = appointment.getLocation();
	
	var description = document.getElementById("txtMsgBody");
		description.value = appointment.getDescription();
	
	var alldayEvent = document.getElementById("cbAllDayEvent");
	alldayEvent.checked = appointment.getAllDayEvent();
	cbAllDayEvent_Clicked();
		
	if(!dialogInterface.getAllowAllDayEvents())	
	{
	    alldayEvent.checked = false;
		alldayEvent.style.display = "none"; 
	
	    
		var allDayLabel = document.getElementById("UltraWebTab1__ctl0__ctl0_AllDayEventLabel");
		if(allDayLabel)
		    allDayLabel.style.display = "none";		    
		
		cbAllDayEvent_Clicked();
	}
		
	var enableReminder = document.getElementById("cbReminder");
	enableReminder.checked = appointment.getEnableReminder();
	cbReminder_Clicked();

	var displayInterval = document.getElementById("ddReminder");	
	var interval = convertTicksToString(appointment.getReminderInterval());
	var index = -1;
	for(var i = 0; i < displayInterval.options.length && index == -1; i++){
		if(interval == displayInterval.options[i].innerHTML)
			index = displayInterval.options[i].index;
	}
	if(index != -1)
		displayInterval.selectedIndex = index;
	else
	{
		var option = document.createElement("OPTION");
		displayInterval.appendChild(option);
		option.innerHTML = interval;
		option.value = "Test";
		displayInterval.value = "Test";
		window.setTimeout("document.getElementById('ddReminder').selectedIndex = document.getElementById('ddReminder').options.length -1;",1);
	}
	
	var showtimeas = document.getElementById("ddShowTimeAs");
	showtimeas.selectedIndex = appointment.getShowTimeAs();;
		
	var importance = igtbar_getToolbarById("UltraWebToolbar2");
	var importanceVal = appointment.getImportance();
	if(importance.Items[4].Type == "1")
		importance = importance.Items[4];
	else
		importance = importance.Items[5];
	if(importanceVal == "0")
		importance.Items[1].setSelected(true);
	else if(importanceVal == "2")
		importance.Items[0].setSelected(true);
	
	var dataKey = appointment.getDataKey();
	if(dataKey == null)
	{	
		HideTBarButton("Delete");
	}
		
	calDifference =  startDateChooser.getValue().getTime() - endDateChooser.getValue().getTime();
}

function HideTBarButton(key)
{
    var button = igtbar_getToolbarById("UltraWebToolbar2").Items.fromKey(key);    
    var colGroup = button.Element;
    while (colGroup.tagName != "TABLE")
    {
        colGroup = colGroup.parentNode;
    }
    for (var i = 0; i < colGroup.childNodes.length; i++)
    {
        if (colGroup.childNodes[i].tagName == "COLGROUP")
        {
            colGroup = colGroup.childNodes[i];
            break;
        }
    }
    if (colGroup.tagName != "COLGROUP")
    {
        return;
    }
            var currentColIndex = -1;
        for (var j = 0; j < colGroup.childNodes.length; j++)
        {
            if (colGroup.childNodes[j].tagName == "COL")
            {
                currentColIndex++;
            }
            if (currentColIndex == button.Index)
            {
                colGroup.childNodes[j].style.display = "none";
                colGroup.childNodes[j].style.visibility = "hidden";
                break;
            }
        }
    if (!ig_shared.IsIE)
    {
        button.Element.style.display = "none";
        button.Element.style.visibility = "hidden";
    }

    button.tabIndex = -1;
}

function OKClicked(oToolbar, oButton, oEvent) 
{	
	if(oButton.Key != "High" && oButton.Key != "Low" )
	{
		if(oButton.Index == 0 || oButton.Index == 2)	// Save and Close and Delete
		{
			var alldayEvent = document.getElementById("cbAllDayEvent");
			if(alldayEvent.checked)
			{
				var time = new Date();
				time.setHours(0,0,0,0);
				startTimeDropDown.setValue(convertDateTimeToString(time));
				time.setHours(23,59,0,0);
				endTimeDropDown.setValue(convertDateTimeToString(time));
			}
			var startDateValue = startDateChooser.getValue();
			var startTime = convertStringToDateTime(startTimeDropDown.getValue());
			
			var endDateValue = endDateChooser.getValue();
			var endTime = convertStringToDateTime(endTimeDropDown.getValue());
			
			var startDateTime = new Date();
			startDateTime.setTime(startDateValue.getTime());
			startDateTime.setHours(startTime.getHours(), startTime.getMinutes());
			
			
			var endDateTime = new Date();
			endDateTime.setTime(endDateValue.getTime());
			endDateTime.setHours(endTime.getHours(), endTime.getMinutes());
			
			var duration = (endDateTime.getTime() - startDateTime.getTime())/60000;
			var intduration = parseInt(duration.toString());
			if((duration - intduration) > 0)
				duration += 1;
			
			var subject = document.getElementById("tbSubject");
			var loc = document.getElementById("tbLocation");
			var description = document.getElementById("txtMsgBody");			
			var enableReminder = document.getElementById("cbReminder");
			
			var displayInterval = document.getElementById("ddReminder");	
			var interval = convertStringToTicks(displayInterval.options[displayInterval.selectedIndex].innerHTML.split(" "));
			
			var showtimeas = document.getElementById("ddShowTimeAs");									
			
			var importance = igtbar_getToolbarById("UltraWebToolbar2");
			var highItem = importance.Items[5].Items[0];
			var lowItem = importance.Items[5].Items[1];
			var importanceValue = "1";
			if(highItem.getSelected())
				importanceValue = "2";
			else if(lowItem.getSelected())
				importanceValue = "0";
		var activityEditProps = {StartDate:		 {	Element: startDateChooser.Element, 
													Object : startDateChooser,
													Value  : startDateValue},
									StartTime:		 {	Element: startTimeDropDown._elem,
													Value  : startTime},
									EndDate:		 {	Element: endDateChooser.Element, 
													Object : endDateChooser,
													Value  : endDateValue},
									EndTime:		 {	Element: endTimeDropDown._elem,
													Value  : endTime},
									AllDayEvent:	 {	Element: alldayEvent, 
													Value  : alldayEvent.checked},
									Subject:		 {	Element: subject, 
													Value  : subject.value},
									Location:		 {	Element: loc, 
													Value  : loc.value}, 
									Description:	 {	Element: description, 
													Value  : description.value},
									EnableReminder: {	Element: enableReminder, 
													Value  : enableReminder.checked},
									DisplayInterval:{	Element: displayInterval, 
													Value  : interval}, 
									ShowTimeAs:	 {	Element: showtimeas, 
													Value  : showtimeas.options[showtimeas.selectedIndex].innerHTML},
									Importance:	 {	ToolBar: importance, 
													HighItem: highItem,
													LowItem: lowItem, 
													Value  : importanceValue},
									Duration:		 {  Value  : duration},
									document:		 document,
									window:		 window
								}	
			
			dialogInterface.setActivityEditProps(activityEditProps);
			var appointment = dialogInterface.getAppointment();
			var variance = false; 
			if(appointment.recurrence != null)
			{
				var type = parseInt(appointment.recurrence.getEditType());
				if(type == 0)
					type = appointment.recurrence.tempEditType; 
				if(type == 2)
					variance = true; 
			}
			else
				variance = appointment.getIsVariance(); 
			if(variance)
			{
				var wsi = dialogInterface.getWebScheduleInfo(); 
				var rootActivity = wsi.getActivities().getItemFromKey(appointment.getRootActivityKey()); 
				var recurrence = rootActivity.getRecurrence(); 
				var recurrenceEndDate = recurrence.getEndDate();
				if(recurrenceEndDate != null)
				{
				    
				    recurrenceEndDate = new Date(recurrenceEndDate.getTime());
					recurrenceEndDate.setDate(recurrenceEndDate.getDate() + 1); 
				}
				else
				{
					
					recurrenceEndDate = new Date();
					recurrenceEndDate.setFullYear(9999,11,31);
				}
					
				var nextOccur = (appointment._nextOccur == null || appointment._nextOccur.length == 0) ? recurrenceEndDate.getTime() : appointment._nextOccur.getTime();
				var rootEndDate = rootActivity.getStartDateTime();
				rootEndDate.setHours(0,0,0,0); 
				var prevOccur = (appointment._prevOccur == null || appointment._prevOccur.length == 0) ? rootEndDate.getTime() : appointment._prevOccur.getTime(); 
				
				if(startDateTime.getTime() <= prevOccur)
				{					
					if(appointment._prevOccur == null || appointment._prevOccur.length == 0)
						alert(AppointmentDialogStrings[2]); 
					else
						alert(AppointmentDialogStrings[3]); 
					return; 
				}
				if(endDateTime.getTime() >= nextOccur)
				{
					if(appointment._nextOccur == null || appointment._nextOccur.length == 0)
						alert(AppointmentDialogStrings[4]); 
					else
						alert(AppointmentDialogStrings[5]); 
					return; 
				}
				
			}
			
			var dataKey = appointment.getDataKey();
			if(oButton.Index == 0 && dataKey != null)
				dialogInterface.setOperation("Update");
			else if(oButton.Index == 0 && dataKey == null)
				dialogInterface.setOperation("Add");
			else if(oButton.Index == 2)	
				dialogInterface.setOperation("Delete");
				
			var type =  null;
			var recurrence = appointment.getRecurrence();
			
			if(recurrence != null)
				type = parseInt(appointment.getRecurrence().getEditType());
			
			if(type == 0)
				type = recurrence.tempEditType;
			
			if(type == 1 && !dialogInterface.getRecurrenceDialogDisplayed())
			{
				var date = new Date();
				date.setTime(appointment.getOriginalStartDate().getTime());
				appointment.setStartDateTime(date);
			}
			
			if(recurrence == null || type == 2)
			{	
				appointment.setAllDayEvent(alldayEvent.checked);
				appointment.setStartDateTime(startDateTime);	
				appointment.setDuration(duration);			
			}
			appointment.setSubject(subject.value);
			appointment.setLocation(loc.value);
			appointment.setDescription(description.value);
			appointment.setEnableReminder(enableReminder.checked);
			appointment.setReminderInterval(interval);
			appointment.setShowTimeAs(showtimeas.selectedIndex);
			appointment.setImportance(importanceValue);
							
			dialogInterface.saveAndClose();
		}
		else if(oButton.Index == 1) // Print
		{
			var ActiveResourceName = dialogInterface.getActiveResourceName();

			var frame = document.getElementById("printFrame");
			frame.style.position = 'absolute';
			frame.style.zIndex = -1;
			frame.style.height = 1;
			frame.style.width = 1;
			frame.style.visibility = "visible";

			var frameDocument = frame.contentWindow.document;
            
			frameDocument.getElementById("ResourceLabel").innerHTML = ActiveResourceName;
			frameDocument.getElementById("SubjectLabel").innerHTML =  document.getElementById("UltraWebTab1__ctl0_ctl00_SubjectLabel").innerHTML;
			frameDocument.getElementById("tbSubject").innerHTML = document.getElementById("tbSubject").value;
			frameDocument.getElementById("LocationLabel").innerHTML =  document.getElementById("UltraWebTab1__ctl0_ctl00_LocationLabel").innerHTML;
			frameDocument.getElementById("tbLocation").innerHTML = document.getElementById("tbLocation").value;
			frameDocument.getElementById("StartTimeLabel").innerHTML = document.getElementById("UltraWebTab1__ctl0_ctl00_StartTimeLabel").innerHTML;
			frameDocument.getElementById("EndTimeLabel").innerHTML = document.getElementById("UltraWebTab1__ctl0_ctl00_EndTimeLabel").innerHTML;
			frameDocument.getElementById("cbAllDayEvent").checked = document.getElementById("cbAllDayEvent").checked;
			if(frameDocument.getElementById("cbAllDayEvent").checked)
			{
				frameDocument.getElementById("ddStartTime").style.display = "none";
				frameDocument.getElementById("ddEndTime").style.display = "none";
			}
			else
			{
				frameDocument.getElementById("ddStartTime").innerHTML = startTimeDropDown.getValue();
				frameDocument.getElementById("ddEndTime").innerHTML = endTimeDropDown.getValue();
			}
			frameDocument.getElementById("AllDayEventLabel").innerHTML = document.getElementById("UltraWebTab1__ctl0_ctl00_AllDayEventLabel").innerHTML;
			frameDocument.getElementById("cbReminder").checked = document.getElementById("cbReminder").checked;
			frameDocument.getElementById("cbReminderLabel").innerHTML = document.getElementById("UltraWebTab1__ctl0_ctl00_ReminderLabel").innerHTML;
			var ddShowTimeAs = document.getElementById("ddShowTimeAs");
			frameDocument.getElementById("ddShowTimeAs").innerHTML = ddShowTimeAs.options[ddShowTimeAs.selectedIndex].innerHTML;
			var ddReminder = document.getElementById("ddReminder");
			frameDocument.getElementById("ddReminder").innerHTML = ddReminder.options[ddReminder.selectedIndex].innerHTML;
			frameDocument.getElementById("txtMsgBody").value = document.getElementById("txtMsgBody").value;
			frameDocument.getElementById("wdcStartTime").innerHTML = startDateChooser.getText();
			frameDocument.getElementById("wdcEndTime").innerHTML = endDateChooser.getText();
		
			frame.contentWindow.focus();
			frame.contentWindow.print();
			
			frame.style.visibility = "hidden";
		}
				
	}
	if(oButton.Index == 3) // Recurrence
	{
		var app = dialogInterface.getAppointment();
		var startDateValue = startDateChooser.getValue();
		var startTime = convertStringToDateTime(startTimeDropDown.getValue());
		
		var endDateValue = endDateChooser.getValue();
		var endTime = convertStringToDateTime(endTimeDropDown.getValue());
		
		var startDateTime = new Date();
		startDateTime.setTime(startDateValue.getTime());
		startDateTime.setHours(startTime.getHours(), startTime.getMinutes());
		app.setStartDateTime(startDateTime);
		
		var endDateTime = new Date();
		endDateTime.setTime(endDateValue.getTime());
		endDateTime.setHours(endTime.getHours(), endTime.getMinutes());
		var duration = (endDateTime.getTime() - startDateTime.getTime())/60000;
		app.setDuration(duration);
		
		dialogInterface.recurrenceDialogClosed = updateRecurrence;
		

		setTimeout("dialogInterface.showRecurrenceDialog()", 0);
	}
}
document.recurrenceApplied = false;

function updateRecurrence(appointment, recurrence)
{
	var recurrenceTable = document.getElementById("recurrenceTable");
	var durationTable = document.getElementById("durationTable");
	if(recurrence != null)
	{
		durationTable.style.display = "none";
		var label = document.getElementById("UltraWebTab1__ctl0__ctl0_recurrenceDescriptionText");
		if(!label)
		    label = document.getElementById("UltraWebTab1__ctl0_ctl00_recurrenceDescriptionText");
		label.innerHTML = AppointmentDialogStrings[6]; 
		recurrenceTable.style.display = "";
	}	
	else
	{
		durationTable.style.display = "";
		recurrenceTable.style.display = "none";
	}
}


function convertTicksToString(ticks){
	var seconds = ticks / 10000000;
	var minutes = seconds / 60;
	var hours = minutes / 60; 
	var days = hours / 24;
	var weeks = days / 7;
	if(days%7 != 0)
		weeks = 0; 
	var returnString; 
	
	if(weeks == 1)
		returnString = "1 " + AppointmentDialogStrings[7];
	else if(weeks > 1)
		returnString = weeks + " " + AppointmentDialogStrings[8];
	else if(days == 1)
		returnString = "1 " + AppointmentDialogStrings[9];
	else if (days > 1)
		returnString = days + " " + AppointmentDialogStrings[10];		
	else if(hours == 1)
		returnString = "1 " + AppointmentDialogStrings[11];
	else if (hours > 1)
		returnString = hours + " " + AppointmentDialogStrings[12];
	else if(minutes == 1)
		returnString = "1 " + AppointmentDialogStrings[13];
	else if (minutes > 1 || minutes == 0)
		returnString = minutes + " " + AppointmentDialogStrings[14];
	else
		returnString = AppointmentDialogStrings[15];
		
	if(returnString == "12 " + AppointmentDialogStrings[12])
		returnString = "0.5 " + AppointmentDialogStrings[10];
					
	return returnString;
}
function convertStringToTicks(string){
	var interval = string[0];
	var units = string[1];
	var ticks = 0; 
	if(units.indexOf(AppointmentDialogStrings[7],0) != -1)
		ticks = interval * 7 * 24 * 60 * 60 * 10000000;
	else if(units.indexOf(AppointmentDialogStrings[9],0) != -1)
		ticks = interval * 24 * 60 * 60 * 10000000;
	else if(units.indexOf(AppointmentDialogStrings[11],0) != -1)
		ticks = interval * 60 * 60 * 10000000;
	else if(units.indexOf(AppointmentDialogStrings[13],0) != -1)
		ticks = interval * 60 * 10000000;
	return ticks;	
}

function cbAllDayEvent_Clicked()
{
	var cb = document.getElementById("cbAllDayEvent");
	var td1 = document.getElementById("startTime");
	var td2 = document.getElementById("endTime");

	if(cb.checked)
	{
		startTimeDropDown._elem.style.visibility = "hidden"; 
		startTimeDropDown._elem.style.display = "none";
		endTimeDropDown._elem.style.visibility = "hidden"; 
		endTimeDropDown._elem.style.display = "none";
		td1.style.display = "none";
		td1.style.visibility = "hidden";
		td2.style.display = "none";
		td2.style.visibility = "hidden";
	}
	else
	{
		startTimeDropDown._elem.style.visibility = ""; 
		startTimeDropDown._elem.style.display = "";
		endTimeDropDown._elem.style.visibility = ""; 
		endTimeDropDown._elem.style.display = "";
		td1.style.display = "";
		td1.style.visibility = "";
		td2.style.display = "";
		td2.style.visibility = "";
	}
}



function cbReminder_Clicked()
{
	var reminder = document.getElementById("cbReminder");
	var ddreminder = document.getElementById("ddReminder");
	ddreminder.disabled = !reminder.checked;
}


