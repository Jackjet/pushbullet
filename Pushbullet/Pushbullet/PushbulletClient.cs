using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pushbullet
{
    public class PushbulletClient : IPushbulletClient
    {
        public const string ORIGIN = "https://api.pushbullet.com";

        public string ApiKey { get; set; }

        public PushbulletClient(string apiKey)
        {
            this.ApiKey = apiKey;
        }

        public bool GetDevices(Action<DeviceList> callback) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/{1}", PushbulletClient.ORIGIN, "api/devices"));
            request.Credentials = new NetworkCredential(this.ApiKey, "");
            request.Method = "GET";

            try
            {
                IAsyncResult asyncGetResult = request.BeginGetResponse(new AsyncCallback((asyncResult) =>
                {
                    HttpWebRequest aRequest = (HttpWebRequest)asyncResult.AsyncState;
                    HttpWebResponse aResponse = (HttpWebResponse)aRequest.EndGetResponse(asyncResult);

                    using (StreamReader streamReader = new StreamReader(aResponse.GetResponseStream()))
                    {
                        string jsonString = streamReader.ReadToEnd();
                      
                        if (callback != null)
                        {
                            DeviceList output = JsonConvert.DeserializeObject<DeviceList>(jsonString);
                            callback(output);
                        }
                    }
                }), request);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public void PushAddress(Device device, string name, string address, Action<PushResponse> callback) 
        {
            string postData = string.Format("device_iden={0}&type={1}&title={2}&name={3}&address={4}", device.Identifier, "list", HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(address));
            this.Push(postData, callback);
        }

        public void PushFile(Device device, string file, Action<PushResponse> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("device_iden", device.Identifier);
            parameters.Add("type", "file");

            this.Push(parameters, file, callback);
        }

        public void PushLink(Device device, string title, string link, Action<PushResponse> callback)
        {
            string postData = string.Format("device_iden={0}&type={1}&title={2}&url={3}", device.Identifier, "list", HttpUtility.UrlEncode(title), HttpUtility.UrlEncode(link));
            this.Push(postData, callback);
        }

        public void PushList(Device device, string title, List<string> items, Action<PushResponse> callback)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("device_iden={0}&type={1}&title={2}", device.Identifier, "list", HttpUtility.UrlEncode(title)));
            foreach (string s in items)
            {
                builder.Append(string.Format("&items={0}", HttpUtility.UrlEncode(s)));
            }

            this.Push(builder.ToString(), callback);
        }

        public void PushNote(Device device, string title, string body, Action<PushResponse> callback)
        {
            string postData = string.Format("device_iden={0}&type={1}&title={2}&body={3}", device.Identifier, "list", HttpUtility.UrlEncode(title), HttpUtility.UrlEncode(body));
            this.Push(postData, callback);
        }

        private void Push(string postData, Action<PushResponse> callback) 
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}", PushbulletClient.ORIGIN, "api/pushes"));
            request.Method = "POST";
            request.Credentials = new NetworkCredential(this.ApiKey, "");

            byte[] data = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            Post(callback, request, data);
        }

        private void Push(Dictionary<string, object> parameters, string file, Action<PushResponse> callback)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}", PushbulletClient.ORIGIN, "api/pushes"));
            request.Method = "POST";
            request.Credentials = new NetworkCredential(this.ApiKey, "");

            string boundary = String.Format("----------{0:N}", Guid.NewGuid());

            byte[] data = GetMultipartFormData(parameters, file, boundary);

            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.ContentLength = data.Length;

            Post(callback, request, data);
        }

        private static void Post(Action<PushResponse> callback, HttpWebRequest request, byte[] data)
        {
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            request.BeginGetResponse((x) =>
            {
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonString = streamReader.ReadToEnd();

                        if (callback != null)
                        {
                            PushResponse output = JsonConvert.DeserializeObject<PushResponse>(jsonString);
                            callback(output);
                        }
                    }
                }
            }, null);
        }

        // Based on the implementation of http://www.briangrinstead.com/blog/multipart-form-post-in-c
        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string fileName, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
           
            foreach (var param in postParameters)
            {
                string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    boundary,
                    param.Key,
                    param.Value);

                formDataStream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
                formDataStream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, Encoding.UTF8.GetByteCount("\r\n"));
            }

            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                boundary,
                "file",
                fileName,
                "application/octet-stream");

            formDataStream.Write(Encoding.UTF8.GetBytes(header), 0, Encoding.UTF8.GetByteCount(header));
            
            // Write the file data directly to the Stream, rather than serializing it to a string.
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                formDataStream.Write(bytes, 0, (int)file.Length);
            }

            // Add the end of the request. Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(Encoding.UTF8.GetBytes(footer), 0, Encoding.UTF8.GetByteCount(footer));

            formDataStream.Position = 0;
            var sr = new StreamReader(formDataStream);
            var myStr = sr.ReadToEnd();

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }
}
