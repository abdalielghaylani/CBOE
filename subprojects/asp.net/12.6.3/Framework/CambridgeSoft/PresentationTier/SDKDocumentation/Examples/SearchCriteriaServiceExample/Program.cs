using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace SearchCriteriaServiceExample
{
    class Program
    {
        static void Main(string[] args)
        {
            COEPrincipal.Login("CSSADMIN", "CSSADMIN");
            SearchCriteriaServiceMenu menu = new SearchCriteriaServiceMenu();
            menu.MainMenu();
        }
    }
}
