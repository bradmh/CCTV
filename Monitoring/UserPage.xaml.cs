using System;
using System.Collections.Generic;
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
    public sealed partial class UserPage : Page
    {
        public UserPage()
        {
            this.InitializeComponent();
        }
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddEvent));
        }
        private void change_password_Click(object sender, RoutedEventArgs e)
        {
            string pwd = "";
            bool isCurrentPasswordCorrect = Db.ValidateUser(ActiveUser.User, password_current.Password);
            try
            {
                if (isCurrentPasswordCorrect)
                {
                    if (password_change.Password == password_change_confirm.Password)
                    {
                        pwd = password_change.Password; //BCrypt.Net.BCrypt.HashPassword(password_change.Password);
                        Db.User(ActiveUser.User, pwd, "change");
                        ActiveUser.User = "";
                        this.Frame.Navigate(typeof(MainPage));
                    }
                    else throw new Exception("Wprowadzone hasła nie są identyczne!");
                }
                else throw new Exception("Popraw aktualne hasło!");
            }
            catch (Exception ex)
            {
                ToastCreator.CreateToast(ex.Message, "Problem z hasłem");
            }
        }

    }
}
