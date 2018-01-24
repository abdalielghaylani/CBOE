<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- the date this document was transformed -->
  <xsl:param name="TransformedOn"/>

  <!-- We're going to use the results as a stand-alone XML document -->
  <xsl:output method="xml" standalone="yes"/>

  <xsl:template match="/MultiCompoundRegistryRecord">

    <!-- NOTE!
    We could hard-code every element just by typing in 
      <MultiCompoundRegistryRecord> (or whatever the start tag is)
    instead of 
      <xsl:element name="MultiCompoundRegistryRecord">,
    but this keeps us from adding any useful attributes such as ID and count
    -->
    <xsl:element name="MultiCompoundRegistryRecord">

    <!-- root element attributes -->
    <xsl:attribute name="transformDate">
      <!-- variables and xsl parameters are accessed using the '$' prefix -->
      <xsl:value-of select="$TransformedOn"></xsl:value-of>
    </xsl:attribute>
    
    <!-- repository identifier -->
    <xsl:attribute name="id">
      <xsl:value-of select="ID"/>
    </xsl:attribute>

    <!-- root immediate children -->
    <xsl:element name="ID">
      <xsl:value-of select="ID"/>
    </xsl:element>

    <!-- mixture registration number -->
    <xsl:element name="RegNumber">
      <xsl:element name="RegID">
        <xsl:value-of select="ID"/>
      </xsl:element>
      <xsl:for-each select="RegNumber">
        <xsl:element name="RegNumber">
          <xsl:value-of select="RegNumber"/>
        </xsl:element>
      </xsl:for-each>
    </xsl:element>

    <!-- root property-list -->
    <xsl:for-each select="PropertyList">
      <xsl:element name="PropertyList">
        <!-- list length -->
        <xsl:attribute name="count">
          <xsl:value-of select="count(./Property)"/>
        </xsl:attribute>
        <!-- You can use the built-in 'node()' function to get the next DOM node:
        we can also simply use 'select="Property"' also, which is generally safer
        because it protects against reading non-'Property' elements (which shouldn't exist!)
        -->
        <xsl:for-each select="node()">
          <Property>
            <xsl:attribute name="name">
              <xsl:value-of select="@name"/>
            </xsl:attribute>
            <xsl:attribute name="friendlyName">
              <xsl:value-of select="@friendlyName"/>
            </xsl:attribute>
            <xsl:attribute name="pickListDomainID">
              <xsl:value-of select="@pickListDomainID"/>
            </xsl:attribute>
            <xsl:attribute name="pickListDisplayValue">
              <xsl:value-of select="@pickListDisplayValue"/>
            </xsl:attribute>
            <xsl:value-of select="."/>
          </Property>
        </xsl:for-each>
      </xsl:element>
    </xsl:for-each>

    <!-- root component-list -->
    <xsl:for-each select="ComponentList">
      <xsl:element name="ComponentList">
        <!-- list length -->
        <xsl:attribute name="count">
          <xsl:value-of select="count(./Component)"/>
        </xsl:attribute>

        <xsl:for-each select="Component/Compound">
          <xsl:element name="Component">

            <!-- repository identifier -->
            <xsl:attribute name="id">
              <xsl:value-of select="CompoundID"/>
            </xsl:attribute>

            <!-- compounent immediate children -->
            <xsl:element name="CompoundID">
              <xsl:value-of select="CompoundID"/>
            </xsl:element>

            <!-- compounent property-list -->
            <xsl:for-each select="PropertyList">
              <xsl:element name="PropertyList">
                <!-- list length -->
                <xsl:attribute name="count">
                  <xsl:value-of select="count(./Property)"/>
                </xsl:attribute>
                <xsl:for-each select="node()">
                  <Property>
                    <xsl:attribute name="name">
                      <xsl:value-of select="@name"/>
                    </xsl:attribute>
                    <xsl:attribute name="friendlyName">
                      <xsl:value-of select="@friendlyName"/>
                    </xsl:attribute>
                    <xsl:attribute name="pickListDomainID">
                      <xsl:value-of select="@pickListDomainID"/>
                    </xsl:attribute>
                    <xsl:attribute name="pickListDisplayValue">
                      <xsl:value-of select="@pickListDisplayValue"/>
                    </xsl:attribute>
                    <xsl:value-of select="."/>
                  </Property>
                </xsl:for-each>
              </xsl:element>
            </xsl:for-each>

            <!-- component registration number -->
            <xsl:element name="RegNumber">
              <xsl:for-each select="RegNumber">
                <xsl:element name="RegNumber">
                <xsl:value-of select="RegNumber"/>
                </xsl:element>
              </xsl:for-each>
            </xsl:element>

            <!-- component fragment information -->
            <xsl:for-each select="BaseFragment/Structure">
              <xsl:element name="BaseFragment">
                <xsl:element name="Structure">
                  <!-- repository identifier -->
                  <xsl:attribute name="id">
                    <xsl:value-of select="StructureID"/>
                  </xsl:attribute>
                  
                  <!-- nested Structure element -->
                  <xsl:for-each select="Structure">
                  <xsl:element name="Structure">
                    <xsl:attribute name="molWeight">
                      <xsl:value-of select="@molWeight"/>
                    </xsl:attribute>
                    <xsl:attribute name="formula">
                      <xsl:value-of select="@formula"/>
                    </xsl:attribute>
                  </xsl:element>
                </xsl:for-each>
              </xsl:element>
              </xsl:element>
            </xsl:for-each>

            <!-- component identifier-list -->
            <xsl:for-each select="IdentifierList">
              <!-- This is the simplest way to use entire source node 'as-is':
              It's also a bit less flexible if you want to add 'count' attributes
              such as 'CASCount="1"'
              -->
              <xsl:copy-of select="."/>
            </xsl:for-each>

          </xsl:element>
        </xsl:for-each>
      </xsl:element> <!-- END component list -->
    </xsl:for-each>

  </xsl:element>  <!-- END root element -->
  </xsl:template>

</xsl:stylesheet>