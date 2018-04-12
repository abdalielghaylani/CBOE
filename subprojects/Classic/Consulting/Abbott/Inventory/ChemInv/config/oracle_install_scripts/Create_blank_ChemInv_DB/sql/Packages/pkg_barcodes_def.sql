CREATE OR REPLACE PACKAGE              "BARCODES"
	AS
	
   FUNCTION BARCODESEQUENCEEXISTS(pBarcodeTypeID IN inv_barcode_desc.barcode_desc_id%Type)
  		RETURN boolean; 
   
   FUNCTION GETNEXTBARCODESEQVALUE(pBarcodeTypeID IN inv_barcode_desc.barcode_desc_id%Type)
   	RETURN integer;	
   
   PROCEDURE CREATEBARCODESEQUENCE(	pBarcodeTypeID IN inv_barcode_desc.barcode_desc_id%Type, 
   									pSeqStart IN inv_barcode_desc.run_start%Type, 
   									pSeqEnd IN inv_barcode_desc.run_end%Type, 
   									pSeqIncrement IN inv_barcode_desc.run_increment%Type);
   	
   FUNCTION GETNEXTBARCODE
      (pBarcodeTypeID IN inv_barcode_desc.barcode_desc_id%Type
	  )
	     RETURN varChar2;
   
   		  
	 
END BARCODES;
/
show errors;