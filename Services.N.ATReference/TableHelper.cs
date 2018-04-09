using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.N.Location;

namespace Services.N.Consulta.ATReference
{
    public class TableHelper
    {
        private readonly ITableServices _tableServices;

        public TableHelper(ITableServices tableServices)
        {
            _tableServices = tableServices;
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

                return (await _tableServices.GetTableByPost<List<Sublocality>>("SUCUR1", filters)).Count > 0;
            }

            filters.Add("NOM_LOCALIDAD", locality);

            return (await _tableServices.GetTableByPost<List<Locality>>($"LOCA{province.Id}", filters))?.Count > 0;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            var paises = await _tableServices.GetTableByPost<List<Country>>("PAIS", new Dictionary<string, string>());
            paises.ForEach(p => p.Description = p.Description.Replace('#', 'ñ').ToLower());

            return paises;
        }

        public async Task<List<Province>> GetProvincesAsync()
        {
            var provincias = await _tableServices.GetTableByPost<List<Province>>("PROVIN", new Dictionary<string, string>());

            provincias.ForEach(p => p.Name = p.Name.Replace('#', 'ñ').ToLower());

            return provincias;
        }

        public async Task<List<BranchOffice>> GetBranchOfficesAsync(Dictionary<string, string> filters)
        {
            return await _tableServices.GetTableByPost<List<BranchOffice>>("SUCUR", filters);
        }

        public async Task<List<BranchOffice>> GetBranchOfficesByLocalityAsync()
        {
            var filters = new Dictionary<string, string>();
            filters.Add("TI_SUCURSAL", "TR");

            return await _tableServices.GetTableByPost<List<BranchOffice>>("SUCUR1", filters);
        }

        public async Task<List<Locality>> GetLocalitiesByProvinceAsync(Province province)
        {
            var localities = await _tableServices.GetTableByPost<List<Locality>>($"LOCA{province.Id}", new Dictionary<string, string>());
            var branchOffices = await GetBranchOfficesAsync(new Dictionary<string, string>());
            var branchOfficesPerLocality = await GetBranchOfficesByLocalityAsync();

            localities.ForEach(l =>
            {
                l.BranchOffices = branchOffices.Where(s => branchOfficesPerLocality.Where(sp => sp.LocalityCode == l.LocalityCode).ToList().Exists(sp => sp.Code == s.Code)).ToList();
                l.Name = l.Name.Replace('#', 'ñ').ToLower();
            });

            return localities;
        }

        public async Task<List<Locality>> GetLocalitiesByProvinceWithCPAsync(Province province)
        {
            var localities = await _tableServices.GetTableByPost<List<Locality>>($"LOCACP{province.Id}", new Dictionary<string, string>());
            var branchOffices = await GetBranchOfficesAsync(new Dictionary<string, string>());
            var branchOfficesPerLocality = await GetBranchOfficesByLocalityAsync();

            localities.ForEach(l =>
            {
                l.BranchOffices = branchOffices.Where(s => branchOfficesPerLocality.Where(sp => sp.LocalityCode == l.LocalityCode).ToList().Exists(sp => sp.Code == s.Code)).ToList();
                l.Name = l.Name.Replace('#', 'ñ').ToLower();
            });

            return localities;
        }

        public async Task<List<Sublocality>> GetSublocalitiesAsync()
        {
            var filters = new Dictionary<string, string>();
            filters.Add("TI_SUCURSAL", "TR");
            filters.Add("COD_PROVINCIA", "01");
            filters.Add("COD_ESTADO", "A");

            var branchOffices = await GetBranchOfficesAsync(new Dictionary<string, string>());

            var result = (await _tableServices.GetTableByPost<List<Sublocality>>("SUCUR1", filters))
                .GroupBy(b => b.Name, (Key, Value) => new
                {
                    Sublocality = Key,
                    BranchOffices = Value.Select(s => s.BranchCode)
                }).Select(b => new Sublocality()
                {
                    Name = b.Sublocality.Replace('#', 'ñ').ToLower(),
                    BranchOffices = branchOffices.Where(s => b.BranchOffices.Contains(s.Code)).ToList()
                }).OrderBy(b => b.Name).ToList();

            return result;
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