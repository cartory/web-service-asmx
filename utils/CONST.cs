using System;
using System.Web;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using log4net;

namespace utils
{
    public static class CONST
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}