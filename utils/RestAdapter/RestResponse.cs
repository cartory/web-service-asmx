using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace utils.RestAdapter
{
    public static class CodeRes
    {
        public const string COD000 = "COD000";
        public const string COD003 = "COD003";
    }


    [DataContract]
    public abstract class BaseResponse : JSON<RestResponse>
    {
        [DataMember]
        [JsonProperty("codRes")]
        public string CodRes { get ; set; }
        
        [DataMember]
        [JsonProperty("codError")]
        public string CodError { get; set; }
        
        [DataMember]
        [JsonProperty("desError")]
        public string DesError { get; set; }
    }

    public abstract class RestResponse : BaseResponse
    {
        public static RestResponse DefaultResponse(string code, string desError)
        {
            return RestResponse.FromJson("{codeRes: \"" + code +"\",codeError:\"" + code +"\",desError:\"" + desError +"\"}");
        }

        public virtual void Run() { }
    }
}