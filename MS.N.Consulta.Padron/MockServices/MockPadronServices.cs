using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Models.N.Consulta.Padron;
using Newtonsoft.Json;
using Services.N.Consulta.Padron;

namespace MS.N.Consulta.Padron.MockServices
{
    public class MockPadronServices : IPadronServices
    {
        private IHostingEnvironment _enviroment;

        public MockPadronServices(IHostingEnvironment enviroment)
        {
            _enviroment = enviroment;
        }

        public async Task<DatosPadron> ConsultaPadron(string cuix)
        {
            var data = JsonConvert.DeserializeObject<List<DatosPadron>>(await File.ReadAllTextAsync($"{_enviroment.ContentRootPath}/MockServices/Data/Mock.json"));
            return data.FirstOrDefault(d => d.Cuix == cuix);
        }
    }
}
