using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrudeMonitoring
{
    public static class Common
    {
        public static string WebRequest(string url, string addheader = null, string addvalue = null)
        {
            HttpWebRequest webRequest = null;
            string responseData = string.Empty;

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (addheader != null) webRequest.Headers.Add(addheader, addvalue);
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 200000;
            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;
        }

        public static string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = string.Empty;

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader = null;
                }
            }
            return responseData;
        }
        public static DateTime FromUnixTime(long unixTime)
        {
            var date = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
            return date;
        }

    }
}
