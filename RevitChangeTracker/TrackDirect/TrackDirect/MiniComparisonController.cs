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
    public class MiniComparisonController
    {
       
        private static string _pCreatedDateName = VCFParameters.VCF_CreateAt.ToString();
        private static string _pModifiedDateName = VCFParameters.VCF_ModifyAt.ToString();
        private static string _pChangeTypeName = VCFParameters.VCF_ChangeType.ToString();
        private static string _pUserName = VCFParameters.VCF_User.ToString();
        public static string _dateRecord = DateTime.Now.ToShortDateString();
        private static string _userRevit = CmdAutoTrack.UserRevit;

        public static int nAdded = 0;
        public static int nDeleted = 0;
        public static int nModified = 0;
        public static int nIdentical = 0;


        #region Report differences Solution Complete
        /// <summary>
        /// Compare the start and end states and report the 
        /// differences found. In this implementation, we
        /// just store a hash code of the element state.
        /// If you choose to store the full string 
        /// representation, you can use that for comparison,
        /// and then report exactly what changed and the
        /// original values as well.
        /// </summary>
        public static void MiniReportDifferences(
          Document doc,
          Dictionary<string, MiniDataComparision> _previousElems,
          Dictionary<string, MiniDataComparision> _currentElems)
        {
            nAdded = 0;
            nDeleted = 0;
            nModified = 0;
            nIdentical = 0;
            List<string> newElements = new List<string>();
            Dictionary<string,string> changedElements = new Dictionary<string, string>();

            foreach (var currentPair in _currentElems)
            {
                var current = currentPair.Value;
                if (_previousElems.ContainsKey(currentPair.Key))
                {
                    //Changed Element
                    var previous = _previousElems[currentPair.Key];
                    string changeType = MiniComparisonMaker.GetTypeChange(current, previous);

                    //Same element
                    if (string.IsNullOrEmpty(changeType))
                    {
                        ++nIdentical;
                    }
                    else //Changed element
                    {
                        ++nModified;
                        changedElements.Add(current.UniqueId, changeType);
                    }
                    
                }
                else
                {
                    // New Element
                    ++nAdded;
                    newElements.Add(current.UniqueId);

                }
            }
            // Deleted Element
            foreach (var previousPair in _previousElems)
            {
                if (!_currentElems.ContainsKey(previousPair.Key))
                {
                    ++nDeleted;
                }
            }

            using (Transaction trans = new Transaction(doc, "Set Parameter"))
            {
                trans.Start();
                try
                {
                    foreach(var guid in newElements)
                    {
                        MiniSetParameterNewElement(doc, guid);
                    }
                    foreach (var keypair in changedElements)
                    {
                        MiniSetParameterModifiedElement(doc, keypair.Key,keypair.Value);
                    }
                }
                catch
                {
                    trans.RollBack();
                }
                trans.Commit();
            }
            
            //string msg = $"Stopped tracking changes now.\r\n"
            //  + $"{nDeleted} deleted, {nAdded} added, {nModified} modified, "
            //  + $"{nIdentical} identical elements:";


            //TaskDialog dlg = new TaskDialog("Track Changes") { MainInstruction = msg };
            //dlg.Show();
        }
        #endregion // Report differences


        //Set value parameter for new elements
        private static void MiniSetParameterNewElement(Document doc, string uniqueId)
        {
            Element e = doc.GetElement(uniqueId);

            if (e is null) return; //Ignore if we cant not get element by UniqueId
            IList<Parameter> paramCreatedDate = e.GetParameters(_pCreatedDateName);
            IList<Parameter> paramChangeType = e.GetParameters(_pChangeTypeName);
            IList<Parameter> paramUser = e.GetParameters(_pUserName);

            //Set parameter
                try
                {
                    if (paramCreatedDate.Count > 0)
                        paramCreatedDate.FirstOrDefault().Set(_dateRecord);
                    if (paramChangeType.Count > 0)
                        paramChangeType.FirstOrDefault().Set(ChangedElement.ChangeTypeEnum.NewElement.ToString());
                    if (paramUser.Count > 0)
                        paramUser.FirstOrDefault().Set(_userRevit);
                }
                catch {}
            
        }

        //Set value parameter for modified Elements
        private static void MiniSetParameterModifiedElement(Document doc, string uniqueId, string ChangeType)
        {
            Element e = doc.GetElement(uniqueId);

            if (e is null) return; //Ignore if we cant not get element by UniqueId
            IList<Parameter> paramModifiedDate = e.GetParameters(_pModifiedDateName);
            IList<Parameter> paramChangeType = e.GetParameters(_pChangeTypeName);
            IList<Parameter> paramUser = e.GetParameters(_pUserName);

            //Set parameter
                try
                {
                    if (paramModifiedDate.Count > 0)
                        paramModifiedDate.FirstOrDefault().Set(_dateRecord);
                    if (paramChangeType.Count > 0)
                        paramChangeType.FirstOrDefault().Set(ChangeType);
                    if (paramUser.Count > 0)
                        paramUser.FirstOrDefault().Set(_userRevit);
                }
                catch{}

            
        }
    }
}
