let testSessionData = {
  'token': '',
  'user': { 'fullName': 'Test User Name' },
  'hasError': null,
  'isLoading': null,
  'lookups': {
    'identifierTypes': [{
      'ID': 4,
      'NAME': 'Chemical Name',
      'DESCRIPTION': 'Test Chemical Name',
      'TYPE': 'C',
      'ACTIVE': 'T'
    }],
  }
};

let testViewModel = {
  editorOptions: {
    value: {
      Identifier: [{
        IdentifierID: {
          '_Name': 'Alias',
          '_Active': 'T',
          '__text': '3'
        },
        InputText: 'Test Identifier',
        OrderIndex: '1'
      }]
    },
    idListType: 'R',
    dataField: 'Mix_IdentifiersUltraGrid',
    readOnly: false,
    inputAttr: {
      id: 'dx_dx-a2fe26d8-d3f8-6765-397b-239794cba2cf_Mix_IdentifiersUltraGrid'
    },
    name: 'Mix_IdentifiersUltraGrid',
    fieldConfig: {
      tables: {
        table: [
          {
            Columns: {
              Column: [{
                _name: 'Value',
                formElement: {
                  configInfo: {
                    MaxLength: 10
                  }
                }
              }]
            }
          }
        ]
      }
    }
  }
};

let expectedColumnData = [
  {
    'dataField': 'id',
    'caption': 'Identifier',
    'editorType': 'dxSelectBox',
    'lookup': {
      'dataSource': [{
        'ID': 3,
        'NAME': 'Alias',
        'DESCRIPTION': 'Alias',
        'TYPE': 'R',
        'ACTIVE': 'T'
      }],

      // 'ID': 4,
      // 'NAME': 'Chemical Name',
      // 'DESCRIPTION': 'Test Chemical Name',
      // 'TYPE': 'C',
      // 'ACTIVE': 'T'

      'displayExpr': 'NAME',
      'valueExpr': 'ID',
      'placeholder': 'Select Identifier'
    },
    'validationRules': [{
      'type': 'required',
      'message': 'A valid identifier type is required.'
    }]
  },
  {
    'dataField': 'inputText',
    'caption': 'Value'
  }
];

let deSerializeInputVal = {
  'Identifier': [{
    'IdentifierID': {
      '__text': 3,
      '_Active': 'T',
      '_Name': 'Alias',
      '_Description': 'Alias'
    },
    'InputText': 'Test Identifier'
  }]
};

let deSerializedOutputVal = [{
  'id': 3,
  'inputText': 'Test Identifier'
}];

let expectedEditingMode = 'row';

let idListTestData = {
  sessionData: testSessionData,
  viewModel: testViewModel,
  deSerializeInputVal: deSerializeInputVal,
  deSerializedOutputVal: deSerializedOutputVal,
  expectedEditingMode: expectedEditingMode,
  expectedColumnData: expectedColumnData
};

export { idListTestData };
