using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Net;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace utils.RestAdapter
{
    public class RequestOptions
    {
        public static RequestOptions DefaultOptions => new RequestOptions();

        public int Timeout = -1;
        public RestRequest req = null;

        public string Method = "GET";
        public string Accept = "application/json";
        public string UserAgent = "Studio Manager";
        public string ContentType = "application/json; charset=UTF-8";
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
    }

    public class RestAdapter
    {
        private static readonly log4net.ILog log = CONST.log;
        
        public string Fetch(string URL, RequestOptions options = null) 
        {
            RestResponse res;
            
            if (options == null) options = RequestOptions.DefaultOptions;
            log.Debug(options.req?.ToJson() ?? URL);

            try
            {
                // Executes Action before Calling REST API
                options.req?.Run();
                // Calling REST API and ObjectConversion
                string json = this.CallRestAPI(URL, options);
                res = RestResponse.FromJson(json);
                // Executes Action after Calling REST API
                res.Run();
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                res = RestResponse.DefaultResponse(CodeRes.COD003, e.Message);
            }

            log.Info(res.ToJson());
            return this.ToXML(res);
        }

        private string CallRestAPI(string URL, RequestOptions options)
        {
            try
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(options.req?.ToJson() ?? "");
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);

                req.Accept = options.Accept;
                req.Method = options.Method;
                req.UserAgent = options.UserAgent;
                req.ContentType = options.ContentType;

                req.ContentLength = jsonBytes.Length;

                foreach (var header in options.Headers)
                {
                    req.Headers.Add(header.Key, header.Value);
                }

                Stream reqStream = req.GetRequestStream();
                reqStream.Write(jsonBytes, 0, jsonBytes.Length);
                reqStream.Close();

                WebResponse res = req.GetResponse();
                Stream resStream = res.GetResponseStream();

                string jsonRes = new StreamReader(resStream).ReadToEnd();

                res.Close();
                resStream.Close();

                return jsonRes;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Error on API CALL : {e.Message}");
            }
        }

        private string ToXML(RestResponse res)
        {
            string xml;

            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(res.GetType());
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.GetEncoding("ISO-8859-1"));

            xmlSerializer.Serialize(xmlTextWriter, res.GetType());
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

            xml = Encoding.GetEncoding("ISO-8859-1").GetString(memoryStream.ToArray());

            memoryStream.Close();
            xmlTextWriter.Close();
            memoryStream.Dispose();

            return xml;
        }
    }
}