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
                      <template match="FrameworkConfig">
                        <children>
                          <template match="Components">
                            <children>
                              <template match="Component">
                                <children>
                                  <template match="SelectClauseItem">
                                    <children>
                                      <template match="SelectClauseItem">
                                        <children>
                                          <template match="@class">
                                            <children>
                                              <condition>
                                                <editorproperties />
                                                <properties />
                                                <styles />
                                                <children>
                                                  <conditionbranch xpath="../@configEditorName=&quot;literal&quot; and ../@name=&quot;literal&quot; and ../../@configEditorName=&quot;Select Clause Items&quot; and ../../../@configEditorName=&quot;SqlGenerator&quot; and ../../../../@configEditorName=&quot;Components&quot; and ../../../../../@name=&quot;Global&quot;">
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
                                                                      <text fixtext="class: ">
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
                                                                      <editfield>
                                                                        <editorproperties />
                                                                        <properties size="40" />
                                                                        <styles font-weight="bold" />
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
                                                                      </editfield>
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
                  <template match="COEConfig">
                    <children>
                      <template match="FrameworkConfig">
                        <children>
                          <template match="Components">
                            <children>
                              <template match="Component">
                                <children>
                                  <template match="SelectClauseItem">
                                    <children>
                                      <template match="SelectClauseItem">
                                        <children>
                                          <template match="@value">
                                            <children>
                                              <condition>
                                                <editorproperties />
                                                <properties />
                                                <styles />
                                                <children>
                                                  <conditionbranch xpath="../@configEditorName=&quot;literal&quot; and ../@name=&quot;literal&quot; and ../../@configEditorName=&quot;Select Clause Items&quot; and ../../../@configEditorName=&quot;SqlGenerator&quot; and ../../../../@configEditorName=&quot;Components&quot; and ../../../../../@name=&quot;Global&quot;">
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
                                                                      <text fixtext="value: ">
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
                                                                      <editfield>
                                                                        <editorproperties />
                                                                        <properties size="40" />
                                                                        <styles font-weight="bold" />
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
                                                                      </editfield>
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