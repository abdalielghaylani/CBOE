--' Add dummy compound
INSERT INTO inv_compounds (compound_ID, Substance_Name) 
		VALUES (-1, 'Invalid Substance Assigned.  See Administrator.');
commit;
