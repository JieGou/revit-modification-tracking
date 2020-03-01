using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrackDirect.Models
{
    public class MiniDataComparision

    {
        public int ElementId { get; set; }

        public string UniqueId { get; set; }

        public string GeometryInfo { get; set; }

        public string GeoParameters { get; set; }

        public string ElementDescription { get; set; }

        //Dictionary Parameters to control paramter exist in Element
        public Dictionary<string, string> DicRvtParams { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DicSharedParams { get; set; } = new Dictionary<string, string>();


        //public string SharedParameter { get; set; } = String.Empty;

        //public string RevitParameter { get; set; } = String.Empty;
    }
}
