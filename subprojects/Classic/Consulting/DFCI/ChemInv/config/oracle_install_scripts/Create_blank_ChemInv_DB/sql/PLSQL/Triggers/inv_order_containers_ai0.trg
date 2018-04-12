CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_ORD_CNTNRS_AI0
  AFTER INSERT
  ON inv_order_containers
  FOR EACH ROW
DECLARE
  raid number(10);
BEGIN
 	SELECT seq_audit.NEXTVAL INTO raid FROM dual;
  audit_trail.record_transaction
    (raid, 'INV_ORDER_CONTAINERS', :NEW.rid, 'I');
END;
/