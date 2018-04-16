using System;
using System.Linq;
using AutoMapper;
using Services.N.Client.BUS;

namespace Services.N.Client
{
    public class ClientProfiler : Profile
    {
        public ClientProfiler()
        {
            CreateMap<Models.N.Client.PadronData, Services.N.Client.BUS.Persona1>()
                .ForMember(d => d.NombrePersona, opt => opt.MapFrom(s => new CrearClienteDatosBasicosRequestDatosPersonaNombrePersona
                {
                    Apellido = s.LastName,
                    Nombre = s.Name
                }))
                .ForMember(d => d.DatosNacimiento, opt => opt.MapFrom(s => new DatosNacimiento
                {
                    CodigoNacionalidad = "080",
                    CodigoPais = "080",
                    FechaNacimiento = s.Birthdate
                }))
                .ForMember(d => d.Documentos, opt => opt.MapFrom(s => new documento1[] {
                    new documento1{
                        Numero = s.DocumentNumber ,
                        Tipo = s.DocumentType 
                    }
                }))
                .ForMember(d => d.IdentificacionTributariaNumero, opt => opt.MapFrom(s => s.CuixNumber))
                .ForMember(d => d.IdentificacionTributariaTipo, opt => opt.MapFrom(s => s.CuixCode))
                .ForMember(d => d.CodigoEstadoCivil, opt => opt.MapFrom(s => "S"))
                .ForMember(d => d.CodigoSexo, opt => opt.MapFrom(s => s.Sex.Substring(0, 1)))
                .ForMember(d => d.CodigoProfesion, opt => opt.MapFrom(s => 0))
                .ForMember(d => d.CodigoEjerceProfesion, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.CantidadPersonalOcupado, opt => opt.MapFrom(s => 0))
                .ForMember(d => d.CodigoActividadPrincipal, opt => opt.MapFrom(s => 0))
                .ForMember(d => d.CodigoCondicionImpositivaIBR, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.CodigoCondicionImpositivaIVA, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.CodigoJurisdiccionIBR, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.CodigoRelacionVivienda, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.CodigoSituacionLaboral, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.IngresosAnuales, opt => opt.MapFrom(s => 0))
                .ForMember(d => d.MarcaPracticaAjuste, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.NumeroIdentificacionIBR, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.NumeroImportadorExportador, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.NumeroRegistroIndustria, opt => opt.MapFrom(s => ""))
                .ForMember(d => d.PatrimonioNeto, opt => opt.MapFrom(s => 0));


            CreateMap<Models.N.Client.PadronData, BUS.PersonaFisicaConOrganizacionCredito>()
            .ForMember(d => d.Persona, opt => opt.MapFrom(s => Mapper.Map<Models.N.Client.PadronData, Persona1>(s)));

            CreateMap<Models.N.Client.PadronData, Services.N.Client.BUS.CrearPersonaFisica>()
            .ForMember(d => d.PersonaFisicaConOrganizacionCredito, opt => opt.MapFrom(s => Mapper.Map<Models.N.Client.PadronData,BUS.PersonaFisicaConOrganizacionCredito>(s)))
            .ForMember(d => d.Domicilio, opt => opt.MapFrom(s => new Domicilio1
            {
                Direccion = new domicilio2salida
                {
                    Calle = s.Addresses.FirstOrDefault(a => a.Default).Street,
                    CodigoPais = s.Addresses.FirstOrDefault(a => a.Default).CoutryCode,
                    CPA = s.Addresses.FirstOrDefault(a => a.Default).PostalCode,
                    CodigoUsoPersona = "PA",
                    Departamento = s.Addresses.FirstOrDefault(a => a.Default).FlatNumber,
                    Piso = s.Addresses.FirstOrDefault(a => a.Default).Floor,
                    CodigoProvincia = s.Addresses.FirstOrDefault(a => a.Default).Province.Code,
                    NumeroPuerta = s.Addresses.FirstOrDefault(a => a.Default).Number,
                    Latitud = Convert.ToDecimal(s.Addresses.FirstOrDefault(a => a.Default).Location.Latitude),
                    Longitud = Convert.ToDecimal(s.Addresses.FirstOrDefault(a => a.Default).Location.Longitude)
                }
            }))
            //.ForMember(d => d.Telefonos, opt => opt.MapFrom(s => s.Phones.Select(p => new telefonoBasico1
            //{
            //    Basico = new telefonoBasicoRespuestaNV1
            //    {
            //        celular = (p.LineType != "FIJO").ToString(),
            //        Numero = p.Number
            //    },
            //    CodigoUso = "PA",
            //})))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Emails.FirstOrDefault()));
        }
    }
}
