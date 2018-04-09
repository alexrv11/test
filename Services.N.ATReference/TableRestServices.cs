using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.N.Trace;
using Microsoft.Extensions.Configuration;
using Services.N.Core.Rest;

namespace Services.N.Consulta.ATReference
{
    public class TableRestServices : ITraceable
    {
        private readonly IConfiguration _configuration;

        public string Request { get; set; }
        public string Response { get; set; }
        public int ElapsedTime { get; set; }

        public TableRestServices(IConfiguration configuration) {
            _configuration = configuration;
        }

        /// <summary>
        /// Consulta tabla de Atreference por GET. Acepta solamente 1 filtro. Formato de los Filtros: Key=Columna, Value=Nombre de columna, Key=Valor, Value= Valor a filtar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<T> GetTableByGet<T>(string tableName, Dictionary<string,string> filters)
        {
            RestServices service = null;

            try
            {
                var url = string.Format
                            (_configuration["ATReference:Url"], tableName,
                                string.Join("&", filters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))
                            );
                
                service = new RestServices()
                {
                    ContentType = "application/json",
                    Method = "GET",
                    Url = url
                };

                var result = await service.ExecuteAsync<T>();

                return result;
                
            }
            catch (Exception)
            {
                throw;
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
            RestServices service = null;

            try
            {
                var url = string.Format
                            (_configuration["ATReference:Url"], tableName, "");

                service = new RestServices()
                {
                    ContentType = "application/x-www-form-urlencoded",
                    Method = "POST",
                    Url = url,
                    PayLoad = string.Join("&", filters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))
                };

                var result = await service.ExecuteAsync<T>();

                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
