import sys
sys.path.append('C:\Program Files (x86)\PerkinElmer\Python25\Lib')
from os import *
from os.path import *
from ChemScript19 import *
from time import *
import logging

# This scripts expects that two local variables are setup externally
# cdx ==> Contains a base64cdx string
# scriptsPath ==> Contains the path to the child script location
logString = ''
workingMol = ''
clogpValue = ''
LOG_FILENAME = scriptsPath + 'clogp.txt'
logging.basicConfig(
    level=logging.DEBUG,
    format='%(asctime)s %(levelname)s %(message)s',
    filename=LOG_FILENAME,
    filemode='w'
)

try:
    # Setup inputs expected by child scripts
    InputChemistry = cdx
    mydict = {'InputChemistry':InputChemistry,'status_string':logString,'scriptsPath':scriptsPath}

    # Transform script
    execfile(scriptsPath + 'TransformScript.py', mydict)
    mydict['status_string'] = ''

    # Clean script
    execfile(scriptsPath + 'CleanScript.py', mydict)
    mydict['status_string'] = ''

    # PartitionCoefficient calculation
    workingMol = StructureData.LoadData(mydict['InputChemistry'])
    clogpValue = workingMol.PartitionCoefficient
    logString = 'Calculated value:' + str(clogpValue)
except:
    # print "Unexpected error:", sys.exc_info()[0]
    logging.exception('Unexpected error! ')
    logString = sys.exc_info()[1]
finally:
    print(logString)	
	
    
