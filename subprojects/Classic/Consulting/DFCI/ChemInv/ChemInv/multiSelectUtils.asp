<SCRIPT LANGUAGE="javascript">

	function SelectMarked(){
				var reml = document.form1.removeList;
				var len = reml.value.length
				if (reml.value.substring(len -1 , len) == ","){
					reml.value = reml.value.substring(0, len - 1);
				}
				//alert(reml.value);
				document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>";
				elm = document.form1.selectChckBox;
				<%if bDisableChkBoxes then%>
					//MCD: added check for case where there's only one element
					if (elm.length){
						for (i=0; i < elm.length ; i++){
							elm[i].disabled = false;
						}
					}
					else{
						elm.disabled = false;
					}
				<%end if%>
				document.form1.submit();
				<%if bDisableChkBoxes then%>
					//MCD: added check for case where there's only one element
					if (elm.length){
						for (i=0; i < elm.length ; i++){
							elm[i].disabled = true;
						}
					}
					else{
						elm.disabled = true;
					}
				<%end if%>
				reml.value =""; 
			}
			
			function CheckAll(bCheck){
				var cbObj = document.form1.selectChckBox;
				
				if (cbObj.length){
					for (i=0; i< cbObj.length; i++){
						if (cbObj[i].checked ^ bCheck){
							cbObj[i].checked = bCheck;
							if (!bCheck) Removals(cbObj[i].value, false);
						}
					}
				}
				else{
					if (cbObj.checked ^ bCheck){
							cbObj.checked = bCheck;
							if (!bCheck) Removals(cbObj.value, false);
						}
				}
				SelectMarked();	
			}
		
		
		
			function Removals(id, bRemove){
				var idc = id + ",";
				var reml = document.form1.removeList;
			
				if (bRemove){
					if (reml.value.indexOf(idc) >=0){
						reml.value = reml.value.replace(idc,"");
					}
				}
				else{
					if (reml.value.indexOf(idc) ==  -1){
						reml.value += id + ",";
					}
				}
				//alert(reml.value);				
			}
			function HappySound(){
				snd1.src='/cheminv/sounds/yes1.wav';
				return true;
			}
	
			function SadSound(){
				snd1.src='/cheminv/sounds/no1.wav';
			}
			
			function SadSound2(){
				snd1.src='/cheminv/sounds/no2.wav';
			}
			
</SCRIPT>