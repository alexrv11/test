using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BGBA.Models.N.Core.Utils.ObjectFactory
{
    public class ObjectFactory : IObjectFactory
    {
        public async Task<T> InstantiateFromJsonFile<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
        }

        public T InstantiateFromJsonString<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
