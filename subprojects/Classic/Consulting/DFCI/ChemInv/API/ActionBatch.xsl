<?xml version='1.0'?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:variable name="actionType" select="CHEMINVACTIONBATCH/@actionType"/>
  <xsl:template match="/">
    <HTML>
	  <HEAD>
	    <script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
	  </HEAD>	
      <BODY>
		<table border="0" cellspacing="0" cellpadding="0" width="600">
			<tr>
				<td valign="top">
					<img src="../graphics/cheminventory_banner.gif" border="0"/>
				</td>
			</tr>
		</table>
		<span class="GUIFeedback">
			<p>
				<xsl:choose>
				   <xsl:when test="CHEMINVACTIONBATCH/@actionType[.='test']">
				      Following are the results of your test:
				   </xsl:when>
				   <xsl:when test="CHEMINVACTIONBATCH/@actionType[.='commit']">
				      The following actions have been performed:
				   </xsl:when>
				   <xsl:otherwise>
				      The following actions will be performed:
				   </xsl:otherwise>
				</xsl:choose>
			</p>
		</span>
		<xsl:apply-templates select="CHEMINVACTIONBATCH/CREATESUBSTANCE[1]" />
        <xsl:apply-templates select="CHEMINVACTIONBATCH/CREATECONTAINER[1]" />
		<table>
			<tr>
			<td width="600" align="right"><BR/>
			<xsl:choose>
			   <xsl:when test="CHEMINVACTIONBATCH/@actionType[.='test']">
			    <a class="MenuLink" href="#" onclick="history.go(-2); return false">Back</a>
				<xsl:choose>
					<xsl:when test="CHEMINVACTIONBATCH/@ErrorsFound[.='False']"> 
						| <a class="MenuLink" href="/cheminv/api/ProcessActionBatch.asp?action=commit">Commit</a>
					</xsl:when>
				</xsl:choose>			   
			   </xsl:when>
			   <xsl:when test="CHEMINVACTIONBATCH/@actionType[.='commit']">
			      <a class="MenuLink" href="#" onclick="window.close(); return false">Close</a> | <a class="MenuLink"><xsl:attribute name="href">/cheminv/cheminv/BrowseInventory_frset.asp?gotoNode=<xsl:value-of select="CHEMINVACTIONBATCH/CREATECONTAINER/@LocationID"/>&amp;SelectContainer=<xsl:value-of select="CHEMINVACTIONBATCH/CREATECONTAINER/RETURNCODE"/></xsl:attribute>View</a>
			   </xsl:when>
			   <xsl:otherwise>
				  <a class="MenuLink" href="#" onclick="history.back(); return false">Back</a> | <a class="MenuLink" href="/cheminv/api/ProcessActionBatch.asp?action=commit">Commit</a>
			   </xsl:otherwise>
			</xsl:choose>
			</td>
		</tr>	
		</table>
		<p><xsl:value-of select="CHEMINVACTIONBATCH/@timeStamp"/></p>
      </BODY>
    </HTML>
  </xsl:template>
  <xsl:template match="CREATECONTAINER[1]">
	<TABLE BORDER="0" cellspacing="1" cellpadding="4">
          <TR>
			<Th colspan="10">Create Container</Th>
          </TR>
          <TR>
            <Th>LocationID</Th>
            <Th>Copies</Th>
            <th>Barcode</th>
            <Th>Size</Th>            
            <Th>Amount</Th>
            <Th>UnitID</Th>
            <Th>Container Type</Th>
            <Th>Container Name</Th>
            <Th>RegID</Th>
            <Th>Batch Number</Th>
            <xsl:choose>
			   <xsl:when test="$actionType[.='test']">
			     <Th>TEST RESULTS</Th>
			   </xsl:when>
			   <xsl:when test="$actionType[.='commit']">
			      <Th>RESULTS</Th>
			   </xsl:when>
			</xsl:choose>
          </TR>
          <xsl:for-each select="/CHEMINVACTIONBATCH/CREATECONTAINER">
            <TR> 
              <TD><xsl:value-of select="@LocationID"/></TD>
              <TD><xsl:value-of select="OPTIONALPARAMS/NUMCOPIES"/></TD>
		<td><xsl:value-of select="OPTIONALPARAMS/BARCODE"/></td>
              <TD><xsl:value-of select="@MaxQty"/></TD>              
              <TD><xsl:value-of select="@InitialQty"/></TD> 
              <TD><xsl:value-of select="@UOMName"/></TD> 
              <TD><xsl:value-of select="@ContainerTypeName"/></TD>  
			  <TD><xsl:value-of select="OPTIONALPARAMS/CONTAINERNAME"/></TD>
              <TD><xsl:value-of select="OPTIONALPARAMS/REGID"/></TD>
              <TD><xsl:value-of select="OPTIONALPARAMS/BATCHNUMBER"/></TD>
			  <xsl:choose>
				<xsl:when test="$actionType[.='test']">
				  <Td><font size="-2"><b><font color="green"><xsl:value-of select="RETURNVALUE/@result"/></font>: <xsl:value-of select="RETURNVALUE"/></b></font></Td>
				</xsl:when>
				<xsl:when test="$actionType[.='commit']">
				   <Td><font size="-2"><b><font color="green"><xsl:value-of select="RETURNVALUE/@result"/></font>: <xsl:value-of select="RETURNVALUE"/></b></font></Td>
				</xsl:when>
			  </xsl:choose>
            </TR>
          </xsl:for-each>	
	</TABLE>
	<BR></BR>        
  </xsl:template>
  <xsl:template match="CREATESUBSTANCE[1]">
	<TABLE BORDER="0">
          <TR>
			<Th colspan="3">Create Substance</Th>
          </TR>
          <TR>
            <Th>CAS</Th>
            <Th>ACX ID</Th>
            <Th>Substance Name</Th>
            
            <xsl:choose>
			   <xsl:when test="$actionType[.='test']">
			     <Th>TEST RESULTS</Th>
			   </xsl:when>
			   <xsl:when test="$actionType[.='commit']">
			      <Th>RESULTS</Th>
			   </xsl:when>
			</xsl:choose>
          </TR>
          <xsl:for-each select="/CHEMINVACTIONBATCH/CREATESUBSTANCE">
            <TR>
              <TD><xsl:value-of select="@CAS"/></TD>
              <TD><xsl:value-of select="@ACX_ID"/></TD>
              <TD><xsl:value-of select="substanceName"/></TD>  
			  <xsl:choose>
				<xsl:when test="$actionType[.='test']">
				  <Td><font size="-2"><b><font color="green"><xsl:value-of select="RETURNVALUE/@result"/></font>: <xsl:value-of select="RETURNVALUE"/></b></font></Td>
				</xsl:when>
				<xsl:when test="$actionType[.='commit']">
				   <Td><font size="-2"><b><font color="green"><xsl:value-of select="RETURNVALUE/@result"/></font>: <xsl:value-of select="RETURNVALUE"/></b></font></Td>
				</xsl:when>
			  </xsl:choose>
            </TR>
          </xsl:for-each>
	</TABLE>
	<BR></BR>        
  </xsl:template>
</xsl:stylesheet>
