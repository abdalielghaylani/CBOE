export class FragmentData {
  FragmentID: number;
  Code: number;
  FragmentType: string;
  Description: string;
  Structure: string;
  DateCreated: Date;
  DateLastModified: Date;
}

export const COMPONENT_DESC_LIST = [{
  dataField: 'ComponentIndex',
  dataType: 'number',
  editorOptions: { disabled: true }
}, {
  dataField: 'ComponentComments',
  dataType: 'string',
  colSpan: 2
}, {
  dataField: 'StereochemistryComments',
  dataType: 'string',
  colSpan: 2
}];

export const COMPOUND_DESC_LIST = [{
  dataField: 'CompoundID',
  dataType: 'number',
  editorOptions: { disabled: true }
}, {
  dataField: 'DateCreated',
  dataType: 'date',
  editorType: 'dxDateBox',
  editorOptions: { disabled: true }
}, {
  dataField: 'PersonCreated',
  dataType: 'string',
  editorOptions: { disabled: true }
}, {
  dataField: 'PersonApproved',
  dataType: 'string',
  editorOptions: { disabled: true }
}, {
  dataField: 'DateLastModified',
  dataType: 'date',
  editorType: 'dxDateBox'
}];

export const FRAGMENT_DESC_LIST = [{
  dataField: 'FragmentID',
  label: 'Last Modified Date',
  dataType: 'date',
  editorType: 'dxDateBox',
  editorOptions: { disabled: true }
}, {
  dataField: 'Code',
  label: 'Last Modified Date',
  dataType: 'number',
  editorOptions: { disabled: true }
}, {
  dataField: 'FragmentType',
  dataType: 'string',
  editorOptions: { disabled: true }
}, {
  dataField: 'Structure',
  dataType: 'string',
  editorOptions: { disabled: true }
}, {
  dataField: 'DateCreated',
  dataType: 'date',
  editorType: 'dxDateBox',
}, {
  dataField: 'DateLastModified',
  label: 'Last Modified Date',
  dataType: 'date',
  editorType: 'dxDateBox',
}];

export const BATCH_DESC_LIST = [{
  dataField: 'BatchID',
  dataType: 'number',
  editorOptions: { disabled: true }
}, {
  dataField: 'DateCreated',
  dataType: 'date',
  editorType: 'dxDateBox',
  editorOptions: { disabled: true }
}, {
  dataField: 'DateLastModified',
  label: 'Last Modified Date',
  dataType: 'date',
  editorType: 'dxDateBox',
  editorOptions: { disabled: true }
}, {
  dataField: 'PersonCreated',
  label: 'Scientist',
  dataType: 'string',
  editorType: 'dxSelectBox',
  editorOptions: {
    displayExpr: 'USERID',
    valueExpr: 'PERSONID',
    placeholder: ''
  }
}, {
  dataField: 'SynthesisDate',
  dataType: 'date',
  editorType: 'dxDateBox',
}, {
  dataField: 'NotebookReference',
  dataType: 'string'
}, {
  dataField: 'Amount',
  dataType: 'string'
}, {
  dataField: 'Unit',
  dataType: 'string'
}, {
  dataField: 'Appearance',
  dataType: 'string'
}, {
  dataField: 'Purity',
  dataType: 'string'
}, {
  dataField: 'PurityComments',
  dataType: 'string'
}, {
  dataField: 'SampleID',
  dataType: 'string'
}, {
  dataField: 'Solubility',
  dataType: 'string'
}, {
  dataField: 'PercentActive',
  dataType: 'string'
}, {
  dataField: 'FormulaWeight',
  dataType: 'string'
}, {
  dataField: 'MolecularFormula',
  dataType: 'string'
}, {
  dataField: 'BatchComments',
  dataType: 'string',
  colSpan: 2
}, {
  dataField: 'StorageRequirementsWarnings',
  dataType: 'string',
  colSpan: 2
}];
