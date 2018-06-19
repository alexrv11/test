using System.Threading.Tasks;

namespace BGBA.Services.N.Location
{
    public interface IMapServices : Models.N.Core.Trace.ITraceService
    {
        Task<Models.N.Location.GoogleMapsAddress> GetFullAddress(Models.N.Location.Address address);
        Task<Models.N.Location.GoogleMapsAddress> GetFullAddress(string placeId);
        Task<Models.N.Location.PredictionsResult> GetPrediction(Models.N.Location.MapOptions options);
        string GetUrlMap(Models.N.Location.MapOptions options);
        Task<bool> NormalizeAddress(Models.N.Location.MapOptions mapOptions, Models.N.Location.GoogleMapsAddress mapAddress);
        Task<Models.N.Location.Address> NormalizeAddressByPlaceId(Models.N.Location.GoogleMapsAddress mapAddress, Models.N.Location.MapOptions options);

    }
}
