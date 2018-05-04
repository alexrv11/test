using System.Threading.Tasks;

namespace BGBA.Models.N.Core.Utils.ObjectFactory
{
    public interface IObjectFactory
    {
        Task<T> InstantiateFromJsonFile<T>(string path);
        T InstantiateFromJsonString<T>(string json);
    }
}
