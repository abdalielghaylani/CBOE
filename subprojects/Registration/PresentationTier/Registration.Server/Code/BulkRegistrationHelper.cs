using CambridgeSoft.COE.Registration;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class BulkRegistrationHelper
    {
        public static DuplicateAction GetDuplicateAction(string duplicateActionName)
        {
            DuplicateAction duplicateAction = DuplicateAction.None;
            switch (duplicateActionName)
            {
                case "Duplicate":
                    duplicateAction = DuplicateAction.Duplicate;
                    break;
                case "Batch":
                    duplicateAction = DuplicateAction.Batch;
                    break;
                case "Temporary":
                    duplicateAction = DuplicateAction.Temporary;
                    break;
                case "Compound":
                    duplicateAction = DuplicateAction.Compound;
                    break;
                default:
                    duplicateAction = DuplicateAction.None;
                    break;
            }

            return duplicateAction;
        }
    }
}