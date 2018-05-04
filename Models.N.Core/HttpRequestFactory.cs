using System;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BGBA.Models.N.Core.Trace;

namespace BGBA.Services.N.Core.HttpClient
{
    public class HttpRequestFactory : ITraceable
    {
        public string Request { get;  set; }
        public string Response { get; set; }
        public long ElapsedTime { get; set; }

        public async Task<HttpResponseMessage> Get(string requestUri)
            => await Get(requestUri, "", null);

        public  async Task<HttpResponseMessage> Get(string requestUri, string bearerToken)
            => await Get(requestUri, bearerToken, null);
        public async Task<HttpResponseMessage> Get(string requestUri, X509Certificate2 certificate)
            => await Get(requestUri, "", certificate);

        public  async Task<HttpResponseMessage> Get(string requestUri, string bearerToken, X509Certificate2 certificate)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken)
                                .AddCertificcate(certificate);

            this.Request = requestUri;

            var stopWatch = Stopwatch.StartNew();

            var result = await builder.SendAsync();

            this.ElapsedTime = stopWatch.ElapsedMilliseconds;
            this.Response = result.ContentAsString();

            return result;
        }

        public  async Task<HttpResponseMessage> Post(string requestUri, object value)
            => await Post(requestUri, value, "", null);

        public async Task<HttpResponseMessage> Post(string requestUri, object value, X509Certificate2 certificate)
            => await Post(requestUri, value, "", certificate);

        public  async Task<HttpResponseMessage> Post(
            string requestUri, SoapJsonContent value)
        => await Post(requestUri, value, "", null);

        public  async Task<HttpResponseMessage> Post(
            string requestUri, SoapJsonContent value, X509Certificate2 certificate)
            => await Post(requestUri, value, "", certificate, new TimeSpan(0, 0, 15));

        public async Task<HttpResponseMessage> Post(
            string requestUri, SoapJsonContent value, X509Certificate2 certificate, TimeSpan timeout)
            => await Post(requestUri, value, "", certificate, timeout);

        public  async Task<HttpResponseMessage> Post(
            string requestUri, SoapJsonContent value, string bearerToken, X509Certificate2 certificate, TimeSpan timeout)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(value)
                                .AddBearerToken(bearerToken)
                                .AddCertificcate(certificate)
                                .AddTimeout(timeout);


            this.Request = await value.ReadAsStringAsync();
            var stopWatch = Stopwatch.StartNew();

            var result = await builder.SendAsync();

            this.ElapsedTime = stopWatch.ElapsedMilliseconds;
            this.Response = result.ContentAsString();
            
            return result ;
        }
        
        public  async Task<HttpResponseMessage> Post(
            string requestUri, Object value, string bearerToken, X509Certificate2 certificate)
        {
            var content = new JsonContent(value);

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(content)
                                .AddBearerToken(bearerToken)
                                .AddCertificcate(certificate);

            this.Request = await content.ReadAsStringAsync();
            var stopWatch = Stopwatch.StartNew();

            var result = await builder.SendAsync();

            this.ElapsedTime = stopWatch.ElapsedMilliseconds;
            this.Response = result.ContentAsString();

            return result;
        }

        public  async Task<HttpResponseMessage> Put(string requestUri, object value)
            => await Put(requestUri, value, "");

        public  async Task<HttpResponseMessage> Put(
            string requestUri, object value, string bearerToken)
        {
            var content = new JsonContent(value);

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddContent(content)
                                .AddBearerToken(bearerToken);

            this.Request = await content.ReadAsStringAsync();
            var stopWatch = Stopwatch.StartNew();

            var result = await builder.SendAsync();

            this.ElapsedTime = stopWatch.ElapsedMilliseconds;
            this.Response = result.ContentAsString();

            return result;
        }

        public  async Task<HttpResponseMessage> Patch(string requestUri, object value)
            => await Patch(requestUri, value, "");

        public  async Task<HttpResponseMessage> Patch(
            string requestUri, object value, string bearerToken)
        {
            var content = new JsonContent(value);
            var builder = new HttpRequestBuilder()
                                .AddMethod(new HttpMethod("PATCH"))
                                .AddRequestUri(requestUri)
                                .AddContent(content)
                                .AddBearerToken(bearerToken);

            this.Request = await content.ReadAsStringAsync();
            var stopWatch = Stopwatch.StartNew();

            var result = await builder.SendAsync();

            this.ElapsedTime = stopWatch.ElapsedMilliseconds;
            this.Response = result.ContentAsString();

            return result;
        }

        public  async Task<HttpResponseMessage> Delete(string requestUri)
            => await Delete(requestUri, "");

        public  async Task<HttpResponseMessage> Delete(
            string requestUri, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Delete)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken);

            this.Request = requestUri;
            var stopWatch = Stopwatch.StartNew();

            var result = await builder.SendAsync();

            this.ElapsedTime = stopWatch.ElapsedMilliseconds;
            this.Response = result.ContentAsString();

            return result;
        }

        public  async Task<HttpResponseMessage> PostFile(string requestUri,
            string filePath, string apiParamName)
            => await PostFile(requestUri, filePath, apiParamName, "");

        public  async Task<HttpResponseMessage> PostFile(string requestUri,
            string filePath, string apiParamName, string bearerToken)
        {
            var content = new FileContent(filePath, apiParamName);

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(content)
                                .AddBearerToken(bearerToken);

            this.Request = await content.ReadAsStringAsync();
            var stopWatch = Stopwatch.StartNew();

            var result = await builder.SendAsync();

            this.ElapsedTime = stopWatch.ElapsedMilliseconds;
            this.Response = result.ContentAsString();

            return result;
        }
    }
}
