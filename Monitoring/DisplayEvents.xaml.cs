using Microsoft.Toolkit.Uwp.UI.Controls;
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
    public sealed partial class DisplayEvents : Page
    {

        bool areFiltersApplied = false;
        ObservableCollection<string> AppliedFilters = new ObservableCollection<string>();
        Dictionary<int, string> months = new Dictionary<int, string>
                {
                    {1, "Styczeń" },
                    {2, "Luty" },
                    {3, "Marzec" },
                    {4, "Kwiecień" },
                    {5, "Maj" },
                    {6, "Czerwiec" },
                    {7, "Lipiec" },
                    {8, "Sierpień" },
                    {9, "Wrzesień" },
                    {10, "Październik" },
                    {11, "Listopad" },
                    {12, "Grudzień" }
                };

        public DisplayEvents()
        {
            this.InitializeComponent();
            //IF NOT ADMIN SHOW ONLY USER'S EVENTS AND DISABLE FILTERS
            if (ActiveUser.User.Equals("Administrator"))
            {
                ObservableCollection<Zdarzenie> list = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                           orderby item.data_godzina_zdarzenia ascending
                                                                                           select item);
                dtGrid.DataContext = list;
                List<string> uniqueOps = new List<string>();
                List<string> uniqueActions = new List<string>();
                List<string> uniqueTypes = new List<string>();

                //Creating list of unique operators, types and actions to fill filter menu
                foreach (Zdarzenie evt in list)
                {
                    if (!uniqueOps.Contains(evt.user.ToString())) uniqueOps.Add(evt.user.ToString());
                    if (!uniqueActions.Contains(evt.przekazanie.ToString())) uniqueActions.Add(evt.przekazanie.ToString());
                    if (!uniqueTypes.Contains(evt.rodzaj_zdarzenia.ToString())) uniqueTypes.Add(evt.rodzaj_zdarzenia.ToString());

                }
                AddFiltersMenu(uniqueOps, uniqueActions, uniqueTypes);
            }
            else
            {
                filter_btn.IsEnabled = false;
                ObservableCollection<Zdarzenie> list = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                           where item.user.Equals(ActiveUser.User)
                                                                                           orderby item.data_godzina_zdarzenia ascending
                                                                                           select item);
                dtGrid.DataContext = list;
                opcol.Visibility = Visibility.Collapsed;
                label_count.DataContext = list.Count();
            }
        }
        private void AddFiltersMenu(List<string> uniqueOps, List<string> uniqueActions, List<string> uniqueTypes)
        {
            //Adding buttons - CLEAR FILTERS
            MenuFlyoutItem opx = new MenuFlyoutItem();
            opx.Text = "Wyczyść filtry";
            opx.Click += ItemClicked;
            flyoutmenuFilter.Items.Add(opx);

            //Adding buttons with menu (SUBBUTTONS)

            //OPERATORZY
            MenuFlyoutSubItem operatorChose = new MenuFlyoutSubItem();
            operatorChose.Text = "Operator";

            //Adding each operator to subbutton OPERATORZY
            foreach (string uniqueOp in uniqueOps)
            {
                MenuFlyoutItem op = new MenuFlyoutItem();
                op.Text = uniqueOp;
                op.Click += ItemClicked;
                operatorChose.Items.Add(op);

            }
            //Adding button to menu
            flyoutmenuFilter.Items.Add(operatorChose);


            //PRZEKAZANIA
            MenuFlyoutSubItem actionChose = new MenuFlyoutSubItem();
            actionChose.Text = "Przekazanie";

            //Adding each action to subbutton PRZEKAZANIA
            foreach (string uniqueAction in uniqueActions)
            {
                MenuFlyoutItem ac = new MenuFlyoutItem();
                ac.Text = uniqueAction;
                ac.Click += ItemClickedActions;
                actionChose.Items.Add(ac);

            }
            //Adding button to menu
            flyoutmenuFilter.Items.Add(actionChose);

            //RODZAJE
            MenuFlyoutSubItem typeChose = new MenuFlyoutSubItem();
            typeChose.Text = "Rodzaj zdarzenia";

            //Adding each type to subbutton Rodzaje
            foreach (string uniqueType in uniqueTypes)
            {
                MenuFlyoutItem tp = new MenuFlyoutItem();
                tp.Text = uniqueType;
                tp.Click += ItemClickedTypes;
                typeChose.Items.Add(tp);

            }
            //Adding button to menu
            flyoutmenuFilter.Items.Add(typeChose);

            //MIESIĄCE
            MenuFlyoutSubItem monthsBtn = new MenuFlyoutSubItem();
            monthsBtn.Text = "Miesiąc";

            //Adding months to subbutton Miesiące
            foreach (KeyValuePair<int, string> month in months)
            {
                MenuFlyoutItem mt = new MenuFlyoutItem();
                mt.Text = month.Value;
                mt.Click += ItemClickedMonths;
                monthsBtn.Items.Add(mt);

            }
            //Adding button to menu
            flyoutmenuFilter.Items.Add(monthsBtn);
        }
        private void ItemClicked(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem selected = sender as MenuFlyoutItem;
            AppliedFilters.Add(selected.Text);
            //wybieranie z bazy tylko wyników z wybranym tekstem (-operatorem)
            ObservableCollection<Zdarzenie> filteredList = (ObservableCollection<Zdarzenie>)dtGrid.DataContext;


            if (selected.Text.Equals("Wyczyść filtry"))
            {
                AppliedFilters.Clear();
                //we need list.Count() == 0 ( Check items_refreshed() )
                dtGrid.DataContext = new ObservableCollection<Zdarzenie>();
                areFiltersApplied = false;
            }
            else
            {
                dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in filteredList
                                                                         where item.user.Equals(selected.Text)
                                                                         select item);
                areFiltersApplied = true;
            }

        }
        private void ItemClickedActions(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem selected = sender as MenuFlyoutItem;
            AppliedFilters.Add(selected.Text);
            //wybieranie z bazy tylko wyników z wybranym tekstem (-operatorem)
            ObservableCollection<Zdarzenie> filteredList = (ObservableCollection<Zdarzenie>)dtGrid.DataContext;

            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in filteredList
                                                                     where item.przekazanie.Equals(selected.Text)
                                                                     select item);
            areFiltersApplied = true;

        }
        private void ItemClickedTypes(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem selected = sender as MenuFlyoutItem;
            AppliedFilters.Add(selected.Text);
            //wybieranie z bazy tylko wyników z wybranym tekstem (-operatorem)
            ObservableCollection<Zdarzenie> filteredList = (ObservableCollection<Zdarzenie>)dtGrid.DataContext;

            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in filteredList
                                                                     where item.rodzaj_zdarzenia.Equals(selected.Text)
                                                                     select item);
            areFiltersApplied = true;
        }
        private void ItemClickedMonths(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem selected = sender as MenuFlyoutItem;
            AppliedFilters.Add(selected.Text);
            //wybieranie z bazy tylko wyników z wybranym tekstem (-operatorem)
            ObservableCollection<Zdarzenie> filteredList = (ObservableCollection<Zdarzenie>)dtGrid.DataContext;

            foreach (KeyValuePair<int, string> month in months)
            {
                if (month.Value == selected.Text) dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in filteredList
                                                                                                           where item.data_godzina_zdarzenia.Month.Equals(month.Key)
                                                                                                           select item);
            }
            areFiltersApplied = true;
        }
        private void AppBar_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
        private void dtgrid_sorting(object sender, DataGridColumnEventArgs e)
        {
            if (ActiveUser.User == "Administrator")
            {
                try
                {
                    if (e.Column.Tag.Equals("Event_Date"))
                    {
                        if (!(e.Column.SortDirection == DataGridSortDirection.Ascending))
                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.data_godzina_zdarzenia ascending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Ascending;
                        }
                        else

                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.data_godzina_zdarzenia descending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Descending;
                        }
                    }

                    else if (e.Column.Tag.Equals("user"))
                    {
                        if (!(e.Column.SortDirection == DataGridSortDirection.Ascending))
                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.user ascending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Ascending;
                        }
                        else

                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.user descending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Descending;
                        }
                    }

                    else if (e.Column.Tag.Equals("cam"))
                    {
                        if (!(e.Column.SortDirection == DataGridSortDirection.Ascending))
                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.kamera ascending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Ascending;
                        }
                        else

                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.kamera descending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Descending;
                        }
                    }

                    else if (e.Column.Tag.Equals("shift"))
                    {
                        if (!(e.Column.SortDirection == DataGridSortDirection.Ascending))
                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.zmiana ascending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Ascending;
                        }
                        else

                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.zmiana descending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Descending;
                        }
                    }

                    else if (e.Column.Tag.Equals("created"))
                    {
                        if (!(e.Column.SortDirection == DataGridSortDirection.Ascending))
                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.utworzone_data ascending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Ascending;
                        }
                        else

                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.utworzone_data descending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Descending;
                        }
                    }

                    else if (e.Column.Tag.Equals("type"))
                    {
                        if (!(e.Column.SortDirection == DataGridSortDirection.Ascending))
                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.rodzaj_zdarzenia ascending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Ascending;
                        }
                        else

                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.rodzaj_zdarzenia descending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Descending;
                        }
                    }

                    else if (e.Column.Tag.Equals("action"))
                    {
                        if (!(e.Column.SortDirection == DataGridSortDirection.Ascending))
                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.przekazanie ascending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Ascending;
                        }
                        else

                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.przekazanie descending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Descending;
                        }
                    }
                    else if (e.Column.Tag.Equals("loc"))
                    {
                        if (!(e.Column.SortDirection == DataGridSortDirection.Ascending))
                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.lokalizacja ascending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Ascending;
                        }
                        else

                        {
                            dtGrid.DataContext = new ObservableCollection<Zdarzenie>(from item in Db.ReadData_Zdarzenie()
                                                                                     orderby item.lokalizacja descending
                                                                                     select item);
                            e.Column.SortDirection = DataGridSortDirection.Descending;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToastCreator.CreateToast(ex.Message, "Błąd");
                }
            }
        }
        private void items_refreshed(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ActiveUser.User == "Administrator")
            {


                ObservableCollection<Zdarzenie> list = (ObservableCollection<Zdarzenie>)dtGrid.DataContext;
                if (list.Count() == 0)
                {
                    ToastCreator.CreateToast("Filtry zostały usunięte. Powodem może być brak danych spełniających warunki lub ponowne wybranie tego samego filtra.", "Uwaga!");
                    list = Db.ReadData_Zdarzenie();
                    dtGrid.DataContext = list;
                    areFiltersApplied = false;
                    AppliedFilters.Clear();
                    label_count.DataContext = list.Count();
                    chosenFilters.Text = "";
                }
                else
                {
                    label_count.DataContext = list.Count();
                }

                if (areFiltersApplied == true)
                {
                    chosenFiltersFlat.Visibility = Visibility.Visible;
                    chosenFilters.Visibility = Visibility.Visible;
                    chosenFilters.Text = "";
                    foreach (string filter in AppliedFilters)
                    {
                        chosenFilters.Text += filter;
                        chosenFilters.Text += " ";
                    }
                }
                else
                {
                    chosenFilters.Text = "";
                    chosenFiltersFlat.Visibility = Visibility.Collapsed;
                    chosenFilters.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void Filter_Click(object sender, RoutedEventArgs e)
        {

        }
        private void admin_btn_top_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveUser.User.Equals("Administrator")) this.Frame.Navigate(typeof(Admin));
            else ToastCreator.CreateToast("Nie masz uprawnień żeby przejść do ustawień administratora!", "Wstęp wzbroniony!");
        }

    }
}
