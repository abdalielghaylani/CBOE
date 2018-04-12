-- Add columns to table:

/* Add a new column ACTIVE,SORTORDER to INV_UNITS*/

Alter table INV_UNITS
add(
"ACTIVE" NCHAR(1) DEFAULT 'T' ,
"SORTORDER" NUMBER DEFAULT '1' 
);





