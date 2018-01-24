## This script is meant to bypass calls to ChemScript for debugging purposes
## Backup the existing parentscript.py and replace it with the contents of this script
## whenever you want to avoid invoking ChemScript


# This scripts expects that two local variables are setup externally
# cdx ==> Contains a base64cdx string
# scriptsPath ==> Contains the path to the child script location

# bypass child scripts and simply return the original molecule
logstring  = ''
normalizedBase64= cdx
