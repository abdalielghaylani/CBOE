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
<title>CambridgeSoft's ChemACX.com - Search, find and buy chemicals online.</title>
</head>


<body background="http://images.cambridgesoft.com/chemfinder/background_finderblue_1200.gif" leftmargin="9" topmargin="9">

<!--#INCLUDE FILE = "banner.asp"-->

<TABLE valign="top" width="600" cellpadding="2">
<tr><td></td>
<td>
<h2>Tips for searching ChemACX</h2>
</td>
</tr>
<tr>
<td valign=top width="5">
<table cellpadding="0" cellspacing="1">

<!--#INCLUDE FILE = "leftnavigationlist.asp"-->

</table>

</td>
<td valign=top>

<p>
<a name="byname"></a><b>I want to find information on (some compound name).</b><br>
You can search for a compound by name by typing the name into the first box on the search form 
and leaving all other boxes empty. See <a href="#nohits">below</a> for suggestions of what to 
do if ChemACX returns no hits.</p>

<p>
<a name="nohits"></a><b>Help! ChemACX returned no hits.</b><br>
One of the most common reasons ChemACX would return no hits is because it has no information 
about the compound in question. If you think ChemACX should have information about your 
compound, there are several things to look at:
</p>
<ul>
<li>All searches</li>
<ul>
<li>If you are searching ChemACX Net, you are only looking at about 30 chemical catalogs -- a tiny fraction
of all of the chemicals available commercially. ChemACX Net is only meant as a demonstration, and isn't
a good choice for serious searching.  To search over 300 chemical catalogs, you should sign into
<a href="http://chemfinder.cambridgesoft.com/chemicals/chemacxpro.asp">ChemACX Pro</a> instead.
To find out if you are currently searching ChemACX Net or ChemACX Pro, look at the logo at the top
left corner of this page.</li>
</ul>
<li>Name searches</li>
<ul>
<li>Check your spelling.</li>
<li>Make sure that you have entered an English name.</li>
<li>Make sure you have entered the name of a chemical ('saltpeter', 'glucosamine') not a consumer 
product ('firecrackers', 'chicken', 'milk').</li>
<li>Don't add any adjectives to the chemical name. 'Hydrobromic acid' will work, but '10% 
hydrobromic acid solution' will not.
<li>Enter your search string exactly as you expect it to appear. For example, do not put 
the name in quote marks unless you feel that the quote marks are an integral part of the name.</li>
<li>Always enter double-prime marks with two single quotes, not one double quote mark.</li>
<li>Don't enter formulas as chemical names. Use a formula search instead.</li>
<li>Don't search for classes of compounds ('protein' or 'aldehyde')</li>
<li>Check your spelling!</li>
</ul>
<li>Molecular Weight searches</li>
<ul>
<li>Don't enter more significant figures than you are <em>sure</em> about.</li>
</ul>
<!--
<li>Structure searches</li>
<ul>
<li>Check that you have entered a valid SMILES string (unless you are using the 
<a href="http://chemstore.cambridgesoft.com/software/category.cfm?group=plugin">ChemDraw Plugin</a>). If you have 
not created your SMILES string using a chemical drawing package such as 
<a href="http://www.cambridgesoft.com/products/family.cfm?FID=2">ChemDraw</a>, it is probably not valid.</li>
<li>If you are not using the ChemDraw Plugin, you should. It makes structure searching 
<em>much</em> easier.</li>
</ul>
-->
<li>Combined searches</li>
<ul>
<li>Don't enter more data than is necessary to identify a compound. For example, 
if you are sure about a compound's name but unsure of its formula, leave the 
Formula field empty. ChemFinder does a boolean AND over all search terms, so an 
incorrect formula will cause an otherwise-valid name search to fail.</li>
</ul>
</ul>

<p>
<a name="invalidcas"></a><b>It says my CAS number is invalid!</b><br>
This is one case where ChemACX is unquestionably correct. All CAS numbers have 
a built-in <a href="http://chemfinder.cambridgesoft.com/about/chemfinder/errors.asp#invalidcas">checksum</a>. If the checksum 
check fails, the CAS number is impossible, and cannot refer to any compound. 
You will only get a message about an 'invalid' CAS number if its checksum fails.<p>

<p>
<a name="nohitcas"></a><b>I know my CAS number is correct, but it still returns no hits.</b><br>
Quite possible. The <a href="http://www.cas.org" target="_blank">Chemical Abstracts Service</a> 
has indexed (and assigned CAS numbers to) over 40 million substances. On the other 
hand, there are fewer than 1 million substances listed in ChemACX. 
With luck, the substances that are listed in ChemACX are the 
'important' or 'common' ones, but on a strictly statistical sense, there is a 
greater than 95% chance that a given CAS number will <em>not</em> be listed.</p>

<p>
<a name="twoentries"></a><b>Why are there two entries for limestone (for example)?</b><br>
ChemACX maintains a separate entry for each unique chemical entity it 
indexes. In some cases (as with limestone), several compounds share the same name. 
CAS No. 1317-65-3 refers to the actual sedimentary rock, which may contain various 
seashell fossils, impurities, and so on. CAS No. 471-34-1 refers to the pure substance 
Calcium Carbonate, which is often what people mean when they refer to limestone.<p>
In some cases it might not be clear from the entries at ChemACX 
what the exact differences are between two similarly-named entries with different 
CAS Numbers.  For more information about specific CAS Numbers, you will have to 
purchase an account at <a href="http://www.cas.org">CAS</a>.<p>
Similar apparent duplicates can show up in other cases, particularly related to 
chiral, racemic, and unspecified forms.</p>
</p>

</td>
</tr>
</table>
</body>
</html>



