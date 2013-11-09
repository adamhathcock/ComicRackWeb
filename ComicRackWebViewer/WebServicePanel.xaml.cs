using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nancy.Hosting.Self;

namespace ComicRackWebViewer
{
    /// <summary>
    /// Interaction logic for WebServicePanel.xaml
    /// </summary>
    public partial class WebServicePanel : Window
    {
        private static readonly ManualResetEvent mre = new ManualResetEvent(false);
        private static NancyHost HOST;
        private int? actualPort;
        private string address;


        public WebServicePanel()
        {
            InitializeComponent();
            SetEnabledState();
            addressTextBox.Text = Settings.GetSetting("ip") ?? "localhost";
            portTextBox.Text = Settings.GetSetting("port") ?? "8080";
            bindAll.IsChecked = bool.Parse(Settings.GetSetting("bindAll") ?? "false");
        }

        private void PortTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(portTextBox.Text))
            {
                return;
            }
            int x;
            if (int.TryParse(portTextBox.Text, out x))
            {
                actualPort = x;
            }
            else
            {
                actualPort = null;
            }
            SetEnabledState();
        }

        private void SetEnabledState()
        {
            if (startServiceButton == null)
            {
                return;
            }
            startServiceButton.IsEnabled = actualPort.HasValue && HOST == null;
            stopServiceButton.IsEnabled = HOST != null;
            portTextBox.IsEnabled = HOST == null;
            if (!bindAll.IsChecked ?? false)
            {
                addressTextBox.IsEnabled = HOST == null;
            }
            if (HOST == null)
            {
                Status.Text = "Stopped";
            }
            else
            {
                Status.Text = "Running on: ";
                foreach (var uri in GetUris(bindAll.IsChecked ?? false))
                {
                    Status.Text += uri.ToString() + " ";
                }
            }
            Mouse.SetCursor(null);
        }

        public void StartService()
        {
            address = addressTextBox.Text;
            startServiceButton.IsEnabled = false;
            portTextBox.IsEnabled = false;
            addressTextBox.IsEnabled = false;
            Mouse.SetCursor(Cursors.Wait);
            bool bind = bindAll.IsChecked ?? false;
            Task.Factory.StartNew(() => LoadService(bind));
            Status.Text = "Starting";
        }

        public void LoadService(bool bindAll)
        {
            if (HOST != null)
            {
                StopService();
            }

            HOST = new NancyHost(new Bootstrapper(), GetUris(bindAll).ToArray());
            try
            {
                HOST.Start();
                this.Dispatcher.Invoke(new Action(SetEnabledState));
                mre.Reset();
                mre.WaitOne();

                HOST.Stop();
            }
            catch (Exception)
            {
                MessageBox.Show("Error in url binding");
                StopService();
                throw;
            }
            finally
            {
                HOST = null;
                this.Dispatcher.Invoke(new Action(SetEnabledState));
            }
        }

        private IEnumerable<Uri> GetUris(bool bindAll)
        {
            if (bindAll)
            {
                foreach (var ip in GetLocalIPs())
                {
                    string url = string.Format("http://{1}:{0}/", actualPort.Value, ip);
                    yield return new Uri(url);
                }
            }
            else
            {
                string url = string.Format("http://{1}:{0}/", actualPort.Value, address);
                yield return new Uri(url);
            }
        }

        private static IEnumerable<string> GetLocalIPs()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());
        }

        public void StopService()
        {
            mre.Set();
        }

        private void StartServiceButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsCurrentlyRunningAsAdmin())
            {
                Settings.SaveSetting("ip", addressTextBox.Text);
                Settings.SaveSetting("port", portTextBox.Text);
                Settings.SaveSetting("bindAll", (bindAll.IsChecked ?? false).ToString());
                StartService();
            }
            else
            {
                MessageBox.Show("Sorry!, you must be running ComicRack with administrator privileges.");
            }
        }

        private void StopServiceButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            StopService();
        }

        private static bool IsCurrentlyRunningAsAdmin()
        {
            bool isAdmin = false;
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            if (currentIdentity != null)
            {
                WindowsPrincipal pricipal = new WindowsPrincipal(currentIdentity);
                isAdmin = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                pricipal = null;
            }
            return isAdmin;
        }

        private void CheckBoxChecked1(object sender, RoutedEventArgs e)
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            addressTextBox.IsEnabled = false;
        }

        private void BindAllUnchecked1(object sender, RoutedEventArgs e)
        {
            addressTextBox.IsEnabled = true;
        }
    }
}
