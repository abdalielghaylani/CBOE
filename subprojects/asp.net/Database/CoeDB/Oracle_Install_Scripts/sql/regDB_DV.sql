
Connect &&schemaName/&&schemaPass@&&serverName;


Insert into coedb.coedataview (ID,	NAME,	DESCRIPTION,	USER_ID,	IS_PUBLIC,	FORMGROUP,	DATE_CREATED,	DATABASE, COEDATAVIEW	)values(4000,'RegDB','','COEDB',1,0,sysdate,'REGDB',
'<?xml version="1.0" encoding="utf-8" ?>
<COEDataView xmlns="COE.COEDataView" basetable="1" database="REGDB" dataviewid="4000">
	<!-- The list of tables -->
	<tables>
		<!-- Aliased table -->
		<table id="1" database="REGDB" alias="rb" name="VW_REG_BATCHES"  primaryKey="1">
			<fields id="1" name="BATCHID" dataType="INTEGER"/>
			<fields id="2" name="REGID" dataType="INTEGER"/>
			<fields id="3" name="REGNUMBER" dataType="TEXT"/>
			<fields id="4" name="BATCHNUMBER" dataType="INTEGER"/>
			<fields id="5" name="FULL_REG_NUMBER" dataType="TEXT"/>
			<fields id="6" name="AGGREGATESTRUCTURE" dataType="TEXT"/>
		</table>
                                <!-- a table from a different schema -->
		<table id="2" database="BIODMDB" name="TESTASSAY1" primaryKey="10">
			<fields id="10" name="ID" dataType="INTEGER"/>
			<fields id="11" name="VALIDATION" dataType="INTEGER"/>
			<fields id="12" name="ASSAY_DATE" dataType="DATE"/>
			<fields id="13" name="ASSAY_NOTEBOOK" dataType="TEXT"/>
                                                <fields id="14" name="RESULTS_COMMENTS" dataType="TEXT"/>
                                                <fields id="15" name="IC50_NM" dataType="INTEGER"/>
                                                <fields id="16" name="OUTSIDE_RANGE" dataType="INTEGER"/>
                                                <fields id="17" name="CONTROL_1_AR_NUMBER" dataType="TEXT"/>
                                                <fields id="18" name="CONTROL_1_IC50_NM" dataType="INTEGER"/>
                                                <fields id="19" name="MIN__POC" dataType="INTEGER"/>
                                                <fields id="20" name="COMPOUND_CONC__NM" dataType="INTEGER"/>
                                                <fields id="21" name="KI_NM" dataType="INTEGER"/>
                                                <fields id="23" name="Z_PRIME" dataType="INTEGER"/>
                                                <fields id="24" name="SDIVN" dataType="INTEGER"/>
                                                <fields id="25" name="CONTROL_HILL_SLOPE" dataType="INTEGER"/>
                                                <fields id="26" name="ASSAY_IDENTIFIER" dataType="INTEGER"/>
                                                <fields id="27" name="REQUESTOR" dataType="TEXT"/>
                                                <fields id="28" name="PROJECT" dataType="INTEGER"/>
                                                <fields id="29" name="SUBMISSION_COMMENTS" dataType="TEXT"/>
                                                <fields id="30" name="BARCODE" dataType="INTEGER"/>
                                                <fields id="31" name="SUBMISSION_VALIDATED" dataType="INTEGER"/>
                                                <fields id="32" name="REG_BATCH_NUMBER" dataType="TEXT"/>
			</table>
	</tables>
	<!-- The following is the list of relations -->
	<relationships>
		<relationship child="2" parent="1" childkey="32" parentkey="5" jointype="INNER" />
	</relationships>
</COEDataView>');


CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

grant select on REGDB.VW_REG_BATCHES TO COEDB;
grant select on BIODMDB.TESTASSAY1 TO COEDB;
