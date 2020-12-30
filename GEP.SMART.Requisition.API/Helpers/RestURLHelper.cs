using System.Diagnostics.CodeAnalysis;
using Gep.Cumulus.CSM.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace GEP.SMART.Requisition.API.Helpers
{
    [ExcludeFromCodeCoverage]
    public class RestURLHelper 
    {
        private static HttpWebRequest httpWebRequest = null;
        public string InvokeRestUrls(UserExecutionContext UserContext, string RestUrl, string JsonInputData, string MethodType, string jwtToken)
        {
            string returnValue = string.Empty;

            Uri address = new Uri(RestUrl);
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = MethodType;
            request.ContentType = "application/json";

            string userName = UserContext.UserName;
            string clientName = UserContext.UserName;

            UserContext.UserName = string.Empty;
            UserContext.ClientName = string.Empty;          

            request.Headers.Set("UserExecutionContext", JsonConvert.SerializeObject(UserContext));
            request.Headers.Set("Authorization", jwtToken);

            if (!string.IsNullOrEmpty(JsonInputData))
            {
                byte[] byteData = UTF8Encoding.UTF8.GetBytes(JsonInputData);
                request.ContentLength = byteData.Length;
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }
            }
            else
            {
                request.ContentLength = 0;
            }
                       
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    returnValue = reader.ReadToEnd();

                }
            }
            UserContext.UserName = userName;
            UserContext.ClientName = clientName;

            return returnValue;
        }

        public void CreateHttpWebRequest(string strURL, UserExecutionContext userExecutionContext, string method, string jwtToken)
        {
            httpWebRequest = WebRequest.Create(strURL) as HttpWebRequest;
            httpWebRequest.Method = method;
            httpWebRequest.ContentType = @"application/json";

            NameValueCollection nameValueCollection = new NameValueCollection();
            userExecutionContext.UserName = "";
            string userContextJson = Newtonsoft.Json.JsonConvert.SerializeObject(userExecutionContext);
            nameValueCollection.Add("UserExecutionContext", userContextJson);
            nameValueCollection.Add("Authorization", jwtToken);
            httpWebRequest.Headers.Add(nameValueCollection);
        }

        public string GetHttpWebResponse(Dictionary<string, object> odict)
        {            
            var data = JsonConvert.SerializeObject(odict);
            var byteData = Encoding.UTF8.GetBytes(data);


            httpWebRequest.ContentLength = byteData.Length;
            using (Stream stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
            }

            string result = null;
            using (HttpWebResponse resp = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
