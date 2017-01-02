<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:variable name="Vlower" select="'abcdefghijklmnopqrstuvwxyz'"/>
  <xsl:variable name="VUPPER" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <xsl:variable name="VProjectList" select="ProjectList"/>
    <xsl:variable name="VProjectBatchList" select="BatchList/Batch/ProjectList"/>
    <xsl:variable name="VIdentifierList" select="IdentifierList"/>
    <xsl:variable name="VIdentifierBatchList" select="BatchList/Batch/IdentifierList"/>
    <xsl:variable name="VPropertyList" select="PropertyList"/>
    <xsl:variable name="VBatchList_Batch" select="BatchList/Batch"/>
    <xsl:for-each select="BatchList/Batch">
      <VW_TemporaryBatch>
        <ROW>
          <TEMPBATCHID>
            <xsl:value-of select="BatchID"/>
          </TEMPBATCHID>
          <BATCHNUMBER>
            <xsl:value-of select="BatchNumber"/>
          </BATCHNUMBER>
          <DATECREATED>
            <xsl:value-of select="DateCreated"/>
          </DATECREATED>
          <PERSONCREATED>
            <xsl:value-of select="PersonCreated"/>
          </PERSONCREATED>
          <DATELASTMODIFIED>
            <xsl:value-of select="DateLastModified"/>
          </DATELASTMODIFIED>
          <SEQUENCEID>
            <xsl:value-of select="/MultiCompoundRegistryRecord/RegNumber/SequenceID"/>
          </SEQUENCEID>
          <STATUSID>
            <xsl:value-of select="StatusID"/>
          </STATUSID>
          <STRUCTUREAGGREGATION>
            <xsl:copy-of select="StructureAggregation"/>
          </STRUCTUREAGGREGATION>
          <xsl:for-each select="PropertyList/Property">
            <!-- get the element value -->
            <xsl:variable name="eValue" select="."/>
            <!-- get the element name -->
            <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
            <!-- conditionally create the element -->
            <xsl:choose>
              <xsl:when test="$eName = 'DELIVERYDATE' and string-length($eValue) != 0">
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:when>
              <xsl:when test="$eName = 'DATEENTERED' and string-length($eValue) != 0">
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:when>
              <xsl:otherwise>
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:for-each>
          <PROJECTXML>
            <XMLFIELD>
              <xsl:copy-of select="$VProjectList"/>
            </XMLFIELD>
          </PROJECTXML>
          <PROJECTXMLBATCH>
            <XMLFIELD>
              <xsl:copy-of select="$VProjectBatchList"/>
            </XMLFIELD>
          </PROJECTXMLBATCH>
          <IDENTIFIERXML>
            <XMLFIELD>
              <xsl:copy-of select="$VIdentifierList"/>
            </XMLFIELD>
          </IDENTIFIERXML>
          <IDENTIFIERXMLBATCH>
            <XMLFIELD>
              <xsl:copy-of select="$VIdentifierBatchList"/>
            </XMLFIELD>
          </IDENTIFIERXMLBATCH>
          <xsl:for-each select="$VPropertyList/Property">
            <!-- get the element value -->
            <xsl:variable name="eValue" select="."/>
            <!-- get the element name -->
            <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
            <!-- create the element -->
            <xsl:element name="{$eName}">
              <xsl:value-of select="$eValue"/>
            </xsl:element>
          </xsl:for-each>
        </ROW>
      </VW_TemporaryBatch>

      <xsl:for-each select="ProjectList/Project">
        <VW_TemporaryBatchProject>
          <ROW>
            <TEMPBATCHID>0</TEMPBATCHID>
            <PROJECTID>
              <xsl:value-of select="ProjectID"/>
            </PROJECTID>
          </ROW>
        </VW_TemporaryBatchProject>
      </xsl:for-each>

    </xsl:for-each>
    <xsl:for-each select="ComponentList/Component">
      <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
      <xsl:for-each select="Compound">
        <VW_TemporaryCompound>
          <ROW>
            <TEMPCOMPOUNDID>0</TEMPCOMPOUNDID>
            <TEMPBATCHID>0</TEMPBATCHID>
            <xsl:for-each select="RegNumber/RegID">
              <REGID>
                <xsl:choose>
                  <xsl:when test=".='0'">
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="."/>
                  </xsl:otherwise>
                </xsl:choose>
              </REGID>
            </xsl:for-each>
            <xsl:for-each select="BaseFragment/Structure">
              <BASE64_CDX>
                <xsl:value-of select="Structure"/>
              </BASE64_CDX>
              <STRUCTUREID>
                <xsl:value-of select="StructureID"/>
              </STRUCTUREID>
              <xsl:for-each select="PropertyList/Property">
                <!-- get the element value -->
                <xsl:variable name="eValue" select="."/>
                <!-- get the element name -->
                <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                <!-- create the element -->
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:for-each>
              <STRUCTIDENTIFIERXML>
                <XMLFIELD>
                  <xsl:copy-of select="IdentifierList"/>
                </XMLFIELD>
              </STRUCTIDENTIFIERXML>
            </xsl:for-each>
            <DATECREATED>
              <xsl:value-of select="DateCreated"/>
            </DATECREATED>
            <PERSONCREATED>
              <xsl:value-of select="PersonCreated"/>
            </PERSONCREATED>
            <DATELASTMODIFIED>
              <xsl:value-of select="DateLastModified"/>
            </DATELASTMODIFIED>
            <SEQUENCEID>
              <xsl:value-of select="RegNumber/SequenceID"/>
            </SEQUENCEID>
            <TAG>
              <xsl:value-of select="Tag"/>
            </TAG>
            <xsl:for-each select="PropertyList/Property">
              <!-- get the element value -->
              <xsl:variable name="eValue" select="."/>
              <!-- get the element name -->
              <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
              <!-- create the element -->
              <xsl:element name="{$eName}">
                <xsl:value-of select="$eValue"/>
              </xsl:element>
            </xsl:for-each>
            <PROJECTXML>
              <XMLFIELD>
                <xsl:copy-of select="$VProjectList"/>
              </XMLFIELD>
            </PROJECTXML>
            <xsl:for-each select="FragmentList">
              <FRAGMENTXML>
                <XMLFIELD>
                  <xsl:copy-of select="."/>
                </XMLFIELD>
              </FRAGMENTXML>
            </xsl:for-each>
            <xsl:for-each select="BatchComponentFragmentList">
              <BATCHCOMPONENTFRAGMENTXML>
                <XMLFIELD>
                  <xsl:copy-of select="."/>
                </XMLFIELD>
              </BATCHCOMPONENTFRAGMENTXML>
            </xsl:for-each>
            <xsl:for-each select="IdentifierList">
              <IDENTIFIERXML>
                <XMLFIELD>
                  <xsl:copy-of select="."/>
                </XMLFIELD>
              </IDENTIFIERXML>
            </xsl:for-each>
            <xsl:for-each select="Structure/NormalizedStructure">
              <NORMALIZEDSTRUCTURE>
                <xsl:copy-of select="."/>
              </NORMALIZEDSTRUCTURE>
            </xsl:for-each>
            <xsl:for-each select="BaseFragment/Structure/UseNormalization">
              <USENORMALIZATION>
                <xsl:value-of select="."/>
              </USENORMALIZATION>
            </xsl:for-each>
            <xsl:for-each select="$VBatchList_Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
              <xsl:for-each select="PropertyList/Property">
                <!-- get the element value -->
                <xsl:variable name="eValue" select="."/>
                <!-- get the element name -->
                <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                <!-- conditionally create the element -->
                <xsl:choose>
                  <xsl:when test="$eName = 'COMMENTS'">
                    <xsl:element name="BATCHCOMPONENTCOMMENTS">
                      <xsl:value-of select="$eValue"/>
                    </xsl:element>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:element name="{$eName}">
                      <xsl:value-of select="$eValue"/>
                    </xsl:element>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:for-each>
            </xsl:for-each>
          </ROW>
        </VW_TemporaryCompound>
      </xsl:for-each>
    </xsl:for-each>

    <xsl:for-each select="ProjectList/Project">
      <VW_TemporaryRegNumbersProject>
        <ROW>
          <TEMPBATCHID>0</TEMPBATCHID>
          <PROJECTID>
            <xsl:value-of select="ProjectID"/>
          </PROJECTID>
        </ROW>
      </VW_TemporaryRegNumbersProject>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>