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
<title>CambridgeSoft's ChemACX.com - Production process</title>
</head>


<body background="http://images.cambridgesoft.com/chemfinder/background_finderblue_1200.gif" leftmargin="9" topmargin="9">

<!--#INCLUDE FILE = "banner.asp"-->

<TABLE valign="top" width="600" cellpadding="2">
<tr><td></td>
<td>
<h2>ChemACX production process</h2>
</td>
</tr>
<tr>
<td valign=top width="5">
<table cellpadding="0" cellspacing="1">

<!--#INCLUDE FILE = "leftnavigationlist.asp"-->

</table>

</td>
<td valign=top>

It is becoming more and more common for chemical vendors to provide electronic 
versions of their catalogs. ChemACX offers some value by collecting many of 
those catalogs in one location, but it is not a big challenge simply to produce 
a collection of catalogs produced by others. The main value offered by ChemACX 
is not in accumulating catalogs, but rather in consolidating the information 
stored in them and reconciling the many different ways that information is 
presented by the various chemical vendors. Appropriately, the majority of the 
ChemACX production effort is spent in this consolidation and reconciliation 
process.
<h3>Analysis</h3>
<p>When a new catalog is obtained for inclusion in ChemACX, the first step is to 
understand the format in which it is provided. Ultimately, a chemical catalog is 
a collection of information about individual chemical substances -- bottles and 
jars and other containers of physical stuff that can be sold and shipped to an 
interested chemist. The original data, however, might be provided to us in many 
forms, including ChemFinder databases, SD Files, Excel spreadsheets, and many 
others. We must take that data and associate it with the actual items that are 
available for sale. In general, we are looking for four general types of 
information, not all of which are necessarily available in any given catalog:</p>
<ul>
  <li><u>Identity</u>: What substance is being sold? There are many ways to 
  identify a substance, and we will consider just about any information that is 
  provided. Most catalogs have at least one chemical name or a chemical 
  structure. Many catalogs have a collection of synonyms. Various registry 
  numbers, including CAS RNs and EINECS numbers are also common. Any of these 
  items can be used to identify what substance is being sold.</li>
  <li><u>Packaging</u>: How much of the substance is being delivered, and in 
  what purity? Some catalogs don't provide packaging information; they simply 
  state that a given substance is available, without saying if milligrams are 
  available or tons, in 95% purity or 99.9999%.</li>
  <li><u>Pricing</u>: How much will this substance cost? Pricing information is 
  necessarily dependent on packaging; there is no point in saying that a 
  substance will cost $100 without saying whether the chemist will receive 100 
  mg or 100 g for that price. In our experience, chemists seem much more likely 
  to purchase from vendors who list pricing information, and we strongly 
  encourage vendors to include this information with their catalogs. We will 
  preserve multiple currencies when we receive catalogs that contain multiple 
  currencies to start with.</li>
  <li><u>Properties</u>: What is known about the substance? Property information 
  helps describe a given substance, but does not identify it uniquely. It might 
  range from simple properties such as a chemical formula or molecular weight, 
  to physical properties that are measured experimentally, such as boiling point 
  and melting point. Physical properties are particularly useful when chemists 
  are looking for a substance that matches a certain set of criteria, and do not 
  necessarily have a specific substance in mind. We strongly encourage vendors 
  to provide whatever physical property information they have, as its presence 
  will only help chemists find (and buy) the items they are interested in. We 
  will preserve property information as it is provided by vendors in their 
  catalogs, and we will also accumulate that information across multiple 
  catalogs. For example, one vendor might list the melting point of a substance, 
  while another vendor lists its boiling point. In ChemACX, both properties will 
  bring the user to the same substance, with references to both catalogs.</li>
</ul>
<p>Our first step is simply to identify the various types of information in each 
catalog. For chemical names, it is common to have several names strung together, 
such as &quot;methylene chloride; dichloromethane&quot;. Or, a &quot;name&quot; field might contain 
other information that describes how the substance is provided rather than just 
the name itself, such as &quot;Benzene, 99%, HPLC grade&quot;. We also will frequently see 
registry numbers listed as &quot;names&quot;. We must take the catalog information 
provided to us, and make sure that we can provide it in a standardized format, 
so that a name truly is a name when viewed by the eventual user of ChemACX.</p>
<p>Registry numbers have other considerations. CAS RNs always have at least 5 
digits; EINECS numbers always have 7. If something claims to be a CAS RN but has 
fewer than 5 digits (or has a letter in it), we know that there is something 
wrong, and we can investigate further. CAS RNs also have an internal checksum 
that helps identify when one has been mistyped: 50-00-0 is a valid CAS RN, but 
there is no substance that could possibly have a CAS RN of 50-00-1.</p>
<p>Similar checks are performed against all other data types. Items sold in 
units of &quot;100 mh&quot; (instead of &quot;100 mg&quot;) will be questioned, as will items 
selling for negative dollars. Boiling points must always be greater than melting 
points. Values for pKas are usually in the range of -2 to 70. We perform many 
checks against all data, even before it is incorporated into ChemACX. By 
watching for issues such as these, we make sure that the overall quality of the 
final ChemACX database is greater than that of any individual catalog within it. 
We would also be very happy to return this information to any vendor that tells 
us where to send it, so that they might improve all of their future catalogs, 
and not just the version that is included in ChemACX.</p>
<h3>Consolidation</h3>
<p>Once we have reviewed the data in a new catalog, we can then consider merging 
it into ChemACX -- but that process is not simple either. In ChemACX, all 
catalogs are combined based on the substances available for sale. So, ChemACX 
lists a single entry for benzene, and from that one entry are references to 
every catalog that sells benzene in some form. The problem, then, is deciding 
what a given substance actually is. To do this, we compare <i>all</i> identity 
information in a catalog -- all names, all registry numbers, all chemical 
structures -- against all of the corresponding information already in ChemACX.</p>

<p>One possibility is that all of the identity information for a new substance 
matches all of the identity information for an existing substance. That is, the 
names match, any synonyms match, the registry numbers match if present, and the 
structures match if present. In that case, we can be pretty confident that the 
substance in the new catalog really is a match to the existing substance already 
in ChemACX. The new catalog information can be added to the existing ChemACX 
record.</p>

<p>Another possibility is that none of the identity information matches anything 
in ChemACX. In that case, we have to assume that the substance is new, and not 
yet in ChemACX. It will be added as a brand new entry.</p>

<p>Unfortunately those aren't the only possibilities.</p>

<h3>Reconciliation</h3>

<p>The worst case is if some of the information in the new substance matches one 
substance in ChemACX, and other information matches a <i>different</i> 
substance. All of the above analysis steps are designed to prevent this as much as 
possible, but sometimes it is unavoidable, due to errors (or more commonly 
ambiguities!) in the new catalog or in ChemACX. Each of these cases are reviewed 
by hand by trained chemists to prevent inaccurate data from corrupting the 
ChemACX database. In a simple example, 5 synonyms might match one record, while 
one other synonym matches a different record. If so, it's likely that the five 
synonyms are accurate, and there is a problem with the sixth. Other cases are 
more complex, and can be resolved only through our experience in knowing the 
types of errors that are likely to appear in catalogs. Sometimes, the only way 
to resolve the issue is to contact the vendor directly (&quot;Name: Benzene. Synonym: 
Toluene&quot;), and we will do that if we have appropriate contact information.
<h3>
Completion</h3>

<p>Before the ChemACX database is released for public access, we also take the 
opportunity to add data that isn't present in <i>any</i> chemical vendor's 
catalog. CambridgeSoft provides many databases, and we will try to supplement 
existing records in ChemACX with as much supporting information (more synonyms, 
more physical properties) as we can find. We can also produce chemical 
structures in many cases, either from other databases or directly from the 
chemical names using our custom-developed
<a href="http://pubs3.acs.org/acs/journals/doilookup?in_doi=10.1021/ci990062c">
Name=Struct</a> technology.<p>Every action we take is designed to produce a 
high-quality database that is more complete -- and more accurate -- than any 
individual chemical catalog can be.
</td></tr></table>
</td></tr></table>
<p>&nbsp;</p>

</td>
</tr>
</table>
</body>
</html>



