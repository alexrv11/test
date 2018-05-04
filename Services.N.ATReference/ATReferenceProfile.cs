using AutoMapper;
using BGBA.Models.N.Location;
namespace Services.N.ATReference
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
