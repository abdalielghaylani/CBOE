let DDColumnItemViewConfig = { 'dataSource': [
  { 'FRAGMENTID': 39, 'FRAGMENTTYPEID': 1, 'STRUCTURE': 'fragment/39?20090630020232', 
    'CODE': '39', 'DESCRIPTION': 'Bisulphate', 'MOLWEIGHT': 97.07054, 'FORMULA': 'HO4S-' },
  { 'FRAGMENTID': 43, 'FRAGMENTTYPEID': 1, 'STRUCTURE': 'fragment/43?20090630020232', 
    'CODE': '43', 'DESCRIPTION': 'Silver salt', 'MOLWEIGHT': 107.8682, 'FORMULA': 'Ag+' },
  { 'FRAGMENTID': 44, 'FRAGMENTTYPEID': 1, 'STRUCTURE': 'fragment/44?20090630020232', 
    'CODE': '44', 'DESCRIPTION': 'Acetate', 'MOLWEIGHT': 59.04402, 'FORMULA': 'C2H3O2-' },
  { 'FRAGMENTID': 46, 'FRAGMENTTYPEID': 1, 'STRUCTURE': 'fragment/46?20090630020232', 
    'CODE': '46', 'DESCRIPTION': 'Benzoic acid salt', 'MOLWEIGHT': 122.12134, 'FORMULA': 'C7H6O2' }
],
'displayExpr': 'CODE', 'valueExpr': 'CODE', 'dropDownWidth': 600,
'columns': [
  { 'dataField': 'FRAGMENTID', 'caption': 'ID', 'width': 60, 'visible': false },
  { 'dataField': 'CODE', 'caption': 'Code' },
  { 'dataField': 'FRAGMENTTYPEID', 'caption': 'Type', 'width': 60, 'lookup':
     { 'dataSource': [{ 'ID': 1, 'DESCRIPTION': 'Salt' }, { 'ID': 2, 'DESCRIPTION': 'Solvate' }], 'displayExpr': 'DESCRIPTION', 'valueExpr': 'ID' }
  },
  { 'dataField': 'STRUCTURE', 'caption': 'Structure', 'allowFiltering': false, 'allowSorting': false, 
    'width': 100, 'cellTemplate': 'structureTemplate', 'editorOptions': { 'smallImage': true } 
  },
  { 'dataField': 'DESCRIPTION', 'caption': 'Description' },
  { 'dataField': 'MOLWEIGHT', 'caption': 'MW' },
  { 'dataField': 'FORMULA', 'caption': 'MF' }
]
};

export { DDColumnItemViewConfig };
