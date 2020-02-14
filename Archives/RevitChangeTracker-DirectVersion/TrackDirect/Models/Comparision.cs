using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace TrackDirect.Models
{
    public class Comparison
    {
        public string projectId { get; set; }
        public Dictionary<string, RevitElement> TrackedData = new Dictionary<string, RevitElement>();

    }
   
}
