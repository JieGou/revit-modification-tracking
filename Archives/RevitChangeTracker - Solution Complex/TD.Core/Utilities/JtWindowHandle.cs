using System;
using System.Windows.Forms;

namespace TD.Core
{
    public class JtWindowHandle : IWin32Window
    {
        IntPtr _hwnd;

        public JtWindowHandle(IntPtr h)
        {
            
            _hwnd = h;
        }

        public IntPtr Handle
        {
            get
            {
                return _hwnd;
            }
        }
    }
}
