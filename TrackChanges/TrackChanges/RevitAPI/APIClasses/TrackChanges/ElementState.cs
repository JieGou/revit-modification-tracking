using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TrackChanges
{
    public class ElementState
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
        static string GetElementState(Element e)
        {
            string s = null;

            BoundingBoxXYZ bb = e.get_BoundingBox(null);

            if (null != bb)
            {
                List<string> properties = new List<string>
                {
                    StringUtils.ElementDescription(e)
                    + " at " + StringUtils.LocationString(e.Location)
                };


                if (!(e is FamilyInstance))
                {
                    properties.Add("Box="
                                   + StringUtils.BoundingBoxString(bb));

                    properties.Add("Vertices="
                                   + StringUtils.PointArrayString(  SolidUtils.GetCanonicVertices(e)));
                }

                properties.Add("Parameters="
                               + StringUtils.GetPropertiesJson(e.GetOrderedParameters()));

                s = string.Join(", ", properties);

                //Debug.Print( s );
            }
            return s;
        }
        #endregion // Store element state
    }
}
