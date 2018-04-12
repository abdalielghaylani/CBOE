//////////////////////////////////////////////////////////////////////////////////////////
// Generic Cookie Writer
//	
	function WriteCookie(name,value, expdate){
		if (!expdate){
		expdate = new Date();
		expdate.setFullYear(expdate.getFullYear() +1);
		}
		var wholecookie= name + "=" + value + "; expires=" + expdate.toGMTString() 
		document.cookie =  wholecookie;
	}



//validate Postive number
function isPositiveNumber(value){
	//Remove commas
	value = value.replace(/,/gi,"");
	if (isNumber(value)){
		if (value > 0) return true;
	}
	return false;
}

//validate Positive number of type long
function isPositiveLongNumber(value){
	//Remove commas
	value = value.replace(/,/gi,"");
	if (isNumber(value)){
		if (value > 0 && value < 2147483648) return true;
	}
	return false;
}
 

//validate Whole number
function isWholeNumber(value){
	//Remove commas
	value = value.replace(/,/gi,"");
	if (isNumber(value)){
		if (value >= 0) return true;
	}
	return false;
}

// Validate numerical attribute
	function isNumber(object_value){
	    //Returns true if value is a number or is NULL
	    //otherwise returns false	

	    if (object_value.length == 0)
	        return true;

	    //Returns true if value is a number defined as
	    //   having an optional leading + or -.
	    //   having at most 1 decimal point.
	    //   Commas are removed before checking
	    //   otherwise containing only the characters 0-9.
		var start_format = " .+-0123456789";
		var number_format = " .0123456789";
		var check_char;
		var decimal = false;
		var trailing_blank = false;
		var digits = false;
		
		object_value = object_value.replace(/,/gi,"");
		
	    //The first character can be + - .  blank or a digit.
		check_char = start_format.indexOf(object_value.charAt(0))
	    //Was it a decimal?
		if (check_char == 1)
		    decimal = true;
		else if (check_char < 1)
			return false;
	        
		//Remaining characters can be only . or a digit, but only one decimal.
		for (var i = 1; i < object_value.length; i++)
		{
			check_char = number_format.indexOf(object_value.charAt(i))
			if (check_char < 0)
				return false;
			else if (check_char == 1)
			{
				if (decimal)		// Second decimal.
					return false;
				else
					decimal = true;
			}
			else if (check_char == 0)
			{
				if (decimal || digits)	
					trailing_blank = true;
	        // ignore leading blanks

			}
		        else if (trailing_blank)
				return false;
			else
				digits = true;
		}	
	    //All tests passed, so...
	    return true
	}
	
	
// check if a field is empty
function isEmpty(inputVal){
	if(inputVal == "" || inputVal == null){
		return true
	}
	return false
}

//master function to check whether a field contains the right type of data

function isValid(elm, type){
	
	var inputval = ""
	//type integer
	if(type == 1){
		inputval = elm
		if (inputval.value != ""){
			if (!isLongInteger(inputval.value)){
				alert("Please enter a positve or negative integer." + "\n\n" + "-, >, <, <=, >=, and = allowed at start of number, - allowed within range.")
				inputval.focus()
			}
		}
	}
	// type - floating point integer
	if(type == 2){
		inputval = elm
		if (inputval.value != ""){
			if (!isFPInteger(inputval.value)){
				alert("Please enter a positive or negative number." + "\n\n" + "-, >, <, <=, >=, and = allowed at start of number, - allowed within range.")
				inputval.focus()
			}
		}
	}
	//positive - integer
	if(type == 3){
		inputval = elm
		if (inputval.value != ""){
			if (!isPosLongInteger(inputval.value)){
				alert("Please enter a positve integer." + "\n\n" + ">, <, <=, >=, and = allowed at start of number, - allowed within range.")
				inputval.focus()
			}
			
		}
	}
	
	
	//type - positive float/ MW
	if(type == 5){
		inputval = elm
		if (inputval.value != ""){
			if (!isPosFPInteger(inputval.value)){
				alert("Entry must be a positive number." + "\n\n" + ">, <, <=, >=, and = allowed at start of number, - allowed within range.")
				inputval.focus()
			}
		}
	}
	//type - date
	if(type == 8){
		isDateMaster(elm, type)
	}
	
	
	//type - date
	if(type == 9){
		isDateMaster(elm, type)
	}
	//isCas
	if(type == 21){
		inputval = elm
		if (inputval.value.length >0 ){
			if (inputval.value.indexOf('*')==-1){
				if (!isCAS(inputval.value)){
					alert("The number you entered is not a valid CAS Number.");
					inputval.focus()
				}
				
			}
		}
	}
	//not empty
	if(type == 22){
		inputval = elm
	
		if (!inputval.value.length>0){
			alert("This field cannot be empty");
				inputval.focus()
			}
			
	}
	
	if(type == 24){
		inputval = elm
		if (inputval.value.length>0){
			if (!isFirstLetter(inputval.value)){
				alert("Formula queries may only begin with a letter.")
					inputval.focus()
				}
		}
			
	}
	if (type == 25) { //full text input field
		inputval = elm
	}
	

return(true)
}

	function isDateMaster(inputval, type){
			
			var resultText = ""
				//if (formmode.toLowerCase().indexOf("search") != -1){
				// force a plain date check
				if (false){
					if (inputval.value.indexOf("-")== -1){
					
						if (inputval.value.length >0 ){
								
							if (!isDate(inputval.value)){
								
									if (type == 8){
										resultText = "Entry must be in date using the format 'mm/dd/yyyy'"
									}
									if (type == 9){
										resultText = "Entry must be in date using the format 'dd/mm/yyyy'"
									}
								
							}
						}
					}
						
					else{
						
							resultText = checkDateRange(inputval, type)
					}
				
				}
						
				else{
					//old way used when not in search mode - ranges make no sense
						
						if (inputval.value.length >0 ){
							
							if (!isDate(inputval.value)){
								if (type ==8 ){
									resultText="Entry must be in date using the format 'mm/dd/yyyy'"
								}
								if (type ==9 ){
									resultText="Entry must be in date using the format 'dd/mm/yyyy'"
								}
							}
						}
				}
			if (resultText.length > 0 ){
				alert(resultText)
				inputval.focus()
			}
			return true
			
	
	}

function checkDateRange(inputval,type){
	var myAlert = false
	var returnText = ""
	mydatearray = inputval.value.split("-")
		if (mydatearray[0].length>0 ){
			if (!isDate(mydatearray[0])){
				myAlert = true
			}
			if (!myAlert == true){
				for(i=1;i<mydatearray.length;i++){
					if (mydatearray[i].length>0){
						if (!isDate(mydatearray[i])){
							myAlert = true
						}
					}
				}
			}
		}

		if (myAlert == true){
			if (type == 8){
				returnText = "Entry must be in date using the format 'mm/dd/yyyy-mm/dd/yyyy'"
			}
			if (type == 9){
					returnText = "Entry must be in date using the format 'dd/mm/yyyy-dd/mm/yyyy'"

			}
		}

	return returnText
}


//check if Case
function isCAS(maybeCAS) {
	if ((isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-1,maybeCAS.length)))==false) 
		&& (maybeCAS.substring(maybeCAS.length-2,maybeCAS.length-1)=="-")
		&& (isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-4,maybeCAS.length-2)))==false) 
		&& (maybeCAS.substring(maybeCAS.length-5,maybeCAS.length-4))=="-"
		&& (isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-6,maybeCAS.length-5)))==false)) {
		var firstdash=maybeCAS.indexOf("-", 1);

		if ((firstdash<=9) && (isNaN(maybeCAS.substring(1,firstdash))==false)){
			var seconddash=maybeCAS.indexOf("-", firstdash+1);

			var isCAS=false;

			dashfree_maybeCAS=maybeCAS.substring(0,firstdash)+maybeCAS.substring(firstdash+1,firstdash+3)+maybeCAS.substring(seconddash+1,maybeCAS.length)
			CASsum=0;
			i=dashfree_maybeCAS.length-1; 
			while (i>=1) {
			CASsum=CASsum + i*(dashfree_maybeCAS.substring(dashfree_maybeCAS.length-i-1,dashfree_maybeCAS.length-i));
			i--;
			}


		if ((CASsum%10)==((dashfree_maybeCAS.substring(dashfree_maybeCAS.length-1,dashfree_maybeCAS.length))%10)) {
			isCAS=true;		
			return(true);
			
		}

		else {
			return(false);
			}
		}
		}

	else 
		{
		return(false);
		}
}



//check if value is within a range
function inRange(inputVal, range){
if (inputVal.length > 0){
	num = parseInt(inputVal)
	temp = range.split("-")
	min = parseInt(temp[0])
	max = parseInt(temp[1])
		if (num < min || num > max) {
			return false
		}
		return true
	}
return true
}

//check if value if formated as data FORMAT DD/MM/YYYY
function isDateOLD(inputVal){
	if (inputVal.length > 0){
		inputStr = inputVal.toString()		
		strArray = inputStr.split( "/")             
		//3 array elements means day, month, and year
		if(strArray.length == 3){
			//Test the value of each element falls in an acceptable range
			if ((isPosLongInteger(strArray[0])!=true) ||(isPosLongInteger(strArray[1])!=true) ||(isPosLongInteger(strArray[2])!=true)){
				return false;
			}
			if ((strArray[0].indexOf(">")==1)||(strArray[0].indexOf("<")==1)||(strArray[0].indexOf("<=")==1)||(strArray[0].indexOf(">=")==1)||(strArray[0].indexOf("=")==1)){
				return true}
				else{
					if ((strArray[0] < 0) || (strArray[0] >12)){
						return false;
					}
				}
			if ((strArray[1] < 0) || (strArray[1] >31)){
				return false;
			}
					
			if (strArray[2].length != 4){
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


//check if value if formated as data FORMAT MM/DD/YYYY
function isDate2(inputVal){
	if (inputVal.length > 0){
		inputStr = inputVal.toString()		
		strArray = inputStr.split( "/")             
		//3 array elements means day, month, and year
		if(strArray.length == 3){
		//Test the value of each element falls in an acceptable range
			for (var i = 0; i < strArray.length; i++){
				if ((isPosLongInteger(strArray[0])!=true) ||(isPosLongInteger(strArray[1])!=true) ||(isPosLongInteger(strArray[2])!=true)){
					return false;
				}
				if ((strArray[0] < 0) || (strArray[0] >31)){
					return false;
				}
				if ((strArray[1] < 0) || (strArray[1] >12)){
					return false;
				}
				if (strArray[2].length != 4){
					return false;
				}
			return true;
			}
		return false;
		}
	return false;
	}
}
	
function showAlert(msg){
	alert(msg)
	return false
}
//check if input is a positive or negative number
function isFPInteger(inputVal){
	var compareSign = false
	var oneDecimal = false
	var oneDash = false
	if (inputVal.length > 0){
			inputStr = inputVal.toString()		
			for(var i=0; i< inputStr.length; i++){
			var oneChar = inputStr.charAt(i)
			if (i == 0 && oneChar == "-") {
				continue
			}
			if(i == 0 && oneChar == "="){
					continue
					}
			if(i == 0 && ((oneChar == "<") || (oneChar == ">"))){
					compareSign = true
					continue
					}
			if(i == 1 && ((oneChar == "=") && (compareSign == true))){
					continue
					}
			if(oneChar == "." && !oneDecimal) {
				oneDecimal = true
				continue
			}
			if(oneChar == "-" && !oneDash) {
				oneDash = true
				continue
			}
			if(oneChar<"0" || oneChar>"9") {
				return false
			}
		}
	
		return true
	}
	return true
}


//check if input is a positive or negative number
function isFirstInt(inputVal){
	theResult=false
	if (inputVal.length > 0){
		inputStr = inputVal.toString()		
		var oneChar = inputStr.charAt(0)
				
			if(oneChar=="0"){
				theResult= true
			}
			if(oneChar=="1"){
				theResult= true
			}
			if(oneChar=="2"){
				theResult= true
			}
			if(oneChar=="3"){
				theResult= true
			}
			if(oneChar=="4"){
				theResult= true
			}
			if(oneChar=="5"){
				theResult= true
			}
			if(oneChar=="6"){
				theResult= true
			}
			if(oneChar=="7"){
				theResult= true
			}
			if(oneChar=="8"){
				theResult= true
			}
			if(oneChar=="9"){
				theResult= true
			}
		}
		return theResult
	}
	
//check if input is a positive or negative number
function isFirstLetter(inputVal){
	theResult=false
	if (inputVal.length > 0){
		inputStr = inputVal.toString()		
		var oneChar = inputStr.charAt(0)
			if(oneChar.toLowerCase()=="a"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="b"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="c"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="d"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="e"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="f"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="g"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="h"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="i"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="j"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="k"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="l"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="m"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="n"){
				theResult= true
			}			
			if(oneChar.toLowerCase()=="o"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="p"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="q"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="r"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="s"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="t"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="u"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="v"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="w"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="x"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="y"){
				theResult= true
			}
			if(oneChar.toLowerCase()=="z"){
				theResult= true
			}
		}
		return theResult
	}
// check if input is a postive integer
function isPosFPInteger(inputVal){
	var compareSign = false
	var oneDecimal = false
	var oneDash = false
	if (inputVal.length > 0){
			inputStr = inputVal.toString()		
			for(var i=0; i< inputStr.length; i++){
			var oneChar = inputStr.charAt(i)
			if (i == 0 && oneChar == "-") {
				return false
			}
			if(i == 0 && oneChar == "="){
					continue
					}
			if(i == 0 && ((oneChar == "<") || (oneChar == ">"))){
					compareSign = true
					continue
					}
			if(i == 1 && ((oneChar == "=") && (compareSign == true))){
					continue
					}
			if(oneChar == "." && !oneDecimal) {
				oneDecimal = true
				continue
			}
			if(oneChar == "-" && !oneDash) {
				oneDash = true
				continue
			}
			if((i>0 &&(oneChar<"0" || oneChar>"9"))) {
				return false
			}
		}
	
		return true
	}
	return true
}
//check if input is a positive or negative integer
function isLongInteger(inputVal){
	var compareSign = false
	var oneDash = false
	if (inputVal.length > 0){
			inputStr = inputVal.toString()		
			for(var i=0; i< inputStr.length; i++){
			var oneChar = inputStr.charAt(i)
				if(i == 0 && oneChar == "-"){
					continue
					}
				if(i == 0 && oneChar == "="){
					continue
					}
				if(i == 0 && ((oneChar == "<") || (oneChar == ">"))){
					compareSign = true
					continue
					}
				if(i == 1 && ((oneChar == "=") && (compareSign == true))){
					continue
					}
				if((oneChar == "-") && (oneDash != true)){
					oneDash = true
					continue
					}
				if(oneChar < "0" || oneChar > "9") {
					return false
				}
		}	
		return true
	}
	return true	
}

//check if input is a positive or negative integer
function isPosLongInteger(inputVal){
	var compSign = false
	var oneDash = false
	if (inputVal.length > 0){
			inputStr = inputVal.toString()		
			for(var i=0; i< inputStr.length; i++){
			var oneChar = inputStr.charAt(i)
				if(i == 0 && oneChar == "-"){
					continue
					}
				if(i == 0 && oneChar == "="){
					continue
					}
				if(i == 0 && ((oneChar == "<") || (oneChar == ">"))){
					compareSign = true
					continue
					}
				if(i == 1 && ((oneChar == "=") && (compareSign == true))){
					continue
					}
				if((oneChar == "-") && (oneDash != true)){
					oneDash = true
					continue
					}
				if(oneChar < "0" || oneChar > "9") {
					return false
				}
		}	
		return true
	}
	return true	
}

function PopUpDateOLD(strControl,fullsourcepath)
{
	var browserNetscape = "False"
	var strURL = fullsourcepath + "?CTRL=" + strControl;
	var strCurDate = document.forms["form1"].elements[strControl].value;
	if ((strCurDate != null) && (strCurDate != "undefined")) {
		if (strCurDate.length > 0){
			strURL += "&INIT=" + strCurDate;
		}
	}
	var windowDatePicker = ""
	if (windowDatePicker.name == null){
		if (browserNetscape.toLowerCase() == "true"){
			windowDatePicker = window.open(strURL,"dp","toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0," + "width=190,height=200");
			windowDatePicker.focus();
			}
		else{
			windowDatePicker = window.open(strURL,"dp","toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0," + "width=190,height=200,left=" + (window.event.screenX - 190) + ",top=" + (window.event.screenY + 20));
			windowDatePicker.focus();
		}
	}
	else{
		windowDatePicker.focus()
	}
	return false
}	

