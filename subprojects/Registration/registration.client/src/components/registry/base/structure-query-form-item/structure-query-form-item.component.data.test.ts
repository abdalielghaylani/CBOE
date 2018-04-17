
let QueryModelTestData1 = {
  'searchTypeValue': 'Substructure',
  'hitAnyChargeHetero': true,
  'reactionCenter': true,
  'hitAnyChargeCarbon': true,
  'permitExtraneousFragments': false,
  'permitExtraneousFragmentsIfRXN': false,
  'fragmentsOverlap': false,
  'tautometer': false,
  'fullSearch': true,
  'simThreshold': 100,
  'matchStereochemistry': true,
  'tetrahedralStereo': 'Same',
  'doubleBondStereo': 'Same',
  'relativeTetStereo': false
};

let structCriteriaOptionsYes = 'YES';
let structCriteriaOptionsNo = 'NO';

let StructQueryTestData = { 
  'QueryModelTestData1' : QueryModelTestData1, 'structCriteriaOptionsYes': structCriteriaOptionsYes, 'structCriteriaOptionsNo' : structCriteriaOptionsNo
};

export { StructQueryTestData };
