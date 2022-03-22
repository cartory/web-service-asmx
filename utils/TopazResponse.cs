using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace WCFService.utils
{
    abstract class TopazResponse
    {
        [DataMember]
        [JsonProperty("codRes")]
        protected string CodRes { get; set; }

        [DataMember]
        [JsonProperty("codError")]
        protected string CodError { get; set; }

        [DataMember]
        [JsonProperty("desError")]
        protected string DesError { get; set; }

        public string ToXML()
        {
            string xml;

            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.GetEncoding("ISO-8859-1"));

            xmlSerializer.Serialize(xmlTextWriter, this);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

            xml = Encoding.GetEncoding("ISO-8859-1").GetString(memoryStream.ToArray());

            memoryStream.Close();
            xmlTextWriter.Close();
            memoryStream.Dispose();

            return xml;
        }
    }
}