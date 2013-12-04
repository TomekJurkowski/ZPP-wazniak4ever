using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Windows.Storage;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private async void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFile sFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\Math\web.html");

            var fileStream = await sFile.OpenStreamForReadAsync();
            string webString;
            string newString = @"NEW CONTENT: $$\int_{i=0}^{\pi} xdx$$";
            using (var streamReader = new StreamReader(fileStream))
            {
                webString = streamReader.ReadToEnd();
            }
            webString = webString.Insert(webString.IndexOf("<body>") + "<body>".Length, newString);
            System.Diagnostics.Debug.WriteLine(webString);
            WebBrowser.NavigateToString(webString);
        }

        private void NavigationBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainMenu.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}