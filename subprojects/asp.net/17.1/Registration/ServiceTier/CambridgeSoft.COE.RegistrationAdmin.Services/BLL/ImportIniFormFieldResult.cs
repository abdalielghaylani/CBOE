using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.IniParser;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class ImportIniFormFieldResult : ImportIniResult
    {
        private SortedList<IniFormField, List<string>> _formFieldImportInfo = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="formFields">The list of COE form fields parsed from INI</param>
        [COEUserActionDescription("ImportIniFormField")]
        public ImportIniFormFieldResult(IList<IniFormField> formFields)
            : this()
        {
            try
            {
                _formFieldImportInfo = new SortedList<IniFormField, List<string>>();

                foreach (IniFormField formField in formFields)
                {
                    _formFieldImportInfo.Add(formField, new List<string>());
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Constructor, not supposed to be exposed to public
        /// </summary>
        protected ImportIniFormFieldResult()
        {
            base.SuccessMessageTemplate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\tSUCCESSFUL\tForm field '{0}' has been imported to form element '{1}'";
            base.FailureMessageTemplate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\tFAILED\t";
            base.SkipMessageTemplate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\tSKIPPED\tForm field '{0}' was already migrated and was skipped at {1}";

            this.IgnoreMessageTemplate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\tIGNORED\tForm field '{0}' not found in any form group";
        }

        private string _ignoreMessageTemplate = string.Empty;
        /// <summary>
        /// The message template for indicating that the form field was not found in any form group
        /// </summary>
        public string IgnoreMessageTemplate
        {
            get { return _ignoreMessageTemplate; }
            set { _ignoreMessageTemplate = value; }
        }

        /// <summary>
        /// Represents the detailed information about all the form fields import
        /// </summary>
        public SortedList<IniFormField, List<string>> FormFieldImportInfo
        {
            get { return _formFieldImportInfo; }
        }

        [COEUserActionDescription("LogFormFieldImport")]
        public void LogFormFieldImportResult()
        {
            try
            {
                foreach (KeyValuePair<IniFormField, List<string>> importInfo in FormFieldImportInfo)
                {
                    if (importInfo.Value.Count == 0)
                    {
                        base.LogResult(this.IgnoreMessageTemplate, importInfo.Key.FieldKey);
                    }
                    else
                    {
                        for (int i = 0; i < importInfo.Value.Count; i++)
                        {
                            if (importInfo.Key.IsSkipped)
                                base.LogResult(this.SkipMessageTemplate, importInfo.Key.FieldKey, importInfo.Value[i]);
                            else
                                base.LogResult(this.SuccessMessageTemplate, importInfo.Key.FieldKey, importInfo.Value[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }
    }
}
