--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved


--#####################################
-- Updating Registration Configuration
--#####################################
DECLARE
	v_found number(1) := 0;
BEGIN 
	SELECT COUNT(1) INTO v_found FROM &&securitySchemaName..COECONFIGURATION WHERE LOWER(DESCRIPTION)='registration';
	IF v_found > 0 THEN
		--&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','ActiveRLS',NULL ,'This setting enables Row Level Security which is an additional layer of security utilizing Oracle''s fine grain access control functionality.', 'PICKLIST','Off|Registry Level Projects|Batch Level Projects', 'False','CambridgeSoft.COE.RegistrationAdmin.Services.RLSConfigurationProcessor','True');
		
		&&securitySchemaName..ConfigurationManager.CreateOrUpdateParameter('Registration','REGADMIN','AllowCreateCopySharedStructure','True', 'Allow to create copies of edited linked structures', 'PICKLIST','True|False', 'False');

		UPDATE &&securitySchemaName..coeconfiguration c
			SET c.configurationxml = (
				SELECT  XmlType(replace( cr.configurationxml.GetClobVal(), '11.0.3', '11.0.4' ))
			FROM &&securitySchemaName..coeconfiguration cr
				WHERE cr.Description = c.Description )
			WHERE c.Description = 'Registration';

		COMMIT;
	END IF;
END;
/


--####################################
-- Update New Application Column in COEDATAVIEW table
--####################################

--All Registration dataviews are in this range
Update &&securitySchemaName..coedataview Set Application = 'REGISTRATION' where ID >=4000 and ID < 5000;
commit;

--This dataview is currently only used by REGISTRATION for DocManager intgration
update &&securitySchemaName..coedataview Set Application = 'REGISTRATION' where ID =8001;
commit;


--####################################
-- Page Control Settings required XMLs
--####################################

-- update settings for component identifier because of COEForm reorganization
--SubmitMixture page
UPDATE &&securitySchemaName..COEPAGECONTROL 
	SET configurationxml=UPDATEXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX"]/ControlSettings/ControlSetting[ID="Component Identifiers"]/Controls',
		  XMLTYPE('
		<Controls>
			<Control>
				<ID>Compound_IdentifiersUltraGrid</ID>
				<TypeOfControl>COEGenerableControl</TypeOfControl>
				<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
				<COEFormID>1001</COEFormID>
			</Control>
		</Controls>')).getClobVal()
	  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';
--ReviewRegisterMixture page
UPDATE &&securitySchemaName..COEPAGECONTROL 
	SET configurationxml=UPDATEXML(
		  xmltype(configurationxml),
		  '/COEPageControlSettings/Pages/Page[ID="ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX"]/ControlSettings/ControlSetting[ID="Component Identifiers"]/Controls',
		  XMLTYPE('
		  <Controls>
			<Control>
				<ID>Compound_IdentifiersUltraGrid</ID>
				<TypeOfControl>COEGenerableControl</TypeOfControl>
				<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
				<COEFormID>1001</COEFormID>
			</Control>
		</Controls>')).getClobVal()
	  where APPLICATION='REGISTRATION' AND TYPE='CUSTOM';


--#####################################
-- Updating Registration Object Config.
--#####################################

-- Add PropertyList on Structure level
Declare
	XmlTypeConfig								XmlType;
	DOMDocumentConfig				DBMS_XMLDom.DOMDocument;
	DOMNodeConfig						DBMS_XMLDom.DOMNode;
	DOMNodeStructure					DBMS_XMLDom.DOMNode;
	DOMELEMENTPropertyList		DBMS_XMLDom.DOMELEMENT;
	DOMNODEPropertyList				DBMS_XMLDom.DOMNODE;
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
		DOMNodeStructure := dbms_xslprocessor.selectSingleNode(DOMNodeConfig,'MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure');
	  
		-- Create new PropertyList node
		DOMELEMENTPropertyList := DBMS_XMLDOM.CREATEELEMENT(DOMDocumentConfig,'PropertyList','');
		DOMNODEPropertyList := DBMS_XMLDOM.MAKENODE(DOMELEMENTPropertyList);
	  
		-- Append PropertyList node to Structure node
		DOMNODEPropertyList	 := DBMS_XMLDOM.APPENDCHILD(DOMNodeStructure,DOMNODEPropertyList);

		-- Save updated xml to COEOBJECDTCONFIG
		UPDATE &&schemaName..COEOBJECTCONFIG
			SET XML = XmlTypeConfig.GetClobVal()	WHERE ID = 2;
	  
		COMMIT;
	END IF;
End;
/
