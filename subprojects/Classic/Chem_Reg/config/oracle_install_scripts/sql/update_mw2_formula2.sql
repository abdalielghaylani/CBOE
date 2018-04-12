prompt 'updating mw2 and formula2...'

Connect &&schemaName/&&schemaPass@&&serverName

update compound_molecule m set m.mw2 = (select molweight FROM Structures s,CSCARTRIDGE.REGDB_MX_W w where s.rowid=w.rid and s.cpd_internal_id= m.cpd_database_counter), m.formula2 =(select formula FROM Structures s,CSCARTRIDGE.REGDB_MX_A A where s.rowid=A.rid and s.cpd_internal_id= m.cpd_database_counter);

update temporary_structures m set m.mw2 = (select molweight FROM temporary_structures s,CSCARTRIDGE.REGDB_MX2_W w where s.rowid=w.rid), m.formula2 =(select formula FROM temporary_structures s,CSCARTRIDGE.REGDB_MX2_A A where s.rowid=A.rid);

commit;


