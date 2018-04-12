# Chemcleaner script - Firmenich Project

from sys import *
from os import *
from os.path import *
from ChemScript11 import *
Env.setVerbosity(False)

regmol = Mol()

if readfromwebserver == 1:
    regmol.readData(InputBase64)
    regmol.cleanupStructure()
    InputBase64 = regmol.cdx(True)
else:
    ## if not passed single molecule (i.e. in Debug mode, read SD file to clean, and generate SD output
    if not exists('Output'):
       mkdir('Output')
    # Make an SD reader object for the output file
    regreader1 = SDFileReader('./Output/Transformed_output.sdf')   
    # Make an SD writer
    writer2 = SDFileWriter('./Output/Cleaned_output.sdf', False)

    #initialise variables
    regmol = Mol()
    structurenum = 0
    # Read the input SD file
    while regreader1.readNext() != None:
        structurenum += 1   
        regmol = regreader1.current()
    #clean up structure
        regmol.cleanupStructure()
        #status_string = status_string +'RECORD ' + str(structurenum )+  ' CLEANED ' 
        writer2.writeMol(regmol)
    regreader1.close()
    writer2.close()
   
  
