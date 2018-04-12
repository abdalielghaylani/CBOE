CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_SUPPLIERS_AI0
  AFTER INSERT
  ON inv_suppliers
  FOR EACH ROW
DECLARE
  raid number(10);
BEGIN
 	SELECT seq_audit.NEXTVAL INTO raid FROM dual;
  audit_trail.record_transaction
    (raid, 'INV_SUPPLIERS', :NEW.rid, 'I');
END;
/