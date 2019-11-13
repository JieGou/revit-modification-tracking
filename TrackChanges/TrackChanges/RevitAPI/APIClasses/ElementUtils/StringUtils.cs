using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TrackChanges
{
    public class StringUtils
    {
        #region String formatting
        /// <summary>
        /// Convert a string to a byte array.
        /// </summary>
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)]; //howManyByte
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        #region OBSOLETE
        /// <summary>
        /// Define a project identifier for the 
        /// given Revit document.
        /// </summary>
        public static string GetProjectIdentifier(
          Document doc)
        {
            SHA256 hasher = SHA256Managed.Create();
            string key = System.Environment.MachineName + ":" + doc.PathName;
            byte[] hashValue = hasher.ComputeHash(GetBytes(key));
            string hashb64 = Convert.ToBase64String(hashValue);
            return hashb64.Replace('/', '_');
        }
        #endregion // OBSOLETE

        /// <summary>
        /// Return a string for a real number
        /// formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.##");
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
            return $"{PointString(bb.Min)},{PointString(bb.Max)}";
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

            return null == lp
              ? (null == lc
                ? null
                : CurveTessellateString(lc.Curve))
              : PointString(lp.Point);
        }

        /// <summary>
        /// Return a JSON string representing a dictionary
        /// of the given parameter names and values.
        /// </summary>
        public static string GetPropertiesJson(
          IList<Parameter> parameters)
        {
            int n = parameters.Count;
            List<string> a = new List<string>(n);
            foreach (Parameter p in parameters)
            {
                a.Add($"\"{p.Definition.Name}\":\"{p.AsValueString()}\"");
            }
            a.Sort();
            string s = string.Join(",", a);
            return "{" + s + "}";
        }

        /// <summary>
        /// Return a string describing the given element:
        /// .NET type name,
        /// category name,
        /// family and symbol name for a family instance,
        /// element id and element name.
        /// </summary>
        public static string ElementDescription(Element e)
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

        public static string ElementDescription(Document doc,int eleId)
        {
            return ElementDescription(doc.GetElement(new ElementId(eleId)));
        }
        #endregion // String formatting

    }
}
