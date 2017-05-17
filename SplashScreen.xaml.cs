// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved. 

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

using Microsoft.Kinect;

using Coding4Fun.Kinect.Wpf;

namespace Microsoft.Kinect.Samples.KinectPaint
{

    /// <summary>
    /// The main application window
    /// </summary>
    public partial class SplashScreen : UserControl
    {
        /// <summary>
        /// Gets the instance of the main window, assuming it has been created
        /// </summary>
        public static SplashScreen Instance { get; private set; }

        public SplashScreen()
        {
            InitializeComponent();
            // Make sure only one MainWindow ever gets created
            Debug.Assert(Instance == null);
            Instance = this;
        }
    }
}
