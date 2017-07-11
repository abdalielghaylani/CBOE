using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerkinElmer.COE.Registration.Server.Code
{
    /// <summary>
    /// Sub forms in record details view
    /// </summary>
    public enum SubForms
    {
        /// <summary>
        /// MixtureResumeForm
        /// </summary>
        MixtureResumeForm = 0,

        /// <summary>
        /// ComponentForm
        /// </summary>
        ComponentForm = 1,

        /// <summary>
        /// SubmittedComponentForm
        /// </summary>
        SubmittedComponentForm = 2,

        /// <summary>
        /// BatchComponentForm
        /// </summary>
        BatchComponentForm = 3,

        /// <summary>
        /// BatchForm
        /// </summary>
        BatchForm = 4,

        /// <summary>
        /// BatchComponentFragmentsForm
        /// </summary>
        BatchComponentFragmentsForm = 5,

        /// <summary>
        /// BatchComponentListFragmentsForm
        /// </summary>
        BatchComponentListFragmentsForm = 6,

        /// <summary>
        /// RegistryCustomProperties
        /// </summary>
        RegistryCustomProperties = 1000,

        /// <summary>
        /// CompoundCustomProperties
        /// </summary>
        CompoundCustomProperties = 1001,

        /// <summary>
        /// BatchCustomProperties
        /// </summary>
        BatchCustomProperties = 1002,

        /// <summary>
        /// BatchComponentCustomProperties
        /// </summary>
        BatchComponentCustomProperties = 1003,

        /// <summary>
        /// StructureCustomProperties
        /// </summary>
        StructureCustomProperties = 1004
    }

    /// <summary>
    /// The page state of record view
    /// </summary>
    public enum PageState
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// EditRecord
        /// </summary>
        EditRecord,

        /// <summary>
        /// AddSingleCompound
        /// </summary>
        AddSingleCompound,

        /// <summary>
        /// EditSingleCompound
        /// </summary>
        EditSingleCompound,

        /// <summary>
        /// AddComponent
        /// </summary>
        AddComponent,

        /// <summary>
        /// EditComponent
        /// </summary>
        EditComponent,

        /// <summary>
        /// ViewComponent
        /// </summary>
        ViewComponent,

        /// <summary>
        /// AddBatch
        /// </summary>
        AddBatch,

        /// <summary>
        /// EditBatch
        /// </summary>
        EditBatch,

        /// <summary>
        /// End
        /// </summary>
        End,

        /// <summary>
        /// EndComponent
        /// </summary>
        EndComponent,

        /// <summary>
        /// ViewBatch
        /// </summary>
        ViewBatch,

        /// <summary>
        /// DisplayUserPreference
        /// </summary>
        DisplayUserPreference
    }

    /// <summary>
    /// The registry types
    /// </summary>
    public enum RegistryTypes
    {
        /// <summary>
        /// Mixture type
        /// </summary>
        Mixture,

        /// <summary>
        /// Component type
        /// </summary>
        Component,

        /// <summary>
        /// Both Mixture and Component type
        /// </summary>
        Both,
    }

    public enum MixtureRule
    {
        /// <summary>
        /// projectList
        /// </summary>
        ProjectList = 1, 

        /// <summary>
        /// ComponentList
        /// </summary>
        ComponentList = 2,

        /// <summary>
        /// batchList
        /// </summary>
        BatchList = 1
    }
}