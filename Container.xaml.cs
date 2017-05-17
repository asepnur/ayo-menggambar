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
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Threading;

namespace Microsoft.Kinect.Samples.KinectPaint
{
    /// <summary>
    /// The main application window
    /// </summary>
    public partial class Container : Window
    {
        /// <summary>
        /// Gets the instance of the main window, assuming it has been created
        /// </summary>
        public static Container Instance { get; private set; }

        public Container()
        {    
            InitializeComponent();
            Instance = this;
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 3000;
            aTimer.Enabled = true;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Instance.Dispatcher.Invoke(new Action(() => {
                Instance.Menu.Visibility = Visibility.Visible;
                Instance.SplashScreen.Visibility = Visibility.Hidden;
            }), DispatcherPriority.ContextIdle);
        }

        public KinectSensor sensor { get; private set; }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {

            try
            {

                if (KinectSensor.KinectSensors.Count > 0)
                {
                    //grab first
                    sensor = KinectSensor.KinectSensors[0];
                }

                if (sensor.Status != KinectStatus.Connected || KinectSensor.KinectSensors.Count == 0)
                {
                    MessageBox.Show("No Kinect connected!");
                }

                // Set up the Kinect

                var parameters = new TransformSmoothParameters
                {
                    Smoothing = 0.3f,
                    Correction = 0.0f,
                    Prediction = 0.0f,
                    JitterRadius = 1.0f,
                    MaxDeviationRadius = 0.5f
                };

                sensor.SkeletonStream.Enable(parameters);
                sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                sensor.Start();


            }
            catch (Exception err)
            {
                // Failed to set up the Kinect. Show the error onscreen (app will switch to using mouse movement)
                sensor = null;
                MainWindow.Instance.PART_ErrorText.Visibility = Visibility.Visible;
                Console.WriteLine("Panics : " + err.ToString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (sensor != null)
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                }
            Environment.Exit(0);
        }
    }
}
