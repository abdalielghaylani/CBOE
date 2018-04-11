<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:variable name="Vlower" select="'abcdefghijklmnopqrstuvwxyz'"/>
  <xsl:variable name="VUPPER" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <xsl:for-each select="RegNumber">
      <xsl:variable name="VRegNumber" select="RegNumber"/>
      <VW_RegistryNumber>
        <ROW>
          <REGID>0</REGID>
          <SEQUENCENUMBER>
            <xsl:value-of select="SequenceNumber"/>
          </SEQUENCENUMBER>
          <REGNUMBER>
            <xsl:choose>
              <xsl:when test="string-length($VRegNumber) != 0">
                <xsl:value-of select="$VRegNumber"/>
              </xsl:when>
              <xsl:otherwise>null</xsl:otherwise>
            </xsl:choose>
          </REGNUMBER>
          <SEQUENCEID>
            <xsl:value-of select="SequenceID"/>
          </SEQUENCEID>
          <DATECREATED>
            <xsl:value-of select="/MultiCompoundRegistryRecord/DateCreated"/>
          </DATECREATED>
          <PERSONREGISTERED>
            <xsl:value-of select="/MultiCompoundRegistryRecord/PersonCreated"/>
          </PERSONREGISTERED>
        </ROW>
      </VW_RegistryNumber>
    </xsl:for-each>
    <VW_Mixture>
      <ROW>
        <MIXTUREID>0</MIXTUREID>
        <REGID>0</REGID>
        <CREATED>
          <xsl:value-of select="DateCreated"/>
        </CREATED>
        <PERSONCREATED>
          <xsl:value-of select="PersonCreated"/>
        </PERSONCREATED>
        <MODIFIED>
          <xsl:value-of select="DateLastModified"/>
        </MODIFIED>
        <APPROVED>
          <xsl:value-of select="Approved"/>
        </APPROVED>
        <xsl:for-each select="PropertyList/Property">
          <xsl:variable name="eValue" select="."/>
          <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
          <xsl:element name="{$eName}">
            <xsl:value-of select="$eValue"/>
          </xsl:element>
        </xsl:for-each>
      </ROW>
    </VW_Mixture>
    <xsl:for-each select="ProjectList/Project">
      <xsl:if test="ProjectID!=''">
        <VW_RegistryNumber_Project>
          <ROW>
            <PROJECTID>
              <xsl:value-of select="ProjectID"/>
            </PROJECTID>
            <REGID>0</REGID>
            <ORDERINDEX>
              <xsl:value-of select="OrderIndex"/>
            </ORDERINDEX>
          </ROW>
        </VW_RegistryNumber_Project>
      </xsl:if>
    </xsl:for-each>
    <xsl:for-each select="IdentifierList/Identifier">
      <VW_Compound_Identifier>
        <ROW>
          <ID>0</ID>
          <TYPE>
            <xsl:value-of select="IdentifierID"/>
          </TYPE>
          <VALUE>
            <xsl:value-of select="InputText"/>
          </VALUE>
          <REGID>0</REGID>
          <ORDERINDEX>
            <xsl:value-of select="OrderIndex"/>
          </ORDERINDEX>
        </ROW>
      </VW_Compound_Identifier>
    </xsl:for-each>
    <xsl:for-each select="BatchList/Batch">
      <VW_Batch>
        <ROW>
          <BATCHID>0</BATCHID>
          <BATCHNUMBER>0</BATCHNUMBER>
          <FULLREGNUMBER>
            <xsl:choose>
              <xsl:when test="FullRegNumber!=''">
                <xsl:value-of select="FullRegNumber"/>
              </xsl:when>
              <xsl:otherwise>null</xsl:otherwise>
            </xsl:choose>
          </FULLREGNUMBER>
          <DATECREATED>
            <xsl:value-of select="DateCreated"/>
          </DATECREATED>
          <PERSONCREATED>
            <xsl:value-of select="PersonCreated"/>
          </PERSONCREATED>
          <PERSONREGISTERED>
            <xsl:value-of select="PersonRegistered"/>
          </PERSONREGISTERED>
          <DATELASTMODIFIED>
            <xsl:value-of select="DateLastModified"/>
          </DATELASTMODIFIED>
          <STATUSID>
            <xsl:value-of select="StatusID"/>
          </STATUSID>
          <REGID>0</REGID>
          <TEMPBATCHID>
            <xsl:value-of select="BatchID"/>
          </TEMPBATCHID>
          <xsl:for-each select="PropertyList/Property">
            <xsl:variable name="eValue" select="."/>
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
        </ROW>
      </VW_Batch>
      <xsl:for-each select="ProjectList/Project">
        <VW_Batch_Project>
          <ROW>
            <xsl:for-each select="ProjectID">
              <PROJECTID>
                <xsl:value-of select="."/>
              </PROJECTID>
              <BATCHID>0</BATCHID>
            </xsl:for-each>
          </ROW>
        </VW_Batch_Project>
      </xsl:for-each>
      <xsl:for-each select="IdentifierList/Identifier">
        <VW_BatchIdentifier>
          <ROW>
            <ID>0</ID>
            <TYPE>
              <xsl:value-of select="IdentifierID"/>
            </TYPE>
            <VALUE>
              <xsl:value-of select="InputText"/>
            </VALUE>
            <BATCHID>0</BATCHID>
            <ORDERINDEX>
              <xsl:value-of select="OrderIndex"/>
            </ORDERINDEX>
          </ROW>
        </VW_BatchIdentifier>
      </xsl:for-each>
    </xsl:for-each>
    <xsl:for-each select="ComponentList/Component">
      <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
      <xsl:for-each select="Compound">
        <xsl:variable name="VCompound" select="."/>
        <xsl:for-each select="RegNumber">
          <VW_RegistryNumber>
            <ROW>
              <REGID>
                <xsl:value-of select="RegID"/>
              </REGID>
              <!-- this is an odd construct: " " -->
              <SEQUENCENUMBER>
                <xsl:value-of select="SequenceNumber"/>
              </SEQUENCENUMBER>
              <REGNUMBER>" "</REGNUMBER>
              <SEQUENCEID>
                <xsl:value-of select="SequenceID"/>
              </SEQUENCEID>
              <DATECREATED>
                <xsl:value-of select="$VCompound/DateCreated"/>
              </DATECREATED>
              <PERSONREGISTERED>
                <xsl:value-of select="$VCompound/PersonRegistered"/>
              </PERSONREGISTERED>
            </ROW>
          </VW_RegistryNumber>
        </xsl:for-each>
        <xsl:choose>
          <xsl:when test="RegNumber/RegID='0'">
            <xsl:for-each select="BaseFragment/Structure">
              <VW_Structure>
                <ROW>
                  <STRUCTUREID>
                    <xsl:value-of select="StructureID"/>
                  </STRUCTUREID>
                  <STRUCTUREFORMAT>
                    <xsl:value-of select="StructureFormat"/>
                  </STRUCTUREFORMAT>
                  <xsl:for-each select="PropertyList/Property">
                    <xsl:variable name="eValue" select="."/>
                    <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                    <xsl:element name="{$eName}">
                      <xsl:value-of select="$eValue"/>
                    </xsl:element>
                  </xsl:for-each>
                </ROW>
              </VW_Structure>
            </xsl:for-each>
            <xsl:for-each select="BaseFragment/Structure/IdentifierList/Identifier">
              <VW_Structure_Identifier>
                <ROW>
                  <ID>0</ID>
                  <TYPE>
                    <xsl:value-of select="IdentifierID"/>
                  </TYPE>
                  <VALUE>
                    <xsl:value-of select="InputText"/>
                  </VALUE>
                  <STRUCTUREID>0</STRUCTUREID>
                  <ORDERINDEX>
                    <xsl:value-of select="OrderIndex"/>
                  </ORDERINDEX>
                </ROW>
              </VW_Structure_Identifier>
            </xsl:for-each>
            <VW_Compound>
              <ROW>
                <COMPOUNDID>0</COMPOUNDID>
                <DATECREATED>
                  <xsl:value-of select="DateCreated"/>
                </DATECREATED>
                <PERSONCREATED>
                  <xsl:value-of select="PersonCreated"/>
                </PERSONCREATED>
                <PERSONREGISTERED>
                  <xsl:value-of select="PersonRegistered"/>
                </PERSONREGISTERED>
                <DATELASTMODIFIED>
                  <xsl:value-of select="DateLastModified"/>
                </DATELASTMODIFIED>
                <TAG>
                  <xsl:value-of select="Tag"/>
                </TAG>
                <REGID>0</REGID>
                <STRUCTUREID>0</STRUCTUREID>
                <xsl:for-each select="BaseFragment/Structure">
                  <USENORMALIZATION>
                    <xsl:value-of select="UseNormalization"/>
                  </USENORMALIZATION>
                  <!-- JED: Why does this not also pull the "normalized structire" element? -->
                </xsl:for-each>
                <xsl:for-each select="PropertyList/Property">
                  <xsl:variable name="eValue" select="."/>
                  <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                  <xsl:element name="{$eName}">
                    <xsl:value-of select="$eValue"/>
                  </xsl:element>
                </xsl:for-each>
              </ROW>
            </VW_Compound>
            <VW_Mixture_Component>
              <ROW>
                <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                <MIXTUREID>0</MIXTUREID>
                <COMPOUNDID>0</COMPOUNDID>
              </ROW>
            </VW_Mixture_Component>
            <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
              <VW_BatchComponent>
                <ROW>
                  <ID>0</ID>
                  <BATCHID>0</BATCHID>
                  <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                  <ORDERINDEX>
                    <xsl:value-of select="OrderIndex"/>
                  </ORDERINDEX>
                  <xsl:for-each select="PropertyList/Property">
                    <xsl:variable name="eValue" select="."/>
                    <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                    <xsl:element name="{$eName}">
                      <xsl:value-of select="$eValue"/>
                    </xsl:element>
                  </xsl:for-each>
                </ROW>
              </VW_BatchComponent>
            </xsl:for-each>
            <xsl:for-each select="Project/ProjectList">
              <VW_RegistryNumber_Project>
                <ROW>
                  <xsl:for-each select="ProjectID">
                    <PROJECTID>
                      <xsl:value-of select="."/>
                    </PROJECTID>
                    <REGID>0</REGID>
                  </xsl:for-each>
                </ROW>
              </VW_RegistryNumber_Project>
            </xsl:for-each>
            <xsl:for-each select="FragmentList/Fragment">
              <xsl:variable name="VFragmentID" select="FragmentID"/>
              <VW_Fragment>
                <ROW>
                  <FRAGMENTID>
                    <xsl:value-of select="FragmentID"/>
                  </FRAGMENTID>
                </ROW>
              </VW_Fragment>
              <VW_Compound_Fragment>
                <ROW>
                  <ID>0</ID>
                  <COMPOUNDID>0</COMPOUNDID>
                  <FRAGMENTID>0</FRAGMENTID>
                  <EQUIVALENTS>
                    <xsl:value-of select="Equivalents"/>
                  </EQUIVALENTS>
                </ROW>
              </VW_Compound_Fragment>
              <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[$VComponentIndex=ComponentIndex]">
                <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[FragmentID=$VFragmentID]">
                  <VW_BatchComponentFragment>
                    <ROW>
                      <BATCHCOMPONENTID>0</BATCHCOMPONENTID>
                      <COMPOUNDFRAGMENTID>0</COMPOUNDFRAGMENTID>
                      <EQUIVALENT>
                        <xsl:value-of select="Equivalents"/>
                      </EQUIVALENT>
                      <ORDERINDEX>
                        <xsl:value-of select="OrderIndex"/>
                      </ORDERINDEX>
                    </ROW>
                  </VW_BatchComponentFragment>
                </xsl:for-each>
              </xsl:for-each>
            </xsl:for-each>
            <xsl:for-each select="IdentifierList/Identifier">
              <VW_Compound_Identifier>
                <ROW>
                  <ID>0</ID>
                  <TYPE>
                    <xsl:value-of select="IdentifierID"/>
                  </TYPE>
                  <VALUE>
                    <xsl:value-of select="InputText"/>
                  </VALUE>
                  <REGID>0</REGID>
                  <ID>0</ID>
                  <ORDERINDEX>
                    <xsl:value-of select="OrderIndex"/>
                  </ORDERINDEX>
                </ROW>
              </VW_Compound_Identifier>
            </xsl:for-each>
          </xsl:when>
          <xsl:when test="RegNumber/RegID!='0'">
            <VW_Mixture_Component>
              <ROW>
                <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                <MIXTUREID>0</MIXTUREID>
                <COMPOUNDID>0</COMPOUNDID>
              </ROW>
            </VW_Mixture_Component>
            <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
              <VW_BatchComponent>
                <ROW>
                  <ID>0</ID>
                  <BATCHID>0</BATCHID>
                  <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                  <ORDERINDEX>
                    <xsl:value-of select="OrderIndex"/>
                  </ORDERINDEX>
                  <xsl:for-each select="PropertyList/Property">
                    <xsl:variable name="eValue" select="."/>
                    <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                    <xsl:element name="{$eName}">
                      <xsl:value-of select="$eValue"/>
                    </xsl:element>
                  </xsl:for-each>
                </ROW>
              </VW_BatchComponent>
            </xsl:for-each>
            <xsl:for-each select="FragmentList/Fragment">
              <xsl:variable name="VFragmentID" select="FragmentID"/>
              <VW_Fragment>
                <ROW>
                  <FRAGMENTID>
                    <xsl:value-of select="FragmentID"/>
                  </FRAGMENTID>
                </ROW>
              </VW_Fragment>
              <VW_Compound_Fragment>
                <ROW>
                  <ID>0</ID>
                  <COMPOUNDID>0</COMPOUNDID>
                  <FRAGMENTID>0</FRAGMENTID>
                  <EQUIVALENTS>
                    <xsl:value-of select="Equivalents"/>
                  </EQUIVALENTS>
                </ROW>
              </VW_Compound_Fragment>
              <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[FragmentID=$VFragmentID]">
                  <VW_BatchComponentFragment>
                    <ROW>
                      <BATCHCOMPONENTID>0</BATCHCOMPONENTID>
                      <COMPOUNDFRAGMENTID>0</COMPOUNDFRAGMENTID>
                      <EQUIVALENT>
                        <xsl:value-of select="Equivalents"/>
                      </EQUIVALENT>
                      <ORDERINDEX>
                        <xsl:value-of select="OrderIndex"/>
                      </ORDERINDEX>
                    </ROW>
                  </VW_BatchComponentFragment>
                </xsl:for-each>
              </xsl:for-each>
            </xsl:for-each>
          </xsl:when>
        </xsl:choose>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>