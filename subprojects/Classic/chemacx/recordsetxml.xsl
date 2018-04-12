<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/TR/WD-xsl">
<!--<xsl:stylesheet xmlns:xsl="http://www.progsys.com">-->
  <xsl:template match="/">

  <HTML><HEAD><TITLE>XSL-Formatted ADO Recordset</TITLE></HEAD>
  <BODY><FONT FACE="Tahoma" size="2">

    <TABLE border="1" COLOR="BLUE">
      <TR>
        <xsl:for-each select="xml/s:Schema/s:ElementType/s:AttributeType">
          <TD>
	    <STRONG>
	      <FONT COLOR="RED" size="2">
	        <xsl:value-of select="@name"/>
	      </FONT>
	    </STRONG>
	  </TD>
        </xsl:for-each>
      </TR>

      <xsl:for-each select="xml/rs:data/z:row">
        <TR>
          <xsl:for-each select="@*">
            <TD>
	      <FONT COLOR="BLUE" size="2">
	        <xsl:value-of/>
	      </FONT>
	    </TD>
          </xsl:for-each>
        </TR>
      </xsl:for-each>

    </TABLE>

  </FONT></BODY></HTML>

  </xsl:template>
</xsl:stylesheet>