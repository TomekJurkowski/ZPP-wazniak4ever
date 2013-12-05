using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace wazniak_forever
{
    public partial class CourseSelection : PhoneApplicationPage
    {
        public CourseSelection()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadCourses();
        }

        private void Pivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            if (e.Item == AllCourses)
                ApplicationBar.IsVisible = true;
            else
                ApplicationBar.IsVisible = false; 
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AllCoursesList.SelectedItem = null;
            NavigationService.Navigate(new Uri("/Exercise.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}