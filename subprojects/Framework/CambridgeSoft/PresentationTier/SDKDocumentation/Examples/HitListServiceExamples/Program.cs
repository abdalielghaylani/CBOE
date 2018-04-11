using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Web;
using System.Configuration;

namespace HitListServiceExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            COEPrincipal.Login("CSSADMIN", "CSSADMIN");
            HitListServiceMenu hitListMenu = new HitListServiceMenu();
            hitListMenu.MainMenu();
        }

    }
}
