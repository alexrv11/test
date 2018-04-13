using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.N.Utils.ObjectFactory;
using Microsoft.Extensions.Configuration;
using Services.N.Core.HttpClient;

namespace Services.N.Administracion.Cliente
{
    public class AdministracionClienteServices
    {
        private readonly IConfiguration _configuration;
        private readonly IObjectFactory _objectFactory;

        public AdministracionClienteServices(IConfiguration configuration, IObjectFactory objectFactory) {
            _configuration = configuration;
            _objectFactory = objectFactory;
        }
        public async Task<bool> AddClient(BUS.CrearClienteDatosBasicosRequestDatos datos)
        {
            try
            {
                var request = new BUS.CrearClienteDatosBasicosRequest
                {
                    BGBAHeader = await _objectFactory.InstantiateFromJsonFile<BUS.BGBAHeader>(_configuration["AdministracioCliente:CrearCliente:BGBAHeader"]),
                    Datos = datos
                };

                var response = await HttpRequestFactory.Post(_configuration["AdministracioCliente:CrearCliente::Url"], new SoapJsonContent(request, "CrearCliente"));
                var soapResponse = response.SoapContentAsType<BUS.CrearClienteDatosBasicosResponse>();

                if (soapResponse.BGBAResultadoOperacion.Severidad == BUS.severidad.ERROR)
                    throw new Exception($"Error en la respuesta del servicio: Codigo={soapResponse.BGBAResultadoOperacion.Codigo}, Descripcion={soapResponse.BGBAResultadoOperacion.Descripcion}");

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error al generar el request", e);
            }
        }
    }
}
