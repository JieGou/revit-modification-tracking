using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrackChanges
{
    /// <summary>
    /// Interaction logic for WindowTest.xaml
    /// </summary>
    public partial class WindowTest : Window
    {
        public static ExternalEvent _ExtEvent { get; private set; } //public to invoke it when binding with the external event command
        private HighlightElementEvent _hEvent;

        public WindowTest(ExternalEvent eEvent, HighlightElementEvent hEvent)
        {
            InitializeComponent();
            _ExtEvent = eEvent;
            _hEvent = hEvent;

        }
       
    }
}
