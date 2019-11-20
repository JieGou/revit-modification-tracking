using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;


namespace TrackChanges
{
    public class GeometricalUtils
    {
        #region Geometrical Comparison
        const double _eps = 1.0e-9;

        public static double Eps => _eps;

        public static double MinLineLength => _eps;

        public static double TolPointOnPlane => _eps;

        public static bool IsZero(
            double a,
            double tolerance)
        {
            return tolerance > Math.Abs(a);
        }

        public static bool IsZero(double a)
        {
            return IsZero(a, _eps);
        }

        public static bool IsEqual(double a, double b)
        {
            return IsZero(b - a);
        }

        public static int Compare(double a, double b)
        {
            return IsEqual(a, b) ? 0 : (a < b ? -1 : 1);
        }

        public static int Compare(XYZ p, XYZ q)
        {
            int d = Compare(p.X, q.X);

            if (0 != d) return d;
            d = Compare(p.Y, q.Y);

            if (0 != d) return d;
            d = Compare(p.Z, q.Z);
            return d;
        }
        #endregion // Geometrical Comparison
    }
}
