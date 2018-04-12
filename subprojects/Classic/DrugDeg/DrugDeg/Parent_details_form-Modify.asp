<table  border = 2  width = 500>
	<tr>
		<!-- The cell for the parent compound's structure. -->
	    <td  valign = "top"  align = "center">
	 
			<%
			' The "+ 100" in the height & width is there because Karen Alsante requested
			' a bigger structure window in the parent edit display.
			ShowResult dbkey, formgroup, rsBase64, "DRUGDEG_BASE64.BASE64_CDX", "Base64CDX", ( widthStructArea + 100 ), ( heightStructArea + 100 )
			%>
			<%'ShowCFWChemResult dbkey, formgroup, "Structure","DRUGDEG_BASE64.BASE64_CDX", rsBase64("MOL_ID"), "cdx","212","156"%>

		</td>

		<!-- The cell for the rest of the parent information. -->
		<td  valign = "top">
			<table  border = 2>
				<!--
					I am using 'valign = "top" ' for each of the following data cells so that,
					if a title or value is more than one line then the corresponding value or
					title will be pushed to the top of its cell, rather than floating in the
					middle of its cell.  Just an aesthetic point...
				-->


				

						
				<%'only let them change if they have permission.
				if CBool( Session( "DD_APPROVE_RECORDS" & dbkey )) = True then%>				
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Status</strong>
					</td>
					<td  valign = "top">
						
						<%	
							on error resume next
							Set connBase = GetNewConnection( dbkey, formgroup, "base_connection" )
													
							sStatusText = getStatusText (rsParent("STATUS"), connBase)
							sStatusList = GetStatusList( connBase )
							'	Response.Write( "sStatusList = [ " & sStatusList & " ]" )
							ShowLookUpList dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.STATUS", sStatusList, rsParent.Fields("STATUS"), sStatusText, 0, false, "value", ""
							connBase.close
							 %>
					</td>	
				</tr>	
				<input type=hidden name=old_status value="<%=rsParent.Fields( "STATUS" )%>">
				<input type=hidden name=dataaction2 value="SEND_EMAIL">	
				<input type="hidden" name="sendemail" value="SEND_EMAIL">
				<%end if%>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Generic name</strong>
					</td>
					<td  valign = "top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.GENERIC_NAME", "RAW", 0, 40 %>
						
					</td>	
				</tr>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Trade name</strong>
					</td>
					<td  valign = "top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.TRADE_NAME", "RAW", 0, 40 %>
						
					</td>	
				</tr>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Common/Other name</strong>
					</td>
					<td  valign = "top">
					<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.COMMON_NAME", "RAW", 0, 40 %>
						
					</td>	
				</tr>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Compound number</strong>
					</td>
					<td  valign = "top">
					<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.COMPOUND_NUMBER", "RAW", 0, 40 %>
						
					</td>	
				</tr>
			</table>
		</td>
	</tr>
</table>


&nbsp;<br>


<!-- Put up the checkboxes for the salts. -->
<%
' Make a string of the salts already selected.
sSelectedSaltCodes = rsParent.Fields( "SALT" )

' Open a connection for the salts table.
Set connDB = GetNewConnection( dbkey, formgroup, "base_connection" )
%>

<table  border = 1>
	<tr>
		<td  colspan = 2  align = "center">
			<strong>Salts</strong>
		</td>
	</tr>
<%
' Make and fill a record set for the available salts.
Dim	rsSalts
Set rsSalts = Server.CreateObject( "ADODB.Recordset" )
sSQL = "select SALT_CODE, SALT_NAME from DRUGDEG_SALTS order by SALT_CODE"
rsSalts.Open sSQL, connDB

rsSalts.MoveFirst
while not rsSalts.EOF
%>
	<tr>
		<td  valign = "top">
<%
	' Determine whether the current salt is one of those in the list of selected salts.
	if ( 0 < instr( UCase( sSelectedSaltCodes ), UCase( rsSalts.Fields( "SALT_CODE" ) ) ) ) then
		' The current salt _is_ in the list of selected salts.  The box is checked.
%>
			<input  type = "checkbox"  name = "UID.<%=BaseID%>:DRUGDEG_PARENTS.SALT"  value = "<%=rsSalts.Fields( "SALT_CODE" )%>" onBlur="UpdateSaltFieldVal(&quot;<%="UID." & BaseID & ":DRUGDEG_PARENTS.SALT"%>&quot;)" checked>
<%
	else
		' The current salt is not in the list of selected salts.
%>
			<input  type = "checkbox"  name = "UID.<%=BaseID%>:DRUGDEG_PARENTS.SALT"  value = "<%=rsSalts.Fields( "SALT_CODE" )%>" onBlur="UpdateSaltFieldVal(&quot;<%="UID." & BaseID & ":DRUGDEG_PARENTS.SALT"%>&quot;)">
<%
	end if  ' if the current salt is in the list of selected salts...
%>
			<%=rsSalts.Fields( "SALT_CODE" )%> - <%=rsSalts.Fields( "SALT_NAME" )%>
			<!--
				Without the following &nbsp, there can be a blank line between the longest line
				and the line which follows.
			-->
			&nbsp;
		</td>
		<td  valign = "top">
<%
	rsSalts.MoveNext
	if not rsSalts.EOF then
		' Determine whether the current salt is one of those in the list of selected salts.
		if ( 0 < instr( UCase( sSelectedSaltCodes ), UCase( rsSalts.Fields( "SALT_CODE" ) ) ) ) then
			' The current salt _is_ in the list of selected salts.  The box is checked.
%>
			<input  type = "checkbox"  name = "UID.<%=BaseID%>:DRUGDEG_PARENTS.SALT"  value = "<%=rsSalts.Fields( "SALT_CODE" )%>" onBlur="UpdateSaltFieldVal(&quot;<%="UID." & BaseID & ":DRUGDEG_PARENTS.SALT"%>&quot;)" checked>
<%
		else
			' The current salt is not in the list of selected salts.
%>
			<input  type = "checkbox"  name = "UID.<%=BaseID%>:DRUGDEG_PARENTS.SALT"  value = "<%=rsSalts.Fields( "SALT_CODE" )%>" onBlur="UpdateSaltFieldVal(&quot;<%="UID." & BaseID & ":DRUGDEG_PARENTS.SALT"%>&quot;)">
<%
		end if  ' if the current salt is in the list of selected salts...
%>
			<%=rsSalts.Fields( "SALT_CODE" )%> - <%=rsSalts.Fields( "SALT_NAME" )%>
<%
		rsSalts.MoveNext
	end if  ' if there _is_ a current salt...
%>
			<!--
				Without the following &nbsp, there can be a blank line between the longest line
				and the line which follows.  If there is no salt, this space will make the cell
				borders appear, which doesn't look as odd as not having the borders.
			-->
			&nbsp;
		</td>
	</tr>
<%
wend
CloseRS( rsSalts )
CloseConn( connDB )
%>
</table>



&nbsp;<br>
