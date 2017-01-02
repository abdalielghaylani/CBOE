/*  GREATIS FORM DESIGNER FOR .NET
 *  License Provider
 *  Copyright (C) 2004-2006 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Xml;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Greatis
{
   namespace FormDesigner
   {
      class TreasuryLicense : License
      {
         public TreasuryLicense(string license)
         {
            this.licenseStr = license;
         }

         public override string LicenseKey
         {
            get
            {
               return licenseStr;
            }
         }

         public override void Dispose()
         {
         }

         private string licenseStr;

      }

      class TreasuryLP : LicenseProvider
      {
         public TreasuryLP()
         {
         }

         public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
         {
            return new TreasuryLicense("granted");
         }
      }
   }
}
