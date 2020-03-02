#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Drawing;
using TrackDirect.Models;
using System.Windows.Media;
using TrackDirect.Utilities;

#endregion

namespace TrackDirect.UI
{
    public class TrackManagerHandler : IExternalEventHandler
    {
        private static Document _activeDoc;
        private static UIApplication _uiapp;
        private static Autodesk.Revit.ApplicationServices.Application _app;


        public string GetName()
        {
            return "Task External Event1";
        }

        public void Execute(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _app = uiapp.Application;
            _activeDoc = uiapp.ActiveUIDocument.Document;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;
                        }
                    case RequestId.HighLighElement:
                        {
                            GetSnapShot();
                            break;
                        }
                    case RequestId.ColorElement:
                        {
                            GetSnapShot();
                            break;
                        }
                    case RequestId.IsolateElement:
                        {
                            GetSnapShot();
                            break;
                        }
                    case RequestId.RemoveColor:
                        {
                            GetSnapShot();
                            break;
                        }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public CommunicatorRequest Request { get; set; } = new CommunicatorRequest();
        public class CommunicatorRequest
        {
            private int _request = (int)RequestId.None;

            public RequestId Take()
            {
                return (RequestId)Interlocked.Exchange(ref _request, (int)RequestId.None);
            }

            public void Make(RequestId request)
            {
                Interlocked.Exchange(ref _request, (int)request);
            }
        }
        public enum RequestId
        {
            None,
           HighLighElement,
           ColorElement,
           IsolateElement,
            RemoveColor
            
        }

        #region External event

        private void GetSnapShot()
        {
           
        }
        
       
        #endregion //External events

    }
}