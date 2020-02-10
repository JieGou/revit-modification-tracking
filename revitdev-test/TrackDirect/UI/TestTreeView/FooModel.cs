using System;


namespace TrackDirect.UI
{
    public class FooModel
    {
        public FooModel() { }
        public void GetInstanceItems()
        {
            AppCommand.FooHandler.Request.Make(FooRequestHandler.RequestId.GetElementItems);
            AppCommand.ExEvent.Raise();
        }
        
    }
}
