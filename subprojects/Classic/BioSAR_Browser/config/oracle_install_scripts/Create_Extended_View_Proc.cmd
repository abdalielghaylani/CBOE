ECHO OFF
TITLE Create Extended View Procedure
CLS
ECHO You are about to create or replace an stored procedure 
ECHO in the BioAssayHTS Schema.
ECHO The procedure dynamically creates extended views when 
ECHO assay tables are exposed to BioSAR.
ECHO This procedure grants read access on the reg_numbers and 
ECHO batches tables to BioAssayHTS schema owner.
ECHO To abort close this window or press Ctrl-C.
Pause
cd sql
     
sqlplus /NOLOG  @create_extendedview_proc.sql
notepad log_create_extended_view_proc.txt
