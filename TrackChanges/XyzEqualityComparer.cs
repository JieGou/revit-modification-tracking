using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TrackChanges
{
    /// <summary>
    /// Define equality between XYZ objects, ensuring 
    /// that almost equal points compare equal.
    /// </summary>
    public class XyzEqualityComparer : IEqualityComparer<XYZ>
    {
        public bool Equals(XYZ p, XYZ q)
        {
            return p.IsAlmostEqualTo(q);
        }

        public int GetHashCode(XYZ p)
        {
            return StringUtils.PointString(p).GetHashCode();
        }
    }
}
