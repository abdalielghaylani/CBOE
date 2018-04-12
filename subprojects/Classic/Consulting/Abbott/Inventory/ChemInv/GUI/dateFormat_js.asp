<script language="javascript">
//necessary to run CORE date format
//has a dependence on "/cfserverasp/source/utility_func_vbs.asp"

var Version = navigator.appVersion;
var button_gif_path = "";
var dateFormat = "<%=Application("DATE_FORMAT")%>";
var dateFormatString = "<%=Application("DATE_FORMAT_STRING")%>";

//Name: isDate(inputVal)
//Purpose: check if value is valid date in US, European or Asian format
//Parameters: input value
//Return Values: boolean
//Comments:none

function isDate(inputVal){
	var m, d, y;
	
	if (inputVal.length > 0){
		inputStr = inputVal.toString()		
		
		strArray = inputStr.split( "/")             
		
		//SYAN added 12/19/2003 to fix CSBR-35466
		if ('<%=Application("DATE_FORMAT")%>' == '8') { //US mm/dd/yyyy
			m = 0; //index of the array storing month
			d = 1; //index of the array storing day
			y = 2; //index of the array storing year
		}
		
		if ('<%=Application("DATE_FORMAT")%>' == '9') { //European dd/mm/yyyy
			m = 1; //index of the array storing month
			d = 0; //index of the array storing day
			y = 2; //index of the array storing year
		}
		
		if ('<%=Application("DATE_FORMAT")%>' == '10') { //Asian yyyy/mm/dd
			m = 1; //index of the array storing month
			d = 2; //index of the array storing day
			y = 0; //index of the array storing year
		}
		//End of SYAN modification

		//3 array elements means day, month, and year
		if(strArray.length == 3){
			//Test the value of each element falls in an acceptable range
			if ((isPosLongInteger(strArray[m])!=true) ||(isPosLongInteger(strArray[d])!=true) ||(isPosLongInteger(strArray[y])!=true)){
				return false;
			}
			
			if ((strArray[m].indexOf(">")==1)||(strArray[m].indexOf("<")==1)||(strArray[m].indexOf("<=")==1)||(strArray[m].indexOf(">=")==1)||(strArray[m].indexOf("=")==1)){
				return true
			}
			else {
				if ((strArray[m] < 0) || (strArray[m] >12)){
					return false;
				}
			}
			
			if ((strArray[d] < 0) || (strArray[d] >31)){
				return false;
			}
					
			if ((strArray[y].length ==1)||(strArray[y].length == 2) || (strArray[y].length ==3)||(strArray[y].length >4)||(strArray[y].length <1)){
				return false;
			}
			//if none of the above fail then set return value to true
			return true;
		}
		else{
			//strArray.length not = 3
			return false
		}
	}
	else{
		//inputVal.length !> 0 
		return false;
	}
}

//Name: getDate(inputVal)
//Purpose: converts a date string into a JS date object, this is useful when the date format is not the same as the server date format
//Parameters: input value
//Return Values: JS date object
//Comments:none

function getDate(inputVal) {
	var oDate;
	var m,d,y;
	oDate = new Date();

	if (inputVal.length > 0){
		inputStr = inputVal.toString()		
		
		strArray = inputStr.split( "/")             
		if ('<%=Application("DATE_FORMAT")%>' == '8') { //US mm/dd/yyyy
			m = 0; //index of the array storing month
			d = 1; //index of the array storing day
			y = 2; //index of the array storing year
		}

		if ('<%=Application("DATE_FORMAT")%>' == '9') { //European dd/mm/yyyy
			m = 1; //index of the array storing month
			d = 0; //index of the array storing day
			y = 2; //index of the array storing year
		}
		
		if ('<%=Application("DATE_FORMAT")%>' == '10') { //Asian yyyy/mm/dd
			m = 1; //index of the array storing month
			d = 2; //index of the array storing day
			y = 0; //index of the array storing year
		}
		//3 array elements means day, month, and year
		if(strArray.length == 3){
			oDate.setMonth(strArray[m]-1);
			oDate.setDate(strArray[d]);
			oDate.setFullYear(strArray[y]);
			return oDate;
		}	
		else{
			//strArray.length not = 3
			return null;
		}
	}
	else{
		//inputVal.length !> 0 
		return null;
	}


}

</script>
