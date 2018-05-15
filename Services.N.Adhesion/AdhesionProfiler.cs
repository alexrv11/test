using System;
using System.Linq;
using AutoMapper;
using BGBA.Models.N.Adhesion;

namespace Services.N.Adhesion
{
    public class AdhesionProfiler : Profile
    {
        public AdhesionProfiler()
        {
            CreateMap<DatosAdhesion, BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatos>()
                .ForMember(d => d.AdhesionCliente, opt => opt.MapFrom(s => new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatosAdhesionCliente
                {
                    Documentos = new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.documento[] {
                        new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.documento{
                            Numero = new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.id { Value = s.NroDocumento},
                            Tipo = new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.codigo{  Value = s.TipoDocumento}
                        }
                    },
                    IdPersona = new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.id { Value = s.IdHost }
                }))
                .ForMember(d => d.AdhesionProducto, opt => opt.MapFrom(s => new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatosAdhesionProducto
                {
                    Productos = s.ProductosAdheribles.Select(p => new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatosAdhesionProductoProducto
                    {
                        Codigo = new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.codigo { Value = p.Codigo},
                        Numero = Convert.ToUInt64(p.Numero),
                        TarjetaCuentaCredito = Convert.ToUInt64(p.TarjetaCuentaCredito)
                    }).ToArray()
                }))
                .ForMember(d => d.Claves, opt => opt.MapFrom(s => new BGBA.Services.N.Adhesion.BUS.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatosClaves
                {
                    SistemaCentralSeguridad = s.PinEncriptado
                }))
                .ForMember(d => d.Funcion, opt => opt.MapFrom(s => 1));

            CreateMap<DatosAdhesion, BGBA.Services.N.Adhesion.BUS.AdministracionUsuarioHomebanking.CrearUsuarioRequestDatos>()
                .ForMember(d => d.IdUsuario, opt => opt.MapFrom(s => new BGBA.Services.N.Adhesion.BUS.AdministracionUsuarioHomebanking.id { Value = s.UsuarioAlfanumerico))
                .ForMember(d => d.NumeroAdhesionClienteCanalesAlternativos, opt => opt.MapFrom(s => new BGBA.Services.N.Adhesion.BUS.AdministracionUsuarioHomebanking.id { Value = s.IdAdhesion));
        }
    }
}
