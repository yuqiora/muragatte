// ------------------------------------------------------------------------
// Muragatte - A Toolkit for Observation of Swarm Behaviour
//             Visualization Library
//
// Copyright (C) 2012  Jiří Vejmola.
// Developed under the MIT License. See the file license.txt for details.
//
// Muragatte on the internet: http://code.google.com/p/muragatte/
// ------------------------------------------------------------------------

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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Muragatte.Visual;

namespace Muragatte.GUI
{
    /// <summary>
    /// Interaction logic for VisualPlaybackWindow.xaml
    /// </summary>
    public partial class VisualPlaybackWindow : Window
    {
        #region Constants

        private const int DEFAULT_DELAY = 10;

        #endregion

        #region Fields

        private Visualization _visual;
        private Storyboard _playbackStoryboard;
        private DoubleAnimation _playbackAnimation;
        private bool _bPaused = false;

        #endregion

        #region Constructors

        public VisualPlaybackWindow(Visualization visualization)
        {
            InitializeComponent();
            NameScope.SetNameScope(this, new NameScope());
            //this.DataContext = this;
            this.RegisterName(sldFrame.Name, sldFrame);
            _visual = visualization;
            _playbackAnimation = new DoubleAnimation();
            _playbackStoryboard = new Storyboard();
            _playbackStoryboard.Children.Add(_playbackAnimation);
            Storyboard.SetTargetName(_playbackAnimation, sldFrame.Name);
            Storyboard.SetTargetProperty(_playbackAnimation, new PropertyPath(Slider.ValueProperty));
            txtDelay.Text = DEFAULT_DELAY.ToString();
        }

        #endregion

        #region Events

        private void chbAutoDelay_Checked(object sender, RoutedEventArgs e)
        {
            txtDelay.IsEnabled = false;
        }

        private void chbAutoDelay_Unchecked(object sender, RoutedEventArgs e)
        {
            txtDelay.IsEnabled = true;
        }

        private void sldFrame_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _visual.GetCanvas.Redraw(_visual.GetModel.History, (int)sldFrame.Value);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (chbAutoDelay.IsChecked.Value)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                if (_bPaused)
                {
                    _playbackStoryboard.Resume(this);
                    _bPaused = false;
                }
                else
                {
                    _playbackAnimation.To = (int)sldFrame.Maximum;
                    int ms;
                    if (!int.TryParse(txtDelay.Text, out ms))
                    {
                        ms = DEFAULT_DELAY;
                    }
                    _playbackAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(ms * sldFrame.Maximum));
                    _playbackStoryboard.Begin(this, true);
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (chbAutoDelay.IsChecked.Value)
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
            else
            {
                _bPaused = true;
                _playbackStoryboard.Pause(this);
            }
        }

        private void btnToStart_Click(object sender, RoutedEventArgs e)
        {
            if (chbAutoDelay.IsChecked.Value)
            {
                sldFrame.Value = sldFrame.Minimum;
            }
            else
            {
                _playbackStoryboard.Stop(this);
            }
        }

        private void btnToEnd_Click(object sender, RoutedEventArgs e)
        {
            if (chbAutoDelay.IsChecked.Value)
            {
                sldFrame.Value = sldFrame.Maximum;
            }
            else
            {
                _playbackStoryboard.SkipToFill(this);
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (sldFrame.Value < sldFrame.Maximum)
            {
                sldFrame.Value++;
            }
            else
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        #endregion

        #region Methods

        public void UpdateFrameCount(int count)
        {
            lblNumOfFrames.Content = count.ToString();
            sldFrame.Maximum = count;
        }

        #endregion
    }
}
