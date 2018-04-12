
Configuration of Display Fields. Please do not change the fIeldnames or add or remove any of the items in the list. If you
see NOT CONFIGURABLE next to the field this means it is not configurable.

FieldName ; Input Field : Option [; Display Field : Option]

Display Field :Option is optional. It is used only when the display field has some sort of formatting consideration.
For example, CHECKBOX:GIF, FORMULA:24, HYPERLINK:FULL, HYPERLINK:LINK are the only valid options.

NOTE Input Field covers search mode, add record and edit mode. Validation will occur in all modes. Default values will
only be used in Add mode.

Disply Field is really used on when the display type changes for exampe CHECKBOX:GIF and HYPERLINK:FULL or HYPERLINK:LIND

Validate Option	1: valid integer
		2: float
		3: Positive Integer
		5: positive Float
		8: Data mm/dd/yyyy
		21: cAS Number
		22: not empty
		24: Formula

CheckBox Option 0   Shows raw value
		GIF shows a gif with an X for values of "" or 0 and a check for values of "1"

Hyperlink option FULL displays full value in field as link text. Add http:// if missing
		 Link displays the work link as the link text. Adds http:// if missing

Default Values Addin a "!" after the last portion of the InputType: Option definition will input that value when in add mode or set the PickList to that value.
Valid for Text and Picklist only

PickList items must be separated by pipes "|", default value added after last item in pick list separated by ":"
PickList validation: Add ":VALIDATE:myScript(this)" where myScript(this) is a valid script in app_js.js to add script validation for add mode.


Intput Type:  	Text
		Checkbox
		TextArea
		Picklist
		Hyperlink

Result Type:    Same (this the default - uses the Input Type where relevant
		Checkbox
		Hyperlink
		Formula - formats formula using superscripts and supscripts as long as FORMAT_FORMULA=1 earlier in cfserver.ini file.
		
	 	