using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Access;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Core;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// A service class with static methods for matching individual Compounds with single-compound
    /// registrations, or a set of Compounds against registered Mixtures in the repository.
    /// </summary>
    public class RegistrationMatcher
    {
        /// <summary>
        /// After determining if this is an individual Compound or a Mixture, returns a list of
        /// registered entity matches.
        /// </summary>
        /// <param name="targetObject">
        /// the object whose properties must be considered by the matching algorithm
        /// </param>
        /// <returns>
        /// an instance of IMatchResponse specific to the type of <paramref name="targetObject"/>
        /// </returns>
        public static IMatchResponse GetMatches(RegistryRecord targetObject)
        {
            if (targetObject.ComponentList.Count == 1)
                return GetMatchResponse(targetObject.ComponentList[0].Compound);
            else
                return GetMatches(targetObject.ComponentList);
        }

        /// <summary>
        /// Determines if a matched Mixture instance exists in the database.
        /// </summary>
        /// <param name="targetObject">
        /// the Mixture whose Compounds must be considered by the matching algorithm
        /// </param>
        /// <returns>
        /// an instance of IMatchResponse specific to the type of <paramref name="targetObject"/>
        /// </returns>
        public static IMatchResponse GetMatches(ComponentList targetObject)
        {
            List<int> compoundIdList = new List<int>();
            foreach (Component comp in targetObject)
            {
                compoundIdList.Add(comp.Compound.ID);
            }

            if (compoundIdList.Count == 1)
                return GetMatchResponse(targetObject[0].Compound);
            else
                return GetMatchResponse(compoundIdList);
        }

        /// <summary>
        /// Determines if a matched Mixture instance exists in the database, based on a series of
        /// Compound IDs.
        /// </summary>
        /// <param name="targetCompoundIds">
        /// the list of compound IDs that must be considered by the matching algorithm
        /// </param>
        /// <returns>
        /// an instance of IMatchResponse specific to the type of <paramref name="targetObject"/>
        /// </returns>
        public static IMatchResponse GetMatches(List<int> targetCompoundIds)
        {
            return RegistrationMatcher.GetMatchResponse(targetCompoundIds);
        }

        /// <summary>
        /// Determines if a matched Compound instance exists in the database.
        /// </summary>
        /// <param name="targetObject">
        /// the Compound whose properties must be considered by the matching algorithm
        /// </param>
        /// <returns>
        /// an instance of IMatchResponse specific to the type of <paramref name="targetObject"/>
        /// </returns>
        public static IMatchResponse GetMatches(Compound targetObject)
        {
            return GetMatchResponse(targetObject);
        }

        #region [Implementations]

        private static IMatchResponse GetMatchResponse(Compound target)
        {
            KeyValuePair<PreloadDupCheckMechanism, string> dupCheckMechanism;
            string valueToCompare = null;

            Structure structure = target.BaseFragment.Structure;
            if (structure.DrawingType == DrawingType.Chemical)
            {
                string paramsToApply = RegSvcUtilities.GetCartridgeStructureMatchSettings();
                dupCheckMechanism = new KeyValuePair<PreloadDupCheckMechanism, string>(
                    PreloadDupCheckMechanism.Structure, paramsToApply
                    );
                valueToCompare = target.BaseFragment.Structure.Value;
            }
            else
            {
                dupCheckMechanism = RegSvcUtilities.GetNonStructuralDuplicateCheckSettings();
                switch (dupCheckMechanism.Key)
                {
                    case PreloadDupCheckMechanism.StructureProperty:
                    case PreloadDupCheckMechanism.ComponentProperty:
                        {
                            //get the value for the associated property
                            PropertyList properties = null;
                            if (dupCheckMechanism.Key == PreloadDupCheckMechanism.ComponentProperty)
                                properties = target.PropertyList;
                            else
                                properties = target.BaseFragment.Structure.PropertyList;
                            valueToCompare = RegSvcUtilities.GetPropertyValue(properties, dupCheckMechanism.Value);
                            break;
                        }
                    case PreloadDupCheckMechanism.StructureIdentifier:
                    case PreloadDupCheckMechanism.ComponentIdentifier:
                        {
                            //get the first value for the associated identifier
                            IdentifierList identifiers = null;
                            if (dupCheckMechanism.Key == PreloadDupCheckMechanism.ComponentIdentifier)
                                identifiers = target.IdentifierList;
                            else
                                identifiers = target.BaseFragment.Structure.IdentifierList;
                            valueToCompare = RegSvcUtilities.GetIdentifierValue(identifiers, dupCheckMechanism.Value);
                            break;
                        }
                    default:
                        {
                            //do nothing!
                            break;
                        }
                }
            }

            // only perform the duplicate-checing if it is configured properly
            if (dupCheckMechanism.Key != PreloadDupCheckMechanism.None)
            {
                if (!string.IsNullOrEmpty(dupCheckMechanism.Value))
                {
                    if (!string.IsNullOrEmpty(valueToCompare))
                    {
                        string checkType = null;
                        switch (dupCheckMechanism.Key)
                        {
                            case PreloadDupCheckMechanism.Structure: checkType = "S"; break;
                            case PreloadDupCheckMechanism.ComponentProperty: checkType = "P"; break;
                            case PreloadDupCheckMechanism.ComponentIdentifier: checkType = "I"; break;
                            case PreloadDupCheckMechanism.StructureProperty: checkType = "SP"; break;
                            case PreloadDupCheckMechanism.StructureIdentifier: checkType = "SI"; break;
                        }

                        RegistrationOracleDAL dal = null;
                        DalUtils.GetRegistrationDAL(ref dal, Constants.SERVICENAME);

                        CompoundMatchesDTO dto = dal.GetCompoundMatches(checkType, dupCheckMechanism.Value, valueToCompare);

                        System.Diagnostics.Trace.WriteLine(dto.ToXml());

                        return new CompoundMatchResponse(dto);
                    }
                }
            }

            //TODO: have this use the appropriate information
            return new CompoundMatchResponse(new List<string>(), MatchMechanism.None);
        }

        private static IMatchResponse GetMatchResponse(List<int> target)
        {
            RegistrationOracleDAL dal = null;
            DalUtils.GetRegistrationDAL(ref dal, Constants.SERVICENAME);
            MixtureMatchResponse response = null;

            if (target.Count <= 1)
                throw new InvalidOperationException("The list of Compound IDs was not indicative of a mixture.");
            else
            {
                //MixtureMatchesDTO dto = dal.GetMixtureMatches(compoundIdList);
                MixtureMatchesDTO dto = dal.GetMixtureMatches(target);

                System.Diagnostics.Trace.WriteLine(dto.ToXml());

                //return new MixtureMatchResponse(dto);
                response = new MixtureMatchResponse(dto);
            }
            return response;
        }

        #endregion

    }
}
