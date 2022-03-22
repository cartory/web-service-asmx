using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WCFService.utils
{
    abstract class HttpService
    {
        protected string URL { get; }

        private void SetHeaders(HttpWebRequest req)
        {
            string username = ConfigurationManager.AppSettings["username"];
            string password = ConfigurationManager.AppSettings["password"];
            string authEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{username}:{password}"));

            req.Headers.Add("Authorization", $"Basic {authEncoded}");
        }

        private void SetConfig(
            HttpWebRequest req,

            int timeout = -1,
            string method = "POST",
            string accept = "application/json",
            string contentType = "application/json; charset=UTF-8"
        ) 
        {
            req.Method = method;
            req.Accept = accept;
            req.Timeout = timeout;
            req.ContentType = contentType;
        }

        public string CallRestAPI(string json)
        {
            try
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);

                SetConfig(req);
                SetHeaders(req);

                req.ContentLength = jsonBytes.Length;
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
    }
}