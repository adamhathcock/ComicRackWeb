using System;
using System.Windows;
using cYo.Projects.ComicRack.Plugins.Automation;

namespace ComicRackWebViewer
{
    public static class Plugin
    {
        internal static IApplication Application;
        private static WebServicePanel PANEL;
        private static readonly Version RequiredVersion = new Version(0, 9, 153);

        public static void Run(IApplication app)
        {
            try
            {
                Application = app;
                var comicVersion = new Version(app.ProductVersion);
                if (comicVersion < RequiredVersion)
                {
                    MessageBox.Show("ComicRack version required: " + RequiredVersion);
                    return;
                }
                if (PANEL == null)
                {
                    PANEL = new WebServicePanel();
                    PANEL.Closed += new EventHandler(panel_Closed);
                }
                PANEL.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public static void RunAtStartup(IApplication app)
        {
            try
            {
                Application = app;
                var comicVersion = new Version(app.ProductVersion);
                if (comicVersion < RequiredVersion)
                {
                    MessageBox.Show("ComicRack version required: " + RequiredVersion);
                    return;
                }
                if (PANEL == null)
                {
                    PANEL = new WebServicePanel();
                }
                PANEL.StartService();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        static void panel_Closed(object sender, EventArgs e)
        {
            PANEL = null;
        }

        [STAThread]
        public static void Main()
        {
            if (PANEL == null)
            {
                PANEL = new WebServicePanel();
                PANEL.Closed += new EventHandler(panel_Closed);
            }
            PANEL.ShowDialog();
        }

    }
}
