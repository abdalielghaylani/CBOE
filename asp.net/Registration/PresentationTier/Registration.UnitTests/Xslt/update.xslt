<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:variable name="Vlower" select="'abcdefghijklmnopqrstuvwxyz'"/>
  <xsl:variable name="VUPPER" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:variable name="VMixtureID" select="ID" />
      <xsl:variable name="VMixtureRegID" select="RegNumber/RegID/." />
      <!-- MIXTURE -->
      <VW_Mixture>
        <xsl:element name="ROW">
          <xsl:element name="MIXTUREID">
            <xsl:value-of select="ID" />
          </xsl:element>
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
          <xsl:element name="MODIFIED">
            <xsl:value-of select="DateLastModified" />
          </xsl:element>
          <xsl:if test="StructureAggregation/@update='yes'">
            <xsl:element name="STRUCTUREAGGREGATION">
              <xsl:value-of select="StructureAggregation" />
            </xsl:element>
          </xsl:if>
          <xsl:if test="Approved/@update='yes'">
            <xsl:element name="APPROVED">
              <xsl:value-of select="Approved" />
            </xsl:element>
          </xsl:if>
          <xsl:for-each select="PropertyList/Property[@update='yes' or @insert='yes']">
            <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
              <xsl:value-of select="normalize-space(.)"/>
            </xsl:element>
          </xsl:for-each>
        </xsl:element>
      </VW_Mixture>
      <!-- MIXTURE RegNumbers -->
      <VW_RegistryNumber>
        <xsl:for-each select="RegNumber">
          <xsl:element name="ROW">
            <xsl:element name="REGID">
              <xsl:value-of select="RegID" />
            </xsl:element>
            <xsl:if test="SequenceNumber/@update='yes'">
              <xsl:element name="SEQUENCENUMBER">
                <xsl:value-of select="SequenceNumber" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="RegNumber/@update='yes'">
              <xsl:element name="REGNUMBER">
                <xsl:value-of select="RegNumber" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="SequenceID/@update='yes'">
              <xsl:element name="SEQUENCEID">
                <xsl:value-of select="SequenceID" />
              </xsl:element>
            </xsl:if>
          </xsl:element>
        </xsl:for-each>
      </VW_RegistryNumber>
      <!-- MIXTURE Projects -->
      <xsl:for-each select="ProjectList/Project">
        <xsl:variable name="VDeleteProject" select="@delete" />
        <xsl:variable name="VInsertProject" select="@insert" />
        <VW_RegistryNumber_Project>
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
            <xsl:if test="ProjectID/@update='yes' or $VInsertProject='yes'">
              <xsl:element name="PROJECTID">
                <xsl:value-of select="ProjectID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="$VInsertProject='yes'">
              <xsl:element name="REGID">
                <xsl:value-of select="$VMixtureRegID" />
              </xsl:element>
            </xsl:if>
          </xsl:element>
        </VW_RegistryNumber_Project>
      </xsl:for-each>
      <!-- MIXTURE Identifiers -->
      <xsl:for-each select="IdentifierList/Identifier">
        <xsl:variable name="VDeleteIdentifier" select="@delete" />
        <xsl:variable name="VInsertIdentifier" select="@insert" />
        <VW_Compound_Identifier>
          <xsl:choose>
            <xsl:when test="$VDeleteIdentifier='yes'">
              <xsl:attribute name="delete">
                <xsl:value-of select="@delete"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:when test="$VInsertIdentifier='yes'">
              <xsl:attribute name="insert">
                <xsl:value-of select="@insert"/>
              </xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <xsl:element name="ROW">
            <xsl:element name="ID">
              <xsl:value-of select="ID" />
            </xsl:element>
            <xsl:if test="IdentifierID/@update='yes' or $VInsertIdentifier='yes'">
              <xsl:element name="TYPE">
                <xsl:value-of select="IdentifierID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="InputText/@update='yes' or $VInsertIdentifier='yes'">
              <xsl:element name="VALUE">
                <xsl:value-of select="InputText" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="$VInsertIdentifier='yes'">
              <xsl:element name="REGID">
                <xsl:value-of select="$VMixtureRegID" />
              </xsl:element>
            </xsl:if>
          </xsl:element>
        </VW_Compound_Identifier>
      </xsl:for-each>
      <!-- ' || ' -->
      <!-- COMPONENTS -->
      <xsl:for-each select="ComponentList/Component">
        <xsl:variable name="VComponentIndex" select="ComponentIndex/." />
        <xsl:variable name="VDeleteComponent" select="@delete" />
        <xsl:variable name="VInsertComponent" select="@insert" />
        <xsl:variable name="VCompoundID" select="Compound/CompoundID" />
        <!-- COMPOUND -->
        <xsl:for-each select="Compound">
          <xsl:variable name="VRegID" select="RegNumber/RegID" />
          <xsl:variable name="VStructID" select="BaseFragment/Structure/StructureID" />
          <xsl:variable name="VCompound" select="." />
          <!-- COMPOUND RegNumber -->
          <xsl:for-each select="RegNumber">
            <VW_RegistryNumber>
              <xsl:choose>
                <xsl:when test="$VInsertComponent='yes'">
                  <xsl:attribute name="insert">
                    <xsl:value-of select="$VInsertComponent"/>
                  </xsl:attribute>
                </xsl:when>
              </xsl:choose>
              <xsl:element name="ROW">
                <xsl:element name="REGID">
                  <xsl:value-of select="RegID" />
                </xsl:element>
                <xsl:if test="SequenceNumber/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="SEQUENCENUMBER">
                    <xsl:value-of select="SequenceNumber" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="RegNumber/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="REGNUMBER">
                    <xsl:value-of select="RegNumber" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="SequenceID/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="SEQUENCEID">
                    <xsl:value-of select="SequenceID" />
                  </xsl:element>
                </xsl:if>
                <!-- Do we really want to alter the 'creation date' of the record? -->
                <xsl:if test="DateCreated/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="DATECREATED">
                    <xsl:value-of select="DateCreated" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="$VCompound/PersonRegistered/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="PERSONREGISTERED">
                    <xsl:value-of select="$VCompound/PersonRegistered" />
                  </xsl:element>
                </xsl:if>
              </xsl:element>
            </VW_RegistryNumber>
          </xsl:for-each>

          <xsl:if test="$VRegID='0' and $VInsertComponent='yes' or $VDeleteComponent='yes' or string-length($VInsertComponent)=0 ">
            <xsl:if test="$VDeleteComponent!='yes' or string-length($VDeleteComponent)=0 ">
              <xsl:variable name="VInsertStructure" select="BaseFragment/Structure/@insert"/>
              <!-- STRUCTURE -->
              <VW_Structure>
                <xsl:if test="$VInsertComponent='yes' or $VInsertStructure='yes' or BaseFragment/Structure/StructureID='0'">
                  <xsl:attribute name="insert">yes</xsl:attribute>
                </xsl:if>
                <xsl:for-each select="BaseFragment/Structure">
                  <xsl:element name="ROW">
                    <xsl:element name="STRUCTUREID">
                      <xsl:value-of select="StructureID" />
                    </xsl:element>
                    <xsl:if test="StructureFormat/@update='yes' or $VInsertComponent='yes' or ../@insert='yes' or DrawingType/@update='yes'">
                      <xsl:element name="STRUCTUREFORMAT">
                        <xsl:value-of select="StructureFormat" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="DrawingType/@update='yes' or $VInsertComponent='yes' or ../@insert='yes' or DrawingType/@update='yes'">
                      <xsl:element name="DRAWINGTYPE">
                        <xsl:value-of select="DrawingType" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="Structure/@update='yes' or $VInsertComponent='yes' or ../@insert='yes' or DrawingType/@update='yes'">
                      <xsl:element name="STRUCTURE">
                        <xsl:value-of select="Structure" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:for-each select="PropertyList/Property[@update='yes' or @insert='yes']">
                      <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                        <xsl:value-of select="normalize-space(.)"/>
                      </xsl:element>
                    </xsl:for-each>
                  </xsl:element>
                </xsl:for-each>
              </VW_Structure>
              <!-- STRUCTURE Identifiers -->
              <xsl:for-each select="BaseFragment/Structure/IdentifierList/Identifier">
                <xsl:variable name="VDeleteIdentifier" select="@delete" />
                <xsl:variable name="VInsertIdentifier" select="@insert" />
                <VW_Structure_Identifier>
                  <xsl:choose>
                    <xsl:when test="$VDeleteIdentifier='yes'">
                      <xsl:attribute name="delete">
                        <xsl:value-of select="@delete"/>
                      </xsl:attribute>
                    </xsl:when>
                    <xsl:when test="$VInsertIdentifier='yes' or $VInsertStructure='yes'">
                      <xsl:attribute name="insert">
                        <xsl:value-of select="@insert"/>
                      </xsl:attribute>
                    </xsl:when>
                  </xsl:choose>
                  <xsl:element name="ROW">
                    <xsl:element name="ID">
                      <xsl:value-of select="ID" />
                    </xsl:element>
                    <xsl:if test="IdentifierID/@update='yes' or $VInsertComponent='yes' or $VInsertIdentifier='yes'">
                      <xsl:element name="TYPE">
                        <xsl:value-of select="IdentifierID" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="InputText/@update='yes' or $VInsertComponent='yes' or $VInsertIdentifier='yes'">
                      <xsl:element name="VALUE">
                        <xsl:value-of select="InputText" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="$VInsertIdentifier='yes'">
                      <xsl:element name="STRUCTUREID">
                        <xsl:value-of select="$VStructID" />
                      </xsl:element>
                    </xsl:if>
                  </xsl:element>
                </VW_Structure_Identifier>
              </xsl:for-each>
            </xsl:if>

            <!-- COMPOUND data -->
            <VW_Compound>
              <xsl:if test="$VDeleteComponent='yes'">
                <xsl:attribute name="delete">yes</xsl:attribute>
              </xsl:if>
              <xsl:if test="$VInsertComponent='yes'">
                <xsl:attribute name="insert">yes</xsl:attribute>
              </xsl:if>
              <xsl:element name="ROW">
                <xsl:element name="COMPOUNDID">
                  <xsl:value-of select="CompoundID" />
                </xsl:element>
                <xsl:if test="DateCreated/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="DATECREATED">
                    <xsl:value-of select="DateCreated" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="PersonCreated/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="PERSONCREATED">
                    <xsl:value-of select="PersonCreated" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="PersonRegistered/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="PERSONREGISTERED">
                    <xsl:value-of select="PersonRegistered" />
                  </xsl:element>
                </xsl:if>
                <xsl:element name="DATELASTMODIFIED">
                  <xsl:value-of select="DateLastModified" />
                </xsl:element>
                <xsl:if test="Tag/@update='yes' or $VInsertComponent='yes'">
                  <xsl:element name="TAG">
                    <xsl:value-of select="Tag" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="$VInsertComponent='yes'">
                  <xsl:element name="REGID">
                    <xsl:value-of select="RegID" />
                  </xsl:element>
                </xsl:if>
                <!-- COMPOUND structure data -->
                <xsl:for-each select="BaseFragment">
                  <xsl:for-each select="Structure">
                    <xsl:if test="@update='yes' or @insert='yes' or StructureID/@update='yes' or StructureID&lt;0 or $VInsertComponent='yes'">
                      <xsl:element name="STRUCTUREID">
                        <xsl:value-of select="StructureID" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="NormalizedStructure/@update='yes' or $VInsertComponent='yes'">
                      <xsl:element name="NORMALIZEDSTRUCTURE">
                        <xsl:value-of select="NormalizedStructure" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="UseNormalization/@update='yes' or $VInsertComponent='yes'">
                      <xsl:element name="USENORMALIZATION">
                        <xsl:value-of select="UseNormalization" />
                      </xsl:element>
                    </xsl:if>
                  </xsl:for-each>
                </xsl:for-each>
                <xsl:for-each select="PropertyList/Property[@update='yes' or @insert='yes']">
                  <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                    <xsl:value-of select="normalize-space(.)"/>
                  </xsl:element>
                </xsl:for-each>
              </xsl:element>
            </VW_Compound>
            <!-- COMPOUND fragments -->
            <xsl:for-each select="FragmentList/Fragment">
              <xsl:variable name="VInsertFragment" select="@insert" />
              <xsl:variable name="VDeleteFragment" select="@delete" />
              <VW_Compound_Fragment>
                <xsl:choose>
                  <xsl:when test="CompoundFragmentID/@delete='yes' or $VDeleteFragment='yes'">
                    <xsl:attribute name="delete">
                      <xsl:value-of select="@delete"/>
                    </xsl:attribute>
                  </xsl:when>
                  <xsl:when test="$VInsertFragment='yes' or $VInsertComponent='yes'">
                    <xsl:attribute name="insert">
                      <xsl:value-of select="@insert"/>
                    </xsl:attribute>
                  </xsl:when>
                </xsl:choose>
                <xsl:element name="ROW">
                  <xsl:element name="ID">
                    <xsl:value-of select="CompoundFragmentID" />
                  </xsl:element>
                  <xsl:if test="$VInsertFragment='yes' or $VInsertComponent='yes'">
                    <xsl:element name="COMPOUNDID">
                      <xsl:value-of select="$VCompoundID" />
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="FragmentID/@update='yes' or $VInsertFragment='yes' or $VInsertComponent='yes'">
                    <xsl:element name="FRAGMENTID">
                      <xsl:value-of select="FragmentID" />
                    </xsl:element>
                  </xsl:if>
                </xsl:element>
              </VW_Compound_Fragment>
            </xsl:for-each>
            <!-- COMPOUND Identifiers -->
            <xsl:for-each select="IdentifierList/Identifier">
              <xsl:variable name="VDeleteIdentifier" select="@delete" />
              <xsl:variable name="VInsertIdentifier" select="@insert" />
              <VW_Compound_Identifier>
                <xsl:choose>
                  <xsl:when test="$VDeleteIdentifier='yes'">
                    <xsl:attribute name="delete">
                      <xsl:value-of select="@delete"/>
                    </xsl:attribute>
                  </xsl:when>
                  <xsl:when test="$VInsertIdentifier='yes' or $VInsertComponent='yes'">
                    <xsl:attribute name="insert">
                      <xsl:value-of select="@insert"/>
                    </xsl:attribute>
                  </xsl:when>
                </xsl:choose>
                <xsl:element name="ROW">
                  <xsl:element name="ID">
                    <xsl:value-of select="ID" />
                  </xsl:element>
                  <xsl:if test="IdentifierID/@update='yes' or $VInsertComponent='yes' or $VInsertIdentifier='yes'">
                    <xsl:element name="TYPE">
                      <xsl:value-of select="IdentifierID" />
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="InputText/@update='yes' or $VInsertComponent='yes' or $VInsertIdentifier='yes'">
                    <xsl:element name="VALUE">
                      <xsl:value-of select="InputText" />
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="$VCompound/RegNumber/RegID/@update='yes' or $VInsertComponent='yes' or $VInsertIdentifier='yes'">
                    <xsl:element name="REGID">
                      <xsl:value-of select="$VCompound/RegNumber/RegID" />
                    </xsl:element>
                  </xsl:if>
                </xsl:element>
              </VW_Compound_Identifier>
            </xsl:for-each>
          </xsl:if>
          <!-- MIXTURE component linking information -->
          <VW_Mixture_Component>
            <xsl:if test="$VInsertComponent='yes'">
              <xsl:attribute name="insert">yes</xsl:attribute>
            </xsl:if>
            <xsl:element name="ROW">
              <xsl:element name="MIXTURECOMPONENTID">0</xsl:element>
              <xsl:element name="MIXTUREID">0</xsl:element>
              <xsl:element name="COMPOUNDID">0</xsl:element>
            </xsl:element>
          </VW_Mixture_Component>
          <!-- BATCH components -->
          <xsl:if test="$VDeleteComponent!='yes' or string-length($VDeleteComponent)=0">
            <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[((ComponentIndex=$VComponentIndex) and ((@delete!='yes') or (string-length(@delete)=0))) ]">
              <VW_BatchComponent>
                <xsl:if test="$VInsertComponent='yes'">
                  <xsl:attribute name="insert">yes</xsl:attribute>
                </xsl:if>
                <xsl:element name="ROW">
                  <xsl:element name="ID">
                    <xsl:value-of select="ID"/>
                  </xsl:element>
                  <xsl:if test="CompoundID/@update='yes' or $VInsertComponent='yes'">
                    <xsl:element name="MIXTURECOMPONENTID">
                      <xsl:value-of select="CompoundID"/>
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="BatchID/@update='yes' or $VInsertComponent='yes'">
                    <xsl:element name="BATCHID">
                      <xsl:value-of select="BatchID"/>
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="OrderIndex/@update='yes' or $VInsertComponent='yes'">
                    <xsl:element name="ORDERINDEX">
                      <xsl:value-of select="OrderIndex"/>
                    </xsl:element>
                  </xsl:if>
                  <xsl:for-each select="PropertyList/Property[@update='yes' or @insert='yes']">
                    <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                      <xsl:value-of select="normalize-space(.)"/>
                    </xsl:element>
                  </xsl:for-each>
                </xsl:element>
              </VW_BatchComponent>
            </xsl:for-each>
            <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment">
              <VW_BatchComponentFragment>
                <xsl:choose>
                  <xsl:when test="Equivalents/@update='yes'">
                    <xsl:attribute name="update">yes</xsl:attribute>
                  </xsl:when>
                  <xsl:when test="$VInsertComponent='yes'">
                    <xsl:attribute name="insert">yes</xsl:attribute>
                  </xsl:when>
                </xsl:choose>
                <xsl:element name="ROW">
                  <xsl:if test="$VInsertComponent='yes'">
                    <xsl:attribute name="FragmentID">
                      <xsl:value-of select="FragmentID" />
                    </xsl:attribute>
                    <xsl:attribute name="CompoundID">
                      <xsl:value-of select="../../CompoundID" />
                    </xsl:attribute>
                  </xsl:if>
                  <!-- only need the ID for deletes and updates -->
                  <xsl:if test="Equivalents/@update='yes' or $VInsertComponent='yes'">
                    <xsl:element name="ID">
                      <xsl:value-of select="ID" />
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="$VInsertComponent='yes'">
                    <xsl:element name="BATCHCOMPONENTID">
                      <xsl:value-of select="../../ID"/>
                    </xsl:element>
                    <xsl:element name="COMPOUNDFRAGMENTID">0</xsl:element>
                  </xsl:if>
                  <xsl:if test="Equivalents/@update='yes' or $VInsertComponent='yes'">
                    <xsl:element name="EQUIVALENT">
                      <xsl:value-of select="Equivalents"/>
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="OrderIndex/@update='yes' or $VInsertComponent='yes'">
                    <xsl:element name="ORDERINDEX">
                      <xsl:value-of select="OrderIndex"/>
                    </xsl:element>
                  </xsl:if>
                </xsl:element>
              </VW_BatchComponentFragment>
            </xsl:for-each>
          </xsl:if>
        </xsl:for-each>
      </xsl:for-each>
      <!-- BATCH list -->
      <xsl:for-each select="BatchList/Batch">
        <xsl:variable name="VBatch" select="." />
        <xsl:variable name="VInsertBatch" select="@insert" />
        <xsl:variable name="VUpdateTable">
          <!-- TODO: there's no sense to this...why use @insert='yes' in both cases? -->
          <xsl:if test="BatchID!=0 and @insert='yes'">
            <xsl:value-of select="'yes'" />
          </xsl:if>
        </xsl:variable>
        <!-- BATCH data -->
        <VW_Batch>
          <xsl:if test="$VInsertBatch='yes'">
            <xsl:attribute name="insert">yes</xsl:attribute>
          </xsl:if>
          <xsl:element name="ROW">
            <xsl:element name="BATCHID">
              <xsl:value-of select="BatchID" />
            </xsl:element>
            <xsl:if test="BatchNumber/@update='yes' or $VInsertBatch='yes'">
              <xsl:element name="BATCHNUMBER">
                <xsl:value-of select="BatchNumber" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="FullRegNumber/@update='yes' or $VInsertBatch='yes'">
              <xsl:element name="FULLREGNUMBER">
                <xsl:choose>
                  <xsl:when test="FullRegNumber!=''">
                    <xsl:value-of select="FullRegNumber" />
                  </xsl:when>
                  <xsl:otherwise>null</xsl:otherwise>
                </xsl:choose>
              </xsl:element>
            </xsl:if>
            <!-- TODO: unravel THIS tongue-twister of a conditional.
            Is the intention to force an update if the fragments have been changed becuase of the 'salt suffix' concept?
            <xsl:if test="(string-length(BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment[@delete='yes'])>0 or string-length(/MultiCompoundRegistryRecord/ComponentList/Component/Compound/FragmentList/Fragment/FragmentID[@update='yes'])>0) and string-length(FullRegNumber[@update='yes' or $VInsertBatch='yes'])=0">
              <xsl:element name="FULLREGNUMBER">
                <xsl:value-of select="FullRegNumber" />
              </xsl:element>
              <xsl:element name="BATCHNUMBER">
                <xsl:value-of select="BatchNumber" />
              </xsl:element>
            </xsl:if>
            -->
            <xsl:if test="DateCreated/@update='yes' or $VInsertBatch='yes'">
              <xsl:element name="DATECREATED">
                <xsl:value-of select="DateCreated" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="PersonCreated/@update='yes' or $VInsertBatch='yes'">
              <xsl:element name="PERSONCREATED">
                <xsl:value-of select="PersonCreated" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="PersonRegistered/@update='yes' or $VInsertBatch='yes'">
              <xsl:element name="PERSONREGISTERED">
                <xsl:value-of select="PersonRegistered" />
              </xsl:element>
            </xsl:if>
            <xsl:element name="DATELASTMODIFIED">
              <xsl:value-of select="DateLastModified" />
            </xsl:element>
            <xsl:if test="StatusID/@update='yes' or $VInsertBatch='yes'">
              <xsl:element name="STATUSID">
                <xsl:value-of select="StatusID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="$VInsertBatch='yes' or $VUpdateTable='yes'">
              <xsl:element name="REGID">
                <xsl:value-of select="$VMixtureRegID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="TempBatchID/@update='yes' or $VInsertBatch='yes'">
              <xsl:element name="TEMPBATCHID">
                <xsl:value-of select="TempBatchID" />
              </xsl:element>
            </xsl:if>
            <xsl:for-each select="PropertyList/Property[@update='yes' or @insert='yes']">
              <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                <xsl:value-of select="normalize-space(.)"/>
              </xsl:element>
            </xsl:for-each>
          </xsl:element>
        </VW_Batch>
        <!-- TODO: re-factor -->
        <xsl:choose>
          <xsl:when test="string-length($VUpdateTable)>0">
            <xsl:for-each select="$VBatch/BatchComponentList/BatchComponent">
              <VW_BatchComponent>
                <xsl:element name="ROW">
                  <xsl:element name="ID">0</xsl:element>
                  <xsl:element name="BATCHID">
                    <xsl:value-of select="$VBatch/BatchID" />
                  </xsl:element>
                  <xsl:element name="MIXTURECOMPONENTID">
                    <xsl:value-of select="MixtureComponentID" />
                  </xsl:element>
                  <xsl:for-each select="PropertyList/Property[@update='yes' or @insert='yes']">
                    <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                      <xsl:value-of select="normalize-space(.)"/>
                    </xsl:element>
                  </xsl:for-each>
                </xsl:element>
              </VW_BatchComponent>
            </xsl:for-each>
          </xsl:when>
          <xsl:when test="$VInsertBatch='yes'">
            <xsl:for-each select="BatchComponentList/BatchComponent[(@insert='yes')]">
              <VW_BatchComponent>
                <xsl:attribute name="insert">yes</xsl:attribute>
                <xsl:element name="ROW">
                  <xsl:element name="ID">0</xsl:element>
                  <xsl:element name="MIXTURECOMPONENTID">
                    <xsl:value-of select="MixtureComponentID" />
                  </xsl:element>
                  <xsl:element name="COMPOUNDID">
                    <xsl:value-of select="CompoundID" />
                  </xsl:element>
                  <xsl:element name="BATCHID">0</xsl:element>
                  <xsl:element name="ORDERINDEX">
                    <xsl:value-of select="OrderIndex" />
                  </xsl:element>
                  <xsl:for-each select="PropertyList/Property[@update='yes' or @insert='yes']">
                    <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                      <xsl:value-of select="normalize-space(.)"/>
                    </xsl:element>
                  </xsl:for-each>
                </xsl:element>
              </VW_BatchComponent>
            </xsl:for-each>
          </xsl:when>
        </xsl:choose>
        <!-- TODO: re-factor -->
        <xsl:for-each select="BatchComponentList/BatchComponent">
          <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[(@delete='yes')]">
            <VW_BatchComponentFragment>
              <xsl:attribute name="delete">yes</xsl:attribute>
              <xsl:element name="ROW">
                <xsl:element name="ID">
                  <xsl:value-of select="ID"/>
                </xsl:element>
              </xsl:element>
            </VW_BatchComponentFragment>
          </xsl:for-each>
        </xsl:for-each>
        <!-- TODO: re-factor -->
        <xsl:for-each select="BatchComponentList/BatchComponent">
          <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[(@insert='yes')]">
            <VW_BatchComponentFragment>
              <xsl:attribute name="insert">yes</xsl:attribute>
              <xsl:element name="ROW">
                <xsl:attribute name="FragmentID">
                  <xsl:value-of select="FragmentID" />
                </xsl:attribute>
                <xsl:attribute name="CompoundID">
                  <xsl:value-of select="../../CompoundID" />
                </xsl:attribute>
                <xsl:element name="BATCHCOMPONENTID">
                  <xsl:value-of select="../../ID" />
                </xsl:element>
                <xsl:element name="COMPOUNDFRAGMENTID">
                  <xsl:value-of select="ID" />
                </xsl:element>
                <xsl:element name="EQUIVALENT">
                  <xsl:value-of select="Equivalents" />
                </xsl:element>
                <xsl:element name="ORDERINDEX">
                  <xsl:value-of select="OrderIndex" />
                </xsl:element>
              </xsl:element>
            </VW_BatchComponentFragment>
          </xsl:for-each>
        </xsl:for-each>
        <xsl:for-each select="BatchComponentList/BatchComponent[(@insert='yes') and (BatchComponentFragmentList/BatchComponentFragment/@insert!='yes' or string-length(BatchComponentFragmentList/BatchComponentFragment/@insert)=0)]">
          <xsl:variable name="VComponentIndex1" select="ComponentIndex" />
          <xsl:choose>
            <xsl:when test="/MultiCompoundRegistryRecord/ComponentList/Component[@insert!='yes' or string-length(@insert)=0]/ComponentIndex=$VComponentIndex1">
              <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment">
                <VW_BatchComponentFragment>
                  <xsl:attribute name="insert">yes</xsl:attribute>
                  <xsl:element name="ROW">
                    <xsl:attribute name="FragmentID">
                      <xsl:value-of select="FragmentID" />
                    </xsl:attribute>
                    <xsl:attribute name="CompoundID">
                      <xsl:value-of select="../../CompoundID" />
                    </xsl:attribute>
                    <xsl:element name="BATCHCOMPONENTID">
                      <xsl:value-of select="../../ID" />
                    </xsl:element>
                    <xsl:element name="COMPOUNDFRAGMENTID">
                      <xsl:value-of select="ID" />
                    </xsl:element>
                    <xsl:element name="EQUIVALENT">
                      <xsl:value-of select="Equivalents" />
                    </xsl:element>
                    <xsl:element name="ORDERINDEX">
                      <xsl:value-of select="OrderIndex" />
                    </xsl:element>
                  </xsl:element>
                </VW_BatchComponentFragment>
              </xsl:for-each>
            </xsl:when>
          </xsl:choose>
        </xsl:for-each>
        <!-- BATCH projects -->
        <xsl:for-each select="ProjectList/Project">
          <xsl:variable name="VDeleteProject" select="@delete" />
          <xsl:variable name="VInsertProject" select="@insert" />
          <VW_Batch_Project>
            <xsl:choose>
              <xsl:when test="$VDeleteProject='yes'">
                <xsl:attribute name="delete">
                  <xsl:value-of select="@delete"/>
                </xsl:attribute>
              </xsl:when>
              <xsl:when test="$VInsertProject='yes' or $VInsertBatch='yes'">
                <xsl:attribute name="insert">yes</xsl:attribute>
              </xsl:when>
            </xsl:choose>
            <xsl:element name="ROW">
              <xsl:element name="ID">
                <xsl:value-of select="ID" />
              </xsl:element>
              <xsl:if test="ProjectID/@update='yes' or $VInsertBatch='yes' or $VInsertProject='yes'">
                <xsl:element name="PROJECTID">
                  <xsl:value-of select="ProjectID" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="$VInsertProject='yes' or $VInsertBatch='yes'">
                <xsl:element name="BATCHID">
                  <xsl:value-of select="$VBatch/BatchID" />
                </xsl:element>
              </xsl:if>
            </xsl:element>
          </VW_Batch_Project>
        </xsl:for-each>
        <!-- BATCH identifiers -->
        <xsl:for-each select="IdentifierList/Identifier">
          <xsl:variable name="VDeleteIdentifier" select="@delete" />
          <xsl:variable name="VInsertIdentifier" select="@insert" />
          <VW_BatchIdentifier>
            <xsl:choose>
              <xsl:when test="$VDeleteIdentifier='yes'">
                <xsl:attribute name="delete">
                  <xsl:value-of select="@delete"/>
                </xsl:attribute>
              </xsl:when>
              <xsl:when test="$VInsertIdentifier='yes' or $VInsertBatch='yes'">
                <xsl:attribute name="insert">
                  <xsl:value-of select="@insert"/>
                </xsl:attribute>
              </xsl:when>
            </xsl:choose>
            <xsl:element name="ROW">
              <xsl:element name="ID">
                <xsl:value-of select="ID" />
              </xsl:element>
              <xsl:if test="IdentifierID/@update='yes' or $VInsertBatch='yes' or $VInsertIdentifier='yes'">
                <xsl:element name="TYPE">
                  <xsl:value-of select="IdentifierID" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="InputText/@update='yes' or $VInsertBatch='yes' or $VInsertIdentifier='yes'">
                <xsl:element name="VALUE">
                  <xsl:value-of select="InputText" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="$VInsertIdentifier='yes' or $VInsertBatch='yes'">
                <xsl:element name="BATCHID">
                  <xsl:value-of select="$VBatch/BatchID" />
                </xsl:element>
              </xsl:if>
            </xsl:element>
          </VW_BatchIdentifier>
        </xsl:for-each>
      </xsl:for-each>
    </MultiCompoundRegistryRecord>
  </xsl:template>
</xsl:stylesheet>
