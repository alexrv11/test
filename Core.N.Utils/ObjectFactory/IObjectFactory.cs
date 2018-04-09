using System.Threading.Tasks;

namespace Core.N.Utils.ObjectFactory
{
    public interface IObjectFactory
    {
        Task<T> InstantiateFromJsonFile<T>(string path);
        //Task<T> InstantiateJsonXmlFromFile<T>(string path);
        T InstantiateFromJsonString<T>(string json);
    }
}
