using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Services.N.Client.BUS;

namespace Services.N.Client
{
    public class ClientProfiler : Profile
    {
        public ClientProfiler()
        {
            CreateMap<Models.N.Client.ClientData, Services.N.Client.BUS.AdministracionCliente.Persona1>()
                .ForMember(d => d.NombrePersona, opt => opt.MapFrom(s => new BUS.AdministracionCliente.CrearClienteDatosBasicosRequestDatosPersonaNombrePersona
                {
                    Apellido = s.LastName,
                    Nombre = s.Name
                }))
                .ForMember(d => d.DatosNacimiento, opt => opt.MapFrom(s => new BUS.AdministracionCliente.DatosNacimiento
                {
                    CodigoNacionalidad = "080",
                    CodigoPais = "080",
                    FechaNacimiento = s.Birthdate
                }))
                .ForMember(d => d.Documentos, opt => opt.MapFrom(s => new BUS.AdministracionCliente.documento1[] {
                    new BUS.AdministracionCliente.documento1{
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


            CreateMap<Models.N.Client.ClientData, BUS.AdministracionCliente.PersonaFisicaConOrganizacionCredito>()
            .ForMember(d => d.Persona, opt => opt.MapFrom(s => Mapper.Map<Models.N.Client.ClientData, BUS.AdministracionCliente.Persona1>(s)));

            CreateMap<Models.N.Client.ClientData, BUS.AdministracionCliente.CrearPersonaFisica>()
            .ForMember(d => d.PersonaFisicaConOrganizacionCredito, opt => opt.MapFrom(s => Mapper.Map<Models.N.Client.ClientData, BUS.AdministracionCliente.PersonaFisicaConOrganizacionCredito>(s)))
            .ForMember(d => d.Domicilio, opt => opt.MapFrom(s => new BUS.AdministracionCliente.Domicilio1
            {
                Direccion = new BUS.AdministracionCliente.domicilio2salida
                {
                    Calle = s.Addresses.FirstOrDefault(a => a.Default).Street,
                    CodigoPais = s.Addresses.FirstOrDefault(a => a.Default).Country.Code,
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
            //.ForMember(d => d.Domicilio, opt => opt.MapFrom(s => new BUS.AdministracionCliente.Domicilio1
            //{
            //    Direccion = new BUS.AdministracionCliente.domicilio2salida
            //    {
            //        Calle = s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().Street,
            //        CodigoPais = s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().CountryCode,
            //        CPA = s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().PostalCode,
            //        CodigoUsoPersona = "PA",
            //        Departamento = s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().FlatNumber,
            //        Piso = s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().Floor,
            //        CodigoProvincia = s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().Province.Code,
            //        NumeroPuerta = s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().Number,
            //        Latitud = Convert.ToDecimal(s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().Location.Latitude),
            //        Longitud = Convert.ToDecimal(s.Addresses.OrderByDescending(a => a.Default).FirstOrDefault().Location.Longitude)
            //    }
            //}))
            //.ForMember(d => d.Telefonos, opt => opt.MapFrom(s => s.Phones.Select(p => new telefonoBasico1
            //{
            //    Basico = new telefonoBasicoRespuestaNV1
            //    {
            //        celular = (p.LineType != "FIJO").ToString(),
            //        Numero = p.Number
            //    },
            //    CodigoUso = "PA",
            //})))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Emails.FirstOrDefault().Address));

            CreateMap<Models.N.Client.ClientData, BUS.ConsultaCliente.Datos>()
                .ForMember(d => d.CriterioBusqueda, opt => opt.MapFrom(s => new BUS.ConsultaCliente.DatosCriterioBusqueda
                {
                    Item = new BUS.ConsultaCliente.documento1
                    {
                        Numero = new BUS.ConsultaCliente.id { Value = s.DocumentNumber },
                        Tipo = new BUS.ConsultaCliente.codigo
                        {
                            Value = s.DocumentType
                        }
                    }
                }))
                .ForMember(d => d.SeleccionTipoPersona, opt => opt.MapFrom(s => "01"));

            CreateMap<BUS.ConsultaCliente.PersonaFisica1, Models.N.Client.ClientData>()
                .ForMember(d => d.Addresses, opt => opt.MapFrom(s => new List<Models.N.Location.Address> {
                    new Models.N.Location.Address{
                        Country = new Models.N.Location.Country{ Code = s.DatosPersonaComunes.Domicilio.CodigoPais} ,
                        FlatNumber = s.DatosPersonaComunes.Domicilio.Departamento,
                        Floor = s.DatosPersonaComunes.Domicilio.Piso,
                        Number = s.DatosPersonaComunes.Domicilio.NumeroPuerta,
                        Province = new Models.N.Location.Province{
                            Code = s.DatosPersonaComunes.Domicilio.CodigoProvincia
                        },
                        PostalCode = s.DatosPersonaComunes.Domicilio.CodigoPostal,
                       LocalityDescription = s.DatosPersonaComunes.Domicilio.NombreLocalidad,
                       Street = s.DatosPersonaComunes.Domicilio.Calle
                    }
                }))
                .ForMember(d => d.Birthdate, opt => opt.MapFrom(s => s.FechaNacimiento))
                .ForMember(d => d.DocumentNumber, opt => opt.MapFrom(s => s.Documentos.FirstOrDefault(d => !(new string[] { "CUIT", "CUIL" }).Contains(d.Tipo.Value)).Numero.Value))
                .ForMember(d => d.DocumentType, opt => opt.MapFrom(s => s.Documentos.FirstOrDefault(d => !(new string[] { "CUIT", "CUIL" }).Contains(d.Tipo.Value)).Tipo.Value))
                .ForMember(d => d.CuixNumber, opt => opt.MapFrom(s => s.DatosPersonaComunes.IdentificacionTributariaNumero))
                .ForMember(d => d.CuixCode, opt => opt.MapFrom(s => s.DatosPersonaComunes.IdentificacionTributariaTipo))
                .ForMember(d => d.CuixType, opt => opt.MapFrom(s => s.DatosPersonaComunes.IdentificacionTributariaTipo == "02" ? "CUIL" : "CUIT"))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.NombrePersona.Apellido))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.NombrePersona.Nombre))
                .ForMember(d => d.HostId, opt => opt.MapFrom(s => s.DatosPersonaComunes.IdPersona))
                .ForMember(d => d.Sex, opt => opt.MapFrom(s => s.Sexo));

            CreateMap<Models.N.Location.Address, BUS.AdministracionCliente.Domicilio1>()
                .ForMember(d => d.Direccion, opt => opt.MapFrom(s =>  new BUS.AdministracionCliente.domicilio2salida
                    {
                        Calle = s.Street,
                        CodigoPais = s.Country.Code,
                        CPA = s.PostalCode,
                        CodigoUsoPersona = "PA",
                        Departamento = s.FlatNumber,
                        Piso = s.Floor,
                        CodigoProvincia = s.Province.Code,
                        NumeroPuerta = s.Number,
                        Latitud = Convert.ToDecimal(s.Location.Latitude),
                        Longitud = Convert.ToDecimal(s.Location.Longitude)
                    }));
                //}))
                //.ForMember(d => d.ParametrizacionFisica, opt => opt.MapFrom(s => new BUS.AdministracionCliente.ParametrizacionFisica
                //{
                //    ActualizarFisicaDatosDomicilio = true,
                //    ActualizarFisicaDatosDomicilioSpecified  = true
                //}));

            CreateMap<Models.N.Client.MinimumClientData, BUS.AdministracionCliente.CrearClienteDatosBasicosRequestDatos>()
                .ForMember(d => d.Persona, opt => opt.MapFrom(s => new BUS.AdministracionCliente.CrearClienteDatosBasicosRequestDatosPersona
                {
                    CodigoSexo = new BUS.AdministracionCliente.codigov2 { Value = s.Sex },
                    DatosNacimiento = new BUS.AdministracionCliente.CrearClienteDatosBasicosRequestDatosPersonaDatosNacimiento
                    {
                        FechaNacimiento = s.Birthdate
                    },
                    Documentos = new BUS.AdministracionCliente.documento[]{
                        new BUS.AdministracionCliente.documento {
                        Numero = new BUS.AdministracionCliente.id { Value = s.DocumentNumber},
                        Tipo = new BUS.AdministracionCliente.codigo { Value = s.DocumentType}
                        }
                    },
                    NombrePersona = new BUS.AdministracionCliente.CrearClienteDatosBasicosRequestDatosPersonaNombrePersona {
                        Apellido = s.LastName,
                        Nombre = s.Name
                    }
                }));

        }
    }
}
