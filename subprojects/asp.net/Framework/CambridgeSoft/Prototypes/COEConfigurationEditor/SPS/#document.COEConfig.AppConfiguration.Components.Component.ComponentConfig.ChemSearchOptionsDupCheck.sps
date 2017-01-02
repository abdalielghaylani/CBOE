<?xml version="1.0" encoding="UTF-8"?>
<structure version="4" encodinghtml="UTF-8" encodingrtf="ISO-8859-1" encodingpdf="UTF-8">
  <schemasources>
    <namespaces />
    <schemasources>
      <xsdschemasource name="$XML" main="1" schemafile="C:\CambridgeSoft\Web\ConfigurationEditor\Temporary.xsd" workingxmlfile="C:\CambridgeSoft\Web\ConfigurationEditor\Temporary.xml">
        <xmltablesupport />
        <textstateicons />
      </xsdschemasource>
    </schemasources>
  </schemasources>
  <parameters />
  <scripts>
    <javascript name="javascript" />
  </scripts>
  <globalstyles />
  <parts>
    <editorproperties />
    <properties />
    <styles />
    <children>
      <globaltemplate match="/">
        <editorproperties />
        <properties />
        <styles />
        <children>
          <template match="$XML">
            <editorproperties elementstodisplay="1" />
            <properties />
            <styles />
            <children>
              <paragraph paragraphtag="fieldset">
                <editorproperties />
                <properties />
                <styles border-bottom-style="none" border-left-style="none" border-right-style="none" border-top-style="none" />
                <children>
                  <template match="COEConfig">
                    <children>
                      <template match="AppConfiguration">
                        <children>
                          <template match="Components">
                            <children>
                              <template match="Component">
                                <children>
                                  <template match="ComponentConfig">
                                    <children>
                                      <template match="ChemSearchOptionsDupCheck">
                                        <children>
                                          <template match="@allowExtraneousFragments">
                                            <children>
                                              <condition>
                                                <editorproperties />
                                                <properties />
                                                <styles />
                                                <children>
                                                  <conditionbranch xpath="0=0">
                                                    <editorproperties />
                                                    <properties />
                                                    <styles />
                                                    <children>
                                                      <table>
                                                        <editorproperties />
                                                        <properties border="1" width="100%" />
                                                        <styles border-bottom-style="none" border-left-style="none" border-right-style="none" border-top-style="none" />
                                                        <children>
                                                          <tablebody>
                                                            <editorproperties />
                                                            <properties />
                                                            <styles />
                                                            <children>
                                                              <tablerow>
                                                                <editorproperties />
                                                                <properties />
                                                                <styles />
                                                                <children>
                                                                  <tablecell>
                                                                    <editorproperties />
                                                                    <properties height="28" width="112" />
                                                                    <styles />
                                                                    <children>
                                                                      <text fixtext="allowExtraneousFragments: ">
                                                                        <editorproperties />
                                                                        <properties />
                                                                        <styles font-weight="bold" />
                                                                        <children />
                                                                      </text>
                                                                    </children>
                                                                  </tablecell>
                                                                  <tablecell>
                                                                    <editorproperties />
                                                                    <properties height="28" width="294" />
                                                                    <styles />
                                                                    <children>
                                                                      <combobox enumeration="1">
                                                                        <editorproperties />
                                                                        <properties size="1" />
                                                                        <styles width="296px" />
                                                                        <children>
                                                                          <content>
                                                                            <editorproperties />
                                                                            <properties />
                                                                            <styles />
                                                                            <children />
                                                                            <addvalidations />
                                                                            <format />
                                                                          </content>
                                                                        </children>
                                                                        <addvalidations />
                                                                      </combobox>
                                                                    </children>
                                                                  </tablecell>
                                                                </children>
                                                              </tablerow>
                                                            </children>
                                                          </tablebody>
                                                        </children>
                                                      </table>
                                                      <text fixtext=" ">
                                                        <editorproperties />
                                                        <properties />
                                                        <styles font-weight="bold" />
                                                        <children />
                                                      </text>
                                                    </children>
                                                  </conditionbranch>
                                                </children>
                                              </condition>
                                            </children>
                                          </template>
                                        </children>
                                      </template>
                                    </children>
                                  </template>
                                </children>
                              </template>
                            </children>
                          </template>
                        </children>
                      </template>
                    </children>
                  </template>
                </children>
              </paragraph>
            </children>
            <addvalidations />
            <sort />
          </template>
          <newline>
            <editorproperties />
            <properties />
            <styles />
            <children />
          </newline>
        </children>
      </globaltemplate>
    </children>
  </parts>
  <pagelayout>
    <editorproperties />
    <properties />
    <styles />
    <children />
  </pagelayout>
</structure>