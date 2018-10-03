using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Main class. In charge of handling user input and displaying help messages.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Description:
        ///     Patches a system by running a the list of csbr fixes specified with -l parameter. If not provided all csbrs are run at once.
        /// 
        /// Usage: 
        ///     patcher -i orclInstance -u oracleuser -p oraclepass [-l listpath] [-h]
        /// 
        /// Options:
        ///     -l Path to a txt file with the csbrs to apply.
        ///     -i Oracle Instance
        ///     -u Oracle user
        ///     -p Oracle password
        ///     -h Show this help
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string[] userInput = new string[4];
            bool inputIsValid = false;
            try
            {
                userInput = ReadUserInput(args);
                bool runFromCurrentversion = ValidateInput(userInput);
                inputIsValid = true;

                if (runFromCurrentversion)
                {
                    //TODO: improve the mechanism to deside which lists have to be done based on a current version, so that there is no need to recompile this class when new releases delivered.
                    switch (userInput[3])
                    {
                        case "11.0.1":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\11.0.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\11.0.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\11.0.4.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.4.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "11.0.2":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\11.0.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\11.0.4.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.4.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "11.0.3":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\11.0.4.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.4.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "11.0.4":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.4.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.1.0":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.4.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.1.1":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.4.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.1.3":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.3.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.4.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.5.0":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.5.1":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.5.2":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.5.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.5.3":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.0.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.6.0":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.6.1":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.2.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.6.2":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\12.6.3.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "12.6.3":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "17.1.0":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\17.1.1.txt");
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                        case "18.1.0":
                            Patch(userInput[0], userInput[1], userInput[2], ".\\PatchLists\\18.1.0.txt");
                            break;
                    }
                }
                else
                {
                    Patch(userInput[0], userInput[1], userInput[2], userInput[3]);
                }
            }
            catch(Exception ex)
            {
                if(!ex.Message.Contains(Resource.Title_PatcherHelp))
                {
                    Console.WriteLine("The following error occurred:");
                    Console.Write("\t");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(Resource.Title_PatcherHelp);
                }
                if(!inputIsValid)
                    Console.WriteLine(GetHelp());
            }

            #if DEBUG
            Console.WriteLine();
            Console.WriteLine("Press any key to finish...");
            Console.ReadKey();
            #endif
        }

        /// <summary>
        /// Applies a patch with the given information
        /// </summary>
        /// <param name="oracleInstance"></param>
        /// <param name="oracleName"></param>
        /// <param name="oraclePassword"></param>
        /// <param name="fileWithPatchList"></param>
        private static void Patch(string oracleInstance, string oracleUser, string oraclePassword, string fileWithPatchList)
        {
            PatchController controller = new PatchController(oracleInstance, oracleUser, oraclePassword, fileWithPatchList);
            controller.Patch();
        }

        /// <summary>
        /// Returns the following help:
        /// 
        ///  Patches a system by running a the list of csbr fixes specified with -l parameter. If not provided all csbrs are run at once.
        ///     
        ///  -l Path to a txt file with the csbrs to apply.
        ///  -i Oracle Instance
        ///  -u Oracle user
        ///  -p Oracle password
        ///  -h Show this help
        ///  
        /// </summary>
        /// <returns>User help</returns>
        private static string GetHelp()
        {
            return @"
    Description:
        Patches a system by running a the list of csbr fixes specified with -l parameter. If not provided all csbrs are run at once. 
        When prompted, e?nter the version you are upgrading from (not the version you are installing).
        Valid version numbers are 11.0.1, 11.0.2, 11.0.3, 11.0.4, 12.1.0, 12.1.1, 12.1.3
    
    Usage: 
        patcher -i orclInstance -u oracleuser -p oraclepass [-l listpath] [-h]
    
    Options:
        -l Path to a txt file with the csbrs to apply.
        -i Oracle Instance
        -u Oracle user
        -p Oracle password
        -h Show this help
";
        }

        /// <summary>
        /// Validates that the required parameters are being passed and that the file exists if provided.
        /// </summary>
        /// <param name="userInput">a string[] of the form {orclInstance, orclName, orclPass, listPath}</param>
        private static bool ValidateInput(string[] userInput)
        {
            bool runFromCurrVersion = false;
            Console.WriteLine();
            Console.WriteLine();
            if(string.IsNullOrEmpty(userInput[0]))
            {
                Console.Write("Oracle Instance (orcl): ");
                string userTyped = Console.ReadLine();
                if (string.IsNullOrEmpty(userTyped))
                    userInput[0] = "orcl";
                else
                    userInput[0] = userTyped;
            }
            if(string.IsNullOrEmpty(userInput[1]))
            {
                Console.Write("Oracle User (system): ");
                string userTyped = Console.ReadLine();
                if (string.IsNullOrEmpty(userTyped))
                    userInput[1] = "system";
                else
                    userInput[1] = userTyped;
            }
            if(string.IsNullOrEmpty(userInput[2]))
            {
                Console.Write("Oracle Password: ");
                while(true)
                {
                    ConsoleKeyInfo readKey = Console.ReadKey(true);
                    if(readKey.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else
                    {
                        userInput[2] += readKey.KeyChar;
                        Console.Write("*");
                    }
                }
                Console.WriteLine();
                if (string.IsNullOrEmpty(userInput[2])) { /*TODO: handle this error somehow.*/}
            }
            if (!string.IsNullOrEmpty(userInput[3]))
            {
                if (!File.Exists(userInput[3]))
                    throw new Exception(Resource.Error_FileNotFound);
            }
            else
            {
                // Ask for current version.
                Console.Write("What version are you upgrading from? (11.0.1): ");
                string userTyped = Console.ReadLine();
                if (string.IsNullOrEmpty(userTyped))
                    userInput[3] = "11.0.1";
                else
                    userInput[3] = userTyped;

                runFromCurrVersion = true;
            }
            Console.WriteLine();
            Console.WriteLine();
            return runFromCurrVersion;
        }
        
        /// <summary>
        /// Reads user input and returns a list in the form {orclInstance, orclName, orclPass, listPath}
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>a string[] in the form {orclInstance, orclName, orclPass, listPath} </returns>
        private static string[] ReadUserInput(string[] args)
        {
            string[] result = new string[4];
            /*
             * 
             * Handle user command line arguments:
             * 
             * -l Path to a txt file with the csbrs to apply.
             * -i Oracle Instance
             * -u Oracle user
             * -p Oracle password
             * -h Show this help
             * 
             */
            Options previousSelection = Options.None;
            foreach(string arg in args)
            {
                if(previousSelection != Options.None)
                {
                    switch(previousSelection)
                    {
                        case Options.ListPath:
                            result[3] = arg;
                            break;
                        case Options.OracleInstance:
                            result[0] = arg;
                            break;
                        case Options.OraclePassword:
                            result[2] = arg;
                            break;
                        case Options.OracleUser:
                            result[1] = arg;
                            break;
                    }
                    previousSelection = Options.None;
                }
                else
                {
                    switch(arg)
                    {
                        case "-l":
                            previousSelection = Options.ListPath;
                            break;
                        case "-i":
                            previousSelection = Options.OracleInstance;
                            break;
                        case "-u":
                            previousSelection = Options.OracleUser;
                            break;
                        case "-p":
                            previousSelection = Options.OraclePassword;
                            break;
                        case "-h":
                            throw new Exception(Resource.Title_PatcherHelp);
                        default:
                            throw new Exception(Resource.Error_UnrecognizedParameter);
                    }
                }
            }
            return result;
        }
    }

    enum Options
    {
        None,
        ListPath,
        OracleInstance,
        OracleUser,
        OraclePassword
    }
}
