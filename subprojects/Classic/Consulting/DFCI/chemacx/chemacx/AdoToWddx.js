
<script language="JavaScript" runat="server">
  function SerializeFromSql(SQL, Connect) {
    var TempRS = new ActiveXObject("ADODB.Recordset");
    TempRS.Open(SQL, Connect);
    
    return SerializeADORecordset(TempRS);
  }

  function SqlToWddxRS(SQL, Connect) {
    var TempRS = new ActiveXObject("ADODB.Recordset");
    TempRS.Open(SQL, Connect);

    return AdoToWddxRS(TempRS)    
  }


	////////////////////////////////////////////////  
	// Function to "convert" a ADO-fetched recordset
	// to a WDDX-style recordset object w/ same data
	// The SerializeADORecordset function uses this.
	////////////////////////////////////////////////  
	function AdoToWddxRS(AspRS) {
		// Create empty WDDX-style recordset 
		// Removed Server.CreateObject to allow this to work outside of ASP
		var WddxRS = new ActiveXObject("WDDX.Recordset.1");  

		// Add the column names from ADO recordset  
		for (ColNum=0; ColNum< AspRS.Fields.Count; ColNum++) {
			WddxRS.addColumn(AspRS.Fields(ColNum).Name);
		};

		// Make sure the ADO recordset is at first row
		// We'll use RowNum to refer to current row below
		if (!AspRS.BOF) AspRS.MoveFirst();   
		var RowNum = 0;
		
		// For each row of the ADO recordset, add a row to the 
		// WDDX recordset, then copy all the data from each column 
		// in ADO recordset. ColName is column being copied.
		while (!AspRS.EOF) {
			WddxRS.addRows(1);
			RowNum++;

			for (var ColNum=0; ColNum< AspRS.Fields.Count; ColNum++) {
				ColName = AspRS.Fields(ColNum).Name;
				WddxRS.setField(RowNum, ColName, AspRS(ColName).Value);
			};

			AspRS.moveNext();
		}
    
    //AspRS.MoveFirst(); 

		// Return the finished Wddx-style recordset to caller    
		return WddxRS;
	};

	////////////////////////////////////////////////  
	// Function to serialize a ADO-style recordset
	// Returns the serialized WDDX packet
	////////////////////////////////////////////////  
	function SerializeADORecordset(AdoRS) {
		// Convert the ADO Recordset to WDDX-style recordset
		var MyWddxRS = AdoToWddxRS(AdoRS);
	
		// Create instance of Allaire's Serializer, and use it to
		// serialize new recordset. Return serialized packet to caller.
		var MySer = new ActiveXObject("WDDX.Serializer.1");
		return MySer.serialize(MyWddxRS);
		MySer = null;
	}

	function WddxToAdoRS(WddxRS) {
		var NewRS, i, j;
	
		// Constant from the MS Data Access SDK
		var adBSTR = 8;
		
		// Create a new ADO recordset
		// Removed Server.CreateObject to allow this to work outside of ASP
		var NewRS = new ActiveXObject("ADODB.Recordset");
		
		// Get the column names from the WDDX recordset
		var colNames = new VBArray(WddxRS.getColumnNames());
		colNames = colNames.toArray();
		
		// Add each column name to the ADO recordset
		for (i=0; i < colNames.length; i++) {
			NewRS.Fields.Append(colNames[i], adBSTR);
		}

		// Open the ADO recordset
		NewRS.Open();
		
		// For each row in the WDDX recordset...
		for (i=1; i <= WddxRS.getRowCount(); i++) {
		
			// Establish a new array to hold the values from the WDDX recordset
			arValues = new Array(WddxRS.getColumnCount()-1);

			// For each column in the WDDX recordset, copy the values from the
			// current row of the WDDX recordset into the array of values
			for (j=0; j < colNames.length; j++) {
				arValues[j] = WddxRS.getField(i, colNames[j]);
			} 

			// Add the array of values as a fresh row in the ADO recordset
			NewRS.AddNew(colNames, arValues);
		}

		// Move the ADO recordset cursor to the first row
		NewRS.MoveFirst();
		
		// Pass the ADO recordset back to calling process
		return NewRS;
	}
	
	
	function DeserializeToAdoRS(WddxPacket) {
		var MyDeser = new ActiveXObject("WDDX.Deserializer.1");
		var OurWddxRS = MyDeser.deserialize(WddxPacket);
		MyDeser = null;
		
		return WddxToAdoRS(OurWddxRS);
		OurWddxRS = null;   
	}
  
  
function FetchToAdoRS(httpComponent, url, postData, username, password, proxyServer) {  

  var Utils = new ActiveXObject("WDDX.PacketUtils");
  var Packet = Utils.fetchPacketFromWeb(httpComponent, url, postData, username, password, proxyServer);
  var Obj;
  
  if (Packet == '')
    Obj = null;
  else
    Obj = DeserializeToAdoRS(Packet);

  return Obj;
}  
  
  
function AdoRSToJavaScript(AdoRS, topLevelVariable) {
  var WddxRS = AdoToWddxRS(AdoRS);

  var Conv = new ActiveXObject("WDDX.JSConverter.1");
  var JSCode = Conv.convertData(WddxRS, topLevelVariable);
  Conv = null;

  return JSCode;
}  
</script>

