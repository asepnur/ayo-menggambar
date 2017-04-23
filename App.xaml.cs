// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved. 

using System;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Kinect.Samples.KinectPaint
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int TIME_SPLASH = 1500;
        protected override void OnStartup(StartupEventArgs e)
        {
            SplashScreen splash = new SplashScreen();
            splash.Show();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            base.OnStartup(e);

            Menu menu = new Menu();
            timer.Stop();
            int remainingTimeToShowSplash = TIME_SPLASH - (int)timer.ElapsedMilliseconds;
            if (remainingTimeToShowSplash > 0)
                Thread.Sleep(remainingTimeToShowSplash);
            splash.Close();
        }
        /// <summary>
        /// Gets the path to My Pictures\KinectPaint
        /// </summary>
        public static string PhotoFolder { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            PhotoFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "KinectPaint");
            // The app depends on the My Pictures\KinectPaint folder existing, so create it if it isn't there.
            if (!Directory.Exists(PhotoFolder))
                Directory.CreateDirectory(PhotoFolder);

            DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        // Catches unhandled exceptions. Insert a breakpoint here for much easier debugging.
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.Assert(false, e.Exception.ToString());
        }
    }
}
