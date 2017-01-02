using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COEServiceLib
{
    public class ErrorMessage
    {
        public static void ShowDialog(string dialogTitle, string errMessage, string exDesc)
        {
            ErrorMessageDialogForm errorMessageDialog = new ErrorMessageDialogForm();
            errorMessageDialog.SetErrorMessage(dialogTitle, errMessage, exDesc);
            errorMessageDialog.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            errorMessageDialog.ShowInTaskbar = false;
            errorMessageDialog.ShowDialog();
        }
    }
}
