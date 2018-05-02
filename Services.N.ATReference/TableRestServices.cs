using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.N.Core.Trace;
using Services.N.Core.HttpClient;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Services.N.ATReference
{
    public class TableRestServices : Models.N.Core.Trace.TraceServiceBase, ITableServices
    {
        private readonly IConfiguration _configuration;
        private readonly X509Certificate2 _cert;

        public TableRestServices(IConfiguration configuration, X509Certificate2 cert)
        {
            _configuration = configuration;
            _cert = cert;
        }

        /// <summary>
        /// Consulta tabla de Atreference por GET. Acepta solamente 1 filtro. Formato de los Filtros: Key=Columna, Value=Nombre de columna, Key=Valor, Value= Valor a filtar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<T> GetTableByGet<T>(string tableName, Dictionary<string, string> filters)
        {
            var service = new HttpRequestFactory();
            var url = $"{_configuration["ATReference:Url"]}/{tableName}?{string.Join("&", filters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))}";
            var isError = false;

            try
            {
                return (await service.Get(url,_cert)).ContentAsType<T>();
            }
            catch (Exception)
            {
                isError = true;
                throw;
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get table ATReference.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }

        /// <summary>
        /// Consulta tabla de Atreference por Post. Acepta más de un filtro. Formato de los Filtros: Key=Clave, Value= Valor a filtrar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<T> GetTableByPost<T>(string tableName, Dictionary<string, string> filters)
        {
            var service = new HttpRequestFactory();
            var url = $"{_configuration["ATReference:Url"]}/{tableName}";
            var isError = false;

            try
            {
                var content = new StringContent(string.Join("&", filters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), Encoding.UTF8, "application/x-www-form-urlencoded");
                return (await service.Post(url, content, _cert)).ContentAsType<T>();
            }
            catch (Exception)
            {
                isError = true;
                throw;
            }
            finally
            {
                this.Communicator_TraceHandler(this,
                    new TraceEventArgs
                    {
                        Description = "Get table ATReference.",
                        ElapsedTime = service.ElapsedTime,
                        ForceDebug = false,
                        IsError = isError,
                        Request = service.Request,
                        Response = service.Response,
                        URL = url
                    });
            }
        }
    }
}
