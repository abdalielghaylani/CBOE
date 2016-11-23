# Transform script - XXXXXXXXX Project

from sys import *
from os import *
from os.path import *
from ChemScript12 import *
Success = 'FALSE'
   
import os.path
mydir = scriptsPath + "Input"
tomatch = "RXN_"
regmol = StructureData()
status = ''
resultMol1 = StructureData()
structurenum = 0
transformcounter = 0
TransformReaction = ReactionData()
moltext = ' '
regmol.ReadData(InputChemistry)
for f in os.listdir(mydir):
  start_of_filename = f[:4]
  if start_of_filename == tomatch:
      transformcounter +=1
      each = os.path.join(mydir,f)
      TransformReaction.ReadFile (each)
      resultMol1 = TransformReaction.Transform(regmol)
      if (resultMol1 == None):
          resultMol1 = regmol
      else:
          #transformation applied
          regmol = resultMol1
	  Success = 'TRUE'
          status_string = status_string +' ' + f
moltext  = resultMol1.CDX(True)
InputChemistry = moltext
status_string = 'TRANSFORM = ' + Success +' '+ status_string
	        
