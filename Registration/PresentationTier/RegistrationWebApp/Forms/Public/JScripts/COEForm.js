var m_listStructureId = null;

function RestoreOriginalStructure(baseFragmentId, baseFragmentNormalizedId)
{
    if(window.confirm('Are you sure you want revert to original structure?'))
    {
        var originalStructure = cd_getData(baseFragmentNormalizedId, 'chemical/x-cdx');
        cd_putData(baseFragmentId, 'chemical/x-daylight-smiles', originalStructure);
        document.getElementById(baseFragmentId + '_Output').value = cd_getData(baseFragmentId, 'chemical/x-cdx');
        SetDrawingTypeForRestoreLink(baseFragmentId); 
    }
    return false;
}

function SetDrawingTypeForRestoreLink(baseFragmentId) {
    var list = document.getElementById(m_listStructureId);
    if (list != null) {
        var hiddenField = document.getElementById('structureToolbarOption');
        if (hiddenField != null) {
            COEChemDrawToolbar_SetDrawStructureItemValue(list, hiddenField.value);
        var checkedValue = COEChemDrawToolbar_GetSelectedItemValue(list);
        if (checkedValue == '0')
            cd_getSpecificObject(baseFragmentId).viewOnly = false;
        else if (checkedValue == '-2' || checkedValue == '-1' || checkedValue == '-3')
            cd_getSpecificObject(baseFragmentId).viewOnly = true;
        }
    }
}

function SetDefaultDrawing(baseFragmentId, checkedValue) {
    var noStructure = 'VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAEwAAAENoZW1EcmF3IDEyLjBkNjcyCAATAAAAVW50aXRsZWQgRG9jdW1lbnQEAhAAAIBsAABAYgAzc3oAM7O8AAEJCAAAAAAAAAAAAAIJCAAAAOEAAAAsAQ0IAQABCAcBAAE6BAEAATsEAQAARQQBAAE8BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABEBAEAAAoICAADAGAAyAADAAsICAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAHCAQAAAABAAYIBAAAAAQABQgEAAAAHgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAECCBAAAAAkAAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgBgAAAAEAhAAAAAAAAAAAAAAANACAAAcAhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEADwgCAAEABoAWAAAAAAIIAABAdwAAQGIABAIQAACAbAAAQGIAM3N6ADOzvAAKAAIAAQAQACYAAABDaGVtRHJhdyBjYW4ndCBpbnRlcnByZXQgdGhpcyBsYWJlbC4CBwIAAQAABxgAAQAAAAQAAADwAAMATk8gU1RSVUNUVVJFAAAAAAAAAAA=';
    var nonChemical = 'VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAEwAAAENoZW1EcmF3IDEyLjBk NjcyCAATAAAAVW50aXRsZWQgRG9jdW1lbnQEAhAAAEBbAADAYwAzM2kAzAz6AAEJ CAAAAPT/AAD0/wIJCAAAAOEAAADhAA0IAQABCAcBAAE6BAEAATsEAQAARQQBAAE8 BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABEBAEAAAoICAADAGAAyAADAAsI CAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAHCAQAAAABAAYIBAAAAAQABQgEAAAA HgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAECCBAAAAAk AAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAA AP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABJAAAAAIAAwDkBAUAQXJp YWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAQAAAAEAhAAAAAAAAAAAAAAANACAAAc AhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEADwgCAAEABoACAAAAAAIIAAAAZgAA wGMABAIQAABAWwAAwGMAMzNpAMwM+gAKAAIAAQAQACYAAABDaGVtRHJhdyBjYW4n dCBpbnRlcnByZXQgdGhpcyBsYWJlbC4CBwIAAQAAByAAAQAAAAQAAADwAAMATk9O IENIRU1JQ0FMIENPTlRFTlQAAAAAAAAAAA==';
    var wildcard = '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 NgJ3HigAooooAf/ZAAAAAAAAAAA=';

    var choice;
    switch (checkedValue) {
        case '-3': choice = nonChemical; break;
        case '-2': choice = noStructure; break;
        case '-1': choice = wildcard; break;
        case '0': choice = ''; break; s
        default: choice = ''; break;
    }

    if (!choice == '') {
        PutChemDrawData(baseFragmentId, choice);
    }
}

function CheckBoxList_OnClick(baseFragmentId, listStructureId)
{
    var noStructure = 'VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAEwAAAENoZW1EcmF3IDEyLjBkNjcyCAATAAAAVW50aXRsZWQgRG9jdW1lbnQEAhAAAIBsAABAYgAzc3oAM7O8AAEJCAAAAAAAAAAAAAIJCAAAAOEAAAAsAQ0IAQABCAcBAAE6BAEAATsEAQAARQQBAAE8BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABEBAEAAAoICAADAGAAyAADAAsICAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAHCAQAAAABAAYIBAAAAAQABQgEAAAAHgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAECCBAAAAAkAAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgBgAAAAEAhAAAAAAAAAAAAAAANACAAAcAhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEADwgCAAEABoAWAAAAAAIIAABAdwAAQGIABAIQAACAbAAAQGIAM3N6ADOzvAAKAAIAAQAQACYAAABDaGVtRHJhdyBjYW4ndCBpbnRlcnByZXQgdGhpcyBsYWJlbC4CBwIAAQAABxgAAQAAAAQAAADwAAMATk8gU1RSVUNUVVJFAAAAAAAAAAA=';
    var nonChemical = 'VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAEwAAAENoZW1EcmF3IDEyLjBk NjcyCAATAAAAVW50aXRsZWQgRG9jdW1lbnQEAhAAAEBbAADAYwAzM2kAzAz6AAEJ CAAAAPT/AAD0/wIJCAAAAOEAAADhAA0IAQABCAcBAAE6BAEAATsEAQAARQQBAAE8 BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQBAABEBAEAAAoICAADAGAAyAADAAsI CAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAHCAQAAAABAAYIBAAAAAQABQgEAAAA HgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAoCAEAASkIAQABKggBAAECCBAAAAAk AAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAADMgAIAP///////wAAAAAAAP//AAAA AP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wABJAAAAAIAAwDkBAUAQXJp YWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAQAAAAEAhAAAAAAAAAAAAAAANACAAAc AhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEADwgCAAEABoACAAAAAAIIAAAAZgAA wGMABAIQAABAWwAAwGMAMzNpAMwM+gAKAAIAAQAQACYAAABDaGVtRHJhdyBjYW4n dCBpbnRlcnByZXQgdGhpcyBsYWJlbC4CBwIAAQAAByAAAQAAAAQAAADwAAMATk9O IENIRU1JQ0FMIENPTlRFTlQAAAAAAAAAAA==';
    var wildcard = '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 NgJ3HigAooooAf/ZAAAAAAAAAAA=';
    var list = document.getElementById(listStructureId);
    
    var checkedValue = COEChemDrawToolbar_GetSelectedItemValue(list)
    var choice;
    var doReplacement = false;

    switch(checkedValue)
    {
        case '-3': choice = nonChemical; break;
        case '-2': choice = noStructure; break;
        case '-1': choice = wildcard; break;
        case '0': choice = ''; break;
        default: choice = ''; break;
    }

    if (!choice == '')
    {
        //IF there is a structure already drawn, verify with the user before replacing it
        if(cd_getData(baseFragmentId, 'chemical/x-daylight-smiles') != '')
        {
            if(window.confirm('Are you sure you want to continue? You will lose any previous structure'))
            {
                doReplacement = true;
            }
            else {
                // Set to Draw Structure on cancel click
                COEChemDrawToolbar_SetDrawStructureItemValue(list,0);
                return false;
            }
        }
        else
        {
                doReplacement = true;
        }
        
        if (doReplacement == true)
        {
            PutChemDrawData(baseFragmentId, choice);
            return true;
        }        
    }
    else
    {
        PutChemDrawData(baseFragmentId, choice);
        return true;
    }
}

/* Applies a 'structure' to the ChemDraw ActiveX cntrol */
function PutChemDrawData(baseFragmentId, structureChoice)
{
    cd_putData(baseFragmentId, 'chemical/x-daylight-smiles', structureChoice);
    document.getElementById(baseFragmentId + '_Output').value = cd_getData(baseFragmentId, 'chemical/x-cdx');
    //only true chemical strucutres should be user-editable
    var viewOnly = (structureChoice != '');
    cd_getSpecificObject(baseFragmentId).viewOnly = viewOnly;
}

function AfterExitEdit(cellId) {
    if(igtbl_getCellById(cellId).Column.Key == 'InputText') {
        var regex = new RegExp("[&;<;>;%]", "ig");
        if (!(igtbl_getCellById(cellId).getValue() == null) && regex.test(igtbl_getCellById(cellId).getValue())) {
            alert('Invalid text entered.The symbols ' + ' &,<,>,% ' +' are not allowed');
            if (igtbl_getCellById(cellId).isEditable()) {
                var cellValue = igtbl_getCellById(cellId).getValue().toString();
                cellValue = cellValue.toString().replace(regex, '').replace(/^\s+|\s+$/g,""); // Replace regular expression and do trim().
                igtbl_getCellById(cellId).setValue(cellValue);
                igtbl_getCellById(cellId).beginEdit();
            }
        }
        else if (igtbl_getCellById(cellId).getPrevCell().getValue() == null) 
        {
            if(igtbl_getCellById(cellId).getValue() != null)
            {
                alert('Please assign an identifier type');
                arguments.IsValid = false;
                igtbl_getCellById(cellId).getPrevCell().setValue(null);
                if(igtbl_getCellById(cellId).getPrevCell().isEditable())
                    igtbl_getCellById(cellId).getPrevCell().beginEdit();
            }
        }
    }
    else if(igtbl_getCellById(cellId).Column.Key == 'IdentifierID')
    {
        if(igtbl_getCellById(cellId).getValue() == null)
            igtbl_getCellById(cellId).getNextCell().setValue(null);
    }
}
function OnSubmit(baseFragmentStructureId, listStructureId)
{
    var list = document.getElementById(listStructureId);
    if(list != null) {
        var checkedValue = COEChemDrawToolbar_GetSelectedItemValue(list);
        
        if(!cd_isBlankStructure(baseFragmentStructureId, '') && checkedValue == '0')
        {
            var isNonChemical = cd_getMolWeight(baseFragmentStructureId, '') <= 0;
//            if(isNonChemical) {
//                if (window.confirm('You\'re submitting non chemical content. Are you sure you want to proceed?')) {
//                    COEChemDrawToolbar_SetSelectedItemValue(list, "-3");
//                    return true;
//                }
//                else
//                {
//                  YAHOO.ChemOfficeEnterprise.ProgressModal.MasterProgressModal.hide();
//                  ShowChemDraws();
//                  return false;                  
//                }
//            }
      }

  }
  return true;
}

function OnStartUp(baseFragmentStructureId, listStructureId)
{
    var list = document.getElementById(listStructureId);
    m_listStructureId = listStructureId;

    if(list != null)
    {
        var hiddenField = document.getElementById('structureToolbarOption');
        if (hiddenField != null) {
            COEChemDrawToolbar_SetDrawStructureItemValue(list, hiddenField.value);
        }

        var checkedValue = COEChemDrawToolbar_GetSelectedItemValue(list);
        if (checkedValue == '0')
            cd_getSpecificObject(baseFragmentStructureId).viewOnly = false;
        else if (checkedValue == '-2' || checkedValue == '-1' || checkedValue == '-3') {
            cd_getSpecificObject(baseFragmentStructureId).viewOnly = true;
            SetDefaultDrawing(baseFragmentStructureId, checkedValue);
        }
            
        //Add select compound javascript that refers to structure form element.
        var selectComponentSubmit = document.getElementById('ctl00_ContentPlaceHolder_SearchComponentButton');
        if(selectComponentSubmit != null)
            selectComponentSubmit.onclick = function(){return confirmSelectCompound(baseFragmentStructureId, 'You\'re about to overwrite the current compound. Do you want to continue?');};
    }
}

function confirmSelectCompound(baseFragmentStructureId, confirmationMessage)
{
 if(cd_getData(baseFragmentStructureId, 'chemical/x-daylight-smiles') != '')
 {
    return window.confirm(confirmationMessage);
 }
 
 return true;
}