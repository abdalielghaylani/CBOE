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
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','MISC','AppPageTitle','CambridgeSoft Registration Enterprise 12.3.1', 'Defines application page tile.', 'TEXT',NULL, 'False');
	END IF;
END;
/

-- New Setting for DocManager:
UPDATE &&securitySchemaName..COEPAGECONTROL
		SET configurationxml=insertChildXML(
			  xmltype(configurationxml),
			  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
			  'ControlSetting',
			  XMLTYPE('
				<ControlSetting>
					<ID>DocMgrEnabled</ID>
					<Privileges/>
				        <Controls>
					  <Control>
						<ID>8</ID>
						<TypeOfControl>COEForm</TypeOfControl>
						<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						<COEFormID>-1</COEFormID>
						<Action>Hide</Action>
					  </Control>
				       </Controls>
					<AppSettings>
                                                <ID>DocMgrEnabled</ID>
						<Operator>OR</Operator>
						<ID/>
						<AppSetting>
							<ID>DocMgrEnabled</ID>
							<Key>REGISTRATION/DOCMGR/DocMgrEnabled</Key>
							<Value>False</Value>
							<Type>COEConfiguration</Type>
						</AppSetting>
					</AppSettings>
				</ControlSetting>')).getClobVal()
		  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
