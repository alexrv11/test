using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.N.Utils.ObjectFactory
{
    public class ObjectFactory : IObjectFactory
    {
        public async Task<T> InstantiateFromFile<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
        }

        public T InstantiateFromString<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
