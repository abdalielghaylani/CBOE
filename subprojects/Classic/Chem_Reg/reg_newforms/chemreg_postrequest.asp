<%@ LANGUAGE=VBScript %>
<%	Response.expires=0
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
%>
<html>

<head>
<script language="Javascript">
var cd_plugin_threshold="<%=Application("CD_PLUGIN_THRESHOLD")%>"
</script>
<script language="JavaScript" src= "/cfserverasp/source/chemdraw.js"></script>
<script language="JavaScript">  cd_includeWrapperFile("/cfserverasp/source/")</script>

<script language="JavaScript">
	    
	    function GetCDX1(theForm) {
			theForm.elements["reg_numbers.Structure"].value = cd_getData("cdpstructure1", "chemical/x-cdx")
		}

   function GetCDX2(theForm) {
			theForm.elements["Temporary_Structures.Structure"].value =cd_getData("cdpstructure2", "chemical/x-cdx")	
		}

 function GetCDX3(theForm) {
			theForm.elements["Temporary_Structures.Structure"].value =cd_getData("cdpstructure3", "chemical/x-cdx")	

		}



	</script> 
<meta NAME="GENERATOR" Content="Microsoft FrontPage 4.0">
<title>Generic POST Test Interface</title>
</head>

<body >

<h1>Chemical Registration Enterprise Post Interface
</h1>
<p>This post interface describes the methods and parameters for accessing the
<%=Application("appkey")%> system through a post interface.&nbsp; Each section has a form that
will allow you to test the post for each method.</p>

Post action: <a href="<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp"><%=Application("AppPathHTTP")%>/reg/reg_post_action.asp</a>
<p>
NOTE: When posting from outside of the chem_reg directory, you need to add http://your_server_name to the post action.
<p>
<form id="form1" name="form1" METHOD="Post" ACTION="<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp">
  <p></p>
  <table border="5" cellpadding="0" cellspacing="0">
   
      
      <tr><td>
      1. SEARCH&nbsp;
      <p>REG_METHOD <input type = "text" name = "reg_method" value = "SEARCH" size="31">
      </p>
      <p>&nbsp;</p>
        </td></tr>
      <td><table>
        <tr><td> You must send both the username and password for any registration activity to proceed.
            This username/password pair also must have permissions for the task
            being performed (granted role).<p>USER_ID<input type = "text" name = "user_id" value = "T5_84" size="20"><br>
            USER_PWD<input type = "text" name = "user_pwd" value = "T5_84" size="20"><tr>
          <td><!--embed src="/CFWTEMP/<%=Application("appkey")%>/regTEMP/mt.cdx" align="baseline" border="0"  width="300" height="200" type="chemical/x-cdx" name="CDP" id="1"><noembed>The ChemDraw Plugin is required to view this page properly</noembed-->&nbsp;&nbsp;
 <table border="1">
            <tr>
              <td>
              <script language = "javascript">
				var embedString = "<embed name='cdpstructure1' src='/CFWTEMP/<%=Application("appkey")%>/regTEMP/mt.cdx' align='baseline' border='0' width='400' height='300' id='1' type='chemical/x-cdx' viewonly='false'>"
				cd_insertObjectStr(embedString)
			</script>
			</td>
              <td><table border="1">
                <tr>
                  <td>reg_numbers.Structure<input TYPE="text" name="reg_numbers.Structure" value size="20">
                    this is the name of the field for where the base64 string
                    from the plugin is put when you click search.&nbsp;&nbsp;</td>
                  <td></td>
                </tr>
                <tr>
                  <td><font size="1" color="#000000">STRUC_SEARCH_TYPE</font><input TYPE="text" name="struc_search_type" value="" size="20"><br>
                    this is specifying the type of structure search.
                    EXACT|SUBSTRUCTURE|SIMILARITY.&nbsp; If nothing is entered and there is
                    a base64 string submitted it will default to exact.</td>
                </tr>
                 <tr>
                  <td><font size="1" color="#000000">Sim_Search_Threshold: Intger 0-100</font><input TYPE="text" name="Sim_Search_Threshold" value="90" size="20" ID="Text1"><br>
                  </td>
                  <td><font size="1" color="#000000">Full_Structure_Similarity 0|1</font><input TYPE="text" name="Full_Structure_Similarity" value="1" size="20" ID="Text2"><br>
                  </td>
                  </td>
                  <td><font size="1" color="#000000">Match_Tet_Stereo 0|1</font><input TYPE="text" name="Match_Tet_Stereo" value="0" size="20" ID="Text3"><br>
                  </td>
                  <td><font size="1" color="#000000">Match_DB_Stereo 0|1</font><input TYPE="text" name="Match_DB_Stereo" value="0" size="20" ID="Text4"><br>
                  </td>
                  <td><font size="1" color="#000000">Hit_Any_Charge_Carbon 0|1</font><input TYPE="text" name="Hit_Any_Charge_Carbon" value="0" size="20" ID="Text5"><br>
                  </td>
                  <td><font size="1" color="#000000">Rxn_hit_Rxn_Center 0|1</font><input TYPE="text" name="Rxn_hit_Rxn_Center" value="0" size="20" ID="Text6"><br>
                  </td>
                  <td><font size="1" color="#000000">Hit_Any_Charge_Hetero 0|1</font><input TYPE="text" name="Hit_Any_Charge_Hetero" value="0" size="20" ID="Text7"><br>
                  </td>
                </tr>
               
                <tr>
                  <td>RETURN_FIELDS<input TYPE="text" name="return_fields" value="Reg_Numbers.Reg_Number" size="28">
                    &nbsp;<br>
                  You can return any field as long as it is in the ini file. If nothing is specified then
                  the primary key for the base table is returned.&nbsp; For
                    instance, this would return something like R-00030</td>
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
        <tr>
          <td><input TYPE="Submit" VALUE="Search" ONCLICK="GetCDX1(form1)">NOTE
            this button is links to JavaScript that gets the data from the
            plugin and puts the base64 string into reg_numbers.structure</td>
        </tr>
      </table>
  </table>
</form>

<br>
<br>
<br>
<form id="form2" name="form2" METHOD="Post" ACTION="<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp">
  <table border="5" cellpadding="0" cellspacing="0">
   <tr><td>2. REG_TEMP</td></tr><tr><td>

      
      <table>
        <tr>
          <td>
      REG_METHOD <input type = "text" name = "reg_method" value = "REG_TEMP" size="31">
      <p>You must send both the username and password for any registration
      activity to proceed. This username/password pair also must have
      permissions for the task being performed (granted role).
            <p>
       USER_ID<input type = "text" name = "user_id" value = "T5_84" size="20"><br>
       USER_PWD<input type = "text" name = "user_pwd" value = "T5_84" size="20">
            <p>
       &nbsp;</td>
        </tr>
        <tr>
          <td>
      reg_parameter <input type = "text" name = "reg_parameter" size="25" value="ADD_COMPOUND">
      <p>
      possible values:ADD_COMPOUND|ADD_BATCH|ADD_SALT|ADD_IDENTIFIER|AUTHENTICATE
      
      If No value is sent, this defaults the setting specified in reg_post action:getRegParameterDefault
      currently this is ADD_COMPOUND if a structure is submitted and ADD_BATCH is no structure is
      submitted.
      <p>Additional requirements</p>
      <table border="1" width="100%">
        <tr>
    <td width="20%" height="19">ADD_COMPOUND</td>
    <td width="36%" height="19">structure</td>
        </tr>
        <tr>
    <td width="20%" height="16">ADD_BATCH</td>
    <td width="36%" height="16">reg_id</td>
        </tr>
        <tr>
    <td width="20%" height="19">ADD_SALT</td>
    <td width="36%" height="19">cpd_internal_id</td>
        </tr>
        <tr>
    <td width="20%" height="19">ADD_IDENTIFIER</td>
    <td width="36%" height="19">reg_id</td>
        </tr>
      </table>
          </td>
        </tr>
        <tr><td> <tr>
          <td><!--embed src="/CFWTEMP/<%=Application("appkey")%>/regTEMP/mt.cdx" align="baseline" border="0"  width="300" height="200" type="chemical/x-cdx" name="CDP" id="1"><noembed>The ChemDraw Plugin is required to view this page properly</noembed-->&nbsp;
            For data integrity the following required fields are automatically populated
            with values specified reg_post_action.asp:setdataintegritydefaults, unless they are
            explicitly sent with the post.
            <table border="1" width="100%">
              <tr>
                <td width="33%">Temporary_Structures.cpd_internal_id</td>
                <td width="11%"><input type = "text" name = "Temporary_Structures.cpd_internal_id" size="5"></td>
                <td width="56%">*required for ADD_SALT</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.reg_id&nbsp;</td>
                <td width="11%"><input type = "text" name = "Temporary_Structures.reg_ID" size="5"></td>
                <td width="56%">*required for ADD_BATCH and ADD_IDENTIFIER</td>
              </tr>
              <tr>
                <td width="33%">&nbsp;</td>
                <td width="11%">&nbsp;</td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Scientist_ID</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Scientist_ID" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Sequence_ID</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Sequence_ID" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Project_ID</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Project_ID" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Compound_Type</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Compound_Type" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Notebook_Text</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Notebook_Text" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Salt_Code</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Salt_Code" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">&nbsp;</td>
                <td width="11%">&nbsp;</td>
                <td width="56%">&nbsp;</td>
              </tr>
            </table>
            <p>You can also send any other fields in the temporary table that
            you want to populate.&nbsp; The must be sent as
            Tempoary_Structures.Whatever</p>
 <table border="1">
            <tr>
              <td> <script language = "javascript">
				var embedString = "<embed name='cdpstructure2' src='/CFWTEMP/<%=Application("appkey")%>/regTEMP/mt.cdx' align='baseline' border='0' width='400' height='300' id='2' type='chemical/x-cdx' viewonly='false'>"
				cd_insertObjectStr(embedString)
			</script></td>
              <td><table border="1">
                <tr>
                  <td>Temporary_Structures.Structure:<br>
                    <input TYPE="text" name="Temporary_Structures.Structure" value size="20">
                    &nbsp;<br>
                  this is the name of the field for where the base64 string from the plugin is put when you
                  click search.&nbsp;</td>
                  <td>*required for ADD_COMPOUND</td>
                  <td></td>
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
        <tr>
          <td><input TYPE="Submit" VALUE="REG_TEMP" ONCLICK="GetCDX2(form2)">
            this button is linked to JavaScript that puts the base64 string into
            the temporary_structures.structure field.
            <p>&nbsp;</p>
            <p><b>Returns</b> temp_id</p>
          </td>
        </tr>
      </table>
      </td>
        </tr>
  </table>
  </td>
        </tr>
  </table>
</form>
<br>
<br>
<hr>
<br>
 
<form id="form3" name="form3" METHOD="Post" ACTION="<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp">

 <table border="5" cellpadding="0" cellspacing="0">
    <tr><td>2. REGISTER&nbsp;&nbsp;</td></tr>
      
      <tr><td>
       <table>
        <tr>
          <td>
      REG_METHOD <input type = "text" name = "reg_method" value = "REG_PERM" size="31">
      <p>You must send both the username and password for any registration
      activity to proceed. This username/password pair also must have
      permissions for the task being performed (granted role).
            <p>
       USER_ID<input type = "text" name = "user_id" value = "T5_84" size="20"><br>
       USER_PWD<input type = "text" name = "user_pwd" value = "T5_84" size="20">
      <p>&nbsp;</p>
          </td>
        </tr>
        <tr>
          <td>
      reg_parameter <input type = "text" name = "reg_parameter" size="13">&nbsp;&nbsp;
      possible values:OVERRIDE|UNIQUE|UNIQUE_DEL_TEMP|USER_INPUT|NEW_BATCH|NEW_SALT|AUTHENTICATE
      <p>&nbsp;If No value is sent, this defaults the setting specified in reg_postaction:getRegParameterDefault currently this is UNIQUE_DEL_TEMP</p>
      <p>Additional requirements&nbsp;&nbsp;</p>
      <table border="1" width="100%" height="258">
        <tr>
    <td width="20%" height="19">OVERRIDE</td>
    <td width="36%" height="19">adds a new compound even if dups are found</td>
    <td width="15%" height="19">all required fields</td>
    <td width="19%" height="19"></td>
        </tr>
        <tr>
    <td width="20%" height="38">UNIQUE</td>
    <td width="36%" height="38">adds only if no duplicates are found - leaves
      item in temporary table.</td>
    <td width="15%" height="38">all required fields</td>
    <td width="19%" height="38">&nbsp;returns duplicate id and temp_id</td>
        </tr>
        <tr>
    <td width="20%" height="38">UNIQUE_DEL_TEMP</td>
    <td width="36%" height="38">adds only if no duplicates are found and removes
      from temp table.</td>
    <td width="15%" height="38">all required fields</td>
    <td width="19%" height="38">&nbsp;returns duplicate id a</td>
        </tr>
        <tr>
    <td width="20%" height="38">USER_INPUT</td>
    <td width="36%" height="38">returns duplicate_ids and temp_id for user
      decision on dups action</td>
    <td width="15%" height="38">all required fields</td>
    <td width="19%" height="38">&nbsp;</td>
        </tr>
        <tr>
    <td width="20%" height="51">&nbsp;NEW_BATCH</td>
    <td width="36%" height="51">adds a new batch if for the temp_id sent with
      the request</td>
    <td width="15%" height="51">requires reg_id and temp_id - no other required
      fields</td>
    <td width="19%" height="51"></td>
        </tr>
        <tr>
    <td width="20%" height="38">NEW_SALT</td>
    <td width="36%" height="38">adds a new batch if for the temp_id sent with
      the request</td>
    <td width="15%" height="38">requires reg_id and temp_id - no other required
      fields</td>
    <td width="19%" height="38">&nbsp;</td>
        </tr>
      </table>
          </td>
        </tr>
        <tr>
          <td><!--embed src="/CFWTEMP/<%=Application("appkey")%>/regTEMP/mt.cdx" align="baseline" border="0"  width="300" height="200" type="chemical/x-cdx" name="CDP" id="2"><noembed>The ChemDraw Plugin is required to view this page properly</noembed--><p>
            For data integrity the following required fields are automatically populated with values specified
            reg_post_action.asp:setdataintegritydefaults, unless they are
            explicitly sent with the post.
            </p>
            <table border="1" width="100%">
  <tr>
                <td width="33%">Temporary_Structures.temp_compound_id</td>
                <td width="11%"><input type = "text" name = "Temporary_Structures.temp_compound_id" size="5"></td>
                <td width="56%">*required for NEW_SALT</td>
              </tr>
             
 <tr>
                <td width="33%">Temporary_Structures.cpd_internal_id</td>
                <td width="11%"><input type = "text" name = "Temporary_Structures.cpd_internal_id" size="5"></td>
                <td width="56%">*required for NEW_SALT</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.reg_id&nbsp;</td>
                <td width="11%"><input type = "text" name = "Temporary_Structures.reg_ID" size="5"></td>
                <td width="56%">*required for NEW_BATCH</td>
              </tr>
              <tr>
                <td width="33%">&nbsp;</td>
                <td width="11%">&nbsp;</td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Scientist_ID</td>
                <td width="11%"><input type = "text" name = "Temporary_Structures.Scientist_ID" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Sequence_ID</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Sequence_ID" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Project_ID</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Project_ID" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Compound_Type</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Compound_Type" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Notebook_Text</td>
                <td width="11%"><input type = "text"   name = "Temporary_Structures.Notebook_Text" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">Temporary_Structures.Salt_Code</td>
                <td width="11%"><input type = "text"  name = "Temporary_Structures.Salt_Code" size="5"></td>
                <td width="56%">&nbsp;</td>
              </tr>
              <tr>
                <td width="33%">&nbsp;</td>
                <td width="11%">&nbsp;</td>
                <td width="56%">&nbsp;</td>
              </tr>
            </table>
            <p>You can also send any other fields in the temporary table that
            you want to populate.&nbsp; The must be sent as
            Temporary_Structures.Whatever</p>
 <table border="1">
            <tr>
              <td><script language = "javascript">
				var embedString = "<embed name='cdpstructure3' src='/CFWTEMP/<%=Application("appkey")%>/regTEMP/mt.cdx' align='baseline' border='0' width='400' height='300' id='3' type='chemical/x-cdx' viewonly='false'>"
				cd_insertObjectStr(embedString)
			</script></td>
              <td><table border="1">
                <tr>
                  <td>Temporary_Structures.Structure:<br>
                    <input TYPE="text" name="Temporary_Structures.Structure" value size="20">
                    &nbsp;<br>
                  this is the name of the field for where the base64 string from the plugin is put when you
                  click search.&nbsp;</td>
                  <td>*required for ALL BUT NEW_BATCH and NEW_SALT</td>
                  <td></td>
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
        <tr>
          <td><input TYPE="Submit" VALUE="REG_PERM" ONCLICK="GetCDX3(form3)">
            this button is linked to javascript that puts the base64 string into
            the temporary_structures.structure field.</td>
        </tr>
      </table>
      </tr>
      &nbsp;
      <p>Returns reg_number
      <p>&nbsp;
  </table>  </td></tr></table>
</form>
<p>
</p>


</body>
</html>
