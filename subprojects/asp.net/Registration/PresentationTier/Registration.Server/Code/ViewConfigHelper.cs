using System;
using System.Collections.Generic;
using CambridgeSoft.COE.Registration.Services.Types;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class ViewConfigHelper
    {
        internal static ControlState GetRegisterButtonState(RegistryRecord registryRecord, PageState pageMode, RegistryTypes currentRegistryType)
        {
            ControlState controlState = new ControlState();
            controlState.Name = "Register";
            controlState.Visible = false;

            // If ApprovalsEnabled = true in system settings, no need to display Register button
            if (RegUtilities.GetApprovalsEnabled())
                return controlState;

            switch (pageMode)
            {
                case PageState.AddSingleCompound:
                case PageState.EditSingleCompound:
                case PageState.AddComponent:
                case PageState.AddBatch:
                case PageState.EditComponent:
                case PageState.EditBatch:
                case PageState.EditRecord:
                    controlState.Visible = true;
                    break;
                case PageState.ViewComponent:
                    if (currentRegistryType == RegistryTypes.Mixture)
                        controlState.Visible = true;
                    break;
            }

            if (currentRegistryType == RegistryTypes.Mixture || currentRegistryType == RegistryTypes.Both)
            {
                if (pageMode != PageState.End)
                {
                    bool isValidComponentList = true;
                    bool isValidBatchList = true;
                    bool isMixture = false;
                    if (registryRecord.ComponentList.Count < Convert.ToInt32(MixtureRule.ComponentList))
                    {
                        controlState.Tooltip = "Mixtures must have more than 1 component before registration.";
                        isValidComponentList = false;
                    }
                    else if (registryRecord.BatchList[0].BatchComponentList[0].PropertyList["PERCENTAGE"].Value == string.Empty && !pageMode.ToString().Contains("Batch"))
                    {
                        controlState.Tooltip = "Mixtures must have batch component percentage information before registration";
                        isValidBatchList = false;
                    }
                    else if (registryRecord.ComponentList.Count >= Convert.ToInt32(MixtureRule.ComponentList) && pageMode.ToString().Contains("Batch"))
                    {
                        isValidComponentList = true;
                        isValidBatchList = true;
                        isMixture = true;
                    }

                    if (isMixture && isValidComponentList && isValidBatchList)
                    {
                        controlState.Visible = true;
                        controlState.Tooltip = "Click to register mixture";
                    }
                }
            }
            else if (currentRegistryType == RegistryTypes.Component)
            {
                controlState.Tooltip = "Click to register mixture";
            }

            return controlState;
        }

        internal static ControlState GetSubmitButtonState(RegistryRecord registryRecord, PageState pageMode, RegistryTypes currentRegistryType)
        {
            ControlState controlState = new ControlState();
            controlState.Name = "Submit";
            controlState.Visible = false;

            switch (pageMode)
            {
                case PageState.AddSingleCompound:
                case PageState.EditSingleCompound:
                case PageState.AddBatch:
                case PageState.AddComponent:
                case PageState.EditComponent:
                case PageState.EditBatch:
                case PageState.EditRecord:
                    controlState.Visible = true;
                    break;
                case PageState.ViewComponent:
                    if (currentRegistryType == RegistryTypes.Mixture)
                        controlState.Visible = true;
                    break;
            }

            if (currentRegistryType == RegistryTypes.Mixture || currentRegistryType == RegistryTypes.Both)
            {
                if (pageMode != PageState.End)
                {
                    bool isValidComponentList = true;
                    bool isValidBatchList = true;
                    bool isMixture = false;
                    if (registryRecord.ComponentList.Count < Convert.ToInt32(MixtureRule.ComponentList))
                    {
                        controlState.Tooltip = "Mixtures must have more than 1 component before submission.";
                        isValidComponentList = false;
                    }
                    else if (registryRecord.BatchList[0].BatchComponentList[0].PropertyList["PERCENTAGE"].Value == string.Empty && !pageMode.ToString().Contains("Batch"))
                    {
                        controlState.Tooltip = "Mixtures must have batch component percentage information before submission";
                        isValidBatchList = false;
                    }
                    else if (registryRecord.ComponentList.Count >= Convert.ToInt32(MixtureRule.ComponentList) && pageMode.ToString().Contains("Batch"))
                    {
                        isValidComponentList = true;
                        isValidBatchList = true;
                        isMixture = true;
                    }

                    if (isMixture && isValidComponentList && isValidBatchList)
                    {
                        controlState.Visible = true;
                        controlState.Tooltip = "Click to submit mixture to temporary table";
                    }
                }
            }
            else if (currentRegistryType == RegistryTypes.Component)
            {
                controlState.Tooltip = "Click to submit mixture to temporary table";
            }

            return controlState;
        }

        internal static List<ControlState> GetFormsState(RegistryRecord registryRecord, PageState pageMode, RegistryTypes currentRegistryType)
        {
            List<ControlState> forms = new List<ControlState>();

            ControlState componentForm = new ControlState();
            componentForm.Name = "ComponentForm";
            componentForm.Visible = false;
            forms.Add(componentForm);

            ControlState compoundCustomProperties = new ControlState();
            compoundCustomProperties.Name = "CompoundCustomProperties";
            compoundCustomProperties.Visible = false;
            forms.Add(compoundCustomProperties);

            ControlState batchForm = new ControlState();
            batchForm.Name = "BatchForm";
            batchForm.Visible = false;
            forms.Add(batchForm);

            ControlState batchCustomProperties = new ControlState();
            batchCustomProperties.Name = "BatchCustomProperties";
            batchCustomProperties.Visible = false;
            forms.Add(batchCustomProperties);

            ControlState registryCustomProperties = new ControlState();
            registryCustomProperties.Name = "RegistryCustomProperties";
            registryCustomProperties.Visible = false;
            forms.Add(registryCustomProperties);

            ControlState mixtureResumeForm = new ControlState();
            mixtureResumeForm.Name = "MixtureResumeForm";
            mixtureResumeForm.Visible = false;
            forms.Add(mixtureResumeForm);

            ControlState structureCustomProperties = new ControlState();
            structureCustomProperties.Name = "StructureCustomProperties";
            structureCustomProperties.Visible = false;
            forms.Add(structureCustomProperties);

            ControlState batchComponentForm = new ControlState();
            batchComponentForm.Name = "BatchComponentForm";
            batchComponentForm.Visible = false;
            forms.Add(batchComponentForm);

            ControlState submittedComponentForm = new ControlState();
            submittedComponentForm.Name = "SubmittedComponentForm";
            submittedComponentForm.Visible = false;
            forms.Add(submittedComponentForm);

            ControlState batchComponentCustomProperties = new ControlState();
            batchComponentCustomProperties.Name = "BatchComponentCustomProperties";
            batchComponentCustomProperties.Visible = false;
            forms.Add(batchComponentCustomProperties);

            ControlState batchComponentFragmentsForm = new ControlState();
            batchComponentFragmentsForm.Name = "BatchComponentFragmentsForm";
            batchComponentFragmentsForm.Visible = false;
            forms.Add(batchComponentFragmentsForm);

            switch (pageMode)
            {
                case PageState.AddSingleCompound:
                    {
                        switch (currentRegistryType)
                        {
                            case RegistryTypes.Both:
                                registryCustomProperties.Visible = true;
                                mixtureResumeForm.Visible = true;
                                break;
                            case RegistryTypes.Mixture:
                                mixtureResumeForm.Visible = true;
                                break;
                            case RegistryTypes.Component:
                                componentForm.Visible = true;
                                compoundCustomProperties.Visible = true;
                                batchForm.Visible = true;
                                batchCustomProperties.Visible = true;
                                registryCustomProperties.Visible = true;
                                mixtureResumeForm.Visible = true;
                                structureCustomProperties.Visible = true;
                                break;
                        }
                    }
                    break;
                case PageState.EditSingleCompound:
                    {
                        switch (currentRegistryType)
                        {
                            case RegistryTypes.Both:
                                componentForm.Visible = true;
                                structureCustomProperties.Visible = true;
                                break;
                            case RegistryTypes.Mixture:
                                mixtureResumeForm.Visible = true;
                                break;
                            case RegistryTypes.Component:
                                componentForm.Visible = true;
                                compoundCustomProperties.Visible = true;
                                batchForm.Visible = true;
                                batchCustomProperties.Visible = true;
                                registryCustomProperties.Visible = true;
                                mixtureResumeForm.Visible = true;
                                structureCustomProperties.Visible = true;
                                break;
                        }
                    }
                    break;
                case PageState.AddComponent:
                    {
                        switch (currentRegistryType)
                        {
                            case RegistryTypes.Both:
                                componentForm.Visible = true;
                                compoundCustomProperties.Visible = true;
                                structureCustomProperties.Visible = true;
                                break;
                            case RegistryTypes.Mixture:
                                mixtureResumeForm.Visible = true;
                                break;
                            case RegistryTypes.Component:
                                componentForm.Visible = true;
                                compoundCustomProperties.Visible = true;
                                batchForm.Visible = true;
                                batchCustomProperties.Visible = true;
                                registryCustomProperties.Visible = true;
                                batchComponentFragmentsForm.Visible = true;
                                mixtureResumeForm.Visible = true;
                                structureCustomProperties.Visible = true;
                                break;
                        }
                    }
                    break;
                case PageState.AddBatch:
                    batchForm.Visible = true;
                    batchCustomProperties.Visible = true;
                    batchComponentForm.Visible = true;
                    structureCustomProperties.Visible = true;
                    batchComponentCustomProperties.Visible = registryRecord.ComponentList.Count > 1 ? true : false;
                    break;
                case PageState.EditComponent:
                    // TODO:
                    break;
                case PageState.EditBatch:
                    // TODO:
                    break;
                case PageState.EditRecord:
                    // TODO:
                    break;
                case PageState.ViewComponent:
                    // TODO:
                    break;
            }

            return forms;
        }
    }
}