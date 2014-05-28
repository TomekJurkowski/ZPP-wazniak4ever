using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Navigation;
using wazniak_forever.Controls;

namespace wazniak_forever
{
    public partial class CourseSelection : PhoneApplicationPage
    {
        public CourseSelection()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            if (App.ViewModel.AreDownloads) MainPivot.Items.Add(CreatePivotItem("Downloads", "DownloadedCourses"));
            else
            {
                LoadPivot();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadCoursesAsync();
            if (App.ViewModel.db.User != null && !App.ViewModel.AreDownloads)
            {
                MainPivot.Items.Remove(MainPivot.Items[0]);
                MainPivot.Items.Insert(0, CreatePivotItem("My Courses", "MyCourses"));
                MainPivot.SelectedIndex = 0;
            }
        }

        private async void LoadCoursesAsync()
        {
            if (App.ViewModel.OnlineMode)
                await LoadFromAzure();
            await LoadFromLocalDatabase();
        }

        private async System.Threading.Tasks.Task LoadFromLocalDatabase()
        {
            await App.ViewModel.PerformTimeConsumingProcess(this, "Loading downloaded courses...", App.ViewModel.LoadDownloadedCourses);
        }

        private async System.Threading.Tasks.Task LoadFromAzure()
        {
            await App.ViewModel.PerformTimeConsumingProcess(this, "Loading Modules...", App.ViewModel.LoadModules);
            if (App.ViewModel.db.User != null)
                await App.ViewModel.PerformTimeConsumingProcess(this, "Loading My Courses...", App.ViewModel.LoadMyCourses);
            await App.ViewModel.PerformTimeConsumingProcess(this, "Loading all courses...", App.ViewModel.LoadAllCourses);
            await App.ViewModel.PerformTimeConsumingProcess(this, "Loading new courses...", App.ViewModel.LoadNewCourses);
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
            if (App.ViewModel.db.User != null)  
                MainPivot.Items.Add(CreatePivotItem("My Courses", "MyCourses"));
            MainPivot.Items.Add(CreatePivotItem("New", "NewCourses"));
            MainPivot.Items.Add(CreatePivotItem("All Courses", "AllCourses"));
            MainPivot.Items.Add(CreatePivotItem("Downloads", "DownloadedCourses"));
        }
        
    }
}
