﻿using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using BGBA.Models.N.Core.Trace;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Models.N.Adhesion;
using System.Security.Cryptography.X509Certificates;
using BGBA.Services.N.Core.HttpClient;
using AutoMapper;

namespace BGBA.Services.N.Adhesion
{
    public class AdhesionServices : TraceServiceBase
    {
        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;
        private readonly X509Certificate2 _certificate;
        private readonly IMapper _mapper;

        public AdhesionServices(IConfiguration configuration, IObjectFactory objectFactory, X509Certificate2 cert, IMapper mapper)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _certificate = cert;
            _mapper = mapper;
        }

        public async Task<string> AdherirUsuario(DatosAdhesion datos)
        {
            var service = new HttpRequestFactory();
            var url = _configuration["AdhesionPin:Url"];
            var isError = false;

            var request = new BUS.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequest
            {
                BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.AccionesAdhesionBancaAutomatica.BGBAHeader>(_configuration["AdhesionPin:Header"]),
                Datos = _mapper.Map<DatosAdhesion, BUS.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatos>(datos)
            };

            try
            {
                var response = (await service.Post(url, new SoapJsonContent(request, _configuration["AdhesionPin:Operation"]), _certificate)).ContentAsType<BUS.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaResponse>();

                if (response.BGBAResultadoOperacion.Severidad == BUS.AccionesAdhesionBancaAutomatica.severidad.ERROR)
                    throw new Exception($"Error en la respuesta del servicio: Codigo={response.BGBAResultadoOperacion.Codigo}, Descripcion={response.BGBAResultadoOperacion.Descripcion}");

                return response.Datos.NumeroAdhesionClienteCanalesAlternativos.ToString();
            }
            catch (Exception e)
            {
                isError = true;
                throw new InvalidOperationException("Error al realizar el servicio.", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URL = url, Request = service.Request, Response = service.Response, IsError = isError });
            }
        }

        public async Task<string> AltaAlfanumerico(DatosAdhesion datos)
        {
            var service = new HttpRequestFactory();
            var url = _configuration["AdhesionAlfanumerico:Url"];
            var isError = false;

            var request = new BUS.AdministracionUsuarioHomebanking.CrearUsuarioRequest
            {
                BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.AdministracionUsuarioHomebanking.BGBAHeader>(_configuration["AdhesionAlfanumerico:Header"]),
                Datos = _mapper.Map<DatosAdhesion, BUS.AdministracionUsuarioHomebanking.CrearUsuarioRequestDatos>(datos)
            };

            try
            {
                var response = (await service.Post(url, new SoapJsonContent(request, _configuration["AdhesionAlfanumerico:Operation"]), _certificate)).ContentAsType<BUS.AdministracionUsuarioHomebanking.CrearUsuarioResponse>();

                if (response.BGBAResultadoOperacion.Severidad == BUS.AdministracionUsuarioHomebanking.severidad.ERROR)
                    throw new Exception($"Error en la respuesta del servicio: Codigo={response.BGBAResultadoOperacion.Codigo}, Descripcion={response.BGBAResultadoOperacion.Descripcion}");

                return response.BGBAResultadoOperacion.Codigo;

            }
            catch (Exception e)
            {
                isError = true;
                throw new InvalidOperationException("Error realizar el llamado al servicio.", e);
            }
            finally
            {
                this.Communicator_TraceHandler(this, new TraceEventArgs() { ElapsedTime = service.ElapsedTime, URL = url, Request = service.Request, Response = service.Response, IsError = isError });
            }
        }
    }
}
