/*  GREATIS FORM DESIGNER FOR .NET
 *  Some Services Implementation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms.Design;



namespace Greatis
{
   namespace FormDesigner
   {
       #region Project Members
       //class DTEProject : EnvDTE.Project
       //{
       //    PrjItems projectItems = new PrjItems();

       //    public EnvDTE.CodeModel CodeModel
       //    {
       //        get { return null; }
       //    }

       //    public EnvDTE.Projects Collection
       //    {
       //        get { return null; }
       //    }

       //    public EnvDTE.ConfigurationManager ConfigurationManager
       //    {
       //        get { return null; }
       //    }

       //    public EnvDTE.DTE DTE
       //    {
       //        get { return null; }
       //    }

       //    public void Delete()
       //    {
       //    }

       //    public string ExtenderCATID
       //    {
       //        get { return null; }
       //    }

       //    public object ExtenderNames
       //    {
       //        get { return null; }
       //    }

       //    public string FileName
       //    {
       //        get { return ""; }
       //    }

       //    public string FullName
       //    {
       //        get { return ""; }
       //    }

       //    public EnvDTE.Globals Globals
       //    {
       //        get { return null; }
       //    }

       //    public bool IsDirty
       //    {
       //        get
       //        {
       //            return false;
       //        }
       //        set
       //        {
       //        }
       //    }

       //    public string Kind
       //    {
       //        get { return ""; }
       //    }

       //    public string Name
       //    {
       //        get
       //        {
       //            return "";
       //        }
       //        set
       //        {
       //        }
       //    }

       //    public object Object
       //    {
       //        get { return null; }
       //    }

       //    public EnvDTE.ProjectItem ParentProjectItem
       //    {
       //        get { return null; }
       //    }

       //    public EnvDTE.ProjectItems ProjectItems
       //    {
       //        get { return projectItems; }
       //    }

       //    public EnvDTE.Properties Properties
       //    {
       //        get { return null; }
       //    }

       //    public void Save(string FileName)
       //    {
       //    }

       //    public void SaveAs(string NewFileName)
       //    {
       //    }

       //    public bool Saved
       //    {
       //        get
       //        {
       //            return true;
       //        }
       //        set
       //        {
       //        }
       //    }

       //    public string UniqueName
       //    {
       //        get { return ""; }
       //    }

       //    public object get_Extender(string ExtenderName)
       //    {
       //        return null;
       //    }
       //}
       #endregion

       #region ProjectItems Members
       //class PrjItems : EnvDTE.ProjectItems
       //{
       //    public EnvDTE.ProjectItem AddFolder(string Name, string Kind)
       //    {
       //        return null;
       //    }

       //    public EnvDTE.ProjectItem AddFromDirectory(string Directory)
       //    {
       //        return null;
       //    }

       //    public EnvDTE.ProjectItem AddFromFile(string FileName)
       //    {
       //        return null;
       //    }

       //    public EnvDTE.ProjectItem AddFromFileCopy(string FilePath)
       //    {
       //        return null;
       //    }

       //    public EnvDTE.ProjectItem AddFromTemplate(string FileName, string Name)
       //    {
       //        return null;
       //    }

       //    public EnvDTE.Project ContainingProject
       //    {
       //        get { return null; }
       //    }

       //    public int Count
       //    {
       //        get { return 0; }
       //    }

       //    public EnvDTE.DTE DTE
       //    {
       //        get { return null; }
       //    }

       //    public System.Collections.IEnumerator GetEnumerator()
       //    {
       //        return null;
       //    }

       //    public EnvDTE.ProjectItem Item(object index)
       //    {
       //        return null;
       //    }

       //    public string Kind
       //    {
       //        get { return ""; }
       //    }

       //    public object Parent
       //    {
       //        get { return null; }
       //    }
       //}
       #endregion

       #region ProjectItem Members
       //class DTE : ProjectItem
       //{
       //    DTEProject project = new DTEProject();

       //    public ProjectItems Collection
       //    {
       //        get { return null; }
       //    }

       //    public ConfigurationManager ConfigurationManager
       //    {
       //        get { return null; }
       //    }

       //    public Project ContainingProject
       //    {
       //        get { return project; }
       //    }

       //    EnvDTE.DTE ProjectItem.DTE
       //    {
       //        get { return null; }
       //    }

       //    public void Delete()
       //    {
       //    }

       //    public Document Document
       //    {
       //        get { return null; }
       //    }

       //    public void ExpandView()
       //    {
       //    }

       //    public string ExtenderCATID
       //    {
       //        get { return ""; }
       //    }

       //    public object ExtenderNames
       //    {
       //        get { return null; }
       //    }

       //    public FileCodeModel FileCodeModel
       //    {
       //        get { return null; }
       //    }

       //    public short FileCount
       //    {
       //        get { return 0; }
       //    }

       //    public bool IsDirty
       //    {
       //        get
       //        {
       //            return false;
       //        }
       //        set
       //        {
       //        }
       //    }

       //    public string Kind
       //    {
       //        get { return ""; }
       //    }

       //    public string Name
       //    {
       //        get
       //        {
       //            return "";
       //        }
       //        set
       //        {
       //        }
       //    }

       //    public object Object
       //    {
       //        get { return null; }
       //    }

       //    public Window Open(string ViewKind)
       //    {
       //        return null;
       //    }

       //    public ProjectItems ProjectItems
       //    {
       //        get { return null; }
       //    }

       //    public Properties Properties
       //    {
       //        get { return null; }
       //    }

       //    public void Remove()
       //    {
       //    }

       //    public void Save(string FileName)
       //    {
       //    }

       //    public bool SaveAs(string NewFileName)
       //    {
       //        return false;
       //    }

       //    public bool Saved
       //    {
       //        get
       //        {
       //            return false;
       //        }
       //        set
       //        {
       //        }
       //    }

       //    public Project SubProject
       //    {
       //        get { return null; }
       //    }

       //    public object get_Extender(string ExtenderName)
       //    {
       //        return null;
       //    }

       //    public string get_FileNames(short index)
       //    {
       //        return "";
       //    }

       //    public bool get_IsOpen(string ViewKind)
       //    {
       //        return false;
       //    }
       //}
       #endregion
   }
}