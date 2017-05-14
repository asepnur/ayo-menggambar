// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved. 

using System.Windows;
using System.Windows.Controls;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Microsoft.Kinect.Samples.KinectPaint
{
    /// <summary>
    /// Interaction logic for Tutorial.xaml
    /// </summary>
    public partial class Tutorial : UserControl
    {
        public static Tutorial Instance { get; private set; }

        public Tutorial()
        {
            InitializeComponent();
            Instance = this;
        }

        protected void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        protected void Choose(object sender, RoutedEventArgs args)
        {
            Menu.multiple = true;
            string uri = ((Button)sender).Tag.ToString();
            // Hide window and add show selected picture
            this.Visibility = Visibility.Collapsed;
            ImageSource img = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
            MainWindow.Instance.SetTutorialActive(false);
            MainWindow.Instance.PART_LoadedBackground.Source = img;
        }
    }
}
