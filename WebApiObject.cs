using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WebApiAgent
{
    public class WebApiObject
    {
        
        HTTP myHttp = new HTTP();
        private string UrlRoot = "http://localhost/";
        private string ParmEx = "&token=TESTTOKEN";
        public WebApiObject(string _UrlRoot,string appname="default")
        {
            this.UrlRoot = _UrlRoot;
            this.ParmEx += ("&app="+appname);
        }

        public List<T> SearchData<T>(string apiUrl) {

            string jsonResult = myHttp.Get(this.UrlRoot+apiUrl+ParmEx);
            WebApiResponse<T> response = myHttp.ConvertJsonToObj<T>(jsonResult);
            if (response != null && response.DataList != null && response.DataList.Count > 0) { return response.DataList; }
            return new List<T>();
        }
        /// <summary>
        /// 在分页查询中使用的一种接口
        /// totalCount是否有效，需要在Url的参数中明确标识
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiUrl"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<T> SearchData<T>(string apiUrl,ref int totalCount)
        {
            totalCount = 0;
            string jsonResult = myHttp.Get(this.UrlRoot + apiUrl + ParmEx);
            WebApiResponse<T> response = myHttp.ConvertJsonToObj<T>(jsonResult);
            if (response != null && response.DataList != null && response.DataList.Count > 0) 
            { 
                totalCount = response.TotalCount; 
                return response.DataList; 
            }
            return new List<T>();
        }

        public int InsertData(string apiUrl, object data, bool isPost = true) 
        {
            string jsonResult = "";
            if (isPost) { jsonResult = myHttp.Post(this.UrlRoot + apiUrl + ParmEx, myHttp.ConvertObjToJson(data)); }
            else{ jsonResult = myHttp.Get(apiUrl);}
            WebApiResponse<ActionResult> response = myHttp.ConvertJsonToObj<ActionResult>(jsonResult);
            if (response != null && response.DataList != null && response.DataList.Count > 0) { return response.DataList[0].DoneCount; }
            return 0;
        }
        public int Updata(string apiUrl, object data, bool isPost = true) 
        {
            string jsonResult = "";
            if (isPost) { jsonResult = myHttp.Post(this.UrlRoot + apiUrl + ParmEx, myHttp.ConvertObjToJson(data)); }
            else { jsonResult = myHttp.Get(apiUrl); }
            WebApiResponse<ActionResult> response = myHttp.ConvertJsonToObj<ActionResult>(jsonResult);
            if (response != null && response.DataList != null && response.DataList.Count > 0) { return response.DataList[0].DoneCount; }
            return 0;
        }

        public int DeleteData(string apiUrl) 
        {
            string jsonResult = myHttp.Get(this.UrlRoot + apiUrl + ParmEx);
            WebApiResponse<ActionResult> response = myHttp.ConvertJsonToObj<ActionResult>(jsonResult);
            if (response != null && response.DataList != null && response.DataList.Count > 0) { return response.DataList[0].DoneCount; }
            return 0;
        }

        public List<T> RunReadSQL<T>(string apiUrl,RunSQL SQL,bool isDataEncoding = true) 
        {
            string PostData = myHttp.ConvertObjToJson(SQL);
            if (isDataEncoding)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(PostData);
                PostData = Convert.ToBase64String(bytes);
            }
            string jsonResult = myHttp.Post(this.UrlRoot + apiUrl + ParmEx + (isDataEncoding?"&encoding=true":""), PostData,false);
            WebApiResponse<T> response = myHttp.ConvertJsonToObj<T>(jsonResult);
            if (response != null && response.DataList != null && response.DataList.Count > 0) { return response.DataList; }
            return new List<T>();
        }

        public List<T> RunReadSQL<T>(string apiUrl, RunSQL SQL,ref int totalCount, bool isDataEncoding = true)
        {
            totalCount = 0;
            string PostData = myHttp.ConvertObjToJson(SQL);
            if (isDataEncoding)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(PostData);
                PostData = Convert.ToBase64String(bytes);
            }
            string jsonResult = myHttp.Post(this.UrlRoot + apiUrl + ParmEx + (isDataEncoding ? "&encoding=true" : ""), PostData,false);
            WebApiResponse<T> response = myHttp.ConvertJsonToObj<T>(jsonResult);
            if (response != null && response.DataList != null && response.DataList.Count > 0)
            {
                totalCount = response.TotalCount; 
                return response.DataList; 
            }
            return new List<T>();
        }

        public int RunWriteSQL(string apiUrl, RunSQL SQL, bool isDataEncoding = true) 
        {
            string PostData = myHttp.ConvertObjToJson(SQL);
            if (isDataEncoding)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(PostData);
                PostData = Convert.ToBase64String(bytes);
            }
            string jsonResult = myHttp.Post(this.UrlRoot + apiUrl + ParmEx + (isDataEncoding ? "&encoding=true" : ""), PostData);
            WebApiResponse<ActionResult> response = myHttp.ConvertJsonToObj<ActionResult>(jsonResult);
            if (response != null && response.DataList != null && response.DataList.Count > 0) { return response.DataList[0].DoneCount; }
            return 0;
        }
    }
}
