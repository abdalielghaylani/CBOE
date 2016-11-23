<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:variable name="Vlower" select="'abcdefghijklmnopqrstuvwxyz'"/>
  <xsl:variable name="VUPPER" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:variable name="VMixture" select="."/>
      <!-- Mixture projects -->
      <!-- TODO: handle inserts and updates for this table -->

      <xsl:for-each select="ProjectList/Project">
        <xsl:variable name="VDeleteProject" select="@delete" />
        <xsl:variable name="VInsertProject" select="@insert" />
        <VW_TemporaryRegNumbersProject>
          <xsl:choose>
            <xsl:when test="$VDeleteProject='yes'">
              <xsl:attribute name="delete">
                <xsl:value-of select="@delete"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:when test="$VInsertProject='yes'">
              <xsl:attribute name="insert">
                <xsl:value-of select="@insert"/>
              </xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <xsl:element name="ROW">
            <xsl:element name="ID">
              <xsl:value-of select="ID" />
            </xsl:element>
            <xsl:if test="$VInsertProject='yes'">
              <xsl:element name="TEMPBATCHID">
                <xsl:value-of select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="ProjectID/@update='yes' or $VInsertProject='yes'">
              <xsl:element name="PROJECTID">
                <xsl:value-of select="ProjectID" />
              </xsl:element>
            </xsl:if>
          </xsl:element>
        </VW_TemporaryRegNumbersProject>
      </xsl:for-each>

      <xsl:for-each select="BatchList">
        <xsl:for-each select="Batch">
          <!-- Batch projects -->
          <xsl:for-each select="ProjectList/Project">
            <xsl:variable name="VDeleteProject" select="@delete" />
            <xsl:variable name="VInsertProject" select="@insert" />
            <VW_TemporaryBatchProject>
              <xsl:choose>
                <xsl:when test="$VDeleteProject='yes'">
                  <xsl:attribute name="delete">
                    <xsl:value-of select="@delete"/>
                  </xsl:attribute>
                </xsl:when>
                <xsl:when test="$VInsertProject='yes'">
                  <xsl:attribute name="insert">
                    <xsl:value-of select="@insert"/>
                  </xsl:attribute>
                </xsl:when>
              </xsl:choose>
              <xsl:element name="ROW">
                <xsl:element name="ID">
                  <xsl:value-of select="ID" />
                </xsl:element>
                <xsl:if test="$VInsertProject='yes'">
                  <xsl:element name="TEMPBATCHID">
                    <xsl:value-of select="BatchID" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="ProjectID/@update='yes' or $VInsertProject='yes'">
                  <xsl:element name="PROJECTID">
                    <xsl:value-of select="ProjectID" />
                  </xsl:element>
                </xsl:if>
              </xsl:element>
            </VW_TemporaryBatchProject>
          </xsl:for-each>
          <VW_TemporaryBatch>
            <xsl:element name="ROW">
              <xsl:element name="TEMPBATCHID">
                <xsl:value-of select="BatchID" />
              </xsl:element>
              <xsl:if test="BatchNumber/@update='yes'">
                <xsl:element name="BATCHNUMBER">
                  <xsl:value-of select="BatchNumber" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="DateCreated/@update='yes'">
                <xsl:element name="CREATED">
                  <xsl:value-of select="DateCreated" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="PersonCreated/@update='yes'">
                <xsl:element name="PERSONCREATED">
                  <xsl:value-of select="PersonCreated" />
                </xsl:element>
              </xsl:if>
              <xsl:element name="DATELASTMODIFIED">
                <xsl:value-of select="DateLastModified" />
              </xsl:element>
              <xsl:if test="StructureAggregation/@update='yes'">
                <xsl:element name="STRUCTUREAGGREGATION">
                  <xsl:value-of select="StructureAggregation" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="StatusID/@update='yes'">
                <xsl:element name="STATUSID">
                  <xsl:value-of select="StatusID" />
                </xsl:element>
              </xsl:if>
              <!-- Batch PropertyList -->
              <xsl:for-each select="PropertyList/Property[@update='yes' or @insert='yes']">
                <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                  <xsl:value-of select="normalize-space(.)"/>
                </xsl:element>
              </xsl:for-each>
              <!-- Mixture PropertyList -->
              <xsl:for-each select="$VMixture/PropertyList/Property[@update='yes' or @insert='yes']">
                <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                  <xsl:value-of select="normalize-space(.)"/>
                </xsl:element>
              </xsl:for-each>
              <!-- Mixture IdentifierList -->
              <xsl:if test="count($VMixture/IdentifierList/Identifier[@*]) > 0">
                <xsl:element name="IDENTIFIERXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="IdentifierList">
                      <xsl:for-each select="$VMixture/IdentifierList/Identifier[not(@delete)]">
                        <xsl:element name="Identifier">
                          <xsl:copy-of select="node()"/>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
              <!-- Batch IdentifierList -->
              <xsl:if test="count(IdentifierList/Identifier[@*]) > 0">
                <xsl:element name="IDENTIFIERXMLBATCH">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="IdentifierList">
                      <xsl:for-each select="IdentifierList/Identifier[not(@delete)]">
                        <xsl:element name="Identifier">
                          <xsl:copy-of select="node()"/>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
            </xsl:element>
          </VW_TemporaryBatch>
        </xsl:for-each>
      </xsl:for-each>

      <xsl:for-each select="ComponentList/Component">
        <xsl:variable name="VDelete" select="@delete"/>
        <xsl:variable name="VInsert" select="@insert"/>
        <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
        <xsl:for-each select="Compound">
          <VW_TemporaryCompound>
            <xsl:if test="$VDelete='yes'">
              <xsl:attribute name="delete">yes</xsl:attribute>
            </xsl:if>
            <xsl:if test="$VInsert='yes'">
              <xsl:attribute name="insert">yes</xsl:attribute>
            </xsl:if>
            <xsl:element name="ROW">
              <xsl:for-each select="RegNumber/RegID[$VInsert='yes']">
                <xsl:element name="REGID">
                  <xsl:if test=".!='0'">
                    <xsl:value-of select="."/>
                  </xsl:if>
                </xsl:element>
              </xsl:for-each>
              <xsl:element name="TEMPCOMPOUNDID">
                <xsl:value-of select="CompoundID"/>
              </xsl:element>
              <xsl:if test="CompoundID/@insert='yes' or $VInsert='yes'">
                <xsl:element name="TEMPBATCHID">
                  <xsl:value-of select="CompoundID"/>
                </xsl:element>
              </xsl:if>
              <!-- ? is this the correct logic ? -->
              <xsl:if test="BaseFragment/Structure/Structure/@update='yes' or $VInsert='yes'">
                <xsl:element name="BASE64_CDX">
                  <xsl:value-of select="BaseFragment/Structure/Structure"/>
                </xsl:element>
              </xsl:if>
              <!-- ? is this the correct logic ? -->
              <xsl:if test="BaseFragment/Structure/@update='yes' or $VInsert='yes'">
                <xsl:element name="STRUCTUREID">
                  <xsl:value-of select="BaseFragment/Structure/StructureID"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="DateCreated/@update='yes' or $VInsert='yes'">
                <xsl:element name="DATECREATED">
                  <xsl:value-of select="DateCreated"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="PersonCreated/@update='yes' or $VInsert='yes'">
                <xsl:element name="PERSONCREATED">
                  <xsl:value-of select="PersonCreated"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="DateLastModified/@update='yes' or $VInsert='yes'">
                <xsl:element name="DATELASTMODIFIED">
                  <xsl:value-of select="DateLastModified"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="Tag/@update='yes' or $VInsert='yes'">
                <xsl:element name="TAG">
                  <xsl:value-of select="Tag"/>
                </xsl:element>
              </xsl:if>
              <!-- Compound PropertyList -->
              <xsl:for-each select="PropertyList/Property[@update='yes' or $VInsert='yes']">
                <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                  <xsl:value-of select="normalize-space(.)"/>
                </xsl:element>
              </xsl:for-each>
              <!-- Structure PropertyList -->
              <xsl:for-each select="BaseFragment/Structure/PropertyList/Property[@update='yes' or $VInsert='yes']">
                <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                  <xsl:value-of select="normalize-space(.)"/>
                </xsl:element>
              </xsl:for-each>
              <!-- Compound fragment list -->
              <xsl:if test="count(FragmentList/Fragment[@*]) > 0 or $VInsert='yes'">
                <xsl:element name="FRAGMENTXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="FragmentList">
                      <xsl:for-each select="FragmentList/Fragment[not(@delete)]">
                        <xsl:copy-of select="node()"/>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
              <!-- BatchComponent fragment list -->
              <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList[@update='yes' or $VInsert='yes']">
                <xsl:element name="BATCHCOMPFRAGMENTXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:copy-of select="." />
                  </xsl:element>
                </xsl:element>
              </xsl:for-each>
              <!-- Compound identifiers -->
              <xsl:if test="count(IdentifierList/Identifier[@*]) > 0 or $VInsert='yes'">
                <xsl:element name="IDENTIFIERXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="IdentifierList">
                      <xsl:for-each select="IdentifierList/Identifier[not(@delete)]">
                        <xsl:element name="Identifier">
                          <xsl:copy-of select="node()"/>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
              <!-- Structure identifiers : strip all attributes -->
              <xsl:if test="count(BaseFragment/Structure/IdentifierList/Identifier[@*]) > 0 or $VInsert='yes'">
                <xsl:element name="STRUCTIDENTIFIERXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="IdentifierList">
                      <xsl:for-each select="BaseFragment/Structure/IdentifierList/Identifier[not(@delete)]">
                        <xsl:element name="Identifier">
                          <xsl:copy-of select="node()"/>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
              <xsl:if test="BaseFragment/Structure/NormalizedStructure/@update='yes' or $VInsert='yes'">
                <xsl:element name="NORMALIZEDSTRUCTURE">
                  <xsl:copy-of select="BaseFragment/Structure/NormalizedStructure" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="BaseFragment/Structure/UseNormalization/@update='yes' or $VInsert='yes'">
                <xsl:element name="USENORMALIZATION">
                  <xsl:copy-of select="BaseFragment/Structure/UseNormalization" />
                </xsl:element>
              </xsl:if>
              <!-- BatchComponent property list -->
              <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                <xsl:for-each select="PropertyList/Property[@update='yes' or $VInsert='yes']">
                  <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                    <xsl:value-of select="normalize-space(.)"/>
                  </xsl:element>
                </xsl:for-each>
              </xsl:for-each>
            </xsl:element>
          </VW_TemporaryCompound>
        </xsl:for-each>
      </xsl:for-each>
    </MultiCompoundRegistryRecord>
  </xsl:template>
</xsl:stylesheet>
