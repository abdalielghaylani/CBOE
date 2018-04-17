let GroupTestDataInput = 
[
  { 'itemType': 'group',
    'items': [   
      { 'itemName' : 'TestItem 1'},
      { 'itemName' : 'TestItem 2'},
    ] 
  },
  { 'itemType': 'group',
    'items': [
      { 'itemName' : 'TestItem 3'},
    ] 
  }, 
];

let GroupTestDataOutput = [
  { 'itemName' : 'TestItem 1'},
  { 'itemName' : 'TestItem 2'},
  { 'itemName' : 'TestItem 3'},
];

let NonGroupTestDataInput = [ 
  { 'itemName' : 'TestItem Non group 1'},
  { 'itemName' : 'TestItem Non group 2'},
];

let NonGroupTestDataOutput = [ 
  { 'itemName' : 'TestItem Non group 1'},
  { 'itemName' : 'TestItem Non group 2'},
];

let groupItemData = {
  'GroupTestDataInput' : GroupTestDataInput, 'GroupTestDataOutput' : GroupTestDataOutput, 
  'NonGroupTestDataInput' : NonGroupTestDataInput, 'NonGroupTestDataOutput' : NonGroupTestDataOutput,
};


export { groupItemData };
