using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

using Csla;
using Csla.Core;
using Csla.Validation;

namespace CambridgeSoft.COE.Framework.Common.Validation
{
    /// <summary>
    /// Add validation support for business list objects. Inherit this class if your business lists
    /// or collections need validation functionality (ex. for comparing list items to eachother).
    /// </summary>
    /// <typeparam name="T">Type of the business object being defined.</typeparam>
    /// <typeparam name="C">Type of the child objects contained in the list.</typeparam>
    /// <remarks>
    /// <example>
    /// Your derived list class generally override one (or both) of the following two methods:
    /// <code>
    /// protected override void AddInstanceBusinessRules()
    /// {
    ///     //Add business rules for the current list object.
    ///     //ValidationRules.AddInstanceRule(Method, "Method");
    /// }
    /// protected override void AddBusinessRules()
    /// {
    ///     //Add shared rules for alll instances of your list class
    ///     //ValidationRules.AddRule(ChildCount,"ChildCount");
    /// }
    /// </code>
    /// </example>
    /// <para>Also,you may find the <see cref="IsValid"/> and <see cref="BrokenRulesCollection"/> properties are useful.</para>
    /// </remarks>
    [Serializable]
    public abstract class BusinessListWithValidation<T, C> : BusinessListBase<T, C>, IReportBrokenRules
        where T : BusinessListWithValidation<T, C>
        where C : Csla.Core.IEditableBusinessObject
    {
        #region Properties

        private ValidationRules _validationRules;
        private static List<Type> _sharedRulesRegisteredTypes = new List<Type>();

        /// <summary>
        /// Provides access to the broken rules functionality.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is used within your business list logic so you can easily call
        /// the AddRule() methods to add rules. The syntax is the same as the ValidationRules of BusinessBase.
        /// However, the list object does not have a property corresponding to a rule, so you need to choose a 
        /// "property name" for a rule.
        /// Typically, you can use the name of the method that you want to represent the rule.
        /// </para>
        /// <para>
        /// In this base class it also checks rules on ListChanged event and checks the validity of the list.
        /// </para>
        /// </remarks>
        protected ValidationRules ValidationRules
        {
            get
            {
                if (_validationRules == null)
                {
                    //use reflection to create an instance
                    Type validationRulesType = typeof(ValidationRules);
                    Object[] parameters ={ this };
                    _validationRules = (ValidationRules)Activator.CreateInstance(validationRulesType, BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
                }
                return _validationRules;
            }
        }

        /// <summary>
        /// Maintain a list of types whose shared rules has been registered.
        /// </summary>
        private static List<Type> SharedRulesRegisteredTypes
        {
            get
            {
                return _sharedRulesRegisteredTypes;
            }
        }

        /// <summary>
        /// Provides access to the readonly collection of broken business rules
        /// for this list.
        /// </summary>
        /// <returns>A Csla.Validation.BrokenRulesCollection object.</returns>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual BrokenRulesCollection BrokenRulesCollection
        {
            get { return ValidationRules.GetBrokenRules(); }
        }

        /// <summary>
        /// Take into account the list rules
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return base.IsValid && BrokenRulesCollection.ErrorCount == 0;
            }
        }

        #endregion

        #region Methods for adding rules

        /// <summary>
        /// Add business rules
        /// </summary>
        protected BusinessListWithValidation()
        {
            this.AddInstanceBusinessRules();
            if (SharedRulesRegisteredTypes.Contains(this.GetType()) == false)
            {
                this.AddBusinessRules();
                //Mark this type as registered so that its shared rules will not be added again when create new instances.
                _sharedRulesRegisteredTypes.Add(this.GetType());
            }
        }

        /// <summary>
        /// Override this method to add shared (within type) business rules.
        /// </summary>
        protected virtual void AddBusinessRules()
        {}

        /// <summary>
        /// Override this method to add instance business rules
        /// </summary>
        protected virtual void AddInstanceBusinessRules()
        {}

        /// <summary>
        /// Reset the target of ValidationRules after deserialized.
        /// </summary>
        /// <param name="context"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            base.OnDeserialized(context);
            if (_validationRules != null)
            {
                //Because the SetTarget method on ValidationRules is not public, we use reflection to reset the target.
                Type validationRulesType = typeof(ValidationRules);
                Object[] parameters ={ this };
                MethodInfo setTarget = validationRulesType.GetMethod("SetTarget", BindingFlags.NonPublic | BindingFlags.Instance);
                setTarget.Invoke(_validationRules, parameters);
            }
        }

        #endregion

        #region Rules validating

        protected override void OnListChanged(System.ComponentModel.ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            ValidationRules.CheckRules();
        }

        #endregion

        #region Fetch broken rules

        /// <summary>
        /// A statement describing why the list is invalid
        /// </summary>
        public string BrokenRulesDescription
        {
            get
            {
                StringBuilder errorMessage = new StringBuilder();
                List<BrokenRuleDescription> brokenRules = new List<BrokenRuleDescription>();
                GetBrokenRulesDescription(brokenRules);
                foreach (BrokenRuleDescription brokenRule in brokenRules)
                {
                    errorMessage.AppendLine(string.Join(System.Environment.NewLine, brokenRule.BrokenRulesMessages));
                }
                return errorMessage.ToString();
            }
        }

        /// <summary>
        /// Form a list of broken rules from the current object and each child object
        /// </summary>
        /// <param name="brokenRules">A BrokenRuleDescription List to be appended</param>
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            if (brokenRules == null) return;

            //Append broken rules of child object to the list
            int originalCount = brokenRules.Count;
            foreach (C item in this)
            {
                IReportBrokenRules child = item as IReportBrokenRules;
                if (child != null)
                    child.GetBrokenRulesDescription(brokenRules);
            }
            if (originalCount != brokenRules.Count) return;//Stop if there are invalid child objects

            //Add broken rules of current list
            if (BrokenRulesCollection.ErrorCount > 0)
            {
                brokenRules.Add(new BrokenRuleDescription(this, BrokenRulesCollection.ToArray()));
            }
        }

        #endregion
    }
}