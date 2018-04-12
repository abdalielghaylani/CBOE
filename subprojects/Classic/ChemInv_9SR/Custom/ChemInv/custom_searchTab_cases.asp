
<%Case "ehs"%>
<table border="0" width="500" cellpadding="10" cellspacing="0">
		  	<tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td valign="top">
				<!-----COL 1 -------->
				<table border="0">
		      		<tr>
			        	<td>
							<span title="(e.g. 50-50-0)">CAS Registry#:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.CAS", "0","30"%>                     
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="One or more Container Barcode Id values separated by commas">Container ID:<%=GetBarcodeIcon()%>&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Barcode", "0","30"%>
						</td>
					</tr>
		      		<tr>
		        		<td>
							<span title="One or more Location Barcode values separated by commas">Location Barcode:<%=GetBarcodeIcon()%>&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_Locations.Location_Barcode", "0","30"%>
						</td>
					</tr>
					
		      		<tr>
			        	<td>
							<span title="Supplier's catalog number">Catalog Number:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Supplier_CatNum", "0","30"%>	        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Purchase Order Number">PO Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.PO_Number", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Purchase Order Line Number">PO Line Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.po_line_number", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
					</tr>
		      		<tr>
			        	<td>
							
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 2 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Substance Name:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_Compounds.Substance_Name", "0","50"%>        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Substance Synonym:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_synonyms.Substance_Name", "0","50"%>        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Descriptive name of the container">Container Name:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_Name", "0","50"%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Container Comments">Container Comments:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_Comments", "0","50"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Use a barcode reader or the browse link to select the location to search">Location ID: <%=GetBarcodeIcon()%></span><br>
							<%ShowLocationPicker "document.cows_input_form", "tempLocation_ID", "LocationBarCode", "LocationName", 10, 37, true%> 
							<font size="1"><input Type="CheckBox" name="searchSubLocations" onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)">Search Sublocations</font>
							<br>
							<input type="hidden" name="SpecialLocationList" value="<%=invSpecialLocs%>">
							<font size="1"><input Type="CheckBox" name="ExcludeSpecialLocations" value="checked" onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)">Exclude <a class="menuLink" href="#" onclick="OpenDialog('SpecialLocationSelector.asp', 'LocSelectDiag', 1); return false"">Special</a> Locations</font>         
							<input type="hidden" name="inv_containers.Location_ID_FK">
						</td>
		      		</tr>
		      		<tr>
		        		<td>       
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							        
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 3 -------->
				<table border="0">
					<tr>
			        	<td>
							        
						</td>
		      		</tr>
					<tr>
			        	<td>
							
						</td>
		      		</tr>
					<tr>
			        	<td>
							 
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		    	</table>
			</td>
		  </tr>
		</table>
		