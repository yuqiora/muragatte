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
using System.Windows.Controls;
using System.Windows.Data;
using Muragatte.Core.Storage;

namespace Muragatte.Visual
{
    public class HistoryViewer : INotifyPropertyChanged
    {
        #region Fields

        private History _history;
        private int _iTime = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public HistoryViewer(History history, GUI.PlaybackWindow playback)
        {
            _history = history;
            Binding bindFrame = new Binding("Time");
            bindFrame.Source = this;
            bindFrame.Mode = BindingMode.OneWayToSource;
            playback.sldFrame.SetBinding(Slider.ValueProperty, bindFrame);
        }

        #endregion

        #region Properties

        public int Time
        {
            get { return _iTime; }
            set
            {
                _iTime = value;
                if (_iTime > _history.Length) _iTime = _history.Count - 1;
                if (_iTime < 0) _iTime = 0;
                NotifyPropertyChanged("Time");
                NotifyPropertyChanged("Current");
            }
        }

        public HistoryRecord Current
        {
            get { return _history.Count == 0 ? null : _history[_iTime]; }
        }

        public bool IsEmpty
        {
            get { return _history.Count == 0; }
        }

        #endregion

        #region Methods

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
