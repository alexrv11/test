using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Services.N.Core.HttpClient
{
    public static class HttpRequestFactory
    {
        public static async Task<HttpResponseMessage> Get(string requestUri)
            => await Get(requestUri, "", null);

        public static async Task<HttpResponseMessage> Get(string requestUri, string bearerToken)
            => await Get(requestUri, bearerToken, null);

        public static async Task<HttpResponseMessage> Get(string requestUri, string bearerToken, X509Certificate2 certificate)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken)
                                .AddCertificcate(certificate);

            return await builder.SendAsync();
        }

        public static async Task<HttpResponseMessage> Post(string requestUri, object value)
            => await Post(requestUri, value, "", null);

        public static async Task<HttpResponseMessage> Post(
            string requestUri, SoapJsonContent value)
        => await Post(requestUri, value, "", null);

        public static async Task<HttpResponseMessage> Post(
            string requestUri, SoapJsonContent value, X509Certificate2 certificate)
            => await Post(requestUri, value, "", certificate);

        public static async Task<HttpResponseMessage> Post(
            string requestUri, SoapJsonContent value, string bearerToken, X509Certificate2 certificate)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(value)
                                .AddBearerToken(bearerToken)
                                .AddCertificcate(certificate);

            return await builder.SendAsync();
        }
        
        public static async Task<HttpResponseMessage> Post(
            string requestUri, object value, string bearerToken, X509Certificate2 certificate)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddBearerToken(bearerToken)
                                .AddCertificcate(certificate);

            return await builder.SendAsync();
        }

        public static async Task<HttpResponseMessage> Put(string requestUri, object value)
            => await Put(requestUri, value, "");

        public static async Task<HttpResponseMessage> Put(
            string requestUri, object value, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync();
        }

        public static async Task<HttpResponseMessage> Patch(string requestUri, object value)
            => await Patch(requestUri, value, "");

        public static async Task<HttpResponseMessage> Patch(
            string requestUri, object value, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(new HttpMethod("PATCH"))
                                .AddRequestUri(requestUri)
                                .AddContent(new PatchContent(value))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync();
        }

        public static async Task<HttpResponseMessage> Delete(string requestUri)
            => await Delete(requestUri, "");

        public static async Task<HttpResponseMessage> Delete(
            string requestUri, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Delete)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync();
        }

        public static async Task<HttpResponseMessage> PostFile(string requestUri,
            string filePath, string apiParamName)
            => await PostFile(requestUri, filePath, apiParamName, "");

        public static async Task<HttpResponseMessage> PostFile(string requestUri,
            string filePath, string apiParamName, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new FileContent(filePath, apiParamName))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync();
        }
    }
}
