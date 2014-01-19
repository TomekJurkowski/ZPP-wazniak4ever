using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using wazniak_forever.Model;

namespace wazniak_forever.ViewModel
{
    public class SampleViewModel : INotifyPropertyChanged
    {
        private SampleItemContext _sampleDB;

        public SampleViewModel()
        {
            _sampleDB = new SampleItemContext();

            CheckForNetworkAvailability();
        }

        private bool _onlineMode;
        public bool OnlineMode
        {
            get { return _onlineMode; }
            set
            {
                _onlineMode = value;
                NotifyPropertyChanged("OnlineMode");
            }
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

        private List<Course> _myCourses;

        public List<Course> MyCourses
        {
            get { return _myCourses; }
            set
            {
                _myCourses = value;
                NotifyPropertyChanged("MyCourses");
            }
        }

        private bool _areDownloads;

        public bool AreDownloads
        {
            get { return _areDownloads; }
            set
            {
                _areDownloads = value;
                NotifyPropertyChanged("AreDownloads");
            }
        }

        #region ExerciseSolving

        private int _currentQuestionNumber;

        public int CurrentQuestionNumber
        {
            get { return _currentQuestionNumber; }
            set
            {
                _currentQuestionNumber = value;
                NotifyPropertyChanged("CurrentQuestionNumber");
            }
        }

        private AnyExercise _currentExercise;

        public AnyExercise CurrentExercise
        {
            get { return _currentExercise; }
            set
            {
                _currentExercise = value;
                NotifyPropertyChanged("CurrentExercise");
            }
        }


        private List<AnyExercise> _exercises;

        public List<AnyExercise> Exercises
        {
            get { return _exercises; }
            set
            {
                _exercises = value;
                NotifyPropertyChanged("Exercises");
            }
        }

        private List<string> _userChoices = new List<string>();

        public List<string> UserChoices
        {
            get { return _userChoices; }
            set
            {
                _userChoices = value;
                NotifyPropertyChanged("UserChoices");
            }
        }

        public void LoadExercises() {
            Exercises = new List<AnyExercise>() {
                new MultipleChoiceExercise() {
                    Question = "This is a question",
                    Choices = new List<string>() {
                        "first choice",
                        "second choice",
                        "third choice"
                    },
                    Solutions = new List<MultipleChoiceSolution>() {
                        new MultipleChoiceSolution(true, "explanation one"),
                        new MultipleChoiceSolution(false, "explanation two"),
                        new MultipleChoiceSolution(true, "explanation three"),
                    }
                },
                new OpenExercise() {
                    Question = "This is a question",
                    Solution = new OpenExerciseSolution("answer", "explanation")
                },
                new MultipleChoiceExercise() {
                    Question = "This is a question",
                    Choices = new List<string>() {
                        "another first choice",
                        "another second choice",
                        "another third choice"
                    },
                    Solutions = new List<MultipleChoiceSolution>() {
                        new MultipleChoiceSolution(true, "another explanation")
                    }
                }
            };

            CurrentQuestionNumber = 0;
            CurrentExercise = Exercises[0];
            if (CurrentExercise.Type == ExerciseType.MultipleChoice)
                UserChoices = ((MultipleChoiceExercise)Exercises[0]).Choices;
        }
        
        #endregion

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

        public void CheckForNetworkAvailability()
        {
            //NetworkInterface.GetIsNetworkAvailable()
            _onlineMode = DeviceNetworkInformation.IsNetworkAvailable; 
        }

        public void LoadMenu()
        {
            AllOptions = new List<Option>()
            {
                new Option(OptionType.MyCourses, true, "Courses", new Uri("/Assets/StartIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Downloads, false, "Downloads", new Uri("/Assets/DownloadsIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Settings, false, "Settings", new Uri("/Assets/SettingsIcon.png", UriKind.RelativeOrAbsolute))
            };
        }

        public void LoadAllCourses()
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

        public void LoadDownloadedCourses()
        {
            DownloadedCourses = new List<Course>()
            {
                new Course("Databases")
            };
        }

        public void LoadMyCourses()
        {
            MyCourses = new List<Course>()
            {
                new Course("Algorithms I"),
                new Course("Algorithms II"),
                new Course("Databases")
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
