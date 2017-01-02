using System;


namespace CambridgeSoft.COE.Framework.GUIShell
{
	public interface ICOENavigationPanelControl
	{
		#region Properties

		string ID
		{ get; set;}

		#endregion

		#region Events

		event EventHandler<COENavigationPanelControlEventArgs> CommandRaised;

		#endregion

		#region Methods
		//void DataBind(RegistryRecord multiCompoundRegistryRecord);
		//void DataBind(RegistryRecord registryRecord, string nodeTextToDisplayAsSelected);
		//void DataBind(CompoundList compoundList);
		//void DataBind(BatchList batchList);
		#endregion
	}

	/// <summary>
	/// This is a Custom EventArgs to handle our custom information 
	/// about the control wich implement ICoeNavigationPanelControl
	/// </summary>
	[Serializable]
	public class COENavigationPanelControlEventArgs : EventArgs
	{
		#region Variables

		private string _controlID = String.Empty;
		private string _eventType = String.Empty;
        private string[] _customInfo = new string[0];

		#endregion

		#region Methods

		public COENavigationPanelControlEventArgs()
		{

		}

		public COENavigationPanelControlEventArgs(string controlID, string eventType)
		{
			_controlID = controlID;
			_eventType = eventType;
		}

        public COENavigationPanelControlEventArgs(string controlID, string eventType, string[] customInfo)
        {
            _controlID = controlID;
            _eventType = eventType;
            _customInfo = customInfo;
        }
		#endregion

		#region Properties

		public string ControlId
		{
			get { return _controlID; }
		}

		public string EventType
		{
			get { return _eventType; }
		}

        public string[] CustomInfo
        {
            get { return _customInfo; }
        }

		#endregion
	}
}

