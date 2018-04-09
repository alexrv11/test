using System;
using System.Threading.Tasks;
using Core.N.Trace;
using Models.N.Consulta.Padron;
using Microsoft.Extensions.Configuration;

namespace Services.N.Consulta.Padron
{
    public class PadronServices : TraceServiceBase, IPadronServices
    {
        private IConfiguration _configuration;
        public PadronServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DatosPadron> ConsultaPadron(string cuix)
        {


            try
            {

            }
            catch (Exception e)
            {

                throw;
            }


            var service = new  Services.N.Core.Rest.RestServices{
                ContentType = "application/json",
                TimeoutMilliseconds = Convert.ToInt32(_configuration["ConsultaPadron:TimeoutMilliseconds"]),
                Method = "POST",
                Url = _configuration["ConsultaPadron:Url"],
                PayLoad = ""
            };

            return await service.ExecuteAsync<DatosPadron>();

        }

        //public async Task<T> ConsultarCliente<T>(object data)
        //{
        //    BGBA.Edge.Core.Rest.RestServices service = null;

        //    try
        //    {
        //        BGBA.Edge.Core.Configuration.WebServiceConfig wsConfig = GetConfiguration().GetConfigWS("ConsultaTablas");
        //        ForceDebug = wsConfig.IsDebugEnabled;

        //        var url = 

        //        service = new Core.Rest.RestServices
        //        {
        //            ContentType = "application/json",
        //            Method = "post",
        //            Url = url
        //        };

        //        var result = await service.ExecuteAsync<T>();

        //        this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URI = service.Url, Request = service.Request, Response = service.Response });

        //        return result;

        //    }
        //    catch (Exception e)
        //    {
        //        ServiceErrorHandler(new TraceEventArgs()
        //        {
        //            ElapsedTime = service.ElapsedTime,
        //            URI = service.Url,
        //            Request = service.Request,
        //            Response = service.Response,
        //            IsError = true
        //        }
        //       , e
        //       , "GT" + tableName);
        //        throw;
        //    }
        //}


    }
}
