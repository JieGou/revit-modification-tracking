using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media;
using System.Runtime.InteropServices;
using Autodesk.Windows;

namespace TD.Core
{
    public static class RevitUtils
    {

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
            //string pCreatedDateName = VCFParameters.VCF_CreateAt.ToString();
            //string pModifiedDateName = VCFParameters.VCF_ModifyAt.ToString();
            //string pChangeTypeName = VCFParameters.VCF_ChangeType.ToString();
            //string pUserName = VCFParameters.VCF_User.ToString();

            //var TrackSharedParameters = new[]
            //    {
            //    pCreatedDateName,
            //    pModifiedDateName,
            //    pUserName,
            //    pChangeTypeName
            //};


            foreach (Parameter para in e.GetOrderedParameters())
            {

                if (para.IsShared == true)
                {
                    SharedParameterElement sharedParameter = doc.GetElement(para.Id) as SharedParameterElement;
                    if (sharedParameter.GetDefinition().Visible == true /*&& !TrackSharedParameters.Contains(para.Definition.Name)*/)
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


       

        public static IList<Document> GetProjectsInMemory(Autodesk.Revit.ApplicationServices.Application app)
        {
            List<Document> docs = new List<Document>();
            foreach (Document doc in app.Documents)
            {
                if (doc.IsFamilyDocument == false) docs.Add(doc);
            }

            return docs;
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
        public static string GetLevelName(Document doc, IList<Level> levels, Element e)
        {
            string level = "";
            //Get level element
            if (e.LevelId.ToString() != "-1")
            {
                return levels.Where(x => x.Id == e.LevelId).Select(x => x.Name).FirstOrDefault();
            }

            if (e.LevelId.ToString() == "-1")
            {
                //Attention Structural framing do not have the LevelId
                if (e.Category.Name == Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming).Name) //Is beam instance
                {
                    Parameter levelParam = e.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                    if (levelParam != null)
                    {
                        ElementId levelId = levelParam.AsElementId();
                        if (ElementId.InvalidElementId != levelId && null != levelId)
                        {
                            Level levelBeam = doc.GetElement(levelId) as Level;
                            return levelBeam.Name;
                        }
                    }
                }
                else
                {
                    try
                    {
                        double minZ = e.get_BoundingBox(null).Min.Z;
                        return levels.Where(x => minZ >= x.ProjectElevation).Select(x => x.Name).FirstOrDefault();

                    }
                    catch { }
                }
            }
            return level;
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

        public static System.Windows.Media.Brush ConvertColorRevitToWPF(Autodesk.Revit.DB.Color colorRevit)
        {
            System.Windows.Media.Brush colorWPF = System.Windows.Media.Brushes.Red;
            try
            {
                System.Drawing.Color colorSystem = System.Drawing.Color.FromArgb(colorRevit.Red, colorRevit.Green, colorRevit.Blue);
                colorWPF = new SolidColorBrush(System.Windows.Media.Color.FromArgb(colorSystem.A, colorSystem.R, colorSystem.G, colorSystem.B));
            }
            catch { }
            return colorWPF;
        }

        #region Triggering External Event Execute by Setting Focus
        //Thanks for solution:
        //https://thebuildingcoder.typepad.com/blog/2013/12/triggering-immediate-external-event-execute.html
        //https://github.com/jeremytammik/RoomEditorApp/tree/master/RoomEditorApp
        //https://thebuildingcoder.typepad.com/blog/2016/03/implementing-the-trackchangescloud-external-event.html#5

        /// <summary>
        /// The GetForegroundWindow function returns a 
        /// handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Move the window associated with the passed 
        /// handle to the front.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(
          IntPtr hWnd);

        public static void SetFocusToRevit()
        {
            IntPtr hRevit = ComponentManager.ApplicationWindow;
            IntPtr hBefore = GetForegroundWindow();

            if (hBefore != hRevit)
            {
                SetForegroundWindow(hRevit);
                SetForegroundWindow(hBefore);
            }
        }

        #endregion
    }
}

