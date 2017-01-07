using System;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;

/// <summary>
/// Summary description for DataViewHelper
/// </summary>
public class DataViewHelper {
    public static COEDataView GetCompoundsDataView()
    {
        string dataview = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
"<COEDataView xmlns=\"COE.COEDataView\" basetable=\"1\" database= \"REGDB\" dataviewid=\"1\">" +
"	<!-- The list of tables -->" +
"	<tables basetable=\"1\">" +
"       <table id=\"1\" database=\"REGDB\" name=\"VW_COMPOUND\" primaryKey=\"10\">" +
"			<fields id=\"10\" name=\"COMPOUNDID\" dataType=\"INTEGER\"/>" +
"			<fields id=\"11\" name=\"REGID\" dataType=\"INTEGER\"/>" +
"			<fields id=\"12\" name=\"STRUCTUREID\" dataType=\"TEXT\" lookupFieldId=\"40\" lookupDisplayFieldId=\"41\"/>" +
"			<fields id=\"13\" name=\"COMMENTS\" dataType=\"TEXT\"/>" +
"			<fields id=\"14\" name=\"BASE64_CDX\" dataType=\"TEXT\" indexType=\"CS_CARTRIDGE\"/>" +
"			<fields id=\"15\" name=\"REGID\" dataType=\"TEXT\" lookupFieldId=\"20\" lookupDisplayFieldId=\"21\"/>" +
"		</table>" +
"       <table id=\"2\" database=\"REGDB\" name=\"VW_REGISTRYNUMBER\" primaryKey=\"20\">" +
"			<fields id=\"20\" name=\"REGID\" dataType=\"INTEGER\"/>" +
"			<fields id=\"21\" name=\"REGNUMBER\" dataType=\"TEXT\"/>" +
"			<fields id=\"22\" name=\"SEQUENCENUMBER\" dataType=\"INTEGER\"/>" +
"		</table>" +
"       <table id=\"3\" database=\"REGDB\" name=\"VW_MIXTURE_COMPONENT\" primaryKey=\"30\">" +
"			<fields id=\"30\" name=\"MIXTURECOMPONENTID\" dataType=\"INTEGER\"/>" +
"			<fields id=\"31\" name=\"MIXTUREID\" dataType=\"INTEGER\"/>" +
"			<fields id=\"32\" name=\"COMPOUNDID\" dataType=\"INTEGER\"/>" +
"		</table>" +
"       <table id=\"4\" database=\"REGDB\" name=\"VW_STRUCTURE\" primaryKey=\"40\">" +
"			<fields id=\"40\" name=\"STRUCTUREID\" dataType=\"INTEGER\"/>" +
"			<fields id=\"41\" name=\"STRUCTURE\" dataType=\"TEXT\"/>" +
"		</table>" +
"	</tables>" +
"	<!-- The following is the list of relations -->" +
"	<relationships>" +
"		<relationship child=\"2\" " +
"				  parent=\"1\"  " +
"				  childkey=\"20\" " +
"				  parentkey=\"11\"/>" +

"       <relationship child=\"3\" " +
"				  childkey=\"32\" " +
"				  parent=\"1\"  " +
"				  parentkey=\"10\"/>" +
"	</relationships>" +
"</COEDataView>";
        XmlDocument document = new XmlDocument();
        COEDataView resultDataview = new COEDataView();
        resultDataview.GetFromXML(dataview);
        return resultDataview;
    }
}
