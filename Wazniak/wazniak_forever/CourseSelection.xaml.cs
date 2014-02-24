using Microsoft.Phone.Controls;
using System.Windows;
using wazniak_forever.Controls;

namespace wazniak_forever
{
    public partial class CourseSelection : PhoneApplicationPage
    {
        public CourseSelection()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadDownloadedCourses();
            if (App.ViewModel.AreDownloads) MainPivot.Items.Add(CreatePivotItem("Downloads", "DownloadedCourses"));
            else
            {
                App.ViewModel.LoadMyCourses();
                App.ViewModel.LoadAllCourses();
                App.ViewModel.LoadNewCourses();
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
            MainPivot.Items.Add(CreatePivotItem("My Courses", "MyCourses"));
            MainPivot.Items.Add(CreatePivotItem("New", "New"));
            MainPivot.Items.Add(CreatePivotItem("All Courses", "AllCourses"));
            MainPivot.Items.Add(CreatePivotItem("Downloads", "DownloadedCourses"));
        }
        
    }
}
