﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved. 

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect.Samples.KinectPaint;


namespace Microsoft.Kinect.Samples.KinectPaint
{
    /// <summary>
    /// The popup that displays a confirmation dialog
    /// </summary>
    public partial class HelpPopup : UserControl
    {
        MainWindow _window;
        Menu _menu;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The message being confirmed</param>
        /// <param name="data">Caller-defined data</param>
        /// <param name="window">Refernece to the main application window</param>
        public HelpPopup(string message, object data, Menu menu, bool exit, string picture)
        {
            Picture = picture;
            Message = message;
            UserData = data;
            _menu = menu;
            Exit = exit;
            InitializeComponent();
        }
        #region Properties

        /// <summary>
        /// True if the user pressed OK, false if cancel or if the popup hasn't been addressed by the user yet.
        /// </summary>
        public bool DidConfirm { get; private set; }
        public string Picture { get; private set; }

        /// <summary>
        /// Gets the message to be displayed
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the custom data that was passed with the constructor
        /// </summary>
        public object UserData { get; private set; }
        public object Exit { get; private set; }

        #endregion

        #region Button Handlers

        // Called when the user presses OK
        public void OnOk(object sender, RoutedEventArgs args)
        {
            DidConfirm = true;
            try
            {
                if ((bool)Exit)
                    _menu.ConfirmationFinished();
                _menu.ConfirmationFinished();
            }
            catch (Exception)
            {
                _window.ConfirmationFinished();
            }
        }

        // Called when the user presses Cancel
        protected void OnCancel(object sender, RoutedEventArgs args)
        {
            DidConfirm = false;
            try
            {
                if ((bool)Exit)
                    _menu.ConfirmationFinished();
            }
            catch (Exception)
            {
                _window.ConfirmationFinished();
            }
        }

        #endregion
    }
}
