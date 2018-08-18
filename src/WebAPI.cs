using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Jovemnf.WebAPI
{
    public class WebAPI : IDisposable
    {

        public static Exception CheckException(WebException e)
        {
            if (e.Status == WebExceptionStatus.ProtocolError)
            {
                if (e.Response is HttpWebResponse response)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedAccessException();
                        case HttpStatusCode.NotFound:
                            throw new NotFoundException();
                        case HttpStatusCode.InternalServerError:
                            throw new InternalServerError();
                        case HttpStatusCode.PaymentRequired:
                            throw new PaymentRequired();
                    }
                }
            }
            else if (e.Status.Equals(WebExceptionStatus.RequestProhibitedByProxy))
            {
                throw new ProxyProhibited();
            }
            else if (e.Status.Equals(WebExceptionStatus.Timeout))
            {
                throw new TimeoutException();
            }
            else if (e.Status.Equals(WebExceptionStatus.RequestProhibitedByProxy))
            {
                throw new ProxyProhibited();
            }

            throw e;
        }

        public enum MethodRequest
        {
            POST, GET, DELETE, PUT
        }

        private int _timeout = 1000;
        private MethodRequest _method;
        private HttpWebRequest web;

        public WebAPI(string str, int timeout = 10000, MethodRequest metodo = MethodRequest.GET)
        {
            _timeout = timeout;
            _method = metodo;
            web = GetWebRequest(new Uri(str));
            web.KeepAlive = false;
        }

        public WebAPI(string str)
        {
            web = GetWebRequest(new Uri(str));
            web.KeepAlive = false;
        }

        public WebAPI(Uri str)
        {
            web = GetWebRequest(str);
            web.KeepAlive = false;
        }

        public Encoding Encoding;

        public WebAPI(Uri str, int timeout = 10000)
        {
            _timeout = timeout;
            web = GetWebRequest(str);
            web.KeepAlive = false;
        }

        public void Dispose()
        {
            try
            {
                web.Abort();
            }
            catch
            {
                throw new Exception("Impossível fechar conexão com o banco de dados");
            }
        }

        public HttpWebRequest WebRequest
        {
            get { return web; }
        }

        public void SetDataPost(Dictionary<string, object> dados)
        {
            try
            {
                string postData = "";
                foreach (string key in dados.Keys)
                {
                    postData += HttpUtility.UrlEncode(key) + "=" + this.Parse(dados, key) + "&";
                }
                byte[] data = Encoding.UTF8.GetBytes(postData);

                web.ContentType = "application/x-www-form-urlencoded";
                web.ContentLength = data.Length;

                Stream response = web.GetRequestStream();
                response.Write(data, 0, data.Length);
                response.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void SetJson(Dictionary<string, object> dados)
        {
            try
            {
                string output = JsonConvert.SerializeObject(dados);
                byte[] data = Encoding.UTF8.GetBytes(output);

                web.ContentType = "application/json";
                web.ContentLength = data.Length;

                Stream response = web.GetRequestStream();
                response.Write(data, 0, data.Length);
                response.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private object Parse(Dictionary<string, object> dados, string key)
        {
            try
            {
                return HttpUtility.UrlEncode(dados[key].ToString());
            }
            catch { return null; }
        }
       

        private String Parse(Dictionary<string, string> dados, string key)
        {
            try
            {
                return HttpUtility.UrlEncode(dados[key].ToString());
            }
            catch { return null; }
        }

        public void SetHeader(Dictionary<string, string> header)
        {
            try
            {
                foreach (string key in header.Keys)
                {
                    web.Headers.Add(key, header[key]);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string SetDataPost(Dictionary<string, string> dados)
        {
            try
            {
                string postData = "";
                foreach (string key in dados.Keys)
                {
                    postData += HttpUtility.UrlEncode(key) + "=" + this.Parse(dados, key) + "&";
                }

                byte[] data = Encoding.UTF8.GetBytes(postData);

                web.ContentType = "application/x-www-form-urlencoded";
                web.ContentLength = data.Length;

                Console.WriteLine(postData);

                Console.WriteLine(web.GetRequestStream());
                
                Stream response = web.GetRequestStream();
                response.Write(data, 0, data.Length);
                response.Close();

                return postData;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected HttpWebRequest GetWebRequest(Uri uri)
        {
            try {
                HttpWebRequest w = (HttpWebRequest)HttpWebRequest.Create(uri);
                if (_method == MethodRequest.POST)
                {
                    w.Method = "POST";
                }
                else if (_method == MethodRequest.DELETE)
                {
                    w.Method = "DELETE";
                }
                else if (_method == MethodRequest.PUT)
                {
                    w.Method = "PUT";
                }
                w.Timeout = _timeout;
                return w;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}