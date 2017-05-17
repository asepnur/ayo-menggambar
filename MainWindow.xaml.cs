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

namespace Microsoft.Kinect.Samples.KinectPaint
{
    /// <summary>
    /// Enumerates types of actions that require a confirmation popup
    /// </summary>
    public enum ActionAwaitingConfirmation
    {
        Close,
        New,
        Load,
        Back
    }

    /// <summary>
    /// The main application window
    /// </summary>
    public partial class MainWindow : UserControl
    {
        /// <summary>
        /// Gets the instance of the main window, assuming it has been created
        /// </summary>
        public bool MusicStatus = true;
        public static MainWindow Instance { get; private set; }

        #region Data

        bool _isTutorialActive;
        Point _pastCursorPosition;
        bool _imageUnsaved = false;
        FocusingStackPanel _colorpicker;
        bool _isPickingColor;

        #endregion

        public MainWindow()
        {    
            _isTutorialActive = true;
            InitializeComponent();
            SelectedBrush = _availableBrushes[0];
            SelectedColor = _availableColors[0];
            SelectedSize = _availableSizes[0];
            // Make sure only one MainWindow ever gets created
            Debug.Assert(Instance == null);

            Instance = this;
        }

        #region Methods

        /// <summary>
        /// Called by LoadPopup after the user has chosen a file to load
        /// </summary>
        public void LoadingFinished()
        {
            string uri = ((LoadPopup)CurrentPopup).SelectedImage.Image.ToString();
            ImageSource img = null;

            // Set an image
            if (Regex.Match(uri, @"dg\d+_").Success)
            {
                string path = Constant.ImagePathCSharp + "Drawing-" + Regex.Match(uri, @"\d+").ToString() + ".png";
                img = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            }
            else if (Regex.Match(uri, @"cl\d+_").Success)
            {
                string path = Constant.ImagePathCSharp + "Coloring-" + Regex.Match(uri, @"\d+").ToString() + ".png";
                img = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            }
            
            // Hide tutorial layer
            Tutorial.Visibility = Visibility.Collapsed;
            // Change background
            Instance.PART_LoadedBackground.Source = img;
            LoadedImage = new WriteableBitmap(new BitmapImage(((LoadPopup)CurrentPopup).SelectedImage.Image));
            CurrentPopup = null;
            _imageUnsaved = false;
            _isTutorialActive = false;
        }

        /// <summary>
        /// Called by ConfirmationPopup after the user has chosen OK or Cancel
        /// </summary>
        public void ConfirmationFinished()
        {
            ConfirmationPopup popup = (ConfirmationPopup)CurrentPopup;

            CurrentPopup = null;
            ActionAwaitingConfirmation action = (ActionAwaitingConfirmation)popup.UserData;

            switch (action)
            {
                case ActionAwaitingConfirmation.New:
                    if (popup.DidConfirm)
                    {
                        _imageUnsaved = false;
                        CreatePaintableImage();
                    }
                    break;
                case ActionAwaitingConfirmation.Load:
                    if(popup.DidConfirm)
                        CurrentPopup = new LoadPopup(this);
                    break;
                case ActionAwaitingConfirmation.Close:
                    if (popup.DidConfirm)
                        Container.Instance.Close();
                    break;
                case ActionAwaitingConfirmation.Back:
                    if (popup.DidConfirm) {
                        if (Menu.multiple)
                        {
                            MainWindow.Instance.SetTutorialActive(true);
                            Tutorial.Instance.Visibility = Visibility.Visible;
                            Menu.multiple = false;
                            CreatePaintableImage();
                        }
                        else {
                            Container.Instance.Menu.Visibility = Visibility.Visible;
                            Container.Instance.MainWindow.Visibility = Visibility.Hidden;
                            CreatePaintableImage();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Call to hide the UI and begin painting on the canvas
        /// </summary>
        public void StartPainting()
        {
            if (_isTutorialActive)
            {
                return;
            }

            // Make the cursor passive so buttons and stuff don't respond to it
            Container.Instance.PART_Cursor.Passive = true;

            // Draw at the current position and start checking for updates until done
            Point pos = Container.Instance.PART_Cursor.GetPosition(PART_LoadedImageDisplay);
            Draw(pos, pos, null);
            _pastCursorPosition = pos;
            if (Container.Instance.sensor == null)
                CompositionTarget.Rendering += ContinueDrawingStroke;
            else
                Container.Instance.sensor.SkeletonFrameReady += ContinueDrawingStroke;
        }

        /// <summary>
        /// Call to show the UI and stop painting on the canvas
        /// </summary>
        public void StopPainting()
        {
            // Make the cursor active again
            Container.Instance.PART_Cursor.Passive = false;

            _imageUnsaved = true;

            // Stop listening for cursor changes
            if (Container.Instance.sensor == null)
                CompositionTarget.Rendering -= ContinueDrawingStroke;
            else
                Container.Instance.sensor.SkeletonFrameReady -= ContinueDrawingStroke;
        }

        #endregion

        #region Listbox Contents

        public IEnumerable<Color> AvailableColors { get { return _availableColors; } }
        private Color[] _availableColors = new Color[]
        {
            Colors.Red,
            Colors.DarkGreen,
            Colors.Blue,
            Colors.Purple,
            Colors.Orange,
            Colors.Yellow,
            Colors.Black,
            Colors.White,
            Colors.Pink,
            Colors.Brown,
            Colors.Tomato,
            Colors.MistyRose
        };

        public IEnumerable<double> AvailableSizes { get { return _availableSizes; } }
        private double[] _availableSizes = new double[]
        {
            3.0,
            6.0,
            9.0,
            12.0
        };

        public IEnumerable<BrushSelection> AvailableBrushes { get { return _availableBrushes; } }
        private BrushSelection[] _availableBrushes = new BrushSelection[]
        {
            new BrushSelection(
                new Uri("/KinectPaint;component/Resources/pencil.png", UriKind.RelativeOrAbsolute), 
                new Uri("/KinectPaint;component/Resources/pencil_down.png", UriKind.RelativeOrAbsolute), 
                KinectPaintbrush.Marker, 
                "Pensil"),
            new BrushSelection(
                new Uri("/KinectPaint;component/Resources/airbrush.png", UriKind.RelativeOrAbsolute), 
                new Uri("/KinectPaint;component/Resources/airbrush_down.png", UriKind.RelativeOrAbsolute), 
                KinectPaintbrush.Airbrush, 
                "Semprot"),
            new BrushSelection(
                new Uri("/KinectPaint;component/Resources/brush.png", UriKind.RelativeOrAbsolute), 
                new Uri("/KinectPaint;component/Resources/brush_down.png", UriKind.RelativeOrAbsolute), 
                KinectPaintbrush.Brush, 
                "Kuas"),
            new BrushSelection(
                new Uri("/KinectPaint;component/Resources/eraser.png", UriKind.RelativeOrAbsolute), 
                new Uri("/KinectPaint;component/Resources/eraser_down.png", UriKind.RelativeOrAbsolute), 
                KinectPaintbrush.Eraser, 
                "Penghapus")
        };

        #endregion

        #region Window Properties

        /// <summary>
        /// Gets the Kinect runtime object
        /// </summary>

        #region SelectedColor

        /// <summary>
        /// The <see cref="SelectedColor" /> dependency property's name.
        /// </summary>
        public const string SelectedColorPropertyName = "SelectedColor";

        /// <summary>
        /// Gets or sets the value of the currently selected color.
        /// This is a dependency property.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedColor" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            "SelectedColor",
            typeof(Color),
            typeof(MainWindow),
            new UIPropertyMetadata(null));

        #endregion

        #region SelectedSize

        /// <summary>
        /// The <see cref="SelectedSize" /> dependency property's name.
        /// </summary>
        public const string SelectedSizePropertyName = "SelectedSize";

        /// <summary>
        /// Gets or sets the value of the currently selected size.
        /// This is a dependency property.
        /// </summary>
        public double SelectedSize
        {
            get
            {
                return (double)GetValue(SelectedSizeProperty);
            }
            set
            {
                SetValue(SelectedSizeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedSize" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSizeProperty = DependencyProperty.Register(
            SelectedSizePropertyName,
            typeof(double),
            typeof(MainWindow),
            new UIPropertyMetadata(0.0));

        #endregion

        #region SelectedBrush

        /// <summary>
        /// The <see cref="SelectedBrush" /> dependency property's name.
        /// </summary>
        public const string SelectedBrushPropertyName = "SelectedBrush";

        /// <summary>
        /// Gets or sets the value of the currently selected brush.
        /// This is a dependency property.
        /// </summary>
        public BrushSelection SelectedBrush
        {
            get
            {
                return (BrushSelection)GetValue(SelectedBrushProperty);
            }
            set
            {
                SetValue(SelectedBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedBrush" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            SelectedBrushPropertyName,
            typeof(BrushSelection),
            typeof(MainWindow),
            new UIPropertyMetadata(null));

        #endregion

        #region ShowCamera

        /// <summary>
        /// The <see cref="ShowCamera" /> dependency property's name.
        /// </summary>
        public const string ShowCameraPropertyName = "ShowCamera";

        /// <summary>
        /// Gets or sets the value of the <see cref="ShowCamera" />
        /// property. This is a dependency property.
        /// </summary>

        
        #endregion

        #region LoadedImage

        /// <summary>
        /// Path to the currently loaded image
        /// </summary>
        public WriteableBitmap LoadedImage
        {
            get { return _loadedImage; }
            set
            {
                _loadedImage = value;

                PART_LoadedImageDisplay.Source = _loadedImage;
            }
        }
        private WriteableBitmap _loadedImage;

        #endregion

        #region CurrentPopup

        /// <summary>
        /// The current popup (load dialog, or confirmation dialog, or tutorial)
        /// </summary>
        public object CurrentPopup
        {
            get { return _currentPopup; }
            set
            {
                _currentPopup = value;

                PART_PopupDisplay.Content = _currentPopup;
            }
        }
        private object _currentPopup;

        #endregion

        #endregion

        #region Button Handlers

        // called when the user presses the 'Let's draw' button
        private void OnCloseTutorial(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = Visibility.Collapsed;
            Tutorial.Visibility = Visibility.Collapsed;
            _isTutorialActive = false;
        }

        // Called when the user presses the 'New' button
        private void OnNew(object sender, RoutedEventArgs args)
        {
            if (_imageUnsaved)
                CurrentPopup = new ConfirmationPopup("Gambar yang belum disimpan akan hilang, lanjutkan?", ActionAwaitingConfirmation.New, this);
            else
            {
                _imageUnsaved = false;
                CreatePaintableImage();
            }
        }

        // Called when the user presses the 'Save' button
        private void OnSave(object sender, RoutedEventArgs args)
        {
            string pathBackground = "";
            string uri = Instance.PART_LoadedBackground.Source != null ? Instance.PART_LoadedBackground.Source.ToString().ToLower() : "";
            if (uri.Contains("drawing")){
                pathBackground = "dg" + Regex.Match(uri, @"\d+").Value + "_";
            } else if (uri.Contains("coloring"))
            {
                pathBackground = "cl" + Regex.Match(uri, @"\d+").Value + "_";
            }
            LoadedImage.Save(Path.Combine(App.PhotoFolder, pathBackground + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".png"), ImageFormat.Png);
            _imageUnsaved = false;

            // Animate the "Saved" message so the user knows it worked
            DoubleAnimation saveMessageAnimator = new DoubleAnimation(1.0, 0.0, Duration.Automatic, FillBehavior.Stop);
            PART_SaveMessage.BeginAnimation(OpacityProperty, saveMessageAnimator);
        }
        private void OnBack(Object sender, RoutedEventArgs args) {
            if (_imageUnsaved) { 
                CurrentPopup = new ConfirmationPopup("Gambar yang belum disimpan akan hilang, Kembali ke Menu?", ActionAwaitingConfirmation.Back, this);
            }
            else
            {
                _imageUnsaved = false;
            }
        }

        // Called when the user presses the 'Load' button
        private void OnLoad(object sender, RoutedEventArgs args)
        {
            if (_imageUnsaved)
                CurrentPopup = new ConfirmationPopup("Gambar yang belum disimpan akan hilang, lanjutkan?", ActionAwaitingConfirmation.Load, this);
            else
                CurrentPopup = new LoadPopup(this);
        }

        // Called when the user presses the 'Quit' button
        public void OnQuit(object sender, RoutedEventArgs args)
        {
            if (_imageUnsaved)
                CurrentPopup = new ConfirmationPopup("Keluar tanpa menyimpan?", ActionAwaitingConfirmation.Close, this);
            else
                Container.Instance.Close();
        }

        #endregion

        #region Internal

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Set up the color picker's initial state
            _colorpicker = (FocusingStackPanel)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(PART_ColorPickerListBox, 0), 0), 0);
            _colorpicker.FocusedQuantity = 50;
        }

        void ContinueDrawingStroke(object sender, EventArgs e)
        {
            Point pos = Container.Instance.PART_Cursor.GetPosition(PART_LoadedImageDisplay);
            Point prev = Container.Instance.PART_Cursor.GetPreviousPosition(PART_LoadedImageDisplay);
            Draw(prev, pos, _pastCursorPosition);
            _pastCursorPosition = prev;
        }

        private void CreatePaintableImage()
        {
            LoadedImage = new WriteableBitmap(
                (int)PART_PaintCanvas.ActualWidth,
                (int)PART_PaintCanvas.ActualHeight,
                96.0,
                96.0,
                PixelFormats.Pbgra32,
                null);
        }

        // paints on the canvas using the currently selected settings.
        private void Draw(Point from, Point to, Point? past)
        {
            switch (SelectedBrush.Brush)
            {
                case KinectPaintbrush.Eraser:
                    BitmapHelpers.Erase(
                        LoadedImage,
                        from, to,
                        (int)SelectedSize);
                    break;
                case KinectPaintbrush.Marker:
                    BitmapHelpers.Brush(
                        LoadedImage,
                        from, to, past,
                        Color.FromArgb(128, SelectedColor.R, SelectedColor.G, SelectedColor.B),
                        (int)SelectedSize);
                    break;
                case KinectPaintbrush.Airbrush:
                    BitmapHelpers.Airbrush(
                        LoadedImage,
                        from, to,
                        SelectedColor,
                        (int)SelectedSize * 2);
                    break;
                case KinectPaintbrush.Brush:
                    BitmapHelpers.Brush(
                        LoadedImage,
                        from, to, past,
                        SelectedColor,
                        (int)SelectedSize);
                    break;
            }
        }

        // Called when the cursor enters the area of the color picker
        private void KinectPaintListBox_CursorEnter(object sender, CursorEventArgs e)
        {
            _isPickingColor = true;
            CompositionTarget.Rendering += AnimateColorPicker;
        }

        // Called when the cursor leaves the area of the color picker
        private void KinectPaintListBox_CursorLeave(object sender, CursorEventArgs e)
        {
            _isPickingColor = false;
        }
        
        // Animates the color picker
        void AnimateColorPicker(object sender, EventArgs e)
        {
            const double maxsize = 25;
            const double animationTime = 0.4;
            const double fps = 60;

            if (_isPickingColor)
            {
                _colorpicker.FocusedIndex = PART_ColorPickerListBox.SelectedIndex;
                _colorpicker.FocusedQuantity = Math.Max(1, _colorpicker.FocusedQuantity - maxsize / (animationTime * fps));
            }
            else
            {
                _colorpicker.FocusedIndex = PART_ColorPickerListBox.SelectedIndex;
                _colorpicker.FocusedQuantity = Math.Min(maxsize, _colorpicker.FocusedQuantity + maxsize / (animationTime * fps));

                if (_colorpicker.FocusedQuantity == maxsize)
                    CompositionTarget.Rendering -= AnimateColorPicker;
            }
        }

        // Recreate the paintable image when the size changes, since their sizes need to match
        private void PART_PaintCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CreatePaintableImage();
        }

        public void SetTutorialActive(bool status)
        {
            _isTutorialActive = status;
        }

        #endregion
    }
}
