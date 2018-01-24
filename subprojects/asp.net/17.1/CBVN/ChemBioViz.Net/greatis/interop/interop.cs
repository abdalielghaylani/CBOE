/*  GREATIS FORM DESIGNER FOR .NET
 *  Inter assembly operation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Text;

namespace Greatis
{
   namespace FormDesigner
   {
      /// <summary>
      /// Type represents state of the reader
      /// </summary>
      public enum ReaderState 
      { 
         /// <summary>
         /// state before reading
         /// </summary>
         Initial, 

         /// <summary>
         /// reading start element tag
         /// </summary>
         StartElement, 
         
         /// <summary>
         /// reading value
         /// </summary>
         Value, 
         
         /// <summary>
         /// reading end element tag
         /// </summary>
         EndElement, 
         
         /// <summary>
         /// End of file was reached
         /// </summary>
         EOF,

         /// <summary>
         /// Error occured while reading
         /// </summary>
         Error 
      }
      
      /// <summary>
      /// Type represent loading modes
      /// </summary>
      public enum LoadModes
      { 
         /// <summary>
         /// Default behaviour of the loader
         /// </summary>
         Default, 

         /// <summary>
         /// Create new control independently from existing one.
         /// </summary>
         Duplicate,

         /// <summary>
         /// Erase form before loading controls
         /// </summary>
         EraseForm,

         /// <summary>
         /// Only modifying properties of the existing controls
         /// </summary>
         ModifyExisting 
      }

      /// <summary>
      /// Type represent storing modes
      /// </summary>
      public enum StoreModes
      {
         /// <summary>
         /// Store only properties which values differs from the default value
         /// </summary>
         Default,

         /// <summary>
         /// store all component properties
         /// </summary>
         AllProperties 
      }

      /// <summary>
      /// Represents the method that will handle event for DrillDown event
      /// </summary>
      /// <param name="control"></param>
      /// <returns>true if designer can drill down control</returns>
      public delegate bool DrillDownHandler(IComponent control);

      /// <summary>
      /// Represents the method that will handle event for ComponentLoaded event
      /// </summary>
      /// <param name="control">currently loading control</param>
      public delegate void ComponentLoadedHandler(IComponent control);

      #region IWriter
      /// <summary>
      /// interface for serialization components
      /// </summary>
      public interface IWriter : IDisposable
      {
         /// <summary>
         /// Writes start element tag
         /// </summary>
         /// <param name="name">name of the component</param>
         /// <param name="attributes">collection of the attributes</param>
         void WriteStartElement(string name, Hashtable attributes);

         /// <summary>
         /// Writes end element tag
         /// </summary>
         /// <param name="name">name of the component</param>
         void WriteEndElement(string name);

         /// <summary>
         /// Writes value of the parameter
         /// </summary>
         /// <param name="name">name of the parameter</param>
         /// <param name="value">value of the parameter</param>
         /// <param name="attributes">collection of the attributes</param>
         void WriteValue(string name, string value, Hashtable attributes);
         
         /// <summary>
         /// Flushing writing buffers
         /// </summary>
         void Flush();
      }
      #endregion

      #region ILazyWrite
      /// <summary>
      /// interface for lazy write
      /// </summary>
      public interface ILazyWrite
      {
         /// <summary>
         /// begins lazy writing
         /// </summary>
         void Begin();

         /// <summary>
         /// end lazy writing
         /// </summary>
         /// <param name="cancel">if true ignores written data</param>
         void End(bool cancel);
      }
      #endregion

      #region IReader
      /// <summary>
      /// interface for reading serialization data
      /// </summary>
      public interface IReader : IDisposable
      {
         /// <summary>
         /// Read next token from stream
         /// </summary>
         /// <returns>false if error occured or eof</returns>
         bool Read();

         /// <summary>
         /// name of the current token
         /// </summary>
         string Name
         {
            get;
         }

         /// <summary>
         /// collection of the attributes of the current token
         /// </summary>
         Hashtable Attributes
         {
            get;
         }

         /// <summary>
         /// value of the current token
         /// </summary>
         string Value
         {
            get;
         }

         /// <summary>
         /// reader state
         /// </summary>
         ReaderState State
         {
            get;
         }
      }
      #endregion

      #region TextFormReader
      /// <summary>
      /// Implementation of the IReader interface for the text stream
      /// </summary>
      public class TextFormReader : IReader
      {
         private ReaderState state = ReaderState.Initial;
         private TextReader reader;
         private Hashtable attributes;
         private string item = "";
         private string value = "";
         private string curLine;

         public TextFormReader(string fileName)
         {
            reader = new StreamReader(fileName);
            attributes = new Hashtable();
         }

         public TextFormReader(TextReader stream)
         {
            reader = stream;
            attributes = new Hashtable();
         }

         public TextFormReader(Stream stream)
         {
            reader = new StreamReader(stream);
            attributes = new Hashtable();
         }

         public void Dispose()
         {
            reader.Close();
         }

         public string Name
         {
            get { return item; }
         }

         public Hashtable Attributes
         {
            get { return attributes; }
         }

         public string Value
         {
            get { return value; }
         }

         public ReaderState State
         {
            get { return state; }
         }

         public bool Read()
         {
            if (state == ReaderState.EOF)
               return false;

            if (ReadNext() == false)
               return false;

            attributes.Clear();

            if (curLine.IndexOf("Begin ") == 0)
            {
               int startBracket = curLine.IndexOf('[');
               if (startBracket == -1) // previous version of the form layout
               {
                  int startQooute = curLine.IndexOf('"');
                  int endQoute = curLine.LastIndexOf('"');
                  attributes["assembly"] = curLine.Substring(startQooute + 1, endQoute - startQooute - 1);
                  attributes["name"] = curLine.Substring(endQoute + 1).Trim();
                  item = "object";
               }
               else
               {
                  item = curLine.Substring(6, startBracket - 6).Trim();

                  SaveAttributes(curLine.Substring(startBracket + 1, curLine.LastIndexOf(']') - startBracket - 1));
               }
               state = ReaderState.StartElement;
            }
            else if (curLine.IndexOf("End") == 0)
            {
               state = ReaderState.EndElement;
            }
            else
            {
               state = ReaderState.Value;
               int valueBegin;
               int startBracket = curLine.IndexOf('[');
               if (startBracket >= 0) // have attributes
               {
                  int endBracket = curLine.IndexOf(']', startBracket+1);
                  SaveAttributes(curLine.Substring(startBracket + 1, endBracket - startBracket - 1));

                  item = curLine.Substring(0, startBracket).Trim();
                  valueBegin = curLine.IndexOf('=', endBracket) + 1;
               } else
               {
                  valueBegin = curLine.IndexOf('=', 0) + 1;
                  if (valueBegin > 0)
                    item = curLine.Substring(0, valueBegin-1).Trim();
               }
               value = curLine.Substring(valueBegin).Trim();
            }

            return true;
         }

         // split string a1="v1" a2="v2" into hash [a1]=v1, [a2]=v2
         private void SaveAttributes(string attributeString)
         {
            int startIndex = 0;

            while (true)
            {
               int eqSign = attributeString.IndexOf('=', startIndex);
               if (eqSign < 0)
                  return;

               string key = attributeString.Substring(startIndex, eqSign - startIndex).Trim();

               int quoteStart = attributeString.IndexOf('"', eqSign);
               int quoteEnd = attributeString.IndexOf('"', quoteStart + 1);

               if (quoteStart < 0 || quoteEnd < 0)
                  return;

               string value = attributeString.Substring(quoteStart + 1, quoteEnd - quoteStart - 1).Trim();

               attributes[key] = value;
               startIndex = quoteEnd + 1;
            }
         }

         private bool ReadNext()
         {
            if (state == ReaderState.Error || state == ReaderState.EOF)
               return false;

            try
            {
               char[] trimChar = new char[] { ' ' };
               do
               {
                  curLine = reader.ReadLine();
                  if( curLine == null )
                  {
                     state = ReaderState.EOF;
                     return false;
                  }
                  curLine = curLine.TrimStart(trimChar);
               } while (curLine.Length == 0);
            }
            catch (Exception e)
            {
               if (e is EndOfStreamException)
                  state = ReaderState.EOF;
               else
                  state = ReaderState.Error;
               return false;
            }

            return true;
         }
      }
      #endregion

      #region TextFormWriter
      /// <summary>
      /// Implementation of the IWriter and ILazyWrite interface for the text stream
      /// </summary>
      public class TextFormWriter : IWriter, ILazyWrite
      {
         private TextWriter writer, curWriter;
         private StringWriter lazyWriter = null;
         private int indentation = 3;
         private string currentIndent = "";
         
         public TextFormWriter(string fileName)
         {
            writer = new StreamWriter(fileName);
            curWriter = writer;
         }

         public TextFormWriter(TextWriter stream)
         {
            writer = stream;
            curWriter = writer;
         }

         public TextFormWriter(Stream stream)
         {
            writer = new StreamWriter(stream);
            curWriter = writer;
         }

         public int Indentation
         {
            get { return indentation; }
            set { indentation = value; }
         }

         private void WriteAttributes(Hashtable attributes)
         {
            if (attributes == null || attributes.Count == 0)
               return;

            bool first = true;

            curWriter.Write("[");
            foreach (DictionaryEntry de in attributes)
            {
               if( de.Value == null )
                  continue;

               if (first == true) first = false;
               else curWriter.Write(" ");

               curWriter.Write("{0}=\"{1}\"", de.Key.ToString(), de.Value.ToString());
            }
            curWriter.Write("]");
         }

         // Begin [a1="v1" a2="v2"] Name
         public void WriteStartElement(string name, Hashtable attributes)
         {
            curWriter.Write(currentIndent);
            curWriter.Write("Begin ");
            curWriter.Write(name);
            curWriter.Write(" ");
            WriteAttributes(attributes);
            curWriter.WriteLine();

            for (int i = 0; i < indentation; i++)
               currentIndent += ' ';
         }

         public void WriteEndElement(string name)
         {
            currentIndent = currentIndent.Remove(currentIndent.Length - indentation, indentation);
            curWriter.Write(currentIndent);
            curWriter.WriteLine("End");
         }

         public void WriteValue(string name, string value, Hashtable attributes)
         {
            curWriter.Write(currentIndent);
            curWriter.Write("{0}", name);
            WriteAttributes(attributes);
            curWriter.WriteLine("={0}", value);
         }

         public void Dispose()
         {
            writer.Close();

            if( lazyWriter != null )
               lazyWriter.Close();
         }

         public void Flush()
         {
            writer.Flush();
         }

         #region ILazyWrite
         private int callCount = 0;

         public void Begin()
         {
            if (callCount == 0)
            {
               lazyWriter = new StringWriter();
               curWriter = lazyWriter;
            }
            callCount++;
         }

         public void End(bool cancel)
         {
            callCount--;
            if (callCount != 0)
               return;

            curWriter = writer;
            if( lazyWriter == null )
               return;

            if( cancel == false )
            {
               lazyWriter.Flush();
               writer.Write(lazyWriter.ToString());
            }
            lazyWriter = null;
         }
         #endregion
      }
      #endregion

      #region XMLFormReader
      /// <summary>
      /// Implementation of the IReader interface for the XML stream
      /// </summary>
      public class XMLFormReader : IReader
      {
         private XmlReader reader;
         private ReaderState state = ReaderState.Initial;
         protected Hashtable attrmap = new Hashtable();
         protected string item = "";
         private string value = "";
         private bool isEmptyValue = false;

         public XMLFormReader(string fileName)
         {
            reader = new XmlTextReader(fileName);
         }

         public XMLFormReader(XmlReader stream)
         {
            reader = stream;
         }

         public XMLFormReader(Stream stream)
         {
            reader = new XmlTextReader(stream);
         }

         public string Name
         {
            get { return item; }
         }

         public Hashtable Attributes
         {
            get { return attrmap; }
         }

         public string Value
         {
            get { return value; }
         }

         public ReaderState State
         {
            get { return state; }
         }

         public void Dispose()
         {
            reader.Close();
         }

         virtual public bool Read()
         {
            if (state == ReaderState.Error || state == ReaderState.EOF)
               return false;

            if (reader.ReadState != System.Xml.ReadState.Initial &&
               reader.ReadState != System.Xml.ReadState.Interactive)
            {
               return false;
            }

            if (state == ReaderState.Initial)
            {
               if (ReadUntil(XmlNodeType.Element) == false)
               {
                  state = ReaderState.Error;
                  return false;
               }
            }

            if (isEmptyValue)
            {
               isEmptyValue = false;
               ReadNext();
            }

            if (reader.NodeType == XmlNodeType.Element)
            {
               state = ReaderState.StartElement;
               item = reader.Name;
               isEmptyValue = reader.IsEmptyElement;

               ReadAttributes();
               // special case for reading empty elements like <element />
               if (isEmptyValue)
               {
                  value = "";
                  state = ReaderState.Value;
                  return true;
               }
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
               state = ReaderState.EndElement;
               item = ""; // for case </Element></Element>
            } else
               state = ReaderState.Value;

            if (ReadNext() == false)
            {
               state = (reader.ReadState == System.Xml.ReadState.EndOfFile) ? ReaderState.EOF : ReaderState.Error;
               return false;
            }

            if (reader.NodeType == XmlNodeType.Text)
            {
               // a value
               value = reader.Value;

               // next node's type must be a EndElement, check it
               if (ReadNext() == false || reader.NodeType != XmlNodeType.EndElement)
               {
                  state = ReaderState.Error;
                  return false;
               }

               state = ReaderState.Value;
               ReadNext(); // move to next element
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == item)
            {
               // empty element of form <elName></elName>, e.g. <Text></Text>
               value = "";
               state = ReaderState.Value;
               ReadNext(); // move to next element, skip xmlNodeType.EndElement
            }

            return true;
         }

         private void ReadAttributes()
         {
            // precondition: reader.XmlNodeType == XmlNodeType.Element

            attrmap.Clear();
            while (reader.MoveToNextAttribute() == true)
               attrmap[reader.Name] = reader.Value;
         }

         private bool ReadUntil(XmlNodeType nodeType)
         {
            while (ReadNext() == true)
            {
               if (reader.NodeType == nodeType)
                  return true;
            }
            return false;
         }

         private bool ReadNext()
         {
            while (reader.Read() == true)
            {
               switch (reader.NodeType)
               {
                  case XmlNodeType.Element:
                     //if (reader.IsEmptyElement == false)
                        return true;
                     //break;

                  case XmlNodeType.EndElement:
                  case XmlNodeType.Text:
                     return true;
               }
            }
            return false;
         }
      }
      #endregion

      #region XMLFormWriter
      /// <summary>
      /// Implementation of the IWriter and ILazyWrite interface for the XML stream
      /// </summary>
      public class XMLFormWriter : IWriter, ILazyWrite
      {
         XmlWriter writer, lazyWriter = null, curWriter;
         StringWriter stringWriter = null;

         public XMLFormWriter(string fileName)
         {
            writer = new XmlTextWriter(fileName, Encoding.UTF8);
            curWriter = writer;
         }

         public XMLFormWriter(XmlWriter stream)
         {
            writer = stream;
            curWriter = writer;
         }

         public XMLFormWriter(Stream stream)
         {
            writer = new XmlTextWriter(stream, Encoding.UTF8);
            curWriter = writer;
         }

         virtual public void WriteStartElement(string name, Hashtable attributes)
         {
            curWriter.WriteStartElement(name);
            if (attributes != null)
            {
               foreach (DictionaryEntry de in attributes)
               {
                  if( de.Value != null )
                     curWriter.WriteAttributeString(de.Key.ToString(), de.Value.ToString());
               }
            }
         }

         virtual public void WriteEndElement(string name)
         {
            curWriter.WriteEndElement();
         }

         virtual public void WriteValue(string name, string value, Hashtable attributes)
         {
            WriteStartElement(name, attributes);
            curWriter.WriteString(value);
            curWriter.WriteEndElement();
         }

         public void Dispose()
         {
            writer.Close();
            if( lazyWriter != null )
               lazyWriter.Close();
         }
      
         public void Flush()
         {
            writer.Flush();
         }

         #region ILazyWrite
         private int callCount = 0;
         
         public void Begin()
         {
            if (callCount == 0)
            {
               stringWriter = new StringWriter();
               lazyWriter = new XmlTextWriter(stringWriter);
               curWriter = lazyWriter;
            }
            callCount++;
         }

         public void End(bool cancel)
         {
            callCount--;
            if (callCount != 0)
               return;

            curWriter = writer;
            if( lazyWriter == null )
               return;

            if( cancel == false )
            {
               lazyWriter.Flush();

               string v = stringWriter.ToString();
               if (v.Length > 0)
               {
                  XmlTextReader reader = new XmlTextReader(new StringReader(v));
                  writer.WriteNode(reader, false);
               }
            }
            stringWriter = null;
            lazyWriter = null;
         }
         #endregion
      }
      #endregion

      #region ITreasury
      /// <summary>
      /// Interface for loading and storing controls 
      /// </summary>
      public interface ITreasury
      {
         /// <summary>
         /// Fires when Treasury try to drill serialized down control
         /// </summary>
         event DrillDownHandler DrillDown;

         /// <summary>
         /// Fires when component completly loaded into the form
         /// </summary>
         event ComponentLoadedHandler ComponentLoaded;

         /// <summary>
         /// Gets or sets load mode for treasury
         /// </summary>
         LoadModes LoadMode
         {
            get;
            set;
         }

         /// <summary>
         /// Gets or sets store mode for treasury
         /// </summary>
         StoreModes StoreMode
         {
            get;
            set;
         }

         /// <summary>
         /// Sets log file name
         /// </summary>
         string LogFile
         {
            get;
            set;
         }

         /// <summary>
         /// Enables or disables showing error message box on exception
         /// </summary>
         bool ShowErrorMessage
         {
            get;
            set;
         }

         /// <summary>
         /// Gets or sets designer host
         /// </summary>
         IDesignerHost DesignerHost
         {
            get;
            set;
         }

         /// <summary>
         /// Loads controls into parent from passed reader
         /// </summary>
         /// <param name="parent">parent for loaded controls</param>
         /// <param name="reader">reader object containing the data</param>
         /// <param name="components">array of the loaded components</param>
         /// <param name="ignoreParent">if true, supresses loading parent properties</param>
         void Load(Control parent, IReader reader, IComponent[] components, bool ignoreParent);

         /// <summary>
         /// Serializes objects with embeded controls int writer
         /// </summary>
         /// <param name="parents">collection of the parent components</param>
         /// <param name="writer">destination writer</param>
         void Store(IComponent[] parents, IWriter writer);
      }
      #endregion
   }
}
