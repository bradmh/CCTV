using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Monitoring
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Admin : Page
    {
        public Admin()
        {
            this.InitializeComponent();
            ObservableCollection<string> temp = new ObservableCollection<string>(Db.ReadData());

            type_remove.DataContext = Db.ReadData("rodzaje_zdarzen", "rodzaj_zdarzenia");
            action_remove.DataContext = Db.ReadData("przekazanie", "przekazanie");
            operator_remove.DataContext = temp;
            operator_change_password.DataContext = temp;
        }
        private void Add_user_button_Click(object sender, RoutedEventArgs e)
        {
            string user = user_name.Text;
            string pwd = "";
            try
            {
                if (password.Password == password_confirm.Password) pwd = BCrypt.Net.BCrypt.HashPassword(password.Password);
                else throw new Exception("Wprowadzone hasła nie są identyczne!");
            }
            catch (Exception ex)
            {
                ToastCreator.CreateToast("Problem z hasłem", ex.Message);
            }
            Db.User(user, pwd);
            user_name.Text = String.Empty;
            password.Password = String.Empty;
            password_confirm.Password = String.Empty;
            ContextsRefresh("operator");
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AppBar_Back_Click(object sender, RoutedEventArgs e)
        {
            ActiveUser.User = "";
            this.Frame.Navigate(typeof(MainPage));
        }
        private void Operator_remove_btn_Click(object sender, RoutedEventArgs e)
        {
            Db.User(operator_remove.SelectedValue.ToString(), "IDontNeedAPassword", "Delete");
            ContextsRefresh();
        }
        private void Change_password_btn_Click(object sender, RoutedEventArgs e)
        {
            string pwd = "";
            string user = operator_change_password.SelectedValue.ToString();

            try
            {
                if (password_change.Password == password_change_confirm.Password)
                {
                    pwd = password_change.Password; //BCrypt.Net.BCrypt.HashPassword(password_change.Password);
                    Db.User(user, pwd, "change");
                }
                else throw new Exception("Wprowadzone hasła nie są identyczne!");
            }
            catch (Exception ex)
            {
                ToastCreator.CreateToast("Problem z hasłem", ex.Message);
            }

        }
        private void type_remove_btn_Click(object sender, RoutedEventArgs e)
        {
            Db.RemoveFromSQL("rodzaje_zdarzen", "rodzaj_zdarzenia", @type_remove.SelectedValue.ToString());
            ContextsRefresh("types");
        }
        private void type_add_btn_Click(object sender, RoutedEventArgs e)
        {
            Db.AddToSQL(type_add.Text, "rodzaje_zdarzen", "rodzaj_zdarzenia");
            ContextsRefresh("types");
        }
        private void action_remove_btn_Click(object sender, RoutedEventArgs e)
        {
            Db.RemoveFromSQL("przekazanie", "przekazanie", @action_remove.SelectedValue.ToString());
            ContextsRefresh("actions");
        }
        private void action_add_btn_Click(object sender, RoutedEventArgs e)
        {
            Db.AddToSQL(action_add.Text, "przekazanie", "przekazanie");
            ContextsRefresh("actions");
        }
        private void kamery_add_btn_Click(object sender, RoutedEventArgs e)
        {
            Db.RemoveFromSQL("kamery", "kamery", "*");
            Db.AddToSQL(kamery.Text, "kamery", "kamery");
        }
        private void Logout_button_Click(object sender, RoutedEventArgs e)
        {
            ActiveUser.User = "";
            this.Frame.Navigate(typeof(MainPage));
        }
        private void AppBar_display_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DisplayEvents));
        }
        private void ContextsRefresh(string cntx = "operator")
        {
            if (cntx.Equals("types"))
            {
                type_remove.DataContext = null;
                type_remove.DataContext = Db.ReadData("rodzaje_zdarzen", "rodzaj_zdarzenia");
            }
            if (cntx.Equals("actions"))
            {
                action_remove.DataContext = null;
                action_remove.DataContext = Db.ReadData("przekazanie", "przekazanie");
            }
            if (cntx.ToLower().Equals("operator"))
            {
                ObservableCollection<string> temp = new ObservableCollection<string>(Db.ReadData());
                operator_remove.DataContext = temp;
                operator_change_password.DataContext = temp;
            }


        }
    }
}
