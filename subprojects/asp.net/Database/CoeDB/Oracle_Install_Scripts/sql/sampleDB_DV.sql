
Connect &&schemaName/&&schemaPass@&&serverName;

Insert into coedb.coedataview (ID,	NAME,	DESCRIPTION,	USER_ID,	IS_PUBLIC,	FORMGROUP,	DATE_CREATED,	DATABASE, COEDATAVIEW	)values(1000,'Sample','','COEDB',1,0,sysdate,'SAMPLE',
'<?xml version="1.0" encoding="utf-8" ?>
<COEDataView xmlns="COE.COEDataView" basetable="1" dataviewid="1000">
	<tables>
		<table id="1" database="sample" alias="s" name="moltable"  primaryKey="1">
			<fields id="1" name="ID" dataType="INTEGER"/>
			<fields id="2" name="MOLNAME" dataType="TEXT"/>
			<fields id="3" name="BASE64_CDX" dataType="TEXT" indexType="CS_CARTRIDGE"/>
		</table>
		<table id="2" database="sample" alias="y" name="synonyms_r"  primaryKey="4">
			<fields id="4" name="ID" dataType="INTEGER"/>
			<fields id="5" name="SYN_ID" dataType="INTEGER"/>
			<fields id="6" name="SYNONYM_R" dataType="TEXT"/>
		</table>
	</tables>
	<relationships>
    <relationship child="2" parent="1" childkey="5" parentkey="1" jointype="INNER" /></relationships>
</COEDataView>');

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

grant select on SAMPLE.MOLTABLE TO COEDB;
grant select on SAMPLE.SYNONYMS_R TO COEDB;

