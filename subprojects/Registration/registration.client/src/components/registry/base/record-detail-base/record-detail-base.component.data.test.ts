let testSessionData = {
  'token': '',
  'user': { 'fullName': 'Test User Name' },
  'hasError': null,
  'isLoading': null,
  'lookups': {
    'projects': [{
      'PROJECTID': 1,
      'NAME': 'Default Test Proj',
      'ACTIVE': 'T',
      'DESCRIPTION': 'Default test project description',
      'TYPE': 'A',
      'ISPUBLIC': 'T'
    }],
    'systemSettings': [{
      'groupName': 'REGADMIN',
      'groupLabel': 'Registration',
      'name': 'CheckDuplication',
      'controlType': 'PICKLIST',
      'value': 'True',
      'description': 'This setting enables validation of duplication of structures and mixtures with identical compound before of the registration.',
      'pikclistDatabaseName': '',
      'allowedValues': 'True|False',
      'processorClass': '',
      'isAdmin': false,
      'isHidden': null
    }],
  },
};

let testViewModel = {
  'editorOptions': {
    'Project': [
      {
        'ID': '1',
        'ProjectID': {
          '_Description': 'Default test project description',
          '_Name': 'Default Test Proj',
          '_Active': 'T',
          '__text': '1'
        }
      }
    ],
    'value': {
      'Project': [{
        'ID': '1',
        'ProjectID': {
          '_Description': 'Default test project description',
          '_Name': 'Default Test Proj',
          '_Active': 'T',
          '__text': '1'
        }
      }]
    }
  }
};

let expectedDataSource = [{
  'PROJECTID': 1,
  'NAME': 'Default Test Proj',
  'ACTIVE': 'T',
  'DESCRIPTION': 'Default test project description',
  'TYPE': 'A',
  'ISPUBLIC': 'T'
}];

let expectedSavedValue = {
  'Project': [{
    'ID': '1',
    'ProjectID': {
      '_Description': 'Default test project description',
      '_Name': 'Default Test Proj',
      '_Active': 'T',
      '__text': '1'
    }
  }]
};

let deSerializeInputVal = {
  'Project': [{
    'ID': '1',
    'ProjectID': {
      '_Description': 'Default test project description',
      '_Name': 'Default Test Proj',
      '_Active': 'T',
      '__text': '1'
    }
  }]
};

let testEventData = {
  'previousValue' : [1],
  'value' : [1, 2]
};


let deSerializedOutputVal = [1];

let serializeInputParamValue = [1, 2];

let serializeExpectedOutputParamValue = {
  'Project': [{
    'ID': '1',
    'ProjectID': {
      '_Description': 'Default test project description',
      '_Name': 'Default Test Proj',
      '_Active': 'T',
      '__text': '1'
    }
  },
  {
    'ProjectID': {
      '__text': '2'
    }
  }
  ]
};

let serializeInputSavedValue = {
  'Project': [{
    'ID': '1',
    'ProjectID': {
      '_Description': 'Default test project description',
      '_Name': 'Default Test Proj',
      '_Active': 'T',
      '__text': '1'
    }
  }]
};

let serializeOutputSavedValue = {
  'Project': [{
    'ID': '1',
    'ProjectID': {
      '_Description': 'Default test project description',
      '_Name': 'Default Test Proj',
      '_Active': 'T',
      '__text': '1'
    }
  },
  {
    'ProjectID': {
      '__text': '2'
    }
  }
  ]
};


let recordDetailBaseTestData = {
  sessionData: testSessionData,
  viewModel: testViewModel,
  // Test Data for Update Method
  updatedDataSource: expectedDataSource,
  updatedSavedValue: expectedSavedValue,
  // test Data for de serialize Method
  deSerializeInputVal: deSerializeInputVal,
  deSerializedOutputVal: deSerializedOutputVal,
  // test data for onValueChanged method call
  testEventData: testEventData,
  // Test data for serialize method
  serializeInputParamValue: serializeInputParamValue,
  serializeExpectedOutputParamValue: serializeExpectedOutputParamValue,
  serializeInputSavedValue: serializeInputSavedValue,
  serializeOutputSavedValue: serializeOutputSavedValue
};

export { recordDetailBaseTestData };
