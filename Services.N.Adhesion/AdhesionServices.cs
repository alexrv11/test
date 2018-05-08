﻿using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using BGBA.Models.N.Core.Trace;
using BGBA.Models.N.Core.Utils.ObjectFactory;
using BGBA.Models.N.Adhesion;
using Services.N.Core.Rest;

namespace BGBA.Services.N.Adhesion
{
    public class AdhesionServices : TraceServiceBase
    {
        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;

        public AdhesionServices(IConfiguration configuration, IObjectFactory objectFactory)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
        }

        public async Task<string> AdherirUsuario(DatosAdhesion datos)
        {
            var service = new RestServices();

            Models.SoapCallAdhesionBancaAutomaticaResponse.Response response = null;


            try
            {
                service.ContentType = "application/json";
                service.TimeoutMilliseconds = Convert.ToInt32(_configuration["AdhesionPin:TimeoutMilliseconds"]);
                service.Method = "POST";
                service.Url = _configuration["AdhesionPin:Url"];
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error al instanciar el servicio", e);
            }

            try
            {
                var obj=   JObject.Parse(await File.ReadAllTextAsync(_configuration["AdhesionPin:Request"]));
                obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["Claves"]["SistemaCentralSeguridad"] = datos.PinEncriptado;
                obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["AdhesionCliente"]["IdPersona"] = datos.IdHost;
                obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["AdhesionCliente"]["Documentos"]["Documento"]["Tipo"]["$"] = datos.TipoDocumento;
                obj["Envelope"]["Body"]["AdherirClienteFisicoProductoBancaAutomatica"]["AdherirClienteFisicoProductoBancaAutomaticaRequest"]["Datos"]["AdhesionCliente"]["Documentos"]["Documento"]["Numero"]["$"] = datos.NroDocumento;

                service.PayLoad = obj.ToString();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error al generar el request", e);
            }

            try
            {
                response = await service.ExecuteAsync<Models.SoapCallAdhesionBancaAutomaticaResponse.Response>();

                if (response.Envelope.Body.AdherirClienteFisicoProductoBancaAutomaticaResult.AdherirClienteFisicoProductoBancaAutomaticaResponse.BGBAResultadoOperacion.Severidad == Models.AccionesAdhesionBancaAutomatica.severidad.ERROR)
                    throw new Exception($"Error en la respuesta del servicio: Codigo={response.Envelope.Body.AdherirClienteFisicoProductoBancaAutomaticaResult.AdherirClienteFisicoProductoBancaAutomaticaResponse.BGBAResultadoOperacion.Codigo}, Descripcion={response.Envelope.Body.AdherirClienteFisicoProductoBancaAutomaticaResult.AdherirClienteFisicoProductoBancaAutomaticaResponse.BGBAResultadoOperacion.Descripcion}");

                return response.Envelope.Body.AdherirClienteFisicoProductoBancaAutomaticaResult.AdherirClienteFisicoProductoBancaAutomaticaResponse.Datos.NumeroAdhesionClienteCanalesAlternativos.ToString();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error al realizar el servicio.", e);
            }
        }

        public async Task<string> AltaAlfanumerico(DatosAdhesion datos)
        {
            var service = new RestServices();
            Models.SoapCallAdministracionUsuarioHomebankingResponse.Response response = null;
            try
            {
                service.ContentType = "application/json";
                service.TimeoutMilliseconds = Convert.ToInt32(_configuration["AdhesionAlfanumerico:TimeoutMilliseconds"]);
                service.Method = "POST";
                service.Url = _configuration["AdhesionAlfanumerico:Url"];
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error al instanciar el servicio", e);
            }

            try
            {
                var obj = JObject.Parse(await File.ReadAllTextAsync(_configuration["AdhesionAlfanumerico:Request"]));
                obj["Envelope"]["Body"]["CrearUsuario"]["CrearUsuarioRequest"]["Datos"]["IdUsuario"] = datos.UsuarioAlfanumerico;
                obj["Envelope"]["Body"]["CrearUsuario"]["CrearUsuarioRequest"]["Datos"]["NumeroAdhesionClienteCanalesAlternativos"] = datos.IdAdhesion;
                service.PayLoad = obj.ToString();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error al generar el request.", e);
            }

            try
            {
                response = await service.ExecuteAsync<Models.SoapCallAdministracionUsuarioHomebankingResponse.Response>();

                if (response.Envelope.Body.CrearUsuarioResult.CrearUsuarioResponse.BGBAResultadoOperacion.Severidad == Models.AdministracionUsuarioHomebanking.severidad.ERROR)
                    throw new Exception($"Error en la respuesta del servicio: Codigo={response.Envelope.Body.CrearUsuarioResult.CrearUsuarioResponse.BGBAResultadoOperacion.Codigo}, Descripcion={response.Envelope.Body.CrearUsuarioResult.CrearUsuarioResponse.BGBAResultadoOperacion.Descripcion}");

                return response.Envelope.Body.CrearUsuarioResult.CrearUsuarioResponse.BGBAResultadoOperacion.Codigo;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error realizar el llamado al servicio.", e);
            }
        }
    }
}
