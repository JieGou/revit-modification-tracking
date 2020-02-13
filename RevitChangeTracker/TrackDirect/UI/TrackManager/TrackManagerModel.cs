using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackDirect.UI
{
    public class TrackManagerModel
    {
        public TrackManagerModel() { }
        public void HighLighElement()
        {
            AppCommand.ManageHandler.Request.Make(TrackManagerHandler.RequestId.HighLighElement);
            AppCommand.ManageExEvent.Raise();
        }

        public void ColorElement()
        {
            AppCommand.ManageHandler.Request.Make(TrackManagerHandler.RequestId.ColorElement);
            AppCommand.ManageExEvent.Raise();
        }
        public void RemoveColor()
        {
            AppCommand.ManageHandler.Request.Make(TrackManagerHandler.RequestId.RemoveColor);
            AppCommand.ManageExEvent.Raise();
        }
        public void IsolateElement()
        {
            AppCommand.ManageHandler.Request.Make(TrackManagerHandler.RequestId.IsolateElement);
            AppCommand.ManageExEvent.Raise();
        }
        
    }
}
