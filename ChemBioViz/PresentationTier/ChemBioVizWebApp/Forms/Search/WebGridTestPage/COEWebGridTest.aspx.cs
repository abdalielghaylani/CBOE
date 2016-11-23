using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.WebGridTestPage
{
    public partial class COEWebGridTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            COEWebGrid1.LoadFromXml(funXmlString());
            COEWebGrid1.PutData(GridDataBase());
        }

        private string funXmlString()
        {
            string strXml = "<coeForms><coeForm id='0' dataMember='' dataSourceId='ChemBioVizSearchCSLADataSource'>" +
            " <layoutInfo><formElement><defaultValue/><Id>ListView</Id><bindingExpression>Dataset</bindingExpression><configInfo>" +
            " <fieldConfig>	<tables><table name='Table_210'><CSSClass>myTableClass</CSSClass><headerStyle>color: #FFF; background-color: rgb(0, 153, 255); font-weight: bold; font-family: Verdana; font-size: 10px;</headerStyle>" +
            " <Columns><Column>	<height>153px</height><width>20%</width><formElement name='EmpId'><Id>EmpId</Id><displayInfo> " +
            " <type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly</type> " +
            " </displayInfo><configInfo><fieldConfig><Height>140px</Height><Width>140px</Width></fieldConfig> " +
            " </configInfo>	</formElement></Column><Column><width>40%</width><formElement name='Name'> " +
            " <configInfo><fieldConfig>	<CSSClass>COELabelListItems</CSSClass></fieldConfig></configInfo> " +
            " </formElement></Column><Column><width>10%</width>	<formElement name='Designation'>  <configInfo> " +
            " <fieldConfig><CSSClass>COELabelListItems</CSSClass></fieldConfig></configInfo></formElement></Column><Column><width>15%</width> " +
            " <formElement name='Department'><configInfo><fieldConfig><CSSClass>COELabelListItems</CSSClass></fieldConfig></configInfo> " +
            " </formElement></Column><Column><width>15%</width><formElement name='Unit'><displayInfo> " +
            " <type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEShowDetailLinkButton</type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column>	<Column><width>15%</width><formElement name='DOB'><displayInfo> " +
            " <type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEShowDetailLinkButton</type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='DOJ'><displayInfo> " +
            " <type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEShowDetailLinkButton</type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='DOL'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='BossID'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='Address1'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='Address2'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='Address3'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='City'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='State'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='PIN'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='Religon'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='EMailID'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column><Column><width>15%</width><formElement name='Web Site'><displayInfo> " +
            " <type></type> " +
            " </displayInfo><configInfo><fieldConfig><CSSClass>LinkButton</CSSClass><Text>Show Details</Text></fieldConfig></configInfo></formElement> " +
            " </Column></Columns></table></tables></fieldConfig></configInfo><displayInfo><top>20px</top><left>1px</left><width>745px</width> " +
            " <height>1550px</height><type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView</type></displayInfo></formElement> " +
            " </layoutInfo></coeForm></coeForms>";
            return strXml;
        }


        private DataSet GridDataBase()
        {
            DataSet dsGrid = new DataSet();

            dsGrid.Tables.Add("Emp_Detail");
            dsGrid.Tables["Emp_Detail"].Columns.Add("EmpId");
            dsGrid.Tables["Emp_Detail"].Columns.Add("Name");
            dsGrid.Tables["Emp_Detail"].Columns.Add("Designation");
            dsGrid.Tables["Emp_Detail"].Columns.Add("Department");
            dsGrid.Tables["Emp_Detail"].Columns.Add("Unit");
            dsGrid.Tables["Emp_Detail"].Columns.Add("DOB");
            dsGrid.Tables["Emp_Detail"].Columns.Add("DOJ");
            dsGrid.Tables["Emp_Detail"].Columns.Add("DOL");
            dsGrid.Tables["Emp_Detail"].Columns.Add("BossID");

            dsGrid.Tables["Emp_Detail"].Columns.Add("Address1");
            dsGrid.Tables["Emp_Detail"].Columns.Add("Address2");
            dsGrid.Tables["Emp_Detail"].Columns.Add("Address3");
            dsGrid.Tables["Emp_Detail"].Columns.Add("City");
            dsGrid.Tables["Emp_Detail"].Columns.Add("State");
            dsGrid.Tables["Emp_Detail"].Columns.Add("PIN");
            dsGrid.Tables["Emp_Detail"].Columns.Add("Religon");
            dsGrid.Tables["Emp_Detail"].Columns.Add("EMailID");
            dsGrid.Tables["Emp_Detail"].Columns.Add("Web Site");

            ///////////////////////////////////////////////////

            dsGrid.Tables.Add("Dept_Master");
            dsGrid.Tables["Dept_Master"].Columns.Add("DeptId");
            dsGrid.Tables["Dept_Master"].Columns.Add("DeptDetail");
            dsGrid.Tables["Dept_Master"].Columns.Add("SDeptID");

            /////////////////////////////////////////////////////

            dsGrid.Tables.Add("Desg_Master");
            dsGrid.Tables["Desg_Master"].Columns.Add("DesgId");
            dsGrid.Tables["Desg_Master"].Columns.Add("DesgDetail");
            dsGrid.Tables["Desg_Master"].Columns.Add("SDesgID");
            dsGrid.Tables["Desg_Master"].Columns.Add("Grade");
            dsGrid.Tables["Desg_Master"].Columns.Add("Salary");

            ////////////////////////////////////////////////////

            dsGrid.Tables.Add("Education");
            dsGrid.Tables["Education"].Columns.Add("EmpId");
            dsGrid.Tables["Education"].Columns.Add("EduId");
            dsGrid.Tables["Education"].Columns.Add("Details");

            ///////////////////////////////////////////////////

            dsGrid.Tables.Add("ContactNo");
            dsGrid.Tables["ContactNo"].Columns.Add("EmpId");
            dsGrid.Tables["ContactNo"].Columns.Add("CNo");
            dsGrid.Tables["ContactNo"].Columns.Add("Status");

            /////////////////////////////////////////////////

            DataRow newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "1";
            newRow["DeptDetail"] = "Department 1";
            newRow["SDeptID"] = "";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "2";
            newRow["DeptDetail"] = "Department 2";
            newRow["SDeptID"] = "";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "3";
            newRow["DeptDetail"] = "Department 3";
            newRow["SDeptID"] = "";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "4";
            newRow["DeptDetail"] = "Department 4";
            newRow["SDeptID"] = "1";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "5";
            newRow["DeptDetail"] = "Department 5";
            newRow["SDeptID"] = "2";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "6";
            newRow["DeptDetail"] = "Department 6";
            newRow["SDeptID"] = "3";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "7";
            newRow["DeptDetail"] = "Department 7";
            newRow["SDeptID"] = "3";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "8";
            newRow["DeptDetail"] = "Department 8";
            newRow["SDeptID"] = "4";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "9";
            newRow["DeptDetail"] = "Department 9";
            newRow["SDeptID"] = "4";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Dept_Master"].NewRow();
            newRow["DeptId"] = "10";
            newRow["DeptDetail"] = "Department 0";
            newRow["SDeptID"] = "5";

            dsGrid.Tables["Dept_Master"].Rows.Add(newRow);


            /////////////////////////////////////////

            newRow = dsGrid.Tables["Desg_Master"].NewRow();
            newRow["DesgId"] = "1";
            newRow["DesgDetail"] = "Designation 1";
            newRow["SDesgID"] = "";
            newRow["Grade"] = "I";
            newRow["Salary"] = "100000";

            dsGrid.Tables["Desg_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Desg_Master"].NewRow();
            newRow["DesgId"] = "2";
            newRow["DesgDetail"] = "Designation 2";
            newRow["SDesgID"] = "";
            newRow["Grade"] = "I";
            newRow["Salary"] = "100000";

            dsGrid.Tables["Desg_Master"].Rows.Add(newRow);


            newRow = dsGrid.Tables["Desg_Master"].NewRow();
            newRow["DesgId"] = "3";
            newRow["DesgDetail"] = "Designation 3";
            newRow["SDesgID"] = "1";
            newRow["Grade"] = "II";
            newRow["Salary"] = "70000";

            dsGrid.Tables["Desg_Master"].Rows.Add(newRow);


            newRow = dsGrid.Tables["Desg_Master"].NewRow();
            newRow["DesgId"] = "4";
            newRow["DesgDetail"] = "Designation 4";
            newRow["SDesgID"] = "2";
            newRow["Grade"] = "II";
            newRow["Salary"] = "70000";

            dsGrid.Tables["Desg_Master"].Rows.Add(newRow);


            newRow = dsGrid.Tables["Desg_Master"].NewRow();
            newRow["DesgId"] = "5";
            newRow["DesgDetail"] = "Designation 5";
            newRow["SDesgID"] = "3";
            newRow["Grade"] = "II";
            newRow["Salary"] = "60000";

            dsGrid.Tables["Desg_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Desg_Master"].NewRow();
            newRow["DesgId"] = "6";
            newRow["DesgDetail"] = "Designation 6";
            newRow["SDesgID"] = "3";
            newRow["Grade"] = "II";
            newRow["Salary"] = "50000";

            dsGrid.Tables["Desg_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Desg_Master"].NewRow();
            newRow["DesgId"] = "7";
            newRow["DesgDetail"] = "Designation 7";
            newRow["SDesgID"] = "3";
            newRow["Grade"] = "III";
            newRow["Salary"] = "50000";

            dsGrid.Tables["Desg_Master"].Rows.Add(newRow);

            newRow = dsGrid.Tables["Desg_Master"].NewRow();
            newRow["DesgId"] = "8";
            newRow["DesgDetail"] = "Designation 8";
            newRow["SDesgID"] = "7";
            newRow["Grade"] = "IV";
            newRow["Salary"] = "30000";

            dsGrid.Tables["Desg_Master"].Rows.Add(newRow);

            //////////////////////////////////////////////

            for (int i = 1; i <= 500; i++)
            {
                newRow = dsGrid.Tables["Emp_Detail"].NewRow();
                newRow["EmpId"] = i;
                newRow["Name"] = "Name" + i;


                if (i % 9 == 0 || i % 9 == 1)
                    newRow["Department"] = "1";
                if (i % 8 == 0)
                    newRow["Department"] = "2";
                if (i % 7 == 0)
                    newRow["Department"] = "3";
                if (i % 6 == 0)
                    newRow["Department"] = "4";
                if (i % 5 == 0)
                    newRow["Department"] = "5";
                if (i % 4 == 0)
                    newRow["Department"] = "6";
                if (i % 3 == 0)
                    newRow["Department"] = "7";
                if (i % 2 == 0)
                    newRow["Department"] = "8";
                if (i % 11 == 0)
                    newRow["Department"] = "10";


                if (i % 9 == 0 || i % 9 == 1)
                    newRow["Designation"] = "1";
                if (i % 8 == 0)
                    newRow["Designation"] = "2";
                if (i % 7 == 0)
                    newRow["Designation"] = "3";
                if (i % 6 == 0)
                    newRow["Designation"] = "4";
                if (i % 5 == 0)
                    newRow["Designation"] = "5";
                if (i % 4 == 0)
                    newRow["Designation"] = "6";
                if (i % 3 == 0)
                    newRow["Designation"] = "7";
                if (i % 2 == 0)
                    newRow["Designation"] = "8";




                newRow["Unit"] = "1";
                newRow["DOB"] = "";
                newRow["DOJ"] = "";
                newRow["DOL"] = "";

                newRow["BossID"] = "1";
                newRow["Address1"] = "Address1" + i;
                newRow["Address2"] = "Address2" + i;
                newRow["Address3"] = "Address3" + i;
                newRow["City"] = "City" + i;
                newRow["State"] = "State" + i;
                newRow["PIN"] = "PIN" + i;
                newRow["Religon"] = "Religon" + i;
                newRow["EMailID"] = "EMailID@" + i + "com";
                newRow["Web Site"] = "Web Site" + i + ".com";

                dsGrid.Tables["Emp_Detail"].Rows.Add(newRow);

                //////////////////////////////////////////////////
                if (i % 2 == 0)
                {
                    newRow = dsGrid.Tables["Education"].NewRow();
                    newRow["EmpId"] = i;
                    newRow["EduId"] = "1";
                    newRow["Details"] = "M.C.A.";

                    dsGrid.Tables["Education"].Rows.Add(newRow);
                }
                else
                {
                    newRow = dsGrid.Tables["Education"].NewRow();
                    newRow["EmpId"] = i;
                    newRow["EduId"] = "1";
                    newRow["Details"] = "M.Tech.";

                    dsGrid.Tables["Education"].Rows.Add(newRow);
                }

                if (i % 3 == 0)
                {
                    newRow = dsGrid.Tables["Education"].NewRow();
                    newRow["EmpId"] = i;
                    newRow["EduId"] = "2";
                    newRow["Details"] = "B.Tech.";

                    dsGrid.Tables["Education"].Rows.Add(newRow);
                }
                else if (i % 2 == 0)
                {
                    newRow = dsGrid.Tables["Education"].NewRow();
                    newRow["EmpId"] = i;
                    newRow["EduId"] = "2";
                    newRow["Details"] = "B.Sc.";

                    dsGrid.Tables["Education"].Rows.Add(newRow);
                }
                //////////////////////////////////////////////////

                if (i % 3 == 0)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                newRow = dsGrid.Tables["ContactNo"].NewRow();
                                newRow["EmpId"] = i;
                                newRow["CNo"] = "98765" + i;
                                newRow["Status"] = "Home";

                                dsGrid.Tables["ContactNo"].Rows.Add(newRow);
                                break;
                            case 1:
                                newRow = dsGrid.Tables["ContactNo"].NewRow();
                                newRow["EmpId"] = i;
                                newRow["CNo"] = "1121" + i;
                                newRow["Status"] = "Office";

                                dsGrid.Tables["ContactNo"].Rows.Add(newRow);
                                break;
                        }
                    }
                }
                /////////////////////////////////////////////////////////
            }
            dsGrid.Relations.Add("Rel1", dsGrid.Tables["EMP_Detail"].Columns["EmpId"], dsGrid.Tables["ContactNo"].Columns["EmpId"]);
            dsGrid.Relations.Add("Rel2", dsGrid.Tables["Dept_Master"].Columns["DeptId"], dsGrid.Tables["EMP_Detail"].Columns["Department"]);
            dsGrid.Relations.Add("Rel3", dsGrid.Tables["Desg_Master"].Columns["DesgId"], dsGrid.Tables["EMP_Detail"].Columns["Designation"]);
            dsGrid.Relations.Add("Rel4", dsGrid.Tables["EMP_Detail"].Columns["EmpId"], dsGrid.Tables["Education"].Columns["EmpId"]);
            return dsGrid;
        }

    }
}
