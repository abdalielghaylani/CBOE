OPTIONS (SKIP=1)
Load data
 infile 'UsersToImport.csv'
 into table IMPORTED_USERS
 fields terminated by "," optionally enclosed by '"'		  
 TRAILING NULLCOLS 
 ( UserName, LastName, FirstName, Department, Roles, Supervisor, Site, Active,  Address, Telephone, Email, UserCode )


