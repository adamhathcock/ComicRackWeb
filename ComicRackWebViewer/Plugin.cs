using System;
using System.Windows;
using cYo.Projects.ComicRack.Plugins.Automation;

namespace ComicRackWebViewer
{
    public static class Plugin
    {
        internal static IApplication Application;
        private static WebServicePanel panel;
        private static Version requiredVersion = new Version(0, 9, 153);

        public static void Run(IApplication app)
        {
            try
            {
                Application = app;
                var comicVersion = new Version(app.ProductVersion);
                if (comicVersion < requiredVersion)
                {
                    MessageBox.Show("ComicRack version required: " + requiredVersion);
                    return;
                }
                if (panel == null)
                {
                    panel = new WebServicePanel();
                    panel.Closed += new EventHandler(panel_Closed);
                }
                panel.ShowDialog();
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
                if (comicVersion < requiredVersion)
                {
                    MessageBox.Show("ComicRack version required: " + requiredVersion);
                    return;
                }
                if (panel == null)
                {
                    panel = new WebServicePanel();
                }
                panel.StartService();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        static void panel_Closed(object sender, EventArgs e)
        {
            panel = null;
        }

        [STAThread]
        public static void Main()
        {
            if (panel == null)
            {
                panel = new WebServicePanel();
                panel.Closed += new EventHandler(panel_Closed);
            }
            panel.ShowDialog();
        }

    }
}
