CREATE OR REPLACE PACKAGE "BARCODES"
	AS

	FUNCTION BARCODESEQUENCEEXISTS(p_barcodeTypeID inv_barcode_desc.barcode_desc_id%TYPE)
		RETURN BOOLEAN;

	FUNCTION GETNEXTBARCODESEQVALUE(p_barcodeTypeID IN inv_barcode_desc.barcode_desc_id%TYPE)
		RETURN INTEGER;

	PROCEDURE CREATEBARCODESEQUENCE(p_barcodeTypeID IN inv_barcode_desc.barcode_desc_id%TYPE,
																	p_seqStart      IN inv_barcode_desc.run_start%TYPE,
																	p_seqEnd        IN inv_barcode_desc.run_end%TYPE,
																	p_seqIncrement  IN inv_barcode_desc.run_increment%TYPE);

	FUNCTION GETNEXTBARCODE(p_barcodeTypeID IN inv_barcode_desc.barcode_desc_id%TYPE)
		RETURN VARCHAR2;



END BARCODES;
/
show errors;