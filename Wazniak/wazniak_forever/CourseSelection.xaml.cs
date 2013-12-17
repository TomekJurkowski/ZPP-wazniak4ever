using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wazniak_forever.Model;

namespace wazniak_forever
{
    public partial class CourseSelection : PhoneApplicationPage
    {
        public CourseSelection()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadDownloadedCourses();
            if (!App.ViewModel.AreDownloads)
            {
                App.ViewModel.LoadAllCourses();
                App.ViewModel.LoadMyCourses();
                LoadPivot();
            }
        }

        private PivotItem CreatePivotItem(string header, string name)
        {
            PivotItem Item = new PivotItem();
            Item.Header = header;
            Item.Foreground = Application.Current.Resources["PageNameColor"] as System.Windows.Media.Brush;
            CoursePivotItem CoursesItem = new CoursePivotItem();
            CoursesItem.Name = name;
            CoursesItem.setBinding(CoursesItem.Name);
            Item.Content = CoursesItem;
            return Item;
        }

        private void LoadPivot()
        {
            MainPivot.Items.Add(CreatePivotItem("All Courses", "AllCourses"));
            MainPivot.Items.Add(CreatePivotItem("My Courses", "MyCourses"));
        }
        
    }
}