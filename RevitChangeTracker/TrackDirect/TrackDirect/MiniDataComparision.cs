using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrackDirect.Models
{
    public class MiniDataComparision

    {
        public int ElementId { get; set; }

        public string UniqueId { get; set; }

        public string GeometryInfo { get; set; }

        public string SharedParameter { get; set; } = String.Empty;

        public string RevitParameter { get; set; } = String.Empty;

        public string ElementDescription { get; set; }

    }
}
