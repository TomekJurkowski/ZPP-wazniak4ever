using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Storage;
using System.IO;

namespace wazniak_forever
{
    public partial class Exercise : PhoneApplicationPage
    {
        public Exercise()
        {
            InitializeComponent();
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
    }
}