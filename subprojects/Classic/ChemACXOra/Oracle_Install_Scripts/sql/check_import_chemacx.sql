Prompt Substance row count should be 352,353
select count(*) from chemacxdb.substance;

Prompt CSCartridge.chemacxdb_mx row count should be 300,935
SELECT COUNT(*) FROM CSCARTRIDGE.chemacxdb_MX;

Prompt CSCartridge.chemacxdb_mx_nr row count should be 18,153,028
SELECT COUNT(*) FROM CSCARTRIDGE.chemacxdb_MX_NR;

Prompt CHEMACXDB constraint count should be 73
select count(constraint_name) from all_constraints where owner='CHEMACXDB';

Prompt CHEMACXDB index count should be 43
select count(index_name) from all_indexes where table_owner='CHEMACXDB';