using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;

namespace utils.RestAdapter
{
    public abstract class RestRequest : JSON<RestRequest>
    {
        public virtual void Run() { }
    }
}