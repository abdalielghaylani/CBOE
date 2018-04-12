using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.DocumentManager.Services.COEDocumentManagerService
{
	class DAL : DALBase
	{
		internal virtual string GetDocumentList()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		internal virtual string GetDocumentByID(Int32 ID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		internal virtual Int32 AddNewDocument(DocumentManager.Services.Types.Document doc)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		internal virtual string GetEmptyDocObject()
		{
			throw new Exception("The method or operation is not implemented.");
		}

        internal virtual void DeleteDocumentByID(Int32 ID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal virtual void DeleteDocument(DocumentManager.Services.Types.Document doc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal virtual Int32 UpdateDocument(DocumentManager.Services.Types.Document doc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

	}
}
