from os import *
from os.path import *
import sys

#enable chemscript so structure can be sent via b64 cdx text
from ChemScript11 import *
from time import *

#in order to run from registration, comment next two lines and uncomment 'm = mol.load....'
m = Mol()
m.readFile('testparent.cdx')
#m = Mol.loadData(base64, 'cdx')

status = ''

logstring = ''
# set chemistry argument for passing to child scripts
InputChemistry = m.cdx(True)

#print m.formula()
#print InputChemistry
sys.path.append ('C:\Documents and Settings\taitken\Desktop\Projects\Firmenich\Scripts\transformer\Sequential Scripts')

#setup objects to pass to child scripts: Chemistry, status, debugvalue (SD file = 0, individual input =1)
# set 'readfromwebserver' value to 0 in order to run on Input SD files in /Input folder
mydict = {'InputChemistry':InputChemistry, 'status_string':status,'readfromwebserver':0}

#call child scripts here, passing Dictionary object containing chemistry, status string and debug value

execfile('TransformScript.py',mydict)
logstring = logstring + '|'+mydict['status_string']

mydict['status_string'] = ''

execfile('CleanScript.py',mydict)
#logstring = logstring +'|'+ mydict['status_string']

mydict['status_string'] = ''

execfile('AlignScript.py',mydict)
logstring = logstring +'|'+ mydict['status_string']

m.readData(mydict['InputChemistry'])
#output of status information
print ctime(), logstring

#in order to pass data back to calling app (e.g. Reg), comment next line and uncomment the following line

m.writeFile('outputparent.cdx')
#out_m=m
