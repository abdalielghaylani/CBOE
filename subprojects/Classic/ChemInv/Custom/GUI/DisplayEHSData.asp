<%If Application("DISPLAY_EHS_DATA") then%>
<center>
<table border="0" cellpadding="1" cellspacing="2">
		<tr>
			<th colspan="7">
				Environmental Health & Safety Data
			</th>
		</tr>
		<%If EHSFoundData = "0" Then%>
		<tr>
			<td colspan=7 align=center><span class="GUIFeedback"><BR> No EH&S Data found for this compound.</span><br><br></td>
		</tr>
		<%else%>
		<tr>
			<!-- Row 1 Col 1-->		
			<%=ShowField("EH&S Group 1:", "EHSGroup1", 20, "")%>
			<!-- Row 1 Col 2-->
			<%=ShowField("&nbsp;&nbsp;&nbsp;&nbsp;EH&S Group 2:", "EHSGroup2", 20, "")%>
			<!-- Row 1 Col 3-->
			<%=ShowField("&nbsp;&nbsp;&nbsp;&nbsp;EH&S Group 3:", "EHSGroup3", 20, "")%>
		</tr>
		<tr>
			<!-- Row 2 Col 1-->
			<%=ShowField("UN Number:", "EHSUNNumber", 20, "")%>
			<!-- Row 2 Col 2-->
			<%=ShowField("&nbsp;&nbsp;&nbsp;&nbsp;Packing Group:", "EHSPackingGroup", 20, "")%>
			<!-- Row 2 Col 3-->
			<%
				If EHSIsRefrigerated = "0" Then
					EHSIsRefrigerated = "Not specified"
				Else
					EHSIsRefrigerated = "Y"
				End If
			%>
			<%=ShowField("&nbsp;&nbsp;&nbsp;&nbsp;Is Refrigerated:", "EHSIsRefrigerated", 20, "")%>
		</tr>
		<tr>
			<!-- Row 3 Col 1-->
			<%=ShowField("Health:", "EHSHealth", 20, "")%>
			<!-- Row 3 Col 2-->
			<%=ShowField("&nbsp;&nbsp;&nbsp;&nbsp;Flammability:", "EHSFlammability", 20, "")%>
			<!-- Row 3 Col 3-->
			<%=ShowField("&nbsp;&nbsp;&nbsp;&nbsp;Reactivity:", "EHSReactivity", 20, "")%>
		</tr>
		<tr>
			<!-- Row 4 Col 1-->
			<%
				If EHSIsOSHACarcinogen = "0" Then
					EHSIsOSHACarcinogen = "Not specified"
				Else
					EHSIsOSHACarcinogen = "Y"
				End If
			%>
			<%=ShowField("Is OSHA Carcinogen:", "EHSIsOSHACarcinogen", 20, "")%>
			<!-- Row 4 Col 2-->
			<%=ShowField("&nbsp;&nbsp;&nbsp;&nbsp;ACGIH Category:", "EHSACGIHCarcinogenCategory", 20, "")%>
			<!-- Row 4 Col 3-->
			<%=ShowField("&nbsp;&nbsp;&nbsp;&nbsp;IARC Carcinogen:", "EHSIARCCarcinogen", 20, "")%>
		</tr>
		<tr>
			<!-- Row 5 Col 1-->
			<%=ShowField("EU Carcinogen:", "EHSEUCarcinogen", 50, "")%>
		</tr>
		<tr>
			<!-- Row 6 Col 1-->
			<%
				If EHSIsSensitizer = "0" Then
					EHSIsSensitizer = "Not specified"
				Else
					EHSIsSensitizer = "Y"
				End If
			%>
			<%=ShowField("Is Sensitizer:", "EHSIsSensitizer", 20, "")%>
		</tr>
	<%end if%>	
	</table>
</center>
<%end if%>
