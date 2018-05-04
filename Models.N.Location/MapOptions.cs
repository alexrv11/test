using System.Collections.Generic;
using System.Linq;
namespace BGBA.Models.N.Location
{
    public class MapOptions
    {
        public bool LocationIsCoord { get; set; }
        public bool LocationGetCoord { get; set; }
        public Address Address { get; set; }
        public Models.N.Location.Location Location;
        public bool DefaultMarker { get; set; } = true;
        public List<Models.N.Location.Marker> Markers { get; set; }
        public string Size { get; set; } = "400x400";
        public string Zoom { get; set; } = "16";

        public override string ToString()
        {
            var center = !LocationIsCoord ? Address?.ToString()??"" : Location?.ToString() ?? "";
            var markers = Markers == null ? DefaultMarker ? $"&markers=size:mid|color:red|{center}" : "" : string.Join("&markers=", Markers.Select(m => m.ToString()));
            return $"center={center}&zoom={Zoom}&size={Size}{markers}";
        }
    }
}
