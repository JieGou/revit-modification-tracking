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
        private HighlightElementExEvent _hEvent;
        private GetListElementsExEvent _getListElementEvent;
        private ColorElementInViewEvent _colorElementInViewEvent;

        #region Declare all the external event for the wpf window
        public WindowTest()
        {
            InitializeComponent();
        }
        public WindowTest(ExternalEvent eEvent, HighlightElementExEvent handlerEvent)
        {
            InitializeComponent();
            _ExtEvent = eEvent;
            _hEvent = handlerEvent;
        }
        public WindowTest(ExternalEvent eEvent, GetListElementsExEvent handlerEvent)
        {
            InitializeComponent();
            _ExtEvent = eEvent;
            _getListElementEvent = handlerEvent;
        }
        public WindowTest(ExternalEvent eEvent, ColorElementInViewEvent handlerEvent)
        {
            InitializeComponent();
            _ExtEvent = eEvent;
            _colorElementInViewEvent = handlerEvent;
        }
        #endregion


    }
}
