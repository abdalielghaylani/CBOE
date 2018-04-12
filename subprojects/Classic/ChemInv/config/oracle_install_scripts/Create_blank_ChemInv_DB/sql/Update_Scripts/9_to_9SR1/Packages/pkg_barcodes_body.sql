CREATE OR REPLACE
PACKAGE BODY              "BARCODES"
   IS

   	FUNCTION BARCODESEQUENCEEXISTS(pBarcodeTypeID inv_barcode_desc.barcode_desc_id%type)
  		RETURN boolean AS
  		vSequenceName inv_barcode_desc.barcode_desc_name%type;
  	BEGIN
  	  	SELECT sequence_name into vSequenceName from user_sequences where sequence_name = 'SEQ_BARCODE_TYPE_' || pBarcodeTypeID;
  		RETURN true;
  	exception
  		when no_data_found then
  			RETURN false;
  	END BARCODESEQUENCEEXISTS;


  FUNCTION GETNEXTBARCODESEQVALUE(pBarcodeTypeID IN inv_barcode_desc.barcode_desc_id%Type)
   	RETURN integer AS
   	vOut integer;
   	vSQL varchar2(2000);
  BEGIN
  	vSQL :=  'SELECT SEQ_BARCODE_TYPE_' || pBarcodeTypeID || '.nextVal from dual';
  	Execute Immediate vSQL into vOut;

  	RETURN vOut;
  END GETNEXTBARCODESEQVALUE;

   PROCEDURE CREATEBARCODESEQUENCE(	pBarcodeTypeID IN inv_barcode_desc.barcode_desc_id%Type,
   									pSeqStart IN inv_barcode_desc.run_start%Type,
   									pSeqEnd IN inv_barcode_desc.run_end%Type,
   									pSeqIncrement IN inv_barcode_desc.run_increment%Type)
   AS
      vSQL varchar2(2000);
   BEGIN
      vSQL := 'CREATE SEQUENCE SEQ_BARCODE_TYPE_' || pBarcodeTypeID;
      if pSeqStart is not null then vSQL := vSQL || ' START WITH ' || pSeqStart; end if;
      if pSeqEnd is not null then vSQL := vSQL  || ' MAXVALUE ' || pSeqEnd; end if;
      if pSeqIncrement is not null then vSQL := vSQL || ' INCREMENT BY ' || pSeqIncrement; end if;
 		Execute Immediate vSQL;

   END CREATEBARCODESEQUENCE;


   FUNCTION GETNEXTBARCODE
      (pBarcodeTypeID IN inv_barcode_desc.barcode_desc_id%Type
	     )
	RETURN varchar2 AS
		vBarcodeTypeID integer;
		vPrefix varchar2(5);
		vSuffix varchar2(5);
		vPfxSeparator varchar2(3);
		vSfxSeparator varchar2(3);
		vSeqStart integer;
		vSeqEnd integer;
		vSeqIncrement integer;
		vPaddedLength integer;
		vPadChar char(1);
		vNextNumber integer;
		vOut varchar2(30);
		vLen integer;   
		vPlateCount integer;
		vContainerCount integer;           
		vIsUnique boolean := false;
	BEGIN
	     SELECT barcode_desc_id,
	     		PREFIX,
	     		SUFFIX,
	     		PFX_SEPARATOR,
	     		SFX_SEPARATOR,
	     		run_start,
	     		run_end,
	     		run_increment,
	     		NVL(number_size,0),
	     		NVL(PAD_CHARACTER, '0')
	     	INTO
	     		vBarcodeTypeID,
	     		vPrefix,
	     		vSuffix,
	     		vPfxSeparator,
	     		vSfxSeparator,
	     		vSeqStart,
	     		vSeqEnd,
	     		vSeqIncrement,
	     		vPaddedLength,
	     		vPadChar
	     FROM inv_barcode_desc
	     WHERE barcode_desc_id = pBarcodeTypeID;

	     if NOT BARCODESEQUENCEEXISTS(pBarcodeTypeID) then
	     	CREATEBARCODESEQUENCE(pBarcodeTypeID, vSeqStart, vSeqEnd, vSeqIncrement);
	     end if;
		
		WHILE NOT vIsUnique 
		LOOP		
	     	vNextNumber := GETNEXTBARCODESEQVALUE(pBarcodeTypeID);
	     	vLen := length(vNextNumber);
	     	if vPaddedLength < vLen then
	     		vPaddedLength := vLen;
	     	end if;

	    	vOut := vPrefix || vPfxSeparator || LPad(vNextNumber, vPaddedLength, vPadChar) || vSfxSeparator || vSuffix;
	    	SELECT count(*) INTO vPlateCount FROM inv_plates WHERE plate_barcode = vOut;
	    	SELECT count(*) INTO vContainerCount FROM inv_containers WHERE barcode = vOut;
	    	IF (vPlateCount + vContainerCount) = 0 THEN
	    		vIsUnique := true;
	    	END IF;
		END LOOP;
	RETURN vOut;
	END GETNEXTBARCODE;


END BARCODES;
/
show errors;