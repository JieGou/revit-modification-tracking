//Alls icons are using free from the website https://icons8.com/icons


#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using TrackDirect.Properties;
using TrackDirect.Utilities;
using TrackDirect.UI;
using System.Reflection;
using Autodesk.Revit.UI.Events;
using System.IO;
using System.Linq;
using static TrackDirect.UI.AutoTrackDataStorageUtil;
using System.Collections.ObjectModel;
using System.Threading;


#endregion

namespace TrackDirect
{
    class AppCommand : IExternalApplication
    {


        public static FooRequestHandler FooHandler { get; set; }
        public static TrackDirectHandler TrackHandler { get; set; }
        public static ExternalEvent ExEvent { get; set; }
        private SettingTrackView _trackView = null;

     

        private readonly string _tabName = "DRTO-VCF";



        private static UIControlledApplication _uiApp;
        private static bool _started = false;
        private static bool _isRunningSaving = false;
        private static bool _isRunningSynchronizing = false;



        //Get button to change their properties in class command
        internal static PushButton btnTrack { get; set; }
        internal static PushButton btnTrackManager { get; set; }
        internal static PushButton btnTrackSettings { get; set; }
        internal static PushButton btnSnapshot { get; set; }
        internal static PushButton btnCompare { get; set; }
        internal static PushButton btnSettingsDB { get; set; }
        public SplitButton splTrack;


        //Set auto Track element
        private static AutoTrackSettings toolSettings = null;
        private static IList<ElementId> catList = new List<ElementId>();
        private static bool isAutoTrackEventSave { get; set; } = false;
        private static bool isAutoTrackEventDocumentOpen { get; set; } = false;
        private static bool isAutoTrackEventSynchronize { get; set; } = false;
        private static bool isAutoTrackEventViewActive { get; set; } = false;
        private static bool canAutoRun { get; set; } = false;

        internal static string assemblyPath = typeof(AppCommand).Assembly.Location;


        /// <summary>
        /// Orivude access to this class instance
        /// </summary>
        internal static AppCommand GetInstance { get; private set; } = null;

        /// <summary>
        /// Return the full add-in assembly folder path
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string Path
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }


        #region Result Startup
        public Result OnStartup(UIControlledApplication a)
        {
            try
            {
                GetInstance = this;
                _uiApp = a;
                //Buil all ribbon item
                BuildUI(a);

                //External Event
                TrackHandler = new TrackDirectHandler();
                ExEvent = ExternalEvent.Create(TrackHandler);



                //Event
                a.ControlledApplication.DocumentOpened += OnDocumentOpened;
                a.ControlledApplication.DocumentCreated += OnDocumentCreated;
                a.ControlledApplication.DocumentSaving += OnDocumentSaving;
                a.ControlledApplication.DocumentSaved += OnDocumentSaved;
                a.ControlledApplication.DocumentSavingAs += OnDocumentSavingAs;
                a.ControlledApplication.DocumentSynchronizingWithCentral += OnDocumentSynchronizing;
                a.ControlledApplication.DocumentSynchronizedWithCentral += OnDocumentSynchronized;
                a.ViewActivated += new EventHandler<ViewActivatedEventArgs>(OnViewActivated);


                return Result.Succeeded;
            }
            catch (Exception eX)
            {
                TaskDialog td = new TaskDialog("Error in Setup");
                td.ExpandedContent = eX.GetType().Name + ": " + eX.Message + Environment.NewLine + eX.StackTrace;
                td.Show();
                return Result.Failed;
            }
        }
        #endregion

        #region Result Shutdown
        public Result OnShutdown(UIControlledApplication a)
        {
            a.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            a.ControlledApplication.DocumentSaving -= OnDocumentSaving;
            a.ControlledApplication.DocumentSaved -= OnDocumentSaved;
            a.ControlledApplication.DocumentSavingAs -= OnDocumentSavingAs;
            a.ControlledApplication.DocumentSynchronizingWithCentral -= OnDocumentSynchronizing;
            a.ControlledApplication.DocumentSynchronizedWithCentral -= OnDocumentSynchronized;

            //Close auto modeless form if it is still opening
            if (_trackView != null && _trackView.IsVisible)
            {
                _trackView.Close();
            }

            return Result.Succeeded;
        }
        #endregion

        #region Create all ribbon item: tab, panel, button....
        //Create the buton on the panel
        private void BuildUI(UIControlledApplication uiApp)
        {
            #region Panel TrackChange direct solution
            var plTrack = CreatePanel(uiApp, _tabName, "TRACK-DIRECT");
            // Create buttons of trackchanges panel
            var dataTrack = new PushButtonData("btnSnapshot", "Run\nTrack", assemblyPath, typeof(CmdTrackChange).FullName);
            dataTrack.LargeImage = ImageUtils.ConvertFromBitmap(Resources.toggle_off_32);
            dataTrack.Image = ImageUtils.ConvertFromBitmap(Resources.toggle_off_16);
            dataTrack.ToolTip = "Track the change in the model Revit.";
            //btnTrack = plTrack.AddItem(dataTrack) as PushButton;

            var setting1 = new PushButtonData("btnSetting1", "Settings", assemblyPath, typeof(CmdTrackSettings).FullName);
            setting1.ToolTip = "Settings list category and autorun.";
            setting1.Image = ImageUtils.ConvertFromBitmap(Resources.settings_16);
            setting1.LargeImage = ImageUtils.ConvertFromBitmap(Resources.settings_32);
            //IList<RibbonItem> stackedButtons = plTrack.AddStackedItems(setting1, null, null);
            //btnTrackSettings = plTrack.AddItem(setting1) as PushButton;

            SplitButtonData splDataTrack = new SplitButtonData("splTrack", "Direct Track");
            splTrack = plTrack.AddItem(splDataTrack) as SplitButton;
            btnTrack = splTrack.AddPushButton(dataTrack) as PushButton;
            btnTrackSettings = splTrack.AddPushButton(setting1) as PushButton;
            

            var dataTrackManager = new PushButtonData("btnTrackManager", "UI Track\nManager", assemblyPath, typeof(CmdTrackManager).FullName);
            dataTrackManager.LargeImage = ImageUtils.ConvertFromBitmap(Resources.check_book_32);
            dataTrackManager.Image = ImageUtils.ConvertFromBitmap(Resources.check_book_16);
            dataTrackManager.ToolTip = "UI to watch the modification.";
            dataTrackManager.LongDescription = "";
            btnTrackManager = plTrack.AddItem(dataTrackManager) as PushButton;
            //btnTrackManager.AvailabilityClassName = "TrackChange.Utilities.Availability";

            #endregion //Panel Trackchange direct solution


            #region Panel Database solution
            //Create panel workflow using database solution
            var plDatabase = CreatePanel(uiApp, _tabName, "TRACK-DATABASE");

            #region buttons
            // Create buttons of trackchanges panel
            var dataSnapshot = new PushButtonData("btnSnapshot", "Store Data\nTo PC", assemblyPath, typeof(CmdSnapshot).FullName);
            dataSnapshot.LargeImage = ImageUtils.ConvertFromBitmap(Resources.database_export_32);
            dataSnapshot.Image = ImageUtils.ConvertFromBitmap(Resources.database_export_16);
            dataSnapshot.ToolTip = "Take a snapshot of a model";
            dataSnapshot.LongDescription = "Take a snapshot of a model that can be used for later comparison of this version of the model.";
            btnSnapshot =  plDatabase.AddItem(dataSnapshot) as PushButton;
            btnSnapshot.Enabled = false;


            var dataCompare = new PushButtonData("btnTrackChange", "Compare\nData-Model", assemblyPath, typeof(CmdCompare).FullName);
            dataCompare.LargeImage = ImageUtils.ConvertFromBitmap(Resources.compare_32);
            dataCompare.Image = ImageUtils.ConvertFromBitmap(Resources.compare_16);
            dataCompare.ToolTip = "Compare a model against a previous model";
            dataCompare.LongDescription = "Compare a model against a previous snapshot of the model.";
            btnCompare = plDatabase.AddItem(dataCompare) as PushButton;
            btnCompare.Enabled = false;
            // anything below here on the slideout?
            plDatabase.AddSlideOut();
            var setDB = new PushButtonData("btnSettings", "Settings", assemblyPath, typeof(CmdSnapshot).FullName);
            setDB.ToolTip = "Settings for application TrackDirect.";
            setDB.LongDescription = "Clear any Analysis Visualization Framework graphic primitives (faces, boxes, vectors) from the active view.";
            setDB.Image = ImageUtils.ConvertFromBitmap(Resources.settings_16);
            setDB.LargeImage = ImageUtils.ConvertFromBitmap(Resources.settings_32);
            btnSettingsDB =  plDatabase.AddItem(setDB) as PushButton;
            btnSettingsDB.Enabled = false;

            var dataCTestTreeView = new PushButtonData("btnTest", "Test App", assemblyPath, typeof(CmdTestTreeView).FullName);
            dataCTestTreeView.LargeImage = ImageUtils.ConvertFromBitmap(Resources.test_32);
            dataCTestTreeView.Image = ImageUtils.ConvertFromBitmap(Resources.test_16);
            dataCTestTreeView.ToolTip = "Test App demo";
            dataCTestTreeView.LongDescription = "Test TreeView list with textbox in Revit.";
            var btnTest = plDatabase.AddItem(dataCTestTreeView) as PushButton;
            btnTest.Enabled = false;

            #endregion
            #endregion //Panel Database solution


            //instruction file to open by F1 key
            string instructionFile = @"https://github.com/TienDuyNGUYEN";
            if (File.Exists(instructionFile))
            {
                ContextualHelp contextualHelp = new ContextualHelp(ContextualHelpType.Url, instructionFile);
                btnTrack.SetContextualHelp(contextualHelp);
                btnTrackSettings.SetContextualHelp(contextualHelp);
                btnSnapshot.SetContextualHelp(contextualHelp);
                btnCompare.SetContextualHelp(contextualHelp);
                btnSettingsDB.SetContextualHelp(contextualHelp);
            }

        }

        #endregion //Create all ribbon item: tab, panel, button

        #region Events
        private static void CollectAutoTrackSetting(Document doc)
        {
            try
            {
                toolSettings = AutoTrackDataStorageUtil.GetAutoTrackCreatorSettings(doc);
               
                canAutoRun = toolSettings.CanAutoRun;
                if (canAutoRun)
                {
                    isAutoTrackEventSave = toolSettings.IsAutoTrackEventSave;
                    isAutoTrackEventSynchronize = toolSettings.IsAutoTrackEventSynchronize;
                    isAutoTrackEventViewActive = toolSettings.IsAutoTrackEventViewActive;
                    isAutoTrackEventDocumentOpen = toolSettings.IsAutoTrackEventDocumentOpen;
                }
                else
                {
                    isAutoTrackEventSave = false;
                    isAutoTrackEventSynchronize = false;
                    isAutoTrackEventViewActive = false;
                    isAutoTrackEventDocumentOpen = false;
                }
            }
            catch  {}
        }
        private void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {

        }
        private static void OnDocumentClosing(object source, DocumentClosingEventArgs args)
        {
            if (TrackDirectHandler._startState != null)
                runTrack(source);
        }
        private static void OnDocumentOpened(object source, DocumentOpenedEventArgs args)
        {
            if (args.Status != RevitAPIEventStatus.Succeeded)
                return;
            Document doc = args.Document;
            CollectAutoTrackSetting(doc);
            if (isAutoTrackEventDocumentOpen && canAutoRun  && TrackDirectHandler._startState == null)
                runTrack( source);
        }
        private static void OnDocumentCreated(object sender, DocumentCreatedEventArgs args)
        {
            
            if (args.Status != RevitAPIEventStatus.Succeeded)
                return;
            Document doc = args.Document;
            CollectAutoTrackSetting(doc);


        }
        private static void OnDocumentSaving(object sender, DocumentSavingEventArgs args)
        {
            Document doc = args.Document;
            CollectAutoTrackSetting(doc);
            if (isAutoTrackEventSave && canAutoRun && TrackDirectHandler._startState != null)
            {
                runTrack(sender);
                _isRunningSaving = true;
            }
            else
            {
                _isRunningSaving = false;
            }

        }
        private static void OnDocumentSaved(object sender, DocumentSavedEventArgs args)
        {
            if (isAutoTrackEventSave && canAutoRun && _isRunningSaving) runTrack(sender);

        }
        private static void OnDocumentSavingAs(object sender, DocumentSavingAsEventArgs args)
        {
            Document doc = args.Document;
            CollectAutoTrackSetting(doc);
            if (isAutoTrackEventSave && canAutoRun && TrackDirectHandler._startState != null)
                runTrack(sender);
            runTrack(sender);
        }
        private static void OnDocumentSynchronizing(object sender, DocumentSynchronizingWithCentralEventArgs args)
        {
            Document doc = args.Document;
            CollectAutoTrackSetting(doc);
            if (isAutoTrackEventSynchronize && canAutoRun && TrackDirectHandler._startState != null)
            {
                runTrack(sender);
                _isRunningSynchronizing = true;
            }
            else
            {
                _isRunningSynchronizing = false;
            }

        }
        private static void OnDocumentSynchronized(object sender, DocumentSynchronizedWithCentralEventArgs args)
        {
            if (isAutoTrackEventSynchronize && canAutoRun && _isRunningSynchronizing)
                runTrack(sender);
        }

        #region Event switch project by change active view

        private void OnViewActivated(
          object sender,
          ViewActivatedEventArgs e)
        {
            Autodesk.Revit.DB.View vPrevious = e.PreviousActiveView;
            
            View vCurrent = e.CurrentActiveView;
            Document docPrevious = vPrevious.Document;
            string idDocPrevious = string.Empty;
            string idDocCurrent = string.Empty;

            if (vPrevious != null)
            {
                idDocPrevious = docPrevious.ProjectInformation.UniqueId;
            }
            if (vCurrent != null)
            {
                idDocCurrent = vCurrent.Document.ProjectInformation.UniqueId;
            }
            if(!string.IsNullOrEmpty(idDocPrevious) && !string.IsNullOrEmpty(idDocCurrent) && idDocPrevious != idDocCurrent)
            {
                CollectAutoTrackSetting(docPrevious);
                if (isAutoTrackEventViewActive && canAutoRun) runTrack(sender);
                //Restart method
                runTrack(sender);
            }
        }

        #endregion

        //Run plugin track 
        private static void runTrack(object sender)
        {
            CmdTrackChange cmdTrackChange = new CmdTrackChange();
            if (sender is UIApplication)
                cmdTrackChange.Execute(sender as UIApplication);
            else
                cmdTrackChange.Execute(new UIApplication(sender as Autodesk.Revit.ApplicationServices.Application));
        }

        #endregion //Events

        #region Helper pour create panel    
        public RibbonPanel CreatePanel(UIControlledApplication a, string tabName, string panelName)
        {
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab(tabName);
            }
            catch { }
            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);
            }
            catch { }
            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tabName);
            foreach (RibbonPanel p in panels)
            {
                if (p.Name == panelName)
                {
                    ribbonPanel = p;
                }
            }
            return ribbonPanel;
        }
        #endregion //Helper pour create panel

        #region Utilies
        public static string GetUserInfo()
        {
            // make a reasonably unique identifier - but pretty anonymous. This is for analytics tracking.
            return (Environment.UserDomainName + "\\" + Environment.UserName).GetHashCode().ToString();
        }

        public static void FirstTimeRun()
        {
            if (_started) return;
            //otherwise, record the fact that we started, for analytics purposes.
            startup();
        }
        private static void startup()
        {
            //_uiApp.ControlledApplication.WriteJournalComment("Starting up Revit TrackDirect...", false);
            _started = true;
        }

        /// <summary>
        /// Reset the top button to be the current one.
        /// Alternative solution: 
        /// set RibbonItem.IsSynchronizedWithCurrentItem 
        /// to false after creating the SplitButton.
        /// </summary>
        public void SetTopButtonCurrent()
        {
            IList<PushButton> sbList = splTrack.GetItems();
            splTrack.CurrentButton = sbList[0];
        }
        #endregion //end utilities


        #region External event

        /// <summary>
        /// Open Modeless Dialog
        /// </summary>
        /// <param name="uiapp"></param>
        public void ShowWindow(UIApplication uiapp)
        {
            //if (_trackView == null)
            //{
            //    BCFHandler handler = new BCFHandler(uiapp);
            //    ExternalEvent exEvent = ExternalEvent.Create(handler);

            //    _trackView = new _trackView(exEvent, handler);
            //    _trackView.Closed += WindowClosed;
            //    _trackView.Show();
            //}
        }

        #endregion //External event


    }
}
