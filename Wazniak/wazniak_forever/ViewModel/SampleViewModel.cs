using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                    Question = "Give the array that results after the first 6 exchanges (not iterations!) when insertion sorting the following array:\n\t10 14 31 36 43 95 28 90 60 99",
                    Solution = new OpenExerciseSolution("10 14 28 31 36 43 90 60 95 99", "Here is the array after each exchange:\n\t10 14 31 36 43 95 28 90 60 99\n1: 10 14 31 36 43 28 95 90 60 99\n2: 10 14 31 36 28 43 95 90 60 99\n3: 10 14 31 28 36 43 95 90 60 99\n4: 10 14 28 31 36 43 95 90 60 99\n5: 10 14 28 31 36 43 90 95 60 99\n6: 10 14 28 31 36 43 90 60 95 99")
                },
                new OpenExercise() {
                    Question = "Consider the left-leaning red-black BST whose level-order traversal is:\n\t84 46 86 23 55 85 88 16 32 53 63 11 31\nList (in ascending order) the keys in the red nodes. A node is red if the link from its parent is red.",
                    Solution = new OpenExerciseSolution("11 31 46", "The shape of a BST is uniquely determined by its level-order traversal. To deduce which links are red, recall that the length of every path from the root to a null link has the same number of black links; apply this property starting from nodes at the bottom.")
                },
                new OpenExercise() {
                    Question = "Consider the left-leaning red-black BST whose level-order traversal is\n\t44 28 93 24 32 75 97 57 77 49 ( red links = 49 75 )\nWhat is the level-order traversal of the red-black BST that results after inserting the following sequence of keys: 30 92 61",
                    Solution = new OpenExerciseSolution("75 44 93 28 57 92 97 24 32 49 61 77 30", "Here is the level-order traversal of the red-black BST after each insertion:\n\t44 28 93 24 32 75 97 57 77 49 ( red links = 49 75 )\n30: 44 28 93 24 32 75 97 30 57 77 49 ( red links = 30 49 75 )\n92: 44 28 93 24 32 75 97 30 57 92 49 77 ( red links = 30 49 75 77 )\n61: 75 44 93 28 57 92 97 24 32 49 61 77 30 ( red links = 30 44 77 )")
                },
                new MultipleChoiceExercise() {
                    Question = "This is a question",
                    Choices = new List<string>() {
                        "another first choice",
                        "another second choice",
                        "another third choice"
                    },
                    Solutions = new List<MultipleChoiceSolution>() {
                        new MultipleChoiceSolution(true, "another explanation"),
                        new MultipleChoiceSolution(false, "explanation two"),
                        new MultipleChoiceSolution(true, "explanation three"),

                    }
                },
                new OpenExercise() {
                    Question = "Suppose that you do binary search for the key 13 in the following sorted array of size 15:\n\t14 25 26 36 38 47 53 55 59 67 78 84 89 90 97\nGive the sequence of keys in the array that are compared with 13.",
                    Solution = new OpenExerciseSolution("55 36 25 14", "Here is the array to be searched after each compare:\n\t14 25 26 36 38 47 53 55 59 67 78 84 89 90 97\n55:  14 25 26 36 38 47 53  -  -  -  -  -  -  -  -\n36:  14 25 26  -  -  -  -  -  -  -  -  -  -  -  -\n25:  14  -  -  -  -  -  -  -  -  -  -  -  -  -  -\n14:   -  -  -  -  -  -  -  -  -  -  -  -  -  -  -")
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
