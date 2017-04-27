using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class Consts
    {
        public const string apiVersion = "1.0";

        public const string apiPrefix = "api/v{version:apiVersion}/";

        public const string ssoCookieName = "COESSO";
    }
}