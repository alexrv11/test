﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Services.N.Core.HttpClient;
using Core.N.Utils.ObjectFactory;
using AutoMapper;
using Models.N.Client;
using Models.N.Afip;
using Newtonsoft.Json;
using Microsoft.CSharp.RuntimeBinder;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Services.N.Afip
{
    public class AfipServices : IAFIPServices
    {
        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly IMapper _mapper;
        private static Credentials _credentials;
        private static DateTime _endOfValidCredentials;

        public string Request { get; set; }
        public string Response { get; set; }
        public int ElapsedTime { get; set; }

        public AfipServices(IConfiguration configuration, IObjectFactory objectFactory, IMapper mapper)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _mapper = mapper;
        }

        public async Task<Credentials> GetCredentials()
        {

            var service = new HttpRequestFactory();

            if (IsValidCredentials())
                return _credentials;

            try
            {
                var now = DateTime.Now;

                var request = new AutenticarYAutorizarConsumoWebserviceRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BGBAHeader>(_configuration["GetCredentials:BGBAHeader"]),
                    Datos = new AutenticarYAutorizarConsumoWebserviceRequestDatos
                    {
                        IdRequerimiento = new Random(10000).Next(),
                        HoraDesde = now.ToString("yyyy-MM-ddTHH:mm:sss.fff"),
                        HoraHasta = now.AddMilliseconds(Convert.ToInt32(_configuration["GetCredentials:MillisecondsForValidToken"])).ToString("yyyy-MM-ddTHH:mm:sss.fff"),
                        ServicioAConsumir = _configuration["GetCredentials:ServicioAConsumir"]
                    }
                };


                //We read the bytes because the override of x509certificate2 ctor with cert path not work properly
                var b64String = Convert.ToBase64String((await File.ReadAllBytesAsync(_configuration["Certificate:Path"])));
                //var b64String = "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";
                //var cert = new X509Certificate2(_configuration["Certificate:Path"], _configuration["Certificate:Password"]);
                var cert = new X509Certificate2(Convert.FromBase64String(b64String), _configuration["Certificate:Password"]);

                var response = (await service.Post(_configuration["GetCredentials:Url"], new SoapJsonContent(request, _configuration["GetCredentials:Operation"]),cert))
                    .SoapContentAsJsonType<AutenticarYAutorizarConsumoWebserviceResponse>();
                

                if (response.BGBAResultadoOperacion.Severidad == severidad.ERROR)
                    throw new Exception($"{response.BGBAResultadoOperacion.Codigo},{response.BGBAResultadoOperacion.Descripcion}");

                _endOfValidCredentials = now.AddMilliseconds(Convert.ToInt32(_configuration["GetCredentials:MillisecondsForValidToken"]));
                _credentials = _mapper.Map<AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales,Credentials>(response.Datos.Credenciales);

                return _credentials;
            }
            catch (Exception e)
            {
                throw new Exception("Error getting credentials", e);
            }
        }

        public async Task<ClientData> GetClient(string cuix)
        {
            var service = new HttpRequestFactory();
            try
            {
                var credentials = await GetCredentials();
                var request = new getPersona
                {
                    cuitRepresentada = Convert.ToInt64(_configuration["GetClientAfip:BankCuit"]),
                    idPersona = Convert.ToInt64(cuix),
                    sign = credentials.Sign,
                    token = credentials.Token
                };

                var cert = new X509Certificate2(_configuration["Certificate:Path"], _configuration["Certificate:Password"]);

                var response = await service.Post(_configuration["GetClientAfip:Url"], new SoapJsonContent(request, _configuration["GetClientAfip:Operation"]), cert);
                dynamic dynamicResponse = JsonConvert.DeserializeObject<dynamic>(response.ContentAsString());

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception(response.ContentAsString());

                try
                {
                    if (dynamicResponse.Envelope.Body.Fault != null)
                        return null;
                }
                catch (RuntimeBinderException)
                {
                }

                return _mapper.Map<persona, ClientData>(response.SoapContentAsJsonType<getPersonaResponse>().personaReturn.persona);   
            }
            catch (Exception e)
            {
                throw new Exception("Error getting client", e);
            }
        }

        public bool IsValidCredentials()
        {
            return (_endOfValidCredentials - DateTime.Now).TotalMilliseconds > 0 && _credentials != null;
        }
    }
}
