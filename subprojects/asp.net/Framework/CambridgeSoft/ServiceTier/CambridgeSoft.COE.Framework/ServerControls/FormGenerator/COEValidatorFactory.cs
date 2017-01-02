using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Globalization;

namespace CambridgeSoft.COE.Framework.ServerControls.FormGenerator
{
    public class COEValidatorFactory
    {
        public static BaseValidator GetValidator(FormGroup.ValidationRuleInfo coeValidationRule, WebControl controlToValidate, COEFormGenerator formGenerator)
        {
            BaseValidator validator = null;
            if (formGenerator != null && !formGenerator.Page.ClientScript.IsStartupScriptRegistered(typeof(COEFormGenerator), "ValidatorUpdateDisplayAsBlock"))
            {
                string overrideValidatorJS = @"
                ValidatorUpdateDisplay = ValidatorUpdateDisplayAsBlock;
                function ValidatorUpdateDisplayAsBlock(val) {
                    if (typeof(val.display) == 'string') {
                        if (val.display == 'None') {
                            return;
                        }
                        if (val.display == 'Dynamic') {
                            val.style.display = val.isvalid ? 'none' : 'inline-block';
                            return;
                        }
                    }
                    if ((navigator.userAgent.indexOf('Mac') > -1) && (navigator.userAgent.indexOf('MSIE') > -1)) {
                        val.style.display = 'inline';
                    }
                    val.style.visibility = val.isvalid ? 'hidden' : 'visible';
                }";
                formGenerator.Page.ClientScript.RegisterStartupScript(typeof(COEFormGenerator), "ValidatorUpdateDisplayAsBlock", overrideValidatorJS, true);
            }
            if (formGenerator != null && coeValidationRule != null)  //Coverity Fix CID 11654 ASV
            {
                switch (coeValidationRule.ValidationRuleName)
                {
                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.RequiredField:
                        validator = new RequiredFieldValidator();
                        if (controlToValidate is ICOERequireable)
                        {
                            ((ICOERequireable)controlToValidate).Required = true;
                        }
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;
                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.NumericRange:
                        validator = new RangeValidator();
                        ((RangeValidator)validator).Type = ValidationDataType.Double;
                        foreach (CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Parameter param in coeValidationRule.Params)
                        {
                            switch (param.Name.ToLower().Trim())
                            {
                                case Constants.Validator_Min:
                                    ((RangeValidator)validator).MinimumValue = param.Value;
                                    break;
                                case Constants.Validator_Max:
                                    ((RangeValidator)validator).MaximumValue = param.Value;
                                    break;
                            }
                        }
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;
                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.Integer:
                        validator = new CompareValidator();
                        ((CompareValidator)validator).Type = ValidationDataType.Integer;
                        ((CompareValidator)validator).Operator = ValidationCompareOperator.DataTypeCheck;
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;
                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.Date:
                        validator = new CompareValidator();
                        ((CompareValidator)validator).Type = ValidationDataType.Date;
                        ((CompareValidator)validator).Operator = ValidationCompareOperator.DataTypeCheck;
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;
                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.Double:
                        validator = new CompareValidator();
                        ((CompareValidator)validator).Type = ValidationDataType.Double;
                        ((CompareValidator)validator).Operator = ValidationCompareOperator.DataTypeCheck;
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;
                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.PositiveInteger:
                        validator = new CompareValidator();
                        ((CompareValidator)validator).ValueToCompare = "0";
                        ((CompareValidator)validator).Type = ValidationDataType.Integer;
                        ((CompareValidator)validator).Operator = ValidationCompareOperator.GreaterThanEqual;
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;
                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.TextLength:
                        validator = new RegularExpressionValidator();
                        ((RegularExpressionValidator)validator).ValidationExpression = "(.|\r|\n){@min,@max}";
                        foreach (CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Parameter param in coeValidationRule.Params)
                        {
                            switch (param.Name.ToLower().Trim())
                            {
                                case Constants.Validator_Min:
                                    ((RegularExpressionValidator)validator).ValidationExpression = ((RegularExpressionValidator)validator).ValidationExpression.Replace("@min", param.Value.Trim());
                                    break;
                                case Constants.Validator_Max:
                                    ((RegularExpressionValidator)validator).ValidationExpression = ((RegularExpressionValidator)validator).ValidationExpression.Replace("@max", param.Value.Trim());
                                    break;
                            }
                        }
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;

                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.Float:
                        validator = new RegularExpressionValidator();
                        string decimalSeparator = "[\\.,]";
                        ((RegularExpressionValidator)validator).ValidationExpression = @"^(-?\d{1,@intergerPart})(" + decimalSeparator + @"\d{0,@decimaPart})?$|^(-?" + decimalSeparator + @"\d{1,@decimaPart})?$";
                        foreach (CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Parameter param in coeValidationRule.Params)
                        {
                            switch (param.Name.ToLower().Trim())
                            {
                                case Constants.Validator_IntegerPart:
                                    ((RegularExpressionValidator)validator).ValidationExpression = ((RegularExpressionValidator)validator).ValidationExpression.Replace("@intergerPart", param.Value.Trim());
                                    break;
                                case Constants.Validator_DecimalPart:
                                    ((RegularExpressionValidator)validator).ValidationExpression = ((RegularExpressionValidator)validator).ValidationExpression.Replace("@decimaPart", param.Value.Trim());
                                    break;
                            }
                        }
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;

                    case CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleEnum.WordListEnumeration:
                        validator = new RegularExpressionValidator();
                        ((RegularExpressionValidator)validator).ValidationExpression = string.Empty;
                        int i = 0;
                        foreach (CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Parameter param in coeValidationRule.Params)
                        {
                            switch (param.Name.ToLower().Trim())
                            {
                                case Constants.Validatior_ValidWord:
                                    ((RegularExpressionValidator)validator).ValidationExpression += param.Value.Trim();
                                    break;
                            }
                            if (++i < coeValidationRule.Params.Count)
                            {
                                ((RegularExpressionValidator)validator).ValidationExpression += "|";
                            }
                        }
                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;
                    case FormGroup.ValidationRuleEnum.Custom:
                        validator = new CustomValidator();

                        string scriptName = coeValidationRule.ValidationRuleName + "For" + controlToValidate.ID + "clientScript";

                        foreach (FormGroup.Parameter param in coeValidationRule.Params)
                        {
                            if (param.Name.ToLower().Trim() == "clientscript" && !string.IsNullOrEmpty(param.Value))
                            {
                                string scriptBody = string.Format("<script type=\"text/javascript\">function {0}(source, arguments) {{ {1} }}</script>",
                                                                                     scriptName,
                                                                                     formGenerator.ReplaceControlNames(param.Value));

                                if (!formGenerator.Page.ClientScript.IsClientScriptBlockRegistered(scriptName))
                                    formGenerator.Page.ClientScript.RegisterClientScriptBlock(typeof(COEFormGenerator), scriptName, scriptBody);

                                ((CustomValidator)validator).ClientValidationFunction = scriptName;
                                ((CustomValidator)validator).ValidateEmptyText = true;
                            }
                        }

                        validator.EnableClientScript = true;
                        validator.ControlToValidate = controlToValidate.ID;
                        break;

                    case FormGroup.ValidationRuleEnum.NotEmptyStructure:
                        validator = new CustomValidator();

                        string jscriptName = coeValidationRule.ValidationRuleName + "For" + controlToValidate.ID + "clientScript";
                        string jscript = "if( cd_isBlankStructure('@{0}', '')){{arguments.IsValid = false;}}else{{arguments.IsValid = true;}}";
                        foreach (FormGroup.Parameter param in coeValidationRule.Params)
                        {
                            if (param.Name.ToLower().Trim() == "controlid" && !string.IsNullOrEmpty(param.Value))
                            {
                                jscript = string.Format(jscript, param.Value);
                                string scriptBody = string.Format("<script type=\"text/javascript\">function {0}(source, arguments) {{ {1} }}</script>",
                                                                                     jscriptName,
                                                                                     formGenerator.ReplaceControlNames(jscript));

                                if (!formGenerator.Page.ClientScript.IsClientScriptBlockRegistered(jscriptName))
                                    formGenerator.Page.ClientScript.RegisterClientScriptBlock(typeof(COEFormGenerator), jscriptName, scriptBody);

                                ((CustomValidator)validator).ClientValidationFunction = jscriptName;
                                ((CustomValidator)validator).ValidateEmptyText = true;
                            }
                        }
                        break;
                    case FormGroup.ValidationRuleEnum.NotEmptyStructureAndNoText:
                        validator = new CustomValidator();

                        string jscriptNameNotxt = coeValidationRule.ValidationRuleName + "For" + controlToValidate.ID + "clientScript";
                        //string textPattern = "NO STRUCTURE";
                        string textMatchExpression = string.Empty;
                        string controlId = null;
                        foreach (FormGroup.Parameter param in coeValidationRule.Params)
                        {
                            if (param.Name.ToLower().Trim() == "textpattern" && !string.IsNullOrEmpty(param.Value))
                            {
                                textMatchExpression += " && cd_getData('@{0}', 'text/xml').indexOf('" + param.Value + "') < 0";
                            }
                            if (param.Name.ToLower().Trim() == "controlid" && !string.IsNullOrEmpty(param.Value))
                            {
                                controlId = param.Value;
                            }
                        }

                        string jscriptNotxt = string.Format("if( cd_getData('@{0}', 'chemical/x-daylight-smiles') == ''" + textMatchExpression + "){{arguments.IsValid = false;}}else{{arguments.IsValid = true;}}", controlId);

                        string scriptBodyNotxt = string.Format("<script type=\"text/javascript\">function {0}(source, arguments) {{ {1} }}</script>",
                                                         jscriptNameNotxt,
                                                         formGenerator.ReplaceControlNames(jscriptNotxt));

                        if (!formGenerator.Page.ClientScript.IsClientScriptBlockRegistered(jscriptNameNotxt))
                            formGenerator.Page.ClientScript.RegisterClientScriptBlock(typeof(COEFormGenerator), jscriptNameNotxt, scriptBodyNotxt);

                        ((CustomValidator)validator).ClientValidationFunction = jscriptNameNotxt;
                        ((CustomValidator)validator).ValidateEmptyText = true;
                        break;
                    case FormGroup.ValidationRuleEnum.NotEmptyQuery:
                        validator = new CustomValidator();
                        jscriptName = coeValidationRule.ValidationRuleName.ToString();
                        jscript =
                            "var parentElement = source.parentElement.parentElement;\n" +
                            "var elements=parentElement.getElementsByTagName('input');" + "\n" +
                            "var elements_select=parentElement.getElementsByTagName('select');" + "\n" +
                            "var elements_textarea=parentElement.getElementsByTagName('textarea'); \n" +
                            "var availablePlugInCalls = typeof(cd_getSpecificObject) == 'function' && typeof(cd_getMolWeight) == 'function' ? true : false;" + "\n" +
                            "var isEmpty = true;" + "\n" +
                            "if(availablePlugInCalls == true){" + "\n" +
                            "    for(var ix = 0; ix < cd_objectArray.length; ix++) {" + "\n" +
                            "        if(cd_getSpecificObject(cd_objectArray[ix]) != null && cd_getMolWeight(cd_objectArray[ix]) > 0) {" + "\n" +
                            "            isEmpty = false;" + "\n" +
                            "            break;" + "\n" +
                            "        }" + "\n" +
                            "    }" + "\n" +
                            "}" + "\n" +
                            "if(isEmpty == true) {" + "\n" +
                            "    for(var ix = 0; ix < elements.length; ix++) {" + "\n" +
                            "        if((elements[ix].type == 'text' || elements[ix].type == 'file' || elements[ix].type == 'password' || elements[ix].type == 'textarea') && elements[ix].name.indexOf('StructureQueryControl') < 0 && elements[ix].value != '') {" + "\n" +
                            "            isEmpty = false;" + "\n" +
                            "            break;" + "\n" +
                            "        }" + "\n" +
                            "        else if(elements[ix].type == 'select-one' && elements[ix].name.indexOf('StructureQueryControl') < 0 && elements[ix].selectedIndex > 0) {" + "\n" +
                            "            isEmpty = false;" + "\n" +
                            "            break;" + "\n" +
                            "        }" + "\n" +
                            "        else if(elements[ix].type == 'select-multiple' && elements[ix].name.indexOf('StructureQueryControl') < 0) {" + "\n" +
                            "            for(var selectIx = 0; selectIx < elements[ix].options.length; selectIx++) {" + "\n" +
                            "                if(elements[ix].options[selectIx].selected) {" + "\n" +
                            "                    isEmpty = false;" + "\n" +
                            "                    break;" + "\n" +
                            "                }" + "\n" +
                            "            }" + "\n" +
                            "            if(!isEmpty)" + "\n" +
                            "                break;" + "\n" +
                            "        }" + "\n" +
                            "        else if(elements[ix].type == 'radio' && elements[ix].name.indexOf('StructureQueryControl') < 0) {" + "\n" +
                            "            for(var radioIx = 0; radioIx < elements[ix].length; radioIx++) {" + "\n" +
                            "                if(elements[ix][radioIx].checked) {" + "\n" +
                            "                   isEmpty = false;" + "\n" +
                            "                   break;" + "\n" +
                            "                }" + "\n" +
                            "            }" + "\n" +
                            "            if(!isEmpty)" + "\n" +
                            "                break;" + "\n" +
                            "        }" + "\n" +
                            "        else if(elements[ix].type == 'checkbox' && elements[ix].name.indexOf('StructureQueryControl') < 0) {" + "\n" +
                            "                if(elements[ix].checked) {" + "\n" +
                            "                   isEmpty = false;" + "\n" +
                            "                   if(!isEmpty)" + "\n" +
                            "                       break;" + "\n" +
                            "            }" + "\n" +
                            "        }" + "\n" +
                            "    }" + "\n" +
                            "    for(var ix = 0; ix < elements_select.length; ix++) {" + "\n" +
                            "        if(elements_select[ix].value != '' && elements_select[ix].selectedIndex > 0) {" + "\n" +
                            "           isEmpty = false;" + "\n" +
                            "           break;" + "\n" +
                            "        }" + "\n" +
                            "    }" + "\n" +
                            "    for(var ix = 0; ix < elements_textarea.length; ix++) {" + "\n" +
                            "        if(elements_textarea[ix].value != '') {" + "\n" +
                            "           isEmpty = false;" + "\n" +
                            "           break;" + "\n" +
                            "        }" + "\n" +
                            "    }" + "\n" +
                            "}" + "\n" +
                            "arguments.IsValid = (isEmpty==false);";

                        if (!formGenerator.Page.ClientScript.IsClientScriptBlockRegistered(typeof(COEFormGenerator), jscriptName))
                            formGenerator.Page.ClientScript.RegisterClientScriptBlock(typeof(COEFormGenerator), jscriptName, string.Format("\n<script type=\"text/javascript\">function {0}(source, arguments) {{ {1} }}</script>",
                                                                                     jscriptName,
                                                                                     jscript));

                        ((CustomValidator)validator).ClientValidationFunction = jscriptName;
                        validator.EnableClientScript = true;
                        ((CustomValidator)validator).ValidateEmptyText = true;

                        break;
                    case FormGroup.ValidationRuleEnum.CasValidation:
                        validator = new CustomValidator();

                        string validationFunction =
                        "<script type=\"text/javascript\">function IsValidRegistryID(registryID)" +
                        @"{
                        var retVal = false;
                        if(registryID.length == 0)
                            retVal = true;
                        else
                        {
                            var regExpCASRN = new RegExp(" + "\"(^%?[0-9]{1,11}%?$|^%?[0-9]{1,6}[-][0-9]{1,2}%$|^%?[0-9]{1,6}-[0-9]{1,2}-*[0-9*]$)\"" + @");
                            var regExpCASRN1 = new RegExp(" + "\"(^[-][0-9]+$|^[-][0-9]*[-][0-9]+$|^[-][0-9]*[-]+$|^[0-9]{1,11}$)\"" + @");
                            var regExpACX = new RegExp(" + "\"(^[Xx][0-9]{3,7}$|^[Xx][0-9]{3,7}[%*]?$|^[Xx][0-9]{3,7}-[0-9]{1}$)\", \"i\"" + @");  
                            if(regExpCASRN.test(registryID) && !regExpCASRN1.test(registryID))
                                retVal = regExpCASRN.test(registryID);
                            else if (regExpACX.test(registryID))
                                retVal = regExpACX.test(registryID);
                        }
                        return retVal;
                    }" +
                        "</script>";

                        if (!formGenerator.Page.ClientScript.IsClientScriptBlockRegistered("ValidRegistryID"))
                            formGenerator.Page.ClientScript.RegisterClientScriptBlock(typeof(COEFormGenerator), "ValidRegistryID", validationFunction);

                        jscriptName = coeValidationRule.ValidationRuleName + "For" + controlToValidate.ID + "clientScript";
                        string clientValidationFunction =
    "<script type=\"text/javascript\">function " + jscriptName + "(source, arguments)" +
    @"{ 
    arguments.IsValid = IsValidRegistryID(arguments.Value);
}</script>";

                        if (!formGenerator.Page.ClientScript.IsClientScriptBlockRegistered(jscriptName))
                            formGenerator.Page.ClientScript.RegisterClientScriptBlock(typeof(COEFormGenerator), jscriptName, clientValidationFunction);

                        ((CustomValidator)validator).ClientValidationFunction = jscriptName;
                        ((CustomValidator)validator).EnableClientScript = true;
                        ((CustomValidator)validator).ValidateEmptyText = true;
                        validator.ControlToValidate = controlToValidate.ID;

                        break;
                }

                if (validator != null)
                {
                    validator.ID = coeValidationRule.ValidationRuleName + "For" + controlToValidate.ID;
                    validator.Attributes["text"] = coeValidationRule.ErrorMessage;
                    //validator.ValidationGroup = "FormGroup";

                    if (!string.IsNullOrEmpty(coeValidationRule.ValidationGroup))
                        validator.ValidationGroup = coeValidationRule.ValidationGroup;

                    if (coeValidationRule.Display != ValidatorDisplay.None)
                    {
                        validator.ErrorMessage = coeValidationRule.ErrorMessage;
                        validator.Text = string.Empty;
                        validator.Display = coeValidationRule.Display;
                    }
                    else
                    {
                        validator.Display = ValidatorDisplay.None;
                        validator.ErrorMessage = coeValidationRule.ErrorMessage;
                        validator.Text = string.Empty;
                    }
                }
            }
            return validator;
        }
    }
}
