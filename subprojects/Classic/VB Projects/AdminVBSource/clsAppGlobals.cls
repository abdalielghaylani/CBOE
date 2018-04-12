VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsAppGlobals"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F49300C1"
'
Option Base 0

'##ModelId=3778F493022A
Private mvarAPP_NAME As String
'##ModelId=3778F4930266
Private mvarMAIN_WINDOW As String
'##ModelId=3778F49302AC
Private mvarTEMP_DIR_NAME As String
'##ModelId=3778F49303C5
Private mvarTEMP_DIR_PATH As String
'##ModelId=3778F494002F
Private mvarDB_NAMES As String
Private mvarCOWS_PATH As String
Public mvarRunConvert As String
Public mvarChooseCFW As String

Public mvarChooseTask As String


'##ModelId=3778F4940055
Private mvarNAV_BUTTONS_GIF_PATH As String
Private mvarNAV_BAR_WINDOW As String
Private mvarUSER_INFO_WINDOW As String

Public Property Let COWS_PATH(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.COWS_PATH = 5
    mvarCOWS_PATH = vData
End Property


'##ModelId=3778F494011D
Public Property Get COWS_PATH() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.COWS_PATH
    COWS_PATH = mvarCOWS_PATH
End Property
Public Property Let DB_NAMES(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DB_NAMES = 5
    mvarDB_NAMES = vData
End Property


'##ModelId=3778F494011D
Public Property Get DB_NAMES() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DB_NAMES
    DB_NAMES = mvarDB_NAMES
End Property

Public Property Let NAV_BUTTONS_GIF_PATH(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.NAV_BUTTONS_GIF_PATH = 5
    mvarNAV_BUTTONS_GIF_PATH = vData
End Property


'##ModelId=3778F494011D
Public Property Get NAV_BUTTONS_GIF_PATH() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.NAV_BUTTONS_GIF_PATH
    NAV_BUTTONS_GIF_PATH = mvarNAV_BUTTONS_GIF_PATH
End Property







'##ModelId=3778F4940249
Public Property Let TEMP_DIR_PATH(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TEMP_DIR_PATH = 5
    mvarTEMP_DIR_PATH = vData
End Property


'##ModelId=3778F49402D6
Public Property Get TEMP_DIR_PATH() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TEMP_DIR_PATH
    TEMP_DIR_PATH = mvarTEMP_DIR_PATH
End Property



'##ModelId=3778F4940330
Public Property Let TEMP_DIR_NAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TEMP_DIR_NAME = 5
    mvarTEMP_DIR_NAME = vData
End Property


'##ModelId=3778F49403D0
Public Property Get TEMP_DIR_NAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TEMP_DIR_NAME
    TEMP_DIR_NAME = mvarTEMP_DIR_NAME
End Property



'##ModelId=3778F495004C
Public Property Let MAIN_WINDOW(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.MAIN_WINDOW = 5
    mvarMAIN_WINDOW = vData
End Property


'##ModelId=3778F4950100
Public Property Get MAIN_WINDOW() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.MAIN_WINDOW
    MAIN_WINDOW = mvarMAIN_WINDOW
End Property

'##ModelId=3778F495004C
Public Property Let NAV_BAR_WINDOW(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.NAV_BAR_WINDOW = 5
    mvarNAV_BAR_WINDOW = vData
End Property


'##ModelId=3778F4950100
Public Property Get NAV_BAR_WINDOW() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.NAV_BAR_WINDOW
    NAV_BAR_WINDOW = mvarNAV_BAR_WINDOW
End Property

'##ModelId=3778F495004C
Public Property Let USER_INFO_WINDOW(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.USER_INFO_WINDOW = 5
    mvarUSER_INFO_WINDOW = vData
End Property


'##ModelId=3778F4950100
Public Property Get USER_INFO_WINDOW() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.USER_INFO_WINDOW
    USER_INFO_WINDOW = mvarUSER_INFO_WINDOW
End Property
'##ModelId=3778F495016F
Public Property Let APP_NAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.APP_NAME = 5
    mvarAPP_NAME = vData
End Property


'##ModelId=3778F495022D
Public Property Get APP_NAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.APP_NAME
    APP_NAME = mvarAPP_NAME
End Property
'##ModelId=3778F495016F
Public Property Let RunConvert(ByVal vData As String)
    mvarRunConvert = vData
End Property

'##ModelId=3778F495022D
Public Property Get RunConvert() As String
    RunConvert = mvarRunConvert
End Property
'##ModelId=3778F495016F
Public Property Let ChooseCFW(ByVal vData As String)
    mvarChooseCFW = vData
End Property

'##ModelId=3778F495022D
Public Property Get ChooseCFW() As String
    ChooseCFW = mvarChooseCFW
End Property

'##ModelId=3778F495016F
Public Property Let ChooseTask(ByVal vData As String)
    mvarChooseTask = vData
End Property

'##ModelId=3778F495022D
Public Property Get ChooseTask() As String
    ChooseTask = mvarChooseTask
End Property



