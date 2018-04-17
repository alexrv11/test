using System.Linq;
using AutoMapper;
using Models.N.Client;
using Models.N.Location;

namespace Models.N.Afip
{
    public class AfipProfiler : Profile
    {

        public static string RealAddress = "LEGAL/REAL";
        public static string FiscalAddress = "FISCAL";

        public AfipProfiler()
        {
            CreateMap<AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales, Credentials>()
                .ForMember(d => d.Sign, opt => opt.MapFrom(s => s.Firma))
                .ForMember(d => d.Token, opt => opt.MapFrom(s => s.Token));

            CreateMap<persona, ClientData>()
                .ForMember(d => d.Birthdate, opt => opt.MapFrom(s => s.fechaNacimiento))
                .ForMember(d => d.CuixNumber, opt => opt.MapFrom(s => s.idPersona))
                .ForMember(d => d.CuixType, opt => opt.MapFrom(s => s.tipoClave))
                .ForMember(d => d.CuixCode, opt => opt.MapFrom(s => s.tipoClave == "CUIL"? "02" : "04")) //04CUIT
                .ForMember(d => d.DocumentNumber, opt => opt.MapFrom(s => s.numeroDocumento))
                .ForMember(d => d.DocumentType, opt => opt.MapFrom(s => s.tipoDocumento == "DNI" ? "DU":s.tipoDocumento))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.apellido))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.nombre))
                .ForMember(d => d.PersonType, opt => opt.MapFrom(s => s.tipoPersona))
                .ForMember(d => d.Sex, opt => opt.MapFrom(s => s.sexo))
                .ForMember(d => d.Phones, opt => opt.MapFrom(s => s.telefono.Select(t => new Phone
                {
                    LineType = t.tipoLinea,
                    Number = t.numero.ToString(),
                    PhoneType = t.tipoTelefono
                })))
                .ForMember(d => d.Addresses, opt => opt.MapFrom(s =>
                            s.domicilio.Select(d => new Address
                            {
                                AddressType = d.tipoDomicilio,
                                LocalityDescription = d.localidad,
                                Street = d.direccion,
                                PostalCode = d.codPostal,
                                Province = new Province { Name = d.descripcionProvincia },
                                AditionalData = d.datoAdicional,
                                AditionalDataType = d.tipoDatoAdicional,
                                Default = d.tipoDomicilio == RealAddress                                
                            })))
                .ForMember(d => d.Emails, opt => opt.MapFrom(s => s.email.Select(e => new Email
                {
                    Address = e.direccion,
                    State = e.estado,
                    Type = e.tipoEmail
                })));
        }

    }
}
