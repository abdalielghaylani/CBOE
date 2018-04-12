<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->

<%
Dim Conn
Dim RS
QtyRemaining = Request("QtyRemaining")
ContainerID = Request("ContainerID")
UOMAbv = Request("UOMAbv")
Action = Request("Action")
NumContainers = Request("NumContainers")
SampleQty = Request("SampleQty")
ContainerSize = Request("ContainerSize")
BarcodeDescID = Request("BarcodeDescID")
RecordsPerPage=10
CurrPage=Request("CurrPage")
if CurrPage="" then CurrPage=1

'for each key in Request.Form
'	Response.Write key & "=" & request(key) & "<BR>"
'next

'sets the selected location and container types
if NumContainers = "" then 
	LocationID = Session("LocationID")
	ContainerTypeID = Session("ContainerTypeID")
	pageAction = "new"
else
	LocationID = Request("LocationID")
	ContainerTypeID = Request("ContainerTypeID")
	pageAction = "edit"
end if
 
If Action = "split" then
	titleText = "Create a Set of Containers from the Original Container"
	instructionTextA = "Enter the total number of containers."
	instructionTextB = "Adjust the amounts in each container."
	actionText = "Number of Containers"
	CaptionPrefix = "Container"
	CaptionSuffix = "Quantity (" & UOMAbv & ")"
	NamePrefix = Action
	SampleQty = 1
elseif Action = "sample" then
	titleText = "Create Samples from the Original Container"
	instructionTextA = "Enter the number of samples you want to create."
	instructionTextB = "Adjust the amounts in each sample."
	actionText = "Number of Samples"
	CaptionPrefix = "Sample"
	CaptionSuffix = "Quantity (" & UOMAbv & ")"
	NamePrefix = Action	
end if

%>
<html>
<head>
<style type="text/css">

	.nextButton{
        display:block;
	}
	.okButton{
	    display:none;
	}
}   
</style>

<title><%=Application("appTitle")%> -- <%=titleText%></title>
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script type="text/javascript" language="javascript">
<%if (Action = "split") and (not pageAction = "new") then %>

function OnInputChangeValue(fieldname){
    
    var PageNum,Field,ContainerNum;
    PageNum = document.form1.CurrPage.value;
    ContainerNum = ((PageNum-1) * <%=RecordsPerPage%>) + fieldname;
    Field = document.getElementById('split' + ContainerNum );
    ContainerNum = ContainerNum % 10;
    if (ContainerNum == 0) 
    ContainerNum = 10;
    Field.value = eval('document.form1.container' + ContainerNum + '.value');
}

function ChangePage(PageValue){

var i;
var ContainerNum;
var field,field1;
if (PageValue != document.form1.CurrPage.value)
 {
      document.form1.CurrPage.value = PageValue;
      for(i=1;i<=<%=RecordsPerPage%>;i++){
          ContainerNum = ((PageValue-1) * <%=RecordsPerPage%>) + i;
          if (ContainerNum > <%=NumContainers%>)
          {
              document.getElementById('caption'+ i).style.display='none';
              document.getElementById('textfield'+ i).style.display='none';
          }
          else{
              if (document.getElementById('caption'+ i).style.display == 'none'){
                  document.getElementById('caption'+ i).style.display='block';
                  document.getElementById('textfield'+ i).style.display='block';
              }
          document.getElementById('caption'+ i).innerHTML = "<span class='required'><%=CaptionPrefix%>" + ContainerNum  +" <%=CaptionSuffix%>:</Span>";   
        
          field = document.getElementById('container' + i );
          field.value = eval('document.form1.split' + ContainerNum + '.value');
          }
      }
  }
}
<%end if%>
<% if (Action = "sample") and (not pageAction = "new") then %>
function UpdateQuantity(){
		var i;
		var totalSampleQuantity,QuantityRemaining; 
		var NumContainers = document.form1.NumContainers.value;
		var Action = "<%=Action%>";
		var ValueList;
		ValueList='';
		totalSampleQuantity=0;
		for(i=1; i<=NumContainers; i++) {
				x = eval("document.form1." + Action + String(i) + ".value;");
				if (ValueList=='')
				{
					ValueList= x;
				}
				else
				{
					ValueList= ValueList + '|' + x;
				}
		}
			totalSampleQuantity = GetSumofValues(ValueList);
		if (totalSampleQuantity<=document.form1.OriginalQty.value){
			document.form1.QtyRemaining.value=GetSumofValues(document.form1.OriginalQty.value + "|" + "-" + totalSampleQuantity);
		}
		else{
			alert('Please fix the following problems:\r-Total quantity cannot exceed original quantity.'); 
		}
}	<% end if %>
<!--Hide JavaScript
	window.focus();
	function ValidateQtyChange(Action){
		
		var bWriteError = false;
		var ContainerSize = document.form1.ContainerSize.value;
		var NumContainers = document.form1.NumContainers.value;
		var SampleQty = document.form1.SampleQty.value;
		var OriginalQty = Number(document.form1.OriginalQty.value);
		var errmsg = "Please fix the following problems:\r";
		var Action = "<%=Action%>";

        var checkNewQuantities = true;
        if (document.form1.pageAction.value == "new"){
            checkNewQuantities = false;
        }
        
<%if Application("RequireBarcode") = "TRUE" then%>
		//barcode is required
		if(document.form1.BarcodeDescID.value.length == 0) {
			errmsg = errmsg + "- Barcode Description is required.\r";
			bWriteError = true;
		}
<% end if%>
		
		//LocationID is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID must be a positive number
			if (document.form1.LocationID.value == 0){
				errmsg = errmsg + "- Cannot create container at the root location.\r";
				bWriteError = true;
			}
			// LocationID must be a positive number
			if (!isPositiveNumber(document.form1.LocationID.value)&&(document.form1.LocationID.value != 0)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}	

		// Container Size is required
		if (document.form1.ContainerSize.value.length == 0) {
			errmsg = errmsg + "- Container Size is required.\r";
			bWriteError = true;
		}
		else{
			// Container Size must be a number
			if (!isNumber(ContainerSize)){
				errmsg = errmsg + "- Container Size must be a number.\r";
				bWriteError = true;
			}
			if (ContainerSize <= 0){
				errmsg = errmsg + "- Container Size must be a positive number.\r";
				bWriteError = true;
			}
		}
		// Number of Containers is required
		if (document.form1.NumContainers.value.length == 0) {
			errmsg = errmsg + "- <%=actionText%> is required.\r";
			bWriteError = true;
		}
		else{
			// Number of Containers must be a number
			if (!isNumber(NumContainers)){
				errmsg = errmsg + "- <%=actionText%> must be a number.\r";
				bWriteError = true;
			}
			if (NumContainers < 1){
				errmsg = errmsg + "- <%=actionText%> must be a positive number.\r";
				bWriteError = true;
			}
			if (!IsWholeNumber(NumContainers)) {
				errmsg = errmsg + "- <%=actionText%> must be a positive whole number.\r";
				bWriteError = true;
			}
		}
		// For a split, 2 containers required
		if (!bWriteError && Action == "split" && Number(NumContainers) < 2) {
			errmsg = errmsg + "- You must split the container into at least 2 containers.\r";
			bWriteError = true;		
		}
		// For a split, the max number of comtainer is 400
		if (!bWriteError && Action == "split" && Number(NumContainers) > 400) {
			errmsg = errmsg + "- Number of Containers must be less than 400.\r";
			bWriteError = true;		
		}
    	// determine whether there are enough open rack positions
		if (bWriteError == false && document.form1.isRack.value == "1") {
		    if (AreEnoughRackPositions(NumContainers,document.form1.LocationID.value) == false) {
				errmsg = errmsg + "- There are not enough open positions from the selected start position\rin the rack(s) you have selected.\r";
				bWriteError = true;
		    }
		}

		if (Action == "sample"){
			// Quantity per Sample is required
			if (document.form1.SampleQty.value.length == 0) {
				errmsg = errmsg + "- Quantity per Sample is required.\r";
				bWriteError = true;
			}
			else{
				// Quantity per Sample must be a number
				if (!isNumber(SampleQty)){
					errmsg = errmsg + "- Quantity per Sample must be a number.\r";
					bWriteError = true;
				}
				if (SampleQty <= 0){
					errmsg = errmsg + "- Quantity per Sample must be a positive number.\r";
					bWriteError = true;
				}
			}
			// Sample size cannot exceed container size
			if (!bWriteError && (Number(SampleQty) > Number(ContainerSize))) {
				errmsg = errmsg + "- Quantity per Sample cannot exceed Container Size.\r";
				bWriteError = true;
				//update the SampleQty field
				document.form1.SampleQty.value = ContainerSize;
				//document.form1.SampleQty.focus();
				SampleQty = ContainerSize;
			}
				
			//check the sample quantity doesn't create more quantity than is available
			if (!bWriteError && checkNewQuantities == false && Action == "sample" && ((NumContainers * SampleQty) > OriginalQty)) {
				errmsg = errmsg + "- Total quantity cannot exceed original quantity.\r";
				bWriteError = true;
				//update the SampleQty field
				document.form1.SampleQty.value = FormatNumber(OriginalQty/NumContainers,2,true,true);
				//document.form1.SampleQty.focus();
			}
		
		}
		//validation for the form with the new quantity fields
		if (checkNewQuantities == true) {
			var Sum,x,y,ValueList;
			Sum = 0;
			ValueList='';
			var arrValues = new Array(NumContainers);
				
			
			for(i=1; i<=NumContainers; i++) {
				x = eval("document.form1." + Action + String(i) + ".value;");
				if (ValueList=='')
				{
					ValueList= x;
				}
				else
				{
					ValueList= ValueList + '|' + x;
				}
				arrValues[i-1] = x;
			}
			Sum = GetSumofValues(ValueList);

			for (i=0; i<NumContainers; i++) {
				//must be a number
				if (!isNumber(arrValues[i])) {
					errmsg = errmsg + "- All quantities must be numbers.\r";
					bWriteError = true;
					break;
				}
				//must be positive numbers
				if (arrValues[i] <= 0) {
					errmsg = errmsg + "- All quantities must be positive numbers.\r";
					bWriteError = true;
					break;
				}
				//can't contain more quantity than the container size
				if (Number(arrValues[i]) > Number(ContainerSize)) {
					errmsg = errmsg + "- Quantity cannot exceed container size.\r";
					bWriteError = true;
					break;
				}
			}
			
			//check the sum of the inputed container values doesn't exceed the original quantity
			if (!bWriteError  && (Sum > OriginalQty)) {
				errmsg = errmsg + "- Total quantity cannot exceed original quantity.\r";
				bWriteError = true;
			}
			// make sure the total original amount has been allotted
			if (!bWriteError && Action == "split"){
				if (OriginalQty - Sum != 0){
					errmsg = errmsg + "- Total quantity must equal the original quantity.\r- Container 1 reflects the difference.\r";
					document.form1.split1.value = FormatNumber(Number(document.form1.split1.value) + (OriginalQty - Sum),2,true,true);
					bWriteError = true;
				}
			}	
			else if (!bWriteError && Action == "sample") {
				Sum = Number(Sum) + Number(document.form1.QtyRemaining.value);
				if (OriginalQty - Sum != 0){
					errmsg = errmsg + "- Total quantity must equal the original quantity.\r- Quantity Remaining reflects the difference.\r";
					document.form1.QtyRemaining.value = FormatNumber(Number(document.form1.QtyRemaining.value) + (OriginalQty - Sum),2,true,true);
					bWriteError = true;
				}
			}
		}
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
		    var chooseSampleQuantities = false;
		    if (document.form1.chooseSampleQuantities) {
		         if (document.form1.chooseSampleQuantities.checked == true) {
		            chooseSampleQuantities = true;
                    }		            
		    }   
		    if(document.form1.pageAction.value == "new" && chooseSampleQuantities ) {
		        document.form1.action="#";
		    }
		    else{
		        if(document.form1.pageAction.value == "new" && Action == "split" && chooseSampleQuantities) {
		            document.form1.action="#";
		        }
		        else{
		            document.form1.action = "AllotContainer_action.asp";
		            
		        }
		    }
		    document.form1.submit();
		}
	}
	
function FormatNumber(num, decimalNum, bolLeadingZero, bolParens)
   /* IN - num:            the number to be formatted
           decimalNum:     the number of decimals after the digit
           bolLeadingZero: true / false to use leading zero
           bolParens:      true / false to use parenthesis for - num
      RETVAL - formatted number
   */
   {
       var tmpNum = num;
       // Return the right number of decimal places
       tmpNum *= Math.pow(10,decimalNum);
       tmpNum = Math.floor(tmpNum);
       tmpNum /= Math.pow(10,decimalNum);
       var tmpStr = new String(tmpNum);
       // See if we need to hack off a leading zero or not
       if (!bolLeadingZero && num < 1 && num > -1 && num !=0)
           if (num > 0)
               tmpStr = tmpStr.substring(1,tmpStr.length);
           else
               // Take out the minus sign out (start at 2)
               tmpStr = "-" + tmpStr.substring(2,tmpStr.length);                        
       // See if we need to put parenthesis around the number
       if (bolParens && num < 0)
           tmpStr = "(" + tmpStr.substring(1,tmpStr.length) + ")";
       return tmpStr;
   }	
   
   function setButtons(element) {
    if(element.checked==true) {
    	AlterCSS('.okButton','display','none');
    	AlterCSS('.nextButton','display','block');
    }
    else {
    	AlterCSS('.okButton','display','block');
    	AlterCSS('.nextButton','display','none');
    }
   }
-->
</script>

</head>
<body>
<center>
<form name="form1" action="#" method="POST" onsubmit="ValidateQtyChange('<%=Action%>');return false;">
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="Action" value="<%=Action%>">
<input Type="hidden" name="GetData" value="db">
<input type="hidden" name="DateCertified" value="<%=Session("DateCertified")%>">
<input type="hidden" name="pageAction" value="<%=pageAction%>" />
<input Type="hidden" name="CurrPage" value="<%=CurrPage%>">
<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">
			<%
			if pageAction = "new" then
				Response.Write instructionTextA
			else
				Response.Write instructionTextB
			end if
			%>
			</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Original Quantity (<%=UOMAbv%>):
		</td>
		<td>		
			<%if pageAction = "new" then%>
				<input TYPE="text" SIZE="10" Maxlength="50" NAME="OriginalQty" class="readOnly" VALUE="<%=QtyRemaining%>" READONLY>
			<%else%>
				<input TYPE="text" SIZE="10" Maxlength="50" NAME="OriginalQty" class="readOnly" VALUE="<%=Request("OriginalQty")%>" READONLY>
			<%end if%>
		</td>
	</tr>

	<tr height="25">
<%if Application("RequireBarcode") = "TRUE" then%>
		<td align="right" valign="top" nowrap width="150"><span class="required">Barcode Description:</span></td>
<%else%>
		<td align="right" valign="top" nowrap width="150">Barcode Description:</td>
<%end if%>	
		<td>
				<%=ShowSelectBox2("BarcodeDescID", BarcodeDescID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null," ","")%>
		</td>
	</tr>
	
	<tr height="25">
		<td align="right" valign="top" nowrap>
			<span class="required">Sample Location ID:</span>
		</td>
		<td colspan="3">
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false%> 
		</td>
	</tr>
	<tr>
		<%=ShowPickList("<SPAN class=required>Sample Container Type:</span>", "ContainerTypeID", ContainerTypeID,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required"><%=actionText%>:</span>
		</td>
		<td>
		<%if pageAction = "new" then%>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="NumContainers" VALUE>
		<%else%>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="NumContainers" class="readOnly" VALUE="<%=NumContainers%>" READONLY>
		<%end if%>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Container Size (<%=UOMAbv%>):</span>
		</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="ContainerSize" VALUE="<%=ContainerSize%>">
		</td>
	</tr>	
	<%if pageAction = "new" and action = "split" then%>
	<tr>
	    <td></td>
	    <td><input type="checkbox" name="chooseSampleQuantities" value="1" checked onclick="setButtons(this);"/>Adjust individual container quantites</td>
	</tr>
	<%end if%>
	<%if pageAction = "new" and action = "sample" then%>
	<tr>
		<td align="right" nowrap>
			<span class="required">Quantity per Sample (<%=UOMAbv%>):</span>
		</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="SampleQty" VALUE>
		</td>
	</tr>	
	<tr>
	    <td></td>
	    <td><input type="checkbox" name="chooseSampleQuantities" value="1" checked onclick="setButtons(this);"/>Adjust individual sample quantites</td>
	</tr>
	<%else%>
		<input TYPE="hidden" SIZE="10" Maxlength="50" NAME="SampleQty" VALUE="<%=SampleQty%>">
	<%end if%>
	
	<%if action = "sample" and pageAction = "edit" then%>
	<tr>
		<td align="right" nowrap>
			Quantity Remaining (<%=UOMAbv%>):
		</td>
		<td>		
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="QtyRemaining" VALUE="<%=(QtyRemaining - (NumContainers*SampleQty))%>">
		</td>
	</tr>
	<%end if%>

	<%
	if pageAction = "new" then 
	%>
	<tr>
		<td colspan="2" align="right"> 
			<span id="nextButton" class="nextButton"><a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a><input type="image" SRC="/cheminv/graphics/sq_btn/next_dialog_btn.gif" border="0"></span>
            <span id="okButton" class="okButton"><a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a><input type="image" SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></span>
		</td>
	</tr>	
	<%else
		arrValues = SetInitialQty(Action, QtyRemaining, NumContainers, SampleQty)
		if action = "sample" then 
			Response.Write GenerateFields2(CaptionPrefix, CaptionSuffix, NamePrefix, NumContainers, arrValues,"UpdateQuantity")
		else
			Response.Write GenerateFields3(CaptionPrefix, CaptionSuffix, NamePrefix, NumContainers, arrValues,RecordsPerPage,CurrPage,"ChangePage(document.form1.selectpage.options[selectedIndex].value);", "OnInputChangeValue")
		end if 	
	%>
	<input type="hidden" name="chooseSampleQuantities" value="<%=Request("chooseSampleQuantities")%>" />
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.back();"><img SRC="/cheminv/graphics/sq_btn/btn_back_61.gif" border="0"></a>
			<a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>
			<input type="image" SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0" id="image2" name="image2">
		</td>
	</tr>	
	<%end if%>
</table>	
</form>
</center>
</body>
</html>
<%
'****************************************************************************************
'*	PURPOSE: 
'*	INPUT: 
'*	OUTPUT: 
'****************************************************************************************
Function SetInitialQty(Action, QtyRemaining, NumContainers, SampleQty)
	Dim arrValues()
	Redim arrValues(NumContainers-1)
	for i = 0 to (NumContainers-1)
		if action = "split" then
			arrValues(i) = FormatNumber(QtyRemaining/NumContainers)
		else 
			arrValues(i) = SampleQty
		end if
	next
	'make sure that the sum of the initial values is equal to the original quantity
	if action = "split" then
		sum = (arrValues(0))*NumContainers
		if sum <> QtyRemaining then
			arrValues(0) = FormatNumber(arrValues(0) + (QtyRemaining - Sum))
		end if
	end if
	SetInitialQty = arrValues
End function	
%>
