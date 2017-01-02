
using System;
namespace SpotfireIntegration.Common
{
    public class Uris
    {
        public static Uri SpotfireServiceBaseAddress
        {
            get
            {
                UriBuilder builder = new UriBuilder(Uri.UriSchemeNetPipe, "localhost");
                builder.Path = Properties.Resources.SpotfireServiceBaseAddress;
                return builder.Uri;
            }
        }

        public static string SpotfireServiceName
        {
            get
            {
                return Properties.Resources.SpotfireServiceName;
            }
        }

        public static Uri SpotfireServiceUri
        {
            get
            {
                return new Uri(SpotfireServiceBaseAddress.ToString() + "/" + SpotfireServiceName);
            }
        }
    }
}
