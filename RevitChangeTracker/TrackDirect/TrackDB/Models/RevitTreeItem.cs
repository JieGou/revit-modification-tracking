using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TrackDirect.Models
{
    public class RevitTreeItem
    {
        public string Name { get; set; }
        public RevitElement RvtElemData { get; set; }
        public ObservableCollection<RevitTreeItem> Items { get; set; }
       


        public RevitTreeItem()
        {
            this.Items = new ObservableCollection<RevitTreeItem>(); //Avoid the null item
        }
    }
}
