using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Monitoring
{
    public sealed partial class AddEvent : Page
    {
        public AddEvent()
        {
            this.InitializeComponent();
            ActiveUserTop.DataContext = ActiveUser.User;
            try
            {
                int cams_count = int.Parse(Db.ReadData("kamery", "Kamery").ElementAt(0));
                for (int i = 1; i <= cams_count; i++) cams.Items.Add(i);
                eventAction.DataContext = Db.ReadData("przekazanie", "przekazanie");
                eventType.DataContext = Db.ReadData("rodzaje_zdarzen", "rodzaj_zdarzenia");
            }
            catch (Exception ex)
            {
                ToastCreator.CreateToast(ex.Message, "Błąd podczas czytania danych z bazy!");
            }
            //dwie zmiany
            shift.Items.Add(1);
            shift.Items.Add(2);
        }
        private void addEventToSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!eventDate.Date.HasValue ||
                cams.SelectedItem == null ||
                shift.SelectedItem == null ||
                eventType.SelectedItem == null ||
                eventAction.SelectedItem == null ||
                eventLoc.Text.Length < 3) throw new Exception("Popraw błędy w formularzu");
                else
                {
                    string date = String.Format("{0}-{1}-{2}",
                        eventDate.Date.Value.Day,
                        eventDate.Date.Value.Month,
                        eventDate.Date.Value.Year);
                    string dateTime = String.Format("{0} {1}",
                        date, eventTime.Time);
                    DateTime dtime = DateTime.Parse(dateTime);
                    Zdarzenie evnt = new Zdarzenie(dtime,
                        int.Parse(cams.SelectedValue.ToString()),
                        int.Parse(shift.SelectedValue.ToString()),
                        eventType.SelectedValue.ToString(),
                        eventAction.SelectedValue.ToString(),
                        eventLoc.Text.ToString());

                    Db.ZdarzenieToSQL(evnt);
                    ToastCreator.CreateToast("Zdarzenie zostało dodane", "Wszystko ok!");
                }
            }
            catch (Exception ex)
            {
                ToastCreator.CreateToast(ex.Message, "Błąd!");
            }
        }
        private void AppBar_display_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DisplayEvents));

        }
        private void User_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserPage));
        }
        private void adminBtnTop_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveUser.User.Equals("Administrator")) this.Frame.Navigate(typeof(Admin));
            else ToastCreator.CreateToast("Nie masz wystarczających uprawnień żeby przejść do ustawień administratora.", "Brak autoryzacji!");
        }
        private void Logout_button_Click(object sender, RoutedEventArgs e)
        {
            ActiveUser.User = "";
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
