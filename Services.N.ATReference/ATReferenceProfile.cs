using AutoMapper;
using Models.N.Location;
namespace Services.N.Consulta.ATReference
{
    public class ATReferenceProfile : Profile
    {
        public ATReferenceProfile() {
            CreateMap<Sublocality, Sublocality>();
            CreateMap<LocalityATReference, Locality>();
            CreateMap<ProvinceATReference, Province>();
            CreateMap<CountryATReference, Country>();
            CreateMap<BranchOfficeATReference, BranchOffice>();
        }
    }
}
