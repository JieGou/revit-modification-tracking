using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackChanges
{
    public class ElementRevit
    {
        public ElementRevit()
        {
            this.Items = new ObservableCollection<ElementRevit>();
        }
        public ElementId Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string FamilyType { get; set; }
        public ObservableCollection<ElementRevit> Items { get; set; }
        public Level Level { get; set; }
        public string IdName
        {
            get => $"{Id.ToString()} - {Name}";
            set
            {
                IdName = value;
            }
        }
    }
}
