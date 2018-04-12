using System.Text;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.SD
{
    public class SDSourceRecord : SourceRecordBase
    {
        #region > Properties, private members <

        private const string EMPTY_MOLECULE_INDICATOR = "0  0  0  0  0  0  0  0  0";
        private const string MOL_END_INDICATOR = "M  END";
        private const string FIELD_NAME_PREFIX = "DT";
        public const string MOLECULE_FIELD = "structure";

        /// <summary>
        /// Internal container for the raw data, in lines, from an SD record.
        /// </summary>
        private List<string> _rawLines;

        private string _rawRecord = string.Empty;
        /// <summary>
        /// The raw data from which the instance was instantiated, if any.
        /// </summary>
        public string RawRecord
        {
            get
            {
                if (_rawLines != null)
                {
                    string[] lines = _rawLines.ToArray();
                    _rawRecord = string.Join("\r\n", lines);
                }
                return _rawRecord;
            }
        }

        private Molfile _mdlMolFile = new Molfile();
        /// <summary>
        /// The structural information for the record. Holds header data and
        /// an MDL MOL format connection table.
        /// </summary>
        public Molfile MdlMolFile
        {
            get { return _mdlMolFile; }
        }

        #endregion

        public SDSourceRecord(int recordIndex, List<string> rawDataLines)
            : base(recordIndex)
        {
            if (rawDataLines.Count > 0)
            {
                _rawLines = rawDataLines;
                ParseSDLines();
            }
        }

        public SDSourceRecord(int recordIndex, string[] rawRecordLines)
            :base(recordIndex)
        {
            _rawLines = new List<string>(rawRecordLines);
            ParseSDLines();
        }

        /// <summary>
        /// Creates an SDFile record from the MolFile and Fields contents, producing a value
        /// for the RawRecord property.
        /// </summary>
        public string ComposeRecord()
        {
            string buf = string.Empty;

            if (_rawLines != null && _rawLines.Count > 0)
                //use the existing raw, untouched data from the constructor
                buf = RawRecord;
            else
            {
                //create the SDRecord from the individual data-points
                //initialize using the MolFile
                StringBuilder sdBuilder = new StringBuilder(this._mdlMolFile.Mol);

                //add each property field sequentially
                foreach (string key in _fieldSet.Keys)
                {
                    //provide placeholders for missing field data
                    sdBuilder.AppendLine(string.Format(">  <{0}>", key));
                    object val = null;
                    if (this._fieldSet.TryGetValue(key, out val))
                        sdBuilder.AppendLine(val.ToString());
                    sdBuilder.Append("\r\n");
                }
                //add the end-of-record marker and a CrLf
                sdBuilder.AppendLine("$$$$");
                buf = sdBuilder.ToString();
            }
            return buf;
        }

        private void ParseSDLines()
        {
            string lineOfData = string.Empty;
            int lineCount = ExtractMolecule();
            
            //parse the custom field data
            int gtIndex;
            int ltIndex;
            string fieldKey = null;
            string fieldValueBuffer = null;

            do
            {
                lineOfData = _rawLines[lineCount];

                //determine if this line defined a data-field
                //  data-field definitions usually, but not always, start with ">  <"
                //  data-field may also be the first string beginning with
                gtIndex = lineOfData.Trim().IndexOf(">");
                if (gtIndex == 0)
                {
                    //wipe out the previous field's data
                    fieldValueBuffer = null;

                    ltIndex = lineOfData.IndexOf("<", gtIndex);
                    if (ltIndex != -1)
                    {
                        //now find the field-closing ">"
                        gtIndex = lineOfData.IndexOf(">", ltIndex);
                        if (gtIndex != -1)
                        {
                            fieldKey = lineOfData.Substring(ltIndex + 1, (gtIndex - ltIndex) - 1);
                        }
                    }
                    else if (lineOfData.Contains(FIELD_NAME_PREFIX))
                    {
                        string[] values = lineOfData.Substring(gtIndex + 1).Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                        if (values != null)
                        {
                            foreach (string value in values)
                            {
                                if (value.StartsWith(FIELD_NAME_PREFIX))
                                {
                                    fieldKey = value;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    /* NOTE: The field's value may be multi-line; accumulate the field's data.
                     * In this case, we accumulate the lines until we reach an empty line, which
                     * indicates the end-of-field. */
                    if (!string.IsNullOrEmpty(fieldKey)
                        && !lineOfData.Contains("$$$$")
                    )
                    {

                        if (string.IsNullOrEmpty(fieldValueBuffer))
                            fieldValueBuffer += lineOfData;
                        else
                            if (!string.IsNullOrEmpty(lineOfData))
                            fieldValueBuffer += "\r\n" + lineOfData;
                    }
                    //Coverity fix - CID 11457
                    if (string.IsNullOrEmpty(lineOfData) && !string.IsNullOrEmpty(fieldKey))
                        SourceFieldTypes.SetValue(fieldKey, fieldValueBuffer, this);

                }

                lineCount++;
            } while (lineCount < _rawLines.Count);

        }

        private int ExtractMolecule()
        {
            StringBuilder molBuilder = new StringBuilder();
            int lineCount = 0;
            string buf = string.Empty;
            bool isEmptyStructure = false;

            //parse the header data and structure value
            do
            {
                buf = _rawLines[lineCount];
                if (buf.Replace(" ", "").StartsWith(EMPTY_MOLECULE_INDICATOR.Replace(" ", "")))
                    isEmptyStructure = true;
                molBuilder.AppendLine(buf);
                lineCount++;
            } while (
                !buf.Trim().StartsWith(MOL_END_INDICATOR)
                && lineCount < _rawLines.Count
            );

            buf = molBuilder.ToString();

            if (
                !string.IsNullOrEmpty(buf)
                && !buf.Trim().StartsWith(EMPTY_MOLECULE_INDICATOR)
                && molBuilder.Length > 2
                )
            {
                this._mdlMolFile.Mol = buf;
            }

            if (!isEmptyStructure)
                SourceFieldTypes.SetValue(MOLECULE_FIELD, this._mdlMolFile.Mol, this);
            else
                SourceFieldTypes.SetValue(MOLECULE_FIELD, string.Empty, this);

            return lineCount;
        }

    }
}
