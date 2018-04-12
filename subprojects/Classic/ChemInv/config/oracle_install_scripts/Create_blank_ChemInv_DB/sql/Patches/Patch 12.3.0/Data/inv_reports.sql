INSERT INTO &&schemaName.."INV_REPORTS" (REPORTDISPLAYNAME, REPORTNAME, REPORTTYPE_ID) VALUES ('Request Label', 'rptRequestLabels', 
(select REPORTTYPE_ID from INV_REPORTTYPES where REPORTTYPEDESC= 'Request Label' ));

INSERT INTO &&schemaName.."INV_REPORTS" (REPORTDISPLAYNAME,REPORTNAME,QUERYNAME,REPORTTYPE_ID) 
	VALUES ('Sample Requests Report', 'rptSamplesRequestReport', 'qrySamplesRequestReport', (SELECT REPORTTYPE_ID FROM inv_reporttypes WHERE REPORTTYPEDESC='Sample Requests Report'));
