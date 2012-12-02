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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Muragatte.Core;

namespace Muragatte.Visual
{
    public class Visualization
    {
        #region Fields

        private MultiAgentSystem _model;
        private Canvas _canvas;
        private GUI.CanvasWindow _wndCanvas;
        private GUI.PlaybackWindow _wndPlayback;
        private GUI.OptionsWindow _wndOptions;
        private GUI.CanvasWindow _wndSnapshotPreview = null;
        private Window _owner = null;

        #endregion

        #region Constructors

        public Visualization(MultiAgentSystem model, double scale, Window owner)
            : this(model, model.Region.Width, model.Region.Height, scale, owner, null) { }

        public Visualization(MultiAgentSystem model, int width, int height, double scale, Window owner)
            : this(model, width, height, scale, owner, null) { }

        public Visualization(MultiAgentSystem model, double scale, Window owner, ObservableCollection<Styles.Style> styles)
            : this(model, model.Region.Width, model.Region.Height, scale, owner, styles) { }

        public Visualization(MultiAgentSystem model, int width, int height, double scale, Window owner, ObservableCollection<Styles.Style> styles)
        {
            DefaultValues.Scale = scale;
            _model = model;
            _owner = owner;
            _canvas = new Canvas(width, height, scale, this);
            _wndCanvas = new GUI.CanvasWindow(this, _canvas);
            _wndCanvas.Owner = owner;
            _wndPlayback = new GUI.PlaybackWindow(this);
            _wndPlayback.Owner = owner;
            _wndOptions = new GUI.OptionsWindow(this, styles);
            _wndOptions.Owner = owner;
        }

        #endregion

        #region Properties

        public MultiAgentSystem GetModel
        {
            get { return _model; }
        }

        public Canvas GetCanvas
        {
            get { return _canvas; }
        }

        public GUI.CanvasWindow GetWindow
        {
            get { return _wndCanvas; }
        }

        public GUI.PlaybackWindow GetPlayback
        {
            get { return _wndPlayback; }
        }

        public GUI.OptionsWindow GetOptions
        {
            get { return _wndOptions; }
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            _canvas.Clear();
            ShowAll();
        }

        public void ShowAll()
        {
            _wndCanvas.Show();
            _wndPlayback.Show();
            _wndOptions.Show();
        }

        public void HideAll()
        {
            _wndCanvas.Hide();
            _wndPlayback.Hide();
            _wndOptions.Hide();
        }

        public void Redraw()
        {
            _canvas.Redraw(_model.History, (int)_wndPlayback.sldFrame.Value);
        }

        public void Redraw(int frame)
        {
            _canvas.Redraw(_model.History, frame);
        }

        public void Close()
        {
            CloseWindow(_wndCanvas);
            CloseWindow(_wndPlayback);
            CloseWindow(_wndOptions);
            CloseWindow(_wndSnapshotPreview);
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        public void ShowSnapshotPreview(Snapshot snapshot)
        {
            ShowSnapshotPreview(snapshot, snapshot.Step);
        }

        public void ShowSnapshotPreview(Snapshot snapshot, int step)
        {
            CloseWindow(_wndSnapshotPreview);
            _wndSnapshotPreview = new GUI.CanvasWindow(this, null);
            _wndSnapshotPreview.Owner = _owner;
            _wndSnapshotPreview.Title = string.Format("Snapshot Preview: i{0} / s{1}", _model.Instance, step);
            snapshot.Redraw(_model.History, step);
            _wndSnapshotPreview.Initialize(snapshot);
            _wndSnapshotPreview.Show();
        }

        #endregion
    }
}
