--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

PROMPT Inserting starting data...

--#########################################################
--ADD STARTING DATA
--#########################################################

INSERT INTO COEOBJECTTYPE (ID, NAME) VALUES (1, 'COEDATAVIEW');
INSERT INTO COEOBJECTTYPE (ID, NAME) VALUES (2, 'COEFORM');
INSERT INTO COEOBJECTTYPE (ID, NAME) VALUES (3, 'COESEARCHCRITERIA');
INSERT INTO COEOBJECTTYPE (ID, NAME) VALUES (4, 'COEGENERICOBJECT');

INSERT INTO COEPRINCIPALTYPE (ID, NAME) VALUES (1, 'USER');
INSERT INTO COEPRINCIPALTYPE (ID, NAME) VALUES (2, 'ROLE');

INSERT INTO COEGLOBALS (ID, VALUE) VALUES ('SCHEMAVERSION', '&&schemaVersion');
INSERT INTO COEGLOBALS (ID, VALUE) VALUES ('VERSION_APP', '&&schemaVersion');

INSERT INTO PRIVILEGE_TABLES(PRIVILEGE_TABLE_NAME, APP_NAME, APP_URL,TABLE_SPACE) values('COE_SECMANAGER_PRIVILEGES','COEMANAGER_SEC','', 'T_COEDB_TABL');
commit;

INSERT INTO SECURITY_ROLES (privilege_table_int_id, role_name,COEIDENTIFIER) VALUES (PRIVILEGE_TABLES_SEQ.CURRVAL, 'COE_SEC_ADMIN', 'COEMANAGER_SEC');
commit;

INSERT INTO COE_SECMANAGER_PRIVILEGES (role_internal_id,  can_browse, can_insert,can_update,can_delete)VALUES(security_roles_seq.CURRVAL,  1, 1, 1,1);
commit;

INSERT INTO PRIVILEGE_TABLES(PRIVILEGE_TABLE_NAME, APP_NAME, APP_URL, TABLE_SPACE) values('COE_DVMANAGER_PRIVILEGES','COEMANAGER_DV','', 'T_COEDB_TABL');
commit;

INSERT INTO SECURITY_ROLES (privilege_table_int_id, role_name,COEIDENTIFIER)VALUES (PRIVILEGE_TABLES_SEQ.CURRVAL, 'COE_DV_ADMIN', 'COEMANAGER_DV');
commit;

INSERT INTO COE_DVMANAGER_PRIVILEGES (role_internal_id, can_browse, can_insert,can_update,can_delete)VALUES(security_roles_seq.CURRVAL,  1, 1, 1,1);
commit;

INSERT INTO COEDATAVIEW(ID,NAME,DESCRIPTION,USER_ID,IS_PUBLIC,FORMGROUP,DATE_CREATED,COEDATAVIEW,DATABASE) values(0,'Master Dataview','Master Dataview','COEDB','1','',sysdate,'<?xml version="1.0" encoding="utf-8"?><COEDataView  	xmlns="COE.COEDataView" basetable="-1" database="COEDB" dataviewid="0"><tables> </tables></COEDataView>','COEDB');


INSERT INTO COEPERMISSIONS(OBJECTID, OBJECTYPEID, PRINCIPALID, PRINCIPALTYPEID) values(0,1,2,1);

INSERT INTO COEFORMTYPE(FormTypeID, FormType) VALUES(1, 'search');
INSERT INTO COEFORMTYPE(FormTypeID, FormType) VALUES(2, 'application');

COMMIT;

--##################################
-- Page Control Settings required XMLs (Security + DV_Mgr)
--##################################

DELETE FROM COEPAGECONTROL WHERE APPLICATION IN ('MANAGER');

DECLARE
 L_MasterXml CLOB:= '<?xml version=''1.0'' encoding=''utf-8''?>
<COEPageControlSettings type="Master">
	<Application>MANAGER</Application>
	<ID>COEPageControlSettings_Manager_Master</ID>
	<Pages>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_DEFINELOOKUP_ASPX</ID>
			<Description>DEFINELOOKUP_ASPX</Description>
			<FriendlyName>DEFINELOOKUP_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_DEFINELOOKUP_ASPX</ID>
					<Description>DEFINELOOKUP_ASPX</Description>
					<FriendlyName>DEFINELOOKUP_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_DEFINERELATIONSHIPS_ASPX</ID>
			<Description>DEFINERELATIONSHIPS_ASPX</Description>
			<FriendlyName>DEFINERELATIONSHIPS_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_DEFINERELATIONSHIPS_ASPX</ID>
					<Description>DEFINERELATIONSHIPS_ASPX</Description>
					<FriendlyName>DEFINERELATIONSHIPS_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_EDITTABLEANDFIELDS_ASPX</ID>
			<Description>EDITTABLEANDFIELDS_ASPX</Description>
			<FriendlyName>EDITTABLEANDFIELDS_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_EDITTABLEANDFIELDS_ASPX</ID>
					<Description>EDITTABLEANDFIELDS_ASPX</Description>
					<FriendlyName>EDITTABLEANDFIELDS_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_ENTERNAMEDESCRIPTION_ASPX</ID>
			<Description>ENTERNAMEDESCRIPTION_ASPX</Description>
			<FriendlyName>ENTERNAMEDESCRIPTION_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_ENTERNAMEDESCRIPTION_ASPX</ID>
					<Description>ENTERNAMEDESCRIPTION_ASPX</Description>
					<FriendlyName>ENTERNAMEDESCRIPTION_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_NEWDATAVIEW_ASPX</ID>
			<Description>NEWDATAVIEW_ASPX</Description>
			<FriendlyName>NEWDATAVIEW_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_NEWDATAVIEW_ASPX</ID>
					<Description>NEWDATAVIEW_ASPX</Description>
					<FriendlyName>NEWDATAVIEW_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_SECURITY_ASPX</ID>
			<Description>SECURITY_ASPX</Description>
			<FriendlyName>SECURITY_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_SECURITY_ASPX</ID>
					<Description>SECURITY_ASPX</Description>
					<FriendlyName>SECURITY_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_SELECTBASETABLE_ASPX</ID>
			<Description>SELECTBASETABLE_ASPX</Description>
			<FriendlyName>SELECTBASETABLE_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_SELECTBASETABLE_ASPX</ID>
					<Description>SELECTBASETABLE_ASPX</Description>
					<FriendlyName>SELECTBASETABLE_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_SELECTTABLES_ASPX</ID>
			<Description>SELECTTABLES_ASPX</Description>
			<FriendlyName>SELECTTABLES_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_SELECTTABLES_ASPX</ID>
					<Description>SELECTTABLES_ASPX</Description>
					<FriendlyName>SELECTTABLES_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_VALIDATIONSUMMARY_ASPX</ID>
			<Description>VALIDATIONSUMMARY_ASPX</Description>
			<FriendlyName>VALIDATIONSUMMARY_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_VALIDATIONSUMMARY_ASPX</ID>
					<Description>VALIDATIONSUMMARY_ASPX</Description>
					<FriendlyName>VALIDATIONSUMMARY_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_HOMEPAGE_HOME_ASPX</ID>
			<Description>DV_HOME_ASPX</Description>
			<FriendlyName>DV_HOME_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_DATAVIEWMANAGER_CONTENTAREA_HOMEPAGE_HOME_ASPX</ID>
					<Description>DV_HOME_ASPX</Description>
					<FriendlyName>DV_HOME_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_PUBLIC_CONTENTAREA_ABOUT_ASPX</ID>
			<Description>ABOUT_ASPX</Description>
			<FriendlyName>ABOUT_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_PUBLIC_CONTENTAREA_ABOUT_ASPX</ID>
					<Description>ABOUT_ASPX</Description>
					<FriendlyName>ABOUT_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_PUBLIC_CONTENTAREA_HELP_ASPX</ID>
			<Description>HELP_ASPX</Description>
			<FriendlyName>HELP_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_PUBLIC_CONTENTAREA_HELP_ASPX</ID>
					<Description>HELP_ASPX</Description>
					<FriendlyName>HELP_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX</ID>
			<Description>PUB_HOME_ASPX</Description>
			<FriendlyName>PUB_HOME_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX</ID>
					<Description>PUB_HOME_ASPX</Description>
					<FriendlyName>PUB_HOME_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_PUBLIC_CONTENTAREA_LOGIN_ASPX</ID>
			<Description>LOGIN_ASPX</Description>
			<FriendlyName>LOGIN_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_PUBLIC_CONTENTAREA_LOGIN_ASPX</ID>
					<Description>LOGIN_ASPX</Description>
					<FriendlyName>LOGIN_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_PUBLIC_CONTENTAREA_MESSAGES_ASPX</ID>
			<Description>MESSAGES_ASPX</Description>
			<FriendlyName>MESSAGES_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_PUBLIC_CONTENTAREA_MESSAGES_ASPX</ID>
					<Description>MESSAGES_ASPX</Description>
					<FriendlyName>MESSAGES_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_CHANGEPASSWORD_ASPX</ID>
			<Description>CHANGEPASSWORD_ASPX</Description>
			<FriendlyName>CHANGEPASSWORD_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_CHANGEPASSWORD_ASPX</ID>
					<Description>CHANGEPASSWORD_ASPX</Description>
					<FriendlyName>CHANGEPASSWORD_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_EDITROLE_ASPX</ID>
			<Description>EDITROLE_ASPX</Description>
			<FriendlyName>EDITROLE_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_EDITROLE_ASPX</ID>
					<Description>EDITROLE_ASPX</Description>
					<FriendlyName>EDITROLE_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_EDITROLEROLES_ASPX</ID>
			<Description>EDITROLEROLES_ASPX</Description>
			<FriendlyName>EDITROLEROLES_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_EDITROLEROLES_ASPX</ID>
					<Description>EDITROLEROLES_ASPX</Description>
					<FriendlyName>EDITROLEROLES_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_EDITROLEUSERS_ASPX</ID>
			<Description>EDITROLEUSERS_ASPX</Description>
			<FriendlyName>EDITROLEUSERS_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_EDITROLEUSERS_ASPX</ID>
					<Description>EDITROLEUSERS_ASPX</Description>
					<FriendlyName>EDITROLEUSERS_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_EDITUSER_ASPX</ID>
			<Description>EDITUSER_ASPX</Description>
			<FriendlyName>EDITUSER_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_EDITUSER_ASPX</ID>
					<Description>EDITUSER_ASPX</Description>
					<FriendlyName>EDITUSER_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_MANAGEROLES_ASPX</ID>
			<Description>MANAGEROLES_ASPX</Description>
			<FriendlyName>MANAGEROLES_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_MANAGEROLES_ASPX</ID>
					<Description>MANAGEROLES_ASPX</Description>
					<FriendlyName>MANAGEROLES_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_MANAGEUSERS_ASPX</ID>
			<Description>MANAGEUSERS_ASPX</Description>
			<FriendlyName>MANAGEUSERS_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_MANAGEUSERS_ASPX</ID>
					<Description>MANAGEUSERS_ASPX</Description>
					<FriendlyName>MANAGEUSERS_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_MANAGEUSERS_ASPX</ID>
			<Description>MANAGEUSERS_ASPX</Description>
			<FriendlyName>MANAGEUSERS_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_MANAGEUSERS_ASPX</ID>
					<Description>MANAGEUSERS_ASPX</Description>
					<FriendlyName>MANAGEUSERS_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_ROLELIST_ASPX</ID>
			<Description>ROLELIST_ASPX</Description>
			<FriendlyName>ROLELIST_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_ROLELIST_ASPX</ID>
					<Description>ROLELIST_ASPX</Description>
					<FriendlyName>ROLELIST_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_USERLIST_ASPX</ID>
			<Description>USERLIST_ASPX</Description>
			<FriendlyName>USERLIST_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.FORMS_SECURITYMANAGER_CONTENTAREA_USERLIST_ASPX</ID>
					<Description>USERLIST_ASPX</Description>
					<FriendlyName>USERLIST_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
		<Page>
			<ID>ASP.LOGOFF_ASPX</ID>
			<Description>LOGOFF_ASPX</Description>
			<FriendlyName>LOGOFF_ASPX</FriendlyName>
			<Controls>
				<Control>
					<ID>ASP.LOGOFF_ASPX</ID>
					<Description>LOGOFF_ASPX</Description>
					<FriendlyName>LOGOFF_ASPX</FriendlyName>
					<PlaceHolderID/>
					<TypeOfControl>Page</TypeOfControl>
				</Control>
			</Controls>
		</Page>
	</Pages>
</COEPageControlSettings>';

 L_CustomXml CLOB:= '<?xml version=''1.0'' encoding=''utf-8''?>
<COEPageControlSettings>
	<Type>Custom</Type>
	<Application>MANAGER</Application>
	<Pages/>
</COEPageControlSettings>';

 L_PrivilegesXml CLOB:= 'COE_DVMANAGER_PRIVILEGES|COE_SECMANAGER_PRIVILEGES';

BEGIN
    INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('MANAGER','MASTER', L_MasterXml);
	INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('MANAGER','CUSTOM', L_CustomXml);
	INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('MANAGER','PRIVILEGES', L_PrivilegesXml);
    COMMIT;
END;
/

DECLARE
   AConfiguration CLob;
BEGIN
    AConfiguration:=
    '<Manager>
    <applicationSettings name="Mgr Settings">
        <groups>
            <remove />
            <add name="DVManager" title="DataViewManager settings" description="Set description">
				<settings>
					<add name="UnpublishableSchemas" value="CTXSYS|DBSNMP|DMSYS|EXFSYS|IX|MDSYS|OLAPSYS|ORDPLUGINS|ORDSYS|SI_INFORMTN_SCHEMA|SYSMAN|TSMSYS|WMSYS"/>
				</settings>
			</add>
			 <remove />
			<add name="MISC" title="Miscellaneous settings">
				<settings>
					<!-- ENABLE or DISABLE -->
					<add name="PageControlsManager" value="DISABLE"/>
					<add name="HomeLinkURL" value="/coemanager/forms/public/contentarea/Home.aspx"/>
					<add name="AppPageTitle" value="CambridgeSoft COEManager Enterprise 11.0"/>
				</settings>
			</add>
        </groups>
    </applicationSettings>
</Manager>';
    &&schemaName..ConfigurationManager.InsertConfiguration ('Manager','CambridgeSoft.COE.Framework.Common.ApplicationDataConfigurationSection, CambridgeSoft.COE.Framework, Version=11.0.1.0, Culture=neutral, PublicKeyToken=1e3754866626dfbf',AConfiguration);
    COMMIT;
END;
/
--######################
-- Page Control Settings required XMLs for CHEMBIOVIZ
--######################

DELETE FROM COEPAGECONTROL WHERE APPLICATION IN ('CHEMBIOVIZ');

DECLARE
 L_MasterXml_1 CLOB:= '<?xml version="1.0" encoding="utf-8"?>
<COEPageControlSettings type="Master">
    <Application>CHEMBIOVIZ</Application>
    <ID>COEPageControlSettings_Chembioviz_Master</ID>
    <Pages>
        <Page>
            <ID>ASP.forms_search_contentarea_chembiovizsearch_aspx</ID>
            <Description>Chembioviz Search Page</Description>
            <FriendlyName>Search Page</FriendlyName>
            <Controls>               
                <Control>
                    <ID>FormGeneratorDebuggingInfo</ID>
                    <Description>Form Generator Debugging Info</Description>
                    <FriendlyName>FormGeneratorDebuggingInfo</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>FormGroupDebuggingInfo</ID>
                    <Description>Form Group Debugging Info</Description>
                    <FriendlyName>Form Group Debugging Info</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
				<Control>
				  <ID>ApprovedStateControl</ID>
				  <Description>Approved or not flag</Description>
				  <FriendlyName>Approved or not flag</FriendlyName>
				  <PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
				  <TypeOfControl>COEGenerableControl</TypeOfControl>
				  <COEFormID>0</COEFormID>
				</Control>
				<Control>
				  <ID>APPROVEDCOLUMN</ID>
				  <Description>Approved or not flag</Description>
				  <FriendlyName>Approved or not flag</FriendlyName>
				  <PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
				  <TypeOfControl>Control</TypeOfControl>
				  <ParentControlID>ListView</ParentControlID>
				  <COEFormID>0</COEFormID>
				</Control>				
				<Control>
				  <ID>ApproveMarkedLink</ID>
				  <Description>Approve Marked linkButton</Description>
				  <FriendlyName>Approve Marked linkButton</FriendlyName>
				  <PlaceHolderID></PlaceHolderID>
				  <TypeOfControl>Control</TypeOfControl>
				  <COEFormID></COEFormID>
				</Control>
            </Controls>
        </Page>
    </Pages>
    <AppSettings>       
		<AppSetting>
		  <ID>ApprovalsEnabled</ID>
		  <Key>Registration/REGADMIN/ApprovalsEnabled</Key>
		  <Value>False</Value>
		  <Type>COEConfiguration</Type>
		</AppSetting>        
    </AppSettings>
</COEPageControlSettings>';

 L_CustomXml_1 CLOB:= '<?xml version="1.0" encoding="utf-8"?>
 <COEPageControlSettings>
	<Type>Custom</Type>
	<Application>Chembioviz</Application>
	<Pages>
		<Page>
			<ID>ASP.forms_search_contentarea_chembiovizsearch_aspx</ID>
			<ControlSettings>
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
							<ID>ApprovedStateControl</ID>
							<PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<COEFormID>0</COEFormID>
						</Control>
						<Control>
						  <ID>APPROVEDCOLUMN</ID>
						  <Description>Approved or not flag</Description>
						  <FriendlyName>Approved or not flag</FriendlyName>
						  <PlaceHolderID>FormGeneratorHolder</PlaceHolderID>
						  <TypeOfControl>Control</TypeOfControl>
						  <ParentControlID>ListView</ParentControlID>
						  <COEFormID>0</COEFormID>
						</Control>						
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
				</ControlSetting>
			</ControlSettings>
		</Page>
	</Pages>
</COEPageControlSettings>
';
 L_PrivilegesXml CLOB:= 'CONFIG_REG';
 
 BEGIN
	INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('CHEMBIOVIZ','MASTER', L_MasterXml_1);

	INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('CHEMBIOVIZ','CUSTOM', L_CustomXml_1);	
	
	INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('CHEMBIOVIZ','PRIVILEGES', L_PrivilegesXml);
	COMMIT;    
END;
/
--######################
-- COEConfiguration entries for CHEMBIOVIZ
--######################
DELETE FROM COEDB.COECONFIGURATION WHERE DESCRIPTION IN ('CHEMBIOVIZ');
BEGIN
	INSERT INTO COEDB.COECONFIGURATION (DESCRIPTION, CLASSNAME, CONFIGURATIONXML) VALUES ('CHEMBIOVIZ','CambridgeSoft.COE.Framework.Common.ApplicationDataConfigurationSection, CambridgeSoft.COE.Framework, Version=11.0.1.0, Culture=neutral, PublicKeyToken=1e3754866626dfbf', XMLType('
	<ChemBioViz>
		<applicationSettings name="General">
			<groups>
				<remove />
				<add name="GeneralSettings" title="CBV General Settings" description="General Configuration Settings for CBV">
					<settings>
                        <add name="NotebookRefAsDropdown" value="False" controlType="TEXT" description="Defines Notebook Reference field control type. Allowed values [true|false]." isAdmin="False"/>
					</settings>
				</add>
                <add name="MISC" title="CBV Misc Settings" description="Miscellaneous Configuration Settings for CBV">
					<settings>
                        <add name="ENABLE_COEFORM_DEBUG_INFO" value="false" description="Enables/Disables debug information shown on COEForms"/>
					</settings>
				</add>
			</groups>
		</applicationSettings>
	</ChemBioViz>
	'));
	
	COMMIT;    
END;
/



