using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace TrackDirect.Models
{
    /// <summary>
    /// Contain information of project and list data container comparision
    /// </summary>
    public class MiniComparison
    {
        public string projectId { get; set; }
        public Dictionary<string, MiniComparisionContainer> TrackedData
            = new Dictionary<string, MiniComparisionContainer>();
    }
   
}
