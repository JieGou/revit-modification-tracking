using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.Data.SQLite;
using System.Globalization;
using TrackDirect.Models;
using TrackDirect.Utilities;
using System.Security.Cryptography;

namespace TrackDirect
{
    public class MiniComparisonMaker
    {
        #region Declarations
        private static Document _doc;
        private Dictionary<string, int> _categoryCount = new Dictionary<string, int>();
        private HashSet<string> _requestedCategoryNames = new HashSet<string>();

        #endregion

        #region Accessors
        public bool AllCategories { get; set; } = true;
        public IList<Category> RequestedCategories { get; set; }
        #endregion

        #region Constructor
        public MiniComparisonMaker(Document doc)
        {
            _doc = doc;
        }

        #endregion

        #region PublicMethods

        #region Store element state

        /// <summary>
        /// Get infomations data to compare for revit element
        /// </summary>
        /// <param name="e"></param>
        /// <param name="isTypes"></param>
        /// <returns></returns>
        public  MiniDataComparision GetHashSetDataComparision(Element e)
        {
            SHA256 hasher = SHA256Managed.Create();
            MiniDataComparision miniComp = new MiniDataComparision();
            // seen at least one case where retrieving bounding box threw an internal error
            BoundingBoxXYZ bb = null;
            try
            {
                bb = e.get_BoundingBox(null);
            }
            catch {}

            if (bb != null)
            {
                //Get only Revit Parameters
                string revitParameters =RevitUtils.SerializeRevitParameters(e);
               
                //Get only Shred Parameters
                string sharedParameters = RevitUtils.SerializeSharedParameters(e,_doc);

                List<string> geometryInfoList = new List<string>();
                //Add location
                geometryInfoList.Add(RevitUtils.LocationString(e.Location));
                //Add boundingbox
                geometryInfoList.Add(RevitUtils.BoundingBoxString(bb));
                //Add vertices to revit element
                geometryInfoList.Add(RevitUtils.PointArrayString(RevitUtils.GetCanonicVertices(e)));

                //Create property for mini comparision container
                miniComp.ElementId = e.Id.IntegerValue;
                miniComp.UniqueId = e.UniqueId;

                //Get hashset for long string to simple compare
                miniComp.ElementDescription = Convert.ToBase64String(
                      hasher.ComputeHash(RevitUtils.GetBytes(RevitUtils.GetElementDescription(e))));
                miniComp.GeometryInfo = Convert.ToBase64String(
                     hasher.ComputeHash(RevitUtils.GetBytes(string.Join(";", geometryInfoList))));
                miniComp.RevitParameter = Convert.ToBase64String(hasher.ComputeHash(RevitUtils.GetBytes(revitParameters)));
                miniComp.SharedParameter = Convert.ToBase64String(hasher.ComputeHash(RevitUtils.GetBytes(sharedParameters)));
            }
            return miniComp;
        }
        #endregion // Store element state


        #endregion //Public methods

        #region PrivateMethods

        public static string GetTypeChange(MiniDataComparision current, MiniDataComparision previous)
        {
            string c = string.Empty;
            if (current.ElementDescription != previous.ElementDescription)
                return "TypeElement Change";
            if (current.GeometryInfo != previous.GeometryInfo)
                return "GeometryOrLocation Change";
            if (current.RevitParameter!= previous.RevitParameter)
                return "RevitParameter Change";
            if (current.SharedParameter != previous.SharedParameter)
                return "SharedParameter Change";
            return c;
        }


        #endregion //private method
    }
}
