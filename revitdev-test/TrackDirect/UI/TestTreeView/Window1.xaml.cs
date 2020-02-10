using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TrackDirect.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            FooViewModel vm = new FooViewModel();
            this.DataContext = vm;
            FooViewModel root = this.tree.Items[0] as FooViewModel;
            this.tree.Focus();
        }
    }
}
