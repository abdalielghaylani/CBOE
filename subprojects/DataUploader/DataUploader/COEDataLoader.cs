using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;

namespace CambridgeSoft.COE.DataLoader
{
    public partial class COEDataLoader : Form
    {
        /// <summary>
        /// Form constructor
        /// </summary>
        public COEDataLoader()
        {
            InitializeComponent();
        }
    }
}