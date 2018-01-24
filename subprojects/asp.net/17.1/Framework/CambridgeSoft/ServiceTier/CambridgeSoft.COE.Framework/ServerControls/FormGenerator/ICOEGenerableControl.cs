using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    /// <summary>
    /// <para>
    /// Every control that is intended to be rendered by a FormGenerator must implement this interface.
    /// In order to allow the proper communication between a FormGenerator containing a control, and the control
    /// itself, four members must be implemented:
    /// </para>
    /// <list type="bullet">
    ///     <item>GetData method: Defines what is being returned by a control, usually this is as simple as returning
    ///     the Text property of the base class, but sometimes this method can be quite more complex.</item>
    ///     <item>PutData method: Defines where is being set the values (usually) entered by the user. It can be as simple
    ///     as setting a Text property of the base class, but can be quite more complex, for instance in a GridView
    ///     implementation.</item>
    ///     <item>LoadFromXml method: Each control will need some specific information, is responsibility of each control
    ///     to define and to understand/parse its own configuration in some xml like format.</item>
    ///     <item>DefaultValue property: When the control is initialized this property may be called.</item>
    /// </list>
    /// </summary>
    public interface ICOEGenerableControl {
        /// <summary>
        /// <para>Returns whatever the underlying control has defined as the main property to communicate with a 
        /// FormGenerator</para>
        /// </summary>
        /// <returns>An object defined by the underlying control.</returns>
        object GetData();

        /// <summary>
        /// <para>
        /// Sets the control's main property used to communicate with a FormGenerator.
        /// </para>
        /// </summary>
        /// <param name="data">An object with the values being sent by the user to the FormGenerator</param>
        void PutData(object data);
        
        /// <summary>
        /// <para>
        /// Method intended to delegate specific configurations to the underlying control. This config info will
        /// come inside an xml tag <b>&lt;configInfo&gt;</b>.
        /// </para>
        /// </summary>
        /// <param name="xmlDataAsString">A string with xml format that contains control's configurations</param>
        void LoadFromXml(string xmlDataAsString);

        /// <summary>
        /// Gets or sets the default value of the control.
        /// </summary>
        string DefaultValue {
            get;
            set;
        }
    }
}
