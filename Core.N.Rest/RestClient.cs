using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.N.Trace;
using Newtonsoft.Json;
using System.Net.Http.Formatting;

namespace Core.N.Rest
{
    public class RestClient : ITraceable
    {
        private HttpClient _client;

        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public string ContentType { get; set; }
        public string PayLoad { get; set; }
        public int TimeoutMilliseconds { get; set; }
        private System.Diagnostics.Stopwatch Watch { set; get; }



        public string Request { get; set; }
        public string Response { get; set; }
        public int ElapsedTime { get; set; }

        public RestClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> GetAsync<T>() {

            var result = default(T);

            this.Request = this.Url + this.PayLoad;
            this.Watch.Start();

            var call = await _client.GetAsync(this.Url + this.PayLoad);

            this.Watch.Stop();
            this.TimeoutMilliseconds = (int)this.Watch.ElapsedMilliseconds;
            this.Response = await call.Content.ReadAsStringAsync();

            if (call.IsSuccessStatusCode)
                result = JsonConvert.DeserializeObject<T>(this.Response);

            return result;
        }

        public async Task<MsgOut> PostJsonAsync<MsgOut, MsgIn>(MsgIn msgIn)
        {
            var result = default(MsgOut);

            this.Request = JsonConvert.SerializeObject(msgIn,new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});
            this.Watch.Start();

            var call = await _client.PostAsJsonAsync(this.Url,msgIn);

            this.Watch.Stop();
            this.TimeoutMilliseconds = (int)this.Watch.ElapsedMilliseconds;
            this.Response = await call.Content.ReadAsStringAsync();

            if (call.IsSuccessStatusCode)
                result = JsonConvert.DeserializeObject<MsgOut>(this.Response);

            return result;
        }
    }
}
