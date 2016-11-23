<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:param name="sbiFlag" select="defaultSBI"/>
  <xsl:param name="isEditableFlag" select="defaultIsEditable"/>
  <xsl:template match="/MIXTURE">
    <xsl:element name="MultiCompoundRegistryRecord">
      <xsl:attribute name="SameBatchesIdentity">
        <xsl:value-of select="$sbiFlag"/>
      </xsl:attribute>
      <xsl:attribute name="IsEditable">
        <xsl:value-of select="$isEditableFlag"/>
      </xsl:attribute>
      <xsl:element name="ID">
        <xsl:value-of select="TEMPBATCHID"/>
      </xsl:element>
      <xsl:element name="DateCreated">
        <xsl:value-of select="DATECREATED"/>
      </xsl:element>
      <xsl:element name="DateLastModified">
        <xsl:value-of select="DATELASTMODIFIED"/>
      </xsl:element>
      <xsl:element name="PersonCreated">
        <xsl:value-of select="PERSONCREATED"/>
      </xsl:element>
      <xsl:element name="StructureAggregation">
        <xsl:value-of select="STRUCTUREAGGREGATION"/>
      </xsl:element>
      <xsl:element name="RegNumber">
        <xsl:element name="RegID">0</xsl:element>
        <xsl:element name="SequenceNumber"/>
        <xsl:element name="RegNumber"/>
        <xsl:element name="SequenceID">
          <xsl:value-of select="SEQUENCEID"/>
        </xsl:element>
      </xsl:element>
      <xsl:copy-of select="REGIDENTIFIERS/IdentifierList"/>
      <xsl:element name="PropertyList">
        <xsl:for-each select="REGPROPLIST/REGPROPLIST_ROW/*">
          <xsl:element name="Property">
            <xsl:attribute name="name">
              <xsl:value-of select="name()"/>
            </xsl:attribute>
            <xsl:value-of select="."/>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
      <xsl:element name="ProjectList">
        <xsl:for-each select="REGPROJECTS/REGPROJECTS_ROW">
          <xsl:element name="Project">
            <xsl:element name="ID"></xsl:element>
            <xsl:element name="ProjectID">
              <xsl:attribute name="Description">
                <xsl:value-of select="DESCRIPTION"/>
              </xsl:attribute>
              <xsl:attribute name="Name">
                <xsl:value-of select="NAME"/>
              </xsl:attribute>
              <xsl:attribute name="Active">
                <xsl:value-of select="ACTIVE"/>
              </xsl:attribute>
              <xsl:value-of select="PROJECTID"/>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
      <!-- ComponentList -->
      <xsl:element name="ComponentList">
        <xsl:for-each select="COMPONENTS/COMPONENTS_ROW">
          <xsl:element name="Component">
            <xsl:element name="ID"></xsl:element>
            <xsl:element name="ComponentIndex">
              <xsl:value-of select="concat('-', TEMPCOMPOUNDID)"/>
            </xsl:element>
            <xsl:element name="Compound">
              <xsl:element name="CompoundID">
                <xsl:value-of select="TEMPCOMPOUNDID"/>
              </xsl:element>
              <xsl:element name="DateCreated">
                <xsl:value-of select="DATECREATED"/>
              </xsl:element>
              <xsl:element name="PersonCreated">
                <xsl:value-of select="PERSONCREATED"/>
              </xsl:element>
              <xsl:element name="DateLastModified">
                <xsl:value-of select="DATELASTMODIFIED"/>
              </xsl:element>
              <xsl:element name="Tag"/>
              <xsl:element name="PropertyList">
                <xsl:for-each select="COMPPROPLIST/COMPPROPLIST_ROW/*">
                  <xsl:element name="Property">
                    <xsl:attribute name="name">
                      <xsl:value-of select="name()"/>
                    </xsl:attribute>
                    <xsl:value-of select="."/>
                  </xsl:element>
                </xsl:for-each>
              </xsl:element>
              <xsl:element name="RegNumber">
                <xsl:element name="RegID">
                  <xsl:value-of select="REGID"/>
                </xsl:element>
                <xsl:element name="SequenceNumber"/>
                <xsl:element name="RegNumber"/>
                <xsl:element name="SequenceID">
                  <xsl:value-of select="SEQUENCEID"/>
                </xsl:element>
              </xsl:element>
              <xsl:element name="BaseFragment">
                <xsl:element name="Structure">
                  <xsl:element name="StructureID">
                    <xsl:value-of select="STRUCTUREID"/>
                  </xsl:element>
                  <xsl:element name="StructureFormat"/>
                  <xsl:element name="Structure">
                    <xsl:attribute name="molWeight">
                      <xsl:value-of select="FORMULAWEIGHT"/>
                    </xsl:attribute>
                    <xsl:attribute name="formula">
                      <xsl:value-of select="MOLECULARFORMULA"/>
                    </xsl:attribute>
                    <xsl:value-of select="STRUCTURE"/>
                  </xsl:element>
                  <xsl:element name="NormalizedStructure">
                    <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                  </xsl:element>
                  <xsl:element name="UseNormalization">
                    <xsl:value-of select="USENORMALIZATION"/>
                  </xsl:element>
                </xsl:element>
              </xsl:element>
              <!-- TODO: extract fragment details somehow -->
              <xsl:copy-of select="FRAGMENTXML/FragmentList"/>
              <!-- TODO: re-format Identifier details -->
              <xsl:copy-of select="IDENTIFIERXML/IdentifierList"/>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
      <!-- BatchList -->
      <xsl:element name="BatchList">
        <xsl:element name="Batch">
          <xsl:element name="BatchID">
            <xsl:value-of select="TEMPBATCHID"/>
          </xsl:element>
          <xsl:element name="BatchNumber">
            <xsl:value-of select="BATCHNUMBER"/>
          </xsl:element>
          <xsl:element name="DateCreated">
            <xsl:value-of select="DATECREATED"/>
          </xsl:element>
          <xsl:element name="PersonCreated">
            <xsl:value-of select="PERSONCREATED"/>
          </xsl:element>
          <xsl:element name="DateLastModified">
            <xsl:value-of select="DATELASTMODIFIED"/>
          </xsl:element>
          <xsl:element name="StatusID">
            <xsl:value-of select="STATUSID"/>
          </xsl:element>
          <xsl:element name="ProjectList">
            <xsl:for-each select="BATCHPROJECTS/BATCHPROJECTS_ROW">
              <xsl:element name="Project">
                <xsl:element name="ID"></xsl:element>
                <xsl:element name="ProjectID">
                  <xsl:attribute name="Description">
                    <xsl:value-of select="DESCRIPTION"/>
                  </xsl:attribute>
                  <xsl:attribute name="Name">
                    <xsl:value-of select="NAME"/>
                  </xsl:attribute>
                  <xsl:attribute name="Active">
                    <xsl:value-of select="ACTIVE"/>
                  </xsl:attribute>
                  <xsl:value-of select="PROJECTID"/>
                </xsl:element>
              </xsl:element>
            </xsl:for-each>
          </xsl:element>
          <xsl:copy-of select="BATCHIDENTIFIERS/IdentifierList"/>
          <xsl:element name="PropertyList">
            <xsl:for-each select="BATCHPROPLIST/BATCHPROPLIST_ROW/*">
              <xsl:element name="Property">
                <xsl:attribute name="name">
                  <xsl:value-of select="name()"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:element>
            </xsl:for-each>
          </xsl:element>
          <xsl:element name="BatchComponentList">
            <xsl:for-each select="COMPONENTS/COMPONENTS_ROW">
              <xsl:element name="BatchComponent">
                <xsl:element name="ID"/>
                <xsl:element name="BatchID">
                  <xsl:value-of select="TEMPBATCHID"/>
                </xsl:element>
                <xsl:element name="CompoundID">
                  <xsl:value-of select="TEMPCOMPOUNDID"/>
                </xsl:element>
                <xsl:element name="ComponentIndex">
                  <xsl:value-of select="concat('-', TEMPCOMPOUNDID)"/>
                </xsl:element>
                <xsl:element name="PropertyList">
                  <xsl:for-each select="BATCHCOMPPROPLIST/BATCHCOMPPROPLIST_ROW/*">
                    <xsl:element name="Property">
                      <xsl:attribute name="name">
                        <xsl:value-of select="name()"/>
                      </xsl:attribute>
                      <xsl:value-of select="."/>
                    </xsl:element>
                  </xsl:for-each>
                </xsl:element>
                <xsl:copy-of select="BATCHCOMPFRAGMENTXML/BatchComponentFragmentList"/>
              </xsl:element>
            </xsl:for-each>
          </xsl:element>
        </xsl:element>
      </xsl:element>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
