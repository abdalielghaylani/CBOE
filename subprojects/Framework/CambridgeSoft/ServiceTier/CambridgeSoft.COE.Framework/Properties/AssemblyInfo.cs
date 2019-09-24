using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Common;


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: System.Reflection.AssemblyTitle("CambridgeSoft.COE.Framework")]
[assembly: System.Reflection.AssemblyDescription("")]
[assembly: System.Reflection.AssemblyConfiguration("")]
[assembly: System.Reflection.AssemblyCompany("PerkinElmer")]
[assembly: System.Reflection.AssemblyProduct("CambridgeSoft.COE.Framework")]
[assembly: System.Reflection.AssemblyCopyright("© Copyright 1998-2019 PerkinElmer Informatics, Inc.")]
[assembly: System.Reflection.AssemblyTrademark("")]
[assembly: System.Reflection.AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3241e0ca-46d7-4049-a7fd-9d2b084b2d95")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: System.Reflection.AssemblyVersion("19.1.0.0")]
[assembly: AssemblyFileVersion("12.5.0.0")]

        //[assembly: InternalsVisibleTo("CambridgeSoft.COE.Framework.UnitTests")]
        [assembly: WebResource(AssemblyInfoConst.Next_B, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Previous_B, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.First_Track_1, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Last_Track_1, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Next_B_d, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Previous_B_d, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.First_Track_1_d, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Last_Track_1_d, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.InProgress, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.Rec_B, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Rec_B_d, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.error, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.DownloadChemDraw, "image/jpg")]
        [assembly: WebResource(AssemblyInfoConst.ThumbUp, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.ThumbDown, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.st_any, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.st_down, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.st_down_thick, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.st_either, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.st_up, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.st_up_thick, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.std_cis, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.std_either, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.std_trans, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.disable, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.down, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.up, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.searching, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.help, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.positioning, "text/javascript")]
        [assembly: WebResource(AssemblyInfoConst.centerUnMarked, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.centerMarked, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.right, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.left, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.Warning_h, "image/png")]
        //Exporting Server control
        [assembly: WebResource(AssemblyInfoConst.export_btn, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.cancel_btn, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Add_h, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.File_Edit, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Folder, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.File_Add, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Remove_h, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.resultcriteria, "text/xml")]
        [assembly: WebResource(AssemblyInfoConst.LiveDataview, "text/xml")]
        [assembly: WebResource(AssemblyInfoConst.LiveResultCriteria, "text/xml")]

        [assembly: WebResource(AssemblyInfoConst.Add_Document, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Arrow_Down_B, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Arrow_Up_B, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Block, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Check, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Close_Window, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Details, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Edit, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Export, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Save, "image/png")]
        [assembly: WebResource(AssemblyInfoConst.Stop, "image/png")]

        [assembly: CambridgeSoft.COE.Framework.AssemblyMinClientVersion("12.4.0.0")]
        [assembly: CambridgeSoft.COE.Framework.AssemblyMinSchemaVersion("12.4.0")]

        //PageControlSetting
        [assembly: WebResource(AssemblyInfoConst.Styles_PageControl, "text/css")]
        [assembly: WebResource(AssemblyInfoConst.file, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.folder, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.buttonimage, "image/gif")]
        [assembly: WebResource(AssemblyInfoConst.backgroundtoolbar, "image/gif")]
    

