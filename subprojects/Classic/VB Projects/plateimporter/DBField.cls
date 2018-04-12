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


Public TableName As String
Public FieldName As String
Public DisplayName As String
Public UseDefault As Boolean
Public SDFileField As String
Public value As Variant
Public eFieldType As FieldTypeEnum

