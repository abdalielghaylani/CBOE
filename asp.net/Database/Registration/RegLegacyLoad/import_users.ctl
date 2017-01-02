OPTIONS (skip=1)
LOAD DATA
INFILE 'data\users.txt'
BADFILE 'data\users.bad'
DISCARDFILE 'data\users.dsc'


INTO TABLE regdb.legacy_users
INSERT
FIELDS TERMINATED BY '	' trailing nullcols 
(PersonID,
UserName,
UserCode,
Active,
Role,
FirstName,
MiddleName,
LastName,
SupervisorID,
Title,
Department,
SiteID,   
Address,
Telephone,
Email)
