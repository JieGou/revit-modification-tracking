using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TrackDirect.UI
{
    public class ColorSettings
    {
        public Color NewElement { get; set; }
        public Color ChangeFamilyType { get; set; }
        public Color ChangeGeometry { get; set; }
        public Color ChangeVolumeLocation { get; set; }
        public Color ChangeRvtPara { get; set; }
        public Color ChangeSharedPara { get; set; }
    }
}
