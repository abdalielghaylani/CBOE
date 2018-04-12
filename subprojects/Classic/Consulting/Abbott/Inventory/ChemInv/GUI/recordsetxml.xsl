<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:s="uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882" xmlns:dt="uuid:C2F41010-65B3-11d1-A29F-00AA00C14882" xmlns:rs="urn:schemas-microsoft-com:rowset" xmlns:z="#RowsetSchema">
<!--<xsl:stylesheet xmlns:xsl="http://www.w3.org/TR/WD-xsl">-->
<!--<xsl:stylesheet xmlns:xsl="http://www.progsys.com">-->
  <xsl:template match="/">

  <HTML><HEAD><TITLE>XSL-Formatted ADO Recordset</TITLE></HEAD>
  <BODY><FONT FACE="Tahoma" size="2">

    <TABLE border="1" COLOR="BLUE">
      <TR>
        <xsl:for-each select="xml/s:Schema/s:ElementType/s:AttributeType">
          <TD>
		<a href="?sortBy={@name}">
			<xsl:value-of select="@name"/>
		</a>
	  </TD>
        </xsl:for-each>
      </TR>

      <xsl:for-each select="xml/rs:data/z:row">
        <TR>
          <xsl:for-each select="@*">
            <TD>
	      <FONT COLOR="BLUE" size="2">
			<xsl:value-of select="."/>
	      </FONT>
	    </TD>
          </xsl:for-each>
        </TR>
      </xsl:for-each>

    </TABLE>

  </FONT></BODY></HTML>

  </xsl:template>
</xsl:stylesheet>