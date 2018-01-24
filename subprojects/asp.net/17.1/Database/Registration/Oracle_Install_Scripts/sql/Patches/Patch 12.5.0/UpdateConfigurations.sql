--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--####################################
-- Page Control Settings required XMLs
--####################################

-- Updating Registration Configuration
DECLARE
	V_FOUND NUMBER(1) := 0;
BEGIN
	SELECT COUNT(1) INTO V_FOUND FROM &&securitySchemaName..COECONFIGURATION WHERE LOWER(DESCRIPTION)='registration';
	
	IF V_FOUND > 0 THEN
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','MISC','AppPageTitle','CambridgeSoft Registration Enterprise 12.5.0', 'Defines application page tile.', 'TEXT',NULL, 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','AllowUnregisteredComponentsInMixtures','False', 'Allows components to be added to Mixtures without registering components.', 'PICKLIST','True|False', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableUseComponentButton','False', 'Controls the display of Use Component  button during component duplicate resolution.', 'PICKLIST','True|False', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','AllowCreateCopySharedStructure','False', 'Allow to create copies of edited linked structures.', 'PICKLIST','True|False', 'False');

&&securitySchemaName..ConfigurationManager.DeleteParameter('Registration','MISC','BatchesToTemp');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableUseBatchPrefixes','False', 'Enables the use of batch level prefixes rather than registry level prefix', 'PICKLIST','True|False', 'True');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','LockingEnabled','False', 'This setting enables the Locking workflow. Records are associated with locking status and are managed by Administrators.', 'PICKLIST','True|False', 'True');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableSubmissionDuplicateChecking','False', 'Enables duplicate checking to occur when records are submitted and requires suggested action from submitter.', 'PICKLIST','True|False', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableAuditing','False', 'Allow logging registry records change history', 'PICKLIST','True|False', 'False','CambridgeSoft.COE.RegistrationAdmin.Services.AuditingConfigurationProcessor');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','BatchHighlightingField','', 'In combination with BachHighlightingValue configures a rule to highlight those batches that has a specific value for a given field', 'TEXT','', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','BatchHighlightingValue','', 'In combination with BachHighlightingField configures a rule to highlight those batches that has a specific value for a given field', 'TEXT','', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','BatchHighlightingTooltip','', 'If a batch is highlithed this tooltip would tell the user why it was highlighted', 'TEXT','', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','DefaultBatchPrefix','7', 'Default Batch Prefix ID used for non-UI temporary or permanent registration through bulkloading or webservice', 'TEXT','', 'True');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','InventoryIntegration','Disabled', 'Allows to disable or enable the whole integration with inventory system', 'PICKLIST','Enabled|Disabled', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','DOCMGR','DocMgrEnabled','False', 'Set enable/disable DocManager link. Allowed values [TRUE|FALSE]', 'PICKLIST','True|False', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','MISC','Batch_Identifiers','False', 'Enables/Disables Identifiers grid at Batch level.Allowed values [TRUE|FALSE]', 'PICKLIST','True|False', 'False');

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','EnableMoveBatch','False', 'Allows reasigning batches between registry records.Allowed values [TRUE|FALSE]', 'PICKLIST','True|False', 'False');
	END IF;
END;
/

UPDATE &&securitySchemaName..COEPAGECONTROL
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
          				<ID>ShowNumComponentsDropDown</ID>
          				<Privileges>
            					<Operator>AND</Operator>
          				</Privileges>
          			<Controls>
            				<Control>
              					<ID>ComponentNumberStateControl</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>0</COEFormID>
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
        	</ControlSetting>')).getClobVal()
		  where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';

-- Batch Identifiers:
UPDATE &&securitySchemaName..COEPAGECONTROL
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
          				<ID>Batch Identifiers Enabled</ID>
          				<Privileges>
            					<Operator>AND</Operator>
          				</Privileges>
          			<Controls>
            				<Control>
              					<ID>IDENTIFIERTYPETextBox</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>2</COEFormID>
            				</Control>
                                        <Control>
              					<ID>IdentifierValueTextBox</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>2</COEFormID>
            				</Control>
          			</Controls>
          			<AppSettings>
            				<Operator>OR</Operator>
            				<ID/>
             				<AppSetting>
              					<ID>Batch_Identifiers</ID>
              					<Key>Registration/MISC/Batch_Identifiers</Key>
              					<Value>True</Value>
              					<Type>COEConfiguration</Type>
            				</AppSetting>
          			</AppSettings>
        	</ControlSetting>')).getClobVal()
		  where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';

-- Structure Identifiers:
UPDATE &&securitySchemaName..COEPAGECONTROL
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
          				<ID>Structure Identifiers Enabled</ID>
          				<Privileges>
            					<Operator>AND</Operator>
          				</Privileges>
          			<Controls>
            				<Control>
              					<ID>IDENTIFIERTYPETextBox</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>4</COEFormID>
            				</Control>
                                        <Control>
              					<ID>IdentifierValueTextBox</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>4</COEFormID>
            				</Control>
          			</Controls>
          			<AppSettings>
            				<Operator>OR</Operator>
            				<ID/>
             				<AppSetting>
              					<ID>Structure_Identifiers</ID>
              					<Key>Registration/MISC/Structure_Identifiers</Key>
              					<Value>True</Value>
              					<Type>COEConfiguration</Type>
            				</AppSetting>
          			</AppSettings>
        	</ControlSetting>')).getClobVal()
		  where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';

-- Registry Identifiers:
UPDATE &&securitySchemaName..COEPAGECONTROL
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml), 
			  '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
          				<ID>Registry Identifiers Enabled</ID>
          				<Privileges>
            					<Operator>AND</Operator>
          				</Privileges>
          			<Controls>
            				<Control>
              					<ID>IDENTIFIERTYPETextBox</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>0</COEFormID>
            				</Control>
                                        <Control>
              					<ID>IdentifierValueTextBox</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>0</COEFormID>
            				</Control>
          			</Controls>
          			<AppSettings>
            				<Operator>OR</Operator>
            				<ID/>
             				<AppSetting>
              					<ID>Registry_Identifiers</ID>
              					<Key>Registration/MISC/Registry_Identifiers</Key>
              					<Value>True</Value>
              					<Type>COEConfiguration</Type>
            				</AppSetting>
          			</AppSettings>
        	</ControlSetting>')).getClobVal()
		  where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';

-- Component Identifiers:
UPDATE &&securitySchemaName..COEPAGECONTROL
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
          				<ID>Component Identifiers Enabled</ID>
          				<Privileges>
            					<Operator>AND</Operator>
          				</Privileges>
          			<Controls>
            				<Control>
              					<ID>IDENTIFIERTYPETextBox</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>1</COEFormID>
            				</Control>
                                        <Control>
              					<ID>IdentifierValueTextBox</ID>
              					<TypeOfControl>COEGenerableControl</TypeOfControl>
              					<PlaceHolderID>FormGeneratorHolder</PlaceHolderID> 
              					<COEFormID>1</COEFormID>
            				</Control>
          			</Controls>
          			<AppSettings>
            				<Operator>OR</Operator>
            				<ID/>
             				<AppSetting>
              					<ID>Component_Identifiers</ID>
              					<Key>Registration/MISC/Component_Identifiers</Key>
              					<Value>True</Value>
              					<Type>COEConfiguration</Type>
            				</AppSetting>
          			</AppSettings>
        	</ControlSetting>')).getClobVal()
		  where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';




--#####################################
-- Updating Registration Object Config.
--#####################################

-- Add PropertyList on Structure level
Declare
	XmlTypeConfig								XmlType;
	DOMDocumentConfig				DBMS_XMLDom.DOMDocument;
	DOMNodeConfig					DBMS_XMLDom.DOMNode;
	DOMNodeBatchComponent				DBMS_XMLDom.DOMNode;
	DOMELEMENTComponentRegNum			DBMS_XMLDom.DOMELEMENT;
	DOMNODEComponentRegNum				DBMS_XMLDom.DOMNODE;
	V_FOUND NUMBER(1) := 0;
Begin

	SELECT COUNT(1) INTO V_FOUND
	  FROM &&schemaName..COEOBJECTCONFIG
    WHERE ID=2;
	
	IF V_FOUND > 0 THEN
		-- Get Structure node
		SELECT XmlType.CreateXml(XML)
		  INTO XmlTypeConfig
		  FROM &&schemaName..COEOBJECTCONFIG
		WHERE ID=2;
		DOMDocumentConfig := DBMS_XMLDom.NewDOMDocument(XmlTypeConfig);
		DOMNodeConfig := DBMS_XMLDom.MakeNode(DOMDocumentConfig);
		DOMNodeBatchComponent := dbms_xslprocessor.selectSingleNode(DOMNodeConfig,'MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent');
	  
		-- Create new PropertyList node
		DOMELEMENTComponentRegNum := DBMS_XMLDOM.CREATEELEMENT(DOMDocumentConfig,'ComponentRegNum','');
		DOMNODEComponentRegNum := DBMS_XMLDOM.MAKENODE(DOMELEMENTComponentRegNum);
	  
		-- Append PropertyList node to Structure node
		DOMNODEComponentRegNum	 := DBMS_XMLDOM.APPENDCHILD(DOMNodeBatchComponent,DOMNODEComponentRegNum);
		-- Save updated xml to COEOBJECDTCONFIG
		UPDATE &&schemaName..COEOBJECTCONFIG
			SET XML = XmlTypeConfig.GetClobVal()	WHERE ID = 2;
	  
		COMMIT;
	END IF;
End;
/