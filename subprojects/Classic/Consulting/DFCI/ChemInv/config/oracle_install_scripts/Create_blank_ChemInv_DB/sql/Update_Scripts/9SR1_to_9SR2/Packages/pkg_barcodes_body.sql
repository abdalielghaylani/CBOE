CREATE OR REPLACE PACKAGE BODY "BARCODES" IS

	FUNCTION BARCODESEQUENCEEXISTS(p_barcodeTypeID inv_barcode_desc.barcode_desc_id%TYPE)
		RETURN BOOLEAN AS
		l_sequenceName inv_barcode_desc.barcode_desc_name%TYPE;
	BEGIN
		SELECT sequence_name
			INTO l_sequenceName
			FROM user_sequences
		 WHERE sequence_name = 'SEQ_BARCODE_TYPE_' || p_barcodeTypeID;
		RETURN TRUE;
	EXCEPTION
		WHEN no_data_found THEN
			RETURN FALSE;
	END BARCODESEQUENCEEXISTS;

	FUNCTION GETNEXTBARCODESEQVALUE(p_barcodeTypeID IN inv_barcode_desc.barcode_desc_id%TYPE)
		RETURN INTEGER AS
		l_out INTEGER;
		l_sql VARCHAR2(2000);
	BEGIN
		l_sql := 'SELECT SEQ_BARCODE_TYPE_' || p_barcodeTypeID ||
						 '.nextVal from dual';
		EXECUTE IMMEDIATE l_sql
			INTO l_out;
	
		RETURN l_out;
	END GETNEXTBARCODESEQVALUE;

	PROCEDURE CREATEBARCODESEQUENCE(p_barcodeTypeID IN inv_barcode_desc.barcode_desc_id%TYPE,
																	p_seqStart      IN inv_barcode_desc.run_start%TYPE,
																	p_seqEnd        IN inv_barcode_desc.run_end%TYPE,
																	p_seqIncrement  IN inv_barcode_desc.run_increment%TYPE) AS
		l_sql VARCHAR2(2000);
	BEGIN
		l_sql := 'CREATE SEQUENCE SEQ_BARCODE_TYPE_' || p_barcodeTypeID;
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
	
	END CREATEBARCODESEQUENCE;

	FUNCTION GETNEXTBARCODE(p_barcodeTypeID IN inv_barcode_desc.barcode_desc_id%TYPE)
		RETURN VARCHAR2 AS
		l_barcodeTypeID  INTEGER;
		l_prefix         VARCHAR2(10);
		l_suffix         VARCHAR2(10);
		l_pfxSeparator   VARCHAR2(3);
		l_sfxSeparator   VARCHAR2(3);
		l_seqStart       INTEGER;
		l_seqEnd         INTEGER;
		l_seqIncrement   INTEGER;
		l_paddedLength   INTEGER;
		l_padChar        CHAR(1);
		l_nextNumber     INTEGER;
		l_out            VARCHAR2(100);
		l_len            INTEGER;
		l_plateCount     INTEGER;
		l_containerCount INTEGER;
		l_isUnique       BOOLEAN := FALSE;
	BEGIN
		SELECT barcode_desc_id,
					 PREFIX,
					 SUFFIX,
					 PFX_SEPARATOR,
					 SFX_SEPARATOR,
					 run_start,
					 run_end,
					 run_increment,
					 NVL(number_size, 0),
					 NVL(PAD_CHARACTER, '0')
			INTO l_barcodeTypeID,
					 l_prefix,
					 l_suffix,
					 l_pfxSeparator,
					 l_sfxSeparator,
					 l_seqStart,
					 l_seqEnd,
					 l_seqIncrement,
					 l_paddedLength,
					 l_padChar
			FROM inv_barcode_desc
		 WHERE barcode_desc_id = p_barcodeTypeID;
	
		IF NOT BARCODESEQUENCEEXISTS(p_barcodeTypeID) THEN
			CREATEBARCODESEQUENCE(p_barcodeTypeID,
														l_seqStart,
														l_seqEnd,
														l_seqIncrement);
		END IF;
	
		WHILE NOT l_isUnique
		LOOP
			l_nextNumber := GETNEXTBARCODESEQVALUE(p_barcodeTypeID);
			l_len        := length(l_nextNumber);
			IF l_paddedLength < l_len THEN
				l_paddedLength := l_len;
			END IF;
		
			l_out := l_prefix || l_pfxSeparator ||
							 LPad(l_nextNumber, l_paddedLength, l_padChar) ||
							 l_sfxSeparator || l_suffix;
			SELECT COUNT(*)
				INTO l_plateCount
				FROM inv_plates
			 WHERE plate_barcode = l_out;
			SELECT COUNT(*)
				INTO l_containerCount
				FROM inv_containers
			 WHERE barcode = l_out;
			IF (l_plateCount + l_containerCount) = 0 THEN
				l_isUnique := TRUE;
			END IF;
		END LOOP;
		RETURN l_out;
	END GETNEXTBARCODE;

END BARCODES;
/
show errors;