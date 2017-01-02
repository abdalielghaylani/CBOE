--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--#####################################
-- Updating Registration Configuration
--#####################################
DECLARE
	v_found number(1) := 0;
BEGIN
	SELECT COUNT(1) INTO v_found FROM &&securitySchemaName..COECONFIGURATION WHERE lower(DESCRIPTION) = 'registration';
	
	if v_found > 0 then
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','TableNameList',NULL, 'This determines the tables from the Table Editor to be exported by the Export tool.',NULL,NULL,NULL);	
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','SameBatchesIdentity',NULL, 'This setting allows different options to appear during the duplicate resolution workflow and determines whether or not batches can be added to existing records or if unique records are created instead.',NULL,NULL,NULL);	
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','CheckDuplication','True', 'This setting enables validation of duplication of structures and mixtures with identical compound before of the registration.', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableMixtures','False', 'Enables managing more than one compound in a registry record.', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableSubmissionTemplates','True', 'Enables the ability to save and load submission data prior to actual submission or registration.', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableFragments','True', 'Enable the use of fragments to track salts and solvates', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableMoveBatch','False', 'Allows reasigning batches between registry records', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableAddBatchButton','True', 'Controls the display of Add Batch button during component duplicate resolution', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableDuplicateButton','True', 'Controls the display of Duplicate button during component duplicate resolution', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableUseComponentButton','True', 'Controls the display of Duplicate button during component duplicate resolution', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableUseStructureButton','True', 'Controls the display of Use Structure button during component duplicate resolution', 'PICKLIST','True|False', 'False');

		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','MISC','CustomPropertyStyles','Std25x40|Std50x40|Std50x80|Std75x40|Std100x40|Std100x80',NULL ,NULL ,NULL , NULL);
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','ActiveRLS',NULL ,'This setting enables Row Level Security which is an additional layer of security utilizing Oracle''s fine grain access control functionality.', 'PICKLIST','True|False', 'False'); --For 11.0.3 a new parameter with the following value should be passed in: CambridgeSoft.COE.RegistrationAdmin.Services.RLSConfigurationProcessor

		UPDATE &&securitySchemaName..coeconfiguration c
			SET c.configurationxml = (
				SELECT  XmlType(replace( cr.configurationxml.GetClobVal(), '11.0.1', '11.0.2' ))
			FROM &&securitySchemaName..coeconfiguration cr
				WHERE cr.Description = c.Description )
			WHERE c.Description = 'Registration';

		COMMIT;
	end if;
END;
/

--####################################
-- Page Control Settings required XMLs
--####################################

DELETE FROM &&securitySchemaName..COEPAGECONTROL WHERE APPLICATION IN ('REGISTRATION');

DECLARE
 L_MasterXml_1 CLOB:= '<?xml version="1.0" encoding="utf-8"?>
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
		</Page>
		<Page>
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
		</Page>
		<Page>
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
		</Page>
		<Page>
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
		</Page>';

 L_MasterXml_2 CLOB:= '<Page>
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
		</Page>
		<Page>
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
		</Page>
		<Page>
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
		</Page>
		<Page>
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
		</Page>
		<Page>
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
		<AppSetting>
			<ID>SearchEngineURL</ID>
			<Key>Registration/CBV/SearchEngineURL</Key>
			<Value>/ChemBioViz/Forms/Search/ContentArea/ChemBioVizSearch.aspx</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>DataLoaderFormGroupId</ID>
			<Key>Registration/CBV/ReviewRegisterRegistryFormGroupId</Key>
			<Value>4011</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>SessionUserID</ID>
			<Key>Session User ID</Key>
			<Value>15</Value>
			<Type>Session</Type>
		</AppSetting>
		<AppSetting>
			<ID>MultipleVariable</ID>
			<Key>MultipleVariable</Key>
			<Value>MultipleVariableValue</Value>
			<Type>All</Type>
		</AppSetting>
		<AppSetting>
			<ID>ApprovalsEnabled</ID>
			<Key>Registration/REGADMIN/ApprovalsEnabled</Key>
			<Value>False</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>IsRegistryApproved</ID>
			<Key>IsRegistryApproved</Key>
			<Value>False</Value>
			<Type>Session</Type>
		</AppSetting>
		<AppSetting>
			<ID>ActiveRLS</ID>
			<Key>Registration/REGADMIN/ActiveRLS</Key>
			<Value>True</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>EnableSubmissionTemplates</ID>
			<Key>Registration/REGADMIN/EnableSubmissionTemplates</Key>
			<Value>True</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>EnableMixtures</ID>
			<Key>Registration/REGADMIN/EnableMixtures</Key>
			<Value>True</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>EnableFragments</ID>
			<Key>Registration/REGADMIN/EnableFragments</Key>
			<Value>True</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>EnableAddBatchButton</ID>
			<Key>Registration/REGADMIN/EnableAddBatchButton</Key>
			<Value>True</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>EnableDuplicateButton</ID>
			<Key>Registration/REGADMIN/EnableDuplicateButton</Key>
			<Value>True</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>EnableUseComponentButton</ID>
			<Key>Registration/REGADMIN/EnableUseComponentButton</Key>
			<Value>True</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
		<AppSetting>
			<ID>EnableUseStructureButton</ID>
			<Key>Registration/REGADMIN/EnableUseStructureButton</Key>
			<Value>True</Value>
			<Type>COEConfiguration</Type>
		</AppSetting>
	</AppSettings>
</COEPageControlSettings>';

 L_CustomXml_1 CLOB:= '<?xml version="1.0" encoding="utf-8"?>
<COEPageControlSettings>
	<Type>Custom</Type>
	<Application>REGISTRATION</Application>
	<Pages>
		<Page>
			<ID>ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>Manage Components</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID/>
						<Privilege>
							<ID>ADD_COMPONENT</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>DefineMixtureImageMenuButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableMixtures</ID>
							<Key>Registration/REGADMIN/EnableMixtures</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>Submission Templates</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID/>
						<Privilege>
							<ID>LOAD_SAVE_RECORD</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>SubmissionTemplateImageMenuButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableSubmissionTemplates</ID>
							<Key>
              Registration/REGADMIN/EnableSubmissionTemplates</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>SUBMITRECORD</ID>
					<Privileges>
						<Operator>OR</Operator>
						<Privilege>
							<ID>ADD_COMPOUND_TEMP</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID>
					</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>REGISTERRECORD</ID>
					<Privileges>
						<Operator>OR</Operator>
						<Privilege>
							<ID>REGISTER_DIRECT</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>RegisterButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID>
					</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>ComponentsFragmentsGrid</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>5</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableFragments</ID>
							<Key>Registration/REGADMIN/EnableFragments</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>Manage Components</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID/>
						<Privilege>
							<ID>ADD_COMPONENT</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>DefineMixtureImageMenuButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableMixtures</ID>
							<Key>Registration/REGADMIN/EnableMixtures</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>EditCompoundTemp</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>EditCompoundTemp</ID>
						<Privilege>
							<ID>EDIT_COMPOUND_TEMP</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>EditButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>RegisterFromTemp</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegisterFromTemp</ID>
						<Privilege>
							<ID>REGISTER_TEMP</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>RegisterButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>ComponentsFragmentsGrid</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>5</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableFragments</ID>
							<Key>Registration/REGADMIN/EnableFragments</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>BatchComponentsFragmentsGrid</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>6</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableFragments</ID>
							<Key>Registration/REGADMIN/EnableFragments</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegisterMarkedPageAccess</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegisterMarkedPageAccess</ID>
						<Privilege>
							<ID>REGISTER_TEMP</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>ComponentsFragmentsGrid</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>1</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>duplicateRepeaterFormHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableFragments</ID>
							<Key>Registration/REGADMIN/EnableFragments</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>AddBatchButton</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>AddBatchButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableAddBatchButton</ID>
							<Key>Registration/REGADMIN/EnableAddBatchButton</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>DuplicateRegistryButton</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>DuplicateRegistryButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableDuplicateButton</ID>
							<Key>Registration/REGADMIN/EnableDuplicateButton</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>CreateCompoundFormButton</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>CreateCompoundFormButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseComponentButton</ID>
							<Key>Registration/REGADMIN/EnableUseComponentButton</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>UseStructureButton</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>UseStructureButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseStructureButton</ID>
							<Key>Registration/REGADMIN/EnableUseStructureButton</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
			</ControlSettings>
		</Page>';	

 L_CustomXml_2 CLOB:= '<Page>
			<ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>Manage Components</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID/>
						<Privilege>
							<ID>ADD_COMPONENT</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>DefineMixtureImageMenuButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableMixtures</ID>
							<Key>Registration/REGADMIN/EnableMixtures</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>EditRegisteredRecord</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>EditRegisteredRecord</ID>
						<Privilege>
							<ID>EDIT_COMPOUND_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>EditButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>DeleteRegisteredRecord</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>DeleteRegisteredRecord</ID>
						<Privilege>
							<ID>DELETE_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>DeleteRegButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>DeleteMoveBatchFromPerm</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>DeleteMoveBatchFromPerm</ID>
						<Privilege>
							<ID>DELETE_BATCH_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>DeleteBatchButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
						<Control>
							<ID>MoveBatchButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>MoveBatchInPerm</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID>MoveBatchInPerm</ID>
					</Privileges>
					<Controls>
						<Control>
							<ID>MoveBatchButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableMoveBatch</ID>
							<Key>Registration/REGADMIN/EnableMoveBatch</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>AddComponentPerm</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>AddComponentPerm</ID>
						<Privilege>
							<ID>ADD_COMPONENT</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>AddComponentButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>AddBatchPerm</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>AddBatchPerm</ID>
						<Privilege>
							<ID>ADD_BATCH_PERM</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>AddBatchButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>SendToInventory</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>SendToInventory</ID>
						<Privilege>
							<ID>INV_CREATE_CONTAINER</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>SendToInventoryImageMenuButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>EditApprovedRecord</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID/>
						<Privilege>
							<ID>TOGGLE_APPROVED_FLAG</ID>
						</Privilege>
						<Privilege>
							<ID>EDIT_COMPOUND_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>EditButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
						<Control>
							<ID>0</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
						<Control>
							<ID>1000</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
						<Control>
							<ID>1</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
						<Control>
							<ID>1001</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>ApprovalsEnabled</ID>
							<Key>Registration/REGADMIN/ApprovalsEnabled</Key>
							<Value>False</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
						<AppSetting>
							<ID>IsRegistryApproved</ID>
							<Key>IsRegistryApproved</Key>
							<Value>False</Value>
							<Type>Session</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>ApprovedEnabled</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID/>
						<Privilege>
							<ID>SET_APPROVED_FLAG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>APPROVEDProperty</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>0</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>AND</Operator>
						<ID/>
						<AppSetting>
							<ID>ApprovalsEnabled</ID>
							<Key>Registration/REGADMIN/ApprovalsEnabled</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>ComponentsFragmentsGrid</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>5</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableFragments</ID>
							<Key>Registration/REGADMIN/EnableFragments</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
				<ControlSetting>
					<ID>ComponentsFragmentsGridEdit</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>6</ID>
							<TypeOfControl>COEForm</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableFragments</ID>
							<Key>Registration/REGADMIN/EnableFragments</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
			</ControlSettings>
		</Page>';
		
L_CustomXml_3 CLOB:= '<Page>
			<ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_MOVEDELETE_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>MoveDeletePageAccess</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>MoveDeletePageAccess</ID>
						<Privilege>
							<ID>DELETE_BATCH_REG</ID>
						</Privilege>
						<Privilege>
							<ID>DELETE_TEMP</ID>
						</Privilege>
						<Privilege>
							<ID>DELETE_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_MOVEDELETE_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURESEARCH_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>SearchRegistry</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>SearchRegistry</ID>
						<Privilege>
							<ID>SEARCH_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURESEARCH_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_DEFAULT_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_Default</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_Default</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_DEFAULT_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>TableEditorPage</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>TableEditorPage</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
				<ControlSetting>
					<ID>VW_PROJECT_ChildTable</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID/>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>VW_PROJECT_ChildTable</ID>
							<TypeOfControl>
              COETableManagerChildTable</TypeOfControl>
							<PlaceHolderID>COETableManager1</PlaceHolderID>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>ApprovalsEnabled</ID>
							<Key>Registration/REGADMIN/ActiveRLS</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ROOTPROPERTYLIST_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_RootPropertyList</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_RootPropertyList</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ROOTPROPERTYLIST_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_COMPOUND_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_Compound</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_Compound</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_COMPOUND_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_BatchPage</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_BatchPage</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PARAMEDIT_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_ParamEdit</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_ParamEdit</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PARAMEDIT_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_VALIDATIONRULES_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_ValRules</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_ValRules</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_VALIDATIONRULES_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINS_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_AddInsPage</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_AddInsPage</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINS_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINSEDIT_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_AddInsEditPage</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_AddInsEditPage</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINSEDIT_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_IMPORTEXPORTCUSTOM_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_ImpExp</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_ImpExp</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_IMPORTEXPORTCUSTOM_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
			<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CONFIGSETTINGS_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>RegAdmin_ConfigSet</ID>
					<Privileges>
						<Operator>OR</Operator>
						<ID>RegAdmin_ConfigSet</ID>
						<Privilege>
							<ID>CONFIG_REG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CONFIGSETTINGS_ASPX</ID>
							<TypeOfControl>Page</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>
			</ControlSettings>
		</Page>
		<Page>
            <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>Test</ID>
                    <Privileges>
                        <Operator>AND</Operator>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ApprovedPermRegistries</ID>
                            <Description>Control description goes here!</Description>
                            <FriendlyName>Control friendly name</FriendlyName>
                            <PlaceHolderID/>
                            <TypeOfControl>Control</TypeOfControl>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                        <AppSetting>
                            <ID>ApprovalsEnabled</ID>
                            <Key>Registration/REGADMIN/ApprovalsEnabled</Key>
                            <Value>True</Value>
                            <Type>COEConfiguration</Type>
                        </AppSetting>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
	</Pages>
</COEPageControlSettings>';
 
 L_PrivilegesXml CLOB:= 'CHEM_REG_PRIVILEGES';

 L_COEPageControl_Seq NUmber(8);

BEGIN
	INSERT INTO &&securitySchemaName..COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('REGISTRATION','MASTER', L_MasterXml_1);
	SELECT &&securitySchemaName..COEPageControl_seq.CURRVAL INTO L_COEPageControl_Seq FROM DUAL;
	UPDATE &&securitySchemaName..COEPAGECONTROL SET CONFIGURATIONXML=CONFIGURATIONXML||L_MasterXml_2 WHERE ID=L_COEPageControl_Seq;

	INSERT INTO &&securitySchemaName..COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('REGISTRATION','CUSTOM', L_CustomXml_1);	
	SELECT &&securitySchemaName..COEPageControl_seq.CURRVAL INTO L_COEPageControl_Seq FROM DUAL;
	UPDATE &&securitySchemaName..COEPAGECONTROL SET CONFIGURATIONXML=CONFIGURATIONXML||L_CustomXml_2||L_CustomXml_3 WHERE ID=L_COEPageControl_Seq;
	
	INSERT INTO &&securitySchemaName..COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('REGISTRATION','PRIVILEGES', L_PrivilegesXml);
	COMMIT;    
END;
/










