prompt 
prompt Starting "pkg_CompoundRegistry_body.sql"...
prompt

CREATE OR REPLACE PACKAGE BODY REGDB."COMPOUNDREGISTRY" IS

    --Variables
    VBatchNumberPad INTEGER;

    --Forward declarations
    FUNCTION TakeOffAndGetClob(AXml IN OUT NOCOPY Clob,ABeginTag VARCHAR2) RETURN CLOB;

    /** XML Declaration constant */
    cXmlDecoration Constant Varchar2(30) := '<?xml version="1.0"?>';

    /** Special section list constant, signifying a 'write-only' action */
    cSectionListEmpty Constant Char(4) := 'NONE';

    /** XSL transform used to fetch a 'temporary' registry record */
    cXslRetrieveMCRRTemp Constant XmlType := XmlType.CreateXml(
'<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
    <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
    <xsl:template match="/MultiCompoundRegistryRecord">
        <MultiCompoundRegistryRecord>
            <xsl:attribute name="SameBatchesIdentity"><xsl:value-of select="@SameBatchesIdentity"/></xsl:attribute>
            <xsl:attribute name="IsEditable"><xsl:value-of select="@IsEditable"/></xsl:attribute>
            <ID><xsl:value-of select="Batch/ROW/TEMPBATCHID"/></ID>
            <DateCreated/>
            <DateLastModified/>
            <PersonCreated/>
            <StructureAggregation>
                <xsl:value-of select="Batch/ROW/STRUCTUREAGGREGATION"/>
            </StructureAggregation>
            <RegNumber>
                <RegID>0</RegID>
                <SequenceNumber/>
                <RegNumber/>
                <SequenceID><xsl:value-of select="Batch/ROW/SEQUENCEID"/></SequenceID>
            </RegNumber>
            <xsl:for-each select="Batch/ROW[1]/IDENTIFIERXML">
                <xsl:value-of select="."/>
            </xsl:for-each>
            <PropertyList>
                <xsl:for-each select="Mixture/ROW[1]/PropertyList">
                    <xsl:for-each select="node()">
                        <Property>
                            <xsl:attribute name="name"><xsl:value-of select="name()"/></xsl:attribute>
                            <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                            <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                            <xsl:value-of select="."/>
                        </Property>
                    </xsl:for-each>
                </xsl:for-each>
            </PropertyList>

            <ProjectList>
                <xsl:for-each select="Project/ROW">
                    <Project>
                        <xsl:variable name="VProject" select="."/>
                        <ID><xsl:value-of select="ID"/></ID>
                        <ProjectID>
                            <xsl:for-each select="PROJECTID">
                                <xsl:for-each select="$VProject/DESCRIPTION">
                                    <xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
                                </xsl:for-each>
                                <xsl:for-each select="$VProject/NAME">
                                    <xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
                                </xsl:for-each>
                                <xsl:for-each select="$VProject/ACTIVE">
                                    <xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
                                </xsl:for-each>
                                <xsl:value-of select="."/>
                            </xsl:for-each>
                        </ProjectID>
                    </Project>
                </xsl:for-each>
            </ProjectList>

            <ComponentList>
                <xsl:for-each select="Compound/ROW">
                    <Component>
                        <ID/>
                        <ComponentIndex><xsl:value-of select="COMPONENTINDEX"/></ComponentIndex>
                        <Compound>
                            <CompoundID><xsl:value-of select="TEMPCOMPOUNDID"/></CompoundID>
                            <DateCreated><xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/></DateCreated>
                            <PersonCreated><xsl:value-of select="PERSONCREATED"/></PersonCreated>
                            <DateLastModified><xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/></DateLastModified>
                            <Tag><xsl:value-of select="TAG"/></Tag>
                            <xsl:for-each select="PropertyList">
                                <PropertyList>
                                    <xsl:for-each select="node()">
                                        <Property>
                                            <xsl:attribute name="name"><xsl:value-of select="name()"/></xsl:attribute>
                                            <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                                            <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                                            <xsl:value-of select="."/>
                                        </Property>
                                    </xsl:for-each>
                                </PropertyList>
                            </xsl:for-each>
                            <RegNumber>
                                <RegID>
                                    <!-- JED changed so default REGID is zero -->
                                    <xsl:if test="string-length(REGID)!=0">
                                        <xsl:value-of select="REGID"/>
                                    </xsl:if>
                                    <xsl:if test="string-length(REGID)=0">
                                        <xsl:value-of select="0"/>
                                    </xsl:if>
                                </RegID>
                                <SequenceNumber><xsl:value-of select="SEQUENCENUMBER"/></SequenceNumber>
                                <RegNumber><xsl:value-of select="REGNUMBER"/></RegNumber>
                                <SequenceID><xsl:value-of select="SEQUENCEID"/></SequenceID>
                            </RegNumber>
                            <BaseFragment>
                                <Structure>
                                    <StructureID><xsl:value-of select="STRUCTUREID"/></StructureID>
                                    <StructureFormat><xsl:value-of select="STRUCTUREFORMAT"/></StructureFormat>
                                    <xsl:variable name="VROW" select="."/>
                                    <Structure>
                                        <xsl:for-each select="STRUCTURE">
                                            <xsl:attribute name="molWeight"><xsl:value-of select="$VROW/FORMULAWEIGHT"/></xsl:attribute>
                                            <xsl:attribute name="formula"><xsl:value-of select="$VROW/MOLECULARFORMULA"/></xsl:attribute>
                                            <xsl:value-of select="."/>
                                        </xsl:for-each>
                                    </Structure>
                                    <NormalizedStructure><xsl:value-of select="NORMALIZEDSTRUCTURE"/></NormalizedStructure>
                                    <UseNormalization><xsl:value-of select="USENORMALIZATION"/></UseNormalization>
                                </Structure>
                            </BaseFragment>
                            <xsl:for-each select="FRAGMENTXML"><xsl:value-of select="."/></xsl:for-each>
                            <xsl:for-each select="IDENTIFIERXML"><xsl:value-of select="."/></xsl:for-each>
                        </Compound>
                    </Component>
                </xsl:for-each>
            </ComponentList>
            <BatchList>
                <xsl:for-each select="Batch/ROW">
                    <Batch>
                        <BatchID><xsl:value-of select="TEMPBATCHID"/></BatchID>
                        <BatchNumber><xsl:value-of select="BATCHNUMBER"/></BatchNumber>
                        <DateCreated>
                            <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
                        </DateCreated>
                        <xsl:variable name="VROW3" select="."/>
                        <PersonCreated>
                            <xsl:for-each select="PERSONCREATED">
                                <!-- JED: note, this element (PERSONCREATEDDISPLAY) does not exist -->
                                <xsl:attribute name="displayName"><xsl:value-of select="$VROW3/PERSONCREATEDDISPLAY"/></xsl:attribute>
                                <xsl:value-of select="."/>
                            </xsl:for-each>
                        </PersonCreated>
                        <DateLastModified>
                            <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
                        </DateLastModified>
                        <StatusID><xsl:value-of select="STATUSID"/></StatusID>
                        <ProjectList>
                            <xsl:for-each select="Project/ROW">
                                <Project>
                                    <xsl:variable name="VProject" select="."/>
                                    <ID><xsl:value-of select="ID"/></ID>
                                    <ProjectID>
                                        <xsl:for-each select="PROJECTID">
                                            <xsl:for-each select="$VProject/DESCRIPTION">
                                                <xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:for-each select="$VProject/NAME">
                                                <xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:for-each select="$VProject/ACTIVE">
                                                <xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:value-of select="."/>
                                        </xsl:for-each>
                                    </ProjectID>
                                </Project>
                            </xsl:for-each>
                        </ProjectList>
                        <xsl:for-each select="IDENTIFIERXMLBATCH"><xsl:value-of select="."/></xsl:for-each>
                        <xsl:for-each select="PropertyList">
                            <PropertyList>
                                <xsl:for-each select="node()">
                                    <xsl:variable name="aName" select="name()"/>
                                    <xsl:variable name="eValue" select="translate(., $VUPPER, $Vlower)"/>
                                    <Property>
                                        <xsl:attribute name="name"><xsl:value-of select="$aName"/></xsl:attribute>
                                        <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                                        <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                                        <xsl:choose>
                                            <xsl:when test="$aName = ''DELIVERYDATE'' and string-length(.) != 0">
                                                <xsl:value-of select="$eValue"/>
                                            </xsl:when>
                                            <xsl:when test="$aName = ''DATEENTERED'' and string-length(.) != 0">
                                                <xsl:value-of select="$eValue"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:value-of select="."/>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    </Property>
                                </xsl:for-each>
                            </PropertyList>
                        </xsl:for-each>
                        <BatchComponentList>
                            <xsl:for-each select="BatchComponent/ROW">
                                <BatchComponent>
                                    <ID/>
                                    <BatchID>
                                        <xsl:value-of select="TEMPBATCHID"/>
                                    </BatchID>
                                    <CompoundID>
                                        <xsl:value-of select="TEMPCOMPOUNDID"/>
                                    </CompoundID>
                                    <ComponentIndex>
                                        <xsl:value-of select="COMPONENTINDEX"/>
                                    </ComponentIndex>
                                    <xsl:for-each select="PropertyList">
                                        <PropertyList>
                                            <xsl:for-each select="node()">
                                                <Property>
                                                    <xsl:attribute name="name"><xsl:value-of select="name()"/></xsl:attribute>
                                                    <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                                                    <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                                                    <xsl:value-of select="."/>
                                                </Property>
                                            </xsl:for-each>
                                        </PropertyList>
                                    </xsl:for-each>
                                    <xsl:for-each select="BATCHCOMPFRAGMENTXML">
                                        <xsl:value-of select="."/>
                                    </xsl:for-each>
                                </BatchComponent>
                            </xsl:for-each>
                        </BatchComponentList>
                    </Batch>
                </xsl:for-each>
            </BatchList>
        </MultiCompoundRegistryRecord>
    </xsl:template>
</xsl:stylesheet>');

    /** XSL transform used to fetch a 'permanent' registry record */
    cXslRetrieveMCRR Constant XmlType := XmlType.CreateXml(
'<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
    <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
    <xsl:template match="/MultiCompoundRegistryRecord">
        <MultiCompoundRegistryRecord>
            <xsl:attribute name="SameBatchesIdentity"><xsl:value-of select="@SameBatchesIdentity"/></xsl:attribute>
            <xsl:attribute name="ActiveRLS"><xsl:value-of select="@ActiveRLS"/></xsl:attribute>
            <xsl:attribute name="IsEditable"><xsl:value-of select="@IsEditable"/></xsl:attribute>
            <xsl:variable name="VMultiCompoundRegistryRecord" select="."/>
            <xsl:variable name="VTypeRegistryRecord" select="@TypeRegistryRecord"/>
            <xsl:for-each select="Mixture">
                <ID><xsl:value-of select="MIXTUREID"/></ID>
                <DateCreated><xsl:value-of select="CREATED"/></DateCreated>
                <DateLastModified><xsl:value-of select="MODIFIED"/></DateLastModified>
                <PersonCreated><xsl:value-of select="PERSONCREATED"/></PersonCreated>
                <StructureAggregation><xsl:value-of select="STRUCTUREAGGREGATION"/></StructureAggregation>
                <Approved><xsl:value-of select="APPROVED"/></Approved>
                <PropertyList>
                    <xsl:for-each select="PropertyList/ROW">
                        <xsl:for-each select="node()">
                            <Property>
                                <xsl:attribute name="name"><xsl:value-of select="name()"/></xsl:attribute>
                                <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                                <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                                <xsl:value-of select="."/>
                            </Property>
                        </xsl:for-each>
                    </xsl:for-each>
                </PropertyList>
                <RegNumber>
                    <RegID><xsl:value-of select="REGID"/></RegID>
                    <SequenceNumber><xsl:value-of select="SEQUENCENUMBER"/></SequenceNumber>
                    <RegNumber><xsl:value-of select="REGNUMBER"/></RegNumber>
                    <SequenceID><xsl:value-of select="SEQUENCEID"/></SequenceID>
                </RegNumber>
                <IdentifierList>
                    <xsl:for-each select="Identifier/ROW">
                        <xsl:variable name="VROW5" select="."/>
                        <Identifier>
                            <ID><xsl:value-of select="ID"/></ID>
                            <IdentifierID>
                                <xsl:for-each select="TYPE">
                                    <xsl:for-each select="$VROW5/DESCRIPTION">
                                        <xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
                                    </xsl:for-each>
                                    <xsl:for-each select="$VROW5/NAME">
                                        <xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
                                    </xsl:for-each>
                                    <xsl:for-each select="$VROW5/ACTIVE">
                                        <xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
                                    </xsl:for-each>
                                    <xsl:value-of select="."/>
                                </xsl:for-each>
                            </IdentifierID>
                            <InputText><xsl:value-of select="VALUE"/></InputText>
                        </Identifier>
                    </xsl:for-each>
                </IdentifierList>
                <ProjectList>
                    <xsl:for-each select="RegistryRecord_Project/ROW">
                        <Project>
                            <xsl:variable name="VROW6" select="."/>
                            <ID><xsl:value-of select="ID"/></ID>
                            <ProjectID>
                                <xsl:for-each select="PROJECTID">
                                    <xsl:for-each select="$VROW6/DESCRIPTION">
                                        <xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
                                    </xsl:for-each>
                                    <xsl:for-each select="$VROW6/NAME">
                                        <xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
                                    </xsl:for-each>
                                    <xsl:for-each select="$VROW6/ACTIVE">
                                        <xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
                                    </xsl:for-each>
                                    <xsl:value-of select="."/>
                                </xsl:for-each>
                            </ProjectID>
                        </Project>
                    </xsl:for-each>
                </ProjectList>
            </xsl:for-each>
            <xsl:if test="$VTypeRegistryRecord=''Mixture''">
                <ComponentList>
                    <xsl:for-each select="Compound/ROW">
                        <Component>
                            <ID/>
                            <ComponentIndex><xsl:value-of select="COMPONENTINDEX"/></ComponentIndex>
                            <Compound>
                                <CompoundID><xsl:value-of select="COMPOUNDID"/></CompoundID>
                                <DateCreated><xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/></DateCreated>
                                <PersonCreated><xsl:value-of select="PERSONCREATED"/></PersonCreated>
                                <PersonRegistered><xsl:value-of select="PERSONREGISTERED"/></PersonRegistered>
                                <DateLastModified><xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/></DateLastModified>
                                <Tag><xsl:value-of select="TAG"/></Tag>
                                <xsl:for-each select="PropertyList/ROW">
                                    <PropertyList>
                                        <xsl:for-each select="node()">
                                            <Property>
                                                <xsl:attribute name="name"><xsl:value-of select="name()"/></xsl:attribute>
                                                <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                                                <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                                                <xsl:value-of select="."/>
                                            </Property>
                                        </xsl:for-each>
                                    </PropertyList>
                                </xsl:for-each>
                                <RegNumber>
                                    <RegID>
                                        <xsl:if test="string-length(REGID)!=0">
                                            <xsl:value-of select="REGID"/>
                                        </xsl:if>
                                        <xsl:if test="string-length(REGID)=0">
                                            <xsl:value-of select="''0''"/>
                                        </xsl:if>
                                    </RegID>
                                    <SequenceNumber><xsl:value-of select="SEQUENCENUMBER"/></SequenceNumber>
                                    <RegNumber><xsl:value-of select="REGNUMBER"/></RegNumber>
                                    <SequenceID><xsl:value-of select="SEQUENCEID"/></SequenceID>
                                </RegNumber>
                                <BaseFragment>
                                    <Structure>
                                        <StructureID><xsl:value-of select="STRUCTUREID"/></StructureID>
                                        <StructureFormat><xsl:value-of select="STRUCTUREFORMAT"/></StructureFormat>
                                        <xsl:variable name="VROW" select="."/>
                                        <Structure>
                                            <xsl:for-each select="STRUCTURE">
                                                <xsl:for-each select="$VROW/FORMULAWEIGHT">
                                                    <xsl:attribute name="molWeight"><xsl:value-of select="."/></xsl:attribute>
                                                </xsl:for-each>
                                                <xsl:for-each select="$VROW/MOLECULARFORMULA">
                                                    <xsl:attribute name="formula"><xsl:value-of select="."/></xsl:attribute>
                                                </xsl:for-each>
                                                <xsl:value-of select="."/>
                                            </xsl:for-each>
                                        </Structure>
                                        <NormalizedStructure><xsl:value-of select="NORMALIZEDSTRUCTURE"/></NormalizedStructure>
                                        <UseNormalization><xsl:value-of select="USENORMALIZATION"/></UseNormalization>
                                    </Structure>
                                </BaseFragment>
                                <FragmentList>
                                    <xsl:for-each select="Fragment/ROW">
                                        <Fragment>
                                            <CompoundFragmentID><xsl:value-of select="ID"/></CompoundFragmentID>
                                            <FragmentID><xsl:value-of select="FRAGMENTID"/></FragmentID>
                                            <Code><xsl:value-of select="CODE"/></Code>
                                            <Name><xsl:value-of select="DESCRIPTION"/></Name>
                                            <FragmentTypeID>
                                                <xsl:attribute name="lookupTable"><xsl:value-of select="''FragmentType''"/></xsl:attribute>
                                                <xsl:attribute name="lookupField"><xsl:value-of select="''ID''"/></xsl:attribute>
                                                <xsl:attribute name="displayField"><xsl:value-of select="''Description''"/></xsl:attribute>
                                                <xsl:attribute name="displayValue"><xsl:value-of select="TYPEDESCRIPTION"/></xsl:attribute>
                                                <xsl:for-each select="FRAGMENTTYPEID">
                                                    <xsl:value-of select="."/>
                                                </xsl:for-each>
                                            </FragmentTypeID>
                                            <DateCreated><xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/></DateCreated>
                                            <DateLastModified><xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/></DateLastModified>
                                            <Equivalents><xsl:value-of select="EQUIVALENTS"/></Equivalents>
                                            <Structure>
                                                <StructureFormat><xsl:value-of select="STRUCTUREFORMAT"/></StructureFormat>
                                                <xsl:variable name="VROW1" select="."/>
                                                <Structure>
                                                    <xsl:for-each select="STRUCTURE">
                                                        <xsl:for-each select="$VROW1/MOLWEIGHT">
                                                            <xsl:attribute name="molWeight"><xsl:value-of select="."/></xsl:attribute>
                                                        </xsl:for-each>
                                                        <xsl:for-each select="$VROW1/FORMULA">
                                                            <xsl:attribute name="formula"><xsl:value-of select="."/></xsl:attribute>
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
                                            <ID><xsl:value-of select="ID"/></ID>
                                            <IdentifierID>
                                                <xsl:for-each select="TYPE">
                                                    <xsl:for-each select="$VROW3/DESCRIPTION">
                                                        <xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
                                                    </xsl:for-each>
                                                    <xsl:for-each select="$VROW3/NAME">
                                                        <xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
                                                    </xsl:for-each>
                                                    <xsl:for-each select="$VROW3/ACTIVE">
                                                        <xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
                                                    </xsl:for-each>
                                                    <xsl:value-of select="."/>
                                                </xsl:for-each>
                                            </IdentifierID>
                                            <InputText><xsl:value-of select="VALUE"/></InputText>
                                        </Identifier>
                                    </xsl:for-each>
                                </IdentifierList>
                            </Compound>
                        </Component>
                    </xsl:for-each>
                </ComponentList>
            </xsl:if>
            <!--Only Compound-->
            <xsl:if test="$VTypeRegistryRecord=''WithoutMixture''">
                <xsl:for-each select="Compound/ROW">
                    <Compound>
                        <CompoundID><xsl:value-of select="COMPOUNDID"/></CompoundID>
                        <DateCreated><xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/></DateCreated>
                        <PersonCreated><xsl:value-of select="PERSONCREATED"/></PersonCreated>
                        <PersonRegistered><xsl:value-of select="PERSONREGISTERED"/></PersonRegistered>
                        <Tag><xsl:value-of select="TAG"/></Tag>
                        <DateLastModified><xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/></DateLastModified>
                        <xsl:for-each select="PropertyList/ROW">
                            <PropertyList>
                                <xsl:for-each select="node()">
                                    <Property>
                                        <xsl:attribute name="name"><xsl:value-of select="name()"/></xsl:attribute>
                                        <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                                        <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                                        <xsl:value-of select="."/>
                                    </Property>
                                </xsl:for-each>
                            </PropertyList>
                        </xsl:for-each>
                        <RegNumber>
                            <RegID><xsl:value-of select="REGID"/></RegID>
                            <SequenceNumber><xsl:value-of select="SEQUENCENUMBER"/></SequenceNumber>
                            <RegNumber><xsl:value-of select="REGNUMBER"/></RegNumber>
                            <SequenceID><xsl:value-of select="SEQUENCEID"/></SequenceID>
                        </RegNumber>
                        <BaseFragment>
                            <Structure>
                                <StructureID><xsl:value-of select="STRUCTUREID"/></StructureID>
                                <StructureFormat><xsl:value-of select="STRUCTUREFORMAT"/></StructureFormat>
                                <xsl:variable name="VROW" select="."/>
                                <Structure>
                                    <xsl:for-each select="STRUCTURE">
                                        <xsl:for-each select="$VROW/FORMULAWEIGHT">
                                            <xsl:attribute name="molWeight"><xsl:value-of select="."/></xsl:attribute>
                                        </xsl:for-each>
                                        <xsl:for-each select="$VROW/MOLECULARFORMULA">
                                            <xsl:attribute name="formula"><xsl:value-of select="."/></xsl:attribute>
                                        </xsl:for-each>
                                        <xsl:value-of select="."/>
                                    </xsl:for-each>
                                </Structure>
                                <NormalizedStructure><xsl:value-of select="NORMALIZEDSTRUCTURE"/></NormalizedStructure>
                                <UseNormalization><xsl:value-of select="USENORMALIZATION"/></UseNormalization>
                            </Structure>
                        </BaseFragment>
                        <FragmentList>
                            <xsl:for-each select="Fragment/ROW">
                                <Fragment>
                                    <CompoundFragmentID><xsl:value-of select="ID"/></CompoundFragmentID>
                                    <FragmentID><xsl:value-of select="FRAGMENTID"/></FragmentID>
                                    <Code><xsl:value-of select="CODE"/></Code>
                                    <Name><xsl:value-of select="DESCRIPTION"/></Name>
                                    <FragmentTypeID>
                                        <xsl:attribute name="lookupTable"><xsl:value-of select="''FragmentType''"/></xsl:attribute>
                                        <xsl:attribute name="lookupField"><xsl:value-of select="''ID''"/></xsl:attribute>
                                        <xsl:attribute name="displayField"><xsl:value-of select="''Description''"/></xsl:attribute>
                                        <xsl:attribute name="displayValue"><xsl:value-of select="TYPEDESCRIPTION"/></xsl:attribute>
                                        <xsl:for-each select="FRAGMENTTYPEID">
                                            <xsl:value-of select="."/>
                                        </xsl:for-each>
                                    </FragmentTypeID>
                                    <DateCreated><xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/></DateCreated>
                                    <DateLastModified><xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/></DateLastModified>
                                    <Equivalents><xsl:value-of select="EQUIVALENTS"/></Equivalents>
                                    <Structure>
                                        <StructureFormat><xsl:value-of select="STRUCTUREFORMAT"/></StructureFormat>
                                        <xsl:variable name="VROW1" select="."/>
                                        <Structure>
                                            <xsl:for-each select="STRUCTURE">
                                                <xsl:for-each select="$VROW1/MOLWEIGHT">
                                                    <xsl:attribute name="molWeight"><xsl:value-of select="."/></xsl:attribute>
                                                </xsl:for-each>
                                                <xsl:for-each select="$VROW1/FORMULA">
                                                    <xsl:attribute name="formula"><xsl:value-of select="."/></xsl:attribute>
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
                                    <ID><xsl:value-of select="ID"/></ID>
                                    <IdentifierID>
                                        <xsl:for-each select="TYPE">
                                            <xsl:for-each select="$VROW3/DESCRIPTION">
                                                <xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:for-each select="$VROW3/NAME">
                                                <xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:for-each select="$VROW3/ACTIVE">
                                                <xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:value-of select="."/>
                                        </xsl:for-each>
                                    </IdentifierID>
                                    <InputText><xsl:value-of select="VALUE"/></InputText>
                                </Identifier>
                            </xsl:for-each>
                        </IdentifierList>
                    </Compound>
                </xsl:for-each>
            </xsl:if>
            <BatchList>
                <xsl:for-each select="$VMultiCompoundRegistryRecord/Batch/ROW">
                    <Batch>
                        <BatchID><xsl:value-of select="BATCHID"/></BatchID>
                        <BatchNumber><xsl:value-of select="BATCHNUMBER"/></BatchNumber>
                        <FullRegNumber><xsl:value-of select="FULLREGNUMBER"/></FullRegNumber>
                        <DateCreated><xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/></DateCreated>
                        <xsl:variable name="VBATCH" select="."/>
                        <PersonCreated>
                            <xsl:for-each select="PERSONCREATED">
                                <xsl:for-each select="$VBATCH/PERSONCREATEDDISPLAY">
                                    <xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
                                </xsl:for-each>
                                <xsl:value-of select="."/>
                            </xsl:for-each>
                        </PersonCreated>
                        <PersonRegistered>
                            <xsl:for-each select="PERSONREGISTERED">
                                <xsl:for-each select="$VBATCH/PERSONREGISTEREDDISPLAY">
                                    <xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
                                </xsl:for-each>
                                <xsl:value-of select="."/>
                            </xsl:for-each>
                        </PersonRegistered>
                        <DateLastModified><xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/></DateLastModified>
                        <StatusID><xsl:value-of select="STATUSID"/></StatusID>
                        <ProjectList>
                            <xsl:for-each select="Batch_Project/ROW">
                                <Project>
                                    <xsl:variable name="VBatchProject" select="."/>
                                    <ID><xsl:value-of select="ID"/></ID>
                                    <ProjectID>
                                        <xsl:for-each select="PROJECTID">
                                            <xsl:for-each select="$VBatchProject/DESCRIPTION">
                                                <xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:for-each select="$VBatchProject/NAME">
                                                <xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:for-each select="$VBatchProject/ACTIVE">
                                                <xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:value-of select="."/>
                                        </xsl:for-each>
                                    </ProjectID>
                                </Project>
                            </xsl:for-each>
                        </ProjectList>
                        <IdentifierList>
                            <xsl:for-each select="BatchIdentifier/ROW">
                                <xsl:variable name="VROW3" select="."/>
                                <Identifier>
                                    <ID><xsl:value-of select="ID"/></ID>
                                    <IdentifierID>
                                        <xsl:for-each select="TYPE">
                                            <xsl:for-each select="$VROW3/DESCRIPTION">
                                                <xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:for-each select="$VROW3/NAME">
                                                <xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:for-each select="$VROW3/ACTIVE">
                                                <xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
                                            </xsl:for-each>
                                            <xsl:value-of select="."/>
                                        </xsl:for-each>
                                    </IdentifierID>
                                    <InputText><xsl:value-of select="VALUE"/></InputText>
                                </Identifier>
                            </xsl:for-each>
                        </IdentifierList>
                        <xsl:for-each select="PropertyList/ROW">
                            <PropertyList>
                                <xsl:for-each select="node()">
                                    <xsl:variable name="aName" select="name()"/>
                                    <xsl:variable name="eValue" select="translate(., $VUPPER, $Vlower)"/>
                                    <Property>
                                        <xsl:attribute name="name"><xsl:value-of select="$aName"/></xsl:attribute>
                                        <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                                        <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                                        <xsl:choose>
                                            <xsl:when test="$aName = ''DELIVERYDATE'' and string-length(.) != 0">
                                                <xsl:value-of select="$eValue"/>
                                            </xsl:when>
                                            <xsl:when test="$aName = ''DATEENTERED'' and string-length(.) != 0">
                                                <xsl:value-of select="$eValue"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:value-of select="."/>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    </Property>
                                </xsl:for-each>
                            </PropertyList>
                        </xsl:for-each>
                        <BatchComponentList>
                            <xsl:for-each select="BatchComponent/ROW">
                                <BatchComponent>
                                    <ID><xsl:value-of select="ID"/></ID>
                                    <BatchID><xsl:value-of select="BATCHID"/></BatchID>
                                    <CompoundID><xsl:value-of select="COMPOUNDID"/></CompoundID>
                                    <MixtureComponentID><xsl:value-of select="MIXTURECOMPONENTID"/></MixtureComponentID>
                                    <ComponentIndex><xsl:value-of select="COMPONENTINDEX"/></ComponentIndex>
                                    <xsl:for-each select="PropertyList/ROW">
                                        <PropertyList>
                                            <xsl:for-each select="node()">
                                                <Property>
                                                    <xsl:attribute name="name"><xsl:value-of select="name()"/></xsl:attribute>
                                                    <xsl:attribute name="pickListDomainID"><xsl:value-of select="@pickListDomainID"/></xsl:attribute>
                                                    <xsl:attribute name="pickListDisplayValue"><xsl:value-of select="@pickListDisplayValue"/></xsl:attribute>
                                                    <xsl:value-of select="."/>
                                                </Property>
                                            </xsl:for-each>
                                        </PropertyList>
                                    </xsl:for-each>
                                    <BatchComponentFragmentList>
                                        <xsl:for-each select="BatchComponentFragment/ROW">
                                            <BatchComponentFragment>
                                                <ID><xsl:value-of select="ID"/></ID>
                                                <FragmentID><xsl:value-of select="FRAGMENTID"/></FragmentID>
                                                <Equivalents><xsl:value-of select="EQUIVALENT"/></Equivalents>
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
</xsl:stylesheet>'
);

    /** XSL transform used to create a 'temporary' registry record */
    cXslCreateMCRRTemp Constant XmlType := XmlType.CreateXml(
'<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
    <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
    <xsl:template match="/MultiCompoundRegistryRecord">
        <xsl:variable name="VProjectList" select="ProjectList"/>
        <xsl:variable name="VProjectBatchList" select="BatchList/Batch/ProjectList"/>
        <xsl:variable name="VIdentifierList" select="IdentifierList"/>
        <xsl:variable name="VIdentifierBatchList" select="BatchList/Batch/IdentifierList"/>
        <xsl:variable name="VPropertyList" select="PropertyList"/>
        <xsl:for-each select="BatchList/Batch">
            <xsl:variable name="VBatchList_Batch" select="."/>
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
                            <xsl:when test="$eName = ''DELIVERYDATE'' and string-length($eValue) != 0">
                                <xsl:element name="{$eName}">
                                    <xsl:value-of select="$eValue"/>
                                </xsl:element>
                            </xsl:when>
                            <xsl:when test="$eName = ''DATEENTERED'' and string-length($eValue) != 0">
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
                                    <xsl:when test=".=''0''">
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
                                    <xsl:when test="$eName = ''COMMENTS''">
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
</xsl:stylesheet>'
);

    /** XSL transform used to create a 'permanent' registry record */
    cXslCreateMCRR Constant XmlType := XmlType.CreateXml(
'<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
    <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
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
            <xsl:if test="ProjectID!=''''">
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
                    <TYPE><xsl:value-of select="IdentifierID"/></TYPE>
                    <VALUE><xsl:value-of select="InputText"/></VALUE>
                    <REGID>0</REGID>
                    <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
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
                            <xsl:when test="FullRegNumber!=''''">
                                <xsl:value-of select="FullRegNumber"/>
                            </xsl:when>
                            <xsl:otherwise>null</xsl:otherwise>
                        </xsl:choose>
                    </FULLREGNUMBER>
                    <DATECREATED><xsl:value-of select="DateCreated"/></DATECREATED>
                    <PERSONCREATED><xsl:value-of select="PersonCreated"/></PERSONCREATED>
                    <PERSONREGISTERED><xsl:value-of select="PersonRegistered"/></PERSONREGISTERED>
                    <DATELASTMODIFIED><xsl:value-of select="DateLastModified"/></DATELASTMODIFIED>
                    <STATUSID><xsl:value-of select="StatusID"/></STATUSID>
                    <REGID>0</REGID>
                    <TEMPBATCHID><xsl:value-of select="BatchID"/></TEMPBATCHID>
                    <xsl:for-each select="PropertyList/Property">
                        <xsl:variable name="eValue" select="."/>
                        <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                        <!-- conditionally create the element -->
                        <xsl:choose>
                            <xsl:when test="$eName = ''DELIVERYDATE'' and string-length($eValue) != 0">
                                <xsl:element name="{$eName}">
                                    <xsl:value-of select="$eValue"/>
                                </xsl:element>
                            </xsl:when>
                            <xsl:when test="$eName = ''DATEENTERED'' and string-length($eValue) != 0">
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
                            <PROJECTID><xsl:value-of select="."/></PROJECTID>
                            <BATCHID>0</BATCHID>
                        </xsl:for-each>
                    </ROW>
                </VW_Batch_Project>
            </xsl:for-each>
            <xsl:for-each select="IdentifierList/Identifier">
                <VW_BatchIdentifier>
                    <ROW>
                        <ID>0</ID>
                        <TYPE><xsl:value-of select="IdentifierID"/></TYPE>
                        <VALUE><xsl:value-of select="InputText"/></VALUE>
                        <BATCHID>0</BATCHID>
                        <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
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
                            <REGID><xsl:value-of select="RegID"/></REGID>
                            <!-- this is an odd construct: " " -->
                            <SEQUENCENUMBER><xsl:value-of select="SequenceNumber"/></SEQUENCENUMBER>
                            <REGNUMBER>" "</REGNUMBER>
                            <SEQUENCEID><xsl:value-of select="SequenceID"/></SEQUENCEID>
                            <DATECREATED><xsl:value-of select="$VCompound/DateCreated"/></DATECREATED>
                            <PERSONREGISTERED><xsl:value-of select="$VCompound/PersonRegistered"/></PERSONREGISTERED>
                        </ROW>
                    </VW_RegistryNumber>
                </xsl:for-each>
                <xsl:choose>
                    <xsl:when test="RegNumber/RegID=''0''">
                        <xsl:for-each select="BaseFragment/Structure">
                            <VW_Structure>
                                <ROW>
                                    <STRUCTUREID><xsl:value-of select="StructureID"/></STRUCTUREID>
                                    <STRUCTUREFORMAT><xsl:value-of select="StructureFormat"/></STRUCTUREFORMAT>
                                </ROW>
                            </VW_Structure>
                        </xsl:for-each>
                        <VW_Compound>
                            <ROW>
                                <COMPOUNDID>0</COMPOUNDID>
                                <DATECREATED><xsl:value-of select="DateCreated"/></DATECREATED>
                                <PERSONCREATED><xsl:value-of select="PersonCreated"/></PERSONCREATED>
                                <PERSONREGISTERED><xsl:value-of select="PersonRegistered"/></PERSONREGISTERED>
                                <DATELASTMODIFIED><xsl:value-of select="DateLastModified"/></DATELASTMODIFIED>
                                <TAG><xsl:value-of select="Tag"/></TAG>
                                <REGID>0</REGID>
                                <STRUCTUREID>0</STRUCTUREID>
                                <xsl:for-each select="BaseFragment/Structure">
                                    <USENORMALIZATION><xsl:value-of select="UseNormalization"/></USENORMALIZATION>
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
                                    <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
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
                                        <PROJECTID><xsl:value-of select="."/></PROJECTID>
                                        <REGID>0</REGID>
                                    </xsl:for-each>
                                </ROW>
                            </VW_RegistryNumber_Project>
                        </xsl:for-each>
                        <xsl:for-each select="FragmentList/Fragment">
                            <xsl:variable name="VFragmentID" select="FragmentID"/>
                            <VW_Fragment>
                                <ROW>
                                    <FRAGMENTID><xsl:value-of select="FragmentID"/></FRAGMENTID>
                                </ROW>
                            </VW_Fragment>
                            <VW_Compound_Fragment>
                                <ROW>
                                    <ID>0</ID>
                                    <COMPOUNDID>0</COMPOUNDID>
                                    <FRAGMENTID>0</FRAGMENTID>
                                    <EQUIVALENTS><xsl:value-of select="Equivalents"/></EQUIVALENTS>
                                </ROW>
                            </VW_Compound_Fragment>
                            <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[$VComponentIndex=ComponentIndex]">
                                <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[FragmentID=$VFragmentID]">
                                    <VW_BatchComponentFragment>
                                        <ROW>
                                            <BATCHCOMPONENTID>0</BATCHCOMPONENTID>
                                            <COMPOUNDFRAGMENTID>0</COMPOUNDFRAGMENTID>
                                            <EQUIVALENT><xsl:value-of select="Equivalents"/></EQUIVALENT>
                                            <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
                                        </ROW>
                                    </VW_BatchComponentFragment>
                                </xsl:for-each>
                            </xsl:for-each>
                        </xsl:for-each>
                        <xsl:for-each select="IdentifierList/Identifier">
                            <VW_Compound_Identifier>
                                <ROW>
                                    <ID>0</ID>
                                    <TYPE><xsl:value-of select="IdentifierID"/></TYPE>
                                    <VALUE><xsl:value-of select="InputText"/></VALUE>
                                    <REGID>0</REGID>
                                    <ID>0</ID>
                                    <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
                                </ROW>
                            </VW_Compound_Identifier>
                        </xsl:for-each>
                    </xsl:when>
                    <xsl:when test="RegNumber/RegID!=''0''">
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
                                    <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
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
                                    <FRAGMENTID><xsl:value-of select="FragmentID"/></FRAGMENTID>
                                </ROW>
                            </VW_Fragment>
                            <VW_Compound_Fragment>
                                <ROW>
                                    <ID>0</ID>
                                    <COMPOUNDID>0</COMPOUNDID>
                                    <FRAGMENTID>0</FRAGMENTID>
                                    <EQUIVALENTS><xsl:value-of select="Equivalents"/></EQUIVALENTS>
                                </ROW>
                            </VW_Compound_Fragment>
                            <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                                <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[FragmentID=$VFragmentID]">
                                    <VW_BatchComponentFragment>
                                        <ROW>
                                            <BATCHCOMPONENTID>0</BATCHCOMPONENTID>
                                            <COMPOUNDFRAGMENTID>0</COMPOUNDFRAGMENTID>
                                            <EQUIVALENT><xsl:value-of select="Equivalents"/></EQUIVALENT>
                                            <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
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
</xsl:stylesheet>'
);

/**
Utility for providing a value for a Registration configuration setting
-->author jed
-->since December 2010
-->return an integer value
*/
FUNCTION GetBatchNumberPadding RETURN INTEGER
IS
BEGIN
    IF (VBatchNumberPad is NULL) THEN
        VBatchNumberPad := to_number(
          COEDB.ConfigurationManager.RetrieveParameter(
            'Registration'
            ,'Registration/applicationSettings/groups/add[@name="MISC"]/settings/add[@name="BatchNumberLength"]/@value'
          )
        );
    END IF;
    RETURN VBatchNumberPad;
END;

/**
Utility to help build an error list that retains the innermost error stack.
-->param AErrorMessage the text to prepend to the exception backtrace
-->param AErrorStack   the text (backtrace) to append to the current exception message, if any exists
*/
FUNCTION AppendError(AErrorMessage CLOB DEFAULT NULL, AErrorStack CLOB DEFAULT NULL)
RETURN CLOB AS
    LNewErr CLOB := empty_clob();
BEGIN

    --tack on message to exception stacktrace
    IF AErrorMessage IS NOT NULL THEN
        LNewErr := AErrorMessage || chr(10) || AErrorStack;
    ELSE
        LNewErr := AErrorStack;
    END IF;

    --tack this derived onto the current exception error messge
    IF SQLCODE <> 0 THEN
        LNewErr := SQLERRM || chr(10) || LNewErr;
    END IF;

    RETURN LNewErr;
END;

/**
Utility for generating a universal xml wrapper for actions taken on one or more registry records.
-->author jed
-->since December 2009
-->param AMessage   the text to be used as the 'message' attribute of the Response element
-->param AError     describes why an action was not carried out
-->param AResult    an xml representation of one or more objects resulting from an action
-->return           a CLOB in the format '<Response message=""><Error></Error><Result></Result></Response>'
*/
FUNCTION CreateRegistrationResponse(
    AMessage VARCHAR2
    , AError CLOB
    , AResult CLOB
) RETURN CLOB
AS
    LResponse CLOB := empty_clob();
    LCleanError CLOB := CASE WHEN AError IS NULL THEN NULL ELSE REPLACE(AError, cXmlDecoration, '') END;
    LCleanResult CLOB := CASE WHEN AResult IS NULL THEN NULL ELSE REPLACE(AResult, cXmlDecoration, '') END;
BEGIN
      LResponse :=
      '<Response message="' || AMessage ||  '">'
        || '<Error>' || LCleanError || '</Error>'
        || '<Result>' || LCleanResult || '</Result>'
      || '</Response>';
    $if CompoundRegistry.Debuging $then InsertLog('CreateRegistrationResponse', LResponse); $end NULL;
    RETURN LResponse;
END;

/**
Dissembles the message, error and result properties from create/update output.
Create and Update events return an xml (CLOB) value from "CreateRegistrationResponse" to provide
the caller with details. This will parse the result for internal callers that require details.
-->author jed
-->since December 2009
-->param AResponseXml 'Response' input as an xml string
-->param AMessage     extracted message output
-->param AError       extracted error output
-->param AResult      extracted object(s) output
*/
PROCEDURE ExtractRegistrationResponse(
    AResponseXml IN CLOB
    , AMessage OUT CLOB
    , AError OUT CLOB
    , AResult OUT CLOB
) IS
    LResponse CLOB := AResponseXml;
BEGIN
    --use string extraction mechanisms to get these values
    AError := TakeOffAndGetClob(LResponse, 'Error');
    AResult := TakeOffAndGetClob(LResponse, 'Result');

    --using xmltype here is 64k-safe because LResponse is so small now
    SELECT
      EXTRACT(XMLTYPE(LResponse), '/Response/@message').GetClobVal()
    INTO AMessage FROM dual;
END;

PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
    INSERT INTO LOG(LogProcedure,LogComment) VALUES($$plsql_unit||'.'||ALogProcedure,ALogComment);
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN NULL; --If logs don't work then don't stop
END;

PROCEDURE SetSessionParameter IS
    PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
    DBMS_SESSION.set_nls('NLS_DATE_FORMAT','''YYYY-MM-DD HH:Mi:SS''');
    DBMS_SESSION.set_nls('NLS_NUMERIC_CHARACTERS', '''.,''');
    COMMIT; --It is necesary to finished the Autonomous-Transaction
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eSetSessionParameter, AppendError('SetSessionParameter', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

/**
Procedure to be fired after a registration has been performed but before the
resulting XML is returned to the caller.
-->author jed
-->since December 2010
-->param ARegID Registration ID (private key) assigned to the registration
*/
PROCEDURE OnRegistrationInsert(ARegID NUMBER) IS
  LProc varchar2(1000) := upper('AfterRegInsert');
  LProcex varchar2(1100) := 'begin ' || LProc || '(:regid); end;';
  LExists number := 0;
BEGIN
    --ensure the event-handling procedure exists before attempting to invoke it
    SELECT COUNT(*) INTO LExists
    FROM user_objects obj
    WHERE obj.object_type IN ('PROCEDURE')
      AND obj.object_name = LProc;
    BEGIN
        IF (LExists = 1) THEN
            --Uncomment the line below for tracing
            -- InsertLog('OnRegistrationInsert', LProcex || ' using ' || to_char(ARegID));
            EXECUTE IMMEDIATE LProcex USING IN ARegID;
        END IF;
    END;
END;

/**
Procedure to be fired after a registration has been performed but before the
resulting XML is returned to the caller. Discovers the Reg ID and then calls
the other overload of the same function.
-->author jed
-->since December 2010
-->param ARegNum 'Registry Number' (public key) assigned to the registration
*/
PROCEDURE OnRegistrationInsert(ARegNum VARCHAR2) IS
  LRegID number;
BEGIN
    SELECT rn.reg_id INTO LRegId
    FROM regdb.reg_numbers rn
    WHERE rn.reg_number = ARegNum;

    OnRegistrationInsert(LRegId);
END;

/**
Procedure to be fired after a registration has been performed but before the
resulting XML is returned to the caller.
-->author jed
-->since December 2010
-->param ARegID Registration ID (private key) assigned to the registration
*/
PROCEDURE OnRegistrationUpdate(ARegID NUMBER) IS
  LProc varchar2(1000) := upper('AfterRegUpdate');
  LProcex varchar2(1100) := 'begin ' || LProc || '(:regid); end;';
  LExists number := 0;
BEGIN
    --ensure the event-handling procedure exists before attempting to invoke it
    SELECT COUNT(*) INTO LExists
    FROM all_objects obj
    WHERE obj.object_type IN ('PROCEDURE')
      AND obj.object_name = LProc;
    BEGIN
        IF (LExists = 1) THEN
            --Uncomment the line below for tracing
            -- InsertLog('OnRegistrationUpdate', LProcex || ' using ' || to_char(ARegID));
            EXECUTE IMMEDIATE LProcex USING IN ARegID;
        END IF;
    END;
END;

/**
Procedure to be fired after a registration has been updated but before the
resulting XML is returned to the caller. Discovers the Reg ID and then calls
the other overload of the same function.
-->author jed
-->since December 2010
-->param ARegNum 'Registry Number' (public key) assigned to the registration
*/
PROCEDURE OnRegistrationUpdate(ARegNum VARCHAR2) IS
  LRegID number;
BEGIN
    SELECT rn.reg_id INTO LRegId
    FROM regdb.reg_numbers rn
    WHERE rn.reg_number = ARegNum;

    OnRegistrationUpdate(LRegId);
END;

/**
Go over the Field's tag of the properties and add the attributes of picklist
-->author Fari
-->since December 2010
-->param AFields   list of field name with picklist
-->param AXml      xml with the property and property's value
-->param ABeginXml tag parent to begin the search

*/
PROCEDURE AddAttribPickList(AFields IN CLOB,AXml IN OUT NOCOPY CLOB,ABeginXml IN CLOB) IS
        LPosBegin                 NUMBER;
        LPosDot                   NUMBER;
        LPoslast                  NUMBER;
        LField                    VARCHAR2(100);
        LFieldTag                 VARCHAR2(100);
        LFieldTagEnd              VARCHAR2(100);
        LpickListDomainID               VARCHAR2(100);
        LFields                   CLOB;

        LPosField                 NUMBER;
        LPosFieldEnd              NUMBER;

        LExt_Table                VW_PickListDomain.Ext_Table%Type;
        LExt_ID_Col               VW_PickListDomain.Ext_ID_Col%Type;
        LExt_Display_Col          VW_PickListDomain.Ext_Display_Col%Type;
        LpickListDisplayValue          CLOB;
        LFieldValue               VARCHAR2(100);
        LSelect                   VARCHAR2(4000);

    BEGIN
        LFields:=AFields||',';
        LPosBegin:=0;
        LPoslast:=1;
        LOOP
            LPosBegin:=INSTR(LFields,',',LPoslast);
            LPosDot:=INSTR(LFields,':',LPoslast);
            LField:=UPPER(SUBSTR(LFields,LPoslast,LPosDot-LPoslast));
            LpickListDomainID:=SUBSTR(LFields,LPosDot+1,LPosBegin-LPosDot-1);
            LPoslast:=LPosBegin+1;
        EXIT WHEN LField IS NULL;
            $if CompoundRegistry.Debuging $then InsertLog('AddAttribPickList',' LField:'||LField||' AXml='||AXml); $end null;
            LFieldTag:='<'||LField||'>';
            LFieldTagEnd:='</'||LField||'>';
            LPosField:=INSTR(AXml,LFieldTag ,INSTR(AXml,ABeginXml)+1);
            $if CompoundRegistry.Debuging $then InsertLog('AddAttribPickList',' LPosField:'||LPosField); $end null;
            IF LPosField<>0 THEN
                $if CompoundRegistry.Debuging $then InsertLog('AddAttribPickList',' LpickListDomainID:'||LpickListDomainID); $end null;
                IF NVL(LpickListDomainID,0)<>0 THEN
                    SELECT    Ext_Table,Ext_ID_Col,Ext_Display_Col
                        INTO  LExt_Table,LExt_ID_Col,LExt_Display_Col
                        FROM  VW_PickListDomain PLM
                        WHERE PLM.ID=LpickListDomainID;

                     LPosFieldEnd:=INSTR(AXml,LFieldTagEnd,INSTR(AXml,ABeginXml)+1);

                     LFieldValue:=SUBSTR(AXml,LPosField+Length(LFieldTag),LPosFieldEnd-(LPosField+Length(LFieldTag))) ;
                     $if CompoundRegistry.Debuging $then InsertLog('AddAttribPickList','LFieldValue:'||LFieldValue); $end null;
                      IF LFieldValue IS NOT NULL AND LExt_Display_Col IS NOT NULL AND LExt_Table IS NOT NULL AND LExt_ID_Col IS NOT NULL THEN
                         LSelect:='SELECT ' || LExt_Display_Col||' FROM '||LExt_Table||' WHERE '||LExt_ID_Col||' = ' || LFieldValue;
                         $if CompoundRegistry.Debuging $then InsertLog('AddAttribPickList','LSelect:'||LSelect); $end null;
                         BEGIN
                            EXECUTE IMMEDIATE LSelect INTO LpickListDisplayValue;
                         EXCEPTION
                            WHEN OTHERS THEN
                            BEGIN
                                $if CompoundRegistry.Debuging $then InsertLog('AddAttribPickList','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE ); $end null;
                                NULL;
                            END;
                         END;

                         IF LpickListDisplayValue IS NOT NULL THEN
                            AXml:=REPLACE(AXml,LFieldTag,'<'||LField||' pickListDomainID="'||LpickListDomainID||'" pickListDisplayValue="'||LpickListDisplayValue||'">');
                         END IF;
                         $if CompoundRegistry.Debuging $then InsertLog('AddAttribPickList',' LField:'||LField||' AXml='||AXml); $end null;
                    END IF;
                END IF;
            END IF;
        END LOOP;
    END;

/**
Get the value of "SameBatchesIdentity" from the "Application Settings" of Registration.
-->author Fari
-->since March 2010
*/
FUNCTION GetSameBatchesIdentity RETURN CLOB IS
BEGIN
    RETURN COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="SameBatchesIdentity"]/@value');
END;

/**
Get the value of "CheckDuplication" from the "Application Settings" of Registration.
"CheckDuplication" setting enables validation of duplication of structures and mixtures with identical compound before of the registration." allowedValues="True|False" isAdmin="False"/>                    <add name="FragmentsUsed
-->author Fari
-->since July 2010
*/
FUNCTION GetDuplicateCheckEnable RETURN CLOB IS
BEGIN
    RETURN COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="CheckDuplication"]/@value');
END;

/**
Get the value of "ActiveRLS" from the "Application Settings" of Registration.
-->author Fari
-->since September 2010
*/
FUNCTION GetActiveRLS RETURN CLOB IS
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('GetActiveRLS','ActiveRLS->'||COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ActiveRLS"]/@value')); $end null;
    RETURN COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ActiveRLS"]/@value');
END;

/**
Define and get the status "Editable". of un registry
-->author Fari
-->since November 2010
*/

FUNCTION GetIsEditable(ARegNumber in VW_RegistryNumber.RegNumber%type) RETURN VARCHAR2 IS
    LIsEditable VARCHAR2(5):='False';
    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    LUserProjectID INTEGER;

    LProjectID VW_Project.ProjectID%type;

    TYPE T_RegProjectAssigned IS REF CURSOR;
    C_RegProjectAssigned   T_RegProjectAssigned;

    CURSOR C_UserProjects IS
        SELECT ProjectID
            FROM  VW_Project
            ORDER BY ProjectID;

BEGIN

    LLevelRLS:=RegistrationRLS.GetLevelRLS;

    IF LLevelRLS!='OFF' THEN
        $if CompoundRegistry.Debuging $then InsertLog('GetIsEditable','LLevelRLS->'||LLevelRLS); $end null;

        LEnableRLS:=RegistrationRLS.GEnableRLS;

        IF LEnableRLS THEN
            RegistrationRLS.GEnableRLS:=False;
        END IF;

        IF LLevelRLS='REGISTRY LEVEL PROJECTS' THEN
            OPEN C_RegProjectAssigned FOR
                SELECT Distinct RNP_StructureUsed.ProjectID
                    FROM  VW_RegistryNumber RN, VW_RegistryNumber_Project RNP,VW_Mixture_RegNumber MR,VW_Mixture_Component MC,VW_Compound C,
                          VW_Mixture_Component MC_CompoundUsed,VW_Mixture_RegNumber MR_CompoundUsed,VW_RegistryNumber_Project RNP_CompoundUsed,
                          VW_Compound C_StructureUsed,VW_Mixture_Component MC_StructureUsed,VW_Mixture_RegNumber MR_StructureUsed, VW_RegistryNumber_Project RNP_StructureUsed
                    WHERE RN.RegID = RNP.RegID AND RN.RegNumber=ARegNumber AND C.CompoundID=MC.CompoundID AND
                          MR.RegID=RN.RegID AND MC.MixtureID=MR.MixtureID AND MC_CompoundUsed.CompoundID=MC.CompoundID AND MR_CompoundUsed.MixtureID=MC_CompoundUsed.MixtureID AND MR_CompoundUsed.RegID = RNP_CompoundUsed.RegID AND
                          C_StructureUsed.StructureID=C.StructureID AND MC_StructureUsed.CompoundID=C_StructureUsed.CompoundID AND MR_StructureUsed.MixtureID = MC_StructureUsed.MixtureID AND MR_StructureUsed.RegID = RNP_StructureUsed.RegID
                    ORDER BY RNP_StructureUsed.ProjectID;
        ELSE
            OPEN C_RegProjectAssigned FOR
                SELECT Distinct BP_StructureUsed.ProjectID
                     FROM VW_RegistryNumber RN,VW_Batch B,VW_Batch_Project BP,VW_Mixture_RegNumber MR,VW_Mixture_Component MC,VW_Compound C,
                          VW_Mixture_Component MC_CompoundUsed,VW_Mixture_RegNumber MR_CompoundUsed,VW_Batch B_CompoundUsed,VW_Batch_Project BP_CompoundUsed,
                          VW_Compound C_StructureUsed,VW_Mixture_Component MC_StructureUsed,VW_Mixture_RegNumber MR_StructureUsed,VW_Batch B_StructureUsed,VW_Batch_Project BP_StructureUsed
                    WHERE RN.RegID = B.RegID AND BP.BatchID=B.BatchID AND RN.RegNumber=ARegNumber AND C.CompoundID=MC.CompoundID AND
                          MR.RegID=RN.RegID AND MC.MixtureID=MR.MixtureID AND MC_CompoundUsed.CompoundID=MC.CompoundID AND MR_CompoundUsed.MixtureID=MC_CompoundUsed.MixtureID AND MR_CompoundUsed.RegID = B_CompoundUsed.RegID AND BP_CompoundUsed.BatchID=B_CompoundUsed.BatchID AND
                          C_StructureUsed.StructureID=C.StructureID AND MC_StructureUsed.CompoundID=C_StructureUsed.CompoundID AND MR_StructureUsed.MixtureID = MC_StructureUsed.MixtureID AND MR_StructureUsed.RegID = B_StructureUsed.RegID AND BP_StructureUsed.BatchID=B_StructureUsed.BatchID
                    ORDER BY BP_StructureUsed.ProjectID;

        END IF;

        RegistrationRLS.GEnableRLS:=True;

        OPEN C_UserProjects;

        LOOP

            FETCH C_RegProjectAssigned INTO LProjectID;
            $if CompoundRegistry.Debuging $then InsertLog('GetIsEditable','LProjectID->'||LProjectID); $end null;

            IF C_RegProjectAssigned%NOTFOUND THEN
                $if CompoundRegistry.Debuging $then InsertLog('GetIsEditable','C_RegProjectAssigned%NOTFOUND'); $end null;
                LIsEditable:='True';
                EXIT;
            END IF;

            LOOP

                FETCH C_UserProjects INTO LUserProjectID;

                IF C_UserProjects%NOTFOUND THEN
                   $if CompoundRegistry.Debuging $then InsertLog('GetIsEditable','C_UserProjects%NOTFOUND '); $end null;
                    EXIT;
                END IF;


                $if CompoundRegistry.Debuging $then InsertLog('GetIsEditable','LUserProjectID:'||LUserProjectID); $end null;

                EXIT WHEN LUserProjectID=LProjectID;


            END LOOP;

            EXIT WHEN  C_UserProjects%NOTFOUND;

        END LOOP;


        CLOSE C_UserProjects;

        CLOSE C_RegProjectAssigned;

        RegistrationRLS.GEnableRLS:=LEnableRLS;
    ELSE
        LIsEditable:='True';
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog('GetIsEditable','LIsEditable->'||LIsEditable); $end null;
    RETURN LIsEditable;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LIsEditable:='False';

        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;

        RAISE_APPLICATION_ERROR(eGenericException, AppendError('GetIsEditable', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;


/**
Define and get the status "Editable". of un registry in the temporary context
-->author Fari
-->since November 2010
*/

FUNCTION GetIsEditableTmp(ATempID  in Number) RETURN VARCHAR2 IS
    LIsEditable VARCHAR2(5):='False';
    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    LUserProjectID INTEGER;

    LProjectID VW_Project.ProjectID%type;

    TYPE T_RegProjectAssigned IS REF CURSOR;
    C_RegProjectAssigned   T_RegProjectAssigned;

    CURSOR C_UserProjects IS
        SELECT ProjectID
            FROM  VW_Project
            ORDER BY ProjectID;

BEGIN

    LLevelRLS:=RegistrationRLS.GetLevelRLS;

    IF LLevelRLS!='OFF' THEN
        $if CompoundRegistry.Debuging $then InsertLog('GetIsEditable','LLevelRLS->'||LLevelRLS); $end null;

        LEnableRLS:=RegistrationRLS.GEnableRLS;

        IF LEnableRLS THEN
            RegistrationRLS.GEnableRLS:=False;
        END IF;

        IF LLevelRLS='REGISTRY LEVEL PROJECTS' THEN
            OPEN C_RegProjectAssigned FOR
                SELECT Distinct TRP.ProjectID
                    FROM  VW_TemporaryRegnumbersProject TRP
                    WHERE TRP.TempBatchID=ATempID
                    ORDER BY TRP.ProjectID;

        ELSE
            OPEN C_RegProjectAssigned FOR
                SELECT Distinct TBP.ProjectID
                    FROM  VW_TemporaryBatchProject TBP
                    WHERE TBP.TempBatchID=ATempID
                    ORDER BY TBP.ProjectID;
        END IF;

        RegistrationRLS.GEnableRLS:=True;

        OPEN C_UserProjects;

        LOOP

            FETCH C_RegProjectAssigned INTO LProjectID;
            $if CompoundRegistry.Debuging $then InsertLog('GetIsEditableTmp','LProjectID->'||LProjectID); $end null;

            IF C_RegProjectAssigned%NOTFOUND THEN
                $if CompoundRegistry.Debuging $then InsertLog('GetIsEditableTmp','C_RegProjectAssigned%NOTFOUND'); $end null;
                LIsEditable:='True';
                EXIT;
            END IF;

            LOOP

                FETCH C_UserProjects INTO LUserProjectID;

                IF C_UserProjects%NOTFOUND THEN
                   $if CompoundRegistry.Debuging $then InsertLog('GetIsEditableTmp','C_UserProjects%NOTFOUND '); $end null;
                    EXIT;
                END IF;


                $if CompoundRegistry.Debuging $then InsertLog('GetIsEditableTmp','LUserProjectID:'||LUserProjectID); $end null;

                EXIT WHEN LUserProjectID=LProjectID;


            END LOOP;

            EXIT WHEN  C_UserProjects%NOTFOUND;

        END LOOP;


        CLOSE C_UserProjects;

        CLOSE C_RegProjectAssigned;

        RegistrationRLS.GEnableRLS:=LEnableRLS;
    ELSE
        LIsEditable:='True';
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog('GetIsEditableTmp','LIsEditable->'||LIsEditable); $end null;
    RETURN LIsEditable;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LIsEditable:='False';

        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;

        RAISE_APPLICATION_ERROR(eGenericException, AppendError('GetIsEditableTmp', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;
/**
Save a log with the Compound Registry Number of the Structures Duplciated by a new strcuture or by a structure updated.
-->author Fari
-->since June 2010
-->param AXMLRegNumberDuplicated Each Registry Number Duplciated
-->param APersonID               Person that added or upadated the compound strcuture
The next is a detail of the table used:

Tale Name: REGDB.DUPLICATED
Description: When a structure of a compound is saved or updated a new row is added in this table for each structure similar existing in the DB .

Column Name          Description
ID                   Row Single identify
REGNUMBER            Registry Number of the Compound added or updated. It is not the Registry Number of the Registry (mixture)
REGNUMBERDUPLICATED  It is Registry Number of the Compound existing in the DB whose structure is similary.
                     It is not the Registry Number of the Registry (mixture)
PERSONID             Person who registered the Registry
CREATED              Duplication Date
*/
PROCEDURE SaveRegNumbersDuplicated(AXMLRegNumberDuplicated XmlType,APersonID VARCHAR2)IS
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('SaveRegNumbersDuplicated','AXMLRegNumberDuplicated->'||AXMLRegNumberDuplicated.GetStringVal()); $end null;

    INSERT INTO VW_Duplicates(RegNumber ,RegNumberDuplicated,PersonID,Created)
        (SELECT extract(value(RegNumberDuplicated), '//REGNUMBER/@NEW').GetStringVal(),extract(value(RegNumberDuplicated), '//REGNUMBER/text()').GetStringVal(),APersonID,SYSDATE
         FROM Table(XMLSequence(Extract(AXMLRegNumberDuplicated, '/ROWSET/REGNUMBER'))) RegNumberDuplicated);
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('SaveRegNumbersDuplicated', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

PROCEDURE AddTags(AXmlSource IN XmlType,AXmlTarget IN OUT NOCOPY XmlType,APathSource IN Varchar2,AAttributeSource IN Varchar2) IS

    LIndex               Number;
    LNodesCount          Number;
    LIndexTarget         Number;
    LTagTarget           Varchar2(4000);
    LNodeName            Varchar2(300);
    LNodePath            Varchar2(4000);

    LDOMDocumentSource   DBMS_XMLDom.DOMDocument;
    LElementSource       DBMS_XMLDom.DOMElement;
    LSourceNode          DBMS_XMLDom.DOMNode;
    LSourceParentNode    DBMS_XMLDom.DOMNode;
    LSourceParentNodeAux DBMS_XMLDom.DOMNode;

    LNodeListSource      DBMS_XMLDom.DOMNodelist;

    LNodeListTarget      DBMS_XMLDom.DOMNodelist;

    LDOMDocumentTarget   DBMS_XMLDom.DOMDocument;
    LNodeTarget          DBMS_XMLDom.DOMNode;

    LAttrs               DBMS_XMLDom.DOMNamedNodeMap;
    LAttr                DBMS_XMLDom.DOMNode;
    LIndexAttr           Number;
    LXPath               Varchar2(1000);
    LAttrName            Varchar2(300);

BEGIN
    LDOMDocumentTarget := DBMS_XMLDom.NewDOMDocument(AXmlTarget);
    LDOMDocumentSource := DBMS_XMLDom.NewDOMDocument(AXmlSource);

    LNodeListSource:=DBMS_XMLDom.GetElementsByTagName(LDOMDocumentSource, APathSource);
    LNodesCount:=DBMS_XMLDom.GetLength(LNodeListSource);
    FOR LIndex IN 0..LNodesCount-1 LOOP
        LNodeTarget       := DBMS_XMLDom.MakeNode(LDOMDocumentTarget);
        LSourceNode       := DBMS_XMLDom.Item(LNodeListSource, LIndex);
        LSourceParentNode := DBMS_XMLDom.GetParentNode(LSourceNode);
        LNodePath:='';
        LOOP
            LNodeName:=DBMS_XMLDom.GetNodeName(LSourceParentNode);
            EXIT WHEN DBMS_XMLDom.ISNULL(LSourceParentNode) OR UPPER(LNodeName)=UPPER('#document');
            LNodePath         := '/'||LNodeName||LNodePath;
            LSourceParentNode := DBMS_XMLDom.GetParentNode(LSourceParentNode);
        END LOOP;
        LSourceParentNode := DBMS_XMLDom.GetParentNode(LSourceNode);
        LXPath:=LNodePath;
        IF AAttributeSource IS NOT NULL THEN
            LAttrs := dbms_xmldom.getattributes(LSourceParentNode);
            FOR LIndexAttr IN 0 .. dbms_xmldom.getLength(LAttrs) - 1 LOOP
                LAttr  := dbms_xmldom.item(LAttrs, LIndexAttr);
                LAttrName :=dbms_xmldom.getNodeName(LAttr);
                IF UPPER(LAttrName)=UPPER(AAttributeSource) THEN
                    LXPath:=LXPath||'[@'||LAttrName||'="'||dbms_xmldom.getNodeValue(LAttr)||'"]';
                    EXIT;
                END IF;
            END LOOP;
        END IF;

        LNodeListTarget:=DBMS_XSLProcessor.SelectNodes(LNodeTarget,LXPath);

        FOR LIndexTarget IN 0..DBMS_XMLDom.GetLength(LNodeListTarget) - 1 LOOP
            LNodeTarget := DBMS_XMLDom.Item(LNodeListTarget, LIndexTarget);
            LSourceNode := DBMS_XMLDom.ImportNode(LDOMDocumentTarget,LSourceNode,TRUE);
            LSourceNode := DBMS_XMLDom.AppendChild(LNodeTarget,LSourceNode);
        END LOOP;
    END LOOP;
END;

FUNCTION ValidateCompoundFragment(ACompoundID VW_Compound.CompoundID%Type, AXMLCompound XmlType,AXMLFragmentEquivalent XmlType) RETURN CLOB IS
    LqryCtx                DBMS_XMLGEN.ctxHandle;
    LFragmentsIdsValue     VARCHAR2(4000);
    LQuery                 VARCHAR2(4000);
    LFragmentCount         INTEGER;
    LPosOld                INTEGER;
    LPos                   INTEGER;
    LSameFragment          VARCHAR2(4000);
    LSameEquivalent        VARCHAR2(4000);
    LResult                VARCHAR2(4000);

BEGIN
    --FragmentsID
    SELECT XmlTransform(extract(AXMLCompound,'/Component/Compound/FragmentList/Fragment/FragmentID'),XmlType.CreateXml('
          <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
            <xsl:template match="/FragmentID">
                  <xsl:for-each select=".">
                      <xsl:value-of select="."/>,</xsl:for-each>
            </xsl:template>
          </xsl:stylesheet>')).GetClobVal()
      INTO LFragmentsIdsValue
      FROM dual;

    IF LFragmentsIdsValue IS NOT NULL THEN

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment','LFragmentsIdsValue->' ||LFragmentsIdsValue); $end null;

        LFragmentsIdsValue := SUBSTR(LFragmentsIdsValue, 1, Length(LFragmentsIdsValue) - 1);

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LFragmentsIdsValue->' || LFragmentsIdsValue); $end null;

        LFragmentCount := 1;
        LPosOld        := 0;
        LOOP
            LPos := NVL(INSTR(LFragmentsIdsValue, ',', LPosOld + 1), 0);
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', ' LFragmentsIdsValue->' || LFragmentsIdsValue || ' LPos->' || LPos || ' LPosOld->' || LPosOld); $end null;

            EXIT WHEN LPos = 0;
            LPosOld        := LPos;
            LFragmentCount := LFragmentCount + 1;
        END LOOP;

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LFragmentCount->' || LFragmentCount); $end null;

        LQuery := '
            SELECT 1
                FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
                WHERE CF.CompoundID=''' || ACompoundID ||
                ''' AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID AND
                      CF.FragmentID IN (' ||
                LFragmentsIdsValue || ')
                GROUP BY CF.CompoundID,BC.BatchID
                HAVING COUNT(1)=' || LFragmentCount ||
                ' AND ' || LFragmentCount ||
                '=(SELECT Count(1) FROM VW_Compound_Fragment CF1,VW_BatchComponentFragment BCF1, VW_BatchComponent BC1
                WHERE CF.CompoundID=CF1.CompoundID AND BC.BatchID=BC1.BatchID AND CF1.ID=BCF1.CompoundFragmentID AND BCF1.BatchComponentID=BC1.ID)';

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LQuery->' || LQuery); $end null;

        LQryCtx := DBMS_XMLGEN.newContext(LQuery);
        DBMS_XMLGEN.setMaxRows(LqryCtx, 3);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LSameFragment := DBMS_XMLGEN.getXML(LqryCtx);
        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LSameFragment->' || LSameFragment); $end null;
        DBMS_XMLGEN.closeContext(LqryCtx);

        IF LSameFragment IS NOT NULL THEN

            --Equivalent

             LQuery := 'SELECT 1 FROM
                (SELECT FragmentID,Equivalent
                    FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
                    WHERE CF.CompoundID=''' || ACompoundID ||
                  ''' AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID AND
                          CF.FragmentID IN (' ||
                  LFragmentsIdsValue || ')
                    ) Fragments WHERE Equivalent<>ExtractValue(Xmltype(''<Head>' ||
                  AXMLFragmentEquivalent.GetClobVal ||
                  '</Head>''),''/Head/BatchComponentFragmentList[1]/BatchComponentFragment[FragmentID=FragmentID]/Equivalents'')';

            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'Equivalent LQuery->' || LQuery); $end null;

            LQryCtx := DBMS_XMLGEN.newContext(LQuery);
            DBMS_XMLGEN.setMaxRows(LqryCtx, 3);
            DBMS_XMLGEN.setRowTag(LqryCtx, '');
            LSameEquivalent := DBMS_XMLGEN.getXML(LqryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LSameEquivalent->' || LSameEquivalent); $end null;
            DBMS_XMLGEN.closeContext(LqryCtx);

            IF LSameEquivalent IS NOT NULL THEN
                LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="False"';
            ELSE
                LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="True"';
            END IF;
        ELSE
            LResult := 'SAMEFRAGMENT="False" SAMEEQUIVALENT="False"';
        END IF;
    ELSE
        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment','LFragmentsIdsValue->' || LFragmentsIdsValue); $end null;

        LQuery := '
            SELECT 1
                FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
                WHERE CF.CompoundID=''' || ACompoundID ||
                ''' AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID';

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LQuery->' || LQuery); $end null;

        LQryCtx := DBMS_XMLGEN.newContext(LQuery);
        DBMS_XMLGEN.setMaxRows(LqryCtx, 3);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LSameFragment := DBMS_XMLGEN.getXML(LqryCtx);
        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LSameFragment->' || LSameFragment); $end null;
        DBMS_XMLGEN.closeContext(LqryCtx);

        IF LSameFragment IS NULL THEN
            LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="True"';
        ELSE
            LResult := 'SAMEFRAGMENT="False" SAMEEQUIVALENT="False"';
        END IF;
    END IF;

    RETURN LResult;

END;

FUNCTION ValidateCompoundMulti(AStructure CLOB, AStructureIDToValidate Number:=NULL, AConfigurationID Number:=1, AXMLCompound XmlType, AXMLFragmentEquivalent XmlType) RETURN CLOB IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LqryCtx              DBMS_XMLGEN.ctxHandle;
    LResult              CLOB;
    LDuplicateCount      Number;
    LParameters          Varchar2(1000);

    LCompoundID          VW_Compound.CompoundID%Type;
    LCount               NUMBER := 0;

    LFormulaWeight       VW_TEMPORARYCOMPOUND.FormulaWeight%Type;
    LPosTag              INTEGER;

    LFragmentsData       VARCHAR2(4000);

    LResultXMLType       XMLType;

    CURSOR C_RegNumbers(ACoumpoundID in VW_Compound.CompoundID%type) IS
        SELECT RegNumber
            FROM VW_RegistryNumber RN,VW_Mixture M,VW_Mixture_Component MC
            WHERE RN.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND MC.CompoundID=ACoumpoundID
            ORDER BY MC.CompoundID;

BEGIN
    BEGIN
        SELECT CSCartridge.MolWeight(AStructure)
            INTO LFormulaWeight
            FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            LFormulaWeight:=0;
        END;
    END;

    IF LFormulaWeight!= 0 THEN
        --Duplicate Validation

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','AStructureXML to validate->'||AStructure); $end null;

        INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (AStructure, 0);

        SELECT XmlTransform(XmlType(CoeDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="DUPLICATE_CHECKING"]/settings')),XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/settings">
                  <xsl:for-each select="add">
                    <xsl:choose>
                      <xsl:when test="@name!= ''''">
                        <xsl:value-of select="@name"/>=<xsl:value-of select="@value"/>,</xsl:when>
                    </xsl:choose>
                  </xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal()
        INTO LParameters
        FROM DUAL;

        LParameters:=SUBSTR(LParameters,1,LENGTH(LParameters)-1);

        IF NVL(AStructureIDToValidate,0)<>0 THEN
            LQryCtx := DBMS_XMLGEN.newContext('
              SELECT C.COMPOUNDID
                  FROM VW_Compound C, VW_Structure S
                  WHERE S.StructureId<>'||AStructureIDToValidate||' AND C.StructureId = S.StructureId AND
                        CsCartridge.MoleculeContains(S.Structure, ''select query from cscartridge.tempqueries where id = 0'','''','''||LParameters||''') = 1
                  ORDER BY C.RegID');
        ELSE
            LQryCtx := DBMS_XMLGEN.newContext('
                SELECT /*+ ORDERED FULL(C) INDEX(S MX)*/ C.COMPOUNDID
                    FROM  VW_Compound C, VW_Structure S
                    WHERE C.StructureId = S.StructureId AND
                          CsCartridge.MoleculeContains(S.Structure, ''select query from cscartridge.tempqueries where id = 0'','''','''||LParameters||''') = 1
                    ORDER BY C.RegID');
        END IF;

        DBMS_XMLGEN.setMaxRows(LqryCtx,30);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LResult := DBMS_XMLGEN.getXML(LqryCtx);
        DBMS_XMLGEN.closeContext(LqryCtx);

        LResult:=replace(LResult,chr(10),'');
        COMMIT;

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','LDuplicateCount->'||LDuplicateCount||' LResult->'||LResult); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','AStructureXML to validate->'||AStructure); $end null;

        LPosTag:=1;
        LDuplicateCount:=0;
        LOOP
            LPosTag := INSTR(LResult,'<COMPOUNDID',LPosTag+13);
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','LPosTag='||LPosTag); $end null;
        EXIT WHEN (LPosTag=0 or LPosTag is null);
            LDuplicateCount:=LDuplicateCount+1;
        END LOOP;

        --LDuplicateCount:=DBMS_XMLGEN.getNumRowsProcessed(LqryCtx); --did not work

        IF LDuplicateCount>0 THEN

            LResultXMLType:=XmlType.CreateXML(LResult);
            LResult:='<REGISTRYLIST>';

            FOR LIndexAux IN 1..LDuplicateCount LOOP
                SELECT extractvalue(LResultXMLType,'node()/node()['||LIndexAux||']')
                    INTO LCompoundID
                    FROM dual;

                IF LCompoundID IS NOT NULL THEN
                    SELECT COUNT (*)
                        INTO LCount
                        FROM VW_Mixture_Component MC
                        WHERE MC.CompoundID = LCompoundID
                            AND (SELECT COUNT (*)
                                FROM VW_Mixture m, VW_Mixture_Component mcc
                                WHERE M.MixtureID = MC.MixtureID AND M.MixtureID = MCC.MixtureID) = 1;

                    LFragmentsData:=ValidateCompoundFragment(LCompoundID,AXMLCompound,AXMLFragmentEquivalent);

                    FOR R_RegNumbers IN C_RegNumbers(LCompoundID) LOOP
                        LResult := LResult||'<REGNUMBER count="'||LCount||'" CompoundID="'||LCompoundID||'" '||LFragmentsData||'>'||R_RegNumbers.RegNumber||'</REGNUMBER>';
                    END LOOP;
                END IF;
            END LOOP;
            LResult:=LResult||'</REGISTRYLIST>';

            RETURN LResult;
        ELSE
            RETURN '';
        END IF;
    ELSE
        RETURN '';
    END IF;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eCompoundValidation, AppendError('Error validating the compound.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
    RETURN '';
END;

/**
Gets the Compound Registry Numbers that duplicate the structure.
-->author Fari
-->since June 2010
-->param ARegNumber              Compund Registry Number of the structure to evaluate
-->param AStructure              Strcuture to validate
-->param ARegIDToValidate        Identify the RegID of the Compound of the Strcuture,It should not be taked into account in the comparation
-->param LXMLRegNumberDuplicated List of the Registry Numbers Duplicated
*/
PROCEDURE VerifyAndAddDuplicateToSave(ARegNumber VW_RegistryNumber.RegNumber%Type,AStructure CLOB, ARegIDToValidate Number:=NULL, LXMLRegNumberDuplicated IN OUT NOCOPY XMLType) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LqryCtx              DBMS_XMLGEN.ctxHandle;
    LResult              CLOB;
    LResultXML           CLOB;
    LDuplicateCount      Number;
    LParameters          Varchar2(1000);
    LResultSerealized    Varchar2(4000);

    LCompoundID          VW_Compound.CompoundID%Type;
    LRegNumber           VW_RegistryNumber.RegNumber%Type;
    LCount               NUMBER := 0;

    LFormulaWeight       VW_TEMPORARYCOMPOUND.FormulaWeight%Type;
    LPosTag              INTEGER;

    LRLSState            Boolean;
    LResultXMLType       XMLType;

    CURSOR C_RegNumbers(ACoumpoundID in VW_Compound.CompoundID%type) IS
        SELECT RegNumber
            FROM VW_RegistryNumber RN,VW_Mixture M,VW_Mixture_Component MC
            WHERE RN.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND MC.CompoundID=ACoumpoundID
            ORDER BY MC.CompoundID;

BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','Begin'); $end null;
    BEGIN
        SELECT CSCartridge.MolWeight(AStructure)
            INTO LFormulaWeight
            FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            LFormulaWeight:=0;
        END;
    END;

    IF LFormulaWeight!= 0 THEN
        --Duplicate Validation

        $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','AStructureXML to validate->'||AStructure); $end null;

        INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (AStructure, 0);

        SELECT XmlTransform(XmlType(CoeDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="DUPLICATE_CHECKING"]/settings')),XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/settings">
                  <xsl:for-each select="add">
                    <xsl:choose>
                      <xsl:when test="@name!= ''''">
                        <xsl:value-of select="@name"/>=<xsl:value-of select="@value"/>,</xsl:when>
                    </xsl:choose>
                  </xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal()
        INTO LParameters
        FROM DUAL;

        LParameters:=SUBSTR(LParameters,1,LENGTH(LParameters)-1);

        LRLSState:=RegistrationRLS.GetStateRLS;
        IF LRLSState THEN
            RegistrationRLS.SetEnableRLS(False);
        END IF;

        IF NVL(ARegIDToValidate,0)<>0 THEN
            LQryCtx := DBMS_XMLGEN.newContext('
              SELECT RN.REGNUMBER
                  FROM VW_Compound C, VW_Structure S,VW_RegistryNumber RN
                  WHERE RN.RegID=C.RegID and C.RegID<>'||ARegIDToValidate||' AND C.StructureId = S.StructureId AND
                        CsCartridge.MoleculeContains(S.Structure, ''select query from cscartridge.tempqueries where id = 0'','''','''||LParameters||''') = 1
                  ORDER BY C.RegID');
        ELSE
            LQryCtx := DBMS_XMLGEN.newContext('
                SELECT /*+ ORDERED FULL(C) INDEX(S MX)*/ RN.REGNUMBER
                    FROM  VW_Compound C, VW_Structure S,VW_RegistryNumber RN
                    WHERE RN.RegID=C.RegID and C.StructureId = S.StructureId AND
                          CsCartridge.MoleculeContains(S.Structure, ''select query from cscartridge.tempqueries where id = 0'','''','''||LParameters||''') = 1
                    ORDER BY C.RegID');
        END IF;


        DBMS_XMLGEN.setMaxRows(LqryCtx,30);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LResultXML := DBMS_XMLGEN.getXML(LqryCtx);
        DBMS_XMLGEN.closeContext(LqryCtx);

        IF LRLSState THEN
            RegistrationRLS.SetEnableRLS(LRLSState);
        END IF;

        LResult:=replace(LResultXML,chr(10),'');
        COMMIT;

        $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LDuplicateCount->'||LDuplicateCount||' LResult->'||LResult); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','AStructureXML to validate->'||AStructure); $end null;


        IF LResultXML IS NOT NULL THEN  --Save in VW_Duplciated
            $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','1ºLResultXML->'||LResultXML); $end null;
            $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LResult->'||LResult); $end null;

            IF LXMLRegNumberDuplicated IS NOT NULL THEN
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','ARegNumber='||ARegNumber); $end null;
                LResultXML:=REPLACE(LResultXML,'<REGNUMBER>','<REGNUMBER NEW='''||ARegNumber||'''>');
                LResultXML:=REPLACE(LResultXML,'</ROWSET>','');
                SELECT extract(LXMLRegNumberDuplicated,'//ROWSET/REGNUMBER').GetClobVal()
                    INTO LResult
                    FROM DUAL;
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LResult||LResultXML||''</ROWSET>''->'||LResult||LResultXML||'</ROWSET>'); $end null;
                LXMLRegNumberDuplicated:=XmlType.CreateXml(LResultXML||LResult||'</ROWSET>');
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LXMLRegNumberDuplicated->'||LXMLRegNumberDuplicated.GetClobVal()); $end null;
            ELSE
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','ARegNumber='||ARegNumber); $end null;
                LResultXML:=REPLACE(LResultXML,'<REGNUMBER>','<REGNUMBER NEW='''||ARegNumber||'''>');
                LXMLRegNumberDuplicated:=XmlType.CreateXml(LResultXML);
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LXMLRegNumberDuplicated->'||LXMLRegNumberDuplicated.GetClobVal()); $end null;
            END IF;
        END IF;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eCompoundValidation, AppendError('Error validating the compound.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;


/**
Retrieve a XML with the list of Registry Number Duplciated. The argument identify the Registry Number appraise.
-->author Fari
-->since  June 2010
-->param  ARegNumber              Compund Registry Number of the structure to evaluate
-->return List of Registry Number Duplciated. For example CompoundRegistry.GetDuplicatedList('C/000048') gets:
            <DuplicateList RegNumber="C/000048">
                    <RegNumber PersonID="94" DuplicateDate="2010-06-08 01:15:40">C/000046/RegNumber>
                    <RegNumber PersonID="94" DuplicateDate="2010-06-08 01:15:350>C/000047</RegNumber>
                    <RegNumber PersonID="94" DuplicateDate="2010-06-08 01:15:55">C/000049</RegNumber>
            </DuplicateList>
*/

FUNCTION GetDuplicatedList(ARegNumber IN VW_RegistryNumber.RegNumber%type) RETURN CLOB IS

    LListDuplicated CLOB;

    CURSOR C_RegNumberDuplicated(ARegNumber IN VW_RegistryNumber.RegNumber%type) IS
        SELECT RegNumberDuplicated,PersonID,Created
            FROM VW_Duplicates
            WHERE RegNumber=ARegNumber
        UNION ALL
        SELECT RegNumber,PersonID,Created
            FROM VW_Duplicates
            WHERE RegNumberDuplicated=ARegNumber
        ORDER BY 1;
BEGIN
    SetSessionParameter;

    LListDuplicated:='<DuplicateList RegNumber="'||ARegNumber||'">';
    FOR R_RegNumberDuplicated IN C_RegNumberDuplicated(ARegNumber) LOOP
        LListDuplicated:=LListDuplicated||'<RegNumber PersonID="'||R_RegNumberDuplicated.PersonID||'" DuplicateDate="'||R_RegNumberDuplicated.Created||'">'||R_RegNumberDuplicated.RegNumberDuplicated||'</RegNumber>';
    END LOOP;

    LListDuplicated:=LListDuplicated||'</DuplicateList>';

    RETURN LListDuplicated;
END;

FUNCTION TakeOffAndGetClob(AXml IN OUT NOCOPY Clob,ABeginTag VARCHAR2) RETURN CLOB IS
    LValue           CLOB;
    LTagBegin        Number;
    LTagEnd          Number;
    LEndTag          VARCHAR2(255);
    LBeginTag        VARCHAR2(255);
BEGIN

    LBeginTag:='<'||ABeginTag||'>';
    LEndTag:='</'||ABeginTag||'>';

    LTagBegin:=INSTR(AXml,LBeginTag);
    LTagEnd:=INSTR(AXml,LEndTag);

    $if CompoundRegistry.Debuging $then InsertLog('TakeOffAndGetClob','LTagBegin->'||LTagBegin||' LTagEnd->'||LTagEnd); $end null;

    IF (LTagBegin<>0) AND (LTagEnd<>0) THEN
        LValue:=SUBSTR(AXml,LTagBegin+LENGTH(LBeginTag),LTagEnd-LTagBegin-LENGTH(LBeginTag));
        AXml:=SUBSTR(AXml,1,LTagBegin-1)||SUBSTR(AXml,LTagEnd+LENGTH(LEndTag),LENGTH(AXml));

        $if CompoundRegistry.Debuging $then InsertLog('TakeOffAndGetClob','LValue->'); $end null;

    ELSE
        LValue:='';
    END IF;

    RETURN LValue;
END;

/**
Helper procedure to trim the element name
-->param ATag   string representation of an xml element
-->return       the element name ignoring any provided '<', '>', or spaces
*/
FUNCTION TrimElement(ATag VARCHAR2) RETURN VARCHAR2 IS
    LTag  VARCHAR2(255);
BEGIN
    -- Get the element name ignoring any provided '<', '>', or spaces
    LTag := Trim(ATag);
    LTag := Ltrim(Ltrim(LTag, '<'));
    LTag := Rtrim(Rtrim(LTag, '>'));
    RETURN LTag;
END;

/**
Helper procedure for locating tags in XML.
ALft and ARht will be the positions of the '<' and '>' respectively. Both are 0 if the element is not found.
-->param AXml
-->param ATag
-->param ALft
-->param ARht
-->param AStart
-->return       a string representation of the element 'begin' tag
*/FUNCTION LocateElement(AXml IN CLOB, ATag VARCHAR2, ALft OUT NUMBER, ARht OUT NUMBER, AStart IN NUMBER := 1) RETURN VARCHAR2 IS
    LTag  VARCHAR2(255);
    LTemp VARCHAR2(255);
    LLft  NUMBER;
BEGIN
    -- Initialize return values
    ALft := 0;
    ARht := 0;
    -- Get the element name ignoring any provided '<', '>', or spaces
    LTag := TrimElement(ATag);
    -- Locate the left end of the element
    LLft := AStart;
    LOOP
        LLft := NVL(Instr(AXml, '<' || LTag, LLft), 0);
        IF LLft = 0 THEN
          RETURN NULL; -- Unable to locate the tag
        END IF;
        LTemp := Substr(AXml, LLft, 1 + Length(LTag) + 1);
        EXIT WHEN (LTemp = '<' || LTag || '>');
        EXIT WHEN (LTemp = '<' || LTag || ' ');
        LLft := LLft + 1;
    END LOOP;
    -- Locate the right end of the element
    ARht := NVL(Instr(AXml, '>', LLft), 0);
    IF ARht = 0 THEN
        RETURN NULL; -- Unable to locate the matching '>' (should not happen)
    END IF;
    ALft := LLft; -- Located the right end so now we can safely return the left end
    RETURN Substr(AXml, ALft, ARht + 1 - ALft);
END;

/**
Helper procedure for locating a matching end element
This is not trivial if elements with the same tag name are nested
AStart must point to the '<' of the begin element
-->param AXml
-->param AStart
-->param ALft
-->param ARht
-->return       a string representation of the element 'end' tag
*/
FUNCTION LocateMatchingElement(AXml IN CLOB, AStart IN NUMBER, ALft OUT NUMBER, ARht OUT NUMBER) RETURN VARCHAR2 IS
    LLft          NUMBER;
    LDepth        NUMBER := 0;
    LTag          VARCHAR2(255);
    LBegin        VARCHAR2(255);
    LBeginLft     NUMBER;
    LBeginRht     NUMBER;
    LEnd          VARCHAR2(255);
    LEndLft       NUMBER;
    LEndRht       NUMBER;
BEGIN
    -- Initialize return values
    ALft := 0;
    ARht := 0;
    -- Get begin element
    LBeginLft := AStart;
    LBeginRht := NVL(Instr(AXml, '>', LBeginLft), 0);
    IF LBeginRht = 0 THEN
        RETURN NULL; -- Unable to locate the matching '>' (should not happen)
    END IF;
    LBegin := Substr(AXml, LBeginLft, LBeginRht + 1 - LBeginLft);
    -- Initial end
    LEnd := '';
    LEndLft := LBeginRht;
    LEndRht := LBeginRht;
    -- Trim begin element to tag
    LTag := LBegin;
    LTag := Ltrim(LTag, '<');
    LTag := Rtrim(LTag, '>');
    LTag := Substr(LTag, NVL(Instr(LTag, ' '), Length(LTag) + 1) - 1);
    -- Set initial depth
    IF Instr(LBegin, Length(LBegin) - 1, 1) != '/' THEN
        LDepth := LDepth + 1; -- Increase for begin element
    END IF;
    LOOP
        EXIT WHEN LDepth = 0; -- Balancing match has been found
        -- Error if previous begin element was not valid
        IF LBeginLft = 0 OR LBeginLft > LEndLft THEN
            RETURN NULL;
        END IF;
        -- Locate the next end element
        LEnd := LocateElement(AXml, '/' || LTag, LEndLft, LEndRht, LEndRht + 1);
        IF LEndLft = 0 THEN
            RETURN NULL;  -- Could not locate balancing end tag (should not happen)
        END IF;
        LDepth := LDepth - 1; -- Decrease for end element
        -- Locate the next begin element before end element that affects level
        LOOP
            LBegin := LocateElement(AXml, LTag, LBeginLft, LBeginRht, LBeginRht + 1);
            EXIT WHEN LBeginLft = 0;  -- No more begin elements
            EXIT WHEN LBeginLft > LEndLft;  -- No more begin lemenets before end elememt
            IF Instr(LBegin, Length(LBegin) - 1, 1) != '/' THEN
              LDepth := LDepth + 1; -- Increase for begin element
              EXIT WHEN TRUE;
            END IF;
        END LOOP;
    END LOOP;
    ALft := LEndLft;
    ARht := LEndRht;
    RETURN Substr(AXml, ALft, ARht + 1 - ALft);
END;

/**
Excises a complete xml element (as a CLOB) from a parent document; caller determines
if it is a 'cut' or 'copy' operation. (?)
-->param AXml
-->param ABeginTag
-->param ABeginTagParent
-->param ABeginTagGranParent  defaults to NULL
-->param ASourceDeleteTagName defaults to True
-->param AUpdateVerify        defaults to False
-->return                     a string representation of an element
*/
FUNCTION TakeOffAndGetClobsList(AXml IN OUT NOCOPY Clob, ABeginTag VARCHAR2,ABeginTagParent VARCHAR2:=NULL,ABeginTagGranParent VARCHAR2:=NULL,ASourceDeleteTagName Boolean:=TRUE, AUpdateVerify Boolean:=FALSE) RETURN CLOB IS
    LReturn       CLOB := '';
    LSearchPos    NUMBER;
    LElement      VARCHAR2(255);
    LElementLft   NUMBER;
    LElementRht   NUMBER;
    LBegin        VARCHAR2(255);
    LBeginLft     NUMBER;
    LBeginRht     NUMBER;
    LEnd          VARCHAR2(255);
    LEndLft       NUMBER;
    LEndRht       NUMBER;
    LSaveInnerXML Boolean;
BEGIN
    LSearchPos := 1;
    LOOP
        -- Locate grandparent if any
        IF ABeginTagGranParent IS NOT NULL THEN
            LElement := LocateElement(AXml, ABeginTagGranParent, LElementLft, LElementRht, LSearchPos);
            EXIT WHEN LElementLft = 0; -- Unable to locate the grandparentubstr(LElement, Length(LElement) - 1, 1) = '/'; -- Grandparent is empty
            LSearchPos := LElementRht + 1; -- Establish the new search position
        END IF;
        -- Locate parent if any
        IF ABeginTagParent IS NOT NULL THEN
            LElement := LocateElement(AXml, ABeginTagParent, LElementLft, LElementRht, LSearchPos);
            EXIT WHEN LElementLft = 0; -- Unable to locate the parent
            EXIT WHEN Substr(LElement, Length(LElement) - 1, 1) = '/'; -- Parent is empty
            LSearchPos := LElementRht + 1; -- Establish the new search position
        END IF;
        -- Locate tag
        LElement := LocateElement(AXml, ABeginTag, LElementLft, LElementRht, LSearchPos);
        EXIT WHEN LElementLft = 0; -- Unable to locate the tag
        LBegin := LElement;
        LBeginLft := LElementLft;
        LBeginRht := LElementRht;
        LSearchPos := LElementRht + 1; -- Establish the new search position
        IF Substr(LElement, Length(LElement) - 1, 1) != '/' THEN
            -- WJC a LOOP must be added in the future to handle the case of nested element of the same name !!!
            LElement := LocateElement(AXml, '/' || TrimElement(ABeginTag), LElementLft, LElementRht, LSearchPos);
            EXIT WHEN LElementLft = 0; -- Unable to locate the end tag (should not happen)
            LEnd := LElement;
            LEndLft := LElementLft;
            LEndRht := LElementRht;
        ELSE
            -- The element is empty so we make an empty end tag
            LEnd := '';
            LEndLft := LBeginRht;
            LEndRht := LEndLft;
        END IF;

        -- Decide if we are saving the inner XML
        IF AUpdateVerify THEN
            $if CompoundRegistry.Debuging $then InsertLog('TakeOffAndGetClobsList',' LBeginElement='||LBegin); $end null;
            IF Instr(Upper(LBegin), 'UPDATE="YES"') = 0 AND
                Instr(Upper(LBegin), 'UPDATE=''YES''') = 0 AND
                Instr(Upper(LBegin), 'INSERT="YES"') = 0 AND
                Instr(Upper(LBegin), 'INSERT=''YES''') = 0 THEN
                LSaveInnerXML := FALSE; -- Not Update="yes" or Insert="yes"
            ELSE
                IF Instr(Upper(LBegin), 'DELETE="YES"') = 0 AND
                   Instr(Upper(LBegin), 'DELETE=''YES''') = 0 THEN
                  LSaveInnerXML := TRUE;  -- Update="yes" or Insert="yes" but not Delete="yes"
                ELSE
                  LSaveInnerXML := FALSE; -- Update="yes" or Insert="yes" and Delete="yes"
                END IF;
            END IF;
        ELSE
            LSaveInnerXML := TRUE;
        END IF;

        -- Save or discard the inner XML
        IF LSaveInnerXML THEN
            LReturn := LReturn || '<Clob>' || Substr(AXml, (LBeginRht + 1), (LEndLft + 1) - 1 - (LBeginRht + 1)) || '</Clob>';
        ELSE
            LReturn := LReturn || '<Clob></Clob>';
        END IF;

        LSearchPos := LElementRht + 1; -- Establish the new search position
        -- Temporarily convert LSearchPos to be relative to the end of the string
        LSearchPos := Length(AXml) - LSearchPos;
        -- Remove inner XML or outer XML depending on ASourceDeleteTagName
        -- If inner XML is removed then substitute placeholder
        IF ASourceDeleteTagName THEN
            -- Remove outer XML
                AXml := Substr(AXml, 1, LBeginLft - 1) || Substr(AXml, LEndRht + 1, Length(AXml));
        ELSE
            -- Remove inner XML
            AXml := Substr(AXml, 1, LBeginRht) || '(Removed' || TrimElement(ABeginTag) || ')' || Substr(AXml, LEndLft, Length(AXml));
        END IF;
        -- Convert LSearchPos back to relative to the start of the string
        LSearchPos := Length(AXml) - LSearchPos;

    END LOOP;
    LReturn := '<ClobList>' || LReturn || '</ClobList>';
    RETURN LReturn;
END;

FUNCTION TakeOnAndGetXml(AXml IN Clob,ATagName VARCHAR,AStructuresList IN OUT NOCOPY Clob) RETURN Clob IS
    LValue             CLOB;
    LStrcutureValue    CLOB;
    LStructuresList    CLOB;
    LTagBegin          Number;
    LValueStr          Varchar2(255);
    LStrcutureTagBegin Number;
BEGIN
    LValue:=AXml;
    LStructuresList:=AStructuresList;
    LValueStr:='(Removed'||ATagName||')';
    LOOP
        LTagBegin:=INSTR(LValue,LValueStr);
    EXIT WHEN (LTagBegin=0);
        LStrcutureValue:=TakeOffAndGetClob(LStructuresList,'Clob');

        LStrcutureTagBegin:=INSTR(LValue,LValueStr);
        IF LStrcutureTagBegin<>0 THEN
            LValue:=SUBSTR(LValue,1,LStrcutureTagBegin-1)||LStrcutureValue||SUBSTR(LValue,LStrcutureTagBegin+LENGTH(LValueStr),LENGTH(LValue));
        END IF;
    END LOOP;
    RETURN LValue;
END;

FUNCTION GetSaltCode(AXmlTables IN XmlType) RETURN VARCHAR2 IS
  LSaltCode   VARCHAR2(2000);
  LFragmentID VW_Fragment.FragmentID%Type;
  LIndex      Number;
  LXMLCompound XmlType;
BEGIN
   -- IF COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="MISC"]/settings/add[@name="SameBatchesIdentity"]/@value')='True' THEN

   SELECT extract(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[2]')
              INTO LXMLCompound
              FROM dual;

   IF LXMLCompound IS NULL THEN --Is Null then is single compound
        $if CompoundRegistry.Debuging $then InsertLog('GetSaltCode', 'Sigle compund'); $end null;
        LIndex:=0;
        LOOP
            LIndex:=LIndex+1;
            $if CompoundRegistry.Debuging $then InsertLog('GetSaltCode','LIndex ->'||LIndex); $end null;

            SELECT extractValue(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[1]/Compound/FragmentList/Fragment['||LIndex||']/FragmentID')
              INTO LFragmentID
              FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('GetSaltCode', 'LFragmentID ->' || LFragmentID); $end null;

        EXIT WHEN LFragmentID IS NULL;
            BEGIN
                 SELECT LSaltCode||F.Code
                    INTO LSaltCode
                    FROM VW_Fragment F,VW_FragmentType FT
                    WHERE F.FragmentTypeID=FT.ID AND UPPER(FT.DESCRIPTION)='SALT' AND F.FragmentID=LFragmentID;
                    $if CompoundRegistry.Debuging $then InsertLog('GetSaltCode', 'LSaltCode ->' || LSaltCode); $end null;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN NULL; --if there isn't a salt then continue
            END;
        END LOOP;

        RETURN LSaltCode;
    ELSE
        $if CompoundRegistry.Debuging $then InsertLog('GetSaltCode', 'Multy compund'); $end null;
        RETURN NULL;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        BEGIN
            $if CompoundRegistry.Debuging $then InsertLog('GetSaltCode','Error getting Salt Code.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, AppendError('Error getting the Salt Code.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
        END;
END;

FUNCTION GetSaltDescription(AXmlTables IN XmlType) RETURN VARCHAR2 IS
  LSaltDescription   VARCHAR2(2000);
  LFragmentID        VW_Fragment.FragmentID%Type;
  LIndex             Number;
  LXMLCompound       XmlType;
BEGIN
    --IF COEDB.ConfigurationManager.RetrieveParameter('Registration','Registration/applicationSettings/groups/add[@name="MISC"]/settings/add[@name="SameBatchesIdentity"]/@value')='True' THEN
    SELECT extract(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[2]')
        INTO LXMLCompound
        FROM dual;

    IF LXMLCompound IS NULL THEN
        LIndex:=0;
        LOOP
            LIndex:=LIndex+1;
            $if CompoundRegistry.Debuging $then InsertLog('LSaltDescription','LIndex ->'||LIndex); $end null;

            SELECT extractValue(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[1]/Compound/FragmentList/Fragment['||LIndex||']/FragmentID')
              INTO LFragmentID
              FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('GetSaltDescription', 'LFragmentID ->' || LFragmentID); $end null;

        EXIT WHEN LFragmentID IS NULL;

            BEGIN
                SELECT LSaltDescription||F.Description
                    INTO LSaltDescription
                    FROM VW_Fragment F,VW_FragmentType FT
                    WHERE F.FragmentTypeID=FT.ID AND UPPER(FT.DESCRIPTION)='SALT' AND F.FragmentID=LFragmentID;
                $if CompoundRegistry.Debuging $then InsertLog('GetSaltDescription', 'LSaltDescription ->' || LSaltDescription); $end null;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN NULL; --if there isn't a salt then continue
            END;

        END LOOP;

        RETURN LSaltDescription;
    ELSE
        RETURN NULL;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        BEGIN
            $if CompoundRegistry.Debuging $then InsertLog('GetSaltDescription','Error getting Salt Names.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
            RAISE_APPLICATION_ERROR(eGenericException, AppendError('Error getting the Salt Description.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
        END;
END;

/**
Generates a full registration number for a registration record.
-->param ASequenceID
-->param ASequenceNumber
-->param AXmlTables
-->param AIncSequence         defaults to 'Y'
-->return                     a registration number
*/
FUNCTION GetRegNumber(ASequenceID in VW_SEQUENCE.SequenceId%Type, ASequenceNumber out VW_REGISTRYNUMBER.SequenceNumber%Type, AXmlTables IN XmlType,AIncSequence IN Char:='Y') RETURN VW_REGISTRYNUMBER.RegNumber%Type IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LRegNumber VW_REGISTRYNUMBER.RegNumber%Type;
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('GetRegNumber','ASequenceID'||'->'||ASequenceID); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('GetRegNumber','AIncSequence'||'->'||AIncSequence); $end null;
    IF AIncSequence='Y' THEN
        SELECT Prefix||PrefixDelimiter||lpad(NVL(NextInSequence,1),RegNumberLength,'0'),NVL(NextInSequence,1)
            INTO LRegNumber,ASequenceNumber
            FROM VW_SEQUENCE
            WHERE SequenceID=ASequenceID
            FOR UPDATE;
        $if CompoundRegistry.Debuging $then InsertLog('GetRegNumber','LRegNumber'||'->'||LRegNumber); $end null;
        UPDATE VW_SEQUENCE
            SET NextInSequence=NVL(NextInSequence,1)+1
            WHERE SequenceID=ASequenceID;
        COMMIT;
    ELSE
         SELECT Prefix||PrefixDelimiter||lpad(NVL(NextInSequence,1),RegNumberLength,'0'),NVL(NextInSequence,1)
            INTO LRegNumber,ASequenceNumber
            FROM VW_SEQUENCE
            WHERE SequenceID=ASequenceID;
            $if CompoundRegistry.Debuging $then InsertLog('GetRegNumber','LRegNumber'||'->'||LRegNumber); $end null;
    END IF;

    RETURN LRegNumber;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('GetRegNumber','Error inseting registry.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('Error getting Registry Number.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

/**
Generates a full registration number for a registration record.
-->param ASequenceID
-->param AXmlTables
-->param AIncSequence         defaults to 'Y'
-->return                     a registration number
*/
FUNCTION GetFullRegNumber(ASequenceID in VW_SEQUENCE.SequenceId%Type, AXmlTables IN XmlType,ARegNumber VW_REGISTRYNUMBER.RegNumber%Type,ABatchNumber VW_BATCH.BatchNumber%Type, ABatchNumberPadding INTEGER) RETURN VW_BATCH.FullRegNumber%Type IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LSaltName        VARCHAR2(2000):='';
    LSaltSuffixType  VW_SEQUENCE.SaltSuffixType%Type;
    LSuffixDelimiter VW_SEQUENCE.SuffixDelimiter%Type;
    LBatchDelimiter  VW_SEQUENCE.BatchDelimiter%Type;
    LFullRegNumber   VW_BATCH.FullRegNumber%Type;
    LSequenceNumber  VW_REGISTRYNUMBER.SequenceNumber%Type;
    LXmlTables       XmlType;

    PROCEDURE CleanXML IS
        LListCompoundFragmentID VARCHAR2(2000);
        LCompoundFragmentID     NUMBER;
        LFragmentID             NUMBER;
        LIndex                  NUMBER;
        LXMLCompound            XmlType;
        LXMLFragmentDeleted     XmlType;
    BEGIN
        SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[2]')
            INTO LXMLCompound
            FROM dual;

        IF LXMLCompound IS NULL THEN
            $if CompoundRegistry.Debuging $then InsertLog('CleanXML', 'CleanXML'); $end null;
            LIndex:=0;
            LOOP
                LIndex:=LIndex+1;
                $if CompoundRegistry.Debuging $then InsertLog('CleanXML','LIndex ->'||LIndex); $end null;

                SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[1]/Compound/FragmentList/Fragment['||LIndex||']/CompoundFragmentID'),
                       extractValue(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[1]/Compound/FragmentList/Fragment['||LIndex||']/FragmentID')
                  INTO LCompoundFragmentID,LFragmentID
                  FROM dual;

                EXIT WHEN LCompoundFragmentID IS NULL;

                $if CompoundRegistry.Debuging $then InsertLog('CleanXML', 'LCompoundFragmentID ->' || LCompoundFragmentID); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('CleanXML', ' INSTR(- LCompoundFragmentID,LListCompoundFragmentID)->' ||  INSTR('-'||LCompoundFragmentID,LListCompoundFragmentID)); $end null;
                IF INSTR(LListCompoundFragmentID,'-'||LCompoundFragmentID)>0 THEN
                    SELECT UpdateXML(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[1]/Compound/FragmentList/Fragment['||LIndex||']'  ,'')
                        INTO LXmlTables
                        FROM dual;
                ELSE
                    $if CompoundRegistry.Debuging $then InsertLog('CleanXML', 'LFragmentID ->' || LFragmentID); $end null;
                    SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment[@delete="yes" and FragmentID='||LFragmentID||']')
                        INTO LXMLFragmentDeleted
                        FROM dual;
                    IF LXMLFragmentDeleted IS NOT NULL THEN
                        $if CompoundRegistry.Debuging $then InsertLog('CleanXML', 'LXMLFragmentDeleted ->' || LXMLFragmentDeleted.getClobVal()); $end null;
                        SELECT UpdateXML(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[1]/Compound/FragmentList/Fragment['||LIndex||']'  ,'')
                            INTO LXmlTables
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('CleanXML', 'LXmlTables='||LXmlTables.getClobVal()); $end null;
                    ELSE
                        LListCompoundFragmentID:=LListCompoundFragmentID||'-'||LCompoundFragmentID;
                    END IF;

                END IF;

                $if CompoundRegistry.Debuging $then InsertLog('CleanXML', 'LListCompoundFragmentID ->' || LListCompoundFragmentID); $end null;
            END LOOP;
        END IF;
    END;

BEGIN
    LXmlTables:=AXmlTables;
    $if CompoundRegistry.Debuging $then InsertLog('GetFullRegNumber','ASequenceID'||'->'||ASequenceID); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('GetFullRegNumber','ARegNumber'||'->'||ARegNumber); $end null;
    SELECT SuffixDelimiter,SaltSuffixType,BatchDelimiter
        INTO LSuffixDelimiter,LSaltSuffixType,LBatchDelimiter
        FROM VW_SEQUENCE
            WHERE SequenceID=ASequenceID;

    LFullRegNumber:=ARegNumber;

    IF LSaltSuffixType IS NOT NULL  THEN
        CleanXML;
        EXECUTE IMMEDIATE 'BEGIN :LSaltName:=CompoundRegistry.'||LSaltSuffixType||'(:LXmlTables); END;' USING  OUT LSaltName, IN LXmlTables ;

        $if CompoundRegistry.Debuging $then InsertLog('GetFullRegNumber','LSaltName'||'->'||LSaltName); $end null;

        IF LSaltName IS NOT NULL THEN
            LFullRegNumber:=SUBSTR(LFullRegNumber||LSuffixDelimiter||LSaltName,1,50);
        END IF;
    END IF;

    LFullRegNumber:=SUBSTR(LFullRegNumber||LBatchDelimiter||LPad(ABatchNumber,ABatchNumberPadding,'0'),1,50);

    $if CompoundRegistry.Debuging $then InsertLog('GetFullRegNumber','LFullRegNumber'||'->'||LFullRegNumber); $end null;
    RETURN LFullRegNumber;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('GetFullRegNumber','Error inseting registry.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('Error getting Registry Number.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;


PROCEDURE InsertData(ATableName IN CLOB,AXmlRows IN CLOB,AStructureValue IN CLOB,AStructureAggregationValue IN CLOB,AFragmentXmlValue IN CLOB,ANormalizedStructureValue IN CLOB,ACompoundID IN Number,AStructureID IN Number,AMixtureID IN Number,AMessage IN OUT NOCOPY CLOB,ARowsInserted IN OUT Number) IS
    LinsCtx       DBMS_XMLSTORE.ctxType;
BEGIN
   --Create the Table Context
    LinsCtx := DBMS_XMLSTORE.newContext(ATableName);
    DBMS_XMLSTORE.clearUpdateColumnList(LinsCtx);

    $if CompoundRegistry.Debuging $then InsertLog('InsertData','ATableName->'||ATableName||' AXmlRows->'||AXmlRows); $end null;

    --Insert Rows and get count it inserted
    ARowsInserted := DBMS_XMLSTORE.insertXML(LinsCtx, AXmlRows);

    --Build Message Logs
    AMessage := AMessage || ' ' || cast(ARowsInserted as string) || ' Row/s Inserted on "' || ATableName || '".' || CHR(13);

    $if CompoundRegistry.Debuging $then InsertLog('InsertData','Message->'||AMessage); $end null;

    --Close the Table Context
    DBMS_XMLSTORE.closeContext(LinsCtx);
    CASE UPPER(ATableName)
        WHEN 'VW_STRUCTURE' THEN
            BEGIN
                IF AStructureValue IS NOT NULL THEN
                    $if CompoundRegistry.Debuging $then InsertLog('InsertData',' LStructureID:'||AStructureID||' LStructureValue:'||AStructureValue); $end null;
                    UPDATE VW_STRUCTURE
                        SET STRUCTURE=AStructureValue
                        WHERE STRUCTUREID=AStructureID;
                END IF;
            END;
        WHEN 'VW_MIXTURE' THEN
            BEGIN
                IF AStructureAggregationValue IS NOT NULL THEN
                   $if CompoundRegistry.Debuging $then InsertLog('InsertData',' LMixtureID:'||AMixtureID||' LStructureAggregationValue:'||AStructureAggregationValue); $end null;
                    UPDATE VW_MIXTURE
                        SET StructureAggregation=AStructureAggregationValue
                        WHERE MixtureID=AMixtureID;
                END IF;
            END;
        WHEN 'VW_FRAGMENT' THEN
            BEGIN
                IF UPPER(ATableName)='VW_FRAGMENT' AND AFragmentXmlValue IS NOT NULL THEN
                    $if CompoundRegistry.Debuging $then InsertLog('InsertData',' LStructureID:'||AStructureID||' LFragmentXmlValue:'||AFragmentXmlValue); $end null;
                    UPDATE VW_STRUCTURE
                        SET STRUCTURE=AFragmentXmlValue
                        WHERE StructureID=AStructureID;
                END IF;
            END;
        WHEN 'VW_COMPOUND' THEN
            BEGIN
                IF ANormalizedStructureValue IS NOT NULL THEN
                    $if CompoundRegistry.Debuging $then InsertLog('InsertData',' ACompoundID:'||ACompoundID||' ANormalizedStructureValue:'||ANormalizedStructureValue); $end null;
                    UPDATE VW_COMPOUND
                        SET NORMALIZEDSTRUCTURE=ANormalizedStructureValue
                        WHERE COMPOUNDID=ACompoundID;
                END IF;
            END;
        ELSE NULL;
    END CASE;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('InsertData','Error inseting registry.'||chr(10)||AMessage||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
        RAISE_APPLICATION_ERROR(eInsertData, 'Error inseting registry.' || chr(13) || SQLERRM || chr(13) || DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
    END;

END;

PROCEDURE DeleteStructure(AStrcutureIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
    LCountStructure Number;
BEGIN
    IF NVL(AStrcutureIDToDelete,0)>0 THEN

        SELECT  NVL(Count(1),0)
                INTO  LCountStructure
                FROM  VW_Compound
                WHERE StructureID=AStrcutureIDToDelete;
        $if CompoundRegistry.Debuging $then InsertLog('DeleteStructure',' LCountStructure:'||LCountStructure); $end null;

        IF LCountStructure=0 THEN
            DELETE VW_Structure WHERE StructureID=AStrcutureIDToDelete;
            AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Structure".';
        END IF;
    END IF;
END;

PROCEDURE DeleteCompound(ACompoundIDToDelete IN Number, AMixtureID IN Number,AMessage IN OUT NOCOPY Varchar2) IS
    LCountMixture Number;
    LCountStructure Number;
    LStructureID Number;
    LRegID Number;
BEGIN
    DELETE VW_BatchComponentFragment WHERE BatchComponentID IN (SELECT ID FROM VW_BatchComponent WHERE MixtureComponentID IN (SELECT MixtureComponentID FROM VW_Mixture_Component WHERE CompoundID=ACompoundIDToDelete and MixtureID=AMixtureID));
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponentFragment".';
    DELETE VW_BatchComponent WHERE MixtureComponentID IN (SELECT MixtureComponentID FROM VW_Mixture_Component WHERE CompoundID=ACompoundIDToDelete and MixtureID=AMixtureID);
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponent".';

    --Delete the compound
    SELECT  NVL(Count(1),0)  --take into account that DELETE VW_Mixture_Component was just done
        INTO  LCountMixture
        FROM  VW_Mixture_Component
        WHERE CompoundID=ACompoundIDToDelete AND MixtureID!=AMixtureID;
    $if CompoundRegistry.Debuging $then InsertLog('DeleteCompound','ACompoundIDToDelete'||ACompoundIDToDelete||' LCountMixture:'||LCountMixture); $end null;
    IF LCountMixture=0 THEN
        SELECT RegID
            INTO LRegID
            FROM VW_Compound
            WHERE CompoundID=ACompoundIDToDelete;

        DELETE VW_Compound_Identifier WHERE RegID = LRegID;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Identifier".';
        DELETE VW_Duplicates WHERE RegNumber IN (SELECT RN.RegNumber FROM VW_Compound C,VW_RegistryNumber RN WHERE C.CompoundID=ACompoundIDToDelete AND RN.RegID=C.RegID);
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Duplicates".';
        DELETE VW_Duplicates WHERE RegNumberDuplicated IN (SELECT RN.RegNumber FROM VW_Compound C,VW_RegistryNumber RN WHERE C.CompoundID=ACompoundIDToDelete AND RN.RegID=C.RegID);
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Duplicates".';
        DELETE VW_RegistryNumber WHERE RegID = LRegID;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_RegistryNumber".';
        DELETE VW_Compound_Fragment WHERE CompoundID=ACompoundIDToDelete;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Fragment".';

        BEGIN
            SELECT StructureID
                INTO LStructureID
                FROM VW_Compound
                WHERE  CompoundID=ACompoundIDToDelete;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                LStructureID:=0;
        END;

        DELETE VW_Mixture_Component WHERE MixtureID=AMixtureID AND CompoundID=ACompoundIDToDelete;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture_Component".';

        DELETE VW_Compound WHERE CompoundID=ACompoundIDToDelete;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound".';
        $if CompoundRegistry.Debuging $then InsertLog('DeleteCompound',' DELETE VW_Compound WHERE:'||SQL%ROWCOUNT); $end null;
        IF LStructureID>0 THEN
            SELECT  NVL(Count(1),0)
                INTO  LCountStructure
                FROM  VW_Compound
                WHERE StructureID=LStructureID;

            IF LCountStructure=0 THEN
                DELETE VW_Structure WHERE StructureID=LStructureID;
                AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Structure".';
            END IF;
        END IF;
    ELSE
        DELETE VW_Mixture_Component WHERE MixtureID=AMixtureID AND CompoundID=ACompoundIDToDelete;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture_Component".';
    END IF;
END;

PROCEDURE DeleteFragment(ACompoundfragmentIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
BEGIN
    DELETE VW_BatchComponentFragment WHERE CompoundFragmentID=ACompoundfragmentIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponentFragment".';
    DELETE VW_Compound_Fragment WHERE ID=ACompoundfragmentIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Fragment".';
END;

PROCEDURE DeleteIdentifier(ACompoundIdentifierIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
BEGIN
    DELETE VW_Compound_Identifier WHERE ID=ACompoundIdentifierIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Identifier".';
END;

PROCEDURE DeleteRegistryNumberProject(ARegistryProjectIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
BEGIN
    DELETE VW_RegistryNumber_Project WHERE ID=ARegistryProjectIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_RegistryNumber_Project".';
END;

PROCEDURE DeleteBatchProject(ABatchProjectIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
BEGIN
    DELETE VW_Batch_Project WHERE ID=ABatchProjectIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Batch_Project".';
END;

PROCEDURE DeleteBatchIdentifier(ABatchIdentifierIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
BEGIN
    DELETE VW_BatchIdentifier WHERE ID=ABatchIdentifierIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchIdentifier".';
END;

PROCEDURE DeleteBatchComponentFragment(ABatchCompFragIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
    LCompoundID          VW_Compound.CompoundID%Type;
BEGIN
    SELECT CF.CompoundID
        INTO LCompoundID
        FROM  VW_BatchComponentFragment BCF, VW_Compound_Fragment CF
        WHERE BCF.ID=ABatchCompFragIDToDelete AND BCF.CompOundFragmentID=CF.ID;

    DELETE VW_BatchComponentFragment WHERE ID=ABatchCompFragIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponentFragment".';

    DELETE VW_Compound_Fragment
        WHERE CompoundID=LCompoundID AND
              ID NOT IN (SELECT CompoundFragmentID
                            FROM VW_BatchComponentFragment BCF, VW_Compound_Fragment CF
                            WHERE BCF.CompOundFragmentID=CF.ID AND CF.CompoundID=LCompoundID  );

    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Fragment".';
END;

FUNCTION ValidateMixture(ARegIDs IN CLOB, ADuplicateCount OUT Number, AMixtureID IN Varchar2:='0',ACompoundIdsValueDeleting IN Varchar2:=NULL,AXmlTables XmlType) RETURN CLOB IS
   /*
            Autor: Fari
            Object: Validate Duplicated Mixture.
            Description: The ValidateMixture function validates if there are others Registries with the Compounds same.
                         If there are others Registries with the Compounds same then it returns a list with the RegNumbers duplicates for
                         than the application shows the regnumbers to the users. ValidateMixture also calls to ValidateCompoundFragment.
                         This function identifies if the compounds have the same fragments and if the fragments are equivalent.
    */
    LResult                CLOB;
    LComponentCount        NUMBER;
    LPos                   NUMBER;
    LPosOld                NUMBER;
    LQuery                 Varchar2(3000);
    LValidation            Varchar2(1000);
    LSameFragment          Varchar2(1000);
    LSameEquivalent        Varchar2(1000);
    LRegNumber             VW_RegistryNumber.RegNumber%type;

    LXMLCompound           XmlType;
    LXMLFragmentEquivalent XmlType;
    LRegID            Number;

    TYPE CursorType        IS REF CURSOR;
    C_RegNumbers           CursorType;

BEGIN

  --Duplicate Validation

   $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','Mixture ARegIDs to validate->'||ARegIDs||' ACompoundIdsValueDeleting->'||ACompoundIdsValueDeleting); $end null;

    LComponentCount:=1;
    LPosOld:=0;
    LOOP
        LPos:=NVL(INSTR(ARegIDs,',',LPosOld+1),0);
        $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture',' ARegIDs->'||ARegIDs||' LPos->'||LPos||' LPosOld->'||LPosOld); $end null;

    EXIT WHEN LPos=0;
        LPosOld:=LPos;
        LComponentCount:=LComponentCount+1;
    END LOOP;

    LQuery:='
       SELECT R.RegNumber
          FROM VW_Mixture M,VW_Mixture_Component MC,VW_Compound C,VW_RegistryNumber R
          WHERE  R.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND '||'M.MixtureID<>'||AMixtureID||' AND C.RegID IN ('||ARegIDs||') ';
    IF ACompoundIdsValueDeleting IS NOT NULL THEN
        LQuery:=LQuery||' AND C.CompoundID NOT IN ('||ACompoundIdsValueDeleting||') ';
    END IF;
    LQuery:=LQuery||' GROUP BY M.RegID, M.MixtureID,R.RegNumber
          HAVING COUNT(1) ='||LComponentCount||' AND (SELECT COUNT(1) FROM VW_Mixture_Component MC WHERE MC.MIXTUREID=M.MIXTUREID)='||LComponentCount;

    $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LQuery->'||LQuery); $end null;

    OPEN C_RegNumbers FOR LQuery;
    FETCH C_RegNumbers INTO LRegNumber;
    $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','C_RegNumbers%ROWCOUNT->'||C_RegNumbers%ROWCOUNT); $end null;

    IF  NOT C_RegNumbers%NOTFOUND THEN
        LSameFragment:='SAMEFRAGMENT="True"';
        LSameEquivalent:='SAMEEQUIVALENT="True"';

        LPosOld:=0;
        LOOP
            LPos:=NVL(INSTR(ARegIDs,',',LPosOld+1),0);

            IF LPos=0 THEN
                LRegID:=DBMS_LOB.SUBSTR(ARegIDs,Length(ARegIDs),LPosOld+1);
            ELSE
                LRegID:=DBMS_LOB.SUBSTR(ARegIDs,LPos-LPosOld-1,LPosOld+1);
            END IF;
            LPosOld:=LPos;
            $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LRegID->'||LRegID||' ARegIDs->'||ARegIDs); $end null;

            SELECT extract(AXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[CompoundID=/MultiCompoundRegistryRecord/ComponentList/Component/Compound[RegNumber/RegID='||LRegID||']/CompoundID]/BatchComponentFragmentList')
                INTO LXMLFragmentEquivalent
                FROM dual;

             $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LXMLFragmentEquivalent->'||LXMLFragmentEquivalent.Getclobval()); $end null;

            SELECT extract(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[Compound/RegNumber/RegID='||LRegID||']'),extractValue(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component/Compound[RegNumber/RegID='||LRegID||']/CompoundID')
              INTO LXMLCompound,LRegID
              FROM dual;

             $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LXMLCompound->'||LXMLCompound.Getclobval()); $end null;
             $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LRegID->'||LRegID); $end null;

            LValidation:=ValidateCompoundFragment(LRegID,LXMLCompound,LXMLFragmentEquivalent);
            IF INSTR(LValidation,'SAMEFRAGMENT="False"')<>0 THEN
                LSameFragment:='SAMEFRAGMENT="False"';
            END IF;
            IF INSTR(LValidation,'SAMEEQUIVALENT="False"')<>0 THEN
                LSameEquivalent:='SAMEEQUIVALENT="False"';
            END IF;
            EXIT WHEN LPos=0;
        END LOOP;

        LResult:='<REGISTRYLIST>';

        LOOP
            LResult:=LResult||'<REGNUMBER '||LSameFragment||' '||LSameEquivalent||'>'||LRegNumber||'</REGNUMBER>';
            FETCH C_RegNumbers INTO LRegNumber;
            EXIT WHEN C_RegNumbers%NOTFOUND;
        END LOOP;
        CLOSE C_RegNumbers;

        LResult:=LResult||'</REGISTRYLIST>';

    ELSE
        IF C_RegNumbers%ISOPEN THEN
            CLOSE C_RegNumbers;
        END IF;
        LResult:='';
    END IF;

    RETURN LResult;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF C_RegNumbers%ISOPEN THEN
            CLOSE C_RegNumbers;
        END IF;
        RAISE_APPLICATION_ERROR(eCompoundValidation, AppendError('Error validating the mixture.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

PROCEDURE ValidateIdentityBetweenBatches(AXmlTables IN XmlType) IS
    LIndex  Number;
    LIndex1 Number;
    LComponentIndex Number;
    LFragmentsIdsValue Varchar2(4000);
    LFragmentsIdsValueLast Varchar2(4000);
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','AXmlTables ->'||AXmlTables.getclobval()); $end null;
    IF GetSameBatchesIdentity='True' THEN
        LIndex:=0;
        LOOP
            LIndex:=LIndex+1;
            $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','LIndex ->'||LIndex); $end null;


            SELECT extractValue(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']/ComponentIndex')
              INTO LComponentIndex
              FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches', 'LComponentIndex ->' || LComponentIndex); $end null;

            LFragmentsIdsValueLast:=NULL;
        EXIT WHEN LComponentIndex IS NULL;

            LIndex1:=0;
            LOOP
                LIndex1:=LIndex1+1;
                $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','LIndex1 ->'||LIndex1); $end null;

                EXIT WHEN AXmlTables.ExistsNode('/MultiCompoundRegistryRecord/BatchList/Batch['||LIndex1||']/BatchComponentList/BatchComponent[ComponentIndex='||LComponentIndex||']')=0; --LFragmentsIdsValue IS NULL

                SELECT XmlTransform(extract(AXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch['||LIndex1||']/BatchComponentList/BatchComponent[ComponentIndex='||LComponentIndex||']/BatchComponentFragmentList/BatchComponentFragment/FragmentID'),XmlType.CreateXml('
                  <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                    <xsl:template match="/FragmentID">
                          <xsl:for-each select=".">
                              <xsl:value-of select="."/>,</xsl:for-each>
                    </xsl:template>
                  </xsl:stylesheet>')).GetClobVal()
                    INTO LFragmentsIdsValue
                    FROM dual;

                $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','LFragmentsIdsValue->'||LFragmentsIdsValue); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','LFragmentsIdsValueLast->'||LFragmentsIdsValueLast); $end null;

                IF LIndex1>1 THEN
                    IF NVL(LFragmentsIdsValueLast,0)<>NVL(LFragmentsIdsValue,0) THEN
                        RAISE_APPLICATION_ERROR(eSameIdentityBetweenBatches,
                          AppendError('Error validating the compound '||ABS(LComponentIndex)||'. The comopund should have the same identity of fragments between batches. (The "SameBatchesIdentity" flag is set in "true")'));
                    END IF;
                END IF;

                LFragmentsIdsValueLast:=LFragmentsIdsValue;

            END LOOP;

        END LOOP;

    END IF;
END;

FUNCTION ValidateFragment(ARegIDs IN CLOB, ADuplicateCount OUT Number, AMixtureID IN Varchar2:='0',ACompoundIdsValueDeleting IN Varchar2:=NULL) RETURN CLOB IS
        LqryCtx DBMS_XMLGEN.ctxHandle;
        LResult CLOB;
        LComponentCount NUMBER;
        LPos NUMBER;
        LPosOld NUMBER;
        LQuery Varchar2(3000);
BEGIN

  --Duplicate Validation

   $if CompoundRegistry.Debuging $then InsertLog('ValidateFragment','Mixture ARegIDs to validate->'||ARegIDs||' ACompoundIdsValueDeleting->'||ACompoundIdsValueDeleting); $end null;

    LComponentCount:=1;
    LPosOld:=0;
    LOOP
      LPos:=NVL(INSTR(ARegIDs,',',LPosOld+1),0);
      $if CompoundRegistry.Debuging $then InsertLog('ValidateFragment',' ARegIDs->'||ARegIDs||' LPos->'||LPos||' LPosOld->'||LPosOld); $end null;

    EXIT WHEN LPos=0;
      LPosOld:=LPos;
      LComponentCount:=LComponentCount+1;
    END LOOP;

    LQuery:='
       SELECT R.RegNumber
          FROM VW_Mixture M,VW_Mixture_Component MC,VW_Compound C,VW_RegistryNumber R
          WHERE  R.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND '||'M.MixtureID<>'||AMixtureID||' AND C.RegID IN ('||ARegIDs||') ';
    IF ACompoundIdsValueDeleting IS NOT NULL THEN
        LQuery:=LQuery||' AND C.CompoundID NOT IN ('||ACompoundIdsValueDeleting||') ';
    END IF;
    LQuery:=LQuery||' GROUP BY M.RegID, M.MixtureID,R.RegNumber
          HAVING COUNT(1) ='||LComponentCount||' AND (SELECT COUNT(1) FROM VW_Mixture_Component MC WHERE MC.MIXTUREID=M.MIXTUREID)='||LComponentCount;

    $if CompoundRegistry.Debuging $then InsertLog('ValidateFragment','LQuery->'||LQuery); $end null;

    LQryCtx := DBMS_XMLGEN.newContext(LQuery);
    DBMS_XMLGEN.setMaxRows(LqryCtx,3);
    DBMS_XMLGEN.setRowTag(LqryCtx, '');
    LResult := DBMS_XMLGEN.getXML(LqryCtx);
    ADuplicateCount:=DBMS_XMLGEN.getNumRowsProcessed(LqryCtx);
    $if CompoundRegistry.Debuging $then InsertLog('ValidateFragment','Mixture LDuplicateCount->'||ADuplicateCount||' LResult->'||LResult); $end null;
    DBMS_XMLGEN.closeContext(LqryCtx);
    IF ADuplicateCount>0 THEN
        LResult:=replace(LResult,cXmlDecoration,'');
        LResult:=replace(LResult,'ROWSET','REGISTRYLIST');
        RETURN LResult;
    ELSE
        RETURN '';
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eCompoundValidation, AppendError('Error validating the fragment.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
    RETURN '';
END;

FUNCTION  ValidateWildcardStructure(AStructureValue CLOB) RETURN Boolean IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LInternalID Integer;
BEGIN

    IF AStructureValue IS  NULL THEN
        RETURN True;
    END IF;

     $if CompoundRegistry.Debuging $then InsertLog('ValidateWildcardStructure','AStructureValue= '||AStructureValue); $end null;

    INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (AStructureValue, 0);

    SELECT  CPD_Internal_ID
        INTO LInternalID
        FROM  Structures
        WHERE cscartridge.moleculecontains(base64_cdx,'SELECT Query FROM CSCartridge.TempQueries WHERE ID = 0', '', 'IDENTITY=YES') =1 AND
              cpd_internal_id = -1;

     $if CompoundRegistry.Debuging $then InsertLog('ValidateWildcardStructure','LInternalID= '||LInternalID); $end null;

    COMMIT;

    IF LInternalID = -1 THEN
        RETURN False;
    ELSE
        RETURN True;
    END IF;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        BEGIN
            COMMIT;
            RETURN True;
        END;
    WHEN OTHERS THEN
        BEGIN
            --JED: WHY commit and then throw an exception?!?!?!
            --Indicative of bad design somewhere...
            COMMIT;
            $if CompoundRegistry.Debuging $then InsertLog('ValidateWildcardStructure','Error validating Wildcard Structure.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
            RAISE_APPLICATION_ERROR(eWildcardValidation, AppendError('Error validating Wildcard Structure.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
        END;
END;

PROCEDURE CanCreateMultiCompoundRegistry(AXml IN CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1) AS
        /*
            Autor: Fari
            Date: 17-Mar-09
            Object: Verify if a Registry is available to be inserted.
            Description: The process calls to the CreateMultiCompoundRegistry procedure and it only validates.
            Choice of ADuplicateCheck:
                ADuplicateCheck='C'  --> Structure Validation, Mixture Validation
                ADuplicateCheck='M'  --> Mixture Validation
            Choice of AMessage:
                AMessage=NULL        --> Validation OK, there isn't duplicated
                AMessage IS NOT NULL --> Validation failed, AMessage gets the duplicated
        */
    LRegNumber VW_RegistryNumber.RegNumber%type;
BEGIN

    IF Upper(ADuplicateCheck)='C' THEN
        CreateMultiCompoundRegistry(AXml, LRegNumber, AMessage, 'V');
    ELSIF Upper(ADuplicateCheck)='M' THEN
        CreateMultiCompoundRegistry(AXml, LRegNumber, AMessage, 'L');
    END IF;

END;

PROCEDURE CreateMultiCompoundRegistry(
  AXml IN CLOB
  , ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%type
  , AMessage OUT CLOB
  , ADuplicateCheck Char:='C'
  , ARegNumGeneration IN CHAR := 'Y'
  , AConfigurationID Number:=1
  , ASectionsList IN Varchar2:=NULL
) IS       /*
            Autor: Fari
            Date:07-Mar-07
            Object: Insert a new Registry
            Description: Look over a Xml searching each Table and insert the rows on it.
            Choice of ADuplicateCheck:
                ADuplicateCheck='C' --> Structure Validation, Mixture Validation and Registry Insert
                ADuplicateCheck='M' --> Mixture Validation and Registry Insert
                ADuplicateCheck='V' --> Structure Validation, Mixture Validation
                ADuplicateCheck='L' --> Mixture Validation
                ADuplicateCheck='N' or others  --> either Validation, Registry Insert, option to duplicate
            Choice of AMessage:
                AMessage=NULL        --> Validation OK, there isn't duplicated
                AMessage IS NOT NULL --> Validation failed, AMessage gets the duplicated
        */

    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlCompReg               CLOB;
    LXmlRows                  CLOB;
    LXmlTypeRows              XmlType;
    LXmlSequenceType          XmlSequenceType;

    LIndex                    Number:=0;
    LRowsInserted             Number:=0;
    LTableName                CLOB;
    LBriefMessage             CLOB;
    LMessage                  CLOB:='';
    LDuplicate                Varchar2(100);

    LStructureValue           CLOB;
    LRegNumberRegID           Number:=0;
    LDuplicatedCompoundID     Number;
    LDuplicatedStructures     CLOB;
    LListDulicatesCompound    CLOB;
    LDuplicateComponentCount  Number:=0;

    LStructuresList            CLOB;
    LStructuresToValidateList  CLOB;
    LFragmentXmlValue          CLOB;
    LFragmentXmlList           CLOB;
    LNormalizedStructureList   CLOB;
    LNormalizedStructureValue  CLOB;
    LStructureAggregationList  CLOB;
    LStructureAggregationValue CLOB;
    LXMLRegistryRecord         CLOB;

    LXMLCompound                  XmlType;
    LXMLFragmentEquivalent        XmlType;
    LXMLRegNumberDuplicated       XmlType;

    LRegDBIdsValue         Varchar2(4000);
    LDuplicatedMixtureCount   Number;
    LDuplicatedMixtureRegIds  Varchar2(4000);
    LDuplicatedAuxStructureID Number:=0;

    LRegID                      Number:=0;
    LBatchID                    Number:=0;
    LCompoundID                 Number:=0;
    LFragmentID                 Number:=0;
    LStructureID                Number:=0;
    LMixtureID                  Number:=0;
    LBatchNumber                Number:=0;
    LMixtureComponentID         Number:=0;
    LBatchComponentID           Number:=0;
    LCompoundFragmentID         Number:=0;
    LRegNumber                  VW_REGISTRYNUMBER.RegNumber%Type;
    LSequenceNumber             VW_REGISTRYNUMBER.SequenceNumber%Type;
    LFullRegNumber              VW_BATCH.FullRegNumber%Type;


    LSequenceID                   Number:=0;
    LProcessingMixture            Varchar2(1);

    LRegIDAux                      Number:=0;
    LExistentRegID                 Number:=0;
    LExistentComponentIndex        Number:=0;

    LXslTables XmlType := cXslCreateMCRR;

    PROCEDURE ValidateRegNumber(ARegNumber VW_RegistryNumber.RegNumber%type, ASequenceID VW_SEQUENCE.SequenceId%Type, AProcessingMixture IN VARCHAR2, ASequenceNumber IN OUT VW_REGISTRYNUMBER.SequenceNumber%Type) IS
        LRegNumber        VW_RegistryNumber.RegNumber%type;
        LFullRegNumber    VW_BATCH.FullRegNumber%Type;
        LSequenceNumber   VW_REGISTRYNUMBER.SequenceNumber%Type;
        LRegNumberLength  VW_SEQUENCE.RegNumberLength%Type;
    BEGIN
        SELECT RegNumberLength
            INTO LRegNumberLength
            FROM VW_SEQUENCE
            WHERE SequenceID=ASequenceID;

        IF AProcessingMixture='Y' THEN
            LRegNumber:=GetRegNumber(ASequenceID,LSequenceNumber,NULL,'N');
            IF SUBSTR(LRegNumber,1,LENGTH(LRegNumber)-LRegNumberLength)<>NVL(SUBSTR(ARegNumber,1,LENGTH(ARegNumber)-LRegNumberLength),' ') THEN
                RAISE_APPLICATION_ERROR(eCompoundValidation,
                  AppendError('Registry Number is incorrect: "'||ARegNumber||'"'));
            END IF;
            IF NVL(ASequenceNumber,0) = 0 THEN
                ASequenceNumber:=SUBSTR(ARegNumber,-LRegNumberLength,LRegNumberLength);
            END IF;
        END IF;

    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            $if CompoundRegistry.Debuging $then InsertLog('ValidateRegNumber', 'Error validating the Registry Number.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
            RAISE_APPLICATION_ERROR(eRegNumberValidation, AppendError('Error validating the Registry Number.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
        END;
    END;

BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Begin ADuplicateCheck='||ADuplicateCheck); $end null;
    SetSessionParameter;

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','AXml= '||AXml); $end null;
    LXmlCompReg:=AXml;

    -- Take Out the Structures because XmlType don't suport > 64k.
    LStructuresList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<BaseFragment>',NULL,FALSE);
    LStructuresToValidateList:=LStructuresList;
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure','<Structure>');
    LStructureAggregationList:=TakeOffAndGetClobsList(LXmlCompReg,'<StructureAggregation');

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LStructuresListt= '||LStructuresList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LNormalizedStructureList= '||LNormalizedStructureList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LStructureAggregationList= '||LStructureAggregationList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXmlCompReg sin Structures= '||LXmlCompReg); $end null;

    -- Get the xml
    LXmlTables:=XmlType.createXML(LXmlCompReg);
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Before ValidateIdentityBetweenBatches'); $end null;

    ValidateIdentityBetweenBatches(LXmlTables);

    AMessage:=NULL;

    IF UPPER(GetDuplicateCheckEnable)='TRUE' THEN
        IF Upper(ADuplicateCheck)='C' OR Upper(ADuplicateCheck)='V'THEN
            --Validate Components Strcuture
            LIndex:=0;
            LOOP
                LIndex:=LIndex+1;
                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LIndex ->'||LIndex); $end null;

                SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']')--.getClobVal()
                  INTO LXMLCompound
                  FROM dual;

            EXIT WHEN LXMLCompound IS NULL;
                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXMLCompound ->'||LXMLCompound.getClobVal()); $end null;

                SELECT extractValue(LXMLCompound,'/Component/Compound/RegNumber/RegID'),extractValue(LXMLCompound,'/Component/Compound/BaseFragment/Structure/StructureID')
                    INTO LRegNumberRegID,LDuplicatedAuxStructureID
                    FROM dual;
                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegNumberRegID ->'||LRegNumberRegID); $end null;

                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Valdiation-LStructuresList='||LStructuresList); $end null;
                LStructureValue:=TakeOffAndGetClob(LStructuresToValidateList,'Clob');
                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Valdiation-LStructureValue= '|| LStructureValue ); $end null;

                IF NVL(LRegNumberRegID,0) = 0 AND NVL(LDuplicatedAuxStructureID,0)>=0 THEN -- (LDuplicatedAuxStructureID <> -1 , -2 , -3)
                    IF ValidateWildcardStructure(LStructureValue) THEN
                        SELECT extractValue(LXMLCompound,'/Component/Compound/CompoundID'),extractValue(LXMLCompound,'/Component/ComponentIndex')
                            INTO LDuplicatedCompoundID,LExistentComponentIndex
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LDuplicatedCompoundID ->'||LDuplicatedCompoundID); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LExistentComponentIndex ->'||LExistentComponentIndex); $end null;

                        IF LDuplicatedCompoundID IS NOT NULL THEN

                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Before ValidateCompoundMulti'); $end null;
                            SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex='||LExistentComponentIndex||']/BatchComponentFragmentList')--.getClobVal()
                                INTO LXMLFragmentEquivalent
                                FROM dual;
                            $if CompoundRegistry.Debuging $then IF LXMLFragmentEquivalent IS NOT NULL THEN InsertLog('CreateMultiCompoundRegistry', 'LXMLFragmentEquivalent->'||LXMLFragmentEquivalent.getClobVal()); END IF; $end null;
                            LDuplicatedStructures:=ValidateCompoundMulti(LStructureValue, NULL, AConfigurationID,LXMLCompound,LXMLFragmentEquivalent);
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LDuplicatedStructures->'||LDuplicatedStructures); $end null;
                            IF LDuplicatedStructures IS NOT NULL AND LDuplicatedStructures<>'<REGISTRYLIST></REGISTRYLIST>'THEN
                                SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']/Compound/CompoundID')
                                  INTO LDuplicatedCompoundID
                                  FROM dual;
                                LListDulicatesCompound:=LListDulicatesCompound||'<COMPOUND>'||'<TEMPCOMPOUNDID>'||LDuplicatedCompoundID||'</TEMPCOMPOUNDID>'||LDuplicatedStructures||'</COMPOUND>';
                                LDuplicateComponentCount:=LDuplicateComponentCount+1;
                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LDuplicateComponentCount->'||LDuplicateComponentCount); $end null;
                            END IF;
                        END IF;
                    END IF;
                END IF;
            END LOOP;
            IF LListDulicatesCompound IS NOT NULL THEN
                LListDulicatesCompound:='<COMPOUNDLIST>'||LListDulicatesCompound||'</COMPOUNDLIST>';
                IF LDuplicateComponentCount=1 THEN
                    BEGIN
                        AMessage := CreateRegistrationResponse('1 duplicated component.', LListDulicatesCompound, NULL);
                    END;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end NULL;
                    RETURN;
                ELSE
                    BEGIN
                        AMessage := CreateRegistrationResponse(to_char(LDuplicateComponentCount) || ' duplicated components.', LListDulicatesCompound, NULL);
                    END;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end NULL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF (Upper(ADuplicateCheck)='M') OR (Upper(ADuplicateCheck)='C') OR (Upper(ADuplicateCheck)='V') OR (Upper(ADuplicateCheck)='L')THEN

            SELECT XmlTransform(extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component/Compound/RegNumber/RegID'),XmlType.CreateXml('
                  <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                    <xsl:template match="/RegID">
                          <xsl:for-each select=".">
                              <xsl:value-of select="."/>,</xsl:for-each>
                    </xsl:template>
                  </xsl:stylesheet>')).GetClobVal()
                INTO LRegDBIdsValue
                FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegDBIdsValue->'||LRegDBIdsValue); $end null;
            LRegDBIdsValue:=SUBSTR(LRegDBIdsValue,1,Length(LRegDBIdsValue)-1);

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegDBIdsValue->'||LRegDBIdsValue); $end null;

            LDuplicatedMixtureRegIds:=ValidateMixture(LRegDBIdsValue,LDuplicatedMixtureCount,'0',null,LXmlTables);

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LDuplicatedMixtureCount->'||LDuplicatedMixtureCount||' LDuplicatedMixtureRegIds->'||LDuplicatedMixtureRegIds); $end null;

            IF LDuplicatedMixtureRegIds IS NOT NULL THEN
                IF LDuplicatedMixtureCount>1 THEN
                    AMessage := CreateRegistrationResponse(to_char( LDuplicatedMixtureCount ) || ' duplicated mixtures.', LDuplicatedMixtureRegIds, NULL);
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end NULL;
                    RETURN;
                ELSE
                    AMessage := CreateRegistrationResponse('1 duplicated mixture.', LDuplicatedMixtureRegIds, NULL);

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end NULL;
                    RETURN;
                END IF;
            END IF;

        END IF;
    END IF;

    IF (Upper(ADuplicateCheck)='V') OR (Upper(ADuplicateCheck)='L') THEN
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'RETURN ADuplicateCheck='||ADuplicateCheck||' AMessage->'||AMessage); $end null;
        RETURN;
    END IF;

    LBriefMessage := 'Compound Validation OK';
    LMessage := LMessage || LBriefMessage ||CHR(13);

    -- Build a new formatted Xml
    LXslTablesTransformed:=LXmlTables.Transform(LXslTables);

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXslTablesTransformed= '||LXslTablesTransformed.getClobVal()); $end null;

    --Look over Xml searching each Table and insert the rows of it.

    SELECT XMLSequence(LXslTablesTransformed.Extract('/node()'))
      INTO LXmlSequenceType
      FROM DUAL;

    LStructureValue:='';
    LProcessingMixture:='Y';

    FOR LIndex IN LXmlSequenceType.FIRST..LXmlSequenceType.LAST LOOP

        --Search each Table
        LXmlTypeRows:=LXmlSequenceType(LIndex);
        LTableName:= LXmlTypeRows.GetRootElement();

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LTableName='||LTableName||' LXmlTypeRows='||LXmlTypeRows.getclobval()); $end null;

        --Build Message Logs
        LMessage := LMessage || chr(10) || 'Processing ' || LTableName || ': ';

        --Customization for each View - Use of Sequences
        CASE UPPER(LTableName)
            WHEN 'VW_STRUCTURE' THEN
                BEGIN
                    LStructureID:= LXmlTypeRows.extract('VW_Structure/ROW/STRUCTUREID/text()').getNumberVal();

                    IF NVL(LStructureID,0)=0 THEN
                        SELECT MOLID_SEQ.NEXTVAL INTO LStructureID FROM DUAL;

                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_STRUCTURE LStructuresList='||LStructuresList); $end null;
                        LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_STRUCTURE LStructureValue= '|| LStructureValue ); $end null;

                        SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/STRUCTUREID/text()',LStructureID)
                            INTO LXmlTypeRows
                            FROM dual;

                        InsertData(LTableName,LXmlTypeRows.getClobVal(),LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);

                        IF Upper(ADuplicateCheck)='N' OR ( NOT (Upper(ADuplicateCheck)='N') AND RegistrationRLS.GetStateRLS ) THEN
                            IF NVL(LRegIDAux,0) = 0 THEN
                                IF ValidateWildcardStructure(LStructureValue) THEN
                                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegNumber= '|| LRegNumber ); $end null;
                                    VerifyAndAddDuplicateToSave(LRegNumber,LStructureValue, NULL,LXMLRegNumberDuplicated);
                                END IF;
                            END IF;
                        END IF;

                    END IF;
                END;
            WHEN 'VW_REGISTRYNUMBER' THEN
                BEGIN
                    IF LProcessingMixture='Y' THEN
                        LRegID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGID/text()').getNumberVal();
                        IF LRegID=0 THEN

                            SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;

                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegID:'||'->'||LRegID); $end null;

                            IF ARegNumGeneration = 'Y' THEN
                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','ARegNumGeneration:'||'->'||ARegNumGeneration); $end null;
                                LRegNumber:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGNUMBER/text()').GetStringVal();
                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegNumber'||'->'||LRegNumber); $end null;

                                LSequenceID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();
                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LSequenceID'||'->'||LSequenceID); $end null;
                                IF LRegNumber='null' THEN
                                    IF LSequenceID IS NOT NULL THEN
                                      LRegNumber:=GetRegNumber(LSequenceID,LSequenceNumber,LXmlTables);
                                    END IF;
                                ELSE
                                    LSequenceNumber:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCENUMBER/text()').GetStringVal();
                                    ValidateRegNumber(LRegNumber,LSequenceID,LProcessingMixture,LSequenceNumber);
                                END IF;

                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegNumber'||'->'||LRegNumber); $end null;
                                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()'     ,LRegID
                                                                 ,'/node()/ROW/REGNUMBER/text()'  ,LRegNumber
                                                                 ,'/node()/ROW/SEQUENCENUMBER/text()',LSequenceNumber
                                                                 ,'/node()/ROW/DATECREATED/text()' ,TO_CHAR(SYSDATE))
                                        INTO LXmlTypeRows
                                        FROM dual;

                                IF ARegNumber IS NULL THEN  --The first regid
                                  ARegNumber:=LRegNumber;
                                END IF;
                            ELSE
                                LRegNumber:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGNUMBER/text()').GetStringVal();
                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','ARegNumGeneration='||ARegNumGeneration||' LRegNumber'||'->'||LRegNumber); $end null;

                                LSequenceID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();
                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LSequenceID'||'->'||LSequenceID); $end null;

                                ValidateRegNumber(LRegNumber,LSequenceID,LProcessingMixture,LSequenceNumber);

                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()',LRegID)
                                    INTO LXmlTypeRows
                                    FROM dual;

                                IF ARegNumber IS NULL THEN  --The first regid
                                  ARegNumber:=LRegNumber;
                                END IF;
                            END IF;

                            LXmlRows:=LXmlTypeRows.getClobVal;
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXmlRows'||'->'||LXmlRows); $end null;

                            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                        END IF;
                    ELSE

                        LRegIDAux:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGID/text()').getNumberVal();

                        IF LRegIDAux=0 THEN

                            SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegID'||'->'||LRegID); $end null;
                            LSequenceID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LSequenceID'||'->'||LSequenceID); $end null;

                            IF LSequenceID IS NOT NULL THEN
                              LRegNumber:=GetRegNumber(LSequenceID,LSequenceNumber,LXmlTables);
                            END IF;

                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegNumber'||'->'||LRegNumber); $end null;
                            SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()'  ,LRegID
                                                         ,'/node()/ROW/REGNUMBER/text()'  ,LRegNumber
                                                         ,'/node()/ROW/SEQUENCENUMBER/text()',LSequenceNumber
                                                         ,'/node()/ROW/DATECREATED/text()' ,TO_CHAR(SYSDATE))
                                    INTO LXmlTypeRows
                                    FROM dual;
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXmlRows'||'->'||LXmlRows); $end null;
                            IF ARegNumber IS NULL THEN  --The first regid
                              ARegNumber:=LRegNumber;
                            END IF;

                            InsertData(LTableName,LXmlTypeRows.getClobVal(),LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                        ELSE
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_REGISTRYNUMBER LRegIDAux->'||LRegIDAux); $end null;
                            SELECT CompoundID
                                INTO LCompoundID
                                FROM VW_Compound WHERE RegID=LRegIDAux;

                            LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LStructureValue= '|| LStructureValue ); $end null;

                        END IF;
                    END IF;
                END;
            WHEN 'VW_COMPOUND_IDENTIFIER' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()',LRegID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_BATCH' THEN
                BEGIN
                    SELECT SEQ_BATCHES.NEXTVAL INTO LBatchID FROM DUAL;

                    SELECT NVL(MAX(BatchNumber),0)+1
                        INTO LBatchNumber
                        FROM VW_Batch
                        WHERE REGID=LRegID;

                    LFullRegNumber:= LXmlTypeRows.extract('VW_Batch/ROW/FULLREGNUMBER/text()').GetStringVal();

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LFullRegNumber='||LFullRegNumber); $end null;
                    IF LFullRegNumber='null' THEN
                        LFullRegNumber:=ARegNumber;
                        IF LSequenceID IS NOT NULL THEN
                            LFullRegNumber:=GetFullRegNumber(LSequenceID,LXmlTables,ARegNumber,LBatchNumber, GetBatchNumberPadding());
                        END IF;
                    ELSE
                        NULL;
                        --LSequenceID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();
                        --$if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LSequenceID'||'->'||LSequenceID); $end null;

                        --ValidateRegNumber(LFullRegNumber,LSequenceID,'N');
                    END IF;


                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHID/text()'  ,LBatchID
                                                 ,'/node()/ROW/REGID/text()'  ,LRegID
                                                 ,'/node()/ROW/BATCHNUMBER/text()'  ,LBatchNumber
                                                 ,'/node()/ROW/FULLREGNUMBER/text()'  ,LFullRegNumber
                                                 ,'/node()/ROW/DATECREATED/text()' ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/DATELASTMODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_BATCH_PROJECT' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHID/text()', LBatchID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_BATCHIDENTIFIER' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHID/text()', LBatchID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_REGISTRYNUMBER_PROJECT' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()', LRegId)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_COMPOUND' THEN
                BEGIN
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry',' LStructureValue= '|| LStructureValue ); $end null;

                    SELECT SEQ_COMPOUND_MOLECULE.NEXTVAL INTO LCompoundID FROM DUAL;
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/COMPOUNDID/text()'  ,LCompoundID
                                                 ,'/node()/ROW/REGID/text()'  ,LRegID
                                                 ,'/node()/ROW/STRUCTUREID/text()'  ,LStructureID
                                                 ,'/node()/ROW/DATECREATED/text()' ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/DATELASTMODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_COMPOUND LXmlRows='||LXmlRows); $end null;

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_STRUCTURE LNormalizedStructureList='||LNormalizedStructureList); $end null;
                    LNormalizedStructureValue :=TakeOffAndGetClob(LNormalizedStructureList,'Clob');
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_STRUCTURE LNormalizedStructureValue = '|| LNormalizedStructureValue  ); $end null;

                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
             WHEN 'VW_FRAGMENT' THEN
                BEGIN
                    LFragmentID:= LXmlTypeRows.extract('VW_Fragment/ROW/FRAGMENTID/text()').getNumberVal();
                END;
              WHEN 'VW_COMPOUND_FRAGMENT' THEN
                BEGIN
                    SELECT Min(ID)
                        INTO LCompoundFragmentID
                        FROM VW_Compound_Fragment
                        WHERE FragmentID=LFragmentID AND CompoundID=LCompoundID;
                    IF NVL(LCompoundFragmentID,0)=0 THEN
                        SELECT SEQ_COMPOUND_FRAGMENT.NEXTVAL INTO LCompoundFragmentID FROM DUAL;

                        SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/ID/text()'  ,LCompoundFragmentID
                                                     ,'/node()/ROW/FRAGMENTID/text()'  ,LFragmentID
                                                     ,'/node()/ROW/COMPOUNDID/text()'  ,LCompoundID)
                            INTO LXmlTypeRows
                            FROM dual;
                        LXmlRows:=LXmlTypeRows.getClobVal();
                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_COMPOUND_FRAGMENT LXmlRows='||LXmlRows); $end null;
                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    END IF;
                END;
              WHEN 'VW_MIXTURE' THEN
                BEGIN
                    SELECT SEQ_MIXTURE.NEXTVAL INTO LMixtureID FROM DUAL;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_MIXTURE LRegID->'||LRegID); $end null;

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_MIXTURE LStructureAggregationList='||LStructureAggregationList); $end null;
                    LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LStructureAggregationValue= '||LStructureAggregationValue); $end null;

                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/MIXTUREID/text()'  ,LMixtureID
                                                 ,'/node()/ROW/REGID/text()'  ,LRegID
                                                 ,'/node()/ROW/CREATED/text()' ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/MODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);

                    LProcessingMixture:='N';
                END;
              WHEN 'VW_MIXTURE_COMPONENT' THEN
                BEGIN
                    SELECT SEQ_MIXTURE_COMPONENT.NEXTVAL INTO LMixtureComponentID FROM DUAL;
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/MIXTURECOMPONENTID/text()'  ,LMixtureComponentID
                                                 ,'/node()/ROW/MIXTUREID/text()'  ,LMixtureID
                                                 ,'/node()/ROW/COMPOUNDID/text()'  ,LCompoundID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_BATCHCOMPONENT' THEN
                BEGIN
                    SELECT SEQ_BATCHCOMPONENT.NEXTVAL INTO LBatchComponentID FROM DUAL;
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/ID/text()',LBatchComponentID
                                                 ,'/node()/ROW/MIXTURECOMPONENTID/text()',LMixtureComponentID
                                                 ,'/node()/ROW/BATCHID/text()'  ,LBatchId)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_BATCHCOMPONENTFRAGMENT' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHCOMPONENTID/text()',LBatchComponentID
                                                 ,'/node()/ROW/COMPOUNDFRAGMENTID/text()', LCompoundFragmentID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;

              ELSE  LMessage := LMessage || ' "' || LTableName || '" table stranger.' ||chr(13);

        END CASE;

    END LOOP;

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Fin ARegNumber='||ARegNumber); $end null;
    IF ARegNumber IS NOT NULL THEN

        OnRegistrationInsert(ARegNumber);

        IF LXMLRegNumberDuplicated IS NOT NULL THEN
            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'LXMLRegNumberDuplicated->'||LXMLRegNumberDuplicated.getClobVal()); $end NULL;
            IF LXslTablesTransformed.ExistsNode('VW_RegistryNumber[1]/ROW/PERSONREGISTERED/text()')=1 THEN
                SaveRegNumbersDuplicated(LXMLRegNumberDuplicated,LXslTablesTransformed.extract('VW_RegistryNumber[1]/ROW/PERSONREGISTERED/text()').getStringVal());
            ELSE
                SaveRegNumbersDuplicated(LXMLRegNumberDuplicated,NULL);
            END IF;
        END IF;

        IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'Before RetrieveMultiCompoundRegistry'); $end NULL;
            RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord);
            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'LXMLRegistryRecord->'||LXMLRegistryRecord); $end NULL;
        END IF;

        AMessage := CreateRegistrationResponse(LBriefMessage, NULL, LXMLRegistryRecord);

    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LMessage := LMessage || ' ' || cast(0 as string) || ' Row/s Inserted on "' || LTableName || '".';
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eCreateMultiCompoundRegistry, AppendError('CreateMultiCompoundRegistry', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

PROCEDURE RetrieveMultiCompoundRegistry(
  ARegNumber in VW_RegistryNumber.RegNumber%type
  , AXml out NOCOPY clob
  , ASectionsList in Varchar2 := NULL
) IS

    LQryCtx                   DBMS_XMLGEN.ctxHandle;
    LResult                   CLOB;
    LXml                      CLOB;
    LXmlTemp                  CLOB;

    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlResult                XmlType;
    LMessage                  CLOB:='';
    LStructuresList           CLOB;
    LNormalizedStructureList  CLOB;
    LStructureAggregationList CLOB;

    LRegID                    reg_numbers.reg_id%type;
    LCompundRegID             reg_numbers.reg_id%type;


    LMixtureFields            CLOB;
    LCompoundFields           CLOB;
    LCompoundPickListFields   CLOB;
    LBatchFields              CLOB;
    LBatchComponentFields     CLOB;

    LCoeObjectConfigField     XmlType;

    CURSOR C_Batch(LRegID reg_numbers.reg_id%type)  IS
      SELECT BatchID FROM VW_Batch WHERE RegID=LRegID ORDER BY BatchNumber;

    CURSOR C_CompoundRegIDs(ARegID in VW_RegistryNumber.regid%type) IS
      SELECT C.RegID
        FROM VW_Mixture M,VW_Mixture_Component MC, VW_Compound C
        WHERE M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND M.RegID=ARegID ORDER BY C.RegID;

    CURSOR C_BatchComponentIDs(ABatchID BatchComponent.batchid%Type) IS
      SELECT P.ID
        FROM VW_BatchComponent P
       WHERE P.BatchID=ABatchID
       ORDER BY P.OrderIndex;

    CURSOR C_BatchComponentFragmentIDs(ABatchComponetID BatchComponentFragment.BatchComponentID%Type) IS
      SELECT BCF.ID
        FROM VW_BatchComponentFragment BCF
       WHERE BCF.BatchComponentID=ABatchComponetID
       ORDER BY BCF.OrderIndex;

    LXslTables XmlType := cXslRetrieveMCRR;

    PROCEDURE AddNullFields(AFields IN CLOB,AXml IN OUT NOCOPY CLOB) IS
        LPosBegin                 NUMBER;
        LPoslast                  NUMBER;
        LField                    VARCHAR2(30);
        LFields                   CLOB;
        LPosField                 NUMBER;
    BEGIN
        LFields:=AFields||',';
        LPosBegin:=0;
        LPoslast:=1;
        LOOP
            LPosBegin:=INSTR(LFields,',',LPoslast);
            LField:=UPPER(SUBSTR(LFields,LPoslast,LPosBegin-LPoslast));
            LPoslast:=LPosBegin+1;
            EXIT WHEN LField IS NULL;
                LPosField:=INSTR(AXml, '<'||LField);
                IF LPosField=0 THEN
                    AXml:=REPLACE(AXml,'</ROW>',' <'||LField||'/>'||CHR(10)||' </ROW>');
                END IF;
        END LOOP;
    END;


    PROCEDURE CompundProcess(ARegID Number) IS
    BEGIN
        --**Get Compound**
        IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Compound'), 0) > 0 ) THEN
            LQryCtx:=DBMS_XMLGEN.newContext(
            'SELECT C.CompoundID,C.DateCreated,C.PersonCreated,C.PersonRegistered,C.DateLastModified,NVL(C.FormulaWeight,csCartridge.MolWeight(S.Structure)) FormulaWeight,
                    NVL(C.MolecularFormula,csCartridge.Formula(S.Structure,'''')) MolecularFormula,
                    R.RegID,R.SequenceNumber,R.RegNumber,R.SequenceID,
                    S.StructureID,S.StructureFormat,S.Structure,-C.CompoundID COMPONENTINDEX,C.NormalizedStructure,C.UseNormalization,C.Tag
               FROM VW_Compound C,VW_RegistryNumber R,VW_Structure S
               WHERE C.RegID(+)=R.RegID AND S.StructureID(+)=C.StructureID AND C.RegID='||ARegID );

            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
            DBMS_XMLGEN.closeContext(LQryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('CompundProcess','ARegID='||ARegID||' Select Compound LXml:'|| chr(10)||LXml); $end null;

            IF LXml IS NULL THEN
                RAISE_APPLICATION_ERROR(eNoRowsReturned, AppendError('No rows returned.'));
            END IF;

            LNormalizedStructureList:=LNormalizedStructureList||TakeOffAndGetClobslist(LXml,'<NORMALIZEDSTRUCTURE>',NULL,Null,FALSE);
            LNormalizedStructureList:='<ClobList>'||REPLACE(REPLACE(LNormalizedStructureList,'<ClobList>',''),'</ClobList>','')||'</ClobList>';

            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LNormalizedStructureList:'|| LNormalizedStructureList); $end null;

            LXml:=Replace(LXml,'</ROWSET>','');
            LXml:=Replace(LXml,'ROWSET','Compound');
            LXml:=Replace(LXml,'</ROW>','');

            LResult:=LResult||LXml;

            --**Get the PropertyList Fields from the XML field
            SELECT
                XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                    <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                        <xsl:template match="/MultiCompoundRegistryRecord">
                            <xsl:for-each select="ComponentList/Component/Compound/PropertyList/Property">
                                <xsl:value-of select="@name"/>,</xsl:for-each>
                        </xsl:template>
                    </xsl:stylesheet>')).GetStringVal(),
                XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                    <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                        <xsl:template match="/MultiCompoundRegistryRecord">
                            <xsl:for-each select="ComponentList/Component/Compound/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                        </xsl:template>
                    </xsl:stylesheet>')).GetStringVal()
                INTO LCompoundFields,LCompoundPickListFields
                FROM DUAL;

            IF LCompoundFields IS NOT NULL THEN
                --take out the last character  ,'
                LCompoundFields:=SUBSTR(LCompoundFields,1,LENGTH(LCompoundFields)-1);

                --**Get the Compound's property list **
                LQryCtx:=DBMS_XMLGEN.newContext(
                    'SELECT '||LCompoundFields||'
                        FROM VW_Compound C,VW_RegistryNumber R,VW_Structure S
                        WHERE C.RegID(+)=R.RegID AND S.StructureID(+)=C.StructureID AND C.RegID='||ARegID);
                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                DBMS_XMLGEN.closeContext(LQryCtx);
                 $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LCompoundFields:'|| LCompoundFields||' LXml:'|| LXml); $end null;
                 $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LCompoundPickListFields:'|| LCompoundPickListFields||' LXml:'|| LXml); $end null;

                AddAttribPickList(LCompoundPickListFields,LXml,'<ROW>');
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LXml:'|| LXml); $end null;
                LXml:=Replace(LXml,'ROWSET','PropertyList');
                --Add NULL fields
                AddNullFields(LCompoundFields,LXml);
                LResult:=LResult ||LXml;
            END IF;

            --**Get Fragment**
            IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Fragment'), 0) > 0 ) THEN
               LQryCtx:=DBMS_XMLGEN.newContext(
               'SELECT F.Fragmentid,F.Code,F.Description,F.FragmentTypeID,FT.Description TypeDescription,F.Molweight,F.Formula,F.Created,F.Modified,F.MolWeight,F.Formula,
                       F.StructureFormat,F.Structure,
                       CF.Equivalents,CF.ID
                  FROM VW_Fragment F, VW_Compound_Fragment CF, VW_Compound C,VW_FragmentType FT
                  WHERE F.FragmentID=CF.FragmentID AND C.CompoundID=CF.CompoundID AND FT.ID=F.FragmentTypeID AND C.RegID='||ARegID);

               LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
               DBMS_XMLGEN.closeContext(LQryCtx);
               $if CompoundRegistry.Debuging $then InsertLog('CompundProcess','Select Fragment:'|| chr(10)||LXml); $end null;
               LXml:=Replace(LXml,'ROWSET','Fragment');
               LResult:=LResult ||LXml;
            END IF;

            --**Get Compound_Identifier**
            IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Identifier'), 0) > 0 ) THEN
                LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT CI.ID,CI.Type,CI.Regid,CI.Value,I.Description Description,I.Name Name,I.Active
                   FROM VW_Compound_Identifier CI,VW_IdentifierType I
                   WHERE CI.Type=I.ID(+) AND CI.RegID='||ARegID||' ORDER BY CI.OrderIndex');

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                DBMS_XMLGEN.closeContext(LQryCtx);
                $if CompoundRegistry.Debuging $then InsertLog('CompundProcess','Select:'|| chr(10)||LXml); $end null;
                LXml:=Replace(LXml,'ROWSET','Identifier');
                LResult:=LResult ||LXml;
            END IF;

            LResult:=LResult || '</ROW></Compound>';
        END IF;
    END;
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','ARegNumber:'|| ARegNumber ||' ASectionsList:'||ASectionsList); $end null;
    SetSessionParameter;

    --Get Query or Get empty template xml
    IF (ARegNumber IS NOT NULL) THEN

        SELECT XmlType.CreateXml(XML)
          INTO LCoeObjectConfigField
          FROM COEOBJECTCONFIG
          WHERE ID=2;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LCoeObjectConfigField:'|| LCoeObjectConfigField.GetClobVal()); $end null;

        IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Mixture'), 0) > 1 ) THEN
            LResult:='<MultiCompoundRegistryRecord '||'SameBatchesIdentity="'||GetSameBatchesIdentity||'" ActiveRLS="'||GetActiveRLS||'" IsEditable="'||GetIsEditable(ARegNumber)||'" TypeRegistryRecord="Mixture">';

            LResult:=LResult||'<Mixture>';

            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LResult:'|| LResult); $end null;

            LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT M.MixtureID,M.Created,M.Modified,M.PersonCreated,
                        R.RegID,R.SequenceNumber,R.RegNumber,R.SequenceID,M.Approved,M.StructureAggregation
                   FROM VW_Mixture M,VW_RegistryNumber R
                   WHERE M.RegID=R.RegID AND R.RegNumber='''||ARegNumber||'''');

            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
            DBMS_XMLGEN.closeContext(LQryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Mixture LXml:'|| chr(10)||LXml); $end null;

            IF LXml IS NULL THEN
                IF RegistrationRLS.GetStateRLS THEN
                    RAISE_APPLICATION_ERROR(eInvalidRegNum,
                      AppendError('The Registry "'||ARegNumber||'" doesn''t exist or isen''t available.'));
                ELSE
                    RAISE_APPLICATION_ERROR(eInvalidRegNum,
                      AppendError('The Registry "'||ARegNumber||'" doesn''t exist.'));
                END IF;
            END IF;

            LStructureAggregationList:=TakeOffAndGetClobslist(LXml,'<STRUCTUREAGGREGATION>',NULL,Null,FALSE);

            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LStructureAggregationList:'|| LStructureAggregationList); $end null;

            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LXml Mixture SIN ESTRUCTURA:'|| LXml); $end null;

             SELECT  extractvalue(XmlType(LXml),'ROWSET/ROW/REGID')
                INTO  LRegID
                FROM  DUAL;

            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LRegID:'|| LRegID); $end null;

            LXml:=Replace(LXml,'</ROWSET>','');
            LXml:=Replace(LXml,'<ROWSET>','');
            LXml:=Replace(LXml,'<ROW>','');
            LXml:=Replace(LXml,'</ROW>','');
            LResult:=LResult ||LXml;

            --Get the PropertyList Fields from the XML field

            SELECT
                XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                    <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                      <xsl:template match="/MultiCompoundRegistryRecord">
                            <xsl:for-each select="PropertyList/Property">
                                <xsl:value-of select="@name"/>,</xsl:for-each>
                      </xsl:template>
                    </xsl:stylesheet>')).GetStringVal(),
                XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                    <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                        <xsl:template match="/MultiCompoundRegistryRecord">
                            <xsl:for-each select="PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                        </xsl:template>
                    </xsl:stylesheet>')).GetStringVal()
                INTO LMixtureFields,LCompoundPickListFields
                FROM DUAL;

            --Take out the last character (',')
            LMixtureFields:=SUBSTR(LMixtureFields,1,LENGTH(LMixtureFields)-1);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LMixtureFields Mixture :'|| LMixtureFields); $end null;
            IF (LMixtureFields IS NOT NULL) THEN
                --**Get the Compound's property list **

                LQryCtx:=DBMS_XMLGEN.newContext(
                   'SELECT '||LMixtureFields||'
                       FROM VW_Mixture M,VW_RegistryNumber R
                       WHERE M.RegID=R.RegID AND M.RegID='||LRegID);

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                DBMS_XMLGEN.closeContext(LQryCtx);
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LCompoundPickListFields:'|| LCompoundPickListFields); $end null;
                AddAttribPickList(LCompoundPickListFields,LXml,'<ROW>');
                LXml:=Replace(LXml,'ROWSET','PropertyList');
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select MixturePropertyList:'|| chr(10)||LXml); $end null;
                AddNullFields(LMixtureFields,LXml);
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','with AddNullFields'|| chr(10)||LXml); $end null;
                LResult:=LResult ||LXml;
            END IF;

            LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT CI.ID,CI.Type,CI.Regid,CI.Value,I.Description Description,I.Name Name,I.Active
                   FROM VW_Compound_Identifier CI,VW_IdentifierType I
                   WHERE CI.Type=I.ID(+) AND CI.RegID='||LRegID||' ORDER BY CI.OrderIndex');

            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
            DBMS_XMLGEN.closeContext(LQryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Mixture Identifier:'|| chr(10)||LXml); $end null;
            LXml:=Replace(LXml,'ROWSET','Identifier');
            LResult:=LResult ||LXml;

            -- Get and RegistryNumberProject record
            LQryCtx:=DBMS_XMLGEN.newContext(
            'SELECT RP.ID,RP.ProjectID , P.Description, P.Name, P.Active
               FROM VW_RegistryNumber_Project RP,VW_Project P
               WHERE RP.ProjectID=P.ProjectID AND RP.RegID='||LRegID||' ORDER BY RP.OrderIndex');

            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
            DBMS_XMLGEN.closeContext(LQryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Registry Project:'|| chr(10)||LXml); $end null;
            LXml:=Replace(LXml,'ROWSET','RegistryRecord_Project');

            LResult:=LResult ||LXml||'</Mixture>';
        ELSE
            LResult:='<MultiCompoundRegistryRecord TypeRegistryRecord="WithoutMixture">';
        END IF;

        IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Compound'), 0) > 0)  THEN
            IF  (ASectionsList IS NULL) OR INSTR(ASectionsList,'Mixture')<>0 THEN
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Into compound LRegID='||LRegID); $end null;
                FOR R_CompoundRegIDs IN C_CompoundRegIDs(LRegID) LOOP
                    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','BEFORE CompundProcess('||R_CompoundRegIDs.RegID||') LResult:'|| LResult); $end null;
                    CompundProcess(R_CompoundRegIDs.RegID);
                    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','AFTER CompundProcess('||R_CompoundRegIDs.RegID||') LResult:'|| LResult); $end null;
                END LOOP;
            ELSE
                SELECT RegID
                    INTO LCompundRegID
                    FROM VW_RegistryNumber
                    WHERE RegNumber=ARegNumber;
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LCompundRegID:'|| LCompundRegID); $end null;
                CompundProcess(LCompundRegID);
            END IF;
            LStructuresList:=TakeOffAndGetClobslist(LResult,'<STRUCTURE>',NULL,Null,FALSE);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LStructuresList:'|| LStructuresList); $end null;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LResult STRUCTURE SIN ESTRUCTURA:'|| LResult); $end null;
        END IF;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','**ASectionsList'|| chr(10)||ASectionsList); $end null;

        IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Batch'), 0) > 0 ) THEN
            LResult:=LResult||'<Batch>';
            --Get Batch
            FOR  R_Batch IN C_Batch(LRegID) LOOP
                LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT B.Batchid, B.FullRegNumber,B.Batchnumber,B.Datecreated, B.Personcreated, B.PersonRegistered, B.Datelastmodified,

                        P1.User_ID PersonCreatedDisplay,B.StatusID
                   FROM VW_Batch B,CS_SECURITY.People P1,CS_SECURITY.People P3
                   WHERE P1.Person_ID(+)=B.Personcreated AND P3.Person_ID(+)=B.PersonRegistered AND B.BatchID='||R_Batch.BatchID);

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                DBMS_XMLGEN.closeContext(LQryCtx);
                LXml:=Replace(LXml,'<ROWSET>','');
                LXml:=Replace(LXml,'</ROWSET>','');
                LXml:=Replace(LXml,'</ROW>','');
                LResult:=LResult ||LXml;

                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LCoeObjectConfigField.GetStringVal:'||LCoeObjectConfigField.GetClobVal()); $end null;
                --Get the PropertyList Fields from the XML field
                SELECT
                    XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                    <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                        <xsl:template match="/MultiCompoundRegistryRecord">
                            <xsl:for-each select="BatchList/Batch/PropertyList/Property">
                                <xsl:value-of select="@name"/>,</xsl:for-each>
                        </xsl:template>
                    </xsl:stylesheet>')).GetStringVal(),
                    XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                    <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                        <xsl:template match="/MultiCompoundRegistryRecord">
                            <xsl:for-each select="BatchList/Batch/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                        </xsl:template>
                    </xsl:stylesheet>')).GetStringVal()
                    INTO LBatchFields,LCompoundPickListFields
                    FROM COEOBJECTCONFIG
                    WHERE ID=2;

                IF LBatchFields IS NOT NULL THEN
                    --Take out the last character  ,'
                    LBatchFields:=SUBSTR(LBatchFields,1,LENGTH(LBatchFields)-1);

                    --Get and add Batch Property List record
                    LQryCtx:=DBMS_XMLGEN.newContext(
                    'SELECT '||LBatchFields|| '
                        FROM VW_Batch B
                        WHERE B.BatchID='||R_Batch.BatchID);

                    LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                    DBMS_XMLGEN.closeContext(LQryCtx);
                    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Custom:'|| chr(10)||LXml); $end null;
                    AddAttribPickList(LCompoundPickListFields,LXml,'<ROW>');
                    LXml:=Replace(LXml,'ROWSET','PropertyList');
                    AddNullFields(LBatchFields,LXml);
                    LResult:=LResult ||LXml;
                END IF;

                -- Get and add Batch Project record
                LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT BP.ID, BP.ProjectID , P.Description, P.Active, P.Name
                   FROM VW_Batch_Project BP,VW_Project P
                   WHERE BP.ProjectID=P.ProjectID AND BP.BatchID='||R_Batch.BatchID);

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                DBMS_XMLGEN.closeContext(LQryCtx);
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Project:'|| chr(10)||LXml); $end null;
                LXml:=Replace(LXml,'ROWSET','Batch_Project');
                LResult:=LResult ||LXml;

                -- Get and add Batch Identifier record
                LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT BI.ID, BI.Type , BI.Value, P.Description, P.Active, P.Name
                   FROM VW_BatchIdentifier BI,VW_IdentifierType P
                   WHERE BI.Type=P.ID AND BI.BatchID='||R_Batch.BatchID);

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                DBMS_XMLGEN.closeContext(LQryCtx);
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Identifier:'|| chr(10)||LXml); $end null;
                LXml:=Replace(LXml,'ROWSET','BatchIdentifier');
                LResult:=LResult ||LXml;

                --Get the PropertyList Fields from the XML field
                SELECT
                    XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                        <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                            <xsl:template match="/MultiCompoundRegistryRecord">
                                <xsl:for-each select="BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property">
                                    <xsl:value-of select="@name"/>,</xsl:for-each>
                            </xsl:template>
                        </xsl:stylesheet>')).GetStringVal(),
                    XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                        <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                            <xsl:template match="/MultiCompoundRegistryRecord">
                                <xsl:for-each select="BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                            </xsl:template>
                        </xsl:stylesheet>')).GetStringVal()
                    INTO LBatchComponentFields,LCompoundPickListFields
                    FROM COEOBJECTCONFIG
                    WHERE ID=2;
                LBatchComponentFields:=SUBSTR(LBatchComponentFields,1,LENGTH(LBatchComponentFields)-1);

                -- Get and add Batch Component record
                LResult:=LResult ||' <BatchComponent>' ;
                FOR R_BatchComponentIDs in C_BatchComponentIDs(R_Batch.BatchID) LOOP
                    LResult:=LResult ||' <ROW>' ;
                    LQryCtx:=DBMS_XMLGEN.newContext(
                    'SELECT P.ID,P.BatchID,M.CompoundID,M.MixtureComponentID,-M.CompoundID COMPONENTINDEX
                       FROM VW_BatchComponent P, VW_Mixture_Component M
                       WHERE P.MixtureComponentID=M.MixtureComponentID AND P.ID='||R_BatchComponentIDs.ID);

                    LXmlTemp:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                    DBMS_XMLGEN.closeContext(LQryCtx);
                    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Component:'|| chr(10)||LXmlTemp); $end null;
                    LXml:=TakeOffAndGetClob(LXmlTemp,'ROW');
                    LResult:=LResult ||LXml;

                    IF LBatchComponentFields IS NOT NULL THEN
                        --Get and add Batch Property List record
                        LQryCtx:=DBMS_XMLGEN.newContext(
                        'SELECT '||LBatchComponentFields|| '
                            FROM VW_BatchComponent B
                            WHERE B.ID='||R_BatchComponentIDs.ID);
                        LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                        DBMS_XMLGEN.closeContext(LQryCtx);
                        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Component Custom:'|| chr(10)||LXml); $end null;
                        AddAttribPickList(LCompoundPickListFields,LXml,'<ROW>');
                        LXml:=Replace(LXml,'ROWSET','PropertyList');
                        AddNullFields(LBatchComponentFields,LXml);
                        LResult:=LResult ||LXml;
                    END IF;

                    LResult:=LResult ||' <BatchComponentFragment>' ;
                    FOR R_BatchComponentFragmentIDs in C_BatchComponentFragmentIDs(R_BatchComponentIDs.ID) LOOP
                        LResult:=LResult ||' <ROW>' ;
                        LQryCtx:=DBMS_XMLGEN.newContext(
                        'SELECT CF.FragmentID,BCF.EQUIVALENT,BCF.ID
                           FROM VW_BatchComponentFragment BCF, VW_COMPOUND_FRAGMENT CF
                           WHERE  BCF.COMPOUNDFRAGMENTID=CF.ID AND BCF.ID='||R_BatchComponentFragmentIDs.ID);

                        LXmlTemp:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
                        DBMS_XMLGEN.closeContext(LQryCtx);
                        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Component Fragment:'|| chr(10)||LXmlTemp); $end null;
                        LXml:=TakeOffAndGetClob(LXmlTemp,'ROW');
                        LResult:=LResult ||LXml;

                        LResult:=LResult ||' </ROW>' ;
                    END LOOP;
                    LResult:=LResult ||' </BatchComponentFragment>' ;

                    LResult:=LResult ||' </ROW>' ;
                END LOOP;
                LResult:=LResult ||' </BatchComponent>' ;

                LResult:=LResult ||' </ROW>' ;
             END LOOP;
             LResult:=LResult||'</Batch>';
        END IF;

        LResult:=LResult || '</MultiCompoundRegistryRecord>';

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','MultiCompoundRegistryRecord SIN TRANSFORMACION:'|| chr(10)||LResult); $end null;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','before LXmlTables'); $end null;

        LXmlTables:=XmlType.CreateXml(LResult);

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','After LXmlTables'); $end null;

        --Build a new formatted Xml
        SELECT XmlTransform(LXmlTables,LXslTables).GetClobVal()
            INTO AXml
            FROM DUAL;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','After XmlTransform'); $end null;

        --Replace '&lt;' and '&lt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        AXml:=replace(replace(replace(AXml,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>'),'&quot;','"');

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','AXml:'|| chr(10)||AXml); $end null;
        LXmlResult:=XmlType(AXml);
        AddTags(LCoeObjectConfigField,LXmlResult,'AddIns',Null);
        AddTags(LCoeObjectConfigField,LXmlResult,'ValidationRuleList','name');

        AXml:= TakeOnAndGetXml(LXmlResult.GetClobVal(),'STRUCTURE',LStructuresList);
        AXml:= TakeOnAndGetXml(AXml,'STRUCTUREAGGREGATION',LStructureAggregationList);
        AXml:= TakeOnAndGetXml(AXml,'NORMALIZEDSTRUCTURE',LNormalizedStructureList);

    ELSE
        --Validate xml template with the CreateXml object and get it.
        SELECT XmlType.CreateXml(XML).GetClobVal()
            INTO AXml
            FROM COEOBJECTCONFIG
            WHERE ID=2;

         AXml:=
          '<MultiCompoundRegistryRecord SameBatchesIdentity="' || GetSameBatchesIdentity || '" ActiveRLS="'||GetActiveRLS||'" '
          ||Substr(AXml,29,Length(AXml));
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','RetrieveMultiCompoundRegistry:'|| chr(10)||AXml); $end null;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegistry, AppendError('RetrieveMultiCompoundRegistry', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

PROCEDURE UpdateMultiCompoundRegistry(
  AXml in CLOB
  , AMessage OUT CLOB
  , ADuplicateCheck Char:='C'
  , AConfigurationID Number:=1
  , ASectionsList Varchar2 := NULL
) IS
    /*
         Autor: Fari
         Date:07-Mar-07
         Object: Insert a single compound record
         Description: Look over a Xml searching each Table and insert the rows on it.
         Pending.
             Use of setUpdateColumn;
             Optimize use INSTR with XSLT or REGEXP_INSTR
             Optimize use SUBSTR with XSLT or REGEXPR_SUBSTR
             Optimize repase of'&lt;' and &gt;'
             Optimize XSLT
     */

    LinsCtx                     DBMS_XMLSTORE.ctxType;
    LXmlTables                  XmlType;
    LXslTablesTransformed       XmlType;
    LXmlCompReg                 CLOB;
    LXmlRows                    CLOB;
    LXmlTypeRows                XmlType;
    LFieldToUpdate              CLOB;
    LStructureUpdating          CLOB;
    LRowsUpdated                Number:=0;
    LRowsProcessed              Number:=0;

    LIndex                      Number:=0;
    LIndexField                 Number:=0;
    LRowsInserted               Number:=0;
    LTableName                  CLOB;
    LFieldName                  CLOB;
    LBriefMessage               CLOB;
    LMessage                    CLOB:='';
    LUpdate                     boolean;
    LSomeUpdate                 boolean:=FALSE;
    LSectionInsert              boolean;
    LSectionDelete              boolean;

    LRegID                      Number:=0;
    LRegIDTag CONSTANT          VARCHAR2(10):='<REGID>';
    LRegIDTagEnd CONSTANT       VARCHAR2(10):='</REGID>';

    LBatchID                    Number:=0;
    LBatchIDTag CONSTANT        VARCHAR2(10):='<BATCHID>';
    LBatchIDTagEnd CONSTANT     VARCHAR2(10):='</BATCHID>';

    LCompoundID                 Number:=0;
    LCompoundIDAux              Number:=0;
    LCompoundIDTemp             Number:=0;
    LCompoundIDTag CONSTANT     VARCHAR2(15):='<COMPOUNDID>';
    LCompoundIDTagEnd CONSTANT  VARCHAR2(15):='</COMPOUNDID>';

    LFragmentID                 Number:=0;
    LFragmentIDTag CONSTANT     VARCHAR2(15):='<FRAGMENTID>';
    LFragmentIDTagEnd CONSTANT  VARCHAR2(15):='</FRAGMENTID>';

    LCompoundFragmentID                 Number:=0;
    LCompoundFragmentIdTag CONSTANT     VARCHAR2(15):='<ID>';
    LCompoundFragmentIdTagEnd CONSTANT  VARCHAR2(15):='</ID>';

    LBatchComponentIdTag CONSTANT       VARCHAR2(20):='<ID>';
    LBatchComponentIdTagEnd CONSTANT    VARCHAR2(20):='</ID>';

    LBatchComponentID                   Number:=0;
    LBatchCompFragIdTag CONSTANT        VARCHAR2(20):='<BATCHCOMPONENTID>';
    LBatchCompFragIdTagEnd CONSTANT     VARCHAR2(25):='</BATCHCOMPONENTID>';
    LBatchCompoundFragIdTag CONSTANT    VARCHAR2(20):='<COMPOUNDFRAGMENTID>';
    LBatchCompoundFragIdTagEnd CONSTANT VARCHAR2(25):='</COMPOUNDFRAGMENTID>';

    LStructureID                        Number:=0;
    LStructureIDTag CONSTANT            VARCHAR2(15):='<STRUCTUREID>';
    LStructureIDTagEnd CONSTANT         VARCHAR2(15):='</STRUCTUREID>';

    LMixtureComponentID                 Number:=0;
    LMixtureComponentIDTag CONSTANT     VARCHAR2(25):='<MIXTURECOMPONENTID>';
    LMixtureComponentIDTagEnd CONSTANT  VARCHAR2(25):='</MIXTURECOMPONENTID>';

    LBatchNumber                     Number:=0;
    LBatchNumberTag CONSTANT         VARCHAR2(15):='<BATCHNUMBER>';
    LBatchNumberTagEnd CONSTANT      VARCHAR2(15):='</BATCHNUMBER>';

    LRegNumber                       VW_REGISTRYNUMBER.RegNumber%Type;
    LMixtureRegNumber                VW_REGISTRYNUMBER.RegNumber%Type;
    LRegNumberAux                    VW_REGISTRYNUMBER.RegNumber%Type;
    LRegNumberTag CONSTANT           VARCHAR2(15):='<REGNUMBER>';
    LRegNumberTagEnd CONSTANT        VARCHAR2(15):='</REGNUMBER>';

    LFullRegNumber                   VW_REGISTRYNUMBER.RegNumber%Type;
    LFullRegNumberTag CONSTANT       VARCHAR2(20):='<FULLREGNUMBER>';
    LFullRegNumberTagEnd CONSTANT VARCHAR2(20):='</FULLREGNUMBER>';

    LSequenceNumber                  VW_REGISTRYNUMBER.SequenceNumber%Type;
    LSequenceNumberTag CONSTANT      VARCHAR2(20):='<SEQUENCENUMBER>';
    LSequenceNumberTagEnd CONSTANT   VARCHAR2(20):='</SEQUENCENUMBER>';


    LMixtureRegID                    Number:=0;


    LMixtureID                       Number:=0;
    LMixtureIDTag CONSTANT           VARCHAR2(15):='<MIXTUREID>';
    LMixtureIDTagEnd CONSTANT        VARCHAR2(15):='</MIXTUREID>';

    LComponentID                     Number:=0;

    LStructureValue                  CLOB;
    LStructuresList                  CLOB;
    LStructuresToValidateList        CLOB;
    LFragmentXmlValue                CLOB;
    LFragmentXmlList                 CLOB;
    LNormalizedStructureList         CLOB;
    LNormalizedStructureValue        CLOB;
    LStructureAggregationList        CLOB;
    LStructureAggregationValue       CLOB;
    LXMLRegistryRecord               CLOB;

    LDuplicatedCompoundID            Number;
    LDuplicatedStructures            CLOB;
    LListDulicatesCompound           CLOB;
    LDuplicateComponentCount         Number:=0;
    LStructureIDToValidate           Number;
    LRegIdsValue                Varchar2(4000);
    LDuplicatedMixtureRegIds         Varchar2(4000);
    LDuplicatedMixtureCount          Number;
    LMixtureIDAux                    Varchar2(20);
    LCompoundIdsValueDeleting        Varchar2(4000);

    LXMLCompound                     XmlType;
    LXMLFragmentEquivalent           XmlType;
    LXMLRegNumberDuplicated          XmlType;
    LIDTodelete                      Number;
    LExistentComponentIndex          Number:=0;

    LSequenceID                      Number:=0;

    LRegIDAux                        Number:=0;
    LExistentRegID                   Number:=0;

    LXslTables                       XmlType;

    LDuplicatedAuxStructureID        Number:=0;
    LStructureIDToUpdate             Number;

    LRLSState                        Boolean;

    LXslTablesXML Clob:='
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:variable name="VMixtureID" select="ID" />
      <VW_Mixture>
        <ROW>
          <xsl:for-each select="ID">
            <MIXTUREID>
              <xsl:value-of select="." />
            </MIXTUREID>
          </xsl:for-each>
          <xsl:for-each select="DateCreated[@update=''yes'']">
            <CREATED>
              <xsl:value-of select="." />
            </CREATED>
          </xsl:for-each>
          <xsl:for-each select="PersonCreated[@update=''yes'']">
            <PERSONCREATED>
              <xsl:value-of select="." />
            </PERSONCREATED>
          </xsl:for-each>
          <xsl:for-each select="DateLastModified">
            <MODIFIED>
              <xsl:value-of select="." />
            </MODIFIED>
          </xsl:for-each>
          <xsl:for-each select="StructureAggregation[@update=''yes'']">
            <STRUCTUREAGGREGATION>
              <xsl:copy-of select="." />
            </STRUCTUREAGGREGATION>
          </xsl:for-each>
          <xsl:for-each select="Approved[@update=''yes'']">
            <APPROVED>
              <xsl:value-of select="." />
            </APPROVED>
          </xsl:for-each>
          <xsl:for-each select="PropertyList">
            <xsl:for-each select="Property[@update=''yes'']">
              <xsl:variable name="V1" select="." />
              <xsl:for-each select="@name">
                <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')" />
                                        LESS_THAN_SIGN;<xsl:value-of select="$V2" />GREATER_THAN_SIGN;<xsl:value-of select="$V1" />LESS_THAN_SIGN;/<xsl:value-of select="$V2" />GREATER_THAN_SIGN;</xsl:for-each>
            </xsl:for-each>
          </xsl:for-each>
        </ROW>
      </VW_Mixture>
      <xsl:variable name="VMixtureRegID" select="RegNumber/RegID/." />
      <VW_RegistryNumber>
        <xsl:for-each select="RegNumber">
          <ROW>
            <xsl:for-each select="RegID">
              <REGID>
                <xsl:value-of select="." />
              </REGID>
            </xsl:for-each>
            <xsl:for-each select="SequenceNumber[@update=''yes'']">
              <SEQUENCENUMBER>
                <xsl:value-of select="." />
              </SEQUENCENUMBER>
            </xsl:for-each>
            <xsl:for-each select="RegNumber[@update=''yes'']">
              <REGNUMBER>
                <xsl:value-of select="." />
              </REGNUMBER>
            </xsl:for-each>
            <xsl:for-each select="SequenceID[@update=''yes'']">
              <SEQUENCEID>
                <xsl:value-of select="." />
              </SEQUENCEID>
            </xsl:for-each>
          </ROW>
        </xsl:for-each>
      </VW_RegistryNumber>
      <xsl:for-each select="ProjectList/Project">
        <xsl:variable name="VDeleteProject" select="@delete" />
        <xsl:variable name="VInsertProject" select="@insert" />
        <xsl:for-each select="ID[$VDeleteProject=''yes'']">
          <VW_RegistryNumber_Project>delete="yes"<ROW><ID><xsl:value-of select="." /></ID></ROW></VW_RegistryNumber_Project>
        </xsl:for-each>
        <VW_RegistryNumber_Project>
          <xsl:if test="$VInsertProject=''yes''">insert="yes"</xsl:if>
          <ROW>
            <xsl:for-each select="ID">
              <ID>
                <xsl:value-of select="." />
              </ID>
            </xsl:for-each>
            <xsl:for-each select="ProjectID[@update=''yes'' or $VInsertProject=''yes'']">
              <PROJECTID>
                <xsl:value-of select="." />
              </PROJECTID>
            </xsl:for-each>
            <xsl:if test="$VInsertProject=''yes''">
              <REGID>
                <xsl:value-of select="$VMixtureRegID" />
              </REGID>
            </xsl:if>
          </ROW>
        </VW_RegistryNumber_Project>
      </xsl:for-each>
      <xsl:for-each select="IdentifierList/Identifier">
        <xsl:variable name="VDeleteIdentifier" select="@delete" />
        <xsl:variable name="VInsertIdentifier" select="@insert" />
        <xsl:for-each select="ID[$VDeleteIdentifier=''yes'']">
          <VW_Compound_Identifier>delete="yes"<ROW><ID><xsl:value-of select="." /></ID></ROW></VW_Compound_Identifier>
        </xsl:for-each>
        <VW_Compound_Identifier>
          <xsl:if test="$VInsertIdentifier=''yes''">insert="yes"</xsl:if>
          <ROW>
            <xsl:for-each select="ID">
              <ID>
                <xsl:value-of select="." />
              </ID>
            </xsl:for-each>
            <xsl:for-each select="IdentifierID[@update=''yes'' or $VInsertIdentifier=''yes'']">
              <TYPE>
                <xsl:value-of select="." />
              </TYPE>
            </xsl:for-each>
            <xsl:for-each select="InputText[@update=''yes'' or $VInsertIdentifier=''yes'']">
              <VALUE>
                <xsl:value-of select="." />
              </VALUE>
            </xsl:for-each>
            <xsl:if test="$VInsertIdentifier=''yes''">
              <REGID>
                <xsl:value-of select="$VMixtureRegID" />
              </REGID>
            </xsl:if>
          </ROW>
        </VW_Compound_Identifier>
      </xsl:for-each>
      <xsl:for-each select="ComponentList/Component">
        <xsl:variable name="VComponentIndex" select="ComponentIndex/." />
        <xsl:variable name="VDeleteComponent" select="@delete" />
        <xsl:variable name="VInsertComponent" select="@insert" />
        <xsl:variable name="VCompoundID" select="Compound/CompoundID" />
        <xsl:for-each select="Compound">
          <xsl:variable name="VRegID" select="RegNumber/RegID" />
          <xsl:variable name="VCompound" select="." />
          <VW_RegistryNumber>
            <xsl:choose>
              <xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
            </xsl:choose>
            <xsl:for-each select="RegNumber">
              <ROW>
                <xsl:for-each select="RegID">
                  <REGID>
                    <xsl:value-of select="." />
                  </REGID>
                </xsl:for-each>
                <xsl:for-each select="SequenceNumber[@update=''yes''or $VInsertComponent=''yes'']">
                  <SEQUENCENUMBER>
                    <xsl:value-of select="." />
                  </SEQUENCENUMBER>
                </xsl:for-each>
                <xsl:for-each select="RegNumber[@update=''yes'' or $VInsertComponent=''yes'']">
                  <REGNUMBER>
                    <xsl:value-of select="." />
                  </REGNUMBER>
                </xsl:for-each>
                <xsl:for-each select="SequenceID[@update=''yes'' or $VInsertComponent=''yes'']">
                  <SEQUENCEID>
                    <xsl:value-of select="." />
                  </SEQUENCEID>
                </xsl:for-each>
                <xsl:for-each select="$VCompound/DateCreated[@update=''yes'' or $VInsertComponent=''yes'']">
                  <DATECREATED>
                    <xsl:value-of select="." />
                  </DATECREATED>
                </xsl:for-each>
                <xsl:for-each select="$VCompound/PersonRegistered[@update=''yes'' or $VInsertComponent=''yes'']">
                  <PERSONREGISTERED>
                    <xsl:value-of select="." />
                  </PERSONREGISTERED>
                </xsl:for-each>
              </ROW>
            </xsl:for-each>
          </VW_RegistryNumber>
          <xsl:choose>
            <xsl:when test="$VRegID=''0'' and $VInsertComponent=''yes'' or $VDeleteComponent=''yes'' or string-length($VInsertComponent)=0 ">
              <xsl:choose>
                <xsl:when test="$VDeleteComponent!=''yes'' or string-length($VDeleteComponent)=0 ">
                  <VW_Structure>
                    <xsl:choose>
                      <xsl:when test="$VInsertComponent=''yes'' or BaseFragment/Structure[@insert=''yes'']">insert="yes"</xsl:when>
                      <xsl:when test="BaseFragment/Structure/StructureID=0">insert="yes"</xsl:when>
                    </xsl:choose>
                    <xsl:for-each select="BaseFragment/Structure">
                      <ROW>
                        <xsl:for-each select="StructureID">
                          <STRUCTUREID>
                            <xsl:value-of select="." />
                          </STRUCTUREID>
                        </xsl:for-each>
                        <xsl:for-each select="StructureFormat[@update=''yes''or $VInsertComponent=''yes'' or ../@insert=''yes'' or ../StructureID=0]">
                          <STRUCTUREFORMAT>
                            <xsl:value-of select="." />
                          </STRUCTUREFORMAT>
                        </xsl:for-each>
                        <xsl:for-each select="Structure[@update=''yes''or $VInsertComponent=''yes'' or ../@insert=''yes'' or ../StructureID=0]">
                          <STRUCTURE>
                            <xsl:value-of select="." />
                          </STRUCTURE>
                        </xsl:for-each>
                      </ROW>
                    </xsl:for-each>
                  </VW_Structure>
                </xsl:when>
              </xsl:choose>
              <VW_Compound>
                <xsl:choose>
                  <xsl:when test="$VDeleteComponent=''yes''">delete="yes"</xsl:when>
                </xsl:choose>
                <xsl:choose>
                  <xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
                </xsl:choose>
                <ROW>
                  <xsl:for-each select="CompoundID">
                    <COMPOUNDID>
                      <xsl:value-of select="." />
                    </COMPOUNDID>
                  </xsl:for-each>
                  <xsl:for-each select="DateCreated[@update=''yes''or $VInsertComponent=''yes'']">
                    <DATECREATED>
                      <xsl:value-of select="." />
                    </DATECREATED>
                  </xsl:for-each>
                  <xsl:for-each select="PersonCreated[@update=''yes''or $VInsertComponent=''yes'']">
                    <PERSONCREATED>
                      <xsl:value-of select="." />
                    </PERSONCREATED>
                  </xsl:for-each>
                  <xsl:for-each select="PersonRegistered[@update=''yes''or $VInsertComponent=''yes'']">
                    <PERSONREGISTERED>
                      <xsl:value-of select="." />
                    </PERSONREGISTERED>
                  </xsl:for-each>
                  <xsl:for-each select="DateLastModified">
                    <DATELASTMODIFIED>
                      <xsl:value-of select="." />
                    </DATELASTMODIFIED>
                  </xsl:for-each>
                  <xsl:for-each select="Tag[@update=''yes''or $VInsertComponent=''yes'']">
                    <TAG>
                      <xsl:value-of select="." />
                    </TAG>
                  </xsl:for-each>
                  <xsl:for-each select="RegNumber[@update=''yes''or $VInsertComponent=''yes'']">
                    <xsl:for-each select="RegID">
                      <REGID>
                        <xsl:value-of select="." />
                      </REGID>
                    </xsl:for-each>
                  </xsl:for-each>
                  <xsl:for-each select="BaseFragment">
                    <xsl:for-each select="Structure">
                      <xsl:choose>
                        <xsl:when test="@update=''yes''">
                          <STRUCTUREID>
                            <xsl:value-of select="." />
                          </STRUCTUREID>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:for-each select="StructureID[@update=''yes'' or . &lt; 0 or $VInsertComponent=''yes'' or ../@insert=''yes'']">
                            <STRUCTUREID>
                              <xsl:value-of select="." />
                            </STRUCTUREID>
                          </xsl:for-each>
                        </xsl:otherwise>
                      </xsl:choose>
                      <xsl:for-each select="NormalizedStructure[@update=''yes''or $VInsertComponent=''yes'']">
                        <NORMALIZEDSTRUCTURE>
                          <xsl:value-of select="." />
                        </NORMALIZEDSTRUCTURE>
                      </xsl:for-each>
                      <xsl:for-each select="UseNormalization[@update=''yes''or $VInsertComponent=''yes'']">
                        <USENORMALIZATION>
                          <xsl:value-of select="." />
                        </USENORMALIZATION>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <xsl:for-each select="PropertyList">
                    <xsl:for-each select="Property[@update=''yes'' or $VInsertComponent=''yes'']">
                      <xsl:variable name="V1" select="." />
                      <xsl:for-each select="@name">
                        <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')" />
LESS_THAN_SIGN;<xsl:value-of select="$V2" />GREATER_THAN_SIGN;<xsl:value-of select="$V1" />LESS_THAN_SIGN;/<xsl:value-of select="$V2" />GREATER_THAN_SIGN;</xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                </ROW>
              </VW_Compound>
              <xsl:variable name="VReg" select="." />
              <xsl:for-each select="FragmentList/Fragment">
                <xsl:variable name="VInsertFragment" select="@insert" />
                <xsl:variable name="VDeleteFragment" select="@delete" />
                <xsl:variable name="VFragmentID" select="FragmentID" />
                <xsl:for-each select="FragmentID[$VInsertFragment=''yes'' or $VInsertComponent=''yes'']">
                  <VW_Compound_Fragment>insert="yes"<ROW><ID>0</ID><COMPOUNDID><xsl:value-of select="$VCompoundID" /></COMPOUNDID><FRAGMENTID><xsl:value-of select="." /></FRAGMENTID></ROW></VW_Compound_Fragment>
                </xsl:for-each>
                <xsl:for-each select="CompoundFragmentID[$VDeleteFragment=''yes'']">
                  <VW_Compound_Fragment>delete="yes"<ROW><ID><xsl:value-of select="." /></ID></ROW></VW_Compound_Fragment>
                </xsl:for-each>
              </xsl:for-each>
              <xsl:variable name="VCompound" select="." />
              <xsl:for-each select="FragmentList/Fragment/FragmentID[@update=''yes'']">
                <VW_Compound_Fragment>update="yes"
        <ROW><ID><xsl:value-of select="../CompoundFragmentID" /></ID><FRAGMENTID><xsl:value-of select="." /></FRAGMENTID></ROW></VW_Compound_Fragment>
              </xsl:for-each>
              <xsl:for-each select="IdentifierList/Identifier">
                <xsl:variable name="VDeleteIdentifier" select="@delete" />
                <xsl:variable name="VInsertIdentifier" select="@insert" />
                <xsl:for-each select="ID[$VDeleteIdentifier=''yes'']">
                  <VW_Compound_Identifier>delete="yes"<ROW><ID><xsl:value-of select="." /></ID></ROW></VW_Compound_Identifier>
                </xsl:for-each>
                <VW_Compound_Identifier>
                  <xsl:choose>
                    <xsl:when test="$VInsertComponent=''yes'' or $VInsertIdentifier=''yes''">insert="yes"</xsl:when>
                  </xsl:choose>
                  <ROW>
                    <xsl:for-each select="ID">
                      <ID>
                        <xsl:value-of select="." />
                      </ID>
                    </xsl:for-each>
                    <xsl:for-each select="IdentifierID[@update=''yes'' or $VInsertComponent=''yes'' or $VInsertIdentifier=''yes'']">
                      <TYPE>
                        <xsl:value-of select="." />
                      </TYPE>
                    </xsl:for-each>
                    <xsl:for-each select="InputText[@update=''yes'' or $VInsertComponent=''yes'' or $VInsertIdentifier=''yes'']">
                      <VALUE>
                        <xsl:value-of select="." />
                      </VALUE>
                    </xsl:for-each>
                    <xsl:for-each select="$VCompound/RegNumber">
                      <xsl:for-each select="RegID[@update=''yes'' or $VInsertComponent=''yes'' or $VInsertIdentifier=''yes'']">
                        <REGID>
                          <xsl:value-of select="." />
                        </REGID>
                      </xsl:for-each>
                    </xsl:for-each>
                  </ROW>
                </VW_Compound_Identifier>
              </xsl:for-each>
            </xsl:when>
          </xsl:choose>
          <VW_Mixture_Component>
            <xsl:choose>
              <xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
            </xsl:choose>
            <ROW>
              <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
              <MIXTUREID>0</MIXTUREID>
              <COMPOUNDID>0</COMPOUNDID>
            </ROW>
          </VW_Mixture_Component>
          <xsl:choose>
            <xsl:when test="$VDeleteComponent!=''yes'' or string-length($VDeleteComponent)=0">
              <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[((ComponentIndex=$VComponentIndex) and ((@delete!=''yes'') or (string-length(@delete)=0))) ]">
                <VW_BatchComponent>
                  <xsl:choose>
                    <xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
                  </xsl:choose>
                  <ROW>
                    <xsl:for-each select="ID">
                      <ID>
                        <xsl:value-of select="." />
                      </ID>
                    </xsl:for-each>
                    <xsl:for-each select="CompoundID[(@update=''yes'') or ($VInsertComponent=''yes'')]">
                      <MIXTURECOMPONENTID>
                        <xsl:value-of select="." />
                      </MIXTURECOMPONENTID>
                    </xsl:for-each>
                    <xsl:for-each select="BatchID[(@update=''yes'') or ($VInsertComponent=''yes'')]">
                      <BATCHID>
                        <xsl:value-of select="." />
                      </BATCHID>
                    </xsl:for-each>
                    <xsl:for-each select="OrderIndex[(@update=''yes'') or ($VInsertComponent=''yes'')]">
                      <ORDERINDEX>
                        <xsl:value-of select="." />
                      </ORDERINDEX>
                    </xsl:for-each>
                    <xsl:for-each select="PropertyList">
                      <xsl:for-each select="Property[(@update=''yes'' or $VInsertComponent=''yes'')]">
                        <xsl:variable name="V1" select="." />
                        <xsl:for-each select="@name">
                          <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')" />
                                    LESS_THAN_SIGN;<xsl:value-of select="$V2" />GREATER_THAN_SIGN;<xsl:value-of select="$V1" />LESS_THAN_SIGN;/<xsl:value-of select="$V2" />GREATER_THAN_SIGN;
                            </xsl:for-each>
                      </xsl:for-each>
                    </xsl:for-each>
                  </ROW>
                </VW_BatchComponent>
                <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment">
                  <xsl:variable name="VFragmentID" select="FragmentID" />
                  <VW_BatchComponentFragment>
                    <xsl:choose>
                      <xsl:when test="($VInsertComponent=''yes'')">insert="yes"
                        <ROW>
                            <xsl:attribute name="FragmentID"><xsl:value-of select="FragmentID" /></xsl:attribute>
                            <xsl:attribute name="CompoundID"><xsl:value-of select="../../CompoundID" /></xsl:attribute>
                            <BATCHCOMPONENTID>
                                <xsl:value-of select="../../ID" />
                            </BATCHCOMPONENTID>
                            <COMPOUNDFRAGMENTID>0</COMPOUNDFRAGMENTID>
                            <EQUIVALENT>
                                <xsl:value-of select="Equivalents" />
                            </EQUIVALENT>
                            <ORDERINDEX>
                                <xsl:value-of select="OrderIndex" />
                            </ORDERINDEX>
                        </ROW>
                      </xsl:when>
                      <xsl:when test="Equivalents/@update=''yes''">update="yes"
                            <ROW>
                                <ID>
                                    <xsl:value-of select="ID" />
                                </ID>
                                <xsl:for-each select="Equivalents[@update=''yes'']">
                                    <EQUIVALENT>
                                        <xsl:value-of select="." />
                                    </EQUIVALENT>
                                </xsl:for-each>
                                <xsl:for-each select="OrderIndex[(@update=''yes'') or ($VInsertComponent=''yes'')]">
                                    <ORDERINDEX>
                                        <xsl:value-of select="." />
                                    </ORDERINDEX>
                                </xsl:for-each>
                            </ROW>
                        </xsl:when>
                    </xsl:choose>
                  </VW_BatchComponentFragment>
                </xsl:for-each>
              </xsl:for-each>
            </xsl:when>
          </xsl:choose>
        </xsl:for-each>
      </xsl:for-each>
      <xsl:variable name="VCompound" select="." />'||
      '<xsl:for-each select="BatchList/Batch">
        <xsl:if test="BatchID =0 and @insert=''yes''">
          <xsl:variable name="VInsertBacth" select="@insert" />
        </xsl:if>
        <xsl:if test="BatchID !=0 and @insert=''yes''">
          <xsl:variable name="VUpdateTable" select="''yes''" />
        </xsl:if>
        <VW_Batch>
          <xsl:choose>
            <xsl:when test="$VInsertBacth=''yes''">insert="yes"</xsl:when>
          </xsl:choose>
          <ROW>
            <xsl:for-each select="BatchID">
              <BATCHID>
                <xsl:value-of select="." />
              </BATCHID>
            </xsl:for-each>
            <xsl:for-each select="BatchNumber[@update=''yes'' or $VInsertBacth=''yes'']">
              <BATCHNUMBER>
                <xsl:value-of select="." />
              </BATCHNUMBER>
            </xsl:for-each>
            <xsl:for-each select="FullRegNumber[@update=''yes'' or $VInsertBacth=''yes'']">
              <FULLREGNUMBER>
                <xsl:choose>
                  <xsl:when test="FullRegNumber!=''''">
                    <xsl:value-of select="FullRegNumber" />
                  </xsl:when>
                  <xsl:otherwise>null</xsl:otherwise>
                </xsl:choose>
              </FULLREGNUMBER>
            </xsl:for-each>
            <xsl:if test="(string-length(BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment[@delete=''yes''])>0 or string-length(/MultiCompoundRegistryRecord/ComponentList/Component/Compound/FragmentList/Fragment/FragmentID[@update=''yes''])>0) and string-length(FullRegNumber[@update=''yes''])=0 and $VInsertBacth!=''yes''">
                <FULLREGNUMBER>
                    <xsl:value-of select="string-length(FullRegNumber[@update=''yes'' or $VInsertBacth=''yes''])" />                    
                    <xsl:value-of select="FullRegNumber" />
                </FULLREGNUMBER>
                <BATCHNUMBER>
                    <xsl:value-of select="BatchNumber" />
                </BATCHNUMBER>
            </xsl:if>
            <xsl:for-each select="DateCreated[@update=''yes'' or $VInsertBacth=''yes'']">
              <DATECREATED>
                <xsl:value-of select="." />
              </DATECREATED>
            </xsl:for-each>
            <xsl:for-each select="PersonCreated[@update=''yes'' or $VInsertBacth=''yes'']">
              <PERSONCREATED>
                <xsl:value-of select="." />
              </PERSONCREATED>
            </xsl:for-each>
            <xsl:for-each select="PersonRegistered[@update=''yes'' or $VInsertBacth=''yes'']">
              <PERSONREGISTERED>
                <xsl:value-of select="." />
              </PERSONREGISTERED>
            </xsl:for-each>
            <xsl:for-each select="DateLastModified">
              <DATELASTMODIFIED>
                <xsl:value-of select="." />
              </DATELASTMODIFIED>
            </xsl:for-each>
            <xsl:for-each select="StatusID[@update=''yes'' or $VInsertBacth=''yes'']">
              <STATUSID>
                <xsl:value-of select="." />
              </STATUSID>
            </xsl:for-each>
            <xsl:for-each select="BatchID[$VInsertBacth=''yes'' or $VUpdateTable=''yes'']">
              <REGID>
                <xsl:value-of select="$VMixtureRegID" />
              </REGID>
            </xsl:for-each>
            <xsl:for-each select="TempBatchID[@update=''yes'' or $VInsertBacth=''yes'']">
              <TEMPBATCHID>
                <xsl:value-of select="." />
              </TEMPBATCHID>
            </xsl:for-each>
            <xsl:for-each select="PropertyList">
              <xsl:for-each select="Property[@update=''yes'' or $VInsertBacth=''yes'']">
                <xsl:variable name="V1" select="." />
                <xsl:for-each select="@name">
                  <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')" />
                  <xsl:choose>
                    <xsl:when test="$V2 = ''DELIVERYDATE'' and $V1 != ''''">
LESS_THAN_SIGN;<xsl:value-of select="$V2" />GREATER_THAN_SIGN;<xsl:value-of select="$V1" />LESS_THAN_SIGN;/<xsl:value-of select="$V2" />GREATER_THAN_SIGN;</xsl:when>
                    <xsl:when test="$V2 = ''DATEENTERED'' and $V1 != ''''">
LESS_THAN_SIGN;<xsl:value-of select="$V2" />GREATER_THAN_SIGN;<xsl:value-of select="$V1" />LESS_THAN_SIGN;/<xsl:value-of select="$V2" />GREATER_THAN_SIGN;</xsl:when>
                    <xsl:otherwise>
LESS_THAN_SIGN;<xsl:value-of select="$V2" />GREATER_THAN_SIGN;<xsl:value-of select="$V1" />LESS_THAN_SIGN;/<xsl:value-of select="$V2" />GREATER_THAN_SIGN;</xsl:otherwise>
                  </xsl:choose>
                </xsl:for-each>
              </xsl:for-each>
            </xsl:for-each>
          </ROW>
        </VW_Batch>
        <xsl:for-each select="BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment[@delete=''yes'']">
          <VW_BatchComponentFragment>delete="yes"<ROW><ID><xsl:value-of select="ID" /></ID></ROW></VW_BatchComponentFragment>
        </xsl:for-each>
        <xsl:variable name="VBatch" select="." />
        <xsl:choose>
          <xsl:when test="$VUpdateTable=''yes'' ">
            <xsl:for-each select="$VBatch/BatchComponentList/BatchComponent">
              <VW_BatchComponent>
                <ROW>
                  <ID>0</ID>
                  <xsl:for-each select="$VBatch/BatchID">
                    <BATCHID>
                      <xsl:value-of select="." />
                    </BATCHID>
                  </xsl:for-each>
                  <xsl:for-each select="MixtureComponentID">
                    <MIXTURECOMPONENTID>
                      <xsl:value-of select="." />
                    </MIXTURECOMPONENTID>
                  </xsl:for-each>
                  <xsl:for-each select="PropertyList/Property">
                    <xsl:variable name="V1" select="." />
                    <xsl:for-each select="@name">
                      <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')" />
                                    LESS_THAN_SIGN;<xsl:value-of select="$V2" />GREATER_THAN_SIGN;<xsl:value-of select="$V1" />LESS_THAN_SIGN;/<xsl:value-of select="$V2" />GREATER_THAN_SIGN;
                                </xsl:for-each>
                  </xsl:for-each>
                </ROW>
              </VW_BatchComponent>
            </xsl:for-each>
          </xsl:when>
          <xsl:when test="$VInsertBacth=''yes''">
            <xsl:for-each select="BatchComponentList/BatchComponent[(@insert=''yes'')]">
              <VW_BatchComponent>
                insert="yes"
                <ROW><ID>0</ID><xsl:for-each select="MixtureComponentID"><MIXTURECOMPONENTID><xsl:value-of select="." /></MIXTURECOMPONENTID></xsl:for-each><COMPOUNDID><xsl:value-of select="CompoundID" /></COMPOUNDID><BATCHID>0</BATCHID><ORDERINDEX><xsl:value-of select="OrderIndex" /></ORDERINDEX><xsl:for-each select="PropertyList"><xsl:for-each select="Property"><xsl:variable name="V1" select="." /><xsl:for-each select="@name"><xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')" />
                                LESS_THAN_SIGN;<xsl:value-of select="$V2" />GREATER_THAN_SIGN;<xsl:value-of select="$V1" />LESS_THAN_SIGN;/<xsl:value-of select="$V2" />GREATER_THAN_SIGN;
                            </xsl:for-each></xsl:for-each></xsl:for-each></ROW></VW_BatchComponent>
            </xsl:for-each>
          </xsl:when>
        </xsl:choose>
        <xsl:for-each select="BatchComponentList/BatchComponent">
          <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[(@insert=''yes'')]">
            <VW_BatchComponentFragment>insert="yes"
                <ROW>
                     <xsl:attribute name="FragmentID"><xsl:value-of select="FragmentID" /></xsl:attribute>
                     <xsl:attribute name="CompoundID"><xsl:value-of select="../../CompoundID" /></xsl:attribute>
                    <BATCHCOMPONENTID>
                        <xsl:value-of select="../../ID" />
                    </BATCHCOMPONENTID>
                    <COMPOUNDFRAGMENTID>
                        <xsl:value-of select="ID" />
                    </COMPOUNDFRAGMENTID>
                    <EQUIVALENT>
                        <xsl:value-of select="Equivalents" />
                    </EQUIVALENT>
                    <ORDERINDEX>
                        <xsl:value-of select="OrderIndex" />
                    </ORDERINDEX>
                </ROW>
            </VW_BatchComponentFragment>
          </xsl:for-each>
        </xsl:for-each>
        <xsl:for-each select="BatchComponentList/BatchComponent[(@insert=''yes'') and (BatchComponentFragmentList/BatchComponentFragment/@insert!=''yes'' or string-length(BatchComponentFragmentList/BatchComponentFragment/@insert)=0)]">
            <xsl:variable name="VComponentIndex1" select="ComponentIndex" />
            <xsl:choose>
                <xsl:when test="/MultiCompoundRegistryRecord/ComponentList/Component[@insert!=''yes'' or string-length(@insert)=0]/ComponentIndex=$VComponentIndex1">
                  <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment">
                    <VW_BatchComponentFragment>insert="yes"
                        <ROW>
                             <xsl:attribute name="FragmentID"><xsl:value-of select="FragmentID" /></xsl:attribute>
                             <xsl:attribute name="CompoundID"><xsl:value-of select="../../CompoundID" /></xsl:attribute>
                            <BATCHCOMPONENTID>
                                <xsl:value-of select="../../ID" />
                            </BATCHCOMPONENTID>
                            <COMPOUNDFRAGMENTID>
                                <xsl:value-of select="ID" />
                            </COMPOUNDFRAGMENTID>
                            <EQUIVALENT>
                                <xsl:value-of select="Equivalents" />
                            </EQUIVALENT>
                            <ORDERINDEX>
                                <xsl:value-of select="OrderIndex" />
                            </ORDERINDEX>
                        </ROW>
                    </VW_BatchComponentFragment>
                  </xsl:for-each>
                </xsl:when>
            </xsl:choose>
        </xsl:for-each>
        <xsl:for-each select="ProjectList/Project">
          <xsl:variable name="VDeleteProject" select="@delete" />
          <xsl:variable name="VInsertProject" select="@insert" />
          <xsl:for-each select="ID[$VDeleteProject=''yes'']">
            <VW_Batch_Project>delete="yes"<ROW><ID><xsl:value-of select="." /></ID></ROW></VW_Batch_Project>
          </xsl:for-each>
          <VW_Batch_Project>
            <xsl:if test="$VInsertProject=''yes'' or $VInsertBacth=''yes''">insert="yes"</xsl:if>
            <ROW>
              <ID>0</ID>
              <xsl:for-each select="ProjectID[@update=''yes'' or $VInsertProject=''yes'' or $VInsertBacth=''yes'']">
                <PROJECTID>
                  <xsl:value-of select="." />
                </PROJECTID>
              </xsl:for-each>
              <xsl:if test="$VInsertProject=''yes''  or $VInsertBacth=''yes''">
                <xsl:for-each select="$VBatch/BatchID">
                  <BATCHID>
                    <xsl:value-of select="." />
                  </BATCHID>
                </xsl:for-each>
              </xsl:if>
            </ROW>
          </VW_Batch_Project>
        </xsl:for-each>
        <xsl:for-each select="IdentifierList/Identifier">
          <xsl:variable name="VDeleteIdentifier" select="@delete" />
          <xsl:variable name="VInsertIdentifier" select="@insert" />
          <xsl:for-each select="ID[$VDeleteIdentifier=''yes'']">
            <VW_BatchIdentifier>delete="yes"<ROW><ID><xsl:value-of select="." /></ID></ROW></VW_BatchIdentifier>
          </xsl:for-each>
          <VW_BatchIdentifier>
            <xsl:if test="$VInsertIdentifier=''yes''">insert="yes"</xsl:if>
            <ROW>
              <xsl:for-each select="ID">
                <ID>
                  <xsl:value-of select="." />
                </ID>
              </xsl:for-each>
              <xsl:for-each select="IdentifierID[@update=''yes'' or $VInsertIdentifier=''yes'']">
                <TYPE>
                  <xsl:value-of select="." />
                </TYPE>
              </xsl:for-each>
              <xsl:for-each select="InputText[@update=''yes'' or $VInsertIdentifier=''yes'']">
                <VALUE>
                  <xsl:value-of select="." />
                </VALUE>
              </xsl:for-each>
              <xsl:if test="$VInsertIdentifier=''yes''">
                <xsl:for-each select="$VBatch/BatchID">
                  <BATCHID>
                    <xsl:value-of select="." />
                  </BATCHID>
                </xsl:for-each>
              </xsl:if>
            </ROW>
          </VW_BatchIdentifier>
        </xsl:for-each>
      </xsl:for-each>
    </MultiCompoundRegistryRecord>
  </xsl:template>
</xsl:stylesheet>
';

    PROCEDURE SetKeyValue(AID VARCHAR2,AIDTag VARCHAR2,AIDTagEnd VARCHAR2) IS
        LPosTag                   Number:=0;
        LPosTagNull                       Number:=0;
        LPosTagEnd                Number:=0;
    BEGIN
        LPosTag:=1;
        LOOP
            LPosTagNull := INSTR(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',LPosTag);
            IF LPosTagNull<>0 THEN
                LXmlRows:=REPLACE(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',AIDTag||AIDTagEnd);
            END IF;
           LPosTag := INSTR(LXmlRows,AIDTag,LPosTag);
        EXIT WHEN LPosTag=0;
            LPosTag  := LPosTag + LENGTH(AIDTag)- 1;
            LPosTagEnd := INSTR(LXmlRows,AIDTagEnd,LPosTag);
            LXmlRows:=SUBSTR(LXmlRows,1,LPosTag)||AID||SUBSTR(LXmlRows,LPosTagEnd,LENGTH(LXmlRows));
        END LOOP;
    END;

BEGIN

     $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','AXml->'||AXml); $end null;

    LXslTables :=XmlType.CreateXml(LXslTablesXML);

    SetSessionParameter;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','ADuplicateCheck->'||ADuplicateCheck); $end null;

    LXmlCompReg:=AXml;

    LSomeUpdate:=False;
    -- Take Out the Structures because the nodes of XmlType don't suport a size grater 64k.
    --LFragmentXmlList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<Fragment>');
    LStructuresList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<BaseFragment>',NULL,FALSE);
    LStructuresToValidateList:=LStructuresList;
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure',NULL,NULL,TRUE,FALSE);
    LStructureAggregationList:=TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation',NULL,NULL,TRUE,TRUE);

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LFragmentXmlList= '||LFragmentXmlList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructuresListt= '||LStructuresList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LNormalizedStructureList= '||LNormalizedStructureList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureAggregationList= '||LStructureAggregationList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXmlCompReg sin Structures= '||LXmlCompReg); $end null;

    --Get the xml
    LXmlTables:=XmlType.createXML(LXmlCompReg);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','after'); $end null;


    --Get the reg number for retrieval of the record as part of the output message
    SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/RegNumber')
      INTO LRegNumber
      FROM dual;

    LMixtureRegNumber:=LRegNumber;

    ValidateIdentityBetweenBatches(LXmlTables);

    IF UPPER(GetDuplicateCheckEnable)='TRUE' THEN
        IF Upper(ADuplicateCheck)='C' THEN
            --Validate Components Strcuture
            LIndex:=0;
            LOOP
                LIndex:=LIndex+1;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LIndex ->'||LIndex); $end null;

                SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']')
                  INTO LXMLCompound
                  FROM dual;

            EXIT WHEN LXMLCompound IS NULL;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXMLCompound='||LXMLCompound.getclobVal()); $end null;


                SELECT extract(LXMLCompound,'/Component/Compound/BaseFragment/Structure/Structure[@update="yes" or @insert="yes"]/text()').getClobVal()
                  INTO LStructureUpdating
                  FROM dual;

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Valdiation-LStructuresList='||LStructuresList); $end null;
                LStructureValue:=TakeOffAndGetClob(LStructuresToValidateList,'Clob');
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Valdiation-LStructureValue='||LStructureValue); $end null;

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureUpdating='||LStructureUpdating); $end null;

                IF LStructureValue IS NOT NULL AND INSTR(UPPER(LXMLCompound.getClobVal()),'DELETE="YES"')=0 AND LStructureUpdating IS NOT NULL THEN
                    SELECT extractValue(LXMLCompound,'/Component/Compound/BaseFragment/Structure/StructureID')
                        INTO LDuplicatedAuxStructureID
                        FROM dual;
                    IF NVL(LDuplicatedAuxStructureID,0)>=0 THEN
                        IF ValidateWildcardStructure(LStructureValue) THEN
                            SELECT extractValue(LXMLCompound,'/Component/Compound/BaseFragment/Structure/StructureID'),extractValue(LXMLCompound,'/Component/ComponentIndex')
                                INTO LStructureIDToValidate,LExistentComponentIndex
                                FROM dual;

                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureIDToValidate ->'||LStructureIDToValidate); $end null;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LExistentComponentIndex ->'||LExistentComponentIndex); $end null;

                            SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex='||LExistentComponentIndex||']/BatchComponentFragmentList')--.getClobVal()
                                    INTO LXMLFragmentEquivalent
                                    FROM dual;
                            $if CompoundRegistry.Debuging $then IF LXMLFragmentEquivalent IS NOT NULL  THEN InsertLog('UpdateMultiCompoundRegistry', 'LXMLFragmentEquivalent->'||LXMLFragmentEquivalent.getClobVal()); END IF; $end null;
                            LDuplicatedStructures:=ValidateCompoundMulti(LStructureValue,LStructureIDToValidate, AConfigurationID, LXMLCompound,LXMLFragmentEquivalent);
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureIDToValidate='||LStructureIDToValidate||'LDuplicatedStructures->'||LDuplicatedStructures); $end null;
                            IF LDuplicatedStructures IS NOT NULL AND LDuplicatedStructures<>'<REGISTRYLIST></REGISTRYLIST>'THEN
                                SELECT extractValue(LXMLCompound,'/Component/Compound/CompoundID')
                                    INTO LDuplicatedCompoundID
                                    FROM dual;
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LDuplicatedCompoundID='||LDuplicatedCompoundID); $end null;
                                LListDulicatesCompound:=LListDulicatesCompound||'<COMPOUND>'||'<TEMPCOMPOUNDID>'||LDuplicatedCompoundID||'</TEMPCOMPOUNDID>'||LDuplicatedStructures||'</COMPOUND>';
                                LDuplicateComponentCount:=LDuplicateComponentCount+1;
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LDuplicateComponentCount->'||LDuplicateComponentCount); $end null;
                            END IF;
                        END IF;
                    END IF;
                END IF;
            END LOOP;
            IF LListDulicatesCompound IS NOT NULL THEN
                LListDulicatesCompound:='<COMPOUNDLIST>'||LListDulicatesCompound||'</COMPOUNDLIST>';
                IF LDuplicateComponentCount=1 THEN
                    BEGIN
                        AMessage := CreateRegistrationResponse('1 duplicated component.', LListDulicatesCompound, NULL);
                    END;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'AMessage->'||AMessage); $end NULL;
                    RETURN;
                ELSE
                    BEGIN
                        AMessage := CreateRegistrationResponse(to_char(LDuplicateComponentCount) || ' duplicated components.', LListDulicatesCompound, NULL);
                    END;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'AMessage->'||AMessage); $end NULL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF (Upper(ADuplicateCheck)='M') OR (Upper(ADuplicateCheck)='C') THEN

            SELECT XmlTransform(extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[((@delete!=''yes'') or (string-length(@delete)=0))]/Compound/RegNumber/RegID'),XmlType.CreateXml('
                  <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                    <xsl:template match="/RegID">
                          <xsl:for-each select=".">
                              <xsl:value-of select="."/>,</xsl:for-each>
                    </xsl:template>
                  </xsl:stylesheet>')).GetClobVal()
              INTO LRegIdsValue
              FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LRegIdsValue->'||LRegIdsValue); $end null;
            LRegIdsValue:=SUBSTR(LRegIdsValue,1,Length(LRegIdsValue)-1);

            SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/ID'),XmlTransform(extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[@delete=''yes'']/Compound/CompoundID'),XmlType.CreateXml('
                  <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                    <xsl:template match="/CompoundID">
                          <xsl:for-each select=".">
                              <xsl:value-of select="."/>,</xsl:for-each>
                    </xsl:template>
                  </xsl:stylesheet>')).GetClobVal()
                INTO LMixtureIDAux,LCompoundIdsValueDeleting
                FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LMixtureIDAux:'||LMixtureIDAux); $end null;

            LCompoundIdsValueDeleting:=SUBSTR(LCompoundIdsValueDeleting,1,Length(LCompoundIdsValueDeleting)-1);

            LDuplicatedMixtureRegIds:=ValidateMixture(LRegIdsValue,LDuplicatedMixtureCount,LMixtureIDAux,null,LXmlTables);
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LDuplicatedMixtureCount->'||LDuplicatedMixtureCount||' LDuplicatedMixtureRegIds->'||LDuplicatedMixtureRegIds); $end null;

            IF LDuplicatedMixtureRegIds IS NOT NULL THEN
                IF LDuplicatedMixtureCount > 1 THEN
                    BEGIN
                        AMessage := CreateRegistrationResponse(to_char( LDuplicatedMixtureCount ) || ' duplicated mixtures.', LDuplicatedMixtureRegIds, NULL);
                    END;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'AMessage->'||AMessage); $end NULL;
                    RETURN;
                ELSE
                    BEGIN
                        AMessage := CreateRegistrationResponse('1 duplicated mixture.', LDuplicatedMixtureRegIds, NULL);
                    END;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'AMessage->'||AMessage); $end NULL;
                    RETURN;
                END IF;
            END IF;

        END IF;
    END IF;

    LBriefMessage := 'Compound Validation OK';
    LMessage := LMessage || LBriefMessage ||CHR(13);

    --Build a new formatted Xml
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','before LXslTablesTransformed->'||LXmlTables.getClobVal()); $end null;
    SELECT XmlTransform(LXmlTables,LXslTables)
        INTO LXslTablesTransformed
        FROM DUAL;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXslTablesTransformed->'||LXslTablesTransformed.getClobVal()); $end null;

    LStructureValue:='';

    LIndex:=0;
    --Look over Xml searching each Table and update the rows of it.
    LOOP

        --Search each Table
        LIndex:=LIndex+1;
        SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/node()['||LIndex||']').getClobVal()
            INTO LXmlRows
            FROM dual;

        LXmlTypeRows := XmlType.CreateXml(LXmlRows);

    EXIT WHEN LXmlRows IS NULL;

        --Replace '&lt;' and '&gt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        LXmlRows:=replace(replace(replace(LXmlRows,'&quot;','"'),'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');

        --Get Table Name. Remove  '<' and '>'
        LTableName:= substr(LXmlRows,2,INSTR(LXmlRows,'>')-2);

        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXmlRows->'||LXmlRows); $end null;

        IF INSTR(LXmlRows,'insert="yes"')=0 THEN
            LSectionInsert:=FALSE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionInsert->FALSE'); $end null;
        ELSE
            LSectionInsert:=TRUE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionInsert->TRUE'); $end null;
        END IF;

        IF INSTR(LXmlRows,'delete="yes"')=0 THEN
            LSectionDelete:=FALSE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete->FALSE'); $end null;
        ELSE
            LSectionDelete:=TRUE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete->TRUE'); $end null;
        END IF;

        IF LSectionDelete THEN
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Processing delete on ' || LTableName ); $end null;
            CASE UPPER(LTableName)
                WHEN 'VW_COMPOUND' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Compound/ROW/COMPOUNDID')
                            INTO LIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteCompound LIDTodelete='||LIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteCompound(LIDTodelete,LMixtureID,LMessage);
                    END;
                 WHEN 'VW_COMPOUND_FRAGMENT' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Compound_Fragment/ROW/ID')
                            INTO LIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteFragment LIDTodelete='||LIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteFragment(LIDTodelete,LMessage);
                    END;
                 WHEN 'VW_COMPOUND_IDENTIFIER' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Compound_Identifier/ROW/ID')
                            INTO LIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteIdentifier LIDTodelete='||LIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteIdentifier(LIDTodelete,LMessage);
                    END;
                 WHEN 'VW_REGISTRYNUMBER_PROJECT' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_RegistryNumber_Project/ROW/ID')
                            INTO LIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteRegistryNumberProject LIDTodelete='||LIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteRegistryNumberProject(LIDTodelete,LMessage);
                    END;
                WHEN 'VW_BATCHIDENTIFIER' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_BatchIdentifier/ROW/ID')
                            INTO LIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteBatchIdentifier LIDTodelete='||LIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteBatchIdentifier(LIDTodelete,LMessage);
                    END;
                 WHEN 'VW_BATCH_PROJECT' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Batch_Project/ROW/ID')
                            INTO LIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteBatchProject LIDTodelete='||LIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteBatchProject(LIDTodelete,LMessage);
                    END;
                  WHEN 'VW_BATCHCOMPONENTFRAGMENT' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_BatchComponentFragment/ROW/ID')
                            INTO LIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteBatchComponentFragment LIDTodelete='||LIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteBatchComponentFragment(LIDTodelete,LMessage);
                    END;
                  WHEN 'VW_STRUCTURE' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Structure/ROW/STRUCTUREID')
                            INTO LIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteStructure LIDTodelete='||LIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteStructure(LIDTodelete,LMessage);
                    END;
                 ELSE  LMessage := LMessage || ' "' || LTableName || '" table stranger.' ||CHR(13);
            END CASE;
        ELSIF LSectionInsert THEN
        /*Insert*/
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LTableName 3->'||LTableName||' LXmlRows='||LXmlRows); $end null;

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Processing insert on ' || LTableName ); $end null;

            --Customization for each View - Use of Sequences
            CASE UPPER(LTableName)
                WHEN 'VW_BATCH' THEN
                    BEGIN
                        SELECT SEQ_BATCHES.NEXTVAL INTO LBatchID FROM DUAL;
                        SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
                        SELECT  extractvalue(XmlType(LXmlRows),'VW_Batch/ROW[1]/REGID')
                            INTO LMixtureRegID
                            FROM dual;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting VW_BATCH LMixtureRegID->'||LMixtureRegID); $end null;


                        LRLSState:=RegistrationRLS.GetStateRLS;
                        IF LRLSState THEN
                            RegistrationRLS.SetEnableRLS(False);
                        END IF;

                        SELECT NVL(MAX(BatchNumber),0)+1
                            INTO LBatchNumber
                            FROM VW_Batch
                            WHERE REGID=LMixtureRegID;

                        IF LRLSState THEN
                            RegistrationRLS.SetEnableRLS(LRLSState);
                        END IF;


                        SELECT  extractvalue(XmlType(LXmlRows),'VW_Batch/ROW/FULLREGNUMBER')
                            INTO LFullRegNumber
                            FROM dual;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LFullRegNumber='||LFullRegNumber); $end null;
                        IF LFullRegNumber='null' THEN
                             SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/RegNumber'),extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/SequenceID')
                                INTO LFullRegNumber,LSequenceID
                                FROM dual;
                             $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Mixture RegNumber ='||LFullRegNumber); $end null;

                            IF LSequenceID IS NOT NULL THEN
                                LFullRegNumber:=GetFullRegNumber(LSequenceID,LXmlTables,LFullRegNumber,LBatchNumber,GetBatchNumberPadding());
                            END IF;
                        ELSE
                            NULL;
                        --LSequenceID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();
                        --$if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LSequenceID'||'->'||LSequenceID); $end null;

                        --ValidateRegNumber(LFullRegNumber,LSequenceID,'N');
                        END IF;

                        SetKeyValue(LBatchNumber,LBatchNumberTag,LBatchNumberTagEnd);
                        SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
                        SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                        SetKeyValue(LFullRegNumber,LFullRegNumberTag,LFullRegNumberTagEnd);
                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    END;
                WHEN 'VW_BATCH_PROJECT' THEN
                    BEGIN
                        IF NVL(LBatchID,0)<>0 THEN
                            SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
                        END IF;

                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    END;
                WHEN 'VW_BATCHIDENTIFIER' THEN
                    BEGIN
                        IF NVL(LBatchID,0)<>0 THEN
                            SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
                        END IF;

                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    END;
                WHEN 'VW_BATCHCOMPONENT' THEN
                    BEGIN
                        SELECT SEQ_BATCHCOMPONENT.NEXTVAL INTO LBatchComponentID FROM DUAL;
                        SetKeyValue(LBatchComponentID,LBatchComponentIdTag,LBatchComponentIdTagEnd);

                        IF NVL(LBatchId,0)<>0 THEN
                          SetKeyValue(LBatchId,LBatchIdTag,LBatchIdTagEnd);
                        END IF;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LBatchId='||LBatchId||' LMixtureComponentID='||LMixtureComponentID||' LMixtureComponentIDTag='||LMixtureComponentIDTag||' LMixtureComponentIDTagEnd='||LMixtureComponentIDTagEnd||'LXmlRows1='||LXmlRows); $end null;

                        SELECT ExtractValue (XmlType (LXmlRows), 'VW_BatchComponent/ROW/COMPOUNDID')
                              INTO LComponentID
                              FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting LComponentID->'||LComponentID||'LMixtureID->'||LMixtureID); $end null;

                        IF NVL (LComponentID, 0) > 0  THEN
                            SELECT MixtureComponentID
                                INTO LMixtureComponentID
                                FROM VW_Mixture_Component
                                WHERE MixtureID = LMixtureID AND CompoundID = LComponentID;
                        ELSE
                            IF  NVL (LMixtureComponentID, 0) <=0 THEN
                                SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/VW_Compound[1]/ROW/COMPOUNDID/text()').GetNumberVal()
                                  INTO LComponentID
                                  FROM dual;

                                SELECT MixtureComponentID
                                    INTO LMixtureComponentID
                                    FROM VW_Mixture_Component
                                    WHERE MixtureID = LMixtureID AND CompoundID = LComponentID;
                            END IF;
                        END IF;

                        IF INSTR(LXmlRows,'<COMPOUNDID>')<>0 THEN
                          LXmlRows:=SUBSTR(LXmlRows,1,INSTR(LXmlRows,'<COMPOUNDID>')-1)||SUBSTR(LXmlRows,INSTR(LXmlRows,'</COMPOUNDID>')+13,LENGTH(LXmlRows));
                        END IF;
                        LXmlRows:=REPLACE(LXmlRows,'insert="yes"','');
                        SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXmlRows2='||LXmlRows||' LMixtureComponentID='||LMixtureComponentID||' LMixtureComponentIDTag='||LMixtureComponentIDTag||' LMixtureComponentIDTagEnd='||LMixtureComponentIDTagEnd); $end null;

                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);

                    END;

               WHEN 'VW_STRUCTURE' THEN
                BEGIN
                    SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_Structure/ROW/STRUCTUREID/text()')
                        INTO LStructureID
                        FROM dual;
                    IF NVL(LStructureID,0)=0 THEN
                        SELECT MOLID_SEQ.NEXTVAL INTO LStructureID FROM DUAL;
                        SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting VW_STRUCTURE LStructuresList='||LStructuresList); $end null;
                        LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting VW_STRUCTURE LStructureValue= '|| LStructureValue ); $end null;
                        LXmlRows:=Replace(LXmlRows,'<STRUCTURE>(RemovedStructure)</STRUCTURE>','');
                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);

                        IF Upper(ADuplicateCheck)='N' OR ( NOT (Upper(ADuplicateCheck)='N') AND RegistrationRLS.GetStateRLS )  THEN
                            IF NVL(LRegIDAux,0) = 0 THEN
                                IF ValidateWildcardStructure(LStructureValue) THEN
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LRegNumber= '|| LRegNumber ); $end null;
                                    VerifyAndAddDuplicateToSave(LRegNumber,LStructureValue, NULL,LXMLRegNumberDuplicated);
                                END IF;
                            END IF;
                        END IF;
                    ELSE
                      LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                    END IF;
                END;
            WHEN 'VW_REGISTRYNUMBER' THEN
                BEGIN
                    SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_RegistryNumber/ROW/REGID/text()')
                        INTO LRegIDAux
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting LRegIDAux'||'->'||LRegIDAux); $end null;
                    IF LRegIDAux=0 THEN

                        SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;
                        SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting LRegID'||'->'||LRegID); $end null;
                        SELECT  extractvalue(XmlType(LXmlRows),'VW_RegistryNumber/ROW/SEQUENCEID')
                          INTO LSequenceID
                          FROM dual;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting LSequenceID'||'->'||LSequenceID); $end null;

                        IF LSequenceID IS NOT NULL THEN
                          LRegNumber:=GetRegNumber(LSequenceID,LSequenceNumber,LXmlTables);
                        END IF;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating LRegNumber'||'->'||LRegNumber); $end null;
                        SetKeyValue(LRegNumber,LRegNumberTag,LRegNumberTagEnd);
                        SetKeyValue(LSequenceNumber,LSequenceNumberTag,LSequenceNumberTagEnd);
                        SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating LXmlRows'||'->'||LXmlRows); $end null;

                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    ELSE
                        LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating1 VW_REGISTRYNUMBER LRegIDAux->'||LRegIDAux); $end null;
                        SELECT CompoundID INTO LCompoundID
                            FROM VW_Compound WHERE RegID=LRegIDAux;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating VW_REGISTRYNUMBER LCompoundID->'||LCompoundID); $end null;

                    END IF;
                END;
            WHEN 'VW_COMPOUND_IDENTIFIER' THEN
                BEGIN
                    SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_COMPOUND_PROJECT' THEN
                BEGIN
                    SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_REGISTRYNUMBER_PROJECT' THEN
                BEGIN
                    SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_COMPOUND' THEN
                BEGIN
                    SELECT SEQ_COMPOUND_MOLECULE.NEXTVAL INTO LCompoundID FROM DUAL;

                    SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
                    SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
                    SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);
                    SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
                    SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');

                    $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry','VW_COMPOUND LNormalizedStructureList='||LNormalizedStructureList); $end null;
                    LNormalizedStructureValue :=TakeOffAndGetClob(LNormalizedStructureList,'Clob');
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry','VW_COMPOUND LNormalizedStructureValue = '|| LNormalizedStructureValue  ); $end null;

                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_COMPOUND_FRAGMENT' THEN
                BEGIN
                /*    SELECT Min(ID)
                        INTO LCompoundFragmentID
                        FROM VW_Compound_Fragment
                        WHERE FragmentID=LFragmentID AND CompoundID=LCompoundID;
                    IF NVL(LCompoundFragmentID,0)=0 THEN    */
                        SELECT SEQ_COMPOUND_FRAGMENT.NEXTVAL INTO LCompoundFragmentID FROM DUAL;
                        SetKeyValue(LCompoundFragmentID,LCompoundFragmentIdTag,LCompoundFragmentIdTagEnd);
                        --SetKeyValue(LFragmentID,LFragmentIdTag,LFragmentIdTagEnd);
                        IF NVL(LCompoundID,0)<>0 THEN
                            SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
                        END IF;
                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    --END IF;
                END;
              WHEN 'VW_MIXTURE_COMPONENT' THEN
                BEGIN
                    SELECT SEQ_MIXTURE_COMPONENT.NEXTVAL INTO LMixtureComponentID FROM DUAL;

                    SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);
                    SetKeyValue(LMixtureID,LMixtureIDTag,LMixtureIDTagEnd);
                    SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_BATCHCOMPONENTFRAGMENT' THEN
                BEGIN
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LCompoundFragmentID1= '|| LCompoundFragmentID ); $end null;
                     SELECT ExtractValue(LXmlTypeRows, 'VW_BatchComponentFragment/ROW/@FragmentID'),
                            ExtractValue(LXmlTypeRows, 'VW_BatchComponentFragment/ROW/@CompoundID')
                            INTO LFragmentID,LCompoundIDAux
                            FROM DUAL;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LCompoundIDAux= '|| LCompoundIDAux ); $end null;
                    IF NVL(LCompoundIDAux,0)>0 THEN
                        LCompoundID:=LCompoundIDAux;
                    END IF;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LFragmentID= '|| LFragmentID ); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LCompoundID= '|| LCompoundID ); $end null;
                    IF NVL(LFragmentID,0)<>0 AND NVL(LCompoundID,0)<>0 THEN
                        SELECT  MIN(ID)
                            INTO LCompoundFragmentID
                            FROM VW_Compound_Fragment
                           WHERE CompoundID = LCompoundID AND
                                 FragmentID = LFragmentID;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LCompoundFragmentID2= '|| LCompoundFragmentID ); $end null;
                    END IF;

                    IF NVL(LCompoundFragmentID,0)<>0 THEN
                        SetKeyValue(LCompoundFragmentID,LBatchCompoundFragIdTag,LBatchCompoundFragIdTagEnd);
                    END IF;


                    IF NVL(LBatchComponentID,0)<>0 THEN
                        SetKeyValue(LBatchComponentID,LBatchCompFragIdTag,LBatchCompFragIdTagEnd);
                    END IF;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','VW_BATCHCOMPONENTFRAGMENT= '|| LXmlRows ); $end null;



                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;

                ELSE  LMessage:=LMessage || ' "' || LTableName||'" table stranger.';
            END CASE;

            LXmlRows:=replace(LXmlRows,'insert="yes"','');
            LXmlRows:=replace(LXmlRows,'delete="yes"','');

            IF LRowsInserted>0 THEN
                LRowsProcessed:=LRowsProcessed + LRowsInserted;
                LSomeUpdate:=TRUE;
            END IF;

        ELSE
        -- Update

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Processing update on ' || LTableName ); $end null;

            LinsCtx := DBMS_XMLSTORE.newContext(LTableName);

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Before LFieldName:'||LTableName||'-'||LXmlRows); $end null;

            SELECT  XmlType(LXmlRows).extract(LTableName||'/ROW[1]/node()[1]').getClobVal()
                INTO LFieldName
                FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','After LFieldName: LFieldName='||LFieldName); $end null;

            LUpdate:=FALSE;
            IF LFieldName IS NOT NULL THEN
                DBMS_XMLSTORE.clearUpdateColumnList(LinsCtx);

                CASE UPPER(LTableName)
                    WHEN 'VW_MIXTURE' THEN
                    BEGIN
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Mixture/ROW/MIXTUREID')
                          INTO LMixtureID
                          FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','W_MIXTURE LMixtureID:'||LMixtureID); $end null;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureAggregationValue= '||LStructureAggregationValue); $end null;
                        LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureAggregationValue= '||LStructureAggregationValue); $end null;

                        SetKeyValue(SYSDATE,'<MODIFIED>','</MODIFIED>');

                    END;

                    WHEN 'VW_STRUCTURE' THEN
                        BEGIN

                            SELECT  extractvalue(XmlType(LXmlRows),'VW_Structure/ROW[1]/node()[1]')
                                INTO LStructureID
                                FROM dual;

                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructuresList='||LStructuresList); $end null;
                            LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry',' LStructureValue= '|| LStructureValue ); $end null;

                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureID='||LStructureID||' LStructureValue='||LStructureValue); $end null;
                            IF NVL(LRegID,0)<>0 AND LStructureID>0 THEN

                                SELECT RegNumber
                                    INTO LRegNumberAux
                                    FROM VW_RegistryNumber
                                    WHERE RegID=LRegID;

                                DELETE VW_Duplicates WHERE RegNumber =LRegNumberAux;

                                DELETE VW_Duplicates WHERE RegNumberDuplicated = LRegNumberAux;

                                IF ValidateWildcardStructure(LStructureValue) THEN
                                    VerifyAndAddDuplicateToSave(LRegNumberAux,LStructureValue, LRegID,LXMLRegNumberDuplicated);
                                END IF;

                            END IF;
                        END;
                    WHEN 'VW_COMPOUND' THEN
                        BEGIN
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry','LStructureID='||LStructureID||' LStructureValue='||LStructureValue); $end null;

                            $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry','VW_COMPOUND LXmlRows= '|| LXmlRows ); $end null;
                            SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                            SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);

                            LNormalizedStructureValue :=TakeOffAndGetClob(LNormalizedStructureList,'Clob');
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry',' LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LCompoundID='||LCompoundID||' LNormalizedStructureValue='||LNormalizedStructureValue); $end null;

                            IF XmlType(LXmlRows).ExistsNode('VW_Compound/ROW/STRUCTUREID')=1 THEN
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','VW_Compound/ROW/COMPOUNDID/text()='||XmlType.CreateXml(LXmlRows).extract('VW_Compound/ROW/COMPOUNDID/text()').getStringVal()); $end null;
                                SELECT StructureID
                                    INTO LStructureIDToUpdate
                                    FROM VW_Compound
                                    WHERE CompoundID=XmlType.CreateXml(LXmlRows).extract('VW_Compound/ROW/COMPOUNDID/text()').getStringVal();
                            END IF;
                        END;
                    WHEN 'VW_REGISTRYNUMBER' THEN
                        BEGIN
                            SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_RegistryNumber/ROW/REGID/text()')
                                INTO LRegID
                                FROM dual;
                        END;
                    WHEN 'VW_BATCH' THEN
                        BEGIN
                            IF XmlType(LXmlRows).ExistsNode('VW_Batch/ROW/FULLREGNUMBER')=1 THEN

                                SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/RegNumber'),extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/SequenceID'),extractvalue(XmlType(LXmlRows),'VW_Batch/ROW/BATCHNUMBER')
                                    INTO LFullRegNumber,LSequenceID,LBatchNumber
                                    FROM dual;
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Mixture RegNumber ='||LFullRegNumber); $end null;
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LBatchNumber='||LBatchNumber); $end null;

                                IF LSequenceID IS NOT NULL THEN
                                    LFullRegNumber:=GetFullRegNumber(LSequenceID,LXmlTables,LFullRegNumber,LBatchNumber,GetBatchNumberPadding());
                                END IF;

                                SetKeyValue(LFullRegNumber,LFullRegNumberTag,LFullRegNumberTagEnd);

                            END IF;
                            SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                        END;
                    WHEN 'VW_BATCHCOMPONENT' THEN
                        BEGIN
                            SELECT ExtractValue (XmlType (LXmlRows), 'VW_BatchComponent/ROW/MIXTURECOMPONENTID')
                              INTO LComponentID
                              FROM dual;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating LComponentID->'||LComponentID||'LMixtureID->'||LMixtureID); $end null;

                            IF NVL (LComponentID, 0) <> 0  THEN
                                SELECT MixtureComponentID
                                  INTO LMixtureComponentID
                                  FROM VW_Mixture_Component
                                 WHERE MixtureID = LMixtureID AND CompoundID = LComponentID
                                   AND ROWNUM < 2 ORDER BY MixtureComponentID;

                                SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);
                            END IF;
                        END;
                    ELSE  NULL;
                END CASE;

                LFieldName:=XMLType(LFieldName).getRootElement();
                DBMS_XMLSTORE.setKeyColumn(LinsCtx,LFieldName);

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LFieldName->'||LFieldName); $end null;

                LIndexField:=1;
                LOOP
                    --Search each Table
                    LIndexField:=LIndexField+1;
                    SELECT  XmlType(LXmlRows).extract(LTableName||'/ROW[1]/node()['||LIndexField||']').getClobVal()
                        INTO LFieldToUpdate
                        FROM dual;

                    IF LFieldToUpdate IS NOT NULL THEN
                        LUpdate:=TRUE;
                        LFieldToUpdate:=XMLType(LFieldToUpdate).getRootElement();
                    END IF;

                EXIT WHEN LFieldToUpdate IS NULL;
                    DBMS_XMLSTORE.setUpdateColumn(LinsCtx, LFieldToUpdate);
                END LOOP;
            END IF;

            --Insert Rows and get count it inserted
            IF LUpdate THEN
                LSomeUpdate:=TRUE;
                 $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','updateXM LXmlRows->'||LXmlRows); $end null;

                LXmlRows:=Replace(LXmlRows,'<STRUCTURE>(RemovedStructure)</STRUCTURE>','');
                LRowsUpdated := DBMS_XMLSTORE.updateXML(LinsCtx, LXmlRows );
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','updating LRowsUpdated->'||LRowsUpdated); $end null;
                LRowsProcessed:=LRowsProcessed + LRowsUpdated;
                --Build Message Logs
                LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".'||CHR(13);
            ELSE
                --Build Message Logs
                LMessage:=LMessage || ' 0 Row Updated on "'||LTableName||'".';
            END IF;

             --Close the Table Context
             DBMS_XMLSTORE.closeContext(LinsCtx);

            IF UPPER(LTableName)='VW_STRUCTURE' AND LStructureValue IS NOT NULL THEN
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureValue:'||LStructureValue||' LStructureID:'||LStructureID); $end null;
                UPDATE VW_STRUCTURE
                    SET STRUCTURE=LStructureValue
                    WHERE STRUCTUREID=LStructureID;
            END IF;

            IF UPPER(LTableName)='VW_MIXTURE' AND LStructureAggregationValue IS NOT NULL THEN
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry',' LMixtureID:'||LMixtureID||' LStructureAggregationValue:'||LStructureAggregationValue); $end null;
                UPDATE VW_MIXTURE
                    SET StructureAggregation=LStructureAggregationValue
                    WHERE MixtureID=LMixtureID;
            END IF;

            IF UPPER(LTableName)='VW_FRAGMENT' AND LFragmentXmlValue IS NOT NULL THEN
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry',' LStructureID:'||LStructureID||' LFragmentXmlValue:'||LFragmentXmlValue); $end null;
                UPDATE VW_STRUCTURE
                    SET STRUCTURE=LFragmentXmlValue
                    WHERE StructureID=LStructureID;
            END IF;

            IF UPPER(LTableName)='VW_COMPOUND' THEN
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LNormalizedStructureValue:'||LNormalizedStructureValue||' LCompoundID:'||LCompoundID); $end null;
                IF LNormalizedStructureValue IS NOT NULL THEN
                    SELECT  extractvalue(XmlType(LXmlRows),'VW_Compound/ROW[1]/node()[1]')
                               INTO LCompoundIDTemp
                               FROM dual;
                    UPDATE VW_COMPOUND
                        SET NORMALIZEDSTRUCTURE=LNormalizedStructureValue
                        WHERE COMPOUNDID=LCompoundIDTemp;

                    IF LStructureIDToUpdate IS NOT NULL THEN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureIDToUpdate='||LStructureIDToUpdate); $end null;
                        DeleteStructure(LStructureIDToUpdate,LMessage);
                    END IF;
                END IF;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureIDToUpdate='||LStructureIDToUpdate); $end null;
                IF LStructureIDToUpdate IS NOT NULL THEN

                    DeleteStructure(LStructureIDToUpdate,LMessage);
                END IF;
            END IF;

        END IF;

    END LOOP;

    IF LSomeUpdate THEN

        --Build Message Logs
        LBriefMessage := 'Total: ' || cast(LRowsProcessed as string) || ' Row/s Processed.';
        LMessage := LMessage || chr(10) || LBriefMessage ||CHR(13);

        IF LXMLRegNumberDuplicated IS NOT NULL THEN
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'LXMLRegNumberDuplicated->'||LXMLRegNumberDuplicated.getClobVal()); $end NULL;
            IF LXslTablesTransformed.ExistsNode('VW_RegistryNumber[1]/ROW/PERSONREGISTERED/text()')=1 THEN
              SaveRegNumbersDuplicated(LXMLRegNumberDuplicated,LXmlTables.extract('/MultiCompoundRegistryRecord/PersonCreated/text()').getStringVal());
            ELSE
              SaveRegNumbersDuplicated(LXMLRegNumberDuplicated,NULL);
            END IF;
        END IF;
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'LRegNumber->'||LRegNumber); $end NULL;
        IF LRegNumber IS NOT NULL THEN
            OnRegistrationUpdate(LMixtureRegNumber);
            IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'Before RetrieveMultiCompoundRegistry'); $end NULL;
                RetrieveMultiCompoundRegistry(LMixtureRegNumber, LXMLRegistryRecord, ASectionsList);
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'LXMLRegistryRecord->'||LXMLRegistryRecord); $end NULL;
            END IF;
            AMessage := CreateRegistrationResponse(LBriefMessage, NULL, LXMLRegistryRecord);
        END IF;
    ELSE
        --Build Message Logs
        LMessage := LMessage || chr(10) || 'Total: ' || cast(LRowsProcessed as string) || ' Row/s Processed.'||CHR(13);
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSomeUpdate->false There aren''t fields/sections to update/insert.'); $end null;
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('No fields/sections to update/insert.'));
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Finished -->'||chr(10) ||LMessage); $end null;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LMessage := LMessage || ' ' || cast(0 as string) || ' Row/s Updated on "' || LTableName || '".'|| chr(13) ||'Total: 0 Row/s Updated.';
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eUpdateMultiCompoundRegistry, AppendError('UpdateMultiCompoundRegistry', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;


PROCEDURE DeleteMultiCompoundRegistry(ARegNumber in VW_RegistryNumber.regnumber%type) IS
    CURSOR C_Components(ARegNumber VW_RegistryNumber.regnumber%type)  IS
      SELECT MC.CompoundID,MC.MixtureID
            FROM VW_Mixture_Component MC,VW_Mixture M,VW_RegistryNumber R
            WHERE MC.MixtureID=M.MixtureID AND M.RegID=R.RegID AND R.RegNumber=ARegNumber;

    CURSOR C_Batches(ARegNumber VW_RegistryNumber.regnumber%type)  IS
        SELECT B.BatchID,M.MixtureID
            FROM VW_Batch B,VW_RegistryNumber R, VW_Mixture M
            WHERE B.RegID=R.RegID AND R.RegNumber=ARegNumber AND M.RegID=R.RegID;
   LMessage                  CLOB:='';
   LRegID Number;

   LIsNotEditableDeleting EXCEPTION;

BEGIN

    $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegistry','Begining mixture deleted process. ARegNumber='||ARegNumber); $end null;

    IF GetIsEditable(ARegNumber)='False' then
        RAISE LIsNotEditableDeleting ;
    END IF;

    LMessage:=chr(10) || 'Begining mixture deleted process.';

    BEGIN
        SELECT RegID
            INTO LRegID
            FROM VW_RegistryNumber R
            WHERE R.RegNumber=ARegNumber;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            LMessage:=chr(10) || '0 Row/s found on "VW_RegistryNumber".';
            RAISE_APPLICATION_ERROR(eInvalidRegNum,
                AppendError('The Registry "'||ARegNumber||'" does not exist.'));
    END;

    FOR  R_Components IN C_Components(ARegNumber) LOOP
        LMessage:=LMessage||chr(10) || 'Deleting CompoundID='||R_Components.CompoundID;
        $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegistry','Deleting R_Components.CompoundID='||R_Components.CompoundID||'Deleting R_Components.MixtureID='||R_Components.MixtureID); $end null;
        DeleteCompound(R_Components.CompoundID,R_Components.MixtureID,LMessage);
    END LOOP;

    DELETE VW_Mixture_Component
        WHERE MixtureID IN (SELECT M.MixtureID
                                FROM VW_RegistryNumber RN,VW_Mixture M
                                WHERE RN.RegID=M.RegID AND RN.RegNumber=ARegNumber);

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture_Component".';

    DELETE VW_Compound_Identifier WHERE RegID = LRegID;

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Identifie".';

    DELETE VW_Batch_Project WHERE BatchID IN (SELECT B.BatchID FROM VW_Batch B,VW_RegistryNumber RN WHERE B.RegID = LRegID );

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Batch_Project".';

    DELETE VW_BatchIdentifier WHERE BatchID IN (SELECT B.BatchID FROM VW_Batch B WHERE B.RegID = LRegID);

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchIdentifier".';

    DELETE VW_Batch WHERE RegID = LRegID;

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Batch".';

    DELETE VW_Mixture WHERE RegID = LRegID;

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture".';

    DELETE VW_RegistryNumber_Project WHERE RegID = LRegID;

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_RegistryNumber_Project".';

    DELETE VW_RegistryNumber WHERE  RegID = LRegID;

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_RegistryNumber".';

    $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegistry','Finished -->'||chr(10) ||LMessage); $end null;
EXCEPTION
    WHEN LIsNotEditableDeleting THEN
        RAISE_APPLICATION_ERROR(eIsNotEditableDeleting, 'You are not authorized to delete this record. (Registry# '||ARegNumber||')');
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegistry','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eDeleteMultiCompoundRegistry, AppendError('DeleteMultiCompoundRegistry', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;


PROCEDURE RetrieveMultiCompoundRegList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob) IS
       -- Autor: Fari
       -- Date:17-May-07
       -- Object:
       -- Pending:
      LMessage                  CLOB:='';
      LXml                      CLOB:='';
      LXmlList                  CLOB:='';
      LSectionList   CONSTANT   VARCHAR2(500):='Compound,Fragment,Batch,Mixture,Identifier';
      LRegNumber                VW_RegistryNumber.RegNumber%type;
      LIndex                    Number;
BEGIN
    LIndex:=1;
    LOOP
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegList','AXmlRegIDs->'||AXmlRegNumbers); $end null;
        SELECT  extractValue(XmlType(AXmlRegNumbers),'REGISTRYLIST/node()['||LIndex||']/node()[1]')
          INTO LRegNumber
          FROM dual;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegList','LRegNumber->'||LRegNumber); $end null;
    EXIT WHEN LRegNumber IS NULL;
        BEGIN
            RetrieveMultiCompoundRegistry(LRegNumber,LXml,LSectionList);
            LXmlList:=LXmlList||CHR(10)||LXml;
        EXCEPTION
           WHEN OTHERS THEN
           BEGIN
                IF INSTR(DBMS_UTILITY.format_error_stack,eNoRowsReturned)<>0 THEN NULL; --Though a Compound doesn't exist to get the others
                ELSE
                   RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList,DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
                END IF;
           END;
        END;
        LIndex:=LIndex+1;
    END LOOP;
    AXmlCompoundList:='<MultiCompoundRegistryRecordList>'||CHR(10)||LXmlList||CHR(10)||'</MultiCompoundRegistryRecordList>';
    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegList','List->'||AXmlCompoundList); $end null;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList, AppendError('RetrieveMultiCompoundRegList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

PROCEDURE CreateMultiCompoundRegTmp(
  AXml in CLOB
  , ATempID out Number
  , AMessage OUT CLOB
  , ASectionsList IN Varchar2 := NULL
) IS
    /*
         Autor: Fari
         Date:09-apr-07
         Object: Insert a single compound record temporary
         Description: Look over a Xml searching each Table and insert the rows on it.
         Pending.
             Optimize repase of'&lt;' and &gt;'
     */

    LinsCtx                   DBMS_XMLSTORE.ctxType;
    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlCompReg               CLOB;
    LXmlRows                  CLOB;
    LXmlTypeRows              XmlType;
    LXmlSequenceType          XmlSequenceType;
    LXmlSequenceTypeField     XmlSequenceType;
    LXmlField                 CLOB;
    LIndex                    Number:=0;
    LRowsInserted             Number:=0;
    LTableName                CLOB;
    LBriefMessage             CLOB;
    LMessage                  CLOB:='';

    LPosTagBegin              Number:=0;
    LPosTagEnd                Number:=0;
    LTagXmlFieldBegin         VARCHAR2(10):='<XMLFIELD>';
    LTagXmlFieldEnd           VARCHAR2(11):='</XMLFIELD>';

    LTempCompoundID                 Number:=0;
    LTempBatchID                    Number:=0;

    LStructureValue                 CLOB;
    LStructuresList                 CLOB;
    LFragmentXmlValue               CLOB;
    LBatchCompFragmentXMLValue      CLOB;
    LFragmentXmlList                CLOB;
    LBatchComponentFragmentXMLList  CLOB;
    LNormalizedStructureList        CLOB;
    LNormalizedStructureValue       CLOB;
    LStructureAggregationList       CLOB;
    LStructureAggregationValue      CLOB;
    LXMLRegistryRecord              CLOB;

    LStructureID                    VARCHAR2(8);

    LProjectsSequenceType           XmlSequenceType;
    LProjectName                    VW_PROJECT.Name%Type;
    LProjectdescription             VW_PROJECT.Description%Type;
    LProjectID                      VW_PROJECT.ProjectID%Type;
    LProjectsIndex                  Number;
    LIdentifiersSequenceType        XmlSequenceType;
    LIdentifierName                 VW_IdentifierType.Name%Type;
    LIdentifierdescription          VW_IdentifierType.Description%Type;
    LIdentifierID                   VW_IdentifierType.ID%Type;
    LIdentifiersIndex               Number;


    LXslTables XmlType := cXslCreateMCRRTemp;

BEGIN
    SetSessionParameter;

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','AXml= '||AXml); $end null;

    LXmlCompReg:=AXml;

    -- Take Out the Structures because XmlType don't suport > 64k.
    --LFragmentXmlList:=TakeOffAndGetClobsList(LXmlCompReg,'<Fragment>','<FragmentList>');
    LFragmentXmlList:=TakeOffAndGetClobslist(LXmlCompReg,'<FragmentList');
    LBatchComponentFragmentXMLList:=TakeOffAndGetClobslist(LXmlCompReg,'<BatchComponentFragmentList');
    LStructuresList:=TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure','<BaseFragment>');
    --LStructuresList:=TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure',FALSE,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LStructuresListt= '||LStructuresList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlCompReg sin Structures2= '||LXmlCompReg); $end null;
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure','<Structure');
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LNormalizedStructureList= '||LNormalizedStructureList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlCompReg sin Structures3= '||LXmlCompReg); $end null;
    LStructureAggregationList:=TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation');
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LStructureAggregationList= '||LStructureAggregationList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlCompReg sin Structures4= '||LXmlCompReg); $end null;

    -- Get the xml
    LXmlTables:=XmlType.CreateXML(LXmlCompReg);

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlTables= '||LXmlTables.getClobVal()); $end null;

    -- Build a new formatted Xml
    LXslTablesTransformed:=LXmlTables.Transform(LXslTables);

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXslTablesTransformed= '||LXslTablesTransformed.getClobVal()); $end null;

    --Get ID
    LTempBatchID:=LXmlTables.Extract('node()[1]/ID/text()').getNumberVal();
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LTempBatchID= '||LTempBatchID); $end null;

    --Look over Xml searching each Table and insert the rows of it.
    SELECT XMLSequence(LXslTablesTransformed.Extract('/node()'))
        INTO LXmlSequenceType
        FROM DUAL;

    FOR LIndex IN  LXmlSequenceType.FIRST..LXmlSequenceType.LAST LOOP
        --Search each Table
        LXmlTypeRows:=LXmlSequenceType(LIndex);
        LTableName:= LXmlTypeRows.GetRootElement();

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LTableName= '||LTableName||CHR(10)||' LXmlRows= '||LXmlRows); $end null;

        --Customization for each View - Use of Sequences and parser for Strcutres
        CASE UPPER(LTableName)
            WHEN 'VW_TEMPORARYBATCH' THEN
                BEGIN
                    --Use of Sequences
                    IF NVL(LTempBatchID,0)=0 THEN
                      SELECT SEQ_TEMPORARY_BATCH.NEXTVAL INTO LTempBatchID FROM DUAL;
                    END IF;

                    ATempID:=LTempBatchID;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYBATCH LTempBatchID= '||LTempBatchID); $end null;

                    LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LStructureAggregationValue= '||LStructureAggregationValue); $end null;

                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/TEMPBATCHID/text()'     ,LTempBatchID
                                                 ,'/node()/ROW/DATECREATED/text()'     ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/DATELASTMODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    --Project List section
                    BEGIN
                        SELECT XMLSequence(LXmlTables.Extract('/node()/ProjectList/Project/ProjectID'))
                            INTO LProjectsSequenceType
                            FROM DUAL
                            WHERE ExistsNode(LXmlTables,'/node()/ProjectList/Project/ProjectID')=1;

                         $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectsSequenceType= '||LProjectsSequenceType(1).getclobVal()); $end null;

                         FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectsSequenceType= '||LProjectsSequenceType(LProjectsIndex).getclobVal()); $end null;
                            LProjectID:=LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                            IF LProjectID IS NOT NULL THEN
                                SELECT Name,Description
                                    INTO LProjectName,LProjectDescription
                                    FROM VW_Project WHERE ProjectID=LProjectID;
                                 $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectName= '||LProjectName); $end null;

                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/PROJECTXML/XMLFIELD/ProjectList/Project[ProjectID='||LProjectID||']/ProjectID'
                                                             ,XmlType('<ProjectID name="'||LProjectName||'" description="'||LProjectDescription||'">'||LProjectID||'</ProjectID>'))
                                    INTO LXmlTypeRows
                                    FROM dual;
                            END IF;
                         END LOOP;
                     EXCEPTION
                        WHEN NO_DATA_FOUND THEN NULL;
                     END;
                     --Project Batch List section
                    BEGIN
                        SELECT XMLSequence(LXmlTables.Extract('/node()/BatchList/Batch/ProjectList/Project/ProjectID'))
                            INTO LProjectsSequenceType
                            FROM DUAL
                            WHERE ExistsNode(LXmlTables,'/node()/BatchList/Batch/ProjectList/Project/ProjectID')=1;

                         $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectsSequenceType (Batch)= '||LProjectsSequenceType(1).getclobVal()); $end null;

                         FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectsSequenceType(Batch)= '||LProjectsSequenceType(LProjectsIndex).getclobVal()); $end null;
                            LProjectID:=LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                            IF LProjectID IS NOT NULL THEN
                                SELECT Name,Description
                                    INTO LProjectName,LProjectDescription
                                    FROM VW_Project WHERE ProjectID=LProjectID;
                                 $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectName (Batch)= '||LProjectName); $end null;

                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/PROJECTXMLBATCH/XMLFIELD/ProjectList/Project[ProjectID='||LProjectID||']/ProjectID'
                                                             ,XmlType('<ProjectID name="'||LProjectName||'" description="'||LProjectDescription||'">'||LProjectID||'</ProjectID>'))
                                    INTO LXmlTypeRows
                                    FROM dual;
                            END IF;
                         END LOOP;
                     EXCEPTION
                        WHEN NO_DATA_FOUND THEN NULL;
                     END;
                     --Identifier List section
                     BEGIN
                        SELECT XMLSequence(LXmlTables.Extract('/node()/IdentifierList/Identifier/IdentifierID'))
                            INTO LIdentifiersSequenceType
                            FROM DUAL
                            WHERE ExistsNode(LXmlTables,'/node()/IdentifierList/Identifier/IdentifierID')=1;

                         $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifiersSequenceType= '||LIdentifiersSequenceType(1).getclobVal()); $end null;

                         FOR LIdentifiersIndex IN  LIdentifiersSequenceType.FIRST..LIdentifiersSequenceType.LAST LOOP
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifiersSequenceType= '||LIdentifiersSequenceType(LIdentifiersIndex).getclobVal()); $end null;
                            LIdentifierID:=LIdentifiersSequenceType(LIdentifiersIndex).Extract('IdentifierID/text()').getNumberVal();
                            IF LIdentifierID IS NOT NULL THEN
                                SELECT Name,Description
                                    INTO LIdentifierName,LIdentifierDescription
                                    FROM VW_IdentifierType WHERE ID=LIdentifierID;
                                 $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifierName= '||LIdentifierName); $end null;

                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/IDENTIFIERXML/XMLFIELD/IdentifierList/Identifier[IdentifierID='||LIdentifierID||']/IdentifierID'
                                                             ,XmlType('<IdentifierID name="'||LIdentifierName||'" description="'||LIdentifierDescription||'">'||LIdentifierID||'</IdentifierID>'))
                                    INTO LXmlTypeRows
                                    FROM dual;
                            END IF;
                         END LOOP;
                     EXCEPTION
                        WHEN NO_DATA_FOUND THEN NULL;
                     END;
                     --Identifier Batch List section
                     BEGIN
                        SELECT XMLSequence(LXmlTables.Extract('/node()/BatchList/Batch/IdentifierList/Identifier/IdentifierID'))
                            INTO LIdentifiersSequenceType
                            FROM DUAL
                            WHERE ExistsNode(LXmlTables,'//node()/BatchList/Batch/IdentifierList/Identifier/IdentifierID')=1;

                         $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifiersSequenceType= '||LIdentifiersSequenceType(1).getclobVal()); $end null;

                         FOR LIdentifiersIndex IN  LIdentifiersSequenceType.FIRST..LIdentifiersSequenceType.LAST LOOP
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifiersSequenceType= '||LIdentifiersSequenceType(LIdentifiersIndex).getclobVal()); $end null;
                            LIdentifierID:=LIdentifiersSequenceType(LIdentifiersIndex).Extract('IdentifierID/text()').getNumberVal();
                            IF LIdentifierID IS NOT NULL THEN
                                SELECT Name,Description
                                    INTO LIdentifierName,LIdentifierDescription
                                    FROM VW_IdentifierType WHERE ID=LIdentifierID;
                                 $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifierName= '||LIdentifierName); $end null;

                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/IDENTIFIERXMLBATCH/XMLFIELD/IdentifierList/Identifier[IdentifierID='||LIdentifierID||']/IdentifierID'
                                                             ,XmlType('<IdentifierID name="'||LIdentifierName||'" description="'||LIdentifierDescription||'">'||LIdentifierID||'</IdentifierID>'))
                                    INTO LXmlTypeRows
                                    FROM dual;
                            END IF;
                         END LOOP;
                     EXCEPTION
                        WHEN NO_DATA_FOUND THEN NULL;
                     END;
                END;
            WHEN 'VW_TEMPORARYCOMPOUND' THEN
                BEGIN
                    SELECT SEQ_TEMPORARY_COMPOUND.NEXTVAL INTO LTempCompoundID FROM DUAL;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LTempCompoundID= '||LTempCompoundID); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LStructuresList='||LStructuresList); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LFragmentXmlList='||LFragmentXmlList); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LBatchComponentFragmentXMLList='||LBatchComponentFragmentXMLList); $end null;

                    LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');

                    LStructureID:= LXmlTypeRows.extract('/VW_TemporaryCompound/ROW/STRUCTUREID/text()').getNumberVal();
                    IF LStructureID='-1' THEN
                        SELECT Structure
                            INTO LStructureValue
                            FROM VW_Structure
                            WHERE StructureID=-1;
                    END IF;

                    LFragmentXmlValue:='<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
                    LBatchCompFragmentXMLValue:='<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchComponentFragmentXMLList,'Clob')||'</BatchComponentFragmentList>';
                    LNormalizedStructureValue:=TakeOffAndGetClob(LNormalizedStructureList,'Clob');

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LStructureValue= '|| LStructureValue ); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LFragmentXmlValue= '|| LFragmentXmlValue ); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LBatchCompFragmentXMLValue= '|| LBatchCompFragmentXMLValue ); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LStructureID= '|| LStructureID ); $end null;
                    IF NVL(LStructureID,0)<=-2 THEN  -- (LStructureID= -2,-3)
                        SELECT Structure
                            INTO LStructureValue
                            FROM VW_Structure
                            WHERE StructureID=LStructureID;
                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LStructureValue= '|| LStructureValue ); $end null;
                    END IF;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LXmlTypeRows.getClobVal1= '|| LXmlTypeRows.getClobVal ); $end null;
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/TEMPCOMPOUNDID/text()'  ,LTempCompoundID
                                                 ,'/node()/ROW/TEMPBATCHID/text()'     ,LTempBatchID
                                                 ,'/node()/ROW/DATECREATED/text()'     ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/DATELASTMODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                END;
             WHEN 'VW_TEMPORARYBATCHPROJECT' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/TEMPBATCHID/text()', LTempBatchID)
                        INTO LXmlTypeRows
                        FROM dual;
                     $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LXmlTypeRows= '|| LXmlTypeRows.getClobVal ); $end null;

                END;
             WHEN 'VW_TEMPORARYREGNUMBERSPROJECT' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/TEMPBATCHID/text()', LTempBatchID)
                        INTO LXmlTypeRows
                        FROM dual;
                     $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LXmlTypeRows= '|| LXmlTypeRows.getClobVal ); $end null;

                END;
            ELSE
                --Build Message Logs
                LMessage := LMessage || ' "' || LTableName || '" table stranger.'||CHR(13);

        END CASE;

        LXmlRows:=LXmlTypeRows.getClobVal;
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlRows1= '||LXmlRows); $end null;

        --PropertyList fields: Replace '&lt;' and '&lt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        LXmlRows:=replace(replace(LXmlRows,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlRows2= '||LXmlRows); $end null;

        --XML fields: Replace '<' and '>'  by '&lt;' and '&lt;'' to the XML fields . Necesary to save a xml field using insertXML. I can't to do it using "XmlTransform".
        LPosTagBegin:=1;
        LOOP
            LPosTagBegin := INSTR(LXmlRows,LTagXmlFieldBegin,LPosTagBegin);
            LPosTagEnd   := INSTR(LXmlRows,LTagXmlFieldEnd,LPosTagBegin);

              $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LOOP LXmlRows='||LXmlRows||' LPosTagBegin='||LPosTagBegin||' LPosTagEnd='||LPosTagEnd); $end null;

        EXIT WHEN (LPosTagBegin=0) or (LPosTagEnd=0);
            LXmlField:= SUBSTR(LXmlRows,LPosTagBegin+LENGTH(LTagXmlFieldBegin),LPosTagEnd-(LPosTagBegin+LENGTH(LTagXmlFieldBegin)));

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlField='||LXmlField||' '||LPosTagBegin||' '||LTagXmlFieldEnd||LPosTagEnd); $end null;

            LXmlField:= replace(replace(LXmlField,'<','&lt;') ,'>','&gt;');

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' Before LXmlRows='||LXmlRows); $end null;
            LXmlRows:= SUBSTR(LXmlRows,1,LPosTagBegin-1)||LXmlField|| SUBSTR(LXmlRows,LPosTagEnd+LENGTH(LTagXmlFieldEnd),LENGTH(LXmlRows)) ;

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' After LXmlRows='||LXmlRows); $end null;
        END LOOP;


        --Create the Table Context
        LinsCtx := DBMS_XMLSTORE.newContext(LTableName);
        DBMS_XMLSTORE.clearUpdateColumnList(LinsCtx);

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',LTableName||'->'||LXmlRows); $end null;

        SELECT XMLSequence(XmlType(LXmlRows).Extract('/node()/node()/node()'))
            INTO LXmlSequenceTypeField
            FROM DUAL;
        FOR LIndex IN  LXmlSequenceTypeField.FIRST..LXmlSequenceTypeField.LAST LOOP
            DBMS_XMLSTORE.SetupDateColumn (LinsCtx, UPPER(LXmlSequenceTypeField(LIndex).GetRootElement()));
        END LOOP;

        --Insert Rows and get count it inserted
        LRowsInserted := DBMS_XMLSTORE.insertXML(LinsCtx, LXmlRows );

        --Build Message Logs
        LMessage:=LMessage || ' ' || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';

        --Close the Table Context
        DBMS_XMLSTORE.closeContext(LinsCtx);

        --When structure is more length than 4000 we can't use insertXML
       $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','BEGIN UPDATE VW_TEMPORARYCOMPOUN'); $end null;

        IF UPPER(LTableName)='VW_TEMPORARYCOMPOUND' THEN
            IF LBatchCompFragmentXMLValue IS NOT NULL THEN
                IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXMLValue, NormalizedStructure=LNormalizedStructureValue
                        WHERE TempCompoundID=LTempCompoundID;
                ELSIF LStructureValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                      SET BASE64_CDX=LStructureValue, BatchCompFragmentXML=LBatchCompFragmentXMLValue,NormalizedStructure=LNormalizedStructureValue
                      WHERE TempCompoundID=LTempCompoundID;
                ELSIF LFragmentXmlValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                      SET FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXMLValue,NormalizedStructure=LNormalizedStructureValue
                      WHERE TempCompoundID=LTempCompoundID;
                END IF;
            ELSE
                IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                        WHERE TempCompoundID=LTempCompoundID;
                ELSIF LStructureValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                      SET BASE64_CDX=LStructureValue, NormalizedStructure=LNormalizedStructureValue
                      WHERE TempCompoundID=LTempCompoundID;
                ELSIF LFragmentXmlValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                      SET FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                      WHERE TempCompoundID=LTempCompoundID;
                END IF;
            END IF;
       ELSIF UPPER(LTableName)='VW_TEMPORARYBATCH' THEN
          IF LStructureAggregationValue IS NOT NULL THEN
            UPDATE VW_TEMPORARYBATCH
                SET StructureAggregation=LStructureAggregationValue
                WHERE TempBatchID=LTempBatchID;
          END IF;

       END IF;

       $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp', 'END UDAPTE VW_TEMPORARYCOMPOUN'||CHR(13)||LMessage); $end null;

    END LOOP;

    --conditionally fetch the new record into the output message
    IF ATempID is not NULL THEN
        IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
            RetrieveMultiCompoundRegTmp(ATempID, LXMLRegistryRecord);
        END IF;
        AMessage := CreateRegistrationResponse('Temporary registration created successfully.', NULL, LXMLRegistryRecord);
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LMessage := LMessage || ' ' || cast(0 as string) || ' Row/s Inserted on "' || LTableName || '".';
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eCreateMultiCompoundRegTmp,CHR(13)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE||'Process Log:'||CHR(13)||LMessage||CHR(13));
    END;
END;

FUNCTION  GetFragmentXML( ATempCompundID  in Number) RETURN CLOB IS
    LFragmentXML  CLOB;

    CURSOR C_Fragments(ATempCompoundID in VW_TEMPORARYCOMPOUND.TempCompoundID%type) IS
        SELECT To_Clob('<Fragment><FragmentID>'||Extract(value(FragmentXML), '/FragmentID/text()').GetStringVal()||'</FragmentID><Code>'||Code||'</Code><Description>'||Description||'</Description><DateCreated>'||CREATED||'</DateCreated><DateLastModified>'||MODIFIED||'</DateLastModified><Structure><Structure molWeight="'||MOLWEIGHT||'" formula="'||FORMULA||'">'||STRUCTURE||'</Structure></Structure></Fragment>') FragmentXML
            FROM VW_FRAGMENT F,Table(SELECT XMLSequence(XmlType(TB.FragmentXML).Extract('/FragmentList/Fragment/FragmentID')) FROM VW_TEMPORARYCOMPOUND TB WHERE ATempCompoundID=TempCompoundID AND TB.FragmentXML IS NOT NULL) FragmentXML
            WHERE F.FragmentID(+)=Extract(value(FragmentXML), '/FragmentID/text()').GetStringVal();
BEGIN
    --** Get Fragment**

    LFragmentXML:='<FragmentList>';
    FOR R_Fragment IN C_Fragments(ATempCompundID) LOOP
        LFragmentXML:=LFragmentXML||R_Fragment.FragmentXML;
    END LOOP;
    LFragmentXML:=LFragmentXML||'</FragmentList>';

    RETURN LFragmentXML;
END;

FUNCTION  GetIdentifierCompundXML( ATempCompundID  in Number) RETURN CLOB IS
    LIdentifierXML  CLOB;

  CURSOR C_Identifiers(ATempCompoundID in VW_TEMPORARYCOMPOUND.TempCompoundID%type) IS
    SELECT
      To_Clob(
        '<Identifier><IdentifierID '
        || 'Name="' || Name || '" '
        || 'Description="' || Description || '" '
        || 'Active="' ||Active || '">'
        || Extract(value(IdentifierXML), '/Identifier/IdentifierID/text()').GetStringVal()
        || '</IdentifierID><InputText>'
        || Extract(value(IdentifierXML), '/Identifier/InputText/text()').GetStringVal()
        || '</InputText></Identifier>'
      ) IdentifierXML
      FROM
        VW_IdentifierType IT, Table(
          SELECT XMLSequence(XmlType(TC.IdentifierXML).Extract('/IdentifierList/Identifier'))
          FROM VW_TemporaryCompound TC
          WHERE TempCompoundID=ATempCompoundID
            AND TC.IdentifierXML IS NOT NULL
        ) IdentifierXML
      WHERE IT.ID(+) =
        Extract(value(IdentifierXML), '/Identifier/IdentifierID/text()').GetStringVal()
        --ExtractValue(value(IdentifierXML), '/Identifier/IdentifierID')
        ;

BEGIN
    --** Get Fragment**

    LIdentifierXML:='<IdentifierList>';
    FOR R_Identifier IN C_Identifiers(ATempCompundID) LOOP
        LIdentifierXML:=LIdentifierXML||R_Identifier.IdentifierXML;
    END LOOP;
    LIdentifierXML:=LIdentifierXML||'</IdentifierList>';

    RETURN LIdentifierXML;
END;

PROCEDURE RetrieveMultiCompoundRegTmp( ATempID  in Number, AXml out NOCOPY clob) IS

       -- Autor: Fari
       -- Date:11-jun-077
       -- Object:
       -- Pending:
            --Optimize use Replace with XSLT

    LQryCtx                   DBMS_XMLGEN.ctxHandle;
    LXmlMixture               CLOB;
    LXmlComponent             CLOB;
    LXmlQuery                 CLOB;
    LXmlBatch                 CLOB;
    LXml                      CLOB;
    LSelect                   CLOB;
    LXmlBatchComponent        CLOB;

    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlResult                XmlType;
    LMessage                  CLOB:='';
    LStructuresList           CLOB;
    LNormalizedStructureList  CLOB;
    LStructureAggregationList CLOB;
    LFragmentXmlList          CLOB;

    LCoeObjectConfigField     XmlType;

    LIndex                    Number;
    LXmlRows                  CLOB;

    LMixtureFields            CLOB;
    LCompoundFields           CLOB;
    LCompoundPickListFields   CLOB;
    LBatchFields              CLOB;
    LBatchComponentFields     CLOB;

    LProjectXML               CLOB;
    LProjectXMLBatch          CLOB;
    LIdentifierXML            CLOB;
    LIdentifierXMLBatch       CLOB;
    LXslTables XmlType := cXslRetrieveMCRRTemp;
    LCoeObjectConfig          XmlType;


    CURSOR C_Identifiers(ATempBatchID in VW_TemporaryBatch.TempBatchID%type) IS
        SELECT To_Clob('<Identifier><IdentifierID Name="'||Name||'" Description="'||Description||'" Active="'||Active||'">'||Extract(value(IdentifierXML), '/Identifier/IdentifierID/text()').GetStringVal()||'</IdentifierID><InputText>'||Extract(value(IdentifierXML), '/Identifier/InputText/text()').GetStringVal()||'</InputText></Identifier>') IdentifierXML
            FROM VW_IdentifierType IT,Table(SELECT XMLSequence(XmlType(TB.IdentifierXML).Extract('/IdentifierList/Identifier')) FROM VW_TemporaryBatch TB WHERE TempBatchID=ATempBatchID AND TB.IdentifierXML IS NOT NULL) IdentifierXML
            WHERE IT.ID(+)=Extract(value(IdentifierXML), '/Identifier/IdentifierID/text()').GetStringVal();

     CURSOR C_IdentifiersBatch(ATempBatchID in VW_TemporaryBatch.TempBatchID%type) IS
        SELECT To_Clob('<Identifier><IdentifierID Name="'||Name||'" Description="'||Description||'" Active="'||Active||'">'||Extract(value(IdentifierXMLBatch), '/Identifier/IdentifierID/text()').GetStringVal()||'</IdentifierID><InputText>'||Extract(value(IdentifierXMLBatch), '/Identifier/InputText/text()').GetStringVal()||'</InputText></Identifier>') IdentifierXMLBatch
            FROM VW_IdentifierType IT,Table(SELECT XMLSequence(XmlType(TB.IdentifierXMLBatch).Extract('/IdentifierList/Identifier')) FROM VW_TemporaryBatch TB WHERE TempBatchID=ATempBatchID AND TB.IdentifierXMLBatch IS NOT NULL) IdentifierXMLBatch
            WHERE IT.ID(+)=Extract(value(IdentifierXMLBatch), '/Identifier/IdentifierID/text()').GetStringVal();

    CURSOR C_Projects(ATempBatchID in VW_TemporaryBatch.TempBatchID%type) IS
        SELECT To_Clob('<Project><ProjectID Name="'||Name||'" Description="'||Description||'" Active="'||Active||'">'||Extract(value(ProjectXML), '/ProjectID/text()').GetStringVal()||'</ProjectID></Project>') ProjectXML
            FROM VW_Project P,Table(SELECT XMLSequence(XmlType(TB.ProjectXML).Extract('/ProjectList/Project/ProjectID')) FROM VW_TemporaryBatch TB WHERE TempBatchID=ATempBatchID AND TB.ProjectXML IS NOT NULL) ProjectXML
            WHERE P.ProjectID(+)=Extract(value(ProjectXML), '/ProjectID/text()').GetStringVal();

    CURSOR C_ProjectsBatch(ATempBatchID in VW_TemporaryBatch.TempBatchID%type) IS
        SELECT To_Clob('<Project><ProjectID Name="'||Name||'" Description="'||Description||'" Active="'||Active||'">'||Extract(value(ProjectXMLBatch), '/ProjectID/text()').GetStringVal()||'</ProjectID></Project>') ProjectXMLBatch
            FROM VW_Project P,Table(SELECT XMLSequence(XmlType(TB.ProjectXMLBatch).Extract('/ProjectList/Project/ProjectID')) FROM VW_TemporaryBatch TB WHERE TempBatchID=ATempBatchID AND TB.ProjectXMLBatch IS NOT NULL) ProjectXMLBatch
            WHERE P.ProjectID(+)=Extract(value(ProjectXMLBatch), '/ProjectID/text()').GetStringVal() AND ExistsNode(value(ProjectXMLBatch),'/ProjectID/text()')=1;

    PROCEDURE AddNullFields(AFields IN CLOB,AXml IN OUT NOCOPY CLOB,ATagSearch IN CLOB,ABeginXml IN CLOB) IS
        LPosBegin                 NUMBER;
        LPoslast                  NUMBER;
        LField                    VARCHAR2(100);
        LFields                   CLOB;

        LPosField                 NUMBER;

    BEGIN
        LFields:=AFields||',';
        LPosBegin:=0;
        LPoslast:=1;
        LOOP
            LPosBegin:=INSTR(LFields,',',LPoslast);
            LField:=UPPER(SUBSTR(LFields,LPoslast,LPosBegin-LPoslast));
            LPoslast:=LPosBegin+1;
        EXIT WHEN LField IS NULL;
            $if CompoundRegistry.Debuging $then InsertLog('AddNullFields',' LField:'||LField||' AXml='||AXml||' INSTR(AXml,ABeginXml)='||INSTR(AXml,ABeginXml)); $end null;
            LPosField:=INSTR(AXml, '<'||LField,INSTR(AXml,ABeginXml)+1);
            IF LPosField=0 OR LPosField > INSTR(AXml,ATagSearch) THEN
               AXml:=REPLACE(AXml,ATagSearch,' <'||LField||'/>'||CHR(10)||' '||ATagSearch);
            END IF;
        END LOOP;
    END;


BEGIN
    SetSessionParameter;
    --Get Query XML or Get empty template xml
    IF (NVL(ATempID,0)>0) THEN
        --**Get the PropertyList Fields from the XML field

        SELECT XmlType.CreateXml(XML)
            INTO LCoeObjectConfig
            FROM COEOBJECTCONFIG
            WHERE ID=2;

        SELECT XmlTransform(LCoeObjectConfig,XmlType.CreateXml('
          <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
             <xsl:template match="/MultiCompoundRegistryRecord">
                <xsl:for-each select="ComponentList/Component/Compound/PropertyList/Property">TC.<xsl:value-of select="@name"/>,</xsl:for-each>
             </xsl:template>
          </xsl:stylesheet>')).GetStringVal(),
          XmlTransform(LCoeObjectConfig,XmlType.CreateXml('
          <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
             <xsl:template match="/MultiCompoundRegistryRecord">
                <xsl:for-each select="ComponentList/Component/Compound/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
             </xsl:template>
          </xsl:stylesheet>')).GetStringVal()
        INTO LCompoundFields,LCompoundPickListFields
        FROM DUAL;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LCompoundFields='||LCompoundFields||' LCompoundPickListFields='||LCompoundPickListFields); $end null;

        --Get Compound**

        LSelect:='SELECT TC.TempCompoundID,
                    TC.TempBatchID,
                    csCartridge.MolWeight(TC.BASE64_CDX) FORMULAWEIGHT, csCartridge.Formula(TC.BASE64_CDX,'''') MOLECULARFORMULA,TC.PERSONCREATED, TC.DATELASTMODIFIED, TC.DATECREATED,TC.SequenceID,
                    TC.BASE64_CDX STRUCTURE,
                    ''PropertyListBegin'' Aux,'||LCompoundFields||'''PropertyListEnd'' Aux,
                    CompoundRegistry.GetFragmentXML(TC.TempCompoundID) FRAGMENTXML,
                    CompoundRegistry.GetIdentifierCompundXML(TC.TempCompoundID) IDENTIFIERXML,
                    TC.REGID,
                    R.REGNUMBER,
                    DECODE(NVL(R.RegID,0),0,-TC.TempCompoundID,-C.COMPOUNDID) COMPONENTINDEX,
                    TC.NormalizedStructure,
                    TC.UseNormalization,
                    TC.StructureID,
                    TC.Tag
                 FROM VW_TEMPORARYCOMPOUND TC, VW_RegistryNumber R, VW_Compound C
                 WHERE TC.RegID = C.RegID(+) AND TC.RegID=R.RegID(+) AND TC.TempBatchID='||ATempID||' ORDER BY TC.TempCompoundID';

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LSelect='||LSelect); $end null;

        LQryCtx:=DBMS_XMLGEN.newContext(LSelect);

        LXmlQuery:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
        DBMS_XMLGEN.closeContext(LQryCtx);
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery:'|| LXmlQuery); $end null;
        LStructuresList:=TakeOffAndGetClobslist(LXmlQuery,'<STRUCTURE>',NULL,NULL,FALSE);
        LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlQuery,'<NORMALIZEDSTRUCTURE>',NULL,NULL,FALSE);
        LFragmentXMLList:=TakeOffAndGetClobslist(LXmlQuery,'<FRAGMENTXML>',NULL,NULL,FALSE);

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LStructuresList:'|| LStructuresList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LFragmentXMLList:'|| LFragmentXMLList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery SIN ESTRUCTURA:'|| LXmlQuery); $end null;

        LCompoundFields:=SUBSTR(LCompoundFields,4,LENGTH(LCompoundFields));
        LCompoundFields:=Replace(LCompoundFields,'TC.','');

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LCompoundFields II='||LCompoundFields); $end null;

        IF LXmlQuery IS NULL THEN
            RAISE_APPLICATION_ERROR(eGenericException, AppendError('No rows returned.'));
        END IF;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','Replace compound LXmlQuery='||LXmlQuery); $end null;

        LIndex:=0;
        LOOP
            --Search each Compounds
            LIndex:=LIndex+1;
            SELECT XmlType(LXmlQuery).extract('/ROWSET/ROW['||LIndex||']').getClobVal()
              INTO LXmlRows
              FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','erplace compound  LXmlRows='||LXmlRows); $end null;

        EXIT WHEN LXmlRows IS NULL;
            AddAttribPickList(LCompoundPickListFields,LXmlRows,'<COMPOUND>');
            LXmlRows:=Replace(LXmlRows,'<AUX>PropertyListBegin</AUX>','<PropertyList>');
            AddNullFields(LCompoundFields,LXmlRows,'<AUX>PropertyListEnd</AUX>','<COMPOUND>');
            LXmlRows:=Replace(LXmlRows,'<AUX>PropertyListEnd</AUX>','</PropertyList>');
            LXmlComponent:=LXmlComponent || LXmlRows;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','compound LXmlComponent='||LXmlComponent); $end null;
        END LOOP;

        LXmlComponent:='<MultiCompoundRegistryRecord '||'SameBatchesIdentity="'||GetSameBatchesIdentity||'" IsEditable="'||GetIsEditableTmp(ATempID)||'"> <Compound>'||LXmlComponent||'</Compound>';

        --**Get the PropertyList Fields from the XML field (Mixture)
        SELECT XmlTransform(LCoeObjectConfig,XmlType.CreateXml('
            <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                    <xsl:for-each select="PropertyList/Property">
                        <xsl:value-of select="@name"/>,</xsl:for-each>
                </xsl:template>
            </xsl:stylesheet>')).GetStringVal(),
               XmlTransform(LCoeObjectConfig,XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                 <xsl:template match="/MultiCompoundRegistryRecord">
                    <xsl:for-each select="PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                 </xsl:template>
              </xsl:stylesheet>')).GetStringVal()
            INTO LMixtureFields,LCompoundPickListFields
            FROM Dual;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LMixtureFields='||LMixtureFields); $end null;
        --**Get Mixture**
        LSelect:='SELECT
                      ''BatchPropertyListBegin'' Aux,'||LMixtureFields||'''BatchPropertyListEnd'' Aux
                  FROM VW_TEMPORARYBATCH
                  WHERE TempBatchID='||ATempID;

        LQryCtx:=DBMS_XMLGEN.newContext(LSelect);
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LSelect='||LSelect); $end null;
        LXmlMixture:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
        DBMS_XMLGEN.closeContext(LQryCtx);

        -- Get and RegistryNumberProject record
        LQryCtx:=DBMS_XMLGEN.newContext(
        'SELECT P.ProjectID , P.Description, P.Name, P.Active
           FROM VW_TemporaryRegNumbersProject TRNP,VW_Project P
           WHERE TRNP.ProjectID=P.ProjectID AND TRNP.TempBatchID='||ATempID||' ORDER BY P.ProjectID');

        LProjectXML:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
        DBMS_XMLGEN.closeContext(LQryCtx);
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LProjectXML:'|| chr(10)||LProjectXML); $end null;
        LProjectXML:=Replace(LProjectXML,'ROWSET','Project');

        AddAttribPickList(LCompoundPickListFields,LXmlMixture,'<Mixture>');
        LXmlMixture:=Replace(LXmlMixture,'<ROWSET>','<Mixture>');
        LXmlMixture:=Replace(LXmlMixture,'<AUX>BatchPropertyListBegin</AUX>','<PropertyList>');
        AddNullFields(LMixtureFields,LXmlMixture,'<AUX>BatchPropertyListEnd</AUX>','<Mixture>');
        LXmlMixture:=Replace(LXmlMixture,'<AUX>BatchPropertyListEnd</AUX>','</PropertyList>');
        LXmlMixture:=Replace(LXmlMixture,'</ROWSET>','</Mixture>');
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlMixture='||LXmlMixture); $end null;

        --**Get the PropertyList Fields from the XML field
        SELECT XmlTransform(LCoeObjectConfig,XmlType.CreateXml('
                <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                  <xsl:template match="/MultiCompoundRegistryRecord">
                    <xsl:for-each select="BatchList/Batch/PropertyList/Property">
                        <xsl:value-of select="@name"/>,</xsl:for-each>
                  </xsl:template>
                </xsl:stylesheet>')).GetStringVal(),
                XmlTransform(LCoeObjectConfig,XmlType.CreateXml('
                <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                 <xsl:template match="/MultiCompoundRegistryRecord">
                    <xsl:for-each select="BatchList/Batch/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                 </xsl:template>
                </xsl:stylesheet>')).GetStringVal()
            INTO LBatchFields,LCompoundPickListFields
            FROM COEOBJECTCONFIG
            WHERE ID=2;

        --**Get Identifier**
        LIdentifierXML:='<IdentifierList>';
        FOR R_Identifiers IN C_Identifiers(ATempID) LOOP
            LIdentifierXML:=LIdentifierXML||R_Identifiers.IdentifierXML;
        END LOOP;
        LIdentifierXML:=LIdentifierXML||'</IdentifierList>';

        --**Get Identifier Batch**
        LIdentifierXMLBatch:='<IdentifierList>';
        FOR R_Identifiers IN C_IdentifiersBatch(ATempID) LOOP
            LIdentifierXMLBatch:=LIdentifierXMLBatch||R_Identifiers.IdentifierXMLBatch;
        END LOOP;
        LIdentifierXMLBatch:=LIdentifierXMLBatch||'</IdentifierList>';

        --**Get Batch**
        LSelect:='SELECT
                      TempBatchID, BATCHNUMBER,PERSONCREATED, DATELASTMODIFIED, DATECREATED,SequenceID,StatusID,
                      ''BatchPropertyListBegin'' Aux,'||LBatchFields||'''BatchPropertyListEnd'' Aux,
                      '''||LIdentifierXML||''' IDENTIFIERXML,'''||LIdentifierXMLBatch||''' IDENTIFIERXMLBATCH,
                      STRUCTUREAGGREGATION
                  FROM VW_TEMPORARYBATCH
                  WHERE TempBatchID='||ATempID;
         $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LSelect='||LSelect); $end null;

        LQryCtx:=DBMS_XMLGEN.newContext(LSelect);
        LXmlBatch:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
        DBMS_XMLGEN.closeContext(LQryCtx);

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlBatch='||LXmlBatch); $end null;
        LStructureAggregationList:=TakeOffAndGetClobslist(LXmlBatch,'<STRUCTUREAGGREGATION>',NULL,NULL,FALSE);

        -- Get and BatchProject record
        LQryCtx:=DBMS_XMLGEN.newContext(
        'SELECT P.ProjectID , P.Description, P.Name, P.Active
           FROM VW_TemporaryBatchProject TBP,VW_Project P
           WHERE TBP.ProjectID=P.ProjectID AND TBP.TempBatchID='||ATempID||' ORDER BY P.ProjectID');

        LProjectXMLBatch:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
        DBMS_XMLGEN.closeContext(LQryCtx);
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LProjectXMLBatch:'|| chr(10)||LProjectXMLBatch); $end null;
        LProjectXMLBatch:=Replace(LProjectXMLBatch,'ROWSET','Project');


        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LStructureAggregationList='||LStructureAggregationList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlBatch sin StructureAggregation='||LXmlBatch); $end null;


        IF LXmlBatch IS NULL THEN
             RAISE_APPLICATION_ERROR(eGenericException, AppendError('No rows returned.'));
        END IF;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlBatch='||LXmlBatch); $end null;

        AddAttribPickList(LCompoundPickListFields,LXmlBatch,'<Batch>');
        LXmlBatch:=Replace(LXmlBatch,'<ROWSET>','<Batch>');
        LXmlBatch:=Replace(LXmlBatch,'<AUX>BatchPropertyListBegin</AUX>','<PropertyList>');
        AddNullFields(LBatchFields,LXmlBatch,'<AUX>BatchPropertyListEnd</AUX>','<Batch>');
        LXmlBatch:=Replace(LXmlBatch,'<AUX>BatchPropertyListEnd</AUX>','</PropertyList>');
        LXmlBatch:=Replace(LXmlBatch,'</ROW>',LProjectXMLBatch);
        LXmlBatch:=Replace(LXmlBatch,'</ROWSET>','');

        --**Get the PropertyList Fields from the XML field
        SELECT XmlTransform(LCoeObjectConfig,XmlType.CreateXml('
                <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                  <xsl:template match="/MultiCompoundRegistryRecord">
                    <xsl:for-each select="BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property">TC.<xsl:value-of select="@name"/>,</xsl:for-each>
                  </xsl:template>
                </xsl:stylesheet>')).GetStringVal(),
                XmlTransform(LCoeObjectConfig,XmlType.CreateXml('
                <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                 <xsl:template match="/MultiCompoundRegistryRecord">
                    <xsl:for-each select="BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                 </xsl:template>
                </xsl:stylesheet>')).GetStringVal()
            INTO LBatchComponentFields,LCompoundPickListFields
          FROM COEOBJECTCONFIG
         WHERE ID=2;
        --take out the last character and add Alias to first field
        LBatchComponentFields:=Replace(LBatchComponentFields,'COMMENTS','BATCHCOMPONENTCOMMENTS COMMENTS');

        --**Get Batch Component**
        LSelect:='SELECT TC.TempCompoundID, TC.TempBatchID,TC.BatchCompFragmentXML,
                         DECODE(NVL(r.regid,0),0,-TC.TempCompoundID,-C.COMPOUNDID) COMPONENTINDEX,
                         ''BatchComponentPropertyListBegin'' Aux,'||LBatchComponentFields||'''BatchComponentPropertyListEnd'' Aux
                    FROM VW_TemporaryCompound TC, VW_RegistryNumber R, VW_Compound C
                   WHERE TC.RegID = C.RegID(+) AND TC.RegID = R.RegID(+)
                     AND TC.TempBatchID = '||ATempID||' ORDER BY TC.TempCompoundID';

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LSelect='||LSelect); $end null;

        LQryCtx:=DBMS_XMLGEN.newContext(LSelect);

        LXmlQuery:=Replace(DBMS_XMLGEN.getXML(LQryCtx),cXmlDecoration,'');
        DBMS_XMLGEN.closeContext(LQryCtx);

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlQuery1='||LXmlQuery); $end null;

        LBatchComponentFields:=SUBSTR(LBatchComponentFields,4,LENGTH(LBatchComponentFields));
        LBatchComponentFields:=Replace(LBatchComponentFields,'TC.','');
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery2='||LXmlQuery); $end null;
        LBatchComponentFields:=Replace(LBatchComponentFields,'BATCHCOMPONENTCOMMENTS COMMENTS','COMMENTS');
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery3='||LXmlQuery); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery4='||LXmlQuery); $end null;

        IF LXmlQuery IS NULL THEN
             RAISE_APPLICATION_ERROR(eGenericException, AppendError('No rows returned.'));
        END IF;

        LIndex:=0;
        LOOP
            --Search each Compounds
            LIndex:=LIndex+1;
            SELECT XmlType(LXmlQuery).extract('/ROWSET/ROW['||LIndex||']').getClobVal()
              INTO LXmlRows
              FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','replace batchcompound  LXmlRows='||LXmlRows); $end null;

        EXIT WHEN LXmlRows IS NULL;
            AddAttribPickList(LCompoundPickListFields,LXmlRows,'<BatchComponent>');
            LXmlRows:=Replace(LXmlRows,'<AUX>BatchComponentPropertyListBegin</AUX>','<PropertyList>');
            AddNullFields(LBatchComponentFields,LXmlRows,'<AUX>BatchComponentPropertyListEnd</AUX>','<BatchComponent>');
            LXmlRows:=Replace(LXmlRows,'<AUX>BatchComponentPropertyListEnd</AUX>','</PropertyList>');
            LXmlBatchComponent:=LXmlBatchComponent || LXmlRows;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','batchcompound LXmlBatchComponent='||LXmlBatchComponent); $end null;
        END LOOP;

        LXmlBatchComponent:='<BatchComponent>'||LXmlBatchComponent||'</BatchComponent>';

        LXml:=LXmlComponent||LXmlMixture||LProjectXML||LXmlBatch||LXmlBatchComponent||'</ROW></Batch></MultiCompoundRegistryRecord> ';

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','MultiCompoundRegTmp SIN TRANSFORMACION:'|| chr(10)||LXml); $end null;

        LXmlTables:=XmlType.CreateXml(LXml);

        -- Build a new formatted Xml
        SELECT XmlTransform(LXmlTables,LXslTables).GetClobVal()
          INTO AXml
          FROM DUAL;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','AXml:'|| chr(10)||AXml); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LStructuresList:'|| chr(10)||LStructuresList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LNormalizedStructureList:'|| chr(10)||LNormalizedStructureList); $end null;

        SELECT XmlType.CreateXml(XML)
            INTO LCoeObjectConfigField
            FROM COEOBJECTCONFIG
            WHERE ID=2;

        LXmlResult:=XmlType(AXml);

        AddTags(LCoeObjectConfigField,LXmlResult,'AddIns',Null);
        AddTags(LCoeObjectConfigField,LXmlResult,'ValidationRuleList','name');

        AXml:= TakeOnAndGetXml(LXmlResult.GetClobVal(),'STRUCTURE',LStructuresList);
        AXml:= TakeOnAndGetXml(AXml,'NORMALIZEDSTRUCTURE',LNormalizedStructureList);
        AXml:= TakeOnAndGetXml(AXml,'STRUCTUREAGGREGATION',LStructureAggregationList);
        AXml:= TakeOnAndGetXml(AXml,'FRAGMENTXML',LFragmentXMLList );

        --Replace '&lt;' and '&gt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        AXml:=replace(replace(replace(AXml,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>'),'&quot;','"');
        AXml:=replace(replace(AXml,'&lt;','<') ,'&gt;','>');

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','AXml:'|| chr(10)||AXml); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LStructuresList:'|| chr(10)||LStructuresList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LNormalizedStructureList:'|| chr(10)||LNormalizedStructureList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LStructureAggregationList:'|| chr(10)||LStructureAggregationList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LFragmentXMLList:'|| chr(10)||LFragmentXMLList); $end null;

        AXml:=replace(AXml,'<FRAGMENTXML>','');
        AXml:=replace(AXml,'</FRAGMENTXML>','');
        AXml:=replace(AXml,'<BATCHCOMPFRAGMENTXML>','');
        AXml:=replace(AXml,'</BATCHCOMPFRAGMENTXML>','');
        AXml:=replace(AXml,'<IDENTIFIERXML>','');
        AXml:=replace(AXml,'</IDENTIFIERXML>','');

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','MultiCompoundRegistryRecord:'|| chr(10)||AXml); $end null;
    ELSE
        --Validate xml template with the CreateXml object and get it.
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','ATempID:'|| chr(10)||ATempID); $end null;
        SELECT XmlType.CreateXml(XML).GetClobVal()
          INTO AXml
          FROM COEOBJECTCONFIG
          WHERE ID=2;

        AXml:='<MultiCompoundRegistryRecord '||'SameBatchesIdentity="'||GetSameBatchesIdentity || '" ActiveRLS="'||GetActiveRLS||'" '||Substr(AXml,29,Length(AXml));

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','COEOBJECTCONFIG:'|| chr(10)||AXml); $end null;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE || CHR(10) || LMessage); $end null;
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegTmp, AppendError('RetrieveMultiCompoundRegTmp', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

PROCEDURE UpdateMultiCompoundRegTmp(
  AXml in clob
  , AMessage OUT CLOB
  , ASectionsList in Varchar2:=NULL
) IS
     /*
        Autor: Fari
        Date:10-may-20077
        Object:
        Description:
        Pending.
    */
    LCtx                   DBMS_XMLSTORE.ctxType;
    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlCompReg               CLOB;
    LXmlRows                  CLOB;
    LXmlTypeRows              XmlType;
    LXmlField                 CLOB;
    LFieldToUpdate            CLOB;

    LIndex                    Number:=0;
    LFieldIndex               Number:=0;
    LRowsUpdated              Number:=0;
    LRowsDeleted              Number:=0;
    LRowsInserted             Number:=0;
    LTableName                CLOB;
    LMessage                  CLOB:='';
    LUpdate                   BOOLEAN;
    LSomeUpdate               BOOLEAN;
    LKeyFieldName             CLOB;
    LSectionDelete            BOOLEAN;
    LSectionInsert            BOOLEAN;
    LID                       VARCHAR2(1000);

    LPosTagBegin              Number:=0;
    LPosTagEnd                Number:=0;
    LTagXmlFieldBegin         VARCHAR2(10):='<XMLFIELD>';
    LTagXmlFieldEnd           VARCHAR2(11):='</XMLFIELD>';

    LTempCompoundID                   Number:=0;
    LTempCompoundIDTag      CONSTANT VARCHAR2(20):='<TEMPCOMPOUNDID>';
    LTempCompoundIDTagEnd   CONSTANT VARCHAR2(20):='</TEMPCOMPOUNDID>';

    LTempBatchID                     Number:=0;
    LTempBatchIDTag         CONSTANT VARCHAR2(15):='<TEMPBATCHID>';
    LTempBatchIDTagEnd      CONSTANT VARCHAR2(15):='</TEMPBATCHID>';

    LStructureValue                CLOB;
    LStructuresList                CLOB;
    LFragmentXmlValue              CLOB;
    LFragmentXmlList               CLOB;
    LBatchCompFragmentXmlList      CLOB;
    LBatchCompFragmentXmlValue     CLOB;

    LNormalizedStructureList       CLOB;
    LNormalizedStructureValue      CLOB;
    LStructureAggregationList      CLOB;
    LStructureAggregationValue     CLOB;

    LStructureID                   Number(8);

    LProjectsSequenceType          XmlSequenceType;
    LProjectID                     VW_PROJECT.ProjectID%Type;

    LXslTables XmlType:=XmlType.CreateXml('<?xml version="1.0" encoding="UTF-8"?>
        <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
        <xsl:template match="/MultiCompoundRegistryRecord">
        <MultiCompoundRegistryRecord>
            <xsl:variable name="VMixture" select="."/>
            <xsl:for-each select="BatchList">
                <xsl:for-each select="Batch">
                    <VW_TEMPORARYBATCH>
                        <ROW>
                            <xsl:for-each select="BatchID">
                              <TEMPBATCHID>
                                <xsl:value-of select="."/>
                              </TEMPBATCHID>
                            </xsl:for-each>
                            <xsl:for-each select="BatchNumber[@update=''yes'']">
                              <BATCHNUMBER>
                                <xsl:value-of select="."/>
                              </BATCHNUMBER>
                            </xsl:for-each>
                            <xsl:for-each select="DateCreated[@update=''yes'']">
                              <DATECREATED>
                                 <xsl:value-of select="."/>
                              </DATECREATED>
                            </xsl:for-each>
                            <xsl:for-each select="PersonCreated[@update=''yes'']">
                              <PERSONCREATED>
                                <xsl:value-of select="."/>
                              </PERSONCREATED>
                            </xsl:for-each>
                            <xsl:for-each select="DateLastModified[@update=''yes'']">
                              <DATELASTMODIFIED>
                                <xsl:value-of select="."/>
                              </DATELASTMODIFIED>
                            </xsl:for-each>
                            <xsl:for-each select="StatusID[@update=''yes'']">
                              <STATUSID>
                                <xsl:value-of select="."/>
                              </STATUSID>
                            </xsl:for-each>
                            <xsl:for-each select="StructureAggregation[@update=''yes'']">
                              <STRUCTUREAGGREGATION>
                                <xsl:copy-of select="." />
                              </STRUCTUREAGGREGATION>
                            </xsl:for-each>
                            <xsl:for-each select="PropertyList">
                              <xsl:for-each select="Property[@update=''yes'']">
                                <xsl:variable name="V1" select="."/>
                                <xsl:for-each select="@name">
                                  <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                                  <xsl:choose>
                                    <xsl:when test="$V2 = ''DELIVERYDATE''">
                    LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
                                    <xsl:when test="$V2 = ''DATEENTERED''">
                    LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
                                    <xsl:otherwise>
                    LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:otherwise>
                                  </xsl:choose>
                                </xsl:for-each>
                              </xsl:for-each>
                            </xsl:for-each>

                            <xsl:for-each select="$VMixture/PropertyList">
                                <xsl:for-each select="Property[@update=''yes'']">
                                    <xsl:variable name="V1" select="."/>
                                    <xsl:for-each select="@name">
                                        <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                                            LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
                                </xsl:for-each>
                            </xsl:for-each>
                            <xsl:choose>
                                <xsl:when test="$VMixture/ProjectList/Project[@insert=''yes'']!=''''">
                                    <PROJECTXML>
                                         <XMLFIELD>
                                             <xsl:copy-of select="$VMixture/ProjectList/." />
                                         </XMLFIELD>
                                      </PROJECTXML>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:for-each select="$VMixture/ProjectList[@update=''yes'']">
                                        <PROJECTXML>
                                            <XMLFIELD>
                                                <xsl:copy-of select="." />
                                            </XMLFIELD>
                                        </PROJECTXML>
                                    </xsl:for-each>
                                </xsl:otherwise>
                           </xsl:choose>
                            <xsl:choose>
                                <xsl:when test="$VMixture/IdentifierList/Identifier[@insert=''yes'']!=''''">
                                    <IDENTIFIERXML>
                                        <XMLFIELD>
                                            <xsl:copy-of select="$VMixture/IdentifierList/." />
                                        </XMLFIELD>
                                    </IDENTIFIERXML>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:for-each select="$VMixture/IdentifierList[@update=''yes'']">
                                        <IDENTIFIERXML>
                                            <XMLFIELD>
                                                <xsl:copy-of select="." />
                                            </XMLFIELD>
                                        </IDENTIFIERXML>
                                    </xsl:for-each>
                                </xsl:otherwise>
                           </xsl:choose>
                           <xsl:choose>
                                <xsl:when test="ProjectList/Project[@insert=''yes'']!=''''">
                                    <PROJECTXMLBATCH>
                                         <XMLFIELD>
                                             <xsl:copy-of select="ProjectList/." />
                                         </XMLFIELD>
                                      </PROJECTXMLBATCH>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:for-each select="ProjectList[@update=''yes'']">
                                        <PROJECTXMLBATCH>
                                            <XMLFIELD>
                                                <xsl:copy-of select="." />
                                            </XMLFIELD>
                                        </PROJECTXMLBATCH>
                                    </xsl:for-each>
                                </xsl:otherwise>
                           </xsl:choose>
                            <xsl:choose>
                                <xsl:when test="IdentifierList/Identifier[@insert=''yes'']!=''''">
                                    <IDENTIFIERXMLBATCH>
                                        <XMLFIELD>
                                            <xsl:copy-of select="IdentifierList/." />
                                        </XMLFIELD>
                                    </IDENTIFIERXMLBATCH>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:for-each select="IdentifierList[@update=''yes'']">
                                        <IDENTIFIERXMLBATCH>
                                            <XMLFIELD>
                                                <xsl:copy-of select="." />
                                            </XMLFIELD>
                                        </IDENTIFIERXMLBATCH>
                                    </xsl:for-each>
                                </xsl:otherwise>
                           </xsl:choose>
                        </ROW>
                    </VW_TEMPORARYBATCH>
                    <xsl:for-each select="ProjectList/Project[@delete=''yes'']">
                        <PROJECTXMLBATCH>deleting<ROW>
                                <ID><xsl:value-of select="ProjectID"/></ID>
                            </ROW>
                        </PROJECTXMLBATCH>
                    </xsl:for-each>
                    <xsl:for-each  select="IdentifierList/Identifier[@delete=''yes'']">
                        <IDENTIFIERXMLBATCH>deleting<ROW>
                                <ID><xsl:value-of select="IdentifierID"/></ID>
                            </ROW>
                        </IDENTIFIERXMLBATCH>
                    </xsl:for-each>
                </xsl:for-each>
            </xsl:for-each>
            <xsl:for-each select="ProjectList/Project[@delete=''yes'']">
                <PROJECTXML>deleting<ROW>
                        <ID><xsl:value-of select="ProjectID"/></ID>
                    </ROW>
                </PROJECTXML>
            </xsl:for-each>
            <xsl:for-each select="IdentifierList/Identifier[@delete=''yes'']">
                <IDENTIFIERXMLMIXTURE>deleting<ROW>
                        <ID><xsl:value-of select="IdentifierID"/></ID>
                    </ROW>
                </IDENTIFIERXMLMIXTURE>
            </xsl:for-each>
            <xsl:for-each select="ComponentList/Component">
            <xsl:variable name="VDelete" select="@delete"/>
            <xsl:variable name="VInsert" select="@insert"/>
            <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
            <xsl:for-each select="Compound">
             <VW_TEMPORARYCOMPOUND>
               <xsl:choose>
                 <xsl:when test="$VDelete=''yes''">deleting</xsl:when>
                 <xsl:when test="$VInsert=''yes''">inserting</xsl:when>
               </xsl:choose>
               <ROW>

                <xsl:for-each select="RegNumber/RegID[$VInsert=''yes'']">
                  <REGID>
                    <xsl:choose>
                      <xsl:when test=".=''0''">
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="."/>
                      </xsl:otherwise>
                    </xsl:choose>
                  </REGID>
                </xsl:for-each>

                <xsl:for-each select="CompoundID">
                  <TEMPCOMPOUNDID>
                    <xsl:value-of select="."/>
                  </TEMPCOMPOUNDID>
                </xsl:for-each>
                <xsl:for-each select="CompoundID[$VInsert=''yes'']">
                   <TEMPBATCHID>
                     <xsl:value-of select="."/>
                   </TEMPBATCHID>
                 </xsl:for-each>
                <xsl:for-each select="BaseFragment/Structure/Structure[@update=''yes'' or $VInsert=''yes'']">
                  <BASE64_CDX>
                    <xsl:value-of select="."/>
                  </BASE64_CDX>
                </xsl:for-each>
                <xsl:for-each select="BaseFragment/Structure[@insert=''yes'']/StructureID">
                  <STRUCTUREID>
                    <xsl:value-of select="."/>
                  </STRUCTUREID>
                </xsl:for-each>
                <xsl:for-each select="DateCreated[@update=''yes'' or $VInsert=''yes'']">
                  <DATECREATED>
                    <xsl:value-of select="."/>
                  </DATECREATED>
                </xsl:for-each>
                <xsl:for-each select="PersonCreated[@update=''yes'' or $VInsert=''yes'']">
                  <PERSONCREATED>
                    <xsl:value-of select="."/>
                  </PERSONCREATED>
                </xsl:for-each>
                <xsl:for-each select="DateLastModified[@update=''yes'' or $VInsert=''yes'']">
                  <DATELASTMODIFIED>
                    <xsl:value-of select="."/>
                  </DATELASTMODIFIED>
                </xsl:for-each>
                <xsl:for-each select="Tag[@update=''yes'' or $VInsert=''yes'']">
                  <TAG>
                    <xsl:value-of select="."/>
                  </TAG>
                </xsl:for-each>
                <xsl:for-each select="PropertyList">
                  <xsl:for-each select="Property[@update=''yes'' or $VInsert=''yes'']">
                     <xsl:variable name="V1" select="."/>
                     <xsl:for-each select="@name">
                       <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
                   </xsl:for-each>
                </xsl:for-each>
                <xsl:for-each select="ProjectList[@update=''yes'' or $VInsert=''yes'']">
                  <PROJECTXML>
                    <XMLFIELD>
                      <xsl:copy-of select="." />
                    </XMLFIELD>
                  </PROJECTXML>
                </xsl:for-each>
                <xsl:for-each select="FragmentList[@update=''yes'' or $VInsert=''yes'']">
                  <FRAGMENTXML>
                    <XMLFIELD>
                      <xsl:copy-of select="." />
                    </XMLFIELD>
                  </FRAGMENTXML>
                </xsl:for-each>
                <xsl:for-each select="FragmentList/Fragment[@delete=''yes'']">
                    <FRAGMENTXML>deleting<ROW>
                            <ID><xsl:value-of select="FragmentID"/></ID>
                        </ROW>
                    </FRAGMENTXML>
                </xsl:for-each>
                <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList[@update=''yes'' or $VInsert=''yes'']">
                  <BATCHCOMPFRAGMENTXML>
                    <XMLFIELD>
                      <xsl:copy-of select="." />
                    </XMLFIELD>
                  </BATCHCOMPFRAGMENTXML>
                </xsl:for-each>


                <xsl:choose>
                    <xsl:when test="IdentifierList/Identifier[@insert=''yes'']!=''''">
                        <IDENTIFIERXML>
                            <XMLFIELD>
                                <xsl:copy-of select="IdentifierList/." />
                            </XMLFIELD>
                        </IDENTIFIERXML>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:for-each select="IdentifierList[@update=''yes'' or $VInsert=''yes'']">
                          <IDENTIFIERXML>
                            <XMLFIELD>
                              <xsl:copy-of select="." />
                            </XMLFIELD>
                          </IDENTIFIERXML>
                        </xsl:for-each>
                    </xsl:otherwise>
               </xsl:choose>


                <xsl:for-each select="Structure/NormalizedStructure[@update=''yes'' or $VInsert=''yes'']">
                  <NORMALIZEDSTRUCTURE>
                      <xsl:copy-of select="." />
                  </NORMALIZEDSTRUCTURE>
                </xsl:for-each>
                <xsl:for-each select="BaseFragment/Structure/UseNormalization[@update=''yes'' or $VInsert=''yes'']">
                  <USENORMALIZATION>
                      <xsl:value-of select="."/>
                  </USENORMALIZATION>
                </xsl:for-each>
                <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                  <xsl:for-each select="PropertyList/Property[@update=''yes'' or $VInsert=''yes'']">
                    <xsl:variable name="V1" select="."/>
                    <xsl:for-each select="@name">
                      <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                      <xsl:choose>
                        <xsl:when test="$V2 = ''COMMENTS''">
                          LESS_THAN_SIGN;BATCHCOMPONENTCOMMENTSGREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/BATCHCOMPONENTCOMMENTSGREATER_THAN_SIGN;
                        </xsl:when>
                        <xsl:otherwise>
                          LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;
                        </xsl:otherwise>
                      </xsl:choose>
                    </xsl:for-each>
                  </xsl:for-each>
                </xsl:for-each>
              </ROW>
            </VW_TEMPORARYCOMPOUND>
            <xsl:for-each select="IdentifierList/Identifier[@delete=''yes'']">
                <IDENTIFIERXMLCOMPOUND>deleting<ROW>
                        <ID><xsl:value-of select="IdentifierID"/></ID>
                    </ROW>
                </IDENTIFIERXMLCOMPOUND>
            </xsl:for-each>
           </xsl:for-each>
          </xsl:for-each>
      </MultiCompoundRegistryRecord>
     </xsl:template>
    </xsl:stylesheet>');


    PROCEDURE SetKeyValue(AID VARCHAR2,AIDTag VARCHAR2,AIDTagEnd VARCHAR2) IS
        LPosTag                   Number:=0;
        LPosTagNull               Number:=0;
        LPosTagEnd                Number:=0;
    BEGIN
        LPosTag:=1;
        LOOP
            LPosTagNull := INSTR(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',LPosTag);
            IF LPosTagNull<>0 THEN
                LXmlRows:=REPLACE(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',AIDTag||AIDTagEnd);
            END IF;
           LPosTag := INSTR(LXmlRows,AIDTag,LPosTag);
        EXIT WHEN LPosTag=0;
            LPosTag  := LPosTag + LENGTH(AIDTag)- 1;
            LPosTagEnd := INSTR(LXmlRows,AIDTagEnd,LPosTag);
            LXmlRows:=SUBSTR(LXmlRows,1,LPosTag)||AID||SUBSTR(LXmlRows,LPosTagEnd,LENGTH(LXmlRows));
        END LOOP;
    END;

BEGIN
    SetSessionParameter;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' AXml='||AXml); $end null;

    LXmlCompReg:=AXml;

    LSomeUpdate:=False;
    -- Take Out the Structures because the nodes of XmlType don't suport a size grater 64k.
    LFragmentXmlList:=TakeOffAndGetClobslist(LXmlCompReg,'<FragmentList',NULL,NULL,TRUE,TRUE);

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin FragmentList= '||LXmlCompReg); $end null;
    LBatchCompFragmentXmlList:=TakeOffAndGetClobslist(LXmlCompReg,'<BatchComponentFragmentList',NULL,NULL,TRUE,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin BatchComponentFragmentList= '||LXmlCompReg); $end null;
    LStructuresList:=TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure','<BaseFragment>',TRUE);

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin BaseFragment= '||LXmlCompReg); $end null;
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure',NULL,NULL,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin NormalizedStructure= '||LXmlCompReg); $end null;
    LStructureAggregationList:=TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation',NULL,NULL,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin StructureAggregation= '||LXmlCompReg); $end null;

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LFragmentXmlList= '||LFragmentXmlList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LBatchCompFragmentXmlList= '||LBatchCompFragmentXmlList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LStructuresListt= '||LStructuresList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LNormalizedStructureList= '||LNormalizedStructureList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LStructureAggregationList= '||LStructureAggregationList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin Structures= '||LXmlCompReg); $end null;

    -- Get the xml
    LXmlTables:=XmlType.createXML(LXmlCompReg);

    -- Build a new formatted Xml
    SELECT XmlTransform(LXmlTables,LXslTables)
      INTO LXslTablesTransformed
      FROM DUAL;

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXslTablesTransformed='||LXslTablesTransformed.getClobVal()); $end null;

    --Look over Xml searching each Table and update the rows of it.
     LOOP
        --Search each Table
        LIndex:=LIndex+1;
        SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/node()['||LIndex||']').getClobVal()
        INTO LXmlRows
        FROM dual;

     EXIT WHEN LXmlRows IS NULL;

        LXmlTypeRows:=XmlType(LXmlRows);

        --Get Table Name. Remove  '<' and '>'
        LTableName:= substr(LXmlRows,2,INSTR(LXmlRows,'>')-2);

        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LTableName='||LTableName||' LXmlRows='||LXmlRows); $end null;

        --Replace '&lt;' and '&gt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        LXmlRows:=replace(replace(replace(LXmlRows,'&quot;','"'),'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');

        IF INSTR(LXmlRows,'deleting')=0 THEN
            LSectionDelete:=FALSE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LSectionDelete->FALSE'); $end null;
        ELSE
            LSectionDelete:=TRUE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LSectionDelete->TRUE'); $end null;
        END IF;

        IF INSTR(LXmlRows,'inserting')=0 THEN
            LSectionInsert:=FALSE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LSectionInsert->FALSE'); $end null;
        ELSE
            LSectionInsert:=TRUE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LSectionInsert->TRUE'); $end null;
        END IF;

        IF LSectionInsert THEN
            LRowsInserted:=0;

            CASE UPPER(LTableName)
                WHEN 'VW_TEMPORARYCOMPOUND' THEN
                    BEGIN
                        SELECT SEQ_TEMPORARY_COMPOUND.NEXTVAL INTO LTempCompoundID FROM DUAL;
                        SetKeyValue(LTempCompoundID,LTempCompoundIDTag,LTempCompoundIDTagEnd);
                        SetKeyValue(LTempBatchID,LTempBatchIDTag,LTempBatchIDTagEnd);

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LTempCompoundID= '||LTempCompoundID); $end null;

                        --When the structure is more length than 4000 we can't use insertXML
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LTempBatchID= '||LTempBatchID); $end null;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LStructuresList='||LStructuresList); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LFragmentXmlList='||LFragmentXmlList); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LBatchComponentFragmentList='||LBatchCompFragmentXmlList); $end null;
                        LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');

                        SELECT extractValue(XmlType.createXML(LXmlRows),'/VW_TemporaryCompound/ROW/STRUCTUREID')
                            INTO LStructureID
                            FROM dual;

                        IF LStructureID='-1' THEN
                            SELECT Structure
                                INTO LStructureValue
                                FROM VW_Structure
                                WHERE StructureID=-1;
                        END IF;

                        LFragmentXmlValue:='<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
                        LBatchCompFragmentXmlValue:='<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchCompFragmentXmlList,'Clob')||'</BatchComponentFragmentList>';
                        LNormalizedStructureValue:=TakeOffAndGetClob(LNormalizedStructureList,'Clob');

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LStructureValue= '|| LStructureValue ); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LFragmentXmlValue= '|| LFragmentXmlValue ); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LBatchCompFragmentXmlValue= '|| LBatchCompFragmentXmlValue ); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;

                        IF NVL(LStructureID,0)<=-2 THEN  -- (LStructureID= -2,-3)
                            SELECT Structure
                                INTO LStructureValue
                                FROM VW_Structure
                                WHERE StructureID=LStructureID;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LStructureValue= '|| LStructureValue ); $end null;
                        END IF;

                        SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
                        SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                    END;
                ELSE  LMessage:=LMessage || ' "' || LTableName||'" table stranger.';
            END CASE;
            --Replace '<' and '>'  by '&lt;' and '&gt;'' to the XML fields . Necesary to save a xml field using insertXML. I can't to do it using "XmlTransform".
            LPosTagBegin:=1;
            LOOP
                LPosTagBegin := INSTR(LXmlRows,LTagXmlFieldBegin,LPosTagBegin);
                LPosTagEnd   := INSTR(LXmlRows,LTagXmlFieldEnd,LPosTagBegin);

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LOOP LXmlRows='||LXmlRows||' LPosTagBegin='||LPosTagBegin||' LPosTagEnd='||LPosTagEnd); $end null;

            EXIT WHEN (LPosTagBegin=0) or (LPosTagEnd=0);
                LXmlField:= SUBSTR(LXmlRows,LPosTagBegin+LENGTH(LTagXmlFieldBegin),LPosTagEnd-(LPosTagBegin+LENGTH(LTagXmlFieldBegin)));

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlField='||LXmlField||' '||LPosTagBegin||' '||LTagXmlFieldEnd||LPosTagEnd); $end null;

                LXmlField:= replace(replace(LXmlField,'<','&lt;') ,'>','&gt;');

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' Before LXmlRows='||LXmlRows); $end null;
                LXmlRows:= SUBSTR(LXmlRows,1,LPosTagBegin-1)||LXmlField|| SUBSTR(LXmlRows,LPosTagEnd+LENGTH(LTagXmlFieldEnd),LENGTH(LXmlRows)) ;

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' After LXmlRows='||LXmlRows); $end null;
            END LOOP;
            --Create the Table Context
            LCtx := DBMS_XMLSTORE.newContext(LTableName);
            DBMS_XMLSTORE.clearUpdateColumnList(LCtx);

            LXmlRows:=replace(LXmlRows,'insert="yes"','');

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows); $end null;

            --Insert Rows and get count it inserted
            LRowsInserted := DBMS_XMLSTORE.insertXML(LCtx, LXmlRows );

            --Build Message Logs
            LMessage:=LMessage || ' ' || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';

            --Close the Table Context
            DBMS_XMLSTORE.closeContext(LCtx);

            --When structure is more length than 4000 we can't use insertXML
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','BEGIN UPDATE VW_TEMPORARYCOMPOUND'); $end null;

            IF UPPER(LTableName)='VW_TEMPORARYCOMPOUND' THEN
                IF LBatchCompFragmentXmlValue IS NOT NULL THEN
                     IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
                       UPDATE VW_TEMPORARYCOMPOUND
                           SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                           WHERE TempCompoundID=LTempCompoundID;
                     ELSE
                        IF LStructureValue IS NOT NULL THEN
                          UPDATE VW_TEMPORARYCOMPOUND
                            SET BASE64_CDX=LStructureValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue,NormalizedStructure=LNormalizedStructureValue
                            WHERE TempCompoundID=LTempCompoundID;
                        END IF;
                        IF LFragmentXmlValue IS NOT NULL THEN
                          UPDATE VW_TEMPORARYCOMPOUND
                            SET FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue,NormalizedStructure=LNormalizedStructureValue
                            WHERE TempCompoundID=LTempCompoundID;
                        END IF;
                    END IF;
                ELSE
                    IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
                         UPDATE VW_TEMPORARYCOMPOUND
                             SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                             WHERE TempCompoundID=LTempCompoundID;
                    ELSE
                         IF LStructureValue IS NOT NULL THEN
                           UPDATE VW_TEMPORARYCOMPOUND
                             SET BASE64_CDX=LStructureValue, NormalizedStructure=LNormalizedStructureValue
                             WHERE TempCompoundID=LTempCompoundID;
                         END IF;
                         IF LFragmentXmlValue IS NOT NULL THEN
                           UPDATE VW_TEMPORARYCOMPOUND
                             SET FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                             WHERE TempCompoundID=LTempCompoundID;
                         END IF;
                    END IF;
                END IF;
            END IF;

            IF (LRowsInserted>0) THEN
                LSomeUpdate:=True;
            END IF;
            LMessage:=LMessage ||  chr(10) || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
        ELSIF LSectionDelete THEN
            LRowsDeleted:=0;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'SectionDelete LTableName='||LTableName); $end null;

            CASE UPPER(LTableName)
            WHEN 'VW_TEMPORARYCOMPOUND' THEN
                BEGIN
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]').getClobVal()
                        INTO LKeyFieldName
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LKeyFieldName='||LKeyFieldName); $end null;
                    IF LKeyFieldName IS NOT NULL THEN
                        LCtx := DBMS_XMLSTORE.newContext(LTableName);
                        LKeyFieldName:=XMLType(LKeyFieldName).getRootElement();
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'2º LKeyFieldName='||LKeyFieldName); $end null;
                        DBMS_XMLSTORE.setKeyColumn(LCtx,LKeyFieldName);
                        LRowsDeleted := DBMS_XMLSTORE.deleteXML(LCtx, LXmlRows );
                        DBMS_XMLSTORE.closeContext(LCtx);
                        IF (LRowsDeleted>0) THEN
                            LSomeUpdate:=True;
                        END IF;
                    END IF;
                    LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Row/s Deleted on "'||LTableName||'".';
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
                END;
            WHEN 'PROJECTXML' THEN
                BEGIN
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]/text()').getClobVal()
                        INTO LID
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LID='||LID||' LTempBatchID='||LTempBatchID); $end null;
                    IF LID IS NOT NULL AND NVL(LTempBatchID,0)>0 THEN
                        UPDATE VW_TemporaryBatch SET ProjectXML =DELETEXML(XmlType(ProjectXML),'/ProjectList/Project[ProjectID="'||LID||'"]').getClobVal()
                            WHERE TempBatchID = LTempBatchID AND ExistsNode(XmlType(ProjectXML),'/ProjectList/Project[ProjectID="'||LID||'"]')=1;

                        DELETE VW_TemporaryRegNumbersProject WHERE TempBatchID=LTempBatchID AND ProjectID=LID;

                        LRowsDeleted:=SQL%ROWCOUNT;
                        IF (LRowsDeleted>0) THEN
                            LSomeUpdate:=True;
                        END IF;
                        DELETE VW_TemporaryBatchProject WHERE TempBatchID=LTempBatchID AND ProjectID=LID;
                    END IF;
                    LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on "'||LTableName||'".';
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
                END;
            WHEN 'PROJECTXMLBATCH' THEN
                BEGIN
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]/text()').getClobVal()
                        INTO LID
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LID='||LID||' LTempBatchID='||LTempBatchID); $end null;
                    IF LID IS NOT NULL AND NVL(LTempBatchID,0)>0 THEN
                        UPDATE VW_TemporaryBatch SET ProjectXMLBatch =DELETEXML(XmlType(ProjectXMLBatch),'/ProjectList/Project[ProjectID="'||LID||'"]').getClobVal()
                            WHERE TempBatchID = LTempBatchID AND ExistsNode(XmlType(ProjectXMLBatch),'/ProjectList/Project[ProjectID="'||LID||'"]')=1;

                        DELETE VW_TemporaryBatchProject WHERE TempBatchID=LTempBatchID AND ProjectID=LID;

                        LRowsDeleted:=SQL%ROWCOUNT;
                        IF (LRowsDeleted>0) THEN
                            LSomeUpdate:=True;
                        END IF;
                    END IF;
                    LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on "'||LTableName||'".';
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
                END;
            WHEN 'FRAGMENTXML' THEN
                BEGIN
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]/text()').getClobVal()
                        INTO LID
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LID='||LID||' LTempCompoundID='||LTempCompoundID); $end null;
                    IF LID IS NOT NULL AND NVL(LTempCompoundID,0)>0 THEN
                        UPDATE VW_TemporaryCompound SET FragmentXML =DELETEXML(XmlType(FragmentXML),'/FragmentList/Fragment[FragmentID="'||LID||'"]').getClobVal()
                            WHERE TempCompoundID = LTempCompoundID AND ExistsNode(XmlType(FragmentXML),'/FragmentList/Fragment[FragmentID="'||LID||'"]')=1;
                        LRowsDeleted:=SQL%ROWCOUNT;
                        IF (LRowsDeleted>0) THEN
                            LSomeUpdate:=True;
                        END IF;
                    END IF;
                    LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on "'||LTableName||'".';
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
                END;
             WHEN 'IDENTIFIERXMLMIXTURE' THEN
                BEGIN
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]/text()').getClobVal()
                        INTO LID
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LID='||LID||' LTempBatchID='||LTempBatchID); $end null;
                    IF LID IS NOT NULL AND NVL(LTempBatchID,0)>0 THEN
                        UPDATE VW_TemporaryBatch SET IdentifierXML =DELETEXML(XmlType(IdentifierXML),'/IdentifierList/Identifier[IdentifierID="'||LID||'"]').getClobVal()
                            WHERE TempBatchID = LTempBatchID AND ExistsNode(XmlType(IdentifierXML),'/IdentifierList/Identifier[IdentifierID="'||LID||'"]')=1;
                        LRowsDeleted:=SQL%ROWCOUNT;
                        IF (LRowsDeleted>0) THEN
                            LSomeUpdate:=True;
                        END IF;
                    END IF;
                    LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on "'||LTableName||'".';
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
                END;
            WHEN 'IDENTIFIERXMLCOMPOUND' THEN
                BEGIN
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]/text()').getClobVal()
                        INTO LID
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LID='||LID||' LTempCompoundID='||LTempCompoundID); $end null;
                    IF LID IS NOT NULL AND NVL(LTempCompoundID,0)>0 THEN
                        UPDATE VW_TemporaryCompound SET IdentifierXML =DELETEXML(XmlType(IdentifierXML),'/IdentifierList/Identifier[IdentifierID="'||LID||'"]').getClobVal()
                            WHERE TempCompoundID = LTempCompoundID AND ExistsNode(XmlType(IdentifierXML),'/IdentifierList/Identifier[IdentifierID="'||LID||'"]')=1;
                        LRowsDeleted:=SQL%ROWCOUNT;
                        IF (LRowsDeleted>0) THEN
                            LSomeUpdate:=True;
                        END IF;
                    END IF;
                    LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on "'||LTableName||'".';
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
                END;
            WHEN 'IDENTIFIERXMLBATCH' THEN
                BEGIN
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]/text()').getClobVal()
                        INTO LID
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LID='||LID||' LTempBatchID='||LTempBatchID); $end null;
                    IF LID IS NOT NULL AND NVL(LTempBatchID,0)>0 THEN
                        UPDATE VW_TemporaryBatch SET IdentifierXMLBatch =DELETEXML(XmlType(IdentifierXMLBatch),'/IdentifierList/Identifier[IdentifierID="'||LID||'"]').getClobVal()
                            WHERE TempBatchID = LTempBatchID AND ExistsNode(XmlType(IdentifierXMLBatch),'/IdentifierList/Identifier[IdentifierID="'||LID||'"]')=1;
                        LRowsDeleted:=SQL%ROWCOUNT;
                        IF (LRowsDeleted>0) THEN
                            LSomeUpdate:=True;
                        END IF;
                    END IF;
                    LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on "'||LTableName||'".';
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
                END;
            END CASE;
        ELSE
            LRowsUpdated:=0;

            SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]').getClobVal()
                INTO LKeyFieldName
                FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LKeyFieldName='||LKeyFieldName); $end null;

            IF LKeyFieldName IS NOT NULL THEN

                --Replace '<' and '>'  by '&lt;' and '&gt;'' to the XML fields . Necesary to save a xml field using insertXML. I can't to do it using "XmlTransform".
                LPosTagBegin:=1;
                LOOP
                    LPosTagBegin := INSTR(LXmlRows,LTagXmlFieldBegin,LPosTagBegin);
                    LPosTagEnd   := INSTR(LXmlRows,LTagXmlFieldEnd,LPosTagBegin);
                EXIT WHEN (LPosTagBegin=0) or (LPosTagEnd=0);
                    LXmlField:= SUBSTR(LXmlRows,LPosTagBegin+LENGTH(LTagXmlFieldBegin),LPosTagEnd-(LPosTagBegin+LENGTH(LTagXmlFieldBegin)));

                    LXmlField:= replace(replace(LXmlField,'<','&lt;') ,'>','&gt;');
                    LXmlRows:= SUBSTR(LXmlRows,1,LPosTagBegin-1)||LXmlField|| SUBSTR(LXmlRows,LPosTagEnd+LENGTH(LTagXmlFieldEnd),LENGTH(LXmlRows)) ;
                END LOOP;

                --Build Message Logs
                LMessage:=LMessage || chr(10) || 'Processing '||LTableName|| ': ';

                --Create the Table Context
                LCtx := DBMS_XMLSTORE.newContext(LTableName);


                CASE UPPER(LTableName)
                    WHEN 'VW_TEMPORARYBATCH' THEN
                    BEGIN
                        SELECT  extractvalue(XmlType(LXmlRows),'node()[1]/ROW/node()[1]')
                            INTO LTempBatchID
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYBATCH LTempBatchID= '||LTempBatchID); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LStructureAggregationValue= '||LStructureAggregationValue); $end null;
                        LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LStructureAggregationValue= '||LStructureAggregationValue); $end null;

                        BEGIN
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlTypeRows= '||LXmlTypeRows.getClobVal); $end null;
                            SELECT XMLSequence(LXmlTypeRows.Extract('node()[1]/ROW/PROJECTXML/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID'))
                                INTO LProjectsSequenceType
                                FROM DUAL
                                WHERE ExistsNode(LXmlTypeRows,'node()[1]/ROW/PROJECTXML/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID')=1;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','/Project[@insert="yes"]/ProjectID= '||LXmlTypeRows.Extract('node()[1]/ROW/PROJECTXML/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID').getclobVal()); $end null;
                              FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP

                                LProjectID:=LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LProjectID= '||LProjectID); $end null;
                                IF LProjectID IS NOT NULL THEN

                                    INSERT INTO VW_TemporaryRegNumbersProject (TempBatchID, ProjectID) VALUES (LTempBatchID,LProjectID);

                                END IF;
                             END LOOP;

                        EXCEPTION
                            WHEN NO_DATA_FOUND THEN NULL;
                        END;

                         BEGIN
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlTypeRows= '||LXmlTypeRows.getClobVal); $end null;

                            SELECT XMLSequence(LXmlTypeRows.Extract('node()[1]/ROW/PROJECTXMLBATCH/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID'))
                                INTO LProjectsSequenceType
                                FROM DUAL
                                WHERE ExistsNode(LXmlTypeRows,'node()[1]/ROW/PROJECTXMLBATCH/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID')=1;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','/Project[@insert="yes"]/ProjectID= '||LXmlTypeRows.Extract('node()[1]/ROW/PROJECTXMLBATCH/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID').getclobVal()); $end null;
                            FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP

                                LProjectID:=LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','Batch LProjectID= '||LProjectID); $end null;
                                IF LProjectID IS NOT NULL THEN
                                    INSERT INTO VW_TemporaryBatchProject (TempBatchID, ProjectID) VALUES (LTempBatchID,LProjectID);
                                END IF;
                            END LOOP;
                        EXCEPTION
                            WHEN NO_DATA_FOUND THEN NULL;
                        END;
                    END;
                    WHEN 'VW_TEMPORARYCOMPOUND' THEN
                        BEGIN
                            SELECT  extractvalue(XmlType(LXmlRows),'node()[1]/ROW/node()[1]')
                                INTO LTempCompoundID
                                FROM dual;

                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LStructuresList='||LStructuresList); $end null;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LFragmentXmlList='||LFragmentXmlList); $end null;
                            LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LStructureValue= '|| LStructureValue ); $end null;
                            LFragmentXmlValue:='<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
                            IF LFragmentXmlValue='<FragmentList></FragmentList>' THEN
                                LFragmentXmlValue:='';
                            END IF;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LFragmentXmlValue= '|| LFragmentXmlValue ); $end null;
                            LBatchCompFragmentXmlValue:='<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchCompFragmentXmlList,'Clob')||'</BatchComponentFragmentList>';
                            IF LBatchCompFragmentXmlValue='<BatchComponentFragmentList></BatchComponentFragmentList>' THEN
                                LBatchCompFragmentXmlValue:='';
                            END IF;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LBatchCompFragmentXmlValue= '|| LBatchCompFragmentXmlValue ); $end null;
                            LNormalizedStructureValue:=TakeOffAndGetClob(LNormalizedStructureList,'Clob');
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;

                        END;
                    ELSE  NULL;
                END CASE;


                DBMS_XMLSTORE.clearUpdateColumnList(LCtx);
                LKeyFieldName:=XMLType(LKeyFieldName).getRootElement();
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LKeyFieldName= '|| LKeyFieldName ); $end null;
                DBMS_XMLSTORE.setKeyColumn(LCtx,LKeyFieldName);
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' after setKeyColumn LKeyFieldName= '|| LKeyFieldName ); $end null;
                LFieldIndex:=1;
                LUpdate:=FALSE;
                LOOP
                    LFieldIndex:=LFieldIndex+1;
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW/node()['||LFieldIndex||']').getClobVal()
                    INTO LFieldToUpdate
                    FROM dual;

                    IF LFieldToUpdate IS NOT NULL THEN
                        LUpdate:=TRUE;
                        LFieldToUpdate:=XMLType(LFieldToUpdate).getRootElement();
                    END IF;

                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'->'||LFieldToUpdate); $end null;

                EXIT WHEN LFieldToUpdate IS NULL;
                    DBMS_XMLSTORE.setUpdateColumn(LCtx, LFieldToUpdate);
                END LOOP;

                --Insert Rows and get count it inserted
                IF LUpdate THEN
                    LRowsUpdated := DBMS_XMLSTORE.updateXML(LCtx, LXmlRows );
                    LSomeUpdate:=TRUE;
                END IF;
                --Close the Table Context
                DBMS_XMLSTORE.closeContext(LCtx);

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LTempCompoundID= '|| LTempCompoundID ); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LStructureValue= '|| LStructureValue ); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LFragmentXmlValue= '|| LFragmentXmlValue ); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LBatchCompFragmentXmlValue= '|| LBatchCompFragmentXmlValue ); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;

                IF LFragmentXmlValue IS NOT NULL THEN
                    LRowsDeleted:=0;
                    LPosTagBegin:=1;
                    LOOP
                        LPosTagBegin := INSTR(UPPER(LFragmentXmlValue),'<FRAGMENT DELETE="YES"',LPosTagBegin);
                        IF LPosTagBegin=0 THEN
                            LPosTagBegin := INSTR(UPPER(LFragmentXmlValue),'<FRAGMENT DELETE=''YES''',LPosTagBegin);
                        END IF;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','Fragment Deleting LPosTagBegin='||LPosTagBegin); $end null;
                    EXIT WHEN (LPosTagBegin=0);
                        LPosTagEnd := INSTR(LFragmentXmlValue,'</Fragment>',LPosTagBegin);

                        LFragmentXmlValue:= SUBSTR(LFragmentXmlValue,1,LPosTagBegin-1)||SUBSTR(LFragmentXmlValue,LPosTagEnd+LENGTH('</Fragment>'),LENGTH(LFragmentXmlValue)) ;
                        LRowsDeleted:=LRowsDeleted+1;
                        LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on FRAGMENTXML".';
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','Fragment Deleted LFragmentXmlValue='||LFragmentXmlValue); $end null;
                    END LOOP;
                END IF;

                IF UPPER(LTableName)='VW_TEMPORARYCOMPOUND' THEN
                    IF  LBatchCompFragmentXmlValue IS NOT NULL THEN
                        IF LStructureValue IS NOT NULL THEN
                            IF LFragmentXmlValue IS NOT NULL THEN
                                IF LNormalizedStructureValue IS NOT NULL THEN
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 1'); $end null;
                                ELSE
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 2'); $end null;
                                END IF;
                            ELSE
                                IF LNormalizedStructureValue IS NOT NULL THEN
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,NormalizedStructure=LNormalizedStructureValue,BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 3'); $end null;
                                ELSE
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 4'); $end null;
                                END IF;
                            END IF;
                        ELSE
                            IF LFragmentXmlValue IS NOT NULL THEN
                                  IF LNormalizedStructureValue IS NOT NULL THEN
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                      $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 5'); $end null;
                                  ELSE
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET FRAGMENTXML=LFragmentXmlValue,BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                      $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 6'); $end null;
                                  END IF;
                              ELSE
                                  IF LNormalizedStructureValue IS NOT NULL THEN
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET NormalizedStructure=LNormalizedStructureValue,BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                      $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 7'); $end null;
                                  END IF;
                              END IF;
                        END IF;
                   ELSE
                        IF LStructureValue IS NOT NULL THEN
                            IF LFragmentXmlValue IS NOT NULL THEN
                                IF LNormalizedStructureValue IS NOT NULL THEN
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 8'); $end null;
                                ELSE
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 9'); $end null;
                                END IF;
                            ELSE
                                IF LNormalizedStructureValue IS NOT NULL THEN
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,NormalizedStructure=LNormalizedStructureValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 10'); $end null;
                                ELSE
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 11'); $end null;
                                END IF;
                            END IF;
                        ELSE
                            IF LFragmentXmlValue IS NOT NULL THEN
                                  IF LNormalizedStructureValue IS NOT NULL THEN
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                      $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 12'); $end null;
                                  ELSE
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET FRAGMENTXML=LFragmentXmlValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                      $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 13'); $end null;
                                  END IF;
                              ELSE
                                  IF LNormalizedStructureValue IS NOT NULL THEN
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET NormalizedStructure=LNormalizedStructureValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                      $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 14'); $end null;
                                  END IF;
                              END IF;
                        END IF;
                     END IF;
                END IF;

                IF UPPER(LTableName)='VW_TEMPORARYBATCH' THEN
                    IF LStructureAggregationValue IS NOT NULL THEN
                        LSomeUpdate:=True;
                        UPDATE VW_TEMPORARYBATCH
                            SET StructureAggregation=LStructureAggregationValue
                            WHERE TempBatchID=LTempBatchID;
                        LRowsUpdated:=SQL%ROWCOUNT;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || ' updated->' || 'section 15'); $end null;
                  END IF;
                END IF;
            END IF;

            LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".' || chr(13) ;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || '->' || LXmlRows || chr(10) || LMessage); $end null;
       END IF;
    END LOOP;

    IF LSomeUpdate THEN
        AMessage := '';
    /* JED: We can only retrieve 'the' registration if there was only ONE record registration, lol!
      IF LRegNumber is not NULL then
        begin
          if UPPER(ASectionsList) <> cSectionListEmpty then
            RetrieveMultiCompoundRegistry(LRegNumber, LXMLRegistryRecord, ASectionsList);
          end if;
          AMessage := CreateRegistrationResponse(LBriefMessage, null, LXMLRegistryRecord);
        end;
      END IF;
    */
    ELSE
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('No data to update.'));
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".';
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eUpdateMultiCompoundRegTmp, AppendError('UpdateMultiCompoundRegTmp', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;


PROCEDURE DeleteMultiCompoundRegTmp( ATempID  in Number) IS

   LIsNotEditableDeletingTmp EXCEPTION;

BEGIN

    $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegTmp','Begin: ATempID:'||ATempID); $end null;

    IF GetIsEditableTmp (ATempID)='False' then
        RAISE LIsNotEditableDeletingTmp ;
    END IF;

    DELETE VW_TemporaryCompound WHERE TempBatchID=ATempID;

    DELETE VW_TemporaryBatchProject WHERE TempBatchID=ATempID;

    DELETE VW_TemporaryRegNumbersProject WHERE TempBatchID=ATempID;

    DELETE VW_TemporaryBatch WHERE TempBatchID =ATempID;

    -- JED: removed this error temporarily. Business objects are inadvertantly, but harmlessly,
    --      invoking this method after promoting a temporary record to permanent, despite the fact
    --      that NOT all temporary records come from the temp tables (they can also come from
    --      coedb.coegenericobject)
    -- Suggest we incorporate a readonly RegistryRecord property that indicates the SOURCE of the record
    -- so we can process it properly
    /*
    IF SQL%NOTFOUND THEN
        RAISE_APPLICATION_ERROR(eGenericException, 'TempID '||ATempID||' doesn''t exist. 0 Row Deleted on VW_TemporaryBatch.');
    END IF;
    */

    $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegTmp','End'); $end null;

EXCEPTION
    WHEN LIsNotEditableDeletingTmp THEN
        RAISE_APPLICATION_ERROR(eIsNotEditableDeletingTmp, 'You are not authorized to delete this record. (Temporary Registry# '||ATempID||')');
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegTmp','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);$end null;
        RAISE_APPLICATION_ERROR(eDeleteMultiCompoundRegTmp, AppendError('DeleteMultiCompoundRegTmp', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

PROCEDURE RetrieveCompoundRegistryList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob) IS
       -- Autor: Fari
       -- Date:17-May-07
       -- Object:
       -- Pending:
      LMessage                  CLOB:='';
      LXml                      CLOB:='';
      LXmlList                  CLOB:='';
      LSectionList   CONSTANT   VARCHAR2(500):='Compound,Identifier';
      LRegNumber                VW_RegistryNumber.RegNumber%type;
      LIndex                    Number;
      LAux                      CLOB:='';
      LIndexAux                 Number;
      LPosBegin                 Number;
      LPosEnd                   Number;
BEGIN
  LIndex:=1;
  LOOP
      $if CompoundRegistry.Debuging $then InsertLog('RetrieveCompoundRegistryList','AXmlRegNumbers->'||AXmlRegNumbers); $end null;
      SELECT  extractValue(XmlType(AXmlRegNumbers),'REGISTRYLIST/node()['||LIndex||']/node()[1]')
        INTO LRegNumber
        FROM dual;
      $if CompoundRegistry.Debuging $then InsertLog('RetrieveCompoundRegistryList','LRegNumber->'||LRegNumber); $end null;
  EXIT WHEN LRegNumber IS NULL;
      BEGIN
          RetrieveMultiCompoundRegistry(LRegNumber,LXml,LSectionList);

          LPosBegin:=Instr(LXml,'<MultiCompoundRegistryRecord');
          LPosEnd:=Instr(LXml,'>',LPosBegin);
          LXml:=Substr(LXml,1,LPosBegin-1)||Substr(LXml,LPosEnd-LPosBegin+2,length(LXml));
          LXml:=replace(LXml,'</MultiCompoundRegistryRecord>','');

          LXmlList:=LXmlList||CHR(10)||LXml;
      EXCEPTION
         WHEN OTHERS THEN
         BEGIN
           IF INSTR(DBMS_UTILITY.format_error_stack,eNoRowsReturned)<>0 THEN NULL; --Though a Compound doesn't exist to get the others
           ELSE
              RAISE_APPLICATION_ERROR(eRetrieveCompoundRegistryList,DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
           END IF;
         END;
      END;
      LIndex:=LIndex+1;
  END LOOP;
  AXmlCompoundList:='<CompoundList>'||CHR(10)||LXmlList||CHR(10)||'</CompoundList>';
  $if CompoundRegistry.Debuging $then InsertLog('RetrieveCompoundRegistryList','List->'||AXmlCompoundList); $end null;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eRetrieveCompoundRegistryList, AppendError('RetrieveCompoundRegistryList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
END;

/**
Workhorse method for "ConvertHitlistTempsToPerm" method.
Given a list of temporary registry IDs and duplicate-resolution instructions, fetch
the records by ID, create permanent records, and eliminate the temporary records.
-->author jed
-->since December 2009
-->param ATempIds           the collection of temporary registry identifiers to act upon
-->param ADuplicateAction   indicator of duplication-resolution strategy to use
-->param ADescription       processing note added by user
-->param AUserID            user responsible for performing the action
-->param AMessage           output of abbreviated responses (xml string) wrapped in a parent element
-->param ARegistration      defaults to 'Y'
-->param ARegNumGeneration  defaults to 'Y'
-->param AConfigurationID   defaults to 1
-->param ASectionsList      defaults to NULL
*/
PROCEDURE ConvertTempRegRecordsToPerm(
    ATempIds IN tNumericIdList
    , ADuplicateAction IN CHAR
    , ADescription IN VARCHAR2
    , AUserID IN VARCHAR2
    , ALogID OUT NUMBER
    , AMessage OUT NOCOPY CLOB
    , ARegistration IN CHAR := 'Y'
    , ARegNumGeneration IN CHAR := 'Y'
    , AConfigurationID IN Number := 1
    , ASectionsList IN Varchar2 := NULL
) IS
    LRegistryResponses CLOB;
    -- very important to init "LRegistryResponseList" for concatenation
    LRegistryResponseList CLOB := empty_clob();
    LRegistryResponseItem CLOB;
    LErrorCount NUMBER := 0;
    Lthis_tempid NUMBER;
    Lthis_regxml CLOB;
    Lthis_action Char;
    Lthis_regnum VW_RegistryNumber.RegNumber%type;
    LExtractedMessage CLOB;
    LExtractedError CLOB;
    LExtractedResult CLOB;
    LIterator NUMBER;
    LCounter NUMBER := 0;
    LDuplicateResolutionAction CHAR := ADuplicateAction;
BEGIN
    --'open' the overall messages container
    LRegistryResponses := '<Responses message="';

    --by-pass erroneous 'Temporary' duplicate-resolution instructions
    IF (ADuplicateAction = 'T') THEN
        LDuplicateResolutionAction := 'N';
    END IF;

    --create the log header for the bulk-migration event
    LogBulkRegistrationId(ALogID, LDuplicateResolutionAction, AUserID, ADescription);

    --attempt the migration for each temporary registry record
    LIterator := ATempIds.FIRST;
    LOOP
        EXIT WHEN LIterator IS NULL;
        Lthis_tempid := ATempIds(LIterator);

        IF GetIsEditableTmp(Lthis_tempid)='False' THEN
             LRegistryResponseItem := CreateRegistrationResponse('You are not authorized to register this Temporary Registry.', 'You are not authorized to register this Temporary Registry.', NULL);
             Lthis_action:='N';
        ELSE

            --fetch the record
            $if CompoundRegistry.Debuging $then InsertLog('ConvertTempRegRecordsToPerm','Lthis_tempid->'||Lthis_tempid); $end null;
            $if CompoundRegistry.Debuging $then InsertLog('ConvertTempRegRecordsToPerm','Lthis_regxml->'||Lthis_regxml); $end null;
            RetrieveMultiCompoundRegTmp(Lthis_tempid, Lthis_regxml);

            --insert it to permanent
            BEGIN
                LoadMultiCompoundRegRecord(
                    Lthis_regxml
                    , LDuplicateResolutionAction
                    , Lthis_action
                    , LRegistryResponseItem
                    , Lthis_regnum
                    , ARegistration
                    , ARegNumGeneration
                    , AConfigurationID
                    , ASectionsList
                );

            /* Uncomment this only for debugging
            EXCEPTION
              WHEN OTHERS THEN
                LRegistryResponseItem := CreateRegistrationResponse(
                  'Error: Result node contains the original XML causing the failure'
                  , SQLERRM || chr(10) || DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE
                  , Lthis_regxml
                );
            */
            END;
        END IF;
        --determine errors
        $if CompoundRegistry.Debuging $then InsertLog('ConvertTempRegRecordsToPerm','LRegistryResponseItem->'||LRegistryResponseItem); $end null;
        IF LRegistryResponseItem IS NOT NULL THEN
            ExtractRegistrationResponse(LRegistryResponseItem, LExtractedMessage, LExtractedError, LExtractedResult);
            $if CompoundRegistry.Debuging $then InsertLog('ConvertTempRegRecordsToPerm','LExtractedMessage->'||LExtractedMessage||' LExtractedError->'||LExtractedError||' LExtractedResult->'||LExtractedResult); $end null;
            --create the log detail for this particular temp record
            IF LExtractedError IS NOT NULL THEN
                LErrorCount := LErrorCount + 1;
                LogBulkregistration(ALogID, Lthis_tempid, Lthis_action, NULL, NULL, LExtractedMessage);
            ELSE
                --delete the temp record
                LogBulkregistration(ALogID, NULL, Lthis_action, Lthis_regnum, NULL, LExtractedMessage);
                DeleteMultiCompoundRegTmp(Lthis_tempid);
            END IF;
            --TODO: as necessary, reconstruct the individual message items with focus on brevity
        END IF;
        $if CompoundRegistry.Debuging $then InsertLog('ConvertTempRegRecordsToPerm','4 LRegistryResponseList->'||LRegistryResponseList); $end null;
        --append to the message list
        IF LRegistryResponseList IS NULL THEN
          LRegistryResponseList := LRegistryResponseItem;
        ELSE
          LRegistryResponseList := LRegistryResponseList || LRegistryResponseItem;
        END IF;

        --maintain counters
        LCounter := LCounter + 1;
        LIterator := ATempIds.NEXT(LIterator);

        COMMIT;
    END LOOP;

    --fill and 'close' the messages container
    LRegistryResponses :=
      LRegistryResponses
      || to_char(LCounter) || ' records processed, with ' || to_char(LErrorCount)|| ' errors (unresolved duplicates)">'
      || LRegistryResponseList
      || '</Responses>';

    --set the messages container as the output
    AMessage := LRegistryResponses;

    RETURN;
EXCEPTION
    WHEN OTHERS THEN
        $if CompoundRegistry.Debuging $then InsertLog('ConvertTempRegRecordsToPerm',chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
        RAISE_APPLICATION_ERROR(EGenericException, AppendError('ConvertTempRegRecordsToPerm', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;

/**
Fetches a list of registration IDs based on a result-set identifier.
(GUI-derived identifier collections - 'hits', often the result-set from a search - are
saved so they can later be acted upon en masse.)
-->author jed
-->since December 2009
-->param Ahitlistid identifier of a saved 'hit' list of temporary registrations
-->param AId        collection output of temporary registry identifiers
*/
PROCEDURE GetHitlistItemIdsFromHitlistId(
    Ahitlistid IN NUMBER
    , AId OUT tNumericIdList
)
IS
    LIdList tNumericIdList;
    LCounter NUMBER := 0;
    CURSOR CTempRecordList(someId NUMBER)
    IS
    SELECT item.id FROM COEDB.COESAVEDHITLIST item
    WHERE item.hitListId = Ahitlistid;
BEGIN
    FOR itemRow in CTempRecordList(Ahitlistid) LOOP
        LIdList(LCounter) := itemRow.id;
        LCounter := LCounter + 1;
    END LOOP;
    AId := LIdList;
END;

/**
Registration records can be either 'temporary' (requiring review) or 'permanent'.
This procedure finalizes the review process by bulk-promoting 'temporary' records
to permanent status, using a list identifier as a source of temporary record IDs.
-->author jed
-->since December 2009
-->see "ConvertHitlistTempsToPerm" method, which does the real work
-->param Ahitlistid         identifier of a saved 'hit' list of temporary registrations
-->param ADuplicateAction   indicator of duplication-resolution strategy to use
-->param ADescription       processing note added by user
-->param AUserID            user responsible for performing the action
-->param AMessage           output of abbreviated responses (xml string) wrapped in a parent element
-->param ARegistration      defaults to 'Y'
-->param ARegNumGeneration  defaults to 'Y'
-->param AConfigurationID   defaults to 1
-->param ASectionsList      defaults to NULL
*/
PROCEDURE ConvertHitlistTempsToPerm(
    Ahitlistid IN NUMBER
    , ADuplicateAction IN CHAR
    , ADescription IN VARCHAR2
    , AUserID IN VARCHAR2
    , ALogID OUT NUMBER
    , AMessage OUT NOCOPY CLOB
    , ARegistration IN CHAR := 'Y'
    , ARegNumGeneration IN CHAR := 'Y'
    , AConfigurationID IN NUMBER := 1
    , ASectionsList IN VARCHAR2 := NULL
) IS
    LIdList tNumericIdList;
BEGIN
    GetHitlistItemIdsFromHitlistId( Ahitlistid, LIdList );
    IF LIdList IS NOT NULL AND LIdList.COUNT > 0 THEN
        ConvertTempRegRecordsToPerm(
            LIdList
            , ADuplicateAction
            , ADescription
            , AUserID
            , ALogID
            , AMessage
            , ARegistration
            , ARegNumGeneration
            , AConfigurationID
            , ASectionsList
        );
    END IF;
END;

PROCEDURE MoveBatch(ABatchID in Number,ARegNumber in VW_RegistryNumber.RegNumber%type) IS
    LBatchCount            Integer;
    LValidRegistry         Integer;
    LTargetComponentCount  Integer;
    LSourceComponentCount  Integer;
    LTargetBatchID         Integer;
    LTargetRegID           Integer;
    LTargeMixtureID        Integer;
    LBatchRegID            Integer;
    LCompoundCountBatch    Integer;
    LCompoundCountRegistry Integer;
    LSourceMixtureID       Integer;
    LMatchCompunds         Integer;
    LMatchFragment         Integer;
    LErrorMessage          Varchar2(2000);
    LErrorCode             Integer;
    LSequenceID            Integer;
    LBatchNumber           VW_Batch.BatchNumber%Type;
    LFullregNumber         VW_Batch.FullRegNumber%Type;
    LXMLRegistryRecord     CLOB;
    LRLSState              Boolean;
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'ABatchID: ' || ABatchID||' ARegNumber: ' || ARegNumber); $end null;

    SELECT RegID
        INTO LBatchRegID
        FROM  VW_Batch
        WHERE BatchID=ABatchID;

    SELECT COUNT(1)
        INTO  LBatchCount
        FROM  VW_Batch
        WHERE RegID = LBatchRegID;

    --Ulises 9-12-08: Check destiny RegNum before moving.
    SELECT COUNT(1)
        INTO LValidRegistry
        FROM VW_RegistryNumber
        WHERE Upper(REGNUMBER) = Upper(ARegNumber);

    IF LBatchCount=0 THEN
        Raise_Application_Error (eNoRowsReturned, 'No rows returned. The batch was not moved.');
    ELSIF LBatchCount=1 THEN
        Raise_Application_Error (eOnlyOneBatch, 'The Batch''s Registry has an only Batch. The Batch was not moved.');
    ELSIF LValidRegistry=0 THEN
        Raise_Application_Error (eInvalidRegNum, 'The given RegNum does not exist.');
    ELSE

        SELECT RN.RegID,MRN.MixtureID,RN.SequenceID
            INTO LTargetRegID,LTargeMixtureID,LSequenceID
            FROM  VW_RegistryNumber RN,VW_Mixture_RegNumber MRN
            WHERE MRN.RegID=RN.RegID AND RN.RegNumber=ARegNumber;
        $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetRegID: ' || LTargetRegID||' LTargeMixtureID: ' || LTargeMixtureID); $end null;

        SELECT Count(1)
            INTO LCompoundCountRegistry
            FROM VW_Mixture_Component
            WHERE MixtureID=(SELECT MixtureID FROM VW_Mixture_RegNumber WHERE REGID=LTargetRegID);

        $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LCompoundCountRegistry: ' || LCompoundCountRegistry); $end null;

        SELECT MixtureID
            INTO LSourceMixtureID
            FROM VW_Mixture_RegNumber WHERE REGID=LBatchRegID;

         SELECT Count(1)
            INTO LCompoundCountBatch
            FROM VW_Mixture_Component
            WHERE MixtureID=LSourceMixtureID;

         $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LCompoundCountBatch: ' || LCompoundCountBatch); $end null;

        IF LCompoundCountRegistry = LCompoundCountBatch THEN
           SELECT Min(BatchID)
               INTO LTargetBatchID
               FROM VW_Batch
               WHERE RegID=LTargetRegID AND BatchID<>ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetBatchID: ' || LTargetBatchID); $end null;

            SELECT Count(1)
                INTO LTargetComponentCount
                FROM VW_BatchComponent
                WHERE BatchID=LTargetBatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetComponentCount: ' || LTargetComponentCount); $end null;

            SELECT RN.RegID,MRN.MixtureID
                INTO LTargetRegID,LTargeMixtureID
                FROM  VW_RegistryNumber RN,VW_Mixture_RegNumber MRN
                WHERE MRN.RegID=RN.RegID AND RN.RegNumber=ARegNumber;

            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetRegID: ' || LTargetRegID||' LTargeMixtureID: ' || LTargeMixtureID); $end null;
            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'GetSameBatchesIdentity: ' || GetSameBatchesIdentity); $end null;

            IF GetSameBatchesIdentity='True' THEN

                SELECT Count(1)
                    INTO LMatchFragment
                    FROM
                        (SELECT CF_Source.FragmentID
                            FROM VW_Compound_Fragment CF_Source, VW_Mixture_Component MC_Source
                            WHERE  CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID
                            GROUP BY CF_Source.FragmentID
                        MINUS
                        SELECT CF_Target.FragmentID
                            FROM VW_Compound_Fragment CF_Target, VW_Mixture_Component MC_Target
                            WHERE  CF_Target.CompoundID=MC_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID
                            GROUP BY CF_Target.FragmentID);

                IF LMatchFragment<>0 THEN
                    LErrorMessage:='The Fragment List from source and target record is different. New fragment list was applied to the moved batch.';
                    LErrorCode:= eFragmentNotMatchIdentityTrue;
                END IF;

                DELETE VW_BatchComponentFragment BC
                    WHERE BatchComponentID IN (SELECT ID FROM VW_BatchComponent WHERE BatchID=ABatchID);
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponentFragmen: ' || SQL%ROWCOUNT); $end null;

                DELETE VW_BatchComponent BC1
                    WHERE BatchID=ABatchID;
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponent: ' || SQL%ROWCOUNT); $end null;


                INSERT INTO VW_Compound_Fragment(ID,FragmentID,CompoundID)
                    (SELECT Seq_Compound_Fragment.NextVal,CF_Source.FragmentID, MC_Target.CompoundID
                        FROM VW_Compound_Fragment CF_Source ,VW_Mixture_Component MC_Source, VW_Mixture_Component MC_Target
                        WHERE CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID AND MC_Target.MixtureID=LTargeMixtureID AND
                              CF_Source.FragmentID NOT IN (SELECT CF_Target.FragmentID
                                                        FROM VW_Compound_Fragment CF_Target,VW_Mixture_Component MC_Target
                                                        WHERE MC_Target.CompoundID=CF_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID));
                 $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_Compound_Fragment: ' || SQL%ROWCOUNT); $end null;

                INSERT INTO VW_BatchComponent(ID,BATCHID,MIXTURECOMPONENTID,ORDERINDEX)
                    (SELECT SEQ_BatchComponent.NEXTVAL,ABatchID,MixtureComponentID,(SELECT NVL(MAX(OrderIndex),1) FROM VW_BatchComponent WHERE MixtureComponentID=MC_Target.MixtureComponentID)
                        FROM VW_Mixture_Component MC_Target
                        WHERE MC_Target.MixtureID=LTargeMixtureID);
                 $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponent: ' || SQL%ROWCOUNT); $end null;

                INSERT INTO VW_BatchComponentFragment(BATCHCOMPONENTID,COMPOUNDFRAGMENTID,EQUIVALENT,ORDERINDEX)
                    SELECT BC.ID,CF.ID,1,1
                        FROM VW_Compound_Fragment CF, VW_Mixture_Component MC_Target,VW_BatchComponent BC
                        WHERE MC_Target.MixtureComponentID= BC.MixtureComponentID AND MC_Target.CompoundID=CF.CompoundID AND MC_Target.MixtureID=LTargeMixtureID AND
                        NOT EXISTS ( SELECT 1 FROM VW_BatchComponentFragment BCF1 WHERE BCF1.BatchComponentID=BC.ID AND BCF1.CompoundFragmentID=CF.ID);
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponentFragment: ' || SQL%ROWCOUNT); $end null;
            ELSE
                SELECT Count(1)
                    INTO LMatchFragment
                    FROM
                        (SELECT CF_Source.FragmentID
                            FROM VW_Compound_Fragment CF_Source, VW_Mixture_Component MC_Source
                            WHERE  CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID
                            GROUP BY CF_Source.FragmentID
                        MINUS
                        SELECT CF_Target.FragmentID
                            FROM VW_Compound_Fragment CF_Target, VW_Mixture_Component MC_Target
                            WHERE  CF_Target.CompoundID=MC_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID
                            GROUP BY CF_Target.FragmentID);

                IF LMatchFragment<>0 THEN
                    LErrorMessage:='The Fragment List from source and target record is different. New fragment list was applied to the moved batch.';
                    LErrorCode:= eFragmentNotMatchIdentityTrue;
                END IF;

                INSERT INTO VW_Compound_Fragment(ID,FragmentID,CompoundID)
                    (SELECT Seq_Compound_Fragment.NextVal,CF_Source.FragmentID, MC_Target.CompoundID
                        FROM  VW_BatchComponentFragment BCF_Source, VW_BatchComponent BC_Source, VW_Compound_Fragment CF_Source ,VW_Mixture_Component MC_Source, VW_Mixture_Component MC_Target
                        WHERE BC_Source.BatchID=ABatchID AND BCF_Source.BatchComponentID=BC_Source.ID AND BCF_Source.CompoundfragmentID= CF_Source.ID AND CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID AND
                              MC_Target.MixtureID=LTargeMixtureID AND
                              CF_Source.FragmentID NOT IN (SELECT CF_Target.FragmentID
                                                        FROM VW_Compound_Fragment CF_Target,VW_Mixture_Component MC_Target
                                                        WHERE MC_Target.CompoundID=CF_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID));
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_Compound_Fragment: ' || SQL%ROWCOUNT); $end null;


                INSERT INTO VW_BatchComponent(ID,BATCHID,MIXTURECOMPONENTID,ORDERINDEX)
                    (SELECT SEQ_BatchComponent.NEXTVAL,ABatchID,MixtureComponentID,(SELECT NVL(MAX(OrderIndex),1) FROM VW_BatchComponent WHERE MixtureComponentID=MC_Target.MixtureComponentID)
                        FROM VW_Mixture_Component MC_Target
                        WHERE MC_Target.MixtureID=LTargeMixtureID);
                 $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponent: ' || SQL%ROWCOUNT); $end null;

                INSERT INTO VW_BatchComponentFragment(BATCHCOMPONENTID,COMPOUNDFRAGMENTID,EQUIVALENT,ORDERINDEX)
                    SELECT BC_Target.ID,CF_Target.ID,BCF_Source.Equivalent,BCF_Source.OrderIndex
                        FROM VW_Compound_Fragment CF_Target, VW_Mixture_Component MC_Target,VW_BatchComponent BC_Target,
                             VW_BatchComponentFragment BCF_Source, VW_BatchComponent BC_Source, VW_Compound_Fragment CF_Source ,VW_Mixture_Component MC_Source
                        WHERE BC_Target.BatchID=ABatchID AND MC_Target.MixtureComponentID= BC_Target.MixtureComponentID AND MC_Target.CompoundID=CF_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID AND
                              CF_Target.FragmentID = CF_Source.FragmentID AND
                              BC_Source.BatchID=ABatchID AND BCF_Source.BatchComponentID=BC_Source.ID AND BCF_Source.CompoundfragmentID= CF_Source.ID AND CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID;
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponentFragment: ' || SQL%ROWCOUNT); $end null;

                DELETE VW_BatchComponentFragment BC
                    WHERE BatchComponentID IN (SELECT BC_Source.ID
                                    FROM  VW_BatchComponent BC_Source, VW_Mixture_Component MC_Source
                                    WHERE BC_Source.BatchID=ABatchID AND BC_Source.MixtureComponentID=MC_Source.MixtureComponentID AND MC_Source.MixtureID=LSourceMixtureID);
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponentFragmen: ' || SQL%ROWCOUNT); $end null;

                DELETE VW_BatchComponent BC
                    WHERE ID IN (SELECT BC_Source.ID
                                    FROM  VW_BatchComponent BC_Source, VW_Mixture_Component MC_Source
                                    WHERE BC_Source.BatchID=ABatchID AND BC_Source.MixtureComponentID=MC_Source.MixtureComponentID AND MC_Source.MixtureID=LSourceMixtureID) ;
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponent: ' || SQL%ROWCOUNT); $end null;

            END IF;

            LRLSState:=RegistrationRLS.GetStateRLS;
            IF LRLSState THEN
                RegistrationRLS.SetEnableRLS(False);
            END IF;

            SELECT NVL(MAX(BatchNumber),0)+1
                INTO LBatchNumber
                FROM VW_Batch
                WHERE RegID=LTargetRegID;

            IF LRLSState THEN
                RegistrationRLS.SetEnableRLS(LRLSState);
            END IF;

            LFullRegNumber:=ARegNumber;

            $if CompoundRegistry.Debuging $then InsertLog('MoveBatch','LSequenceID='||LSequenceID||' ARegNumber='||ARegNumber||' LBatchNumber='||LBatchNumber); $end null;
            RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord);
            LFullRegNumber:=GetFullRegNumber(LSequenceID,XmlType(LXMLRegistryRecord),ARegNumber,LBatchNumber,GetBatchNumberPadding());

            $if CompoundRegistry.Debuging $then InsertLog('MoveBatch','LFullRegNumber='||LFullRegNumber); $end null;

            UPDATE VW_Batch
                SET RegID = LTargetRegID,BatchNumber=LBatchNumber,FullregNumber=LFullRegNumber
                WHERE BatchID=ABatchID;

            COMMIT;

            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'UPDATE VW_Batch SQL%ROWCOUNT : ' || SQL%ROWCOUNT); $end null;

            IF LErrorMessage IS NOT NULL THEN
                LErrorMessage:='Components do not match between registries. '||LErrorMessage||' The batch was moved.';
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LErrorMessage: ' || LErrorMessage); $end null;
                Raise_Application_Error (LErrorCode,LErrorMessage);
            ELSE
                RETURN;
            END IF;

        END IF;

        IF LCompoundCountRegistry > LCompoundCountBatch THEN
            LErrorMessage:='The target record has more components than the source. New batch component information should be manually adjusted on the target record.';
            LErrorCode:=eMoreComponentsTarget;
        ELSIF LCompoundCountRegistry < LCompoundCountBatch THEN
            LErrorMessage:='The source record has more components than the target.';
            LErrorCode:=eMoreComponentsSource;
        END IF;

        SELECT Min(BatchID)
           INTO LTargetBatchID
           FROM VW_Batch
           WHERE RegID=LTargetRegID AND BatchID<>ABatchID;
        $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetBatchID: ' || LTargetBatchID); $end null;

        SELECT Count(1)
            INTO LTargetComponentCount
            FROM VW_BatchComponent
            WHERE BatchID=LTargetBatchID;
        $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetComponentCount: ' || LTargetComponentCount); $end null;

        SELECT RN.RegID,MRN.MixtureID
            INTO LTargetRegID,LTargeMixtureID
            FROM  VW_RegistryNumber RN,VW_Mixture_RegNumber MRN
            WHERE MRN.RegID=RN.RegID AND RN.RegNumber=ARegNumber;
        $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetRegID: ' || LTargetRegID||' LTargeMixtureID: ' || LTargeMixtureID); $end null;

        DELETE VW_BatchComponentFragment BC
            WHERE BatchComponentID IN (SELECT ID FROM VW_BatchComponent WHERE BatchID=ABatchID);

        DELETE VW_BatchComponent BC1
            WHERE BatchID=ABatchID;

        INSERT INTO VW_BatchComponent(ID,BATCHID,MIXTURECOMPONENTID,ORDERINDEX)
            (SELECT SEQ_BatchComponent.NEXTVAL,ABatchID,MixtureComponentID,OrderIndex
                FROM VW_BatchComponent
                WHERE BatchID=LTargetBatchID);

        LRLSState:=RegistrationRLS.GetStateRLS;
        IF LRLSState THEN
            RegistrationRLS.SetEnableRLS(False);
        END IF;

        SELECT NVL(MAX(BatchNumber),0)+1
            INTO LBatchNumber
            FROM VW_Batch
            WHERE RegID=LTargetRegID;

        IF LRLSState THEN
            RegistrationRLS.SetEnableRLS(LRLSState);
        END IF;

        LFullRegNumber:=ARegNumber;

        $if CompoundRegistry.Debuging $then InsertLog('MoveBatch','LSequenceID='||LSequenceID||' ARegNumber='||ARegNumber||' LBatchNumber='||LBatchNumber); $end null;
        RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord);
        LFullRegNumber:=GetFullRegNumber(LSequenceID,XmlType(LXMLRegistryRecord),ARegNumber,LBatchNumber,GetBatchNumberPadding());

        $if CompoundRegistry.Debuging $then InsertLog('MoveBatch','LFullRegNumber='||LFullRegNumber); $end null;

        UPDATE VW_Batch
            SET RegID = LTargetRegID,BatchNumber=LBatchNumber,FullregNumber=LFullRegNumber
            WHERE BatchID=ABatchID;


        $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'UPDATE VW_Batch SQL%ROWCOUNT : ' || SQL%ROWCOUNT); $end null;
        COMMIT;

        IF LErrorMessage IS NOT NULL THEN
            LErrorMessage:='Components do not match between registries. '||LErrorMessage||' The batch was moved.';
            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LErrorMessage: ' || LErrorMessage); $end null;
            Raise_Application_Error (LErrorCode,LErrorMessage);
        END IF;
    END IF;


EXCEPTION
    WHEN OTHERS THEN
        $if CompoundRegistry.Debuging $then InsertLog('MoveBatch',chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
        RAISE_APPLICATION_ERROR(EGenericException, AppendError('MoveBatch', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;


PROCEDURE DeleteBatch(ABatchID in Number) IS
    LBatchCount Integer;
BEGIN
    SELECT COUNT(1)
        INTO  LBatchCount
        FROM  VW_Batch
        WHERE REGID = (SELECT RegID FROM  VW_Batch WHERE BatchID=ABatchID);

    IF LBatchCount=0 THEN
        BEGIN
            Raise_Application_Error (eNoRowsReturned, AppendError('No rows returned. The batch was not moved.'));
        END;
    ELSIF LBatchCount=1 THEN
        BEGIN
            Raise_Application_Error (eOnlyOneBatchDeleting,
              AppendError('The Batch''s Registry has only one Batch. The Batch was not deleted.'));
        END;
    ELSE
        BEGIN
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'ABatchID: ' || ABatchID); $end null;
            DELETE VW_BATCH_PROJECT WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCH_PROJECT SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCHCOMPONENTFRAGMENT WHERE BATCHCOMPONENTID IN (SELECT ID FROM VW_BATCHCOMPONENT WHERE BATCHID=ABatchID);
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCH_BATCHCOMPONENTFRAGMENT%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_COMPOUND_FRAGMENT WHERE ID NOT IN (SELECT CompoundFragmentID FROM VW_BATCHCOMPONENTFRAGMENT);
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_COMPONENTFRAGMENT%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCHCOMPONENT WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCHCOMPONENT SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCHIDENTIFIER WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCHIDENTIFIER SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCH WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCH SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
        END;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('DeleteBatch', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;

FUNCTION ExtractRegNumber (AXmlError IN CLOB, ATagError IN VARCHAR2,AFilter IN VARCHAR2:=NULL) RETURN VARCHAR2 IS
    LTagBeginError   VARCHAR2 (255);
    LTagEndError     VARCHAR2 (255);
    LStartIndex      NUMBER;
    LEndIndex        NUMBER;
    LXmlDuplicates   CLOB;
    LPath            VARCHAR2 (100) := '';
    LRegnumber       VARCHAR2 (100) := '';
BEGIN
    -- Extract Reg List Duplicates
    LTagBeginError := '<' || ATagError || '>';
    LTagEndError := '</' || ATagError || '>';
    LStartIndex := INSTR (AXmlError, LTagBeginError);
    LEndIndex := INSTR (AXmlError, LTagEndError) + LENGTH (LTagEndError);
    LXmlDuplicates := SUBSTR (AXmlError, LStartIndex, LEndIndex - LStartIndex);

    IF ATagError = 'COMPOUNDLIST' THEN
        LPath := 'COMPOUNDLIST/COMPOUND[1]/';
    END IF;

    IF AFilter IS NOT NULL THEN
        LPath:=LPath || 'REGISTRYLIST/REGNUMBER['||AFilter||'][1]/node()[1]';
    ELSE
        LPath:=LPath || 'REGISTRYLIST/REGNUMBER[1]/node()[1]';
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog ('ExtractRegNumber', 'LXmlDuplicates ' || LXmlDuplicates|| 'Path: '||LPath); $end null;

    SELECT EXTRACTVALUE (XMLTYPE.CreateXml (LXmlDuplicates), LPath)
      INTO LRegNumber
      FROM DUAL;

    RETURN LRegNumber;
END;

/**
As part of the 'Add as new batch' duplicate-resolution strategy, performs a
cut/paste operation on a COEBatch during the conversion of a new or temporary
registration into an existing permanent one.
-->author Fari
-->since 17 March 2010
-->param AXml       this has the new batch and return the new Registry with the new batch
-->param AXmlError  this has Duplicated Reg List
-->param ATagError  this has the tag for identify the Duplicated Reg
-->param AFilter    this has a filter for identify the Duplicated Reg
*/
PROCEDURE AddBatch (AXml IN OUT NOCOPY CLOB, AXmlError IN CLOB, ATagError IN VARCHAR2,AFilter IN VARCHAR2:=NULL) IS
    LRegNumber          VARCHAR2 (50) := '';
    LXml                          XMLTYPE;
    LXmlNewBatch                  XMLTYPE;
    LXmlNewFragment               XMLTYPE;
  --LXmlNewBatchComponentFragment XMLTYPE;
    LXmlRows                      CLOB;
   -- LXmlTypeRows                  XMLTYPE;
    LCompoundID         VW_Compound.CompoundID%TYPE;
    LTempBatchID        VW_Batch.BatchID%TYPE;
    LPersonRegistered   VW_RegistryNumber.PersonRegistered%TYPE;
    LMixtureComponentID VW_Mixture_Component.MixtureComponentID%TYPE;
    LIndex              Integer:=0;


    --Get Fragments isn't in target.
    CURSOR C_NewFragments(ARegNumber in VW_RegistryNumber.RegNumber%type,AXmlNewBatch XMLTYPE) IS
        SELECT FragmentID
            FROM (
                    SELECT  extract(value(FragmentIDs), '//text()').getNumberVal() FragmentID
                        FROM Table(XMLSequence(extract(AXmlNewBatch, '/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment/FragmentID'))) FragmentIDs
                    MINUS
                    SELECT CF.FragmentID FragmentID
                        FROM VW_Compound_Fragment CF, VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
                       WHERE MC.CompoundID=CF.CompoundID AND RN.RegID = MR.RegID  AND MR.MixtureID=MC.MixtureID AND RN.RegNumber = ARegNumber
                );

    --Get Mixture's Componentes Fragments
    CURSOR C_Components(ARegNumber in VW_RegistryNumber.RegNumber%type) IS
        SELECT MC.CompoundID,MC.MixtureComponentID,RN.PersonRegistered
            INTO LCompoundID,LMixtureComponentID,LPersonRegistered
            FROM VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
            WHERE RN.RegID = MR.RegID  AND MR.MixtureID=MC.MixtureID AND RN.RegNumber = ARegNumber;
BEGIN

    --Get the registry record the compound was originally found in: AXmlError is a listing of such records
    LRegNumber := ExtractRegNumber (AXmlError, ATagError,AFilter);
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LRegNumber:' || LRegNumber); $end null;

    --Get target Registry
    RetrieveMultiCompoundRegistry (LRegNumber, LXmlRows);
     $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlRows:' || LXmlRows); $end null;

    LXml:=XmlType.CreateXml(AXml);
    --LXmlTypeRows:=XmlType.CreateXml(LXmlRows);


    --Get the existing batch information as an xml document for easier manipulation
    SELECT  extract(LXml, '/MultiCompoundRegistryRecord/BatchList/Batch'),
            extractValue(LXml, '/MultiCompoundRegistryRecord/BatchList/Batch/BatchID')
        INTO
            LXmlNewBatch,
            LTempBatchID FROM DUAL;
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatch 1:' || LXmlNewBatch.getClobVal()); $end null;

    --Replace the existing BatchComponentList data as necessary:
    SELECT updateXml(
            LXmlNewBatch,
            '/Batch/BatchID/text()', '0',
            '/Batch/BatchComponentList/BatchComponent/BatchID/text()', '0')
        INTO LXmlNewBatch FROM DUAL;

    FOR R_Components IN C_Components(LRegNumber) LOOP
        LIndex:=LIndex+1;
        SELECT updateXml(
                LXmlNewBatch,
                '/Batch/BatchComponentList/BatchComponent['||LIndex||']/CompoundID/text()', R_Components.CompoundID,
                '/Batch/BatchComponentList/BatchComponent['||LIndex||']/ComponentIndex/text()', -R_Components.CompoundID )
            INTO LXmlNewBatch FROM DUAL;
        SELECT insertChildXml(LXmlNewBatch, '/Batch/BatchComponentList/BatchComponent['||LIndex||']', 'MixtureComponentID', XMLType('<MixtureComponentID>'||R_Components.MixtureComponentID||'</MixtureComponentID>'))
            INTO LXmlNewBatch FROM dual;

        --BatchComponentFragment
       /* IF GetSameBatchesIdentity='True' THEN
            SELECT  XmlType('<BatchComponentFragmentList>'||extract(LXmlTypeRows, '/MultiCompoundRegistryRecord/BatchList/Batch[1]/BatchComponentList/BatchComponent[CompoundID='||R_Components.CompoundID||']/BatchComponentFragmentList/BatchComponentFragment').getStringVal()||'</BatchComponentFragmentList>')
                INTO LXmlNewBatchComponentFragment FROM DUAL;
             $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatch CON LXmlNewBatchComponentFragment:' || LXmlNewBatchComponentFragment.getClobVal()); $end null;
            SELECT updateXml(
                LXmlNewBatchComponentFragment,
                'BatchComponentFragmentList/BatchComponentFragment/ID/text()',0)
                INTO LXmlNewBatchComponentFragment FROM DUAL;
             SELECT insertChildXml(LXmlNewBatchComponentFragment, 'BatchComponentFragmentList/BatchComponentFragment', '@insert', 'yes')
                INTO LXmlNewBatchComponentFragment FROM dual;
             $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatchComponentFragment:' || LXmlNewBatchComponentFragment.getClobVal()); $end null;
            SELECT insertChildXml(LXmlNewBatch, '/Batch/BatchComponentList/BatchComponent['||LIndex||']', 'BatchComponentFragmentList', LXmlNewBatchComponentFragment)
                INTO LXmlNewBatch FROM dual;

         END IF;*/
    END LOOP;

   $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatch 2:' || LXmlNewBatch.getClobVal()); $end null;

    --Mark this batch as an insert
    SELECT insertChildXml(LXmlNewBatch, '/Batch', '@insert', 'yes')
        INTO LXmlNewBatch FROM dual;
    SELECT insertChildXml(LXmlNewBatch, '/Batch/BatchComponentList/BatchComponent', '@insert', 'yes')
        INTO LXmlNewBatch FROM dual;
    SELECT insertChildXml(LXmlNewBatch, '/Batch/ProjectList/Project', '@insert', 'yes')
        INTO LXmlNewBatch FROM dual;
    IF  LXmlNewBatch.existsNode('/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment[@insert="yes"]')=0 THEN
        SELECT insertChildXml(LXmlNewBatch, '/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment', '@insert', 'yes')
            INTO LXmlNewBatch FROM dual;
    END IF;
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatch 3:' || LXmlNewBatch.getClobVal()); $end null;

    --This batch need more data
    IF LXmlNewBatch.ExistsNode('/Batch/FullRegNumber')=0 THEN
        SELECT insertChildXml(LXmlNewBatch, '/Batch', 'FullRegNumber', XMLType('<FullRegNumber></FullRegNumber>'))
            INTO LXmlNewBatch FROM dual;
    END IF;
    SELECT insertChildXml(LXmlNewBatch, '/Batch', 'TempBatchID', XMLType('<TempBatchID>'||LTempBatchID||'</TempBatchID>'))
        INTO LXmlNewBatch FROM dual;
    SELECT insertChildXml(LXmlNewBatch, '/Batch', 'PersonRegistered', XMLType('<PersonRegistered>'||LPersonRegistered||'</PersonRegistered>'))
        INTO LXmlNewBatch FROM dual;

    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlNewBatch5:' || LXmlNewBatch.getClobVal()); $end null;

    IF not GetSameBatchesIdentity='True' THEN
        --Add Frament isn't in target
        FOR R_NewFragments IN C_NewFragments(LRegNumber,LXmlNewBatch) LOOP
            SELECT extract(LXml, '/MultiCompoundRegistryRecord/ComponentList/Component/Compound/FragmentList/Fragment[FragmentID='||R_NewFragments.FragmentID||']')
                INTO LXmlNewFragment FROM DUAL;
             --Mark this Fragment as an insert
            SELECT insertChildXml(LXmlNewFragment, '/Fragment', '@insert', 'yes')
                INTO LXmlNewFragment FROM dual;
             $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlNewFragment:' || LXmlNewFragment.getClobVal()); $end null;
            SELECT insertChildXml(XmlType(LXmlRows), '/MultiCompoundRegistryRecord/ComponentList/Component/Compound/FragmentList','Fragment' , LXmlNewFragment).GetClobVal()
                INTO LXmlRows FROM dual;
             $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlRows with Fragment:' || LXmlRows); $end null;
         END LOOP;
    END IF;


    --Append this cloned batch to the pre-existing perm. reg. record
    SELECT insertChildXml(XmlType(LXmlRows), '/MultiCompoundRegistryRecord/BatchList','Batch' , LXmlNewBatch).GetClobVal()
        INTO AXml FROM dual;
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'AXml:' || AXml); $end null;

EXCEPTION
    WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('AddBatch', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;

PROCEDURE UseCompound (AXml IN OUT NOCOPY CLOB, AXmlError IN CLOB, ATagError IN VARCHAR2) IS
    LRegnumber         VARCHAR2 (100) := '';
    LXmlTrash          CLOB;
    LXmlCompoundList   CLOB;
    LXmlCompound       CLOB;
    LPosTagEnd         NUMBER;
    LRegistryList      CLOB;
    LXmlType           XmlType;
    CURSOR C_RegNumbersCompound(AXmlError IN CLOB) IS
        SELECT RN.RegNumber ,extractvalue((value(CompoundIDs)),'COMPOUND/TEMPCOMPOUNDID') TempCompoundID
            FROM VW_RegistryNumber RN,VW_Compound C ,Table(XMLSequence(extract(XmlType(AXmlError),'COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER[@count="1"]/../..'))) CompoundIDs
            WHERE C.RegID=RN.RegID AND C.CompoundID =  extractvalue((value(CompoundIDs)),'COMPOUND/REGISTRYLIST/REGNUMBER/@CompoundID');
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'AXmlError: ' || AXmlError || ' ATagError: ' || ATagError); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'AXml: ' || AXml ); $end null;
    LXmlType:=XmlType(AXml);
    FOR R_RegNumbersCompound IN C_RegNumbersCompound(AXmlError) LOOP

        LRegistryList:='<REGISTRYLIST><REGNUMBER>' || R_RegNumbersCompound.RegNumber || '</REGNUMBER></REGISTRYLIST>';
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'LRegistryList: ' || LRegistryList ); $end null;
        RetrieveCompoundRegistryList (LRegistryList, LXmlCompoundList);
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Compound List a: ' || LXmlCompoundList); $end null;
        LXmlCompound := '<Compound>'||TakeOffAndGetClob (LXmlCompoundList, 'Compound')||'</Compound>';
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Compound List b: ' || LXmlCompoundList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Compound: ' || LXmlCompound); $end null;

        SELECT UpdateXML(LXmlType,'/MultiCompoundRegistryRecord/ComponentList/Component/Compound[CompoundID='||R_RegNumbersCompound.TempCompoundID||']',LXmlCompound)
            INTO LXmlType
            FROM DUAL;
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'R_RegNumbersCompound.TempCompoundID: ' || R_RegNumbersCompound.TempCompoundID||' LXmlType.GetClobValue()'||LXmlType.GetClobVal()); $end null;

    END LOOP;

    AXml:=LXmlType.GetClobVal();

    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Xml con Compound: ' || AXml); $end null;
EXCEPTION
    WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('UseCompound', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;

PROCEDURE LoadMultiCompoundRegRecord(
  ARegistryXml IN CLOB
  , ADuplicateAction IN CHAR
  , AAction OUT NOCOPY CHAR
  , AMessage OUT NOCOPY CLOB
  , ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%TYPE
  , ARegistration IN CHAR := 'Y'
  , ARegNumGeneration IN CHAR := 'Y'
  , AConfigurationID IN Number := 1
  , ASectionsList IN Varchar2:=NULL
) IS
    LXmlRows       CLOB           := ARegistryXml;
    LTempId        NUMBER;
    LError         CLOB;
    LMessage       CLOB;
    LRegNumber     CLOB;
    LDiscard       CLOB;
    LXMLRegistryRecord   CLOB;
    LRegNumberToAddBatch CLOB;

    FUNCTION ExistOneMixtureSameStructures(AXmlRows IN CLOB, AError IN CLOB, ARegNumberToAddBatch OUT VARCHAR2,AMessage OUT CLOB) RETURN BOOLEAN IS
        LCountCompound NUMBER;
        LCountCompoundDuplicated NUMBER;  --in same mixture
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'LXmlRows: ' || LXmlRows); $end null;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'AError: ' || AError); $end null;

        SELECT  COUNT(1)
            INTO LCountCompound
            FROM Table(XMLSequence(extract(XmlType(AXmlRows), 'MultiCompoundRegistryRecord/ComponentList/Component'))) Components;

        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'LCountCompound: ' || LCountCompound); $end null;

        BEGIN
            IF GetSameBatchesIdentity='False' THEN
                SELECT  count(1) CountCompoundDuplicated,  extract(value(RegNumbers), '//text()').GetStringVal() RegNumbers
                    INTO LCountCompoundDuplicated,ARegNumberToAddBatch
                    FROM Table(XMLSequence(extract(XmlType(AError), 'COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER'))) RegNumbers
                    WHERE LCountCompound=(SELECT Count(1)
                                            FROM VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
                                            WHERE  RN.RegID = MR.RegID  AND MR.MixtureID=MC.MixtureID AND RN.RegNumber = extract(value(RegNumbers), '//text()').GetStringVal() )
                    GROUP BY extract(value(RegNumbers), '//text()').GetStringVal();
            ELSE
                 SELECT  count(1) CountCompoundDuplicated,  extract(value(RegNumbers), '//text()').GetStringVal() RegNumbers
                    INTO LCountCompoundDuplicated,ARegNumberToAddBatch
                    FROM Table(XMLSequence(extract(XmlType(AError), 'COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER[@SAMEEQUIVALENT="True"]'))) RegNumbers
                    WHERE LCountCompound=(SELECT Count(1)
                                            FROM VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
                                            WHERE  RN.RegID = MR.RegID  AND MR.MixtureID=MC.MixtureID AND RN.RegNumber = extract(value(RegNumbers), '//text()').GetStringVal() )
                    GROUP BY extract(value(RegNumbers), '//text()').GetStringVal();
            END IF;
        EXCEPTION
            WHEN TOO_MANY_ROWS THEN --then exist more than 1 mixture with some strcutres
                AMessage:='Conflicts with duplicated.';
                ARegNumberToAddBatch:=NULL;
            WHEN NO_DATA_FOUND THEN
                ARegNumberToAddBatch:=NULL;
        END;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'CountCompoundDuplicated: ' || LCountCompoundDuplicated); $end null;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'ARegNumberToAddBatch: ' || ARegNumberToAddBatch); $end null;

        IF ARegNumberToAddBatch IS NOT NULL THEN
            RETURN TRUE;
        ELSE
            RETURN FALSE;
        END IF;
    END;

    FUNCTION ExistOneStructureByDuplicated(AXmlRows IN CLOB, AError IN CLOB, AMessage OUT CLOB) RETURN BOOLEAN IS
        LExistOneStructureByDuplicated NUMBER;
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneStructureByDuplicated', 'LXmlRows: ' || LXmlRows); $end null;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneStructureByDuplicated', 'AError: ' || AError); $end null;

        IF GetSameBatchesIdentity='False' THEN
            SELECT   Count(1)  --extractvalue((value(RegNumbers)),'COMPOUND/TEMPCOMPOUNDID') TempCompoundID,
                               --       (SELECT  Count(1)
                               --          FROM Table(XMLSequence(extract(value(RegNumbers), 'COMPOUND/REGISTRYLIST/REGNUMBER'))) RegNumbers) CountDuplicated
                INTO LExistOneStructureByDuplicated
                FROM Table(XMLSequence(extract(XmlType(AError), 'COMPOUNDLIST/COMPOUND'))) RegNumbers
                WHERE (SELECT  Count(1)
                          FROM Table(XMLSequence(extract(value(RegNumbers), 'COMPOUND/REGISTRYLIST/REGNUMBER'))) RegNumbers)>1;
        ELSE
            SELECT   Count(1)
                INTO LExistOneStructureByDuplicated
                FROM Table(XMLSequence(extract(XmlType(AError), 'COMPOUNDLIST/COMPOUND'))) RegNumbers
                WHERE (SELECT  Count(1)
                          FROM Table(XMLSequence(extract(value(RegNumbers), 'COMPOUND/REGISTRYLIST/REGNUMBER'))) RegNumbers)>1
                       OR ExtractValue(value(RegNumbers), 'COMPOUND/REGISTRYLIST/REGNUMBER[1]/@SAMEEQUIVALENT')='False'; 
                       -- Even if the first operand of the OR operator returns true, the second one (ExtractValue) will still be calculated
        END IF;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneStructureByDuplicated', 'LExistOneStructureByDuplicated=' || LExistOneStructureByDuplicated); $end null;

        IF LExistOneStructureByDuplicated=0 THEN
            RETURN TRUE;
        ELSE
            AMessage:='Conflicts with duplicated.';
            RETURN FALSE;
        END IF;
    END;
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegistryXml: ' || ARegistryXml); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ADuplicateAction: ' || ADuplicateAction); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegistration: ' || ARegistration); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AConfigurationID: ' || AConfigurationID); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ASectionsList: ' || ASectionsList); $end null;

    AAction := 'E';
    IF ARegistration = 'Y' THEN
        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegistration: ' || ARegistration); $end null;

        CreateMultiCompoundRegistry (LXmlRows, ARegNumber, AMessage, 'C', ARegNumGeneration, AConfigurationID, ASectionsList);

        --split the Response into its parts
        ExtractRegistrationResponse(AMessage, LMessage, LError, LDiscard);

        IF LError IS NULL THEN
            --the outgoing message (AMessage) parameter is already populated
            AAction := 'C';
            $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'RegNumber: ' || ARegNumber); $end null;
        ELSE
            --reset the outgoing message
            AMessage := null;
            AAction := ADuplicateAction;
            IF ADuplicateAction = 'D' THEN
                CreateMultiCompoundRegistry (LXmlRows, ARegNumber, AMessage, 'N', ARegNumGeneration, 1, ASectionsList);
            ELSIF ADuplicateAction = 'B' THEN
                LRegNumber := LError; -- Copy so we won't destroy LError
                LRegNumber := TakeOffAndGetClobsList(LRegNumber, 'REGNUMBER');  -- Get the RegNumbers
                LRegNumber := TakeOffAndGetClob(LRegNumber, 'Clob');  -- Get only the first one
                LDiscard := TakeOffAndGetClobsList(LRegNumber, 'SAMEFRAGMENT'); -- Discard
                LDiscard := TakeOffAndGetClobsList(LRegNumber, 'SAMEEQUIVALENT'); -- Discard
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ADuplicateAction = ''B'''); $end null;
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'LError: ' || LError); $end null;
                IF INSTR (LError, '<COMPOUNDLIST>') > 0 AND INSTR (LError, '</COMPOUNDLIST>') > 0 THEN
                    IF ExistOneMixtureSameStructures(LXmlRows,LError,LRegNumberToAddBatch,LMessage) THEN
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Adding Batch...LXmlRows: ' || LXmlRows); $end null;
                        AddBatch (LXmlRows, LError, 'COMPOUNDLIST','. = "'||LRegNumberToAddBatch||'"');
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Updating... LXmlRows:'|| LXmlRows); $end null;
                        UpdateMultiCompoundRegistry (LXmlRows, AMessage, 'D', 1, ASectionsList);
                        ARegNumber := LRegNumber;
                        -- Preserve the original message and conditionally return the new object
                        IF ARegNumber IS NOT NULL THEN
                            BEGIN
                                IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                                    RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord, ASectionsList);
                                END IF;
                                AMessage := CreateRegistrationResponse(LMessage, NULL, LXMLRegistryRecord);
                            end;
                        END IF;
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'After Update' || LRegNumber); $end null;
                    ELSIF ExistOneStructureByDuplicated(LXmlRows,LError,LMessage) THEN
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Using Compund...LXmlRows: ' || LXmlRows); $end null;
                        UseCompound (LXmlRows, LError, 'COMPOUNDLIST');
                        CreateMultiCompoundRegistry (LXmlRows, ARegNumber, AMessage, 'D', ARegNumGeneration);
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AMessage:' || AMessage); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegNumber:' || ARegNumber); $end null;
                        -- Preserve the original message and conditionally return the new object
                        IF ARegNumber IS NOT NULL THEN
                            BEGIN
                                IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                                    RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord, ASectionsList);
                                END IF;
                                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'LMessage:' || LMessage); $end null;
                                AMessage := CreateRegistrationResponse(LMessage, NULL, LXMLRegistryRecord);
                                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AMessage:' || AMessage); $end null;
                            end;
                        END IF;
                        AAction := 'U';
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'UseCompound (COMPOUNDLIST): ' || LRegNumber); $end null;
                    ELSE
                        ARegNumber := '';
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'LError:' || LError); $end null;
                        AAction := 'N';
                        AMessage := CreateRegistrationResponse(LMessage, LError, NULL);
                    END IF;
                ELSIF INSTR (LError, '<REGISTRYLIST>') > 0 AND INSTR (LError, '</REGISTRYLIST>') > 0 THEN
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Before-AddBatch LXmlRows): ' || LXmlRows); $end null;
                    AddBatch (LXmlRows, LError, 'REGISTRYLIST');
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'After-AddBatch LXmlRows): ' || LXmlRows); $end null;
                    UpdateMultiCompoundRegistry (LXmlRows, LXMLRegistryRecord, 'D', ASectionsList);
                    ARegNumber := LRegNumber;
                    -- Preserve the original message and conditionally return the new object
                    IF ARegNumber IS NOT NULL THEN
                        BEGIN
                            IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                                RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord, ASectionsList);
                            END IF;
                            AMessage := CreateRegistrationResponse(LMessage, NULL, LXMLRegistryRecord);
                        END;
                    END IF;
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AddBatch (REGISTRYLIST): ' || LRegNumber); $end null;
                ELSE
                    Raise_Application_Error (EGenericException,
                      AppendError('Duplciation errors exist but were incorrectly formatted.'));
                END IF;
            ELSIF ADuplicateAction = 'T' THEN
                CreateMultiCompoundRegTmp (LXmlRows, LTempID, AMessage, ASectionsList);
                ARegNumber := LTempID; --Necessary in DataLoader Application
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID1: ' || LTempID); $end null;
            ELSIF ADuplicateAction = 'N' THEN
                ARegNumber := '0';
                AMessage := CreateRegistrationResponse('Duplicate found', LError, NULL);
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID2: ' || LTempID); $end null;
            ELSE
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
                Raise_Application_Error (EGenericException,
                  AppendError('Invalid duplicate-resolution instruction: "' || ADuplicateAction || '"'));
            END IF;
       END IF;
    ELSE
        CreateMultiCompoundRegTmp (LXmlRows, LTempID, AMessage);
        ARegNumber := LTempID;
        IF ARegNumber IS NOT NULL THEN
            BEGIN
                IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                    RetrieveMultiCompoundRegTmp(ARegNumber, LXMLRegistryRecord);
                END IF;
                AMessage := CreateRegistrationResponse(LMessage, NULL, LXMLRegistryRecord);
            END;
        END IF;
        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID3: ' || LTempID); $end null;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord',  DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('LoadMultiCompoundRegRecord', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;

PROCEDURE LoadMultiCompoundRegRecordList(
  ARegistryListXml IN CLOB
  , ADuplicateAction IN CHAR
  , ARegistration IN CHAR
  , ARegNumGeneration IN CHAR
  , AConfigurationID IN NUMBER
  , LogID IN NUMBER
) IS
    -- returns to C# every 100 records, so LogID has to be passed in
    LATempID        VARCHAR2(8) := '0';
    LXmlRows        CLOB;
    LXmlCompReg     CLOB := ARegistryListXml;
    AMessage        CLOB;

    -- temporarily. should get from xml. need confirmation
    ADuplicateCheck CHAR := 'C';
    -- temporarily. should be returned from LoadMultiCompoundRegRecord
    IsDuplicate     NUMBER := 0;
    RegID           NUMBER := 1;
    BatchID         NUMBER := 1;
    LAction         CHAR;
    LRegNum         VARCHAR2(100) := '';
BEGIN
    LOOP
        BEGIN
            SAVEPOINT one_record;
            LXmlRows := TakeOffAndGetClob(LXmlCompReg,
                                          'MultiCompoundRegistryRecord');
            EXIT WHEN LXmlRows IS NULL;
            LXmlRows := '<MultiCompoundRegistryRecord>' || LXmlRows || '</MultiCompoundRegistryRecord>';
            SELECT EXTRACTVALUE(XMLTYPE.CreateXml(LXmlRows),
                                '/MultiCompoundRegistryRecord/ID[1]')
              INTO LATempID
              FROM DUAL;

            /* Commented out until the extra parameters are returned.
            LoadMultiCompoundRegRecord(LXmlRows, ADuplicateAction, ARegistration, ARegNumGeneration, AConfigurationID, ADuplicateCheck, IsDuplicate, RegID, BatchID);
            */
            LoadMultiCompoundRegRecord(LXmlRows, ADuplicateAction, LAction, AMessage, LRegNum, ARegistration, ARegNumGeneration, AConfigurationID);

            -- Register Marked only, not DataLoader
            IF LATempID <> '0' THEN
                DeleteMultiCompoundRegTmp(LATempID);
            END IF;

            -- WJC This area may need to be rethought
            -- We have LRegNum but not RegID
            -- Need to map LAction into IsDuplicate
            -- Don't have BatchID
            LogBULKREGISTRATION(LogID, LATempID,  LAction, LRegNum, BatchID, 'Succeed');

        EXCEPTION
            WHEN OTHERS THEN
                ROLLBACK TO one_record;
                LogBULKREGISTRATION(LogID, LATempID, LAction, LRegNum, BatchID, SUBSTR (DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE, 1, 500));
        END;
        COMMIT;
    END LOOP;
EXCEPTION
    WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('LoadMultiCompoundRegRecordList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;

PROCEDURE RetrieveTempIDList(Ahitlistid IN Number, Aid OUT NOCOPY CLOB) IS
    Cursor temp_get_hitlistid(m_Id Number) Is
      Select Id
        From COEDB.COESAVEDHITLIST
       Where HITLISTID = m_Id;

    LTempIdList CLOB := '';

BEGIN
    For Lrow in temp_get_hitlistid(Ahitlistid)
    Loop
      LTempIdList := LTempIdList || Lrow.Id || ',';
    END LOOP;
    Aid := Rtrim(LTempIdList, ',');
EXCEPTION
    WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList, AppendError('RetrieveTempIDList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;

PROCEDURE LogBulkRegistrationId(
    ALogID OUT NUMBER
    , ADuplicateAction IN Varchar2
    , AUserID IN Varchar2
    , ADescription IN Varchar2 DEFAULT NULL
) IS
    LALOG_ID NUMBER;
BEGIN
    INSERT INTO LOG_BULKREGISTRATION_ID
      (DUPLICATE_ACTION, DESCRIPTION, USER_ID, DATETIME_STAMP)
    VALUES
      (ADuplicateAction, ADescription, AUserID, sysdate)
    returning LOG_ID INTO LALOG_ID;

    ALogID := LALOG_ID;
EXCEPTION
    WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('LogBulkRegistrationId', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
END;


PROCEDURE LogBulkRegistration(
    ALogID IN NUMBER
    , LATempID IN VARCHAR2
    , AAction IN char
    , RegNumber IN VARCHAR2
    , BatchID IN NUMBER
    , Result IN VARCHAR2
) IS
BEGIN

    INSERT INTO LOG_BULKREGISTRATION
            (log_id, temp_id, action, reg_number, batch_number,
             comments
            )
     VALUES (ALogID, LATempID, AAction, RegNumber, BatchID,
             Result
            );
END;

-- Gets a property list from the object definition in COEOBJECTCONFIG
-- APath is the path between MultiCompoundRegistryRecord and PropertyList eg. 'BatchList/Batch'
-- ARemoveTrailingComma will remove the trailing comma if TRUE
-- ATerm controls how each term is expanded
FUNCTION GetPropertyList(APath IN CLOB, ARemoveTrailingComma IN BOOLEAN := FALSE, ATerm IN CLOB := '<xsl:value-of select="@name"/>,') RETURN CLOB IS
  LReturn CLOB;
  LXml    CLOB;
  LXsl    CLOB := '
    <xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
      <xsl:template match="/MultiCompoundRegistryRecord">
       <xsl:for-each select="@PATH/PropertyList/Property">@TERM</xsl:for-each>
      </xsl:template>
    </xsl:stylesheet>
    ';
BEGIN
    -- Prepare XSL
    LXsl := Replace(LXsl, '@PATH', APath); -- eg. BatchList/Batch
    LXsl := Replace(LXsl, '@TERM', ATerm); -- eg. DECODE(<xsl:value-of select="@name"/>, NULL, '' '', <xsl:value-of select="@name"/>) <xsl:value-of select="@name"/>,
    -- Get XML
    SELECT XmlType.CreateXml(XML).GetClobVal()
      INTO LXml
      FROM COEOBJECTCONFIG
      WHERE ID=2;
    -- Transform
    SELECT XmlTransform(XmlType.CreateXml(LXml), XmlType.CreateXml(LXsl)).GetClobVal()
      INTO LReturn
      FROM DUAL;
    -- Remove trailing comma is requested
    IF ARemoveTrailingComma AND Length(LReturn) > 0 THEN
        LReturn := Rtrim(LReturn, ',');
    END IF;
    --
    RETURN LReturn;
END;

PROCEDURE RetrieveBatchCommon( AID IN NUMBER, AXml OUT NOCOPY CLOB, AIsTemp IN BOOLEAN) IS
    -- vary based on AIsTemp
    LDebugProcedure               CLOB;
    LBatchID                      CLOB;
    LCompoundID                   CLOB;
    LID                           CLOB;
    LMixtureComponentID           CLOB;
    LPersonRegistered             CLOB;
    LViewBatch                    CLOB;
    LViewCompound                 CLOB;
    -- result XML
    LXmlBatchComponentList        CLOB;
    LXmlBatch                     CLOB;
    -- SELECT phrases
    LBatchProperties              CLOB;
    LBatchComponentProperties     CLOB;
    -- for queries
    LQryCtx                       DBMS_XMLGEN.ctxHandle;
    LSelect                       CLOB;
    -- for XML Transforms
    LXslBatch                     CLOB := '
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/BATCH">
    <Batch>
      <BatchID>
        <xsl:for-each select="BATCHID">
          <xsl:value-of select="."/>
        </xsl:for-each>
      </BatchID>
      <BatchNumber>
        <xsl:for-each select="BATCHNUMBER">
          <xsl:value-of select="."/>
        </xsl:for-each>
      </BatchNumber>
      <DateCreated>
        <xsl:for-each select="DATECREATED">
          <xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
          <xsl:value-of select="."/>
        </xsl:for-each>
      </DateCreated>
      <xsl:variable name="VBATCH" select="."/>
      <PersonCreated>
        <xsl:for-each select="PERSONCREATED">
          <xsl:for-each select="$VBATCH/PERSONCREATEDDISPLAY">
            <xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
          </xsl:for-each>
          <xsl:value-of select="."/>
        </xsl:for-each>
      </PersonCreated>
      <PersonRegistered>
        <xsl:for-each select="PERSONREGISTERED">
          <xsl:for-each select="$VBATCH/PERSONREGISTEREDDISPLAY">
            <xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
          </xsl:for-each>
          <xsl:value-of select="."/>
        </xsl:for-each>
      </PersonRegistered>
      <DateLastModified>
        <xsl:for-each select="DATELASTMODIFIED">
          <xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
          <xsl:value-of select="."/>
        </xsl:for-each>
      </DateLastModified>
      <xsl:for-each select="PROPERTYLIST">
        <PropertyList>
          <xsl:for-each select="node()">
            LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/>
            <xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;
          </xsl:for-each>
        </PropertyList>
      </xsl:for-each>
    </Batch>
  </xsl:template>
</xsl:stylesheet>
';
    LXslBatchComponentList        CLOB := '
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/BATCHCOMPONENTLIST">
    <BatchComponentList>
      <xsl:for-each select="BATCHCOMPONENT">
        <BatchComponent>
          <ID>
            <xsl:for-each select="ID">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </ID>
          <BatchID>
            <xsl:for-each select="BATCHID">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </BatchID>
          <MixtureComponentID>
            <xsl:for-each select="MIXTURECOMPONENTID">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </MixtureComponentID>
          <CompoundID>
            <xsl:for-each select="COMPOUNDID">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </CompoundID>
          <ComponentIndex>
            <xsl:for-each select="COMPONENTINDEX">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </ComponentIndex>
          <xsl:for-each select="PROPERTYLIST">
            <PropertyList>
              <xsl:for-each select="node()">
                LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/>
                <xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;
              </xsl:for-each>
            </PropertyList>
          </xsl:for-each>
        </BatchComponent>
      </xsl:for-each>
    </BatchComponentList>
  </xsl:template>
</xsl:stylesheet>
';
BEGIN
    -- Set variables based on temp versus perm
    IF AIsTemp THEN LDebugProcedure := 'RetrieveBatchCommon(temp)'; ELSE LDebugProcedure := 'RetrieveBatchCommon(perm)'; END IF;
    IF AIsTemp THEN LBatchID := 'TEMPBATCHID'; ELSE LBatchID := 'BATCHID'; END IF;
    IF AIsTemp THEN LCompoundID := 'TEMPCOMPOUNDID'; ELSE LCompoundID := '0'; END IF;
    IF AIsTemp THEN LID := '0'; ELSE LID := 'ID'; END IF;
    IF AIsTemp THEN LPersonRegistered := ''''''; ELSE LPersonRegistered := 'PERSONREGISTERED'; END IF;
    IF AIsTemp THEN LMixtureComponentID := '0'; ELSE LMixtureComponentID := 'MIXTURECOMPONENTID'; END IF;
    IF AIsTemp THEN LViewBatch := 'VW_TEMPORARYBATCH'; ELSE LViewBatch := 'VW_BATCH'; END IF;
    IF AIsTemp THEN LViewCompound := 'VW_TEMPORARYCOMPOUND'; ELSE LViewCompound := 'VW_BATCHCOMPONENT'; END IF;

    --
    -- Start Prepare SELECT phrases for PropertyLists
    --
    LBatchProperties := GetPropertyList('BatchList/Batch', FALSE, 'DECODE(<xsl:value-of select="@name"/>, NULL, '' '', <xsl:value-of select="@name"/>) <xsl:value-of select="@name"/>,');
    LBatchProperties := Replace(LBatchProperties, '&apos;', '''');
    LBatchProperties := Replace(LBatchProperties, Chr(10), '');
    LBatchComponentProperties := GetPropertyList('BatchList/Batch/BatchComponentList/BatchComponent', FALSE, 'DECODE(<xsl:value-of select="@name"/>, NULL, '' '', <xsl:value-of select="@name"/>) <xsl:value-of select="@name"/>,');
    LBatchComponentProperties := Replace(LBatchComponentProperties, '&apos;', '''');
    LBatchComponentProperties := Replace(LBatchComponentProperties, Chr(10), '');
    --
    -- End Prepare SELECT phrases for PropertyLists
    --

    --
    -- Start Batch (without BatchComponentList)
    --

    -- Fetch XML
    LSelect :=
      'SELECT ' ||
      LBatchID || ' AS BATCHID, ' ||
      'BATCHNUMBER, ' ||
      'DATECREATED, ' ||
      'PERSONCREATED, ' ||
      LPersonRegistered || ' AS PERSONREGISTERED, ' ||
      'DATELASTMODIFIED, ' ||
      '''BatchPropertyListBegin'' Aux,' || LBatchProperties || '''BatchPropertyListEnd'' Aux ' ||
      'FROM ' || LViewBatch || ' ' ||
      'WHERE ' || LBatchID || '=' || AID
      ;
    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LSelect(' || LViewBatch || ')=' || LSelect); $end null;

    LQryCtx := DBMS_XMLGEN.newContext(LSelect);

    LXmlBatch := Replace(DBMS_XMLGEN.getXML(LQryCtx), cXmlDecoration, '');
    DBMS_XMLGEN.closeContext(LQryCtx);

    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LXmlBatch=' || LXmlBatch); $end null;

    IF LXmlBatch IS NULL THEN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('No rows returned.'));
    END IF;

    -- Manipulate XML
    LXmlBatch := Replace(LXmlBatch, '<ROWSET>', '');
    LXmlBatch := Replace(LXmlBatch, '<ROW>', '<BATCH>');
    LXmlBatch := Replace(LXmlBatch, '<PROJECTXML>', '');
    LXmlBatch := Replace(LXmlBatch, '</PROJECTXML>', '');
    LXmlBatch := Replace(LXmlBatch, '<AUX>BatchPropertyListBegin</AUX>', '<PROPERTYLIST>');
    LXmlBatch := Replace(LXmlBatch, '> <', '><');
    LXmlBatch := Replace(LXmlBatch, '<AUX>BatchPropertyListEnd</AUX>', '</PROPERTYLIST>');
    LXmlBatch := Replace(LXmlBatch, '</ROW>', '</BATCH>');
    LXmlBatch := Replace(LXmlBatch, '</ROWSET>', '');
    LXmlBatch := Trim(Chr(10) from LXmlBatch);

    -- Replace entities
    -- WJC PROJECTXML has entities but probably shouldn't
    LXmlBatch := Replace(LXmlBatch, '&quot;', '"');
    LXmlBatch := Replace(Replace(LXmlBatch, 'LESS_THAN_SIGN;', '<'), '&lt;', '<');
    LXmlBatch := Replace(Replace(LXmlBatch, 'GREATER_THAN_SIGN;', '>'), '&gt;', '>');

    -- Transform
    SELECT XmlTransform(XmlType.CreateXml(LXmlBatch), XmlType.CreateXml(LXslBatch)).GetClobVal() INTO LXmlBatch FROM DUAL;

    -- Replace entities
    -- WJC we're really only expecting LESS_THAN_SIGN and GREATER_THAN_SIGN at this point
    LXmlBatch := Replace(LXmlBatch, '&quot;', '"');
    LXmlBatch := Replace(Replace(LXmlBatch, 'LESS_THAN_SIGN;', '<'), '&lt;', '<');
    LXmlBatch := Replace(Replace(LXmlBatch, 'GREATER_THAN_SIGN;', '>'), '&gt;', '>');

    --
    -- End Batch (without BatchComponentList)
    --

    --
    -- Start BatchComponentList
    --

    -- Fetch XML
    LSelect :=
      'SELECT ' ||
      LID || ' AS ID, ' ||
      LBatchID || ' AS BATCHID, ' ||
      LMixtureComponentID || ' AS MixtureComponentID, ' ||
      LCompoundID || ' AS COMPOUNDID, ' ||
      '-' || LCompoundID || ' AS COMPONENTINDEX, ' ||
      '''PropertyListBegin'' Aux,' || LBatchComponentProperties || '''PropertyListEnd'' Aux ' ||
      'FROM ' || LViewCompound || ' ' ||
      'WHERE ' || LBatchID || '=' || AID
      ;

    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LSelect(' || LViewCompound || ')=' || LSelect); $end null;

    LQryCtx := DBMS_XMLGEN.newContext(LSelect);

    LXmlBatchComponentList := Replace(DBMS_XMLGEN.getXML(LQryCtx), cXmlDecoration, '');
    DBMS_XMLGEN.closeContext(LQryCtx);

    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LXmlBatchComponentList=' || LXmlBatchComponentList); $end null;

    IF LXmlBatchComponentList IS NULL THEN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('No rows returned.'));
    END IF;

    -- Manipulate XML
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<ROWSET>', '<BATCHCOMPONENTLIST>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<ROW>', '<BATCHCOMPONENT>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<AUX>PropertyListBegin</AUX>', '<PROPERTYLIST>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '> <', '><');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<AUX>PropertyListEnd</AUX>', '</PROPERTYLIST>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<BATCHCOMPFRAGMENTXML>', '');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '</BATCHCOMPFRAGMENTXML>', '');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '</ROW>', '</BATCHCOMPONENT>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '</ROWSET>', '</BATCHCOMPONENTLIST>');

    -- Replace entities
    -- WJC BATCHCOMPFRAGMENTXML has entities but probably shouldn't
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '&quot;', '"');
    LXmlBatchComponentList := Replace(Replace(LXmlBatchComponentList, 'LESS_THAN_SIGN;', '<'), '&lt;', '<');
    LXmlBatchComponentList := Replace(Replace(LXmlBatchComponentList, 'GREATER_THAN_SIGN;', '>'), '&gt;', '>');

    -- Transform
    SELECT XmlTransform(XmlType.CreateXml(LXmlBatchComponentList), XmlType.CreateXml(LXslBatchComponentList)).GetClobVal() INTO LXmlBatchComponentList FROM DUAL;

    -- Replace entities
    -- WJC we're really only expecting LESS_THAN_SIGN and GREATER_THAN_SIGN at this point
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '&quot;', '"');
    LXmlBatchComponentList := Replace(Replace(LXmlBatchComponentList, 'LESS_THAN_SIGN;', '<'), '&lt;', '<');
    LXmlBatchComponentList := Replace(Replace(LXmlBatchComponentList, 'GREATER_THAN_SIGN;', '>'), '&gt;', '>');

    --
    -- End BatchComponentList
    --

    -- Final assembly of complete Batch
    AXml := Replace(LXmlBatch, '</Batch>', LXmlBatchComponentList || '</Batch>');

    -- Quick cleanup
    -- WJC debugging only ?
    AXml := Replace(AXml, Chr(10), '');
    AXml := Replace(AXml, '>                ', '>');
    AXml := Replace(AXml, '>        ', '>');
    AXml := Replace(AXml, '>    ', '>');
    AXml := Replace(AXml, '>  ', '>');
    AXml := Replace(AXml, '> ', '>');
    AXml := Replace(AXml, '><', '>' || Chr(10) || '<');
    AXml := Replace(AXml, '>' || Chr(10) || '</', '></');

    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,'Batch:'|| chr(10)||AXml); $end null;

    RETURN;
END;

PROCEDURE RetrieveBatch( AID IN NUMBER, AXml OUT NOCOPY CLOB) IS
BEGIN
    RetrieveBatchCommon(AID, AXml, FALSE);
    RETURN;
END;

PROCEDURE RetrieveBatchTmp( ATempID IN NUMBER, AXml OUT NOCOPY CLOB) IS
BEGIN
    RetrieveBatchCommon(ATempID, AXml, TRUE);
    RETURN;
END;

PROCEDURE UpdateBatchCommon( AXml IN CLOB, AIsTemp IN BOOLEAN) IS
    -- vary based on AIsTemp
    LDebugProcedure               CLOB;
    LBatchID                      CLOB;
    LViewBatch                    CLOB;
    -- for XML Transforms
    LXslBatch                     CLOB := '
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
 <xsl:template match="/">
  UPDATE {ViewBatch}
  SET
  <xsl:for-each select="Batch/PropertyList/Property[@update=''yes'']"><xsl:value-of select="@name"/>=''<xsl:value-of select="."/>'',</xsl:for-each>{WHERE}
  {BatchID}=<xsl:value-of select="Batch/BatchID"/>
 </xsl:template>
</xsl:stylesheet>
';
    -- for queries
    LUpdate                       VARCHAR2(4000);
BEGIN
    -- Ensure proper date format for auto-conversion from string
    BEGIN
        SetSessionParameter();
    END;
    -- Set variables based on temp versus perm
    IF AIsTemp THEN LDebugProcedure := 'UpdateBatchCommon(temp)'; ELSE LDebugProcedure := 'UpdateBatchCommon(perm)'; END IF;
    IF AIsTemp THEN LBatchID := 'TEMPBATCHID'; ELSE LBatchID := 'BATCHID'; END IF;
    IF AIsTemp THEN LViewBatch := 'VW_TEMPORARYBATCH'; ELSE LViewBatch := 'VW_BATCH'; END IF;

    -- Set XSL based on temp versus perm
    LXslBatch := Replace(LXslBatch, '{BatchID}', LBatchID);
    LXslBatch := Replace(LXslBatch, '{ViewBatch}', LViewBatch);

    -- Transform XML into SQL
    SELECT XmlTransform(XmlType.CreateXml(AXml), XmlType.CreateXml(LXslBatch)).GetClobVal() INTO LUpdate FROM DUAL;

    -- Fix apostrophes
    LUpdate := Replace(LUpdate, '&apos;', '''');
    -- Remove trailing comma in the SET clause
    LUpdate := Replace(LUpdate, ',{WHERE}', ' WHERE');

    EXECUTE IMMEDIATE LUpdate;

    RETURN;
END;

PROCEDURE UpdateBatch( AXml IN CLOB) IS
BEGIN
    UpdateBatchCommon(AXml, FALSE);
    RETURN;
END;

PROCEDURE UpdateBatchTmp( AXml IN CLOB) IS
BEGIN
    UpdateBatchCommon(AXml, TRUE);
    RETURN;
END;

END;
/
