-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

spool log_enableSpotfireSettings.txt

@@prompts.sql
@@parameters.sql

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

--#########################################################
-- SCRIPT VARIABLES
--######################################################### 
UPDATE COECONFIGURATION SET configurationxml='<Manager>
  <applicationSettings name="Mgr Settings">
    <groups>
      <remove/>
      <add name="DVManager" title="DataViewManager settings" description="Set description">
        <settings>
          <add name="UnpublishableSchemas" value="CTXSYS|DBSNMP|DMSYS|EXFSYS|IX|MDSYS|OLAPSYS|ORDPLUGINS|ORDSYS|SI_INFORMTN_SCHEMA|SYSMAN|TSMSYS|WMSYS"/>
          <add name="Apply_Indexing" value="DISABLE" controlType="PICKLIST" allowedValues="ENABLE|DISABLE" description="Enable or disable the offer to index fields for default query field"/>
          <add name="PublishDataviewsToSpotfire" value="YES" controlType="PICKLIST" allowedValues="YES|NO" description="Publish Dataviews to Spotfire or not."/>
        </settings>
      </add>
      <remove/>
      <add name="MISC" title="Miscellaneous settings">
        <settings>
          <!-- ENABLE or DISABLE -->
          <add name="PageControlsManager" value="DISABLE" controlType="PICKLIST" allowedValues="ENABLE|DISABLE"/>
          <add name="HomeLinkURL" value="/coemanager/forms/public/contentarea/Home.aspx"/>
          <add name="AppPageTitle" value="CambridgeSoft COEManager Enterprise 12.0"/>
        </settings>
      </add>
    </groups>
  </applicationSettings>
</Manager>'
WHERE description='Manager';
COMMIT;

--
spool off
exit