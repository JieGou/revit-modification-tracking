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

        private static Document _doc;
        
       

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

                string geoParameters = RevitUtils.SerializeGeoParameters(e);
                miniComp.GeoParameters  = Convert.ToBase64String(hasher.ComputeHash(RevitUtils.GetBytes(geoParameters)));

                //string revitParameters = RevitUtils.SerializeRevitParameters(e);//Get only Revit Parameters
                //string sharedParameters = RevitUtils.SerializeSharedParameters(e, _doc);//Get only Shred Parameters

                //miniComp.RevitParameter = Convert.ToBase64String(hasher.ComputeHash(RevitUtils.GetBytes(revitParameters)));
                //miniComp.SharedParameter = Convert.ToBase64String(hasher.ComputeHash(RevitUtils.GetBytes(sharedParameters)));
                miniComp.DicRvtParams = GetRvtParams(e);
                miniComp.DicSharedParams = GetSharedParams(e);
            }
            return miniComp;
        }
        #endregion // Store element state

        public static string GetTypeChange(MiniDataComparision previous, MiniDataComparision current)
        {
            string c = string.Empty;
            if (current.ElementDescription != previous.ElementDescription)
                return ChangedElement.ChangeTypeEnum.FamilyOrTypeChange.ToString();
            if (current.GeoParameters != previous.GeoParameters)
                return ChangedElement.ChangeTypeEnum.GeometryChange.ToString();
            if (current.GeometryInfo != previous.GeometryInfo)
                return ChangedElement.ChangeTypeEnum.VolumeOrLocationChange.ToString();
            if (!MiniCompareRevitParameters(previous,current))
                return ChangedElement.ChangeTypeEnum.RevitParameterChange.ToString();
            if (!MiniCompareSharedParameters(previous,current))
                return ChangedElement.ChangeTypeEnum.SharedParameterChange.ToString();
            return c;
        }
        #endregion //Public methods

        #region PrivateMethods
        private static bool MiniCompareRevitParameters(MiniDataComparision previous, MiniDataComparision current)
        {
            //StringBuilder description = null;
            bool isEqual = true;
            //int numParmsChanged = 0;
            foreach (var pair in current.DicRvtParams)
            {
                if (previous.DicRvtParams.ContainsKey(pair.Key))
                {
                    // test if they match
                    if (current.DicRvtParams[pair.Key] != previous.DicRvtParams[pair.Key])
                    {
                        isEqual = false;
                        return isEqual;
                        //if (description == null) description = new StringBuilder();
                        //numParmsChanged++;
                        //if (numParmsChanged > 1) description.Append(", ");
                        //description.Append(pair.Key + " From: " + previous.DicRvtParams[pair.Key] + " to " + current.DicRvtParams[pair.Key]);
                    }

                }
            }
            return isEqual;
        }

        //Compare shared parameter between 2 data comparison
        private static bool MiniCompareSharedParameters(MiniDataComparision previous, MiniDataComparision current)
        {
            bool isEqual = true;
            foreach (var pair in current.DicSharedParams)
            {
                if (previous.DicSharedParams.ContainsKey(pair.Key)) //Only compare with same shared parameters
                {
                    if (current.DicSharedParams[pair.Key] != previous.DicSharedParams[pair.Key])
                    {
                        isEqual = false;
                        return isEqual;
                    }

                }
            }
            return isEqual;
        }
        
        private static Dictionary<string, string> GetRvtParams(Element e)
        {
            //Get only Revit Parameters
            var dicRvtParams = new Dictionary<string, string>();
            var rvtParameters = RevitUtils.GetRevitParametersExcludeGeometryParam(e);
            foreach (var p in rvtParameters)
            {
                try
                {
                    if (p.Definition == null) continue; // we don't want this!
                    string definition = p.Definition.Name;
                    string val = null;
                    switch (p.StorageType)
                    {
                        case StorageType.String:
                            val = p.AsString();
                            break;

                        default:
                            val = p.AsValueString();
                            break;
                    }

                    if (val == null) val = "(n/a)";

                    dicRvtParams[p.Definition.Name] = val;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Weird database: " + ex);
                }
            }
            return dicRvtParams;

        }
        private static Dictionary<string, string> GetSharedParams(Element e)
        {
            var dicSharedParams = new Dictionary<string, string>();
            var sharedParameters = RevitUtils.GetSharedParameters(e, _doc);
            foreach (var p in sharedParameters)
            {
                try
                {
                    string val = RevitUtils.ParameterToString(p);
                    dicSharedParams[p.Definition.Name] = val;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Weird database: " + ex);
                }

            }
            return dicSharedParams;
        }

        #endregion //private method
    }
}
