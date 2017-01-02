--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--#####################################
-- Updating Registration Configuration
--#####################################
DECLARE
	V_FOUND NUMBER(1) := 0;
BEGIN
	SELECT COUNT(1) INTO V_FOUND FROM &&securitySchemaName..COECONFIGURATION WHERE LOWER(DESCRIPTION)='registration';
	
	IF V_FOUND > 0 THEN
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','ShowRequestFromContainer','True', 'Configures whether to show the request material link from a specific container', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','ShowRequestFromBatch','True', 'Configures whether to show the request material link from a specific batch', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','ShowRequestMaterial','True', 'Configures whether to show the request material link from the record itself', 'PICKLIST','True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','UseFullContainerForm','True', 'Indicates if full container form is going to be used', 'PICKLIST', 'True|False', 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','ChemInvSchemaName','CHEMINVDB2', 'Configures ChemInv schema name', 'TEXT', NULL, 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','InvContainersDataviewID','8005', 'Configures Inventory Containers dataviewid.', 'TEXT', NULL, 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','RequestMaterialURL','/Cheminv/GUI/RequestSample.asp?action=create', 'Configures Inventory url to request material', 'TEXT', NULL, 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','NewContainerURL','/Cheminv/gui/CreateOrEditContainer.asp?GetData=new', 'Configures Inventory url to create a new container', 'TEXT', NULL, 'False');
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','INVENTORY','ViewContainerURL','/Cheminv/gui/ViewBatchContainer.asp', 'Configures Inventory url to view a container', 'TEXT', NULL, 'False');
	END IF;
END;
/

--#####################################
-- Adding Locking Workflow Privileges
--#####################################
DECLARE
	PRIVLEGETABLE VARCHAR(1000);
	SQLSTMT		  VARCHAR(6000);
BEGIN
	SELECT PRIVILEGE_TABLE_NAME INTO PRIVLEGETABLE FROM COEDB.PRIVILEGE_TABLES WHERE UPPER(APP_NAME) = 'CHEMICAL REGISTRATION';
	SQLSTMT := 'ALTER TABLE ' || '&&securitySchemaName..' || PRIVLEGETABLE || ' ADD SET_LOCKED_FLAG NUMBER(1) DEFAULT 0';
	DBMS_OUTPUT.PUT_LINE(SQLSTMT);
	EXECUTE IMMEDIATE SQLSTMT;
	SQLSTMT := 'ALTER TABLE ' || '&&securitySchemaName..' || PRIVLEGETABLE || ' ADD TOGGLE_LOCKED_FLAG NUMBER(1) DEFAULT 0';
	DBMS_OUTPUT.PUT_LINE(SQLSTMT);
	EXECUTE IMMEDIATE SQLSTMT;
END;
/

--####################################
-- Page Control Settings required XMLs
--####################################
-- New Setting InventoryIntegration
 UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=updateXML(
		  xmltype(configurationxml),
		  '//ControlSetting[ID="SendToInventory"]/AppSettings',
		  XMLTYPE( '<AppSettings>
						<ID>InventoryIntegration</ID>
						<Operator>AND</Operator>
						<AppSetting>
							<ID>InventoryIntegration</ID>
							<Key>Registration/INVENTORY/InventoryIntegration</Key>
							<Value>Enabled</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>')).getClobVal()
	where APPLICATION='REGISTRATION' AND TYPE = 'CUSTOM';

-- View the coeform that holds containers:
UPDATE &&securitySchemaName..COEPAGECONTROL 
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
				<ID>ShowContainers</ID>
				<Privileges>
					<Operator>OR</Operator>
					<ID>ShowContainers</ID>
					<Privilege>
						<ID>INV_BROWSE_ALL</ID>
					</Privilege>
				</Privileges>
				<Controls>
					<Control>
						<ID>9</ID>
						<TypeOfControl>COEForm</TypeOfControl>
						<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						<COEFormID>-1</COEFormID>
						<Action>Hide</Action>
					</Control>
				</Controls>
				<AppSettings>
					<ID>InventoryIntegration</ID>
					<Operator>AND</Operator>
					<AppSetting>
						<ID>InventoryIntegration</ID>
						<Key>Registration/INVENTORY/InventoryIntegration</Key>
						<Value>Enabled</Value>
						<Type>COEConfiguration</Type>
					</AppSetting>
				</AppSettings>
			</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM';

-- Request material:
UPDATE &&securitySchemaName..COEPAGECONTROL 
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
				<ID>RequestMaterial</ID>
				<Privileges>
					<Operator>OR</Operator>
					<ID>RequestMaterial</ID>
					<Privilege>
						<ID>INV_SAMPLE_REQUEST</ID>
					</Privilege>
				</Privileges>
				<Controls>
					<Control>
						<ID>ReqMaterial</ID>
						<TypeOfControl>COEGenerableControl</TypeOfControl>
						<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						<COEFormID>9</COEFormID>
					</Control>
				</Controls>
				<AppSettings>
					<Operator>AND</Operator>
					<ID/>
					<AppSetting>
						<ID>ShowRequestMaterial</ID>
						<Key>Registration/INVENTORY/ShowRequestMaterial</Key>
						<Value>True</Value>
						<Type>COEConfiguration</Type>
					</AppSetting>
					<AppSetting>
						<ID>InvHasRegIdInGroupingField</ID>
						<Key>InvHasRegIdInGroupingField</Key>
						<Value>True</Value>
						<Type>Application</Type>
					</AppSetting>
				</AppSettings>
			</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM';

-- Request material from a specific batch:
UPDATE &&securitySchemaName..COEPAGECONTROL 
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
				<ID>RequestMaterialFromBatch</ID>
				<Privileges>
					<Operator>OR</Operator>
					<ID>RequestMaterialFromBatch</ID>
					<Privilege>
						<ID>INV_SAMPLE_REQUEST</ID>
					</Privilege>
				</Privileges>
				<Controls>
					<Control>
						<ID>RequestFromBatchURL</ID>
						<TypeOfControl>Control</TypeOfControl>
						<ParentControlID>VContainersUltraGrid0OF1</ParentControlID>
						<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						<COEFormID>9</COEFormID>
					</Control>
				</Controls>
				<AppSettings>
					<Operator>AND</Operator>
					<ID/>
					<AppSetting>
						<ID>ShowRequestFromBatch</ID>
						<Key>Registration/INVENTORY/ShowRequestFromBatch</Key>
						<Value>True</Value>
						<Type>COEConfiguration</Type>
					</AppSetting>
					<AppSetting>
						<ID>InvHasRegBatchIdInGroupingField</ID>
						<Key>InvHasRegBatchIdInGroupingField</Key>
						<Value>True</Value>
						<Type>Application</Type>
					</AppSetting>
				</AppSettings>
			</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM';
	  
-- Request material from a specific container:
UPDATE &&securitySchemaName..COEPAGECONTROL 
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
				<ID>RequestMaterialFromContainer</ID>
				<Privileges>
					<Operator>OR</Operator>
					<ID>RequestMaterialFromContainer</ID>
					<Privilege>
						<ID>INV_SAMPLE_REQUEST</ID>
					</Privilege>
				</Privileges>
				<Controls>
					<Control>
						<ID>RequestFromContainerURL</ID>
						<TypeOfControl>Control</TypeOfControl>
						<ParentControlID>VContainersUltraGrid0OF1</ParentControlID>
						<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						<COEFormID>9</COEFormID>
					</Control>
				</Controls>
				<AppSettings>
					<Operator>OR</Operator>
					<ID/>
					<AppSetting>
						<ID>ShowRequestFromContainer</ID>
						<Key>Registration/INVENTORY/ShowRequestFromContainer</Key>
						<Value>True</Value>
						<Type>COEConfiguration</Type>
					</AppSetting>
				</AppSettings>
			</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM';
	  
-- To display the flask in chembioviz list view to create containers:

 UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=UPDATEXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Application',
		  XMLTYPE('<Application>CHEMBIOVIZ</Application>')).getClobVal()
	where APPLICATION='CHEMBIOVIZ' AND TYPE IN ('MASTER','CUSTOM');

UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/Controls',
		  'Control',
		  XMLTYPE('
				<Control>
				  <ID>CreateContainerLink</ID>
				  <Description>Link for create container</Description>
				  <FriendlyName>Create Container</FriendlyName>
				  <PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
				  <TypeOfControl>CompositeControl</TypeOfControl>
				  <ParentControlID>ListView</ParentControlID>
				  <COEFormID>0</COEFormID>
				</Control>')).getClobVal()
	where APPLICATION='CHEMBIOVIZ' AND TYPE='MASTER';
	
UPDATE &&securitySchemaName..COEPAGECONTROL 
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
                    <ID>CreateContainerForBatch</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>CreateContainerPriv</ID>
						<Privilege>
                            <ID>INV_CREATE_CONTAINER</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
							<ID>CreateContainerLink</ID>
                            <PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
                            <TypeOfControl>CompositeControl</TypeOfControl>
                            <ParentControlID>ListView</ParentControlID>
                            <COEFormID>0</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                        <Operator>AND</Operator>
                        <ID>SendToInventory</ID>
                        <AppSetting>
                            <ID>SendToInventory</ID>
                            <Key>Registration/INVENTORY/SendtoInventory</Key>
                            <Value>True</Value>
                            <Type>COEConfiguration</Type>
                        </AppSetting>
						<AppSetting>
							<ID>InventoryIntegration</ID>
							<Key>Registration/INVENTORY/InventoryIntegration</Key>
							<Value>Enabled</Value>
						<Type>COEConfiguration</Type>
					</AppSetting>
                    </AppSettings>
                </ControlSetting>')).getClobVal()
	  where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';
	  
UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
                    <ID>CreateContainer</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>CreateContainerPriv</ID>
						<Privilege>
                            <ID>INV_CREATE_CONTAINER</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
							<ID>SendToInventoryLink</ID>
                            <PlaceHolderID />
                            <TypeOfControl />
                            <COEFormID />
                        </Control>
                    </Controls>
                    <AppSettings>
                        <Operator>AND</Operator>
                        <ID>SendToInventory</ID>
                        <AppSetting>
                            <ID>SendToInventory</ID>
                            <Key>Registration/INVENTORY/SendtoInventory</Key>
                            <Value>True</Value>
                            <Type>COEConfiguration</Type>
                        </AppSetting>
						<AppSetting>
							<ID>InventoryIntegration</ID>
							<Key>Registration/INVENTORY/InventoryIntegration</Key>
							<Value>Enabled</Value>
						<Type>COEConfiguration</Type>
					</AppSetting>
                    </AppSettings>
                </ControlSetting>')).getClobVal()
	  where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM' AND NOT (EXISTSNODE(XMLTYPE(CONFIGURATIONXML), '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings/ControlSetting[ID="CreateContainer"]')=1);
	  
--------- APPROVALS WORKFLOW ----------
UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=updateXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX"]/ControlSettings/ControlSetting[ID="Test"]',
		  '<ControlSetting>
					<ID>ApprovedTempRegistries</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>ApprovedTempRegistries</ID>
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
				</ControlSetting>').getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM' and (EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX"]/ControlSettings/ControlSetting[ID="Test"]') = 1);

UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('<ControlSetting>
					<ID>SubmittedTempRegistries</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>SubmittedTempRegistries</ID>
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
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM' and not (EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX"]/ControlSettings/ControlSetting[ID="SubmittedTempRegistries"]') = 1);

UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('<ControlSetting>
					<ID>TempRegistries</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>TempRegistries</ID>
							<PlaceHolderID/>
							<TypeOfControl>Control</TypeOfControl>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<AppSetting>
							<ID>ApprovalsEnabled</ID>
							<Key>Registration/REGADMIN/ApprovalsEnabled</Key>
							<Value>False</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM' and not (EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX"]/ControlSettings/ControlSetting[ID="TempRegistries"]') = 1);
	  
UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>StatusControlSettingForApprovals</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>StatusProperty</ID>
							<Description>This control allows the modification of the status from Submitted to Approved and the other way around</Description>
							<FriendlyName>Approved Control</FriendlyName>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<COEFormID>0</COEFormID>
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
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM' and not(EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings/ControlSetting[ID="StatusControlSettingForApprovals"]') = 1);

UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>SearchStatusControlSettingForApprovals</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>StatusStateControl</ID>
							<Description>This control allows to filter temp records by status</Description>
							<FriendlyName>Approved Control</FriendlyName>
							<PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<COEFormID>0</COEFormID>
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
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM' and not(EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings/ControlSetting[ID="SearchStatusControlSettingForApprovals"]') = 1);
	  
	  
UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>ListStatusControlSettingForApprovals</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>STATUSCOLUMN</ID>
							<Description>Approved or not flag</Description>
							<FriendlyName>Approved or not flag</FriendlyName>
							<PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
							<TypeOfControl>Control</TypeOfControl>
							<ParentControlID>ListView</ParentControlID>
							<COEFormID>0</COEFormID>
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
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM' and not(EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings/ControlSetting[ID="ListStatusControlSettingForApprovals"]') = 1);


UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=updateXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings/ControlSetting[ID="ApprovalsEnabled"]',
		  XMLTYPE('
			<ControlSetting>
					<ID>ApprovalsEnabled</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
						<Privilege>
							<ID>SET_APPROVED_FLAG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ApproveMarkedLink</ID>
							<Description>Approve Marked linkButton</Description>
							<FriendlyName>Approve Marked linkButton</FriendlyName>
							<PlaceHolderID/>
							<TypeOfControl/>
							<COEFormID/>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>AND</Operator>
						<AppSetting>
							<Key>Registration/REGADMIN/ApprovalsEnabled</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM' and (EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings/ControlSetting[ID="ApprovalsEnabled"]') = 1);


---------- LOCKED WORKFLOW ---------

-- Locking mechanism is to be used for preventing permanent records edition
UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=UPDATEXML(
		  xmltype(configurationxml),
		  '//AppSetting[ID="IsRegistryApproved"]/..',
		  XMLTYPE('<AppSettings>
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
					</AppSettings>')).getClobVal()
	where UPPER(APPLICATION)='REGISTRATION' AND TYPE IN ('MASTER','CUSTOM');
	
UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>StatusControlSettingForLocking</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>LockedStatusProperty</ID>
							<Description>This control allows the modification of the status from Registered to Locked and the other way around</Description>
							<FriendlyName>Lock Control</FriendlyName>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<COEFormID>0</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<AppSetting>
							<ID>LockingEnabled</ID>
							<Key>Registration/REGADMIN/LockingEnabled</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM' and not(EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings/ControlSetting[ID="StatusControlSettingForLocking"]') = 1);

UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>SearchStatusControlSettingForLocking</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>LockedStatusStateControl</ID>
							<Description>This control allows to filter temp records by status</Description>
							<FriendlyName>Approved Control</FriendlyName>
							<PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<COEFormID>0</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<AppSetting>
							<ID>LockingEnabled</ID>
							<Key>Registration/REGADMIN/LockingEnabled</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM' and not(EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings/ControlSetting[ID="SearchStatusControlSettingForLocking"]') = 1);
	  
	  
UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>ListLockedStatusControlSettingForLocking</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>LOCKEDSTATUSCOLUMN</ID>
							<Description>Approved or not flag</Description>
							<FriendlyName>Approved or not flag</FriendlyName>
							<PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
							<TypeOfControl>Control</TypeOfControl>
							<ParentControlID>ListView</ParentControlID>
							<COEFormID>0</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<AppSetting>
							<ID>LockingEnabled</ID>
							<Key>Registration/REGADMIN/LockingEnabled</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM' and not(EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings/ControlSetting[ID="ListLockedStatusControlSettingForLocking"]') = 1);
	  
UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>LockingEnabled</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
						<Privilege>
							<ID>SET_LOCKED_FLAG</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>LockMarkedLink</ID>
							<Description>Lock Marked linkButton</Description>
							<FriendlyName>Lock Marked linkButton</FriendlyName>
							<PlaceHolderID/>
							<TypeOfControl/>
							<COEFormID/>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>AND</Operator>
						<AppSetting>
							<Key>Registration/REGADMIN/LockingEnabled</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM' and not (EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings/ControlSetting[ID="LockingEnabled"]') = 1);
	  
--**Changes for Batch_prefix field (Setting the visibility of the control based on EnableUseBatchPrefixes config value**-----

	UPDATE &&securitySchemaName..COEPAGECONTROL
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
					<ID>EnableUseBatchPrefixes</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>BATCH_PREFIXProperty</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1002</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseBatchPrefixes</ID>
							<Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
		  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
	      
	  UPDATE &&securitySchemaName..COEPAGECONTROL 
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
					<ID>EnableUseBatchPrefixes</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>BATCH_PREFIXProperty</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1002</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseBatchPrefixes</ID>
							<Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
		  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
	      
		UPDATE &&securitySchemaName..COEPAGECONTROL
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
					<ID>EnableUseBatchPrefixes</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>SequenceDropDownList</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>0</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseBatchPrefixes</ID>
							<Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
							<Value>False</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
		  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
	      
		 UPDATE &&securitySchemaName..COEPAGECONTROL 
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
					<ID>EnableUseBatchPrefixes</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>SALTANDBATCHSUFFIXProperty</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1002</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseBatchPrefixes</ID>
							<Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
		  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
	      
	  UPDATE &&securitySchemaName..COEPAGECONTROL 
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
					<ID>EnableUseBatchPrefixes</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>SALTANDBATCHSUFFIXProperty</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1002</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseBatchPrefixes</ID>
							<Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
		  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
		  
    	  UPDATE &&securitySchemaName..COEPAGECONTROL 
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
					<ID>EnableUseBatchPrefixes</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>BATCH_PREFIXProperty</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1002</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseBatchPrefixes</ID>
							<Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
		  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
		  
		UPDATE &&securitySchemaName..COEPAGECONTROL 
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
					<ID>EnableUseBatchPrefixes</ID>
					<Privileges>
						<Operator>AND</Operator>
					</Privileges>
					<Controls>
						<Control>
							<ID>SALTANDBATCHSUFFIXProperty</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1002</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>EnableUseBatchPrefixes</ID>
							<Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
		  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
		  
		UPDATE &&securitySchemaName..COEPAGECONTROL 
        SET configurationxml=insertChildXML(
              xmltype(configurationxml),
              '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings',
              'ControlSetting',
              XMLTYPE('
                <ControlSetting>
                    <ID>EnableUseBatchPrefixes</ID>
                    <Privileges>
                        <Operator>AND</Operator>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>SequenceDropDownList</ID>
                            <TypeOfControl>COEGenerableControl</TypeOfControl>
                            <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                            <COEFormID>0</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                        <Operator>OR</Operator>
                        <ID/>
                        <AppSetting>
                            <ID>EnableUseBatchPrefixes</ID>
                            <Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
                            <Value>False</Value>
                            <Type>COEConfiguration</Type>
                        </AppSetting>
                    </AppSettings>
                </ControlSetting>')).getClobVal()
          where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
      --Batch prefix and Prefix made visible/ not visible in Search forms   
		UPDATE &&securitySchemaName..COEPAGECONTROL 
        SET configurationxml=insertChildXML(
              xmltype(configurationxml),
              '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
              'ControlSetting',
              XMLTYPE('
                <ControlSetting>
                    <ID>EnablePrefixes</ID>
                    <Privileges>
                        <Operator>AND</Operator>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>PREFIXTextBox</ID>
                            <TypeOfControl>COEGenerableControl</TypeOfControl>
                            <PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
                            <ParentControlID>QueryForm</ParentControlID>
                            <COEFormID>0</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                        <Operator>OR</Operator>
                        <ID/>
                        <AppSetting>
                            <ID>EnableUseBatchPrefixes</ID>
                            <Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
                            <Value>False</Value>
                            <Type>COEConfiguration</Type>
                        </AppSetting>
                    </AppSettings>
                </ControlSetting>')).getClobVal()
          where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';
          
        UPDATE &&securitySchemaName..COEPAGECONTROL 
        SET configurationxml=insertChildXML(
              xmltype(configurationxml),
              '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
              'ControlSetting',
              XMLTYPE('
                <ControlSetting>
                    <ID>EnableUseBatchPrefixes</ID>
                    <Privileges>
                        <Operator>AND</Operator>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>BATCH_PREFIXProperty</ID>
                            <TypeOfControl>COEGenerableControl</TypeOfControl>
                            <PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
                            <ParentControlID>QueryForm</ParentControlID>
                            <COEFormID>0</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                        <Operator>OR</Operator>
                        <ID/>
                        <AppSetting>
                            <ID>EnableUseBatchPrefixes</ID>
                            <Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
                            <Value>True</Value>
                            <Type>COEConfiguration</Type>
                        </AppSetting>
                    </AppSettings>
                </ControlSetting>')).getClobVal()
          where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';
          
        UPDATE &&securitySchemaName..COEPAGECONTROL 
        SET configurationxml=insertChildXML(
              xmltype(configurationxml),
              '/COEPageControlSettings/Pages/Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
              'ControlSetting',
              XMLTYPE('
                <ControlSetting>
                    <ID>EnableBatchPrefixesSearchReg</ID>
                    <Privileges>
                        <Operator>AND</Operator>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>BATCH_PREFIXProperty</ID>
                            <TypeOfControl>COEGenerableControl</TypeOfControl>
                            <PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
                            <ParentControlID>QueryForm</ParentControlID>
                            <COEFormID>2</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                        <Operator>OR</Operator>
                        <ID/>
                        <AppSetting>
                            <ID>EnableUseBatchPrefixes</ID>
                            <Key>Registration/REGADMIN/EnableUseBatchPrefixes</Key>
                            <Value>True</Value>
                            <Type>COEConfiguration</Type>
                        </AppSetting>
                    </AppSettings>
                </ControlSetting>')).getClobVal()
          where APPLICATION='CHEMBIOVIZ' AND TYPE='CUSTOM';
		   
 ---End of batch prefix changes---   

 
 ------ SubmissionComments workflow: For temporary duplicates --------
 
 UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>SubmissionCommentsIDSettingForTempDups</ID>
					<Privileges>
						<Operator>OR</Operator>
					  <Privilege>
						<ID>REGISTER_TEMP</ID>
					  </Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>SubmissionCommentsID</ID>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<COEFormID>0</COEFormID>
						</Control>
					</Controls>
					<AppSettings>
						<AppSetting>
							<ID>ApprovalsEnabled</ID>
							<Key>Registration/REGADMIN/EnableSubmissionDuplicateChecking</Key>
							<Value>True</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='REGISTRATION' AND TYPE='CUSTOM' and not(EXISTSNODE(xmltype(configurationxml), '//Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings/ControlSetting[ID="SubmissionCommentsIDSettingForTempDups"]') = 1);