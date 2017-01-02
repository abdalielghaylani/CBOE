using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;

namespace CambridgeSoft.COE.Framework.Controls.ChemDraw
{
    [ToolboxData("<{0}:COEChemDrawEmbedBoundField runat=server></{0}:COEChemDrawEmbedBoundField>")]
    [Description("Displays the ChemOfice plugin application as a bound field.")]
    public class COEChemDrawEmbedBoundField : BoundField
    {
        private COEChemDrawEmbed _chemDraw;


        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            if((rowState & DataControlRowState.Edit) != 0 || (rowState & DataControlRowState.Insert) != 0)
            {
                _chemDraw = new COEChemDrawEmbed();
                _chemDraw.ViewOnly = true;
                _chemDraw.MimeType = MimeTypes.xcdx;
                cell.Controls.Add(_chemDraw);
            }

            if(!String.IsNullOrEmpty(DataField))
                cell.DataBinding +=new EventHandler(OnDataBindField);

            base.InitializeDataCell(cell, rowState);
        }

        protected override void OnDataBindField(object sender, EventArgs e)
        {
            DataControlFieldCell cell = sender as DataControlFieldCell;
            if(cell != null)
            {
                object dataValue = GetValue(cell.Parent);
                if(dataValue == null)
                    throw new Exception("No value found");

                if(cell.Controls.Count > 0)
                {
                    COEChemDrawEmbed chem = cell.Controls[0] as COEChemDrawEmbed;
                    if(chem != null)
                        chem.InlineData = dataValue.ToString();
                }
                else
                {
                    _chemDraw = new COEChemDrawEmbed();
                    _chemDraw.ViewOnly = true;
                    _chemDraw.MimeType = MimeTypes.xcdx;
                    cell.Controls.Add(_chemDraw);
                }
            }
        }

    }
}
