// ##########################################
// Solution: KTR
// Project: KTR.Main
// File: TreeView4TierView.xaml.cs
// 
// Last User: Chris Hildebran
// Last Edit: 2019-02-20 7:23 AM
// ##########################################
namespace KTR.Main.TreeViewAllElementInstances.V
{
    using System;
    using System.Windows;

    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    using KTR.Main.TreeViewAllElementInstances.VM;
    //using KTR.Utility;

    using Application = Autodesk.Revit.ApplicationServices.Application;

    [Transaction(TransactionMode.Manual)]
    public sealed partial class TreeView4TierView : Window, IExternalCommand
    {
        #region Fields

        private ExternalCommandData _commandData;

        private Application _revitApp;

        private Document _revitDoc;

        private UIApplication _revitUiApp;

        private UIDocument _revitUiDoc;

        #endregion

        #region Constructors

        public TreeView4TierView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                //Common.CatchException("Initializing Component", e);
            }
        }

        #endregion

        #region Methods

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            {
                _commandData = commandData;
                _revitApp = commandData.Application.Application;
                _revitDoc = commandData.Application.ActiveUIDocument.Document;
                _revitUiApp = commandData.Application;
                _revitUiDoc = commandData.Application.ActiveUIDocument;
            }

            DataContext = new TreeView4TierViewModel(_commandData);


            //ShowDialog();

            Show();

            return Result.Succeeded;
        }

        #endregion
    }
}