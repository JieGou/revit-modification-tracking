using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TrackDirect.Utilities;
using TrackDirect.Models;


namespace TrackDirect
{
    public class ComparisonController
    {
        //List Parameter of Track Change
        public static List<ChangedElement> NewElementList = new List<ChangedElement>();
        public static List<ChangedElement> DeletedElementList = new List<ChangedElement>();
        public static List<ChangedElement> ChangedElementList = new List<ChangedElement>();

        private static string _pCreatedDateName = VCFParameters.VCF_CreateAt.ToString();
        private static string _pModifiedDateName = VCFParameters.VCF_ModifyAt.ToString();
        private static string _pDesctiptionName = VCFParameters.VCF_ChangeDescription.ToString();
        private static string _pChangeTypeName = VCFParameters.VCF_ChangeType.ToString();
        private static string _pUserName = VCFParameters.VCF_User.ToString();
        public static string _dateRecord = DateTime.Now.ToShortDateString();
        private static string _userRevit = CmdAutoTrack.UserRevit;

        public static int nAdded = 0;
        public static int nDeleted = 0;
        public static int nModified = 0;
        public static int nIdentical = 0;


        #region Report differences Solution Advanced
        /// <summary>
        /// Compare the start and end states and report the 
        /// differences found. In this implementation, we
        /// just store a hash code of the element state.
        /// If you choose to store the full string 
        /// representation, you can use that for comparison,
        /// and then report exactly what changed and the
        /// original values as well.
        /// </summary>
        public static void ReportDifferences(
          Document doc,
          Dictionary<string, RevitElement> _previousElems,
          Dictionary<string, RevitElement> _currentElems)
        {
            int n1 = _previousElems.Keys.Count;
            int n2 = _currentElems.Keys.Count;
            nAdded = 0;
            nDeleted = 0;
            nModified = 0;
            nIdentical = 0;

            ComparisonController.NewElementList = new List<ChangedElement>();
            ComparisonController.DeletedElementList = new List<ChangedElement>();
            ComparisonController.ChangedElementList = new List<ChangedElement>();


            List<string> keys = new List<string>(_previousElems.Keys);

            //keys are the new elements in end state
            foreach (var currentPair in _currentElems)
            {
                var current = currentPair.Value;
                if (_previousElems.ContainsKey(currentPair.Key))
                {
                    //Changed Element

                    var previous = _previousElems[currentPair.Key];

                    var change = ComparisonMaker.compareElements(current, previous);
                    if (change != null)
                    {
                        ++nModified;
                        ComparisonController.ChangedElementList.Add(change);
                    }
                    else
                    {
                        ++nIdentical;
                    }

                }
                else
                {
                    // New Element
                    ++nAdded;
                    ComparisonController.NewElementList.Add(ComparisonMaker.buildNew(current));
                }
            }
            // Deleted Element
            foreach (var previousPair in _previousElems)
            {
                if (!_currentElems.ContainsKey(previousPair.Key))
                {
                    ++nDeleted;
                    ComparisonController.DeletedElementList.Add(ComparisonMaker.buildDeleted(previousPair.Value));
                }
            }

            foreach (var e in ComparisonController.NewElementList)
            {
                SetParameterNewElement(doc, e);
            }
            foreach (var e in ComparisonController.ChangedElementList)
            {
                SetParameterModifiedElement(doc, e);
            }
            string msg = $"Stopped tracking changes now.\r\n"
              + $"{nDeleted} deleted, {nAdded} added, {nModified} modified, "
              + $"{nIdentical} identical elements:";


            TaskDialog dlg = new TaskDialog("Track Changes") { MainInstruction = msg };
            dlg.Show();
        }
        #endregion // Report differences


        //Set value parameter for new elements
        private static void SetParameterNewElement(Document doc, ChangedElement ele)
        {
            Element e = doc.GetElement(ele.UniqueId);

            if (e is null) return; //Ignore if we cant not get element by UniqueId
            IList<Parameter> paramCreatedDate = e.GetParameters(_pCreatedDateName);
            IList<Parameter> paramChangeType = e.GetParameters(_pChangeTypeName);
            IList<Parameter> paramUser = e.GetParameters(_pUserName);

            //Set parameter
            using (Transaction trans = new Transaction(doc, "Set Parameter"))
            {
                trans.Start();
                try
                {
                    if (paramCreatedDate.Count > 0)
                        paramCreatedDate.FirstOrDefault().Set(_dateRecord);
                    if (paramChangeType.Count > 0)
                        paramChangeType.FirstOrDefault().Set(ChangedElement.ChangeTypeEnum.NewElement.ToString());
                    if (paramUser.Count > 0)
                        paramUser.FirstOrDefault().Set(_userRevit);
                    trans.Commit();
                }
                catch
                {
                    trans.RollBack();
                }

            }
        }

        //Set value parameter for modified Elements
        private static void SetParameterModifiedElement(Document doc, ChangedElement ele)
        {
            Element e = doc.GetElement(ele.UniqueId);

            if (e is null) return; //Ignore if we cant not get element by UniqueId
            IList<Parameter> paramModifiedDate = e.GetParameters(_pModifiedDateName);
            IList<Parameter> paramChangeDescription = e.GetParameters(_pDesctiptionName);
            IList<Parameter> paramChangeType = e.GetParameters(_pChangeTypeName);
            IList<Parameter> paramUser = e.GetParameters(_pUserName);

            //Set parameter
            using (Transaction trans = new Transaction(doc, "Set Parameter"))
            {
                trans.Start();
                try
                {
                    if (paramModifiedDate.Count > 0)
                        paramModifiedDate.FirstOrDefault().Set(_dateRecord);
                    if (paramChangeType.Count > 0)
                        paramChangeType.FirstOrDefault().Set(ele.ChangeType.ToString());
                    if (paramChangeDescription.Count > 0)
                        paramChangeDescription.FirstOrDefault().Set(ele.ChangeDescription);
                    if (paramUser.Count > 0)
                        paramUser.FirstOrDefault().Set(_userRevit);
                    trans.Commit();
                }
                catch
                {
                    trans.RollBack();
                }

            }
        }
    }
}
