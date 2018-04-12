VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "DBField"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' just a datatype

Public Enum FieldTypeEnum
    eText = 0
    eInteger
    eReal
    eDate
End Enum

Public Enum FieldSubstanceClass
    eClassNeither
    eClassInvCompound
    eClassRegCompound
End Enum

Public Designator As String
Public eFieldSubstClass As FieldSubstanceClass
Public FieldName As String
Public DisplayName As String
' Public UseDefault As Boolean
Public SDFileField As String
Public value As String
Public eFieldType As FieldTypeEnum
Public FieldWidth As Integer
Public Required As Boolean
Public CanBeList As Boolean
Public PicklistName As String
Public PickListLoaded As Boolean
Public Picklist As Dictionary
Public XMLParent As String
Public XMLPosition As String


