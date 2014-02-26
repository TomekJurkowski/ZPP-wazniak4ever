using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using wazniak_forever.Model;
using Microsoft.Phone.Shell;

namespace wazniak_forever.ViewModel
{
    public class ClarifierViewModel : INotifyPropertyChanged
    {
        private DatabaseContext db;

        public ClarifierViewModel()
        {
            db = new DatabaseContext();

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

        private List<Option> _courseOptions;

        public List<Option> CourseOptions
        {
            get { return _courseOptions; }
            set
            {
                _courseOptions = value;
                NotifyPropertyChanged("CourseOptions");
            }
        }

        private List<Subject> _allCourses;

        public List<Subject> AllCourses
        {
            get { return _allCourses; }
            set
            {
                _allCourses = value;
                NotifyPropertyChanged("AllCourses");
            }
        }

        private List<Subject> _downloadedCourses;

        public List<Subject> DownloadedCourses
        {
            get { return _downloadedCourses; }
            set
            {
                _downloadedCourses = value;
                NotifyPropertyChanged("DownloadedCourses");
            }
        }

        private List<Subject> _myCourses;

        public List<Subject> MyCourses
        {
            get { return _myCourses; }
            set
            {
                _myCourses = value;
                NotifyPropertyChanged("MyCourses");
            }
        }

        private List<Subject> _newCourses;

        public List<Subject> NewCourses
        {
            get { return _newCourses; }
            set
            {
                _newCourses = value;
                NotifyPropertyChanged("NewCourses");
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

        private int _correctAnswers = 0;
        public int CorrectAnswers
        {
            get { return _correctAnswers; }
            set
            {
                _correctAnswers = value;
                NotifyPropertyChanged("CorrectAnswers");
            }
        }

        private RegularExercise _currentExercise;
        public RegularExercise CurrentExercise
        {
            get { return _currentExercise; }
            set
            {
                _currentExercise = value;
                NotifyPropertyChanged("CurrentExercise");
            }
        }

        private Solution _currentSolution;
        public Solution CurrentSolution
        {
            get { return _currentSolution; }
            set
            {
                _currentSolution = value;
                NotifyPropertyChanged("CurrentSolution");
            }
        }
        public int CurrentCourseID { get; set; }

        private List<RegularExercise> _exercises;
        public List<RegularExercise> Exercises
        {
            get { return _exercises; }
            set
            {
                _exercises = value;
                NotifyPropertyChanged("Exercises");
            }
        }

        private List<Solution> _solutions;
        public List<Solution> Solutions
        {
            get { return _solutions; }
            set
            {
                _solutions = value;
                NotifyPropertyChanged("Solutions");
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

        private Solution prepareSolution(Answer ans)
        {
            return null;
        }

        public async System.Threading.Tasks.Task LoadExercises()
        {
            /*var exercises = await db.Tasks.Where(task => task.SubjectID == CurrentCourseID)
                .ToListAsync();
            var exerciseIds = exercises.Select(task => task.ID).ToList();*/

            var tasksWithAnswers = await db.TasksWithAnswers.ToListAsync();

            System.Diagnostics.Debug.WriteLine(tasksWithAnswers.Count);
            /*var exerciseAnswer = from exercise in exercises
                                 join answer in answers on exercise.ID equals answer.TaskID
                                 select new { exercise, answer };*/

            Exercises = new List<RegularExercise>();
            Solutions = new List<Solution>();

            foreach (var task in tasksWithAnswers)
            {
                Solution solution = null;

                System.Diagnostics.Debug.WriteLine(task.Title);

                switch (task.AnswerDiscriminator) 
                {
                    case "SingleValueAnswer":
                        solution = new SingleValueSolution(task.TaskID, task.Value, null);
                        break;
                    case "TextAnswer":
                        solution = new TextSolution(task.TaskID, task.AnswerText, null);
                        break;
                }

                Solutions.Add(solution);

                System.Diagnostics.Debug.WriteLine(task.AnswerDiscriminator);

                switch (task.TaskDiscriminator)
                {
                    case "RegularTask":
                        var ex = new RegularExercise(task.ID, CurrentCourseID, task.TaskID,
                            task.Title, task.Text1, 
                            AllCourses.Where(course => course.ID == CurrentCourseID).First(),
                            solution);
                        solution.Exercise = ex;
                        Exercises.Add(ex);
                        break;
                }                      
            }
            System.Diagnostics.Debug.WriteLine("Finished");
            /*
            Exercises = new List<RegularExercise>();
            string[] questions = new string[6] 
            { 
                "This is a question", 
                "Give the array that results after the first 6 exchanges (not iterations!) when insertion sorting the following array:\n\t10 14 31 36 43 95 28 90 60 99",
                "Consider the left-leaning red-black BST whose level-order traversal is:\n\t84 46 86 23 55 85 88 16 32 53 63 11 31\nList (in ascending order) the keys in the red nodes. A node is red if the link from its parent is red.",
                "Consider the left-leaning red-black BST whose level-order traversal is\n\t44 28 93 24 32 75 97 57 77 49 ( red links = 49 75 )\nWhat is the level-order traversal of the red-black BST that results after inserting the following sequence of keys: 30 92 61",
                "This is a question",
                "Suppose that you do binary search for the key 13 in the following sorted array of size 15:\n\t14 25 26 36 38 47 53 55 59 67 78 84 89 90 97\nGive the sequence of keys in the array that are compared with 13."
            };
            for (int i = 0; i < 6; i++)
            {
                RegularExercise re = new RegularExercise();
                re.ID = i;
                re.SubjectID = 0;
                re.SolutionID = i;
                re.Title = "";
                re.Question = questions[i];
                Exercises.Add(re);
            }

            Solutions = new List<Solution> {
                new MultipleChoiceSolution(
                    Exercises[0].ID,
                    new List<string>() {
                        "first choice",
                        "second choice",
                        "third choice"
                    },
                    new List<bool>() {
                        false, true, false
                    },
                    Exercises[0]
                ),
                new TextSolution(
                    Exercises[1].ID,
                    "10 14 28 31 36 43 90 60 95 99\n\n" + "Here is the array after each exchange:\n\t10 14 31 36 43 95 28 90 60 99\n1: 10 14 31 36 43 28 95 90 60 99\n2: 10 14 31 36 28 43 95 90 60 99\n3: 10 14 31 28 36 43 95 90 60 99\n4: 10 14 28 31 36 43 95 90 60 99\n5: 10 14 28 31 36 43 90 95 60 99\n6: 10 14 28 31 36 43 90 60 95 99",
                    Exercises[1]
                ),
                new TextSolution(
                    Exercises[2].ID,
                    "11 31 46\n\n" + "The shape of a BST is uniquely determined by its level-order traversal. To deduce which links are red, recall that the length of every path from the root to a null link has the same number of black links; apply this property starting from nodes at the bottom.",
                    Exercises[2]
                ),
                new TextSolution(
                    Exercises[3].ID,
                    "75 44 93 28 57 92 97 24 32 49 61 77 30\n\n" + "Here is the level-order traversal of the red-black BST after each insertion:\n\t44 28 93 24 32 75 97 57 77 49 ( red links = 49 75 )\n30: 44 28 93 24 32 75 97 30 57 77 49 ( red links = 30 49 75 )\n92: 44 28 93 24 32 75 97 30 57 92 49 77 ( red links = 30 49 75 77 )\n61: 75 44 93 28 57 92 97 24 32 49 61 77 30 ( red links = 30 44 77 )",
                    Exercises[3]
                ),
                new MultipleChoiceSolution(
                    Exercises[4].ID,
                    new List<string>() {
                        "another first choice",
                        "another second choice",
                        "another third choice"
                    },
                    new List<bool>() {
                        true, false, true
                    },
                    Exercises[4]
                ),
                new TextSolution(
                    Exercises[5].ID,
                    "55 36 25 14\n\n" + "Here is the array to be searched after each compare:\n\t14 25 26 36 38 47 53 55 59 67 78 84 89 90 97\n55:  14 25 26 36 38 47 53  -  -  -  -  -  -  -  -\n36:  14 25 26  -  -  -  -  -  -  -  -  -  -  -  -\n25:  14  -  -  -  -  -  -  -  -  -  -  -  -  -  -\n14:   -  -  -  -  -  -  -  -  -  -  -  -  -  -  -",
                    Exercises[5]
                )
            };

            for (int i = 0; i < 6; i++) Exercises[i].Solution = Solutions[i];*/

            CurrentQuestionNumber = 0;
            CurrentExercise = Exercises[0];
            CurrentSolution = Solutions[0];
            UserChoices = Exercises[0].Solution.Choices;

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

        /*public async void LoadCollectionsFromDatabase()
        {
            AllSampleItems = await _sampleDB.Items.ToCollectionAsync();
        }*/

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

        public void LoadCoursePage()
        {
            CourseOptions = new List<Option>()
            {
                new Option(OptionType.Start, false, "Start", new Uri("/Assets/StartIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.AddToMyCourses, false, "Add to my courses", new Uri("/Assets/TickIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Download, true, "Download", new Uri("/Assets/DownloadsIcon.png", UriKind.RelativeOrAbsolute))
            };
        }

        public async System.Threading.Tasks.Task LoadAllCourses()
        {
            AllCourses = await db.Subjects.ToListAsync();
        }

        public void LoadDownloadedCourses()
        {
            DownloadedCourses = new List<Subject>()
            {
                new Subject("Databases")
            };
        }

        public void LoadMyCourses()
        {
            MyCourses = new List<Subject>()
            {
                new Subject("Algorithms I"),
                new Subject("Algorithms II"),
                new Subject("Databases")
            };
        }

        public void LoadNewCourses()
        {
            NewCourses = new List<Subject>();
        }

        public void SetProgressIndicator(System.Windows.DependencyObject depObject, string text)
        {
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = false,
                IsIndeterminate = false,

                Text = text
            };

            SystemTray.SetProgressIndicator(depObject, progressIndicator);
        }

        public void ActivateProgressForTimeConsumingProcess(System.Windows.DependencyObject depObject)
        {
            SystemTray.GetProgressIndicator(depObject).IsVisible = true;
            SystemTray.GetProgressIndicator(depObject).IsIndeterminate = true;
        }

        public void DeactivateProgressForTimeConsumingProcess(System.Windows.DependencyObject depObject)
        {
            SystemTray.GetProgressIndicator(depObject).IsVisible = false;
            SystemTray.GetProgressIndicator(depObject).IsIndeterminate = false;
        }

        public async System.Threading.Tasks.Task PerformTimeConsumingProcess(
            System.Windows.DependencyObject depObject, string actionDescr, 
            Func<System.Threading.Tasks.Task> Method)
        {
            SetProgressIndicator(depObject, actionDescr);
            ActivateProgressForTimeConsumingProcess(depObject);
            await Method();
            DeactivateProgressForTimeConsumingProcess(depObject);
            System.Diagnostics.Debug.WriteLine("Finished job!!!");

        }

        /*public async void AddSampleItem(SampleItem newSampleItem)
        {
            await _sampleDB.Items.InsertAsync(newSampleItem);
            AllSampleItems.Add(newSampleItem);

        }

        public async void DeleteSampleitem(SampleItem removedSampleitem)
        {
            await _sampleDB.Items.DeleteAsync(removedSampleitem);
            AllSampleItems.Remove(removedSampleitem);
        }*/

        
    }
}