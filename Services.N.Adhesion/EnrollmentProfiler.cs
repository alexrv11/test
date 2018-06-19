using System;
using System.Linq;
using AutoMapper;
using BGBA.Models.N.Client.Enrollment;

namespace BGBA.Services.N.Enrollment
{
    public class EnrollmentProfiler : Profile
    {
        public EnrollmentProfiler()
        {
            CreateMap<EnrollmentData, BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatos>()
                .ForMember(d => d.AdhesionCliente, opt => opt.MapFrom(s => new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatosAdhesionCliente
                {
                    Documentos = new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.documento[] {
                        new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.documento{
                            Numero = new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.id { Value = s.DocumentNumber},
                            Tipo = new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.codigo{  Value = s.DocumentType}
                        }
                    },
                    IdPersona = new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.id { Value = s.IdHost }
                }))
                .ForMember(d => d.AdhesionProducto, opt => opt.MapFrom(s => new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatosAdhesionProducto
                {
                    Productos = s.EnrollmentProducts.Select(p => new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatosAdhesionProductoProducto
                    {
                        Codigo = new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.codigo { Value = p.Code },
                        Numero = Convert.ToUInt64(p.Number),
                        TarjetaCuentaCredito = Convert.ToUInt64(p.CardCreditAccount)
                    }).ToArray()
                }))
                .ForMember(d => d.Claves, opt => opt.MapFrom(s => new BGBA.Services.N.Enrollment.Models.AccionesAdhesionBancaAutomatica.AdherirClienteFisicoProductoBancaAutomaticaRequestDatosClaves
                {
                    SistemaCentralSeguridad = s.EncryptedPin
                }))
                .ForMember(d => d.Funcion, opt => opt.MapFrom(s => 1))
                .ForMember(d => d.FuncionSpecified, opt => opt.MapFrom(s => true));

            CreateMap<EnrollmentData, BGBA.Services.N.Enrollment.Models.AdministracionUsuarioHomebanking.CrearUsuarioRequestDatos>()
                .ForMember(d => d.IdUsuario, opt => opt.MapFrom(s => new BGBA.Services.N.Enrollment.Models.AdministracionUsuarioHomebanking.id { Value = s.AlphanumericUser }))
                .ForMember(d => d.NumeroAdhesionClienteCanalesAlternativos, opt => opt.MapFrom(s => new BGBA.Services.N.Enrollment.Models.AdministracionUsuarioHomebanking.id { Value = s.EnrollNumber }));

            CreateMap<string, BGBA.Services.N.Enrollment.Models.ConsultaClienteCanalesAlternativos.BuscarClienteFisicoCanalesAlternativosPorIdentificacionRequestDatos>()
                .ForMember(d => d.Cliente, opt => opt.MapFrom(s => new BGBA.Services.N.Enrollment.Models.ConsultaClienteCanalesAlternativos.BuscarClienteFisicoCanalesAlternativosPorIdentificacionRequestDatosCliente
                {
                    NumeroDocumento = Convert.ToUInt64(s)
                }));

            CreateMap<BGBA.Services.N.Enrollment.Models.ConsultaClienteCanalesAlternativos.BuscarClienteFisicoCanalesAlternativosPorIdentificacionResponseDatosDetalleCliente, BGBA.Models.N.Client.Enrollment.EnrolledClient>()
                .ForMember(d => d.BirthDate, opt => opt.MapFrom(s => s.FechaNacimiento))
                .ForMember(d => d.DocumentNumber, opt => opt.MapFrom(s => s.Documento.FirstOrDefault().Numero))
                .ForMember(d => d.DocumentType, opt => opt.MapFrom(s => s.Documento.FirstOrDefault().Tipo))
                .ForMember(d => d.HostId, opt => opt.MapFrom(s => s.IdPersona.Value))
                .ForMember(d => d.Sex, opt => opt.MapFrom(s => s.sexo))
                .ForMember(d => d.CentralSystemSecurityCodeId, opt => opt.MapFrom(s => s.AdhesionCanalesAlternativos.IdClaveSistemaCentralSeguridad.Value))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.AdhesionCanalesAlternativos.Estado))
                .ForMember(d => d.EnrollNumber, opt => opt.MapFrom(s => s.AdhesionCanalesAlternativos.Numero));
        }
    }
}
