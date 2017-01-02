

--######################
-- Page Control Settings required XMLs
--######################


prompt 
prompt Starting "LoadCoepagecontrols_reg_master.sql"...
prompt Starting "Updating the coepagercontrol table(Registration Master)"...
prompt 

DECLARE



 L_MasterXml_1 CLOB:= '<?xml version="1.0" encoding="WINDOWS-1252"?>
<COEPageControlSettings type="Master">
  <Application>REGISTRATION</Application>
  <ID>COEPageControlSettings_Registration_Master</ID>
  <Pages>
    <Page>
      <ID>ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX</ID>
      <Description>Page to submit registries (temporary registries until registration)</Description>
      <FriendlyName>Submit New Record </FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX</ID>
          <Description>Page to submit registries (temporary registries until registration)</Description>
          <FriendlyName>Submit Registries</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
        <Control>
          <ID>SearchComponentButton</ID>
          <Description>Search components to add to the current registry</Description>
          <FriendlyName>Search Components button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>AddComponentButton</ID>
          <Description>Add components to the current registry</Description>
          <FriendlyName>Add Components button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>DoneAddingButton</ID>
          <Description>Finish adding components to the current registry</Description>
          <FriendlyName>Finish Adding Components button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>DefineMixtureImageMenuButton</ID>
          <Description>Manage Components</Description>
          <FriendlyName>Manage Components</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>SubmitButton</ID>
          <Description>Submit the current registry</Description>
          <FriendlyName>Submit button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>RegisterButton</ID>
          <Description>Permanently Register skipping Temp</Description>
          <FriendlyName>Register Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>SaveButton</ID>
          <Description>Save the current registry</Description>
          <FriendlyName>Save button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>1</ID>
          <Description>Details form Mixture</Description>
          <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>FormGeneratorDebuggingInfo</ID>
          <TypeOfControl>Control</TypeOfControl>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>FormGeneratorDebuggingInfo</ID>
          <TypeOfControl>Control</TypeOfControl>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <COEFormID>5</COEFormID>
        </Control>
        <Control>
          <ID>FormGeneratorDebuggingInfo</ID>
          <TypeOfControl>Control</TypeOfControl>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <COEFormID>1001</COEFormID>
        </Control>
        <Control>
          <ID>FormGeneratorDebuggingInfo</ID>
          <TypeOfControl>Control</TypeOfControl>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <COEFormID>1002</COEFormID>
        </Control>
        <Control>
          <ID>FormGroupDebuggingInfo</ID>
          <TypeOfControl>Control</TypeOfControl>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <COEFormID>-1</COEFormID>
        </Control>
        <Control>
          <ID>5</ID>
          <Description>Components Fragments Grid</Description>
          <FriendlyName>ComponentsFragmentsGrid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1001</ID>
          <Description>Compound Properties COEForm</Description>
          <FriendlyName>Compound Properties COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1002</ID>
          <Description>Batch Properties COEForm</Description>
          <FriendlyName>Batch Properties COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>4</ID>
          <Description>Batch COEForm</Description>
          <FriendlyName>Batch COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1003</ID>
          <Description>Batch Component COEForm</Description>
          <FriendlyName>Batch Component COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>VCompound_IdentifiersUltraGrid</ID>
          <Description>Identifier grid</Description>
          <FriendlyName>Identifier grid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>AMix_IdentifiersUltraGrid</ID>
          <Description>Identifiers grid - Add Mode</Description>
          <FriendlyName>Identifiers grid - Add Mode</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>EMix_ProjectsUltraGrid</ID>
          <Description>Projects grid - Edit Mode</Description>
          <FriendlyName>Projects grid - Edit Mode</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>VCompound_IdentifiersUltraGrid</ID>
          <Description>Identifiers grid</Description>
          <FriendlyName>Identifiers grid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>SequenceDropDownList</ID>
          <Description>Component Sequence</Description>
          <FriendlyName>Component Sequence</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>SequenceDropDownList</ID>
          <Description>Registry Sequence</Description>
          <FriendlyName>Registry Sequence</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>AMix_ProjectsUltraGrid</ID>
          <Description>Projects grid - Add</Description>
          <FriendlyName>Projects grid - Add</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>IdentifiersUltraGrid</ID>
          <Description>Identifiers grid</Description>
          <FriendlyName>Identifiers grid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
      </Controls>
    </Page>';


 L_MasterXml_2 CLOB:= '<Page>
      <ID>ASP.FORMS_SUBMITRECORD_SUBMITMIXTURE_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>SUBMITMIXTURE_ASPX</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_SUBMITRECORD_SUBMITMIXTURE_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_SUBMITRECORD_SEARCHCOMPOUND_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Submit Search Component</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_SUBMITRECORD_SEARCHCOMPOUND_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_SUBMITRECORD_LOADMIXTUREFORM_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Load Template</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_SUBMITRECORD_LOADMIXTUREFORM_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_SUBMITRECORD_SAVEMIXTUREFORM_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Save Template</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_SUBMITRECORD_SAVEMIXTUREFORM_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>';

 L_MasterXml_3 CLOB:= '<Page>
      <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX</ID>
      <Description>Page to submit registries (temporary registries until registration)</Description>
      <FriendlyName>Review Register</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX</ID>
          <Description>Page to submit registries (temporary registries until registration)</Description>
          <FriendlyName>Submit Registries</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
        <Control>
          <ID>SearchComponentButton</ID>
          <Description>Search components to add to the current registry</Description>
          <FriendlyName>Search Components button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>EditButton</ID>
          <Description>Edit record button allows you to go into Edit mode</Description>
          <FriendlyName>Edit Record Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>RegisterButton</ID>
          <Description>Register the temporary record to permanent registry</Description>
          <FriendlyName>Register Record Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>AddComponentButton</ID>
          <Description>Add components to the current registry</Description>
          <FriendlyName>Add Components button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>DoneAddingButton</ID>
          <Description>Finish adding components to the current registry</Description>
          <FriendlyName>Finish Adding Components button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>SubmitButton</ID>
          <Description>Submit the current registry</Description>
          <FriendlyName>Submit button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>SaveButton</ID>
          <Description>Save the current registry</Description>
          <FriendlyName>Save button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>1</ID>
          <Description>Details form Mixture</Description>
          <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>5</ID>
          <Description>Components Fragments Grid</Description>
          <FriendlyName>ComponentsFragmentsGrid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>6</ID>
          <Description>BatchComponents Fragments Grid</Description>
          <FriendlyName>BatchComponentsFragmentsGrid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1001</ID>
          <Description>Compound Properties COEForm</Description>
          <FriendlyName>Compound Properties COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1002</ID>
          <Description>Batch Properties COEForm</Description>
          <FriendlyName>Batch Properties COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>4</ID>
          <Description>Batch COEForm</Description>
          <FriendlyName>Batch COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1003</ID>
          <Description>Batch Component COEForm</Description>
          <FriendlyName>Batch Component COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>ACompoundIdentifiersUltraGrid</ID>
          <Description>Identifier grid</Description>
          <FriendlyName>Identifier grid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>EMixIdentifiersUltraGrid</ID>
          <Description>Identifiers grid</Description>
          <FriendlyName>Identifiers grid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>EMix_ProjectsUltraGrid</ID>
          <Description>Projects grid - Edit Mode</Description>
          <FriendlyName>Projects grid - Edit Mode</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>SequenceDropDownList</ID>
          <Description>Component Sequence</Description>
          <FriendlyName>Component Sequence</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>SequenceDropDownList</ID>
          <Description>Registry Sequence</Description>
          <FriendlyName>Registry Sequence</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERSEARCH_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Search Temporary Records</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERSEARCH_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
        <Control>
          <ID>FormGeneratorDebuggingInfo</ID>
          <TypeOfControl>Control</TypeOfControl>
          <PlaceHolderID>SearchTempFrame</PlaceHolderID>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>FormGeneratorDebuggingInfo</ID>
          <TypeOfControl>Control</TypeOfControl>
          <PlaceHolderID>SearchTempFrame</PlaceHolderID>
          <COEFormID>1</COEFormID>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_DELETEMARKED_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Delete Marked</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_DELETEMARKED_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>';

 L_MasterXml_4 CLOB:= '<Page>
      <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Register Marked</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRYDUPLICATES_CONTENTAREA_REGISTRYDUPLICATES_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Regisrty Duplicate Resolution</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRYDUPLICATES_CONTENTAREA_REGISTRYDUPLICATES_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
        <Control>
          <ID>6</ID>
          <Description>BatchComponents Fragments Grid</Description>
          <FriendlyName>BatchComponentsFragmentsGrid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Component Duplicate Resolution</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
        <Control>
          <ID>1</ID>
          <Description>Components Fragments Grid</Description>
          <FriendlyName>ComponentsFragmentsGrid</FriendlyName>
          <PlaceHolderID>duplicateRepeaterFormHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>AddBatchButton</ID>
          <Description>Add Batch button during component duplicate resolution</Description>
          <FriendlyName>Add Batch Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>DuplicateRegistryButton</ID>
          <Description> Duplicate  button during component duplicate resolution</Description>
          <FriendlyName>Duplicate Registry Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>CreateCompoundFormButton</ID>
          <Description>Use Component  button during component duplicate resolution</Description>
          <FriendlyName>Create Compound Form Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>UseStructureButton</ID>
          <Description>Use Structure button during component duplicate resolution</Description>
          <FriendlyName>Use Structure Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
      </Controls>
    </Page>';

 L_MasterXml_5 CLOB:= '<Page>
      <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>View Mixture</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
        <Control>
          <ID>EditButton</ID>
          <Description>Edit record button allows you to go into Edit mode</Description>
          <FriendlyName>Edit Record Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>DeleteRegButton</ID>
          <Description>Delete record button allows you to go into Delete mode</Description>
          <FriendlyName>Delete Record Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>DeleteBatchButton</ID>
          <Description>Delete batch button allows you to go into Delete  batch mode</Description>
          <FriendlyName>Delete Batch Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>SendToInventory</ID>
          <Description>Send Permanent Record to Inventory</Description>
          <FriendlyName>Send To Inventory Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>AddComponentButton</ID>
          <Description>Add components to the current registry</Description>
          <FriendlyName>Add Components button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>MoveBatchButton</ID>
          <Description>Move batch button allows you to go Move a batch to another registry record</Description>
          <FriendlyName>Move Batch Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>AddBatchButton</ID>
          <Description>Add batch button allows you to go add a batch to another registry record</Description>
          <FriendlyName>Add Batch Button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>0</ID>
          <Description>Details form Mixture</Description>
          <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1000</ID>
          <Description>Custom properties Details form Mixture</Description>
          <FriendlyName>This shows the custom properties of a registry (View Mode)</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1</ID>
          <Description>Details form Mixture</Description>
          <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>5</ID>
          <Description>Components Fragments Grid</Description>
          <FriendlyName>ComponentsFragmentsGrid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>6</ID>
          <Description>BatchComponents Fragments Grid</Description>
          <FriendlyName>BatchComponentsFragmentsGrid</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>8</ID>
          <Description>DocManager COEForm</Description>
          <FriendlyName>DocManager COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1001</ID>
          <Description>Compound Properties COEForm</Description>
          <FriendlyName>Compound Properties COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1002</ID>
          <Description>Batch Properties COEForm</Description>
          <FriendlyName>Batch Properties COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>4</ID>
          <Description>Batch COEForm</Description>
          <FriendlyName>Batch COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>1003</ID>
          <Description>Batch Component COEForm</Description>
          <FriendlyName>Batch Component COEForm</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEForm</TypeOfControl>
        </Control>
        <Control>
          <ID>APPROVEDProperty</ID>
          <Description>Approved or not flag</Description>
          <FriendlyName>Approved or not flag</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>EMix_ProjectsUltraGrid</ID>
          <Description>Registry Identifier grid - Edit</Description>
          <FriendlyName>RegistryIdentifier grid - Edit</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>ACompound_IdentifiersUltraGrid</ID>
          <Description>Compound Identifiers grid - Add</Description>
          <FriendlyName>Compound Identifiers grid - Add</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>ECompound_IdentifiersUltraGrid</ID>
          <Description>Compound Identifers grid - Edit</Description>
          <FriendlyName>Compound Identifiers grid - Edit</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>SequenceDropDownList</ID>
          <Description>Component Sequence</Description>
          <FriendlyName>Component Sequence</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>1</COEFormID>
        </Control>
        <Control>
          <ID>SequenceDropDownList</ID>
          <Description>Registry Sequence</Description>
          <FriendlyName>Registry Sequence</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
        <Control>
          <ID>EMix_ProjectsUltraGrid</ID>
          <Description>Registry Projects</Description>
          <FriendlyName>Registry Projects</FriendlyName>
          <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
          <TypeOfControl>COEGenerableControl</TypeOfControl>
          <COEFormID>0</COEFormID>
        </Control>
      </Controls>
    </Page>';


L_MasterXml_6 CLOB:='<Page>
      <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_MOVEDELETE_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Move Delete Batch</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_MOVEDELETE_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURESEARCH_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Search Permanent Records</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURESEARCH_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_DEFAULT_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Reg Admin</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_DEFAULT_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
      <Description>Page to submit registries (temporary registries until registration)</Description>
      <FriendlyName>Table Editor</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
          <Description>Page to submit registries (temporary registries until registration)</Description>
          <FriendlyName>Submit Registries</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
        <Control>
          <ID>AddTableButton</ID>
          <Description/>
          <FriendlyName>Add Table button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>AddRecordButton</ID>
          <Description>Add components to the current registry</Description>
          <FriendlyName>Add Record button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>EditRecordButton</ID>
          <Description>Finish adding components to the current registry</Description>
          <FriendlyName>Edit Record button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>DeleteRecordButton</ID>
          <Description>Submit the current registry</Description>
          <FriendlyName>Delete Record button</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
        <Control>
          <ID>VW_PROJECT_ChildTable</ID>
          <Description>Represent the child table of the project table</Description>
          <FriendlyName>ProjectChildTable</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>COETableManagerChildTable</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ROOTPROPERTYLIST_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Manage Registry Properties</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ROOTPROPERTYLIST_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_COMPOUND_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Manage Component Properties</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_COMPOUND_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Manage Batch Properties</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCHCOMPONENT_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Manage Batch Component Properties</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCHCOMPONENT_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PARAMEDIT_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Edit Parameter</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PARAMEDIT_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_VALIDATIONRULES_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Edit Validation Rule</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_VALIDATIONRULES_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>';


L_MasterXml_7 CLOB:='<Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINS_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Manage Addins</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINS_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINSEDIT_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Edit Addin</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINSEDIT_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_EDITFORMSXML_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Edit Form XML</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_EDITFORMSXML_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_IMPORTEXPORTCUSTOM_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Import Export Configuration</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_IMPORTEXPORTCUSTOM_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CONFIGSETTINGS_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Config Settings</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CONFIGSETTINGS_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_PAGECONTROLSETTING_CONTENTAREA_PAGECONTROLSETTING_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Page Control Setting</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_PAGECONTROLSETTING_CONTENTAREA_PAGECONTROLSETTING_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HELP_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Help Page</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HELP_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>';

L_MasterXml_8 CLOB:='<Page>
      <ID>ASP.FORMS_PUBLIC_CONTENTAREA_ABOUT_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>About Page</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_PUBLIC_CONTENTAREA_ABOUT_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>Home</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
        <Control>
          <ID>ApprovedPermRegistries</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Control</TypeOfControl>
        </Control>
      </Controls>
    </Page>
    <Page>
      <ID>ASP.FORMS_PUBLIC_CONTENTAREA_MESSAGES_ASPX</ID>
      <Description>Here goes the page description text</Description>
      <FriendlyName>MESSAGES_ASPX</FriendlyName>
      <Controls>
        <Control>
          <ID>ASP.FORMS_PUBLIC_CONTENTAREA.MESSAGES_ASPX</ID>
          <Description>Control description goes here!</Description>
          <FriendlyName>Control friendly name</FriendlyName>
          <PlaceHolderID/>
          <TypeOfControl>Page</TypeOfControl>
        </Control>
      </Controls>
    </Page>
  </Pages>
  <AppSettings>
    <Operator>AND</Operator>
    <ID>LockingEnabled</ID>
    <AppSetting>
      <ID>ApprovalsEnabled</ID>
      <Key>Registration/REGADMIN/LockingEnabled</Key>
      <Value>False</Value>
      <Type>COEConfiguration</Type>
    </AppSetting>
    <AppSetting>
      <ID>IsRegistryLocked</ID>
      <Key>IsRegistryLocked</Key>
      <Value>True</Value>
      <Type>Session</Type>
    </AppSetting>
  </AppSettings>
</COEPageControlSettings>';



BEGIN

  
    UPDATE COEDB.COEPAGECONTROL SET CONFIGURATIONXML=L_MasterXml_1||L_MasterXml_2||L_MasterXml_3||L_MasterXml_4||L_MasterXml_5||L_MasterXml_6||L_MasterXml_7||L_MasterXml_8 WHERE APPLICATION IN ('REGISTRATION') AND TYPE='MASTER';



END;
/
