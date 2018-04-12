	
   
	// Calculates formula from plugin if formula field is present
	function GetFormula(){
		var Formula 
		for (var i=0; i< cd_objectArray.length ; i++){
			Formula = cd_getFormula(cd_objectArray[i]);
			elm = eval("document.all.FORMULA" + i)
			if (elm){
				if (Formula.length > 15) {
					elm.innerHTML = Formula.substring(0,15);
					elm.title = Formula;
				}
				else{
					elm.innerHTML = Formula;
				}
			}
		}
	}
	
	// Calculates molW from plugin if Mw field is present
	function GetMolWeight(){
		var MolW 
		for (var i=0; i< cd_objectArray.length ; i++){
			MolW = cd_getMolWeight(cd_objectArray[i]);
			elm = eval("document.all.MOLWEIGHT" + i)
			if (elm){
				elm.innerHTML = round(MolW + 0,4);
			}
		}
	}
	
	
	function round(number,X) {
		// rounds number to X decimal places, defaults to 2
	    X = (!X ? 2 : X);
	    return Math.round(number*Math.pow(10,X))/Math.pow(10,X);
	}
	
//-->