--1999-2010 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET verify off

--#########################################################
-- SCRIPT VARIABLES
--######################################################### 
DEFINE schemaName = REGDB
DEFINE schemaPass = ORACLE
DEFINE schemaVersion = 3.0
DEFINE securitySchemaName = COEDB
DEFINE securitySchemaPass = ORACLE
DEFINE cartSchemaName = CSCARTRIDGE
DEFINE cartSchemaPass = CSCARTRIDGE 

DEFINE pathToChemImp = chemimp.exe
DEFINE sdimptable = legacy_regcompounds_sdf
DEFINE fragmentssdimptable = legacy_fragments_sdf
DEFINE sdfile = data\&&regCompoundsSdfName
DEFINE fragmentssdfile = data\fragments.sdf
DEFINE loadinguser = CSSADMIN
DEFINE tmplog = log\tmp.log