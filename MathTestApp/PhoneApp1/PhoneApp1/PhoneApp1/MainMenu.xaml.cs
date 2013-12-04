using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;

namespace PhoneApp1
{
    public partial class MainMenu : PhoneApplicationPage
    {
        public ObservableCollection<Option> AllOptions { get; private set; }
        public MainMenu()
        {
            InitializeComponent();
            DataContext = this;

            LoadOptions();
        }

        private void LoadOptions()
        {
            AllOptions = new ObservableCollection<Option>()
            {
                new Option("Start Course", "Uri"),
                new Option("Settings", "Uri")
            };
        }
    }
}