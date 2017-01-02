import sys
sys.path.append('C:\Program Files (x86)\CambridgeSoft\Python25\Lib')
from os import *
from os.path import *
from ChemScript14 import *
from time import *

# This scripts expects that two local variables are setup externally
# cdx ==> Contains a base64cdx string
# scriptsPath ==> Contains the path to the child script location

# Setup inputs expected by child scripts
status = ''
InputChemistry = cdx
mydict = {'InputChemistry':InputChemistry, 'status_string':status,'scriptsPath':scriptsPath}


# Call Transform script
logstring = ''
execfile(scriptsPath + 'TransformScript.py',mydict)
logstring = logstring + '|'+mydict['status_string']
mydict['status_string'] = ''

# Call Clean script
execfile(scriptsPath + 'CleanScript.py',mydict)
logstring = logstring +'|'+ mydict['status_string']
mydict['status_string'] = ''

# Call Scafold alignment script
execfile(scriptsPath + 'AlignScript.py',mydict)
logstring = logstring +'|'+ mydict['status_string']

# Setup the expected return variable
normalizedBase64= mydict['InputChemistry']
