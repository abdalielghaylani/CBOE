using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;
using System.Data;//Jerry
using System.Xml;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEExportService
{
	internal class Excel : FormatterBase, IFormatter
	{
		[NonSerialized]
		static COELog _coeLog = COELog.GetSingleton("COEExport");
		protected Infragistics.WebUI.UltraWebGrid.ExcelExport.UltraWebGridExcelExporter UltraWebGridExcelExporter1= new  Infragistics.WebUI.UltraWebGrid.ExcelExport.UltraWebGridExcelExporter();
		protected Infragistics.WebUI.UltraWebGrid.UltraWebGrid UltraWebGrid1 = new Infragistics.WebUI.UltraWebGrid.UltraWebGrid();

		#region IFormatter Members

		public string FormatDataSet(DataSet dataSet, COEDataView dataView, ResultsCriteria resultCriteria)
		{
			System.IO.StringWriter sw = new System.IO.StringWriter();
			dataSet.WriteXml(sw);

			string excelXML = "";
			//excelXML += AddExcelHeader();
			excelXML += AddExcelWorkbook();
			excelXML += AddExcelStyles();

			//Now add new worksheet with a name
			excelXML += "<Worksheet ss:Name=\"ExportedData\">\n";


			excelXML += "<Table>\n";

			excelXML += "<Row>\n";

			int parentOffset = dataSet.Tables[0].Columns.Count;

				foreach (DataColumn dc in dataSet.Tables[0].Columns)
				{
				    excelXML += "<Cell ss:StyleID=\"s1\"><Data ss:Type=\"String\">" + dc.ColumnName + "</Data></Cell>";

				}
				excelXML += "</Row>\n";

				foreach (DataRow dr in dataSet.Tables[0].Rows)
				{
					excelXML += "<Row>\n";
					foreach (DataColumn dc2 in dataSet.Tables[0].Columns)
					{
						string cellValue = dr[dc2.ColumnName].ToString();

						if (dc2.Ordinal == 0)
						{
							excelXML += BuildStructureCell(cellValue);
						}
						else
						{
							excelXML += BuildCell(cellValue);
						}
					}
					int childOffset = 0;
					foreach (DataRelation drel in dataSet.Relations)
					{
						int i = 1;
						foreach (DataRow subrow in dr.GetChildRows(drel))
						{

							foreach (DataColumn dc3 in dataSet.Tables[drel.ChildTable.TableName].Columns)
							{
								string cellValue = subrow[dc3.ColumnName].ToString();
								excelXML += BuildCell(cellValue);
							}

							if (i < dr.GetChildRows(drel).Length)
							{
								excelXML += "</Row>\n<Row>\n";

								int j = 0;
								while (j < parentOffset)
								{
									excelXML += BuildCell("");
									j++;
								}

								j = 0;
								while (j < childOffset)
								{
									excelXML += BuildCell("");
									j++;
								}


							}
							i++;
							
						}
						childOffset += dataSet.Tables[drel.ChildTable.TableName].Columns.Count;
					}
					excelXML += "</Row>\n";
				}

				excelXML += "</Table>\n";
				excelXML += "</Worksheet>\n";
				excelXML += "</Workbook>";
			return excelXML;


			//try to use infragistics
			//System.IO.MemoryStream stream = new System.IO.MemoryStream();


			//this.UltraWebGrid1.DataSource = dataSet;
			//this.UltraWebGrid1.DisplayLayout.Pager.AllowPaging = false;
			//this.UltraWebGrid1.DataBind();
			////System.IO.Stream si = new System.IO.Stream;
			//this.UltraWebGrid1.Browser = Infragistics.WebUI.UltraWebGrid.BrowserLevel.Xml;
			//this.UltraWebGridExcelExporter1.Export(this.UltraWebGrid1, stream);
			//stream.Seek(0, System.IO.SeekOrigin.Begin);
			////System.IO.StreamReader iostr = new System.IO.StreamReader(stream,Encoding.UTF8);
			//System.IO.BinaryReader iobstr = new System.IO.BinaryReader(stream);


			//WebGrid w = new WebGrid();
			//string swrtarget = System.Text.ASCIIEncoding.ASCII.GetString(ReadFully(stream, 0)); //Read the file from Start to End in one go.
			//Infragistics.Excel.BIFF8Writer.




			//return swrtarget;
		}

		#endregion

		private string BuildCell(string cellData)
		{
			string returnXML = String.Empty;
			if (IsNumeric(cellData))
			{
				returnXML += "<Cell><Data ss:Type=\"Number\">" + cellData + "</Data></Cell>\n";
			}
			
			else if (IsDate(cellData))
			{
				returnXML += "<Cell><Data ss:Type=\"String\">" + Convert.ToDateTime(cellData) + "</Data></Cell>\n";
			}
			else
			{
				returnXML += "<Cell><Data ss:Type=\"String\">" + cellData + "</Data></Cell>\n";

			}
			return returnXML;
		}

		private string BuildStructureCell(string cellData)
		{
			string returnXML = String.Empty;

			returnXML += "<Cell><Data ss:Type=\"String\">Structure</Data><Comment><Data>" + cellData + "</Data></Comment></Cell>\n";

			return returnXML;
		}




		private string AddExcelHeader()
		{
			string xmlHeader = "<xsl:stylesheet version=\"1.0\"" +
			"xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"" +
			"xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" " +
			"xmlns:msxsl=\"urn:schemas-microsoft-com:xslt\"" +
			"xmlns:user=\"urn:my-scripts\"" +
			"xmlns:o=\"urn:schemas-microsoft-com:office:office\"" +
			"xmlns:x=\"urn:schemas-microsoft-com:office:excel\"" +
			"xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" >\n";  

			return xmlHeader;

		}


		private string AddExcelWorkbook()
		{
			//"<?xml version=\"1.0\"?>" +
			//        "<?mso-application progid=\"Excel.Sheet\"?>" +
			string excelWorkbook =  "<?xml version=\"1.0\"?>" +
             " <?mso-application progid=\"Excel.Sheet\"?>" +
			 "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"" +
			 " xmlns:o=\"urn:schemas-microsoft-com:office:office\"" +
			 " xmlns:x=\"urn:schemas-microsoft-com:office:excel\"" +
			 " xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"" +
			 " xmlns:html=\"http://www.w3.org/TR/REC-html40\">\n";

			return excelWorkbook;
		}

		private string AddExcelStyles()
		{
			 string excelStyles = "<Styles>\n" +
			  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\n" +
			   "<Alignment ss:Vertical=\"Bottom\"/>\n" +
			   "<Borders/>\n" +
			   "<Font/>\n" +
			   "<Interior/>\n" +
			   "<NumberFormat/>\n" +
			   "<Protection/>\n" +
			  "</Style>\n" +
			  "<Style ss:ID=\"s1\">\n" +
					"<Font ss:Bold=\"1\"/>\n" +
				"</Style>\n" +
			 "</Styles>\n";

			return excelStyles;
		}
		
		public static bool IsNumeric(string anyString) { 

			if (anyString == null) { anyString = ""; } 
			if (anyString.Length > 0) { double dummyOut = new double(); 
				System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US", true); 
				return Double.TryParse(anyString, System.Globalization.NumberStyles.Any, cultureInfo.NumberFormat, out dummyOut); } 
			else { return false; } 
		}
		public static bool IsDate(string anyString) { 
			if (anyString == null) 
			{ 
				anyString = ""; 
			} 
			if (anyString.Length > 0) 
			{ 
				DateTime dummyDate = DateTime.MinValue; 
				try
				{
					dummyDate = DateTime.Parse(anyString); 
				} 
				catch 
				{ 
					return false; 
				} 
				return true; 
			} 
			else 
			{
				return false; 
			} 
		}

		/// <summary>
		/// Reads data from a stream until the end is reached. The
		/// data is returned as a byte array. An IOException is
		/// thrown if any of the underlying IO calls fail.
		/// </summary>
		/// <param name="stream">The stream to read data from</param>
		/// <param name="initialLength">The initial buffer length</param>
		public static byte[] ReadFully(System.IO.Stream stream, int initialLength)
		{
			// If we've been passed an unhelpful initial length, just
			// use 32K.
			if (initialLength < 1)
			{
				initialLength = 32768;
			}

			byte[] buffer = new byte[initialLength];
			int read = 0;

			int chunk;
			while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
			{
				read += chunk;

				// If we've reached the end of our buffer, check to see if there's
				// any more information
				if (read == buffer.Length)
				{
					int nextByte = stream.ReadByte();

					// End of stream? If so, we're done
					if (nextByte == -1)
					{
						return buffer;
					}

					// Nope. Resize the buffer, put in the byte we've just
					// read, and continue
					byte[] newBuffer = new byte[buffer.Length * 2];
					Array.Copy(buffer, newBuffer, buffer.Length);
					newBuffer[read] = (byte)nextByte;
					buffer = newBuffer;
					read++;
				}
			}
			// Buffer is now too big. Shrink it.
			byte[] ret = new byte[read];
			Array.Copy(buffer, ret, read);
			return ret;
		}


	}
}
