Alter table INV_CONTAINERS
add(
"BATCH_ID2_FK"        NUMBER,
    "BATCH_ID3_FK"        NUMBER);

Alter table INV_CONTAINERS 
modify BATCH_ID_FK number;