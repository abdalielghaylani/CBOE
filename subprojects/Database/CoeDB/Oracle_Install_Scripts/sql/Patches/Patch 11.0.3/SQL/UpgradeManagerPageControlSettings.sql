--####################################
-- Enable Page Controls for Manager
--####################################
BEGIN
	&&schemaName..ConfigurationManager.CreateOrUpdateParameter('Manager','MISC','PageControlsManager','ENABLE',NULL, NULL,NULL, NULL);
	COMMIT;
END;
/

--####################################
-- Page Control Settings required XMLs
--####################################

DELETE FROM &&schemaName..COEPAGECONTROL WHERE APPLICATION IN ('MANAGER') AND TYPE IN ('CUSTOM');
UPDATE &&schemaName..COEPAGECONTROL SET CONFIGURATIONXML='COE_DVMANAGER_PRIVILEGES|COE_SECMANAGER_PRIVILEGES|CS_SECURITY_PRIVILEGES' WHERE APPLICATION IN ('MANAGER') AND TYPE IN ('PRIVILEGES');

DECLARE
 L_CustomXml_1 CLOB:= '<?xml version="1.0" encoding="utf-8"?>
<COEPageControlSettings>
	<Type>Custom</Type>
	<Application>MANAGER</Application>
	<Pages>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_MANAGEUSERS_ASPX</ID>
			<ControlSettings>
				<ControlSetting>
					<ID>Add_User</ID>
					<Privileges>
						<Operator>OR</Operator>
						<Privilege>
							<ID>CSS_CREATE_USER</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>AddButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
				</ControlSetting>
				<ControlSetting>
					<ID>Edit_User</ID>
					<Privileges>
						<Operator>OR</Operator>
						<Privilege>
							<ID>CSS_EDIT_USER</ID>
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
				</ControlSetting>
				<ControlSetting>
					<ID>Delete_User</ID>
					<Privileges>
						<Operator>OR</Operator>
						<Privilege>
							<ID>CSS_DELETE_USER</ID>
						</Privilege>
					</Privileges>
					<Controls>
						<Control>
							<ID>DeleteButton</ID>
							<TypeOfControl>Control</TypeOfControl>
							<PlaceHolderID/>
							<COEFormID>-1</COEFormID>
						</Control>
					</Controls>
				</ControlSetting>
			</ControlSettings>
		</Page>
	</Pages>
</COEPageControlSettings>';	

 
BEGIN
	INSERT INTO &&schemaName..COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('MANAGER','CUSTOM', L_CustomXml_1);	
	COMMIT;    
END;
/