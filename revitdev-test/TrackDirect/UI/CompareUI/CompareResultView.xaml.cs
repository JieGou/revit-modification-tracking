using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace TrackDirect.UI
{
    /// <summary>
    /// Interaction logic for Compare.xaml
    /// </summary>
    public partial class CompareResultView : Window
    {
        public CompareResultView()
        {
            InitializeComponent();
            CompareResultViewModel root = this.treeComapreResult.Items[0] as CompareResultViewModel;
            this.treeComapreResult.Focus();
            CompareResultViewModel vm = new CompareResultViewModel();
            this.DataContext = vm;
        }
        
    }
}
