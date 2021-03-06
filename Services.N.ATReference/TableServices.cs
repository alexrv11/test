﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BGBA.Services.N.ATReference
{
    public class TableServices : BGBA.Models.N.Core.Trace.TraceServiceBase, ITableServices
    {
        private readonly IConfiguration _configuration;

        public TableServices(IConfiguration configuration) {
            _configuration = configuration;

        }

        public async Task<T> GetTableByGet<T>(string tableName, Dictionary<string, string> filters)
        {
            return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync($"{_configuration["Table:Path"]}/{tableName}.json"));
        }

        public async Task<T> GetTableByPost<T>(string tableName, Dictionary<string, string> filters)
        {
            return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync($"{_configuration["Table:Path"]}/{tableName}.json"));
        }
    }
}
