-- Updating Registration Configuration
DECLARE
	V_FOUND NUMBER(1) := 0;
BEGIN
	SELECT COUNT(1) INTO V_FOUND FROM &&securitySchemaName..COECONFIGURATION WHERE LOWER(DESCRIPTION)='registration';
	
	IF V_FOUND > 0 THEN

&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','LockingEnabled','False', 'This setting enables the Locking workflow. Records are associated with locking status and are managed by Administrators.', 'PICKLIST','True|False', 'False');

	END IF;
END;
/

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
							<ID>STATUSID</ID>
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
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM';

UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
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
							<ID>ASP.forms_search_contentarea_chembiovizsearch_aspx</ID>
							<TypeOfControl>Page</TypeOfControl>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM';

UPDATE &&securitySchemaName..COEPAGECONTROL
	SET configurationxml=insertChildXML(
		  xmltype(configurationxml),
		  '//Page[ID="ASP.forms_search_contentarea_chembiovizsearch_aspx"]/ControlSettings',
		  'ControlSetting',
		  XMLTYPE('
			<ControlSetting>
					<ID>SearchTempRegistry</ID>
					<Privileges>
						<Operator>OR</Operator> 
                                                <ID>SearchTempRegistry</ID>
						 <Privilege>
					          <ID>SEARCH_TEMP</ID> 
					         </Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>ASP.forms_search_contentarea_chembiovizsearch_aspx</ID>
							<TypeOfControl>Page</TypeOfControl>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
					<AppSettings/>
				</ControlSetting>')).getClobVal()
	  where UPPER(APPLICATION)='CHEMBIOVIZ' AND TYPE='CUSTOM';
/