using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TrackChanges
{
    public class MenuTreeItem
    {
        public string Name { get; set; }
        public Category CategoryRvt{ get; set; }
        public FamilyType FamilySymbolRvt { get; set; }
        public string ParameterRvt { get; set; }
        public ObservableCollection<MenuTreeItem> Items { get; set; }


        public MenuTreeItem()
        {
            this.Items = new ObservableCollection<MenuTreeItem>(); //To ensure we don't have the null item

        }

    }
}
