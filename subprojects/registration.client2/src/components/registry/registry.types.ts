export class FragmentData {
  FragmentID: number;
  Code: number;
  FragmentType: string;
  Description: string;
  Structure: string;
  DateCreated: Date;
  DateLastModified: Date;
}

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
  caption: 'Last Modified Date',
  dataType: 'date',
  editorType: 'dxDateBox',
  editorOptions: { disabled: true }
}, {
  dataField: 'PersonCreated',
  caption: 'Scientist',
  dataType: 'string',
  editorOptions: { disabled: true }
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

export const FRAGMENT_DESC_LIST = [{
  dataField: 'FragmentID',
  caption: 'Last Modified Date',
  dataType: 'date',
  editorType: 'dxDateBox',
  editorOptions: { disabled: true }
}, {
  dataField: 'Code',
  caption: 'Last Modified Date',
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
  caption: 'Last Modified Date',
  dataType: 'date',
  editorType: 'dxDateBox',
}];
