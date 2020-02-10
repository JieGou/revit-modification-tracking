using System;


namespace TrackChanges
{
    public class WindowTestModel
    {
        public WindowTestModel() { }
        public void SelectElement()
        {
            AppTestWpf.Handler.Request.Make(WindowTestRequestHandler.RequestId.SelectElement);
            AppTestWpf.ExEvent.Raise();
        }

        public void ColorElement()
        {
            AppTestWpf.Handler.Request.Make(WindowTestRequestHandler.RequestId.ColorElement);
            AppTestWpf.ExEvent.Raise();
        }
        public void UnColorElement()
        {
            AppTestWpf.Handler.Request.Make(WindowTestRequestHandler.RequestId.UnColorElement);
            AppTestWpf.ExEvent.Raise();
        }

        public void GetTreeView()
        {
            AppTestWpf.Handler.Request.Make(WindowTestRequestHandler.RequestId.GetTreeView);
            AppTestWpf.ExEvent.Raise();
        }

    }
}
