using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using BGBA.Models.N.Client;
using BGBA.Models.N.Location;

namespace BGBA.Models.N.Afip
{
    public class AfipProfiler : Profile
    {

        public static readonly string RealAddress = "LEGAL/REAL";
        public static readonly string FiscalAddress = "FISCAL";

        public AfipProfiler()
        {
            CreateMap<AutenticarYAutorizarConsumoWebserviceResponseDatosCredenciales, Credentials>()
                .ForMember(d => d.Sign, opt => opt.MapFrom(s => s.Firma))
                .ForMember(d => d.Token, opt => opt.MapFrom(s => s.Token));

            CreateMap<persona, ClientData>()
                .ForMember(d => d.Birthdate, opt => opt.MapFrom(s => s.fechaNacimiento))
                .ForMember(d => d.CuixNumber, opt => opt.MapFrom(s => s.idPersona))
                .ForMember(d => d.CuixType, opt => opt.MapFrom(s => s.tipoClave))
                .ForMember(d => d.CuixCode, opt => opt.MapFrom(s => s.tipoClave == "CUIL" ? "02" : "04")) //04CUIT
                .ForMember(d => d.DocumentNumber, opt => opt.MapFrom(s => s.numeroDocumento))
                .ForMember(d => d.DocumentType, opt => opt.MapFrom(s => s.tipoDocumento == "DNI" ? "DU" : s.tipoDocumento))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.apellido))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.nombre))
                .ForMember(d => d.PersonType, opt => opt.MapFrom(s => s.tipoPersona))
                .ForMember(d => d.Sex, opt => opt.MapFrom(s => s.sexo))
                .ForMember(d => d.Phones, opt => opt.MapFrom(s => s.telefono.Select(t => new Phone
                {
                    IsCellphone = t.tipoLinea != "FIJO",
                    Number = t.numero.ToString(),
                    PhoneType = t.tipoTelefono
                })))
                .ForMember(d => d.Addresses, opt => opt.MapFrom(s =>
                            s.domicilio.Select(d => NormalizeAddressAfip(d))))
                .ForMember(d => d.Emails, opt => opt.MapFrom(s => s.email.Select(e => new Email
                {
                    Address = e.direccion,
                    State = e.estado,
                    Type = e.tipoEmail
                })));
        }

        public Address NormalizeAddressAfip(domicilio d)
        {

            var address = new Address
            {
                AddressType = d.tipoDomicilio,
                LocalityDescription = d.localidad,
                PostalCode = d.codPostal,
                Province = new Province { Name = d.descripcionProvincia },
                AditionalData = d.datoAdicional,
                AditionalDataType = d.tipoDatoAdicional,
                Default = d.tipoDomicilio == RealAddress
            };

            var fullAddress = Regex.Match(d.direccion, @"(([a-z0-9 °]+) (\d+))", RegexOptions.IgnoreCase);
            var floor = Regex.Match(d.direccion, @"(p|piso):([a-z0-9]+)", RegexOptions.IgnoreCase);
            var flat = Regex.Match(d.direccion, @"(d|dpto|departamento):([a-z0-9]+)", RegexOptions.IgnoreCase);

            if (fullAddress.Success)
            {
                address.Street = fullAddress.Groups[2].Value;
                address.Number = fullAddress.Groups[3].Value;
            }
            if (floor.Success)
                address.Floor = floor.Groups[2].Value;
            if (flat.Success)
                address.FlatNumber = flat.Groups[2].Value;

            return address;
        }

    }
}
