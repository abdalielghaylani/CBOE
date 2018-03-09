using System;
using System.Reflection;
using System.Diagnostics;
using System.Resources;
using System.Data.Common;
using System.Configuration;
using System.Web.Configuration;

using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using Oracle.DataAccess.Client;
using Csla;

using CambridgeSoft.COE.Framework.Common.Exceptions;
using CambridgeSoft.COE.Framework.Properties;
using Csla.Validation;

namespace CambridgeSoft.COE.Framework.ExceptionHandling
{
    /// <summary>
    /// This class is responsible for dispatching exceptions of different type to their corresponding handlers
    /// </summary>
    public static class COEExceptionDispatcher
    {
        #region Constants for policy names

        public const string LOG_AND_THROWNEWEXCEPTION = "LogAndThrowNewException";
        public const string LOG_ONLY = "LogOnly";

        #endregion
        //Exception Policy types configured in web.config file.
        public enum Policy
        {
            LOG_AND_THROWNEWEXCEPTION,
            LOG_ONLY
        }

        /// <summary>
        /// Determines if a given exception is of type T, or its root exception is of type T.
        /// </summary>
        /// <typeparam name="T">The type of desired root exception</typeparam>
        /// <param name="exception">The exception to check</param>
        /// <returns></returns>
        private static bool RootExceptionIsType<T>(Exception exception)
            where T : Exception
        {
            return exception is T
                || exception.GetBaseException() is T;
        }

        /// <summary>
        /// Get a string describing 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GetMethodDescription(MethodBase method)
        {
            Object[] attributes = method.GetCustomAttributes(typeof(COEUserActionDescriptionAttribute), true);

            if (attributes.Length == 0)
                return string.Empty;

            Assembly assembly = method.DeclaringType.Assembly;
            //remove the file extension
            string resourceFileName = assembly.GetManifestResourceNames()[0];
            resourceFileName = resourceFileName.Remove(resourceFileName.LastIndexOf(".resources"));

            ResourceManager resourceManager = new ResourceManager(resourceFileName, assembly);
            string resourceKey = ((COEUserActionDescriptionAttribute)attributes[0]).Key;

            return resourceManager.GetString(resourceKey);
        }

        /// <summary>
        /// <para>When called by Handle***Exception methods of COEExceptionDispatcher,
        /// it fetch a string decribing what is done in the method (through Attribute) , which caught the exception and transfer to COEExceptionDispatcher.
        /// Then conbine this string with a template to construct the Message of the caught exception.</para>
        /// This method makes sense only invoked by Handle***Exceptioin methods.
        /// </summary>
        /// <returns>A string could be used as Message of the caught exception</returns>
        private static string BuildWrapperExceptionMessage()
        {
            MethodBase method = new StackFrame(2, false).GetMethod();//the method that catch the exception
            string methodDescription = GetMethodDescription(method);

            //If no Attributes,use common message. 
            //Usually this will not happen because,at least,we attach an Attribute to the method that call the HandleBLLException
            if (string.IsNullOrEmpty(methodDescription))
                methodDescription = Resources.BLLExceptionCommonReason;

            return string.Format(Resources.ExceptionMessageTemplate, DateTime.Now.ToString(), methodDescription);
        }

        /// <summary>
        /// All exceptions caught in DAL layer ared handled through this meethod.
        /// </summary>
        /// <param name="exception">The exception which is caught in DAL layer</param>
        /// <param name="context">ExceptionContext encapsulate some useful info</param>
        public static void HandleDALException(Exception exception, ExceptionContext context)
        {
            COEDALException dalException;
            DbCommand command;

            // If the caught exception is an OracleException, we wrap it with a COEDALException,
            // setting its IsDBException to true.
            if (exception is OracleException)
            {
                // Stop passing the OracleException as inner exception as it cannot be deserialized.
                dalException = new COEDALException(exception.Message, null, true);
            }
            else
            {
                dalException = new COEDALException(exception.Message, exception, false);
            }
            if (context != null)
            {
                dalException.SetExceptionContext(context);
            }

            ExceptionPolicy.HandleException(dalException, LOG_AND_THROWNEWEXCEPTION);
         }

        /// <summary>
        /// This OverLoaded method is written in order not to change the existing Exception Handling.  
        /// All exceptions caught in the entry point of the BLL layer are handled through this method.
         /// Here HandleException is exceuted based on the policy sent by BO caller.
        /// Most excepted policy is log only, where the exception is logged without throwing it
        /// </summary>
        /// <param name="exception">The exception caught in business layer</param>
        public static void HandleBLLException(Exception exception, Policy policy)
        {
            COEBusinessLayerException wrapperException = null;
            string wrapperExceptionMessage = BuildWrapperExceptionMessage();
            //It is added to maintain the existing functionality.
            if (RootExceptionIsType<ValidationException>(exception) ||
               RootExceptionIsType<EditAffectsOtherMixturesException>(exception))
            {
                //If the caught exception is ValidationException, just throw it to UI layer.
                //The caller is aware that it must handle this.
                throw exception;
            }
            wrapperException = new COEBusinessLayerException(wrapperExceptionMessage, exception);
            if (policy == Policy.LOG_ONLY)// It will cheeck the type policy sent by caller method
            {
                ExceptionPolicy.HandleException(wrapperException, COEExceptionDispatcher.LOG_ONLY);//
            }
            else
            {
                ExceptionPolicy.HandleException(wrapperException, COEExceptionDispatcher.LOG_AND_THROWNEWEXCEPTION);
                throw exception;
            }
            
        }
        /// <summary>
        /// All exceptions caught in the entry point of the BLL layer are handled through this method.
        /// Force fully throwing the exception, when it is not throw by HandleException call due to bad configuration in Web.config file
        /// </summary>
        /// <param name="exception">The exception caught in business layer</param>
        public static void HandleBLLException(Exception exception)
        {
            HandleBLLException(exception, Policy.LOG_AND_THROWNEWEXCEPTION);
          
        }

        /// <summary>
        /// All exceptions caught in the UI layer are handled through this method.
        /// </summary>
        /// <param name="exception">The exception caught in UI layer</param>
        public static void HandleUIException(Exception exception)
        {
            //Because most exception caught in UI layer is wraped into COEBusinessLayerException, and they have already handled and loged.
            //So, we caught the exception isn't COEBusinessLayerException then do the handle process and log.
            // The call to Server.Transfer method will raise a ThreadAbortException by .NET framework internally. But we don't 
            // want to log this kind of exception so we simply ignore them.
            if (!(exception is COEBusinessLayerException) && !(exception is System.Threading.ThreadAbortException))
            {
                ExceptionPolicy.HandleException(exception, LOG_ONLY);
            }
        }
        /// <summary>
        /// This method is will logs and throws the exception caught in DAL Layer 
        /// </summary>
        /// <param name="dispatchException"></param>//Excception caught in DAL 
        /// <param name="command"></param>Dbcommand used in DAL method.
        public static void DispatchDALException( Exception dispatchException,DbCommand command)
        {
            ExceptionContext exContext = new ExceptionContext();
            exContext.Command = command;
            COEExceptionDispatcher.HandleDALException(dispatchException, exContext);
        }
    }
}
