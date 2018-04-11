function getObj(name)
{
    if (document.getElementById) {
	    if(document.getElementById(name)) {
            this.obj = document.getElementById(name);
            this.style = document.getElementById(name).style;
        }
	    else {
		    this.obj = null;
		    this.style = null;
	    }
    }
    else if (document.all) {
       this.obj = document.all[name];
       this.style = document.all[name].style;
    }
    else if (document.layers) {
        if (document.layers[name]) {
            this.obj = document.layers[name];
            this.style = document.layers[name];
        }
        else {
            this.obj = document.layers.testP.layers[name];
            this.style = document.layers.testP.layers[name];
        }
    }
}

function fitToContents(elementDiv){
	if(elementDiv != null) {
	    elementDiv.style.height = '0px';
	    elementDiv.style.width = '0px';
    	
    	var tables = elementDiv.getElementsByTagName('table');
        for(i = 0; i < tables.length; i++) {
            tables[i].parentNode.style.height = '0px';
            tables[i].parentNode.parentNode.style.height = '0px';
	        fitContainer(tables[i]);
	        fitContainer(tables[i].parentNode);
        }
        
	    var divList = elementDiv.getElementsByTagName('div');
	    for(i = 0; i < divList.length; i++) {
	        fitContainer(divList[i]);
        }
	}
}

function fitContainer(elementDiv) {
    var childRight = parseInt(elementDiv.offsetLeft) + parseInt(elementDiv.offsetWidth) + 'px';
    var childBottom = parseInt(elementDiv.offsetTop) + parseInt(elementDiv.offsetHeight) + 'px';
	//var childBottom = parseInt(elementDiv.offsetTop) - parseInt(elementDiv.parentNode.offsetTop) + parseInt(elementDiv.offsetHeight) - parseInt(elementDiv.parentNode.offsetHeight) + 'px';

    if((parseInt(childRight) > 0) && (elementDiv.parentNode.style.width == '' || parseInt(elementDiv.parentNode.style.width) < parseInt(childRight)))
        elementDiv.parentNode.style.width = childRight;
    if((parseInt(childBottom) > 0) && (elementDiv.parentNode.style.height == '' || parseInt(elementDiv.parentNode.style.height) < parseInt(childBottom)))
        elementDiv.parentNode.style.height = childBottom;
}