using System.Text;
using System.ComponentModel;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.SD
{
    /// <summary>
    /// Represents the MDL Molfile format for chemical structure data. This may be a
    /// stand-alone data point or it may be part of the SDFile record format.
    /// </summary>
    [DefaultProperty("Mol")]
    public class Molfile
    {
        private string _mol = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        public Molfile() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rawMolFile">String representation of the MDL Molfile format.</param>
        public Molfile(string rawMolFile)
        {
            this._mol = rawMolFile;
        }

        /// <summary>
        /// Represents the 'MolFile' (MOL) part of the SD record unit. Contains both the 
        /// standardized header information (first 3 lines) for the molecule (depending
        /// on the source) as well as the structural information of the molecule (all
        /// remaining lines).
        /// </summary>
        public string Mol
        {
            get { return _mol; }
            set { _mol = value; }
        }
    }
}
