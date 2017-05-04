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
        public static Menu Instance { get; private set; }
        public Menu()
        {
            InitializeComponent();
            Instance = this;
        }

        //private void Menggambar(object sender, RoutedEventArgs e)
        //{
        //    MainWindow mw = new MainWindow();
        //    mw.Show();
        //    this.Close();
        //}
    }
}
