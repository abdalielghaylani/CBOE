-- docmgr_documents_au0.trg
-- Script to create AFTER-UPDATE audit trigger for the 
-- changed docmgr_documents fields.

CREATE OR REPLACE TRIGGER TRG_AUDIT_DMDOC_AU0 
AFTER UPDATE OF 
    DOCID, DOCLOCATION, DOCNAME, DOCSIZE, DOCTYPE, TITLE,
    AUTHOR, SUBMITTER, SUBMITTER_COMMENTS, DATE_SUBMITTED
ON DOCMGR_DOCUMENTS FOR EACH ROW 
DECLARE
  raid number(10);
BEGIN
  SELECT seq_audit.NEXTVAL INTO raid FROM dual;

  audit_trail.record_transaction
    (raid, 'DOCMGR_DOCUMENTS', :old.rid, 'U');

  IF UPDATING('DOCID') THEN
    IF NVL(:old.DOCID,' ') != NVL(:new.DOCID,' ') THEN
       audit_trail.column_update
         (raid, 'DOCID',
         :old.DOCID, :new.DOCID);
    END IF;
  END IF;

  IF UPDATING('DOCLOCATION') THEN
    IF NVL(:old.DOCLOCATION,' ') != NVL(:new.DOCLOCATION,' ') THEN
       audit_trail.column_update
         (raid, 'DOCLOCATION',
         :old.DOCLOCATION, :new.DOCLOCATION);
    END IF;
  END IF;

  IF UPDATING('DOCNAME') THEN
    IF NVL(:old.DOCNAME,0) != NVL(:new.DOCNAME,0) THEN
       audit_trail.column_update
         (raid, 'DOCNAME',
         :old.DOCNAME, :new.DOCNAME);
    END IF;
  END IF;

  IF UPDATING('DOCSIZE') THEN
    IF NVL(:old.DOCSIZE,' ') != NVL(:new.DOCSIZE,' ') THEN
       audit_trail.column_update
         (raid, 'DOCSIZE',
         :old.DOCSIZE, :new.DOCSIZE);
    END IF;
  END IF;

  IF UPDATING('DOCTYPE') THEN
    IF NVL(:old.DOCTYPE,' ') != NVL(:new.DOCTYPE,' ') THEN
       audit_trail.column_update
         (raid, 'DOCTYPE',
         :old.DOCTYPE, :new.DOCTYPE);
    END IF;
  END IF;

  IF UPDATING('TITLE') THEN
    IF :old.TITLE != :new.TITLE THEN
       audit_trail.column_update
         (raid, 'TITLE',
         :old.TITLE, :new.TITLE);
    END IF;
  END IF;

  IF UPDATING('AUTHOR') THEN
    IF :old.AUTHOR != :new.AUTHOR THEN
       audit_trail.column_update
         (raid, 'AUTHOR',
         :old.AUTHOR, :new.AUTHOR);
    END IF;
  END IF;

  IF UPDATING('SUBMITTER') THEN
    IF :old.SUBMITTER != :new.SUBMITTER THEN
       audit_trail.column_update
         (raid, 'SUBMITTER',
         :old.SUBMITTER, :new.SUBMITTER);
    END IF;
  END IF;

  IF UPDATING('SUBMITTER_COMMENTS') THEN
    IF :old.SUBMITTER_COMMENTS != :new.SUBMITTER_COMMENTS THEN
       audit_trail.column_update
         (raid, 'SUBMITTER_COMMENTS',
         :old.DATE_SUBMITTED, :new.SUBMITTER_COMMENTS);
    END IF;
  END IF;

  IF UPDATING('DATE_SUBMITTED') THEN
    IF :old.DATE_SUBMITTED != :new.DATE_SUBMITTED THEN
       audit_trail.column_update
         (raid, 'DATE_SUBMITTED',
         :old.DATE_SUBMITTED, :new.DATE_SUBMITTED);
    END IF;
  END IF;

END;
/
