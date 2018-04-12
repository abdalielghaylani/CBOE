<html>

<head>
<meta NAME="GENERATOR" Content="Microsoft FrontPage 3.0">
<title>Generic POST Test Interface</title>
</head>

<body>

<h1>Generic POST Test Interface</h1>

<p><script language="JavaScript">
	    
	    function GetCDX(theForm) {
			if (document.applets["CDPHelper"].getMolWeight(0) > 0) {
				theForm.elements["Substance.Structure"].value = document.applets["CDPHelper"].getData(0)	
			} 
			else {
				theForm.elements["Substance.Structure"].value = ""
			}
			//alert(document.applets["CDPHelper"].getData(0))
		}

	</script> </p>

<p>&nbsp;</p>

<p>The following is an example of the input that will generate information when submitted
to cows:</p>

<p>All but Substance.Structure can be put directly in the query string rather than in a
form.</p>

<p>The query string must have the following at a minumum: (this example is a request to
the &quot;chemacx&quot; applications' &quot;sample&quot; data view)</p>
<font SIZE="1" COLOR="#ff0000">

<p></font><font COLOR="#ff0000" size="3">ACTION=&quot;</font><font COLOR="#0000ff"
size="3"><%=Application("ActionForm" & "chemacx")%>?dbname=chemacx&amp;dataaction=search_no_gui</font></p>

<p><font COLOR="#000000" size="3">dbname = &quot;the name of the db view e.g.
chemacx&quot; and&nbsp; </font></p>

<p><font COLOR="#000000" size="3">dataaction = &quot;search_no_gui&quot;.</font></p>

<form id="form2" name="form2" METHOD="Post"
xACTION="<%=Application("ActionForm" & "chemacx")%>?dbname=chemacx&amp;dataaction=search_no_gui"
ACTION="<%=Application("ActionForm" & "chemacx")%>?dbname=chemacx&amp;dataaction=search&formgroup=acxml&formode=search&dbname=chemacx">
  <table border="
3" cellpadding="0" cellspacing="0">
    <tr>
      <td><table>
        <tr>
          <td>&nbsp; EXAMPLE from a postrequest form.&nbsp; <table border="1">
            <tr>
              <td><embed src="<%=Application("TempFileDirectoryHTTP" & dbkey) & "mt.cdx"%>" border="3" width="253" height="151"
              type="chemical/x-cdx" name="CDP" id="1"></td>
              <td><table border="1">
                <tr>
                  <td>Substance.Structure:<input TYPE="text" name="Substance.Structure" value size="20">
                  this is the name of the field for where the base64 string from the plugin is put when you
                  click search.&nbsp; The name of this field must match the structure field name in the
                  dataview. In this example for the sample dataview, the structure field name is
                  Substance.Structure.**</td>
                </tr>
                <tr>
                  <td>Struc_search_type:<input TYPE="text" name="struc_search_type" value="exact" size="20">this
                  is specifing the type of structure search. Either exact or substructure.&nbsp; If nothing
                  is entered and there is a base64 string submitted it will default to exact.</td>
                </tr>
                <tr>
                  <td>Synonym.Name:<input TYPE="text" name="Synonym.Name" value="" size="20"> You can
                  have any number of relational fields as long as they are in the ini file searchable
                  fields**</td>
                </tr>
                <tr>
                  <td>Return Field<input TYPE="text" name="return_fields" value="Substance.CSNum" size="20" onfocus="blur()">
                  You can return any field as long as it is in the ini file. If nothing is specified then
                  the primary key for the base table is returned.</td>
                </tr>
                <tr>
                  <td>&nbsp;</td>
                </tr>
              </table>
              </td>
              <td>&nbsp;</td>
            </tr>
          </table>
          </td>
        </tr>
      </table>
      <table border="0">
	  <tr><td>
        <table border="1">
		<tr>
		<td colspan=2 align="center"><b>Return Format:</b></td>
		</tr>
		<tr>
		<td width=100><input type="radio" name="return_format" value="server_rs" checked="true">CSV</td>
		<!---<td width=100><input type="radio" name="return_format" value="client_wddx">Wddx</td>--->
		<td width=100><input type="radio" name="return_format" value="acxxml" >ACXxml</td>
		</tr>
		<tr>
		<td>&nbsp;</td>
		<!---<td>&nbsp;</td>--->
		

<input type=hidden name="SearchFields" value="">
<input type=hidden name="RelationalSearchFields" value="Synonym.Name">


<BR><BR></td>
		</tr>
		</table>
		</td></tr>
		<tr>
          <td><input TYPE="Submit" VALUE="Search" ONCLICK="GetCDX(form2)"></td>
        </tr>
      </table>
      </td>
    </tr>
  </table>
</form>

<p>
<applet align="baseline" code="camsoft.cdp.CDPHelperAppSimple" height="0" id="Applet1"
name="CDPHelper" width="10">
  <param name="ID" value="1">
</applet>
</p>

<p>If you were to put everything in the querystring and only post the structure then the
Action would be:</p>

<p><font COLOR="#ff0000" size="3">ACTION=&quot;</font><font COLOR="#0000ff" size="3"><%=Application("ActionForm" & "chemacx")%>?dbname=chemacx</font><font
size="3" color="#0000FF">&amp;dataaction=search_no_gui&amp;struc_search_type=exact&amp;return_field=Substance.CsNum&amp;Synonym.Name=ben</font></p>

<p>**if you want to find out what the searchable fields are for the dataview you can send
a special query request to cows and get a string of the fields back.</p>

<p><font COLOR="#ff0000" size="3">ACTION=&quot;</font><font COLOR="#0000ff" size="3"><%=Application("ActionForm" & "chemacx")%>?dbname=chemacx</font><font
size="3" color="#0000FF">&amp;dataaction=get_info&amp;info_type=structure_fields</font></p>

<p><font COLOR="#ff0000" size="3">ACTION=&quot;</font><font COLOR="#0000ff" size="3"><%=Application("ActionForm" & "chemacx")%>?dbname=chemacx</font><font
size="3" color="#0000FF">&amp;dataaction=get_info&amp;info_type=formula_fields</font></p>

<p><font COLOR="#ff0000" size="3">ACTION=&quot;</font><font COLOR="#0000ff" size="3"><%=Application("ActionForm" & "chemacx")%>?dbname=chemacx</font><font
size="3" color="#0000FF">&amp;dataaction=get_info&amp;info_type=molweight_fields</font></p>

<p><font COLOR="#ff0000" size="3">ACTION=&quot;</font><font COLOR="#0000ff" size="3"><%=Application("ActionForm" & "chemacx")%>?dbname=chemacx</font><font
size="3" color="#0000FF">&amp;dataaction=get_info&amp;info_type=base_table</font></p>

<p><font COLOR="#ff0000" size="3">ACTION=&quot;</font><font COLOR="#0000ff" size="3"><%=Application("ActionForm" & "chemacx")%>?dbname=chemacx</font><font
size="3" color="#0000FF">&amp;dataaction=get_info&amp;info_type=molecule_table</font></p>

<p><font COLOR="#ff0000" size="3">ACTION=&quot;</font><font COLOR="#0000ff" size="3"><%=Application("ActionForm" & "chemacx")%>?dbname=chemacx</font><font
size="3" color="#0000FF">&amp;dataaction=get_info&amp;info_type=rel_fields</font></p>

<p><font size="3" color="#0000FF">If some case there are more than a single structure, mw
and formula field, if the form that is in the dataview has more then one
(reactant/solvent/catalyst)</font></p>

<p><font size="3" color="#0000FF">relfields is returned as a list of type
fieldname1;datatype,fieldname2;datatype. You do not need the datatype when you submit the
data - it can be used for pre processing purposed to make sure the data submitted is
correct.</font></p>

<p><font size="3" color="#0000FF">other fields are not returned with a datatype as it is
already known.</font></p>
</body>
</html>
