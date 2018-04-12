using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Properties;
using CommandLine;

using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace CambridgeSoft.COE.DataLoader
{
    static class Program
    {
        private static string _user = null;
        private static string _password = null;
        private static bool _authenticated = false;
        private static ConsoleKeyInfo cki;
        private static bool _hadValidArguments = false;
        private const string SHOW_STACKTRACE = "ShowStackTrace";
        private static ConsoleWriter _consoleWriter = new ConsoleWriter();
        private static int totalRecordToProcess = 0;
        private static int noOfRecordsPorcessed = 0;
        private static int recordsimported = 0;
        private static int percentageCompleted = 0;
        private static int invalidRecords = 0;
        private static int dupRecords = 0;
        private static int uniqueRecords = 0;
        private static int noActionRecords = 0;
        private static Stopwatch recordProcessStopWatch = new Stopwatch(); 
        /// <summary>
        /// The main entry point for the application.
        /// <para>
        /// If there are command-line arguments, these will be parsed and included in the state
        /// object which governs the session. If those arguments are sufficient to execute an
        /// import (to run unattended), then the import will be executed via the command-line.
        /// </para>
        /// </summary>
        //[STAThread]
        static void Main(string[] args)
        {
            try
            {
                string[] arguments = (string[])args.Clone();
                // use the WinForms GUI if there are no command-line arguments
                if (arguments.Length == 0)
                {
                    return;
                    //Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    //Application.Run(new CambridgeSoft.COE.DataLoader.COEDataLoader());
                }

                // use a fall-through system to derive all the required arguments

                // (0) help?
                if (Parser.ParseHelp(arguments))
                {
                    RemindUserOfValidArguments();
                    return;
                }

                // (1) determine if the user is trying to use a control file or not
                DecisionArgumentsInfo test = new DecisionArgumentsInfo();
                Parser.ParseArguments(args, test, new ErrorReporter(SilentErrorReporter));
                // extract the control file's contents
                if (test != null && !string.IsNullOrEmpty(test.CommandFilePath))
                {
                    arguments = ProcessControlFileArgs(args);
                }

                // (2) Continue processing:
                //   --> either a control file has been successfully parsed...
                //   --> ... OR the original command-line had multiple arguments
                IndividualArgumentsCommandInfo parsedArgs = new IndividualArgumentsCommandInfo();
                bool parseDone = Parser.ParseArguments(arguments, parsedArgs, new ErrorReporter(ConsoleErrorReporter));
                 _consoleWriter.RunSilent = parsedArgs.RunSilent;
                if (parseDone)
                {
                    Console.Clear();
                    JobParameters job = ProcessArguments(parsedArgs);
                    CambridgeSoft.COE.DataLoader.Core.Workflow.JobExecutor jobexe = new Core.Workflow.JobExecutor();
                    
                    if (job != null)
                    {   // instantiate _consoleWriter class with argument list so it can set it's self
                        //to output to the console or run silent
                        // perform the requested job
                        //jobexe.JobResult.ProcessComplete += ProcessCompleteConsole;
                        //jobexe.JobResult.BeforeRecordsChunkProcess += BeforeRecordsChunkProcessConsole;
                        //jobexe.JobResult.AfterRecordsChunkProcess += AfterRecordsChunkProcessConsole;
                        jobexe.JobResult.RecordsValidated += RecordsValidatedConsole;
                        jobexe.JobResult.RecordsDupChecked += RecordsDupCheckedConsole;
                        jobexe.JobResult.RecordsUniqueChecked += RecordsUniqueCheckedConsole;
                        jobexe.JobResult.RecordsTotal += TotalRecordsToProcessConsole;
                        jobexe.JobResult.RecordsImported += RecordsImportedConsole;
                        jobexe.JobResult.RecordsInvalid += RecordsInValidatedConsole;
                        jobexe.JobResult.RecordsNoAction += RecordsWithNoActionConsole;
                        jobexe.JobResult.RecordsValidateData += RecordsValidateDataProgressConsole;
                        jobexe.JobResult.ShowOutput += ShowOutputConsole;
                        recordProcessStopWatch.Start();
                        jobexe.DoUnattendedJob(job);
                       
                    }
                    do {System.Threading.Thread.Sleep(1000);} while (!jobexe.JobResult.JobComplete);
                    
                    Console.WriteLine("Press any key to finish...");
                    Console.ReadKey(true);
                }
                else
                {
                    _consoleWriter.WriteLine();

                    // remind the caller of the command-line options that are viable
                    if (!_hadValidArguments) RemindUserOfValidArguments();
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                if (System.Configuration.ConfigurationManager.AppSettings[SHOW_STACKTRACE].ToLower() == "true")
                    message += Environment.NewLine + e.StackTrace;
                 _consoleWriter.WriteLine(message);
            }
        }
        public static void ProcessCompleteConsole()
        {
            //_consoleWriter.WriteLine(string.Format("Please find log output at: '{0}'", Log.LogFilePath));
        }

        public static void BeforeRecordsChunkProcessConsole(object sender, EventArgs e)
        {
            IndexRange ir = (IndexRange)sender;
            // We are not displaying any chunk processing info here
        }

        public static void AfterRecordsChunkProcessConsole(object sender, EventArgs e)
        {
            IndexRange ir = (IndexRange)sender;
            string message = string.Format(Resources.AfterRecordsChunkProcessing, ir.RangeBegin + 1, ir.RangeEnd + 1);
            Console.WriteLine(message);
        }

        private static void RecordsValidatedConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            //_consoleWriter.WriteLine("Total Records Validated : "+iCount.ToString());
            noOfRecordsPorcessed = iCount;
        }

        private static void RecordsInValidatedConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            invalidRecords = iCount;
            //_consoleWriter.WriteLine("Total Records Validated : "+iCount.ToString());
        }

        private static void TotalRecordsToProcessConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            totalRecordToProcess = iCount;
            ShowProgress();
        }

        private static void RecordsWithNoActionConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            noActionRecords = iCount;
            noOfRecordsPorcessed = recordsimported + noActionRecords + invalidRecords;
            ShowProgress();
        }
        private static void RecordsDupCheckedConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            dupRecords = iCount;
            noOfRecordsPorcessed = dupRecords + uniqueRecords + invalidRecords;
            ShowProgress();
            //_consoleWriter.WriteLine("Total records duplicate checked : " + iCount.ToString());

        }

        private static void RecordsUniqueCheckedConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            uniqueRecords = iCount;
            noOfRecordsPorcessed = dupRecords + uniqueRecords + invalidRecords;
            ShowProgress();
            //_consoleWriter.WriteLine("Total records duplicate checked : " + iCount.ToString());

        }

        private static void RecordsImportedConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            recordsimported = iCount;
            noOfRecordsPorcessed = recordsimported +noActionRecords + invalidRecords;
            ShowProgress();
        }
        private static void RecordsValidateDataProgressConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            noOfRecordsPorcessed = iCount;
            ShowProgress();
        }
        private static void RecordsExtractedConsole(object sender, EventArgs e)
        {
            int iCount = (int)sender;
            //_consoleWriter.WriteLine("Total Records Extracted : " + iCount.ToString());

        }
        private static void ShowProgress()
        {
            if (noOfRecordsPorcessed > 0 && totalRecordToProcess > 0)
                percentageCompleted = (noOfRecordsPorcessed * 100 / totalRecordToProcess);
            Console.Clear();
            Console.SetCursorPosition(0, 4);
            Console.Write("Total records to be processed : " + totalRecordToProcess.ToString());
            Console.SetCursorPosition(0, 7);
            _consoleWriter.Write("Total records processed : " + noOfRecordsPorcessed.ToString());
            Console.SetCursorPosition(0, 10);
            _consoleWriter.Write("Percentage completed : " + percentageCompleted.ToString() +" %");
            Console.SetCursorPosition(0, 13);
            _consoleWriter.Write("Process rate : " + ((double) noOfRecordsPorcessed / ((double)recordProcessStopWatch.ElapsedMilliseconds / 1000)).ToString() );
        }
        private static void ShowOutputConsole(object sender, EventArgs e)
        {
            string msg= (string)sender;
            Console.WriteLine("");
            Console.WriteLine(msg);

        }
        /// <summary>
        /// If the raw command-line arguments contained a 'cmd' argument, we assume it contains
        /// all of the desired arguments within it except possibly for user name and password.
        /// </summary>
        /// <param name="args">the raw string-argument values</param>
        /// <returns>the arguments extracted from the control file</returns>
        private static string[] ProcessControlFileArgs(string[] args)
        {
            string[] arguments = (string[])args.Clone();

            // the user is passing the path for a control file
            ControlFileCommandInfo cfci = new ControlFileCommandInfo();
            if (Parser.ParseArguments(args, cfci, new ErrorReporter(ConsoleErrorReporter)))
            {
                arguments = UnpackControlFile(cfci.CommandFilePath);
                List<string> listArgs = new List<string>(arguments);

                if (!string.IsNullOrEmpty(cfci.UserName))
                    listArgs.Add(string.Format("/user:{0}", cfci.UserName));

                if (!string.IsNullOrEmpty(cfci.Password))
                    listArgs.Add(string.Format("/pwd:{0}", cfci.Password));
                
                arguments = listArgs.ToArray();
            }

            return arguments;
        }

        /// <summary>
        /// Open the file, split its contents, and forward the arguments
        /// </summary>
        /// <param name="controlFilePath">path to the control file, containing the required parameters</param>
        /// <returns>an array of string arguments</returns>
        private static string[] UnpackControlFile(string controlFilePath)
        {
            string[] arguments = new string[] { };
            if (System.IO.File.Exists(controlFilePath))
                arguments = System.IO.File.ReadAllLines(controlFilePath);
            return arguments;
        }

        /// <summary>
        /// Extracts individual command-line arguments, regardless of the source, and uses
        /// them to create a new JobParameters object, which is then returned.
        /// </summary>
        /// <param name="parsedArgs">
        /// arguments provided by either the raw command-line entry or the parsing of a control file
        /// </param>
        /// <returns>an initialized JobParameters object to allow further processing</returns>
        public static JobParameters ProcessArguments(IndividualArgumentsCommandInfo parsedArgs)
        {
            // Commandline arguments validation
            List<string> errorMessgeList = new List<string>();
            if (!JobCommandInfoConverter.ValidateCMDArguments(parsedArgs, out errorMessgeList))
            {
                _consoleWriter.Write("Some invalid arguments were found, please check your syntax!");
                _consoleWriter.Write("");
                int countHeader = 1;
                foreach (string errorMessage in errorMessgeList)
                {
                    _consoleWriter.Write(countHeader.ToString() + "." + errorMessage);
                    countHeader++;
                }
                return null;
            }
            // convert to internal object
            JobParameters job = JobCommandInfoConverter.ConvertToJobParameters(parsedArgs);

            bool requiresAuthentication = (
                job.TargetActionType == TargetActionType.FindDuplicates ||
                job.TargetActionType == TargetActionType.ImportRegDupAsCreateNew ||
                job.TargetActionType == TargetActionType.ImportRegDupAsNewBatch ||
                job.TargetActionType == TargetActionType.ImportRegDupNone ||
                job.TargetActionType == TargetActionType.ImportRegDupAsTemp ||
                job.TargetActionType == TargetActionType.ImportTemp ||
                job.TargetActionType == TargetActionType.ValidateData
                );

            if (requiresAuthentication)
            {
                if (!_authenticated)
                {
                    if (!string.IsNullOrEmpty(parsedArgs.UserName) && !string.IsNullOrEmpty(parsedArgs.Password))
                        AuthenticateUser(parsedArgs.UserName, parsedArgs.Password);
                    else
                        AuthenticateUser();

                    if (!_authenticated)
                    {
                        //NOTE: Clearing the console causes error when this app is run from withina another process
                        //Console.Clear();
                        _consoleWriter.Write("Invalid credentials provided.");
                        return null;
                    }
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CslaDataPortalUrl"]))
                    {
                        job.UserName = _user;
                        job.Password = _password;
                    }
                }
                if (!AuthorizeUser(job))
                {
                    _consoleWriter.Write("Insufficient privileges for requested operation.");
                    return null;
                }

            }

            _hadValidArguments = true;
            return job;
        }

        /// <summary>
        /// If the command-line parsers could not evaluate valid arguments, display '/help'
        /// information to the user.
        /// </summary>
        private static void RemindUserOfValidArguments()
        {
            _consoleWriter.WriteLine();
            _consoleWriter.Write("You can use a control file to encapsulate your settings...");
            System.Console.Write(Parser.ArgumentsUsage(typeof(ControlFileCommandInfo)));

            _consoleWriter.WriteLine();
            _consoleWriter.Write("You can provide the individual settings directly...");
            _consoleWriter.Write(Parser.ArgumentsUsage(typeof(IndividualArgumentsCommandInfo)));
        }

        /// <summary>
        /// Console reporter delegate for command-line error reporting.
        /// </summary>
        /// <param name="message">The error message from the third-party parser</param>
        private static void ConsoleErrorReporter(string message)
        {
            _consoleWriter.Write(message);
        }




       

        /// <summary>
        /// Provides a silent err-rreporter for command-line parsing the DecisionArgumentsInfo
        /// object, used solely to determine if the control-file option is being exercised.
        /// </summary>
        /// <param name="message">the message to report...or in this case, to ignore</param>
        private static void SilentErrorReporter(string message) { }

        #region > Authentication & Authorizations <

        private static bool AuthorizeUser(JobParameters job)
        {
            bool isAuthorized = false;

            //these two permisions dictate access to the temporary Registry
            bool hasRegTempPermission = (
                Csla.ApplicationContext.User.IsInRole("ADD_COMPOUND_TEMP")
                || Csla.ApplicationContext.User.IsInRole("REGISTER_TEMP")
            );
            //these two permisions dictate access to the permanent Registry
            bool hasRegPermPermission = (
                Csla.ApplicationContext.User.IsInRole("ADD_COMPONENT")
                || Csla.ApplicationContext.User.IsInRole("EDIT_COMPOUND_REG")
                || Csla.ApplicationContext.User.IsInRole("REGISTER_DIRECT")
            );

            switch (job.TargetActionType)
            {
                case TargetActionType.CountRecords:
                case TargetActionType.SplitFile:
                case TargetActionType.ListFields:
                case TargetActionType.ListTables:
                    {
                        isAuthorized = true;
                        break;
                    }
                case TargetActionType.ValidateMapping:
                case TargetActionType.ValidateData:
                    {
                        isAuthorized = (hasRegTempPermission || hasRegPermPermission);
                        break;
                    }
                case TargetActionType.ImportTemp:
                    {
                        isAuthorized = hasRegTempPermission;
                        break;
                    }
                case TargetActionType.FindDuplicates:
                case TargetActionType.ImportRegDupAsTemp:
                case TargetActionType.ImportRegDupAsCreateNew:
                case TargetActionType.ImportRegDupAsNewBatch:
                case TargetActionType.ImportRegDupNone:
                    {
                        isAuthorized = hasRegPermPermission;
                        break;
                    }
            }

            return isAuthorized;
        }

        /// <summary>
        /// Authenticates the console user via COE security services.
        /// </summary>
        /// <param name="user">the 'user' or 'login' argument</param>
        /// <param name="password">the 'pwd' or 'password' argument</param>
        private static void AuthenticateUser(string user, string password)
        {
            _user = user;
            _password = password;

            string msg = string.Empty;
            try
            {
                _authenticated = COEPrincipal.Login(_user, _password);
            }
            catch (Csla.DataPortalException cex)
            {
                msg = cex.BusinessException.GetBaseException().Message;
            }
            catch (Exception ex)
            {
                //try a second time
                _authenticated = COEPrincipal.Login(_user, _password);
                if (!_authenticated)
                {
                    msg = ex.GetBaseException().Message;
                }
            }
        }

        /// <summary>
        /// Authenticates the console user via COE security services.
        /// If the user presses Excape at any time during authentication, the application will exit.
        /// </summary>
        private static void AuthenticateUser()
        {
            _user = ReadLogin();
            if (cki.Key == ConsoleKey.Escape)
                Environment.Exit(0);

            _password = ReadPassword();
            if (cki.Key == ConsoleKey.Escape)
                Environment.Exit(0);

            string msg = string.Empty;
            try
            {
                _authenticated = COEPrincipal.Login(_user, _password);
            }
            catch (Csla.DataPortalException cex)
            {
                msg = cex.BusinessException.GetBaseException().Message;
            }
            catch (Exception ex)
            {
                msg = ex.GetBaseException().Message;
            }

            // display any 'error' message
            if (!string.IsNullOrEmpty(msg))
            {
                Console.WriteLine();
                Console.WriteLine(msg);
                Console.ReadLine();
                Console.Clear();
                AuthenticateUser();
            }
        }

        /// <summary>
        /// Gets the user name to be used for authentication purposes.
        /// </summary>
        /// <returns>the user's login</returns>
        private static string ReadLogin()
        {
            // gather credential information from user input
            Console.Write("Enter login: ");

            Stack<string> bits = new Stack<string>();
            //keep reading
            for (cki = Console.ReadKey(true); cki.Key != ConsoleKey.Enter && cki.Key != ConsoleKey.Escape; cki = Console.ReadKey(true))
            {
                if (cki.Key == ConsoleKey.Backspace)
                {
                    //rollback the cursor and write a space so it looks backspaced to the user
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(" ");
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    if (bits.Count > 0)
                        bits.Pop();
                }
                else
                {
                    Console.Write(cki.KeyChar.ToString());
                    bits.Push(cki.KeyChar.ToString());
                }
            }

            string[] login = bits.ToArray();
            Array.Reverse(login);
            string user = string.Join(string.Empty, login);
            return user;
        }

        /// <summary>
        /// From: http://ryepup.unwashedmeme.com/blog/2007/07/05/reading-passwords-from-the-console-in-c/
        /// Essentially, collects input and builds a password string while masking the display.
        /// </summary>
        /// <returns></returns>
        private static string ReadPassword()
        {
            Console.WriteLine();
            Console.Write("Enter password: ");

            Stack<string> passbits = new Stack<string>();
            //keep reading
            for (ConsoleKeyInfo cki = Console.ReadKey(true); cki.Key != ConsoleKey.Enter && cki.Key != ConsoleKey.Escape; cki = Console.ReadKey(true))
            {
                if (cki.Key == ConsoleKey.Backspace)
                {
                    //rollback the cursor and write a space so it looks backspaced to the user
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(" ");
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    passbits.Pop();
                }
                else if (cki.Key == ConsoleKey.Escape)
                {
                    Application.Exit();
                }
                else
                {
                    Console.Write("*");
                    passbits.Push(cki.KeyChar.ToString());
                }
            }
            string[] pass = passbits.ToArray();
            Array.Reverse(pass);
            string pwd = string.Join(string.Empty, pass);
            return pwd;
        }

        #endregion

    }
}