using System;

namespace SuperOwner
{
    public static class EntryPoint
    {
        [STAThread]
        public static int Main()
        {
            using (var menu = new MainWindow())
            {
                menu.Run();
            }
            return 0;
        }
    }
}
