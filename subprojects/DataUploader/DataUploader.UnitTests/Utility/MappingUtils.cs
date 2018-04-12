using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core.DataMapping;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.Utility
{
    /// <summary>
    /// Provides helper methods to quickly generate Mappings.Mapping objects from the reuisite values.
    /// </summary>
    /// <remarks>Intended (initially) to be used internally for </remarks>
    public class MappingUtils
    {
        private const string BATCH_BY_INDEX = "this.BatchList[{0}]";
        private const string COMPOUND_BY_INDEX = "this.ComponentList[{0}].Compound";
        private const string STRUCTURE_BY_INDEX = COMPOUND_BY_INDEX + "." + "BaseFragment.Structure.Value";
        private const string MIXTURE_ROOT = "this";

        private const string PROPERTY_MEMBER_PATH = "{0}.PropertyList['{1}']";
        private const string IDENTIFIER_MEMBER_NAME = "AddIdentifier";
        private const string DEFAULT_STRUCTURE_MEMBER_NAME = "AssignDefaultStructureById";

        private const string STRING_TYPE = "string";
        private const string INTEGER_TYPE = "int";
        private const string FLOAT_TYPE = "float";

        /// <summary>
        /// Provided strictly as developer guidance for using these public utility methods.
        /// </summary>
        private void Sample_Usage()
        {
            Mappings mySetOfMappings = new Mappings();
            Mappings.Mapping myMap = null;

            mySetOfMappings.DestinationRecordType = Mappings.DestinationRecordTypeEnum.RegistryRecord;
            mySetOfMappings.MappingCollection.Add(NewRegistryPropertyMapping("REG_COMMENTS", "reg_comment_field", null));
            mySetOfMappings.MappingCollection.Add(NewStructureMapping(0, "structure_field"));
            mySetOfMappings.MappingCollection.Add(NewDefaultStructureMapping(0, -2));
            mySetOfMappings.MappingCollection.Add(NewComponentPropertyMapping(0, "CMP_COMMENTS", "comp_comment_field", null));
            mySetOfMappings.MappingCollection.Add(NewComponentPropertyMapping(0, "PURIFICATION_METHOD", "how_purified", "Purification"));
            mySetOfMappings.MappingCollection.Add(NewCompoundIdentifierMapping(0, "Synonym", "chemical_name"));

            //As necessary, you can use the fully-expanded public methods
            {
                myMap = CreateMappingWithMemberInfo("this", "a simple mapping", Mappings.MemberTypeEnum.Property, null, Mappings.TypeEnum.Instance);
                Mappings.Arg myArg = CreateMemberInfoArgument(0, Mappings.InputEnum.Derived, null, "string", "field_to_recolve");
                Mappings.Resolver myArgResolver = CreateArgumentResolver(@"c:\myfile.txt", Mappings.DelimiterEnum.Tab, "ext_val", "int_val");

                myMap.MemberInformation.Args.Add(myArg);
                myArg.Resolver = myArgResolver;
                mySetOfMappings.MappingCollection.Add(myMap);
            }
        }

        /// <summary>
        /// Generates a Mapping.Resolver from its required properties.
        /// </summary>
        /// <param name="filePath">the path to the file used as the look-up table</param>
        /// <param name="delimiter">the nature of the look-up table's column-separators</param>
        /// <param name="externalValueColumn">the column name containing the data-point to match from the data-file</param>
        /// <param name="internalValueColumn">the column name containing the datato substitute for the matched data-point</param>
        /// <returns>a new Mappings.Resolver instance</returns>
        public static Mappings.Resolver CreateArgumentResolver(
            string filePath
            , Mappings.DelimiterEnum delimiter
            , string externalValueColumn
            , string internalValueColumn
            )
        {
            Mappings.Resolver res = new Mappings.Resolver();
            res.Delimiter = delimiter;
            res.ExternalValueColumn = externalValueColumn;
            res.InternalValueColumn = internalValueColumn;
            res.File = filePath;

            return res;
        }

        /// <summary>
        /// Creates a single Mapping.Argument that can be added to the MemberInformation.Args collection.
        /// </summary>
        /// <param name="index">the new argument's index (0 for property-maps)</param>
        /// <param name="input">the nature of the source of the data (it may be a constant or it may be derived from the data-file)</param>
        /// <param name="pickListCode">if the value requires picklist-validation, the name of ID of the picklist to use</param>
        /// <param name="type">
        /// the data-type the input value represents; especially important for a method mbinding for proper
        /// relfection-based invocation
        /// </param>
        /// <param name="value">for a constant, the actual value to apply; otherwise, the field name from which the value will be derived/obtained</param>
        /// <returns>a new Mappings.Arg instance</returns>
        public static Mappings.Arg CreateMemberInfoArgument(
            int index
            , Mappings.InputEnum input
            , string pickListCode
            , string type
            , string value
            )
        {
            Mappings.Arg arg = new Mappings.Arg();
            arg.Index = index;
            arg.Input = input;
            arg.PickListCode = pickListCode;
            arg.Type = type;
            arg.Value = value;

            return arg;
        }

        /// <summary>
        /// Generates a Mappings.Map object, constructured with a Mappings.MemberInformation instance
        /// derived from the arguments provided.
        /// </summary>
        /// <param name="bindingPath">
        /// for a property binding, the path to the property itself; for a method binding, the path to
        /// the object housing the method
        /// </param>
        /// <param name="memberInfoDescription">a description of the member</param>
        /// <param name="memberInfoType">describes the nature of the binding as either to a property or a method</param>
        /// <param name="memberInfoName">
        /// for method bindings, the name of the method itself; invocation arguments can be provided via a separate method
        /// </param>
        /// <param name="memberInfoInvocationType">
        /// describes whether the property or method is a static member or an instance member
        /// </param>
        /// <returns>a new Mappings.Map instance</returns>
        public static Mappings.Mapping CreateMappingWithMemberInfo(
            string bindingPath
            , string memberInfoDescription
            , Mappings.MemberTypeEnum memberInfoType
            , string memberInfoName
            , Mappings.TypeEnum memberInfoInvocationType
            )
        {
            Mappings.Mapping map = new Mappings.Mapping();
            map.Enabled = true;
            map.ObjectBindingPath = bindingPath;

            Mappings.MemberInformation minfo = new Mappings.MemberInformation();
            minfo.Description = memberInfoDescription;
            minfo.MemberType = memberInfoType;
            minfo.Name = memberInfoName;
            minfo.Type = memberInfoInvocationType;
            map.MemberInformation = minfo;

            return map;
        }

        /// <summary>
        /// Generates a mapping for a new Batch-level PropertyList Property
        /// </summary>
        /// <param name="batchIndex">the batch's index in the list (typically zero)</param>
        /// <param name="targetPropertyName">the custom property name</param>
        /// <param name="sourceFieldName">the field from which the data will be derived</param>
        /// <param name="pickListCode">if the value requires picklist-validation, the name of ID of the picklist to use</param>
        /// <returns>a new Mappings.Mapping instance specific for a batch custom property</returns>
        public static Mappings.Mapping NewBatchPropertyMapping(
            int batchIndex
            , string targetPropertyName
            , string sourceFieldName
            , string pickListCode
            )
        {
            string bindingPathRoot = string.Format(BATCH_BY_INDEX, batchIndex);
            return CreateCustomPropertyMapping(bindingPathRoot, targetPropertyName, sourceFieldName, pickListCode);
        }

        /// <summary>
        /// Generates a mapping for a new Component-level PropertyList Property
        /// </summary>
        /// <param name="componentIndex">the component's index in the list (typically zero)</param>
        /// <param name="targetPropertyName">the custom property name</param>
        /// <param name="sourceFieldName">the field from which the data will be derived</param>
        /// <param name="pickListCode">if the value requires picklist-validation, the name of ID of the picklist to use</param>
        /// <returns>a new Mappings.Mapping instance specific for a component custom property</returns>
        public static Mappings.Mapping NewComponentPropertyMapping(
            int componentIndex
            , string targetPropertyName
            , string sourceFieldName
            , string pickListCode
            )
        {
            string bindingPathRoot = string.Format(COMPOUND_BY_INDEX, componentIndex);
            return CreateCustomPropertyMapping(bindingPathRoot, targetPropertyName, sourceFieldName, pickListCode);
        }

        /// <summary>
        /// Generates a mapping for a new Registry-level PropertyList Property
        /// </summary>
        /// <param name="targetPropertyName">the custom property name</param>
        /// <param name="sourceFieldName">the field from which the data will be derived</param>
        /// <param name="pickListCode">if the value requires picklist-validation, the name of ID of the picklist to use</param>
        /// <returns>a new Mappings.Mapping instance specific for a registry custom property</returns>
        public static Mappings.Mapping NewRegistryPropertyMapping(
            string targetPropertyName
            , string sourceFieldName
            , string pickListCode
            )
        {
            return CreateCustomPropertyMapping(MIXTURE_ROOT, targetPropertyName, sourceFieldName, pickListCode);
        }

        /// <summary>
        /// Generates a mapping for a new Batch-level Identifier
        /// </summary>
        /// <param name="batchIndex">the batch's index in the list (typically zero)</param>
        /// <param name="targetIdentifierName">the name of the batch-level identifier to use</param>
        /// <param name="sourceFieldName">the field from which the data will be derived</param>
        /// <returns>a new Mappings.Mapping instance specific for a batch-level identifier</returns>
        public static Mappings.Mapping NewBatchIdentifierMapping(
            int batchIndex
            , string targetIdentifierName
            , string sourceFieldName
            )
        {
            string bindingPathRoot = string.Format(BATCH_BY_INDEX, batchIndex);
            return CreateCustomIdentifierMapping(bindingPathRoot, targetIdentifierName, sourceFieldName);
        }

        /// <summary>
        /// Generates a mapping for a new Component-level Identifier
        /// </summary>
        /// <param name="componentIndex">the component's index in the list (typically zero)</param>
        /// <param name="targetIdentifierName">the name of the component-level identifier to use</param>
        /// <param name="sourceFieldName">the field from which the data will be derived</param>
        /// <returns>a new Mappings.Mapping instance specific for a component-level identifier</returns>
        public static Mappings.Mapping NewCompoundIdentifierMapping(
            int componentIndex
            , string targetIdentifierName
            , string sourceFieldName
            )
        {
            string bindingPathRoot = string.Format(COMPOUND_BY_INDEX, componentIndex);
            return CreateCustomIdentifierMapping(bindingPathRoot, targetIdentifierName, sourceFieldName);
        }

        /// <summary>
        /// Generates a mapping for a new Registry-level Identifier
        /// </summary>
        /// <param name="targetIdentifierName">the name of the registry-level identifier to use</param>
        /// <param name="sourceFieldName">the field from which the data will be derived</param>
        /// <returns>a new Mappings.Mapping instance specific for a registry-level identifier</returns>
        public static Mappings.Mapping NewRegistryIdentifierMapping(
            string targetIdentifierName
            , string sourceFieldName
            )
        {
            return CreateCustomIdentifierMapping(MIXTURE_ROOT, targetIdentifierName, sourceFieldName);
        }

        /// <summary>
        /// Generates a mapping for the Structure.Value property based on a specific data-field. 
        /// </summary>
        /// <param name="componentIndex">the component's index in the list (typically zero)</param>
        /// <param name="sourceFieldName">the field from which the structural representation will be derived</param>
        /// <returns>a new Mappings.Mapping instance specific for a chemical structure</returns>
        public static Mappings.Mapping NewStructureMapping(
            int componentIndex
            , string sourceFieldName
            )
        {
            Mappings.Mapping map = CreateMappingWithMemberInfo(
                string.Format(STRUCTURE_BY_INDEX, componentIndex)
                , string.Format("provide structure using '{0}'", sourceFieldName)
                , Mappings.MemberTypeEnum.Property
                , null
                , Mappings.TypeEnum.Instance
            );

            map.MemberInformation.Args.Add(
                CreateMemberInfoArgument(0, Mappings.InputEnum.Derived, null, STRING_TYPE, sourceFieldName)
            );

            return map;
        }

        /// <summary>
        /// Generates a mapping for the Structure.Value property based on a default structure identifier.
        /// </summary>
        /// <param name="componentIndex">the component's index in the list (typically zero)</param>
        /// <param name="defaultStructureId">the ID of an existing structure in the repository (typically -2)</param>
        /// <returns>a new Mappings.Mapping instance specific for an existing structure from the repository</returns>
        public static Mappings.Mapping NewDefaultStructureMapping(
            int componentIndex
            , int defaultStructureId
            )
        {
            Mappings.Mapping map = CreateMappingWithMemberInfo(
                string.Format(MIXTURE_ROOT, componentIndex)
                , string.Format("provide default structure '{0}'", defaultStructureId)
                , Mappings.MemberTypeEnum.Method
                , DEFAULT_STRUCTURE_MEMBER_NAME
                , Mappings.TypeEnum.Instance
            );

            map.MemberInformation.Args.Add(
                CreateMemberInfoArgument(0, Mappings.InputEnum.Constant, null, INTEGER_TYPE, componentIndex.ToString())
            );
            map.MemberInformation.Args.Add(
                CreateMemberInfoArgument(1, Mappings.InputEnum.Constant, null, INTEGER_TYPE, defaultStructureId.ToString())
            );

            return map;
        }

        #region > Internal Helpers <

        private static Mappings.Mapping CreateCustomPropertyMapping(
            string propertyListRootPath
            , string targetPropertyName
            , string sourceFieldName
            , string pickListCode
            )
        {
            Mappings.Mapping map = CreateMappingWithMemberInfo(
                string.Format(PROPERTY_MEMBER_PATH, propertyListRootPath, targetPropertyName)
                , string.Format("custom property '{0}'", targetPropertyName)
                , Mappings.MemberTypeEnum.Property
                , null
                , Mappings.TypeEnum.Instance
            );

            map.MemberInformation.Args.Add(
                CreateMemberInfoArgument(0, Mappings.InputEnum.Derived, pickListCode, STRING_TYPE, sourceFieldName)
            );

            return map;
        }

        private static Mappings.Mapping CreateCustomIdentifierMapping(
            string identifierListRootPath
            , string targetIdentifierName
            , string sourceFieldName
            )
        {
            Mappings.Mapping map = CreateMappingWithMemberInfo(
                string.Format("{0}", identifierListRootPath)
                , string.Format("custom identifier '{0}'", targetIdentifierName)
                , Mappings.MemberTypeEnum.Method
                , IDENTIFIER_MEMBER_NAME
                , Mappings.TypeEnum.Instance
            );

            map.MemberInformation.Args.Add(
                CreateMemberInfoArgument(0, Mappings.InputEnum.Constant, null, STRING_TYPE, targetIdentifierName)
            );
            map.MemberInformation.Args.Add(
                CreateMemberInfoArgument(1, Mappings.InputEnum.Derived, null, STRING_TYPE, sourceFieldName)
            );

            return map;
        }

        #endregion

    }
}