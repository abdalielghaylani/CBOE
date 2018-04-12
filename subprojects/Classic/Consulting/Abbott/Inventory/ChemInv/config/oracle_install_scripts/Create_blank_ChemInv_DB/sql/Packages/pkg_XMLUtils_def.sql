CREATE OR REPLACE
PACKAGE XMLUtils
AS                 

FUNCTION transformXML(pXSLT IN CLOB, pDoc IN CLOB, pParser xmlparser.Parser, pEngine xslprocessor.Processor) RETURN CLOB;
FUNCTION transformXML(pXSLT IN CLOB, pDoc IN xmldom.DOMDocument, pParser IN xmlparser.Parser, pEngine IN xslprocessor.Processor) RETURN CLOB;
FUNCTION CLOB2DOC(pParser xmlparser.Parser,	pClob CLOB) RETURN xmldom.DOMDocument;
FUNCTION CLOB2NODE(pParser xmlparser.Parser,pClob CLOB) RETURN xmldom.DOMNode;
FUNCTION RemoveXMLElement(pParser xmlparser.Parser, pXML IN CLOB) RETURN CLOB;
FUNCTION MERGE_XML_CLOBS(pClob1 CLOB, pClob2 CLOB) RETURN CLOB;
 		
END;
/
show errors;