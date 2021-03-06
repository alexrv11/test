﻿using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BGBA.Models.N.Core.Trace;
using Newtonsoft.Json;

namespace Services.N.Core.Rest
{
    public class RestServices : ITraceable
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public string PayLoad { get; set; }
        public int TimeoutMilliseconds { get; set; }
        public int StatusCode { get; set; }
        private System.Diagnostics.Stopwatch Watch { set; get; }
        public X509Certificate2 Certificate { get; set; }


        #region ITraceable
        public string Request { get; set; }
        public string Response { get; set; }
        public long ElapsedTime { get; set; }
        #endregion


        public RestServices()
        {
            this.TimeoutMilliseconds = 60000;
        }

        public async Task<MsgOut> ExecuteAsync<MsgOut, MsgIn>(MsgIn model)
        {
            this.PayLoad = JsonConvert.SerializeObject(model);
            return await this.ExecuteAsync<MsgOut>();
        }

        public async Task<MsgOut> ExecuteAsync<MsgOut>()
        {
            try
            {
                await this.ExecuteAsync();
                return JsonConvert.DeserializeObject<MsgOut>(this.Response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                this.Watch = System.Diagnostics.Stopwatch.StartNew();
                System.Net.HttpWebRequest webrequest = (HttpWebRequest)System.Net.WebRequest.Create(this.Url);                
                webrequest.Method = this.Method;
                webrequest.ContentType = this.ContentType;
                webrequest.Timeout = this.TimeoutMilliseconds;
                if (this.Certificate != null)
                {
                    webrequest.ServerCertificateValidationCallback = (message, cert, chain, errors) => true;
                    webrequest.ClientCertificates.Add(this.Certificate);
                }

                switch (this.Method)
                {
                    case "PUT":
                    case "POST":
                        var byteArray = Encoding.UTF8.GetBytes(this.PayLoad);
                        webrequest.ContentLength = byteArray.Length;
                        await (await webrequest.GetRequestStreamAsync()).WriteAsync(byteArray, 0, byteArray.Length);
                        break;
                    case "GET":
                    default:
                        webrequest.ContentLength = 0;
                        break;
                }

                this.Request = $"{Url} {PayLoad}";

                using (var response = await webrequest.GetResponseAsync())
                {
                    var responseStream = response.GetResponseStream();
                    this.Watch.Stop();
                    this.ElapsedTime = (int)Watch.ElapsedMilliseconds;
                    this.Response = await (new StreamReader(response.GetResponseStream())).ReadToEndAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                this.Watch.Stop();
                this.ElapsedTime = (int)Watch.ElapsedMilliseconds;
                if (e.GetType() == typeof(WebException) && ((WebException)e).Response != null)
                    this.Response = await (new StreamReader(((WebException)e).Response.GetResponseStream())).ReadToEndAsync();

                throw;
            }
        }
    }
}
