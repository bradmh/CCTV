using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Monitoring
{
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            dbName.Text = DbCreds.dbName;
            userName.Text = DbCreds.userid;
            port.Text = DbCreds.port;
            ip.Text = DbCreds.server;
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            DbCreds.dbName = dbName.Text;
            DbCreds.userid = userName.Text;
            DbCreds.password = pw.Password;
            DbCreds.port = port.Text;
            DbCreds.server = ip.Text;

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["server"] = ip.Text;
            localSettings.Values["port"] = port.Text;
            localSettings.Values["userid"] = userName.Text;
            localSettings.Values["password"] = pw.Password;
            localSettings.Values["dbName"] = dbName.Text;
        }

        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            string cmd = "checknetisolation loopbackexempt -a -n=Monitoring_04dfd829ardye";
            var dataPackage = new DataPackage();
            dataPackage.SetText(cmd);
            Clipboard.SetContent(dataPackage);
        }
    }
}
