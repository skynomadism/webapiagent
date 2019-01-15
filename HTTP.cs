using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WebApiAgent
{
    public class HTTP
    {
        JavaScriptSerializer jsonconvert = new JavaScriptSerializer();//用于将Json解析成数据结构
        public string Post(string url, string param,bool isJsonData=true)
        {
            string result = string.Empty;
            try
            {
                // 设置提交的相关参数 
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //HTTPS时，需要增加ServicePointManager相关设置
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    // 这里设置了协议类型。
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                    ServicePointManager.CheckCertificateRevocationList = true;
                    ServicePointManager.DefaultConnectionLimit = 100;
                    ServicePointManager.Expect100Continue = false;
                    request.ProtocolVersion = HttpVersion.Version11;
                }
                 //注意提交的编码 这边是需要改变的 这边默认的是Default：系统当前编码
                byte[] postData = Encoding.UTF8.GetBytes(param);
                request.Method = "POST";
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.Referer = null;
                request.Accept = "*/*";
                request.ContentType = isJsonData ? "application/json" : "text/plain";//application/json//text/plain//application/x-www-form-urlencoded//multipart/form-data
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR  3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.ContentLength = postData.Length;
                request.Headers.Add("datafrom", "Maxqueen");
                
                // POST 提交请求数据 
                using (System.IO.Stream outputStream = request.GetRequestStream()) 
                {
                    outputStream.Write(postData, 0, postData.Length);
                }
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream()) 
                {
                    using (StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception exp)
            {
                LogFile.WriteLog(exp.ToString(), "HTTP_EROO", 1, 7);
            }
            return result;
        }
        public string Get(string Url, string paramdataStr = "")
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (paramdataStr == "" ? "" : "?") + paramdataStr);
                //HTTPS时，需要增加ServicePointManager相关设置
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    // 这里设置了协议类型。
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                    ServicePointManager.CheckCertificateRevocationList = true;
                    ServicePointManager.DefaultConnectionLimit = 100;
                    ServicePointManager.Expect100Continue = false;
                    request.ProtocolVersion = HttpVersion.Version11;
                }

                request.Method = "GET";
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.Referer = null;
                request.Accept = "*/*";
                request.ContentType = "application/json;charset=UTF-8";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR  3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Headers.Add("datafrom", "Maxqueen");

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream myResponseStream = response.GetResponseStream()) 
                {
                    using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                    {
                        result = myStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception exp)
            {
                LogFile.WriteLog(exp.ToString(), "HTTP_EROO", 1, 7);
                return string.Empty;
            }
            return result;
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
        
        /// <summary>
        /// 将Json数据转换成结构（注意这个结构是封装在WebApiResponse对像内的）
        /// 如果要想转换的结构与WebApiResponse没有关系，请使用ConvertJsonToSimpleObj
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public WebApiResponse<T> ConvertJsonToObj<T>(string jsonString)
        {
            return jsonString != null && jsonString != "" ? jsonconvert.Deserialize<WebApiResponse<T>>(jsonString) : null;
        }

        public T ConvertJsonToSimpleObj<T>(string jsonString)
        {
            return jsonString != null && jsonString != "" ? jsonconvert.Deserialize<T>(jsonString) : default(T);
        }
        public string ConvertObjToJson<T>(T obj)
        {
            return jsonconvert.Serialize(obj);
        }
    }
   
    
    [DataContract]
    public class WebApiResponse<T>
    {
        [DataMember]
        public string IsSuccess { get; set; }
        [DataMember]
        public bool dbconn { get; set; }
        [DataMember]
        public string Action { get; set; }
        [DataMember]
        public int DataCount { get; set; }
        [DataMember]
        public string RequestTime { get; set; }
        [DataMember]
        public int TotalCount { get; set; }
        [DataMember]
        public string Debug { get; set; }
        [DataMember]
        public List<T> DataList { get; set; }
    }
    [DataContract]
    public class ActionResult 
    {
        [DataMember]
        public int DoneCount { get; set; }
    }
    [DataContract]
    public class RunSQL 
    {
        public RunSQL() { this.cnfields = new List<string>(); }
        public RunSQL(string _cmd) { this.cmd = _cmd; this.cnfields = new List<string>(); }
        public RunSQL(string _cmd, string _fields) 
        {
            this.cnfields = new List<string>();
            this.cmd = _cmd;
            this.AddCnFields(_fields);
        }
        public void AddCnFields(string _fields) 
        {
            string[] fields = _fields.Split(',');
            for (int i = 0; i < fields.Length; i++) 
            {
                this.cnfields.Add(fields[i]);
            }
        }
        [DataMember]
        public string cmd { get; set; }
        [DataMember]
        public List<string> cnfields { get; set; }
        [DataMember]
        public string countCmd { get; set; }
    }
    
   
}
