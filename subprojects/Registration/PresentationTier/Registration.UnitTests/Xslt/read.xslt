<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:variable name="Vlower" select="'abcdefghijklmnopqrstuvwxyz'"/>
  <xsl:variable name="VUPPER" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:attribute name="SameBatchesIdentity">
        <xsl:value-of select="@SameBatchesIdentity"/>
      </xsl:attribute>
      <xsl:attribute name="ActiveRLS">
        <xsl:value-of select="@ActiveRLS"/>
      </xsl:attribute>
      <xsl:attribute name="IsEditable">
        <xsl:value-of select="@IsEditable"/>
      </xsl:attribute>
      <xsl:variable name="VMultiCompoundRegistryRecord" select="."/>
      <xsl:variable name="VTypeRegistryRecord" select="@TypeRegistryRecord"/>
      <xsl:for-each select="Mixture/ROW">
        <ID>
          <xsl:value-of select="MIXTUREID"/>
        </ID>
        <DateCreated>
          <xsl:value-of select="CREATED"/>
        </DateCreated>
        <DateLastModified>
          <xsl:value-of select="MODIFIED"/>
        </DateLastModified>
        <PersonCreated>
          <xsl:value-of select="PERSONCREATED"/>
        </PersonCreated>
        <StructureAggregation>
          <xsl:value-of select="STRUCTUREAGGREGATION"/>
        </StructureAggregation>
        <Approved>
          <xsl:value-of select="APPROVED"/>
        </Approved>
        <PropertyList>
          <xsl:for-each select="..">
            <xsl:apply-templates select="PropertyList"/>
          </xsl:for-each>
        </PropertyList>
        <RegNumber>
          <RegID>
            <xsl:value-of select="REGID"/>
          </RegID>
          <SequenceNumber>
            <xsl:value-of select="SEQUENCENUMBER"/>
          </SequenceNumber>
          <RegNumber>
            <xsl:value-of select="REGNUMBER"/>
          </RegNumber>
          <SequenceID>
            <xsl:value-of select="SEQUENCEID"/>
          </SequenceID>
        </RegNumber>
        <IdentifierList>
          <xsl:for-each select="../IdentifierList/ROW">
            <Identifier>
              <ID>
                <xsl:value-of select="ID"/>
              </ID>
              <IdentifierID>
                <xsl:for-each select="TYPE">
                  <xsl:attribute name="Description">
                    <xsl:value-of select="../DESCRIPTION"/>
                  </xsl:attribute>
                  <xsl:attribute name="Name">
                    <xsl:value-of select="../NAME"/>
                  </xsl:attribute>
                  <xsl:attribute name="Active">
                    <xsl:value-of select="../ACTIVE"/>
                  </xsl:attribute>
                  <xsl:value-of select="."/>
                </xsl:for-each>
              </IdentifierID>
              <InputText>
                <xsl:value-of select="VALUE"/>
              </InputText>
            </Identifier>
          </xsl:for-each>
        </IdentifierList>
        <ProjectList>
          <xsl:for-each select="../ProjectList/ROW">
            <Project>
              <ID>
                <xsl:value-of select="ID"/>
              </ID>
              <ProjectID>
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
              </ProjectID>
            </Project>
          </xsl:for-each>
        </ProjectList>
      </xsl:for-each>
      <xsl:if test="$VTypeRegistryRecord='Mixture'">
        <ComponentList>
          <xsl:for-each select="Compound/ROW">
            <Component>
              <ID/>
              <ComponentIndex>
                <xsl:value-of select="COMPONENTINDEX"/>
              </ComponentIndex>
              <Compound>
                <CompoundID>
                  <xsl:value-of select="COMPOUNDID"/>
                </CompoundID>
                <DateCreated>
                  <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
                </DateCreated>
                <PersonCreated>
                  <xsl:value-of select="PERSONCREATED"/>
                </PersonCreated>
                <PersonRegistered>
                  <xsl:value-of select="PERSONREGISTERED"/>
                </PersonRegistered>
                <DateLastModified>
                  <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
                </DateLastModified>
                <Tag>
                  <xsl:value-of select="TAG"/>
                </Tag>
                <PropertyList>
                  <xsl:for-each select="..">
                    <xsl:apply-templates select="PropertyList"/>
                  </xsl:for-each>
                </PropertyList>
                <RegNumber>
                  <RegID>
                    <xsl:if test="string-length(REGID)!=0">
                      <xsl:value-of select="REGID"/>
                    </xsl:if>
                    <xsl:if test="string-length(REGID)=0">
                      <xsl:value-of select="'0'"/>
                    </xsl:if>
                  </RegID>
                  <SequenceNumber>
                    <xsl:value-of select="SEQUENCENUMBER"/>
                  </SequenceNumber>
                  <RegNumber>
                    <xsl:value-of select="REGNUMBER"/>
                  </RegNumber>
                  <SequenceID>
                    <xsl:value-of select="SEQUENCEID"/>
                  </SequenceID>
                </RegNumber>
                <BaseFragment>
                  <xsl:for-each select="../Structure/ROW">
                    <Structure>
                      <StructureID>
                        <xsl:value-of select="STRUCTUREID"/>
                      </StructureID>
                      <StructureFormat>
                        <xsl:value-of select="STRUCTUREFORMAT"/>
                      </StructureFormat>
                      <Structure>
                        <xsl:for-each select="STRUCTURE">
                          <xsl:attribute name="molWeight">
                            <xsl:value-of select="../FORMULAWEIGHT"/>
                          </xsl:attribute>
                          <xsl:attribute name="formula">
                            <xsl:value-of select="../MOLECULARFORMULA"/>
                          </xsl:attribute>
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </Structure>
                      <NormalizedStructure>
                        <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                      </NormalizedStructure>
                      <UseNormalization>
                        <xsl:value-of select="USENORMALIZATION"/>
                      </UseNormalization>
                      <DrawingType>
                        <xsl:value-of select="DRAWINGTYPE"/>
                      </DrawingType>
                      <PropertyList>
                        <xsl:for-each select="..">
                          <xsl:apply-templates select="PropertyList"/>
                        </xsl:for-each>
                      </PropertyList>
                      <IdentifierList>
                        <xsl:for-each select="../IdentifierList/ROW">
                          <Identifier>
                            <ID>
                              <xsl:value-of select="ID"/>
                            </ID>
                            <IdentifierID>
                              <xsl:for-each select="TYPE">
                                <xsl:attribute name="Description">
                                  <xsl:value-of select="../DESCRIPTION"/>
                                </xsl:attribute>
                                <xsl:attribute name="Name">
                                  <xsl:value-of select="../NAME"/>
                                </xsl:attribute>
                                <xsl:attribute name="Active">
                                  <xsl:value-of select="../ACTIVE"/>
                                </xsl:attribute>
                                <xsl:value-of select="."/>
                              </xsl:for-each>
                            </IdentifierID>
                            <InputText>
                              <xsl:value-of select="VALUE"/>
                            </InputText>
                          </Identifier>
                        </xsl:for-each>
                      </IdentifierList>
                    </Structure>
                  </xsl:for-each>
                </BaseFragment>
                <FragmentList>
                  <xsl:for-each select="../FragmentList/ROW">
                    <Fragment>
                      <CompoundFragmentID>
                        <xsl:value-of select="ID"/>
                      </CompoundFragmentID>
                      <FragmentID>
                        <xsl:value-of select="FRAGMENTID"/>
                      </FragmentID>
                      <Code>
                        <xsl:value-of select="CODE"/>
                      </Code>
                      <Name>
                        <xsl:value-of select="DESCRIPTION"/>
                      </Name>
                      <FragmentTypeID>
                        <xsl:attribute name="lookupTable">
                          <xsl:value-of select="'FragmentType'"/>
                        </xsl:attribute>
                        <xsl:attribute name="lookupField">
                          <xsl:value-of select="'ID'"/>
                        </xsl:attribute>
                        <xsl:attribute name="displayField">
                          <xsl:value-of select="'Description'"/>
                        </xsl:attribute>
                        <xsl:attribute name="displayValue">
                          <xsl:value-of select="TYPEDESCRIPTION"/>
                        </xsl:attribute>
                        <xsl:for-each select="FRAGMENTTYPEID">
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </FragmentTypeID>
                      <DateCreated>
                        <xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/>
                      </DateCreated>
                      <DateLastModified>
                        <xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/>
                      </DateLastModified>
                      <Equivalents>
                        <xsl:value-of select="EQUIVALENTS"/>
                      </Equivalents>
                      <Structure>
                        <StructureFormat>
                          <xsl:value-of select="STRUCTUREFORMAT"/>
                        </StructureFormat>
                        <xsl:variable name="VROW1" select="."/>
                        <Structure>
                          <xsl:for-each select="STRUCTURE">
                            <xsl:for-each select="$VROW1/MOLWEIGHT">
                              <xsl:attribute name="molWeight">
                                <xsl:value-of select="."/>
                              </xsl:attribute>
                            </xsl:for-each>
                            <xsl:for-each select="$VROW1/FORMULA">
                              <xsl:attribute name="formula">
                                <xsl:value-of select="."/>
                              </xsl:attribute>
                            </xsl:for-each>
                            <xsl:value-of select="."/>
                          </xsl:for-each>
                        </Structure>
                      </Structure>
                    </Fragment>
                  </xsl:for-each>
                </FragmentList>
                <IdentifierList>
                  <xsl:for-each select="../IdentifierList/ROW">
                    <Identifier>
                      <ID>
                        <xsl:value-of select="ID"/>
                      </ID>
                      <IdentifierID>
                        <xsl:for-each select="TYPE">
                          <xsl:attribute name="Description">
                            <xsl:value-of select="../DESCRIPTION"/>
                          </xsl:attribute>
                          <xsl:attribute name="Name">
                            <xsl:value-of select="../NAME"/>
                          </xsl:attribute>
                          <xsl:attribute name="Active">
                            <xsl:value-of select="../ACTIVE"/>
                          </xsl:attribute>
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </IdentifierID>
                      <InputText>
                        <xsl:value-of select="VALUE"/>
                      </InputText>
                    </Identifier>
                  </xsl:for-each>
                </IdentifierList>
              </Compound>
            </Component>
          </xsl:for-each>
        </ComponentList>
      </xsl:if>
      <!--Only Compound-->
      <xsl:if test="$VTypeRegistryRecord='WithoutMixture'">
        <xsl:for-each select="Compound/ROW">
          <Compound>
            <CompoundID>
              <xsl:value-of select="COMPOUNDID"/>
            </CompoundID>
            <DateCreated>
              <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
            </DateCreated>
            <PersonCreated>
              <xsl:value-of select="PERSONCREATED"/>
            </PersonCreated>
            <PersonRegistered>
              <xsl:value-of select="PERSONREGISTERED"/>
            </PersonRegistered>
            <Tag>
              <xsl:value-of select="TAG"/>
            </Tag>
            <DateLastModified>
              <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
            </DateLastModified>
            <PropertyList>
              <xsl:for-each select="..">
                <xsl:apply-templates select="PropertyList"/>
              </xsl:for-each>
            </PropertyList>
            <RegNumber>
              <RegID>
                <xsl:value-of select="REGID"/>
              </RegID>
              <SequenceNumber>
                <xsl:value-of select="SEQUENCENUMBER"/>
              </SequenceNumber>
              <RegNumber>
                <xsl:value-of select="REGNUMBER"/>
              </RegNumber>
              <SequenceID>
                <xsl:value-of select="SEQUENCEID"/>
              </SequenceID>
            </RegNumber>
            <BaseFragment>
              <xsl:for-each select="../Structure/ROW">
                <Structure>
                  <StructureID>
                    <xsl:value-of select="STRUCTUREID"/>
                  </StructureID>
                  <StructureFormat>
                    <xsl:value-of select="STRUCTUREFORMAT"/>
                  </StructureFormat>
                  <Structure>
                    <xsl:for-each select="STRUCTURE">
                      <xsl:attribute name="molWeight">
                        <xsl:value-of select="../FORMULAWEIGHT"/>
                      </xsl:attribute>
                      <xsl:attribute name="formula">
                        <xsl:value-of select="../MOLECULARFORMULA"/>
                      </xsl:attribute>
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </Structure>
                  <NormalizedStructure>
                    <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                  </NormalizedStructure>
                  <UseNormalization>
                    <xsl:value-of select="USENORMALIZATION"/>
                  </UseNormalization>
                  <DrawingType>
                    <xsl:value-of select="DRAWINGTYPE"/>
                  </DrawingType>
                  <PropertyList>
                    <xsl:for-each select="..">
                      <xsl:apply-templates select="PropertyList"/>
                    </xsl:for-each>
                  </PropertyList>
                  <IdentifierList>
                    <xsl:for-each select="../IdentifierList/ROW">
                      <Identifier>
                        <ID>
                          <xsl:value-of select="ID"/>
                        </ID>
                        <IdentifierID>
                          <xsl:for-each select="TYPE">
                            <xsl:attribute name="Description">
                              <xsl:value-of select="../DESCRIPTION"/>
                            </xsl:attribute>
                            <xsl:attribute name="Name">
                              <xsl:value-of select="../NAME"/>
                            </xsl:attribute>
                            <xsl:attribute name="Active">
                              <xsl:value-of select="../ACTIVE"/>
                            </xsl:attribute>
                            <xsl:value-of select="."/>
                          </xsl:for-each>
                        </IdentifierID>
                        <InputText>
                          <xsl:value-of select="VALUE"/>
                        </InputText>
                      </Identifier>
                    </xsl:for-each>
                  </IdentifierList>
                </Structure>
              </xsl:for-each>
            </BaseFragment>
            <FragmentList>
              <xsl:for-each select="Fragment/ROW">
                <Fragment>
                  <CompoundFragmentID>
                    <xsl:value-of select="ID"/>
                  </CompoundFragmentID>
                  <FragmentID>
                    <xsl:value-of select="FRAGMENTID"/>
                  </FragmentID>
                  <Code>
                    <xsl:value-of select="CODE"/>
                  </Code>
                  <Name>
                    <xsl:value-of select="DESCRIPTION"/>
                  </Name>
                  <FragmentTypeID>
                    <xsl:attribute name="lookupTable">
                      <xsl:value-of select="'FragmentType'"/>
                    </xsl:attribute>
                    <xsl:attribute name="lookupField">
                      <xsl:value-of select="'ID'"/>
                    </xsl:attribute>
                    <xsl:attribute name="displayField">
                      <xsl:value-of select="'Description'"/>
                    </xsl:attribute>
                    <xsl:attribute name="displayValue">
                      <xsl:value-of select="TYPEDESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:for-each select="FRAGMENTTYPEID">
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </FragmentTypeID>
                  <DateCreated>
                    <xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/>
                  </DateCreated>
                  <DateLastModified>
                    <xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/>
                  </DateLastModified>
                  <Equivalents>
                    <xsl:value-of select="EQUIVALENTS"/>
                  </Equivalents>
                  <Structure>
                    <StructureFormat>
                      <xsl:value-of select="STRUCTUREFORMAT"/>
                    </StructureFormat>
                    <xsl:variable name="VROW1" select="."/>
                    <Structure>
                      <xsl:for-each select="STRUCTURE">
                        <xsl:for-each select="$VROW1/MOLWEIGHT">
                          <xsl:attribute name="molWeight">
                            <xsl:value-of select="."/>
                          </xsl:attribute>
                        </xsl:for-each>
                        <xsl:for-each select="$VROW1/FORMULA">
                          <xsl:attribute name="formula">
                            <xsl:value-of select="."/>
                          </xsl:attribute>
                        </xsl:for-each>
                        <xsl:value-of select="."/>
                      </xsl:for-each>
                    </Structure>
                  </Structure>
                </Fragment>
              </xsl:for-each>
            </FragmentList>
            <IdentifierList>
              <xsl:for-each select="Identifier/ROW">
                <xsl:variable name="VROW3" select="."/>
                <Identifier>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <IdentifierID>
                    <xsl:for-each select="TYPE">
                      <xsl:for-each select="$VROW3/DESCRIPTION">
                        <xsl:attribute name="Description">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW3/NAME">
                        <xsl:attribute name="Name">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW3/ACTIVE">
                        <xsl:attribute name="Active">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </IdentifierID>
                  <InputText>
                    <xsl:value-of select="VALUE"/>
                  </InputText>
                </Identifier>
              </xsl:for-each>
            </IdentifierList>
          </Compound>
        </xsl:for-each>
      </xsl:if>
      <BatchList>
        <xsl:for-each select="Batch/ROW">
          <Batch>
            <BatchID>
              <xsl:value-of select="BATCHID"/>
            </BatchID>
            <BatchNumber>
              <xsl:value-of select="BATCHNUMBER"/>
            </BatchNumber>
            <FullRegNumber>
              <xsl:value-of select="FULLREGNUMBER"/>
            </FullRegNumber>
            <DateCreated>
              <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
            </DateCreated>
            <PersonCreated>
              <xsl:for-each select="PERSONCREATED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONCREATEDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonCreated>
            <PersonRegistered>
              <xsl:for-each select="PERSONREGISTERED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONREGISTEREDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonRegistered>
            <DateLastModified>
              <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
            </DateLastModified>
            <StatusID>
              <xsl:value-of select="STATUSID"/>
            </StatusID>
            <ProjectList>
              <xsl:for-each select="../ProjectList/ROW">
                <Project>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <ProjectID>
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
                  </ProjectID>
                </Project>
              </xsl:for-each>
            </ProjectList>
            <IdentifierList>
              <xsl:for-each select="../IdentifierList/ROW">
                <Identifier>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <IdentifierID>
                    <xsl:attribute name="Description">
                      <xsl:value-of select="DESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:attribute name="Name">
                      <xsl:value-of select="NAME"/>
                    </xsl:attribute>
                    <xsl:attribute name="Active">
                      <xsl:value-of select="ACTIVE"/>
                    </xsl:attribute>
                    <xsl:value-of select="TYPE"/>
                  </IdentifierID>
                  <InputText>
                    <xsl:value-of select="VALUE"/>
                  </InputText>
                </Identifier>
              </xsl:for-each>
            </IdentifierList>
            <PropertyList>
              <xsl:for-each select="..">
                <xsl:apply-templates select="PropertyList"/>
              </xsl:for-each>
            </PropertyList>
            <BatchComponentList>
              <xsl:for-each select="../BatchComponent/ROW">
                <BatchComponent>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <BatchID>
                    <xsl:value-of select="BATCHID"/>
                  </BatchID>
                  <CompoundID>
                    <xsl:value-of select="COMPOUNDID"/>
                  </CompoundID>
                  <MixtureComponentID>
                    <xsl:value-of select="MIXTURECOMPONENTID"/>
                  </MixtureComponentID>
                  <ComponentIndex>
                    <xsl:value-of select="COMPONENTINDEX"/>
                  </ComponentIndex>
                  <PropertyList>
                    <xsl:for-each select="..">
                      <xsl:apply-templates select="PropertyList"/>
                    </xsl:for-each>
                  </PropertyList>
                  <BatchComponentFragmentList>
                    <xsl:for-each select="../BatchComponentFragment/ROW">
                      <BatchComponentFragment>
                        <ID>
                          <xsl:value-of select="ID"/>
                        </ID>
                        <CompoundFragmentID>
                          <xsl:value-of select="COMPOUNDFRAGMENTID"/>
                        </CompoundFragmentID>
                        <FragmentID>
                          <xsl:value-of select="FRAGMENTID"/>
                        </FragmentID>
                        <Equivalents>
                          <xsl:value-of select="EQUIVALENT"/>
                        </Equivalents>
                      </BatchComponentFragment>
                    </xsl:for-each>
                  </BatchComponentFragmentList>
                </BatchComponent>
              </xsl:for-each>
            </BatchComponentList>
          </Batch>
        </xsl:for-each>
      </BatchList>
    </MultiCompoundRegistryRecord>
  </xsl:template>
  <xsl:template match="PropertyList">
    <xsl:for-each select="ROW">
      <xsl:for-each select="*">
        <Property>
          <xsl:attribute name="name">
            <xsl:value-of select="name()"/>
          </xsl:attribute>
          <xsl:if test="string-length(@pickListDomainID)!=0">
            <xsl:attribute name="pickListDomainID">
              <xsl:value-of select="@pickListDomainID"/>
            </xsl:attribute>
            <xsl:attribute name="pickListDisplayValue">
              <xsl:value-of select="@pickListDisplayValue"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:value-of select="."/>
        </Property>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>'
);
else
vXslRetrieveMcrr := XmlType.CreateXml(
'<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:variable name="Vlower" select="'abcdefghijklmnopqrstuvwxyz'"/>
  <xsl:variable name="VUPPER" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:attribute name="SameBatchesIdentity">
        <xsl:value-of select="@SameBatchesIdentity"/>
      </xsl:attribute>
      <xsl:attribute name="ActiveRLS">
        <xsl:value-of select="@ActiveRLS"/>
      </xsl:attribute>
      <xsl:attribute name="IsEditable">
        <xsl:value-of select="@IsEditable"/>
      </xsl:attribute>
      <xsl:variable name="VMultiCompoundRegistryRecord" select="."/>
      <xsl:variable name="VTypeRegistryRecord" select="@TypeRegistryRecord"/>
      <xsl:for-each select="Mixture/ROW">
        <ID>
          <xsl:value-of select="MIXTUREID"/>
        </ID>
        <DateCreated>
          <xsl:value-of select="CREATED"/>
        </DateCreated>
        <DateLastModified>
          <xsl:value-of select="MODIFIED"/>
        </DateLastModified>
        <PersonCreated>
          <xsl:value-of select="PERSONCREATED"/>
        </PersonCreated>
        <StructureAggregation>
          <xsl:value-of select="STRUCTUREAGGREGATION"/>
        </StructureAggregation>
        <Approved>
          <xsl:value-of select="APPROVED"/>
        </Approved>
        <xsl:copy-of select="../PropertyList"/>
        <RegNumber>
          <RegID>
            <xsl:value-of select="REGID"/>
          </RegID>
          <SequenceNumber>
            <xsl:value-of select="SEQUENCENUMBER"/>
          </SequenceNumber>
          <RegNumber>
            <xsl:value-of select="REGNUMBER"/>
          </RegNumber>
          <SequenceID>
            <xsl:value-of select="SEQUENCEID"/>
          </SequenceID>
        </RegNumber>
        <IdentifierList>
          <xsl:for-each select="../IdentifierList/ROW">
            <Identifier>
              <ID>
                <xsl:value-of select="ID"/>
              </ID>
              <IdentifierID>
                <xsl:for-each select="TYPE">
                  <xsl:attribute name="Description">
                    <xsl:value-of select="../DESCRIPTION"/>
                  </xsl:attribute>
                  <xsl:attribute name="Name">
                    <xsl:value-of select="../NAME"/>
                  </xsl:attribute>
                  <xsl:attribute name="Active">
                    <xsl:value-of select="../ACTIVE"/>
                  </xsl:attribute>
                  <xsl:value-of select="."/>
                </xsl:for-each>
              </IdentifierID>
              <InputText>
                <xsl:value-of select="VALUE"/>
              </InputText>
            </Identifier>
          </xsl:for-each>
        </IdentifierList>
        <ProjectList>
          <xsl:for-each select="../ProjectList/ROW">
            <Project>
              <ID>
                <xsl:value-of select="ID"/>
              </ID>
              <ProjectID>
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
              </ProjectID>
            </Project>
          </xsl:for-each>
        </ProjectList>
      </xsl:for-each>
      <xsl:if test="$VTypeRegistryRecord='Mixture'">
        <ComponentList>
          <xsl:for-each select="Compound/ROW">
            <Component>
              <ID/>
              <ComponentIndex>
                <xsl:value-of select="COMPONENTINDEX"/>
              </ComponentIndex>
              <Compound>
                <CompoundID>
                  <xsl:value-of select="COMPOUNDID"/>
                </CompoundID>
                <DateCreated>
                  <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
                </DateCreated>
                <PersonCreated>
                  <xsl:value-of select="PERSONCREATED"/>
                </PersonCreated>
                <PersonRegistered>
                  <xsl:value-of select="PERSONREGISTERED"/>
                </PersonRegistered>
                <DateLastModified>
                  <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
                </DateLastModified>
                <Tag>
                  <xsl:value-of select="TAG"/>
                </Tag>
                <xsl:copy-of select="../PropertyList"/>
                <RegNumber>
                  <RegID>
                    <xsl:if test="string-length(REGID)!=0">
                      <xsl:value-of select="REGID"/>
                    </xsl:if>
                    <xsl:if test="string-length(REGID)=0">
                      <xsl:value-of select="'0'"/>
                    </xsl:if>
                  </RegID>
                  <SequenceNumber>
                    <xsl:value-of select="SEQUENCENUMBER"/>
                  </SequenceNumber>
                  <RegNumber>
                    <xsl:value-of select="REGNUMBER"/>
                  </RegNumber>
                  <SequenceID>
                    <xsl:value-of select="SEQUENCEID"/>
                  </SequenceID>
                </RegNumber>
                <BaseFragment>
                  <xsl:for-each select="../Structure/ROW">
                    <Structure>
                      <StructureID>
                        <xsl:value-of select="STRUCTUREID"/>
                      </StructureID>
                      <StructureFormat>
                        <xsl:value-of select="STRUCTUREFORMAT"/>
                      </StructureFormat>
                      <Structure>
                        <xsl:for-each select="STRUCTURE">
                          <xsl:attribute name="molWeight">
                            <xsl:value-of select="../FORMULAWEIGHT"/>
                          </xsl:attribute>
                          <xsl:attribute name="formula">
                            <xsl:value-of select="../MOLECULARFORMULA"/>
                          </xsl:attribute>
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </Structure>
                      <NormalizedStructure>
                        <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                      </NormalizedStructure>
                      <UseNormalization>
                        <xsl:value-of select="USENORMALIZATION"/>
                      </UseNormalization>
                      <xsl:copy-of select="../PropertyList"/>
                      <IdentifierList>
                        <xsl:for-each select="../IdentifierList/ROW">
                          <Identifier>
                            <ID>
                              <xsl:value-of select="ID"/>
                            </ID>
                            <IdentifierID>
                              <xsl:for-each select="TYPE">
                                <xsl:attribute name="Description">
                                  <xsl:value-of select="../DESCRIPTION"/>
                                </xsl:attribute>
                                <xsl:attribute name="Name">
                                  <xsl:value-of select="../NAME"/>
                                </xsl:attribute>
                                <xsl:attribute name="Active">
                                  <xsl:value-of select="../ACTIVE"/>
                                </xsl:attribute>
                                <xsl:value-of select="."/>
                              </xsl:for-each>
                            </IdentifierID>
                            <InputText>
                              <xsl:value-of select="VALUE"/>
                            </InputText>
                          </Identifier>
                        </xsl:for-each>
                      </IdentifierList>
                    </Structure>
                  </xsl:for-each>
                </BaseFragment>
                <FragmentList>
                  <xsl:for-each select="../FragmentList/ROW">
                    <Fragment>
                      <CompoundFragmentID>
                        <xsl:value-of select="ID"/>
                      </CompoundFragmentID>
                      <FragmentID>
                        <xsl:value-of select="FRAGMENTID"/>
                      </FragmentID>
                      <Code>
                        <xsl:value-of select="CODE"/>
                      </Code>
                      <Name>
                        <xsl:value-of select="DESCRIPTION"/>
                      </Name>
                      <FragmentTypeID>
                        <xsl:attribute name="lookupTable">
                          <xsl:value-of select="'FragmentType'"/>
                        </xsl:attribute>
                        <xsl:attribute name="lookupField">
                          <xsl:value-of select="'ID'"/>
                        </xsl:attribute>
                        <xsl:attribute name="displayField">
                          <xsl:value-of select="'Description'"/>
                        </xsl:attribute>
                        <xsl:attribute name="displayValue">
                          <xsl:value-of select="TYPEDESCRIPTION"/>
                        </xsl:attribute>
                        <xsl:for-each select="FRAGMENTTYPEID">
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </FragmentTypeID>
                      <DateCreated>
                        <xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/>
                      </DateCreated>
                      <DateLastModified>
                        <xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/>
                      </DateLastModified>
                      <Equivalents>
                        <xsl:value-of select="EQUIVALENTS"/>
                      </Equivalents>
                      <Structure>
                        <StructureFormat>
                          <xsl:value-of select="STRUCTUREFORMAT"/>
                        </StructureFormat>
                        <xsl:variable name="VROW1" select="."/>
                        <Structure>
                          <xsl:for-each select="STRUCTURE">
                            <xsl:for-each select="$VROW1/MOLWEIGHT">
                              <xsl:attribute name="molWeight">
                                <xsl:value-of select="."/>
                              </xsl:attribute>
                            </xsl:for-each>
                            <xsl:for-each select="$VROW1/FORMULA">
                              <xsl:attribute name="formula">
                                <xsl:value-of select="."/>
                              </xsl:attribute>
                            </xsl:for-each>
                            <xsl:value-of select="."/>
                          </xsl:for-each>
                        </Structure>
                      </Structure>
                    </Fragment>
                  </xsl:for-each>
                </FragmentList>
                <IdentifierList>
                  <xsl:for-each select="../IdentifierList/ROW">
                    <Identifier>
                      <ID>
                        <xsl:value-of select="ID"/>
                      </ID>
                      <IdentifierID>
                        <xsl:for-each select="TYPE">
                          <xsl:attribute name="Description">
                            <xsl:value-of select="../DESCRIPTION"/>
                          </xsl:attribute>
                          <xsl:attribute name="Name">
                            <xsl:value-of select="../NAME"/>
                          </xsl:attribute>
                          <xsl:attribute name="Active">
                            <xsl:value-of select="../ACTIVE"/>
                          </xsl:attribute>
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </IdentifierID>
                      <InputText>
                        <xsl:value-of select="VALUE"/>
                      </InputText>
                    </Identifier>
                  </xsl:for-each>
                </IdentifierList>
              </Compound>
            </Component>
          </xsl:for-each>
        </ComponentList>
      </xsl:if>
      <!--Only Compound-->
      <xsl:if test="$VTypeRegistryRecord='WithoutMixture'">
        <xsl:for-each select="Compound/ROW">
          <Compound>
            <CompoundID>
              <xsl:value-of select="COMPOUNDID"/>
            </CompoundID>
            <DateCreated>
              <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
            </DateCreated>
            <PersonCreated>
              <xsl:value-of select="PERSONCREATED"/>
            </PersonCreated>
            <PersonRegistered>
              <xsl:value-of select="PERSONREGISTERED"/>
            </PersonRegistered>
            <Tag>
              <xsl:value-of select="TAG"/>
            </Tag>
            <DateLastModified>
              <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
            </DateLastModified>
            <xsl:copy-of select="../PropertyList"/>
            <RegNumber>
              <RegID>
                <xsl:value-of select="REGID"/>
              </RegID>
              <SequenceNumber>
                <xsl:value-of select="SEQUENCENUMBER"/>
              </SequenceNumber>
              <RegNumber>
                <xsl:value-of select="REGNUMBER"/>
              </RegNumber>
              <SequenceID>
                <xsl:value-of select="SEQUENCEID"/>
              </SequenceID>
            </RegNumber>
            <BaseFragment>
              <Structure>
                <StructureID>
                  <xsl:value-of select="STRUCTUREID"/>
                </StructureID>
                <StructureFormat>
                  <xsl:value-of select="STRUCTUREFORMAT"/>
                </StructureFormat>
                <xsl:variable name="VROW" select="."/>
                <Structure>
                  <xsl:for-each select="STRUCTURE">
                    <xsl:for-each select="$VROW/FORMULAWEIGHT">
                      <xsl:attribute name="molWeight">
                        <xsl:value-of select="."/>
                      </xsl:attribute>
                    </xsl:for-each>
                    <xsl:for-each select="$VROW/MOLECULARFORMULA">
                      <xsl:attribute name="formula">
                        <xsl:value-of select="."/>
                      </xsl:attribute>
                    </xsl:for-each>
                    <xsl:value-of select="."/>
                  </xsl:for-each>
                  <xsl:copy-of select="../PropertyList"/>
                  <IdentifierList>
                    <xsl:for-each select="../IdentifierList/ROW">
                      <Identifier>
                        <ID>
                          <xsl:value-of select="ID"/>
                        </ID>
                        <IdentifierID>
                          <xsl:for-each select="TYPE">
                            <xsl:attribute name="Description">
                              <xsl:value-of select="../DESCRIPTION"/>
                            </xsl:attribute>
                            <xsl:attribute name="Name">
                              <xsl:value-of select="../NAME"/>
                            </xsl:attribute>
                            <xsl:attribute name="Active">
                              <xsl:value-of select="../ACTIVE"/>
                            </xsl:attribute>
                            <xsl:value-of select="."/>
                          </xsl:for-each>
                        </IdentifierID>
                        <InputText>
                          <xsl:value-of select="VALUE"/>
                        </InputText>
                      </Identifier>
                    </xsl:for-each>
                  </IdentifierList>
                </Structure>
                <NormalizedStructure>
                  <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                </NormalizedStructure>
                <UseNormalization>
                  <xsl:value-of select="USENORMALIZATION"/>
                </UseNormalization>
              </Structure>
              <xsl:copy-of select="../PropertyList"/>
            </BaseFragment>
            <FragmentList>
              <xsl:for-each select="Fragment/ROW">
                <Fragment>
                  <CompoundFragmentID>
                    <xsl:value-of select="ID"/>
                  </CompoundFragmentID>
                  <FragmentID>
                    <xsl:value-of select="FRAGMENTID"/>
                  </FragmentID>
                  <Code>
                    <xsl:value-of select="CODE"/>
                  </Code>
                  <Name>
                    <xsl:value-of select="DESCRIPTION"/>
                  </Name>
                  <FragmentTypeID>
                    <xsl:attribute name="lookupTable">
                      <xsl:value-of select="'FragmentType'"/>
                    </xsl:attribute>
                    <xsl:attribute name="lookupField">
                      <xsl:value-of select="'ID'"/>
                    </xsl:attribute>
                    <xsl:attribute name="displayField">
                      <xsl:value-of select="'Description'"/>
                    </xsl:attribute>
                    <xsl:attribute name="displayValue">
                      <xsl:value-of select="TYPEDESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:for-each select="FRAGMENTTYPEID">
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </FragmentTypeID>
                  <DateCreated>
                    <xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/>
                  </DateCreated>
                  <DateLastModified>
                    <xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/>
                  </DateLastModified>
                  <Equivalents>
                    <xsl:value-of select="EQUIVALENTS"/>
                  </Equivalents>
                  <Structure>
                    <StructureFormat>
                      <xsl:value-of select="STRUCTUREFORMAT"/>
                    </StructureFormat>
                    <xsl:variable name="VROW1" select="."/>
                    <Structure>
                      <xsl:for-each select="STRUCTURE">
                        <xsl:for-each select="$VROW1/MOLWEIGHT">
                          <xsl:attribute name="molWeight">
                            <xsl:value-of select="."/>
                          </xsl:attribute>
                        </xsl:for-each>
                        <xsl:for-each select="$VROW1/FORMULA">
                          <xsl:attribute name="formula">
                            <xsl:value-of select="."/>
                          </xsl:attribute>
                        </xsl:for-each>
                        <xsl:value-of select="."/>
                      </xsl:for-each>
                    </Structure>
                  </Structure>
                </Fragment>
              </xsl:for-each>
            </FragmentList>
            <IdentifierList>
              <xsl:for-each select="Identifier/ROW">
                <xsl:variable name="VROW3" select="."/>
                <Identifier>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <IdentifierID>
                    <xsl:for-each select="TYPE">
                      <xsl:for-each select="$VROW3/DESCRIPTION">
                        <xsl:attribute name="Description">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW3/NAME">
                        <xsl:attribute name="Name">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW3/ACTIVE">
                        <xsl:attribute name="Active">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </IdentifierID>
                  <InputText>
                    <xsl:value-of select="VALUE"/>
                  </InputText>
                </Identifier>
              </xsl:for-each>
            </IdentifierList>
          </Compound>
        </xsl:for-each>
      </xsl:if>
      <BatchList>
        <xsl:for-each select="Batch/ROW">
          <Batch>
            <BatchID>
              <xsl:value-of select="BATCHID"/>
            </BatchID>
            <BatchNumber>
              <xsl:value-of select="BATCHNUMBER"/>
            </BatchNumber>
            <FullRegNumber>
              <xsl:value-of select="FULLREGNUMBER"/>
            </FullRegNumber>
            <DateCreated>
              <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
            </DateCreated>
            <PersonCreated>
              <xsl:for-each select="PERSONCREATED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONCREATEDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonCreated>
            <PersonRegistered>
              <xsl:for-each select="PERSONREGISTERED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONREGISTEREDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonRegistered>
            <DateLastModified>
              <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
            </DateLastModified>
            <StatusID>
              <xsl:value-of select="STATUSID"/>
            </StatusID>
            <ProjectList>
              <xsl:for-each select="../ProjectList/ROW">
                <Project>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <ProjectID>
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
                  </ProjectID>
                </Project>
              </xsl:for-each>
            </ProjectList>
            <IdentifierList>
              <xsl:for-each select="../IdentifierList/ROW">
                <Identifier>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <IdentifierID>
                    <xsl:attribute name="Description">
                      <xsl:value-of select="DESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:attribute name="Name">
                      <xsl:value-of select="NAME"/>
                    </xsl:attribute>
                    <xsl:attribute name="Active">
                      <xsl:value-of select="ACTIVE"/>
                    </xsl:attribute>
                    <xsl:value-of select="TYPE"/>
                  </IdentifierID>
                  <InputText>
                    <xsl:value-of select="VALUE"/>
                  </InputText>
                </Identifier>
              </xsl:for-each>
            </IdentifierList>
            <xsl:copy-of select="../PropertyList"/>
            <BatchComponentList>
              <xsl:for-each select="../BatchComponent/ROW">
                <BatchComponent>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <BatchID>
                    <xsl:value-of select="BATCHID"/>
                  </BatchID>
                  <CompoundID>
                    <xsl:value-of select="COMPOUNDID"/>
                  </CompoundID>
                  <MixtureComponentID>
                    <xsl:value-of select="MIXTURECOMPONENTID"/>
                  </MixtureComponentID>
                  <ComponentIndex>
                    <xsl:value-of select="COMPONENTINDEX"/>
                  </ComponentIndex>
                  <xsl:copy-of select="../PropertyList"/>
                  <BatchComponentFragmentList>
                    <xsl:for-each select="../BatchComponentFragment/ROW">
                      <BatchComponentFragment>
                        <ID>
                          <xsl:value-of select="ID"/>
                        </ID>
                        <CompoundFragmentID>
                          <xsl:value-of select="COMPOUNDFRAGMENTID"/>
                        </CompoundFragmentID>
                        <FragmentID>
                          <xsl:value-of select="FRAGMENTID"/>
                        </FragmentID>
                        <Equivalents>
                          <xsl:value-of select="EQUIVALENT"/>
                        </Equivalents>
                      </BatchComponentFragment>
                    </xsl:for-each>
                  </BatchComponentFragmentList>
                </BatchComponent>
              </xsl:for-each>
            </BatchComponentList>
          </Batch>
        </xsl:for-each>
      </BatchList>
    </MultiCompoundRegistryRecord>
  </xsl:template>
</xsl:stylesheet>