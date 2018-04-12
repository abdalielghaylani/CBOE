VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "FieldMapping"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Public Enum MappingType
    eNone = 0
    eInventoryPlates
    eInventoryStructures
    eInventoryContainers
    eRegistration
End Enum

Public sName As String
Public sXMLFilePath As String
Public eMappingType As MappingType

