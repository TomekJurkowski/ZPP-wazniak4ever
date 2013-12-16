using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using wazniak_forever.Model;
using wazniak_forever.Resources;
using wazniak_forever.ViewModel;

namespace wazniak_forever
{
   

    public partial class MainPage : PhoneApplicationPage
    {
        // MobileServiceCollectionView implements ICollectionView (useful for databinding to lists) and 
        // is integrated with your Mobile Service to make it easy to bind your data to the ListView
        /*private MobileServiceCollection<TodoItem, TodoItem> items;

        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();*/

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var sampleItem = new SampleItem { Text = TodoInput.Text };
            App.ViewModel.AddSampleItem(sampleItem); ;
        }

        /*private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            item.Complete = true;
            UpdateCheckedTodoItem(item);
        }*/

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.ViewModel.LoadCollectionsFromDatabase();
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.LoadCollectionsFromDatabase();
        }
    }
}