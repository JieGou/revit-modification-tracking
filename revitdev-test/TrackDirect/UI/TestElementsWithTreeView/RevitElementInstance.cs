using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace KTR.Main.TreeViewAllElementInstances
{
    public class RevitElementInstance
    {
        public string _id { get; set; }

        public string Guid { get; set; }

        public int ElementId { get; set; }

        public Boolean IsType { get; set; }

        public string Category { get; set; }

        public string Level { get; set; }

        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public Dictionary<int, int> ParameterValueIds { get; set; } = new Dictionary<int, int>();

        public XYZ LocationPoint { get; set; }

        public XYZ LocationPoint2 { get; set; }

        public BoundingBoxXYZ BoundingBox { get; set; }



        public string Name { get; set; }

        public string Project { get; set; }

        public string Version { get; set; }

        public string SharedParameters { get; set; }

        public string WorksetId { get; set; }

        public string Location { get; set; }

        public string Centroid { get; set; }

        public string TypeId { get; set; }

        public string Volume { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int __v { get; set; }
    }
}

