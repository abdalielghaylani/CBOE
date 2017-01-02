
Connect &&schemaName/&&schemaPass@&&serverName;


Insert into coedb.coedataview (ID,	NAME,	DESCRIPTION,	USER_ID,	IS_PUBLIC,	FORMGROUP,	DATE_CREATED,	DATABASE, COEDATAVIEW	)values(2000,'ChemInventory','','COEDB',1,0,sysdate,'CHEMACXDB',
'<?xml version="1.0" encoding="utf-8" ?>
<COEDataView xmlns="COE.COEDataView" basetable="1" dataviewid="2000" database="CHEMACXDB">
  <tables>
    <table id="1" database="CHEMACXDB" name="SUBSTANCE"  primaryKey="1">
			<fields id="1" name="CSNUM" dataType="INTEGER"/>
			<fields id="2" name="CAS" dataType="TEXT"/>
			<fields id="3" name="ACX_ID" dataType="TEXT"/>
      <fields id="4" name="BASE64_CDX" dataType="TEXT" indexType="CS_CARTRIDGE"/>
			<fields id="5" name="SUPPLIERID" dataType="INTEGER"/>
      <fields id="6" name="SYNONYMID" dataType="INTEGER"/>
		</table>
		<table id="2" database="CHEMACXDB" alias="prd" name="PRODUCT" primaryKey="7">
			<fields id="7" name="PRODUCTID" dataType="INTEGER"/>
			<fields id="8" name="CSNUM" dataType="INTEGER"/>
			<fields id="9" name="PRODNAME" dataType="TEXT"/>
			<fields id="10" name="CATALOGNUM" dataType="TEXT"/>
		</table>
		<table id="3" database="CHEMACXDB" alias="pkg" name="PACKAGE" primaryKey="11">
			<fields id="11" name="PACKAGEID" dataType="INTEGER"/>
      <fields id="12" name="PRODUCTID" dataType="INTEGER"/>
			<fields id="13" name="SUPPLIERID" dataType="INTEGER"/>
			<fields id="14" name="SIZE" dataType="TEXT"/>
      <fields id="15" name="PRICE" dataType="REAL"/>
      <fields id="16" name="CSYMBOL" dataType="TEXT"/>
		</table>
    <table id="4" database="CHEMACXDB" alias="syn" name="ACX_SYNONYM" primaryKey="17">
			<fields id="17" name="SYNONYMID" dataType="INTEGER"/>
      <fields id="18" name="NAME" dataType="TEXT"/>
		</table>
	</tables>
	<relationships>
		<relationship child="2"
				  parent="1"
				  childkey="8"
				  parentkey="1"/>
		<relationship child="3"
				  parent="2"
				  childkey="12"
				  parentkey="7"/>
		<!-- commented out this relationship because of CSBR-85313
		<relationship child="4"
				  parent="1"
				  childkey="17"
				  parentkey="6"
				  jointype="OUTER" />
		-->
	</relationships>
</COEDataView>');


CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

grant select on CHEMACXDB.SUBSTANCE TO COEDB;
grant select on CHEMACXDB.PRODUCT TO COEDB;
grant select on CHEMACXDB.PACKAGE TO COEDB;
grant select on CHEMACXDB.ACX_SYNONYM TO COEDB;

