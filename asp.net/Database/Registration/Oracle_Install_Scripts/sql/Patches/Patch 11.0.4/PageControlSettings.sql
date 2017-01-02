--Copyright 1999-2011 Perkin Elmer. All rights reserved.

--#####################################
-- Updating Registration Page Control Settings xml
--#####################################

/* Add page control settings for Structure Identifiers of SubmitMixture form */
update &&securitySchemaName..coepagecontrol set configurationxml=insertChildXml(
  xmltype(ConfigurationXml),
  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX"]/ControlSettings',
  'ControlSetting',
  xmltype('
				<ControlSetting>
					<ID>Structure Identifiers</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>Structure_IdentifiersUltraGrid</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1</COEFormID>
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
				</ControlSetting>')
  ).getclobval()
  where application='REGISTRATION' and type='CUSTOM';
COMMIT;

/* Add page control settings for Structure Identifiers of ViewMixture form */
update &&securitySchemaName..coepagecontrol set configurationxml=insertChildXml(
  xmltype(ConfigurationXml),
  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
  'ControlSetting',
  xmltype('
				<ControlSetting>
					<ID>Structure Identifiers</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>Structure_IdentifiersUltraGrid</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1</COEFormID>
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
				</ControlSetting>')
  ).getclobval()
  where application='REGISTRATION' and type='CUSTOM';
COMMIT;

/* Add page control settings for Component Identifiers of ViewMixture form */
update &&securitySchemaName..coepagecontrol set configurationxml=insertChildXml(
  xmltype(ConfigurationXml),
  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX"]/ControlSettings',
  'ControlSetting',
  xmltype('
				<ControlSetting>
					<ID>Component Identifiers</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>Compound_IdentifiersUltraGrid</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1001</COEFormID>
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
				</ControlSetting>')
  ).getclobval()
  where application='REGISTRATION' and type='CUSTOM';
COMMIT;

/* Add page control settings for Structure Identifiers of ComponentDuplicate form */
update &&securitySchemaName..coepagecontrol set configurationxml=insertChildXml(
  xmltype(ConfigurationXml),
  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX"]/ControlSettings',
  'ControlSetting',
  xmltype('
				<ControlSetting>
					<ID>Structure Identifiers</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>Structure_IdentifiersUltraGrid</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1</COEFormID>
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
				</ControlSetting>')
  ).getclobval()
  where application='REGISTRATION' and type='CUSTOM';
COMMIT;

/* Add page control settings for Structure Identifiers of ReviewRegister form */
update &&securitySchemaName..coepagecontrol set configurationxml=insertChildXml(
  xmltype(ConfigurationXml),
  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings',
  'ControlSetting',
  xmltype('
				<ControlSetting>
					<ID>Structure Identifiers</ID>
					<Privileges>
						<Operator>AND</Operator>
						<ID/>
					</Privileges>
					<Controls>
						<Control>
							<ID>Structure_IdentifiersUltraGrid</ID>
							<TypeOfControl>COEGenerableControl</TypeOfControl>
							<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
							<COEFormID>1</COEFormID>
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
				</ControlSetting>')
  ).getclobval()
  where application='REGISTRATION' and type='CUSTOM';
COMMIT;

/* Update page control settings for fragments form */
update &&securitySchemaName..coepagecontrol set configurationxml=deleteXML(
xmltype(ConfigurationXml),
'/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX"]/ControlSettings/ControlSetting[ID="ComponentsFragmentsGrid"]'
).getclobval()
where application='REGISTRATION' and type='CUSTOM';

update &&securitySchemaName..coepagecontrol set configurationxml=insertChildXml(
  xmltype(ConfigurationXml),
  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX"]/ControlSettings',
  'ControlSetting',
 xmltype('<ControlSetting>
          <ID>ComponentsFragmentsGrid</ID>
          <Privileges>
            <Operator>AND</Operator>
            <ID/>
          </Privileges>
          <Controls>
            <Control>
              <ID>3</ID>
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
        </ControlSetting>')
).getclobval()
where application='REGISTRATION' and type='CUSTOM';
