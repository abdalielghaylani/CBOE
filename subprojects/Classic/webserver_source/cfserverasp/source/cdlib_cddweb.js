/////////////////////////////////////////////////////////////////////////////////////////////
//
// This is the Javascript wrapper for CDJS methods, used by Chrome and Firefox.
//
//
// This file contains the all following functions that can be used from a web page:
//
//   cd_getFormula(objName, selection)
//   cd_getAnalysis(objName, selection)
//   cd_getMolWeight(objName, selection)
//   cd_getExactMass(objName, selection)
//   cd_getData(objName, dataType)
//   cd_putData(objName, dataType, data)
//   cd_clear(objName)
//
//
//
// All Rights Reserved.
/////////////////////////////////////////////////////////////////////////////////////////////




/////////////////////////////////////////////////////////////////////////////////////////////
// Internal function to test if the structure is blank

function cd_isBlankStructure(objName, selection) {
    return cd_getSpecificObject(objName).isBlankStructure();
}


/////////////////////////////////////////////////////////////////////////////////////////////
// Clear all drawings in the Plugin named *objName*

function cd_clear(objName) {
    return cd_getSpecificObject(objName).clear();
}


/////////////////////////////////////////////////////////////////////////////////////////////
// Return the *Formula* of selected/all structures in the Plugin named *objName*

function cd_getFormula(objName, selection) {
    // TODO: implement method
    return "";
}


/////////////////////////////////////////////////////////////////////////////////////////////
// Return the *Analysis* of selected/all structures in the Plugin named *objName*

function cd_getAnalysis(objName, selection) {
    // TODO: implement method
    return "";
}


/////////////////////////////////////////////////////////////////////////////////////////////
// Return the *Molecular Weight* of selected/all structures in the Plugin named *objName*

function cd_getMolWeight(objName, callback) {
    var cddInstance = cd_getSpecificObject(objName);
    var cdXML = cddInstance.getCDXML();
    cddInstance.getMolecularWeight(cdXML, "chemical/x-cdx", function (weight, error) {
        if (!error) {
            callback(weight);
        } else {
            // TODO: deal with error
        }
    });
}


/////////////////////////////////////////////////////////////////////////////////////////////
// Return the *Exact Mass* of selected/all structures in the Plugin named *objName*

function cd_getExactMass(objName, selection) {
    // TODO: implement method
    return "";
}


/////////////////////////////////////////////////////////////////////////////////////////////
// Return version of Plugin

function cd_getVersion(objName) {
    return cd_getSpecificObject(objName).getVersion();
}


/////////////////////////////////////////////////////////////////////////////////////////////
// Return the Coding String of *dataType* type from selected/all drawings in the Plugin named *objName*

function cd_getData(objName, mimetype, checkMW) {
    if (checkMW == null)
        checkMW = true;

    var r = "";

    if (!checkMW || !cd_isBlankStructure(objName, 0))
        r = cd_getSpecificObject(objName).getB64CDX();

    return r;
}


/////////////////////////////////////////////////////////////////////////////////////////////
// Set the Plugin named *objName* to *data* 

function cd_putData(objName, mimetype, data) {
    // TODO: implement method
}
