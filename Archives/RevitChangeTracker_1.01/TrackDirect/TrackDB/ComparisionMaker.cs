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
    public class ComparisonMaker
    {
        #region Declarations
        private static Document _doc;
        private string _filename;
        private string _dbFilename;
        private Dictionary<int, string> _parameterDict = new Dictionary<int, string>();
        private Dictionary<string, string> _headerDict = new Dictionary<string, string>();
        private Dictionary<int, string> _valueDict = new Dictionary<int, string>();
        private Dictionary<string, int> _categoryCount = new Dictionary<string, int>();
        private HashSet<string> _requestedCategoryNames = new HashSet<string>();
        private static IList<Level> _allLevels;
        private bool _isMetric = false;



        // TODO: separate categories by dictionary of category, elementid, parameter
        private Dictionary<string, RevitElement> _previousElems = new Dictionary<string, RevitElement>();
        private Dictionary<string, RevitElement> _currentElems = new Dictionary<string, RevitElement>();


        #endregion

        #region Accessors
        public bool AllCategories { get; set; } = true;
        public IList<Category> RequestedCategories { get; set; }
        public double MoveTolerance { get; set; }
        public float RotateTolerance { get; set; }
        #endregion

        #region Constructor
        public ComparisonMaker(Document doc)
        {
            _doc = doc;
            _allLevels = RevitUtils.GetLevels(_doc);
        }
        public ComparisonMaker(Document doc, string previousFile)
        {
            MoveTolerance = 0.0006;  // default;
            RotateTolerance = 0.0349f; // default

            _doc = doc;
            if (doc.DisplayUnitSystem == DisplayUnit.METRIC) _isMetric = true;

            _filename = previousFile;

            _dbFilename = _filename;
            // see: http://system.data.sqlite.org/index.html/info/bbdda6eae2
            if (_filename.StartsWith(@"\\")) _dbFilename = @"\\" + _dbFilename;
            //doc.Application.WriteJournalComment("Previous File DB: " + _dbFilename, false);
        }
        #endregion

        #region PublicMethods
        public IList<ChangedElement> Compare()
        {
            // we want to load up a previous model, 
            //readPrevious();
            // read the existing data into memory
            //readModel();

            // make our comparisons
            return compareData();


        }

        public void Serialize(string filename, IList<ChangedElement> changes)
        {
            System.IO.File.WriteAllText(filename, Serialize(changes));
        }

        public static ChangeSummary DeSerialize(string filename)
        {
            string content = System.IO.File.ReadAllText(filename);
            ChangeSummary cs = Newtonsoft.Json.JsonConvert.DeserializeObject<ChangeSummary>(content);

            return cs;
        }

        public string Serialize(IList<ChangedElement> changes)
        {
            // build the change summary:

            ChangeSummary summary = new ChangeSummary();
            summary.ModelName = _doc.Title;
            summary.ModelPath = _doc.PathName;
            summary.NumberOfChanges = changes.Count;
            summary.PreviousFile = _filename;
            summary.ComparisonDate = DateTime.Now;
            summary.Changes = changes;
            summary.ModelSummary = _categoryCount;
            summary.LevelNames = _allLevels.OrderBy(a => a.Elevation).Select(a => a.Name).ToList();


            string result = Newtonsoft.Json.JsonConvert.SerializeObject(summary);
            //var serialize = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string result = serialize.Serialize(summary);

            return result;

        }

        #region Store element state

        /// <summary>
        /// Get infomations data to compare for revit element
        /// </summary>
        /// <param name="e"></param>
        /// <param name="isTypes"></param>
        /// <returns></returns>
        public  RevitElement GetDataComparision(Element e, bool isTypes)
        {
            RevitElement revitElem = new RevitElement();
            // seen at least one case where retrieving bounding box threw an internal error
            BoundingBoxXYZ bb = null;
            try
            {
                bb = e.get_BoundingBox(null);
            }
            catch (Exception ex)
            {
                if (ex is Autodesk.Revit.Exceptions.ApplicationException)
                {
                    var aex = ex as Autodesk.Revit.Exceptions.ApplicationException;
                }
            }

            if (bb != null)
            {
                    //Add bounding box to revit element
                    revitElem.BoundingBox = bb;
                    //Add vertices to revit element
                    revitElem.Vertices = RevitUtils.PointArrayString(RevitUtils.GetCanonicVertices(e));
                    Category c = e.Category;
                if (e is FamilySymbol)
                {
                    FamilySymbol fs = e as FamilySymbol;
                    c = fs.Family.FamilyCategory;
                }
                revitElem.ElementId = e.Id.IntegerValue;
                revitElem.UniqueId = e.UniqueId;
                revitElem.Category = (c != null) ? c.Name : "(none)";
                revitElem.ElementDescription = RevitUtils.GetElementDescription(e);
                revitElem.Location = RevitUtils.LocationString(e.Location);
                revitElem.Level = RevitUtils.GetLevelName(_allLevels, e);

                //Get only Revit Parameters
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

                        revitElem.RevitParameters[p.Definition.Name] = val;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Weird database: " + ex);
                    }
                }

                ////Specify for geometry parameters
                //var geoParameters = RevitUtils.GetGeoParameters(e);
                //foreach (var p in rvtParameters)
                //{
                //    try
                //    {
                //        string val = p.AsDouble().ToString();
                //        revitElem.GeoParameters[p.Definition.Name] = val;
                //    }
                //    catch (Exception ex)
                //    {

                //        System.Diagnostics.Debug.WriteLine("Weird database: " + ex);
                //    }

                //}

                //Get only shared parameter
                var sharedParameters = RevitUtils.GetSharedParameters(e, _doc);
                foreach (var p in sharedParameters)
                {
                    try
                    {
                        string val = RevitUtils.ParameterToString(p);
                        revitElem.SharedParameters[p.Definition.Name] = val;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Weird database: " + ex);
                    }

                }

            }
            return revitElem;
        }
        #endregion // Store element state


        #endregion //Public methods

        #region PrivateMethods
        public IList<ChangedElement> compareData()
        {
            List<ChangedElement> changes = new List<ChangedElement>();
            ComparisonController.NewElementList = new List<ChangedElement>();
            ComparisonController.DeletedElementList = new List<ChangedElement>();
            ComparisonController.ChangedElementList = new List<ChangedElement>();

            foreach (var currentPair in _currentElems)
            {
                var current = currentPair.Value;
                // find it from the previous
                if (_previousElems.ContainsKey(currentPair.Key))
                {
                    // it exists, so let's compare
                    var previous = _previousElems[currentPair.Key];

                    var change = compareElements(current, previous);
                    if (change != null) ComparisonController.ChangedElementList.Add(change);
                }
                else
                {
                    // it has been removed.
                    ComparisonController.NewElementList.Add(buildNew(current));
                }
            }

            // now look for deleted items
            foreach (var previousPair in _previousElems)
            {
                if (_currentElems.ContainsKey(previousPair.Key) == false)
                {
                    if (!AllCategories && (_requestedCategoryNames.Contains(previousPair.Value.Category) == false)) continue; // do not include
                    ComparisonController.DeletedElementList.Add(buildDeleted(previousPair.Value));
                }
            }

            return changes;
        }

        public static ChangedElement buildNew(RevitElement current)
        {
            Element e = _doc.GetElement(current.UniqueId);

            ChangedElement c = new ChangedElement()
            {
                ElementId = current.ElementId,
                UniqueId = current.UniqueId,
                Category = current.Category,
                ChangeType = ChangedElement.ChangeTypeEnum.NewElement,
                CreatedDate = ComparisonController._dateRecord,
                User = CmdAutoTrack.UserRevit,
                Level = (current.Level != null) ? current.Level : ""
            };
            c.BoundingBoxDescription = Utilities.RevitUtils.SerializeBoundingBox(current.BoundingBox);

            return c;
        }
        public static ChangedElement buildDeleted(RevitElement element)
        {
            ChangedElement c = new ChangedElement()
            {
                ElementId = element.ElementId,
                UniqueId = element.UniqueId,
                Category = element.Category,
                ChangeType = ChangedElement.ChangeTypeEnum.DeletedElement,
                Level = (element.Level != null) ? element.Level : ""
            };
            c.BoundingBoxDescription = Utilities.RevitUtils.SerializeBoundingBox(element.BoundingBox);
            return c;
        }
        public static ChangedElement compareElements(RevitElement current, RevitElement previous)
        {
            ChangedElement c = compareElementDescription(current, previous);
            if (c != null) return c;

             c = compareSharedParameters(current, previous);
            if (c != null) return c;

             c = compareRevitParameters(current, previous);
            if (c != null) return c;

            c = compareGeometry(current, previous);
            return c;

        }

        public static ChangedElement compareRevitParameters(RevitElement current, RevitElement previous)
        {
            StringBuilder description = null;
            int numParmsChanged = 0;
            foreach (var pair in current.RevitParameters)
            {
                if (previous.RevitParameters.ContainsKey(pair.Key))
                {
                    // test if they match
                    if (current.RevitParameters[pair.Key] != previous.RevitParameters[pair.Key])
                    {
                        if (description == null) description = new StringBuilder();
                        numParmsChanged++;
                        if (numParmsChanged > 1) description.Append(", ");
                        description.Append(pair.Key + " From: " + previous.RevitParameters[pair.Key] + " to " + current.RevitParameters[pair.Key]);
                    }

                }
            }
            if (numParmsChanged > 0)
            {
                Element e = _doc.GetElement(current.UniqueId);
                ChangedElement c = new ChangedElement()
                {
                    ElementId = current.ElementId,
                    UniqueId = e.UniqueId,
                    Category = (e.Category != null) ? e.Category.Name : "(none)",
                    ChangeType = ChangedElement.ChangeTypeEnum.RevitParameterChange,
                    ChangeDescription = description.ToString(),
                    ModifiedDate = ComparisonController._dateRecord,
                    User = CmdAutoTrack.UserRevit,
                    Level = (current.Level != null) ? current.Level : ""
                };
                c.BoundingBoxDescription = Utilities.RevitUtils.SerializeBoundingBox(current.BoundingBox);

                return c;
            }

            return null;
        }

        public static ChangedElement compareSharedParameters(RevitElement current, RevitElement previous)
        {
            StringBuilder description = null;
            int numParmsChanged = 0;
            foreach (var pair in current.SharedParameters)
            {
                if (previous.SharedParameters.ContainsKey(pair.Key))
                {
                    // test if they match
                    if (current.SharedParameters[pair.Key] != previous.SharedParameters[pair.Key])
                    {
                        if (description == null) description = new StringBuilder();
                        numParmsChanged++;
                        if (numParmsChanged > 1) description.Append(", ");
                        description.Append(pair.Key + " From: " + previous.SharedParameters[pair.Key] + " to " + current.SharedParameters[pair.Key]);
                    }

                }
            }
            if (numParmsChanged > 0)
            {
                Element e = _doc.GetElement(new ElementId(current.ElementId));
                ChangedElement c = new ChangedElement()
                {
                    ElementId = current.ElementId,
                    UniqueId = e.UniqueId,
                    Category = (e.Category != null) ? e.Category.Name : "(none)",
                    ChangeType = ChangedElement.ChangeTypeEnum.SharedParameterChange,
                    ModifiedDate = ComparisonController._dateRecord,
                    User = CmdAutoTrack.UserRevit,
                    ChangeDescription = description.ToString(),
                    Level = (current.Level != null) ? current.Level : ""
                };
                c.BoundingBoxDescription = Utilities.RevitUtils.SerializeBoundingBox(current.BoundingBox);

                return c;
            }

            return null;
        }


        public static ChangedElement compareElementDescription(RevitElement current, RevitElement previous)
        {
            string desCurrent = current.ElementDescription;
            string descPrevious = previous.ElementDescription;
            
            if (desCurrent != descPrevious)
            {
                ChangedElement c = new ChangedElement()
                {
                    ChangeType = ChangedElement.ChangeTypeEnum.FamilyOrTypeChange,
                    Category = current.Category,
                    ElementId = current.ElementId,
                    UniqueId = current.UniqueId,
                    ModifiedDate = ComparisonController._dateRecord,
                    User = CmdAutoTrack.UserRevit,
                    Level = (current.Level != null) ? current.Level : "",
                    ChangeDescription = "Type Element Change",
                };
                if (current.BoundingBox != null) c.BoundingBoxDescription = Utilities.RevitUtils.SerializeBoundingBox(current.BoundingBox);
                return c;
            }
            return null;
        }

        public static ChangedElement compareGeometry(RevitElement current, RevitElement previous)
        {
            //    SHA256 hasher = SHA256Managed.Create();

            //    List<string> a = new List<string>(current.GeoParameters.Count);
            //    foreach (var geoPair in current.GeoParameters)
            //    {
            //        a.Add(string.Format("\"{0}\":\"{1}\"",
            //          geoPair.Key,geoPair.Value));
            //    }
            //    a.Sort();
            //    string sCurrent = "{" + string.Join(",", a) + "}";

            //    sCurrent = $"{sCurrent};{current.Vertices};{current.BoundingBox}";


            //    //Previous
            //    List<string> a2 = new List<string>(previous.GeoParameters.Count);
            //    foreach (var geoPair2 in previous.GeoParameters)
            //    {
            //        a2.Add(string.Format("\"{0}\":\"{1}\"",
            //          geoPair2.Key, geoPair2.Value));
            //    }
            //    a2.Sort();
            //    string sPrevious = "{" + string.Join(",", a2) + "}";

            //    sPrevious = $"{sPrevious};{previous.Vertices};{previous.BoundingBox}";


            //string hashb64Previous = Convert.ToBase64String(hasher.ComputeHash(GetBytes(sPrevious)));
            //string hashb64Current = Convert.ToBase64String(hasher.ComputeHash(GetBytes(sCurrent)));
            string gPrevious = $"{previous.Vertices};{previous.BoundingBox};{previous.Location}";
            string gCurrent = $"{current.Vertices};{current.BoundingBox};{current.Location}";

            if (gPrevious != gCurrent)
            {
                ChangedElement c = new ChangedElement()
                {
                    ChangeType = ChangedElement.ChangeTypeEnum.VolumeOrLocationChange,
                    Category = current.Category,
                    ElementId = current.ElementId,
                    UniqueId = current.UniqueId,
                    Level = (current.Level != null) ? current.Level : "",
                    ModifiedDate = ComparisonController._dateRecord,
                    User = CmdAutoTrack.UserRevit,
                    ChangeDescription = "Geometry or Location Change",
                };
                if (current.BoundingBox != null) c.BoundingBoxDescription = Utilities.RevitUtils.SerializeBoundingBox(current.BoundingBox);
                return c;
            }
            return null;
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)]; //howManyByte
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }


        
        public void readModel()
        {

            if (!AllCategories)
            {
                foreach (var c in RequestedCategories) _requestedCategoryNames.Add(c.Name);

            }

            FilteredElementCollector coll = new FilteredElementCollector(_doc);
            coll.WhereElementIsNotElementType();

            var elems = coll.ToElements().Where(e => e.Category != null).ToList();
            Dictionary<ElementId, Element> typesToCheck = new Dictionary<ElementId, Element>();
            foreach (var elem in elems)
            {
                if (elem.Category == null) continue; // we don't want this.

                ElementId typeId = elem.GetTypeId();
                if (typeId != ElementId.InvalidElementId)
                {
                    if (typesToCheck.ContainsKey(typeId) == false) typesToCheck[typeId] = _doc.GetElement(typeId);
                }
            }

            //////////////////
            CreateComparision(typesToCheck.Values.ToList(), true);
            CreateComparision(elems, false);

        }

        public string getHumanComparison(double distInFeet)
        {
            if (_isMetric)
            {
                // less than 1 foot
                if (distInFeet < 1.0)
                {
                    return (distInFeet * 304.8).ToString("F1") + "mm";
                }
                if (distInFeet < 3.28)  // less than a m, do cm...
                {
                    return (distInFeet * 30.48).ToString("F2") + "cm";
                }
                return (distInFeet * 0.3048) + "m";
            }
            else
            {
                if (distInFeet < 3.0)
                {
                    return (distInFeet * 12.0).ToString("F3") + "in.";
                }
                else
                {
                    return distInFeet + "ft.";
                }
            }
        }
        private void CreateComparision(IList<Element> elems, bool isTypes)
        {
            foreach (var e in elems)
            {
                Category c = e.Category;
                if (e is FamilySymbol)
                {
                    FamilySymbol fs = e as FamilySymbol;
                    c = fs.Family.FamilyCategory;
                }

                if (c == null) continue; // we don't want these things?

                if (!AllCategories && (_requestedCategoryNames.Contains(c.Name) == false)) continue; // not appropriate

                // count the category usage, for reference.
                if (_categoryCount.ContainsKey(c.Name) == false) _categoryCount[c.Name] = 0;
                _categoryCount[c.Name]++;

                var revitElem = GetDataComparision(e, false);

                _currentElems.Add(e.UniqueId, revitElem);
            }
        }


        //private void readHeader()
        //{
        //    try
        //    {
        //        using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + _dbFilename + ";Version=3"))
        //        {
        //            conn.Open();

        //            // first see if there is a headers table.
        //            var cmd = conn.CreateCommand();
        //            cmd.CommandText = "select name from sqlite_master WHERE type='table' and name='_objects_header'";
        //            object table = cmd.ExecuteScalar();
        //            Version schemaVersion = new Version(0, 0);
        //            if (table != null)
        //            {
        //                // there are headers, check the schema version
        //                var sv = conn.CreateCommand();
        //                sv.CommandText = "select value from _objects_header WHERE keyword=\"SchemaVersion\"";
        //                object o = sv.ExecuteScalar();
        //                if (o != null)
        //                {
        //                    string val = o.ToString();
        //                    if (o is byte[]) val = UTF8Encoding.ASCII.GetString(o as byte[]); // not sure why we get a byte array, but it happens.

        //                    if (Version.TryParse(val, out schemaVersion))
        //                    {

        //                    }
        //                }

        //            }
        //            if (schemaVersion < Utilities.DataUtility.CurrentVersion)
        //            {
        //                Utilities.DataUtility.UpgradeFrom(conn, schemaVersion, (msg) => { _doc.Application.WriteJournalComment(msg, false); });
        //            }

        //            // now we can get around to reading the actual headers.
        //            var headCmd = conn.CreateCommand();
        //            headCmd.CommandText = "select * from _objects_header";
        //            SQLiteDataReader reader = headCmd.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                string key = reader.GetString(1);
        //                string val = reader.GetString(2);
        //                _headerDict[key] = val;
        //            }

        //            conn.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _doc.Application.WriteJournalComment("Error reading headers: " + ex.GetType().Name + ": " + ex.Message, false);
        //    }

        //}

        //private void readParameters()
        //{
        //    using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + _dbFilename + ";Version=3;"))
        //    {
        //        conn.Open();

        //        var cmd = conn.CreateCommand();
        //        cmd.CommandText = "select id,name FROM _objects_attr";

        //        SQLiteDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            int id = reader.GetInt32(0);
        //            string name = reader.GetString(1);
        //            _parameterDict[id] = name;
        //        }

        //    }
        //}

        //private void readValues()
        //{
        //    using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + _dbFilename + ";Version=3;"))
        //    {
        //        conn.Open();

        //        var cmd = conn.CreateCommand();
        //        cmd.CommandText = "select id,value FROM _objects_val";

        //        SQLiteDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            int id = reader.GetInt32(0);
        //            string value = reader.GetString(1);
        //            _valueDict[id] = value;
        //        }

        //    }
        //}

        //private void readElements()
        //{
        //    using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + _dbFilename + ";Version=3;"))
        //    {
        //        conn.Open();

        //        var cmd = conn.CreateCommand();
        //        cmd.CommandText = "select id,entity_id,attribute_id,value_id FROM _objects_eav";

        //        using (SQLiteDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                int entity_id = reader.GetInt32(1);
        //                int attribute_id = reader.GetInt32(2);
        //                int value_id = reader.GetInt32(3);

        //                if (_idValues.ContainsKey(entity_id) == false) _idValues[entity_id] = new RevitElement() { ElementId = entity_id };

        //                _idValues[entity_id].ParameterValueIds[attribute_id] = value_id;
        //                _idValues[entity_id].Parameters[_parameterDict[attribute_id]] = _valueDict[value_id];

        //            }
        //        }


        //        /// read the ID information for each element.
        //        cmd = conn.CreateCommand();
        //        cmd.CommandText = "select id,external_id,category,isType FROM _objects_id";
        //        using (SQLiteDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                int id = reader.GetInt32(0);
        //                if (_idValues.ContainsKey(id) == false) _idValues[id] = new RevitElement() { ElementId = id };
        //                string guid = reader.GetString(1);
        //                string cat = reader.GetString(2);
        //                int isType = reader.GetInt32(3);

        //                var elem = _idValues[id];
        //                elem.Category = cat;
        //                elem.UniqueId = guid;
        //                elem.IsType = (isType == 1);
        //            }
        //        }
        //    }
        //}

        //private void readGeometry()
        //{
        //    using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + _dbFilename + ";Version=3;"))
        //    {
        //        conn.Open();

        //        var cmd = conn.CreateCommand();
        //        cmd.CommandText = "select id,BoundingBoxMin,BoundingBoxMax,Location,Location2,Level,Rotation FROM _objects_geom";

        //        using (SQLiteDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                int entity_id = reader.GetInt32(0);

        //                if (_idValues.ContainsKey(entity_id) == false) continue; // not sure how, but let's protect just in case.
        //                string bbMin = reader.GetString(1);
        //                string bbMax = reader.GetString(2);
        //                string lp = reader.GetString(3);
        //                string lp2 = reader.GetString(4);
        //                string levName = reader.GetString(5);
        //                float rot = reader.GetFloat(6);

        //                RevitElement elem = _idValues[entity_id];

        //                if (!String.IsNullOrEmpty(bbMin)) elem.BoundingBox = new BoundingBoxXYZ()
        //                {
        //                    Min = parsePoint(bbMin),
        //                    Max = parsePoint(bbMax)
        //                };
        //                if (!String.IsNullOrEmpty(lp)) elem.LocationPoint = parsePoint(lp);
        //                if (!String.IsNullOrEmpty(lp2)) elem.LocationPoint2 = parsePoint(lp2);

        //                elem.Level = levName;
        //                elem.Rotation = rot;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// From a string point, return an XYZ 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private XYZ parsePoint(string input)
        {
            if (String.IsNullOrEmpty(input)) return null;

            string[] pieces = input.Split(',');
            if (pieces.Length != 3) return null;

            return new XYZ(Double.Parse(pieces[0], CultureInfo.InvariantCulture), Double.Parse(pieces[1], CultureInfo.InvariantCulture), Double.Parse(pieces[2], CultureInfo.InvariantCulture));
        }
        #endregion
    }
}
