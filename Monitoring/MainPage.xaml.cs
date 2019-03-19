using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Monitoring
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                DbCreds.GetCreds();
                UsersComboBox.DataContext = Db.ReadData();
            }
            catch (Exception ex)
            {
                ToastCreator.CreateToast(ex.Message, "Błąd!");
            }
        }
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedUser = UsersComboBox.SelectedValue.ToString();
            bool auth = false;
            try
            {
                auth = Db.ValidateUser(selectedUser, passwordBox.Password);
            }
            catch (Exception ex)
            {
                ToastCreator.CreateToast(ex.Message, "Błąd uwierzytelnienia");
            }
            if (auth)
            {
                ActiveUser.User = selectedUser;
                this.Frame.Navigate(typeof(AddEvent));
            }
            else ToastCreator.CreateToast("Podano złe hasło!", "Błąd uwierzytelnienia");
        }
        private void ActivatePredefinedSettings(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.F7)
            {
                if (predefinedSettingsBtn.Visibility == Visibility.Collapsed) predefinedSettingsBtn.Visibility = Visibility.Visible;
                else predefinedSettingsBtn.Visibility = Visibility.Collapsed;
            }
        }
        private void PredefinedSettings_Click(object sender, RoutedEventArgs e)
        {
            //Przed uruchomieniem tej funkcji pusta baza danych musi być stworzona!!
            //Predefined Settings in four lists (5th list in Db.CreateTables())
            List<string> predef_ops = new List<string>{
                "Administrator",
                "User 1",
                "User 2",
                "User 3",
                "User 4",
                "User 5",
                "User 6",
                "User 7",
                "User 8",
                "User 9",
                "User 10",
                "User 11",
                "User 12" };
            List<string> predef_pws = new List<string>{
                "dsa",
                "asd",
                "asd",
                "asd",
                "asd",
                "asd",
                "asd",
                "asd",
                "asd",
                "asd",
                "asd",
                "asd",
                "asd" };
            List<string> predef_przekazanie = new List<string>{
               "Straż Miejska",
               "Komenda Miejska Policji",
               "Państwowa Straż Pożarna",
               "Pogotowie",
               "Portiernia",
               "Sporządzona notatka",
               "Inne"};
            List<string> predef_rodzaje = new List<string>{
                "Dewastacja mienia",
                "Kradzież",
                "Zakłucanie spokoju",
                "Przestępstwo przeciwko osobie",
                "Źle zaparkowany samochód",
                "Wypadki",
                "Leżący człowiek",
                "Podpalenia i pożary",
                "Awaria kamery",
                "Brak nagrywania",
                "Sprawdzenie systemów",
                "Sprawdzenie zabezpieczenia budynku",
                "Zamknięcie budynku",
                "Awaria techniczna",
                "Inne"};
            try
            {
                //Creating all tables
                Db.CreateTables();
                //Adding users with passwords from lists
                foreach (var op in predef_ops.Zip(predef_pws, Tuple.Create)) Db.User(@op.Item1, @op.Item2);
                //Adding actions, types and setting up 12 cams
                Db.AddToSQLBulk(predef_przekazanie, "przekazanie", "przekazanie");
                Db.AddToSQLBulk(predef_rodzaje, "rodzaje_zdarzen", "rodzaj_zdarzenia");
                Db.AddToSQL("12", "kamery", "kamery");
            }
            catch (Exception ex)
            {
                ToastCreator.CreateToast(ex.Message, "Błąd!");
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }
    }
}
