using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WeChatAuthorization.Helper
{
    public class HttpMethods
    {
        #region POST
        /// <summary>
        /// 模拟Http POST请求 
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="param"></param>
        /// <returns>返回响应值</returns>
        public static string HttpPost(string url, string param)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            string responseStr;
            var requestStream = new StreamWriter(request.GetRequestStream());
            requestStream.Write(param);
            requestStream.Close();

            var response = request.GetResponse();
            try
            {
                StreamReader reader = new StreamReader(stream: response.GetResponseStream(), encoding: Encoding.UTF8);
                responseStr = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception e)
            {
                //捕获异常
                throw e;
            }

            return responseStr;
        }

        /// <summary>
        /// HTTP POST方式请求数据(带图片)
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="param">POST的数据</param>
        /// <param name="filePath">图片文件路径</param>
        /// <returns>响应文本</returns>
        public static string HttpPost(string url, IDictionary<object, object> param, string filePath)
        {
            string boundary = "-----------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string responseStr = null;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (var key in param.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, param[key]);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }

            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "pic", filePath, "text/plain");
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                if (stream2 != null)
                {
                    StreamReader reader2 = new StreamReader(stream2);
                    responseStr = reader2.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                if (wresp != null)
                {
                    wresp.Close();
                }
            }

            return responseStr;
        }

        /// <summary>
        /// HTTP POST方式请求数据(带图片)
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="param">POST的数据</param>
        /// <param name="fileByte">图片(字节数组)</param>
        /// <returns>响应文本</returns>
        public static string HttpPost(string url, IDictionary<object, object> param, byte[] fileByte)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string responseStr = null;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in param.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, param[key]);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "pic", fileByte, "text/plain"); //image/jpeg
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            rs.Write(fileByte, 0, fileByte.Length);

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                if (stream2 != null)
                {
                    StreamReader reader2 = new StreamReader(stream2);
                    responseStr = reader2.ReadToEnd();
                }
            }
            catch (Exception)
            {
                if (wresp != null)
                {
                    wresp.Close();
                }
            }
            return responseStr;
        }
        #endregion

        #region GET
        /// <summary>
        /// 模拟Http Get请求
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <returns>返回请求结果</returns>
        public static string HttpGet(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            string responseStr;
            StreamReader reader = null;
            var response = request.GetResponse();
            try
            {
                reader = new StreamReader(response.GetResponseStream(), encoding: Encoding.UTF8);
                responseStr = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                request.GetResponse().GetResponseStream().Close();
                reader.Close();
            }
            return responseStr;
        }

        /// <summary>
        /// 模拟Http Get请求
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <returns>字节数组</returns>
        public static byte[] GetHttpRequestStream(string url)
        {
            byte[] bytes = null;
            if (string.IsNullOrEmpty(url))
                return bytes;
           
            StreamReader stream = null;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = new CookieContainer();
            CookieContainer cookie = request.CookieContainer;
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;

            try
            {
                stream = new StreamReader(request.GetResponse().GetResponseStream());
                List<byte> lBtyes = new List<byte>();
                while (stream.BaseStream.CanRead)
                {
                    int result = stream.BaseStream.ReadByte();
                    if (result == -1) break;
                    lBtyes.Add((byte)result);
                }
                bytes = lBtyes.ToArray();
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                request.GetResponse().GetResponseStream().Close();
                stream.Close();
            }
            return bytes;
        }

        /// <summary>
        /// 模拟Http Get请求
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="ec">编码方式</param>
        /// <returns></returns>
        public static string GetHttpRequest(string url, Encoding ec)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;
            StreamReader stream = null;
            string responseValue = string.Empty;
            try
            {
                stream = new StreamReader(request.GetResponse().GetResponseStream(), ec);
                responseValue = stream.ReadToEnd();
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                request.GetResponse().GetResponseStream().Close();
                stream.Close();
            }
            return responseValue;
        }
        #endregion
    }
}