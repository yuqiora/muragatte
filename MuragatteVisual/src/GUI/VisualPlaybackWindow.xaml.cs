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
using System.Windows.Threading;
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
        private DispatcherTimer _visTimer;
        private bool _bPlaying = false;

        #endregion

        #region Constructors

        public VisualPlaybackWindow(Visualization visualization)
        {
            InitializeComponent();
            _visual = visualization;
            _visTimer = new DispatcherTimer();
            _visTimer.Tick += new EventHandler(_visTimer_Tick);
            _visTimer.Interval = TimeSpan.FromMilliseconds(DEFAULT_DELAY);
            txtDelay.Text = DEFAULT_DELAY.ToString();
        }

        #endregion

        #region Events

        void _visTimer_Tick(object sender, EventArgs e)
        {
            NextFrame();
        }

        private void chbAutoDelay_Checked(object sender, RoutedEventArgs e)
        {
            txtDelay.IsEnabled = false;
            if (_bPlaying)
            {
                _visTimer.Stop();
                AutoDelayOn();
            }
        }

        private void chbAutoDelay_Unchecked(object sender, RoutedEventArgs e)
        {
            txtDelay.IsEnabled = true;
            if (_bPlaying)
            {
                AutoDelayOff();
                _visTimer.Start();
            }
        }

        private void sldFrame_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _visual.Redraw((int)sldFrame.Value);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            sldFrame.Value = sldFrame.Minimum;
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            sldFrame.Value = sldFrame.Maximum;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            NextFrame();
        }

        private void txtDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            double ms;
            if (!double.TryParse(txtDelay.Text, out ms))
            {
                ms = DEFAULT_DELAY;
            }
            _visTimer.Interval = TimeSpan.FromMilliseconds(ms);
        }

        private void txtFrame_TextChanged(object sender, TextChangedEventArgs e)
        {
            int frame;
            if (!int.TryParse(txtFrame.Text, out frame) || frame < sldFrame.Minimum || frame > sldFrame.Maximum)
            {
                frame = 0;
            }
            sldFrame.Value = frame;
        }

        #endregion

        #region Methods

        public void UpdateFrameCount(int count)
        {
            lblNumOfFrames.Content = count.ToString();
            sldFrame.Maximum = count;
        }

        private void Play()
        {
            if (chbAutoDelay.IsChecked.Value)
            {
                AutoDelayOn();
            }
            else
            {
                _visTimer.Start();
            }
            Playing(true);
        }

        private void Pause()
        {
            if (chbAutoDelay.IsChecked.Value)
            {
                AutoDelayOff();
            }
            else
            {
                _visTimer.Stop();
            }
            Playing(false);
        }

        private void NextFrame()
        {
            if (sldFrame.Value < sldFrame.Maximum)
            {
                sldFrame.Value++;
            }
            else
            {
                Pause();
            }
        }

        private void AutoDelayOn()
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void AutoDelayOff()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void Playing(bool value)
        {
            _bPlaying = value;
            txtFrame.IsReadOnly = value;
        }

        #endregion
    }
}
