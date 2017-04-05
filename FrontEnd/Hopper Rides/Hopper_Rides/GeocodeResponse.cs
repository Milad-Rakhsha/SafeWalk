using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Hopper_Rides
{
    [DataContract]
    class GeocodeResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "results")]
        public MapResult[] Results { get; set; }
    }

    [DataContract]
    class MapResult
    {
        [DataMember(Name = "address_components")]
        public AddressComp[] Address_components { get; set; }

        [DataMember(Name = "formatted_address")]
        public string Formatted_address { get; set; }

        [DataMember(Name = "geometry")]
        public Geometry Geo { get; set; }

        [DataMember(Name = "place_id")]
        public string Place_id { get; set; }

        [DataMember(Name = "types")]
        public string[] Types { get; set; }
    }

    [DataContract]
    class AddressComp
    {
        [DataMember(Name = "long_name")]
        public string Long_name { get; set; }

        [DataMember(Name = "short_name")]
        public string Short_name { get; set; }

        [DataMember(Name = "types")]
        public string[] Types { get; set; }
    }

    [DataContract]
    class Geometry
    {
        [DataMember(Name = "bounds")]
        public Bounds Boundaries { get; set; }

        [DataMember(Name = "location")]
        public Location Loc { get; set; }

        [DataMember(Name = "location_type")]
        public string Location_type { get; set; }

        [DataMember(Name = "viewport")]
        public Viewport View { get; set; }
    }

    [DataContract]
    class Bounds
    {
        [DataMember(Name = "northeast")]
        public Northeast NE { get; set; }

        [DataMember(Name = "southwest")]
        public Southwest SW { get; set; }
    }

    [DataContract]
    class Location
    {
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }
    }

    [DataContract]
    class Viewport
    {
        [DataMember(Name = "northeast")]
        public Northeast NE { get; set; }

        [DataMember(Name = "southwest")]
        public Southwest SW { get; set; }
    }

    [DataContract]
    class Northeast
    {
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }
    }

    [DataContract]
    class Southwest
    {
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }
    }


}
