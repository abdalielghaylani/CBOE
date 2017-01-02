using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    /// <summary>
    /// <para>
    /// This interface defines methods and properties that are usefull for those controls that are intended to
    /// contain other controls, and moreover, that may contain datasources.
    /// </para>
    /// </summary>
    public interface ICOEContainer {
        /// <summary>
        /// <para>
        /// Gets or set the container's display mode. Every container should allow one of the following modes:
        /// </para>
        /// <list type="bullet">
        ///     <item>Add</item>
        ///     <item>Edit</item>
        ///     <item>Query</item>
        ///     <item>View</item>
        /// </list>
        /// </summary>
        CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode DisplayMode { get; set; }
        /// <summary>
        /// <para>If the container will have pagging capabilities, this property should be implemented to 
        /// get or set the current page index.</para>
        /// </summary>
        int PageIndex { get; set;}
        /// <summary>
        /// <para>Gets or sets if the container should be rendered or not.</para>
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// <para>Gets or sets the container's datasource.</para>
        /// <remarks>DataSource and DataSourceID mutually exclusive.</remarks>
        /// </summary>
        object DataSource { get; set; }
        /// <summary>
        /// <para>Gets or sets the container's datasourceid.</para>
        /// <remarks>DataSource and DataSourceID mutually exclusive.</remarks>
        /// </summary>
        string DataSourceID { get; set; }
        /// <summary>
        /// <para>Triggers the databinding process.</para>
        /// </summary>
        void DataBind();
        /// <summary>
        /// <para>
        /// If used with DataSourceID this should start its datasource update process, otherwise it should
        /// fire some "Updating" events.
        /// </para>
        /// </summary>
        void Update();
    }
}
