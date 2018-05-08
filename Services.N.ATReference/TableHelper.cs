using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BGBA.Models.N.Location;
using Services.N.ATReference;

namespace BGBA.Services.N.ATReference
{
    public class TableHelper
    {
        private readonly ITableServices _tableServices;
        private readonly IMapper _mapper;

        public TableHelper(ITableServices tableServices, IMapper mapper)
        {
            _tableServices = tableServices;
            _mapper = mapper;
        }
        
        public async Task<bool> CheckLocalityAsync(Province province, string locality)
        {
            var filters = new Dictionary<string, string>();
            //Si el codigo de provinicia es 01 entonces se trata de la ciudad de buenos aires
            //Para la ciudad de buenos aires se piden los barrios y no las localidades
            if (province.Code == "01")
            {

                filters.Add("TI_SUCURSAL", "TR");
                filters.Add("COD_PROVINCIA", "01");
                filters.Add("COD_ESTADO", "A");
                filters.Add("TXT_BARRIO", locality);

                return (await _tableServices.GetTableByPost<List<SublocalityATReference>>("SUCUR1", filters)).Count > 0;
            }

            filters.Add("NOM_LOCALIDAD", locality);

            return (await _tableServices.GetTableByPost<List<LocalityATReference>>($"LOCA{province.Id}", filters))?.Count > 0;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            var countries = await _tableServices.GetTableByPost<List<CountryATReference>>("PAIS", new Dictionary<string, string>());
            countries.ForEach(p => p.Description = p.Description.Replace('#', 'ñ').ToLower());

            return _mapper.Map<List<Country>>(countries);
        }

        public async Task<List<Province>> GetProvincesAsync()
        {
            var provinces = await _tableServices.GetTableByPost<List<ProvinceATReference>>("PROVIN", new Dictionary<string, string>());

            provinces.ForEach(p => p.Name = p.Name.Replace('#', 'ñ').ToLower());

            return _mapper.Map<List<Province>>(provinces);
        }

        public async Task<Province> GetProvinceAsync(string name)
        {
            var filters = new Dictionary<string, string>
            {
                { "NOM_PROVINCIA", name.ToUpper() }
            };

            var provinces = await _tableServices.GetTableByPost<List<ProvinceATReference>>("PROVIN", filters);

            provinces.ForEach(p => p.Name = p.Name.Replace('#', 'ñ').ToLower());

            return _mapper.Map<Province>(provinces);
        }


        public async Task<List<BranchOffice>> GetBranchOfficesAsync(Dictionary<string, string> filters)
        {
            var result = await _tableServices.GetTableByPost<List<BranchOfficeATReference>>("SUCUR", filters);
            return _mapper.Map<List<BranchOffice>>(result);
        }

        public async Task<List<BranchOffice>> GetBranchOfficesByLocalityAsync()
        {
            var filters = new Dictionary<string, string>();
            filters.Add("TI_SUCURSAL", "TR");

            var result = await _tableServices.GetTableByPost<List<BranchOffice>>("SUCUR1", filters);

            return _mapper.Map<List<BranchOffice>>(result);
        }

        public async Task<List<Locality>> GetLocalitiesByProvinceAsync(Province province)
        {
            var localities = await _tableServices.GetTableByPost<List<LocalityATReference>>($"LOCA{province.Id}", new Dictionary<string, string>());
            var branchOffices = await GetBranchOfficesAsync(new Dictionary<string, string>());
            var branchOfficesPerLocality = await GetBranchOfficesByLocalityAsync();

            localities.ForEach(l =>
            {
                l.BranchOffices = branchOffices.Where(s => branchOfficesPerLocality.Where(sp => sp.LocalityCode == l.LocalityCode).ToList().Exists(sp => sp.Code == s.Code)).ToList();
                l.Name = l.Name.Replace('#', 'ñ').ToLower();
            });
            
            return _mapper.Map<List<Locality>>(localities);
        }

        public async Task<List<Locality>> GetLocalitiesByProvinceWithCPAsync(Province province)
        {
            var localities = await _tableServices.GetTableByPost<List<LocalityATReference>>($"LOCACP{province.Id}", new Dictionary<string, string>());
            var branchOffices = await GetBranchOfficesAsync(new Dictionary<string, string>());
            var branchOfficesPerLocality = await GetBranchOfficesByLocalityAsync();

            localities.ForEach(l =>
            {
                l.BranchOffices = branchOffices.Where(s => branchOfficesPerLocality.Where(sp => sp.LocalityCode == l.LocalityCode).ToList().Exists(sp => sp.Code == s.Code)).ToList();
                l.Name = l.Name.Replace('#', 'ñ').ToLower();
            });

            return _mapper.Map<List<Locality>>(localities);
        }

        public async Task<List<Sublocality>> GetSublocalitiesAsync()
        {
            var filters = new Dictionary<string, string>();
            filters.Add("TI_SUCURSAL", "TR");
            filters.Add("COD_PROVINCIA", "01");
            filters.Add("COD_ESTADO", "A");

            var branchOffices = await GetBranchOfficesAsync(new Dictionary<string, string>());

            var result = (await _tableServices.GetTableByPost<List<SublocalityATReference>>("SUCUR1", filters))
                .GroupBy(b => b.Name, (Key, Value) => new
                {
                    Sublocality = Key,
                    BranchOffices = Value.Select(s => s.BranchCode)
                }).Select(b => new SublocalityATReference()
                {
                    Name = b.Sublocality.Replace('#', 'ñ').ToLower(),
                    BranchOffices = branchOffices.Where(s => b.BranchOffices.Contains(s.Code)).ToList()
                }).OrderBy(b => b.Name).ToList();

            return _mapper.Map<List<Sublocality>>(result);
        }

        public async Task<BranchOffice> GetBranchOfficeByCodeAsync(string branchCode)
        {
            var filters = new Dictionary<string, string>();
            filters.Add("SUCURSAL", branchCode);

            var branchOffices = await GetBranchOfficesAsync(filters);

            return branchOffices.FirstOrDefault(b => b.Code == branchCode);

        }
    }
}