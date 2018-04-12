from sys import *
from os import *
from os.path import *
from ChemScript11 import *
from ChemScript11 import *
import ChemScriptUtils
Env.setVerbosity(False)

status_string = ''
st = SaltTable()
st.ClearFragmentList()

reader = SDFileReader("C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\salts.sdf")
while (reader.readNext() != None):
	st.RegisterFragment(reader.current())

reader = SDFileReader("C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\solvates.sdf")
while (reader.readNext() != None):
	st.RegisterFragment(reader.current())

m = Mol()
#print InputBase64
m.readData(InputBase64)
#print 'In saltStripping.py, ' + m.cdx(True)


##m = Mol.loadData(base64, 'cdx')
##m.readFile('C:\\Inetpub\\wwwroot\\ChemOffice\\chem_reg\\ChemScript\\saltstripping.cdx')

frags = ChemScriptUtils.splitSaltsCountRepeat_Str(m, st, 'C:\\Inetpub\\wwwroot\\ChemOffice\\chem_reg\\ChemScript\\solvates.sdf', '$$$$') 


if frags != None:
        MainBase64 = frags[0]
else:
        MainBase64 = InputBase64
        
SplittedBase64 = frags


##print frags

##if fragments != None:
##	mainMol = fragments[0]
##	salts = fragments[1]
##	solvates = fragments[2]
	
##else:
##	mainMol = m
##	salts = None



 
