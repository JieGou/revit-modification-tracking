using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using TrackDirect.Utilities;

namespace TrackDirect.Models
{
    public class RevitElement
    {
        public string UniqueId { get; set; }

        public string ProjectId { get; set; }

        public int ElementId { get; set; }

        public bool IsType { get; set; }

        public string Category { get; set; }

        public string Level { get; set; }

        public Dictionary<string, string> RevitParameters { get; set; } = new Dictionary<string, string>();

        //public Dictionary<string, string> GeoParameters { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> SharedParameters { get; set; } = new Dictionary<string, string>();

        public BoundingBoxXYZ BoundingBox { get; set; }

        public string Vertices { get; set; }

        public string ElementDescription { get; set; }
         public string Location { get; set; }


        public RevitElement() { }
        public RevitElement(Document doc,Element element)
        {
            
        }
    }
}

