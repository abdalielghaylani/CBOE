let GroupTestDataInput = 
[
  { 'itemType': 'group', 'caption': 'Structure Information', 'cssClass': 'tan', 'colCount': 5, 
    'items': [   
      { 'label': { 'text': 'Structure' }, 'editorType': 'dxTextBox', 'dataField': 'BaseFragmentStructureEndComponent',
          'template': 'structureImageTemplate', 'editorOptions': { 'readOnly': true } },
      { 'label': { 'text': 'Normalized Structure' }, 'editorType': 'dxTextBox', 'dataField': 'BaseFragmentNormalizedStructureEndComponent',
          'template': 'structureImageTemplate', 'editorOptions': { 'readOnly': true } }, 
      { 'label': { 'text': 'MF' }, 'editorType': 'dxTextBox', 'dataField': 'FormulaLabelEndComponent', 
          'template': 'textTemplate', 'editorOptions': { 'readOnly': true } },     
      { 'label': { 'text': 'MW' }, 'editorType': 'dxTextBox', 'dataField': 'MolWeightLabel', 
          'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
      { 'label': { 'text': 'Structure Name' }, 'editorType': 'dxTextBox', 'dataField': 'STRUCT_NAMEProperty', 
          'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
      { 'label': { 'text': 'Structure Comments' }, 'editorType': 'dxTextBox', 'dataField': 'STRUCT_COMMENTSProperty', 
          'template': 'textTemplate', 'editorOptions': { 'readOnly': true } } ] },
  { 'itemType': 'group', 'caption': 'Component Information', 'cssClass': 'tan', 'colCount': 5,
    'items': [
      { 'label': { 'text': 'Component Identifiers' }, 'editorType': 'dxTextBox', 'dataField': 'Compound_IdentifiersUltraGrid', 
          'template': 'idListTemplate', 'editorOptions': { 'idListType': 'C', 'dataField': 'Compound_IdentifiersUltraGrid', 'readOnly': false }, 'colSpan': 2 },
      { 'label': { 'text': 'Component Comments' }, 'editorType': 'dxTextBox', 'dataField': 'CMP_COMMENTSProperty', 
          'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
      { 'label': { 'text': 'Stereochemistry Comments' }, 'editorType': 'dxTextBox', 'dataField': 'STRUCTURE_COMMENTS_TXTProperty', 
          'template': 'textTemplate', 'editorOptions': { 'readOnly': true } } ] }, 
  { 'itemType': 'group', 'caption': 'Fragment Information', 'cssClass': 'tan', 'colCount': 5, 
    'items': [
      { 'label': { 'visible': false }, 'editorType': 'dxTextBox', 'dataField': 'BatchComponentFragmentList', 
          'template': 'fragmentsTemplate', 'colSpan': 5, 'editorOptions': { 'readOnly': false } 
      } ] }
];

let GroupTestDataOuput = [
  { 'label': { 'text': 'Structure' }, 'editorType': 'dxTextBox', 'dataField': 'BaseFragmentStructureEndComponent',
    'template': 'structureImageTemplate', 'editorOptions': { 'readOnly': true } }, 
  { 'label': { 'text': 'Normalized Structure' }, 'editorType': 'dxTextBox', 'dataField': 'BaseFragmentNormalizedStructureEndComponent',
    'template': 'structureImageTemplate', 'editorOptions': { 'readOnly': true } }, 
  { 'label': { 'text': 'MF' }, 'editorType': 'dxTextBox', 'dataField': 'FormulaLabelEndComponent',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
  { 'label': { 'text': 'MW' }, 'editorType': 'dxTextBox', 'dataField': 'MolWeightLabel',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
  { 'label': { 'text': 'Structure Name' }, 'editorType': 'dxTextBox', 'dataField': 'STRUCT_NAMEProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
  { 'label': { 'text': 'Structure Comments' }, 'editorType': 'dxTextBox', 'dataField': 'STRUCT_COMMENTSProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
  { 'label': { 'text': 'Component Identifiers' }, 'editorType': 'dxTextBox', 'dataField': 'Compound_IdentifiersUltraGrid',
    'template': 'idListTemplate', 'editorOptions': { 'idListType': 'C', 'dataField': 'Compound_IdentifiersUltraGrid', 'readOnly': false }, 'colSpan': 2 }, 
  { 'label': { 'text': 'Component Comments' }, 'editorType': 'dxTextBox', 'dataField': 'CMP_COMMENTSProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
  { 'label': { 'text': 'Stereochemistry Comments' }, 'editorType': 'dxTextBox', 'dataField': 'STRUCTURE_COMMENTS_TXTProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': true } }, 
  { 'label': { 'visible': false }, 'editorType': 'dxTextBox', 'dataField': 'BatchComponentFragmentList',
    'template': 'fragmentsTemplate', 'colSpan': 5, 'editorOptions': { 'readOnly': false } }
];

let NonGroupTestDataInput = [ 
  { 'label': { 'text': 'TempBatchID'}, 'editorType': 'dxTextBox', 'dataField': 'BatchIDTextBox',  
    'template': 'textTemplate', 'editorOptions': {'readOnly': false } }, 
  {'label': {'text': 'Start Date Created'}, 'editorType': 'dxDateBox', 'dataField': 'StartRegDateCreatedTextBox', 
    'template': 'dateTemplate', 'editorOptions': {'readOnly': false} }
];

let NonGroupTestDataOutput = [ 
  { 'label': { 'text': 'TempBatchID'}, 'editorType': 'dxTextBox', 'dataField': 'BatchIDTextBox',  
    'template': 'textTemplate', 'editorOptions': {'readOnly': false } }, 
  {'label': {'text': 'Start Date Created'}, 'editorType': 'dxDateBox', 'dataField': 'StartRegDateCreatedTextBox', 
    'template': 'dateTemplate', 'editorOptions': {'readOnly': false} }
];

let ItemTestData = 
[
  { 'label': { 'visible': false }, 'editorType': 'dxTextBox', 'dataField': 'StructureQueryControl',
    'template': 'structureQueryTemplate', 'colSpan': 5, 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'TempBatchID' }, 'editorType': 'dxTextBox', 'dataField': 'BatchIDTextBox',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Start Date Created' }, 'editorType': 'dxDateBox', 'dataField': 'StartRegDateCreatedTextBox',
    'template': 'dateTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'End Date Created' }, 'editorType': 'dxDateBox', 'dataField': 'EndRegDateCreatedTextBox',
    'template': 'dateTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Number of Components' }, 'editorType': 'dxTextBox', 'dataField': 'ComponentNumberStateControl',
    'template': 'dropDownTemplate', 'editorOptions': { 'dataSource': [{ '_text': 'Any', '_value': '' }, { '_text': 'Single Component', '_value': '1' }, { '_text': 'More than one component', '_value': '>1' }], 'valueExpr': '_value', 'displayExpr': '_text', 'readOnly': false } }, 
  { 'label': { 'text': 'Created By:' }, 'editorType': 'dxTextBox', 'dataField': 'PERSONCREATEDTextBox',
    'template': 'dropDownTemplate', 'editorOptions': { 'pickListDomain': 3, 'readOnly': false } }, 
  { 'label': { 'text': 'MW' }, 'editorType': 'dxTextBox', 'dataField': 'MolWeightTextBox',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false, 'customRules': { 'validationRule': [{ 'params': { 'param': [{ '_name': 'clientScript', '_value': 'if(!IsValidMolWeight(arguments.Value)){arguments.IsValid = false;}else{arguments.IsValid=true;}' }] }, '_validationRuleName': 'custom', '_errorMessage': 'Not a valid MW expression!', '_displayPosition': 'Top_Right' }] } } }, 
  { 'label': { 'text': 'MF' }, 'editorType': 'dxTextBox', 'dataField': 'FormulaTextBox',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false, 'customRules': { 'validationRule': [{ 'params': { 'param': [{ '_name': 'clientScript', '_value': 'if(!IsValidFormula(arguments.Value)){arguments.IsValid = false;}                else{arguments.IsValid=true;}' }] }, '_validationRuleName': 'custom', '_errorMessage': 'Not a valid Molecular Formula!', '_displayPosition': 'Top_Right' }] } } }, 
  { 'label': { 'text': 'Scientist' }, 'editorType': 'dxTextBox', 'dataField': 'SCIENTIST_IDProperty',
    'template': 'dropDownTemplate', 'editorOptions': { 'pickListDomain': 3, 'readOnly': false } }, 
  { 'label': { 'text': 'Prefix' }, 'editorType': 'dxTextBox', 'dataField': 'PREFIXTextBox',
    'template': 'dropDownTemplate', 'editorOptions': { 'dropDownItemsSelect': 'SELECT S.SEQUENCEID AS KEY, S.PREFIX AS VALUE FROM REGDB.VW_SEQUENCE S WHERE (TYPE = \'R\' OR TYPE = \'A\') ORDER BY S.PREFIX ASC', 'readOnly': false } }, 
  { 'label': { 'text': 'Registry Project Name' }, 'editorType': 'dxTextBox', 'dataField': 'REGISTRY_PROJECTDropDownListTemp',
    'template': 'dropDownTemplate', 'editorOptions': { 'dropDownItemsSelect': 'SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE =\'R\' OR TYPE=\'A\') AND (ACTIVE = \'T\' OR ACTIVE = \'F\')', 'readOnly': false } }, 
  { 'label': { 'text': 'Notebook Reference' }, 'editorType': 'dxTextBox', 'dataField': 'NOTEBOOK_TEXTProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Start Synthesis Date' }, 'editorType': 'dxDateBox', 'dataField': 'START_CREATION_DATEProperty',
    'template': 'dateTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'End Synthesis Date' }, 'editorType': 'dxDateBox', 'dataField': 'END_CREATION_DATEProperty',
    'template': 'dateTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Amount' }, 'editorType': 'dxTextBox', 'dataField': 'AMOUNTProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Units' }, 'editorType': 'dxTextBox', 'dataField': 'AMOUNT_UNITSProperty',
    'template': 'dropDownTemplate', 'editorOptions': { 'pickListDomain': 2, 'readOnly': false } }, 
  { 'label': { 'text': 'Appearance' }, 'editorType': 'dxTextBox', 'dataField': 'APPEARANCEProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Purity' }, 'editorType': 'dxTextBox', 'dataField': 'PURITYProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Purity Comments' }, 'editorType': 'dxTextBox', 'dataField': 'PURITY_COMMENTSProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Sample ID' }, 'editorType': 'dxTextBox', 'dataField': 'SAMPLEIDProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Solubility' }, 'editorType': 'dxTextBox', 'dataField': 'SOLUBILITYProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Batch Comments' }, 'editorType': 'dxTextBox', 'dataField': 'BATCH_COMMENTProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }, 
  { 'label': { 'text': 'Storage Requirements Warnings' }, 'editorType': 'dxTextBox', 'dataField': 'STORAGE_REQ_AND_WARNINGSProperty',
    'template': 'textTemplate', 'editorOptions': { 'readOnly': false } }
];

let groupItemData = {
  'GroupTestDataInput' : GroupTestDataInput, 'GroupTestDataOuput' : GroupTestDataOuput, 
  'NonGroupTestDataInput' : NonGroupTestDataInput, 'NonGroupTestDataOutput' : NonGroupTestDataOutput,
  'ItemTestData' : ItemTestData
};


export { groupItemData };
