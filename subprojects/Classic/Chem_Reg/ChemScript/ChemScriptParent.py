from os import *
from os.path import *
import sys

#enable chemscript so structure can be sent via b64 cdx text
from ChemScript11 import *

#in order to run from registration, comment next two lines and uncomment 'm = mol.load....'
m = Mol()
#m.readFile('C:\\Inetpub\\wwwroot\\ChemOffice\\chem_reg\\ChemScript\\saltstripping.cdx', 'cdx')
m = Mol.loadData(base64, 'cdx')

status=''
logstring = ''

InputBase64 = m.cdx(True)
MainBase64 = ''
SplittedBase64 = ''

#print InputBase64

mydict = {'InputBase64':InputBase64, 'MainBase64': MainBase64, 'SplittedBase64': SplittedBase64, 'status_string':status, 'readfromwebserver':1 }


execfile('C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\SaltStripping.py', mydict)
mydict['InputBase64'] = mydict['MainBase64'] #following operation is only on main fragment
logstring = logstring + '|'+mydict['status_string']
mydict['status_string'] = ''

execfile('C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\TransformScript.py', mydict)
logstring = logstring + '|'+mydict['status_string']
mydict['status_string'] = ''

execfile('C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\CleanScript.py', mydict)
logstring = logstring +'|'+ mydict['status_string']
mydict['status_string'] = ''

execfile('C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\AlignScript.py', mydict)
logstring = logstring +'|'+ mydict['status_string']

m.readData(mydict['InputBase64'])

#print 'InputBase64 = ' + mydict['InputBase64']
#print 'MainBase64 = ' + mydict['MainBase64']
#print mydict['SplittedBase64']

m.writeFile('C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\outputparent.cdx')

#out_m = m.cdx(True)
#fragments = m
out_m = mydict['InputBase64']
fragments = mydict['SplittedBase64']


