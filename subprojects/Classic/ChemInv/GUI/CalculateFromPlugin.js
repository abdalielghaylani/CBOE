

// Calculates formula from plugin if formula field is present
function GetFormula() {
    for (var i = 0; i < cd_objectArray.length ; i++) {
        var formula = cd_getFormula(cd_objectArray[i]);
        elm = eval("document.all.FORMULA" + i)
        if (elm) {
            if (formula.length > 15) {
                elm.innerHTML = formula.substring(0, 15);
                elm.title = formula;
            } else {
                elm.innerHTML = formula;
            }
        }
    }
}

// Calculates molW from plugin if Mw field is present
function GetMolWeight() {
    var handler = function (weight) {
        elm = eval("document.all.MOLWEIGHT" + this.i)
        if (elm) {
            elm.innerHTML = round(weight + 0, 4);
        }
    }

    for (var i = 0; i < cd_objectArray.length ; i++) {
        if (cd_currentUsing == 4) {
            cd_getMolWeight(cd_objectArray[i], handler.bind({ i: i }));
        } else {
            var molW = cd_getMolWeight(cd_objectArray[i]);
             elm = eval("document.all.MOLWEIGHT" + i)
             if (elm) {
                elm.innerHTML = round(molW + 0, 4);
             }
        }
    }
}

function round(number, x) {
    // rounds number to X decimal places, defaults to 2
    x = (!x ? 2 : x);
    return Math.round(number * Math.pow(10, x)) / Math.pow(10, x);
}

//-->