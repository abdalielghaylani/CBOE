<%
Dim LastPercent
Dim StartTime


Sub Progressbar(currRow, totalRows, flushRate)
		PercentDone = Int((currRow/totalRows* 100)/5)
		if NOT Response.IsClientConnected then Response.end 
		
		if ((currRow MOD flushRate)=0) OR (currRow = totalRows)then 
				rowsRemaining = totalRows - currRow
				If LastPercent <> PercentDone then
					Block = "Block" & PercentDone
					response.write "<SCRIPT>"
					for i =  1  to PercentDone 
						response.write "document.Block" & i & ".src = '/cfserverasp/source/graphics/blue.gif';"
					next
					response.write "</SCRIPT>"
				End if	
				TimeElapsed = Timer - StartTime	
				RowsCompleted = totalRows - rowsremaining 
				Rate = Round(RowsCompleted/TimeElapsed, 2)
				response.write "<SCRIPT>document.form1.rowsRemaining.value='" & rowsRemaining & "';document.form1.timeElapsed.value='" & SecondsToTimer(TimeElapsed) & "';document.form1.rate.value='" & Rate & "';document.form1.totalRows.value='" & totalRows & "';document.form1.rowsCompleted.value='" & RowsCompleted & "';</SCRIPT>"
				response.flush
				LastPercent = PercentDone					
		End if
End sub

Function SecondsToTimer(seconds)
	Hours = Int(seconds/3600)
	seconds = seconds - hours * 3600
	If hours < 10 then hours = "0" & hours 
	Minutes = Int(seconds/60)
	If Minutes < 10 then Minutes = "0" & Minutes
	seconds = Int(seconds - minutes * 60)
	If seconds < 10 then seconds = "0" & seconds
	SecondsToTimer = Hours & ":" & Minutes & ":" & seconds
End function

Sub InitializeProgressBar()
	StartTime = Timer
	response.write "<HTML><HEAD><STYLE TYPE=""text/css"">"
	response.write "#ctext {position:relative; background-color:#FFFFFF; layer-background-color:#FFFFFF;font-family:verdana; font-size:12}"
	response.write "#caption {position:relative; background-color:#FFFFFF; layer-background-color:#FFFFFF;font-family:verdana; font-size:12}"
	response.write "</STYLE></HEAD><BODY><center><form name=form1>"
	response.write "<div id=""caption""></div>"
	response.write "<TABLE WIDTH=""100"" CELLPADDING=0 CELLSPACING=0 BORDER=0>"
	response.write "<TR><TD align=right nowrap><div id=""ctext""> Total rows :&nbsp;</div></TD><TD colspan=20><input size=""8"" disabled onfocus=""blur()"" type=""text"" name=""totalRows""></TD></TR>"
	response.write "<TR><TD align=right nowrap><div id=""ctext"">Rows remaining:&nbsp;</div></TD><TD colspan=20><input size=""8"" disabled onfocus=""blur()"" type=""text"" name=""rowsRemaining""></TD></TR>"
	response.write "<TR><TD align=right nowrap><div id=""ctext"">Rows completed:&nbsp;</div></TD><TD colspan=20><input  size=""8"" disabled onfocus=""blur()"" type=""text"" name=""rowsCompleted""></TD></TR>"
	response.write "<TR><TD align=right><div id=""ctext"">Progress:&nbsp;</div></TD>"
	For X = 1 to 20
	response.write "<TD><IMG SRC=""/cfserverasp/source/graphics/grey.gif"" NAME=""Block" & X & """ WIDTH=10 HEIGHT=15 ALT=""Progress Bar""><IMG SRC=""/cfserverasp/source/graphics/White.gif"" WIDTH=""2"" height=""15""></TD>"
	Next
	response.write "</TR>"
	response.write "<tr><TD align=right nowrap><div id=""ctext"">Time elapsed:&nbsp;</div></TD><TD colspan=20><input  size=""8"" disabled onfocus=""blur()"" type=""text"" name=""timeElapsed""></TD></tr>"
	response.write "<tr><TD align=right nowrap><div id=""ctext"">Rate (rows/sec):&nbsp;</div></TD><TD colspan=20><input  size=""8"" disabled onfocus=""blur()""  type=""text"" name=""rate""></TD></tr>"
	response.write "<tr><TD align=center colspan=21><BR><a href=# onclick=""window.close()"" ><IMG border=0 src=""" & Application("NavButtonGifPath") & "cancel_dialog_btn.gif" & """></a></TD></tr>"
	response.write "</TABLE></form></center></BODY></HTML>"
End Sub



%>
