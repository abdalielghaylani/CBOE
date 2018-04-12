Attribute VB_Name = "modObjlib"
'ISIS/Object Library constants file for Visual Basic
'V2.1.4 - April 13, 1998

'   (C) Copyright 1995-1998 by MDL Information Systems, Inc.  All rights
'   reserved.  Use of copyright notice does not imply publication or
'   disclosure.  No portion of this software may be reproduced, transmitted,
'   transcribed, stored in a retrieval system, or translated into any computer
'   language, in any form or by any means, electronic, magnetic, optical,
'   chemical, manual, or otherwise except as permitted in writing by
'   MDL Information Systems, Inc., 14600 Catalina Street, San Leandro,
'   California 94577#
'
'   MDL Information Systems, Inc. reserves the right to use the technology
'   embodied in this program, in part or in whole, in applications we sell.

'************** Object Library-specific constants *********************

'OCX scaling modes
  Global Const SCALE_FITBOX = 0
  Global Const SCALE_IFNEEDED = 1
  Global Const SCALE_ASDRAWN = 2
  Global Const SCALE_STDBOND = 3

'Layers for DataOrQuery property
  Global Const DISPLAY_QUERY = 0
  Global Const DISPLAY_DATA = 1

'OCX hydrogen display
  Global Const HYDROGENS_NONE = 0
  Global Const HYDROGENS_HETERO = 1
  Global Const HYDROGENS_TERMINAL = 2
  Global Const HYDROGENS_ALL = 3
  Global Const HYDROGENS_OFF = 0

'OCX atom number display
  Global Const ATOMNUMBERS_NONE = 0
  Global Const ATOMNUMBERS_ALL = 1
  Global Const ATOMNUMBERS_AAMAPPED = 2

'OCX reacting center display
  Global Const RXNCENTER_OFF = 0
  Global Const RXNCENTER_COLOR = 1
  Global Const RXNCENTER_THICKENED = 2
  Global Const RXNCENTER_HASH = 3

'OCX DBID property; use INVALID_ID (0) for error value, as in PL
  Global Const DBID_NONE = -1

'OCX Debug property
  Global Const OLDEBUG_NONE = 0
  Global Const OLDEBUG_LOWLEVEL = 1
  Global Const OLDEBUG_HIGHLEVEL = 4
  Global Const OLDEBUG_BOTHLEVELS = 5
 
'OCX BuildQuery and CheckQuery method; zero and positive values
'are the same as PL's QERR_... constants, listed below.
  Global Const QERR_CANTGET_OCX = -4
  Global Const QERR_CANTPUSHDB = -3
  Global Const QERR_DBCLOSED = -2
  Global Const QERR_NULLQUERY = -1

'OCX ReadList, ReadMolfile, ReadRxnfile, WriteList methods
  Global Const READWRITE_FAILED = 0
  Global Const READWRITE_SUCCESS = 1
 
'OCX RemoteOpenDialog
  Global Const REMOTEOPEN_INITIAL = 0
  Global Const REMOTEOPEN_OPENING_HVIEW = 1
  Global Const REMOTEOPEN_DOWNLOAD_DICTIONARY = 2
 
'OCX Alignment property
  Global Const OLFORMBOX_ALIGN_LEFT = 0
  Global Const OLFORMBOX_ALIGN_CENTER = 1
  Global Const OLFORMBOX_ALIGN_RIGHT = 2
 
'OCX record navigation button placement
  Global Const RN_NONE = 0
  Global Const RN_STANDALONEHORZ = 1
  Global Const RN_STANDALONEVERT = 2
  Global Const RN_BOTTOMLEFT = 3
  Global Const RN_BOTTOMRIGHT = 4
  Global Const RN_TOPLEFT = 5
  Global Const RN_TOPRIGHT = 6
  Global Const RN_LEFTBOTTOM = 7
  Global Const RN_RIGHTBOTTOM = 8
  Global Const RN_LEFTTOP = 9
  Global Const RN_RIGHTTOP = 10

'OL click modifiers for ButtonDown, ButtonUp, ClickEx, DoubleClick,
'KeyDown, KeyUp, MouseMoveEx
  Global Const LEFT_BUTTON = 1
  Global Const RIGHT_BUTTON = 2
  Global Const MIDDLE_BUTTON = 4
  Global Const SHIFT_MASK = 1
  Global Const CTRL_MASK = 2
  Global Const ALT_MASK = 4

'Pass 0 to isisInitializeObjlib for VB-style string parameters
  Global Const OL_INIT = 0

'Object Library General
  Global Const INVALID_OBJECT = -1

'************** Constants shared between Object Library and PL ***************

'misc constants
  Global Const INVALID_ID = 0
  Global Const INVALID_INDEX = -1
  Global Const INVALID_RECORDINDEX = -1
  Global Const INVALID_DBOBJINDEX = 0
  Global Const INVALID_FIELDNUM = -1
  Global Const NOFIELD = -1
  Global Const MAXINDEX_UNKNOWN = -2
  Global Const ROOT_FIELDNUM = 0

'module ID for accessing ISIS resources
  Global Const MODULE_ISIS = 0

'platform constants
  Global Const PLATFORM_CURRENT = 1
  Global Const PLATFORM_WINDOWS = 1
  Global Const PLATFORM_MAC = 2
  Global Const PLATFORM_UNIX = 3

'system type constants
  Global Const SYSTEMTYPE_UNKNOWN = 0
  Global Const SYSTEMTYPE_WIN32 = 1
  Global Const SYSTEMTYPE_WIN16 = 2
  Global Const SYSTEMTYPE_POWERPC = 3
  Global Const SYSTEMTYPE_MAC68K = 4
  Global Const SYSTEMTYPE_UNIX = 5

'possible return values from ExecutePLProgram
  Global Const EXECUTEPL_ALREADYLOADED = -1
  Global Const EXECUTEPL_LOADFAILED = 0
  Global Const EXECUTEPL_LOADSUCCESS = 1

'possible return values from UnloadPLProgram
  Global Const UNLOADPL_UNLOADSUCCESS = 1
  Global Const UNLOADPL_INUSE = 0
  Global Const UNLOADPL_NOTLOADED = -1
  Global Const UNLOADPL_UNLOADFAILED = -2

'possible return values from UnloadExternalResourceModule
  Global Const UNLOADRESOURCE_UNLOADSUCCESS = 1
  Global Const UNLOADRESOURCE_INUSE = 0
  Global Const UNLOADRESOURCE_NOTLOADED = -1

'registered clipboard data types
  Global Const CLIPTYPE_SKETCH = 0
  Global Const CLIPTYPE_CTFILE = 1
  Global Const CLIPTYPE_RTF = 2

'MSW registered clipboard data types
  Global Const CLIPTYPE_TEXT = -1
  Global Const CLIPTYPE_META = -3
  Global Const CLIPTYPE_BITMAP = -2
  Global Const CLIPTYPE_SYLK = -4
  Global Const CLIPTYPE_DIF = -5
  Global Const CLIPTYPE_TIFF = -6
  Global Const CLIPTYPE_OEMTEXT = -7
  Global Const CLIPTYPE_DIB = -8
  Global Const CLIPTYPE_PALETTE = -9
  Global Const CLIPTYPE_PENDATA = -10
  Global Const CLIPTYPE_RIFF = -11
  Global Const CLIPTYPE_WAVE = -12

'DDE Trigger return constants
  Global Const DDETRIGGER_NOTPROCESSED = 0
  Global Const DDETRIGGER_PROCESSED = 1

'General DDE constants
  Global Const DDEDATA_REQUEST = 1
  Global Const DDEDATA_POKE = 2
  Global Const DDECLIENT_ADVISE = 1
  Global Const DDECLIENT_DISCONNECT = 9

'topic data for the pre-defined SYSTEM topic
  Global Const DDESYSTEM_TOPICDATA = 0

'field type constants
  Global Const FIELDTYPE_FTEXT = 1
  Global Const FIELDTYPE_VTEXT = 2
  Global Const FIELDTYPE_LONG = 3
  Global Const FIELDTYPE_REAL = 4
  Global Const FIELDTYPE_RANGE = 5
  Global Const FIELDTYPE_DATE = 6
  Global Const FIELDTYPE_VBINARY = 7
  Global Const FIELDTYPE_PARENT = 8
  Global Const FIELDTYPE_STRUCT = 9
  Global Const FIELDTYPE_RXN = 10
  Global Const FIELDTYPE_FMLA = 11
  Global Const FIELDTYPE_SKETCH = 15
  Global Const FIELDTYPE_LINK = 16
  Global Const FIELDTYPE_CALCULATED = 17

'query error constants
  Global Const QERR_NONE = 0
  Global Const QERR_PARSEEOL = 1
  Global Const QERR_PARSEALGEBRA = 2
  Global Const QERR_PARSERPAREN = 3
  Global Const QERR_PARSEFIELD = 4
  Global Const QERR_PARSEFXNNAME = 5
  Global Const QERR_PARSEQUERYSTRUCT = 6
  Global Const QERR_PARSECONTAIN = 7
  Global Const QERR_PARSEOPTYPE = 8
  Global Const QERR_PARSELPAREN = 9
  Global Const QERR_PARSEFXNARG = 10
  Global Const QERR_PARSENUM = 11
  Global Const QERR_PARSEINT = 12
  Global Const QERR_PARSESTRLEN = 13
  Global Const QERR_PARSEQUOTE = 14
  Global Const QERR_PARSELOPERAND = 15
  Global Const QERR_PARSEFUNCTION = 16
  Global Const QERR_PARSELOGLEVEL = 17
  Global Const QERR_PARSEPARENT = 18
  Global Const QERR_PARSENOT = 19
  Global Const QERR_PARSESSS = 20
  Global Const QERR_PARSENODATA = 21
  Global Const QERR_PARSELSTRING = 22
  Global Const QERR_PARSEOVERFLOW = 47
  Global Const QERR_PARSEDELFIELD = 48
  Global Const QERR_PARSENOSTRUCT = 49
  Global Const QERR_PARSEUNKNOWN = 50

'query formula error constants
  Global Const QERR_PARSEATOM = 23
  Global Const QERR_PARSESYMLEN = 24
  Global Const QERR_PARSEATOMUNKNOWN = 25
  Global Const QERR_PARSEQATOM = 26
  Global Const QERR_PARSEATOMCOUNT = 27
  Global Const QERR_PARSEATOMRANGE = 28
  Global Const QERR_PARSEATOMRANGE2 = 29
  Global Const QERR_PARSEATOMRANGE3 = 30
  Global Const QERR_PARSEEXTRAARG = 31
  Global Const QERR_PARSENOFMLA = 32
  Global Const QERR_PARSEINVALIDFMLA = 33
  Global Const QERR_PARSETYPEMATCH = 34
  Global Const QERR_PARSEARITH = 35
  Global Const QERR_PARSELOG = 36
  Global Const QERR_PARSEFMLAQUERY = 37
  Global Const QERR_PARSEINVALIDDATE = 38
  Global Const QERR_PARSEIDLIST = 39
  Global Const QERR_PARSEINTID = 40
  Global Const QERR_PARSEIDTOOLONG = 41
  Global Const QERR_PARSEROPERAND = 42
  Global Const QERR_PARSESTRUCTOP = 43
  Global Const QERR_PARSEQSTRING = 44
  Global Const QERR_PARSEHOSTQUERY = 45
  Global Const QERR_PARSERQUERY = 46

'list logic constants
  Global Const LISTLOGIC_AND = 1
  Global Const LISTLOGIC_OR = 0
  Global Const LISTLOGIC_SUBTRACT = 2

'SetMXRXLockMode intrinsic return values
  Global Const SET_LOCKMODE_SUCCESS = 1
  Global Const SET_LOCKMODE_TRY_AGAIN = 2
  Global Const SET_LOCKMODE_FAILURE = 3

'ReadRDFile/ReadSDFile duplicate constants
  Global Const DUPLICATE_DEFAULT = -1
  Global Const DUPLICATE_SKIP = 1
  Global Const DUPLICATE_HARDMERGE = 2
  Global Const DUPLICATE_PROMPT = 3
  Global Const DUPLICATE_APPEND = 4
  Global Const DUPLICATE_APPENDSUB = 5
  Global Const DUPLICATE_SOFTMERGE = 6
  Global Const DUPLICATE_OVERWRITE = 7
  Global Const DUPLICATE_ABORT = 8

'ReadRDFile/ReadSDFile date format constants
  Global Const DATEORDER_MDY = 0
  Global Const DATEORDER_DMY = 1
  Global Const DATEORDER_YMD = 2
  Global Const DATEORDER_YDM = 3

'ExecuteMolSearch constants for remote searches
  Global Const EXEMOLSEARCH_SSS = 0
  Global Const EXEMOLSEARCH_EXACT = 10
  Global Const EXEMOLSEARCH_LOCAL_DUPCHECK = -10
  Global Const EXEMOLSEARCH_REMOTE_ISOMER = 11
  Global Const EXEMOLSEARCH_REMOTE_TAUTOMER = 12
  Global Const EXEMOLSEARCH_REMOTE_SALT = 13

'database object type constants
  Global Const DBOBJTYPE_STRUCT = -1
  Global Const DBOBJTYPE_LIST = -2
  Global Const DBOBJTYPE_FORM = -3
  Global Const DBOBJTYPE_QUERY = -4
  Global Const DBOBJTYPE_DOMAINLIST = -5
  Global Const DBOBJTYPE_SORTINSTRUCTION = -6
  Global Const DBOBJTYPE_AUTOLIST = -7
  Global Const DBOBJTYPE_PLOBJ = -8

'database field flag constants
  Global Const DBFIELDFLAG_DIRTY = 1                 'set if node needs flushing to disk
  Global Const DBFIELDFLAG_GOTTEN = 2                'set if context up to date
  Global Const DBFIELDFLAG_LOCAL = 4                 'set if local database
  Global Const DBFIELDFLAG_ROOT = 8                  'set if field is a root
  Global Const DBFIELDFLAG_FILTERING = 16     'set if filtering in progress
  Global Const DBFIELDFLAG_HOSTUPDATE = 32    'set if host update ok for field
  Global Const DBFIELDFLAG_COMPONENT = 64     'set if component field
  Global Const DBFIELDFLAG_PARENT = 128                'set if field is a parent
  Global Const DBFIELDFLAG_HIDDEN = 256                'set if field is hidden
  Global Const DBFIELDFLAG_NOUPDATE = 512              'set if field can't be updated
  Global Const DBFIELDFLAG_VOCABUPDATE = 1024   'set if vocab updated
  Global Const DBFIELDFLAG_UPDATEOK = 2048              'set if database can be updated
  Global Const DBFIELDFLAG_INDEXED = 4096               'set field is indexed
  Global Const DBFIELDFLAG_UNLOAD = 8192                'set when field should be unloaded
  Global Const DBFIELDFLAG_UPDATE = 16384                'set when field will be updated
  Global Const DBFIELDFLAG_INSERT = 32768                'set when field will be inserted
  Global Const DBFIELDFLAG_EDITBATCH = 65536     'set when db index edit batch mode
  Global Const DBFIELDFLAG_DELETED = 131072               'set when field has been deleted      (from DB definition)
  Global Const DBFIELDFLAG_COMMITNEEDED = 262144  'set when update without a commit
  Global Const DBFIELDFLAG_INPROGRESS = 524288    'set when search in progress
  Global Const DBFIELDFLAG_RETRIEVE = 1048576              'set when field should be downloaded
  Global Const DBFIELDFLAG_SORTFIELD = 2097152     'set when field involved in sort
  Global Const DBFIELDFLAG_FIRSTHIT = 4194304              'set when searching but no hit
  Global Const DBFIELDFLAG_FLEX = 8388608                  'set when field is host flexible

'derived field types
  Global Const DERIVEDFIELD_MIN = 4
  Global Const DERIVEDFIELD_MAX = 8
  Global Const DERIVEDFIELD_AVG = 12
  Global Const DERIVEDFIELD_SUM = 16
  Global Const DERIVEDFIELD_COUNT = 20
  Global Const DERIVEDFIELD_MEDIAN = 24

'derived field modifiers
  Global Const DERIVEDFIELD_DISTINCT = 1
  Global Const DERIVEDFIELD_NOFILTER = 2

'database field flag constants
  Global Const FLDCHOICE_TOP_LEVEL_ONLY = 1         'only top level fields available
  Global Const FLDCHOICE_STRUCTURE_ONLY = 2         'only structure fields available
  Global Const FLDCHOICE_NO_STRUCTURES = 4          'no structure fields available
  Global Const FLDCHOICE_NO_GRAPHICS = 8            'no graphics fields available
  Global Const FLDCHOICE_NO_STRUCT_DERIVEDFLDS = 16  'no fields which are derived from struct fields available
  Global Const FLDCHOICE_KEY_FIELD_REQUIRED = 32         'for DB export primarily
  Global Const FLDCHOICE_USE_EXTERNAL_FLDNAMES = 64  'use external field names (if present) for display
  Global Const FLDCHOICE_NO_INDENT = 128                          'don't indent for db hierarchy
  Global Const FLDCHOICE_NO_PARENTS = 256                         'don't put parent fields in listboxes
  Global Const FLDCHOICE_NO_CASECHANGE = 512              'don't capitalize parent/lowercase regular fields
  Global Const FLDCHOICE_SINGLE_FLD_SELECT = 1024          'allow only a single fields to be selected
  Global Const FLDCHOICE_NO_UNCOUPLED_TABLES = 2048    'don't allowed uncouple tables to be select
  Global Const FLDCHOICE_NO_AGGREGATE_FIELDS = 4096    'don't allow aggregate fields

'error string resource ids returned by some intrinsics
  Global Const ERROR_STRID_NOKEYFIELD = 5221           '
  Global Const ERROR_STRID_INVALIDKEYFIELD = 5268 '
  Global Const ERROR_STRID_FILEWRITE = 5518            '

'binary file transfer modes
  Global Const BINARY_STANDARD = 0

'remote connection types for OpenDBWithFullInfo()
  Global Const REMOTE_CONNTYPE_NONE = -1
  Global Const REMOTE_CONNTYPE_UNKNOWN = 0
  Global Const REMOTE_CONNTYPE_APPLETALK = 1
  Global Const REMOTE_CONNTYPE_DECNET = 2
  Global Const REMOTE_CONNTYPE_TCP = 3
  Global Const REMOTE_CONNTYPE_SERIAL = 4

'IOResult Error constants
  Global Const IOERR_NOERROR = 0
  Global Const IOERR_FILENOTFOUND = 1
  Global Const IOERR_CANTCREATE = 2
  Global Const IOERR_NOTCLOSED = 3
  Global Const IOERR_PASTEND = 4
  Global Const IOERR_CANTREAD = 5
  Global Const IOERR_CANTWRITE = 6
  Global Const IOERR_NOTBINARY = 7
  Global Const IOERR_READFORMAT = 8
  Global Const IOERR_BADFILEHDL = 9
  Global Const IOERR_FILEOPEN = 10
  Global Const IOERR_OUTOFFILEHDLS = 11
  Global Const IOERR_RENAMEERROR = 12
  Global Const IOERR_INVALIDMODE = 13
  Global Const IOERR_PASTEOLN = 14

'file type constants
  Global Const FILETYPE_SKETCH = 30
  Global Const FILETYPE_FORM = 31
  Global Const FILETYPE_DAT = 32
  Global Const FILETYPE_MOL = 33
  Global Const FILETYPE_RXN = 34
  Global Const FILETYPE_JOURNAL = 35
  Global Const FILETYPE_WMETA = 36
  Global Const FILETYPE_TGF = 37
  Global Const FILETYPE_TEMPLATE = 38
  Global Const FILETYPE_DB = 39
  Global Const FILETYPE_ANY = 40
  Global Const FILETYPE_LIST = 41
  Global Const FILETYPE_SD = 42
  Global Const FILETYPE_XLAT = 43
  Global Const FILETYPE_TABLE = 44
  Global Const FILETYPE_LOG = 45
  Global Const FILETYPE_EPL = 46
  Global Const FILETYPE_CFG = 47
  Global Const FILETYPE_RD = 48
  Global Const FILETYPE_TEXT = 49
  Global Const FILETYPE_TXT = 49
  Global Const FILETYPE_HELP = 50
  Global Const FILETYPE_SEQ = 54
  Global Const FILETYPE_RTF = 51
  Global Const FILETYPE_MDLMETA = 52

' ISIS directory constants
  Global Const ISISDIR_APPLICATION = 90
  Global Const ISISDIR_USERSELECT = 91
  Global Const ISISDIR_PREFERENCES = 92
  Global Const ISISDIR_TEMPLATES = 93
  Global Const ISISDIR_PL = 94
  Global Const ISISDIR_HELPFILES = 95
  Global Const ISISDIR_TMP = 96
  Global Const ISISDIR_PLSTARTUP = 5090
  Global Const ISISDIR_CURRENT_PL_PROG = 5091

' reaction component type constants
  Global Const RXN_REACTANT = 1
  Global Const RXN_PRODUCT = 2

' prototype atom/bond id constants
  Global Const PROTOTYPE_ATOMID = -1
  Global Const PROTOTYPE_BONDID = -1

'pl unit constants
  Global Const UNITS_INCHES = 1
  Global Const UNITS_CENTIMETERS = 2
  Global Const UNITS_DECIPOINTS = 3
  Global Const UNITS_ANGSTROMS = 4

'font emphasis constants
  Global Const FONT_PLAIN = 0
  Global Const FONT_BOLD = 1
  Global Const FONT_ITALIC = 2
  Global Const FONT_UNDERSCORE = 4
  Global Const FONT_SUPERSCRIPT = 8
  Global Const FONT_SUBSCRIPT = 16
  Global Const FONT_FORMULA = 32

' Get/SetSkObjArrowEndPoints constants
  Global Const ARROW_NONE = 0
  Global Const ARROW_LEFT = 1
  Global Const ARROW_RIGHT = 2
  Global Const ARROW_BOTH = 3

'text alignment constants
  Global Const TEXTALIGN_LEFT = 1
  Global Const TEXTALIGN_CENTER = 2
  Global Const TEXTALIGN_RIGHT = 4
  Global Const TEXTALIGN_TOP = 16
  Global Const TEXTALIGN_MIDDLE = 32
  Global Const TEXTALIGN_BOTTOM = 64

'system cursor type constants
  Global Const CURSOR_NORMAL = 4
  Global Const CURSOR_POINTER = 0
  Global Const CURSOR_WAIT = 1
  Global Const CURSOR_CROSS = 2
  Global Const CURSOR_WAITBASE = 3

'pen line style constants
  Global Const LINESTYLE_SOLID = 0
  Global Const LINESTYLE_DASH = 1
  Global Const LINESTYLE_DOT = 2
  Global Const LINESTYLE_DOTDASH = 3
  Global Const PENSTYLE_SOLID = 0
  Global Const PENSTYLE_DASH = 1
  Global Const PENSTYLE_DOT = 2
  Global Const PENSTYLE_DOTDASH = 3

'fill pattern constants
  Global Const FILLPATTERN_NONE = 0
  Global Const FILLPATTERN_SOLID = 1
  Global Const FILLPATTERN_LEFTHASH = 2
  Global Const FILLPATTERN_CHECKED = 3
  Global Const FILLPATTERN_HASHED = 4
  Global Const FILLPATTERN_RIGHTHASH = 5
  Global Const FILLPATTERN_VERTICAL = 7
  Global Const FILLPATTERN_HORIZONTAL = 6

'sketch mode constants
  Global Const SKMODE_CHEM = 1101
  Global Const SKMODE_SKETCH = 1102
  Global Const SKMODE_MOL = 1003

'sketch object constants
  Global Const SKOBJTYPE_RECTANGLE = 2
  Global Const SKOBJTYPE_RECT = 2         ' alternate constant name
  Global Const SKOBJTYPE_ROUNDRECT = 3
  Global Const SKOBJTYPE_ROUNDED_RECT = 3 ' alternate constant name
  Global Const SKOBJTYPE_LINE = 4
  Global Const SKOBJTYPE_POLYGON = 5
  Global Const SKOBJTYPE_ELLIPSE = 6
  Global Const SKOBJTYPE_ARC = 7
  Global Const SKOBJTYPE_POLYLINE = 8
  Global Const SKOBJTYPE_TEXT = 9
  Global Const SKOBJTYPE_GROUP = 10
  Global Const SKOBJTYPE_META = 11
  Global Const SKOBJTYPE_MOL = 12
  Global Const SKOBJTYPE_RXNPLUS = 13
  Global Const SKOBJTYPE_RXNARROW = 14
  Global Const SKOBJTYPE_BRACKETS = 21
  Global Const SKOBJTYPE_RGROUPID = 17
  Global Const SKOBJTYPE_RLOGIC = 20
  Global Const SKOBJTYPE_CHIRAL = 19
  Global Const SKOBJTYPE_BOND = 32
  Global Const SKOBJTYPE_ATOM = 33
  Global Const SKOBJTYPE_SGROUP = 34
  Global Const SKOBJTYPE_DATASGROUP = 35
  Global Const SKOBJTYPE_REACTION = 22
  Global Const SKOBJTYPE_RGROUP = 23
  Global Const SKOBJTYPE_CIRCULAR_ARC = 24
  Global Const SKOBJTYPE_SMOOTH_SPLINE = 25
  Global Const SKOBJTYPE_NOSTRUCTURE = 26
  Global Const SKOBJTYPE_3D_POINT_DISTANCE = 48
  Global Const SKOBJTYPE_3D_POINT_PERCENT = 49
  Global Const SKOBJTYPE_3D_POINT_NORMAL = 50
  Global Const SKOBJTYPE_3D_LINE = 51
  Global Const SKOBJTYPE_3D_PLANE_POINT = 52
  Global Const SKOBJTYPE_3D_PLANE_LINE = 53
  Global Const SKOBJTYPE_3D_CENTROID = 54
  Global Const SKOBJTYPE_3D_NORMAL = 55
  Global Const SKOBJTYPE_3D_DISTANCE_POINT = 56
  Global Const SKOBJTYPE_3D_DISTANCE_LINE = 57
  Global Const SKOBJTYPE_3D_DISTANCE_PLANE = 58
  Global Const SKOBJTYPE_3D_ANGLE_POINT = 59
  Global Const SKOBJTYPE_3D_ANGLE_LINE = 60
  Global Const SKOBJTYPE_3D_ANGLE_PLANE = 61
  Global Const SKOBJTYPE_3D_DIHEDRAL = 62
  Global Const SKOBJTYPE_3D_EXCLUSION = 63
  Global Const SKOBJTYPE_3D_ATOM = 64
  Global Const SKOBJTYPE_3D_ATOMPAIR = 65
  Global Const SKOBJTYPE_3D_CORE_OBJ = 66
  Global Const SKOBJTYPE_3D_VALUE_OBJ = 67
  Global Const SKOBJTYPE_3D_RANGE_OBJ = 68
  Global Const SKOBJTYPE_3D_TOLERANCE_OBJ = 69
  Global Const SKOBJTYPE_3D_POINT_DIST_OBJ = 70
  Global Const SKOBJTYPE_3D_ATOM_DATA_OBJ = 71
  Global Const SKOBJTYPE_3D_POINT_OBJ = 72

'atom mass constants
  Global Const ATOM_DEFAULTMASS = -1
  Global Const ATOM_DEFAULTISOTOPE = -1   ' alternate constant name

'atom valence constant
  Global Const ATOM_DEFAULTVALENCE = 0

'atom radical type constants
  Global Const ATOM_SINGLET = 1
  Global Const ATOM_DOUBLET = 2
  Global Const ATOM_TRIPLET = 3
  Global Const ATOM_NONRADICAL = 0

' constants for Get/Set AtomSubstCount
  Global Const ATOM_SUBSTMAX = 6
  Global Const ATOM_SUBSTASDRAWN = -2
  Global Const ATOM_SUBSTOFF = -3

' constants for Get/Set AtomRBCount
  Global Const ATOM_RBMAX = 4
  Global Const ATOM_RBASDRAWN = -2
  Global Const ATOM_RBOFF = -3

'turn link atom off constant
  Global Const ATOM_LINKATOMOFF = -1

'constants for Get/Set AtomRxnStereo
  Global Const ATOM_RXNSTEOFF = 0
  Global Const ATOM_RXNSTEINV = 1
  Global Const ATOM_RXNSTERET = 2

'DisplayAtomNumbers() atom number modes
  Global Const ATOM_NUMBERS_OFF = 0
  Global Const ATOM_NUMBERS_ON = 1

'DisplayHydrogens() hydrogen display modes
  Global Const HYDROGEN_LABEL_OFF = 0
  Global Const HYDROGEN_LABEL_ON = 2
  Global Const HYDROGEN_LABEL_HETERO = 1
  Global Const HYDROGEN_LABEL_ON_ALL = 3

'bond type constants
  Global Const BOND_SINGLE = 1
  Global Const BOND_DOUBLE = 2
  Global Const BOND_TRIPLE = 3
  Global Const BOND_UP = 17
  Global Const BOND_DOWN = 97
  Global Const BOND_EITHER = 65
  Global Const BOND_DBLEITHER = 50
  Global Const BOND_ANY = 8
  Global Const BOND_AROMATIC = 4
  Global Const BOND_SA = 6
  Global Const BOND_DA = 7
  Global Const BOND_SD = 5

'bond topology constants
  Global Const BOND_TOPORING = 1
  Global Const BOND_TOPOCHAIN = 2
  Global Const BOND_TOPONONE = 0

'bond reaction center constants
  Global Const BOND_RXNCENTNONE = 0
  Global Const BOND_RXNCENTCENTER = 1
  Global Const BOND_RXNCENTNOCHANGE = 2
  Global Const BOND_RXNCENTMAKE = 4
  Global Const BOND_RXNCENTCHANGE = 8
  Global Const BOND_RXNCENTNOCENTER = 15

'r-group atom attatchment point constants
  Global Const ATOM_APOINTPRIMARY = 1
  Global Const ATOM_APOINTSECONDARY = 2
  Global Const ATOM_APOINTDOUBLE = 3
  Global Const ATOM_APOINTOFF = 0

'StereoChemistry constants
  Global Const STEREO_CONFIG_R = 1
  Global Const STEREO_CONFIG_S = -1
  Global Const STEREO_CONFIG_PERCEIVED = 3
  Global Const STEREO_CONFIG_NONE = 0
  Global Const STEREO_CONFIG_E = 2
  Global Const STEREO_CONFIG_Z = -2

'MDL standard ptable constants
  Global Const CARBON = 6
  Global Const HYDROGEN = 1
  Global Const NITROGEN = 7
  Global Const OXYGEN = 8
  Global Const PHOSPHORUS = 15
  Global Const SULFUR = 16
  Global Const CHLORINE = 17
  Global Const BROMINE = 35
  Global Const SILICON = 14
  Global Const BERYLLIUM = 4
  Global Const ALUMINUM = 13
  Global Const ATOM_H = 1
  Global Const ATOM_BE = 4
  Global Const ATOM_C = 6
  Global Const ATOM_N = 7
  Global Const ATOM_O = 8
  Global Const ATOM_AL = 13
  Global Const ATOM_SI = 14
  Global Const ATOM_P = 15
  Global Const ATOM_S = 16
  Global Const ATOM_CL = 17
  Global Const ATOM_BR = 35
  Global Const ATOM_QUERYA = 258 ' atype 'A' = any
  Global Const ATOM_QUERYQ = 257 ' atype 'Q' = hetero
  Global Const ATOM_LIST = 270   ' atom list
  Global Const ATOM_NOTLIST = 271 ' atom not list
  Global Const ATOM_ABBREV = 272 ' group abbreviation
  Global Const ATOM_HDOT = 273
  Global Const ATOM_EXPLC = 274
  Global Const ATOM_RGROUP = 275
  Global Const ATOM_D = 104
  Global Const ATOM_T = 105
  Global Const ATOM_X = 107
  Global Const ATOM_HPLUS = 132
  Global Const ATOM_H2 = 133
  Global Const ATOM_R = 206
  Global Const ATOM_GLY = 108
  Global Const ATOM_ALA = 109
  Global Const ATOM_VAL = 110
  Global Const ATOM_LEU = 111
  Global Const ATOM_ILE = 112
  Global Const ATOM_SER = 113
  Global Const ATOM_THR = 114
  Global Const ATOM_ASP = 115
  Global Const ATOM_ASN = 116
  Global Const ATOM_GLU = 117
  Global Const ATOM_GLN = 118
  Global Const ATOM_LYS = 119
  Global Const ATOM_HYL = 120
  Global Const ATOM_HIS = 121
  Global Const ATOM_ARG = 122
  Global Const ATOM_PHE = 123
  Global Const ATOM_TYR = 124
  Global Const ATOM_TRP = 125
  Global Const ATOM_THY = 126
  Global Const ATOM_CYS = 127
  Global Const ATOM_CST = 128
  Global Const ATOM_MET = 129
  Global Const ATOM_PRO = 130
  Global Const ATOM_HYP = 131

'constants for Sgroup types
  Global Const SGROUP_TYPE_LINKNODE = 1
  Global Const SGROUP_TYPE_ABBREV = 2
  Global Const SGROUP_TYPE_SRU = 3
  Global Const SGROUP_TYPE_MONOMER = 4
  Global Const SGROUP_TYPE_MER = 5
  Global Const SGROUP_TYPE_COPOLYMER = 6
  Global Const SGROUP_TYPE_ALT_COPOLYMER = 7
  Global Const SGROUP_TYPE_RAN_COPOLYMER = 8
  Global Const SGROUP_TYPE_BLK_COPOLYMER = 9
  Global Const SGROUP_TYPE_CROSSLINK = 10
  Global Const SGROUP_TYPE_GRAFT = 11
  Global Const SGROUP_TYPE_MODIFICATION = 12
  Global Const SGROUP_TYPE_COMPONENT = 13
  Global Const SGROUP_TYPE_MIXTURE = 14
  Global Const SGROUP_TYPE_FORMULATION = 15
  Global Const SGROUP_TYPE_MULTIPLEGROUP = 16
  Global Const SGROUP_TYPE_GENERIC = 17
  Global Const SGROUP_TYPE_ANYPOLYMER = 18

'constants for Sgroup Configurations
  Global Const SGROUP_CONFIGURATION_HEAD_TAIL = 0
  Global Const SGROUP_CONFIGURATION_HEAD_HEAD = 1
  Global Const SGROUP_CONFIGURATION_EITHER_UNKNOWN = 2

'constants for Sgroup Bracket Styles
  Global Const SGROUP_BRACKETSTYLE_SQUARE = 0
  Global Const SGROUP_BRACKETSTYLE_CURVED = 1

'constants for Data Sgroup Name Maximum Length
  Global Const DATASGROUP_FIELD_NAME_MAX_LENGTH = 33

'constants for Data Sgroup Field Types
  Global Const DATASGROUP_FIELD_TYPE_NUMERIC = 1
  Global Const DATASGROUP_FIELD_TYPE_FORMATTED = 2
  Global Const DATASGROUP_FIELD_TYPE_TEXT = 3
  Global Const DATASGROUP_FIELD_TYPE_UNKNOWN = 0

'constants for Data Sgroup Tag or Data Position
  Global Const DATASGROUP_ALIGNMENT_AUTO = 1
  Global Const DATASGROUP_ALIGNMENT_TOP_LEFT = 2
  Global Const DATASGROUP_ALIGNMENT_TOP_CENTER = 3
  Global Const DATASGROUP_ALIGNMENT_TOP_RIGHT = 4
  Global Const DATASGROUP_ALIGNMENT_CENTER_LEFT = 5
  Global Const DATASGROUP_ALIGNMENT_CENTER = 6
  Global Const DATASGROUP_ALIGNMENT_CENTER_RIGHT = 7
  Global Const DATASGROUP_ALIGNMENT_BOTTOM_LEFT = 8
  Global Const DATASGROUP_ALIGNMENT_BOTTOM_CENTER = 9
  Global Const DATASGROUP_ALIGNMENT_BOTTOM_RIGHT = 10

'constants for Cheshire intrinsics
  Global Const CCATDEFAULTID = 1

'constants for showstate for StartWindowsApp
  Global Const SW_HIDE = 0
  Global Const SW_SHOWNORMAL = 1
  Global Const SW_NORMAL = 1
  Global Const SW_SHOWMINIMIZED = 2
  Global Const SW_SHOWMAXIMIZED = 3
  Global Const SW_MAXIMIZE = 3
  Global Const SW_SHOWNOACTIVATE = 4
  Global Const SW_SHOW = 5
  Global Const SW_MINIMIZE = 6
  Global Const SW_SHOWMINNOACTIVE = 7
  Global Const SW_SHOWNA = 8
  Global Const SW_RESTORE = 9

'set operation constants for SetXXXXOperation() intrinsics
  Global Const SETOP_TOGGLE_MEMBER = 1
  Global Const SETOP_MEMBER_ON = 2
  Global Const SETOP_MEMBER_OFF = 3
  Global Const SETOP_IS_MEMBER = 4
  Global Const SETOP_NEXT_MEMBER = 5
  Global Const SETOP_COUNT_MEMBERS = 6
  Global Const SETOP_CLEAR_SET = 7
  Global Const SETOP_SET_ALL = 8

' query operator constants
  Global Const OPTYPE_RELATIONAL = 1
  Global Const OPTYPE_MULTIPLICATIVE = 2
  Global Const OPTYPE_ARITHMETIC = 3
  Global Const OPTYPE_LOGICAL = 4
  Global Const OPTYPE_CUMULATIVE = 5
  Global Const OPTYPE_UNARY = 6
  Global Const ARGTYPE_NONE = -4
  Global Const ARGTYPE_MATCHANY = -3
  Global Const ARGTYPE_MATCHNONSTRUCT = -2
  Global Const ARGTYPE_NONSTRUCT = -1
  Global Const ARGTYPE_ANY = 0

' possible return values from GetFieldTextBlankType
  Global Const BLANKTYPE_NULLDATA = 1
  Global Const BLANKTYPE_EMPTYSTRING = 2
  Global Const BLANKTYPE_NOTBLANK = 3

' internal isis/base error constants (returned by GetLastDBError)
  Global Const BSERR_NO_ERROR = 0
  Global Const BSERR_MAJ_NODBOPEN = 1
  Global Const BSERR_MAJ_LIST = 2
  Global Const BSERR_MIN_UNABLETOREADLIST = 1
  Global Const BSERR_MIN_UNABLETOGETROOTLIST = 2
  Global Const BSERR_MIN_LOGICOPFAILED = 3
  Global Const BSERR_MIN_CANTCREATELIST = 4
  Global Const BSERR_MIN_CANTEXPANDLIST = 5
  Global Const BSERR_MIN_CANTCOPYLIST = 6
  Global Const BSERR_MIN_CANTFINDLISTNAME = 8
  Global Const BSERR_MIN_INVALIDLISTTOTAL = 9
  Global Const BSERR_MAJ_NOTOPENFORWRITE = 3
  Global Const BSERR_MAJ_UPDATE = 4
  Global Const BSERR_MIN_HOSTCOMMITFAILED = 1
  Global Const BSERR_MIN_HOSTROLLBACKFAILED = 2
  Global Const BSERR_MIN_INSERTFAILED = 3
  Global Const BSERR_MIN_HOSTPUTFAILED = 4
  Global Const BSERR_MIN_INVALIDFIELD = 5
  Global Const BSERR_MIN_IDCREATEFAILED = 6
  Global Const BSERR_MIN_RECORDCREATEFAILED = 7
  Global Const BSERR_MIN_FLUSH = 8
  Global Const BSERR_MIN_UPDATEBUFFER = 9
  Global Const BSERR_MIN_UPDATEDBFULL = 10
  Global Const BSERR_MAJ_NUMERIC = 5
  Global Const BSERR_MIN_REALUNDERFLOW = 1
  Global Const BSERR_MIN_REALOVERFLOW = 2
  Global Const BSERR_MIN_INTEGEROVERFLOW = 3
  Global Const BSERR_MIN_ZERODIVIDE = 4
  Global Const BSERR_MAJ_PARSEERROR = 6
  Global Const BSERR_MAJ_INIT = 7
  Global Const BSERR_MIN_DBINIT = 1
  Global Const BSERR_MIN_SCRATCH = 2
  Global Const BSERR_MIN_INITMEMORY = 3
  Global Const BSERR_MAJ_NOOPEN = 8
  Global Const BSERR_MIN_VERSION = 1
  Global Const BSERR_MIN_PASSWORD = 2
  Global Const BSERR_MIN_DBINTERNAL = 3
  Global Const BSERR_MIN_TOOMANYDB = 4
  Global Const BSERR_MIN_REMOTEREOPEN = 5
  Global Const BSERR_MIN_NOCONNECT = 6
  Global Const BSERR_MIN_NOREMOTEINFO = 7
  Global Const BSERR_MIN_HVIEW = 8
  Global Const BSERR_MIN_INDEX = 9
  Global Const BSERR_MIN_OPENTOOMANYFIELDS = 10
  Global Const BSERR_MIN_OPENMEMORY = 11
  Global Const BSERR_MIN_REVUPREQUIRED = 12
  Global Const BSERR_MAJ_NETWORK = 9
  Global Const BSERR_MIN_STARTUPFAILED = 1
  Global Const BSERR_MIN_NOSENDAUTH = 2
  Global Const BSERR_MIN_NORECVAUTH = 3
  Global Const BSERR_MIN_BADMAGIC = 4
  Global Const BSERR_MIN_SENDSERVICEREQUEST = 5
  Global Const BSERR_MIN_RECVREQRESPONSE = 6
  Global Const BSERR_MIN_BROKERRESPONSE = 7
  Global Const BSERR_MIN_UNKNOWNRESPONSE = 8
  Global Const BSERR_MIN_TCPSERVICE = 9
  Global Const BSERR_MIN_TCPHOST = 10
  Global Const BSERR_MIN_TCPSOCKET = 11
  Global Const BSERR_MIN_TCPLOADLIB = 12
  Global Const BSERR_MAJ_OBJS = 10
  Global Const BSERR_MIN_NOOBJTABLE = 1
  Global Const BSERR_MIN_NONAMETABLE = 2
  Global Const BSERR_MAJ_CREATE = 11
  Global Const BSERR_MIN_CREATETOOMANYDB = 1
  Global Const BSERR_MIN_FILECREATE = 2
  Global Const BSERR_MIN_CREATETOOMANYFIELDS = 3
  Global Const BSERR_MAJ_DELETE = 12
  Global Const BSERR_MIN_NORECORDS = 1
  Global Const BSERR_MIN_INDEXFIELD = 2
  Global Const BSERR_MIN_NOTPARENT = 3
  Global Const BSERR_MIN_NODELETEINSERT = 4
  Global Const BSERR_MIN_HOSTDELETE = 5
  Global Const BSERR_MAJ_EXPORTDB = 13
  Global Const BSERR_MIN_FIELDLIST = 1
  Global Const BSERR_MIN_NOIDFIELD = 2
  Global Const BSERR_MAJ_EDITINDEX = 14
  Global Const BSERR_MIN_EDITNOTAPARENT = 1
  Global Const BSERR_MIN_EDITDUPLICATE = 2
  Global Const BSERR_MIN_EDITBADLENGTH = 3
  Global Const BSERR_MIN_EDITNOMEMORY = 4
  Global Const BSERR_MIN_EDITREMOTE = 5
  Global Const BSERR_MIN_EDITREADONLY = 6
  Global Const BSERR_MIN_EDITBADFIELD = 7
  Global Const BSERR_MIN_EDITBATCH = 8
  Global Const BSERR_MIN_EDITALREADYOPEN = 9
  Global Const BSERR_MIN_EDITNOOPEN = 10
  Global Const BSERR_MIN_EDITNOBATCH = 11
  Global Const BSERR_MIN_EDITDBCLOSED = 12
  Global Const BSERR_MIN_EDITCANTSAVE = 13
  Global Const BSERR_MIN_EDITCANTCOPYDB = 14
  Global Const BSERR_MIN_EDITINTERNAL = 15
  Global Const BSERR_MIN_EDITTOOMANYFIELDS = 16
  Global Const BSERR_MIN_EDITUPDATEPENDING = 17
  Global Const BSERR_MAJ_REVUP = 15
  Global Const BSERR_MIN_REVUPNOMEMORY = 1
  Global Const BSERR_MIN_REVUPREMOTE = 2
  Global Const BSERR_MIN_REVUPREADONLY = 3
  Global Const BSERR_MIN_REVUPSAVEDEF = 4
  Global Const BSERR_MAJ_SORT = 16
  Global Const BSERR_MIN_SORTBADFIELD = 1
  Global Const BSERR_MIN_SORTWRONGPARENT = 2
  Global Const BSERR_MIN_SORTCANTCREATETREE = 3
  Global Const BSERR_MIN_SORTCANTCREATELIST = 4
  Global Const BSERR_MIN_SORTKEYBUFFER = 5
  Global Const BSERR_MIN_SORTEXPANDLIST = 6
  Global Const BSERR_MIN_SORTBADPARENT = 7
  Global Const BSERR_MIN_SORTUNSORTABLEFIELD = 8
  Global Const BSERR_MIN_SORTFMLAFILE = 9
  Global Const BSERR_MIN_SORTFMLAMEMORY = 10
  Global Const BSERR_MAJ_SDFILE = 17
  Global Const BSERR_MIN_SDFOPENFILE = 1
  Global Const BSERR_MIN_SDFBADSTRUCT = 2
  Global Const BSERR_MIN_SDFNOTDATAORSTRUCT = 3
  Global Const BSERR_MIN_SDFBADALLOC = 4
  Global Const BSERR_MIN_SDFNOTSTRUCTFIELD = 5
