using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
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

        private int _gridCounter;

        public int GridCounter
        { 
            get { return _gridCounter; }
            set
            {
                _gridCounter = value;
                NotifyPropertyChanged("GridCounter");
            }
        }

        private double _breakingPoint;

        public double BreakingPoint
        {
            get { return _breakingPoint; }
            set
            {
                _breakingPoint = value;
                NotifyPropertyChanged("BreakingPoint");
            }
        }

        public bool isSorted { get; set; }

        public int CompareSubjects(Subject s1, Subject s2)
        {
            UserSubject uS1 = _userSubjectMappings.Find(mapping =>
                mapping.SubjectID == s1.ID && mapping.UserID == db.User.UserId);
            UserSubject uS2 = _userSubjectMappings.Find(mapping =>
                mapping.SubjectID == s2.ID && mapping.UserID == db.User.UserId);
            return uS1.CurrentModuleIndex - uS2.CurrentModuleIndex;
        }

        public void CalculateBreakingPoint()
        {
            int id = MyCourses[MyCourses.Count - GridCounter - 1].ID;
            double divisor = (double)Modules.FindAll(module => module.SubjectID == id).Count;
            if (divisor > 0.0) BreakingPoint = (double)_userSubjectMappings.Find(subject => subject.SubjectID == id).CurrentModuleIndex / divisor;
            else BreakingPoint = 0.0;
            GridCounter++;
        }

        public void AddAnswer(bool ans)
        {
            if (CourseType == CourseType.StudyWithClarifier) ModulesAnswers[CurrentModule.SequenceNo].Add(ans);
        }

        private double countWrongAnswerRatio(int correctAnswers, int wrongAnswerSum)
        {
            if (wrongAnswerSum == 0) return 0.0;
            return ((double)UserModule.ATTEMPTS - (double)correctAnswers) / (double)wrongAnswerSum;
        }

        private int[] countWeights(int CurrentModuleIndex, int RepetitionBase, List<UserModule> UserModules)
        {
            int[] weights = new int[CurrentModuleIndex];
            int wrongAnswerSum = 0;
            for (int i = 0; i < CurrentModuleIndex; i++) wrongAnswerSum += UserModule.ATTEMPTS - UserModules[i].CountCorrectAnswers();
            for (int i = 0; i < CurrentModuleIndex; i++)
            {
                weights[i] = (int)Math.Round(RepetitionBase * countWrongAnswerRatio(UserModules[i].CountCorrectAnswers(), wrongAnswerSum));
            }
            return weights;
        }

        private List<Exercise> randomExercises(List<Exercise> moduleExercises, int Count)
        {
            List<Exercise> result = new List<Exercise>();
            Random r = new Random();
            for (int i = 0; i < Math.Min(Count, moduleExercises.Count); i++)
            {
                Exercise ex = moduleExercises[r.Next(moduleExercises.Count)];
                result.Add(ex);
                moduleExercises.Remove(ex);
            }
            return result;
        }

        private List<Exercise> chooseRepetitionExercises(int CurrentModuleIndex, int[] weights)
        {
            List<Exercise> result = new List<Exercise>();
            for (int i = 0; i < CurrentModuleIndex; i++)
            {
                List<Exercise> moduleExercises = Exercises.FindAll(ex => ex.ModuleID == SubjectModules[i].ID);
                result.Concat(randomExercises(moduleExercises, weights[i]));
            }
            return result;
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

        public void pickExercises()
        {
            if (SubjectModules == null)
            {
                MessageBox.Show("Unfortunately, there are no exercises for this course yet!");
                return;
            }

            CurrentModuleIndex = _userSubjectMappings.Find(subject => subject.SubjectID == CurrentCourseID).CurrentModuleIndex;
            CurrentModule = SubjectModules[CurrentModuleIndex];
            if (_userModuleMappings.FindAll(module => module.SubjectID == CurrentCourseID).Count <= CurrentModuleIndex)
            {
                UserModule uM = new UserModule(db.User.UserId, CurrentModule.ID, CurrentCourseID, CurrentModule.SequenceNo, 0, new List<bool>());
                _userModuleMappings.Add(uM);
                db.UserModules.InsertAsync(uM);
            }

            List<UserModule> UserModules = _userModuleMappings.FindAll(module => module.SubjectID == CurrentCourseID);
            int RepetitionBase = 2 * CurrentModuleIndex + 1 - Convert.ToInt32(CurrentModuleIndex == 0);
            int[] ModuleWeights = countWeights(CurrentModuleIndex, RepetitionBase, UserModules);
            int RandomExerciseCount = RepetitionBase - ModuleWeights.Sum();

            List<Exercise> ExercisesBackup = randomExercises(Exercises.FindAll(ex => ex.ModuleID == CurrentModule.ID), UserModule.ATTEMPTS);
            ExercisesBackup.Concat(chooseRepetitionExercises(CurrentModuleIndex, ModuleWeights));
            if (RandomExerciseCount > 0) ExercisesBackup.Concat(randomExercises(Exercises.FindAll(ex => ex.ModuleID < CurrentModule.ID), RandomExerciseCount));

            Exercises = ExercisesBackup;
            if (Exercises.Count == 0)
            {
                MessageBox.Show("Unfortunately, there are no exercises for this module yet!");
                return;
            }
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

        private int _currentModuleIndex;
        public int CurrentModuleIndex
        {
            get { return _currentModuleIndex; }
            set
            {
                _currentModuleIndex = value;
                NotifyPropertyChanged("CurrentModuleIndex");
            }
        }

        private Module _currentModule;
        public Module CurrentModule
        {
            get { return _currentModule; }
            set
            {
                _currentModule = value;
                NotifyPropertyChanged("CurrentModule");
            }
        }

        private List<List<bool>> _modulesAnswers;
        public List<List<bool>> ModulesAnswers
        {
            get { return _modulesAnswers; }
            set
            {
                _modulesAnswers = value;
                NotifyPropertyChanged("ModulesCorrect");
            }
        }

        private Exercise _currentExercise;
        public Exercise CurrentExercise
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

        private List<Module> _subjectModules;
        public List<Module> SubjectModules
        {
            get { return _subjectModules; }
            set
            {
                _subjectModules = value;
                NotifyPropertyChanged("SubjectModules");
            }
        }

        private List<Module> _modules;
        public List<Module> Modules
        {
            get { return _modules; }
            set
            {
                _modules = value;
                NotifyPropertyChanged("Modules");
            }
        }

        private List<Exercise> _exercises;
        public List<Exercise> Exercises
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

        public async System.Threading.Tasks.Task LoadModules()
        {
            Modules = await db.Modules.ToListAsync();
        }

        public async System.Threading.Tasks.Task LoadSubjectModules()
        {
            SubjectModules = OnlineMode
                ? await db.Modules.Where(module => module.SubjectID == CurrentCourseID).ToListAsync()
                : await db.LoadModulesOffline(CurrentCourseID);

            ModulesAnswers = new List<List<bool>>();
            foreach (Module m in SubjectModules)
            {
                ModulesAnswers.Add(new List<bool>());
            }
        }

        public async System.Threading.Tasks.Task LoadExercises(IEnumerable<int> moduleIdList)
        {
            var tasksWithAnswers = OnlineMode
                ? await
                    db.TasksWithAnswers.Where(task => task.SubjectID == CurrentCourseID).LoadAllAync()
                : await db.LoadExercisesOffline(CurrentCourseID);
            if (moduleIdList.Count() > 0)
            {
                tasksWithAnswers = tasksWithAnswers.FindAll(task => (moduleIdList as List<int>).Exists(module => module == task.ModuleID));
            }

            var multipleChoiceExerciseOptions = OnlineMode ?
                await db.MultipleChoiceOptions.Where(option => option.SubjectID == CurrentCourseID).LoadAllAync() :
                await db.LoadExerciseChoicesOffline<MultipleChoiceExerciseOption>(CurrentCourseID);

            var singleChoiceExerciseOptions = OnlineMode ?
                await db.SingleChoiceOptions.Where(option => option.SubjectID == CurrentCourseID).IncludeTotalCount().LoadAllAync() :
                await db.LoadExerciseChoicesOffline<SingleChoiceExerciseOption>(CurrentCourseID);

            Exercises = new List<Exercise>();
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
                    var subject = OnlineMode ?
                           AllCourses.First(course => course.ID == CurrentCourseID) :
                           DownloadedCourses.First(course => course.ID == CurrentCourseID);
                    switch (task.TaskDiscriminator)
                    {
                       
                        default:
                            var regExer = new RegularExercise(task.ID, CurrentCourseID, task.ModuleID, task.TaskID,
                                task.Title, task.Text1, subject, solution, task.ModifiedText, task.ImageUrl, task.TaskDiscriminator);
                            solution.Exercise = regExer;
                            Exercises.Add(regExer);
                            break;
                    }
                }  
            }

            if (Exercises.Count == 0)
            {
                MessageBox.Show("Unfortunately, there are no exercises for this course yet!");
                return;
            }

            if (CourseType != CourseType.StudyWithClarifier)
            {
                var RandomExercises = new List<Exercise>();
                var RandomSolutions = new List<Solution>();
                var r = new Random();
                
                int maxExercises;
                if (CourseType == CourseType.FixedNumber) { maxExercises = 10; }
                else { maxExercises = Exercises.Count; }
                
                while (RandomExercises.Count < maxExercises && Exercises.Count > 0)
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


        public void CheckForNetworkAvailability()
        {
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
            /*messageBox.Dismissed += (s, e) =>
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
                };*/

            //while (db.User == null && !success)
            //{
            //    string message;
                try
                {
                    db.User = await DatabaseContext.MobileService
                                        .LoginAsync(provider);

                    
                    MessageBox.Show(string.Format("You are now logged in as {0}", db.User.UserId));
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("We could not complete singing in.", "Sign in", MessageBoxButton.OK);
                }

            //}
            
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
                CourseOptions.Insert(1, new Option(OptionType.StudyWithClarifier, false, "Study with Clarifier", new Uri("/Assets/IdeaIcon.png", UriKind.RelativeOrAbsolute)));
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
        }

        public async void LoadCoursePage()
        {
            await LoadDownloadedCourses();
            LoadCourseOptions();
        }

        public async System.Threading.Tasks.Task LoadAllCourses()
        {
            AllCourses = await db.Subjects.ToListAsync();
        }

        public async System.Threading.Tasks.Task LoadDownloadedCourses()
        {
            DownloadedCourses = await db.LoadSubjectsOffline();
        }

        private List<UserSubject> _userSubjectMappings;

        private List<UserModule> _userModuleMappings;

        private List<UserExercise> _userExerciseMappings;

        public async System.Threading.Tasks.Task LoadMyCourses()
        {
            var mySubjects = await db.MySubjects
                .Where(user => user.UserID == DatabaseContext.MobileService.CurrentUser.UserId)
                .ToListAsync();

            var myModules = await db.UserModules.
                Where(module => module.UserID == DatabaseContext.MobileService.CurrentUser.UserId)
                .ToListAsync();

            MyCourses = new List<Subject>();

            _userSubjectMappings = new List<UserSubject>();
            mySubjects.ForEach(subject =>
            {
                MyCourses.Add(new Subject(subject.ID, subject.Name,
                    subject.Description, subject.LastUpdated));
                _userSubjectMappings.Add(new UserSubject(subject.MappingID, subject.UserID,
                    subject.ID, subject.CurrentModuleIndex, subject.CorrectAnswers, subject.Attempts, subject.LastAttempt));
            });

            _userModuleMappings = new List<UserModule>();
            myModules.ForEach(module =>
            {
                UserModule uM = new UserModule(module.ID, module.UserID, module.ModuleID, module.SubjectID, module.SequenceNo, module.AnswersNumber, module.parseAnswersToList(UserModule.ATTEMPTS));
                _userModuleMappings.Add(uM);
            });

            _userExerciseMappings = new List<UserExercise>();
            var testIfNull = (await db.UsersAndExercises.Where(ue => ue.UserID == db.User.UserId).ToListAsync()).FirstOrDefault();
            if (testIfNull != null)
            {
                _userExerciseMappings = await db.UsersAndExercises.Where(ue => ue.UserID == db.User.UserId).ToListAsync();
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

        private void calculateModuleIndex(UserSubject currentSubject)
        {
            UserModule currentModule = _userModuleMappings.Find(module => module.ModuleID == SubjectModules[currentSubject.CurrentModuleIndex].ID);
            if (currentModule.CheckAnswers())
            {
                currentSubject.CurrentModuleIndex++;
            }
        }

        public async System.Threading.Tasks.Task SendMyResults(int subjectCorrectAnswers, int subjectAttempts)
        {

            // MODULES
            foreach (UserModule currentUserModuleMapping in _userModuleMappings.FindAll(mapping => mapping.UserID == db.User.UserId && mapping.SubjectID == CurrentCourseID))
            {
                currentUserModuleMapping.AddAnswers(ModulesAnswers[currentUserModuleMapping.SequenceNo]);
                await db.UserModules.UpdateAsync(currentUserModuleMapping);
            }

            // SUBJECTS
            var currentUserSubjectMapping = _userSubjectMappings.Find(mapping =>
                mapping.SubjectID == CurrentCourseID
                && mapping.UserID == db.User.UserId);

            currentUserSubjectMapping.CorrectAnswers += subjectCorrectAnswers;
            currentUserSubjectMapping.Attempts += subjectAttempts;
            currentUserSubjectMapping.LastAttempt = System.DateTime.Now;
            calculateModuleIndex(currentUserSubjectMapping);
            await db.UsersAndSubjects.UpdateAsync(currentUserSubjectMapping);

            // EXERCISES
            foreach (KeyValuePair<int, bool> t in _givenAnswers)
            {
                var userExerciseMap = (await db.UsersAndExercises
                    .Where(ue => ue.UserID == db.User.UserId && ue.ExerciseID == t.Key).ToListAsync()).FirstOrDefault();
                if (userExerciseMap == null)
                {
                    UserExercise newUE = new UserExercise(db.User.UserId, CurrentCourseID, t.Key, 1, t.Value ? 1 : 0, System.DateTime.Now);
                    await db.UsersAndExercises.InsertAsync(newUE);
                }
                else
                {
                    userExerciseMap.Attempts++;
                    if (t.Value) userExerciseMap.CorrectAnswers++;
                    userExerciseMap.LastAttempt = System.DateTime.Now;
                    await db.UsersAndExercises.UpdateAsync(userExerciseMap);
                }

                //UPDATE STATS
                var exerciseStats =
                    (await db.ExerciseStatistics.Where(stats => stats.TaskID == t.Key)
                    .ToListAsync()).FirstOrDefault();

                if (exerciseStats == null)
                {
                    await db.ExerciseStatistics.InsertAsync(new TaskStatistics
                    {
                        TaskID = t.Key,
                        CorrectAnswers = t.Value ? 1 : 0,
                        Attempts = 1
                    });
                }
                else
                {
                    exerciseStats.Attempts++;
                    if (t.Value) exerciseStats.CorrectAnswers++;
                    await db.ExerciseStatistics.UpdateAsync(exerciseStats);
                }

                var currTask =
                    (await db.TasksWithAnswers.Where(task => task.TaskID == t.Key).ToListAsync())
                    .FirstOrDefault();

                if (currTask == null) return;
                var taskModule = currTask.ModuleID;

                var moduleStats =
                    (await db.ModuleStatistics.Where(stats => stats.ModuleID == taskModule)
                    .ToListAsync()).FirstOrDefault();

                if (moduleStats == null)
                {
                    await db.ModuleStatistics.InsertAsync(new ModuleStatistics
                    {
                        ModuleID = taskModule,
                        CorrectAnswers = t.Value ? 1 : 0,
                        Attempts = 1
                    });
                }
                else
                {
                    moduleStats.Attempts++;
                    if (t.Value) moduleStats.CorrectAnswers++;
                    await db.ModuleStatistics.UpdateAsync(moduleStats);
                }
            }
        }

        public async System.Threading.Tasks.Task AddToMyCourses()
        {
            if (MyCourses.Find(subject => subject.ID == CurrentCourseID) != null) return;

            UserSubject uS = new UserSubject(db.User.UserId, CurrentCourseID, 0, 0, 0, DateTime.Now);

            if (_userSubjectMappings.Find(subject => subject.SubjectID == CurrentCourseID) == null)
            {
                _userSubjectMappings.Add(uS);
            }

            await db.UsersAndSubjects.InsertAsync(uS);
            MyCourses.Add(AllCourses.Find(course => course.ID == CurrentCourseID));
            MyCourses.Sort(CompareSubjects);
            LoadCoursePage();
        }

        public async System.Threading.Tasks.Task DeleteFromMyCourses()
        {
            var deletedMapping = _userSubjectMappings.Find(mapping =>
                mapping.SubjectID == CurrentCourseID
                && mapping.UserID == DatabaseContext.MobileService.CurrentUser.UserId);

            await db.UsersAndSubjects.DeleteAsync(deletedMapping);
            _userSubjectMappings.Remove(deletedMapping);
            MyCourses.Remove(MyCourses.Find(course => course.ID == CurrentCourseID));
            var exercisesToRemove = _userExerciseMappings.FindAll(mapping => mapping.UserID == db.User.UserId && mapping.SubjectID == CurrentCourseID);
            foreach (UserExercise uE in exercisesToRemove) await db.UsersAndExercises.DeleteAsync(uE);
            foreach (UserModule uM in _userModuleMappings.FindAll(module => module.UserID == db.User.UserId && module.SubjectID == CurrentCourseID)) await db.UserModules.DeleteAsync(uM);

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
