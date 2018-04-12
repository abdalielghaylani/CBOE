# Chemcleaner script - XXXXXXXXX Project

from sys import *
from os import *
from os.path import *
from ChemScript11 import *
Env.setVerbosity(False)
Success = 'N/A'
##Option 1: read structure as passed to Webservice
#regmol = Mol.loadData(base64, 'cdx')
   
import os.path
mydir = "C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\input"
tomatch = "RXN_"
regmol = Mol()
status = ''
resultMol1 = Mol()
structurenum = 0
transformcounter = 0
TransformReaction = Rxn()
moltext = ' '

if readfromwebserver == 1:
    regmol.readData(InputBase64)
    #print regmol
    for f in os.listdir(mydir):
        start_of_filename = f[:4]
    
        if start_of_filename == tomatch:
            transformcounter +=1
            each = os.path.join(mydir,f)
            TransformReaction.readFile (each)
            resultMol1 = TransformReaction.transform(regmol)

            
            if (resultMol1 == None):    
                resultMol1 = regmol
                #print 'no transform applied' + regmol.cdx(True)
            else:
            
                #transformation applied
                Success = 'SUCCEEDED'
                regmol = resultMol1
                status_string = status_string +' ' + f
                #print 'transform applied' + regmol.cdx(True)

    moltext  = regmol.cdx(True)
    status_string = 'TRANSFORMED = ' + Success +' '+ status_string
    InputBase64 = moltext
    #InputBase64 = 'CCCC'
    #print 'in transform' + regmol.cdx(True)
    #print 'in transform ' + moltext
            
else:
    if not exists('Output'):
        mkdir('Output')
    regreader1 = SDFileReader('./Input/InputFile.sdf')
    writer1 = SDFileWriter('./Output/Transformed_output.sdf', False)

    # Read the input SD file
    while regreader1.readNext() != None:
        structurenum += 1
        status_string = status_string +' RECORD '
        status_string = status_string + str(structurenum)
        regmol = regreader1.current()
        transformcounter = 0
        Success = 'FALSE '
        status = ''
        moltext = ' '
        
    # read in each file in Input directory beginning with 'RXN_', and apply each in turn as a transform

        for f in os.listdir(mydir):
            start_of_filename = f[:4]
            
            if start_of_filename == tomatch:
                transformcounter +=1
                each = os.path.join(mydir,f)
                TransformReaction.readFile (each)
                resultMol1 = TransformReaction.transform(regmol)
                if (resultMol1 == None):    
                   resultMol1 = regmol               
                else:
                    
              #transformation applied
                    Success = 'TRUE '
                    regmol = resultMol1
                    status = status +' ' + f
                    moltext  = regmol.cdx(True)
    #append output string                
        moltext  = regmol.cdx(True)
        status = 'TRANSFORM = ' + Success + status #+ 'on record ' , structurenum

        writer1.writeMol(regmol)
        status_string = status_string + ' ' + status

    writer1.close()

