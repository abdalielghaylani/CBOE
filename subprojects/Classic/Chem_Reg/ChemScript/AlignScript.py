# Chemcleaner script - Firmenich Project

from sys import *
from os import *
from os.path import *
from ChemScript11 import *
Env.setVerbosity(False)
success = 'N/A '

#initialise variables
regmol = Mol()
scaffold = Mol()
structurenum = 0
scaffoldnum = 0
DoMol = Mol()
if readfromwebserver == 1:
    regmol.readData(InputBase64)
#################
    n = 0
    frags= regmol.splitFragments();
    if (frags != None):
        n = len(frags);
        fragNum = 0
        for DoMol in frags:
            fragNum +=1
            scaffoldreader1 = SDFileReader('C:\Inetpub\wwwroot\ChemOffice\chem_reg\ChemScript\Input\scaffold_list.sdf')
            scaffoldnum = 0
        ### apply scaffolds    
            while scaffoldreader1.readNext() != None:
                scaffoldnum += 1
                scaffold = scaffoldreader1.current()      
                if DoMol.overlay(scaffold) ==1:
                    status = status + 'scaffold #' + str (scaffoldnum)
                    success = 'SUCCEEDED '
                    scaffoldreader1.close()
                    scaffoldnum = 0

    InputBase64 = regmol.cdx(True)
    status =  'ALIGNED = ' + success + status
    status_string = status + status_string


#################
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

    # Read the input SD file
        
    if not exists('Output'):
       mkdir('Output')

    # Make an SD reader object for the output file
    regreader1 = SDFileReader('./Output/Cleaned_output.sdf')
    # Make an SD writer
    writer2 = SDFileWriter('./Output/Aligned_output.sdf', False)

    while regreader1.readNext() != None:
        success = 'FALSE '
        status = ''
        scaffoldnum = 0
        structurenum += 1   
        regmol = regreader1.current()

        n = 0
        frags= regmol.splitFragments();
        if (frags != None):
            n = len(frags);
            fragNum = 0
            for DoMol in frags:
                fragNum +=1
                scaffoldreader1 = SDFileReader('./Input/scaffold_list.sdf')
                scaffoldnum = 0
            ### apply scaffolds    
                while scaffoldreader1.readNext() != None:
                    scaffoldnum += 1
                    scaffold = scaffoldreader1.current()      
                    if DoMol.overlay(scaffold) ==1:
                        status = status + 'scaffold #' + str (scaffoldnum)
                        success = 'TRUE '
                        scaffoldreader1.close()
                        scaffoldnum = 0
        writer2.writeMol(regmol)
        status =  'RECORD '+ str(structurenum) +' ALIGN = ' + success + status +' '
        status_string =  status_string + status
    writer2.close()

  
