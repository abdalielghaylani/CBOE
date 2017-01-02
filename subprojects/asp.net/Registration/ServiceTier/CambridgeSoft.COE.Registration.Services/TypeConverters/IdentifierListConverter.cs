using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    public class IdentifierListConverter : TypeConverter
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
                return this.CreateIdentifierList(culture, ((DataSet) value).Tables[0].Rows);
            }
            else if(value is DataTable)
            {
                return this.CreateIdentifierList(culture, ((DataTable) value).Rows);
            }
            else if(value is DataRowCollection)
            {
                return this.CreateIdentifierList(culture, ((DataRowCollection) value));
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value is IdentifierList)
            {
                if(destinationType == typeof(DataSet))
                {
                    DataSet ds = new DataSet();
                    ds.Tables.Add(this.CreateDataTable(culture, value as IdentifierList));
                    return ds;
                }
                else if(value == typeof(DataTable))
                {
                    return this.CreateDataTable(culture, value as IdentifierList);
                }
                else if(value == typeof(DataRowCollection))
                {
                    return this.CreateDataTable(culture, value as IdentifierList).Rows;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private DataTable CreateDataTable(CultureInfo provider, IdentifierList value)
        {
            DataTable table = new DataTable("IdentifierList");
            table.Columns.Add(new DataColumn("UniqueID", typeof(string)));
            table.Columns.Add(new DataColumn("ID", typeof(int)));
            table.Columns.Add(new DataColumn("IdentifierID", typeof(int)));
            table.Columns.Add(new DataColumn("Name", typeof(string)));
            table.Columns.Add(new DataColumn("Description", typeof(string)));
            table.Columns.Add(new DataColumn("InputText", typeof(string)));
            table.Columns.Add(new DataColumn("Active", typeof(bool)));
            table.Columns.Add(new DataColumn("OrderIndex", typeof(int)));
            foreach(Identifier identifier in value)
            {
                DataRow row = table.NewRow();
                row[0] = identifier.UniqueID.ToString(provider);
                row[1] = identifier.ID.ToString(provider);
                row[2] = identifier.IdentifierID.ToString(provider);
                row[3] = identifier.Name.ToString(provider);
                row[4] = identifier.Description.ToString(provider);
                row[4] = identifier.InputText.ToString(provider);
                row[5] = identifier.Active.ToString(provider);
                row[6] = identifier.OrderIndex.ToString(provider);
                
                if(identifier.IsNew)
                    row.SetAdded();
                else if(identifier.IsDirty)
                    row.SetModified();
                else if(identifier.IsDeleted)
                    row.Delete();

                table.Rows.Add(row);
            }
            return table;
        }

        private IdentifierList CreateIdentifierList(CultureInfo provider, DataRowCollection rows)
        {
            IdentifierList identifierList = IdentifierList.NewIdentifierList();
            Identifier identifier = null;
            for(int i = 0; i < rows.Count; i++)
            {
                if(rows[i].RowState == DataRowState.Deleted)
                {
                    identifier = Identifier.NewIdentifier("<Identifier />", false, false);
                    identifier.ID = Convert.ToInt32(rows[i]["ID", DataRowVersion.Original]);
                    identifier.IdentifierID = Convert.ToInt32(rows[i]["IdentifierID", DataRowVersion.Original]);
                    identifierList.Add(identifier);
                    identifierList.RemoveAt(identifierList.Count - 1);
                }
                else if(rows[i].RowState == DataRowState.Unchanged || rows[i].RowState == DataRowState.Modified)
                {
                    identifier = Identifier.NewIdentifier("<Identifier />", false, false);
                    if(rows[i].Table.Columns.Contains("ID"))
                        identifier.ID = Convert.ToInt32(rows[i]["ID"]);
                    if(rows[i].Table.Columns.Contains("IdentifierID"))
                        identifier.IdentifierID = Convert.ToInt32(rows[i]["IdentifierID"]);
                    if(rows[i].Table.Columns.Contains("Name"))
                        identifier.Name = Convert.ToString(rows[i]["Name"]);
                    if(rows[i].Table.Columns.Contains("Description"))
                        identifier.Description = Convert.ToString(rows[i]["Description"]);
                    if(rows[i].Table.Columns.Contains("InputText"))
                        identifier.InputText = Convert.ToString(rows[i]["InputText"]);
                    if(rows[i].Table.Columns.Contains("Active"))
                        identifier.Active = Convert.ToBoolean(rows[i]["Active"]);
                    identifierList.Add(identifier);
                }
                else if(rows[i].RowState == DataRowState.Added)
                {
                    identifier = Identifier.NewIdentifier("<Identifier />", true, true);
                    if(rows[i].Table.Columns.Contains("ID"))
                        identifier.ID = Convert.ToInt32(rows[i]["ID"]);
                    if(rows[i].Table.Columns.Contains("IdentifierID"))
                        identifier.IdentifierID = Convert.ToInt32(rows[i]["IdentifierID"]);
                    if(rows[i].Table.Columns.Contains("Name"))
                        identifier.Name = Convert.ToString(rows[i]["Name"]);
                    if(rows[i].Table.Columns.Contains("Description"))
                        identifier.Description = Convert.ToString(rows[i]["Description"]);
                    if(rows[i].Table.Columns.Contains("InputText"))
                        identifier.InputText = Convert.ToString(rows[i]["InputText"]);
                    if(rows[i].Table.Columns.Contains("Active"))
                        identifier.Active = Convert.ToBoolean(rows[i]["Active"]);
                    identifierList.Add(identifier);
                }
            }
            return identifierList;
        }
    }
}
