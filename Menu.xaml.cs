using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Globalization;
using System.Diagnostics;

using Microsoft.Kinect;

using Coding4Fun.Kinect.Wpf;
using System.Text.RegularExpressions;


namespace Microsoft.Kinect.Samples.KinectPaint
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public static bool multiple;

        public static Menu Instance { get; private set; }
        public Menu()
        {
            InitializeComponent();
            Instance = this;
        }
        
        private void Draw(object sender, RoutedEventArgs e)
        {
            multiple = false;    
            const string uri1 = @"/KinectPaint;component/Resources/Drawing-1.png";
            const string uri2 = @"/KinectPaint;component/Resources/Drawing-2.png";
            const string uri3 = @"/KinectPaint;component/Resources/Drawing-3.png";
            const string uri4 = @"/KinectPaint;component/Resources/Drawing-4.png";
            const string uri5 = @"/KinectPaint;component/Resources/Drawing-5.png";
            const string uri6 = @"/KinectPaint;component/Resources/Drawing-6.png";

            ImageSource img1 = new BitmapImage(new Uri(uri1, UriKind.RelativeOrAbsolute));
            ImageSource img2 = new BitmapImage(new Uri(uri2, UriKind.RelativeOrAbsolute));
            ImageSource img3 = new BitmapImage(new Uri(uri3, UriKind.RelativeOrAbsolute));
            ImageSource img4 = new BitmapImage(new Uri(uri4, UriKind.RelativeOrAbsolute));
            ImageSource img5 = new BitmapImage(new Uri(uri5, UriKind.RelativeOrAbsolute));
            ImageSource img6 = new BitmapImage(new Uri(uri6, UriKind.RelativeOrAbsolute));

            Tutorial.Instance.Frame1.Source = img1;
            Tutorial.Instance.Frame2.Source = img2;
            Tutorial.Instance.Frame3.Source = img3;
            Tutorial.Instance.Frame4.Source = img4;
            Tutorial.Instance.Frame5.Source = img5;
            Tutorial.Instance.Frame6.Source = img6;

            Tutorial.Instance.BFrame1.Tag = uri1;
            Tutorial.Instance.BFrame2.Tag = uri2;
            Tutorial.Instance.BFrame3.Tag = uri3;
            Tutorial.Instance.BFrame4.Tag = uri4;
            Tutorial.Instance.BFrame5.Tag = uri5;
            Tutorial.Instance.BFrame6.Tag = uri6;

            Container.Instance.Menu.Visibility = Visibility.Hidden;
            Container.Instance.MainWindow.Visibility = Visibility.Visible;
            Container.Instance.MainWindow.SetTutorialActive(true);
            Container.Instance.MainWindow.PART_LoadedBackground.Source = null;
            MainWindow.Instance.SetTutorialActive(true);
            Tutorial.Instance.Visibility = Visibility.Visible;
        }
        private void FreeDraw(object sender, RoutedEventArgs e)
        {
            multiple = false;
            Container.Instance.Menu.Visibility = Visibility.Hidden;
            Container.Instance.MainWindow.Visibility = Visibility.Visible;
            Container.Instance.MainWindow.SetTutorialActive(false);
            Container.Instance.MainWindow.PART_LoadedBackground.Source = null;
            MainWindow.Instance.SetTutorialActive(false);
            Tutorial.Instance.Visibility = Visibility.Hidden;

        }

        private void Coloring(object sender, RoutedEventArgs e)
        {
            multiple = false;
            const string uri1 = @"/KinectPaint;component/Resources/Coloring-1.png";
            const string uri2 = @"/KinectPaint;component/Resources/Coloring-2.png";
            const string uri3 = @"/KinectPaint;component/Resources/Coloring-3.png";
            const string uri4 = @"/KinectPaint;component/Resources/Coloring-4.png";
            const string uri5 = @"/KinectPaint;component/Resources/Coloring-5.png";
            const string uri6 = @"/KinectPaint;component/Resources/Coloring-6.png";

            ImageSource img1 = new BitmapImage(new Uri(uri1, UriKind.RelativeOrAbsolute));
            ImageSource img2 = new BitmapImage(new Uri(uri2, UriKind.RelativeOrAbsolute));
            ImageSource img3 = new BitmapImage(new Uri(uri3, UriKind.RelativeOrAbsolute));
            ImageSource img4 = new BitmapImage(new Uri(uri4, UriKind.RelativeOrAbsolute));
            ImageSource img5 = new BitmapImage(new Uri(uri5, UriKind.RelativeOrAbsolute));
            ImageSource img6 = new BitmapImage(new Uri(uri6, UriKind.RelativeOrAbsolute));

            Tutorial.Instance.Frame1.Source = img1;
            Tutorial.Instance.Frame2.Source = img2;
            Tutorial.Instance.Frame3.Source = img3;
            Tutorial.Instance.Frame4.Source = img4;
            Tutorial.Instance.Frame5.Source = img5;
            Tutorial.Instance.Frame6.Source = img6;

            Tutorial.Instance.BFrame1.Tag = uri1;
            Tutorial.Instance.BFrame2.Tag = uri2;
            Tutorial.Instance.BFrame3.Tag = uri3;
            Tutorial.Instance.BFrame4.Tag = uri4;
            Tutorial.Instance.BFrame5.Tag = uri5;
            Tutorial.Instance.BFrame6.Tag = uri6;

            Container.Instance.Menu.Visibility = Visibility.Hidden;
            Container.Instance.MainWindow.Visibility = Visibility.Visible;
            Container.Instance.MainWindow.SetTutorialActive(true);
            Container.Instance.MainWindow.PART_LoadedBackground.Source = null;
            MainWindow.Instance.SetTutorialActive(true);
            Tutorial.Instance.Visibility = Visibility.Visible;
        }
    }
}
