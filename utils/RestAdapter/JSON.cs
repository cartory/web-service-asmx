using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace utils.RestAdapter
{
    public abstract class JSON<T>
    {
        public virtual string ToJson(JsonSerializerSettings settings = null) => JsonConvert.SerializeObject(this, settings ?? JsonConvert.DefaultSettings());
        public static T FromJson(string json, JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<T>(json, settings ?? JsonConvert.DefaultSettings());
    }
}