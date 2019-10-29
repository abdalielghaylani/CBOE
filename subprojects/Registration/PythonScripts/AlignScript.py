# Chemcleaner script

from sys import *
from os import *
from os.path import *
from ChemScript19 import *
Env.SetVerbosity(False)
success = 'FALSE '

regmol = StructureData()
scaffold = StructureData()
structurenum = 0
scaffoldnum = 0
DoMol = StructureData()
regmol.ReadData(InputChemistry)
n = 0
rc = []
frags= regmol.SplitFragments()
if (frags != None):
	n = len(frags);
	fragNum = 0
	status = ''
	for DoMol in frags:
	    fragNum +=1
	    scaffoldreader1 = SDFileReader.OpenFile(scriptsPath + '/Input/scaffold_list.sdf')
	    scaffoldnum = 0
	### apply scaffolds    
	    while scaffoldreader1.ReadNext() != None:
		scaffoldnum += 1
		scaffold = scaffoldreader1.Current()      
		if DoMol.Overlay(scaffold) ==1:
		    status = status + 'scaffold #' + str (scaffoldnum)
		    success = 'TRUE '
		    scaffoldreader1.Close()
		    rc.append(None)
		    scaffoldnum = 0
InputChemistry = regmol.CDXBase64
status =  'ALIGNED = ' + success + status
status_string = status + status_string

