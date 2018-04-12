using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.DocumentManager.Services.AddIns;
using CambridgeSoft.COE.DocumentManager.Services.Types;
using System.Diagnostics;

namespace CambridgeSoft.COE.DocumentManager.Services.DocManagerAddIns
{
	[Serializable]
	public class DocumentAddIn : IAddIn
	{
        private string _initParams;
        public DocumentAddIn()
		{ }
		public void OnLoadedHandler(object sender, EventArgs args)
		{
			_document = (IDocument)sender;
            _document.Comments += "\nThis document passed through DocumentAddIn in OnLoadedHandler. And these are the init params: " + _initParams;
		}

        public void OnInsertingHandler(object sender, EventArgs args)
        {
            _document = (IDocument) sender;

            _document.Comments += "\nThis document passed through DocumentAddIn in OnInsertingHandler. And these are the init params: " + _initParams;
        }
		#region IAddIn Members
		public IDocument _document;

		public IDocument Document
		{
			get
			{
				return _document;
			}
			set
			{
				_document = value;
			}
		}

		public void Initialize(string xmlConfiguration)
		{
            _initParams = xmlConfiguration;
		}

		#endregion
	}
}
