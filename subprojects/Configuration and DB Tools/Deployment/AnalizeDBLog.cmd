echo %1 Oracle database Log Summary: > SummaryLog_%1
echo ------- >> SummaryLog_%1
find /C "ERROR" %2 | ..\Deployment\sed "s/---------- %2/Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "SP2-0310" %2 | ..\Deployment\sed "s/---------- %2/Error in opening files/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-01430" %2 | ..\Deployment\sed "s/---------- %2/Column already exists IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-00955" %2 | ..\Deployment\sed "s/---------- %2/Name already used IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-04080" %2 | ..\Deployment\sed "s/---------- %2/Drop nonexisting trigger IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-00001" %2 | ..\Deployment\sed "s/---------- %2/Unique constraint violated IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-02275" %2 | ..\Deployment\sed "s/---------- %2/Existing referential constraint IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-01418" %2 | ..\Deployment\sed "s/---------- %2/Drop nonexisting index IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-12002" %2 | ..\Deployment\sed "s/---------- %2/Drop nonexisting materialized view log IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-12003" %2 | ..\Deployment\sed "s/---------- %2/Drop nonexisting materialized view IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-01451" %2 | ..\Deployment\sed "s/---------- %2/Column not nullable IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-02443" %2 | ..\Deployment\sed "s/---------- %2/Drop nonexisting constraint IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-02264" %2 | ..\Deployment\sed "s/---------- %2/Name used by existing constraint IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-02289" %2 | ..\Deployment\sed "s/---------- %2/Drop nonexisting sequence IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-00942" %2 | ..\Deployment\sed "s/---------- %2/Drop nonexisting table IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-01432" %2 | ..\Deployment\sed "s/---------- %2/Drop nonexisting public synonym IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-29863: warning" %2 | ..\Deployment\sed "s/---------- %2/CS Cartridge Warning IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-02431" %2 | ..\Deployment\sed "s/---------- %2/Cannot disable non-existing constraint IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "ORA-01408" %2 | ..\Deployment\sed "s/---------- %2/Such column list already indexed  IGNORABLE Errors/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "PL/SQL procedure successfully completed." %2 | ..\Deployment\sed "s/---------- %2/PL-SQL procedures completed/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "User created." %2 | ..\Deployment\sed "s/---------- %2/Users created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Grant succeeded." %2 | ..\Deployment\sed "s/---------- %2/Grants succeeded/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Table created." %2 | ..\Deployment\sed "s/---------- %2/Tables created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "View created." %2 | ..\Deployment\sed "s/---------- %2/Views created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Materialized view created." %2 | ..\Deployment\sed "s/---------- %2/Materialized Views created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Synonym created." %2 | ..\Deployment\sed "s/---------- %2/Synonyms created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Trigger created." %2 | ..\Deployment\sed "s/---------- %2/Triggers created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Index created." %2 | ..\Deployment\sed "s/---------- %2/Indexes created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Table altered." %2 | ..\Deployment\sed "s/---------- %2/Table alterations/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Index altered." %2 | ..\Deployment\sed "s/---------- %2/Indexes altered/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Procedure created." %2 | ..\Deployment\sed "s/---------- %2/Procedures created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Function created." %2 | ..\Deployment\sed "s/---------- %2/Functions created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Package created." %2 | ..\Deployment\sed "s/---------- %2/Packages created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Package body created." %2 | ..\Deployment\sed "s/---------- %2/Package bodies created/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "row created." %2 | ..\Deployment\sed "s/---------- %2/Rows added individually/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "rows created." %2 | ..\Deployment\sed "s/---------- %2/Groups of rows added/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Call completed." %2 | ..\Deployment\sed "s/---------- %2/Procedure calls completed/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
find /C "Commit complete." %2 | ..\Deployment\sed "s/---------- %2/Commits completed/g" >> SummaryLog_%1
echo . >> SummaryLog_%1
echo ------- >> SummaryLog_%1
echo For more details inspect the following log file: %2 >> SummaryLog_%1
echo accessible via Start:All Programs:ChemBioOffice Enterprise:Setup Logs >> SummaryLog_%1
