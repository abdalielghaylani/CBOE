using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core.Common
{
    //utility class for writing to the console that prevents writing is  silent has been specified
   [Serializable]
    public class ConsoleWriter
    {
        private bool _runSilent;

	    public bool RunSilent
	    {
		    
            set
            {
                
                 _runSilent = value;
                
            
           }
	    }
	
        public ConsoleWriter()
        {

        }

        public  void Write(string input)
        {
            if (_runSilent == false)
            {
                Console.Write(input);
            }
        }

        public void Write(string format, object arg0)
        {
            if (_runSilent == false)
            {
                Console.Write(format, arg0);
            }
        }

        public void Write(string format, object arg0, object arg1)
        {
            if (_runSilent == false)
            {
                Console.Write(format, arg0, arg1);
            }
        }


        public void WriteLine(string format, object arg0, object arg1)
        {
            if (_runSilent == false)
            {
                Console.WriteLine(format, arg0, arg1);
            }
        }

        public void WriteLine()
        {
            if (_runSilent == false)
            {
                Console.WriteLine();
            }
        }

        public void WriteLine(string format, object arg0)
        {
            if (_runSilent == false)
            {
                Console.WriteLine(format, arg0);
            }
        }

        public void WriteLine(string input)
        {
            if (_runSilent == false)
            {
                Console.WriteLine(input);
            }
        }

        public void CursorLeft(int input)
        {
            if (_runSilent == false)
            {
                Console.CursorLeft= input;
            }
        }

       

     

    }
}
