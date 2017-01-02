BEGIN
 dbms_stats.gather_schema_stats(ownname => 'REGDB',
                               estimate_percent => dbms_stats.auto_sample_size,
                               method_opt => 'FOR ALL COLUMNS SIZE AUTO',
                               cascade => TRUE);
END;
/
BEGIN
 dbms_stats.gather_schema_stats(ownname => 'COEDB',
                               estimate_percent => dbms_stats.auto_sample_size,
                               method_opt => 'FOR ALL COLUMNS SIZE AUTO',
                               cascade => TRUE);
END;
/
BEGIN
 dbms_stats.gather_schema_stats(ownname => 'CSCARTRIDGE',
                               estimate_percent => dbms_stats.auto_sample_size,
                               method_opt => 'FOR ALL COLUMNS SIZE AUTO',
                               cascade => TRUE);
END;
/
