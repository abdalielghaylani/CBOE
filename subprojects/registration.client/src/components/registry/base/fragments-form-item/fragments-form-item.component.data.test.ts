let testSessionData = {
  'token': '',
  'user': { 'fullName': 'Test User Name' },
  'hasError': null,
  'isLoading': null,
  'lookups': {
    'fragments': [{
      'FRAGMENTID': 48,
      'FRAGMENTTYPEID': 1,
      'STRUCTURE': 'fragment/48?20090630020232',
      'CODE': '48',
      'DESCRIPTION': '2,4,6-Trimethylbenzenesulphonic acid salt',
      'MOLWEIGHT': 200.25478,
      'FORMULA': 'C9H12O3S'
    }],
    'fragmentTypes': [{ 
      'ID': 1, 
      'DESCRIPTION': 'Salt' 
    }],
  }
};

let fragmentsTestData = {
  sessionData : testSessionData,
};

export { fragmentsTestData };
