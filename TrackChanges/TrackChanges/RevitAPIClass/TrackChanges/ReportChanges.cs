using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TrackChanges
{
    public class ReportChange
    {
        #region Store element state
        /// <summary>
        /// Return a string representing the given element
        /// state. This is the information you wish to track.
        /// It is up to you to ensure that all data you are
        /// interested in really is included in this snapshot.
        /// In this case, we ignore all elements that do not
        /// have a valid bounding box.
        /// </summary>
        public static string GetElementState(Element e)
        {
            string s = null;

            BoundingBoxXYZ bb = e.get_BoundingBox(null);

            if (null != bb)
            {
                List<string> properties = new List<string>
                {
                    StringUtils.ElementDescription(e)+ " at " + StringUtils.LocationString(e.Location)
                };


                if (!(e is FamilyInstance))
                {
                    properties.Add("Box="+ StringUtils.BoundingBoxString(bb));

                    properties.Add("Vertices="+ StringUtils.PointArrayString(SolidUtils.GetCanonicVertices(e)));
                }

                properties.Add("Parameters="+ StringUtils.GetPropertiesJson(e.GetOrderedParameters()));

                s = string.Join(", ", properties);

                //Debug.Print( s );
            }
            return s;
        }
        #endregion // Store element state


        #region Creating a Database State Snapshot
        /// <summary>
        /// Return a dictionary mapping element id values
        /// to hash codes of the element state strings. 
        /// This represents a snapshot of the current 
        /// database state.
        /// </summary>
        public static Dictionary<int, string> GetSnapshot(IEnumerable<Element> a)
        {
            Dictionary<int, string> d = new Dictionary<int, string>();

            SHA256 hasher = SHA256Managed.Create();

            foreach (Element e in a)
            {
                //Debug.Print( e.Id.IntegerValue.ToString() 
                //  + " " + e.GetType().Name );

                string s = GetElementState(e);

                if (null != s)
                {
                    string hashb64 = Convert.ToBase64String(hasher.ComputeHash(StringUtils.GetBytes(s)));

                    d.Add(e.Id.IntegerValue, hashb64);
                    //d.Add(e.UniqueId, hashb64);
                }
            }
            return d;
        }
        #endregion // Creating a Database State Snapshot

  
        #region Report differences
        /// <summary>
        /// Compare the start and end states and report the 
        /// differences found. In this implementation, we
        /// just store a hash code of the element state.
        /// If you choose to store the full string 
        /// representation, you can use that for comparison,
        /// and then report exactly what changed and the
        /// original values as well.
        /// </summary>
        public static void ReportDifferences(
          Document doc,
          Dictionary<int, string> start_state,
          Dictionary<int, string> end_state)
        {
            int n1 = start_state.Keys.Count;
            int n2 = end_state.Keys.Count;

            List<int> keys = new List<int>(start_state.Keys);

            //new created/added elements
            //loop for each key in start_state
            foreach (int id in end_state.Keys)
            {
                if (!keys.Contains(id))
                {
                    keys.Add(id);
                }
            }

            keys.Sort();

            int n = keys.Count;

            Debug.Print($"{n1} elements before, {n2} elements after, {n} total");

            int nAdded = 0;
            int nDeleted = 0;
            int nModified = 0;
            int nIdentical = 0;
            List<string> report = new List<string>();
            string pCreatedDate = "CreatedDate";
            string pModifiedDate = "ModifiedDate";
            string pDesctiption = "DescriptionChange";
            string date = DateTime.Now.Date.ToString("d");
            
            //keys are the new elements in end state
            foreach (int id in keys)
            {
                //New elements
                if (!start_state.ContainsKey(id))
                {
                    ++nAdded;
                    report.Add(id.ToString() + " added " + StringUtils.ElementDescription(doc, id));
                    //Add date created to parameter DateCreate
                    //How to check the change: by geometry or by parameter
                    ElementId elemId = new ElementId(id);
                    Element ele = doc.GetElement(elemId);

                    IList<Parameter> param = ele.GetParameters(nameof(pCreatedDate));
                    Parameter param1 = ele.LookupParameter("CreatedDate");
                    Parameter param2 = ele.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    int countParam = param.Count;
                    Debug.Assert(1 >= countParam, "expected maximum one shared parameters " + "named " + nameof(pCreatedDate));
                    //Set parameter
                    using (Transaction trans = new Transaction(doc, "Set Parameter"))
                    {
                        trans.Start();
                        try
                        {
                            if (countParam > 0)
                                param[0].Set(date);
                            if (param1 != null)
                                param1.Set(date);
                            if (param2 != null)
                                param2.Set(date);
                        }
                        catch{}
                        trans.Commit();
                    }
                    
                    

                }
                //Deleted elements
                else if (!end_state.ContainsKey(id))
                {
                    ++nDeleted;
                    report.Add(id.ToString() + " deleted");
                    //Add nothing
                }
                //Modified elements
                else if (start_state[id] != end_state[id])
                {
                    ++nModified;
                    report.Add(id.ToString() + " modified "
                      + StringUtils.ElementDescription(doc, id));
                    //Add modified date to paremeter
                    ElementId elemId = new ElementId(id);
                    Element ele = doc.GetElement(elemId);

                    IList<Parameter> param = ele.GetParameters(nameof(pModifiedDate));
                    Parameter param1 = ele.LookupParameter("ModifiedDate");
                    Parameter param2 = ele.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    int countParam = param.Count;
                    Debug.Assert(1 >= countParam, "expected maximum one shared parameters " + "named " + nameof(pCreatedDate));

                    using (Transaction trans = new Transaction(doc, "Set Parameter"))
                    {
                        trans.Start();
                        try
                        {
                            if (countParam > 0)
                                param[0].Set(date);
                            if (countParam > 0)
                                param[0].Set(date);
                            if (param1 != null)
                                param1.Set(date);
                            if (param2 != null)
                                param2.Set(date);
                        }
                        catch { }
                        trans.Commit();
                    }
                }
                else
                {
                    ++nIdentical;
                }
            }

            string msg = $"Stopped tracking changes now.\r\n"
              + $"{nDeleted} deleted, {nAdded} added, {nModified} modified, "
              + $"{nIdentical} identical elements:";

            string s = string.Join("\r\n", report);

            Debug.Print(msg + "\r\n" + s);
            TaskDialog dlg = new TaskDialog("Track Changes") { MainInstruction = msg, MainContent = s };
            dlg.Show();
        }
        #endregion // Report differences
    }
}
