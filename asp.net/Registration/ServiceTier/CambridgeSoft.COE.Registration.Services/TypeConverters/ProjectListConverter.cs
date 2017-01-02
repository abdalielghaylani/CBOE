using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    public class ProjectListConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if(sourceType == typeof(DataSet) || sourceType == typeof(DataTable) || sourceType == typeof(DataRowCollection))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if(destinationType == typeof(DataSet) || destinationType == typeof(DataTable) || destinationType == typeof(DataRowCollection))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if(value is DataSet)
            {
                return this.CreateProjectList(culture, ((DataSet) value).Tables[0].Rows);
            }
            else if(value is DataTable)
            {
                return this.CreateProjectList(culture, ((DataTable) value).Rows);
            }
            else if(value is DataRowCollection)
            {
                return this.CreateProjectList(culture, ((DataRowCollection) value));
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value is ProjectList)
            {
                if(destinationType == typeof(DataSet))
                {
                    DataSet ds = new DataSet();
                    ds.Tables.Add(this.CreateDataTable(culture, value as ProjectList));
                    return ds;
                }
                else if(value == typeof(DataTable))
                {
                    return this.CreateDataTable(culture, value as ProjectList);
                }
                else if(value == typeof(DataRowCollection))
                {
                    return this.CreateDataTable(culture, value as ProjectList).Rows;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private DataTable CreateDataTable(CultureInfo provider, ProjectList value)
        {
            DataTable table = new DataTable("ProjectList");
            table.Columns.Add(new DataColumn("UniqueID", typeof(string)));
            table.Columns.Add(new DataColumn("ID", typeof(int)));
            table.Columns.Add(new DataColumn("ProjectID", typeof(int)));
            table.Columns.Add(new DataColumn("Name", typeof(string)));
            table.Columns.Add(new DataColumn("Description", typeof(string)));
            table.Columns.Add(new DataColumn("Active", typeof(bool)));
            table.Columns.Add(new DataColumn("OrderIndex", typeof(int)));
            foreach(Project proj in value)
            {
                DataRow row = table.NewRow();
                row[0] = proj.UniqueID.ToString(provider);
                row[1] = proj.ID.ToString(provider);
                row[2] = proj.ProjectID.ToString(provider);
                row[3] = proj.Name.ToString(provider);
                row[4] = proj.Description.ToString(provider);
                row[5] = proj.Active.ToString(provider);
                row[6] = proj.OrderIndex.ToString(provider);

                if(proj.IsNew)
                    row.SetAdded();
                else if(proj.IsDirty)
                    row.SetModified();
                else if(proj.IsDeleted)
                    row.Delete();

                table.Rows.Add(row);
            }
            return table;
        }

        private ProjectList CreateProjectList(CultureInfo provider, DataRowCollection rows)
        {
            ProjectList projectList = ProjectList.NewProjectList();
            Project project = null;
            for(int i = 0; i < rows.Count; i++)
            {
                if(rows[i].RowState == DataRowState.Deleted)
                {
                    project = Project.NewProject("<Project />", false,false);
                    project.ID = Convert.ToInt32(rows[i]["ID", DataRowVersion.Original]);
                    project.ProjectID = Convert.ToInt32(rows[i]["ProjectID", DataRowVersion.Original]);
                    projectList.Add(project);
                    projectList.RemoveAt(projectList.Count - 1);
                }
                else if(rows[i].RowState == DataRowState.Unchanged || rows[i].RowState == DataRowState.Modified)
                {
                    project = Project.NewProject("<Project />", false,false);
                    if(rows[i].Table.Columns.Contains("ID"))
                        project.ID = Convert.ToInt32(rows[i]["ID"]);
                    if(rows[i].Table.Columns.Contains("ProjectID"))
                        project.ProjectID = Convert.ToInt32(rows[i]["ProjectID"]);
                    if(rows[i].Table.Columns.Contains("Name"))
                        project.Name = Convert.ToString(rows[i]["Name"]);
                    if(rows[i].Table.Columns.Contains("Description"))
                        project.Description = Convert.ToString(rows[i]["Description"]);
                    if(rows[i].Table.Columns.Contains("Active"))
                        project.Active = Convert.ToBoolean(rows[i]["Active"]);
                    projectList.Add(project);
                }
                else if(rows[i].RowState == DataRowState.Added)
                {
                    project = Project.NewProject("<Project />", true,true);
                    if(rows[i].Table.Columns.Contains("ID"))
                        project.ID = Convert.ToInt32(rows[i]["ID"]);
                    if(rows[i].Table.Columns.Contains("ProjectID"))
                        project.ProjectID = Convert.ToInt32(rows[i]["ProjectID"]);
                    if(rows[i].Table.Columns.Contains("Name"))
                        project.Name = Convert.ToString(rows[i]["Name"]);
                    if(rows[i].Table.Columns.Contains("Description"))
                        project.Description = Convert.ToString(rows[i]["Description"]);
                    if(rows[i].Table.Columns.Contains("Active"))
                        project.Active = Convert.ToBoolean(rows[i]["Active"]);
                    projectList.Add(project);
                }
            }
            return projectList;
        }
    }
}
