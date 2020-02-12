using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using System.Globalization;
using Autodesk.Revit.UI;
using System.Security.Cryptography;
using System.Diagnostics;


namespace TrackDirect.Utilities
{
    internal static class RevitUtils
    {
        /// <summary>
        /// The SerializePoint
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static String SerializePoint(XYZ pt)
        {
            if (pt == null) return String.Empty;
            return pt.X.ToString(CultureInfo.InvariantCulture) + "," + pt.Y.ToString(CultureInfo.InvariantCulture) + "," + pt.Z.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// The SerializeLocation 
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string SerializeLocation(Element e)
        {
            string location = string.Empty;

            Location loc = e.Location;

            if (loc == null)
            {

                return location;
            }

            else
            {
                LocationPoint pt = loc as LocationPoint;
                if (pt != null)
                {

                    location = SerializePoint(pt.Point);

                }
                else
                {

                    LocationCurve crv = loc as LocationCurve;
                    if (crv != null)
                    {
                        if (crv.Curve.IsBound)
                        {
                            XYZ p1 = crv.Curve.GetEndPoint(0);
                            XYZ p2 = crv.Curve.GetEndPoint(1);
                            location = SerializePoint(p1) + ";" + SerializePoint(p2);
                        }
                    }

                }

                return location;
            }
        }

        public static string SerializeBoundingBox(BoundingBoxXYZ box)
        {
            if (box == null) return String.Empty;
            return $"{SerializePoint(box.Min)},{SerializePoint(box.Max)}";
        }

        public static BoundingBoxXYZ DeserializeBoundingBox(string input)
        {
            if (String.IsNullOrEmpty(input)) return null;

            string[] values = input.Split(',');

            if (values.Length == 6)
            {
                BoundingBoxXYZ box = new BoundingBoxXYZ()
                {
                    Min = new XYZ(Double.Parse(values[0], CultureInfo.InvariantCulture), Double.Parse(values[1], CultureInfo.InvariantCulture), Double.Parse(values[2], CultureInfo.InvariantCulture)),
                    Max = new XYZ(Double.Parse(values[3], CultureInfo.InvariantCulture), Double.Parse(values[4], CultureInfo.InvariantCulture), Double.Parse(values[5], CultureInfo.InvariantCulture))
                };

                return box;
            }

            return null;
        }


        public static string SerializeMove(XYZ from, XYZ to)
        {
            return from.X.ToString(CultureInfo.InvariantCulture) + "," + from.Y.ToString(CultureInfo.InvariantCulture) + "," + from.Z.ToString(CultureInfo.InvariantCulture) + "," +
                to.X.ToString(CultureInfo.InvariantCulture) + "," + to.Y.ToString(CultureInfo.InvariantCulture) + "," + to.Z.ToString(CultureInfo.InvariantCulture);

        }

        public static string SerializeDoubleMove(XYZ from1, XYZ to1, XYZ from2, XYZ to2)
        {
            // in the case where two locationpoints are moving, capture both, in an array of 12.
            return from1.X.ToString(CultureInfo.InvariantCulture) + "," + from1.Y.ToString(CultureInfo.InvariantCulture) + "," + from1.Z.ToString(CultureInfo.InvariantCulture) + "," +
                to1.X.ToString(CultureInfo.InvariantCulture) + "," + to1.Y.ToString(CultureInfo.InvariantCulture) + "," + to1.Z.ToString(CultureInfo.InvariantCulture) + "," +
                from2.X.ToString(CultureInfo.InvariantCulture) + "," + from2.Y.ToString(CultureInfo.InvariantCulture) + "," + from2.Z.ToString(CultureInfo.InvariantCulture) + "," +
                to2.X.ToString(CultureInfo.InvariantCulture) + "," + to2.Y.ToString(CultureInfo.InvariantCulture) + "," + to2.Z.ToString(CultureInfo.InvariantCulture);

        }

        public static string SerializeRotation(XYZ point, XYZ vector, float rotation)
        {
            return point.X.ToString(CultureInfo.InvariantCulture) + "," + point.Y.ToString(CultureInfo.InvariantCulture) + "," + point.Z.ToString(CultureInfo.InvariantCulture) + "," +
              vector.X.ToString(CultureInfo.InvariantCulture) + "," + vector.Y.ToString(CultureInfo.InvariantCulture) + "," + vector.Z.ToString(CultureInfo.InvariantCulture) + "," +
              rotation.ToString(CultureInfo.InvariantCulture);


        }

        public static bool DeSerializeRotation(string input, out XYZ point, out XYZ vector, out float rotation)
        {
            point = null; vector = null; rotation = -1;
            if (String.IsNullOrEmpty(input)) return false;

            string[] values = input.Split(',');
            if (values.Length != 7) return false;

            point = new XYZ(Double.Parse(values[0], CultureInfo.InvariantCulture), Double.Parse(values[1], CultureInfo.InvariantCulture), Double.Parse(values[2], CultureInfo.InvariantCulture));
            vector = new XYZ(Double.Parse(values[3], CultureInfo.InvariantCulture), Double.Parse(values[4]), Double.Parse(values[5], CultureInfo.InvariantCulture));
            rotation = float.Parse(values[6], CultureInfo.InvariantCulture);

            return true;
        }

        public static bool DeSerializeMove(string input, out XYZ from1, out XYZ to1, out XYZ from2, out XYZ to2)
        {
            from1 = null; to1 = null; from2 = null; to2 = null;

            if (String.IsNullOrEmpty(input)) return false;

            string[] values = input.Split(',');
            if ((values.Length != 6) && (values.Length != 12)) return false;

            from1 = new XYZ(Double.Parse(values[0], CultureInfo.InvariantCulture), Double.Parse(values[1], CultureInfo.InvariantCulture), Double.Parse(values[2], CultureInfo.InvariantCulture));
            to1 = new XYZ(Double.Parse(values[3], CultureInfo.InvariantCulture), Double.Parse(values[4], CultureInfo.InvariantCulture), Double.Parse(values[5], CultureInfo.InvariantCulture));

            if (values.Length == 12)
            {
                from2 = new XYZ(Double.Parse(values[6], CultureInfo.InvariantCulture), Double.Parse(values[7], CultureInfo.InvariantCulture), Double.Parse(values[8], CultureInfo.InvariantCulture));
                to2 = new XYZ(Double.Parse(values[9]), Double.Parse(values[10], CultureInfo.InvariantCulture), Double.Parse(values[11], CultureInfo.InvariantCulture));

            }

            return true;
        }

        /// <summary>
        /// Get list revit parameters
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        /// 
        public static List<Parameter> GetRevitParametersExcludeGeometryParam(Element e)
        {
            List<Parameter> parameters = new List<Parameter>();

            foreach (Parameter para in e.GetOrderedParameters())
            {
                //exclude geometryParameters
                if (para.IsShared == false)
                {
                    //exclude geometryParameters
                    if (
                        ((para.Definition.ParameterGroup == BuiltInParameterGroup.PG_CONSTRAINTS
                        || para.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY)
                        && para.StorageType != StorageType.Double)

                        || (para.Definition.ParameterGroup != BuiltInParameterGroup.PG_CONSTRAINTS
                        && para.Definition.ParameterGroup != BuiltInParameterGroup.PG_GEOMETRY)
                        )
                    {
                        parameters.Add(para);

                    }
                }
            }
            return parameters;
        }

        /// <summary>
        /// Get list geometry parameters
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        /// 
        public static List<Parameter> GetGeoParameters(Element e)
        {
            List<Parameter> parameters = new List<Parameter>();

            foreach (Parameter para in e.GetOrderedParameters())
            {
                if (para.IsShared == false
                    && para.StorageType == StorageType.Double
                    && (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_CONSTRAINTS
                    || para.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY))
                {
                    parameters.Add(para);
                }
            }

            return parameters;
        }

        /// <summary>
        /// Get list shared parameters
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static List<Parameter> GetSharedParameters(Element e, Document doc)
        {
            List<Parameter> parameters = new List<Parameter>();
            foreach (Parameter para in e.GetOrderedParameters())
            {

                if (para.IsShared == true)
                {
                    SharedParameterElement sharedParameter = doc.GetElement(para.Id) as SharedParameterElement;
                    if (sharedParameter.GetDefinition().Visible == true)
                    {
                        parameters.Add(para);
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        /// The SerializeRevitParameters
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string SerializeRevitParameters(Element e)
        {
            string parameters = "";
            var charsToRemove = new string[] { "<", ">" };
            IList<Parameter> paraList = e.GetOrderedParameters();

            foreach (Parameter para in e.GetOrderedParameters())
            {
                if (para.IsShared == false)
                {
                    string val = null;
                    //exclude geometryParameters
                    if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_CONSTRAINTS
                        || para.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY)
                    {
                        if (para.StorageType != StorageType.Double)
                        {
                            switch (para.StorageType)
                            {
                                case StorageType.String:
                                    val = para.AsString();
                                    break;

                                default:
                                    val = para.AsValueString();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (para.StorageType)
                        {
                            case StorageType.String:
                                val = para.AsString();
                                break;

                            default:
                                val = para.AsValueString();
                                break;
                        }
                    }

                    if (val == null) val = "(n/a)";
                    parameters += string.Format("{0}:{1}={2}", para.Id, para.Definition.Name, val) + ";";
                    foreach (var c in charsToRemove)
                    {
                        parameters = parameters.Replace(c, string.Empty);
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        /// get all parameter in group parameter constraints and geometry
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string SerializeGeoParameters(Element e)
        {
            string parameters = "";
            foreach (Parameter para in e.GetOrderedParameters())
            {
                if (para.IsShared == false
                    && para.StorageType == StorageType.Double
                    && (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_CONSTRAINTS
                    || para.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY))
                {
                    string val = para.AsDouble().ToString();

                    parameters += string.Format("{0}:{1}={2}", para.Id, para.Definition.Name, val) + ";";
                }
            }

            return parameters;
        }

        /// <summary>
        /// The SerializeSharedParameters
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string SerializeSharedParameters(Element e, Document doc)
        {
            string parameters = "";
            IList<Parameter> paraList = e.GetOrderedParameters();

            //List shared parameter info data tracking
            string pCreatedDateName = VCFParameters.VCF_CreateAt.ToString();
            string pModifiedDateName = VCFParameters.VCF_ModifyAt.ToString();
            string pChangeTypeName = VCFParameters.VCF_ChangeType.ToString();
            string pUserName = VCFParameters.VCF_User.ToString();

            var TrackSharedParameters = new[]
                {
                pCreatedDateName,
                pModifiedDateName,
                pUserName,
                pChangeTypeName
            };


            foreach (Parameter para in e.GetOrderedParameters())
            {

                if (para.IsShared == true)
                {
                    SharedParameterElement sharedParameter = doc.GetElement(para.Id) as SharedParameterElement;
                    if (sharedParameter.GetDefinition().Visible == true && !TrackSharedParameters.Contains(para.Definition.Name))
                    {
                        parameters += string.Format("{0}:{1}={2}", para.Id, para.Definition.Name, RevitUtils.ParameterToString(para)) + ";";
                    }

                }
            }

            return parameters;
        }


        /// <summary>
        /// Helper function: return a string form of a given parameter.
        /// </summary>
        /// <param name="param">The param<see cref="Parameter"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string ParameterToString(Parameter param)
        {
            string val = "none";

            if (param == null)
            {
                return val;
            }

            // To get to the parameter value, we need to parse it depending on its storage type

            switch (param.StorageType)
            {
                case StorageType.Double:
                    double dVal = param.AsDouble();
                    val = dVal.ToString();
                    break;
                case StorageType.Integer:
                    int iVal = param.AsInteger();
                    val = iVal.ToString();
                    break;
                case StorageType.String:
                    string sVal = param.AsString();
                    val = sVal;
                    break;
                case StorageType.ElementId:
                    ElementId idVal = param.AsElementId();
                    val = idVal.IntegerValue.ToString();
                    break;

                case StorageType.None:
                    break;
            }
            return val;
        }

        /// <summary>
        /// The Get List parameter of project
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="Dictionary{string, string}"/></returns>
        public static IList<Parameter> GetParameters(Element e)
        {
            IList<Parameter> parms = e.GetOrderedParameters();

            // add a couple more that we might be interested in...
            if (e is TextNote)
            {
                Parameter text = e.get_Parameter(BuiltInParameter.TEXT_TEXT);
                if (text != null) parms.Add(text);
            }

            return parms;
        }


        /// <summary>
        /// The GetAllParametersElement
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="Dictionary{string, string}"/></returns>
        public static Dictionary<string, string> GetAllParametersElement(Element e)
        {
            //Key is parameter name, value is paramter value
            Dictionary<string, string> dic = new Dictionary<string, string>();

            IList<Parameter> parameters = e.GetOrderedParameters();
            List<string> param_values = new List<string>(parameters.Count);
            foreach (Parameter p in parameters)
            {
                // AsValueString displays the value as the 
                // user sees it. In some cases, the underlying
                // database value returned by AsInteger, AsDouble,
                // etc., may be more relevant.

                param_values.Add(string.Format("{0}={1}",
                  p.Definition.Name, p.AsValueString()));
            }
            foreach (Parameter p in parameters)
            {
                var xxx = p.AsValueString();
                var xx = p.Definition.ParameterType;

                dic.Add(p.Id.ToString(), ParameterToString(p));
            }

            return dic;
        }

        public static IList<Document> GetCurrentDocumentAndLinks(Document active)
        {
            List<Document> allDocs = new List<Document>();
            allDocs.Add(active);
            FilteredElementCollector coll = new FilteredElementCollector(active);
            coll.OfClass(typeof(RevitLinkInstance));

            IList<RevitLinkInstance> instances = coll.Cast<RevitLinkInstance>().ToList();

            foreach (var instance in instances)
            {
                Document linkDoc = instance.GetLinkDocument();
                if (linkDoc != null) allDocs.Add(linkDoc);
            }

            return allDocs;
        }

        public static string GetModelPath(Document doc)
        {
            if (doc == null) return String.Empty;

            if (doc.IsWorkshared)
            {

                ModelPath mp = doc.GetWorksharingCentralModelPath();
                return ModelPathUtils.ConvertModelPathToUserVisiblePath(mp);
            }
            else
            {
                return doc.PathName;
            }
        }


        public static Level GetNextLevelDown(XYZ pt, IList<Level> levels)
        {
            if (pt == null) return null;
            if (levels == null) return null;

            // we want the next level down from the z value...
            IList<Level> levelsBelow = levels.Where(v => v.Elevation <= pt.Z).ToList();
            Level lev = null;
            if (levelsBelow.Count == 0) return null;
            if (levelsBelow.Count == 1) return levelsBelow[0];

            // otherwise look for which one is closest.
            lev = levelsBelow.Aggregate((x, y) => Math.Abs(x.Elevation - pt.Z) < Math.Abs(y.Elevation - pt.Z) ? x : y);

            return lev;
        }

        public static IList<Document> GetProjectsInMemory(Autodesk.Revit.ApplicationServices.Application app)
        {
            List<Document> docs = new List<Document>();
            foreach (Document doc in app.Documents)
            {
                if (doc.IsFamilyDocument == false) docs.Add(doc);
            }

            return docs;
        }

        internal static void GetExtents(Autodesk.Revit.UI.UIApplication uiApp, out int x, out int y)
        {
            //2017+
            try
            {
                x = uiApp.DrawingAreaExtents.Left;
                y = uiApp.DrawingAreaExtents.Top;
            }
            catch
            {
                x = 10;
                y = 10;
            }
        }

        // <summary>
        /// The GetDocumentByTitle
        /// </summary>
        /// <param name="ui_app">The ui_app<see cref="UIApplication"/></param>
        /// <param name="TitreDocument">The TitreDocument<see cref="string"/></param>
        /// <returns>The <see cref="Document"/></returns>
        public static Document GetDocumentByTitle(UIApplication ui_app, string TitreDocument)
        {
            foreach (Document doc in ui_app.Application.Documents)
            {
                if (doc.Title == TitreDocument)
                {
                    return doc;
                }
            }

            return null;
        }

        //Get levels of document
        public static IList<Level> GetLevels(Document doc)
        {
            return new FilteredElementCollector(doc)
                  .OfClass(typeof(Level))
                  .Cast<Level>()
                  .OrderBy(lev => lev.ProjectElevation).ToList();
        }

        /// <summary>
        /// The GetLevelElement
        /// </summary>
        /// <param name="levels">The levles<see cref="IList{Level}"/></param>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetLevelName(IList<Level> levels, Element e)
        {
            string level = "";

            //Get level element
            if (e.LevelId.ToString() == "-1")
            {
                level = levels[0].Name;
                for (int i = 0; i < levels.Count; i++)
                {
                    try
                    {
                        double x = e.get_BoundingBox(null).Min.Z;
                        if (x >= levels[i].ProjectElevation)
                            level = levels[i].Name;
                    }
                    catch
                    {
                        level = "No Level";
                    }
                }
            }
            else
            {
                foreach (Level lv in levels)
                {
                    if (lv.Id == e.LevelId)
                        level = lv.Name;
                }
            }
            return level;
        }

        #region String formatting
        /// <summary>
        /// Convert a string to a byte array.
        /// </summary>
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)]; //howManyByte
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Return a string for a real number
        /// formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.###");
        }

        /// <summary>
        /// Return a string for an XYZ point
        /// or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(XYZ p)
        {
            return $"{RealString(p.X)},{RealString(p.Y)},{RealString(p.Z)}";
        }

        /// <summary>
        /// Return a string for this bounding box
        /// with its coordinates formatted to two
        /// decimal places.
        /// </summary>
        public static string BoundingBoxString(BoundingBoxXYZ bb)
        {
            try
            {
                return $"{PointString(bb.Min)},{PointString(bb.Max)}";
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// Return a string for this point array
        /// with its coordinates formatted to two
        /// decimal places.
        /// </summary>
        public static string PointArrayString(IList<XYZ> pts)
        {
            return string.Join(", ",
              pts.Select<XYZ, string>(
                p => PointString(p)));
        }

        /// <summary>
        /// Return a string for this curve with its
        /// tessellated point coordinates formatted
        /// to two decimal places.
        /// </summary>
        public static string CurveTessellateString(
          Curve curve)
        {
            return PointArrayString(curve.Tessellate());
        }

        /// <summary>
        /// Return a string for this curve with its
        /// tessellated point coordinates formatted
        /// to two decimal places.
        /// </summary>
        public static string LocationString(
          Location location)
        {
            LocationPoint lp = location as LocationPoint;
            LocationCurve lc = (null == lp)
              ? location as LocationCurve
              : null;

            return null == lp ? (null == lc ? null : CurveTessellateString(lc.Curve))
              : PointString(lp.Point);
        }

        /// <summary>
        /// Return a string describing the given element:
        /// .NET type name,
        /// category name,
        /// family and symbol name for a family instance,
        /// element id and element name.
        /// </summary>
        public static string GetElementDescription(Element e)
        {
            if (null == e)
            {
                return "<null>";
            }

            // For a wall, the element name equals the
            // wall type name, which is equivalent to the
            // family name ...

            FamilyInstance fi = e as FamilyInstance;

            string typeName = e.GetType().Name;

            string categoryName = (null == e.Category)
              ? string.Empty
              : e.Category.Name;

            string familyName = (null == fi)
              ? string.Empty
              : fi.Symbol.Family.Name;

            string symbolName = (null == fi
              || e.Name.Equals(fi.Symbol.Name))
                ? string.Empty
                : fi.Symbol.Name;
            return $"{typeName} {categoryName} {familyName} {symbolName}<{e.Id.IntegerValue} {e.Name}>";

        }

        public static string ElementDescription(Document doc, int eleId)
        {
            return GetElementDescription(doc.GetElement(new ElementId(eleId)));
        }
        #endregion // String formatting


        #region Retrieve solid vertices


        /// <summary>
        /// Add the vertices of the given solid to 
        /// the vertex lookup dictionary.
        /// </summary>
        public static void AddVertices(
          Dictionary<XYZ, int> vertexLookup, Transform t, Solid s)
        {
            //Debug.Assert(0 < s.Edges.Size, "expected a non-empty solid");
            try
            {
                foreach (Face f in s.Faces)
                {
                    Mesh m = f.Triangulate();

                    if (m is null) continue;
                    foreach (XYZ p in m.Vertices)
                    {
                        XYZ q = t.OfPoint(p);
                        if (!vertexLookup.ContainsKey(q))
                        {
                            vertexLookup.Add(q, 1);
                        }
                        else
                        {
                            ++vertexLookup[q];
                        }
                    }
                }
            }
            catch { }

        }

        /// <summary>
        /// Recursively add vertices of all solids found
        /// in the given geometry to the vertex lookup.
        /// Untested!
        /// </summary>
        public static void AddVertices(
          Dictionary<XYZ, int> vertexLookup, Transform t, GeometryElement geo)
        {
            //if (geo == null)
            //{
            //    Debug.Assert(geo != null, "null GeometryElement");
            //    throw new System.ArgumentException("null GeometryElement");
            //}
            if (geo != null)
            {
                foreach (GeometryObject obj in geo)
                {
                    Solid solid = obj as Solid;

                    if (solid != null)
                    {
                        if (0 < solid.Faces.Size)
                        {
                            AddVertices(vertexLookup, t, solid);
                        }
                    }
                    else
                    {
                        GeometryInstance inst = obj as GeometryInstance;

                        if (inst is null) continue;
                        //GeometryElement geoi = inst.GetInstanceGeometry();
                        GeometryElement geos = inst.GetSymbolGeometry();

                        //Debug.Assert( null == geoi || null == geos,
                        //  "expected either symbol or instance geometry, not both" );

                        //Debug.Assert(inst.Transform != null,
                        //    "null inst.Transform");

                        //Debug.Assert( null != inst.GetSymbolGeometry(),
                        //  "null inst.GetSymbolGeometry" );

                        if (geos != null)
                        {
                            AddVertices(vertexLookup,
                                inst.Transform.Multiply(t),
                                geos);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return a sorted list of all unique vertices 
        /// of all solids in the given element's geometry
        /// in lexicographical order.
        /// </summary>
        public static List<XYZ> GetCanonicVertices(Element e)
        {
            try
            {
                GeometryElement geo = e.get_Geometry(new Options());
                Transform t = Transform.Identity;

                Dictionary<XYZ, int> vertexLookup = new Dictionary<XYZ, int>(new XyzEqualityComparer());

                AddVertices(vertexLookup, t, geo);

                List<XYZ> keys = new List<XYZ>(vertexLookup.Keys);

                keys.Sort(GeometricalUtils.Compare);

                return keys;
            }
            catch { }
            return new List<XYZ>();
        }
        #endregion // Retrieve solid vertices
    }
}

