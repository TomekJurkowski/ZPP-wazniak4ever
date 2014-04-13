using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using wazniak_forever.Model;
using Microsoft.Phone.Shell;
using System.Windows;
using Microsoft.Phone.Controls;
using Coding4Fun.Toolkit.Controls;

namespace wazniak_forever.ViewModel
{
    public class ClarifierViewModel : INotifyPropertyChanged
    {
        public DatabaseContext db { get; set; }

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

        private CourseType _courseType;
        public CourseType CourseType
        {
            get { return _courseType; }
            set
            {
                _courseType = value;
                NotifyPropertyChanged("CourseType");
            }
        }

        private List<AuthenticationProvider> _authenticationProviders;

        public List<AuthenticationProvider> AuthenticationProviders
        {
            get { return _authenticationProviders; }
            set
            {
                _authenticationProviders = value;
                NotifyPropertyChanged("AuthenticationProviders");
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

        private int getWeight(double ratio, System.DateTime lastAttempt) 
        {
            TimeSpan t = System.DateTime.Now - lastAttempt;
            double lastAttemptWeight = (double)t.TotalDays + 1;
            return (int)((1 - ratio) * 10 + lastAttemptWeight);
        }

        private int compareExerciseData(int correctAnswers1, int attempts1, System.DateTime lastAttempt1, int correctAnswers2, int attempts2, System.DateTime lastAttempt2, int attemptFactor)
        {
            int weight1 = getWeight(correctAnswers1 / attempts1, lastAttempt1);
            int weight2 = getWeight(correctAnswers2 / attempts2, lastAttempt2);
            int result = weight1 - (attempts1 * attemptFactor)- weight2 + (attempts2 * attemptFactor);
            return result;
        }

        private int compareSubjects(Subject s1, Subject s2)
        {
            UserSubject uS1 = _userSubjectMappings.Find(mapping =>
                mapping.SubjectID == s1.ID && mapping.UserID == db.User.UserId);
            UserSubject uS2 = _userSubjectMappings.Find(mapping =>
                mapping.SubjectID == s2.ID && mapping.UserID == db.User.UserId);
            if (uS1.Attempts == 0 && uS2.Attempts == 0) return 0;
            else if (uS1.Attempts == 0) return -1;
            else if (uS2.Attempts == 0) return 1;
            return -compareExerciseData(uS1.CorrectAnswers, uS1.Attempts, uS1.LastAttempt, uS2.CorrectAnswers, uS2.Attempts, uS2.LastAttempt, 0);
        }

        private int compareExercises(Exercise ex1, Exercise ex2)
        {
            UserExercise uE1 = _userExerciseMappings.Find(mapping =>
                mapping.ExerciseId == ex1.ID && mapping.UserId == db.User.UserId);
            UserExercise uE2 = _userExerciseMappings.Find(mapping =>
                mapping.ExerciseId == ex2.ID && mapping.UserId == db.User.UserId);
            if (uE1 == null && uE2 == null) return 0;
            if (uE1 == null) return -1;
            if (uE2 == null) return 1;
            return -compareExerciseData(uE1.CorrectAnswers, uE1.Attempts, uE1.LastAttempt, uE2.CorrectAnswers, uE2.Attempts, uE2.LastAttempt, 2);
        }

        private List<Solution> matchSolutions()
        {
            List<Solution> result = new List<Solution>();
            foreach (Exercise ex in Exercises)
            {
                result.Add(ex.Solution);
            }
            return result;
        }

        public void sortExercisesByProgress()
        {
            Exercises.Sort(compareExercises);
            Solutions = matchSolutions();
            CurrentExercise = Exercises[0];
            CurrentSolution = Solutions[0];
            UserChoices = Exercises[0].Solution.Choices;
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

        private List<string> _modules;
        public List<string> Modules
        {
            get { return _modules; }
            set
            {
                _modules = value;
                NotifyPropertyChanged("Modules");
            }
        }

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

        public void LoadModules()
        {
            Modules = new List<string>();
            Modules.Add("Module 1");
            Modules.Add("Module 2");
            Modules.Add("Module 3");
            Modules.Add("Module 4");
            Modules.Add("Module 5");
            Modules.Add("Module 6");
            Modules.Add("Module 7");
            Modules.Add("Module 8");
            Modules.Add("Module 9");
            Modules.Add("Module 10");
        }

        public async System.Threading.Tasks.Task LoadExercises()
        {
            var tasksWithAnswers = OnlineMode ? 
                await db.TasksWithAnswers.Where(task => task.SubjectID == CurrentCourseID).LoadAllAync() :
                await db.LoadExercisesOffline(CurrentCourseID);

            var multipleChoiceExerciseOptions = OnlineMode ?
                await db.MultipleChoiceOptions.Where(option => option.SubjectID == CurrentCourseID).LoadAllAync() :
                await db.LoadExerciseChoicesOffline<MultipleChoiceExerciseOption>(CurrentCourseID);

            var singleChoiceExerciseOptions = OnlineMode ?
                await db.SingleChoiceOptions.Where(option => option.SubjectID == CurrentCourseID).IncludeTotalCount().LoadAllAync() :
                await db.LoadExerciseChoicesOffline<SingleChoiceExerciseOption>(CurrentCourseID);

            Exercises = new List<RegularExercise>();
            Solutions = new List<Solution>();


            foreach (var task in tasksWithAnswers)
            {
                
                if (task.AnswerDiscriminator == "TextAnswer" && CourseType != CourseType.Classic) continue;

                Solution solution = null;

                switch (task.AnswerDiscriminator) 
                {
                    case "SingleValueAnswer":
                        solution = new SingleValueSolution(task.TaskID, task.Value, null);
                        break;
                    case "TextAnswer":
                        solution = new TextSolution(task.TaskID, task.AnswerText, null);
                        break;
                    case "MultipleChoiceAnswer":
                        var choices = new List<string>();
                        var answers = new List<bool>();
                        multipleChoiceExerciseOptions.FindAll(option => option.TaskID == task.TaskID)
                            .ForEach(option => 
                            {
                                choices.Add(option.ChoiceString);
                                answers.Add(option.ChoiceBool);
                            });
                        solution = new MultipleChoiceSolution(task.TaskID, choices, answers, null);
                        break;
                    case "SingleChoiceAnswer":
                        var sChoices = new List<string>();
                        singleChoiceExerciseOptions.FindAll(option => option.TaskID == task.TaskID)
                            .ForEach(option => sChoices.Add(option.ChoiceString));
                        if (sChoices.Count > 0)
                        {
                            var answer = sChoices[task.CorrectAnswer];
                            solution = new SingleChoiceSolution(task.TaskID, sChoices, answer, null);
                        }
                        break;
                }
                if (solution != null)
                {
                    Solutions.Add(solution);

                    switch (task.TaskDiscriminator)
                    {
                        case "RegularTask":
                            var subject = OnlineMode ?
                                AllCourses.First(course => course.ID == CurrentCourseID) :
                                DownloadedCourses.First(course => course.ID == CurrentCourseID);
                            var ex = new RegularExercise(task.ID, CurrentCourseID, task.TaskID,
                                task.Title, task.Text1,
                                subject,
                                solution);
                            solution.Exercise = ex;
                            Exercises.Add(ex);
                            break;
                    }
                }  
            }

            if (Exercises.Count == 0)
            {
                MessageBox.Show("Unfortunately, there are no exercises for this course yet!");
                return;
            }

            if (CourseType == CourseType.FixedNumber)
            {
                var RandomExercises = new List<RegularExercise>();
                var RandomSolutions = new List<Solution>();
                var r = new Random();
                while (RandomExercises.Count < 10 && Exercises.Count > 0)
                {
                    var index = r.Next(0, Exercises.Count);
                    RandomExercises.Add(Exercises.ElementAt(index));
                    RandomSolutions.Add(Solutions.ElementAt(index));
                    Exercises.RemoveAt(index);
                    Solutions.RemoveAt(index);
                }
                Exercises = RandomExercises;
                Solutions = RandomSolutions;
            }

            CurrentQuestionNumber = 0;
            CurrentExercise = Exercises[0];
            CurrentSolution = Solutions[0];
            UserChoices = Exercises[0].Solution.Choices;

        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        public void NotifyPropertyChanged(string propertyName)
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
            //OnlineMode = DeviceNetworkInformation.IsNetworkAvailable;
            OnlineMode = NetworkInterface.GetIsNetworkAvailable();
            LoadMenu();
            LoadCourseOptions();
        }

        public void LoadMenu()
        {
            AllOptions = new List<Option>()
            {
                new Option(OptionType.MyCourses, true, "Courses", new Uri("/Assets/StartIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Downloads, false, "Downloads", new Uri("/Assets/DownloadsIcon.png", UriKind.RelativeOrAbsolute)),
                //new Option(OptionType.Settings, false, "Settings", new Uri("/Assets/SettingsIcon.png", UriKind.RelativeOrAbsolute))
            };

            if (db.User != null)
            {
                AllOptions.Add(new Option(OptionType.Logout, true, "Sign out", new Uri("/Assets/SignOutIcon.png", UriKind.RelativeOrAbsolute)));
            } else 
            {
                AllOptions.Add(new Option(OptionType.Login, true, "Sign in", new Uri("/Assets/SignInIcon.png", UriKind.RelativeOrAbsolute)));
            }
        }

        #region Authentication

        public void LoadAuthenticationProviders()
        {
            AuthenticationProviders = new List<AuthenticationProvider>()
            {
                new AuthenticationProvider(AuthenticationProviderType.Microsoft, "Microsoft"),
                new AuthenticationProvider(AuthenticationProviderType.Google, "Google"),
                new AuthenticationProvider(AuthenticationProviderType.Facebook, "Facebook"),
                new AuthenticationProvider(AuthenticationProviderType.Twitter, "Twitter")
            };
        }

        public async System.Threading.Tasks.Task Authenticate(MobileServiceAuthenticationProvider provider)
        {
            var success = false;
            var messageBox = new CustomMessageBox()
            {
                Caption = "Authentication error",
                Message = "We could not complete logging on to your account. Would you like to try again?",
                LeftButtonContent = "yes",
                RightButtonContent = "no",
                IsFullScreen = false
            };

            messageBox.Dismissed += (s, e) =>
                {
                    switch (e.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            break;
                        case CustomMessageBoxResult.RightButton:
                            success = true;
                            break;
                        default:
                            break;
                    }
                };

            while (!success)
            {
                if (db.User == null)
                {
                    string message;
                    try
                    {
                        db.User = await DatabaseContext.MobileService
                            .LoginAsync(provider);
                        success = true;
                        message =
                            string.Format("You are now logged in as {0}", db.User.UserId);
                        MessageBox.Show(message);
                    }
                    catch (InvalidOperationException)
                    {
                        messageBox.Show();
                    }
                    
                }
            }
            
        }

        public void Logout() {
            try
            {
                if (DatabaseContext.MobileService.CurrentUser != null 
                    && DatabaseContext.MobileService.CurrentUser.UserId != null)
                     DatabaseContext.MobileService.Logout();
                db.User = null;
            }
            catch (InvalidOperationException iopEx)
            {
                MessageBox.Show(string.Format("Error Occured: \n {0}", iopEx.ToString()));
            }
        }
    

        #endregion

        private void CheckCourseOwnership()
        {
            if (db.User == null || MyCourses == null) { return; }
            
            if (MyCourses.Any(course => course.ID == CurrentCourseID))
            {
                CourseOptions.Add(new Option(OptionType.DeleteFromMyCourses, false, "Delete from My Courses", new Uri("/Assets/DeleteIcon.png", UriKind.RelativeOrAbsolute)));
            }
            else
            {
                CourseOptions.Add(new Option(OptionType.AddToMyCourses, false, "Add to My Courses", new Uri("/Assets/TickIcon.png", UriKind.RelativeOrAbsolute)));
            }
            
        }

        private void LoadDownloadedCourseOptions()
        {
            if (DownloadedCourses == null) return;

            if (DownloadedCourses.Any(course => course.ID == CurrentCourseID))
            {
                CourseOptions.RemoveAll(option => option.Type == OptionType.Download);
                CourseOptions.Add(new Option(OptionType.Update, true, "Update", new Uri("/Assets/DownloadsIcon.png", UriKind.RelativeOrAbsolute)));
                CourseOptions.Add(new Option(OptionType.DeleteFromDownloads, false, "Delete from Downloads", new Uri("/Assets/DeleteIcon.png", UriKind.RelativeOrAbsolute)));
            }
        }

        private void LoadLoggedInOptions()
        {
            if (db.User != null)
            {
                CourseOptions.Insert(1, new Option(OptionType.StudyWithClarifier, false, "Study with Clarifier", new Uri("/Assets/IdeaIcon.png", UriKind.RelativeOrAbsolute)));
            }
        }
        
        private void LoadCourseOptions()
        {
            CourseOptions = new List<Option>()
            {
                new Option(OptionType.Start, false, "Practice", new Uri("/Assets/StartIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.FixedNumber, false, "10Q Challenge", new Uri("/Assets/Quick.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Timer, false, "Time Challenge", new Uri("/Assets/Timer.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Download, true, "Download", new Uri("/Assets/DownloadsIcon.png", UriKind.RelativeOrAbsolute))
            };
            
            LoadDownloadedCourseOptions();
            CheckCourseOwnership();
            LoadLoggedInOptions();
        }

        public async void LoadCoursePage()
        {
            await LoadDownloadedCourses();
            LoadCourseOptions();
        }

        /*
        public void LoadDownloadedCoursePage()
        {
            CourseOptions = new List<Option>()
            {
                new Option(OptionType.Start, false, "Start", new Uri("/Assets/StartIcon.png", UriKind.RelativeOrAbsolute)),
                new Option(OptionType.Update, true, "Update", new Uri("/Assets/DownloadsIcon.png", UriKind.RelativeOrAbsolute))
            };
            CheckCourseOwnership();
        }
        */

        public async System.Threading.Tasks.Task LoadAllCourses()
        {
            AllCourses = await db.Subjects.ToListAsync();
        }

        public async System.Threading.Tasks.Task LoadDownloadedCourses()
        {
            DownloadedCourses = await db.LoadSubjectsOffline();
            /*DownloadedCourses = new List<Subject>()
            {
                new Subject("Databases")
            };*/
        }

        private List<UserSubject> _userSubjectMappings;

        private List<UserExercise> _userExerciseMappings;

        public async System.Threading.Tasks.Task LoadMyCourses()
        {
            var mySubjects = await db.MySubjects
                .Where(user => user.UserID == DatabaseContext.MobileService.CurrentUser.UserId)
                .ToListAsync();

            MyCourses = new List<Subject>();

            _userSubjectMappings = new List<UserSubject>();
            mySubjects.ForEach(subject =>
            {
                MyCourses.Add(new Subject(subject.ID, subject.Name, 
                    subject.Description, subject.LastUpdated));
                _userSubjectMappings.Add(new UserSubject(subject.MappingID, subject.UserID, 
                    subject.ID, subject.CorrectAnswers, subject.Attempts, subject.LastAttempt));
            });

            MyCourses.Sort(compareSubjects);

            _userExerciseMappings = new List<UserExercise>();
            var testIfNull = (await db.UsersAndExercises.Where(ue => ue.UserId == db.User.UserId).ToListAsync()).FirstOrDefault();
            if (testIfNull != null)
            {
                _userExerciseMappings = await db.UsersAndExercises.Where(ue => ue.UserId == db.User.UserId).ToListAsync();
            }
        }

        private List<KeyValuePair<int, bool>> _givenAnswers = new List<KeyValuePair<int, bool>>();

        public List<KeyValuePair<int, bool>> GivenAnswers
        {
            get { return _givenAnswers; }
            set { _givenAnswers = value; }
        } 

        public void AddAnswer(int exerciseId, bool correctAnswer)
        {
            _givenAnswers.Add(new KeyValuePair<int, bool>(exerciseId, correctAnswer));
        }


        public async System.Threading.Tasks.Task SendMyResults(int correctAnswers, int attempts)
        {
            var currentUserSubjectMapping = _userSubjectMappings.Find(mapping =>
                mapping.SubjectID == CurrentCourseID
                && mapping.UserID == db.User.UserId);

            currentUserSubjectMapping.CorrectAnswers += correctAnswers;
            currentUserSubjectMapping.Attempts += attempts;
            currentUserSubjectMapping.LastAttempt = System.DateTime.Now;

            await db.UsersAndSubjects.UpdateAsync(currentUserSubjectMapping);
            foreach (KeyValuePair<int, bool> t in _givenAnswers)
            {
                var userExerciseMap = (await db.UsersAndExercises
                    .Where(ue => ue.UserId == db.User.UserId && ue.ExerciseId == t.Key).ToListAsync()).FirstOrDefault();
                if (userExerciseMap == null)
                {
                    UserExercise newUE = new UserExercise(db.User.UserId, t.Key, 1, t.Value ? 1 : 0, System.DateTime.Now);
                    await db.UsersAndExercises.InsertAsync(newUE);
                }
                else
                {
                    userExerciseMap.Attempts++;
                    if (t.Value) userExerciseMap.CorrectAnswers++;
                    userExerciseMap.LastAttempt = System.DateTime.Now;
                    await db.UsersAndExercises.UpdateAsync(userExerciseMap);
                }
            }
        }

        public async System.Threading.Tasks.Task AddToMyCourses()
        {
            await db.UsersAndSubjects.InsertAsync(
                new UserSubject
                {
                    UserID = DatabaseContext.MobileService.CurrentUser.UserId,
                    SubjectID = CurrentCourseID,
                    CorrectAnswers = 0,
                    Attempts = 0
                });
            MyCourses.Add(AllCourses.Find(course => course.ID == CurrentCourseID));
            LoadCoursePage();
        }

        public async System.Threading.Tasks.Task DeleteFromMyCourses()
        {
            var deletedMapping = _userSubjectMappings.Find(mapping =>
                mapping.SubjectID == CurrentCourseID
                && mapping.UserID == db.User.UserId);

            await db.UsersAndSubjects.DeleteAsync(deletedMapping);

            _userSubjectMappings.Remove(deletedMapping);
            MyCourses.Remove(MyCourses.Find(course => course.ID == CurrentCourseID));

            // BELOW: DO NOT DELETE YET
            var lol = _userExerciseMappings.FindAll(mapping => mapping.UserId == db.User.UserId);
            foreach (UserExercise uE in lol) await db.UsersAndExercises.DeleteAsync(uE);

            LoadCoursePage();  
        }

        public async System.Threading.Tasks.Task LoadNewCourses()
        {
            var subjects = await db.Subjects.ToListAsync();
            NewCourses = subjects.OrderByDescending(x => x.LastUpdated).Take(10).ToList();
        }

        public void SetProgressIndicator(System.Windows.DependencyObject depObject, string text)
        {
            var progressIndicator = new ProgressIndicator()
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
            Func<System.Threading.Tasks.Task> method)
        {
            SetProgressIndicator(depObject, actionDescr);
            ActivateProgressForTimeConsumingProcess(depObject);
            var success = false;
            var count = 0;
            while (!success && count < 5)
            {
                try
                {
                    await method();
                    success = true;
                }
                catch (MobileServiceInvalidOperationException e) 
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    count++; 
                }
            }
            if (count == 5) MessageBox.Show("Clarifier cannot complete this operation!");
            DeactivateProgressForTimeConsumingProcess(depObject);
        }

        public void ShowToast(string message)
        {
            var prompt = new ToastPrompt
            {
                Title = "Clarifier",
                Message = message,
                
            };
            prompt.Show();
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

        public DTimer Timer { get; set; }

        private int currentTime;

        public int CurrentTime
        {
            get { return currentTime; }
            set
            {
                currentTime = value;
                NotifyPropertyChanged("CurrentTime");
            }
        }

        public void TimerModeTickHandler(int time)
        {
            CurrentTime = time;
            if (time == 0)
            {
                MessageBox.Show("Time's up!");
                HandleTimesUp();
            }
                
        }

        public event Action HandleTimesUp;   

        public bool HandleCourseExit(CancelEventArgs e)
        {
            if (CourseType == CourseType.Time)
                Timer.Stop();
            var quit = true;

            var result = MessageBox.Show("Do you want to leave the course?", "", MessageBoxButton.OKCancel);
            switch (result)
            {
                case MessageBoxResult.OK:
                    e.Cancel = false;
                    break;
                default:
                    e.Cancel = true;
                    if (CourseType == CourseType.Time)
                        Timer.Resume();
                    quit = false;
                    break;
            }
            return quit;
        }
    }

    public static class AzureLoadingExtensions
    {
        //Azure Mobile Services allow for taking 50 records at a time by default and 1000 records at maximum
        public async static Task<List<T>> LoadAllAync<T>(this IMobileServiceTableQuery<T> table, int bufferSize = 1000)
        {
            var query = table.IncludeTotalCount();
            var results = await query.ToEnumerableAsync();
            if (results == null) return null;
            var count = ((ITotalCountProvider) results).TotalCount;
            if (count <= 0) return new List<T>();
            var updates = new List<T>();
            while (updates.Count < count)
            {
                var next = await query.Skip(updates.Count).Take(bufferSize).ToListAsync();
                updates.AddRange(next);
            }
            return updates;
        }
        
    }
}
