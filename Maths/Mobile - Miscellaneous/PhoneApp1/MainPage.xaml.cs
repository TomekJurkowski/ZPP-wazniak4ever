using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            //var contentControl = ContentControl;
            //contentControl.Content = new List<string> {"one", "two", "three"};
            FillRichTextBox(@"This is a text $[img:1]$ containing a couple of images $[img:2]$." + 
                "This text continues seamlessly here, and there goes another image: $[img:3]$" + 
                "Another picture: $[img:4]$ and another one: $[img:5]$");

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }



        private async void FillRichTextBox(string text)
        {
            var start = 0;
            var regex = new Regex("\\$\\[img:[0-9]+\\]\\$");
            var paragraph = new Paragraph();
            foreach (Match match in regex.Matches(text))
            {
                var imageId = Int32.Parse(Regex.Match(match.Value, @"[0-9]+").Value);
                System.Diagnostics.Debug.WriteLine(imageId);
                paragraph.AddText(start, match.Index, text);
                await paragraph.ReplaceLabelWithImage(imageId);
                start = match.Index + match.Length;
            }
            paragraph.AddText(start, text.Length, text);
            //App.ViewModel.Blocks.Add(paragraph);
            RichTextBox.Blocks.Add(paragraph);
        }

        private void FillRichTextBox()
        {
            var myRun = new Run {Text = "Displaying text with inline image"};
            var runTwo = new Run {Text = "Another run"};
            var myImage = new Image
            {
                Source = new BitmapImage(new Uri("/Assets/CodeCogsEqn.png", UriKind.RelativeOrAbsolute)),
                Height = 50,
                Width = 50
            };
            var imageTwo = new Image
            {
                Source = new BitmapImage(new Uri("/Assets/CodeCogsEqn.png", UriKind.RelativeOrAbsolute))
            };
            
            var can = new Canvas();
            var bor = new Border();
            bor.Child = myImage;
            can.Children.Add(bor);
            can.LayoutUpdated += (s, e) =>
            {
                can.Width = bor.Child.DesiredSize.Width;
                can.Height = bor.Child.DesiredSize.Height/1.3d;
            };
            var can1 = new Canvas();
            var bor1 = new Border();
            bor1.Child = imageTwo;
            can1.Children.Add(bor1);
            can1.LayoutUpdated += (s, e) =>
            {
                can1.Width = bor.Child.DesiredSize.Width;
                can1.Height = bor.Child.DesiredSize.Height / 1.3d;
            };
            var myUi = new InlineUIContainer { Child = can };
            var uiTwo = new InlineUIContainer {Child = can1};

            // Create a paragraph and add the paragraph to the RichTextBox.
            var myParagraph = new Paragraph();
            RichTextBox.Blocks.Add(myParagraph);

            // Add the Run and image to it.
            myParagraph.Inlines.Add(myRun);
            myParagraph.Inlines.Add(myUi);
            myParagraph.Inlines.Add(runTwo);
            myParagraph.Inlines.Add(uiTwo);
        }

        /*private async void WebBrowser_Loaded(object sender, RoutedEventArgs e)
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
        }*/

        private void NavigationBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainMenu.xaml", UriKind.RelativeOrAbsolute));
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}

public static class RichTextBoxExtensions
{
    public static void AddText(this Paragraph paragraph, int start, int end, string text)
    {
        var run = new Run
        {
            Text = text.Substring(start, end - start), 
            Foreground = new SolidColorBrush(Colors.Black),
        };
        paragraph.Inlines.Add(run);
    }

    private static async Task<byte[]> LoadEquationImage(string exp, bool isInline)
    {
        var requestUri = isInline
            ? "http://latex.codecogs.com/png.download?%5Cinline%20" + exp
            : "http://latex.codecogs.com/png.download?%20" + exp;
        using (var client = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var responseMessage = await client.SendAsync(requestMessage);
            var a = await responseMessage.Content.ReadAsByteArrayAsync();

            return await responseMessage.Content.ReadAsByteArrayAsync();
        }
    }

    private static BitmapImage LoadImage(byte[] imageBytes)
    {
        using (var ms = new MemoryStream(imageBytes))
        {
            var image = new BitmapImage();
            image.SetSource(ms);
            return image;
        }
    }

    public async static Task ReplaceLabelWithImage(this Paragraph paragraph, int id)
    {
        var imageData = await LoadEquationImage("%5Csum", true);
        var image = new Image
        {
            Source = LoadImage(imageData),
            //Source = new BitmapImage(new Uri("/Assets/CodeCogsEqnInline.png", UriKind.RelativeOrAbsolute)),
            Height = 30
        };
        
        var uiImage = new InlineUIContainer { Child = image };
        paragraph.Inlines.Add(uiImage);
    }
}