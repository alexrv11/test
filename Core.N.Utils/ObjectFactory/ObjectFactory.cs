using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Core.N.Utils.ObjectFactory
{
    public class ObjectFactory : IObjectFactory
    {
        public async Task<T> InstantiateFromJsonFile<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
        }

        //public async Task<T> InstantiateFromXmlFile<T>(string path)
        //{

        //}

        public T InstantiateFromJsonString<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
