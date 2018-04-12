
DECLARE
	PROCEDURE CreateSequenceAndTrigger(p_tableName    IN VARCHAR2,
																		 p_PK           VARCHAR2,
																		 p_seqStart     IN NUMBER,
																		 p_seqEnd       IN NUMBER,
																		 p_seqIncrement IN NUMBER) IS
		l_seqName VARCHAR2(30);
		FUNCTION CreateSequence(p_tableName    IN VARCHAR2,
														p_seqStart     IN NUMBER,
														p_seqEnd       IN NUMBER,
														p_seqIncrement IN NUMBER) RETURN VARCHAR2 AS
			l_sql          VARCHAR2(2000);
			l_sequenceName VARCHAR2(30);
			l_count        NUMBER;
		BEGIN
			l_sequenceName := 'SEQ_' || p_tableName;
		
			--' check for existing sequence 
			SELECT COUNT(sequence_name)
				INTO l_count
				FROM user_sequences
			 WHERE sequence_name = l_sequenceName;
		
			--' if a sequence does NOT exist create it, otherwise do nothing
			IF l_count = 0 THEN
				l_sql := 'CREATE SEQUENCE ' || l_sequenceName;
				IF p_seqStart IS NOT NULL THEN
					l_sql := l_sql || ' START WITH ' || p_seqStart;
				END IF;
				IF p_seqEnd IS NOT NULL THEN
					l_sql := l_sql || ' MAXVALUE ' || p_seqEnd;
				END IF;
				IF p_seqIncrement IS NOT NULL THEN
					l_sql := l_sql || ' INCREMENT BY ' || p_seqIncrement;
				END IF;
				EXECUTE IMMEDIATE l_sql;
			END IF;
		
			RETURN l_sequenceName;
		
		END CreateSequence;
		
		PROCEDURE CreateTrigger(p_tableName IN VARCHAR2,
														p_seqName   IN VARCHAR2,
														p_PK        IN VARCHAR2) IS
			l_sql VARCHAR2(2000);
		BEGIN
			l_sql := 'CREATE OR REPLACE TRIGGER TRG_' || p_tableName;
			l_sql := l_sql || ' BEFORE INSERT ON ' || p_tableName;
			l_sql := l_sql || ' FOR EACH ROW';
			l_sql := l_sql || ' BEGIN';
			l_sql := l_sql || ' IF :new.' || p_PK || ' IS NULL THEN';
			l_sql := l_sql || ' SELECT ' || p_seqName || '.nextval INTO :new.' || p_PK ||
							 ' FROM DUAL;';
			l_sql := l_sql || ' END IF;';
			l_sql := l_sql || ' END;';
			EXECUTE IMMEDIATE l_sql;
		END CreateTrigger;
	BEGIN
		l_seqName := CreateSequence(p_tableName,
																p_seqStart,
																p_seqEnd,
																p_seqIncrement);
		CreateTrigger(p_tableName, l_seqName, p_PK);
	END CreateSequenceAndTrigger;
BEGIN
	CreateSequenceAndTrigger('INV_REQUESTS','REQUEST_ID',1,NULL,1);
END;