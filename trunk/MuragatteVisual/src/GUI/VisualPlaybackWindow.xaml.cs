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
using System.ComponentModel;
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
using Xceed.Wpf.Toolkit;

namespace Muragatte.Visual.GUI
{
    /// <summary>
    /// Interaction logic for VisualPlaybackWindow.xaml
    /// </summary>
    public partial class VisualPlaybackWindow : Window, INotifyPropertyChanged
    {
        #region Constants

        private const int DEFAULT_DELAY = 10;

        #endregion

        #region Fields

        private Visualization _visual;
        private DispatcherTimer _visTimer;
        private bool _bPlaying = false;
        private int _iFrameIncrement = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public VisualPlaybackWindow(Visualization visualization)
        {
            InitializeComponent();
            DataContext = this;

            _visual = visualization;

            _visTimer = new DispatcherTimer();
            _visTimer.Tick += new EventHandler(_visTimer_Tick);
            _visTimer.Interval = TimeSpan.FromMilliseconds(DEFAULT_DELAY);

            iudDelay.Value = DEFAULT_DELAY;
            int substeps = _visual.GetModel.Substeps;
            if (substeps == 1 || _visual.GetModel.History.Mode == Core.Storage.HistoryMode.NoSubsteps)
            {
                chbSkipSubsteps.IsEnabled = false;
                FrameIncrement = substeps;
            }
            else
            {
                chbSkipSubsteps.Content += string.Format(" ({0})", substeps - 1);
            }
        }

        #endregion

        #region Properties

        public bool IsPlaying
        {
            get { return _bPlaying; }
            private set
            {
                _bPlaying = value;
                NotifyPropertyChanged("IsPlaying");
            }
        }

        public int FrameIncrement
        {
            get { return _iFrameIncrement; }
            set
            {
                _iFrameIncrement = value;
                NotifyPropertyChanged("FrameIncrement");
            }
        }

        public Core.MultiAgentSystem GetModel
        {
            get { return _visual.GetModel; }
        }

        #endregion

        #region Events

        void _visTimer_Tick(object sender, EventArgs e)
        {
            NextFrame();
        }

        private void chbAutoDelay_Checked(object sender, RoutedEventArgs e)
        {
            iudDelay.IsEnabled = false;
            if (_bPlaying)
            {
                _visTimer.Stop();
                AutoDelayOn();
            }
        }

        private void chbAutoDelay_Unchecked(object sender, RoutedEventArgs e)
        {
            iudDelay.IsEnabled = true;
            if (_bPlaying)
            {
                AutoDelayOff();
                _visTimer.Start();
            }
        }

        private void sldFrame_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Redraw();
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

        private void btnRedraw_Click(object sender, RoutedEventArgs e)
        {
            Redraw();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            NextFrame();
        }

        private void iudDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _visTimer.Interval = TimeSpan.FromMilliseconds(iudDelay.Value.GetValueOrDefault(DEFAULT_DELAY));
        }

        private void chbSkipSubsteps_Checked(object sender, RoutedEventArgs e)
        {
            FrameIncrement = _visual.GetModel.Substeps;
        }

        private void chbSkipSubsteps_Unchecked(object sender, RoutedEventArgs e)
        {
            FrameIncrement = 1;
        }

        #endregion

        #region Methods

        public void Clear()
        {
            sldFrame.Value = sldFrame.Minimum;
            Pause();
        }

        private void Redraw()
        {
            _visual.Redraw((int)sldFrame.Value);
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
            IsPlaying = true;
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
            IsPlaying = false;
        }

        private void NextFrame()
        {
            if (sldFrame.Value < sldFrame.Maximum)
            {
                sldFrame.Value += _iFrameIncrement;
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

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
