using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Security.AccessControl;
using System.IO;
using System.Diagnostics;

namespace FormDBLib
{
    public class SecurityUtils
    {
        public static void GrantAccess(string path)
        {
            StringBuilder sb = new StringBuilder();

            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            WindowsPrincipal wp = new WindowsPrincipal(wi);
#if DEBUG
            sb.Append("WinIdentity - Authentication type: ");
            sb.AppendLine(wi.AuthenticationType.ToString());
            sb.Append("WinIdentity - name: ");
            sb.AppendLine(wi.Name);

            if (wp.IsInRole("Administrators"))
                sb.Append("You are an Administrator \n");
            else
                sb.AppendLine("You are NOT an Administrator \n");
            Debug.Write(sb.ToString());
#endif

            //  Get a well-known OS group that you want to grant permissions to. Get the SID and then convert it to an account.
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));

#if DEBUG
            sb.Append("NTAccount: ");
            sb.Append(string.Concat(account.Value.ToString(), " \n"));
            Debug.Write(sb.ToString());
#endif

            // Gets the access control list (ACL) entries
            FileSecurity sec = File.GetAccessControl(path);
            // Add a new Access Rule for the account with Full Control over the path.
            sec.AddAccessRule(new FileSystemAccessRule(account, FileSystemRights.FullControl, AccessControlType.Allow));
            // Applies the access control list (ACL) to the spefified file
            File.SetAccessControl(path, sec);
        }
    }

}
