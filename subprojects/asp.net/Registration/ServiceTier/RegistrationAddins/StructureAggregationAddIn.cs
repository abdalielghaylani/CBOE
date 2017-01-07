using System;
using System.Text;
using System.Xml;

using CambridgeSoft.COE.Registration.Services.AddIns;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.COELoggingService;
using ChemDrawControl14;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    [Serializable]
    public class StructureAggregationAddIn : IAddIn
    {
        #region Events Handlers
        bool isUpdate = false;
        bool isTemp = false;

        public void OnInsertingHandler(object sender, EventArgs args)
        {
            _registryRecord = (IRegistryRecord)sender;
            if (_registryRecord.IsDirty)
                isTemp = _registryRecord.IsTemporal;
                _registryRecord.StructureAggregation = AggregateStructure();
        }

        public void OnUpdatingHandler(object sender, EventArgs args)
        {
            _registryRecord = (IRegistryRecord)sender;
            if (!_registryRecord.IsNew && _registryRecord.IsDirty)
                isUpdate = true;
                isTemp = _registryRecord.IsTemporal;
                _registryRecord.StructureAggregation = AggregateStructure();
        }
        public void OnSavingHandler(object sender, EventArgs args)
        {
            _registryRecord = (IRegistryRecord)sender;
            if (_registryRecord.IsDirty)
                isTemp = _registryRecord.IsTemporal;
                _registryRecord.StructureAggregation = AggregateStructure();
        }

        #endregion

        #region Variables

        private int _numberOfColumns = 4;
        private object _base64Settings;
        private string _cacheKey;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("StructureAggregationAddIn");

        #endregion

        #region Methods

        /// <summary>
        /// Method to check if there is one than more component, order the compoundList in a grid.
        /// </summary>
        /// <returns>The base64 value of the ordered ChemDrawCtrl</returns>
        private string AggregateStructure()
        {
            string retVal = String.Empty;
            ChemDrawCtl finalDraw = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            try
            {
                finalDraw.Objects.Clear();

                List<Component> componentList = this.GetComponentList();

                if (componentList.Count > 1)
                {
                    string aggregatedStructure = string.Empty;

                    double numberOfPossibleIterations = (double)componentList.Count / (double)_numberOfColumns;
                    int numberOfIterations = (int)Math.Truncate(numberOfPossibleIterations);
                    if (numberOfPossibleIterations > Math.Truncate(numberOfPossibleIterations))
                        numberOfIterations += 1;

                    for (int i = 0; i < numberOfIterations; i++)
                    {
                        string[] componentsBase64s = new string[_numberOfColumns];
                        for (int j = 0; j < _numberOfColumns; j++)
                        {
                            componentsBase64s[j] = GetCurrentComponentStructureValue(componentList, i, j);
                        }
                        ChemDrawCtl currentCellDraw = new ChemDrawCtl();
                        currentCellDraw = this.PositioningRoughAlgorithm(componentsBase64s);
                        this.AddRow(finalDraw, currentCellDraw);
                    }
                    finalDraw.DataEncoded = true; //Necesary to get the correct base64 value.
                    retVal = finalDraw.get_Data("chemical/x-cdx").ToString();
                }
                else
                {
                    //If just one compound, no structure to aggregate.
                    retVal = GetCurrentComponentStructureValue(componentList, 0, 0);
                }
            }
            catch (Exception exception)
            {
                _coeLog.Log(exception.Message);
                throw exception;
            }
            finally
            {
                RegAddInsUtilities.ChemDrawReleaseMutex();
            }
            return retVal;
        }

        private List<Component> GetComponentList()
        {
            List<Component> result = new List<Component>();
            if (_registryRecord != null && _registryRecord.ComponentList != null)
            {
                foreach (Component currentComponent in _registryRecord.ComponentList)
                    if (!currentComponent.IsDeleted)
                        result.Add(currentComponent.Clone());
            }
            return result;
        }

        /// <summary>
        /// Check and return the base64 Value from a given Component indexes.
        /// </summary>
        /// <param name="rowNumber">Number of row to search</param>
        /// <param name="cellNumber">Number of cell to search</param>
        /// <returns>The base64 value of the given component</returns>
        private string GetCurrentComponentStructureValue(List<Component> componentList, int rowNumber, int cellNumber)
        {
            string value = string.Empty;
            int currentPosition = GetCurrrentPosition(rowNumber, cellNumber);

            if (currentPosition < componentList.Count)
            {
                Structure currentStructure = componentList[currentPosition].Compound.BaseFragment.Structure;

                //conditionally use the normalized structure for the aggregation
                if (currentStructure.UseNormalizedStructure && !string.IsNullOrEmpty(currentStructure.NormalizedStructure)){
                    if(isUpdate==true && (isTemp==true ||currentStructure.IsBeingRegistered)){
                       
                        value = currentStructure.NormalizedStructure;
                    }
                    else if(isUpdate == true && isTemp == false){
                        value = currentStructure.Value;

                    }else{
                        value = currentStructure.NormalizedStructure;
                    }
                }
                else{
                    
                        value = currentStructure.Value;
                }   
            }

            return value;
        }

        /// <summary>
        /// Calculate the index for the current cell - row.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="cellNumber"></param>
        /// <returns></returns>
        private int GetCurrrentPosition(int rowNumber, int cellNumber)
        {
            return (rowNumber * _numberOfColumns) + cellNumber;
        }

        /// <summary>
        /// Method to order the Components like a grid.
        /// </summary>
        /// <param name="base64s">The array of base64 values of the current row</param>
        /// <returns>A ordered control with the given components</returns>
        /// <remarks>This method just order the items horizontally (x) </remarks>
        private ChemDrawCtl PositioningRoughAlgorithm(string[] base64s)
        {
            ChemDrawCtl targetControl = new ChemDrawCtl();
            RegAddInsUtilities.ChemDrawWaitOnce();
            try
            {
                targetControl.Settings.ApplySettings("chemical/x-cdx", _base64Settings);
                double moveX = double.MinValue;
                double marginWidthX = 20.0;
                //Add the first element and move to the left + margin
                targetControl.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(base64s[0]));
                targetControl.Objects.Select();
                for (int i = 1; i < base64s.Length; i++)
                {
                    if (!string.IsNullOrEmpty(base64s[i]))
                    {
                        //This is temp ChemDrawCtl to know the control's properties (Width)
                        ChemDrawCtl currentControl = new ChemDrawCtl();
                        currentControl.Settings.ApplySettings("chemical/x-cdx", _base64Settings);
                        currentControl.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(base64s[i]));

                        if (currentControl.Objects.Width != double.MinValue)
                        {
                            targetControl.Objects.Select();
                            //Move according the previous selection
                            moveX = (targetControl.selection.Objects.Left + targetControl.selection.Objects.Width - currentControl.Objects.Left) + marginWidthX;
                            targetControl.selection.Objects.Move(-moveX, 0);
                        }
                        //Addd new control to target
                        targetControl.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(base64s[i]));
                    }
                }
            }
            catch (Exception exception)
            {
                _coeLog.Log(exception.Message);
                throw exception;
            }
            finally
            {
                RegAddInsUtilities.ChemDrawReleaseMutex();
            }
            return targetControl;
        }

        /// <summary>
        /// Method to ordered the given Row vertically
        /// </summary>
        /// <param name="final">The output ChemDraw</param>
        /// <param name="line">The current ChemDraw (latest row)</param>
        private void AddRow(ChemDrawCtl final, ChemDrawCtl line)
        {
            double marginWidthY = 15.0;
            final.Objects.Select();

            if (final.Objects.Top != 0 && final.Objects.Height != 0)
            {
                //Move up to the 0. Avoid figures overlapping.
                double moveY = line.Objects.Top - (final.Objects.Top + final.Objects.Height);
                final.Objects.Move(0, moveY - marginWidthY);
            }
            final.Settings.ApplySettings("chemical/x-cdx", _base64Settings);
            object obj = line.get_Data("chemical/x-cdx");
            final.Objects.set_Data("chemical/x-cdx", null, line.Objects.Width, line.Objects.Height, obj);
        }

        #endregion

        #region IAddIn Members

        private IRegistryRecord _registryRecord;

        public IRegistryRecord RegistryRecord
        {
            get
            {
                return _registryRecord;
            }
            set
            {
                _registryRecord = value;
            }
        }

        public void Initialize(string xmlConfiguration)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlConfiguration);
                XmlNode currentNode = document.SelectSingleNode("AddInConfiguration/NumberOfColumns");
                if (currentNode != null)
                    int.TryParse(currentNode.InnerText, out _numberOfColumns);
            }
            catch
            {
                this._numberOfColumns = 4; //Default value in case the xml failed or has not particular value for this setting.
            }

        }

        #endregion
    }
}
