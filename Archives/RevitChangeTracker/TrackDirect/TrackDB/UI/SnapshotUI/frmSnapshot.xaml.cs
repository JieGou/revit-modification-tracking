using Autodesk.Revit.DB;
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
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
//using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Forms;


namespace TrackDirect.UI
{

    public partial class frmSnapshot : Window, INotifyPropertyChanged
    {
        #region declarations
        public ObservableCollection<Autodesk.Revit.DB.Document> Docs { get; set; }
        public Autodesk.Revit.DB.Document DocActive { get; set; }
        private string _fileName;
        private Autodesk.Revit.DB.Document _selectedDocument;

        public String Filename
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }
        public Autodesk.Revit.DB.Document SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                _selectedDocument = value;
                OnPropertyChanged("SelectedDocument");
            }
        }
        #endregion //End regions declarations


        #region Initiate form
        public frmSnapshot()
        {
            InitializeComponent();
        }

        public frmSnapshot(IList<Autodesk.Revit.DB.Document> docs, Autodesk.Revit.DB.Document current)
        {
            InitializeComponent();
            Docs = new ObservableCollection<Autodesk.Revit.DB.Document>();
            DocActive = current;
            foreach (Document doc in docs)
            {
                Docs.Add(doc);
            }
            cboModel.ItemsSource = Docs;
            cboModel.SelectedItem = current;
            tbxFileName.Text = _fileName;
        }
        private void FormSnapshot_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        //Getsuggest file Name from name of model
        private string suggestModelName()
        {
            Autodesk.Revit.DB.Document doc = cboModel.SelectedItem as Autodesk.Revit.DB.Document;

            if (doc != null)
            {
                string filename = Path.GetFileNameWithoutExtension(doc.PathName);
                if (doc.IsWorkshared && (!doc.IsDetached))
                {
                    try
                    {

                        var mp = doc.GetWorksharingCentralModelPath();
                        string centralPath = Autodesk.Revit.DB.ModelPathUtils.ConvertModelPathToUserVisiblePath(mp);
                        if ((mp.ServerPath == false) && System.IO.Path.IsPathRooted(centralPath))
                        {
                            string folder = Path.GetDirectoryName(centralPath);
                            string baseName = Path.GetFileNameWithoutExtension(centralPath);
                            filename = Path.Combine(folder, "Snapshots", baseName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".db");
                        }

                    }
                    catch { }
                }
                else
                {
                    if (String.IsNullOrEmpty(doc.PathName)) return String.Empty;

                    try
                    {
                        filename = Path.Combine(Path.GetDirectoryName(doc.PathName), filename + "_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".db");
                    }
                    catch (Exception ex)
                    {
                        //doc.Application.WriteJournalComment("Note: struggling to get suggested filename: " + ex.GetType().Name + ": " + ex.Message, false);
                    }
                }

                return filename;
            }

            return string.Empty;
        }

        #region events
        //change text of texbox file name when selectedItem of combobox change
        private void CboModel_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _fileName = suggestModelName();
            tbxFileName.Text = _fileName;
        }
        
        //Get path of folder export
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".db";
            dlg.Filter = "Database File (*.db)|*.db;*.sdb;*.sqlite|All Files|*";
            dlg.FileName = Path.GetFileName(_fileName);

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                tbxFileName.Text = filename;
            }
        }

        //Get valid file name export
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (!Utilities.FileUtils.IsValidPath(tbxFileName.Text))
            {
                System.Windows.Forms.MessageBox.Show("Please enter a valid path for the file.");
                return;
            }

            if (cboModel.SelectedItem is null)
            {
                System.Windows.Forms.MessageBox.Show("Please select a valid model to snapshot.");
                return;
            }

            SelectedDocument = cboModel.SelectedItem as Autodesk.Revit.DB.Document;
            Filename = tbxFileName.Text;

            //this.DialogResult = DialogResult.OK;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion //end regions event


        #region Utilities
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion //eng region utilities

       
    }
}
