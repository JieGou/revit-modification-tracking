using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

using Autodesk.Revit.UI;

namespace TrackChanges
{
    public class SolidUtils
    {
        #region Retrieve solid vertices
        

        /// <summary>
        /// Add the vertices of the given solid to 
        /// the vertex lookup dictionary.
        /// </summary>
        public static void AddVertices(
          Dictionary<XYZ, int> vertexLookup,Transform t,Solid s)
        {
            Debug.Assert(0 < s.Edges.Size,"expected a non-empty solid");

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

        /// <summary>
        /// Recursively add vertices of all solids found
        /// in the given geometry to the vertex lookup.
        /// Untested!
        /// </summary>
        public static void AddVertices(
          Dictionary<XYZ, int> vertexLookup,Transform t,GeometryElement geo)
        {
            if (geo is null)
            {
                Debug.Assert(geo != null, "null GeometryElement");
                throw new System.ArgumentException("null GeometryElement");
            }

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

                    Debug.Assert( inst.Transform != null,
                        "null inst.Transform");

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

        #region OBSOLETE
        /// <summary>
        /// Retrieve the first non-empty solid found for 
        /// the given element. In case the element is a 
        /// family instance, it may have its own non-empty
        /// solid, in which case we use that. Otherwise we 
        /// search the symbol geometry. If we use the 
        /// symbol geometry, we have to keep track of the 
        /// instance transform to map it to the actual
        /// instance project location.
        /// </summary>
        static Solid GetSolid2(Element e, Options opt)
        {
            GeometryElement geo = e.get_Geometry(opt);

            Dictionary<XYZ, int> a = new Dictionary<XYZ, int>(new XyzEqualityComparer());

            Solid solid = null;
            GeometryInstance inst = null;
            Transform t = Transform.Identity;

            // Some family elements have no own solids, so we
            // retrieve the geometry from the symbol instead; 
            // others do have own solids on the instance itself 
            // and no contents in the instance geometry 
            // (e.g. in rst_basic_sample_project.rvt).

            foreach (GeometryObject obj in geo)
            {
                solid = obj as Solid;

                if (null != solid && 0 < solid.Faces.Size)
                    break;

                inst = obj as GeometryInstance;
            }

            if (null == solid && null != inst)
            {
                geo = inst.GetSymbolGeometry();
                t = inst.Transform;

                foreach (GeometryObject obj in geo)
                {
                    solid = obj as Solid;

                    if (null != solid
                      && 0 < solid.Faces.Size)
                    {
                        break;
                    }
                }
            }
            return solid;
        }
        #endregion // OBSOLETE

        /// <summary>
        /// Return a sorted list of all unique vertices 
        /// of all solids in the given element's geometry
        /// in lexicographical order.
        /// </summary>
        public static List<XYZ> GetCanonicVertices(Element e)
        {
            GeometryElement geo = e.get_Geometry(new Options());
            Transform t = Transform.Identity;

            Dictionary<XYZ, int> vertexLookup = new Dictionary<XYZ, int>(new XyzEqualityComparer());

            AddVertices(vertexLookup, t, geo);

            List<XYZ> keys = new List<XYZ>(vertexLookup.Keys);

            keys.Sort(GeometricalUtils.Compare);

            return keys;
        }
        #endregion // Retrieve solid vertices
    }
}
