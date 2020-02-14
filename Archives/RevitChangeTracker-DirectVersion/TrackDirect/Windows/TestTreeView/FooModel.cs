using System;


namespace TrackDirect.UI
{
    public class FooModel
    {
        public FooModel() { }
        public void GetInstanceItems()
        {
            AppCommand.Handler.Request.Make(FooRequestHandler.RequestId.GetElementItems);
            AppCommand.ExEvent.Raise();
        }
        
    }
}
