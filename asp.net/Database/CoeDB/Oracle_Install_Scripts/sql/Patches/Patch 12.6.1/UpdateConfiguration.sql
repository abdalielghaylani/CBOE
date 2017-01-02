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
					<add name="AppPageTitle" value="CambridgeSoft COEManager Enterprise 12.0"/>
				</settings>
			</add>
        </groups>
    </applicationSettings>
</Manager>';
    &&schemaName..ConfigurationManager.UpdateConfiguration ('Manager','CambridgeSoft.COE.Framework.Common.ApplicationDataConfigurationSection, CambridgeSoft.COE.Framework, Version=12.1.0.0, Culture=neutral, PublicKeyToken=1e3754866626dfbf',AConfiguration);
    COMMIT;
END;
/
