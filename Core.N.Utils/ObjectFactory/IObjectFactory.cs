using System.Threading.Tasks;

namespace Core.N.Utils.ObjectFactory
{
    public interface IObjectFactory
    {
        Task<T> InstantiateFromFile<T>(string path);
        T InstantiateFromString<T>(string json);
    }
}
