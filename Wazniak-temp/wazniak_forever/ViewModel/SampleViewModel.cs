using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wazniak_forever.Model;

namespace wazniak_forever.ViewModel
{
    public class SampleViewModel : INotifyPropertyChanged
    {
        private SampleItemContext _sampleDB;

        public SampleViewModel()
        {
            _sampleDB = new SampleItemContext();
        }

        private MobileServiceCollection<SampleItem, SampleItem> _allSampleItems;
        public MobileServiceCollection<SampleItem, SampleItem> AllSampleItems
        {
            get { return _allSampleItems; }
            set
            {
                _allSampleItems = value;
                NotifyPropertyChanged("AllSampleItems");
            }
        }

        private List<Option> _allOptions;

        public List<Option> AllOptions
        {
            get { return _allOptions; }
            set
            {
                _allOptions = value;
                NotifyPropertyChanged("AllOptions");
            }
        }

        private bool _onlineCourses;

        public bool OnlineCourses
        {
            get { return _onlineCourses; }
            set
            {
                _onlineCourses = value;
                NotifyPropertyChanged("OnlineCourses");
            }
        }

        private List<Course> _downloadedCourses;

        public List<Course> DownloadedCourses
        {
            get { return _downloadedCourses; }
            set
            {
                _downloadedCourses = value;
                NotifyPropertyChanged("DownloadedCourses");
            }
        }

        private List<Course> _allCourses;

        public List<Course> AllCourses
        {
            get { return _allCourses; }
            set
            {
                _allCourses = value;
                NotifyPropertyChanged("AllCourses");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public async void LoadCollectionsFromDatabase()
        {
            AllSampleItems = await _sampleDB.Items.ToCollectionAsync();
        }

        public void LoadMenu()
        {
            AllOptions = new List<Option>()
            {
                new Option(OptionType.MyCourses, "My Courses", new Uri("/Assets/StartIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Downloads, "Downloads", new Uri("/Assets/DownloadsIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Settings, "Settings", new Uri("/Assets/SettingsIcon.png", UriKind.RelativeOrAbsolute))
            };
        }

        public void LoadDownloadedCourses()
        {
            AllCourses = new List<Course>()
            {
                new Course("Databases")
            };
        }

        public void LoadCourses()
        {
            AllCourses = new List<Course>()
            {
                new Course("Algorithms I"),
                new Course("Algorithms II"),
                new Course("Databases"),
                new Course("Operating Systems"),
                new Course("Linear Algebra I"),
                new Course("Linear Algebra II"),
                new Course("Linear Algebra III"),
                new Course("Numerical Analysis")
            };
        }

        public async void AddSampleItem(SampleItem newSampleItem)
        {
            await _sampleDB.Items.InsertAsync(newSampleItem);
            AllSampleItems.Add(newSampleItem);

        }

        public async void DeleteSampleitem(SampleItem removedSampleitem)
        {
            await _sampleDB.Items.DeleteAsync(removedSampleitem);
            AllSampleItems.Remove(removedSampleitem);
        }
    }
}
