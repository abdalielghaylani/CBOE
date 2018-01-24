using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// Interface to be implemented for controls that will allow uploading files.
    /// </summary>
    public interface ICOEFileUpload
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is file upload.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is file upload; otherwise, <c>false</c>.
        /// </value>
        bool IsFileUpload { get; set;}
        /// <summary>
        /// Gets or sets the page comunication provider.
        /// </summary>
        /// <value>The page comunication provider.</value>
        /// <example>GUIShell</example>
        string PageComunicationProvider { get; set; }

        /// <summary>
        /// Gets or sets the file upload binding expression.
        /// </summary>
        /// <value>The file upload binding expression.</value>
        string FileUploadBindingExpression { get; set; }
    }
}
