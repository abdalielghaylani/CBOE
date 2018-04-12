<script language="javascript">
	/*function keepWhich(whichToKeep) {
		alert(whichToKeep);
		if (whichToKeep == "original") {
			<%session("KeepMofified")%> = false;
		}
		
		else if (whichToKeep == "modified") {
			<%session("KeepMofified")%> = true;
		}
	}*/
</script>
<%

Const kStrucUnchanged = "UNCHANGED"
Const kStrucModified = "MODIFIED"
Const kStrucFailed = "FAILED"




%>
<tr>
    <td colspan="2">
        <table border="0" bordercolor="" width="100%">
            <tr>
                <td align="left" <%=td_header_bgcolor%>><strong><font <%=font_header_default_2%>>Structure Normalization</font></strong></td>
                <td <%=td_header_bgcolor%> align="right"><font  color="green"><%ShowResult dbkey, formgroup,BaseRS, "Temporary_Structures.STATUS", "raw_no_edit", "8", "20"%></font></td>
            </tr>
            <tr>
                <td>
                    <table border="1"><tr><td>
                    <%ShowResult dbkey, formgroup,BaseRS, "Temporary_Structures.BASE64_CDX", struc_output, 325,220%>
                    
                    
                    </td></tr>
                    
                    <tr><td>Salt Stripped: <%ShowResult dbkey, formgroup,BaseRS, "Temporary_Structures.SALT_NAME", "raw_no_edit", "8", "20"%></td>
                    </tr>

                    <tr><td>Solvate Stripped: <%ShowResult dbkey, formgroup,BaseRS, "Temporary_Structures.SOLVATE_NAME", "raw_no_edit", "8", "20"%></td>
                    </tr>
                    </table>
                </td>
                <td valign="top">
                    <table>
                        <tr>
                            <td valign="top">
                                <p>
                                    The submitted structure has been automatically modified to conform to corporate standard drawing conventions.
                                </p>
                                <input name="KeepRadio" value="original" type="radio" onclick="javascript:alert('This selection will be lost when you leave the page. So Register now is recommended if you want to keep the original structure.')" /> Keep the original structure
                                <br />
                                <input name="KeepRadio" value="modified" type="radio" checked/> Keep the modfied structure
                                <br />
                            </td>
                         </tr>
                         
                         <tr>
							<td><a href="javascript:MainWindow.doReApply_ChemScript_Rules_Single(<%=BaseRS("temp_compound_id")%>)">Re-Apply ChemScript Rules</a></td>
                         </tr>
                         
                         <tr>
                            <td align="right">
                                <br /><br /><br /><br />
                                <%'stop%>
                                <a href="ViewChemScriptLog.asp?tempid=<%=BaseRS("temp_compound_id")%>" target="_new" >View ChemScript log</a>
                            </td>
                         </tr>
                     </table>
                </td>
            </tr>
        </table>
        
    </td>
</tr>
