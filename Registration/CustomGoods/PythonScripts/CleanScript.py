# Chemcleaner script 
# Expects base64cdx data passed into InputChemistry local variable
# Cleans the molecule and rewrites it to the ImputChemistry variable


from ChemScript12 import *

m = StructureData()
m.ReadData(InputChemistry)
m.CleanupStructure()
InputChemistry = m.CDX(True)

