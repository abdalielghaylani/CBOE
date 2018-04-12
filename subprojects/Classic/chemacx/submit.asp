<style type="text/css">{  }
body, td { font-family: Verdana, arial, helvetica, sans-serif; font-size: x-small; }
tt, pre { font-family: monospace; }
sup, sub { font-size: 60%; }
</style>
<%@ LANGUAGE=VBScript %>
<%
' Determines whether the user is logged into ChemACX Pro or ChemACX Net
Session("IsNet") = Not Session("okOnService_10")
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<!--COWS1-->
	<head>
		<title>CambridgeSoft's ChemACX.com - Catalog submission form</title>
	</head>
	<body background="http://images.cambridgesoft.com/chemfinder/background_finderblue_1200.gif"
		leftmargin="9" topmargin="9">
		<!--#INCLUDE FILE = "banner.asp"-->
		<TABLE valign="top" width="600" cellpadding="2">
			<tr>
				<td></td>
				<td>
					<h2>ChemACX submission form</h2>
				</td>
			</tr>
			<tr>
				<td valign="top" width="5">
					<table cellpadding="0" cellspacing="1">
						<!--#INCLUDE FILE = "leftnavigationlist.asp"-->
					</table>
				</td>
				<td valign="top">
					<h3>The &quot;No Form&quot; Short Form</h3>
					<p>We want to include your catalog in ChemACX. We'd rather have your data without a 
						pretty form-based submission than not have your data at all.</p>
					<p>Send your catalog or product list to:</p>
					<p>		
					<script language=javascript>
					<!--
					{
						var linktext = "chemacx @ cambridgesoft.com";
						var email1 = "chemacx";
						var email2 = "cambridgesoft.com";
						document.write("<a href=" + "mail" + "to:" + email1 + "@" + email2 + ">" + linktext + "</a>")
					}
					-->
					</script>
					</p>
					<p>or</p>
					<p>ChemACX Submissions<br>
						CambridgeSoft Corporation<br>
						100 CambridgePark Drive<br>
						Cambridge, MA 02140 USA</p>
					<p>The email address is preferred, and our mailer should be able to handle rather 
						large files. If you need special arrangements, just drop us a note and we'll 
						see what we can do.</p>
					<p>Of course, we'd <i>prefer</i> that you filled out the long form below. And you 
						might also want to consider looking at our <a href="datatips.asp">tips for data 
							submission</a>. But neither of those are required.</p>
					<p>Note that we will assume that any data provided to us is provided without 
						restrictions -- the point is for as many people to know about you and your 
						products, after all. If you have any restrictions on our use of your data, be 
						sure to include complete details. We will absolutely honor any restrictions 
						that you require, but keep in mind that the safest way for us to honor any 
						restriction is to delete the data entirely and not include it in ChemACX at 
						all. Please consider carefully before placing restrictions on the use of your 
						data.&nbsp;
					</p>
					<hr>
					<h3>The (Not Very) Long Form</h3>
					<p>We want to include your catalog in ChemACX, and we would love to describe your 
						company accurately as well. The information in this form will help us present 
						you and your products in the best light possible.</p>
					<form method="POST" action="handlesubmission.asp" ID="Form1" enctype="multipart/form-data">
					<table width="100%">
						<tr>
							<td bgcolor="#C0C0C0"><b>Company Information</b>
							</td>
							<td bgcolor="#C0C0C0">(How should a potential purchaser contact you?)</td>
						</tr>
						<tr>
							<td colspan="2">
								<table ID="Table2">
									<tr>
										<td valign="bottom" align="right">Company&nbsp;Name:
										</td>
										<td colspan="5" valign="bottom">
											<input type="text" name="companyname" size="50" ID="Text1"></td>
									</tr>
									<tr>
										<td valign="bottom" align="right">Address:</td>
										<td colspan="5" valign="bottom">
											<input type="text" name="address1" size="50" ID="Text2"></td>
									</tr>
									<tr>
										<td valign="bottom" align="right">&nbsp;</td>
										<td colspan="5" valign="bottom">
											<input type="text" name="address2" size="50" ID="Text3"></td>
									</tr>
									<tr>
										<td valign="bottom" align="right">City:</td>
										<td valign="bottom">
											<input type="text" name="city" size="15" ID="Text4"></td>
										<td valign="bottom" align="right">
											State:</td>
										<td valign="bottom">
											<input type="text" name="state" size="5" ID="Text5"></td>
										<td valign="bottom" align="right">
											Postal&nbsp;Code:</td>
										<td valign="bottom">
											<input type="text" name="zip" size="8" ID="Zip1"></td>
									</tr>
									<tr>
										<td valign="bottom" align="right">Country:</td>
										<td valign="bottom">
											<input type="text" name="country" size="15" ID="Text6"></td>
										<td valign="bottom" align="right">
											&nbsp;</td>
										<td valign="bottom">
											&nbsp;</td>
										<td valign="bottom">
											&nbsp;</td>
										<td valign="bottom">
											&nbsp;</td>
									</tr>
									<tr>
										<td valign="bottom" align="right">Phone:</td>
										<td valign="bottom">
											<input type="text" name="phone" size="15" ID="Text7"></td>
										<td valign="bottom" align="right">
											Fax:</td>
										<td colspan="3" valign="bottom">
											<input type="text" name="fax" size="25" ID="Text8"></td>
									</tr>
									<tr>
										<td valign="bottom" align="right">Email:</td>
										<td valign="bottom">
											<input type="text" name="email" size="15" ID="Text9"></td>
										<td valign="bottom" align="right">
											Website:</td>
										<td colspan="3" valign="bottom">
											<input type="text" name="website" size="25" ID="Text10"></td>
									</tr>
								</table>
								<br>
							</td>
						</tr>
						<tr>
							<td bgcolor="#C0C0C0"><b>Catalog Data</b>
							</td>
							<td bgcolor="#C0C0C0">(What 
								should we include in ChemACX?)</td>
						</tr>
						<tr>
							<td colspan="2">Please consider 
								looking at the <a href="datatips.asp">
									tips for data submission</a>, but that is not mandatory.<table ID="Table3">
									<tr>
										<td valign="top"><input type="radio" value="Contact me to discuss transferring the catalog data" name="catalogsource" ID="Radio1"></td>
										<td>Please contact me at the 
											email address below and we can discuss how to transfer the data</td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="catalogsource" value="I will mail the catalog to you" ID="Radio2"></td>
										<td>I will mail it to 
											<script language=javascript>
											<!--
											{
												var linktext = "chemacx @ cambridgesoft.com";
												var email1 = "chemacx";
												var email2 = "cambridgesoft.com";
												document.write("<a href=" + "mail" + "to:" + email1 + "@" + email2 + ">" + linktext + "</a>")
											}
											-->
											</script>
										</td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="catalogsource" checked value="I am uploading the catalog now" ID="Radio3"></td>
										<td>I am uploading it via this 
											form: <input type="file" name="catalogfile" size="30" ID="File1"></td>
									</tr>
								</table>
								<br>
							</td>
						</tr>
						<tr>
							<td bgcolor="#C0C0C0"><b>Catalog Updates</b>
							</td>
							<td bgcolor="#C0C0C0">(How often does your catalog change?)</td>
						</tr>
						<tr>
							<td colspan="2">If possible, please add us to your list of people who are 
								automatically notified (via postal mail or email) when your catalog is updated. 
								The best email and postal addresses for ChemACX are listed at the top of this 
								page. If we haven't heard from you in a while, we will try to contact you 
								proactively on our own, but it helps if we know roughly how often you update 
								your catalog in general.<br>
								&nbsp;<table ID="Table4">
									<tr>
										<td>Our catalog is updated every <input type="text" name="updatefreq" size="22" value="[time period]" ID="Text11">
											or so.</td>
									</tr>
								</table>
								<br>
							</td>
						</tr>
						<tr>
							<td bgcolor="#C0C0C0"><b>Logo</b></td>
							<td bgcolor="#C0C0C0">&nbsp;</td>
						</tr>
						<tr>
							<td colspan="2">
								<table ID="Table5">
									<tr>
										<td valign="top">
											<input type="radio" checked value="No logo" name="logo" ID="Radio4"></td>
										<td>
											Do not display our logo</td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="logo" value="Contact me to discuss the logo" ID="Radio5"></td>
										<td>Please contact me at the 
											email address below and we can discuss this further</td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="logo" value="I will contact you to discuss the logo" ID="Radio6"></td>
										<td>I will email
										<script language=javascript>
										<!--
										{
											var linktext = "chemacx @ cambridgesoft.com";
											var email1 = "chemacx";
											var email2 = "cambridgesoft.com";
											document.write("<a href=" + "mail" + "to:" + email1 + "@" + email2 + ">" + linktext + "</a>")
										}
										-->
										</script>
										and discuss this further</td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="logo" value="Use our logo from the web" ID="Radio7"></td>
										<td>Please use the logo at this 
											web location: <input type="text" name="logourl" size="30" value="http://" ID="Text12"></td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="logo" value="I am uploading the logo to you" ID="Radio8"></td>
										<td>Please use the logo that I am 
											uploading via this form: <input type="file" name="logofile" size="12" ID="File2"></td>
									</tr>
								</table>
								<br>
							</td>
						</tr>
						<tr>
							<td bgcolor="#C0C0C0"><b>Restrictions</b></td>
							<td bgcolor="#C0C0C0">(Are there any limits on what we can do with your data?)</td>
						</tr>
						<tr>
							<td colspan="2"><p>CambridgeSoft will assume that any data provided to us is provided 
									without restrictions -- the point is for as many people to know about you and 
									your products, after all. If you have any restrictions on our use of your data, 
									be sure to include complete details. We will absolutely honor any restrictions 
									that you require, but keep in mind that the safest way for us to honor any 
									restriction is to delete the data entirely and not include it in ChemACX at 
									all. Please consider carefully before placing restrictions on the use of your 
									data.</p>
								<table ID="Table6">
									<tr>
										<td valign="top"><input type="radio" checked value="There are no restrictions on this data." name="restrictions" ID="Radio9"></td>
										<td>
											There are no restrictions. We want as many people as possible to know as much 
											about our products as possible</td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="restrictions" value="Contact me to discuss restrictions on this data." ID="Radio10"></td>
										<td>Please contact me at the email address below and we can discuss this further</td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="restrictions" value="I will contact you to discuss restrictions on this data." ID="Radio11"></td>
										<td>I will email
											<script language=javascript>
											<!--
											{
												var linktext = "chemacx @ cambridgesoft.com";
												var email1 = "chemacx";
												var email2 = "cambridgesoft.com";
												document.write("<a href=" + "mail" + "to:" + email1 + "@" + email2 + ">" + linktext + "</a>")
											}
											-->
											</script>
											and discuss this further</td>
									</tr>
									<tr>
										<td valign="top">
											<input type="radio" name="restrictions" value="There are the following restrictions on this data:" ID="Radio12"></td>
										<td>CambridgeSoft may use our the data we provide with the following restrictions:<br>
											<textarea rows="10" name="restrictionstext" cols="50" wrap="virtual" ID="Textarea1"></textarea></td>
									</tr>
								</table>
								<br>
							</td>
						</tr>
						<tr>
							<td bgcolor="#C0C0C0"><b>Personal Information</b></td>
							<td bgcolor="#C0C0C0">(Who should we contact with questions about this form?)
							</td>
						</tr>
						<tr>
							<td colspan="2">This information is used during the production of ChemACX only and 
								is never displayed to users of the database.<table ID="Table7">
									<tr>
										<td valign="bottom">
											Your &nbsp;Name:
										</td>
										<td colspan="5" valign="bottom" style="font-family: Verdana, arial, helvetica, sans-serif; font-size: x-small">
											<input type="text" name="techname" size="40" ID="Text13"></td>
									</tr>
									<tr>
										<td valign="bottom">
											Your email address or telephone number:</td>
										<td colspan="5" valign="bottom">
											<input type="text" name="techemail" size="40" ID="Text14"></td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td bgcolor="#C0C0C0"><b>Submit</b></td>
							<td bgcolor="#C0C0C0">&nbsp</td>
						</tr>
						<tr>
							<td colspan="2">
								<p><input type="submit" value="Submit" name="B1" ID="Submit1"><input type="reset" value="Reset" name="B2" ID="Reset1"></p>
							</td>
						</tr>
					</table>
				</td></tr>
		</TABLE>
		</form> </td> </tr> </TABLE>
	</body>
</html>
