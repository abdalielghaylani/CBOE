Create index ix_icb_bf1 on INV_CONTAINER_BATCHES(BATCH_FIELD_1);
Create index ix_icb_bf2 on INV_CONTAINER_BATCHES(BATCH_FIELD_2);
Create index ix_icb_bf3 on INV_CONTAINER_BATCHES(BATCH_FIELD_3);
Create index ix_icb_btype on INV_CONTAINER_BATCHES(BATCH_TYPE);

create index ix_ireq_btypefk on INV_REQUESTS(BATCH_TYPE_ID_FK);
create index ix_ireq_requnit on INV_REQUESTS(REQUIRED_UNIT_ID_FK);
create index ix_ireq_delby on INV_REQUESTS(DELIVERED_BY_ID_FK);
create index ix_ireq_ctype on INV_REQUESTS(CONTAINER_TYPE_ID_FK);
create index ix_ireq_orguid on INV_REQUESTS(ORG_UNIT_ID_FK);
create index ix_ireq_times on INV_REQUESTS(TIMESTAMP);

create index ix_icontbid2 on INV_CONTAINERS(BATCH_ID2_FK);
create index ix_icontbid3 on INV_CONTAINERS(BATCH_ID3_FK);